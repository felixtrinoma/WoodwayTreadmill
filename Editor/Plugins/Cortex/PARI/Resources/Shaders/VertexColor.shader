// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// simple vertex color shader with cull off
// author: Quentin Delamarre, inspired from Aras (http://forum.unity3d.com/threads/11327-3dsmax-FBX-Vertex-colors)

Shader "Cortex/Vertex color Cull Off" {

SubShader {
	Cull Off
    Pass {
        Fog { Mode Off }

CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
#pragma exclude_renderers gles
#pragma vertex vert
#pragma fragment frag

// vertex input: position, color
struct appdata {
    float4 vertex : POSITION;
    float4 color : COLOR;
};
 
struct v2f {
    float4 pos : POSITION;
    float4 color : COLOR;
};
v2f vert (appdata v) {
    v2f o;
    o.pos = UnityObjectToClipPos( v.vertex );
    o.color = v.color;
    return o;
}
fixed4 frag(v2f i) : SV_Target
{
	fixed4 col = i.color;
	return col;
}
ENDCG
    }
}
}