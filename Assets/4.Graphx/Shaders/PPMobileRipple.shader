Shader "XL/PP/PPMobileRipple"
{
    Properties
    {
        _MainTex ("_MainTex", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            ZTest Always Cull Off ZWrite Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#undef _CAMERA_DEPTH_TEXTURE
            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings   { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD0; };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float2 _RippleCenter;
            float  _RippleStrength;
            float  _RippleFrequency;
            float  _RippleSpeed;
            float  _TimeValue;

            Varyings Vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 Frag(Varyings i) : SV_Target
            {
                // Flip Y for 2D Renderer
                float2 uv = float2(i.uv.x, 1 - i.uv.y);

                float2 dir  = uv - _RippleCenter;
                float  dist = length(dir);
                float  ripple = sin(dist * _RippleFrequency - _TimeValue * _RippleSpeed);
                uv += normalize(dir) * ripple * _RippleStrength;

                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
            }
            ENDHLSL
        }
    }
}
