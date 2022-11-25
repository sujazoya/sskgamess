// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/Effects/Maths/Distance"{
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
			float Dimensions;
			float X;
			float Y;
			float Z;
			float W;

			float4 frag (v2f i) : SV_Target{
				float4 OldColor = tex2D(_Previous,i.uv);
				if (Dimensions == 0)
					return DoAlphaModeStuff(OldColor,distance(OldColor.x,X));
				if (Dimensions == 1)
					return DoAlphaModeStuff(OldColor,distance(OldColor.xy,float2(X,Y)));
				if (Dimensions == 2)
					return DoAlphaModeStuff(OldColor,distance(OldColor.xyz,float3(X,Y,Z)));
				if (Dimensions == 3)
					return DoAlphaModeStuff(OldColor,distance(OldColor.xyzw,float4(X,Y,Z,W)));
					
				return OldColor;
			}
			ENDCG
		}
	}
}
