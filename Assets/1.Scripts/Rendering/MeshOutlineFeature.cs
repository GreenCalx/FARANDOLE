using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class MeshOutlineFeature : ScriptableRendererFeature
{
    class OutlinePass : ScriptableRenderPass
    {
        private Material outlineMaterial;
        private FilteringSettings filteringSettings;
        private ProfilingSampler profilingSampler;

        public OutlinePass(Material material)
        {
            outlineMaterial = material;
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            filteringSettings = new FilteringSettings(RenderQueueRange.opaque, ~0);
            profilingSampler = new ProfilingSampler("Outline");
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            // Get the camera color target as a TextureHandle
            var cameraData = frameData.Get<UniversalCameraData>();
            var colorTarget = renderGraph.ImportTexture(cameraData.renderer.cameraColorTargetHandle);

            using (var builder = renderGraph.AddRenderPass<OutlinePassData>("Outline", out var passData, profilingSampler))
            {
                // Use the color target as input
                builder.UseColorBuffer(colorTarget, 0);

                builder.SetRenderFunc((OutlinePassData data, RenderGraphContext context) =>
                {
                    var sortFlags = cameraData.defaultOpaqueSortFlags;

                    var camera = frameData.Get<UniversalCameraData>().camera;
                    SortingSettings sortSettings = new SortingSettings(camera);
                    DrawingSettings drawSettings = new DrawingSettings(
                        new ShaderTagId("UniversalForward"), sortSettings
                    );
                    drawSettings.overrideMaterial = outlineMaterial;
                    drawSettings.overrideMaterialPassIndex = 0;

                    camera.TryGetCullingParameters(out var cullingParameters);
                    CullingResults cullingResults = context.renderContext.Cull(ref cullingParameters);
                    
                    var rendererListParams = new RendererListParams(cullingResults, drawSettings, filteringSettings);
                    context.cmd.DrawRendererList(
                        context.renderContext.CreateRendererList(ref rendererListParams)
                    );
                });
            }
        }

        // For backward compatibility (not used in Render Graph)
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {}
    }

    [SerializeField]
    private Material outlineMaterial;

    private OutlinePass outlinePass;

    public override void Create()
    {
        outlinePass = new OutlinePass(outlineMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(outlinePass);
    }
}

public class OutlinePassData {}
