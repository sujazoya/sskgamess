// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/Types/CloudNoise"{
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
			
float Unique1D(float t){
	return frac(sin(dot(t ,12.9898)) * 43758.5453);
}
void Unique1DFastHashRewrite(float2 Pos,out float4 hash_0){
	float2 Offset = float2(26,161);
	float4 P = float4(Pos.xy,Pos.xy+1);
	P = P-floor(P*(1.0/71))*71;
	P += Offset.xyxy;
	P *= P;
	P = P.xzxz*P.yyww;
	hash_0 = frac(P*(1/951.135664));
}
float Unique2D(float2 t){
	float x = frac(sin(dot(floor(t) ,float2(12.9898,78.233))) * 43758.5453);
	return x;
}
float Lerp2D(float2 P, float Col1,float Col2,float Col3,float Col4){
	float2 ft = P * 3.1415927;
	float2 f = (1 - cos(ft)) * 0.5;
	P = f;
	float S1 = lerp(Col1,Col2,P.x);
	float S2 = lerp(Col3,Col4,P.x);
	float L = lerp(S1,S2,P.y);
	return L;
}
float NoiseCloud2D(float2 P){
	float4 HashX;
	Unique1DFastHashRewrite(floor(P),HashX);
	float xx = Lerp2D(frac(P),HashX.x,HashX.y,HashX.z,HashX.w);
	return xx;
}
float Lerp2DNoCos(float2 P, float Col1,float Col2,float Col3,float Col4){
	float S1 = lerp(Col1,Col2,P.x);
	float S2 = lerp(Col3,Col4,P.x);
	float L = lerp(S1,S2,P.y);
	return L;
}
float NoiseCloud2DNoCos(float2 P){
	float4 HashX;
	Unique1DFastHashRewrite(floor(P),HashX);
	float xx = Lerp2DNoCos(frac(P),HashX.x,HashX.y,HashX.z,HashX.w);
	return xx;
}

float NoiseCloud1D(float P){
	float ft = frac(P) * 3.1415927;
	float f = (1 - cos(ft)) * 0.5;
	P = floor(P);
	float SS = Unique1D(P);
	float SE = Unique1D(P+1);
	return lerp(SS,SE,f);//SS+((SE-SS)*f);
}
float NoiseCloud1DNoCos(float P){
	float f = frac(P);
	P = floor(P);
	float SS = Unique1D(P);
	float SE = Unique1D(P+1);
	
	return lerp(SS,SE,f);//SS+((SE-SS)*f);
}
float Unique3D(float3 t){
	float x = frac(tan(dot(tan(floor(t)),float3(12.9898,78.233,35.344))) * 9.5453);
	return x;
}

float Lerp3D(float3 P, float SSS,float SES,float ESS,float EES, float SSE,float SEE,float ESE,float EEE){
	float3 ft = P * 3.1415927;
	float3 f = (1 - cos(ft)) * 0.5;
	float S1 = lerp(SSS,SES,f.x);
	float S2 = lerp(ESS,EES,f.x);
	float F1 = lerp(S1,S2,f.y);
	float S3 = lerp(SSE,SEE,f.x);
	float S4 = lerp(ESE,EEE,f.x);
	float F2 = lerp(S3,S4,f.y);
	float L = lerp(F1,F2,f.z);//F1;
	return L;
}
float Lerp3DNoCos(float3 P, float SSS,float SES,float ESS,float EES, float SSE,float SEE,float ESE,float EEE){
	float3 f = P;
	float S1 = lerp(SSS,SES,f.x);
	float S2 = lerp(ESS,EES,f.x);
	float F1 = lerp(S1,S2,f.y);
	float S3 = lerp(SSE,SEE,f.x);
	float S4 = lerp(ESE,EEE,f.x);
	float F2 = lerp(S3,S4,f.y);
	float L = lerp(F1,F2,f.z);//F1;
	return L;
}
float NoiseCloud3D(float3 P){
	float SSS = Unique3D(P+float3(0,0,0));
	float SES = Unique3D(P+float3(1,0,0));
	float ESS = Unique3D(P+float3(0,1,0));
	float EES = Unique3D(P+float3(1,1,0));
	float SSE = Unique3D(P+float3(0,0,1));
	float SEE = Unique3D(P+float3(1,0,1));
	float ESE = Unique3D(P+float3(0,1,1));
	float EEE = Unique3D(P+float3(1,1,1));
	float xx = Lerp3D(frac(P),SSS,SES,ESS,EES,SSE,SEE,ESE,EEE);
	return xx;
}
float NoiseCloud3DNoCos(float3 P){
	float SSS = Unique3D(P+float3(0,0,0));
	float SES = Unique3D(P+float3(1,0,0));
	float ESS = Unique3D(P+float3(0,1,0));
	float EES = Unique3D(P+float3(1,1,0));
	float SSE = Unique3D(P+float3(0,0,1));
	float SEE = Unique3D(P+float3(1,0,1));
	float ESE = Unique3D(P+float3(0,1,1));
	float EEE = Unique3D(P+float3(1,1,1));
	float xx = Lerp3DNoCos(frac(P),SSS,SES,ESS,EES,SSE,SEE,ESE,EEE);
	return xx;
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
			
			sampler2D _UVs;
			float Weird_Noise_Dimensions;
			float Smoother;
			float Gamma_Correct;

			float4 frag (v2f i) : SV_Target{
				float3 UV = tex2D(_UVs,i.uv);
				float4 PixCol = 0;
				if (Smoother){
					if (Weird_Noise_Dimensions==0)
						PixCol = NoiseCloud1D(UV*3);
					if (Weird_Noise_Dimensions==1)
						PixCol = NoiseCloud2D(UV*3);
					if (Weird_Noise_Dimensions==2)
						PixCol = NoiseCloud3D(UV*3);
				}else{
					if (Weird_Noise_Dimensions==0)
						PixCol = NoiseCloud1DNoCos(UV*3);
					if (Weird_Noise_Dimensions==1)
						PixCol = NoiseCloud2DNoCos(UV*3);
					if (Weird_Noise_Dimensions==2)
						PixCol = NoiseCloud3DNoCos(UV*3);
				}
				#ifndef UNITY_COLORSPACE_GAMMA
					if (Gamma_Correct)PixCol.rgb *= PixCol.rgb;
				#endif
				return PixCol;
			}
			ENDCG
		}
	}
}
