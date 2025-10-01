Shader "XL/MobileEmissiveUnlit"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        [HDR]_EmissiveColor ("EmissiveColor",Color) = (0,0,0,0)
        _MainTex ("Texture", 2D) = "white" {}
        _Brightness ("Brightness", Float) = 1
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode("CullMode", Int) = 0.
        [Enum(Off,0,On,1)] _ZWrite("ZWrite", Int) = 1
    }
    SubShader
    {
        Tags {
            "Queue" = "AlphaTest"
            "IgnoreProjector"="True"
            "RenderType"="TransparentCutout"
            "RenderPipeline" = "UniversalPipeline"
            }
        ZWrite [_ZWrite]
        LOD 100
        Cull [_CullMode]
        Lighting Off

        Pass
        {
            AlphaToMask On

            CGPROGRAM
            #pragma vertex vert
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
            float4 _EmissiveColor;
            float4 _MainTex_ST;
            float _Brightness;
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
                // col.r *= _Color.r * _Brightness;
                // col.g *= _Color.g * _Brightness;
                // col.b *= _Color.b * _Brightness;
                col *= _Color;
                if (col.a > 0)
                    col += _EmissiveColor;
                return col;
            }
            ENDCG
        }
    }
    Fallback "Mobile/Unlit"
}
