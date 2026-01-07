Shader "XL/ProceduralArrowsSplinePathGuide"
{
    Properties
    {
        _Color ("Arrow Color", Color) = (1,1,1,0.9)
        _ArrowCount ("Arrows Count", Float) = 6
        _ArrowSpeed ("Arrow Speed", Float) = 1
        _ShaftWidth ("Shaft Width", Range(0,1)) = 0.25
        _HeadLength ("Head Length", Range(0,1)) = 0.35
        _EdgeFade ("Edge Fade", Range(0,0.5)) = 0.15
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _Color;
            float _ArrowCount;
            float _ArrowSpeed;
            float _ShaftWidth;
            float _HeadLength;
            float _EdgeFade;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                // Always scroll forward along spline length
                o.uv.y = o.uv.y + _Time.y * _ArrowSpeed;

                return o;
            }

            // Draw a single arrow in 0..1 space
            float ArrowMask(float2 uv)
            {
                // Split spline into repeating arrow segments
                float v = frac(uv.y * _ArrowCount);

                // Shaft (rectangle)
                float shaft =
                    step(0.0, v) *
                    step(v, 1.0 - _HeadLength) *
                    step(abs(uv.x - 0.5), _ShaftWidth * 0.5);

                // Arrow head (triangle)
                float headV = (v - (1.0 - _HeadLength)) / _HeadLength;
                float head =
                    step(0.0, headV) *
                    step(abs(uv.x - 0.5), (1.0 - headV) * 0.5);

                return max(shaft, head);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float mask = ArrowMask(i.uv);

                // Soft edges on width
                float edge =
                    smoothstep(0, _EdgeFade, i.uv.x) *
                    smoothstep(1, 1 - _EdgeFade, i.uv.x);

                float alpha = mask * edge * _Color.a;

                return fixed4(_Color.rgb, alpha);
            }
            ENDHLSL
        }
    }
}
