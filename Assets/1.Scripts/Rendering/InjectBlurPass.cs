using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;
public class InjectBlurPass : MonoBehaviour
{
    [SerializeField] public Shader m_Shader;
    public BlurSettings blurSettings;
    BlurPass m_RenderPass = null;
    Material m_Material;
    private void OnEnable()
    {
        m_Material = CoreUtils.CreateEngineMaterial(m_Shader);
        m_RenderPass = new BlurPass(m_Material, blurSettings);
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
