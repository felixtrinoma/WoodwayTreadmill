// Lines Colored Blended
// for showing the focus distance and the physical screen in the Scene View in the Unity editor

Shader "Cortex/Lines Colored Blended" {

	SubShader { 
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off Cull Off Fog { Mode Off }
			BindChannels {
				  Bind "vertex", vertex Bind "color", color
			}
		}
	}
}