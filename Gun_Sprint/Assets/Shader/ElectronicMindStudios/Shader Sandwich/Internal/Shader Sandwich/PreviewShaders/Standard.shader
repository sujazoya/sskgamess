// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'mul(UNITY_MATRIX_MVP,*)'

Shader "Hidden/ShaderSandwich/Standard" {
Properties {
	_MainTex ("Texture to blend", 2D) = "black" {} 
	_Color ("Main Color", Color) = (1,1,1,1) 
}
SubShader {
	Tags { "Queue" = "Transparent" }
	Pass {
		Blend SrcAlpha OneMinusSrcAlpha ZWrite Off ColorMask RGBA Fog {Mode Off}
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				col.a = 1;
				col *= _Color;
				//UNITY_OPAQUE_ALPHA(col.a);
				#if !defined(UNITY_COLORSPACE_GAMMA)
					col = pow(max(0,col),1/2.2);
				#endif
				return col;
			}
		ENDCG
	}
}
}