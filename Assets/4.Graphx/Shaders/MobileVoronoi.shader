Shader "XL/MobileVoronoi"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CellScale("CellScale", float) = 6.0
        _TimeScale("TimeScale",float) = 1.0
        _MinDist("MinDist",float) = 1.0

        _InnerCellColor("InnerCellColor", Color) = (1, 1, 1, 1)
        _OutCellColor("OutCellColor", Color) = (0, 1, 1, 1)
        _Tint("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
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
            float4 _MainTex_ST;
            float _CellScale;
            float _TimeScale;
            float _MinDist;
            float4 _InnerCellColor;
            float4 _OutCellColor;
            float4 _Tint;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            float2 random2(float2 p)
			{
				return frac(sin(float2(dot(p,float2(127.1,311.7)),dot(p,float2(269.5,183.3))))*43758.5453);
			}

            float2 random3(float2 p)
			{
				return frac(
                        tan(
                            float2(
                                dot(p,float2(2,1)),
                                dot(p,float2(-2,0))
                                )
                            )
                            );
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = fixed4(0,0,0,1);
				float2 uv = i.uv;
				uv *= _CellScale; //Scaling amount (larger number more cells can be seen)
				float2 iuv = floor(uv); //gets integer values no floating point
				float2 fuv = frac(uv); // gets only the fractional part
				float minDist = _MinDist;  // minimun distance
				for (int y = -1; y <= 1; y++)
				{
					for (int x = -1; x <= 1; x++)
					{
						// Position of neighbour on the grid
						float2 neighbour = float2(float(x), float(y));
						// Random position from current + neighbour place in the grid
						float2 pointv = random3(iuv + neighbour);
						//float2 pointv = random3(iuv + neighbour);

						// Move the point with time
						pointv = 0.5 + 0.5*sin(_TimeScale*_Time.z + 6.2236*pointv);//each point moves in a certain way
																		// Vector between the pixel and the point
						float2 diff = neighbour + pointv - fuv;
						// Distance to the point
						float dist = length(diff);

                        minDist = min(minDist,minDist*dist);
					}
				}

                col += smoothstep(_InnerCellColor, _OutCellColor, minDist);
				return col*_Tint;
			}
            ENDCG
        }
    }
}
