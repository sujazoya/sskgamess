// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/Effects/ColorSplit"{
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
			float4 R_Vector;
			float4 G_Vector;
			float4 B_Vector;
			float4 A_Vector;

			float4 frag (v2f i) : SV_Target{
				float4 OldColor = tex2D(_Previous,i.uv);
				float4 OldColorR = tex2D(_Previous,i.uv+R_Vector);
				float4 OldColorG = tex2D(_Previous,i.uv+G_Vector);
				float4 OldColorB = tex2D(_Previous,i.uv+B_Vector);
				float4 OldColorA = tex2D(_Previous,i.uv+A_Vector);
				
				return DoAlphaModeStuff(OldColor, float4(OldColorR.r,OldColorG.g,OldColorB.b,OldColorA.a));
			}
			ENDCG
		}
	}
}
