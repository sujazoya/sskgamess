// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader Sandwich/Specific/Ocean" {//The Shaders Name
//The inputs shown in the material panel
Properties {
	[Normal]_BumpMap ("Small Wave Texture", 2D) = "bump" {}
	_Small_Wave_Texture_Scale ("Small Wave Texture Scale", Float) = 5.000000000
	_Second_Small_Waves_Texture_Scale ("Second Small Waves Texture Scale", Float) = 18.000000000
	_Parallax ("Small Wave Texture Strength", Range(0.000000000,1.000000000)) = 0.789256200
	_SpecColor ("Specular - Main Color", Color) = (0.2470588,0.2470588,0.2470588,1)
	_Color ("Texture - Main Color", Color) = (0.2392157,0.2705882,0.4196079,1)
	_Cutoff ("Transparency", Range(0.000000000,1.000000000)) = 0.623327800
	_Small_Waves ("Small Waves", Range(0.000000000,1.000000000)) = 0.161157000
	_Large_Waves ("Large Waves", Range(0.000000000,1.000000000)) = 0.161157000
	_Foam_Amount ("Foam Amount", Range(0.000000000,1.000000000)) = 1.000000000
	_Foam_Color ("Foam Color", Color) = (0.627451,0.8,0.8823529,1)
	_Foam_Scale ("Foam Scale", Float) = 4.000000000
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
				sampler2D _BumpMap;
float4 _BumpMap_ST;
float4 _BumpMap_HDR;
				float _Small_Wave_Texture_Scale;
				float _Second_Small_Waves_Texture_Scale;
				float _Parallax;
								float4 _Color;
				float _Cutoff;
				float _Small_Waves;
				float _Large_Waves;
				float _Foam_Amount;
				float4 _Foam_Color;
				float _Foam_Scale;

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




























struct VertexShaderInput{
	float4 vertex : POSITION;
	float4 tangent : TANGENT;
	float3 normal : NORMAL;
	float4 texcoord : TEXCOORD0;
};
struct VertexToPixel{
	float4 position : POSITION;
	float4 TtoWSpaceX : TEXCOORD0;
	float4 TtoWSpaceY : TEXCOORD1;
	float4 TtoWSpaceZ : TEXCOORD2;
	float2 genericTexcoord : TEXCOORD3;
	float2 uv_BumpMap : TEXCOORD4;
	#define pos position
		UNITY_FOG_COORDS(5)
	#undef pos
};

struct VertexData{
	float3 worldPos;
	float4 position;
	float3 worldNormal;
	float3 worldTangent;
	float3 worldBitangent;
	float3 worldViewDir;
	float4 TtoWSpaceX;
	float4 TtoWSpaceY;
	float4 TtoWSpaceZ;
	float2 genericTexcoord;
	float2 uv_BumpMap;
	float4 Mask0;
	float Mask1;
	float Atten;
};
//OutputPremultiplied: False
//UseAlphaGenerate: True
half3 Tangent_Normals ( VertexData vd){
	half3 Surface = half3(0,0,1);
		//Generate layers for the Normals channel.
			//Generate Layer: Normal Map
				//Sample parts of the layer:
					half4 Normal_MapNormals_Sample1 = tex2D(_BumpMap,((vd.uv_BumpMap*float2(_Small_Wave_Texture_Scale,_Small_Wave_Texture_Scale))+float2(569.1406,_Time.y * 0.1666667)));
	
				//Apply Effects:
					Normal_MapNormals_Sample1 = float4(UnpackNormal(Normal_MapNormals_Sample1),Normal_MapNormals_Sample1.a);
					Normal_MapNormals_Sample1.a = 1;
	
	Surface = lerp(Surface,Normal_MapNormals_Sample1.rgb,Normal_MapNormals_Sample1.a * _Parallax);//1
	
	
			//Generate Layer: Normal Map Copy
				//Sample parts of the layer:
					half4 Normal_Map_CopyNormals_Sample1 = tex2D(_BumpMap,((vd.uv_BumpMap*float2(_Second_Small_Waves_Texture_Scale,_Second_Small_Waves_Texture_Scale))+float2(569.1406,_Time.y * 0.1666667)));
	
				//Apply Effects:
					Normal_Map_CopyNormals_Sample1 = float4(UnpackNormal(Normal_Map_CopyNormals_Sample1),Normal_Map_CopyNormals_Sample1.a);
					Normal_Map_CopyNormals_Sample1.a = 1;
	
	Surface = lerp(Surface,half4(normalize(half3(Surface.xy+Normal_Map_CopyNormals_Sample1.xy,Normal_Map_CopyNormals_Sample1.z)),Normal_Map_CopyNormals_Sample1.a).rgb,Normal_Map_CopyNormals_Sample1.a * _Parallax);//1
	
	
	return normalize(half3(dot(vd.TtoWSpaceX.xyz, Surface),dot(vd.TtoWSpaceY.xyz, Surface),dot(vd.TtoWSpaceZ.xyz, Surface)));
	
}
//OutputPremultiplied: False
//UseAlphaGenerate: False
half4 Diffuse ( VertexData vd){
	half4 Surface = half4(0,0,0,0);
		//Generate layers for the Albedo channel.
			//Generate Layer: Height Copy 2
				//Sample parts of the layer:
					half4 Height_Copy_2Albedo_Sample1 = GammaToLinear((PerlinNoise2D((vd.genericTexcoord+float2(_Time.y * 0.1666667,0))*3)+1)/2);
	
	Surface = half4(Height_Copy_2Albedo_Sample1.rgb,1);//7
	
	
			//Generate Layer: Texture
				//Sample parts of the layer:
					half4 TextureAlbedo_Sample1 = _Color;
	
	Surface = half4((Surface.rgb * TextureAlbedo_Sample1.rgb),1) ;//0
	
	
			//Generate Layer: Texture2
				//Sample parts of the layer:
					half4 Texture2Albedo_Sample1 = _Foam_Color;
	
	Surface.rgb = lerp(Surface.rgb,(Surface.rgb + Texture2Albedo_Sample1.rgb),_Foam_Amount * vd.Mask1);//6
	
	
	return Surface;
	
}
//OutputPremultiplied: False
//UseAlphaGenerate: True
half4 Set_Alpha_Channel ( half4 outputColor){
	float Surface = 0;
		//Generate layers for the Transparency channel.
			//Generate Layer: Transparency
				//Sample parts of the layer:
					half4 TransparencyTransparency_Sample1 = 1;
	
	Surface = TransparencyTransparency_Sample1.a;//2
	
	
	
	return float4(outputColor.rgb,Surface);
	
}
//OutputPremultiplied: False
//UseAlphaGenerate: True
half4 Specular ( VertexData vd){
	half3 Surface = half4(0,0,0,1);//Specular Mode
		//Generate layers for the Specular Color channel.
			//Generate Layer: Specular
				//Sample parts of the layer:
					half4 SpecularSpecular_Sample1 = _SpecColor;
	
	Surface = lerp(Surface,SpecularSpecular_Sample1.rgb,SpecularSpecular_Sample1.a);//1
	
	
	half reflectivity = SpecularStrength(Surface);
	half oneMinusReflectivity = 1-reflectivity;
	half nv = saturate(dot(vd.worldNormal, vd.worldViewDir));
	half nvPow5 = Pow5 (1-nv);
	return float4(Surface,lerp(1,nvPow5,oneMinusReflectivity));
	
}
float4 Transparency ( float4 outputColor){
	outputColor *= _Cutoff;
	return outputColor;
	
}
void Displace ( inout VertexData vd, inout VertexShaderInput v){
	half3 Surface = half3(1,1,1);
		//Generate layers for the Displacement channel.
			//Generate Layer: Zero
				//Sample parts of the layer:
					half4 ZeroSurface_Sample1 = 0;
	
	Surface = ZeroSurface_Sample1.rgb;//0
	
	
			//Generate Layer: Height Copy
				//Sample parts of the layer:
					half4 Height_CopySurface_Sample1 = (PerlinNoise2D((vd.genericTexcoord+float2(_Time.y * 0.1666667,0))*3)+1)/2;
	
	Surface = lerp(Surface,(Surface + Height_CopySurface_Sample1.rgb),_Small_Waves);//1
	
	
			//Generate Layer: Height Copy Copy
				//Sample parts of the layer:
					half4 Height_Copy_CopySurface_Sample1 = (PerlinNoise2D((vd.genericTexcoord*float2(0.5,0.5))*3)+1)/2;
	
	Surface = lerp(Surface,(Surface + Height_Copy_CopySurface_Sample1.rgb),_Large_Waves);//1
	
	
	
	v.vertex.xyz = float3(v.vertex.xyz + v.normal * Surface * 1);
	
}
float4 Mask_Mask0 ( VertexData vd){
		//Set default mask color
			float4 Mask0 = float4(0,0,0,0);
		//Generate layers for the UVs channel.
			//Generate Layer: Mask1
				//Sample parts of the layer:
					half4 Mask1Mask1_Sample1 = float4(vd.worldPos,1);
	
				//Apply Effects:
					Mask1Mask1_Sample1 = Mask1Mask1_Sample1.rbba;
					Mask1Mask1_Sample1.a = 1;
	
	Mask0 = Mask1Mask1_Sample1;//10
	
	
	return Mask0;
	
}
float Mask_Mask1 ( VertexData vd){
		//Set default mask color
			float Mask1 = 0;
		//Generate layers for the Mask0 channel.
			//Generate Layer: Mask0
				//Sample parts of the layer:
					half4 Mask0Mask0_Sample1 = GammaToLinear((PerlinNoise2D((((float3(vd.Mask0.r,vd.Mask0.g,0))*1)).xy*3)+1)/2);
	
				//Apply Effects:
					Mask0Mask0_Sample1.rgb = pow(Mask0Mask0_Sample1.rgb,8.28);
					Mask0Mask0_Sample1.rgb = Mask0Mask0_Sample1.rgb * ceil(saturate(dot(float3(0.3,0.59,0.11),Mask0Mask0_Sample1.rgb)-0.05));
					Mask0Mask0_Sample1.rgb = (Mask0Mask0_Sample1.rgb*2.39);
	
	Mask1 = Mask0Mask0_Sample1.r;//2
	
	
	return Mask1;
	
}
VertexToPixel Vertex (VertexShaderInput v){
	VertexToPixel vtp;
	UNITY_INITIALIZE_OUTPUT(VertexToPixel,vtp);
	VertexData vd;
	UNITY_INITIALIZE_OUTPUT(VertexData,vd);
	vd.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	vd.position = UnityObjectToClipPos(v.vertex);
	vd.worldNormal = UnityObjectToWorldNormalNew(v.normal);
	vd.worldTangent = UnityObjectToWorldNormalNew(v.tangent);
	vd.worldBitangent = cross(vd.worldNormal, vd.worldTangent) * v.tangent.w * unity_WorldTransformParams.w;
	vd.TtoWSpaceX = float4(vd.worldTangent.x, vd.worldBitangent.x, vd.worldNormal.x, vd.worldPos.x);
	vd.TtoWSpaceY = float4(vd.worldTangent.y, vd.worldBitangent.y, vd.worldNormal.y, vd.worldPos.y);
	vd.TtoWSpaceZ = float4(vd.worldTangent.z, vd.worldBitangent.z, vd.worldNormal.z, vd.worldPos.z);
	vd.genericTexcoord = v.texcoord;
	vd.uv_BumpMap = TRANSFORM_TEX(v.texcoord, _BumpMap);
	
	
vd.Mask0 = Mask_Mask0 ( vd);
	Displace ( vd, v);
	vd.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	vd.position = UnityObjectToClipPos(v.vertex);
	vd.worldNormal = UnityObjectToWorldNormalNew(v.normal);
	vd.worldTangent = UnityObjectToWorldNormalNew(v.tangent);
	vd.worldBitangent = cross(vd.worldNormal, vd.worldTangent) * v.tangent.w * unity_WorldTransformParams.w;
	vd.TtoWSpaceX = float4(vd.worldTangent.x, vd.worldBitangent.x, vd.worldNormal.x, vd.worldPos.x);
	vd.TtoWSpaceY = float4(vd.worldTangent.y, vd.worldBitangent.y, vd.worldNormal.y, vd.worldPos.y);
	vd.TtoWSpaceZ = float4(vd.worldTangent.z, vd.worldBitangent.z, vd.worldNormal.z, vd.worldPos.z);
	vd.genericTexcoord = v.texcoord;
	vd.uv_BumpMap = TRANSFORM_TEX(v.texcoord, _BumpMap);
	
	
	vtp.position = vd.position;
	vtp.TtoWSpaceX = vd.TtoWSpaceX;
	vtp.TtoWSpaceY = vd.TtoWSpaceY;
	vtp.TtoWSpaceZ = vd.TtoWSpaceZ;
	vtp.genericTexcoord = vd.genericTexcoord;
	vtp.uv_BumpMap = vd.uv_BumpMap;
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
				vd.worldPos = half3(vtp.TtoWSpaceX.w,vtp.TtoWSpaceY.w,vtp.TtoWSpaceZ.w);
				vd.worldNormal = half3(vtp.TtoWSpaceX.z,vtp.TtoWSpaceY.z,vtp.TtoWSpaceZ.z);
				vd.worldViewDir = normalize(UnityWorldSpaceViewDir(vd.worldPos));
				vd.TtoWSpaceX = vtp.TtoWSpaceX;
				vd.TtoWSpaceY = vtp.TtoWSpaceY;
				vd.TtoWSpaceZ = vtp.TtoWSpaceZ;
				vd.genericTexcoord = vtp.genericTexcoord;
				vd.uv_BumpMap = vtp.uv_BumpMap;
				outputNormal = vd.worldNormal;
vd.Mask1 = Mask_Mask1 ( vd);
				half3 outputTangent_Normals = Tangent_Normals ( vd);
				outputNormal = outputTangent_Normals.rgb;//0
								vd.worldNormal = outputNormal;
				half4 outputDiffuse = Diffuse ( vd);
				outputColor = half4(outputDiffuse.rgb,1);//7
								half4 outputSet_Alpha_Channel = Set_Alpha_Channel ( outputColor);
				outputColor = half4(outputSet_Alpha_Channel.rgb * outputSet_Alpha_Channel.a, outputSet_Alpha_Channel.a);//11
								half4 outputSpecular = Specular ( vd);
				outputColor = ((outputColor) * (1 - (outputSpecular.a)) + (half4(outputSpecular.rgb, outputSpecular.a)));//1
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
				sampler2D _BumpMap;
float4 _BumpMap_ST;
float4 _BumpMap_HDR;
				float _Small_Wave_Texture_Scale;
				float _Second_Small_Waves_Texture_Scale;
				float _Parallax;
								float4 _Color;
				float _Cutoff;
				float _Small_Waves;
				float _Large_Waves;
				float _Foam_Amount;
				float4 _Foam_Color;
				float _Foam_Scale;

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




























struct VertexShaderInput{
	float4 vertex : POSITION;
	float4 tangent : TANGENT;
	float3 normal : NORMAL;
	float4 texcoord : TEXCOORD0;
	float4 texcoord1 : TEXCOORD1;
	float4 texcoord2 : TEXCOORD2;
};
struct VertexToPixel{
	float4 position : POSITION;
	float3 worldViewDir : TEXCOORD0;
	float4 TtoWSpaceX : TEXCOORD1;
	float4 TtoWSpaceY : TEXCOORD2;
	float4 TtoWSpaceZ : TEXCOORD3;
	float2 genericTexcoord : TEXCOORD4;
	float2 uv_BumpMap : TEXCOORD5;
	#if UNITY_SHOULD_SAMPLE_SH
		float3 sh : TEXCOORD6;
#endif
	#define pos position
		SHADOW_COORDS(7)
		UNITY_FOG_COORDS(8)
#undef pos
	#if (!defined(LIGHTMAP_OFF))||(SHADER_TARGET >= 30)
		float4 lmap : TEXCOORD9;
	#endif
};

struct VertexData{
	float3 worldPos;
	float4 position;
	float3 worldNormal;
	float3 worldTangent;
	float3 worldBitangent;
	float3 worldViewDir;
	float3 worldRefl;
	float4 TtoWSpaceX;
	float4 TtoWSpaceY;
	float4 TtoWSpaceZ;
	float2 genericTexcoord;
	float2 uv_BumpMap;
	#if UNITY_SHOULD_SAMPLE_SH
		float3 sh;
#endif
	#if (!defined(LIGHTMAP_OFF))||(SHADER_TARGET >= 30)
		float4 lmap;
	#endif
	float4 Mask0;
	float Mask1;
	float Atten;
};
//OutputPremultiplied: False
//UseAlphaGenerate: True
half3 Tangent_Normals ( VertexData vd){
	half3 Surface = half3(0,0,1);
		//Generate layers for the Normals channel.
			//Generate Layer: Normal Map
				//Sample parts of the layer:
					half4 Normal_MapNormals_Sample1 = tex2D(_BumpMap,((vd.uv_BumpMap*float2(_Small_Wave_Texture_Scale,_Small_Wave_Texture_Scale))+float2(569.1406,_Time.y * 0.1666667)));
	
				//Apply Effects:
					Normal_MapNormals_Sample1 = float4(UnpackNormal(Normal_MapNormals_Sample1),Normal_MapNormals_Sample1.a);
					Normal_MapNormals_Sample1.a = 1;
	
	Surface = lerp(Surface,Normal_MapNormals_Sample1.rgb,Normal_MapNormals_Sample1.a * _Parallax);//1
	
	
			//Generate Layer: Normal Map Copy
				//Sample parts of the layer:
					half4 Normal_Map_CopyNormals_Sample1 = tex2D(_BumpMap,((vd.uv_BumpMap*float2(_Second_Small_Waves_Texture_Scale,_Second_Small_Waves_Texture_Scale))+float2(569.1406,_Time.y * 0.1666667)));
	
				//Apply Effects:
					Normal_Map_CopyNormals_Sample1 = float4(UnpackNormal(Normal_Map_CopyNormals_Sample1),Normal_Map_CopyNormals_Sample1.a);
					Normal_Map_CopyNormals_Sample1.a = 1;
	
	Surface = lerp(Surface,half4(normalize(half3(Surface.xy+Normal_Map_CopyNormals_Sample1.xy,Normal_Map_CopyNormals_Sample1.z)),Normal_Map_CopyNormals_Sample1.a).rgb,Normal_Map_CopyNormals_Sample1.a * _Parallax);//1
	
	
	return normalize(half3(dot(vd.TtoWSpaceX.xyz, Surface),dot(vd.TtoWSpaceY.xyz, Surface),dot(vd.TtoWSpaceZ.xyz, Surface)));
	
}
//OutputPremultiplied: False
//UseAlphaGenerate: False
half4 Diffuse ( VertexData vd, UnityGI gi, UnityGIInput giInput){
	half4 Surface = half4(0,0,0,0);
		//Generate layers for the Albedo channel.
			//Generate Layer: Height Copy 2
				//Sample parts of the layer:
					half4 Height_Copy_2Albedo_Sample1 = GammaToLinear((PerlinNoise2D((vd.genericTexcoord+float2(_Time.y * 0.1666667,0))*3)+1)/2);
	
	Surface = half4(Height_Copy_2Albedo_Sample1.rgb,1);//7
	
	
			//Generate Layer: Texture
				//Sample parts of the layer:
					half4 TextureAlbedo_Sample1 = _Color;
	
	Surface = half4((Surface.rgb * TextureAlbedo_Sample1.rgb),1) ;//0
	
	
			//Generate Layer: Texture2
				//Sample parts of the layer:
					half4 Texture2Albedo_Sample1 = _Foam_Color;
	
	Surface.rgb = lerp(Surface.rgb,(Surface.rgb + Texture2Albedo_Sample1.rgb),_Foam_Amount * vd.Mask1);//6
	
	
	float3 Lighting;
	#if SSUNITY_BRDF_PBS==1
	half roughness = 1-0.926;
	half3 halfDir = normalize (gi.light.dir + vd.worldViewDir);
	
	half nh = saturate(dot(vd.worldNormal, halfDir));
	half nl = saturate(dot(vd.worldNormal, gi.light.dir));
	half nv = abs(dot(vd.worldNormal, vd.worldViewDir));
	half lh = saturate(dot(gi.light.dir, halfDir));
	
	half nlPow5 = Pow5 (1-nl);
	half nvPow5 = Pow5 (1-nv);
	#else
	Lighting = saturate(dot(vd.worldNormal, gi.light.dir));
	#endif
	Unity_GlossyEnvironmentData g;
	g.roughness = (1-0.926);
	g.reflUVW = vd.worldViewDir;
	gi = UnityGlobalIllumination(giInput, 1, vd.worldNormal,g);
	#if SSUNITY_BRDF_PBS==1
	half Fd90 = 0.5 + 2 * lh * lh * roughness;
	half disneyDiffuse = (1 + (Fd90-1) * nlPow5) * (1 + (Fd90-1) * nvPow5);
	
	half diffuseTerm = disneyDiffuse * nl;
	Lighting = diffuseTerm;
	#endif
	Lighting *= gi.light.color;
	return half4(gi.indirect.diffuse + Lighting,1)*Surface;
}
//OutputPremultiplied: False
//UseAlphaGenerate: True
half4 Set_Alpha_Channel ( half4 outputColor){
	float Surface = 0;
		//Generate layers for the Transparency channel.
			//Generate Layer: Transparency
				//Sample parts of the layer:
					half4 TransparencyTransparency_Sample1 = 1;
	
	Surface = TransparencyTransparency_Sample1.a;//2
	
	
	
	return float4(outputColor.rgb,Surface);
	
}
//OutputPremultiplied: False
//UseAlphaGenerate: True
half4 Specular ( VertexData vd, UnityGI gi, UnityGIInput giInput){
	half3 Surface = half4(0,0,0,1);//Specular Mode
		//Generate layers for the Specular Color channel.
			//Generate Layer: Specular
				//Sample parts of the layer:
					half4 SpecularSpecular_Sample1 = _SpecColor;
	
	Surface = lerp(Surface,SpecularSpecular_Sample1.rgb,SpecularSpecular_Sample1.a);//1
	
	
	half oneMinusRoughness = 0.926;
	half perceptualRoughness = 1-oneMinusRoughness;
	half realRoughness = perceptualRoughness*perceptualRoughness;		// need to square perceptual roughness
	half reflectivity = SpecularStrength(Surface);
	half oneMinusReflectivity = 1-reflectivity;
	Unity_GlossyEnvironmentData g;
	g.roughness = (1-0.926);
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
	
	#if SSUNITY_BRDF_PBS==1
	half nvPow5 = Pow5 (1-nv);
	#elif SSUNITY_BRDF_PBS==2||SSUNITY_BRDF_PBS==3
	half nvPow5 = Pow4 (1-nv);
	#endif
	
	half3 Lighting;
	#if SSUNITY_BRDF_PBS==1
	gi.indirect.specular *= 1 - perceptualRoughness*(nv*0.2+0.4);
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
	gi.indirect.specular *= 1 - perceptualRoughness*(nv*0.2+0.4);
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
	#if SSUNITY_BRDF_PBS==1
	Lighting =	(Lighting + gi.indirect.specular) * lerp(Surface,1,nvPow5);
	#elif SSUNITY_BRDF_PBS==2
	Lighting =	(Lighting * nl + gi.indirect.specular) * lerp(Surface,1,nvPow5);
	#else
	Lighting = (Lighting+gi.indirect.specular) * lerp (Surface.rgb, 1, fresnelTerm);
	#endif
	return half4(Lighting.rgb,lerp(1,nvPow5,oneMinusReflectivity));
	
}
float4 Transparency ( float4 outputColor){
	outputColor *= _Cutoff;
	return outputColor;
	
}
void Displace ( inout VertexData vd, inout VertexShaderInput v){
	half3 Surface = half3(1,1,1);
		//Generate layers for the Displacement channel.
			//Generate Layer: Zero
				//Sample parts of the layer:
					half4 ZeroSurface_Sample1 = 0;
	
	Surface = ZeroSurface_Sample1.rgb;//0
	
	
			//Generate Layer: Height Copy
				//Sample parts of the layer:
					half4 Height_CopySurface_Sample1 = (PerlinNoise2D((vd.genericTexcoord+float2(_Time.y * 0.1666667,0))*3)+1)/2;
	
	Surface = lerp(Surface,(Surface + Height_CopySurface_Sample1.rgb),_Small_Waves);//1
	
	
			//Generate Layer: Height Copy Copy
				//Sample parts of the layer:
					half4 Height_Copy_CopySurface_Sample1 = (PerlinNoise2D((vd.genericTexcoord*float2(0.5,0.5))*3)+1)/2;
	
	Surface = lerp(Surface,(Surface + Height_Copy_CopySurface_Sample1.rgb),_Large_Waves);//1
	
	
	
	v.vertex.xyz = float3(v.vertex.xyz + v.normal * Surface * 1);
	
}
float4 Mask_Mask0 ( VertexData vd){
		//Set default mask color
			float4 Mask0 = float4(0,0,0,0);
		//Generate layers for the UVs channel.
			//Generate Layer: Mask1
				//Sample parts of the layer:
					half4 Mask1Mask1_Sample1 = float4(vd.worldPos,1);
	
				//Apply Effects:
					Mask1Mask1_Sample1 = Mask1Mask1_Sample1.rbba;
					Mask1Mask1_Sample1.a = 1;
	
	Mask0 = Mask1Mask1_Sample1;//10
	
	
	return Mask0;
	
}
float Mask_Mask1 ( VertexData vd){
		//Set default mask color
			float Mask1 = 0;
		//Generate layers for the Mask0 channel.
			//Generate Layer: Mask0
				//Sample parts of the layer:
					half4 Mask0Mask0_Sample1 = GammaToLinear((PerlinNoise2D((((float3(vd.Mask0.r,vd.Mask0.g,0))*1)).xy*3)+1)/2);
	
				//Apply Effects:
					Mask0Mask0_Sample1.rgb = pow(Mask0Mask0_Sample1.rgb,8.28);
					Mask0Mask0_Sample1.rgb = Mask0Mask0_Sample1.rgb * ceil(saturate(dot(float3(0.3,0.59,0.11),Mask0Mask0_Sample1.rgb)-0.05));
					Mask0Mask0_Sample1.rgb = (Mask0Mask0_Sample1.rgb*2.39);
	
	Mask1 = Mask0Mask0_Sample1.r;//2
	
	
	return Mask1;
	
}
VertexToPixel Vertex (VertexShaderInput v){
	VertexToPixel vtp;
	UNITY_INITIALIZE_OUTPUT(VertexToPixel,vtp);
	VertexData vd;
	UNITY_INITIALIZE_OUTPUT(VertexData,vd);
	vd.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	vd.position = UnityObjectToClipPos(v.vertex);
	vd.worldNormal = UnityObjectToWorldNormalNew(v.normal);
	vd.worldTangent = UnityObjectToWorldNormalNew(v.tangent);
	vd.worldBitangent = cross(vd.worldNormal, vd.worldTangent) * v.tangent.w * unity_WorldTransformParams.w;
	vd.worldViewDir = normalize(UnityWorldSpaceViewDir(vd.worldPos));
	vd.TtoWSpaceX = float4(vd.worldTangent.x, vd.worldBitangent.x, vd.worldNormal.x, vd.worldPos.x);
	vd.TtoWSpaceY = float4(vd.worldTangent.y, vd.worldBitangent.y, vd.worldNormal.y, vd.worldPos.y);
	vd.TtoWSpaceZ = float4(vd.worldTangent.z, vd.worldBitangent.z, vd.worldNormal.z, vd.worldPos.z);
	vd.genericTexcoord = v.texcoord;
	vd.uv_BumpMap = TRANSFORM_TEX(v.texcoord, _BumpMap);
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
	
	
vd.Mask0 = Mask_Mask0 ( vd);
	Displace ( vd, v);
	vd.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	vd.position = UnityObjectToClipPos(v.vertex);
	vd.worldNormal = UnityObjectToWorldNormalNew(v.normal);
	vd.worldTangent = UnityObjectToWorldNormalNew(v.tangent);
	vd.worldBitangent = cross(vd.worldNormal, vd.worldTangent) * v.tangent.w * unity_WorldTransformParams.w;
	vd.worldViewDir = normalize(UnityWorldSpaceViewDir(vd.worldPos));
	vd.TtoWSpaceX = float4(vd.worldTangent.x, vd.worldBitangent.x, vd.worldNormal.x, vd.worldPos.x);
	vd.TtoWSpaceY = float4(vd.worldTangent.y, vd.worldBitangent.y, vd.worldNormal.y, vd.worldPos.y);
	vd.TtoWSpaceZ = float4(vd.worldTangent.z, vd.worldBitangent.z, vd.worldNormal.z, vd.worldPos.z);
	vd.genericTexcoord = v.texcoord;
	vd.uv_BumpMap = TRANSFORM_TEX(v.texcoord, _BumpMap);
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
	
	
	vtp.position = vd.position;
	vtp.worldViewDir = vd.worldViewDir;
	vtp.TtoWSpaceX = vd.TtoWSpaceX;
	vtp.TtoWSpaceY = vd.TtoWSpaceY;
	vtp.TtoWSpaceZ = vd.TtoWSpaceZ;
	vtp.genericTexcoord = vd.genericTexcoord;
	vtp.uv_BumpMap = vd.uv_BumpMap;
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
				vd.worldPos = half3(vtp.TtoWSpaceX.w,vtp.TtoWSpaceY.w,vtp.TtoWSpaceZ.w);
				vd.worldNormal = half3(vtp.TtoWSpaceX.z,vtp.TtoWSpaceY.z,vtp.TtoWSpaceZ.z);
				vd.worldViewDir = normalize(UnityWorldSpaceViewDir(vd.worldPos));
				vd.worldRefl = reflect(-vd.worldViewDir, vd.worldNormal);
				vd.TtoWSpaceX = vtp.TtoWSpaceX;
				vd.TtoWSpaceY = vtp.TtoWSpaceY;
				vd.TtoWSpaceZ = vtp.TtoWSpaceZ;
				vd.genericTexcoord = vtp.genericTexcoord;
				vd.uv_BumpMap = vtp.uv_BumpMap;
	#if UNITY_SHOULD_SAMPLE_SH
				vd.sh = vtp.sh;
#endif
	#if (!defined(LIGHTMAP_OFF))||(SHADER_TARGET >= 30)
				vd.lmap = vtp.lmap;
	#endif
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
vd.Mask1 = Mask_Mask1 ( vd);
				half3 outputTangent_Normals = Tangent_Normals ( vd);
				outputNormal = outputTangent_Normals.rgb;//0
								vd.worldNormal = outputNormal;
				vd.worldRefl = reflect(-vd.worldViewDir, vd.worldNormal);
				half4 outputDiffuse = Diffuse ( vd, gi, giInput);
				outputColor = half4(outputDiffuse.rgb,1);//7
								half4 outputSet_Alpha_Channel = Set_Alpha_Channel ( outputColor);
				outputColor = half4(outputSet_Alpha_Channel.rgb * outputSet_Alpha_Channel.a, outputSet_Alpha_Channel.a);//11
								half4 outputSpecular = Specular ( vd, gi, giInput);
				outputColor = ((outputColor) * (1 - (outputSpecular.a)) + (half4(outputSpecular.rgb, outputSpecular.a)));//1
								outputColor = Transparency ( outputColor);
				UNITY_APPLY_FOG(vtp.fogCoord, outputColor); // apply fog (UNITY_FOG_COORDS));
				return outputColor;

			}
		ENDCG
	}
AlphaToMask Off
	Pass {
		Name "FORWARD"
		Tags { "LightMode" = "ForwardAdd" }
	Fog { Color (0,0,0,0) }
	ZTest LEqual
	ZWrite Off
	Blend One One
	Cull Back//Culling specifies which sides of the models faces to hide.

		
		CGPROGRAM
			// compile directives
				#pragma vertex Vertex
				#pragma fragment Pixel
				#pragma target 3.0
				#pragma multi_compile_fog
				#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
				#pragma multi_compile_fwdadd_fullshadows
				#include "HLSLSupport.cginc"
				#include "UnityShaderVariables.cginc"
				#define UNITY_PASS_FORWARDADD
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "UnityPBSLighting.cginc"
				#include "AutoLight.cginc"

				#define INTERNAL_DATA
				#define WorldReflectionVector(data,normal) data.worldRefl
				#define WorldNormalVector(data,normal) normal
			//Make our inputs accessible by declaring them here.
				sampler2D _BumpMap;
float4 _BumpMap_ST;
float4 _BumpMap_HDR;
				float _Small_Wave_Texture_Scale;
				float _Second_Small_Waves_Texture_Scale;
				float _Parallax;
								float4 _Color;
				float _Cutoff;
				float _Small_Waves;
				float _Large_Waves;
				float _Foam_Amount;
				float4 _Foam_Color;
				float _Foam_Scale;

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




























struct VertexShaderInput{
	float4 vertex : POSITION;
	float4 tangent : TANGENT;
	float3 normal : NORMAL;
	float4 texcoord : TEXCOORD0;
	float4 texcoord1 : TEXCOORD1;
	float4 texcoord2 : TEXCOORD2;
};
struct VertexToPixel{
	float4 position : POSITION;
	float3 worldViewDir : TEXCOORD0;
	float4 TtoWSpaceX : TEXCOORD1;
	float4 TtoWSpaceY : TEXCOORD2;
	float4 TtoWSpaceZ : TEXCOORD3;
	float2 genericTexcoord : TEXCOORD4;
	float2 uv_BumpMap : TEXCOORD5;
	#if UNITY_SHOULD_SAMPLE_SH
		float3 sh : TEXCOORD6;
#endif
	#define pos position
		SHADOW_COORDS(7)
		UNITY_FOG_COORDS(8)
#undef pos
	#if (!defined(LIGHTMAP_OFF))||(SHADER_TARGET >= 30)
		float4 lmap : TEXCOORD9;
	#endif
};

struct VertexData{
	float3 worldPos;
	float4 position;
	float3 worldNormal;
	float3 worldTangent;
	float3 worldBitangent;
	float3 worldViewDir;
	float3 worldRefl;
	float4 TtoWSpaceX;
	float4 TtoWSpaceY;
	float4 TtoWSpaceZ;
	float2 genericTexcoord;
	float2 uv_BumpMap;
	#if UNITY_SHOULD_SAMPLE_SH
		float3 sh;
#endif
	#if (!defined(LIGHTMAP_OFF))||(SHADER_TARGET >= 30)
		float4 lmap;
	#endif
	float4 Mask0;
	float Mask1;
	float Atten;
};
//OutputPremultiplied: False
//UseAlphaGenerate: True
half3 Tangent_Normals ( VertexData vd){
	half3 Surface = half3(0,0,1);
		//Generate layers for the Normals channel.
			//Generate Layer: Normal Map
				//Sample parts of the layer:
					half4 Normal_MapNormals_Sample1 = tex2D(_BumpMap,((vd.uv_BumpMap*float2(_Small_Wave_Texture_Scale,_Small_Wave_Texture_Scale))+float2(569.1406,_Time.y * 0.1666667)));
	
				//Apply Effects:
					Normal_MapNormals_Sample1 = float4(UnpackNormal(Normal_MapNormals_Sample1),Normal_MapNormals_Sample1.a);
					Normal_MapNormals_Sample1.a = 1;
	
	Surface = lerp(Surface,Normal_MapNormals_Sample1.rgb,Normal_MapNormals_Sample1.a * _Parallax);//1
	
	
			//Generate Layer: Normal Map Copy
				//Sample parts of the layer:
					half4 Normal_Map_CopyNormals_Sample1 = tex2D(_BumpMap,((vd.uv_BumpMap*float2(_Second_Small_Waves_Texture_Scale,_Second_Small_Waves_Texture_Scale))+float2(569.1406,_Time.y * 0.1666667)));
	
				//Apply Effects:
					Normal_Map_CopyNormals_Sample1 = float4(UnpackNormal(Normal_Map_CopyNormals_Sample1),Normal_Map_CopyNormals_Sample1.a);
					Normal_Map_CopyNormals_Sample1.a = 1;
	
	Surface = lerp(Surface,half4(normalize(half3(Surface.xy+Normal_Map_CopyNormals_Sample1.xy,Normal_Map_CopyNormals_Sample1.z)),Normal_Map_CopyNormals_Sample1.a).rgb,Normal_Map_CopyNormals_Sample1.a * _Parallax);//1
	
	
	return normalize(half3(dot(vd.TtoWSpaceX.xyz, Surface),dot(vd.TtoWSpaceY.xyz, Surface),dot(vd.TtoWSpaceZ.xyz, Surface)));
	
}
//OutputPremultiplied: False
//UseAlphaGenerate: False
half4 Diffuse ( VertexData vd, UnityGI gi, UnityGIInput giInput){
	half4 Surface = half4(0,0,0,0);
		//Generate layers for the Albedo channel.
			//Generate Layer: Height Copy 2
				//Sample parts of the layer:
					half4 Height_Copy_2Albedo_Sample1 = GammaToLinear((PerlinNoise2D((vd.genericTexcoord+float2(_Time.y * 0.1666667,0))*3)+1)/2);
	
	Surface = half4(Height_Copy_2Albedo_Sample1.rgb,1);//7
	
	
			//Generate Layer: Texture
				//Sample parts of the layer:
					half4 TextureAlbedo_Sample1 = _Color;
	
	Surface = half4((Surface.rgb * TextureAlbedo_Sample1.rgb),1) ;//0
	
	
			//Generate Layer: Texture2
				//Sample parts of the layer:
					half4 Texture2Albedo_Sample1 = _Foam_Color;
	
	Surface.rgb = lerp(Surface.rgb,(Surface.rgb + Texture2Albedo_Sample1.rgb),_Foam_Amount * vd.Mask1);//6
	
	
	float3 Lighting;
	#if SSUNITY_BRDF_PBS==1
	half roughness = 1-0.926;
	half3 halfDir = normalize (gi.light.dir + vd.worldViewDir);
	
	half nh = saturate(dot(vd.worldNormal, halfDir));
	half nl = saturate(dot(vd.worldNormal, gi.light.dir));
	half nv = abs(dot(vd.worldNormal, vd.worldViewDir));
	half lh = saturate(dot(gi.light.dir, halfDir));
	
	half nlPow5 = Pow5 (1-nl);
	half nvPow5 = Pow5 (1-nv);
	#else
	Lighting = saturate(dot(vd.worldNormal, gi.light.dir));
	#endif
	UnityGI o_gi;
	ResetUnityGI(o_gi);
	o_gi.light = giInput.light;
	o_gi.light.color *= giInput.atten;
	gi = o_gi;
	#if SSUNITY_BRDF_PBS==1
	half Fd90 = 0.5 + 2 * lh * lh * roughness;
	half disneyDiffuse = (1 + (Fd90-1) * nlPow5) * (1 + (Fd90-1) * nvPow5);
	
	half diffuseTerm = disneyDiffuse * nl;
	Lighting = diffuseTerm;
	#endif
	Lighting *= gi.light.color;
	return half4(Lighting,1)*Surface;
}
//OutputPremultiplied: False
//UseAlphaGenerate: True
half4 Set_Alpha_Channel ( half4 outputColor){
	float Surface = 0;
		//Generate layers for the Transparency channel.
			//Generate Layer: Transparency
				//Sample parts of the layer:
					half4 TransparencyTransparency_Sample1 = 1;
	
	Surface = TransparencyTransparency_Sample1.a;//2
	
	
	
	return float4(outputColor.rgb,Surface);
	
}
//OutputPremultiplied: False
//UseAlphaGenerate: True
half4 Specular ( VertexData vd, UnityGI gi, UnityGIInput giInput){
	half3 Surface = half4(0,0,0,1);//Specular Mode
		//Generate layers for the Specular Color channel.
			//Generate Layer: Specular
				//Sample parts of the layer:
					half4 SpecularSpecular_Sample1 = _SpecColor;
	
	Surface = lerp(Surface,SpecularSpecular_Sample1.rgb,SpecularSpecular_Sample1.a);//1
	
	
	half oneMinusRoughness = 0.926;
	half perceptualRoughness = 1-oneMinusRoughness;
	half realRoughness = perceptualRoughness*perceptualRoughness;		// need to square perceptual roughness
	half reflectivity = SpecularStrength(Surface);
	half oneMinusReflectivity = 1-reflectivity;
	UnityGI o_gi;
	ResetUnityGI(o_gi);
	o_gi.light = giInput.light;
	o_gi.light.color *= giInput.atten;
	gi = o_gi;
	half3 halfDir = normalize (gi.light.dir + vd.worldViewDir);
	//#if UNITY_BRDF_GGX 
	//	half shiftAmount = dot(vd.worldNormal, vd.worldViewDir);
	//	vd.worldNormal = shiftAmount < 0.0f ? vd.worldNormal + vd.worldViewDir * (-shiftAmount + 1e-5f) : vd.worldNormal;
	//#endif
	half nh = saturate(dot(vd.worldNormal, halfDir));
	half nl = saturate(dot(vd.worldNormal, gi.light.dir));
	half nv = abs(dot(vd.worldNormal, vd.worldViewDir));
	half lh = saturate(dot(gi.light.dir, halfDir));
	
	#if SSUNITY_BRDF_PBS==1
	half nvPow5 = Pow5 (1-nv);
	#elif SSUNITY_BRDF_PBS==2||SSUNITY_BRDF_PBS==3
	half nvPow5 = Pow4 (1-nv);
	#endif
	
	half3 Lighting;
	#if SSUNITY_BRDF_PBS==1
	gi.indirect.specular *= 1 - perceptualRoughness*(nv*0.2+0.4);
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
	gi.indirect.specular *= 1 - perceptualRoughness*(nv*0.2+0.4);
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
	#if SSUNITY_BRDF_PBS==1
	Lighting =	(Lighting) * lerp(1,Surface,(1-nvPow5));
	#elif SSUNITY_BRDF_PBS==2
	Lighting =	(Lighting * nl) * lerp(1,Surface,(1-nvPow5));
	#elif SSUNITY_BRDF_PBS==3
	Lighting =	Lighting * Surface;
	#endif
	return half4(Lighting.rgb,lerp(1,nvPow5,oneMinusReflectivity));
	
}
float4 Transparency ( float4 outputColor){
	outputColor *= _Cutoff;
	return outputColor;
	
}
void Displace ( inout VertexData vd, inout VertexShaderInput v){
	half3 Surface = half3(1,1,1);
		//Generate layers for the Displacement channel.
			//Generate Layer: Zero
				//Sample parts of the layer:
					half4 ZeroSurface_Sample1 = 0;
	
	Surface = ZeroSurface_Sample1.rgb;//0
	
	
			//Generate Layer: Height Copy
				//Sample parts of the layer:
					half4 Height_CopySurface_Sample1 = (PerlinNoise2D((vd.genericTexcoord+float2(_Time.y * 0.1666667,0))*3)+1)/2;
	
	Surface = lerp(Surface,(Surface + Height_CopySurface_Sample1.rgb),_Small_Waves);//1
	
	
			//Generate Layer: Height Copy Copy
				//Sample parts of the layer:
					half4 Height_Copy_CopySurface_Sample1 = (PerlinNoise2D((vd.genericTexcoord*float2(0.5,0.5))*3)+1)/2;
	
	Surface = lerp(Surface,(Surface + Height_Copy_CopySurface_Sample1.rgb),_Large_Waves);//1
	
	
	
	v.vertex.xyz = float3(v.vertex.xyz + v.normal * Surface * 1);
	
}
float4 Mask_Mask0 ( VertexData vd){
		//Set default mask color
			float4 Mask0 = float4(0,0,0,0);
		//Generate layers for the UVs channel.
			//Generate Layer: Mask1
				//Sample parts of the layer:
					half4 Mask1Mask1_Sample1 = float4(vd.worldPos,1);
	
				//Apply Effects:
					Mask1Mask1_Sample1 = Mask1Mask1_Sample1.rbba;
					Mask1Mask1_Sample1.a = 1;
	
	Mask0 = Mask1Mask1_Sample1;//10
	
	
	return Mask0;
	
}
float Mask_Mask1 ( VertexData vd){
		//Set default mask color
			float Mask1 = 0;
		//Generate layers for the Mask0 channel.
			//Generate Layer: Mask0
				//Sample parts of the layer:
					half4 Mask0Mask0_Sample1 = GammaToLinear((PerlinNoise2D((((float3(vd.Mask0.r,vd.Mask0.g,0))*1)).xy*3)+1)/2);
	
				//Apply Effects:
					Mask0Mask0_Sample1.rgb = pow(Mask0Mask0_Sample1.rgb,8.28);
					Mask0Mask0_Sample1.rgb = Mask0Mask0_Sample1.rgb * ceil(saturate(dot(float3(0.3,0.59,0.11),Mask0Mask0_Sample1.rgb)-0.05));
					Mask0Mask0_Sample1.rgb = (Mask0Mask0_Sample1.rgb*2.39);
	
	Mask1 = Mask0Mask0_Sample1.r;//2
	
	
	return Mask1;
	
}
VertexToPixel Vertex (VertexShaderInput v){
	VertexToPixel vtp;
	UNITY_INITIALIZE_OUTPUT(VertexToPixel,vtp);
	VertexData vd;
	UNITY_INITIALIZE_OUTPUT(VertexData,vd);
	vd.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	vd.position = UnityObjectToClipPos(v.vertex);
	vd.worldNormal = UnityObjectToWorldNormalNew(v.normal);
	vd.worldTangent = UnityObjectToWorldNormalNew(v.tangent);
	vd.worldBitangent = cross(vd.worldNormal, vd.worldTangent) * v.tangent.w * unity_WorldTransformParams.w;
	vd.worldViewDir = normalize(UnityWorldSpaceViewDir(vd.worldPos));
	vd.TtoWSpaceX = float4(vd.worldTangent.x, vd.worldBitangent.x, vd.worldNormal.x, vd.worldPos.x);
	vd.TtoWSpaceY = float4(vd.worldTangent.y, vd.worldBitangent.y, vd.worldNormal.y, vd.worldPos.y);
	vd.TtoWSpaceZ = float4(vd.worldTangent.z, vd.worldBitangent.z, vd.worldNormal.z, vd.worldPos.z);
	vd.genericTexcoord = v.texcoord;
	vd.uv_BumpMap = TRANSFORM_TEX(v.texcoord, _BumpMap);
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
	
	
vd.Mask0 = Mask_Mask0 ( vd);
	Displace ( vd, v);
	vd.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	vd.position = UnityObjectToClipPos(v.vertex);
	vd.worldNormal = UnityObjectToWorldNormalNew(v.normal);
	vd.worldTangent = UnityObjectToWorldNormalNew(v.tangent);
	vd.worldBitangent = cross(vd.worldNormal, vd.worldTangent) * v.tangent.w * unity_WorldTransformParams.w;
	vd.worldViewDir = normalize(UnityWorldSpaceViewDir(vd.worldPos));
	vd.TtoWSpaceX = float4(vd.worldTangent.x, vd.worldBitangent.x, vd.worldNormal.x, vd.worldPos.x);
	vd.TtoWSpaceY = float4(vd.worldTangent.y, vd.worldBitangent.y, vd.worldNormal.y, vd.worldPos.y);
	vd.TtoWSpaceZ = float4(vd.worldTangent.z, vd.worldBitangent.z, vd.worldNormal.z, vd.worldPos.z);
	vd.genericTexcoord = v.texcoord;
	vd.uv_BumpMap = TRANSFORM_TEX(v.texcoord, _BumpMap);
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
	
	
	vtp.position = vd.position;
	vtp.worldViewDir = vd.worldViewDir;
	vtp.TtoWSpaceX = vd.TtoWSpaceX;
	vtp.TtoWSpaceY = vd.TtoWSpaceY;
	vtp.TtoWSpaceZ = vd.TtoWSpaceZ;
	vtp.genericTexcoord = vd.genericTexcoord;
	vtp.uv_BumpMap = vd.uv_BumpMap;
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
				vd.worldPos = half3(vtp.TtoWSpaceX.w,vtp.TtoWSpaceY.w,vtp.TtoWSpaceZ.w);
				vd.worldNormal = half3(vtp.TtoWSpaceX.z,vtp.TtoWSpaceY.z,vtp.TtoWSpaceZ.z);
				vd.worldViewDir = normalize(UnityWorldSpaceViewDir(vd.worldPos));
				vd.worldRefl = reflect(-vd.worldViewDir, vd.worldNormal);
				vd.TtoWSpaceX = vtp.TtoWSpaceX;
				vd.TtoWSpaceY = vtp.TtoWSpaceY;
				vd.TtoWSpaceZ = vtp.TtoWSpaceZ;
				vd.genericTexcoord = vtp.genericTexcoord;
				vd.uv_BumpMap = vtp.uv_BumpMap;
	#if UNITY_SHOULD_SAMPLE_SH
				vd.sh = vtp.sh;
#endif
	#if (!defined(LIGHTMAP_OFF))||(SHADER_TARGET >= 30)
				vd.lmap = vtp.lmap;
	#endif
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
vd.Mask1 = Mask_Mask1 ( vd);
				half3 outputTangent_Normals = Tangent_Normals ( vd);
				outputNormal = outputTangent_Normals.rgb;//0
								vd.worldNormal = outputNormal;
				vd.worldRefl = reflect(-vd.worldViewDir, vd.worldNormal);
				half4 outputDiffuse = Diffuse ( vd, gi, giInput);
				outputColor = half4(outputDiffuse.rgb,1);//7
								half4 outputSet_Alpha_Channel = Set_Alpha_Channel ( outputColor);
				outputColor = half4(outputSet_Alpha_Channel.rgb * outputSet_Alpha_Channel.a, outputSet_Alpha_Channel.a);//11
								half4 outputSpecular = Specular ( vd, gi, giInput);
				outputColor = ((outputColor) * (1 - (outputSpecular.a)) + (half4(outputSpecular.rgb, outputSpecular.a)));//1
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
				sampler2D _BumpMap;
float4 _BumpMap_ST;
float4 _BumpMap_HDR;
				float _Small_Wave_Texture_Scale;
				float _Second_Small_Waves_Texture_Scale;
				float _Parallax;
								float4 _Color;
				float _Cutoff;
				float _Small_Waves;
				float _Large_Waves;
				float _Foam_Amount;
				float4 _Foam_Color;
				float _Foam_Scale;

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




























struct VertexShaderInput{
	float4 vertex : POSITION;
	float4 tangent : TANGENT;
	float3 normal : NORMAL;
	float4 texcoord : TEXCOORD0;
};
struct VertexToPixel{
	float4 position : POSITION;
	float4 TtoWSpaceX : TEXCOORD0;
	float4 TtoWSpaceY : TEXCOORD1;
	float4 TtoWSpaceZ : TEXCOORD2;
	float2 genericTexcoord : TEXCOORD3;
	float2 uv_BumpMap : TEXCOORD4;
	#define pos position
		UNITY_FOG_COORDS(5)
#undef pos
	#ifdef SHADOWS_CUBE
		float3 vec : TEXCOORD6;
#endif
	#ifndef SHADOWS_CUBE
		#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
			float4 hpos : TEXCOORD7;
		#endif
	#endif
};

struct VertexData{
	float3 worldPos;
	float4 position;
	float3 worldNormal;
	float3 worldTangent;
	float3 worldBitangent;
	float3 worldViewDir;
	float4 TtoWSpaceX;
	float4 TtoWSpaceY;
	float4 TtoWSpaceZ;
	float2 genericTexcoord;
	float2 uv_BumpMap;
	#ifdef SHADOWS_CUBE
		float3 vec;
#endif
	#ifndef SHADOWS_CUBE
		#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
			float4 hpos;
		#endif
	#endif
	float4 Mask0;
	float Mask1;
	float Atten;
};
//OutputPremultiplied: False
//UseAlphaGenerate: True
half3 Tangent_Normals ( VertexData vd){
	half3 Surface = half3(0,0,1);
		//Generate layers for the Normals channel.
			//Generate Layer: Normal Map
				//Sample parts of the layer:
					half4 Normal_MapNormals_Sample1 = tex2D(_BumpMap,((vd.uv_BumpMap*float2(_Small_Wave_Texture_Scale,_Small_Wave_Texture_Scale))+float2(569.1406,_Time.y * 0.1666667)));
	
				//Apply Effects:
					Normal_MapNormals_Sample1 = float4(UnpackNormal(Normal_MapNormals_Sample1),Normal_MapNormals_Sample1.a);
					Normal_MapNormals_Sample1.a = 1;
	
	Surface = lerp(Surface,Normal_MapNormals_Sample1.rgb,Normal_MapNormals_Sample1.a * _Parallax);//1
	
	
			//Generate Layer: Normal Map Copy
				//Sample parts of the layer:
					half4 Normal_Map_CopyNormals_Sample1 = tex2D(_BumpMap,((vd.uv_BumpMap*float2(_Second_Small_Waves_Texture_Scale,_Second_Small_Waves_Texture_Scale))+float2(569.1406,_Time.y * 0.1666667)));
	
				//Apply Effects:
					Normal_Map_CopyNormals_Sample1 = float4(UnpackNormal(Normal_Map_CopyNormals_Sample1),Normal_Map_CopyNormals_Sample1.a);
					Normal_Map_CopyNormals_Sample1.a = 1;
	
	Surface = lerp(Surface,half4(normalize(half3(Surface.xy+Normal_Map_CopyNormals_Sample1.xy,Normal_Map_CopyNormals_Sample1.z)),Normal_Map_CopyNormals_Sample1.a).rgb,Normal_Map_CopyNormals_Sample1.a * _Parallax);//1
	
	
	return normalize(half3(dot(vd.TtoWSpaceX.xyz, Surface),dot(vd.TtoWSpaceY.xyz, Surface),dot(vd.TtoWSpaceZ.xyz, Surface)));
	
}
//OutputPremultiplied: False
//UseAlphaGenerate: False
half4 Diffuse ( VertexData vd){
	half4 Surface = half4(0,0,0,0);
		//Generate layers for the Albedo channel.
			//Generate Layer: Height Copy 2
				//Sample parts of the layer:
					half4 Height_Copy_2Albedo_Sample1 = GammaToLinear((PerlinNoise2D((vd.genericTexcoord+float2(_Time.y * 0.1666667,0))*3)+1)/2);
	
	Surface = half4(Height_Copy_2Albedo_Sample1.rgb,1);//7
	
	
			//Generate Layer: Texture
				//Sample parts of the layer:
					half4 TextureAlbedo_Sample1 = _Color;
	
	Surface = half4((Surface.rgb * TextureAlbedo_Sample1.rgb),1) ;//0
	
	
			//Generate Layer: Texture2
				//Sample parts of the layer:
					half4 Texture2Albedo_Sample1 = _Foam_Color;
	
	Surface.rgb = lerp(Surface.rgb,(Surface.rgb + Texture2Albedo_Sample1.rgb),_Foam_Amount * vd.Mask1);//6
	
	
	return Surface;
	
}
//OutputPremultiplied: False
//UseAlphaGenerate: True
half4 Set_Alpha_Channel ( half4 outputColor){
	float Surface = 0;
		//Generate layers for the Transparency channel.
			//Generate Layer: Transparency
				//Sample parts of the layer:
					half4 TransparencyTransparency_Sample1 = 1;
	
	Surface = TransparencyTransparency_Sample1.a;//2
	
	
	
	return float4(outputColor.rgb,Surface);
	
}
//OutputPremultiplied: False
//UseAlphaGenerate: True
half4 Specular ( VertexData vd){
	half3 Surface = half4(0,0,0,1);//Specular Mode
		//Generate layers for the Specular Color channel.
			//Generate Layer: Specular
				//Sample parts of the layer:
					half4 SpecularSpecular_Sample1 = _SpecColor;
	
	Surface = lerp(Surface,SpecularSpecular_Sample1.rgb,SpecularSpecular_Sample1.a);//1
	
	
	half reflectivity = SpecularStrength(Surface);
	half oneMinusReflectivity = 1-reflectivity;
	half nv = saturate(dot(vd.worldNormal, vd.worldViewDir));
	half nvPow5 = Pow5 (1-nv);
	return float4(Surface,lerp(1,nvPow5,oneMinusReflectivity));
	
}
float4 Transparency ( float4 outputColor){
	clip (outputColor.a - 0);
	outputColor *= _Cutoff;
	return outputColor;
	
}
void Displace ( inout VertexData vd, inout VertexShaderInput v){
	half3 Surface = half3(1,1,1);
		//Generate layers for the Displacement channel.
			//Generate Layer: Zero
				//Sample parts of the layer:
					half4 ZeroSurface_Sample1 = 0;
	
	Surface = ZeroSurface_Sample1.rgb;//0
	
	
			//Generate Layer: Height Copy
				//Sample parts of the layer:
					half4 Height_CopySurface_Sample1 = (PerlinNoise2D((vd.genericTexcoord+float2(_Time.y * 0.1666667,0))*3)+1)/2;
	
	Surface = lerp(Surface,(Surface + Height_CopySurface_Sample1.rgb),_Small_Waves);//1
	
	
			//Generate Layer: Height Copy Copy
				//Sample parts of the layer:
					half4 Height_Copy_CopySurface_Sample1 = (PerlinNoise2D((vd.genericTexcoord*float2(0.5,0.5))*3)+1)/2;
	
	Surface = lerp(Surface,(Surface + Height_Copy_CopySurface_Sample1.rgb),_Large_Waves);//1
	
	
	
	v.vertex.xyz = float3(v.vertex.xyz + v.normal * Surface * 1);
	
}
float4 Mask_Mask0 ( VertexData vd){
		//Set default mask color
			float4 Mask0 = float4(0,0,0,0);
		//Generate layers for the UVs channel.
			//Generate Layer: Mask1
				//Sample parts of the layer:
					half4 Mask1Mask1_Sample1 = float4(vd.worldPos,1);
	
				//Apply Effects:
					Mask1Mask1_Sample1 = Mask1Mask1_Sample1.rbba;
					Mask1Mask1_Sample1.a = 1;
	
	Mask0 = Mask1Mask1_Sample1;//10
	
	
	return Mask0;
	
}
float Mask_Mask1 ( VertexData vd){
		//Set default mask color
			float Mask1 = 0;
		//Generate layers for the Mask0 channel.
			//Generate Layer: Mask0
				//Sample parts of the layer:
					half4 Mask0Mask0_Sample1 = GammaToLinear((PerlinNoise2D((((float3(vd.Mask0.r,vd.Mask0.g,0))*1)).xy*3)+1)/2);
	
				//Apply Effects:
					Mask0Mask0_Sample1.rgb = pow(Mask0Mask0_Sample1.rgb,8.28);
					Mask0Mask0_Sample1.rgb = Mask0Mask0_Sample1.rgb * ceil(saturate(dot(float3(0.3,0.59,0.11),Mask0Mask0_Sample1.rgb)-0.05));
					Mask0Mask0_Sample1.rgb = (Mask0Mask0_Sample1.rgb*2.39);
	
	Mask1 = Mask0Mask0_Sample1.r;//2
	
	
	return Mask1;
	
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
	vd.worldTangent = UnityObjectToWorldNormalNew(v.tangent);
	vd.worldBitangent = cross(vd.worldNormal, vd.worldTangent) * v.tangent.w * unity_WorldTransformParams.w;
	vd.TtoWSpaceX = float4(vd.worldTangent.x, vd.worldBitangent.x, vd.worldNormal.x, vd.worldPos.x);
	vd.TtoWSpaceY = float4(vd.worldTangent.y, vd.worldBitangent.y, vd.worldNormal.y, vd.worldPos.y);
	vd.TtoWSpaceZ = float4(vd.worldTangent.z, vd.worldBitangent.z, vd.worldNormal.z, vd.worldPos.z);
	vd.genericTexcoord = v.texcoord;
	vd.uv_BumpMap = TRANSFORM_TEX(v.texcoord, _BumpMap);
	
	
vd.Mask0 = Mask_Mask0 ( vd);
	Displace ( vd, v);
	vd.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	vd.position = 0;
	TRANSFER_SHADOW_CASTER_NOPOS(vd,vd.position);
	vd.worldNormal = UnityObjectToWorldNormalNew(v.normal);
	vd.worldTangent = UnityObjectToWorldNormalNew(v.tangent);
	vd.worldBitangent = cross(vd.worldNormal, vd.worldTangent) * v.tangent.w * unity_WorldTransformParams.w;
	vd.TtoWSpaceX = float4(vd.worldTangent.x, vd.worldBitangent.x, vd.worldNormal.x, vd.worldPos.x);
	vd.TtoWSpaceY = float4(vd.worldTangent.y, vd.worldBitangent.y, vd.worldNormal.y, vd.worldPos.y);
	vd.TtoWSpaceZ = float4(vd.worldTangent.z, vd.worldBitangent.z, vd.worldNormal.z, vd.worldPos.z);
	vd.genericTexcoord = v.texcoord;
	vd.uv_BumpMap = TRANSFORM_TEX(v.texcoord, _BumpMap);
	
	
	vtp.position = vd.position;
	vtp.TtoWSpaceX = vd.TtoWSpaceX;
	vtp.TtoWSpaceY = vd.TtoWSpaceY;
	vtp.TtoWSpaceZ = vd.TtoWSpaceZ;
	vtp.genericTexcoord = vd.genericTexcoord;
	vtp.uv_BumpMap = vd.uv_BumpMap;
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
				vd.worldPos = half3(vtp.TtoWSpaceX.w,vtp.TtoWSpaceY.w,vtp.TtoWSpaceZ.w);
				vd.worldNormal = half3(vtp.TtoWSpaceX.z,vtp.TtoWSpaceY.z,vtp.TtoWSpaceZ.z);
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
				vd.TtoWSpaceX = vtp.TtoWSpaceX;
				vd.TtoWSpaceY = vtp.TtoWSpaceY;
				vd.TtoWSpaceZ = vtp.TtoWSpaceZ;
				vd.genericTexcoord = vtp.genericTexcoord;
				vd.uv_BumpMap = vtp.uv_BumpMap;
	#ifdef SHADOWS_CUBE
				vd.vec = vtp.vec;
#endif
	#ifndef SHADOWS_CUBE
		#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
				vd.hpos = vtp.hpos;
		#endif
	#endif
				outputNormal = vd.worldNormal;
vd.Mask1 = Mask_Mask1 ( vd);
				half3 outputTangent_Normals = Tangent_Normals ( vd);
				outputNormal = outputTangent_Normals.rgb;//0
								vd.worldNormal = outputNormal;
				half4 outputDiffuse = Diffuse ( vd);
				outputColor = half4(outputDiffuse.rgb,1);//7
								half4 outputSet_Alpha_Channel = Set_Alpha_Channel ( outputColor);
				outputColor = half4(outputSet_Alpha_Channel.rgb * outputSet_Alpha_Channel.a, outputSet_Alpha_Channel.a);//11
								half4 outputSpecular = Specular ( vd);
				outputColor = ((outputColor) * (1 - (outputSpecular.a)) + (half4(outputSpecular.rgb, outputSpecular.a)));//1
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
			Type(Text): "Image"
			VisName(Text): "Small Wave Texture"
			ImageDefault(Float): 0
			Image(Text): "fb6566c21f717904f83743a5a76dd0b0"
			NormalMap(Float): 1
			DefaultTexture(Text): "White"
			SeeTilingOffset(Toggle): True
			TilingOffset(Vec): 1,1,0,0
			MainType(Text): "BumpMap"
			CustomFallback(Text): "_BumpMap"
		End Shader Input


		Begin Shader Input
			Type(Text): "Float"
			VisName(Text): "Small Wave Texture Scale"
			Number(Float): 5
			Range0(Float): 0
			Range1(Float): 1
			CustomFallback(Text): "_Small_Wave_Texture_Scale"
		End Shader Input


		Begin Shader Input
			Type(Text): "Float"
			VisName(Text): "Second Small Waves Texture Scale"
			Number(Float): 18
			Range0(Float): 0
			Range1(Float): 1
			CustomFallback(Text): "_Second_Small_Waves_Texture_Scale"
		End Shader Input


		Begin Shader Input
			Type(Text): "Range"
			VisName(Text): "Small Wave Texture Strength"
			Number(Float): 0.7892562
			Range0(Float): 0
			Range1(Float): 1
			MainType(Text): "Parallax"
			CustomFallback(Text): "_Parallax"
		End Shader Input


		Begin Shader Input
			Type(Text): "Color"
			VisName(Text): "Specular - Main Color"
			Color(Vec): 0.2470588,0.2470588,0.2470588,1
			MainType(Text): "SpecularColor"
			CustomFallback(Text): "_SpecColor"
		End Shader Input


		Begin Shader Input
			Type(Text): "Color"
			VisName(Text): "Texture - Main Color"
			Color(Vec): 0.2392157,0.2705882,0.4196079,1
			MainType(Text): "MainColor"
			CustomFallback(Text): "_Color"
		End Shader Input


		Begin Shader Input
			Type(Text): "Range"
			VisName(Text): "Transparency"
			Number(Float): 0.6233278
			Range0(Float): 0
			Range1(Float): 1
			MainType(Text): "Cutoff"
			CustomFallback(Text): "_Cutoff"
		End Shader Input


		Begin Shader Input
			Type(Text): "Range"
			VisName(Text): "Small Waves"
			Number(Float): 0.161157
			Range0(Float): 0
			Range1(Float): 1
			CustomFallback(Text): "_Small_Waves"
		End Shader Input


		Begin Shader Input
			Type(Text): "Range"
			VisName(Text): "Large Waves"
			Number(Float): 0.161157
			Range0(Float): 0
			Range1(Float): 1
			CustomFallback(Text): "_Large_Waves"
		End Shader Input


		Begin Shader Input
			Type(Text): "Range"
			VisName(Text): "Foam Amount"
			Number(Float): 1
			Range0(Float): 0
			Range1(Float): 1
			CustomFallback(Text): "_Foam_Amount"
		End Shader Input


		Begin Shader Input
			Type(Text): "Color"
			VisName(Text): "Foam Color"
			Color(Vec): 0.627451,0.8,0.8823529,1
			CustomFallback(Text): "_Foam_Color"
		End Shader Input


		Begin Shader Input
			Type(Text): "Float"
			VisName(Text): "Foam Scale"
			Number(Float): 4
			Range0(Float): 0
			Range1(Float): 1
			CustomFallback(Text): "_Foam_Scale"
		End Shader Input


		Begin Shader Input
			Type(Text): "Float"
			VisName(Text): "X Offset"
			Number(Float): 32.56169
			Range0(Float): 0
			Range1(Float): 1
			SpecialType(Text): "Time"
			InputScale(Float): 0.1666667
			InEditor(Float): 0
			CustomFallback(Text): "_Time.y * 0.1666667"
		End Shader Input


		Begin Shader Input
			Type(Text): "Float"
			VisName(Text): "Mask0 - Z Offset"
			Number(Float): 32.56169
			Range0(Float): 0
			Range1(Float): 1
			SpecialType(Text): "Time"
			InputScale(Float): 0.1666667
			InEditor(Float): 0
			CustomFallback(Text): "_Time.y * 0.1666667"
		End Shader Input

		ShaderName(Text): "Shader Sandwich/Specific/Ocean"
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

				LayerListUniqueName(Text): "Mask1"
				LayerListName(Text): "UVs"
				Is Mask(Toggle): True
				EndTag(Text): "rgba"

				Begin Shader Layer
					Layer Name(Text): "Mask1"
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
					Stencil(ObjectArray): SSNone - {ObjectID = -2}
					Color(Vec): 0.5,0.8823529,1,1
					Number(Float): 0.5
					Begin Shader Effect
						TypeS(Text): "SSESwizzle"
						IsVisible(Toggle): True
						UseAlpha(Float): 1
						Channel R(Type): R - {TypeID = 0}
						Channel G(Type): B - {TypeID = 2}
						Channel B(Type): B - {TypeID = 2}
						Channel A(Type): A - {TypeID = 3}
					End Shader Effect

				End Shader Layer

			End Shader Layer List

			Begin Shader Layer List

				LayerListUniqueName(Text): "Mask0"
				LayerListName(Text): "Mask0"
				Is Mask(Toggle): True
				EndTag(Text): "r"

				Begin Shader Layer
					Layer Name(Text): "Mask0"
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
					Noise Dimensions(Type): 2D - {TypeID = 0}
					Image Based(Toggle): False
					Gamma Correct(Toggle): True
					Cubemap(Cubemap): 
					Color(Vec): 1,1,1,1
					Color 2(Vec): 0,0,0,1
					Texture(Texture): 
					Jitter(Float): 0
					Fill(Float): 0
					MinSize(Float): 0
					Edge(Float): 1
					MaxSize(Float): 1
					Square(Toggle): False
					Begin Shader Effect
						TypeS(Text): "SSEUVSet"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Values(ObjectArray): UVs - {ObjectID = 0}
						Scale(Float): 1
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEUVOffset"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						X Offset(Float): 0
						Y Offset(Float): 0
						Z Offset(Float): 79.73138 - {Input = 13}
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEUVScale"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Seperate(Toggle): False
						X Scale(Float): 4 - {Input = 11}
						Y Scale(Float): 4 - {Input = 11}
						Z Scale(Float): 4 - {Input = 11}
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEMathPow"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Power(Float): 8.28
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEThreshold"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Threshold(Float): 0.05
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEMathMul"
						IsVisible(Toggle): True
						UseAlpha(Float): 0
						Multiply(Float): 2.39
					End Shader Effect

				End Shader Layer

			End Shader Layer List

		End Masks

		Begin Shader Pass
			Name(Text): "Base"
			Visible(Toggle): True

			Begin Shader Ingredient

				Type(Text): "ShaderSurfaceTangentNormals"
				User Name(Text): "Tangent Normals"
				Use Custom Lighting(Toggle): False
				Use Custom Ambient(Toggle): False
				Alpha Blend Mode(Type): Blend - {TypeID = 0}
				Mix Amount(Float): 1
				Mix Type(Type): Mix - {TypeID = 0}
				Use Alpha(Toggle): True
				ShouldLink(Toggle): False
				Normalize(Toggle): True

				Begin Shader Layer List

					LayerListUniqueName(Text): "Normals"
					LayerListName(Text): "Normals"
					Is Mask(Toggle): False
					EndTag(Text): "rgb"

					Begin Shader Layer
						Layer Name(Text): "Normal Map"
						Layer Type(ObjectArray): SLTTexture - {ObjectID = 1}
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
						Mix Amount(Float): 0.7892562 - {Input = 3}
						Mix Type(Type): Mix - {TypeID = 0}
						Stencil(ObjectArray): SSNone - {ObjectID = -1}
						Texture(Texture): fb6566c21f717904f83743a5a76dd0b0 - {Input = 0}
						Cubemap(Cubemap): 
						Noise Dimensions(Type): SSN/A - {TypeID = 0}
						Color(Vec): 0,0,1,1
						Color 2(Vec): 0,0,0,1
						Jitter(Float): 0
						Fill(Float): 0
						MinSize(Float): 0
						Edge(Float): 1
						MaxSize(Float): 1
						Square(Toggle): False
						Begin Shader Effect
							TypeS(Text): "SSEUVOffset"
							IsVisible(Toggle): True
							UseAlpha(Float): 0
							X Offset(Float): 569.1406
							Y Offset(Float): 79.73138 - {Input = 12}
							Z Offset(Float): 0
						End Shader Effect

						Begin Shader Effect
							TypeS(Text): "SSEUVScale"
							IsVisible(Toggle): True
							UseAlpha(Float): 0
							Seperate(Toggle): False
							X Scale(Float): 5 - {Input = 1}
							Y Scale(Float): 5 - {Input = 1}
							Z Scale(Float): 5 - {Input = 1}
						End Shader Effect

						Begin Shader Effect
							TypeS(Text): "SSEUnpackNormal"
							IsVisible(Toggle): True
							UseAlpha(Float): 1
						End Shader Effect

					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Normal Map Copy"
						Layer Type(ObjectArray): SLTTexture - {ObjectID = 1}
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
						Mix Amount(Float): 0.7892562 - {Input = 3}
						Mix Type(Type): Normals Mix - {TypeID = 7}
						Stencil(ObjectArray): SSNone - {ObjectID = -1}
						Texture(Texture): fb6566c21f717904f83743a5a76dd0b0 - {Input = 0}
						Cubemap(Cubemap): 
						Noise Dimensions(Type): SSN/A - {TypeID = 0}
						Color(Vec): 0,0,1,1
						Color 2(Vec): 0,0,0,1
						Jitter(Float): 0
						Fill(Float): 0
						MinSize(Float): 0
						Edge(Float): 1
						MaxSize(Float): 1
						Square(Toggle): False
						Begin Shader Effect
							TypeS(Text): "SSEUVOffset"
							IsVisible(Toggle): True
							UseAlpha(Float): 0
							X Offset(Float): 569.1406
							Y Offset(Float): 79.73138 - {Input = 12}
							Z Offset(Float): 0
						End Shader Effect

						Begin Shader Effect
							TypeS(Text): "SSEUVScale"
							IsVisible(Toggle): True
							UseAlpha(Float): 0
							Seperate(Toggle): False
							X Scale(Float): 18 - {Input = 2}
							Y Scale(Float): 18 - {Input = 2}
							Z Scale(Float): 18 - {Input = 2}
						End Shader Effect

						Begin Shader Effect
							TypeS(Text): "SSEUnpackNormal"
							IsVisible(Toggle): True
							UseAlpha(Float): 1
						End Shader Effect

					End Shader Layer

				End Shader Layer List

			End Shader Ingredient


			Begin Shader Ingredient

				Type(Text): "ShaderSurfaceDiffuse"
				User Name(Text): "Diffuse"
				Use Custom Lighting(Toggle): False
				Use Custom Ambient(Toggle): False
				Alpha Blend Mode(Type): Blend - {TypeID = 0}
				Mix Amount(Float): 1
				Mix Type(Type): Mix - {TypeID = 0}
				Use Alpha(Toggle): True
				ShouldLink(Toggle): True
				Lighting Type(Type): Unity Standard - {TypeID = 1}
				Roughness Or Smoothness(Type): Smoothness - {TypeID = 0}
				Smoothness(Float): 0.926
				Roughness(Float): 0
				Light Size(Float): 0
				Wrap Amount(Float): 0
				Wrap Color(Vec): 0.4,0.2,0.2,1
				PBR Quality(Type): Auto - {TypeID = 0}
				Disable Normals(Float): 0
				Use Ambient(Toggle): True
				Use Tangents(Toggle): False

				Begin Shader Layer List

					LayerListUniqueName(Text): "Albedo"
					LayerListName(Text): "Albedo"
					Is Mask(Toggle): False
					EndTag(Text): "rgba"

					Begin Shader Layer
						Layer Name(Text): "Height Copy 2"
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
						Stencil(ObjectArray): SSNone - {ObjectID = -1}
						Noise Dimensions(Type): 2D - {TypeID = 0}
						Image Based(Toggle): False
						Gamma Correct(Toggle): True
						Cubemap(Cubemap): 
						Color(Vec): 1,1,1,1
						Color 2(Vec): 0,0,0,1
						Texture(Texture): 
						Jitter(Float): 0
						Fill(Float): 0
						MinSize(Float): 0
						Edge(Float): 1
						MaxSize(Float): 1
						Square(Toggle): False
						Begin Shader Effect
							TypeS(Text): "SSEUVOffset"
							IsVisible(Toggle): True
							UseAlpha(Float): 0
							X Offset(Float): 79.73138 - {Input = 12}
							Y Offset(Float): 0
							Z Offset(Float): 0
						End Shader Effect

					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Texture"
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
						Mix Type(Type): Multiply - {TypeID = 3}
						Stencil(ObjectArray): SSNone - {ObjectID = -1}
						Color(Vec): 0.2392157,0.2705882,0.4196079,1 - {Input = 5}
						Cubemap(Cubemap): 
						Noise Dimensions(Type): SSN/A - {TypeID = 0}
						Color 2(Vec): 0,0,0,1
						Texture(Texture): 
						Jitter(Float): 0
						Fill(Float): 0
						MinSize(Float): 0
						Edge(Float): 1
						MaxSize(Float): 1
						Square(Toggle): False
					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Texture2"
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
						Mix Amount(Float): 1 - {Input = 9}
						Mix Type(Type): Add - {TypeID = 1}
						Stencil(ObjectArray): Mask0 - {ObjectID = 1}
						Color(Vec): 0.627451,0.8,0.8823529,1 - {Input = 10}
						Cubemap(Cubemap): 
						Noise Dimensions(Type): SSN/A - {TypeID = 0}
						Color 2(Vec): 0,0,0,1
						Texture(Texture): 
						Jitter(Float): 0
						Fill(Float): 0
						MinSize(Float): 0
						Edge(Float): 1
						MaxSize(Float): 1
						Square(Toggle): False
					End Shader Layer

				End Shader Layer List

			End Shader Ingredient


			Begin Shader Ingredient

				Type(Text): "ShaderSurfaceTransparency"
				User Name(Text): "Transparency"
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
						Layer Name(Text): "Transparency"
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
						Number(Float): 1
						Color(Vec): 0.5,0.8823529,1,1
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
				Mix Type(Type): Mix - {TypeID = 0}
				Use Alpha(Toggle): True
				ShouldLink(Toggle): True
				Specular Type(Type): The Greatest Sandwich\n...shader model - {TypeID = 0}
				Roughness Or Smoothness(Type): Smoothness - {TypeID = 0}
				Smoothness(Float): 0.926
				Roughness(Float): 0
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
						Layer Name(Text): "Specular"
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
						Use Alpha(Toggle): True
						Alpha Blend Mode(Type): Blend - {TypeID = 0}
						Mix Amount(Float): 1
						Mix Type(Type): Mix - {TypeID = 0}
						Stencil(ObjectArray): SSNone - {ObjectID = -1}
						Color(Vec): 0.2470588,0.2470588,0.2470588,1 - {Input = 4}
						Cubemap(Cubemap): 
						Noise Dimensions(Type): SSN/A - {TypeID = 0}
						Color 2(Vec): 0,0,0,1
						Texture(Texture): 
						Jitter(Float): 0
						Fill(Float): 0
						MinSize(Float): 0
						Edge(Float): 1
						MaxSize(Float): 1
						Square(Toggle): False
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
				Generate Forward Add Pass(Toggle): True
				Generate Deferred Pass(Toggle): True
				Generate Shadow Caster Pass(Toggle): True

			End Shader Ingredient


			Begin Shader Ingredient

				Type(Text): "ShaderGeometryModifierTransparency"
				User Name(Text): "Transparency"
				Mix Amount(Float): 1
				ShouldLink(Toggle): False
				Transparency Type(Type): Fade - {TypeID = 1}
				Transparency ZWrite(Type): Full - {TypeID = 2}
				Shadow Type Fade(Type): Cutout - {TypeID = 1}
				Cutout Amount(Float): 0
				Transparency(Float): 0.6233278 - {Input = 6}
				Blend Mode(Type): Mix - {TypeID = 0}

			End Shader Ingredient


			Begin Shader Ingredient

				Type(Text): "ShaderGeometryModifierDisplacement"
				User Name(Text): "Displace"
				Mix Amount(Float): 1
				ShouldLink(Toggle): False
				Direction(Type): Normal - {TypeID = 0}
				Coordinate Space(Type): Object - {TypeID = 0}
				Strength(Float): 1
				MidLevel(Float): 0
				Independent XYZ(Toggle): True

				Begin Shader Layer List

					LayerListUniqueName(Text): "Surface"
					LayerListName(Text): "Displacement"
					Is Mask(Toggle): False
					EndTag(Text): "rgb"

					Begin Shader Layer
						Layer Name(Text): "Zero"
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
						Color(Vec): 0.5,0.8823529,1,1
					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Height Copy"
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
						Mix Amount(Float): 0.161157 - {Input = 7}
						Mix Type(Type): Add - {TypeID = 1}
						Stencil(ObjectArray): SSNone - {ObjectID = -1}
						Noise Dimensions(Type): 2D - {TypeID = 0}
						Image Based(Toggle): False
						Gamma Correct(Toggle): False
						Cubemap(Cubemap): 
						Color(Vec): 1,1,1,1
						Color 2(Vec): 0,0,0,1
						Texture(Texture): 
						Jitter(Float): 0
						Fill(Float): 0
						MinSize(Float): 0
						Edge(Float): 1
						MaxSize(Float): 1
						Square(Toggle): False
						Begin Shader Effect
							TypeS(Text): "SSEUVOffset"
							IsVisible(Toggle): True
							UseAlpha(Float): 0
							X Offset(Float): 79.73138 - {Input = 12}
							Y Offset(Float): 0
							Z Offset(Float): 0
						End Shader Effect

					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Height Copy Copy"
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
						Mix Amount(Float): 0.161157 - {Input = 8}
						Mix Type(Type): Add - {TypeID = 1}
						Stencil(ObjectArray): SSNone - {ObjectID = -1}
						Noise Dimensions(Type): 2D - {TypeID = 0}
						Image Based(Toggle): False
						Gamma Correct(Toggle): False
						Cubemap(Cubemap): 
						Color(Vec): 1,1,1,1
						Color 2(Vec): 0,0,0,1
						Texture(Texture): 
						Jitter(Float): 0
						Fill(Float): 0
						MinSize(Float): 0
						Edge(Float): 1
						MaxSize(Float): 1
						Square(Toggle): False
						Begin Shader Effect
							TypeS(Text): "SSEUVScale"
							IsVisible(Toggle): True
							UseAlpha(Float): 0
							Seperate(Toggle): False
							X Scale(Float): 0.5
							Y Scale(Float): 0.5
							Z Scale(Float): 0.5
						End Shader Effect

					End Shader Layer

				End Shader Layer List

			End Shader Ingredient

		End Shader Pass

	End Shader Base
End Shader Sandwich Shader
*/
