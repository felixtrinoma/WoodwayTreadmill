// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit shader. Simplest possible color and texture shader with image corrections
// - color and texture
// - no lighting
// - no lightmap support
// - brightness, contrast, saturation and gamma corrections

// by Quentin Delamarre, from Unlit/Texture shader

Shader "Cortex/Unlit Color Texture Cull Off Image Processing" {
	Properties{
		_Color("_Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Brightness("Brightness", Range(-1, 1)) = 0
		_Contrast("Contrast", Range(-2, 2)) = 0
		_Saturation("Saturation", Range(0, 8)) = 1
		_Gamma("Gamma", Range(0.1, 4)) = 2.2
	}

	SubShader{
	Tags{"RenderType" = "Opaque" "IgnoreProjector" = "True" }
	Cull Off
	LOD 100

	Pass{
		Lighting Off

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

		float4 _Color;
		sampler2D _MainTex;
		float4 _MainTex_ST;
		float _Brightness;
		float _Contrast;
		float _Saturation;
		float _Gamma;


		v2f vert(appdata_t v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
			return o;
		}

		static const float3 lumCoeff = float3(0.2125, 0.7154, 0.0721);

		fixed4 frag(v2f i) : COLOR // SV_Target in Unity 4.5 ?
		{
			fixed4 col = tex2D(_MainTex, i.texcoord) * _Color;
			col.rgb = (col.rgb - 0.5) *(_Contrast + 1.0) + 0.5;
			col.rgb = col.rgb + _Brightness;
			float3 intensity = float(dot(col.rgb, lumCoeff));
			col.rgb = lerp(intensity, col.rgb, _Saturation);
			return pow(col, _Gamma);
		}
		ENDCG

	}
	}
}