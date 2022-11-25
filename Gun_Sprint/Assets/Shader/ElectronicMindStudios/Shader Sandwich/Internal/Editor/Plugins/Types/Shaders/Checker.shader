// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/Types/Checker"{
	Properties{
		Color_A ("Color",Color) = (1,1,1,1)
		Color_B ("Color",Color) = (1,1,1,1)
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
			float4 Color_2;
			float Weird_Noise_Dimensions;
			
half4 Checker1D(float UV, half4 ColA, half4 ColB){
	if ((floor(UV.x))%2.0 < 1)
		return ColA;
	else
		return ColB;
}
			
half4 Checker2D(float2 UV, half4 ColA, half4 ColB){
	if ((floor(UV.x) + floor(UV.y))%2.0 < 1)
		return ColA;
	else
		return ColB;
}
			
half4 Checker3D(float3 UV, half4 ColA, half4 ColB){
	if ((floor(UV.x) + floor(UV.y) + floor(UV.z))%2.0 < 1)
		return ColA;
	else
		return ColB;
}

			float4 frag (v2f i) : SV_Target{
				float4 UV = tex2D(_UVs,i.uv);
				
				if (Weird_Noise_Dimensions==0)
					return Checker1D(UV.x,_Color,Color_2);
				if (Weird_Noise_Dimensions==1)
					return Checker2D(UV.xy,_Color,Color_2);
				if (Weird_Noise_Dimensions==2)
					return Checker3D(UV.xyz,_Color,Color_2);
					
				return 0;
			}
			ENDCG
		}
	}
}
