Shader "Hidden/ShaderSandwich/Alpha Lighten" {
Properties {_MainTex ("Texture to blend", 2D) = "black"{} 
_Color ("Main Color", Color) = (1,1,1,1) }
SubShader {
	Tags { "Queue" = "Transparent" }
	Pass {
		Blend One One ZWrite Off ColorMask RGBA Fog {Mode Off}
		Lighting Off
		SetTexture [_MainTex] { combine texture * texture alpha, texture}
		SetTexture [_MainTex] {constantColor [_Color] combine previous*constant alpha}
		SetTexture [_MainTex] {constantColor [_Color] combine previous*constant}
	}
}
}