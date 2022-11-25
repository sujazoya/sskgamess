// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/Effects/Mapping/UVDisplace"{
	Properties{}
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
			
			sampler2D _Previous;
			float Displace_Amount;
			float Displace_Middle;
			sampler2D Displace_Mask_RGBA;

			float4 frag (v2f i) : SV_Target{
				float4 UV = tex2D(_Previous,i.uv);//This feels so dirty...
				UV.xyz += (tex2D(Displace_Mask_RGBA,i.uv.xy)-Displace_Middle)*Displace_Amount;
				return UV;
			}
			ENDCG
		}
	}
}
