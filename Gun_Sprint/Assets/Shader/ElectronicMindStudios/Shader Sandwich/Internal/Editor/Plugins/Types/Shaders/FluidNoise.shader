// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ShaderSandwich/Previews/Types/FluidNoise"{
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
			
sampler2D _ShaderSandwichPerlinTexture;
//Some noise code based on the fantastic library by Brian Sharpe, he deserves a ton of credit :)
//brisharpe CIRCLE_A yahoo DOT com
//http://briansharpe.wordpress.com
//https://github.com/BrianSharpe
float2 PerlinInterpolation_C2( float2 x ) { return x * x * x * (x * (x * 6.0 - 15.0) + 10.0); }
void PerlinFastHash2D(float2 Pos,out float4 hash_0, out float4 hash_1){
	float2 Offset = float2(26,161);
	float Domain = 71;
	float2 SomeLargeFloats = float2(951.135664,642.9478304);
	float4 P = float4(Pos.xy,Pos.xy+1);
	P = P-floor(P*(1.0/Domain))*Domain;
	P += Offset.xyxy;
	P *= P;
	P = P.xzxz*P.yyww;
	hash_0 = frac(P*(1/SomeLargeFloats.x));
	hash_1 = frac(P*(1/SomeLargeFloats.y));
}
float FluidNoise2D(float3 P){
	float2 Pi = floor(P);
	float4 Pf_Pfmin1 = P.xyxy-float4(Pi,Pi+1);
	float4 HashX, HashY;
	PerlinFastHash2D(Pi,HashX,HashY);
	
	float4 GradX = HashX-0.499999;
	float4 GradY = HashY-0.499999;
	
	float2 scx = half2(sin(P.z),cos(P.z));
	float2 scy = half2(scx.y,-scx.x);
	
	float4 GradX2 = GradX*scy.x;
	float4 GradX3 = GradX*scx.x;
	float4 GradY2 = GradY*scy.y;
	float4 GradY3 = GradY*scx.y;
	
	GradX = GradX2+GradY2;
	GradY = GradX3+GradY3;
	
	float4 GradRes = rsqrt(GradX*GradX+GradY*GradY+0.00001)*(GradX*Pf_Pfmin1.xzxz+GradY*Pf_Pfmin1.yyww);
	
	GradRes *= 1.4142135623730950488016887242097;
	
	float2 blend = PerlinInterpolation_C2(Pf_Pfmin1.xy);
	float4 blend2 = float4(blend,float2(1.0-blend));
	
	return (dot(GradRes,blend2.zxzx*blend2.wwyy));
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
			float Swirl;
			float Gamma_Correct;

			float4 frag (v2f i) : SV_Target{
				float3 UV = tex2D(_UVs,i.uv);//This feels so dirty...
				float4 PixCol = 0;
					if (Noise_Dimensions==0)
						PixCol = FluidNoise2D(float3(UV.xy*3,Swirl));
					if (Noise_Dimensions==1)
						PixCol = FluidNoise2D(UV*3);
					PixCol = (PixCol+1)/2;
				#ifndef UNITY_COLORSPACE_GAMMA
					if (Gamma_Correct)PixCol.rgb *= PixCol.rgb;
				#endif
				return PixCol;
			}
			ENDCG
		}
	}
}
