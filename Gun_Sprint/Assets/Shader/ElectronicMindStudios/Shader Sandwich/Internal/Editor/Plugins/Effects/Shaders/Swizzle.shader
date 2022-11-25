// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/Effects/Swizzle"{
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
			float Channel_R;
			float Channel_G;
			float Channel_B;
			float Channel_A;

			float4 frag (v2f i) : SV_Target{
				float4 OldColor = tex2D(_Previous,i.uv);
				float4 NewColor = OldColor;
				if (Channel_R==1)
					NewColor.r = OldColor.g;
				if (Channel_R==2)
					NewColor.r = OldColor.b;
				if (Channel_R==3)
					NewColor.r = OldColor.a;
					
				if (Channel_G==0)
					NewColor.g = OldColor.r;
				if (Channel_G==2)
					NewColor.g = OldColor.b;
				if (Channel_G==3)
					NewColor.g = OldColor.a;
					
				if (Channel_B==0)
					NewColor.b = OldColor.r;
				if (Channel_B==1)
					NewColor.b = OldColor.g;
				if (Channel_B==3)
					NewColor.b = OldColor.a;
					
				if (Channel_A==0)
					NewColor.a = OldColor.r;
				if (Channel_A==1)
					NewColor.a = OldColor.g;
				if (Channel_A==2)
					NewColor.a = OldColor.b;
				return DoAlphaModeStuff(OldColor, NewColor);
			}
			ENDCG
		}
	}
}
