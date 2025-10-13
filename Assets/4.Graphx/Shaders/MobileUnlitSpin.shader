Shader "XL/MobileUnlitSpin"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode("CullMode", Int) = 0.
        [Enum(Off,0,On,1)] _ZWrite("ZWrite", Int) = 1
        _AlphaCutoff("AlphaCutoff", Float ) = 0.5
        _SpinSpeed("SpinSpeed", Float) = 1.0
    }
    SubShader
    {
        Tags {
            "Queue" = "AlphaTest"
            "IgnoreProjector"="True"
            "RenderType"="TransparentCutout"
            "CanUseSpriteAtlas" = "True"
            "RenderPipeline" = "UniversalPipeline"
            }
        ZWrite [_ZWrite]
        LOD 100
        Cull [_CullMode]
        Lighting Off

        CGINCLUDE
        #include "UnityCG.cginc"
        #define PI 3.14159265358979323846
        struct appdata_t
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            fixed4 color : COLOR;
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
        };

        sampler2D _MainTex;
        float4 _Color;
        float4 _MainTex_ST;
        float _Brightness;
        float _AlphaCutoff;
        float _SpinSpeed;


        float2 rotate2D(float2 _st, float _angle)
        {
            _st -= 0.5;

            float2x2 inter = float2x2(
                        cos(_angle), -sin(_angle),
                        sin(_angle),  cos(_angle)
                        );
            _st =  mul(inter ,_st);
            _st += 0.5;
            return _st;
        }
        
        ENDCG

        // Unlit pass
        Pass
        {
            Name "SpinUnlit"
            AlphaToMask On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 st = rotate2D(i.uv ,PI*_SpinSpeed*_Time.y);
                fixed4 col = tex2D(_MainTex, st) ;

                // col.r *= _Color.r * _Brightness;
                // col.g *= _Color.g * _Brightness;
                // col.b *= _Color.b * _Brightness;
                if (col.a < _AlphaCutoff)
                    discard;
                col *= _Color;
                
                return col;
            }
            ENDCG
        }
    }
    Fallback "Mobile/Unlit"
}
