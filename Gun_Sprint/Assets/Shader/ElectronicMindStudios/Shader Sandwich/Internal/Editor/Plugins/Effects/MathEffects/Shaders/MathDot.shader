// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/Effects/Maths/Dot"{
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
			float R;
			float G;
			float B;
			float A;

			float4 frag (v2f i) : SV_Target{
				float4 OldColor = tex2D(_Previous,i.uv);
				if (_AlphaMode==0)
					return DoAlphaModeStuff(OldColor,dot(OldColor.xyz,float3(R,G,B)));
				else if (_AlphaMode==1)
					return DoAlphaModeStuff(OldColor,dot(OldColor,float4(R,G,B,A)));
				else if (_AlphaMode==2)
					return DoAlphaModeStuff(OldColor,dot(OldColor.a,A));
				
				return OldColor;
			}
			ENDCG
		}
	}
}
