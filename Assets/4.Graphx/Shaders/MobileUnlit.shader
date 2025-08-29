Shader "XL/MobileUnlit"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode("CullMode", Integer) = 1.
        [Enum(Off,0,On,1)] _ZWrite("ZWrite", Int) = 1
        _AlphaCutoff("AlphaCutoff", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        Tags {
            "Queue" = "AlphaTest"
            "RenderType"="Opaque"
            }
        ZWrite [_ZWrite]
        LOD 100
        Cull [_CullMode]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert alpha:fade
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _Color;
            float4 _MainTex_ST;
            float _AlphaCutoff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.uv) ;
                col *= _Color;
                if (col.a < _AlphaCutoff )
                    return (0,0,0,0);
                return col;
            }
            ENDCG
        }
    }
}
