// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader Sandwich/Specific/Hologram Legacy (More flashy)" {//The Shaders Name
//The inputs shown in the material panel
Properties {
	_Rim_Color ("Rim Color", Color) = (0,0.5527849,1,1)
	_Dust_Color ("Dust Color", Color) = (0.3647059,0,0.5960785,1)
	_Sparkles_Color ("Sparkles Color", Color) = (0.01960784,0.1058824,1,1)
	_Squares_Color ("Squares Color", Color) = (1,0.3921569,0,1)
	_Lines_Color ("Lines Color", Color) = (1,0.4745098,0,1)
	_Force_Flicker ("Force Flicker", Range(0.000000000,1.000000000)) = 0.049382720
	_Flicker_Direction ("Flicker Direction", Color) = (0.3161765,0,0.3161765,1)
	_Pull_Height ("Pull Height", Float) = 0.700000000
	_Pull_Direction ("Pull Direction", Color) = (0,1,0,1)
}

SubShader {
	Tags { "RenderType"="Opaque" "Queue"="Transparent" }//A bunch of settings telling Unity a bit about the shader.
	LOD 200
AlphaToMask Off
	Pass {
		Name "ZWritePrePass"
		Tags { }
	ZTest LEqual
	ZWrite On
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Back//Culling specifies which sides of the models faces to hide.
	ColorMask 0

		
		CGPROGRAM
			// compile directives
				#pragma vertex Vertex
				#pragma fragment Pixel
				#pragma target 3.0
				#pragma multi_compile_fog
				#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
				#include "HLSLSupport.cginc"
				#include "UnityShaderVariables.cginc"
				#define SHADERSANDWICH_ZWRITEPREPASS
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "UnityPBSLighting.cginc"
				#include "AutoLight.cginc"

				#define INTERNAL_DATA
				#define WorldReflectionVector(data,normal) data.worldRefl
				#define WorldNormalVector(data,normal) normal
			//Make our inputs accessible by declaring them here.
				float4 _Rim_Color;
				float4 _Dust_Color;
				float4 _Sparkles_Color;
				float4 _Squares_Color;
				float4 _Lines_Color;
				float _Force_Flicker;
				float4 _Flicker_Direction;
				float _Pull_Height;
				float4 _Pull_Direction;

#if !defined (SSUNITY_BRDF_PBS) // allow to explicitly override BRDF in custom shader
	// still add safe net for low shader models, otherwise we might end up with shaders failing to compile
	#if SHADER_TARGET < 30
		#define SSUNITY_BRDF_PBS 3
	#elif defined(UNITY_PBS_USE_BRDF3)
		#define SSUNITY_BRDF_PBS 3
	#elif defined(UNITY_PBS_USE_BRDF2)
		#define SSUNITY_BRDF_PBS 2
	#elif defined(UNITY_PBS_USE_BRDF1)
		#define SSUNITY_BRDF_PBS 1
	#elif defined(SHADER_TARGET_SURFACE_ANALYSIS)
		// we do preprocess pass during shader analysis and we dont actually care about brdf as we need only inputs/outputs
		#define SSUNITY_BRDF_PBS 1
	#else
		#error something broke in auto-choosing BRDF (Shader Sandwich)
	#endif
#endif

//Some noise code based on the fantastic library by Brian Sharpe, he deserves a ton of credit :)
//brisharpe CIRCLE_A yahoo DOT com
//http://briansharpe.wordpress.com
//https://github.com/BrianSharpe

float3 PerlinInterpolation_C2( float3 x ) { return x * x * x * (x * (x * 6.0 - 15.0) + 10.0); }

void PerlinFastHash3D(float3 Pos,out float4 hash_0, out float4 hash_1,out float4 hash_2, out float4 hash_3,out float4 hash_4, out float4 hash_5){
	float2 Offset = float2(50,161);
	float Domain = 69;
	float3 SomeLargeFloats = float3(635.298681, 682.357502, 668.926525 );
	float3 Zinc = float3( 48.500388, 65.294118, 63.934599 );
	
	Pos = Pos-floor(Pos*(1.0/Domain))*Domain;
	float3 Pos_Inc1 = step(Pos,float(Domain-1.5).rrr)*(Pos+1);
	
	float4 P = float4(Pos.xy,Pos_Inc1.xy)+Offset.xyxy;
	P *= P;
	P = P.xzxz*P.yyww;
	
	float3 lowz_mod = float3(1/(SomeLargeFloats+Pos.zzz*Zinc));//Pos.zzz
	float3 highz_mod = float3(1/(SomeLargeFloats+Pos_Inc1.zzz*Zinc));//Pos_Inc1.zzz
	
	hash_0 = frac(P*lowz_mod.xxxx);
	hash_1 = frac(P*lowz_mod.yyyy);
	hash_2 = frac(P*lowz_mod.zzzz);
	hash_3 = frac(P*highz_mod.xxxx);
	hash_4 = frac(P*highz_mod.yyyy);
	hash_5 = frac(P*highz_mod.zzzz);
}
float PerlinNoise3D(float3 P)
{
	float3 Pi = floor(P);
	float3 Pf = P-Pi;
	float3 Pf_min1 = Pf-1.0;
	
	float4 HashX0, HashY0, HashZ0, HashX1, HashY1, HashZ1;
	PerlinFastHash3D(Pi, HashX0, HashY0, HashZ0, HashX1, HashY1, HashZ1);
	
	float4 GradX0 = HashX0-0.49999999;
	float4 GradX1 = HashX1-0.49999999;
	float4 GradY0 = HashY0-0.49999999;
	float4 GradY1 = HashY1-0.49999999;
	float4 GradZ0 = HashZ0-0.49999999;
	float4 GradZ1 = HashZ1-0.49999999;

	float4 GradRes = rsqrt( GradX0 * GradX0 + GradY0 * GradY0 + GradZ0 * GradZ0+0.00001) * ( float2( Pf.x, Pf_min1.x ).xyxy * GradX0 + float2( Pf.y, Pf_min1.y ).xxyy * GradY0 + Pf.zzzz * GradZ0 );
	float4 GradRes2 = rsqrt( GradX1 * GradX1 + GradY1 * GradY1 + GradZ1 * GradZ1+0.00001) * ( float2( Pf.x, Pf_min1.x ).xyxy * GradX1 + float2( Pf.y, Pf_min1.y ).xxyy * GradY1 + Pf_min1.zzzz * GradZ1 );
	
	float3 Blend = PerlinInterpolation_C2(Pf);
	
	float4 Res = lerp(GradRes,GradRes2,Blend.z);
	float4 Blend2 = float4(Blend.xy,float2(1.0-Blend.xy));
	float Final = dot(Res,Blend2.zxzx*Blend2.wwyy);
	Final *= 1.1547005383792515290182975610039;
	return Final;
}

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

float PerlinNoise2D(float2 P){
	float2 Pi = floor(P);
	float4 Pf_Pfmin1 = P.xyxy-float4(Pi,Pi+1);
	float4 HashX, HashY;
	PerlinFastHash2D(Pi,HashX,HashY);
	float4 GradX = HashX-0.499999;
	float4 GradY = HashY-0.499999;
	float4 GradRes = rsqrt(GradX*GradX+GradY*GradY+0.00001)*(GradX*Pf_Pfmin1.xzxz+GradY*Pf_Pfmin1.yyww);
	
	GradRes *= 1.4142135623730950488016887242097;
	float2 blend = PerlinInterpolation_C2(Pf_Pfmin1.xy);
	float4 blend2 = float4(blend,float2(1.0-blend));
	return (dot(GradRes,blend2.zxzx*blend2.wwyy));
}

	//From UnityCG.inc, Unity 2017.01 - works better than any of the earlier ones
	inline float3 UnityObjectToWorldNormalNew( in float3 norm ){
		#ifdef UNITY_ASSUME_UNIFORM_SCALING
			return UnityObjectToWorldDir(norm);
		#else
			// mul(IT_M, norm) => mul(norm, I_M) => {dot(norm, I_M.col0), dot(norm, I_M.col1), dot(norm, I_M.col2)}
			return normalize(mul(norm, (float3x3)unity_WorldToObject));
		#endif
	}
	float4 GammaToLinear(float4 col){
		#if defined(UNITY_COLORSPACE_GAMMA)
			//Best programming evar XD
		#else
			col.rgb = pow(col,2.2);
		#endif
		return col;
	}

	float4 GammaToLinearForce(float4 col){
		#if defined(UNITY_COLORSPACE_GAMMA)
			//Best programming evar XD
		#else
			col.rgb = pow(col,2.2);
		#endif
		return col;
	}

	float4 LinearToGamma(float4 col){
		return col;
	}

	float4 LinearToGammaForWeirdSituations(float4 col){
		#if defined(UNITY_COLORSPACE_GAMMA)
			col.rgb = pow(col,2.2);
		#endif
		return col;
	}

	float3 GammaToLinear(float3 col){
		#if defined(UNITY_COLORSPACE_GAMMA)
			//Best programming evar XD
		#else
			col = pow(col,2.2);
		#endif
		return col;
	}

	float3 GammaToLinearForce(float3 col){
		#if defined(UNITY_COLORSPACE_GAMMA)
			//Best programming evar XD
		#else
			col = pow(col,2.2);
		#endif
		return col;
	}

	float3 LinearToGamma(float3 col){
		return col;
	}

	float GammaToLinear(float col){
		#if defined(UNITY_COLORSPACE_GAMMA)
			//Best programming evar XD
		#else
			col = pow(col,2.2);
		#endif
		return col;
	}







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
	return lerp(SS,SE,f);
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


































struct VertexShaderInput{
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float4 texcoord : TEXCOORD0;
};
struct VertexToPixel{
	float3 worldPos : TEXCOORD0;
	float4 position : POSITION;
	float3 worldNormal : TEXCOORD1;
	float2 genericTexcoord : TEXCOORD2;
	#define pos position
		UNITY_FOG_COORDS(3)
	#undef pos
};

struct VertexData{
	float3 worldPos;
	float4 position;
	float3 worldNormal;
	float3 worldViewDir;
	float2 genericTexcoord;
	float Mask0;
	float Mask1;
	float Mask2;
	float Mask3;
	float Mask4;
	float Mask5;
	float Mask6;
	float Mask7;
	float4 Mask8;
	float Atten;
};
//OutputPremultiplied: False
//UseAlphaGenerate: True
half4 Specular (){
	half3 Surface = half4(0,0,0,1);//Specular Mode
		//Generate layers for the Specular Color channel.
			//Generate Layer: Specular Color
				//Sample parts of the layer:
					half4 Specular_ColorSpecular_Sample1 = GammaToLinear(float4(0, 0.484, 1, 1));
	
	Surface = Specular_ColorSpecular_Sample1.rgb;//0
	
	
	half reflectivity = SpecularStrength(Surface);
	half oneMinusReflectivity = 1-reflectivity;
	return half4(Surface,reflectivity);
	
}
//OutputPremultiplied: False
//UseAlphaGenerate: True
half4 Set_Alpha_Channel ( VertexData vd, half4 outputColor){
	float Surface = 0;
		//Generate layers for the Transparency channel.
			//Generate Layer: Alpha
				//Sample parts of the layer:
					half4 AlphaTransparency_Sample1 = float4(float3((1-dot(vd.worldNormal, vd.worldViewDir)).rrr),1);
	
	Surface = AlphaTransparency_Sample1.r;//2
	
	
			//Generate Layer: Sparkles
				//Sample parts of the layer:
					half4 SparklesTransparency_Sample1 = 1;
	
	Surface = lerp(Surface,(Surface + SparklesTransparency_Sample1.r),vd.Mask2);//1
	
	
			//Generate Layer: Squares
				//Sample parts of the layer:
					half4 SquaresTransparency_Sample1 = 1;
	
	Surface = lerp(Surface,(Surface + SquaresTransparency_Sample1.r),vd.Mask3);//1
	
	
	
	return float4(outputColor.rgb,Surface);
	
}
float4 Transparency ( float4 outputColor){
	outputColor.a *= 1;
	return outputColor;
	
}
void Displace ( inout VertexData vd, inout VertexShaderInput v){
	half3 Surface = half3(1,1,1);
		//Generate layers for the Displacement channel.
			//Generate Layer: Displacement
				//Sample parts of the layer:
					half4 DisplacementSurface_Sample1 = 0;
	
	Surface = DisplacementSurface_Sample1.rgb;//0
	
	
			//Generate Layer: Displacement2
				//Sample parts of the layer:
					half4 Displacement2Surface_Sample1 = _Flicker_Direction;
	
	Surface = lerp(Surface,(Surface + Displacement2Surface_Sample1.rgb),vd.Mask7);//1
	
	
	
	v.vertex.xyz = float3(v.vertex.xyz + v.normal * Surface * (-1));
	
}
void Set ( inout VertexData vd, inout VertexShaderInput v){
	float3 Surface = v.vertex.xyz;
		//Generate layers for the Vertex channel.
			//Generate Layer: Vertex
				//Sample parts of the layer:
					half4 VertexSurface_Sample1 = _Pull_Direction;
	
				//Apply Effects:
					VertexSurface_Sample1.rgb = (VertexSurface_Sample1.rgb*vd.Mask8.rgb);
	
	Surface = lerp(Surface,VertexSurface_Sample1.rgb,vd.Mask5);//1
	
	
	
	v.vertex.xyz = Surface;
	
}
float Mask_Mask0 ( VertexData vd){
		//Set default mask color
			float Mask0 = 0;
		//Generate layers for the Rim channel.
			//Generate Layer: Rim 2
				//Sample parts of the layer:
					half4 Rim_2Mask0_Sample1 = float4(float3(1-float3(dot(vd.worldNormal, vd.worldViewDir).rrr).x,float3(dot(vd.worldNormal, vd.worldViewDir).rrr).y,float3(dot(vd.worldNormal, vd.worldViewDir).rrr).z),1);
	
				//Apply Effects:
					Rim_2Mask0_Sample1.rgb = pow(Rim_2Mask0_Sample1.rgb,4);
	
	Mask0 = Rim_2Mask0_Sample1.r;//2
	
	
	return Mask0;
	
}
float Mask_Mask1 ( VertexData vd){
		//Set default mask color
			float Mask1 = 0;
		//Generate layers for the Dust channel.
			//Generate Layer: Mask1
				//Sample parts of the layer:
					half4 Mask1Mask1_Sample1 = GammaToLinear(NoiseCloud3D(((vd.worldPos*float3(30,30,30))+float3(0,0,_Time.y * 0.5))*3));
	
	Mask1 = Mask1Mask1_Sample1.r;//2
	
	
	return Mask1;
	
}
float Mask_Mask2 ( VertexData vd){
		//Set default mask color
			float Mask2 = 0;
		//Generate layers for the Sparkles channel.
			//Generate Layer: Mask1 Copy
				//Sample parts of the layer:
					half4 Mask1_CopyMask2_Sample1 = GammaToLinear((PerlinNoise3D(((vd.worldPos*float3(300,300,300))+float3(0,0,_Time.y * 0.5))*3)+1)/2);
	
	Mask2 = Mask1_CopyMask2_Sample1.r;//2
	
	
			//Generate Layer: Mask1 Copy Copy 2
				//Sample parts of the layer:
					half4 Mask1_Copy_Copy_2Mask2_Sample1 = GammaToLinear(NoiseCloud3D(((vd.worldPos*float3(30,30,30))+float3(0,0,_Time.y * 0.5))*3));
	
	Mask2 = (Mask2 * Mask1_Copy_Copy_2Mask2_Sample1.r);//0
	
	
			//Generate Layer: Sparkles 2
				//Sample parts of the layer:
					half4 Sparkles_2Mask2_Sample1 = float4(Mask2.rrrr);
	
				//Apply Effects:
					Sparkles_2Mask2_Sample1.rgb = (round(Sparkles_2Mask2_Sample1.rgb*1)/1);
	
	Mask2 = Sparkles_2Mask2_Sample1.r;//0
	
	
	return Mask2;
	
}
float Mask_Mask3 ( VertexData vd){
		//Set default mask color
			float Mask3 = 0;
		//Generate layers for the Squares channel.
			//Generate Layer: Mask1 Copy Copy
				//Sample parts of the layer:
					half4 Mask1_Copy_CopyMask3_Sample1 = GammaToLinear((PerlinNoise3D((((round(vd.worldPos/float3(0.1250099,0.1250099,0.1250099))*float3(0.1250099,0.1250099,0.1250099))*float3(1.5,1.5,1.5))+float3(0,0,_Time.y * 0.5))*3)+1)/2);
	
				//Apply Effects:
					Mask1_Copy_CopyMask3_Sample1.rgb = pow(Mask1_Copy_CopyMask3_Sample1.rgb,2);
	
	Mask3 = Mask1_Copy_CopyMask3_Sample1.r;//2
	
	
	return Mask3;
	
}
float Mask_Mask4 ( VertexData vd){
		//Set default mask color
			float Mask4 = 0;
		//Generate layers for the Lines channel.
			//Generate Layer: Lines 2
				//Sample parts of the layer:
					half4 Lines_2Mask4_Sample1 = float4(((mul(unity_WorldToObject, float4(vd.worldPos,1)).xyz+float3(_Time.y * 0.5,0,0))*float3(50,50,50)),1);
	
				//Apply Effects:
					Lines_2Mask4_Sample1.rgb = ((sin(Lines_2Mask4_Sample1.rgb)+1)/2);
	
	Mask4 = Lines_2Mask4_Sample1.r;//2
	
	
			//Generate Layer: Lines Copy 2
				//Sample parts of the layer:
					half4 Lines_Copy_2Mask4_Sample1 = float4(((mul(unity_WorldToObject, float4(vd.worldPos,1)).xyz+float3(_Time.y * 0.5,0,0))*float3(20,20,20)),1);
	
				//Apply Effects:
					Lines_Copy_2Mask4_Sample1.rgb = sin(Lines_Copy_2Mask4_Sample1.rgb);
	
	Mask4 = (Mask4 * Lines_Copy_2Mask4_Sample1.r);//0
	
	
			//Generate Layer: Lines2
				//Sample parts of the layer:
					half4 Lines2Mask4_Sample1 = float4(Mask4.rrrr);
	
				//Apply Effects:
					Lines2Mask4_Sample1.rgb = clamp(Lines2Mask4_Sample1.rgb,0,1);
					Lines2Mask4_Sample1.rgb = pow(Lines2Mask4_Sample1.rgb,6);
	
	Mask4 = Lines2Mask4_Sample1.r;//0
	
	
	return Mask4;
	
}
float Mask_Mask5 ( VertexData vd){
		//Set default mask color
			float Mask5 = 0;
		//Generate layers for the Pull channel.
			//Generate Layer: Pull
				//Sample parts of the layer:
					half4 PullMask5_Sample1 = float4(vd.worldPos,1);
	
				//Apply Effects:
					PullMask5_Sample1.rgb = (float3(1,1,1)-PullMask5_Sample1.rgb);
					PullMask5_Sample1.rgb = (PullMask5_Sample1.rgb-(_Pull_Height));
					PullMask5_Sample1.rgb = clamp(PullMask5_Sample1.rgb,0,1);
					PullMask5_Sample1.rgb = pow(PullMask5_Sample1.rgb,4);
	
	Mask5 = PullMask5_Sample1.g;//2
	
	
	return Mask5;
	
}
float Mask_Mask6 ( VertexData vd){
		//Set default mask color
			float Mask6 = 0;
		//Generate layers for the Flicker channel.
			//Generate Layer: Lines Copy
				//Sample parts of the layer:
					half4 Lines_CopyMask6_Sample1 = float4(((vd.worldPos*float3(0.1,0.1,0.1))+float3(_Time.y,0,0)),1);
	
				//Apply Effects:
					Lines_CopyMask6_Sample1.rgb = ((sin(Lines_CopyMask6_Sample1.rgb)+1)/2);
					Lines_CopyMask6_Sample1.rgb = pow(Lines_CopyMask6_Sample1.rgb,240);
					Lines_CopyMask6_Sample1.rgb = (round(Lines_CopyMask6_Sample1.rgb*1)/1);
	
	Mask6 = Lines_CopyMask6_Sample1.r;//2
	
	
			//Generate Layer: Flicker
				//Sample parts of the layer:
					half4 FlickerMask6_Sample1 = GammaToLinear(float4(1, 1, 1, 1));
	
	Mask6 = lerp(Mask6,FlickerMask6_Sample1.r,_Force_Flicker);//1
	
	
	return Mask6;
	
}
float Mask_Mask7 ( VertexData vd){
		//Set default mask color
			float Mask7 = 0;
		//Generate layers for the FlickerNoise channel.
			//Generate Layer: FlickerNoise
				//Sample parts of the layer:
					half4 FlickerNoiseMask7_Sample1 = GammaToLinear(float4(0, 0, 0, 1));
	
	Mask7 = FlickerNoiseMask7_Sample1.r;//2
	
	
			//Generate Layer: FlickerNoise2
				//Sample parts of the layer:
					half4 FlickerNoise2Mask7_Sample1 = (PerlinNoise2D(((vd.genericTexcoord+float2(_Time.y * 0.5,0))*float2(14.52,14.52))*3)+1)/2;
	
	Mask7 = lerp(Mask7,FlickerNoise2Mask7_Sample1.r,vd.Mask6);//1
	
	
	return Mask7;
	
}
float4 Mask_Mask8 ( VertexData vd){
		//Set default mask color
			float4 Mask8 = float4(0,0,0,0);
		//Generate layers for the Legacy Position Mask channel.
			//Generate Layer: Legacy Object Space Position
				//Sample parts of the layer:
					half4 Legacy_Object_Space_PositionMask8_Sample1 = float4(mul(unity_WorldToObject, float4(vd.worldPos,1)).xyz,1);
	
	Mask8 = half4(Legacy_Object_Space_PositionMask8_Sample1.rgb,1);//7
	
	
	return Mask8;
	
}
VertexToPixel Vertex (VertexShaderInput v){
	VertexToPixel vtp;
	UNITY_INITIALIZE_OUTPUT(VertexToPixel,vtp);
	VertexData vd;
	UNITY_INITIALIZE_OUTPUT(VertexData,vd);
	vd.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	vd.position = UnityObjectToClipPos(v.vertex);
	vd.worldNormal = UnityObjectToWorldNormalNew(v.normal);
	vd.genericTexcoord = v.texcoord;
	
	
vd.Mask5 = Mask_Mask5 ( vd);
vd.Mask6 = Mask_Mask6 ( vd);
vd.Mask7 = Mask_Mask7 ( vd);
vd.Mask8 = Mask_Mask8 ( vd);
	Displace ( vd, v);
	vd.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	vd.position = UnityObjectToClipPos(v.vertex);
	vd.worldNormal = UnityObjectToWorldNormalNew(v.normal);
	vd.genericTexcoord = v.texcoord;
	Set ( vd, v);
	vd.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	vd.position = UnityObjectToClipPos(v.vertex);
	vd.worldNormal = UnityObjectToWorldNormalNew(v.normal);
	vd.genericTexcoord = v.texcoord;
	
	
	vtp.worldPos = vd.worldPos;
	vtp.position = vd.position;
	vtp.worldNormal = vd.worldNormal;
	vtp.genericTexcoord = vd.genericTexcoord;
	#define pos position
	UNITY_TRANSFER_FOG(vtp,vtp.pos);
	#undef pos
	return vtp;
}
			half4 Pixel (VertexToPixel vtp) : SV_Target {
				half4 outputColor = half4(0,0,0,0);
				half3 outputNormal = half3(0,0,1);
				half3 depth = half3(0,0,0);//Tangent Corrected depth, World Space depth, Normalized depth
				half3 tempDepth = half3(0,0,0);
				VertexData vd;
				UNITY_INITIALIZE_OUTPUT(VertexData,vd);
				vd.worldPos = vtp.worldPos;
				vd.worldNormal = normalize(vtp.worldNormal);
				vd.worldViewDir = normalize(UnityWorldSpaceViewDir(vd.worldPos));
				vd.genericTexcoord = vtp.genericTexcoord;
				outputNormal = vd.worldNormal;
vd.Mask0 = Mask_Mask0 ( vd);
vd.Mask1 = Mask_Mask1 ( vd);
vd.Mask2 = Mask_Mask2 ( vd);
vd.Mask3 = Mask_Mask3 ( vd);
vd.Mask4 = Mask_Mask4 ( vd);
				half4 outputSpecular = Specular ();
				outputColor = ((outputColor) * (1 - (outputSpecular.a)) + (half4((outputSpecular.a * outputColor.rgb + outputSpecular.rgb), outputSpecular.a)));//1
								half4 outputSet_Alpha_Channel = Set_Alpha_Channel ( vd, outputColor);
				outputColor = outputSet_Alpha_Channel;//10
								outputColor = Transparency ( outputColor);
				UNITY_APPLY_FOG(vtp.fogCoord, outputColor); // apply fog (UNITY_FOG_COORDS));
				return 0;

			}
		ENDCG
	}
AlphaToMask Off
	Pass {
		Name "FORWARD"
		Tags { "LightMode" = "ForwardBase" }
	ZTest LEqual
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Back//Culling specifies which sides of the models faces to hide.

		
		CGPROGRAM
			// compile directives
				#pragma vertex Vertex
				#pragma fragment Pixel
				#pragma target 3.0
				#pragma multi_compile_fog
				#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
				#pragma multi_compile_fwdbase
				#include "HLSLSupport.cginc"
				#include "UnityShaderVariables.cginc"
				#define UNITY_PASS_FORWARDBASE
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "UnityPBSLighting.cginc"
				#include "AutoLight.cginc"

				#define INTERNAL_DATA
				#define WorldReflectionVector(data,normal) data.worldRefl
				#define WorldNormalVector(data,normal) normal
			//Make our inputs accessible by declaring them here.
				float4 _Rim_Color;
				float4 _Dust_Color;
				float4 _Sparkles_Color;
				float4 _Squares_Color;
				float4 _Lines_Color;
				float _Force_Flicker;
				float4 _Flicker_Direction;
				float _Pull_Height;
				float4 _Pull_Direction;

#if !defined (SSUNITY_BRDF_PBS) // allow to explicitly override BRDF in custom shader
	// still add safe net for low shader models, otherwise we might end up with shaders failing to compile
	#if SHADER_TARGET < 30
		#define SSUNITY_BRDF_PBS 3
	#elif defined(UNITY_PBS_USE_BRDF3)
		#define SSUNITY_BRDF_PBS 3
	#elif defined(UNITY_PBS_USE_BRDF2)
		#define SSUNITY_BRDF_PBS 2
	#elif defined(UNITY_PBS_USE_BRDF1)
		#define SSUNITY_BRDF_PBS 1
	#elif defined(SHADER_TARGET_SURFACE_ANALYSIS)
		// we do preprocess pass during shader analysis and we dont actually care about brdf as we need only inputs/outputs
		#define SSUNITY_BRDF_PBS 1
	#else
		#error something broke in auto-choosing BRDF (Shader Sandwich)
	#endif
#endif

inline half GGXTermNoFadeout (half NdotH, half realRoughness){
	half a2 = realRoughness * realRoughness;
	half d = (NdotH * a2 - NdotH) * NdotH + 1.0f; // 2 mad
	return a2 / (UNITY_PI * d * d);
}

//Some noise code based on the fantastic library by Brian Sharpe, he deserves a ton of credit :)
//brisharpe CIRCLE_A yahoo DOT com
//http://briansharpe.wordpress.com
//https://github.com/BrianSharpe

float3 PerlinInterpolation_C2( float3 x ) { return x * x * x * (x * (x * 6.0 - 15.0) + 10.0); }

void PerlinFastHash3D(float3 Pos,out float4 hash_0, out float4 hash_1,out float4 hash_2, out float4 hash_3,out float4 hash_4, out float4 hash_5){
	float2 Offset = float2(50,161);
	float Domain = 69;
	float3 SomeLargeFloats = float3(635.298681, 682.357502, 668.926525 );
	float3 Zinc = float3( 48.500388, 65.294118, 63.934599 );
	
	Pos = Pos-floor(Pos*(1.0/Domain))*Domain;
	float3 Pos_Inc1 = step(Pos,float(Domain-1.5).rrr)*(Pos+1);
	
	float4 P = float4(Pos.xy,Pos_Inc1.xy)+Offset.xyxy;
	P *= P;
	P = P.xzxz*P.yyww;
	
	float3 lowz_mod = float3(1/(SomeLargeFloats+Pos.zzz*Zinc));//Pos.zzz
	float3 highz_mod = float3(1/(SomeLargeFloats+Pos_Inc1.zzz*Zinc));//Pos_Inc1.zzz
	
	hash_0 = frac(P*lowz_mod.xxxx);
	hash_1 = frac(P*lowz_mod.yyyy);
	hash_2 = frac(P*lowz_mod.zzzz);
	hash_3 = frac(P*highz_mod.xxxx);
	hash_4 = frac(P*highz_mod.yyyy);
	hash_5 = frac(P*highz_mod.zzzz);
}
float PerlinNoise3D(float3 P)
{
	float3 Pi = floor(P);
	float3 Pf = P-Pi;
	float3 Pf_min1 = Pf-1.0;
	
	float4 HashX0, HashY0, HashZ0, HashX1, HashY1, HashZ1;
	PerlinFastHash3D(Pi, HashX0, HashY0, HashZ0, HashX1, HashY1, HashZ1);
	
	float4 GradX0 = HashX0-0.49999999;
	float4 GradX1 = HashX1-0.49999999;
	float4 GradY0 = HashY0-0.49999999;
	float4 GradY1 = HashY1-0.49999999;
	float4 GradZ0 = HashZ0-0.49999999;
	float4 GradZ1 = HashZ1-0.49999999;

	float4 GradRes = rsqrt( GradX0 * GradX0 + GradY0 * GradY0 + GradZ0 * GradZ0+0.00001) * ( float2( Pf.x, Pf_min1.x ).xyxy * GradX0 + float2( Pf.y, Pf_min1.y ).xxyy * GradY0 + Pf.zzzz * GradZ0 );
	float4 GradRes2 = rsqrt( GradX1 * GradX1 + GradY1 * GradY1 + GradZ1 * GradZ1+0.00001) * ( float2( Pf.x, Pf_min1.x ).xyxy * GradX1 + float2( Pf.y, Pf_min1.y ).xxyy * GradY1 + Pf_min1.zzzz * GradZ1 );
	
	float3 Blend = PerlinInterpolation_C2(Pf);
	
	float4 Res = lerp(GradRes,GradRes2,Blend.z);
	float4 Blend2 = float4(Blend.xy,float2(1.0-Blend.xy));
	float Final = dot(Res,Blend2.zxzx*Blend2.wwyy);
	Final *= 1.1547005383792515290182975610039;
	return Final;
}

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

float PerlinNoise2D(float2 P){
	float2 Pi = floor(P);
	float4 Pf_Pfmin1 = P.xyxy-float4(Pi,Pi+1);
	float4 HashX, HashY;
	PerlinFastHash2D(Pi,HashX,HashY);
	float4 GradX = HashX-0.499999;
	float4 GradY = HashY-0.499999;
	float4 GradRes = rsqrt(GradX*GradX+GradY*GradY+0.00001)*(GradX*Pf_Pfmin1.xzxz+GradY*Pf_Pfmin1.yyww);
	
	GradRes *= 1.4142135623730950488016887242097;
	float2 blend = PerlinInterpolation_C2(Pf_Pfmin1.xy);
	float4 blend2 = float4(blend,float2(1.0-blend));
	return (dot(GradRes,blend2.zxzx*blend2.wwyy));
}

	//From UnityCG.inc, Unity 2017.01 - works better than any of the earlier ones
	inline float3 UnityObjectToWorldNormalNew( in float3 norm ){
		#ifdef UNITY_ASSUME_UNIFORM_SCALING
			return UnityObjectToWorldDir(norm);
		#else
			// mul(IT_M, norm) => mul(norm, I_M) => {dot(norm, I_M.col0), dot(norm, I_M.col1), dot(norm, I_M.col2)}
			return normalize(mul(norm, (float3x3)unity_WorldToObject));
		#endif
	}
	float4 GammaToLinear(float4 col){
		#if defined(UNITY_COLORSPACE_GAMMA)
			//Best programming evar XD
		#else
			col.rgb = pow(col,2.2);
		#endif
		return col;
	}

	float4 GammaToLinearForce(float4 col){
		#if defined(UNITY_COLORSPACE_GAMMA)
			//Best programming evar XD
		#else
			col.rgb = pow(col,2.2);
		#endif
		return col;
	}

	float4 LinearToGamma(float4 col){
		return col;
	}

	float4 LinearToGammaForWeirdSituations(float4 col){
		#if defined(UNITY_COLORSPACE_GAMMA)
			col.rgb = pow(col,2.2);
		#endif
		return col;
	}

	float3 GammaToLinear(float3 col){
		#if defined(UNITY_COLORSPACE_GAMMA)
			//Best programming evar XD
		#else
			col = pow(col,2.2);
		#endif
		return col;
	}

	float3 GammaToLinearForce(float3 col){
		#if defined(UNITY_COLORSPACE_GAMMA)
			//Best programming evar XD
		#else
			col = pow(col,2.2);
		#endif
		return col;
	}

	float3 LinearToGamma(float3 col){
		return col;
	}

	float GammaToLinear(float col){
		#if defined(UNITY_COLORSPACE_GAMMA)
			//Best programming evar XD
		#else
			col = pow(col,2.2);
		#endif
		return col;
	}







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
	return lerp(SS,SE,f);
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


































struct VertexShaderInput{
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float4 texcoord : TEXCOORD0;
	float4 texcoord1 : TEXCOORD1;
	float4 texcoord2 : TEXCOORD2;
};
struct VertexToPixel{
	float3 worldPos : TEXCOORD0;
	float4 position : POSITION;
	float3 worldNormal : TEXCOORD1;
	float3 worldViewDir : TEXCOORD2;
	float2 genericTexcoord : TEXCOORD3;
	#if UNITY_SHOULD_SAMPLE_SH
		float3 sh : TEXCOORD4;
#endif
	#define pos position
		SHADOW_COORDS(5)
		UNITY_FOG_COORDS(6)
#undef pos
	#if (!defined(LIGHTMAP_OFF))||(SHADER_TARGET >= 30)
		float4 lmap : TEXCOORD7;
	#endif
};

struct VertexData{
	float3 worldPos;
	float4 position;
	float3 worldNormal;
	float3 worldViewDir;
	float3 worldRefl;
	float2 genericTexcoord;
	#if UNITY_SHOULD_SAMPLE_SH
		float3 sh;
#endif
	#if (!defined(LIGHTMAP_OFF))||(SHADER_TARGET >= 30)
		float4 lmap;
	#endif
	float Mask0;
	float Mask1;
	float Mask2;
	float Mask3;
	float Mask4;
	float Mask5;
	float Mask6;
	float Mask7;
	float4 Mask8;
	float Atten;
};
//OutputPremultiplied: False
//UseAlphaGenerate: True
half4 Emission ( VertexData vd){
	half4 Surface = half4(0,0,0,0);
		//Generate layers for the Emission channel.
			//Generate Layer: Rim
				//Sample parts of the layer:
					half4 RimEmission_Sample1 = _Rim_Color;
	
				//Apply Effects:
					RimEmission_Sample1.rgb = (RimEmission_Sample1.rgb*3);
	
	Surface = lerp(Surface,half4((Surface.rgb + RimEmission_Sample1.rgb), 1),vd.Mask0);//2
	
	
			//Generate Layer: Dust
				//Sample parts of the layer:
					half4 DustEmission_Sample1 = _Dust_Color;
	
	Surface = lerp(Surface,half4((Surface.rgb + DustEmission_Sample1.rgb), 1),vd.Mask1);//2
	
	
			//Generate Layer: Sparkles 2 2
				//Sample parts of the layer:
					half4 Sparkles_2_2Emission_Sample1 = _Sparkles_Color;
	
				//Apply Effects:
					Sparkles_2_2Emission_Sample1.rgb = (Sparkles_2_2Emission_Sample1.rgb*6);
	
	Surface = lerp(Surface,half4((Surface.rgb + Sparkles_2_2Emission_Sample1.rgb), 1),vd.Mask2);//2
	
	
			//Generate Layer: Squares 2
				//Sample parts of the layer:
					half4 Squares_2Emission_Sample1 = _Squares_Color;
	
	Surface = lerp(Surface,half4((Surface.rgb + Squares_2Emission_Sample1.rgb), 1),vd.Mask3);//2
	
	
			//Generate Layer: Lines
				//Sample parts of the layer:
					half4 LinesEmission_Sample1 = _Lines_Color;
	
	Surface = lerp(Surface,half4((Surface.rgb + LinesEmission_Sample1.rgb), 1),vd.Mask4);//2
	
	
	return Surface;
}
//OutputPremultiplied: False
//UseAlphaGenerate: True
half4 Specular ( VertexData vd, UnityGI gi, UnityGIInput giInput, out half OneMinusAlpha){
	half3 Surface = half4(0,0,0,1);//Specular Mode
		//Generate layers for the Specular Color channel.
			//Generate Layer: Specular Color
				//Sample parts of the layer:
					half4 Specular_ColorSpecular_Sample1 = GammaToLinear(float4(0, 0.484, 1, 1));
	
	Surface = Specular_ColorSpecular_Sample1.rgb;//0
	
	
	half oneMinusRoughness = 0.986;
	half perceptualRoughness = 1-oneMinusRoughness;
	half realRoughness = perceptualRoughness*perceptualRoughness;		// need to square perceptual roughness
	half reflectivity = SpecularStrength(Surface);
	half oneMinusReflectivity = 1-reflectivity;
	Unity_GlossyEnvironmentData g;
	g.roughness = (1-0.986);
	g.reflUVW = reflect(-vd.worldViewDir, vd.worldNormal);
	gi = UnityGlobalIllumination(giInput, 1, vd.worldNormal,g);
	half3 halfDir = normalize (gi.light.dir + vd.worldViewDir);
	//#if UNITY_BRDF_GGX 
	//	half shiftAmount = dot(vd.worldNormal, vd.worldViewDir);
	//	vd.worldNormal = shiftAmount < 0.0f ? vd.worldNormal + vd.worldViewDir * (-shiftAmount + 1e-5f) : vd.worldNormal;
	//#endif
	half nh = saturate(dot(vd.worldNormal, halfDir));
	half nl = saturate(dot(vd.worldNormal, gi.light.dir));
	half nv = abs(dot(vd.worldNormal, vd.worldViewDir));
	half lh = saturate(dot(gi.light.dir, halfDir));
	
	half3 Lighting;
	#if SSUNITY_BRDF_PBS==1
	// surfaceReduction = Int D(NdotH) * NdotH * Id(NdotL>0) dH = 1/(realRoughness^2+1)
	half surfaceReduction;
	if (IsGammaSpace()) surfaceReduction = 1.0 - 0.28*realRoughness*perceptualRoughness;		// 1-0.28*x^3 as approximation for (1/(x^4+1))^(1/2.2) on the domain [0;1]
	else surfaceReduction = 1.0 / (realRoughness*realRoughness + 1.0);			// fade in [0.5;1]
	half whyDoIBotherWithStandardSupport = realRoughness;
	#if UNITY_VERSION<550
		whyDoIBotherWithStandardSupport = perceptualRoughness;
	#endif
	#if UNITY_BRDF_GGX
		half V = SmithJointGGXVisibilityTerm (nl, nv, whyDoIBotherWithStandardSupport);
		half D = GGXTermNoFadeout (nh, realRoughness);
	#else
		half V = SmithBeckmannVisibilityTerm (nl, nv, whyDoIBotherWithStandardSupport);
		half D = NDFBlinnPhongNormalizedTerm (nh, RoughnessToSpecPower (perceptualRoughness));
	#endif
	
	// HACK: theoretically we should divide by Pi diffuseTerm and not multiply specularTerm!
	// BUT 1) that will make shader look significantly darker than Legacy ones
	// and 2) on engine side Non-important lights have to be divided by Pi to in cases when they are injected into ambient SH
	// NOTE: multiplication by Pi is part of single constant together with 1/4 now
	#if UNITY_VERSION>=550//Unity changed the value of PI...:(
		half specularTerm = (V * D) * (UNITY_PI); // Torrance-Sparrow model, Fresnel is applied later (for optimization reasons)
	#else
		half specularTerm = (V * D) * (UNITY_PI/4); // Torrance-Sparrow model, Fresnel is applied later (for optimization reasons)
	#endif
	if (IsGammaSpace())
		specularTerm = sqrt(max(1e-4h, specularTerm));
	specularTerm = max(0, specularTerm * nl);
	
	
	Lighting =	specularTerm;
	#elif SSUNITY_BRDF_PBS==2
	#if UNITY_BRDF_GGX
	
		// GGX Distribution multiplied by combined approximation of Visibility and Fresnel
		// See "Optimizing PBR for Mobile" from Siggraph 2015 moving mobile graphics course
		// https://community.arm.com/events/1155
		half a = realRoughness;
		half a2 = a*a;
	
		half d = nh * nh * (a2 - 1.h) + 1.00001h;
	#ifdef UNITY_COLORSPACE_GAMMA
		// Tighter approximation for Gamma only rendering mode!
		// DVF = sqrt(DVF);
		// DVF = (a * sqrt(.25)) / (max(sqrt(0.1), lh)*sqrt(realRoughness + .5) * d);
		half specularTerm = a / (max(0.32h, lh) * (1.5h + realRoughness) * d);
	#else
		half specularTerm = a2 / (max(0.1h, lh*lh) * (realRoughness + 0.5h) * (d * d) * 4);
	#endif
	
		// on mobiles (where half actually means something) denominator have risk of overflow
		// clamp below was added specifically to "fix" that, but dx compiler (we convert bytecode to metal/gles)
		// sees that specularTerm have only non-negative terms, so it skips max(0,..) in clamp (leaving only min(100,...))
	#if defined (SHADER_API_MOBILE)
		specularTerm = specularTerm - 1e-4h;
	#endif
	
	#else
	
		// Legacy
		half specularPower = RoughnessToSpecPower(perceptualRoughness);
		// Modified with approximate Visibility function that takes roughness into account
		// Original ((n+1)*N.H^n) / (8*Pi * L.H^3) didn't take into account roughness
		// and produced extremely bright specular at grazing angles
	
		half invV = lh * lh * smoothness + perceptualRoughness * perceptualRoughness; // approx ModifiedKelemenVisibilityTerm(lh, perceptualRoughness);
		half invF = lh;
	
		half specularTerm = ((specularPower + 1) * pow (nh, specularPower)) / (8 * invV * invF + 1e-4h);
	
	#ifdef UNITY_COLORSPACE_GAMMA
		specularTerm = sqrt(max(1e-4h, specularTerm));
	#endif
	
	#endif
	
	#if defined (SHADER_API_MOBILE)
		specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
	#endif
	#if defined(_SPECULARHIGHLIGHTS_OFF)
		specularTerm = 0.0;
	#endif
	
		// surfaceReduction = Int D(NdotH) * NdotH * Id(NdotL>0) dH = 1/(realRoughness^2+1)
	
		// 1-0.28*x^3 as approximation for (1/(x^4+1))^(1/2.2) on the domain [0;1]
		// 1-x^3*(0.6-0.08*x)   approximation for 1/(x^4+1)
	#ifdef UNITY_COLORSPACE_GAMMA
		half surfaceReduction = 0.28;
	#else
		half surfaceReduction = (0.6-0.08*perceptualRoughness);
	#endif
		surfaceReduction = 1.0 - realRoughness*perceptualRoughness*surfaceReduction;
	
	Lighting =	specularTerm;
	#else
	half surfaceReduction = 1;
	half2 rlPow4AndFresnelTerm = Pow4 (half2(dot(vd.worldRefl, gi.light.dir), 1-nv));
	half rlPow4 = rlPow4AndFresnelTerm.x; // power exponent must match kHorizontalWarpExp in NHxRoughness() function in GeneratedTextures.cpp
	half fresnelTerm = rlPow4AndFresnelTerm.y;
	
	
	Lighting =	BRDF3_Direct(0,1,rlPow4,oneMinusRoughness)*nl;
	#endif
	Lighting *= gi.light.color;
	half grazingTerm = saturate((oneMinusRoughness) + (reflectivity));
	#if SSUNITY_BRDF_PBS==1
	Lighting =	Lighting * FresnelTerm (Surface, lh) + surfaceReduction * gi.indirect.specular * FresnelLerp (Surface.rgb, grazingTerm, nv);
	#elif SSUNITY_BRDF_PBS==2
	Lighting =	(Lighting * nl + surfaceReduction * gi.indirect.specular) * FresnelLerpFast (Surface.rgb, grazingTerm, nv);
	#else
	Lighting = (Lighting+gi.indirect.specular) * lerp (Surface.rgb, grazingTerm, fresnelTerm);
	#endif
	OneMinusAlpha = oneMinusReflectivity;
	return half4(Lighting,reflectivity);
	
}
//OutputPremultiplied: False
//UseAlphaGenerate: True
half4 Set_Alpha_Channel ( VertexData vd, half4 outputColor){
	float Surface = 0;
		//Generate layers for the Transparency channel.
			//Generate Layer: Alpha
				//Sample parts of the layer:
					half4 AlphaTransparency_Sample1 = float4(float3((1-dot(vd.worldNormal, vd.worldViewDir)).rrr),1);
	
	Surface = AlphaTransparency_Sample1.r;//2
	
	
			//Generate Layer: Sparkles
				//Sample parts of the layer:
					half4 SparklesTransparency_Sample1 = 1;
	
	Surface = lerp(Surface,(Surface + SparklesTransparency_Sample1.r),vd.Mask2);//1
	
	
			//Generate Layer: Squares
				//Sample parts of the layer:
					half4 SquaresTransparency_Sample1 = 1;
	
	Surface = lerp(Surface,(Surface + SquaresTransparency_Sample1.r),vd.Mask3);//1
	
	
	
	return float4(outputColor.rgb,Surface);
	
}
float4 Transparency ( float4 outputColor){
	outputColor.a *= 1;
	return outputColor;
	
}
void Displace ( inout VertexData vd, inout VertexShaderInput v){
	half3 Surface = half3(1,1,1);
		//Generate layers for the Displacement channel.
			//Generate Layer: Displacement
				//Sample parts of the layer:
					half4 DisplacementSurface_Sample1 = 0;
	
	Surface = DisplacementSurface_Sample1.rgb;//0
	
	
			//Generate Layer: Displacement2
				//Sample parts of the layer:
					half4 Displacement2Surface_Sample1 = _Flicker_Direction;
	
	Surface = lerp(Surface,(Surface + Displacement2Surface_Sample1.rgb),vd.Mask7);//1
	
	
	
	v.vertex.xyz = float3(v.vertex.xyz + v.normal * Surface * (-1));
	
}
void Set ( inout VertexData vd, inout VertexShaderInput v){
	float3 Surface = v.vertex.xyz;
		//Generate layers for the Vertex channel.
			//Generate Layer: Vertex
				//Sample parts of the layer:
					half4 VertexSurface_Sample1 = _Pull_Direction;
	
				//Apply Effects:
					VertexSurface_Sample1.rgb = (VertexSurface_Sample1.rgb*vd.Mask8.rgb);
	
	Surface = lerp(Surface,VertexSurface_Sample1.rgb,vd.Mask5);//1
	
	
	
	v.vertex.xyz = Surface;
	
}
float Mask_Mask0 ( VertexData vd){
		//Set default mask color
			float Mask0 = 0;
		//Generate layers for the Rim channel.
			//Generate Layer: Rim 2
				//Sample parts of the layer:
					half4 Rim_2Mask0_Sample1 = float4(float3(1-float3(dot(vd.worldNormal, vd.worldViewDir).rrr).x,float3(dot(vd.worldNormal, vd.worldViewDir).rrr).y,float3(dot(vd.worldNormal, vd.worldViewDir).rrr).z),1);
	
				//Apply Effects:
					Rim_2Mask0_Sample1.rgb = pow(Rim_2Mask0_Sample1.rgb,4);
	
	Mask0 = Rim_2Mask0_Sample1.r;//2
	
	
	return Mask0;
	
}
float Mask_Mask1 ( VertexData vd){
		//Set default mask color
			float Mask1 = 0;
		//Generate layers for the Dust channel.
			//Generate Layer: Mask1
				//Sample parts of the layer:
					half4 Mask1Mask1_Sample1 = GammaToLinear(NoiseCloud3D(((vd.worldPos*float3(30,30,30))+float3(0,0,_Time.y * 0.5))*3));
	
	Mask1 = Mask1Mask1_Sample1.r;//2
	
	
	return Mask1;
	
}
float Mask_Mask2 ( VertexData vd){
		//Set default mask color
			float Mask2 = 0;
		//Generate layers for the Sparkles channel.
			//Generate Layer: Mask1 Copy
				//Sample parts of the layer:
					half4 Mask1_CopyMask2_Sample1 = GammaToLinear((PerlinNoise3D(((vd.worldPos*float3(300,300,300))+float3(0,0,_Time.y * 0.5))*3)+1)/2);
	
	Mask2 = Mask1_CopyMask2_Sample1.r;//2
	
	
			//Generate Layer: Mask1 Copy Copy 2
				//Sample parts of the layer:
					half4 Mask1_Copy_Copy_2Mask2_Sample1 = GammaToLinear(NoiseCloud3D(((vd.worldPos*float3(30,30,30))+float3(0,0,_Time.y * 0.5))*3));
	
	Mask2 = (Mask2 * Mask1_Copy_Copy_2Mask2_Sample1.r);//0
	
	
			//Generate Layer: Sparkles 2
				//Sample parts of the layer:
					half4 Sparkles_2Mask2_Sample1 = float4(Mask2.rrrr);
	
				//Apply Effects:
					Sparkles_2Mask2_Sample1.rgb = (round(Sparkles_2Mask2_Sample1.rgb*1)/1);
	
	Mask2 = Sparkles_2Mask2_Sample1.r;//0
	
	
	return Mask2;
	
}
float Mask_Mask3 ( VertexData vd){
		//Set default mask color
			float Mask3 = 0;
		//Generate layers for the Squares channel.
			//Generate Layer: Mask1 Copy Copy
				//Sample parts of the layer:
					half4 Mask1_Copy_CopyMask3_Sample1 = GammaToLinear((PerlinNoise3D((((round(vd.worldPos/float3(0.1250099,0.1250099,0.1250099))*float3(0.1250099,0.1250099,0.1250099))*float3(1.5,1.5,1.5))+float3(0,0,_Time.y * 0.5))*3)+1)/2);
	
				//Apply Effects:
					Mask1_Copy_CopyMask3_Sample1.rgb = pow(Mask1_Copy_CopyMask3_Sample1.rgb,2);
	
	Mask3 = Mask1_Copy_CopyMask3_Sample1.r;//2
	
	
	return Mask3;
	
}
float Mask_Mask4 ( VertexData vd){
		//Set default mask color
			float Mask4 = 0;
		//Generate layers for the Lines channel.
			//Generate Layer: Lines 2
				//Sample parts of the layer:
					half4 Lines_2Mask4_Sample1 = float4(((mul(unity_WorldToObject, float4(vd.worldPos,1)).xyz+float3(_Time.y * 0.5,0,0))*float3(50,50,50)),1);
	
				//Apply Effects:
					Lines_2Mask4_Sample1.rgb = ((sin(Lines_2Mask4_Sample1.rgb)+1)/2);
	
	Mask4 = Lines_2Mask4_Sample1.r;//2
	
	
			//Generate Layer: Lines Copy 2
				//Sample parts of the layer:
					half4 Lines_Copy_2Mask4_Sample1 = float4(((mul(unity_WorldToObject, float4(vd.worldPos,1)).xyz+float3(_Time.y * 0.5,0,0))*float3(20,20,20)),1);
	
				//Apply Effects:
					Lines_Copy_2Mask4_Sample1.rgb = sin(Lines_Copy_2Mask4_Sample1.rgb);
	
	Mask4 = (Mask4 * Lines_Copy_2Mask4_Sample1.r);//0
	
	
			//Generate Layer: Lines2
				//Sample parts of the layer:
					half4 Lines2Mask4_Sample1 = float4(Mask4.rrrr);
	
				//Apply Effects:
					Lines2Mask4_Sample1.rgb = clamp(Lines2Mask4_Sample1.rgb,0,1);
					Lines2Mask4_Sample1.rgb = pow(Lines2Mask4_Sample1.rgb,6);
	
	Mask4 = Lines2Mask4_Sample1.r;//0
	
	
	return Mask4;
	
}
float Mask_Mask5 ( VertexData vd){
		//Set default mask color
			float Mask5 = 0;
		//Generate layers for the Pull channel.
			//Generate Layer: Pull
				//Sample parts of the layer:
					half4 PullMask5_Sample1 = float4(vd.worldPos,1);
	
				//Apply Effects:
					PullMask5_Sample1.rgb = (float3(1,1,1)-PullMask5_Sample1.rgb);
					PullMask5_Sample1.rgb = (PullMask5_Sample1.rgb-(_Pull_Height));
					PullMask5_Sample1.rgb = clamp(PullMask5_Sample1.rgb,0,1);
					PullMask5_Sample1.rgb = pow(PullMask5_Sample1.rgb,4);
	
	Mask5 = PullMask5_Sample1.g;//2
	
	
	return Mask5;
	
}
float Mask_Mask6 ( VertexData vd){
		//Set default mask color
			float Mask6 = 0;
		//Generate layers for the Flicker channel.
			//Generate Layer: Lines Copy
				//Sample parts of the layer:
					half4 Lines_CopyMask6_Sample1 = float4(((vd.worldPos*float3(0.1,0.1,0.1))+float3(_Time.y,0,0)),1);
	
				//Apply Effects:
					Lines_CopyMask6_Sample1.rgb = ((sin(Lines_CopyMask6_Sample1.rgb)+1)/2);
					Lines_CopyMask6_Sample1.rgb = pow(Lines_CopyMask6_Sample1.rgb,240);
					Lines_CopyMask6_Sample1.rgb = (round(Lines_CopyMask6_Sample1.rgb*1)/1);
	
	Mask6 = Lines_CopyMask6_Sample1.r;//2
	
	
			//Generate Layer: Flicker
				//Sample parts of the layer:
					half4 FlickerMask6_Sample1 = GammaToLinear(float4(1, 1, 1, 1));
	
	Mask6 = lerp(Mask6,FlickerMask6_Sample1.r,_Force_Flicker);//1
	
	
	return Mask6;
	
}
float Mask_Mask7 ( VertexData vd){
		//Set default mask color
			float Mask7 = 0;
		//Generate layers for the FlickerNoise channel.
			//Generate Layer: FlickerNoise
				//Sample parts of the layer:
					half4 FlickerNoiseMask7_Sample1 = GammaToLinear(float4(0, 0, 0, 1));
	
	Mask7 = FlickerNoiseMask7_Sample1.r;//2
	
	
			//Generate Layer: FlickerNoise2
				//Sample parts of the layer:
					half4 FlickerNoise2Mask7_Sample1 = (PerlinNoise2D(((vd.genericTexcoord+float2(_Time.y * 0.5,0))*float2(14.52,14.52))*3)+1)/2;
	
	Mask7 = lerp(Mask7,FlickerNoise2Mask7_Sample1.r,vd.Mask6);//1
	
	
	return Mask7;
	
}
float4 Mask_Mask8 ( VertexData vd){
		//Set default mask color
			float4 Mask8 = float4(0,0,0,0);
		//Generate layers for the Legacy Position Mask channel.
			//Generate Layer: Legacy Object Space Position
				//Sample parts of the layer:
					half4 Legacy_Object_Space_PositionMask8_Sample1 = float4(mul(unity_WorldToObject, float4(vd.worldPos,1)).xyz,1);
	
	Mask8 = half4(Legacy_Object_Space_PositionMask8_Sample1.rgb,1);//7
	
	
	return Mask8;
	
}
VertexToPixel Vertex (VertexShaderInput v){
	VertexToPixel vtp;
	UNITY_INITIALIZE_OUTPUT(VertexToPixel,vtp);
	VertexData vd;
	UNITY_INITIALIZE_OUTPUT(VertexData,vd);
	vd.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	vd.position = UnityObjectToClipPos(v.vertex);
	vd.worldNormal = UnityObjectToWorldNormalNew(v.normal);
	vd.worldViewDir = normalize(UnityWorldSpaceViewDir(vd.worldPos));
	vd.genericTexcoord = v.texcoord;
	#if UNITY_SHOULD_SAMPLE_SH
	vd.sh = 0;
// SH/ambient and vertex lights
#ifdef LIGHTMAP_OFF
	vd.sh = 0;
	// Approximated illumination from non-important point lights
	#ifdef VERTEXLIGHT_ON
		vd.sh += Shade4PointLights (
		unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
		unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
		unity_4LightAtten0, vd.worldPos, vd.worldNormal);
	#endif
	vd.sh = ShadeSHPerVertex (vd.worldNormal, vd.sh);
#endif // LIGHTMAP_OFF
;
#endif
	#if (!defined(LIGHTMAP_OFF))||(SHADER_TARGET >= 30)
	vd.lmap = 0;
#ifndef DYNAMICLIGHTMAP_OFF
	vd.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#endif
#ifndef LIGHTMAP_OFF
	vd.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif
;
	#endif
	
	
vd.Mask5 = Mask_Mask5 ( vd);
vd.Mask6 = Mask_Mask6 ( vd);
vd.Mask7 = Mask_Mask7 ( vd);
vd.Mask8 = Mask_Mask8 ( vd);
	Displace ( vd, v);
	vd.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	vd.position = UnityObjectToClipPos(v.vertex);
	vd.worldNormal = UnityObjectToWorldNormalNew(v.normal);
	vd.worldViewDir = normalize(UnityWorldSpaceViewDir(vd.worldPos));
	vd.genericTexcoord = v.texcoord;
	#if UNITY_SHOULD_SAMPLE_SH
	vd.sh = 0;
// SH/ambient and vertex lights
#ifdef LIGHTMAP_OFF
	vd.sh = 0;
	// Approximated illumination from non-important point lights
	#ifdef VERTEXLIGHT_ON
		vd.sh += Shade4PointLights (
		unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
		unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
		unity_4LightAtten0, vd.worldPos, vd.worldNormal);
	#endif
	vd.sh = ShadeSHPerVertex (vd.worldNormal, vd.sh);
#endif // LIGHTMAP_OFF
;
#endif
	#if (!defined(LIGHTMAP_OFF))||(SHADER_TARGET >= 30)
	vd.lmap = 0;
#ifndef DYNAMICLIGHTMAP_OFF
	vd.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#endif
#ifndef LIGHTMAP_OFF
	vd.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif
;
	#endif
	Set ( vd, v);
	vd.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	vd.position = UnityObjectToClipPos(v.vertex);
	vd.worldNormal = UnityObjectToWorldNormalNew(v.normal);
	vd.worldViewDir = normalize(UnityWorldSpaceViewDir(vd.worldPos));
	vd.genericTexcoord = v.texcoord;
	#if UNITY_SHOULD_SAMPLE_SH
	vd.sh = 0;
// SH/ambient and vertex lights
#ifdef LIGHTMAP_OFF
	vd.sh = 0;
	// Approximated illumination from non-important point lights
	#ifdef VERTEXLIGHT_ON
		vd.sh += Shade4PointLights (
		unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
		unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
		unity_4LightAtten0, vd.worldPos, vd.worldNormal);
	#endif
	vd.sh = ShadeSHPerVertex (vd.worldNormal, vd.sh);
#endif // LIGHTMAP_OFF
;
#endif
	#if (!defined(LIGHTMAP_OFF))||(SHADER_TARGET >= 30)
	vd.lmap = 0;
#ifndef DYNAMICLIGHTMAP_OFF
	vd.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#endif
#ifndef LIGHTMAP_OFF
	vd.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif
;
	#endif
	
	
	vtp.worldPos = vd.worldPos;
	vtp.position = vd.position;
	vtp.worldNormal = vd.worldNormal;
	vtp.worldViewDir = vd.worldViewDir;
	vtp.genericTexcoord = vd.genericTexcoord;
	#if UNITY_SHOULD_SAMPLE_SH
	vtp.sh = vd.sh;
#endif
	#define pos position
	TRANSFER_SHADOW(vtp);
	UNITY_TRANSFER_FOG(vtp,vtp.pos);
#undef pos
	#if (!defined(LIGHTMAP_OFF))||(SHADER_TARGET >= 30)
	vtp.lmap = vd.lmap;
	#endif
	return vtp;
}
			half4 Pixel (VertexToPixel vtp) : SV_Target {
				half4 outputColor = half4(0,0,0,0);
				half3 outputNormal = half3(0,0,1);
				half3 depth = half3(0,0,0);//Tangent Corrected depth, World Space depth, Normalized depth
				half3 tempDepth = half3(0,0,0);
				VertexData vd;
				UNITY_INITIALIZE_OUTPUT(VertexData,vd);
				vd.worldPos = vtp.worldPos;
				vd.worldNormal = normalize(vtp.worldNormal);
				vd.worldViewDir = normalize(UnityWorldSpaceViewDir(vd.worldPos));
				vd.worldRefl = reflect(-vd.worldViewDir, vd.worldNormal);
				vd.genericTexcoord = vtp.genericTexcoord;
	#if UNITY_SHOULD_SAMPLE_SH
				vd.sh = vtp.sh;
#endif
	#if (!defined(LIGHTMAP_OFF))||(SHADER_TARGET >= 30)
				vd.lmap = vtp.lmap;
	#endif
				half OneMinusAlpha = 0; //An optimization to avoid redundant 1-a if already calculated in ingredient function :)
				outputNormal = vd.worldNormal;
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT(UnityGI,gi);
// Setup lighting environment
				#ifndef USING_DIRECTIONAL_LIGHT
					fixed3 lightDir = normalize(UnityWorldSpaceLightDir(vd.worldPos));
				#else
					fixed3 lightDir = _WorldSpaceLightPos0.xyz;
				#endif
				//Uses SHADOW_COORDS
				UNITY_LIGHT_ATTENUATION(atten, vtp, vd.worldPos)
				vd.Atten = atten;
				gi.indirect.diffuse = 0;
				gi.indirect.specular = 0;
				#if !defined(LIGHTMAP_ON)
					gi.light.color = _LightColor0.rgb;
					gi.light.dir = lightDir;
					gi.light.ndotl = LambertTerm (vd.worldNormal, gi.light.dir);
				#endif
				// Call GI (lightmaps/SH/reflections) lighting function
				UnityGIInput giInput;
				UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
				giInput.light = gi.light;
				giInput.worldPos = vd.worldPos;
				giInput.worldViewDir = vd.worldViewDir;
				giInput.atten = atten;
				#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
					giInput.lightmapUV = vd.lmap;
				#else
					giInput.lightmapUV = 0.0;
				#endif
				#if UNITY_SHOULD_SAMPLE_SH
					giInput.ambient = vd.sh;
				#else
					giInput.ambient.rgb = 0.0;
				#endif
				giInput.probeHDR[0] = unity_SpecCube0_HDR;
				giInput.probeHDR[1] = unity_SpecCube1_HDR;
				#if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
					giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
				#endif
				#if UNITY_SPECCUBE_BOX_PROJECTION
					giInput.boxMax[0] = unity_SpecCube0_BoxMax;
					giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
					giInput.boxMax[1] = unity_SpecCube1_BoxMax;
					giInput.boxMin[1] = unity_SpecCube1_BoxMin;
					giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
				#endif
vd.Mask0 = Mask_Mask0 ( vd);
vd.Mask1 = Mask_Mask1 ( vd);
vd.Mask2 = Mask_Mask2 ( vd);
vd.Mask3 = Mask_Mask3 ( vd);
vd.Mask4 = Mask_Mask4 ( vd);
				half4 outputEmission = Emission ( vd);
				outputColor = half4(outputColor.rgb + outputEmission.rgb,outputEmission.a);//10
								half4 outputSpecular = Specular ( vd, gi, giInput, OneMinusAlpha);
				outputColor = ((outputColor) * OneMinusAlpha + (half4((outputSpecular.a * outputColor.rgb + outputSpecular.rgb), outputSpecular.a)));//1
								half4 outputSet_Alpha_Channel = Set_Alpha_Channel ( vd, outputColor);
				outputColor = outputSet_Alpha_Channel;//10
								outputColor = Transparency ( outputColor);
				UNITY_APPLY_FOG(vtp.fogCoord, outputColor); // apply fog (UNITY_FOG_COORDS));
				return outputColor;

			}
		ENDCG
	}
AlphaToMask Off
	Pass {
		Name "ShadowCaster"
		Tags { "LightMode" = "ShadowCaster" }
	ZTest LEqual
	ZWrite On
	Blend Off//No transparency
	Cull Back//Culling specifies which sides of the models faces to hide.

		
		CGPROGRAM
			// compile directives
				#pragma vertex Vertex
				#pragma fragment Pixel
				#pragma target 3.0
				#pragma multi_compile_fog
				#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
				#pragma multi_compile_shadowcaster
				#include "HLSLSupport.cginc"
				#include "UnityShaderVariables.cginc"
				#define SHADERSANDWICH_SHADOWCASTER
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "UnityPBSLighting.cginc"
				#include "AutoLight.cginc"

				#define INTERNAL_DATA
				#define WorldReflectionVector(data,normal) data.worldRefl
				#define WorldNormalVector(data,normal) normal
			//Make our inputs accessible by declaring them here.
				float4 _Rim_Color;
				float4 _Dust_Color;
				float4 _Sparkles_Color;
				float4 _Squares_Color;
				float4 _Lines_Color;
				float _Force_Flicker;
				float4 _Flicker_Direction;
				float _Pull_Height;
				float4 _Pull_Direction;

#if !defined (SSUNITY_BRDF_PBS) // allow to explicitly override BRDF in custom shader
	// still add safe net for low shader models, otherwise we might end up with shaders failing to compile
	#if SHADER_TARGET < 30
		#define SSUNITY_BRDF_PBS 3
	#elif defined(UNITY_PBS_USE_BRDF3)
		#define SSUNITY_BRDF_PBS 3
	#elif defined(UNITY_PBS_USE_BRDF2)
		#define SSUNITY_BRDF_PBS 2
	#elif defined(UNITY_PBS_USE_BRDF1)
		#define SSUNITY_BRDF_PBS 1
	#elif defined(SHADER_TARGET_SURFACE_ANALYSIS)
		// we do preprocess pass during shader analysis and we dont actually care about brdf as we need only inputs/outputs
		#define SSUNITY_BRDF_PBS 1
	#else
		#error something broke in auto-choosing BRDF (Shader Sandwich)
	#endif
#endif

//Some noise code based on the fantastic library by Brian Sharpe, he deserves a ton of credit :)
//brisharpe CIRCLE_A yahoo DOT com
//http://briansharpe.wordpress.com
//https://github.com/BrianSharpe

float3 PerlinInterpolation_C2( float3 x ) { return x * x * x * (x * (x * 6.0 - 15.0) + 10.0); }

void PerlinFastHash3D(float3 Pos,out float4 hash_0, out float4 hash_1,out float4 hash_2, out float4 hash_3,out float4 hash_4, out float4 hash_5){
	float2 Offset = float2(50,161);
	float Domain = 69;
	float3 SomeLargeFloats = float3(635.298681, 682.357502, 668.926525 );
	float3 Zinc = float3( 48.500388, 65.294118, 63.934599 );
	
	Pos = Pos-floor(Pos*(1.0/Domain))*Domain;
	float3 Pos_Inc1 = step(Pos,float(Domain-1.5).rrr)*(Pos+1);
	
	float4 P = float4(Pos.xy,Pos_Inc1.xy)+Offset.xyxy;
	P *= P;
	P = P.xzxz*P.yyww;
	
	float3 lowz_mod = float3(1/(SomeLargeFloats+Pos.zzz*Zinc));//Pos.zzz
	float3 highz_mod = float3(1/(SomeLargeFloats+Pos_Inc1.zzz*Zinc));//Pos_Inc1.zzz
	
	hash_0 = frac(P*lowz_mod.xxxx);
	hash_1 = frac(P*lowz_mod.yyyy);
	hash_2 = frac(P*lowz_mod.zzzz);
	hash_3 = frac(P*highz_mod.xxxx);
	hash_4 = frac(P*highz_mod.yyyy);
	hash_5 = frac(P*highz_mod.zzzz);
}
float PerlinNoise3D(float3 P)
{
	float3 Pi = floor(P);
	float3 Pf = P-Pi;
	float3 Pf_min1 = Pf-1.0;
	
	float4 HashX0, HashY0, HashZ0, HashX1, HashY1, HashZ1;
	PerlinFastHash3D(Pi, HashX0, HashY0, HashZ0, HashX1, HashY1, HashZ1);
	
	float4 GradX0 = HashX0-0.49999999;
	float4 GradX1 = HashX1-0.49999999;
	float4 GradY0 = HashY0-0.49999999;
	float4 GradY1 = HashY1-0.49999999;
	float4 GradZ0 = HashZ0-0.49999999;
	float4 GradZ1 = HashZ1-0.49999999;

	float4 GradRes = rsqrt( GradX0 * GradX0 + GradY0 * GradY0 + GradZ0 * GradZ0+0.00001) * ( float2( Pf.x, Pf_min1.x ).xyxy * GradX0 + float2( Pf.y, Pf_min1.y ).xxyy * GradY0 + Pf.zzzz * GradZ0 );
	float4 GradRes2 = rsqrt( GradX1 * GradX1 + GradY1 * GradY1 + GradZ1 * GradZ1+0.00001) * ( float2( Pf.x, Pf_min1.x ).xyxy * GradX1 + float2( Pf.y, Pf_min1.y ).xxyy * GradY1 + Pf_min1.zzzz * GradZ1 );
	
	float3 Blend = PerlinInterpolation_C2(Pf);
	
	float4 Res = lerp(GradRes,GradRes2,Blend.z);
	float4 Blend2 = float4(Blend.xy,float2(1.0-Blend.xy));
	float Final = dot(Res,Blend2.zxzx*Blend2.wwyy);
	Final *= 1.1547005383792515290182975610039;
	return Final;
}

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

float PerlinNoise2D(float2 P){
	float2 Pi = floor(P);
	float4 Pf_Pfmin1 = P.xyxy-float4(Pi,Pi+1);
	float4 HashX, HashY;
	PerlinFastHash2D(Pi,HashX,HashY);
	float4 GradX = HashX-0.499999;
	float4 GradY = HashY-0.499999;
	float4 GradRes = rsqrt(GradX*GradX+GradY*GradY+0.00001)*(GradX*Pf_Pfmin1.xzxz+GradY*Pf_Pfmin1.yyww);
	
	GradRes *= 1.4142135623730950488016887242097;
	float2 blend = PerlinInterpolation_C2(Pf_Pfmin1.xy);
	float4 blend2 = float4(blend,float2(1.0-blend));
	return (dot(GradRes,blend2.zxzx*blend2.wwyy));
}

	//From UnityCG.inc, Unity 2017.01 - works better than any of the earlier ones
	inline float3 UnityObjectToWorldNormalNew( in float3 norm ){
		#ifdef UNITY_ASSUME_UNIFORM_SCALING
			return UnityObjectToWorldDir(norm);
		#else
			// mul(IT_M, norm) => mul(norm, I_M) => {dot(norm, I_M.col0), dot(norm, I_M.col1), dot(norm, I_M.col2)}
			return normalize(mul(norm, (float3x3)unity_WorldToObject));
		#endif
	}
	float4 GammaToLinear(float4 col){
		#if defined(UNITY_COLORSPACE_GAMMA)
			//Best programming evar XD
		#else
			col.rgb = pow(col,2.2);
		#endif
		return col;
	}

	float4 GammaToLinearForce(float4 col){
		#if defined(UNITY_COLORSPACE_GAMMA)
			//Best programming evar XD
		#else
			col.rgb = pow(col,2.2);
		#endif
		return col;
	}

	float4 LinearToGamma(float4 col){
		return col;
	}

	float4 LinearToGammaForWeirdSituations(float4 col){
		#if defined(UNITY_COLORSPACE_GAMMA)
			col.rgb = pow(col,2.2);
		#endif
		return col;
	}

	float3 GammaToLinear(float3 col){
		#if defined(UNITY_COLORSPACE_GAMMA)
			//Best programming evar XD
		#else
			col = pow(col,2.2);
		#endif
		return col;
	}

	float3 GammaToLinearForce(float3 col){
		#if defined(UNITY_COLORSPACE_GAMMA)
			//Best programming evar XD
		#else
			col = pow(col,2.2);
		#endif
		return col;
	}

	float3 LinearToGamma(float3 col){
		return col;
	}

	float GammaToLinear(float col){
		#if defined(UNITY_COLORSPACE_GAMMA)
			//Best programming evar XD
		#else
			col = pow(col,2.2);
		#endif
		return col;
	}







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
	return lerp(SS,SE,f);
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


































struct VertexShaderInput{
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float4 texcoord : TEXCOORD0;
};
struct VertexToPixel{
	float3 worldPos : TEXCOORD0;
	float4 position : POSITION;
	float3 worldNormal : TEXCOORD1;
	float3 screenPos : TEXCOORD2;
	float2 genericTexcoord : TEXCOORD3;
	#define pos position
		UNITY_FOG_COORDS(4)
#undef pos
	#ifdef SHADOWS_CUBE
		float3 vec : TEXCOORD5;
#endif
	#ifndef SHADOWS_CUBE
		#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
			float4 hpos : TEXCOORD6;
		#endif
	#endif
};

struct VertexData{
	float3 worldPos;
	float4 position;
	float3 worldNormal;
	float3 screenPos;
	float3 worldViewDir;
	float2 genericTexcoord;
	#ifdef SHADOWS_CUBE
		float3 vec;
#endif
	#ifndef SHADOWS_CUBE
		#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
			float4 hpos;
		#endif
	#endif
	float Mask0;
	float Mask1;
	float Mask2;
	float Mask3;
	float Mask4;
	float Mask5;
	float Mask6;
	float Mask7;
	float4 Mask8;
	float Atten;
};
				sampler3D _DitherMaskLOD;
//OutputPremultiplied: False
//UseAlphaGenerate: True
half4 Specular (){
	half3 Surface = half4(0,0,0,1);//Specular Mode
		//Generate layers for the Specular Color channel.
			//Generate Layer: Specular Color
				//Sample parts of the layer:
					half4 Specular_ColorSpecular_Sample1 = GammaToLinear(float4(0, 0.484, 1, 1));
	
	Surface = Specular_ColorSpecular_Sample1.rgb;//0
	
	
	half reflectivity = SpecularStrength(Surface);
	half oneMinusReflectivity = 1-reflectivity;
	return half4(Surface,reflectivity);
	
}
//OutputPremultiplied: False
//UseAlphaGenerate: True
half4 Set_Alpha_Channel ( VertexData vd, half4 outputColor){
	float Surface = 0;
		//Generate layers for the Transparency channel.
			//Generate Layer: Alpha
				//Sample parts of the layer:
					half4 AlphaTransparency_Sample1 = float4(float3((1-dot(vd.worldNormal, vd.worldViewDir)).rrr),1);
	
	Surface = AlphaTransparency_Sample1.r;//2
	
	
			//Generate Layer: Sparkles
				//Sample parts of the layer:
					half4 SparklesTransparency_Sample1 = 1;
	
	Surface = lerp(Surface,(Surface + SparklesTransparency_Sample1.r),vd.Mask2);//1
	
	
			//Generate Layer: Squares
				//Sample parts of the layer:
					half4 SquaresTransparency_Sample1 = 1;
	
	Surface = lerp(Surface,(Surface + SquaresTransparency_Sample1.r),vd.Mask3);//1
	
	
	
	return float4(outputColor.rgb,Surface);
	
}
float4 Transparency ( inout VertexData vd, inout VertexToPixel vtp, float4 outputColor){
	outputColor.a *= 1;
	#if SHADER_TARGET <= 30
		outputColor.a = tex3D(_DitherMaskLOD, float3(floor(vd.screenPos.xy*_ScreenParams.xy)*0.25,outputColor.a*0.9375)).a;
	#else
		outputColor.a = tex3D(_DitherMaskLOD, float3(vtp.position.xy*0.25,outputColor.a*0.9375)).a;
	#endif
	clip (outputColor.a - 0.01);
	return outputColor;
	
}
void Displace ( inout VertexData vd, inout VertexShaderInput v){
	half3 Surface = half3(1,1,1);
		//Generate layers for the Displacement channel.
			//Generate Layer: Displacement
				//Sample parts of the layer:
					half4 DisplacementSurface_Sample1 = 0;
	
	Surface = DisplacementSurface_Sample1.rgb;//0
	
	
			//Generate Layer: Displacement2
				//Sample parts of the layer:
					half4 Displacement2Surface_Sample1 = _Flicker_Direction;
	
	Surface = lerp(Surface,(Surface + Displacement2Surface_Sample1.rgb),vd.Mask7);//1
	
	
	
	v.vertex.xyz = float3(v.vertex.xyz + v.normal * Surface * (-1));
	
}
void Set ( inout VertexData vd, inout VertexShaderInput v){
	float3 Surface = v.vertex.xyz;
		//Generate layers for the Vertex channel.
			//Generate Layer: Vertex
				//Sample parts of the layer:
					half4 VertexSurface_Sample1 = _Pull_Direction;
	
				//Apply Effects:
					VertexSurface_Sample1.rgb = (VertexSurface_Sample1.rgb*vd.Mask8.rgb);
	
	Surface = lerp(Surface,VertexSurface_Sample1.rgb,vd.Mask5);//1
	
	
	
	v.vertex.xyz = Surface;
	
}
float Mask_Mask0 ( VertexData vd){
		//Set default mask color
			float Mask0 = 0;
		//Generate layers for the Rim channel.
			//Generate Layer: Rim 2
				//Sample parts of the layer:
					half4 Rim_2Mask0_Sample1 = float4(float3(1-float3(dot(vd.worldNormal, vd.worldViewDir).rrr).x,float3(dot(vd.worldNormal, vd.worldViewDir).rrr).y,float3(dot(vd.worldNormal, vd.worldViewDir).rrr).z),1);
	
				//Apply Effects:
					Rim_2Mask0_Sample1.rgb = pow(Rim_2Mask0_Sample1.rgb,4);
	
	Mask0 = Rim_2Mask0_Sample1.r;//2
	
	
	return Mask0;
	
}
float Mask_Mask1 ( VertexData vd){
		//Set default mask color
			float Mask1 = 0;
		//Generate layers for the Dust channel.
			//Generate Layer: Mask1
				//Sample parts of the layer:
					half4 Mask1Mask1_Sample1 = GammaToLinear(NoiseCloud3D(((vd.worldPos*float3(30,30,30))+float3(0,0,_Time.y * 0.5))*3));
	
	Mask1 = Mask1Mask1_Sample1.r;//2
	
	
	return Mask1;
	
}
float Mask_Mask2 ( VertexData vd){
		//Set default mask color
			float Mask2 = 0;
		//Generate layers for the Sparkles channel.
			//Generate Layer: Mask1 Copy
				//Sample parts of the layer:
					half4 Mask1_CopyMask2_Sample1 = GammaToLinear((PerlinNoise3D(((vd.worldPos*float3(300,300,300))+float3(0,0,_Time.y * 0.5))*3)+1)/2);
	
	Mask2 = Mask1_CopyMask2_Sample1.r;//2
	
	
			//Generate Layer: Mask1 Copy Copy 2
				//Sample parts of the layer:
					half4 Mask1_Copy_Copy_2Mask2_Sample1 = GammaToLinear(NoiseCloud3D(((vd.worldPos*float3(30,30,30))+float3(0,0,_Time.y * 0.5))*3));
	
	Mask2 = (Mask2 * Mask1_Copy_Copy_2Mask2_Sample1.r);//0
	
	
			//Generate Layer: Sparkles 2
				//Sample parts of the layer:
					half4 Sparkles_2Mask2_Sample1 = float4(Mask2.rrrr);
	
				//Apply Effects:
					Sparkles_2Mask2_Sample1.rgb = (round(Sparkles_2Mask2_Sample1.rgb*1)/1);
	
	Mask2 = Sparkles_2Mask2_Sample1.r;//0
	
	
	return Mask2;
	
}
float Mask_Mask3 ( VertexData vd){
		//Set default mask color
			float Mask3 = 0;
		//Generate layers for the Squares channel.
			//Generate Layer: Mask1 Copy Copy
				//Sample parts of the layer:
					half4 Mask1_Copy_CopyMask3_Sample1 = GammaToLinear((PerlinNoise3D((((round(vd.worldPos/float3(0.1250099,0.1250099,0.1250099))*float3(0.1250099,0.1250099,0.1250099))*float3(1.5,1.5,1.5))+float3(0,0,_Time.y * 0.5))*3)+1)/2);
	
				//Apply Effects:
					Mask1_Copy_CopyMask3_Sample1.rgb = pow(Mask1_Copy_CopyMask3_Sample1.rgb,2);
	
	Mask3 = Mask1_Copy_CopyMask3_Sample1.r;//2
	
	
	return Mask3;
	
}
float Mask_Mask4 ( VertexData vd){
		//Set default mask color
			float Mask4 = 0;
		//Generate layers for the Lines channel.
			//Generate Layer: Lines 2
				//Sample parts of the layer:
					half4 Lines_2Mask4_Sample1 = float4(((mul(unity_WorldToObject, float4(vd.worldPos,1)).xyz+float3(_Time.y * 0.5,0,0))*float3(50,50,50)),1);
	
				//Apply Effects:
					Lines_2Mask4_Sample1.rgb = ((sin(Lines_2Mask4_Sample1.rgb)+1)/2);
	
	Mask4 = Lines_2Mask4_Sample1.r;//2
	
	
			//Generate Layer: Lines Copy 2
				//Sample parts of the layer:
					half4 Lines_Copy_2Mask4_Sample1 = float4(((mul(unity_WorldToObject, float4(vd.worldPos,1)).xyz+float3(_Time.y * 0.5,0,0))*float3(20,20,20)),1);
	
				//Apply Effects:
					Lines_Copy_2Mask4_Sample1.rgb = sin(Lines_Copy_2Mask4_Sample1.rgb);
	
	Mask4 = (Mask4 * Lines_Copy_2Mask4_Sample1.r);//0
	
	
			//Generate Layer: Lines2
				//Sample parts of the layer:
					half4 Lines2Mask4_Sample1 = float4(Mask4.rrrr);
	
				//Apply Effects:
					Lines2Mask4_Sample1.rgb = clamp(Lines2Mask4_Sample1.rgb,0,1);
					Lines2Mask4_Sample1.rgb = pow(Lines2Mask4_Sample1.rgb,6);
	
	Mask4 = Lines2Mask4_Sample1.r;//0
	
	
	return Mask4;
	
}
float Mask_Mask5 ( VertexData vd){
		//Set default mask color
			float Mask5 = 0;
		//Generate layers for the Pull channel.
			//Generate Layer: Pull
				//Sample parts of the layer:
					half4 PullMask5_Sample1 = float4(vd.worldPos,1);
	
				//Apply Effects:
					PullMask5_Sample1.rgb = (float3(1,1,1)-PullMask5_Sample1.rgb);
					PullMask5_Sample1.rgb = (PullMask5_Sample1.rgb-(_Pull_Height));
					PullMask5_Sample1.rgb = clamp(PullMask5_Sample1.rgb,0,1);
					PullMask5_Sample1.rgb = pow(PullMask5_Sample1.rgb,4);
	
	Mask5 = PullMask5_Sample1.g;//2
	
	
	return Mask5;
	
}
float Mask_Mask6 ( VertexData vd){
		//Set default mask color
			float Mask6 = 0;
		//Generate layers for the Flicker channel.
			//Generate Layer: Lines Copy
				//Sample parts of the layer:
					half4 Lines_CopyMask6_Sample1 = float4(((vd.worldPos*float3(0.1,0.1,0.1))+float3(_Time.y,0,0)),1);
	
				//Apply Effects:
					Lines_CopyMask6_Sample1.rgb = ((sin(Lines_CopyMask6_Sample1.rgb)+1)/2);
					Lines_CopyMask6_Sample1.rgb = pow(Lines_CopyMask6_Sample1.rgb,240);
					Lines_CopyMask6_Sample1.rgb = (round(Lines_CopyMask6_Sample1.rgb*1)/1);
	
	Mask6 = Lines_CopyMask6_Sample1.r;//2
	
	
			//Generate Layer: Flicker
				//Sample parts of the layer:
					half4 FlickerMask6_Sample1 = GammaToLinear(float4(1, 1, 1, 1));
	
	Mask6 = lerp(Mask6,FlickerMask6_Sample1.r,_Force_Flicker);//1
	
	
	return Mask6;
	
}
float Mask_Mask7 ( VertexData vd){
		//Set default mask color
			float Mask7 = 0;
		//Generate layers for the FlickerNoise channel.
			//Generate Layer: FlickerNoise
				//Sample parts of the layer:
					half4 FlickerNoiseMask7_Sample1 = GammaToLinear(float4(0, 0, 0, 1));
	
	Mask7 = FlickerNoiseMask7_Sample1.r;//2
	
	
			//Generate Layer: FlickerNoise2
				//Sample parts of the layer:
					half4 FlickerNoise2Mask7_Sample1 = (PerlinNoise2D(((vd.genericTexcoord+float2(_Time.y * 0.5,0))*float2(14.52,14.52))*3)+1)/2;
	
	Mask7 = lerp(Mask7,FlickerNoise2Mask7_Sample1.r,vd.Mask6);//1
	
	
	return Mask7;
	
}
float4 Mask_Mask8 ( VertexData vd){
		//Set default mask color
			float4 Mask8 = float4(0,0,0,0);
		//Generate layers for the Legacy Position Mask channel.
			//Generate Layer: Legacy Object Space Position
				//Sample parts of the layer:
					half4 Legacy_Object_Space_PositionMask8_Sample1 = float4(mul(unity_WorldToObject, float4(vd.worldPos,1)).xyz,1);
	
	Mask8 = half4(Legacy_Object_Space_PositionMask8_Sample1.rgb,1);//7
	
	
	return Mask8;
	
}
VertexToPixel Vertex (VertexShaderInput v){
	VertexToPixel vtp;
	UNITY_INITIALIZE_OUTPUT(VertexToPixel,vtp);
	VertexData vd;
	UNITY_INITIALIZE_OUTPUT(VertexData,vd);
	vd.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	vd.position = 0;
	TRANSFER_SHADOW_CASTER_NOPOS(vd,vd.position);
	vd.worldNormal = UnityObjectToWorldNormalNew(v.normal);
	vd.screenPos = ComputeScreenPos (UnityObjectToClipPos (v.vertex)).xyw;
	vd.genericTexcoord = v.texcoord;
	
	
vd.Mask5 = Mask_Mask5 ( vd);
vd.Mask6 = Mask_Mask6 ( vd);
vd.Mask7 = Mask_Mask7 ( vd);
vd.Mask8 = Mask_Mask8 ( vd);
	Displace ( vd, v);
	vd.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	vd.position = 0;
	TRANSFER_SHADOW_CASTER_NOPOS(vd,vd.position);
	vd.worldNormal = UnityObjectToWorldNormalNew(v.normal);
	vd.screenPos = ComputeScreenPos (UnityObjectToClipPos (v.vertex)).xyw;
	vd.genericTexcoord = v.texcoord;
	Set ( vd, v);
	vd.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	vd.position = 0;
	TRANSFER_SHADOW_CASTER_NOPOS(vd,vd.position);
	vd.worldNormal = UnityObjectToWorldNormalNew(v.normal);
	vd.screenPos = ComputeScreenPos (UnityObjectToClipPos (v.vertex)).xyw;
	vd.genericTexcoord = v.texcoord;
	
	
	vtp.worldPos = vd.worldPos;
	vtp.position = vd.position;
	vtp.worldNormal = vd.worldNormal;
	vtp.screenPos = vd.screenPos;
	vtp.genericTexcoord = vd.genericTexcoord;
	#define pos position
	UNITY_TRANSFER_FOG(vtp,vtp.pos);
#undef pos
	#ifdef SHADOWS_CUBE
	vtp.vec = vd.vec;
#endif
	#ifndef SHADOWS_CUBE
		#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
	vtp.hpos = vd.hpos;
		#endif
	#endif
	return vtp;
}
			half4 Pixel (VertexToPixel vtp) : SV_Target {
				half4 outputColor = half4(0,0,0,0);
				half3 outputNormal = half3(0,0,1);
				half3 depth = half3(0,0,0);//Tangent Corrected depth, World Space depth, Normalized depth
				half3 tempDepth = half3(0,0,0);
				VertexData vd;
				UNITY_INITIALIZE_OUTPUT(VertexData,vd);
				vd.worldPos = vtp.worldPos;
				vd.worldNormal = normalize(vtp.worldNormal);
				vd.screenPos = float3(vtp.screenPos.xy/vtp.screenPos.z,vtp.screenPos.z);
				vd.worldViewDir = 0;
				#ifdef SHADOWS_DEPTH
					//Sorry, no support for directional lights unless Screen Space Shadows are turned off...too much of a pain :/
					vd.worldViewDir = normalize(UnityWorldSpaceLightDir(vd.worldPos));
					if (dot(vd.worldViewDir,vd.worldNormal)<0)
						vd.worldViewDir *= -1;
				#else
					vd.worldViewDir = normalize(UnityWorldSpaceLightDir(vd.worldPos));
				#endif
;
				vd.genericTexcoord = vtp.genericTexcoord;
	#ifdef SHADOWS_CUBE
				vd.vec = vtp.vec;
#endif
	#ifndef SHADOWS_CUBE
		#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
				vd.hpos = vtp.hpos;
		#endif
	#endif
				outputNormal = vd.worldNormal;
vd.Mask0 = Mask_Mask0 ( vd);
vd.Mask1 = Mask_Mask1 ( vd);
vd.Mask2 = Mask_Mask2 ( vd);
vd.Mask3 = Mask_Mask3 ( vd);
vd.Mask4 = Mask_Mask4 ( vd);
				half4 outputSpecular = Specular ();
				outputColor = ((outputColor) * (1 - (outputSpecular.a)) + (half4((outputSpecular.a * outputColor.rgb + outputSpecular.rgb), outputSpecular.a)));//1
								half4 outputSet_Alpha_Channel = Set_Alpha_Channel ( vd, outputColor);
				outputColor = outputSet_Alpha_Channel;//10
								outputColor = Transparency ( vd, vtp, outputColor);
				UNITY_APPLY_FOG(vtp.fogCoord, outputColor); // apply fog (UNITY_FOG_COORDS));
				SHADOW_CASTER_FRAGMENT(vd)
				return outputColor;

			}
		ENDCG
	}
}

Fallback "Legacy Shaders/Diffuse"
}


/*
Shader Sandwich Shader
	File Format Version(Float): 3.0
	Begin Shader Base

		Begin Shader Input
			Type(Text): "Color"
			VisName(Text): "Rim Color"
			Color(Vec): 0,0.5527849,1,1
			CustomFallback(Text): "_Rim_Color"
		End Shader Input


		Begin Shader Input
			Type(Text): "Color"
			VisName(Text): "Dust Color"
			Color(Vec): 0.3647059,0,0.5960785,1
			CustomFallback(Text): "_Dust_Color"
		End Shader Input


		Begin Shader Input
			Type(Text): "Color"
			VisName(Text): "Sparkles Color"
			Color(Vec): 0.01960784,0.1058824,1,1
			CustomFallback(Text): "_Sparkles_Color"
		End Shader Input


		Begin Shader Input
			Type(Text): "Color"
			VisName(Text): "Squares Color"
			Color(Vec): 1,0.3921569,0,1
			CustomFallback(Text): "_Squares_Color"
		End Shader Input


		Begin Shader Input
			Type(Text): "Color"
			VisName(Text): "Lines Color"
			Color(Vec): 1,0.4745098,0,1
			CustomFallback(Text): "_Lines_Color"
		End Shader Input


		Begin Shader Input
			Type(Text): "Range"
			VisName(Text): "Force Flicker"
			Number(Float): 0.04938272
			Range0(Float): 0
			Range1(Float): 1
			CustomFallback(Text): "_Force_Flicker"
		End Shader Input


		Begin Shader Input
			Type(Text): "Color"
			VisName(Text): "Flicker Direction"
			Color(Vec): 0.3161765,0,0.3161765,1
			CustomFallback(Text): "_Flicker_Direction"
		End Shader Input


		Begin Shader Input
			Type(Text): "Float"
			VisName(Text): "Pull Height"
			Number(Float): 0.7
			Range0(Float): 0
			Range1(Float): 1
			CustomFallback(Text): "_Pull_Height"
		End Shader Input


		Begin Shader Input
			Type(Text): "Color"
			VisName(Text): "Pull Direction"
			Color(Vec): 0,1,0,1
			CustomFallback(Text): "_Pull_Direction"
		End Shader Input


		Begin Shader Input
			Type(Text): "Float"
			VisName(Text): "TimeBasicSlow"
			Number(Float): 79.34229
			Range0(Float): 0
			Range1(Float): 1
			SpecialType(Text): "Time"
			InputScale(Float): 0.5
			InEditor(Float): 0
			CustomFallback(Text): "_Time.y * 0.5"
		End Shader Input


		Begin Shader Input
			Type(Text): "Float"
			VisName(Text): "TimeBasicVerySlow"
			Number(Float): 79.34229
			Range0(Float): 0
			Range1(Float): 1
			SpecialType(Text): "Time"
			InputScale(Float): 0.5
			InEditor(Float): 0
			CustomFallback(Text): "_Time.y * 0.5"
		End Shader Input


		Begin Shader Input
			Type(Text): "Float"
			VisName(Text): "LineSpeed"
			Number(Float): 79.34229
			Range0(Float): 0
			Range1(Float): 1
			SpecialType(Text): "Time"
			InputScale(Float): 0.5
			InEditor(Float): 0
			CustomFallback(Text): "_Time.y * 0.5"
		End Shader Input


		Begin Shader Input
			Type(Text): "Float"
			VisName(Text): "GlitchSpeed"
			Number(Float): 158.6846
			Range0(Float): 0
			Range1(Float): 1
			SpecialType(Text): "Time"
			InEditor(Float): 0
			CustomFallback(Text): "_Time.y"
		End Shader Input

		ShaderName(Text): "Shader Sandwich/Specific/Hologram Legacy (More flashy)"
		Tech Lod(Float): 200
		Fallback(Type): Diffuse - {TypeID = 0}
		CustomFallback(Text): "\qLegacy Shaders/Diffuse\q"
		Queue(Type): Auto - {TypeID = 0}
		Custom Queue(Float): 2000
		QueueAuto(Toggle): True
		Replacement(Type): Auto - {TypeID = 0}
		ReplacementAuto(Toggle): True
		Tech Shader Target(Float): 3
		Exclude DX9(Toggle): False

		Begin Masks

			Begin Shader Layer List

				LayerListUniqueName(Text): "Mask0"
				LayerListName(Text): "Rim"
				Is Mask(Toggle): True
				EndTag(Text): "r"

				Begin Shader Layer
					Layer Name(Text): "Rim 2"
					Layer Type(ObjectArray): SLTLiteral - {ObjectID = 4}
					UV Map(Type): Generate - {TypeID = 1}
					Map Local(Toggle): False
					Map Space(Type): World - {TypeID = 0}
					Map Generate Space(Type): Rim - {TypeID = 2}
					Map Inverted(Toggle): False
					Map UV Index(Float): 1
					Map Direction(Type): Normal - {TypeID = 0}
					Map Screen Space(Type): Screen Position - {TypeID = 4}
					Use Fadeout(Toggle): False
					Fadeout Limit Min(Float): 0
					Fadeout Limit Max(Float): 10
					Fadeout Start(Float): 3
					Fadeout End(Float): 5
					Use Alpha(Toggle): False
					Alpha Blend Mode(Type): Blend - {TypeID = 0}
					Mix Amount(Float): 1
					Mix Type(Type): Mix - {TypeID = 0}
					Stencil(ObjectArray): SSNone - {ObjectID = -1}
					Color(Vec): 0.627451,0.8,0.8823529,1
					Begin Shader Effect
						TypeS(Text): "SSEUVFlip"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						X Flip(Toggle): True
						Y Flip(Toggle): False
						Z Flip(Toggle): False
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEMathPow"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Power(Float): 4
					End Shader Effect

				End Shader Layer

			End Shader Layer List

			Begin Shader Layer List

				LayerListUniqueName(Text): "Mask1"
				LayerListName(Text): "Dust"
				Is Mask(Toggle): True
				EndTag(Text): "r"

				Begin Shader Layer
					Layer Name(Text): "Mask1"
					Layer Type(ObjectArray): SLTCloudNoise - {ObjectID = 12}
					UV Map(Type): Position - {TypeID = 2}
					Map Local(Toggle): False
					Map Space(Type): World - {TypeID = 0}
					Map Generate Space(Type): Object - {TypeID = 1}
					Map Inverted(Toggle): True
					Map UV Index(Float): 1
					Map Direction(Type): Normal - {TypeID = 0}
					Map Screen Space(Type): Screen Position - {TypeID = 4}
					Use Fadeout(Toggle): False
					Fadeout Limit Min(Float): 0
					Fadeout Limit Max(Float): 10
					Fadeout Start(Float): 3
					Fadeout End(Float): 5
					Use Alpha(Toggle): False
					Alpha Blend Mode(Type): Blend - {TypeID = 0}
					Mix Amount(Float): 1
					Mix Type(Type): Mix - {TypeID = 0}
					Stencil(ObjectArray): SSNone - {ObjectID = -1}
					Weird Noise Dimensions(Type): 3D - {TypeID = 2}
					Smoother(Toggle): True
					Gamma Correct(Toggle): True
					Noise Dimensions(Type):  - {TypeID = 1}
					Image Based(Toggle): False
					Color(Vec): 0.627451,0.8,0.8823529,1
					Begin Shader Effect
						TypeS(Text): "SSEUVOffset"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						X Offset(Float): 0
						Y Offset(Float): 0
						Z Offset(Float): 367.7224 - {Input = 9}
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEUVScale"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Seperate(Toggle): False
						X Scale(Float): 30
						Y Scale(Float): 30
						Z Scale(Float): 30
					End Shader Effect

				End Shader Layer

			End Shader Layer List

			Begin Shader Layer List

				LayerListUniqueName(Text): "Mask2"
				LayerListName(Text): "Sparkles"
				Is Mask(Toggle): True
				EndTag(Text): "r"

				Begin Shader Layer
					Layer Name(Text): "Mask1 Copy"
					Layer Type(ObjectArray): SLTPerlinNoise - {ObjectID = 10}
					UV Map(Type): Position - {TypeID = 2}
					Map Local(Toggle): False
					Map Space(Type): World - {TypeID = 0}
					Map Generate Space(Type): Object - {TypeID = 1}
					Map Inverted(Toggle): True
					Map UV Index(Float): 1
					Map Direction(Type): Normal - {TypeID = 0}
					Map Screen Space(Type): Screen Position - {TypeID = 4}
					Use Fadeout(Toggle): False
					Fadeout Limit Min(Float): 0
					Fadeout Limit Max(Float): 10
					Fadeout Start(Float): 3
					Fadeout End(Float): 5
					Use Alpha(Toggle): False
					Alpha Blend Mode(Type): Blend - {TypeID = 0}
					Mix Amount(Float): 1
					Mix Type(Type): Mix - {TypeID = 0}
					Stencil(ObjectArray): SSNone - {ObjectID = -1}
					Noise Dimensions(Type): 3D - {TypeID = 1}
					Image Based(Toggle): False
					Gamma Correct(Toggle): True
					Color(Vec): 0.627451,0.8,0.8823529,1
					Weird Noise Dimensions(Type):  - {TypeID = 2}
					Smoother(Toggle): True
					Begin Shader Effect
						TypeS(Text): "SSEUVOffset"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						X Offset(Float): 0
						Y Offset(Float): 0
						Z Offset(Float): 366.5168 - {Input = 9}
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEUVScale"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Seperate(Toggle): False
						X Scale(Float): 300
						Y Scale(Float): 300
						Z Scale(Float): 300
					End Shader Effect

				End Shader Layer

				Begin Shader Layer
					Layer Name(Text): "Mask1 Copy Copy 2"
					Layer Type(ObjectArray): SLTCloudNoise - {ObjectID = 12}
					UV Map(Type): Position - {TypeID = 2}
					Map Local(Toggle): False
					Map Space(Type): World - {TypeID = 0}
					Map Generate Space(Type): Object - {TypeID = 1}
					Map Inverted(Toggle): True
					Map UV Index(Float): 1
					Map Direction(Type): Normal - {TypeID = 0}
					Map Screen Space(Type): Screen Position - {TypeID = 4}
					Use Fadeout(Toggle): False
					Fadeout Limit Min(Float): 0
					Fadeout Limit Max(Float): 10
					Fadeout Start(Float): 3
					Fadeout End(Float): 5
					Use Alpha(Toggle): False
					Alpha Blend Mode(Type): Blend - {TypeID = 0}
					Mix Amount(Float): 1
					Mix Type(Type): Multiply - {TypeID = 3}
					Stencil(ObjectArray): SSNone - {ObjectID = -1}
					Weird Noise Dimensions(Type): 3D - {TypeID = 2}
					Smoother(Toggle): True
					Gamma Correct(Toggle): True
					Noise Dimensions(Type):  - {TypeID = 1}
					Image Based(Toggle): False
					Color(Vec): 0.627451,0.8,0.8823529,1
					Begin Shader Effect
						TypeS(Text): "SSEUVOffset"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						X Offset(Float): 0
						Y Offset(Float): 0
						Z Offset(Float): 366.5168 - {Input = 10}
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEUVScale"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Seperate(Toggle): False
						X Scale(Float): 30
						Y Scale(Float): 30
						Z Scale(Float): 30
					End Shader Effect

				End Shader Layer

				Begin Shader Layer
					Layer Name(Text): "Sparkles 2"
					Layer Type(ObjectArray): SLTPrevious - {ObjectID = 3}
					UV Map(Type): UV Map - {TypeID = 0}
					Map Local(Toggle): False
					Map Space(Type): World - {TypeID = 0}
					Map Generate Space(Type): Object - {TypeID = 1}
					Map Inverted(Toggle): True
					Map UV Index(Float): 1
					Map Direction(Type): Normal - {TypeID = 0}
					Map Screen Space(Type): Screen Position - {TypeID = 4}
					Use Fadeout(Toggle): False
					Fadeout Limit Min(Float): 0
					Fadeout Limit Max(Float): 10
					Fadeout Start(Float): 3
					Fadeout End(Float): 5
					Use Alpha(Toggle): False
					Alpha Blend Mode(Type): Blend - {TypeID = 0}
					Mix Amount(Float): 1
					Mix Type(Type): Mix - {TypeID = 0}
					Stencil(ObjectArray): SSNone - {ObjectID = -1}
					Color(Vec): 0.627451,0.8,0.8823529,1
					Begin Shader Effect
						TypeS(Text): "SSEMathAdd"
						IsVisible(Toggle): False
						UseAlpha(Float): 0
						Add(Float): 0.08
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEMathRound"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Split Number(Float): 1
					End Shader Effect

				End Shader Layer

			End Shader Layer List

			Begin Shader Layer List

				LayerListUniqueName(Text): "Mask3"
				LayerListName(Text): "Squares"
				Is Mask(Toggle): True
				EndTag(Text): "r"

				Begin Shader Layer
					Layer Name(Text): "Mask1 Copy Copy"
					Layer Type(ObjectArray): SLTPerlinNoise - {ObjectID = 10}
					UV Map(Type): Position - {TypeID = 2}
					Map Local(Toggle): False
					Map Space(Type): World - {TypeID = 0}
					Map Generate Space(Type): Object - {TypeID = 1}
					Map Inverted(Toggle): True
					Map UV Index(Float): 1
					Map Direction(Type): Normal - {TypeID = 0}
					Map Screen Space(Type): Screen Position - {TypeID = 4}
					Use Fadeout(Toggle): False
					Fadeout Limit Min(Float): 0
					Fadeout Limit Max(Float): 10
					Fadeout Start(Float): 3
					Fadeout End(Float): 5
					Use Alpha(Toggle): False
					Alpha Blend Mode(Type): Blend - {TypeID = 0}
					Mix Amount(Float): 1
					Mix Type(Type): Mix - {TypeID = 0}
					Stencil(ObjectArray): SSNone - {ObjectID = -1}
					Noise Dimensions(Type): 3D - {TypeID = 1}
					Image Based(Toggle): False
					Gamma Correct(Toggle): True
					Color(Vec): 0.627451,0.8,0.8823529,1
					Begin Shader Effect
						TypeS(Text): "SSEUVOffset"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						X Offset(Float): 0
						Y Offset(Float): 0
						Z Offset(Float): 368.1882 - {Input = 9}
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEUVScale"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Seperate(Toggle): False
						X Scale(Float): 1.5
						Y Scale(Float): 1.5
						Z Scale(Float): 1.5
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEPixelate"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Seperate(Toggle): False
						X Size(Float): 0.1250099
						Y Size(Float): 0.1250099
						Z Size(Float): 0.1250099
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEMathPow"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Power(Float): 2
					End Shader Effect

				End Shader Layer

			End Shader Layer List

			Begin Shader Layer List

				LayerListUniqueName(Text): "Mask4"
				LayerListName(Text): "Lines"
				Is Mask(Toggle): True
				EndTag(Text): "r"

				Begin Shader Layer
					Layer Name(Text): "Lines 2"
					Layer Type(ObjectArray): SLTLiteral - {ObjectID = 4}
					UV Map(Type): Position - {TypeID = 2}
					Map Local(Toggle): False
					Map Space(Type): Object - {TypeID = 1}
					Map Generate Space(Type): Object - {TypeID = 1}
					Map Inverted(Toggle): True
					Map UV Index(Float): 1
					Map Direction(Type): Normal - {TypeID = 0}
					Map Screen Space(Type): Screen Position - {TypeID = 4}
					Use Fadeout(Toggle): False
					Fadeout Limit Min(Float): 0
					Fadeout Limit Max(Float): 10
					Fadeout Start(Float): 3
					Fadeout End(Float): 5
					Use Alpha(Toggle): False
					Alpha Blend Mode(Type): Blend - {TypeID = 0}
					Mix Amount(Float): 1
					Mix Type(Type): Mix - {TypeID = 0}
					Stencil(ObjectArray): SSNone - {ObjectID = -1}
					Color(Vec): 0.627451,0.8,0.8823529,1
					Begin Shader Effect
						TypeS(Text): "SSEUVScale"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Seperate(Toggle): False
						X Scale(Float): 50
						Y Scale(Float): 50
						Z Scale(Float): 50
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEUVOffset"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						X Offset(Float): 371.105 - {Input = 11}
						Y Offset(Float): 0
						Z Offset(Float): 0
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEMathSin"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						0-1(Toggle): True
					End Shader Effect

				End Shader Layer

				Begin Shader Layer
					Layer Name(Text): "Lines Copy 2"
					Layer Type(ObjectArray): SLTLiteral - {ObjectID = 4}
					UV Map(Type): Position - {TypeID = 2}
					Map Local(Toggle): False
					Map Space(Type): Object - {TypeID = 1}
					Map Generate Space(Type): Object - {TypeID = 1}
					Map Inverted(Toggle): True
					Map UV Index(Float): 1
					Map Direction(Type): Normal - {TypeID = 0}
					Map Screen Space(Type): Screen Position - {TypeID = 4}
					Use Fadeout(Toggle): False
					Fadeout Limit Min(Float): 0
					Fadeout Limit Max(Float): 10
					Fadeout Start(Float): 3
					Fadeout End(Float): 5
					Use Alpha(Toggle): False
					Alpha Blend Mode(Type): Blend - {TypeID = 0}
					Mix Amount(Float): 1
					Mix Type(Type): Multiply - {TypeID = 3}
					Stencil(ObjectArray): SSNone - {ObjectID = -1}
					Color(Vec): 0.627451,0.8,0.8823529,1
					Begin Shader Effect
						TypeS(Text): "SSEUVScale"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Seperate(Toggle): False
						X Scale(Float): 20
						Y Scale(Float): 20
						Z Scale(Float): 20
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEUVOffset"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						X Offset(Float): 371.105 - {Input = 11}
						Y Offset(Float): 0
						Z Offset(Float): 0
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEMathSin"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						0-1(Toggle): False
					End Shader Effect

				End Shader Layer

				Begin Shader Layer
					Layer Name(Text): "Lines2"
					Layer Type(ObjectArray): SLTPrevious - {ObjectID = 3}
					UV Map(Type): UV Map - {TypeID = 0}
					Map Local(Toggle): False
					Map Space(Type): World - {TypeID = 0}
					Map Generate Space(Type): Object - {TypeID = 1}
					Map Inverted(Toggle): True
					Map UV Index(Float): 1
					Map Direction(Type): Normal - {TypeID = 0}
					Map Screen Space(Type): Screen Position - {TypeID = 4}
					Use Fadeout(Toggle): False
					Fadeout Limit Min(Float): 0
					Fadeout Limit Max(Float): 10
					Fadeout Start(Float): 3
					Fadeout End(Float): 5
					Use Alpha(Toggle): False
					Alpha Blend Mode(Type): Blend - {TypeID = 0}
					Mix Amount(Float): 1
					Mix Type(Type): Mix - {TypeID = 0}
					Stencil(ObjectArray): SSNone - {ObjectID = -1}
					Color(Vec): 0.627451,0.8,0.8823529,1
					Begin Shader Effect
						TypeS(Text): "SSEMathClamp"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Min(Float): 0
						Max(Float): 1
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEMathPow"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Power(Float): 6
					End Shader Effect

				End Shader Layer

			End Shader Layer List

			Begin Shader Layer List

				LayerListUniqueName(Text): "Mask5"
				LayerListName(Text): "Pull"
				Is Mask(Toggle): True
				EndTag(Text): "g"

				Begin Shader Layer
					Layer Name(Text): "Pull"
					Layer Type(ObjectArray): SLTLiteral - {ObjectID = 4}
					UV Map(Type): Position - {TypeID = 2}
					Map Local(Toggle): False
					Map Space(Type): World - {TypeID = 0}
					Map Generate Space(Type): Object - {TypeID = 1}
					Map Inverted(Toggle): True
					Map UV Index(Float): 1
					Map Direction(Type): Normal - {TypeID = 0}
					Map Screen Space(Type): Screen Position - {TypeID = 4}
					Use Fadeout(Toggle): False
					Fadeout Limit Min(Float): 0
					Fadeout Limit Max(Float): 10
					Fadeout Start(Float): 3
					Fadeout End(Float): 5
					Use Alpha(Toggle): False
					Alpha Blend Mode(Type): Blend - {TypeID = 0}
					Mix Amount(Float): 1
					Mix Type(Type): Mix - {TypeID = 0}
					Stencil(ObjectArray): SSNone - {ObjectID = -1}
					Color(Vec): 0.627451,0.8,0.8823529,1
					Begin Shader Effect
						TypeS(Text): "SSEInvert"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEMathSub"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Subtract(Float): 0.7 - {Input = 7}
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEMathClamp"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Min(Float): 0
						Max(Float): 1
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEMathPow"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Power(Float): 4
					End Shader Effect

				End Shader Layer

			End Shader Layer List

			Begin Shader Layer List

				LayerListUniqueName(Text): "Mask6"
				LayerListName(Text): "Flicker"
				Is Mask(Toggle): True
				EndTag(Text): "r"

				Begin Shader Layer
					Layer Name(Text): "Lines Copy"
					Layer Type(ObjectArray): SLTLiteral - {ObjectID = 4}
					UV Map(Type): Position - {TypeID = 2}
					Map Local(Toggle): False
					Map Space(Type): World - {TypeID = 0}
					Map Generate Space(Type): Object - {TypeID = 1}
					Map Inverted(Toggle): True
					Map UV Index(Float): 1
					Map Direction(Type): Normal - {TypeID = 0}
					Map Screen Space(Type): Screen Position - {TypeID = 4}
					Use Fadeout(Toggle): False
					Fadeout Limit Min(Float): 0
					Fadeout Limit Max(Float): 10
					Fadeout Start(Float): 3
					Fadeout End(Float): 5
					Use Alpha(Toggle): False
					Alpha Blend Mode(Type): Blend - {TypeID = 0}
					Mix Amount(Float): 1
					Mix Type(Type): Mix - {TypeID = 0}
					Stencil(ObjectArray): SSNone - {ObjectID = -1}
					Color(Vec): 0.627451,0.8,0.8823529,1
					Begin Shader Effect
						TypeS(Text): "SSEUVOffset"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						X Offset(Float): 744.3115 - {Input = 12}
						Y Offset(Float): 0
						Z Offset(Float): 0
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEUVScale"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Seperate(Toggle): False
						X Scale(Float): 0.1
						Y Scale(Float): 0.1
						Z Scale(Float): 0.1
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEMathSin"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						0-1(Toggle): True
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEMathPow"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Power(Float): 240
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEMathRound"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Split Number(Float): 1
					End Shader Effect

				End Shader Layer

				Begin Shader Layer
					Layer Name(Text): "Flicker"
					Layer Type(ObjectArray): SLTColor - {ObjectID = 0}
					UV Map(Type): UV Map - {TypeID = 0}
					Map Local(Toggle): False
					Map Space(Type): World - {TypeID = 0}
					Map Generate Space(Type): Object - {TypeID = 1}
					Map Inverted(Toggle): True
					Map UV Index(Float): 1
					Map Direction(Type): Normal - {TypeID = 0}
					Map Screen Space(Type): Screen Position - {TypeID = 4}
					Use Fadeout(Toggle): False
					Fadeout Limit Min(Float): 0
					Fadeout Limit Max(Float): 10
					Fadeout Start(Float): 3
					Fadeout End(Float): 5
					Use Alpha(Toggle): False
					Alpha Blend Mode(Type): Blend - {TypeID = 0}
					Mix Amount(Float): 0 - {Input = 5}
					Mix Type(Type): Mix - {TypeID = 0}
					Stencil(ObjectArray): SSNone - {ObjectID = -1}
					Color(Vec): 1,1,1,1
				End Shader Layer

			End Shader Layer List

			Begin Shader Layer List

				LayerListUniqueName(Text): "Mask7"
				LayerListName(Text): "FlickerNoise"
				Is Mask(Toggle): True
				EndTag(Text): "r"

				Begin Shader Layer
					Layer Name(Text): "FlickerNoise"
					Layer Type(ObjectArray): SLTColor - {ObjectID = 0}
					UV Map(Type): UV Map - {TypeID = 0}
					Map Local(Toggle): False
					Map Space(Type): World - {TypeID = 0}
					Map Generate Space(Type): Object - {TypeID = 1}
					Map Inverted(Toggle): True
					Map UV Index(Float): 1
					Map Direction(Type): Normal - {TypeID = 0}
					Map Screen Space(Type): Screen Position - {TypeID = 4}
					Use Fadeout(Toggle): False
					Fadeout Limit Min(Float): 0
					Fadeout Limit Max(Float): 10
					Fadeout Start(Float): 3
					Fadeout End(Float): 5
					Use Alpha(Toggle): False
					Alpha Blend Mode(Type): Blend - {TypeID = 0}
					Mix Amount(Float): 1
					Mix Type(Type): Mix - {TypeID = 0}
					Stencil(ObjectArray): SSNone - {ObjectID = -1}
					Color(Vec): 0,0,0,1
				End Shader Layer

				Begin Shader Layer
					Layer Name(Text): "FlickerNoise2"
					Layer Type(ObjectArray): SLTPerlinNoise - {ObjectID = 10}
					UV Map(Type): UV Map - {TypeID = 0}
					Map Local(Toggle): False
					Map Space(Type): World - {TypeID = 0}
					Map Generate Space(Type): Object - {TypeID = 1}
					Map Inverted(Toggle): True
					Map UV Index(Float): 1
					Map Direction(Type): Normal - {TypeID = 0}
					Map Screen Space(Type): Screen Position - {TypeID = 4}
					Use Fadeout(Toggle): False
					Fadeout Limit Min(Float): 0
					Fadeout Limit Max(Float): 10
					Fadeout Start(Float): 3
					Fadeout End(Float): 5
					Use Alpha(Toggle): False
					Alpha Blend Mode(Type): Blend - {TypeID = 0}
					Mix Amount(Float): 1
					Mix Type(Type): Mix - {TypeID = 0}
					Stencil(ObjectArray): Flicker - {ObjectID = 6}
					Noise Dimensions(Type): 2D - {TypeID = 0}
					Image Based(Toggle): False
					Gamma Correct(Toggle): False
					Color(Vec): 0.627451,0.8,0.8823529,1
					Begin Shader Effect
						TypeS(Text): "SSEUVScale"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Seperate(Toggle): False
						X Scale(Float): 14.52
						Y Scale(Float): 14.52
						Z Scale(Float): 14.52
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEUVOffset"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						X Offset(Float): 373.7134 - {Input = 9}
						Y Offset(Float): 0
						Z Offset(Float): 0
					End Shader Effect

				End Shader Layer

			End Shader Layer List

			Begin Shader Layer List

				LayerListUniqueName(Text): "Mask8"
				LayerListName(Text): "Legacy Position Mask"
				Is Mask(Toggle): True
				EndTag(Text): "rgba"

				Begin Shader Layer
					Layer Name(Text): "Legacy Object Space Position"
					Layer Type(ObjectArray): SLTLiteral - {ObjectID = 4}
					UV Map(Type): Position - {TypeID = 2}
					Map Local(Toggle): False
					Map Space(Type): Object - {TypeID = 1}
					Map Generate Space(Type): Object - {TypeID = 1}
					Map Inverted(Toggle): True
					Map UV Index(Float): 1
					Map Direction(Type): Normal - {TypeID = 0}
					Map Screen Space(Type): Screen Position - {TypeID = 4}
					Use Fadeout(Toggle): False
					Fadeout Limit Min(Float): 0
					Fadeout Limit Max(Float): 10
					Fadeout Start(Float): 3
					Fadeout End(Float): 5
					Use Alpha(Toggle): False
					Alpha Blend Mode(Type): Blend - {TypeID = 0}
					Mix Amount(Float): 1
					Mix Type(Type): Mix - {TypeID = 0}
					Stencil(ObjectArray): SSNone - {ObjectID = -2}
					Color(Vec): 0.5,0.8788302,1,1
				End Shader Layer

			End Shader Layer List

			Begin Shader Layer List

				LayerListUniqueName(Text): "Mask9"
				LayerListName(Text): "Legacy Normal Mask"
				Is Mask(Toggle): True
				EndTag(Text): "rgba"

				Begin Shader Layer
					Layer Name(Text): "Legacy Object Space Normal"
					Layer Type(ObjectArray): SLTLiteral - {ObjectID = 4}
					UV Map(Type): Direction - {TypeID = 4}
					Map Local(Toggle): False
					Map Space(Type): Object - {TypeID = 1}
					Map Generate Space(Type): Object - {TypeID = 1}
					Map Inverted(Toggle): True
					Map UV Index(Float): 1
					Map Direction(Type): Tangent - {TypeID = 1}
					Map Screen Space(Type): Screen Position - {TypeID = 4}
					Use Fadeout(Toggle): False
					Fadeout Limit Min(Float): 0
					Fadeout Limit Max(Float): 10
					Fadeout Start(Float): 3
					Fadeout End(Float): 5
					Use Alpha(Toggle): False
					Alpha Blend Mode(Type): Blend - {TypeID = 0}
					Mix Amount(Float): 1
					Mix Type(Type): Mix - {TypeID = 0}
					Stencil(ObjectArray): SSNone - {ObjectID = -2}
					Color(Vec): 0.5,0.8788302,1,1
				End Shader Layer

			End Shader Layer List

		End Masks

		Begin Shader Pass
			Name(Text): "Pass 1"
			Visible(Toggle): True

			Begin Shader Ingredient

				Type(Text): "ShaderSurfaceEmission"
				User Name(Text): "Emission"
				Use Custom Lighting(Toggle): False
				Use Custom Ambient(Toggle): False
				Alpha Blend Mode(Type): Blend - {TypeID = 0}
				Mix Amount(Float): 1
				Mix Type(Type): Add - {TypeID = 1}
				Use Alpha(Toggle): True
				ShouldLink(Toggle): False

				Begin Shader Layer List

					LayerListUniqueName(Text): "Emission"
					LayerListName(Text): "Emission"
					Is Mask(Toggle): False
					EndTag(Text): "rgba"

					Begin Shader Layer
						Layer Name(Text): "Rim"
						Layer Type(ObjectArray): SLTColor - {ObjectID = 0}
						UV Map(Type): UV Map - {TypeID = 0}
						Map Local(Toggle): False
						Map Space(Type): World - {TypeID = 0}
						Map Generate Space(Type): Object - {TypeID = 1}
						Map Inverted(Toggle): True
						Map UV Index(Float): 1
						Map Direction(Type): Normal - {TypeID = 0}
						Map Screen Space(Type): Screen Position - {TypeID = 4}
						Use Fadeout(Toggle): False
						Fadeout Limit Min(Float): 0
						Fadeout Limit Max(Float): 10
						Fadeout Start(Float): 3
						Fadeout End(Float): 5
						Use Alpha(Toggle): False
						Alpha Blend Mode(Type): Blend - {TypeID = 0}
						Mix Amount(Float): 1
						Mix Type(Type): Add - {TypeID = 1}
						Stencil(ObjectArray): Rim - {ObjectID = 0}
						Color(Vec): 0,0.5527849,1,1 - {Input = 0}
						Begin Shader Effect
							TypeS(Text): "SSEMathMul"
							IsVisible(Toggle): True
							UseAlpha(Float): 0
							Multiply(Float): 3
						End Shader Effect

					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Dust"
						Layer Type(ObjectArray): SLTColor - {ObjectID = 0}
						UV Map(Type): UV Map - {TypeID = 0}
						Map Local(Toggle): False
						Map Space(Type): World - {TypeID = 0}
						Map Generate Space(Type): Object - {TypeID = 1}
						Map Inverted(Toggle): True
						Map UV Index(Float): 1
						Map Direction(Type): Normal - {TypeID = 0}
						Map Screen Space(Type): Screen Position - {TypeID = 4}
						Use Fadeout(Toggle): False
						Fadeout Limit Min(Float): 0
						Fadeout Limit Max(Float): 10
						Fadeout Start(Float): 3
						Fadeout End(Float): 5
						Use Alpha(Toggle): False
						Alpha Blend Mode(Type): Blend - {TypeID = 0}
						Mix Amount(Float): 1
						Mix Type(Type): Add - {TypeID = 1}
						Stencil(ObjectArray): Dust - {ObjectID = 1}
						Color(Vec): 0.3647059,0,0.5960785,1 - {Input = 1}
					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Sparkles 2 2"
						Layer Type(ObjectArray): SLTColor - {ObjectID = 0}
						UV Map(Type): UV Map - {TypeID = 0}
						Map Local(Toggle): False
						Map Space(Type): World - {TypeID = 0}
						Map Generate Space(Type): Object - {TypeID = 1}
						Map Inverted(Toggle): True
						Map UV Index(Float): 1
						Map Direction(Type): Normal - {TypeID = 0}
						Map Screen Space(Type): Screen Position - {TypeID = 4}
						Use Fadeout(Toggle): False
						Fadeout Limit Min(Float): 0
						Fadeout Limit Max(Float): 10
						Fadeout Start(Float): 3
						Fadeout End(Float): 5
						Use Alpha(Toggle): False
						Alpha Blend Mode(Type): Blend - {TypeID = 0}
						Mix Amount(Float): 1
						Mix Type(Type): Add - {TypeID = 1}
						Stencil(ObjectArray): Sparkles - {ObjectID = 2}
						Color(Vec): 0.01960784,0.1058824,1,1 - {Input = 2}
						Begin Shader Effect
							TypeS(Text): "SSEMathMul"
							IsVisible(Toggle): True
							UseAlpha(Float): 0
							Multiply(Float): 6
						End Shader Effect

					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Squares 2"
						Layer Type(ObjectArray): SLTColor - {ObjectID = 0}
						UV Map(Type): UV Map - {TypeID = 0}
						Map Local(Toggle): False
						Map Space(Type): World - {TypeID = 0}
						Map Generate Space(Type): Object - {TypeID = 1}
						Map Inverted(Toggle): True
						Map UV Index(Float): 1
						Map Direction(Type): Normal - {TypeID = 0}
						Map Screen Space(Type): Screen Position - {TypeID = 4}
						Use Fadeout(Toggle): False
						Fadeout Limit Min(Float): 0
						Fadeout Limit Max(Float): 10
						Fadeout Start(Float): 3
						Fadeout End(Float): 5
						Use Alpha(Toggle): False
						Alpha Blend Mode(Type): Blend - {TypeID = 0}
						Mix Amount(Float): 1
						Mix Type(Type): Add - {TypeID = 1}
						Stencil(ObjectArray): Squares - {ObjectID = 3}
						Color(Vec): 1,0.3921569,0,1 - {Input = 3}
					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Lines"
						Layer Type(ObjectArray): SLTColor - {ObjectID = 0}
						UV Map(Type): UV Map - {TypeID = 0}
						Map Local(Toggle): False
						Map Space(Type): World - {TypeID = 0}
						Map Generate Space(Type): Object - {TypeID = 1}
						Map Inverted(Toggle): True
						Map UV Index(Float): 1
						Map Direction(Type): Normal - {TypeID = 0}
						Map Screen Space(Type): Screen Position - {TypeID = 4}
						Use Fadeout(Toggle): False
						Fadeout Limit Min(Float): 0
						Fadeout Limit Max(Float): 10
						Fadeout Start(Float): 3
						Fadeout End(Float): 5
						Use Alpha(Toggle): False
						Alpha Blend Mode(Type): Blend - {TypeID = 0}
						Mix Amount(Float): 1
						Mix Type(Type): Add - {TypeID = 1}
						Stencil(ObjectArray): Lines - {ObjectID = 4}
						Color(Vec): 1,0.4745098,0,1 - {Input = 4}
					End Shader Layer

				End Shader Layer List

			End Shader Ingredient


			Begin Shader Ingredient

				Type(Text): "ShaderSurfaceSpecular"
				User Name(Text): "Specular"
				Use Custom Lighting(Toggle): False
				Use Custom Ambient(Toggle): False
				Alpha Blend Mode(Type): Blend - {TypeID = 0}
				Mix Amount(Float): 1
				Mix Type(Type): Add - {TypeID = 1}
				Use Alpha(Toggle): True
				ShouldLink(Toggle): False
				Specular Type(Type): Unity Standard - {TypeID = 1}
				Roughness Or Smoothness(Type): Smoothness - {TypeID = 0}
				Smoothness(Float): 0.986
				Roughness(Float): 0.7
				Light Size(Float): 0
				Spec Energy Conserve(Toggle): True
				Spec Offset(Float): 0
				PBR Quality(Type): Auto - {TypeID = 0}
				PBR Model(Type): Specular - {TypeID = 0}
				Use Tangents(Toggle): False
				Use Ambient(Toggle): True
				Use Roughness Darkening(Toggle): True
				Use Fresnel(Toggle): True

				Begin Shader Layer List

					LayerListUniqueName(Text): "Specular"
					LayerListName(Text): "Specular Color"
					Is Mask(Toggle): False
					EndTag(Text): "rgb"

					Begin Shader Layer
						Layer Name(Text): "Specular Color"
						Layer Type(ObjectArray): SLTColor - {ObjectID = 0}
						UV Map(Type): UV Map - {TypeID = 0}
						Map Local(Toggle): False
						Map Space(Type): World - {TypeID = 0}
						Map Generate Space(Type): Object - {TypeID = 1}
						Map Inverted(Toggle): True
						Map UV Index(Float): 1
						Map Direction(Type): Normal - {TypeID = 0}
						Map Screen Space(Type): Screen Position - {TypeID = 4}
						Use Fadeout(Toggle): False
						Fadeout Limit Min(Float): 0
						Fadeout Limit Max(Float): 10
						Fadeout Start(Float): 3
						Fadeout End(Float): 5
						Use Alpha(Toggle): False
						Alpha Blend Mode(Type): Blend - {TypeID = 0}
						Mix Amount(Float): 1
						Mix Type(Type): Mix - {TypeID = 0}
						Stencil(ObjectArray): SSNone - {ObjectID = -2}
						Color(Vec): 0,0.484,1,1
					End Shader Layer

				End Shader Layer List

			End Shader Ingredient


			Begin Shader Ingredient

				Type(Text): "ShaderSurfaceTransparency"
				User Name(Text): "Set Alpha Channel"
				Use Custom Lighting(Toggle): False
				Use Custom Ambient(Toggle): False
				Alpha Blend Mode(Type): Replace - {TypeID = 3}
				Mix Amount(Float): 1
				Mix Type(Type): Mix - {TypeID = 0}
				Use Alpha(Toggle): True
				ShouldLink(Toggle): False

				Begin Shader Layer List

					LayerListUniqueName(Text): "Transparency"
					LayerListName(Text): "Transparency"
					Is Mask(Toggle): False
					EndTag(Text): "r"

					Begin Shader Layer
						Layer Name(Text): "Alpha"
						Layer Type(ObjectArray): SLTLiteral - {ObjectID = 4}
						UV Map(Type): Generate - {TypeID = 1}
						Map Local(Toggle): False
						Map Space(Type): World - {TypeID = 0}
						Map Generate Space(Type): Rim - {TypeID = 2}
						Map Inverted(Toggle): True
						Map UV Index(Float): 1
						Map Direction(Type): Normal - {TypeID = 0}
						Map Screen Space(Type): Screen Position - {TypeID = 4}
						Use Fadeout(Toggle): False
						Fadeout Limit Min(Float): 0
						Fadeout Limit Max(Float): 10
						Fadeout Start(Float): 3
						Fadeout End(Float): 5
						Use Alpha(Toggle): False
						Alpha Blend Mode(Type): Blend - {TypeID = 0}
						Mix Amount(Float): 1
						Mix Type(Type): Mix - {TypeID = 0}
						Stencil(ObjectArray): SSNone - {ObjectID = -1}
						Color(Vec): 0.627451,0.8,0.8823529,0
					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Sparkles"
						Layer Type(ObjectArray): SLTNumber - {ObjectID = 7}
						UV Map(Type): UV Map - {TypeID = 0}
						Map Local(Toggle): False
						Map Space(Type): World - {TypeID = 0}
						Map Generate Space(Type): Object - {TypeID = 1}
						Map Inverted(Toggle): True
						Map UV Index(Float): 1
						Map Direction(Type): Normal - {TypeID = 0}
						Map Screen Space(Type): Screen Position - {TypeID = 4}
						Use Fadeout(Toggle): False
						Fadeout Limit Min(Float): 0
						Fadeout Limit Max(Float): 10
						Fadeout Start(Float): 3
						Fadeout End(Float): 5
						Use Alpha(Toggle): False
						Alpha Blend Mode(Type): Blend - {TypeID = 0}
						Mix Amount(Float): 1
						Mix Type(Type): Add - {TypeID = 1}
						Stencil(ObjectArray): Sparkles - {ObjectID = 2}
						Number(Float): 1
						Color(Vec): 0.5,0.8823529,1,1
					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Squares"
						Layer Type(ObjectArray): SLTNumber - {ObjectID = 7}
						UV Map(Type): UV Map - {TypeID = 0}
						Map Local(Toggle): False
						Map Space(Type): World - {TypeID = 0}
						Map Generate Space(Type): Object - {TypeID = 1}
						Map Inverted(Toggle): True
						Map UV Index(Float): 1
						Map Direction(Type): Normal - {TypeID = 0}
						Map Screen Space(Type): Screen Position - {TypeID = 4}
						Use Fadeout(Toggle): False
						Fadeout Limit Min(Float): 0
						Fadeout Limit Max(Float): 10
						Fadeout Start(Float): 3
						Fadeout End(Float): 5
						Use Alpha(Toggle): False
						Alpha Blend Mode(Type): Blend - {TypeID = 0}
						Mix Amount(Float): 1
						Mix Type(Type): Add - {TypeID = 1}
						Stencil(ObjectArray): Squares - {ObjectID = 3}
						Number(Float): 1
						Color(Vec): 0.5,0.8823529,1,1
					End Shader Layer

				End Shader Layer List

			End Shader Ingredient

			Geometry Ingredients

			Begin Shader Ingredient

				Type(Text): "ShaderGeometryModifierMisc"
				User Name(Text): "Misc Settings"
				Mix Amount(Float): 1
				ShouldLink(Toggle): False
				ZWriteMode(Type): Auto - {TypeID = 0}
				ZTestMode(Type): Closer or Equal - {TypeID = 3}
				CullMode(Type): Back Faces - {TypeID = 0}
				ShaderModel(Type): Shader Model 3.0 - {TypeID = 3}
				Use Fog(Toggle): True
				Use Lightmaps(Toggle): True
				Use Forward Full Shadows(Toggle): True
				Use Forward Vertex Lights(Toggle): True
				Use Shadows(Toggle): True
				Generate Forward Base Pass(Toggle): True
				Generate Forward Add Pass(Toggle): False
				Generate Deferred Pass(Toggle): False
				Generate Shadow Caster Pass(Toggle): True

			End Shader Ingredient


			Begin Shader Ingredient

				Type(Text): "ShaderGeometryModifierTransparency"
				User Name(Text): "Transparency"
				Mix Amount(Float): 1
				ShouldLink(Toggle): False
				Transparency Type(Type): Fade - {TypeID = 1}
				Transparency ZWrite(Type): Full - {TypeID = 2}
				Shadow Type Fade(Type): Dithered - {TypeID = 2}
				Cutout Amount(Float): 0
				Transparency(Float): 1
				Blend Mode(Type): Mix - {TypeID = 0}

			End Shader Ingredient


			Begin Shader Ingredient

				Type(Text): "ShaderGeometryModifierDisplacement"
				User Name(Text): "Displace"
				Mix Amount(Float): 1
				ShouldLink(Toggle): False
				Direction(Type): Normal - {TypeID = 0}
				Coordinate Space(Type): Object - {TypeID = 0}
				Strength(Float): -1
				MidLevel(Float): 0
				Independent XYZ(Toggle): True

				Begin Shader Layer List

					LayerListUniqueName(Text): "Surface"
					LayerListName(Text): "Displacement"
					Is Mask(Toggle): False
					EndTag(Text): "rgb"

					Begin Shader Layer
						Layer Name(Text): "Displacement"
						Layer Type(ObjectArray): SLTNumber - {ObjectID = 7}
						UV Map(Type): UV Map - {TypeID = 0}
						Map Local(Toggle): False
						Map Space(Type): World - {TypeID = 0}
						Map Generate Space(Type): Object - {TypeID = 1}
						Map Inverted(Toggle): True
						Map UV Index(Float): 1
						Map Direction(Type): Normal - {TypeID = 0}
						Map Screen Space(Type): Screen Position - {TypeID = 4}
						Use Fadeout(Toggle): False
						Fadeout Limit Min(Float): 0
						Fadeout Limit Max(Float): 10
						Fadeout Start(Float): 3
						Fadeout End(Float): 5
						Use Alpha(Toggle): False
						Alpha Blend Mode(Type): Blend - {TypeID = 0}
						Mix Amount(Float): 1
						Mix Type(Type): Mix - {TypeID = 0}
						Stencil(ObjectArray): SSNone - {ObjectID = -2}
						Number(Float): 0
						Color(Vec): 0,0,0,1
					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Displacement2"
						Layer Type(ObjectArray): SLTVector - {ObjectID = 8}
						UV Map(Type): UV Map - {TypeID = 0}
						Map Local(Toggle): False
						Map Space(Type): World - {TypeID = 0}
						Map Generate Space(Type): Object - {TypeID = 1}
						Map Inverted(Toggle): True
						Map UV Index(Float): 1
						Map Direction(Type): Normal - {TypeID = 0}
						Map Screen Space(Type): Screen Position - {TypeID = 4}
						Use Fadeout(Toggle): False
						Fadeout Limit Min(Float): 0
						Fadeout Limit Max(Float): 10
						Fadeout Start(Float): 3
						Fadeout End(Float): 5
						Use Alpha(Toggle): False
						Alpha Blend Mode(Type): Blend - {TypeID = 0}
						Mix Amount(Float): 1
						Mix Type(Type): Add - {TypeID = 1}
						Stencil(ObjectArray): FlickerNoise - {ObjectID = 7}
						Color(Vec): 0.5441177,0,0.5441177,1 - {Input = 6}
						ColorOrFloat(Type): Color - {TypeID = 0}
						FillerInput1(Toggle): False
						FillerInput2(Toggle): False
						FillerInput3(Toggle): False
					End Shader Layer

				End Shader Layer List

			End Shader Ingredient


			Begin Shader Ingredient

				Type(Text): "ShaderGeometryModifierSet"
				User Name(Text): "Set"
				Mix Amount(Float): 1
				ShouldLink(Toggle): False
				SetWhatAaahh(Type): Local Position - {TypeID = 0}

				Begin Shader Layer List

					LayerListUniqueName(Text): "Surface"
					LayerListName(Text): "Vertex"
					Is Mask(Toggle): False
					EndTag(Text): "rgb"

					Begin Shader Layer
						Layer Name(Text): "Vertex"
						Layer Type(ObjectArray): SLTColor - {ObjectID = 0}
						UV Map(Type): UV Map - {TypeID = 0}
						Map Local(Toggle): False
						Map Space(Type): World - {TypeID = 0}
						Map Generate Space(Type): Object - {TypeID = 1}
						Map Inverted(Toggle): True
						Map UV Index(Float): 1
						Map Direction(Type): Normal - {TypeID = 0}
						Map Screen Space(Type): Screen Position - {TypeID = 4}
						Use Fadeout(Toggle): False
						Fadeout Limit Min(Float): 0
						Fadeout Limit Max(Float): 10
						Fadeout Start(Float): 3
						Fadeout End(Float): 5
						Use Alpha(Toggle): False
						Alpha Blend Mode(Type): Blend - {TypeID = 0}
						Mix Amount(Float): 1
						Mix Type(Type): Mix - {TypeID = 0}
						Stencil(ObjectArray): Pull - {ObjectID = 5}
						Color(Vec): 0,1,0,1 - {Input = 8}
						Begin Shader Effect
							TypeS(Text): "SSEMaskMultiply"
							IsVisible(Toggle): True
							UseAlpha(Float): 0
							Values(ObjectArray): Legacy_Position_Mask - {ObjectID = 8}
						End Shader Effect

					End Shader Layer

				End Shader Layer List

			End Shader Ingredient

		End Shader Pass

	End Shader Base
End Shader Sandwich Shader
*/
