using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering; // for GraphicsFormat

public class BlitRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class BlitSettings
    {
        public string CommandBufferName = "BlitRendererFeature";
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        public Material blitMaterial;
        public string blitShaderPassName = "Blit";
        public string mainTexPropertyName = "_MainTex";
        public string tempRTName = "_BlitTemp";
        public bool req_lockFrame = false;
        [Header("")]
        public bool opt_HalfResolution = true;
        public bool opt_GraphicFormat = true;
    }

    public BlitSettings settings = new BlitSettings();
    private BlitPass blitPass;
    public bool EnableBlit
    {
        get { return blitPass.EnableBlit; }
        set { if (blitPass != null) { blitPass.EnableBlit = value; } }
    }
    public override void Create()
    {
        if (settings.blitMaterial == null)
        {
            Debug.LogWarning($"[{nameof(BlitRendererFeature)}] No material assigned!");
            return;
        }

        blitPass = new BlitPass(settings);
        blitPass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (blitPass == null) return;
        if (renderingData.cameraData.isSceneViewCamera || renderingData.cameraData.isPreviewCamera) return;
        
        renderer.EnqueuePass(blitPass);
    }

    public class BlitPass : ScriptableRenderPass
    {
        private readonly Material material;
        private readonly int passIndex;
        private readonly BlitSettings settings;
        [System.NonSerialized] public bool EnableBlit = false;

        // persistent temp RTHandle
        RTHandle    tempTarget;
        RTHandle    lockedFrame;
        bool frameLockActive = false;
        RenderTextureDescriptor desc;
        static Mesh fullScreenQuad;

        public BlitPass(BlitSettings settings)
        {
            this.settings = settings;
            material = settings.blitMaterial;
            int found = material != null ? material.FindPass(settings.blitShaderPassName) : -1;
            if (found < 0) found = 0;
            passIndex = found;

            if (fullScreenQuad == null)
                fullScreenQuad = GenerateFullscreenQuad();
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            desc.msaaSamples = 1;

            // Divide resolution by 2 to perform post fx operation faster
            if (settings.opt_HalfResolution)
            {
                desc.width /= 2;
                desc.height /= 2;
            }
            if (settings.opt_GraphicFormat)
            {
                desc.graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(RenderTextureFormat.ARGB32, false);    
            }
            
            if (settings.req_lockFrame)
            {
                RenderingUtils.ReAllocateIfNeeded(ref lockedFrame, desc, name:"_LockedTemp");
            }
            RenderingUtils.ReAllocateIfNeeded(ref tempTarget, desc, name: "_BlitTemp");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!EnableBlit)
            {
                frameLockActive = false;
                return;
            }
                
            if (material == null)
                return;

            //Debug.Log($"Executing blit pass for material: {material.name}");

            var camData = renderingData.cameraData;
            RTHandle cameraColor = camData.renderer.cameraColorTargetHandle;
            if (cameraColor == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get(settings.CommandBufferName);

            // To match mobile resolution that is not square at all
            // If we want not adaptation to screen size we can probably make this optional
            cmd.SetGlobalVector("_MainTex_TexelSize",
                new Vector4(1f / Screen.width, 1f / Screen.height, Screen.width, Screen.height));

            GraphicsFormat colorFormat = cameraColor.rt.graphicsFormat;
            bool projectIsGamma = (QualitySettings.activeColorSpace == ColorSpace.Gamma);
            if (projectIsGamma && GraphicsFormatUtility.IsSRGBFormat(colorFormat))
                colorFormat = GraphicsFormatUtility.GetGraphicsFormat(RenderTextureFormat.Default, false);

            /// Minimal work if frame lock requested and processed
            if (frameLockActive && settings.req_lockFrame)
            {
                CoreUtils.SetRenderTarget(cmd, cameraColor);

               // material.SetTexture("_MainTex", lockedFrame);

                cmd.DrawMesh(fullScreenQuad, Matrix4x4.identity, material, 0, passIndex);

                // Release RT only in Dispose to avoid leaks
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);

                return;
            }
            /// Freeze Frame and Lock if requested
            if (!frameLockActive && settings.req_lockFrame)
            {
                RenderingUtils.ReAllocateIfNeeded(ref lockedFrame, desc, name:"_LockedTemp");
                Blitter.BlitCameraTexture(cmd, cameraColor, lockedFrame);

                // Ensures that the texture is in readable format for the shader
                if (!string.IsNullOrEmpty(settings.mainTexPropertyName))
                {
                    Texture tex = tempTarget.rt != null ? (Texture)tempTarget.rt : (Texture)tempTarget;
                    cmd.SetGlobalTexture(settings.mainTexPropertyName, tex);
                }

                CoreUtils.SetRenderTarget(cmd, cameraColor);

                material.SetTexture("_MainTex", lockedFrame);

                cmd.DrawMesh(fullScreenQuad, Matrix4x4.identity, material, 0, passIndex);

                // Release RT only in Dispose to avoid leaks
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);

                frameLockActive = true;
                return;
            }

            /// Regular Blit
            frameLockActive = false;
            
            Blitter.BlitCameraTexture(cmd, cameraColor, tempTarget);
            // Ensures that the texture is in readable format for the shader
            if (!string.IsNullOrEmpty(settings.mainTexPropertyName))
            {
                Texture tex = tempTarget.rt != null ? (Texture)tempTarget.rt : (Texture)tempTarget;
                cmd.SetGlobalTexture(settings.mainTexPropertyName, tex);
            }

            CoreUtils.SetRenderTarget(cmd, cameraColor);

            material.SetTexture("_MainTex", tempTarget);

            cmd.DrawMesh(fullScreenQuad, Matrix4x4.identity, material, 0, passIndex);

            // Release RT only in Dispose to avoid leaks
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

         public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // No clean up to do as we keep the temp RT allocation alive between frames
        }

        public void Dispose()
        {
            if (tempTarget != null)
            {
                RTHandles.Release(tempTarget);
                tempTarget = null;
            }
        }

        // Generates a real fullscreen quad
        // This is because of a weird unity fuckery where the UV is defined
        // in a different space or some shit
        // This result on wrong coordinates and displays only half screen
        // in a triangle.
        // Using a quad mesh ensures that our UV are correct.
        private static Mesh GenerateFullscreenQuad()
        {
            var mesh = new Mesh { name = "FullScreenQuad" };

            mesh.vertices = new Vector3[]
            {
                new Vector3(-1f, -1f, 0f),
                new Vector3(-1f,  1f, 0f),
                new Vector3( 1f,  1f, 0f),
                new Vector3( 1f, -1f, 0f)
            };

            mesh.uv = new Vector2[]
            {
                new Vector2(0f, 0f),
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 0f)
            };

            mesh.triangles = new int[]
            {
                0, 1, 2,
                0, 2, 3
            };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
