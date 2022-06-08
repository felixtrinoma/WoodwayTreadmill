// Unlit shader. Simplest possible textured shader with multiplicative blending.
// - no lighting
// - no lightmap support
// - no per-material color
// - mesh must have uv2 (used for the blending texture)

// by Quentin Delamarre, from Unlit/Texture shader

Shader "Cortex/Unlit Texture Cull Off with Blend" {
Properties {
	_MainTex ("Main (RGB)", 2D) = "white" {}
	_BlendTex ("Blend (RGB)", 2D) = "white" {}
}

SubShader {
	Tags { "RenderType"="Opaque" }
	Cull Off
	LOD 100
	
	Pass {
		Lighting Off
		BindChannels {
			Bind "texcoord", texcoord0 // main uses 1st uv
			Bind "texcoord1", texcoord1 // blend uses 2nd uv
		}
		SetTexture [_MainTex] { combine texture }
		SetTexture [_BlendTex] { combine previous * texture }
	}
}
}