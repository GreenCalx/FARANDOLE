using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class InjectBlitPass : MonoBehaviour
{
    public BlitRendererFeature.BlitSettings blitSettings;
    BlitRendererFeature.BlitPass m_RenderPass = null;
    private void OnEnable()
    {
        m_RenderPass = new BlitRendererFeature.BlitPass(blitSettings);
        RenderPipelineManager.beginCameraRendering += InjectRenderPass;
    }
    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= InjectRenderPass;
    }
    private void InjectRenderPass(ScriptableRenderContext context, Camera cam)
    {
        cam.GetUniversalAdditionalCameraData().scriptableRenderer.EnqueuePass(m_RenderPass);
    }
}
