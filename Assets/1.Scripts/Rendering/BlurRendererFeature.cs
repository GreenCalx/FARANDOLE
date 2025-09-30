using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]
public class BlurSettings
{
    [Range(0, 0.4f)] public float horizontalBlur;
    [Range(0, 0.4f)] public float verticalBlur;
}

internal class BlurRendererFeature : ScriptableRendererFeature
{
    [SerializeField] public Shader m_Shader;
    [SerializeField] private BlurSettings m_Settings;
    Material m_Material;
    BlurPass m_RenderPass = null;

    public override void AddRenderPasses(ScriptableRenderer renderer,
                                    ref RenderingData renderingData)
    {
        if (m_RenderPass == null)
            return; 
        if (renderingData.cameraData.cameraType == CameraType.Game)
            renderer.EnqueuePass(m_RenderPass);
    }

    // public override void SetupRenderPasses(ScriptableRenderer renderer,
    //                                     in RenderingData renderingData)
    // {
    //     if (renderingData.cameraData.cameraType == CameraType.Game)
    //     {
    //         // Calling ConfigureInput with the ScriptableRenderPassInput.Color argument
    //         // ensures that the opaque texture is available to the Render Pass.
    //         m_RenderPass.ConfigureInput(ScriptableRenderPassInput.Color);
    //         m_RenderPass.SetTarget(renderer.cameraColorTargetHandle, m_Settings);
    //     }
    // }

    public override void Create()
    {
        m_Material = CoreUtils.CreateEngineMaterial(m_Shader);
        m_RenderPass = new BlurPass(m_Material, m_Settings);
        m_RenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(m_Material);
    }
}