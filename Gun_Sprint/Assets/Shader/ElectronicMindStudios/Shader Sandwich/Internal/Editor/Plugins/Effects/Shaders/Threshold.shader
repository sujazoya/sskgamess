// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/Effects/Threshold"{
	Properties{}
	SubShader{
		Cull Off ZWrite Off ZTest Always
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "SSEffectShaderHelperStuff.cginc"

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
			float4 Tint;
			float Threshold;

			float4 frag (v2f i) : SV_Target{
				float4 OldColor = tex2D(_Previous,i.uv);
				//return DoAlphaModeStuff(OldColor,OldColor * ceil(saturate(OldColor-Threshold)));
				if (_AlphaMode==0)
					return float4(OldColor * ceil(saturate(dot(float3(0.3,0.59,0.11),OldColor.rgb).rrr-Threshold)),OldColor.a);
				if (_AlphaMode==1)
					return float4(OldColor * ceil(saturate(dot(float3(0.3,0.59,0.11),OldColor.rgb).rrr-Threshold)),OldColor.a * ceil(saturate(OldColor.a)-Threshold));
				if (_AlphaMode==2)
					return float4(OldColor.rgb, OldColor.a * ceil(saturate(OldColor.a)-Threshold));
					
				return OldColor;
			}
			ENDCG
		}
	}
}
