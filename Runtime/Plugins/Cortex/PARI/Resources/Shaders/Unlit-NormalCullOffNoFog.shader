// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color
// - no fog

// by Quentin Delamarre, from Unlit/Texture shader

Shader "Cortex/Unlit Texture Cull Off No Fog" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
	Tags { "RenderType"="Opaque" "IgnoreProjector"="True" }
	Cull Off
	LOD 100
	
	Pass {
		Fog { Mode Off }
		Lighting Off
		SetTexture [_MainTex] { combine texture } 
	}
}
}