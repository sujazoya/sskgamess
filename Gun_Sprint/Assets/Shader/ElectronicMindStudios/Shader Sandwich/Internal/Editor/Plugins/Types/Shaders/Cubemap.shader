// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/Types/Cubemap"{
	Properties{
	Background ("Color",Color) = (1,1,1,1)
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
			samplerCUBE Cubemap;
			half4 Cubemap_HDR;
			float HDR_Support;
			sampler2D Texture;
			float Preview;
			float4 Background;

			float4 frag (v2f i) : SV_Target{
				float4 UV = tex2D(_UVs,i.uv);//Still feels dirty...
				if (Preview==0){
					UV.xy = UV.xy*2-1;
					UV.xyz = normalize(float3(1,UV.y,-UV.x));
					
					float4 PixCol = texCUBEbias(Cubemap,float4(UV.xyz,-10));
					
					if (HDR_Support==1)
						PixCol = float4(DecodeHDR (PixCol, Cubemap_HDR),PixCol.a);
					return PixCol;
				}else{
					half4 bleg = tex2Dbias(Texture,float4(UV.xy,0,-1));
					return bleg*bleg.a+(Background)*(1-bleg.a);
				}
			}
			ENDCG
		}
	}
}
