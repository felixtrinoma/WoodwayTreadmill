// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color

// by Quentin Delamarre, from Unlit/Texture shader
// See the more up-to-date version Unlit-Normal-CullOff.shader (Cortex/Unlit Texture Cull Off 2)

Shader "Cortex/Unlit Texture Cull Off" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
	Tags { "RenderType"="Opaque" "IgnoreProjector"="True" }
	Cull Off
	LOD 100
	
	Pass {
		Lighting Off
		SetTexture [_MainTex] { combine texture } 
	}
}
}