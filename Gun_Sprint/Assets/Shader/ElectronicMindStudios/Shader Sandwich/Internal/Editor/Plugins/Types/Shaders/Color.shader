// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/Types/Color"{
	Properties{
		_Color ("Color",Color) = (1,1,1,1)
	}
	SubShader{
		Cull Off ZWrite Off ZTest Always
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v){
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _UVs;
			//float4 Color;
			float4 _Color;

			float4 frag (v2f i) : SV_Target{
				//float4 UV = tex2D(_UVs,i.uv);//Yay! :P
				return _Color;//pow(Color,2.2);
			}
			ENDCG
		}
	}
}
