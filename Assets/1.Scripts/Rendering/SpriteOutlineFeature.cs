using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SpriteOutlineFeature : ScriptableRendererFeature
{
    class OutlinePass : ScriptableRenderPass
    {
        private Material outlineMaterial;
        private FilteringSettings filteringSettings;
        private RenderStateBlock renderStateBlock;

        public OutlinePass(Material material)
        {
            outlineMaterial = material;
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            filteringSettings = new FilteringSettings(RenderQueueRange.opaque, ~0);
            renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("Outline");
            using (new ProfilingScope(cmd, new ProfilingSampler("Outline")))
            {
                // Draw all SpriteRenderers with the outline material
                var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
                var drawSettings = CreateDrawingSettings(
                    new ShaderTagId("UniversalForward"), ref renderingData, sortFlags
                );
                drawSettings.overrideMaterial = outlineMaterial;
                drawSettings.overrideMaterialPassIndex = 0;

                context.DrawRenderers(
                    renderingData.cullResults, ref drawSettings, ref filteringSettings, ref renderStateBlock
                );
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    private OutlinePass outlinePass;
    public Material outlineMaterial;

    public override void Create()
    {
        outlinePass = new OutlinePass(outlineMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(outlinePass);
    }
}
