Shader "Cortex/GUIBlendingAdditive" {
  Properties { 
    _MainTex ("Font Texture", 2D) = "white" {} 
    _Color ("Text Color", Color) = (1,1,1,1) 
  } 

  SubShader { 
    Lighting Off 
    Cull Off 
    ZTest lequal 
    ZWrite on 
    Fog { Mode Off } 
    Tags { "Queue"="Transparent" } 

    Pass { 
      Blend SrcAlpha One // Additive blend
      SetTexture [_MainTex] { 
        ConstantColor[_Color] 
        // Part after the comma is alpha, use font's alpha map
        combine constant*constant, texture*constant
      } 
    } 
  } 
}