// Unlit shader. Simplest possible color shader.
// - no lighting
// - no lightmap support
// - no per-material color

// by Quentin Delamarre, from Unlit/Texture shader

Shader "Cortex/Unlit Color Cull Off Overlay"
{
	Properties
	{
		_Color("_Color", Color) = (1,1,1,1)
	}
	
	SubShader
	{
		Tags { "Queue"="Transparent"  "RenderType"="Opaque" }
		Cull Off
		LOD 100
		
		Pass
		{
			Lighting Off
			Color[_Color] 
		}
	}
}