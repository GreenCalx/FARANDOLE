Shader "XL/UI/UIMobileStencilWrite"
{
    Properties
    {
        [MainTexture] _MainTex("Texture", 2D) = "white" {}
        _StencilRef("Stencil Reference", Int) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Geometry-1"
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
        }

        ZWrite On
        ColorMask 0
        Stencil
        {
            Ref [_StencilRef]
            Comp Always
            Pass Replace
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float4 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Sample texture to get alpha, but don't output color
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                clip(col.a - 0.01); // Clip if alpha is too low
                return 0;
            }
            ENDHLSL
        }
    }
}
