// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/Types/Mask"{
	Properties{
	Background ("Color",Color) = (1,1,1,1)}
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
			sampler2D RGBMask;
			float Preview;
			float4 Background;

			float4 frag (v2f i) : SV_Target{
				float4 UV = tex2D(_UVs,i.uv);//Still feels dirty...
				half4 blerg = tex2Dbias(RGBMask,float4(UV.xy,0,-1));
				if (Preview==1){
					blerg = blerg*blerg.a + (Background)*(1-blerg.a);
				}
				return blerg;
			}
			ENDCG
		}
	}
}
