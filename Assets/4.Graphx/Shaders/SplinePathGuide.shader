Shader "XL/SplinePathGuide"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,0.8)
        _MainTex ("Texture", 2D) = "white" {}
        _Scroll ("Scroll Speed", Float) = 0
        _EdgeFade ("Edge Fade", Range(0,0.5)) = 0.2
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float _Scroll;
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
                o.uv.y -= _Time.y * _Scroll;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                float edge =
                    smoothstep(0, _EdgeFade, i.uv.x) *
                    smoothstep(1, 1 - _EdgeFade, i.uv.x);

                col.a *= edge;
                return col;
            }
            ENDHLSL
        }
    }
}
