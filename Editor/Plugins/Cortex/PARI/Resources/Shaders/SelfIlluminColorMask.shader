Shader "AddingLight/SelfIllumColorMask"
{
	Properties 
	{
		_MainTex("_MainTex", 2D) = "white" {}
		_MaskTex("_MaskTex", 2D) = "white" {}
		_Color("_Color", Color) = (1,1,1,1)
	}
	
	SubShader 
	{
		Tags
		{
			//"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}

		
	//Cull Back
	Cull Off
	ZWrite On
	ZTest Always
	//ZTest LEqual
	//ZTest Never


		CGPROGRAM
		#pragma surface surf Lambert alpha
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _MaskTex;
		float4 _Color;

		struct Input 
		{
			float2 uv_MainTex;
		};
			
		void surf (Input IN, inout SurfaceOutput o) 
		{
			float4 col			= tex2D(_MainTex, IN.uv_MainTex);
			float4 colMask	= tex2D(_MaskTex, IN.uv_MainTex);
			float4 col1			= col*_Color;
			o.Emission			= col1*colMask.r;	//Le mask doit etre en niveau de gris, le blanc etant ce qui est tracé et le noir ce qui ne l'est pas
			o.Alpha					= 1;
		}
		ENDCG
	}
	Fallback "Diffuse"
}