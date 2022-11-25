// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shader Sandwich/Specific/Burn" {//The Shaders Name
//The inputs shown in the material panel
Properties {
	_MainTex ("Base", 2D) = "white" {}
	_Burn_Texture ("Burn Texture", 2D) = "white" {}
	_Color ("Burn Color", Color) = (1,0.6117647,0.02941179,1)
	_Cutoff ("Burn Amount", Range(-0.500000000,1.200000000)) = 0.325483800
	_Glow ("Glow", Range(1.000000000,20.000000000)) = 10.116350000
	_Glow_Width ("Glow Width", Range(-1.000000000,0.000000000)) = -0.572403800
}

SubShader {
	Tags { "RenderType"="Opaque" "Queue"="AlphaTest" }//A bunch of settings telling Unity a bit about the shader.
	LOD 200
AlphaToMask Off
	Pass {
		Name "FORWARD"
		Tags { "LightMode" = "ForwardBase" }
	ZTest LEqual
	ZWrite On
	Blend Off//No transparency
	Cull Off//Culling specifies which sides of the models faces to hide.

		
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
				sampler2D _MainTex;
float4 _MainTex_ST;
float4 _MainTex_HDR;
				sampler2D _Burn_Texture;
float4 _Burn_Texture_ST;
float4 _Burn_Texture_HDR;
				float4 _Color;
				float _Cutoff;
				float _Glow;
				float _Glow_Width;

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
	float3 normal : NORMAL;
	float4 texcoord : TEXCOORD0;
	float4 texcoord1 : TEXCOORD1;
	float4 texcoord2 : TEXCOORD2;
};
struct VertexToPixel{
	float3 worldPos : TEXCOORD0;
	float4 position : POSITION;
	float3 worldNormal : TEXCOORD1;
	float2 uv_MainTex : TEXCOORD2;
	float2 uv_Burn_Texture : TEXCOORD3;
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
	float2 uv_MainTex;
	float2 uv_Burn_Texture;
	#if UNITY_SHOULD_SAMPLE_SH
		float3 sh;
#endif
	#if (!defined(LIGHTMAP_OFF))||(SHADER_TARGET >= 30)
		float4 lmap;
	#endif
	float Mask1;
	float Atten;
};
//OutputPremultiplied: False
//UseAlphaGenerate: False
half4 Diffuse ( VertexData vd, UnityGI gi, UnityGIInput giInput){
	half4 Surface = half4(0,0,0,0);
		//Generate layers for the Albedo channel.
			//Generate Layer: Texture
				//Sample parts of the layer:
					half4 TextureAlbedo_Sample1 = tex2D(_MainTex,vd.uv_MainTex);
	
	Surface = half4(TextureAlbedo_Sample1.rgb,1);//7
	
	
	float3 Lighting;
	#if SSUNITY_BRDF_PBS==1
	half roughness = 1-0.3;
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
	g.roughness = (1-0.3);
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
half4 Set_Alpha_Channel ( VertexData vd, half4 outputColor){
	float Surface = 0;
		//Generate layers for the Transparency channel.
			//Generate Layer: Alpha
				//Sample parts of the layer:
					half4 AlphaTransparency_Sample1 = tex2D(_Burn_Texture,vd.uv_Burn_Texture);
	
	Surface = AlphaTransparency_Sample1.a;//2
	
	
	
	return float4(outputColor.rgb,Surface);
	
}
//OutputPremultiplied: False
//UseAlphaGenerate: False
half4 Emission ( VertexData vd){
	half4 Surface = half4(0,0,0,0);
		//Generate layers for the Emission channel.
			//Generate Layer: Alpha Copy
				//Sample parts of the layer:
					half4 Alpha_CopyEmission_Sample1 = tex2D(_Burn_Texture,vd.uv_Burn_Texture);
	
				//Apply Effects:
					Alpha_CopyEmission_Sample1.rgb = float3(dot(float3(0.3,0.59,0.11),Alpha_CopyEmission_Sample1.rgb.rgb),dot(float3(0.3,0.59,0.11),Alpha_CopyEmission_Sample1.rgb.rgb),dot(float3(0.3,0.59,0.11),Alpha_CopyEmission_Sample1.rgb.rgb));
	
	Surface = lerp(Surface,half4(Alpha_CopyEmission_Sample1.rgb, 1),vd.Mask1);//2
	
	
			//Generate Layer: Emission 2 Copy
				//Sample parts of the layer:
					half4 Emission_2_CopyEmission_Sample1 = _Color;
	
	Surface = half4((Surface.rgb * Emission_2_CopyEmission_Sample1.rgb),1) ;//0
	
	
			//Generate Layer: Emission Copy
				//Sample parts of the layer:
					half4 Emission_CopyEmission_Sample1 = Surface;
	
				//Apply Effects:
					Emission_CopyEmission_Sample1.rgb = (Emission_CopyEmission_Sample1.rgb*_Glow);
	
	Surface = half4(Emission_CopyEmission_Sample1.rgb,1) ;//0
	
	
	return Surface;
}
float4 Transparency ( float4 outputColor){
	outputColor.a -= _Cutoff;
	clip(outputColor.a);
	return outputColor;
	
}
float Mask_Mask1 ( VertexData vd){
		//Set default mask color
			float Mask1 = 0;
		//Generate layers for the Mask1 channel.
			//Generate Layer: Mask0 Copy
				//Sample parts of the layer:
					half4 Mask0_CopyMask1_Sample1 = tex2D(_Burn_Texture,vd.uv_Burn_Texture);
	
				//Apply Effects:
					Mask0_CopyMask1_Sample1 = (Mask0_CopyMask1_Sample1-(_Cutoff));
					Mask0_CopyMask1_Sample1 = (Mask0_CopyMask1_Sample1-(_Glow_Width));
					Mask0_CopyMask1_Sample1 = (float4(1,1,1,1)-Mask0_CopyMask1_Sample1);
					Mask0_CopyMask1_Sample1 = lerp(float4(0.5,0.5,0.5,0.5),Mask0_CopyMask1_Sample1,3);
					Mask0_CopyMask1_Sample1 = clamp(Mask0_CopyMask1_Sample1,0,1);
	
	Mask1 = Mask0_CopyMask1_Sample1.a;//2
	
	
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
	vd.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
	vd.uv_Burn_Texture = TRANSFORM_TEX(v.texcoord, _Burn_Texture);
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
	vtp.uv_MainTex = vd.uv_MainTex;
	vtp.uv_Burn_Texture = vd.uv_Burn_Texture;
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
				vd.uv_MainTex = vtp.uv_MainTex;
				vd.uv_Burn_Texture = vtp.uv_Burn_Texture;
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
				half4 outputDiffuse = Diffuse ( vd, gi, giInput);
				outputColor = half4(outputDiffuse.rgb,1);//7
								half4 outputSet_Alpha_Channel = Set_Alpha_Channel ( vd, outputColor);
				outputColor = outputSet_Alpha_Channel;//10
								half4 outputEmission = Emission ( vd);
				outputColor.rgb = (outputColor.rgb + outputEmission.rgb);//4
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
	ZWrite On
	Blend One One
	Cull Off//Culling specifies which sides of the models faces to hide.

		
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
				sampler2D _MainTex;
float4 _MainTex_ST;
float4 _MainTex_HDR;
				sampler2D _Burn_Texture;
float4 _Burn_Texture_ST;
float4 _Burn_Texture_HDR;
				float4 _Color;
				float _Cutoff;
				float _Glow;
				float _Glow_Width;

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
	float3 normal : NORMAL;
	float4 texcoord : TEXCOORD0;
	float4 texcoord1 : TEXCOORD1;
	float4 texcoord2 : TEXCOORD2;
};
struct VertexToPixel{
	float3 worldPos : TEXCOORD0;
	float4 position : POSITION;
	float3 worldNormal : TEXCOORD1;
	float2 uv_MainTex : TEXCOORD2;
	float2 uv_Burn_Texture : TEXCOORD3;
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
	float2 uv_MainTex;
	float2 uv_Burn_Texture;
	#if UNITY_SHOULD_SAMPLE_SH
		float3 sh;
#endif
	#if (!defined(LIGHTMAP_OFF))||(SHADER_TARGET >= 30)
		float4 lmap;
	#endif
	float Mask1;
	float Atten;
};
//OutputPremultiplied: False
//UseAlphaGenerate: False
half4 Diffuse ( VertexData vd, UnityGI gi, UnityGIInput giInput){
	half4 Surface = half4(0,0,0,0);
		//Generate layers for the Albedo channel.
			//Generate Layer: Texture
				//Sample parts of the layer:
					half4 TextureAlbedo_Sample1 = tex2D(_MainTex,vd.uv_MainTex);
	
	Surface = half4(TextureAlbedo_Sample1.rgb,1);//7
	
	
	float3 Lighting;
	#if SSUNITY_BRDF_PBS==1
	half roughness = 1-0.3;
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
half4 Set_Alpha_Channel ( VertexData vd, half4 outputColor){
	float Surface = 0;
		//Generate layers for the Transparency channel.
			//Generate Layer: Alpha
				//Sample parts of the layer:
					half4 AlphaTransparency_Sample1 = tex2D(_Burn_Texture,vd.uv_Burn_Texture);
	
	Surface = AlphaTransparency_Sample1.a;//2
	
	
	
	return float4(outputColor.rgb,Surface);
	
}
float4 Transparency ( float4 outputColor){
	outputColor.a -= _Cutoff;
	clip(outputColor.a);
	return outputColor;
	
}
float Mask_Mask1 ( VertexData vd){
		//Set default mask color
			float Mask1 = 0;
		//Generate layers for the Mask1 channel.
			//Generate Layer: Mask0 Copy
				//Sample parts of the layer:
					half4 Mask0_CopyMask1_Sample1 = tex2D(_Burn_Texture,vd.uv_Burn_Texture);
	
				//Apply Effects:
					Mask0_CopyMask1_Sample1 = (Mask0_CopyMask1_Sample1-(_Cutoff));
					Mask0_CopyMask1_Sample1 = (Mask0_CopyMask1_Sample1-(_Glow_Width));
					Mask0_CopyMask1_Sample1 = (float4(1,1,1,1)-Mask0_CopyMask1_Sample1);
					Mask0_CopyMask1_Sample1 = lerp(float4(0.5,0.5,0.5,0.5),Mask0_CopyMask1_Sample1,3);
					Mask0_CopyMask1_Sample1 = clamp(Mask0_CopyMask1_Sample1,0,1);
	
	Mask1 = Mask0_CopyMask1_Sample1.a;//2
	
	
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
	vd.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
	vd.uv_Burn_Texture = TRANSFORM_TEX(v.texcoord, _Burn_Texture);
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
	vtp.uv_MainTex = vd.uv_MainTex;
	vtp.uv_Burn_Texture = vd.uv_Burn_Texture;
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
				vd.uv_MainTex = vtp.uv_MainTex;
				vd.uv_Burn_Texture = vtp.uv_Burn_Texture;
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
				half4 outputDiffuse = Diffuse ( vd, gi, giInput);
				outputColor = half4(outputDiffuse.rgb,1);//7
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
	Cull Off//Culling specifies which sides of the models faces to hide.

		
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
				sampler2D _MainTex;
float4 _MainTex_ST;
float4 _MainTex_HDR;
				sampler2D _Burn_Texture;
float4 _Burn_Texture_ST;
float4 _Burn_Texture_HDR;
				float4 _Color;
				float _Cutoff;
				float _Glow;
				float _Glow_Width;

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
	float3 normal : NORMAL;
	float4 texcoord : TEXCOORD0;
};
struct VertexToPixel{
	float4 position : POSITION;
	float2 uv_MainTex : TEXCOORD0;
	float2 uv_Burn_Texture : TEXCOORD1;
	#define pos position
		UNITY_FOG_COORDS(2)
#undef pos
	#ifdef SHADOWS_CUBE
		float3 vec : TEXCOORD3;
#endif
	#ifndef SHADOWS_CUBE
		#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
			float4 hpos : TEXCOORD4;
		#endif
	#endif
};

struct VertexData{
	float3 worldPos;
	float4 position;
	float3 worldNormal;
	float2 uv_MainTex;
	float2 uv_Burn_Texture;
	#ifdef SHADOWS_CUBE
		float3 vec;
#endif
	#ifndef SHADOWS_CUBE
		#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
			float4 hpos;
		#endif
	#endif
	float Mask1;
	float Atten;
};
//OutputPremultiplied: False
//UseAlphaGenerate: False
half4 Diffuse ( VertexData vd){
	half4 Surface = half4(0,0,0,0);
		//Generate layers for the Albedo channel.
			//Generate Layer: Texture
				//Sample parts of the layer:
					half4 TextureAlbedo_Sample1 = tex2D(_MainTex,vd.uv_MainTex);
	
	Surface = half4(TextureAlbedo_Sample1.rgb,1);//7
	
	
	return Surface;
	
}
//OutputPremultiplied: False
//UseAlphaGenerate: True
half4 Set_Alpha_Channel ( VertexData vd, half4 outputColor){
	float Surface = 0;
		//Generate layers for the Transparency channel.
			//Generate Layer: Alpha
				//Sample parts of the layer:
					half4 AlphaTransparency_Sample1 = tex2D(_Burn_Texture,vd.uv_Burn_Texture);
	
	Surface = AlphaTransparency_Sample1.a;//2
	
	
	
	return float4(outputColor.rgb,Surface);
	
}
float4 Transparency ( float4 outputColor){
	clip (outputColor.a - _Cutoff);
	return outputColor;
	
}
float Mask_Mask1 ( VertexData vd){
		//Set default mask color
			float Mask1 = 0;
		//Generate layers for the Mask1 channel.
			//Generate Layer: Mask0 Copy
				//Sample parts of the layer:
					half4 Mask0_CopyMask1_Sample1 = tex2D(_Burn_Texture,vd.uv_Burn_Texture);
	
				//Apply Effects:
					Mask0_CopyMask1_Sample1 = (Mask0_CopyMask1_Sample1-(_Cutoff));
					Mask0_CopyMask1_Sample1 = (Mask0_CopyMask1_Sample1-(_Glow_Width));
					Mask0_CopyMask1_Sample1 = (float4(1,1,1,1)-Mask0_CopyMask1_Sample1);
					Mask0_CopyMask1_Sample1 = lerp(float4(0.5,0.5,0.5,0.5),Mask0_CopyMask1_Sample1,3);
					Mask0_CopyMask1_Sample1 = clamp(Mask0_CopyMask1_Sample1,0,1);
	
	Mask1 = Mask0_CopyMask1_Sample1.a;//2
	
	
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
	vd.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
	vd.uv_Burn_Texture = TRANSFORM_TEX(v.texcoord, _Burn_Texture);
	
	
	
	
	vtp.position = vd.position;
	vtp.uv_MainTex = vd.uv_MainTex;
	vtp.uv_Burn_Texture = vd.uv_Burn_Texture;
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
				vd.uv_MainTex = vtp.uv_MainTex;
				vd.uv_Burn_Texture = vtp.uv_Burn_Texture;
	#ifdef SHADOWS_CUBE
				vd.vec = vtp.vec;
#endif
	#ifndef SHADOWS_CUBE
		#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
				vd.hpos = vtp.hpos;
		#endif
	#endif
vd.Mask1 = Mask_Mask1 ( vd);
				half4 outputDiffuse = Diffuse ( vd);
				outputColor = half4(outputDiffuse.rgb,1);//7
								half4 outputSet_Alpha_Channel = Set_Alpha_Channel ( vd, outputColor);
				outputColor = outputSet_Alpha_Channel;//10
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
			VisName(Text): "Base"
			ImageDefault(Float): 0
			Image(Text): "df74a0cbfe3033e48af5c068e2386265"
			NormalMap(Float): 0
			DefaultTexture(Text): "White"
			SeeTilingOffset(Toggle): True
			TilingOffset(Vec): 1,1,0,0
			MainType(Text): "MainTexture"
			CustomFallback(Text): "_MainTex"
		End Shader Input


		Begin Shader Input
			Type(Text): "Image"
			VisName(Text): "Burn Texture"
			ImageDefault(Float): 0
			Image(Text): "354f7eb7f261f094399e01c57c0c2a0d"
			NormalMap(Float): 0
			DefaultTexture(Text): "White"
			SeeTilingOffset(Toggle): True
			TilingOffset(Vec): 1,1,0,0
			CustomFallback(Text): "_Burn_Texture"
		End Shader Input


		Begin Shader Input
			Type(Text): "Color"
			VisName(Text): "Burn Color"
			Color(Vec): 1,0.6117647,0.02941179,1
			MainType(Text): "MainColor"
			CustomFallback(Text): "_Color"
		End Shader Input


		Begin Shader Input
			Type(Text): "Range"
			VisName(Text): "Burn Amount"
			Number(Float): 0.3254838
			Range0(Float): -0.5
			Range1(Float): 1.2
			MainType(Text): "Cutoff"
			CustomFallback(Text): "_Cutoff"
		End Shader Input


		Begin Shader Input
			Type(Text): "Range"
			VisName(Text): "Glow"
			Number(Float): 10.11635
			Range0(Float): 1
			Range1(Float): 20
			CustomFallback(Text): "_Glow"
		End Shader Input


		Begin Shader Input
			Type(Text): "Range"
			VisName(Text): "Glow Width"
			Number(Float): -0.5724038
			Range0(Float): -1
			Range1(Float): 0
			CustomFallback(Text): "_Glow_Width"
		End Shader Input

		ShaderName(Text): "Shader Sandwich/Specific/Burn"
		Tech Lod(Float): 200
		Fallback(Type): Diffuse - {TypeID = 0}
		CustomFallback(Text): "\qLegacy Shaders/Diffuse\q"
		Queue(Type): Auto - {TypeID = 0}
		Custom Queue(Float): 2000
		QueueAuto(Toggle): True
		Replacement(Type): Auto - {TypeID = 0}
		ReplacementAuto(Toggle): True
		Tech Shader Target(Float): 2
		Exclude DX9(Toggle): False

		Begin Masks

			Begin Shader Layer List

				LayerListUniqueName(Text): "Mask0"
				LayerListName(Text): "Mask0"
				Is Mask(Toggle): True
				EndTag(Text): "a"

				Begin Shader Layer
					Layer Name(Text): "Mask0"
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
					Mix Amount(Float): 1
					Mix Type(Type): Mix - {TypeID = 0}
					Stencil(ObjectArray): SSNone - {ObjectID = -1}
					Cubemap(Cubemap): 
					Noise Dimensions(Type): SSN/A - {TypeID = 0}
					Color(Vec): 1,1,1,1
					Texture(Texture): 354f7eb7f261f094399e01c57c0c2a0d - {Input = 1}
					Color 2(Vec): 0,0,0,1
					Jitter(Float): 0
					Fill(Float): 0
					MinSize(Float): 0
					Edge(Float): 1
					MaxSize(Float): 1
					Square(Toggle): False
				End Shader Layer

			End Shader Layer List

			Begin Shader Layer List

				LayerListUniqueName(Text): "Mask1"
				LayerListName(Text): "Mask1"
				Is Mask(Toggle): True
				EndTag(Text): "a"

				Begin Shader Layer
					Layer Name(Text): "Mask0 Copy"
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
					Use Alpha(Toggle): True
					Alpha Blend Mode(Type): Blend - {TypeID = 0}
					Mix Amount(Float): 1
					Mix Type(Type): Mix - {TypeID = 0}
					Stencil(ObjectArray): SSNone - {ObjectID = -1}
					Cubemap(Cubemap): 
					Noise Dimensions(Type): SSN/A - {TypeID = 0}
					Color(Vec): 1,1,1,1
					Texture(Texture): 354f7eb7f261f094399e01c57c0c2a0d - {Input = 1}
					Color 2(Vec): 0,0,0,1
					Jitter(Float): 0
					Fill(Float): 0
					MinSize(Float): 0
					Edge(Float): 1
					MaxSize(Float): 1
					Square(Toggle): False
					Begin Shader Effect
						TypeS(Text): "SSEMathSub"
						IsVisible(Toggle): True
						UseAlpha(Float): 1
						Subtract(Float): 0.3254838 - {Input = 3}
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEMathSub"
						IsVisible(Toggle): True
						UseAlpha(Float): 1
						Subtract(Float): -0.5724038 - {Input = 5}
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEInvert"
						IsVisible(Toggle): True
						UseAlpha(Float): 1
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEContrast"
						IsVisible(Toggle): True
						UseAlpha(Float): 1
						Contrast(Float): 3
					End Shader Effect

					Begin Shader Effect
						TypeS(Text): "SSEMathClamp"
						IsVisible(Toggle): True
						UseAlpha(Float): 1
						Min(Float): 0
						Max(Float): 1
					End Shader Effect

				End Shader Layer

			End Shader Layer List

		End Masks

		Begin Shader Pass
			Name(Text): "Base"
			Visible(Toggle): True

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
				Smoothness(Float): 0.3
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
						Layer Name(Text): "Texture"
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
						Mix Amount(Float): 1
						Mix Type(Type): Mix - {TypeID = 0}
						Stencil(ObjectArray): SSNone - {ObjectID = -1}
						Cubemap(Cubemap): 
						Noise Dimensions(Type): SSN/A - {TypeID = 0}
						Color(Vec): 0.8,0.8,0.8,1
						Texture(Texture): df74a0cbfe3033e48af5c068e2386265 - {Input = 0}
						Color 2(Vec): 0,0,0,1
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
						Layer Name(Text): "Alpha"
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
						Mix Amount(Float): 1
						Mix Type(Type): Mix - {TypeID = 0}
						Stencil(ObjectArray): SSNone - {ObjectID = -1}
						Cubemap(Cubemap): 
						Noise Dimensions(Type): SSN/A - {TypeID = 0}
						Color(Vec): 1,1,1,1
						Texture(Texture): 354f7eb7f261f094399e01c57c0c2a0d - {Input = 1}
						Color 2(Vec): 0,0,0,1
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

				Type(Text): "ShaderSurfaceEmission"
				User Name(Text): "Emission"
				Use Custom Lighting(Toggle): False
				Use Custom Ambient(Toggle): False
				Alpha Blend Mode(Type): Blend Inside - {TypeID = 1}
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
						Layer Name(Text): "Alpha Copy"
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
						Mix Amount(Float): 1
						Mix Type(Type): Mix - {TypeID = 0}
						Stencil(ObjectArray): Mask1 - {ObjectID = 1}
						Cubemap(Cubemap): 
						Noise Dimensions(Type): SSN/A - {TypeID = 0}
						Color(Vec): 1,1,1,1
						Texture(Texture): 354f7eb7f261f094399e01c57c0c2a0d - {Input = 1}
						Color 2(Vec): 0,0,0,1
						Jitter(Float): 0
						Fill(Float): 0
						MinSize(Float): 0
						Edge(Float): 1
						MaxSize(Float): 1
						Square(Toggle): False
						Begin Shader Effect
							TypeS(Text): "SSEDesaturate"
							IsVisible(Toggle): True
							UseAlpha(Float): 0
						End Shader Effect

					End Shader Layer

					Begin Shader Layer
						Layer Name(Text): "Emission 2 Copy"
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
						Cubemap(Cubemap): 
						Noise Dimensions(Type): SSN/A - {TypeID = 0}
						Color(Vec): 1,0.6117647,0.02941179,1 - {Input = 2}
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
						Layer Name(Text): "Emission Copy"
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
						Cubemap(Cubemap): 
						Noise Dimensions(Type): SSN/A - {TypeID = 0}
						Color(Vec): 0,0,0,1
						Color 2(Vec): 0,0,0,1
						Texture(Texture): 
						Jitter(Float): 0
						Fill(Float): 0
						MinSize(Float): 0
						Edge(Float): 1
						MaxSize(Float): 1
						Square(Toggle): False
						Begin Shader Effect
							TypeS(Text): "SSEMathMul"
							IsVisible(Toggle): True
							UseAlpha(Float): 0
							Multiply(Float): 10.11635 - {Input = 4}
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
				CullMode(Type): None - {TypeID = 2}
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
				Transparency Type(Type): Cutout - {TypeID = 0}
				Transparency ZWrite(Type): Off - {TypeID = 0}
				Shadow Type Fade(Type): Cutout - {TypeID = 1}
				Cutout Amount(Float): 0.3254838 - {Input = 3}
				Transparency(Float): 0.3254838 - {Input = 3}
				Blend Mode(Type): Mix - {TypeID = 0}

			End Shader Ingredient

		End Shader Pass

	End Shader Base
End Shader Sandwich Shader
*/
