// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "XL/MobileButton"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _OutlineColor("OutlineColor", Color) = (0,0.5,0.8,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Brightness ("Brightness", Float) = 1
        _OutlineWidth("OutlineWidth", Float) = 1
        _OutlineHeight("OutlineHeight", Float) = 1
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode("CullMode", Integer) = 1.
        [Enum(Off,0,On,1)] _ZWrite("ZWrite", Int) = 1
    }
    SubShader
    {
        Tags {
            "Queue" = "AlphaTest"
            "IgnoreProjector"="True"
            "RenderType"="TransparentCutout"
            }
        ZWrite [_ZWrite]
        LOD 100
        Cull [_CullMode]
        Lighting Off
        CGINCLUDE
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

        struct v2fOutline
        {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        sampler2D _MainTex;
        float4 _Color;
        float4 _OutlineColor;
        float4 _MainTex_ST;
        float _Brightness;
        float _AlphaCutoff;
        float _OutlineWidth;
        float _OutlineHeight;


        v2fOutline vertOutline(appdata_base v)
        {
            v2fOutline output;

            //output.pos = UnityObjectToClipPos(v.vertex)
            output.pos = UnityObjectToClipPos(v.vertex);
            //output.pos.xy *= _OutlineWidth;
            output.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
            output.uv -= float2(.5,.5);
            output.uv.x *= _OutlineWidth;
            output.uv.y *= _OutlineHeight;
            output.uv += float2(.5,.5);

            return output;
        }

        fixed4 fragOutline(v2fOutline i) : SV_Target
        {
            fixed alpha = tex2D(_MainTex, i.uv).a;
            fixed4 col = _OutlineColor;
            col.a = alpha;
            return col;
        }
        ENDCG

        // Outline Pass
        Pass
        {
            AlphaToMask On
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragOutline
            
            v2fOutline vert(appdata_base v)
            {
                return vertOutline(v);
            }

            ENDCG
        }

        // Unlit pass
        Pass
        {
            AlphaToMask On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
                
                return col;
            }
            ENDCG
        }
    }
    Fallback "Mobile/Unlit"
}
