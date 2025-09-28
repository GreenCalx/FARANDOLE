Shader "XL/MobileBlur"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 0)
        _MainTex ("Texture", 2D) = "white" {}
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode("CullMode", Int) = 0.
        [Enum(Off,0,On,1)] _ZWrite("ZWrite", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc ("Blend mode Source", Int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _BlendDst ("Blend mode Destination", Int) = 0
        _BlurSize("BlurSize", Float) = 1
    }
    SubShader
    {
        Tags {
            "RenderQueue" = "Transparent"  
            "RenderType"="Transparent"
            }
        Blend [_BlendSrc] [_BlendDst]
        ZWrite [_ZWrite]
        LOD 100
        Cull [_CullMode]
        Lighting Off

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
            float _BlurSize;

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

                col += tex2D(_MainTex, i.uv + _BlurSize * float2(-1, -1));
                col += tex2D(_MainTex, i.uv + _BlurSize * float2( 0, -1));
                col += tex2D(_MainTex, i.uv + _BlurSize * float2(+1, -1));
                col += tex2D(_MainTex, i.uv + _BlurSize * float2(-1,  0));
                col += tex2D(_MainTex, i.uv + _BlurSize * float2(+1,  0));
                col += tex2D(_MainTex, i.uv + _BlurSize * float2(-1, +1));
                col += tex2D(_MainTex, i.uv + _BlurSize * float2( 0, +1));
                col += tex2D(_MainTex, i.uv + _BlurSize * float2(+1, +1));

                col /= 9;

                col *= _Color;
                return col;
            }
            ENDCG
        }
    }
}
