// ORIGIN : https://github.com/Falme/Unity-URP-2D-SpriteMask/tree/main

Shader "XL/Stencil Draw Out Mask Wobble"
{
Properties
{
	[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
	_Color ("Tint", Color) = (1,1,1,1)
	_Tiling ("Tiling", Vector) = (1,1,0,0)
	_WobbleStrength ("Wobble Strength", Range(0, 10)) = 0.01
	_WobbleSpeed ("Wobble Speed", Range(0, 5)) = 1
	_WobbleScale ("Wobble Scale", Range(1, 20)) = 5
	_VertexWobble ("Vertex Wobble", Range(0, 10)) = 0.005
	_VertexWobbleSpeed ("Vertex Wobble Speed", Range(0, 5)) = 0.5
	_VertexWobbleAmplitude ("Vertex Wobble Amplitude", Range(0, 20)) = 2
	_BlurAmount ("Blur Amount", Range(0, 0.01)) = 0.001
	[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	_StencilRef ("Stencil ID", Float) = 1
}
 
SubShader
{
	Tags
	{
		"Queue"="Transparent+1"              //DON'T FORGET this must be drew later to catch Stencil Ref value.
		"IgnoreProjector"="True"
		"RenderType"="Transparent"
		"PreviewType"="Plane"
		"CanUseSpriteAtlas"="True"
        "RenderPipeline" = "UniversalPipeline"
	}
 
	Cull Off
	Lighting Off
	ZWrite Off
	Fog { Mode Off }
	Blend One OneMinusSrcAlpha
 
	Pass
	{
		Stencil
		{
			Ref [_StencilRef]
			Comp NotEqual
		}
 
	CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile DUMMY PIXELSNAP_ON
		#include "UnityCG.cginc"
 
		struct appdata_t
		{
			float4 vertex   : POSITION;
			float4 color    : COLOR;
			float2 texcoord : TEXCOORD0;
		};
 
		struct v2f
		{
			float4 vertex   : SV_POSITION;
			fixed4 color    : COLOR;
			half2 texcoord  : TEXCOORD0;
		};
 
		fixed4 _Color;
		float4 _Tiling;
		float _WobbleStrength;
		float _WobbleSpeed;
		float _WobbleScale;
		float _VertexWobble;
		float _VertexWobbleSpeed;
		float _VertexWobbleAmplitude;
		float _BlurAmount;
 
		v2f vert(appdata_t IN)
		{
			v2f OUT;

			half baseTime = _Time.y;
			half time = baseTime * _WobbleSpeed;
			half2 wobble;
			wobble.x = sin(time + IN.texcoord.y * _WobbleScale) * _WobbleStrength;
			wobble.y = sin(time * 1.3 + IN.texcoord.x * _WobbleScale * 0.8) * _WobbleStrength;

			float4 worldPos = IN.vertex;
			half vertexTime = baseTime * _VertexWobbleSpeed;
			half wave1 = sin(vertexTime + IN.vertex.y * _VertexWobbleAmplitude);
			half wave2 = sin(vertexTime * 1.5 + IN.vertex.x * _VertexWobbleAmplitude * 0.9);
			half2 smoothWaves = half2(wave1 * wave1 * wave1, wave2 * wave2 * wave2);
			worldPos.xy += smoothWaves * _VertexWobble;

			OUT.vertex = UnityObjectToClipPos(worldPos);
			OUT.texcoord = (IN.texcoord + wobble) * _Tiling.xy;
			OUT.color = IN.color * _Color;
			#ifdef PIXELSNAP_ON
			OUT.vertex = UnityPixelSnap (OUT.vertex);
			#endif

			return OUT;
		}
 
		sampler2D _MainTex;
 
		fixed4 frag(v2f IN) : SV_Target
		{
			half2 uv = IN.texcoord;
			fixed4 c = tex2D(_MainTex, uv);

			half blur = _BlurAmount;
			if (blur > 0.0001)
			{
				c += tex2D(_MainTex, uv + half2(blur, 0));
				c += tex2D(_MainTex, uv + half2(-blur, 0));
				c += tex2D(_MainTex, uv + half2(0, blur));
				c += tex2D(_MainTex, uv + half2(0, -blur));
				c *= 0.2;
			}

			c *= IN.color;
			c.rgb *= c.a;
			return c;
		}
	ENDCG
	 }
}
}