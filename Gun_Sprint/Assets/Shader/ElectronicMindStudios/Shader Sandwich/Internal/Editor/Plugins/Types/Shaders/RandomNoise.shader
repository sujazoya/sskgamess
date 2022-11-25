// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/Types/RandomNoise"{
	Properties{}
	SubShader{
		Cull Off ZWrite Off ZTest Always
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
			#pragma target 3.0
			
			#include "UnityCG.cginc"
			
	float RandomNoise3D(float3 co){
		return frac(sin( dot(co.xyz ,float3(12.9898,78.233,45.5432) )) * 43758.5453);
	}
	float RandomNoise2D(float2 co){
		return frac(sin( dot(co.xy ,float2(12.9898,78.233) )) * 43758.5453);
	}

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
			
			float3 _SSSTime;
			sampler2D _UVs;
			float Noise_Dimensions;
			float Gamma_Correct;

			float4 frag (v2f i) : SV_Target{
				float3 UV = tex2D(_UVs,i.uv);//This feels so dirty...
				float4 PixCol = 0;
					if (Noise_Dimensions==0)
						PixCol = RandomNoise2D(UV);
					if (Noise_Dimensions==1)
						PixCol = RandomNoise3D(UV);
				#ifndef UNITY_COLORSPACE_GAMMA
					if (Gamma_Correct)PixCol.rgb *= PixCol.rgb;
				#endif
				return PixCol;
			}
			ENDCG
		}
	}
}
