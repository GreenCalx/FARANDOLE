Shader "XL/2DOutline"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (1, 0, 0, 1)
        _OutlineWidth("Outline Width", Range(0, 0.1)) = 0.01
    }
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
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            float4 _OutlineColor;
            float _OutlineWidth;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float4 positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                float3 normal = float3(0, 0, -1);
                positionCS.xy += normal.xy * _OutlineWidth;
                OUT.positionHCS = positionCS;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}
