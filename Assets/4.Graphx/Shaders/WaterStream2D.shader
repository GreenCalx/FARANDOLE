Shader "Unlit/WaterStream2D"
{
    Properties
    {
        _Color ("Color", Color) = (0.6,0.85,1,0.9)
        _FlowSpeed ("Flow Speed", Float) = 2.0
        _NoiseStrength ("Noise Strength", Float) = 0.15
        _EdgeSoftness ("Edge Softness", Float) = 0.4
        _WidthWobble ("Width Wobble", Float) = 0.1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float _FlowSpeed;
            float _NoiseStrength;
            float _EdgeSoftness;
            float _WidthWobble;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // Simple hash noise (no texture)
            float hash(float x)
            {
                return frac(sin(x * 12.9898) * 43758.5453);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Flow along stream
                float flow = i.uv.x * 4.0 - _Time.y * _FlowSpeed;

                // Procedural noise
                float noise = hash(floor(flow)) * 2 - 1;
                float distortion = noise * _NoiseStrength;

                // Width wobble
                float widthOffset = sin(flow * 3.0) * _WidthWobble;

                // Distance from center of stream
                float centerDist = abs(i.uv.y - 0.5 + distortion + widthOffset);

                // Soft edges
                float alpha = smoothstep(
                    0.5,
                    0.5 - _EdgeSoftness,
                    centerDist
                );

                return fixed4(_Color.rgb, alpha * _Color.a);
            }
            ENDCG
        }
    }
}
