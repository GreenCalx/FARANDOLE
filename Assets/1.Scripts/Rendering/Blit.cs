using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering; // for GraphicsFormat

public class BlitRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class BlitSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        public Material blitMaterial;
        public string blitShaderPassName = "Blit";
        public string mainTexPropertyName = "_MainTex";
    }

    public BlitSettings settings = new BlitSettings();
    private BlitPass blitPass;

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

        // persistent temp RTHandle (reallocated/resized as needed)
        RTHandle tempTarget;
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
            // Allocate / reallocate tempTarget to match camera descriptor (keeps persistent RTHandle)
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;

            // Use RenderingUtils helper to allocate or reallocate RTHandle if needed
            RenderingUtils.ReAllocateIfNeeded(ref tempTarget, desc, name: "_BlitTemp");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null)
                return;
             
            var camData = renderingData.cameraData;
            RTHandle cameraColor = camData.renderer.cameraColorTargetHandle;
            if (cameraColor == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("BlitRendererFeature");

            // Provide screen size info

            cmd.SetGlobalVector("_MainTex_TexelSize",
                new Vector4(1f / Screen.width, 1f / Screen.height, Screen.width, Screen.height));

            // Choose safe color format for gamma projects
            GraphicsFormat colorFormat = cameraColor.rt.graphicsFormat;
            bool projectIsGamma = (QualitySettings.activeColorSpace == ColorSpace.Gamma);
            if (projectIsGamma && GraphicsFormatUtility.IsSRGBFormat(colorFormat))
                colorFormat = GraphicsFormatUtility.GetGraphicsFormat(RenderTextureFormat.Default, false);

            // Temporary RT
            // tempTarget = RTHandles.Alloc(Vector2.one,
            //     colorFormat: colorFormat,
            //     dimension: TextureDimension.Tex2D,
            //     useDynamicScale: true,
            //     name: "BlitTemp");

            // 1️⃣ Copy scene into temp
            Blitter.BlitCameraTexture(cmd, cameraColor, tempTarget);

            if (!string.IsNullOrEmpty(settings.mainTexPropertyName))
            {
                Texture tex = tempTarget.rt != null ? (Texture)tempTarget.rt : (Texture)tempTarget;
                cmd.SetGlobalTexture(settings.mainTexPropertyName, tex);
            }

            // 2️⃣ Draw quad with shader → back into cameraColor
            CoreUtils.SetRenderTarget(cmd, cameraColor);

            material.SetTexture("_MainTex", tempTarget);

            cmd.DrawMesh(fullScreenQuad, Matrix4x4.identity, material, 0, passIndex);

            //RTHandles.Release(tempTarget);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

         public override void OnCameraCleanup(CommandBuffer cmd)
        {

        }

        public void Dispose()
        {
            if (tempTarget != null)
            {
                RTHandles.Release(tempTarget);
                tempTarget = null;
            }
        }

        // Generates a real fullscreen quad (two triangles, full UV coverage)
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
