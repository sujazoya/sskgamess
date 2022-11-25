//#define UnityGGXCorrection
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using SU = ElectronicMind.Sandwich.ShaderUtil;
using EMindGUI = ElectronicMind.ElectronicMindStudiosInterfaceUtils;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using System.Text;
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
public class ShaderSurfaceSpecular : ShaderSurface{
	public ShaderVar SpecularType = new ShaderVar("Specular Type",new string[] {"The Greatest Sandwich\n...shader model","Unity Standard","Standard", "Circular","Wave"},new string[]{"ImagePreviews/SpecularSandwich.png","ImagePreviews/SpecularStandard.png","ImagePreviews/SpecularBlinn.png","ImagePreviews/SpecularCircular.png","ImagePreviews/SpecularWave.png"},"",new string[] {"Shader Sandwich's own bunch 'o good lookin' hacks :)","A physically based version of the Standard option - based on Disney's BRDF.","Standard specular highlights.", "Circular specular highlights (Unrealistic)","A strange wave like highlight."});	

	public ShaderVar RoughnessOrSmoothness = new ShaderVar("Roughness Or Smoothness",new string[] {"Smoothness","Roughness"});
	public ShaderVar Smoothness = new ShaderVar("Smoothness",0.3f);
	public ShaderVar Roughness = new ShaderVar("Roughness",0.7f);//Use to be called Spec Roughness
	public ShaderVar LightSize = new ShaderVar("Light Size",0f);
	public ShaderVar ConserveEnergy = new ShaderVar("Spec Energy Conserve",true);
	public ShaderVar Offset = new ShaderVar("Spec Offset",0f);
	
	public ShaderVar PBRQuality = new ShaderVar("PBR Quality",new string[] {"Auto", "High", "Medium", "Low"},new string[] {"","","",""},new string[] {"UNITY_BRDF_PBSSS","BRDF1_Unity_PBSSS", "BRDF2_Unity_PBSSS", "BRDF3_Unity_PBSSS"});
	public ShaderVar PBRModel = new ShaderVar("PBR Model",new string[] {"Specular", "Metal"},new string[] {"",""},new string[] {"Specular","Metal"});
	//public ShaderVar PBRSpecularType = new ShaderVar("PBR Specular Type",new string[] {"GGX", "BlinnPhong"},new string[] {"",""},new string[] {"GGX","BlinnPhong"});
	public ShaderVar UseTangents = new ShaderVar("Use Tangents",false);
	public ShaderVar UseAmbient = new ShaderVar("Use Ambient",true);
	
	public ShaderVar UseRoughnessDarkening = new ShaderVar("Use Roughness Darkening",true);
	public ShaderVar UseFresnel = new ShaderVar("Use Fresnel",true);
	
	
	public ShaderFix MobileSpecularType = null;
	public ShaderFix DeferredTypeNotSupported = null;
	public ShaderFix DeferredTypeSemiSupported = null;
	public override void GetMyPersonalPrivateForMySubStandardChildEyesOnlyFixes(ShaderBase shader, ShaderPass pass, List<ShaderFix> fixes){
		if (MobileSpecularType==null)
			MobileSpecularType = new ShaderFix(this,"Use Shader Sandwich's specular model with Micro-occlusion disabled to lower the instruction count by 4-ish.",ShaderFix.FixType.Optimization,
			()=>{
				SpecularType.Type = 0;
				UseRoughnessDarkening.On = false;
			});
		if (DeferredTypeNotSupported==null)
			DeferredTypeNotSupported = new ShaderFix(this,"Switch to Unity Standard Specular; Specular types other than Unity Standard aren't supported in deferred mode (The Greatest Sandwich model is sort of...)",ShaderFix.FixType.Error,
			()=>{
				SpecularType.Type = 1;
			});
		if (DeferredTypeSemiSupported==null)
			DeferredTypeSemiSupported = new ShaderFix(this,"Switch to Unity Standard Specular; The Greatest Sandwich shadel model is only sort of supported in deferred mode - it'll do its fun ambient lighting stuff, but nothing else.",ShaderFix.FixType.Error,
			()=>{
				SpecularType.Type = 1;
			});
		
		if (shader.HardwareTarget.Type==2&&shader.RenderingPathTarget.Type==0){//Mobile Forward
			if (SpecularType.Type!=0)
				fixes.Add(MobileSpecularType);
		}
		if (shader.RenderingPathTarget.Type==1){//Deferred
			if (SpecularType.Type>1)
				fixes.Add(DeferredTypeNotSupported);
			if (SpecularType.Type==0)
				fixes.Add(DeferredTypeSemiSupported);
		}
	}
	
	public override void GetMyShaderVars(List<ShaderVar> SVs){
		SVs.Add(SpecularType);
		SVs.Add(RoughnessOrSmoothness);
		SVs.Add(Smoothness);
		SVs.Add(Roughness);
		SVs.Add(LightSize);
		SVs.Add(ConserveEnergy);
		SVs.Add(Offset);
		SVs.Add(PBRQuality);
		SVs.Add(PBRModel);
		SVs.Add(UseTangents);
		SVs.Add(UseAmbient);
		SVs.Add(UseRoughnessDarkening);
		SVs.Add(UseFresnel);
	}
	new static public string MenuItem = "Lighting/Specular";
	public void OnEnable(){
		Name = "Specular";
		MenuItem = "Lighting/Specular";
		//SurfaceLayers.Description = "Where the shine appears and what color it is.";
		//SurfaceLayers.Name.Text = "Specular";
		SurfaceLayers.Description = "How metallic the surface is.";
		SurfaceLayers.NameUnique.Text = "Specular";
		SurfaceLayers.Name.Text = "Metalness";
		SurfaceLayers.CodeName = "Metalness";
		SurfaceLayers.BaseColor = new Color(0f,0f,0f,1);
		
		CustomLightingLayers.NameUnique.Text = "Specular Direct";
		CustomLightingLayers.Name.Text = "Specular Direct";
		CustomLightingLayers.InputName = "Specular";
		CustomLightingLayers.CodeName = "Lighting";
		CustomLightingLayers.EndTag.Text = "rgb";
		
		CustomAmbientLayers.NameUnique.Text = "Specular Indirect";
		CustomAmbientLayers.Name.Text = "Specular Indirect";
		CustomAmbientLayers.InputName = "Specular Indirect";
		CustomAmbientLayers.CodeName = "gi.indirect.specular";
		CustomAmbientLayers.EndTag.Text = "rgb";
		//Roughness.Range0 = 0.002f;
		Roughness.Range0 = 0.014f;
		Smoothness.Range1 = 0.986f;
		UpdateModelType();
		ForceAlphaGenerate = true;
		UseAlphaGenerate = true;
		//Debug.Log(UseAlpha.On);
	}
	public new void Awake(){
		BaseAwake();
		AlphaBlendMode.Type = 0;
		UseAlpha.On = true;
		//Debug.Log(UseAlpha.On);
		PBRModel.Type=1;
		SpecularType.Type=1;
		
		
		CustomAmbientEnabled = true;
		CustomLightingEnabled = true;
		ShaderLayer SL = ScriptableObject.CreateInstance<ShaderLayer>();
		SL.SetType("SLTData");
		SL.CustomData["Data"].Type = 10;
		CustomLightingLayers.SLs.Add(SL);
		SL.Parent = CustomLightingLayers;
		SL.Name.Text = "Light Color";
		SL.MixType.Type = 3;
		
		SurfaceLayers.EndTag.Text = "a";
	}
	public override int GetHeight(float Width){
		return 421;//124+18+24+25+25+20+25+25+25+25+25+25+25+25;
	}
	public override void RenderUI(float Width){
		SpecularType.Draw(new Rect(0,0,Width,124));
		
		GUI.skin.label.alignment = TextAnchor.UpperCenter;
		EMindGUI.Label(new Rect(0,124+18+(SpecularType.Type==0?10:0),Width,70),SpecularType.Descriptions[SpecularType.Type],12);
		GUI.skin.label.alignment = TextAnchor.UpperLeft;
			int Y = 124+18+24+15;//+25;
			PBRModel.Draw(new Rect(0+16,Y,Width-16,20),"Model: ");Y+=25;
			
			if (SpecularType.Type==0||SpecularType.Type==1){
			GUI.Label(new Rect(0,Y,Width-124,20),"Quality ");Y+=20;
			PBRQuality.Draw(new Rect(0+16,Y,Width-16,20),"Quality");Y+=25;
			}
			
			
			//Roughness.Draw(new Rect(0,Y,Width,20),"Roughness");Y+=25;
			if (RoughnessOrSmoothness.Type==1)//Lol inverted
				Roughness.Draw(new Rect(0,Y,Width,20),"Roughness");
			else
				Smoothness.Draw(new Rect(0,Y,Width,20),"Smoothness");
			GUI.color = new Color(1f,1f,1f,0f);
			Rect downRect = new Rect(70,Y+2,16,20);
			if (RoughnessOrSmoothness.Type==0)
				downRect.x+=6;
			RoughnessOrSmoothness.Type = EditorGUI.Popup(downRect,"",RoughnessOrSmoothness.Type,new string[]{"Smoothness (Unity)","Roughness   (Everyone else)"},GUI.skin.GetStyle("box"));
			
			downRect.y+=2;
			downRect.width=16;
			downRect.height=16;
			
			if (downRect.Contains(Event.current.mousePosition)){
				GUI.color = EMindGUI.BaseInterfaceColor;
				if (EMindGUI.mouse.MouseDrag){
					downRect.y+=1;
				}
			}
			else
				GUI.color = new Color(1f,1f,1f,1f);
			GUI.DrawTexture(downRect,ShaderSandwich.MiniDownArrow);
			GUI.color = new Color(1f,1f,1f,1f);
			Y+=25;
			
			LightSize.Draw(new Rect(0,Y,Width,20),"Light Radius");Y+=25;
			Offset.Range1 = 3.141592f;
			Offset.Draw(new Rect(0,Y,Width,20),"Offset");Y+=25;
			if (SpecularType.Type>1){
				ConserveEnergy.Draw(new Rect(0,Y,Width,20),"Conserve Energy");Y+=25;
			}
			
			//UseAmbient.LabelOffset = (int)Width/2;
			UseFresnel.LabelOffset = (int)Width/2;
			UseRoughnessDarkening.LabelOffset = (int)Width/2;
			if (SpecularType.Type==0)
				UseRoughnessDarkening.Draw(new Rect(Width/2+10,Y,Width,20),"Micro Occlusion");
			UseAmbient.Draw(new Rect(0,Y,Width,20),"Use Ambient");Y+=25;
			UseFresnel.Draw(new Rect(Width/2+10,Y,Width,20),"Use Fresnel");
			UseTangents.Draw(new Rect(0,Y,Width,20),"Use Tangents");Y+=25;
			
			
			//UnityFresnel.Draw(new Rect(0,124+18+24+20+25+25+25+25+25+25+25+25+25+25+25,Width,20),"Unity Fresnel");//Rofl
			UpdateModelType();
		
	}
	public void UpdateModelType(){
		if (PBRModel.Type==0){
			SurfaceLayers.Name.Text = "Specular Color";
			SurfaceLayers.CodeName = "Surface";
			SurfaceLayers.EndTag.Text = "rgb";
			SurfaceLayers.Description = "Where the shine appears and what color it is.";
		}
		else{
			if (SurfaceLayers.EndTag.Text.Length!=1)
				SurfaceLayers.EndTag.Text = "a";
			SurfaceLayers.Name.Text = "Metalness";
			SurfaceLayers.CodeName = "Metalness";
			SurfaceLayers.Description = "Whether the surface is a metal or not.\n\nOr in between but that's just kinda weird...";
		}
	}
	public override Dictionary<string,string> GeneratePrecalculations(){
		Dictionary<string,string> Precalcs = new Dictionary<string,string>();
		return Precalcs;
	}
	public string RoughnessGet(){
		if (RoughnessOrSmoothness.Type==0)
			return "(1-"+Smoothness.Get()+")";
		else
			return Roughness.Get();
		
	}
	public string SmoothnessGet(){
		if (RoughnessOrSmoothness.Type==0)
			return Smoothness.Get();
		else
			return "(1-"+Roughness.Get()+")";
		
	}
	/*public string GenerateCodeHeader(string normals, string OffsetStr){
		string LightingCode = "";
		if (SpecularType.Type == 1){
			LightingCode += "half3 halfDir = normalize (gi.light.dir + vd.worldViewDir"+OffsetStr+");\n"+
			"half3 maybeGGXNormals = "+normals+";\n"+
			"\n"+
			"#if UNITY_BRDF_GGX \n"+
			"	half shiftAmount = dot("+normals+", vd.worldViewDir);\n"+
			"	maybeGGXNormals = shiftAmount < 0.0f ? "+normals+" + vd.worldViewDir * (-shiftAmount + 1e-5f) : "+normals+";\n"+
			"#endif\n";
		}
		if (SpecularType.Type == 2){
			LightingCode += 
			"half3 halfDir = normalize (gi.light.dir + vd.worldViewDir"+OffsetStr+");	\n"+	
			"float nh = max (0, dot ("+normals+", halfDir));\n";
		}
		if (SpecularType.Type != 1&&FresnelType.Type==0)
			LightingCode += "half nvPow5 = Pow5 (1-(DotClamped (vd.worldNormal, vd.worldViewDir)));\n";
		return LightingCode;
	}*/
	public string GenerateCodeBody(ShaderData SD, string normals, string OffsetStr){
		string LightingCode = "";
		if (PBRModel.Type==1){
			if (SpecularType.Type==0)
				//LightingCode += "half3 Surface = lerp (roughnessReduction * 0.034, previousBaseColor, Metalness);\n";
				//LightingCode += "half3 Surface = lerp (0.034-perceptualRoughness*0.01836, previousBaseColor, Metalness);\n";
				LightingCode += "half3 Surface = previousBaseColor*reflectivity;\n";
			else
				LightingCode += "half3 Surface = lerp (unity_ColorSpaceDielectricSpec.rgb, previousBaseColor, Metalness);\n";
		}
		if (PBRQuality.Type==1){
			SD.AdditionalFunctions.Add(
"inline half GGXTermNoFadeout (half NdotH, half realRoughness){\n"+
"	half a2 = realRoughness * realRoughness;\n"+
"	half d = (NdotH * a2 - NdotH) * NdotH + 1.0f; // 2 mad\n"+
"	return a2 / (UNITY_PI * d * d);\n"+
"}\n");
			if (SpecularType.Type == 0){
				if (UseRoughnessDarkening.On)
					LightingCode += 
					//"half3 Surface = lerp (roughnessReduction * 0.034, previousBaseColor, Metalness);\n"+
					//"gi.indirect.specular *= 1 - (0.5 + (nv-nvPow5) * perceptualRoughness) * 0.2 * perceptualRoughness;\n"+
					
					"gi.indirect.specular *= 1 - perceptualRoughness*(nv*0.2+0.4);\n";
			}
				LightingCode += 
				"// surfaceReduction = Int D(NdotH) * NdotH * Id(NdotL>0) dH = 1/(realRoughness^2+1)\n"+
				//"half realRoughness = perceptualRoughness*perceptualRoughness;		// need to square perceptual roughness\n"+
				"half surfaceReduction;\n"+
				"if (IsGammaSpace()) surfaceReduction = 1.0 - 0.28*realRoughness*perceptualRoughness;		// 1-0.28*x^3 as approximation for (1/(x^4+1))^(1/2.2) on the domain [0;1]\n"+
				"else surfaceReduction = 1.0 / (realRoughness*realRoughness + 1.0);			// fade in [0.5;1]\n"+
				"half whyDoIBotherWithStandardSupport = realRoughness;\n"+
				"#if UNITY_VERSION<550\n"+
				"	whyDoIBotherWithStandardSupport = perceptualRoughness;\n"+
				"#endif\n"+
				"#if UNITY_BRDF_GGX\n"+
				"	half V = SmithJointGGXVisibilityTerm (nl, nv, whyDoIBotherWithStandardSupport);\n"+
				"	half D = GGXTermNoFadeout (nh, realRoughness);\n"+
				"#else\n"+
				"	half V = SmithBeckmannVisibilityTerm (nl, nv, whyDoIBotherWithStandardSupport);\n"+
				"	half D = NDFBlinnPhongNormalizedTerm (nh, RoughnessToSpecPower (perceptualRoughness));\n"+
				"#endif\n"+
				"\n"+
				"// HACK: theoretically we should divide by Pi diffuseTerm and not multiply specularTerm!\n"+
				"// BUT 1) that will make shader look significantly darker than Legacy ones\n"+
				"// and 2) on engine side Non-important lights have to be divided by Pi to in cases when they are injected into ambient SH\n"+
				"// NOTE: multiplication by Pi is part of single constant together with 1/4 now\n"+
				"#if UNITY_VERSION>=550//Unity changed the value of PI...:(\n"+
				"	half specularTerm = (V * D) * (UNITY_PI); // Torrance-Sparrow model, Fresnel is applied later (for optimization reasons)\n"+
				"#else\n"+
				"	half specularTerm = (V * D) * (UNITY_PI/4); // Torrance-Sparrow model, Fresnel is applied later (for optimization reasons)\n"+
				"#endif\n"+
				"if (IsGammaSpace())\n"+
				"	specularTerm = sqrt(max(1e-4h, specularTerm));\n"+
				"specularTerm = max(0, specularTerm * nl);\n"+
				"\n"+
				"\n"+
				//"half grazingTerm = saturate((1-perceptualRoughness) + (reflectivity));\n"+
				"Lighting =	specularTerm;\n";
		}
		if (PBRQuality.Type==2){///TODO: Double check brightness
			if (SpecularType.Type == 0){
				if (UseRoughnessDarkening.On)
					LightingCode += 
					//"half3 Surface = lerp (roughnessReduction * 0.034, previousBaseColor, Metalness);\n"+
					//"gi.indirect.specular *= 1 - (0.5 + (nv-nvPow5) * roughness) * 0.2 * roughness;\n"+
					
					"gi.indirect.specular *= 1 - perceptualRoughness*(nv*0.2+0.4);\n";
			}
				LightingCode += 
				//"half realRoughness = perceptualRoughness*perceptualRoughness;		// need to square perceptual roughness\n"+
				"#if UNITY_BRDF_GGX\n"+
				"\n"+
				"	// GGX Distribution multiplied by combined approximation of Visibility and Fresnel\n" + 
				"	// See \"Optimizing PBR for Mobile\" from Siggraph 2015 moving mobile graphics course\n" + 
				"	// https://community.arm.com/events/1155\n" + 
				"	half a = realRoughness;\n" + 
				"	half a2 = a*a;\n" + 
				"\n" + 
				"	half d = nh * nh * (a2 - 1.h) + 1.00001h;\n" + 
				"#ifdef UNITY_COLORSPACE_GAMMA\n" + 
				"	// Tighter approximation for Gamma only rendering mode!\n" + 
				"	// DVF = sqrt(DVF);\n" + 
				"	// DVF = (a * sqrt(.25)) / (max(sqrt(0.1), lh)*sqrt(realRoughness + .5) * d);\n" + 
				"	half specularTerm = a / (max(0.32h, lh) * (1.5h + realRoughness) * d);\n" + 
				"#else\n" + 
				"	half specularTerm = a2 / (max(0.1h, lh*lh) * (realRoughness + 0.5h) * (d * d) * 4);\n" + 
				"#endif\n" + 
				"\n" + 
				"	// on mobiles (where half actually means something) denominator have risk of overflow\n" + 
				"	// clamp below was added specifically to \"fix\" that, but dx compiler (we convert bytecode to metal/gles)\n" + 
				"	// sees that specularTerm have only non-negative terms, so it skips max(0,..) in clamp (leaving only min(100,...))\n" + 
				"#if defined (SHADER_API_MOBILE)\n" + 
				"	specularTerm = specularTerm - 1e-4h;\n" + 
				"#endif\n" + 
				"\n" + 
				"#else\n" + 
				"\n" + 
				"	// Legacy\n" + 
				"	half specularPower = RoughnessToSpecPower(perceptualRoughness);\n" + 
				"	// Modified with approximate Visibility function that takes roughness into account\n" + 
				"	// Original ((n+1)*N.H^n) / (8*Pi * L.H^3) didn't take into account roughness\n" + 
				"	// and produced extremely bright specular at grazing angles\n" + 
				"\n" + 
				"	half invV = lh * lh * smoothness + perceptualRoughness * perceptualRoughness; // approx ModifiedKelemenVisibilityTerm(lh, perceptualRoughness);\n" + 
				"	half invF = lh;\n" + 
				"\n" + 
				"	half specularTerm = ((specularPower + 1) * pow (nh, specularPower)) / (8 * invV * invF + 1e-4h);\n" + 
				"\n" + 
				"#ifdef UNITY_COLORSPACE_GAMMA\n" + 
				"	specularTerm = sqrt(max(1e-4h, specularTerm));\n" + 
				"#endif\n" + 
				"\n" + 
				"#endif\n" + 
				"\n" + 
				"#if defined (SHADER_API_MOBILE)\n" + 
				"	specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles\n" + 
				"#endif\n" + 
				"#if defined(_SPECULARHIGHLIGHTS_OFF)\n" + 
				"	specularTerm = 0.0;\n" + 
				"#endif\n" + 
				"\n" + 
				"	// surfaceReduction = Int D(NdotH) * NdotH * Id(NdotL>0) dH = 1/(realRoughness^2+1)\n" + 
				"\n" + 
				"	// 1-0.28*x^3 as approximation for (1/(x^4+1))^(1/2.2) on the domain [0;1]\n" + 
				"	// 1-x^3*(0.6-0.08*x)   approximation for 1/(x^4+1)\n" + 
				"#ifdef UNITY_COLORSPACE_GAMMA\n" + 
				"	half surfaceReduction = 0.28;\n" + 
				"#else\n" + 
				"	half surfaceReduction = (0.6-0.08*perceptualRoughness);\n" + 
				"#endif\n" + 
				"	surfaceReduction = 1.0 - realRoughness*perceptualRoughness*surfaceReduction;"+
				"\n"+
				"\n"+
				//"half grazingTerm = saturate((1-roughness) + (reflectivity));\n"+
				"Lighting =	specularTerm;\n";
		}
		if (PBRQuality.Type==3){
				LightingCode += 
				"half surfaceReduction = 1;\n"+
				"half2 rlPow4AndFresnelTerm = Pow4 (half2(dot(vd.worldRefl, gi.light.dir), 1-nv));\n"+
				"half rlPow4 = rlPow4AndFresnelTerm.x; // power exponent must match kHorizontalWarpExp in NHxRoughness() function in GeneratedTextures.cpp\n"+
				"half fresnelTerm = rlPow4AndFresnelTerm.y;\n"+
				"\n"+
				"\n"+
				//"half grazingTerm = saturate((1-roughness) + (reflectivity));\n"+
				"Lighting =	BRDF3_Direct(0,1,rlPow4,oneMinusRoughness)*nl;\n";
		}
		if (SpecularType.Type == 2){
			LightingCode += 
			"Lighting = pow (nh, oneMinusRoughness*512.0);\n";
		}		
		if (SpecularType.Type == 3){
			if (SD.ShaderPassIsBaseLighting)
			LightingCode += 
			"Lighting = (dot(normalize(reflect(-gi.light.dir, "+normals+")),vd.worldViewDir"+OffsetStr+"));\n"+	
			"Lighting = pow(max(0.0,Lighting),oneMinusRoughness*512.0);\n";
			else
			LightingCode += 
			"Lighting = (dot(reflect(-gi.light.dir, "+normals+"),vd.worldViewDir"+OffsetStr+"));\n"+	
			"Lighting = pow(max(0.0,Lighting),oneMinusRoughness*512.0);\n";
		}		
		if (SpecularType.Type == 4){
			LightingCode += 
			"Lighting = abs(dot("+normals+",normalize(reflect(-gi.light.dir, -vd.worldViewDir"+OffsetStr+"))));\n"+
			//"Lighting = (half3(1.0,1.0,1.0)-(pow(sqrt(Lighting),2 - (1-"+Roughness.Get()+"))));\n"+
			"Lighting = pow(half3(1.0,1.0,1.0)-sqrt(Lighting),oneMinusRoughness*10);\n"+
			"Lighting = max(0,Lighting);\n";
		}
		///TODO: Re-enable in Unity 4
		//if (SpecularType.Type>1)
		//	LightingCode += "Lighting = Lighting * 2;\n";
		if (ConserveEnergy.On==true&&SpecularType.Type>1)///TODO: Check if it actually makes any sense at all...
			LightingCode += "Lighting = Lighting * ((((oneMinusRoughness*512.0)+9.0)/("+(9.0f*3.14f).ToString()+"))/9.0);\n";
		return LightingCode;
	}
	public override string GenerateCode(ShaderData SD){
		UpdateModelType();
		StringBuilder SpecularCode;
		
		
			if (PBRQuality.Type==0){
				SD.AdditionalFunctions.Add(
				
"#if !defined (SSUNITY_BRDF_PBS) // allow to explicitly override BRDF in custom shader\n"+
"	// still add safe net for low shader models, otherwise we might end up with shaders failing to compile\n"+
"	#if SHADER_TARGET < 30\n"+
"		#define SSUNITY_BRDF_PBS 3\n"+
"	#elif defined(UNITY_PBS_USE_BRDF3)\n"+
"		#define SSUNITY_BRDF_PBS 3\n"+
"	#elif defined(UNITY_PBS_USE_BRDF2)\n"+
"		#define SSUNITY_BRDF_PBS 2\n"+
"	#elif defined(UNITY_PBS_USE_BRDF1)\n"+
"		#define SSUNITY_BRDF_PBS 1\n"+
"	#elif defined(SHADER_TARGET_SURFACE_ANALYSIS)\n"+
"		// we do preprocess pass during shader analysis and we dont actually care about brdf as we need only inputs/outputs\n"+
"		#define SSUNITY_BRDF_PBS 1\n"+
"	#else\n"+
"		#error something broke in auto-choosing BRDF (Shader Sandwich)\n"+
"	#endif\n"+
"#endif\n"
				
				);
			}
		
		if (PBRModel.Type==0)
			SpecularCode = new StringBuilder("half3 Surface = half4(0,0,0,1);//Specular Mode\n");
		else
			SpecularCode = new StringBuilder("half Metalness = 0;//Metallic Mode\n");
		
		SpecularCode.Append(SurfaceLayers.GenerateCode(SD,OutputPremultiplied));
		
		if (SD.ShaderPassIsDeferred||SD.ShaderPassOnlyZ){
			if (PBRModel.Type==1){
				if (SpecularType.Type==0){
					//SpecularCode.Append("float roughnessReduction = 1.0/("+Roughness.Get()+"+0.82);\n");
					//SpecularCode.Append("half3 Surface = lerp (roughnessReduction * 0.034, previousBaseColor, Metalness);\n");
					SpecularCode.Append("half3 Surface = lerp (0.034-"+RoughnessGet()+"*0.01836, previousBaseColor, Metalness);\n");
				}
				else
					SpecularCode.Append("half3 Surface = lerp (unity_ColorSpaceDielectricSpec.rgb, previousBaseColor, Metalness);\n");
				SpecularCode.Append("half oneMinusReflectivity = OneMinusReflectivityFromMetallic(Metalness);\n");
				SpecularCode.Append("half reflectivity = 1-oneMinusReflectivity;\n");
			}else{
				SpecularCode.Append("half reflectivity = SpecularStrength(Surface);\n");
				SpecularCode.Append("half oneMinusReflectivity = 1-reflectivity;\n");
			}
			if (SpecularType.Type==0){
				SpecularCode.Append("half nv = saturate(dot(vd.worldNormal, vd.worldViewDir));\n");
				SpecularCode.Append("half nvPow5 = Pow5 (1-nv);\n");
				if (SD.ShaderPassIsDeferred)
					SpecularCode.Append("OneMinusAlpha = saturate(1-nvPow5*"+SmoothnessGet()+")*(oneMinusReflectivity);\n");
				else
					SpecularCode.Append("return float4(Surface,lerp(1,nvPow5,oneMinusReflectivity));\n");
			}
			else{
				if (SD.ShaderPassIsDeferred)
					SpecularCode.Append("OneMinusAlpha = oneMinusReflectivity;\n");
				else
					SpecularCode.Append("return half4(Surface,reflectivity);\n");
			}
			if (SD.ShaderPassIsDeferred)
				SpecularCode.Append(
				"deferredOut.Alpha = 1-OneMinusAlpha;\n"+"deferredOut.SpecSmoothness = half4(Surface,"+SmoothnessGet()+");\nreturn deferredOut;\n");
			return SpecularCode.ToString();
		}
		
		string normals = "vd.worldNormal";
		if (UseTangents.On)
			normals = "vd.worldTangent";
		
		string OffsetStr = Offset.Get();///"";//TODO: HAHAHAHAHAHA
		if (OffsetStr!="0")
			OffsetStr = " + float3(sin("+Offset.Get()+"),cos("+Offset.Get()+"+1.57075),tan("+Offset.Get()+"))";
		else
			OffsetStr = "";
		
		StringBuilder LightingCode = new StringBuilder("half3 Lighting;\n");
		
		if (PBRQuality.Type==0){
			LightingCode.Append("#if SSUNITY_BRDF_PBS==1\n");
			PBRQuality.Type = 1;
				LightingCode.Append(GenerateCodeBody(SD,normals, OffsetStr));
			LightingCode.Append("#elif SSUNITY_BRDF_PBS==2\n");
			PBRQuality.Type = 2;
				LightingCode.Append(GenerateCodeBody(SD,normals, OffsetStr));
			LightingCode.Append("#else\n");
			PBRQuality.Type = 3;
				LightingCode.Append(GenerateCodeBody(SD,normals, OffsetStr));
			LightingCode.Append("#endif\n");
			PBRQuality.Type = 0;
		}
		else
			LightingCode.Append(GenerateCodeBody(SD,normals, OffsetStr));
		
		if (LightSize.Get()!="0"){
			//LightingCode.Append("half awithalittlething = (realRoughness + (("+LightSize.Get()+")/(2*length(L))));\n");
			//LightingCode.Append("Lighting *= realRoughness/awithalittlething * realRoughness/awithalittlething;\n");
			
LightingCode.Append("half awithalittlething = (realRoughness + (("+LightSize.Get()+")/(2*length(L))));\n"+
"awithalittlething = realRoughness/awithalittlething;\n"+
"Lighting *= max(1e-4f,awithalittlething);\n");
		}
		
		if (UseCustomLighting.On){
			LightingCode.Append(CustomLightingLayers.GenerateCode(SD,false));
		}else{
			LightingCode.Append("Lighting *= gi.light.color;\n");
		}
		if (UseFresnel.On){
				if (UseAmbient.On&&SD.ShaderPassIsBaseLighting){
					if (SpecularType.Type>0)
						LightingCode.Append("half grazingTerm = saturate((oneMinusRoughness) + (reflectivity));\n");
					if (ConserveEnergy.On||SpecularType.Type<=1){
						if (SpecularType.Type>=1){
							if (PBRQuality.Type==0)LightingCode.Append("#if SSUNITY_BRDF_PBS==1\n");
							if (PBRQuality.Type==1||PBRQuality.Type==0)LightingCode.Append("Lighting =	Lighting * FresnelTerm (Surface, lh) + surfaceReduction * gi.indirect.specular * FresnelLerp (Surface.rgb, grazingTerm, nv);\n");
							if (PBRQuality.Type==0)LightingCode.Append("#elif SSUNITY_BRDF_PBS==2\n");
							if (PBRQuality.Type==2||PBRQuality.Type==0)LightingCode.Append("Lighting =	(Lighting * nl + surfaceReduction * gi.indirect.specular) * FresnelLerpFast (Surface.rgb, grazingTerm, nv);\n");
							if (PBRQuality.Type==0)LightingCode.Append("#else\n");
							if (PBRQuality.Type==3||PBRQuality.Type==0)LightingCode.Append("Lighting = (Lighting+gi.indirect.specular) * lerp (Surface.rgb, grazingTerm, fresnelTerm);\n");
							if (PBRQuality.Type==0)LightingCode.Append("#endif\n");
						}
						else{
							//LightingCode.Append("Lighting =	(Lighting * FresnelTerm (Surface, nv) + gi.indirect.specular * FresnelTerm (Surface.rgb, nv));\n");
							if (PBRQuality.Type==0)LightingCode.Append("#if SSUNITY_BRDF_PBS==1\n");
							if (PBRQuality.Type==1||PBRQuality.Type==0)LightingCode.Append("Lighting =	(Lighting + gi.indirect.specular) * lerp(Surface,1,nvPow5);\n");
							//if (PBRQuality.Type==1||PBRQuality.Type==0)LightingCode.Append("Lighting =	Lighting * FresnelTerm (Surface, nv) + gi.indirect.specular * FresnelTerm (Surface.rgb, nv);\n");
							if (PBRQuality.Type==0)LightingCode.Append("#elif SSUNITY_BRDF_PBS==2\n");
							if (PBRQuality.Type==2||PBRQuality.Type==0)LightingCode.Append("Lighting =	(Lighting * nl + gi.indirect.specular) * lerp(Surface,1,nvPow5);\n");
							//if (PBRQuality.Type==2||PBRQuality.Type==0)LightingCode.Append("Lighting =	Lighting * (Surface + (1-Surface) * nvPow5) + gi.indirect.specular * FresnelLerp (Surface.rgb, 1, nv);\n");
							if (PBRQuality.Type==0)LightingCode.Append("#else\n");
							if (PBRQuality.Type==3||PBRQuality.Type==0)LightingCode.Append("Lighting = (Lighting+gi.indirect.specular) * lerp (Surface.rgb, 1, fresnelTerm);\n");
							if (PBRQuality.Type==0)LightingCode.Append("#endif\n");
						}
					}
					else
						LightingCode.Append("Lighting = (Lighting * FresnelTerm (Surface, lh) + gi.indirect.specular * FresnelLerp (Surface.rgb, grazingTerm, nv));\n");
				}
				else{
					if (ConserveEnergy.On||SpecularType.Type<=1){
						if (SpecularType.Type>=1){
							if (PBRQuality.Type==0)LightingCode.Append("#if SSUNITY_BRDF_PBS==1\n");
							if (PBRQuality.Type==1||PBRQuality.Type==0)LightingCode.Append("Lighting =	Lighting * FresnelTerm (Surface, lh);\n");
							if (PBRQuality.Type==0)LightingCode.Append("#elif SSUNITY_BRDF_PBS==2\n");
							if (PBRQuality.Type==2||PBRQuality.Type==0)LightingCode.Append("Lighting =	Lighting * nl * Surface;\n");
							if (PBRQuality.Type==0)LightingCode.Append("#elif SSUNITY_BRDF_PBS==3\n");
							if (PBRQuality.Type==3||PBRQuality.Type==0)LightingCode.Append("Lighting =	Lighting * Surface;\n");
							if (PBRQuality.Type==0)LightingCode.Append("#endif\n");
						}
						else{
							if (PBRQuality.Type==0)LightingCode.Append("#if SSUNITY_BRDF_PBS==1\n");
							if (PBRQuality.Type==1||PBRQuality.Type==0)LightingCode.Append("Lighting =	(Lighting) * lerp(1,Surface,(1-nvPow5));\n");
							if (PBRQuality.Type==0)LightingCode.Append("#elif SSUNITY_BRDF_PBS==2\n");
							if (PBRQuality.Type==2||PBRQuality.Type==0)LightingCode.Append("Lighting =	(Lighting * nl) * lerp(1,Surface,(1-nvPow5));\n");
							if (PBRQuality.Type==0)LightingCode.Append("#elif SSUNITY_BRDF_PBS==3\n");
							if (PBRQuality.Type==3||PBRQuality.Type==0)LightingCode.Append("Lighting =	Lighting * Surface;\n");
							if (PBRQuality.Type==0)LightingCode.Append("#endif\n");
						}
					}
					else
						LightingCode.Append("Lighting = Lighting * FresnelTerm (Surface, lh);\n");
				}
					//LightingCode.Append("Lighting =	Lighting * FresnelTerm (Surface, lh);\n");
			//}
		}else{
			if (UseAmbient.On&&SD.ShaderPassIsBaseLighting){
				if (ConserveEnergy.On||SpecularType.Type<=1)
					LightingCode.Append("Lighting =	Lighting + surfaceReduction * gi.indirect.specular;\n");
				else
					LightingCode.Append("Lighting =	Lighting + gi.indirect.specular;\n");
			}
		}
		
		if (UseFresnel.On){
			if (SpecularType.Type>0)
				LightingCode.Append("OneMinusAlpha = oneMinusReflectivity;\n"+"return half4(Lighting,reflectivity);\n");
			else{
				if (PBRModel.Type==1)
					LightingCode.Append("return half4(Lighting.rgb,lerp(nvPow5,1,reflectivity));\n");
					//LightingCode.Append("return half4(Lighting.rgb,lerp(1,nvPow5,oneMinusReflectivity));\n");
				else
					LightingCode.Append("return half4(Lighting.rgb,lerp(1,nvPow5,oneMinusReflectivity));\n");
					//LightingCode.Append("return half4(Lighting.rgb,lerp(nvPow5,1,reflectivity));\n");
			}
				//LightingCode.Append("return lerp(half4(Lighting,1),half4(Lighting,nvPow5),oneMinusReflectivity);");
		}
		else{
			if (SpecularType.Type>0)
				LightingCode.Append("OneMinusAlpha = oneMinusReflectivity;\n"+"return half4(Lighting,reflectivity);\n");
			else
				LightingCode.Append("OneMinusAlpha = oneMinusReflectivity;\n"+"return half4(Lighting*Surface.rgb,reflectivity);\n");
		}
		string LightingCodeStr = LightingCode.ToString();
		bool needRoughness = LightingCodeStr.Contains("perceptualRoughness");
		bool needOneMinusRoughness = LightingCodeStr.Contains("oneMinusRoughness");
		bool needRealRoughness = LightingCodeStr.Contains("realRoughness");
		bool needReflectivity = LightingCodeStr.Contains("reflectivity");
		bool needOneMinusReflectivity = LightingCodeStr.Contains("oneMinusReflectivity");
		///bool needSurfaceReduction = LightingCodeStr.Contains("surfaceReduction");
		//bool needRoughnessReduction = LightingCodeStr.Contains("roughnessReduction");
		bool needHalfDir = LightingCodeStr.Contains("halfDir");
		bool needAmbient = UseAmbient.On&&SD.ShaderPassIsBaseLighting;
		bool needNH = LightingCodeStr.Contains("nh");
		bool needNL = LightingCodeStr.Contains("nl");
		bool needNV = LightingCodeStr.Contains("nv");
		bool needLH = LightingCodeStr.Contains("lh");
		bool needNLPow5 = LightingCodeStr.Contains("nlPow5");
		bool needNVPow5 = LightingCodeStr.Contains("nvPow5");
		
		if (RoughnessOrSmoothness.Type==1)
			needRoughness |= (needOneMinusRoughness);
		else
			needOneMinusRoughness |= (needRoughness);
		
		needOneMinusReflectivity |= (PBRModel.Type==1&&needReflectivity);
		needReflectivity |= (PBRModel.Type==0&&needOneMinusReflectivity);
		needRoughness |= (needReflectivity&&PBRModel.Type==1);
		needRoughness |= needRealRoughness;
		needNL |= (needNLPow5);
		needNV |= (needNVPow5);
		needHalfDir |= (needNH);
		needHalfDir |= (needLH);
		
		if (RoughnessOrSmoothness.Type==1){
			if (needRoughness)SpecularCode.Append("half perceptualRoughness = "+Roughness.Get()+";\n");
			if (needOneMinusRoughness)SpecularCode.Append("half oneMinusRoughness = 1-perceptualRoughness;\n");
		}else if (RoughnessOrSmoothness.Type==0){
			if (needOneMinusRoughness)SpecularCode.Append("half oneMinusRoughness = "+Smoothness.Get()+";\n");
			if (needRoughness)SpecularCode.Append("half perceptualRoughness = 1-oneMinusRoughness;\n");
			
		}
		if (needRealRoughness)
			SpecularCode.Append("half realRoughness = perceptualRoughness*perceptualRoughness;		// need to square perceptual roughness\n");
		
		if (needReflectivity&&PBRModel.Type==0)SpecularCode.Append("half reflectivity = SpecularStrength(Surface);\n");
		if (needOneMinusReflectivity&&PBRModel.Type==0)SpecularCode.Append("half oneMinusReflectivity = 1-reflectivity;\n");
		if (SpecularType.Type==0){
			if (needReflectivity&&PBRModel.Type==1){
				if (PBRQuality.Type==0)SpecularCode.Append("#if SSUNITY_BRDF_PBS==1||SSUNITY_BRDF_PBS==2\n");
				if (PBRQuality.Type==0||PBRQuality.Type==1||PBRQuality.Type==2)SpecularCode.Append("half reflectivity = lerp (0.034-perceptualRoughness*0.01836, 1, Metalness);\n");
				if (PBRQuality.Type==0)SpecularCode.Append("#else\n");
				if (PBRQuality.Type==0||PBRQuality.Type==3)SpecularCode.Append("half reflectivity = lerp (0.02482, 1, Metalness);\n");
				if (PBRQuality.Type==0)SpecularCode.Append("#endif\n");
			}
			if (needOneMinusReflectivity&&PBRModel.Type==1)SpecularCode.Append("half oneMinusReflectivity = 1-reflectivity;\n");
		}else{
			if (needOneMinusReflectivity&&PBRModel.Type==1)SpecularCode.Append("half oneMinusReflectivity = OneMinusReflectivityFromMetallic(Metalness);\n");
			if (needReflectivity&&PBRModel.Type==1)SpecularCode.Append("half reflectivity = 1-oneMinusReflectivity;\n");
		}
		/*if (needSurfaceReduction)SpecularCode.Append("// surfaceReduction = Int D(NdotH) * NdotH * Id(NdotL>0) dH = 1/(realRoughness^2+1)\n"+
			"half realRoughness = roughness*roughness;		// need to square perceptual roughness\n"+
			"half surfaceReduction;\n"+
			"if (IsGammaSpace()) surfaceReduction = 1.0 - 0.28*realRoughness*roughness;		// 1-0.28*x^3 as approximation for (1/(x^4+1))^(1/2.2) on the domain [0;1]\n"+
			"else surfaceReduction = 1.0 / (realRoughness*realRoughness + 1.0);			// fade in [0.5;1]\n");*/
		//if (needRoughnessReduction)SpecularCode.Append("float roughnessReduction = 1.0/(roughness+0.82);\n");
		if (needAmbient){
			SpecularCode.Append(
				"Unity_GlossyEnvironmentData g;\n"+
				"g.roughness = "+RoughnessGet()+";\n"+
				"g.reflUVW = reflect(-vd.worldViewDir, "+normals+");\n"+
				"gi = UnityGlobalIllumination(giInput, 1, "+normals+",g);\n");
				
			if (UseCustomAmbient.On)
				SpecularCode.Append(CustomAmbientLayers.GenerateCode(SD,false));
		}
		else
			SpecularCode.Append(
				"UnityGI o_gi;\n"+
				"ResetUnityGI(o_gi);\n"+
				"o_gi.light = giInput.light;\n"+
				"o_gi.light.color *= giInput.atten;\n"+
				"gi = o_gi;\n");
		if (LightSize.Get()!="0"){
			SpecularCode.Append("//Spherical lights implementation from http://blog.selfshadow.com/publications/s2013-shading-course/karis/s2013_pbs_epic_notes_v2.pdf - thanks guys! :D\n"+
			"float3 L = UnityWorldSpaceLightDir(vd.worldPos);\n"+
			"float3 centerToRay = dot(L,vd.worldRefl)*vd.worldRefl-L;\n"+
			"float3 closestPoint = L + centerToRay * saturate("+LightSize.Get()+"/length(centerToRay));\n"+
			"gi.light.dir = normalize(closestPoint);\n");
		}
		if (needHalfDir)SpecularCode.Append("half3 halfDir = normalize (gi.light.dir + vd.worldViewDir"+OffsetStr+");\n");
		
		if (SpecularType.Type==0||SpecularType.Type==1)
			#if UnityGGXCorrection
			SpecularCode.Append( 
				"#if UNITY_BRDF_GGX \n"+
				"	half shiftAmount = dot("+normals+", vd.worldViewDir);\n"+
				"	"+normals+" = shiftAmount < 0.0f ? "+normals+" + vd.worldViewDir * (-shiftAmount + 1e-5f) : "+normals+";\n"+
				"#endif\n");
			#else
			SpecularCode.Append( 
				//"//Found the difference between this and Abs-ing the nv too small to be worth the branch and whatnot. You can change this in the file ShaderSurfaceSpecular.cs and ShaderSurfaceDiffuse - set UnityGGXCorrection to 1\n"+
				"//#if UNITY_BRDF_GGX \n"+
				"//	half shiftAmount = dot("+normals+", vd.worldViewDir);\n"+
				"//	"+normals+" = shiftAmount < 0.0f ? "+normals+" + vd.worldViewDir * (-shiftAmount + 1e-5f) : "+normals+";\n"+
				"//#endif\n");
			#endif
					
		if (needNH)SpecularCode.Append("half nh = saturate(dot(vd.worldNormal, halfDir));\n");
		if (needNL)SpecularCode.Append("half nl = saturate(dot(vd.worldNormal, gi.light.dir));\n");
		#if UnityGGXCorrection
			if (needNV)SpecularCode.Append("half nv = saturate(dot(vd.worldNormal, vd.worldViewDir));\n");
		#else
			if (needNV)SpecularCode.Append("half nv = abs(dot(vd.worldNormal, vd.worldViewDir));\n");
		#endif
		if (needLH)SpecularCode.Append("half lh = saturate(dot(gi.light.dir, halfDir));\n\n");
		if (needNLPow5)SpecularCode.Append("half nlPow5 = Pow5 (1-nl);\n");
		//if (needNVPow5)SpecularCode.Append("half nvPow5 = Pow5 (1-nv);\n\n");//Ah the good ol' days...
		if (needNVPow5){
			if (PBRQuality.Type==0)SpecularCode.Append("#if SSUNITY_BRDF_PBS==1\n");
			if (PBRQuality.Type==1||PBRQuality.Type==0)SpecularCode.Append("half nvPow5 = Pow5 (1-nv);\n");
			if (PBRQuality.Type==0)SpecularCode.Append("#elif SSUNITY_BRDF_PBS==2||SSUNITY_BRDF_PBS==3\n");
			if (PBRQuality.Type==2||PBRQuality.Type==3||PBRQuality.Type==0)SpecularCode.Append("half nvPow5 = Pow4 (1-nv);\n");
			if (PBRQuality.Type==0)SpecularCode.Append("#endif\n");
			SpecularCode.Append("\n");
		}
		/*if (PBRModel.Type==1){
			if (SpecularType.Type==0)
				SpecularCode += "half3 Surface = lerp (roughnessReduction * 0.034, previousBaseColor, Metalness);\n";
			else
				SpecularCode += "half3 Surface = lerp (unity_ColorSpaceDielectricSpec.rgb, previousBaseColor, Metalness);\n";
		}*/
		SpecularCode.Append(LightingCodeStr);
		return SpecularCode.ToString();
	}
	public override bool IsPremultiplied(ShaderData SD){
		return true;//Sadly...
	}
}
}