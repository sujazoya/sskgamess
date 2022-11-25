// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/Effects/FractalDetail"{
	Properties{}
	SubShader{
		Cull Off ZWrite Off ZTest Always
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
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
			float Count;
			float Scale;
			float Weighting;

			float4 frag (v2f i) : SV_Target{
				float4 OldColor = tex2D(_Previous,i.uv);
				float4 NewColor = 0;
				float scaleyereasdasd = 1;
				float Weight = 1;
				for(float iii = 0; iii <= Count;iii++){
					NewColor += tex2Dlod(_Previous,float4(i.uv*scaleyereasdasd,0,0))*Weight;
					scaleyereasdasd *= Scale;
					Weight *= Weighting;
				}
				//NewColor = tex2D(_Previous,i.uv+float2(1,1));
				return DoAlphaModeStuff(OldColor, NewColor);
			}
			ENDCG
		}
	}
}
