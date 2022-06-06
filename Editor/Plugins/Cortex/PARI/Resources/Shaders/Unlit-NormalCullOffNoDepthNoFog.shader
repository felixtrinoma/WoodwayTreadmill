// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color
// - no write to depth buffer
// - display first
// - No Fog

// by Quentin Delamarre, from Unlit/Texture shader

Shader "Cortex/Unlit Texture Cull Off No Depth No Fog" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
	Tags { "Queue" = "Background" "RenderType"="Opaque" "IgnoreProjector"="True" }
	Cull Off
	ZWrite Off
	LOD 100
	
	Pass {
		Fog{ Mode Off }
		Lighting Off
		SetTexture [_MainTex] { combine texture } 
	}
}
}