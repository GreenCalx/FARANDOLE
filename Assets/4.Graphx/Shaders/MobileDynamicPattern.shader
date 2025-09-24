Shader "XL/MobileDynamicPattern"
{
    Properties
    { 
        _LerpPatternAB("LerpPatternAB", Float) = 0

        _ColorPattern1 ("ColorPattern1", Color) = (0, 0, 0, 1)
        _ColorPattern2 ("ColorPattern2", Color) = (1, 1, 1, 1)
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _Brightness ("Brightness", Float) = 1
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode("CullMode", Int) = 0.
        [Enum(Off,0,On,1)] _ZWrite("ZWrite", Int) = 1

        [Enum(Checker,0,Boxes,1)] _PatternA("PatternA", Int) = 1
        [Enum(Checker,0,Boxes,1)] _PatternB("PatternB", Int) = 0
        [Enum(Off,0,On,1)] _InvertColors("InvertColors", Int) = 0
        _Tiling("Tiling",Float) = 4
        _BoxSize("BoxSize", Float) = 0.7
        _Angle("Angle", Float) = 0.25
        [ShowAsVector2] _Offset("Offset", Vector) = (0,0,0,0)
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
        #define PI 3.14159265358979323846
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

        float2 tile(float2 _st, float _zoom)
        {
            _st *= _zoom;
            return frac(_st);
        }

        float box(float2 _st, float2 _size, float _smoothEdges)
        {
            _size = float2(0.5,0.5)-_size*0.5;
            float2 aa = float2(_smoothEdges*0.5, _smoothEdges*0.5);
            float2 uv = smoothstep(_size,_size+aa,_st);
            uv *= smoothstep(_size,_size+aa,float2(1.0, 1.0)-_st);
            return uv.x*uv.y;
        }

        float checker(float2 _st, float _tiling)
        {
            float2 pos = floor(_st * float2(_tiling,_tiling));
            float chessboard = pos.x + pos.y;
            chessboard = frac(chessboard*0.5);
            chessboard*=2;
            return chessboard;
        }
        ENDCG

        Pass
        {
            AlphaToMask On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float _LerpPatternAB;
            float4 _Tint;
            float4 _ColorPattern1;
            float4 _ColorPattern2;
            float _Brightness;
            float _AlphaCutoff;
            float _Angle;
            float _BoxSize;
            float _Tiling;
            float2 _Offset;

            fixed _PatternA;
            fixed _PatternB;
            fixed _InvertColors;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float get_pattern_color(v2f i, float2 _st)
            {
                float checker_col   = checker(i.uv, _Tiling);
                float box_col       = box(_st, float2(_BoxSize,_BoxSize),0.01);

                float patA_col = 0;
                if (_PatternA == 0)
                    patA_col = checker_col; 
                else if (_PatternA == 1)
                    patA_col = box_col; 

                float patB_col = 0;
                if (_PatternB == 0)
                    patB_col = checker_col; 
                else if (_PatternB == 1)
                    patB_col = box_col; 

                return lerp(patA_col, patB_col, _LerpPatternAB);
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 st = tile(i.uv + _Offset, _Tiling);
                //st = rotate2D(st, PI*0.25);
                st = rotate2D(st,PI*_Angle);
                
                float patterncol = get_pattern_color(i, st);
                float4 col = float4(
                    patterncol, patterncol, patterncol, 1.0
                    );
                
                // Pattern is in white & black
                if (_InvertColors == 0)
                    col = lerp(_ColorPattern1, _ColorPattern2, patterncol);
                else
                    col = lerp(_ColorPattern2, _ColorPattern1, patterncol);

                // Add Tint
                return col * _Tint;
            }
            ENDCG
        }
    }
    Fallback "Mobile/Unlit"
}
