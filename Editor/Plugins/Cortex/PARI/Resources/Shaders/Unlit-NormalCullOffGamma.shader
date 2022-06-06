// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit shader. Simplest possible textured shader with gamma correction
// - no lighting
// - no lightmap support
// - no per-material color
// - gamma correction (usefull in linear mode)

// by Quentin Delamarre, from Unlit/Texture shader

Shader "Cortex/Unlit Texture Cull Off Gamma" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Gamma ("Gamma", Range (0.1, 4)) = 2.2
}

SubShader {
	Tags { "RenderType"="Opaque" "IgnoreProjector"="True" }
	Cull Off
	LOD 100
	
	Pass {
		//Lighting Off
		//SetTexture [_MainTex] { combine texture } 
		
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Gamma;

			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag (v2f i) : COLOR // SV_Target in Unity 4.5 ?
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				return pow(col, _Gamma);
			}
		ENDCG

	}
}
}