// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "XL/MobileStickerUnlit"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode("CullMode", Int) = 0.
        [Enum(Off,0,On,1)] _ZWrite("ZWrite", Int) = 1
        _AlphaCutoff("AlphaCutoff", Float ) = 0.5
        
        [Enum(Off,0,On,1)]_Shine("Shine",Int) = 1
        _ShineColor ("ShineColor", Color) = (1,1,1,1)
        _ShineBandWidth("ShineBandWidth",Float) = 0.03
        _ShineSpeedX("ShineSpeedX", Float) = 0.05
        _ShineSpeedY("ShineSpeedY", Float) = 0.05
        _ShineAngle("ShineAngle", Float) = 45

        _OutlineColor("OutlineColor", Color) = (1,1,1,1)
        _OutlineSize("OutlineSize", Float) = 1
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
            fixed4 color : COLOR;
        };

        sampler2D _MainTex;
        float4 _Color;
        float4 _OutlineColor;
        float4 _ShineColor;
        float _OutlineSize;
        float4 _MainTex_TexelSize;
        float4 _MainTex_ST;
        fixed _Shine;
        float _ShineBandWidth;
        float _ShineSpeedX;
        float _ShineSpeedY;
        float _AlphaCutoff;
        float _ShineAngle;

        v2f vertOutline(appdata_t IN)
        {
            v2f OUT;

            OUT.vertex = UnityObjectToClipPos(IN.vertex);
			OUT.uv = IN.uv;
			OUT.color = IN.color * _OutlineColor;

            return OUT;
        }

        fixed4 fragOutline(v2f i) : SV_Target
        {
            // Sobel filter
            float d = _MainTex_TexelSize.xy * _OutlineSize;

            half a1 = tex2D(_MainTex, i.uv + d * float2(-1, -1)).a;
            half a2 = tex2D(_MainTex, i.uv + d * float2( 0, -1)).a;
            half a3 = tex2D(_MainTex, i.uv + d * float2(+1, -1)).a;

            half a4 = tex2D(_MainTex, i.uv + d * float2(-1,  0)).a;
            half a6 = tex2D(_MainTex, i.uv + d * float2(+1,  0)).a;

            half a7 = tex2D(_MainTex, i.uv + d * float2(-1, +1)).a;
            half a8 = tex2D(_MainTex, i.uv + d * float2( 0, +1)).a;
            half a9 = tex2D(_MainTex, i.uv + d * float2(+1, +1)).a;

            float gx = - a1 - a2*2 - a3 + a7 + a8*2 + a9;
            float gy = - a1 - a4*2 - a7 + a3 + a6*2 + a9;

            float w = sqrt(gx * gx + gy * gy) / 4;

            // Mix the contour color
            half4 source = tex2D(_MainTex, i.uv);
            return half4(lerp(source.rgb, _OutlineColor.rgb, w), w);
        }


        float invLerp(float from, float to, float value)
        {
            return (value - from) / (to - from);
        }
        float remap(float origFrom, float origTo, float targetFrom, float targetTo, float value)
        {
            float rel = invLerp(origFrom, origTo, value);
            return lerp(targetFrom, targetTo, rel);
        }

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
            Name "StickerUnlit"
            AlphaToMask On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            v2f vert (appdata_t v)
            {
                v2f outline = vertOutline(v);

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = outline.color;
                return outline;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) ;
                col *= _Color;
                //fixed4 col = i.color;
                fixed4 outlineCol = fragOutline(i);
                if (outlineCol.a > 0.8)
                    col = outlineCol;

                if (col.a < _AlphaCutoff)
                    discard;

                if (_Shine < 1)
                    return col;
            
                // Move band center with time
                float bandCenterX = lerp(0,1, frac(_Time.y*_ShineSpeedX));               
                float bandCenterY = lerp(0,1, frac(_Time.y*_ShineSpeedY));               

                // Retrieve frag dist from bandcenter
                //float dist = (bandCenterX - i.uv.x) - (bandCenterY - i.uv.y);
                float dist = ((bandCenterX - rotate2D(i.uv, _ShineAngle).x) - (bandCenterY -  rotate2D(i.uv, _ShineAngle).y));
                dist = sqrt(dist*dist);

                if (dist <= _ShineBandWidth)
                {
                    // clean alpha residues
                    if (col.a > 0.1)
                    {
                        // Smooth Color mixing
                        fixed4 shineCol = col+(_ShineColor.a*_ShineColor);
                        return lerp(shineCol , col, remap(-_ShineBandWidth,_ShineBandWidth,0,1,dist));
                    }
                }
                return col;
            }
            ENDCG
        }
    }
    Fallback "Mobile/Unlit"
}
