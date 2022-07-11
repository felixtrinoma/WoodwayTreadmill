// Unlit shader. Simplest possible color shader.
// - no lighting
// - no lightmap support
// - no per-material color

// by Quentin Delamarre, from Unlit/Texture shader

Shader "Cortex/Unlit Color And Texture Cull Off"
{
	Properties
	{
		_Color("_Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Cull Off
		LOD 100
		
		Pass
		{
			Lighting Off
			SetTexture [_MainTex]
			{ 
				constantColor [_Color]
				combine texture * constant
			} 
		}
	}
}