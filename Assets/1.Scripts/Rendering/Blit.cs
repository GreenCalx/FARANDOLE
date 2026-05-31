using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
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
        get { return blitPass != null && blitPass.EnableBlit; }
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

    protected override void Dispose(bool disposing)
    {
        blitPass?.Dispose();
        blitPass = null;
    }

    public class BlitPass : ScriptableRenderPass
    {
        private readonly Material material;
        private readonly int passIndex;
        private readonly BlitSettings settings;
        [System.NonSerialized] public bool EnableBlit = false;

        // Persistent (imported) frame used by the frame-lock feature; survives between frames.
        RTHandle lockedFrame;
        bool frameLockActive = false;
        static Mesh fullScreenQuad;

        // Per-frame data captured for the RenderGraph render function.
        class PassData
        {
            public Material material;
            public int passIndex;
            public TextureHandle src;     // camera color (read + write)
            public TextureHandle temp;    // transient copy of the camera color
            public TextureHandle locked;  // persistent frozen frame (frame-lock only)
            public string mainTexProperty;
            public bool useLock;
            public bool lockReplay;       // replay the already-frozen frame
            public bool lockCapture;      // freeze this frame, then display it
            public Mesh quad;
        }

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

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (!EnableBlit) { frameLockActive = false; return; }
            if (material == null) return;

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            // Never sample/blit using the back buffer as the active target.
            if (resourceData.isActiveTargetBackBuffer) return;

            TextureHandle src = resourceData.activeColorTexture;
            if (!src.IsValid()) return;

            // Transient half-res / LDR copy of the camera color to feed the material as _MainTex.
            TextureDesc desc = src.GetDescriptor(renderGraph);
            desc.name = settings.tempRTName;
            desc.depthBufferBits = 0;
            desc.msaaSamples = MSAASamples.None;
            if (settings.opt_HalfResolution)
            {
                desc.width = Mathf.Max(1, desc.width / 2);
                desc.height = Mathf.Max(1, desc.height / 2);
            }
            if (settings.opt_GraphicFormat)
                desc.colorFormat = GraphicsFormatUtility.GetGraphicsFormat(RenderTextureFormat.ARGB32, false);
            TextureHandle temp = renderGraph.CreateTexture(desc);

            // Persistent frozen-frame target for the frame-lock feature.
            bool useLock = settings.req_lockFrame;
            TextureHandle locked = default;
            if (useLock)
            {
                if (lockedFrame == null || lockedFrame.rt == null
                    || lockedFrame.rt.width != desc.width || lockedFrame.rt.height != desc.height)
                {
                    lockedFrame?.Release();
                    var rtd = new RenderTextureDescriptor(desc.width, desc.height, desc.colorFormat, 0) { msaaSamples = 1 };
                    lockedFrame = RTHandles.Alloc(rtd, name: "_LockedTemp");
                }
                locked = renderGraph.ImportTexture(lockedFrame);
            }

            bool lockReplay = useLock && frameLockActive;
            bool lockCapture = useLock && !frameLockActive;

            using (var builder = renderGraph.AddUnsafePass<PassData>(settings.CommandBufferName, out var data))
            {
                data.material = material;
                data.passIndex = passIndex;
                data.src = src;
                data.temp = temp;
                data.locked = locked;
                data.mainTexProperty = settings.mainTexPropertyName;
                data.useLock = useLock;
                data.lockReplay = lockReplay;
                data.lockCapture = lockCapture;
                data.quad = fullScreenQuad;

                builder.UseTexture(src, AccessFlags.ReadWrite);
                builder.UseTexture(temp, AccessFlags.ReadWrite);
                if (useLock) builder.UseTexture(locked, AccessFlags.ReadWrite);
                builder.AllowPassCulling(false);

                builder.SetRenderFunc((PassData d, UnsafeGraphContext ctx) => ExecutePass(d, ctx));
            }

            if (lockCapture) frameLockActive = true;
            if (!useLock) frameLockActive = false;
        }

        static void ExecutePass(PassData d, UnsafeGraphContext ctx)
        {
            CommandBuffer cmd = CommandBufferHelpers.GetNativeCommandBuffer(ctx.cmd);

            RTHandle src = d.src;
            RTHandle temp = d.temp;
            RTHandle locked = d.useLock ? (RTHandle)d.locked : null;

            // Match the (often non-square) mobile resolution for the shader's texel maths.
            cmd.SetGlobalVector("_MainTex_TexelSize",
                new Vector4(1f / Screen.width, 1f / Screen.height, Screen.width, Screen.height));

            // Frame-lock: replay the already-frozen frame through the material.
            if (d.lockReplay)
            {
                BindSource(d, cmd, locked);
                CoreUtils.SetRenderTarget(cmd, src);
                cmd.DrawMesh(d.quad, Matrix4x4.identity, d.material, 0, d.passIndex);
                return;
            }

            // Frame-lock: capture the current camera into the persistent frame, then display it.
            if (d.lockCapture)
            {
                Blitter.BlitCameraTexture(cmd, src, locked);
                BindSource(d, cmd, locked);
                CoreUtils.SetRenderTarget(cmd, src);
                cmd.DrawMesh(d.quad, Matrix4x4.identity, d.material, 0, d.passIndex);
                return;
            }

            // Regular blit: copy camera -> temp, then run the material from temp back onto the camera.
            Blitter.BlitCameraTexture(cmd, src, temp);
            BindSource(d, cmd, temp);
            CoreUtils.SetRenderTarget(cmd, src);
            cmd.DrawMesh(d.quad, Matrix4x4.identity, d.material, 0, d.passIndex);
        }

        static void BindSource(PassData d, CommandBuffer cmd, RTHandle tex)
        {
            string prop = string.IsNullOrEmpty(d.mainTexProperty) ? "_MainTex" : d.mainTexProperty;

            // Global binding (used by _MainTex_TexelSize-driven shaders and non-exposed reads).
            cmd.SetGlobalTexture(prop, tex);
            if (prop != "_MainTex") cmd.SetGlobalTexture("_MainTex", tex);

            // Critical: an *exposed* Shader Graph texture property (m_GeneratePropertyBlock) samples the
            // material's slot, which falls back to the property's default (White) texture when empty and
            // OVERRIDES any global of the same name. So write the real source onto the material slot,
            // exactly like the legacy Execute() pass did, or the effect melts a blank white image.
            if (d.material != null)
                d.material.SetTexture(prop, tex);
        }

        public void Dispose()
        {
            if (lockedFrame != null)
            {
                lockedFrame.Release();
                lockedFrame = null;
            }
        }

        // Generates a real fullscreen quad.
        // This is because of a weird unity fuckery where the UV is defined
        // in a different space or some shit.
        // This results in wrong coordinates and displays only half screen
        // in a triangle. Using a quad mesh ensures that our UVs are correct.
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
