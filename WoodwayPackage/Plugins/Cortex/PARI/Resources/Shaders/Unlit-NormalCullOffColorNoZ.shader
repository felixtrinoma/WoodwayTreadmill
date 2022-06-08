// Unlit shader. Simplest possible color shader.
// - no lighting
// - no lightmap support
// - no per-material color

// by Quentin Delamarre, from Unlit/Texture shader

Shader "Cortex/Unlit Color Cull Off No Z"
{
	Properties
	{
		_Color("_Color", Color) = (1,1,1,1)
	}
	
	SubShader
	{
		Tags { "Queue"="Geometry" "LightMode" = "Always" }
		Cull Off
		ZWrite Off
		ZTest LEqual
		ColorMask RGB // alpha not used
		
		Pass
		{
			Lighting Off
			Color[_Color] 
		}
	}
}