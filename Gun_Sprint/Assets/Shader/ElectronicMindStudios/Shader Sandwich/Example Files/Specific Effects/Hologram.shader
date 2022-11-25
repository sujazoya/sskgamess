// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader Sandwich/Specific/Hologram" {//The Shaders Name
//The inputs shown in the material panel
Properties {
	_Rim_Color ("Rim Color", Color) = (0,0.5527849,1,1)
	_Dust_Color ("Dust Color", Color) = (0.1901623,0,0.2205882,1)
	_Sparkles_Color ("Sparkles Color", Color) = (0.125,0.4568966,1,1)
	_Squares_Color ("Squares Color", Color) = (0.4779412,0,0.04944224,1)
	_Lines_Color ("Lines Color", Color) = (0.5310345,0,1,1)
	_Force_Flicker ("Force Flicker", Range(0.000000000,1.000000000)) = 0.382716000
	_Flicker_Direction ("Flicker Direction", Color) = (0.1176471,0,0.1176471,1)
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
	Blend One OneMinusSrcAlpha
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



















	float RandomNoise3D(float3 co){
		return frac(sin( dot(co.xyz ,float3(12.9898,78.233,45.5432) )) * 43758.5453);
	}
	float RandomNoise2D(float2 co){
		return frac(sin( dot(co.xy ,float2(12.9898,78.233) )) * 43758.5453);
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
half4 Set_Alpha_Channel ( VertexData vd, half4 outputColor){
	float Surface = 0;
		//Generate layers for the Transparency channel.
			//Generate Layer: Legacy Transparency Base
				//Sample parts of the layer:
					half4 Legacy_Transparency_BaseTransparency_Sample1 = 0;
	
	Surface = Legacy_Transparency_BaseTransparency_Sample1.a;//2
	
	
			//Generate Layer: Alpha
				//Sample parts of the layer:
					half4 AlphaTransparency_Sample1 = float4(float3((1-dot(vd.worldNormal, vd.worldViewDir)).rrr),1);
	
				//Apply Effects:
					AlphaTransparency_Sample1.rgb = pow(AlphaTransparency_Sample1.rgb,2);
					AlphaTransparency_Sample1.rgb = clamp(AlphaTransparency_Sample1.rgb,0,1);
					AlphaTransparency_Sample1.rgb = (float3(1,1,1)-AlphaTransparency_Sample1.rgb);
	
	Surface = lerp(Surface,AlphaTransparency_Sample1.a,0.4086956);//3
	
	
	
	return float4(outputColor.rgb,Surface);
	
}
float4 Transparency ( float4 outputColor){
	outputColor *= 1;
	return outputColor;
	
}
void Displace ( inout VertexData vd, inout VertexShaderInput v){
	half3 Surface = half3(1,1,1);
		//Generate layers for the Displacement channel.
			//Generate Layer: Legacy Displacement Base
				//Sample parts of the layer:
					half4 Legacy_Displacement_BaseSurface_Sample1 = 0;
	
	Surface = Legacy_Displacement_BaseSurface_Sample1.rgb;//0
	
	
			//Generate Layer: Vertex3
				//Sample parts of the layer:
					half4 Vertex3Surface_Sample1 = _Flicker_Direction;
	
	Surface = lerp(Surface,(Surface + Vertex3Surface_Sample1.rgb),vd.Mask7);//1
	
	
	
	v.vertex.xyz = float3(v.vertex.xyz + v.normal * Surface * 0.7586207);
	
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
			//Generate Layer: Rim
				//Sample parts of the layer:
					half4 RimMask0_Sample1 = float4(float3(1-float3(dot(vd.worldNormal, vd.worldViewDir).rrr).x,float3(dot(vd.worldNormal, vd.worldViewDir).rrr).y,float3(dot(vd.worldNormal, vd.worldViewDir).rrr).z),1);
	
				//Apply Effects:
					RimMask0_Sample1.rgb = pow(RimMask0_Sample1.rgb,3);
	
	Mask0 = RimMask0_Sample1.r;//2
	
	
	return Mask0;
	
}
float Mask_Mask1 ( VertexData vd){
		//Set default mask color
			float Mask1 = 0;
		//Generate layers for the Dust channel.
			//Generate Layer: Mask1
				//Sample parts of the layer:
					half4 Mask1Mask1_Sample1 = GammaToLinear((PerlinNoise3D(((vd.worldPos*float3(30,30,30))+float3(0,0,_Time.y * 0.5))*3)+1)/2);
	
	Mask1 = Mask1Mask1_Sample1.r;//2
	
	
	return Mask1;
	
}
float Mask_Mask2 ( VertexData vd){
		//Set default mask color
			float Mask2 = 0;
		//Generate layers for the Sparkles channel.
			//Generate Layer: Mask1 Copy
				//Sample parts of the layer:
					half4 Mask1_CopyMask2_Sample1 = GammaToLinear((PerlinNoise3D(((vd.worldPos*float3(100,100,100))+float3(0,0,_Time.y * 0.5))*3)+1)/2);
	
	Mask2 = Mask1_CopyMask2_Sample1.r;//2
	
	
			//Generate Layer: Mask1 Copy Copy 2
				//Sample parts of the layer:
					half4 Mask1_Copy_Copy_2Mask2_Sample1 = GammaToLinear((PerlinNoise3D(((vd.worldPos*float3(10,10,10))+float3(0,0,_Time.y * 0.5))*3)+1)/2);
	
	Mask2 = (Mask2 * Mask1_Copy_Copy_2Mask2_Sample1.r);//0
	
	
			//Generate Layer: Sparkles
				//Sample parts of the layer:
					half4 SparklesMask2_Sample1 = float4(Mask2.rrrr);
	
				//Apply Effects:
					SparklesMask2_Sample1.rgb = (SparklesMask2_Sample1.rgb+0.08);
					SparklesMask2_Sample1.rgb = (round(SparklesMask2_Sample1.rgb*1)/1);
	
	Mask2 = SparklesMask2_Sample1.r;//0
	
	
	return Mask2;
	
}
float Mask_Mask3 ( VertexData vd){
		//Set default mask color
			float Mask3 = 0;
		//Generate layers for the Squares channel.
			//Generate Layer: Mask1 Copy Copy
				//Sample parts of the layer:
					half4 Mask1_Copy_CopyMask3_Sample1 = GammaToLinear((PerlinNoise3D((((round(vd.worldPos/float3(0.1250099,0.1250099,0.1250099))*float3(0.1250099,0.1250099,0.1250099))*float3(1.5,1.5,1.5))+float3(0,0,_Time.y * 0.5))*3)+1)/2);
	
	Mask3 = Mask1_Copy_CopyMask3_Sample1.r;//2
	
	
	return Mask3;
	
}
float Mask_Mask4 ( VertexData vd){
		//Set default mask color
			float Mask4 = 0;
		//Generate layers for the Lines channel.
			//Generate Layer: Lines
				//Sample parts of the layer:
					half4 LinesMask4_Sample1 = float4(((vd.worldPos+float3(_Time.y * 0.5,0,0))*float3(50,50,50)),1);
	
				//Apply Effects:
					LinesMask4_Sample1.rgb = ((sin(LinesMask4_Sample1.rgb)+1)/2);
	
	Mask4 = LinesMask4_Sample1.r;//2
	
	
			//Generate Layer: Lines Copy 2
				//Sample parts of the layer:
					half4 Lines_Copy_2Mask4_Sample1 = float4(((vd.worldPos+float3(_Time.y * 0.5,0,0))*float3(20,20,20)),1);
	
				//Apply Effects:
					Lines_Copy_2Mask4_Sample1.rgb = sin(Lines_Copy_2Mask4_Sample1.rgb);
	
	Mask4 = (Mask4 * Lines_Copy_2Mask4_Sample1.r);//0
	
	
			//Generate Layer: Lines2
				//Sample parts of the layer:
					half4 Lines2Mask4_Sample1 = float4(Mask4.rrrr);
	
				//Apply Effects:
					Lines2Mask4_Sample1.rgb = clamp(Lines2Mask4_Sample1.rgb,0,1);
					Lines2Mask4_Sample1.rgb = pow(Lines2Mask4_Sample1.rgb,13.47);
	
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
					PullMask5_Sample1.rgb = pow(PullMask5_Sample1.rgb,8.39);
	
	Mask5 = PullMask5_Sample1.g;//2
	
	
	return Mask5;
	
}
float Mask_Mask6 ( VertexData vd){
		//Set default mask color
			float Mask6 = 0;
		//Generate layers for the Flicker channel.
			//Generate Layer: Lines Copy
				//Sample parts of the layer:
					half4 Lines_CopyMask6_Sample1 = float4(((vd.worldPos*float3(0.1,0.1,0.1))+float3(_Time.z,0,0)),1);
	
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
					half4 FlickerNoise2Mask7_Sample1 = RandomNoise2D((vd.genericTexcoord+float2(_Time.y * 0.5,0)));
	
				//Apply Effects:
					FlickerNoise2Mask7_Sample1.rgb = (FlickerNoise2Mask7_Sample1.rgb*0.57);
					FlickerNoise2Mask7_Sample1.rgb = lerp(float3(0.5,0.5,0.5),FlickerNoise2Mask7_Sample1.rgb,3);
					FlickerNoise2Mask7_Sample1.rgb = clamp(FlickerNoise2Mask7_Sample1.rgb,0,1);
	
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
				half4 outputSet_Alpha_Channel = Set_Alpha_Channel ( vd, outputColor);
				outputColor = half4(outputSet_Alpha_Channel.rgb * outputSet_Alpha_Channel.a, outputSet_Alpha_Channel.a);//11
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
	Blend One OneMinusSrcAlpha
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



















	float RandomNoise3D(float3 co){
		return frac(sin( dot(co.xyz ,float3(12.9898,78.233,45.5432) )) * 43758.5453);
	}
	float RandomNoise2D(float2 co){
		return frac(sin( dot(co.xy ,float2(12.9898,78.233) )) * 43758.5453);
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
half4 Set_Alpha_Channel ( VertexData vd, half4 outputColor){
	float Surface = 0;
		//Generate layers for the Transparency channel.
			//Generate Layer: Legacy Transparency Base
				//Sample parts of the layer:
					half4 Legacy_Transparency_BaseTransparency_Sample1 = 0;
	
	Surface = Legacy_Transparency_BaseTransparency_Sample1.a;//2
	
	
			//Generate Layer: Alpha
				//Sample parts of the layer:
					half4 AlphaTransparency_Sample1 = float4(float3((1-dot(vd.worldNormal, vd.worldViewDir)).rrr),1);
	
				//Apply Effects:
					AlphaTransparency_Sample1.rgb = pow(AlphaTransparency_Sample1.rgb,2);
					AlphaTransparency_Sample1.rgb = clamp(AlphaTransparency_Sample1.rgb,0,1);
					AlphaTransparency_Sample1.rgb = (float3(1,1,1)-AlphaTransparency_Sample1.rgb);
	
	Surface = lerp(Surface,AlphaTransparency_Sample1.a,0.4086956);//3
	
	
	
	return float4(outputColor.rgb,Surface);
	
}
//OutputPremultiplied: True
//UseAlphaGenerate: True
half4 Emission ( VertexData vd){
	half4 Surface = half4(0,0,0,0);
		//Generate layers for the Emission channel.
			//Generate Layer: Emission2
				//Sample parts of the layer:
					half4 Emission2Emission_Sample1 = _Rim_Color;
	
				//Apply Effects:
					Emission2Emission_Sample1.rgb = (Emission2Emission_Sample1.rgb*2);
	
	Surface.rgb = lerp(Surface.rgb,(Surface.rgb + Emission2Emission_Sample1.rgb),vd.Mask0);//6
	
	
			//Generate Layer: Emission3
				//Sample parts of the layer:
					half4 Emission3Emission_Sample1 = _Dust_Color;
	
	Surface.rgb = lerp(Surface.rgb,(Surface.rgb + Emission3Emission_Sample1.rgb),vd.Mask1);//6
	
	
			//Generate Layer: Emission4
				//Sample parts of the layer:
					half4 Emission4Emission_Sample1 = _Sparkles_Color;
	
	Surface.rgb = lerp(Surface.rgb,(Surface.rgb + Emission4Emission_Sample1.rgb),vd.Mask2);//6
	
	
			//Generate Layer: Emission5
				//Sample parts of the layer:
					half4 Emission5Emission_Sample1 = _Squares_Color;
	
	Surface.rgb = lerp(Surface.rgb,(Surface.rgb + Emission5Emission_Sample1.rgb),vd.Mask3);//6
	
	
			//Generate Layer: Emission6
				//Sample parts of the layer:
					half4 Emission6Emission_Sample1 = _Lines_Color;
	
	Surface.rgb = lerp(Surface.rgb,(Surface.rgb + Emission6Emission_Sample1.rgb),vd.Mask4);//6
	
	
			//Generate Layer: Alpha Copy
				//Sample parts of the layer:
					half4 Alpha_CopyEmission_Sample1 = float4(float3((1-dot(vd.worldNormal, vd.worldViewDir)).rrr),1);
	
				//Apply Effects:
					Alpha_CopyEmission_Sample1.rgb = pow(Alpha_CopyEmission_Sample1.rgb,2);
					Alpha_CopyEmission_Sample1.rgb = clamp(Alpha_CopyEmission_Sample1.rgb,0,1);
	
	Surface = lerp(Surface,half4((Surface.rgb * Alpha_CopyEmission_Sample1.rgb), 1),0.5956522);//2
	
	
	return Surface;
}
float4 Transparency ( float4 outputColor){
	outputColor *= 1;
	return outputColor;
	
}
void Displace ( inout VertexData vd, inout VertexShaderInput v){
	half3 Surface = half3(1,1,1);
		//Generate layers for the Displacement channel.
			//Generate Layer: Legacy Displacement Base
				//Sample parts of the layer:
					half4 Legacy_Displacement_BaseSurface_Sample1 = 0;
	
	Surface = Legacy_Displacement_BaseSurface_Sample1.rgb;//0
	
	
			//Generate Layer: Vertex3
				//Sample parts of the layer:
					half4 Vertex3Surface_Sample1 = _Flicker_Direction;
	
	Surface = lerp(Surface,(Surface + Vertex3Surface_Sample1.rgb),vd.Mask7);//1
	
	
	
	v.vertex.xyz = float3(v.vertex.xyz + v.normal * Surface * 0.7586207);
	
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
			//Generate Layer: Rim
				//Sample parts of the layer:
					half4 RimMask0_Sample1 = float4(float3(1-float3(dot(vd.worldNormal, vd.worldViewDir).rrr).x,float3(dot(vd.worldNormal, vd.worldViewDir).rrr).y,float3(dot(vd.worldNormal, vd.worldViewDir).rrr).z),1);
	
				//Apply Effects:
					RimMask0_Sample1.rgb = pow(RimMask0_Sample1.rgb,3);
	
	Mask0 = RimMask0_Sample1.r;//2
	
	
	return Mask0;
	
}
float Mask_Mask1 ( VertexData vd){
		//Set default mask color
			float Mask1 = 0;
		//Generate layers for the Dust channel.
			//Generate Layer: Mask1
				//Sample parts of the layer:
					half4 Mask1Mask1_Sample1 = GammaToLinear((PerlinNoise3D(((vd.worldPos*float3(30,30,30))+float3(0,0,_Time.y * 0.5))*3)+1)/2);
	
	Mask1 = Mask1Mask1_Sample1.r;//2
	
	
	return Mask1;
	
}
float Mask_Mask2 ( VertexData vd){
		//Set default mask color
			float Mask2 = 0;
		//Generate layers for the Sparkles channel.
			//Generate Layer: Mask1 Copy
				//Sample parts of the layer:
					half4 Mask1_CopyMask2_Sample1 = GammaToLinear((PerlinNoise3D(((vd.worldPos*float3(100,100,100))+float3(0,0,_Time.y * 0.5))*3)+1)/2);
	
	Mask2 = Mask1_CopyMask2_Sample1.r;//2
	
	
			//Generate Layer: Mask1 Copy Copy 2
				//Sample parts of the layer:
					half4 Mask1_Copy_Copy_2Mask2_Sample1 = GammaToLinear((PerlinNoise3D(((vd.worldPos*float3(10,10,10))+float3(0,0,_Time.y * 0.5))*3)+1)/2);
	
	Mask2 = (Mask2 * Mask1_Copy_Copy_2Mask2_Sample1.r);//0
	
	
			//Generate Layer: Sparkles
				//Sample parts of the layer:
					half4 SparklesMask2_Sample1 = float4(Mask2.rrrr);
	
				//Apply Effects:
					SparklesMask2_Sample1.rgb = (SparklesMask2_Sample1.rgb+0.08);
					SparklesMask2_Sample1.rgb = (round(SparklesMask2_Sample1.rgb*1)/1);
	
	Mask2 = SparklesMask2_Sample1.r;//0
	
	
	return Mask2;
	
}
float Mask_Mask3 ( VertexData vd){
		//Set default mask color
			float Mask3 = 0;
		//Generate layers for the Squares channel.
			//Generate Layer: Mask1 Copy Copy
				//Sample parts of the layer:
					half4 Mask1_Copy_CopyMask3_Sample1 = GammaToLinear((PerlinNoise3D((((round(vd.worldPos/float3(0.1250099,0.1250099,0.1250099))*float3(0.1250099,0.1250099,0.1250099))*float3(1.5,1.5,1.5))+float3(0,0,_Time.y * 0.5))*3)+1)/2);
	
	Mask3 = Mask1_Copy_CopyMask3_Sample1.r;//2
	
	
	return Mask3;
	
}
float Mask_Mask4 ( VertexData vd){
		//Set default mask color
			float Mask4 = 0;
		//Generate layers for the Lines channel.
			//Generate Layer: Lines
				//Sample parts of the layer:
					half4 LinesMask4_Sample1 = float4(((vd.worldPos+float3(_Time.y * 0.5,0,0))*float3(50,50,50)),1);
	
				//Apply Effects:
					LinesMask4_Sample1.rgb = ((sin(LinesMask4_Sample1.rgb)+1)/2);
	
	Mask4 = LinesMask4_Sample1.r;//2
	
	
			//Generate Layer: Lines Copy 2
				//Sample parts of the layer:
					half4 Lines_Copy_2Mask4_Sample1 = float4(((vd.worldPos+float3(_Time.y * 0.5,0,0))*float3(20,20,20)),1);
	
				//Apply Effects:
					Lines_Copy_2Mask4_Sample1.rgb = sin(Lines_Copy_2Mask4_Sample1.rgb);
	
	Mask4 = (Mask4 * Lines_Copy_2Mask4_Sample1.r);//0
	
	
			//Generate Layer: Lines2
				//Sample parts of the layer:
					half4 Lines2Mask4_Sample1 = float4(Mask4.rrrr);
	
				//Apply Effects:
					Lines2Mask4_Sample1.rgb = clamp(Lines2Mask4_Sample1.rgb,0,1);
					Lines2Mask4_Sample1.rgb = pow(Lines2Mask4_Sample1.rgb,13.47);
	
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
					PullMask5_Sample1.rgb = pow(PullMask5_Sample1.rgb,8.39);
	
	Mask5 = PullMask5_Sample1.g;//2
	
	
	return Mask5;
	
}
float Mask_Mask6 ( VertexData vd){
		//Set default mask color
			float Mask6 = 0;
		//Generate layers for the Flicker channel.
			//Generate Layer: Lines Copy
				//Sample parts of the layer:
					half4 Lines_CopyMask6_Sample1 = float4(((vd.worldPos*float3(0.1,0.1,0.1))+float3(_Time.z,0,0)),1);
	
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
					half4 FlickerNoise2Mask7_Sample1 = RandomNoise2D((vd.genericTexcoord+float2(_Time.y * 0.5,0)));
	
				//Apply Effects:
					FlickerNoise2Mask7_Sample1.rgb = (FlickerNoise2Mask7_Sample1.rgb*0.57);
					FlickerNoise2Mask7_Sample1.rgb = lerp(float3(0.5,0.5,0.5),FlickerNoise2Mask7_Sample1.rgb,3);
					FlickerNoise2Mask7_Sample1.rgb = clamp(FlickerNoise2Mask7_Sample1.rgb,0,1);
	
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
				half4 outputSet_Alpha_Channel = Set_Alpha_Channel ( vd, outputColor);
				outputColor = half4(outputSet_Alpha_Channel.rgb * outputSet_Alpha_Channel.a, outputSet_Alpha_Channel.a);//11
								half4 outputEmission = Emission ( vd);
				outputColor.rgb = ((outputColor.rgb) * (1 - (outputEmission.a)) + ((outputEmission.a * outputColor.rgb + outputEmission.rgb)));//6
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



















	float RandomNoise3D(float3 co){
		return frac(sin( dot(co.xyz ,float3(12.9898,78.233,45.5432) )) * 43758.5453);
	}
	float RandomNoise2D(float2 co){
		return frac(sin( dot(co.xy ,float2(12.9898,78.233) )) * 43758.5453);
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
	#ifdef SHADOWS_CUBE
		float3 vec : TEXCOORD4;
#endif
	#ifndef SHADOWS_CUBE
		#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
			float4 hpos : TEXCOORD5;
		#endif
	#endif
};

struct VertexData{
	float3 worldPos;
	float4 position;
	float3 worldNormal;
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
//OutputPremultiplied: False
//UseAlphaGenerate: True
half4 Set_Alpha_Channel ( VertexData vd, half4 outputColor){
	float Surface = 0;
		//Generate layers for the Transparency channel.
			//Generate Layer: Legacy Transparency Base
				//Sample parts of the layer:
					half4 Legacy_Transparency_BaseTransparency_Sample1 = 0;
	
	Surface = Legacy_Transparency_BaseTransparency_Sample1.a;//2
	
	
			//Generate Layer: Alpha
				//Sample parts of the layer:
					half4 AlphaTransparency_Sample1 = float4(float3((1-dot(vd.worldNormal, vd.worldViewDir)).rrr),1);
	
				//Apply Effects:
					AlphaTransparency_Sample1.rgb = pow(AlphaTransparency_Sample1.rgb,2);
					AlphaTransparency_Sample1.rgb = clamp(AlphaTransparency_Sample1.rgb,0,1);
					AlphaTransparency_Sample1.rgb = (float3(1,1,1)-AlphaTransparency_Sample1.rgb);
	
	Surface = lerp(Surface,AlphaTransparency_Sample1.a,0.4086956);//3
	
	
	
	return float4(outputColor.rgb,Surface);
	
}
float4 Transparency ( float4 outputColor){
	outputColor *= 1;
	return outputColor;
	
}
void Displace ( inout VertexData vd, inout VertexShaderInput v){
	half3 Surface = half3(1,1,1);
		//Generate layers for the Displacement channel.
			//Generate Layer: Legacy Displacement Base
				//Sample parts of the layer:
					half4 Legacy_Displacement_BaseSurface_Sample1 = 0;
	
	Surface = Legacy_Displacement_BaseSurface_Sample1.rgb;//0
	
	
			//Generate Layer: Vertex3
				//Sample parts of the layer:
					half4 Vertex3Surface_Sample1 = _Flicker_Direction;
	
	Surface = lerp(Surface,(Surface + Vertex3Surface_Sample1.rgb),vd.Mask7);//1
	
	
	
	v.vertex.xyz = float3(v.vertex.xyz + v.normal * Surface * 0.7586207);
	
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
			//Generate Layer: Rim
				//Sample parts of the layer:
					half4 RimMask0_Sample1 = float4(float3(1-float3(dot(vd.worldNormal, vd.worldViewDir).rrr).x,float3(dot(vd.worldNormal, vd.worldViewDir).rrr).y,float3(dot(vd.worldNormal, vd.worldViewDir).rrr).z),1);
	
				//Apply Effects:
					RimMask0_Sample1.rgb = pow(RimMask0_Sample1.rgb,3);
	
	Mask0 = RimMask0_Sample1.r;//2
	
	
	return Mask0;
	
}
float Mask_Mask1 ( VertexData vd){
		//Set default mask color
			float Mask1 = 0;
		//Generate layers for the Dust channel.
			//Generate Layer: Mask1
				//Sample parts of the layer:
					half4 Mask1Mask1_Sample1 = GammaToLinear((PerlinNoise3D(((vd.worldPos*float3(30,30,30))+float3(0,0,_Time.y * 0.5))*3)+1)/2);
	
	Mask1 = Mask1Mask1_Sample1.r;//2
	
	
	return Mask1;
	
}
float Mask_Mask2 ( VertexData vd){
		//Set default mask color
			float Mask2 = 0;
		//Generate layers for the Sparkles channel.
			//Generate Layer: Mask1 Copy
				//Sample parts of the layer:
					half4 Mask1_CopyMask2_Sample1 = GammaToLinear((PerlinNoise3D(((vd.worldPos*float3(100,100,100))+float3(0,0,_Time.y * 0.5))*3)+1)/2);
	
	Mask2 = Mask1_CopyMask2_Sample1.r;//2
	
	
			//Generate Layer: Mask1 Copy Copy 2
				//Sample parts of the layer:
					half4 Mask1_Copy_Copy_2Mask2_Sample1 = GammaToLinear((PerlinNoise3D(((vd.worldPos*float3(10,10,10))+float3(0,0,_Time.y * 0.5))*3)+1)/2);
	
	Mask2 = (Mask2 * Mask1_Copy_Copy_2Mask2_Sample1.r);//0
	
	
			//Generate Layer: Sparkles
				//Sample parts of the layer:
					half4 SparklesMask2_Sample1 = float4(Mask2.rrrr);
	
				//Apply Effects:
					SparklesMask2_Sample1.rgb = (SparklesMask2_Sample1.rgb+0.08);
					SparklesMask2_Sample1.rgb = (round(SparklesMask2_Sample1.rgb*1)/1);
	
	Mask2 = SparklesMask2_Sample1.r;//0
	
	
	return Mask2;
	
}
float Mask_Mask3 ( VertexData vd){
		//Set default mask color
			float Mask3 = 0;
		//Generate layers for the Squares channel.
			//Generate Layer: Mask1 Copy Copy
				//Sample parts of the layer:
					half4 Mask1_Copy_CopyMask3_Sample1 = GammaToLinear((PerlinNoise3D((((round(vd.worldPos/float3(0.1250099,0.1250099,0.1250099))*float3(0.1250099,0.1250099,0.1250099))*float3(1.5,1.5,1.5))+float3(0,0,_Time.y * 0.5))*3)+1)/2);
	
	Mask3 = Mask1_Copy_CopyMask3_Sample1.r;//2
	
	
	return Mask3;
	
}
float Mask_Mask4 ( VertexData vd){
		//Set default mask color
			float Mask4 = 0;
		//Generate layers for the Lines channel.
			//Generate Layer: Lines
				//Sample parts of the layer:
					half4 LinesMask4_Sample1 = float4(((vd.worldPos+float3(_Time.y * 0.5,0,0))*float3(50,50,50)),1);
	
				//Apply Effects:
					LinesMask4_Sample1.rgb = ((sin(LinesMask4_Sample1.rgb)+1)/2);
	
	Mask4 = LinesMask4_Sample1.r;//2
	
	
			//Generate Layer: Lines Copy 2
				//Sample parts of the layer:
					half4 Lines_Copy_2Mask4_Sample1 = float4(((vd.worldPos+float3(_Time.y * 0.5,0,0))*float3(20,20,20)),1);
	
				//Apply Effects:
					Lines_Copy_2Mask4_Sample1.rgb = sin(Lines_Copy_2Mask4_Sample1.rgb);
	
	Mask4 = (Mask4 * Lines_Copy_2Mask4_Sample1.r);//0
	
	
			//Generate Layer: Lines2
				//Sample parts of the layer:
					half4 Lines2Mask4_Sample1 = float4(Mask4.rrrr);
	
				//Apply Effects:
					Lines2Mask4_Sample1.rgb = clamp(Lines2Mask4_Sample1.rgb,0,1);
					Lines2Mask4_Sample1.rgb = pow(Lines2Mask4_Sample1.rgb,13.47);
	
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
					PullMask5_Sample1.rgb = pow(PullMask5_Sample1.rgb,8.39);
	
	Mask5 = PullMask5_Sample1.g;//2
	
	
	return Mask5;
	
}
float Mask_Mask6 ( VertexData vd){
		//Set default mask color
			float Mask6 = 0;
		//Generate layers for the Flicker channel.
			//Generate Layer: Lines Copy
				//Sample parts of the layer:
					half4 Lines_CopyMask6_Sample1 = float4(((vd.worldPos*float3(0.1,0.1,0.1))+float3(_Time.z,0,0)),1);
	
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
					half4 FlickerNoise2Mask7_Sample1 = RandomNoise2D((vd.genericTexcoord+float2(_Time.y * 0.5,0)));
	
				//Apply Effects:
					FlickerNoise2Mask7_Sample1.rgb = (FlickerNoise2Mask7_Sample1.rgb*0.57);
					FlickerNoise2Mask7_Sample1.rgb = lerp(float3(0.5,0.5,0.5),FlickerNoise2Mask7_Sample1.rgb,3);
					FlickerNoise2Mask7_Sample1.rgb = clamp(FlickerNoise2Mask7_Sample1.rgb,0,1);
	
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
	vd.genericTexcoord = v.texcoord;
	Set ( vd, v);
	vd.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	vd.position = 0;
	TRANSFER_SHADOW_CASTER_NOPOS(vd,vd.position);
	vd.worldNormal = UnityObjectToWorldNormalNew(v.normal);
	vd.genericTexcoord = v.texcoord;
	
	
	vtp.worldPos = vd.worldPos;
	vtp.position = vd.position;
	vtp.worldNormal = vd.worldNormal;
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
				half4 outputSet_Alpha_Channel = Set_Alpha_Channel ( vd, outputColor);
				outputColor = half4(outputSet_Alpha_Channel.rgb * outputSet_Alpha_Channel.a, outputSet_Alpha_Channel.a);//11
								outputColor = Transparency ( outputColor);
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
			Color(Vec): 0.1901623,0,0.2205882,1
			CustomFallback(Text): "_Dust_Color"
		End Shader Input


		Begin Shader Input
			Type(Text): "Color"
			VisName(Text): "Sparkles Color"
			Color(Vec): 0.125,0.4568966,1,1
			CustomFallback(Text): "_Sparkles_Color"
		End Shader Input


		Begin Shader Input
			Type(Text): "Color"
			VisName(Text): "Squares Color"
			Color(Vec): 0.4779412,0,0.04944224,1
			CustomFallback(Text): "_Squares_Color"
		End Shader Input


		Begin Shader Input
			Type(Text): "Color"
			VisName(Text): "Lines Color"
			Color(Vec): 0.5310345,0,1,1
			CustomFallback(Text): "_Lines_Color"
		End Shader Input


		Begin Shader Input
			Type(Text): "Range"
			VisName(Text): "Force Flicker"
			Number(Float): 0.382716
			Range0(Float): 0
			Range1(Float): 1
			CustomFallback(Text): "_Force_Flicker"
		End Shader Input


		Begin Shader Input
			Type(Text): "Color"
			VisName(Text): "Flicker Direction"
			Color(Vec): 0.1176471,0,0.1176471,1
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
			Number(Float): 325.5376
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
			Number(Float): 325.5376
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
			Number(Float): 325.5376
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
			Number(Float): 302.1504
			Range0(Float): 0
			Range1(Float): 1
			SpecialType(Text): "TimeMul2"
			InEditor(Float): 0
			CustomFallback(Text): "_Time.z"
		End Shader Input

		ShaderName(Text): "Shader Sandwich/Specific/Hologram"
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
					Layer Name(Text): "Rim"
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
						Power(Float): 3
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
						Z Offset(Float): 483.0645 - {Input = 9}
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
					Begin Shader Effect
						TypeS(Text): "SSEUVOffset"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						X Offset(Float): 0
						Y Offset(Float): 0
						Z Offset(Float): 483.0645 - {Input = 9}
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEUVScale"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Seperate(Toggle): False
						X Scale(Float): 100
						Y Scale(Float): 100
						Z Scale(Float): 100
					End Shader Effect

				End Shader Layer

				Begin Shader Layer
					Layer Name(Text): "Mask1 Copy Copy 2"
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
					Mix Type(Type): Multiply - {TypeID = 3}
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
						Z Offset(Float): 483.0645 - {Input = 10}
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEUVScale"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Seperate(Toggle): False
						X Scale(Float): 10
						Y Scale(Float): 10
						Z Scale(Float): 10
					End Shader Effect

				End Shader Layer

				Begin Shader Layer
					Layer Name(Text): "Sparkles"
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
						IsVisible(Toggle): True
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
						Z Offset(Float): 483.0645 - {Input = 9}
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

				End Shader Layer

			End Shader Layer List

			Begin Shader Layer List

				LayerListUniqueName(Text): "Mask4"
				LayerListName(Text): "Lines"
				Is Mask(Toggle): True
				EndTag(Text): "r"

				Begin Shader Layer
					Layer Name(Text): "Lines"
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
						X Offset(Float): 483.0645 - {Input = 11}
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
						X Offset(Float): 483.0645 - {Input = 11}
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
						Power(Float): 13.47
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
						Power(Float): 8.39
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
						X Offset(Float): 932.2578 - {Input = 12}
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
					Mix Amount(Float): 0.382716 - {Input = 5}
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
					Layer Type(ObjectArray): SLTRandomNoise - {ObjectID = 17}
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
					Gamma Correct(Toggle): False
					Color(Vec): 0.627451,0.8,0.8823529,1
					Begin Shader Effect
						TypeS(Text): "SSEUVOffset"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						X Offset(Float): 483.0645 - {Input = 9}
						Y Offset(Float): 0
						Z Offset(Float): 0
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEMathMul"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Multiply(Float): 0.57
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEContrast"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Contrast(Float): 3
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEMathClamp"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Min(Float): 0
						Max(Float): 1
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
					EndTag(Text): "a"

					Begin Shader Layer
						Layer Name(Text): "Legacy Transparency Base"
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
						Color(Vec): 0.5,0.8788302,1,1
					End Shader Layer

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
						Mix Amount(Float): 0.4086956
						Mix Type(Type): Mix - {TypeID = 0}
						Stencil(ObjectArray): SSNone - {ObjectID = -1}
						Color(Vec): 0.627451,0.8,0.8823529,0
						Begin Shader Effect
							TypeS(Text): "SSEMathPow"
							IsVisible(Toggle): True
							UseAlpha(Float): 0
							Power(Float): 2
						End Shader Effect

						Begin Shader Effect
							TypeS(Text): "SSEMathClamp"
							IsVisible(Toggle): True
							UseAlpha(Float): 0
							Min(Float): 0
							Max(Float): 1
						End Shader Effect

						Begin Shader Effect
							TypeS(Text): "SSEInvert"
							IsVisible(Toggle): True
							UseAlpha(Float): 0
						End Shader Effect

					End Shader Layer

				End Shader Layer List

			End Shader Ingredient


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
						Layer Name(Text): "Emission2"
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
							Multiply(Float): 2
						End Shader Effect

					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Emission3"
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
						Color(Vec): 0.1901623,0,0.2205882,1 - {Input = 1}
					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Emission4"
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
						Color(Vec): 0.125,0.4568966,1,1 - {Input = 2}
					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Emission5"
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
						Color(Vec): 0.4779412,0,0.04944224,1 - {Input = 3}
					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Emission6"
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
						Color(Vec): 0.5310345,0,1,1 - {Input = 4}
					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Alpha Copy"
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
						Mix Amount(Float): 0.5956522
						Mix Type(Type): Multiply - {TypeID = 3}
						Stencil(ObjectArray): SSNone - {ObjectID = -1}
						Color(Vec): 0.627451,0.8,0.8823529,0
						Begin Shader Effect
							TypeS(Text): "SSEMathPow"
							IsVisible(Toggle): True
							UseAlpha(Float): 0
							Power(Float): 2
						End Shader Effect

						Begin Shader Effect
							TypeS(Text): "SSEMathClamp"
							IsVisible(Toggle): True
							UseAlpha(Float): 0
							Min(Float): 0
							Max(Float): 1
						End Shader Effect

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
				Shadow Type Fade(Type): Full - {TypeID = 0}
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
				Strength(Float): 0.7586207
				MidLevel(Float): 0
				Independent XYZ(Toggle): True

				Begin Shader Layer List

					LayerListUniqueName(Text): "Surface"
					LayerListName(Text): "Displacement"
					Is Mask(Toggle): False
					EndTag(Text): "rgb"

					Begin Shader Layer
						Layer Name(Text): "Legacy Displacement Base"
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
						Color(Vec): 0.5,0.8788302,1,1
					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Vertex3"
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
						Stencil(ObjectArray): FlickerNoise - {ObjectID = 7}
						Color(Vec): 0.1176471,0,0.1176471,1 - {Input = 6}
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
