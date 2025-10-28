Shader "XL/Mesh2DOutline"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (1, 0, 0, 1)
        _OutlineWidth("Outline Width", Range(0, 2)) = 0.01
    }
    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    // The Blit.hlsl file provides the vertex shader (Vert),
    // the input structure (Attributes), and the output structure (Varyings)
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
    ENDHLSL
    SubShader
    {
        Tags
        {
            "Queue"="Geometry+1"
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
        }
        Pass
        {
            Name "Outline"
            Cull Front
            ZWrite Off
            ZTest Always

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            float4 _OutlineColor;
            float _OutlineWidth;

            // Varyings vert(Attributes IN)
            // {
            //     Varyings OUT;
            //     float4 positionCS = TransformObjectToHClip(IN.positionOS.xyz);
            //     float3 normalVS = mul((float3x3)UNITY_MATRIX_IT_MV, IN.normalOS);
            //     float2 offset = normalize(normalVS.xy) * _OutlineWidth;
            //     positionCS.xy += offset;
            //     OUT.positionHCS = positionCS;
            //     return OUT;
            // }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.texcoord.xy;
                float4 col = SAMPLE_TEXTURE2D(_BlitTexture,sampler_LinearRepeat, uv);
                if (col.a < 0.1)
                    discard;
                return col * _OutlineColor;
            }
            ENDHLSL
        }
    }
}
