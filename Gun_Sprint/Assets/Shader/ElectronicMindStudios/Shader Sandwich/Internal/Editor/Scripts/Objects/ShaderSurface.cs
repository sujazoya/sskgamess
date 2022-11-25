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
//Base class for all surface components (diffuse, specular), and other technically deeper ones (sss, ambient occlusion) lol...ah well XD
public enum ShaderSurfaceComponent{Color,Depth,Normal,Mask};
[System.Serializable]
public class ShaderSurface : ShaderIngredient{
	public ShaderSurface(){
		Name = "GenericSurface";
		MenuItem = "Generic/GenericSurface";
	}
	//public string CodeName{
	//	get{return ShaderUtil.CodeName(Name);}
	//	set{}
	//}
	//public bool Enabled = true;
	public ShaderSurfaceComponent ShaderSurfaceComponent = ShaderSurfaceComponent.Color;
	//public ShaderLayerList	SurfaceLayers = new ShaderLayerList("Surface","Surface","A generic surface component (Hopefully this is updated...)","Surface","Surface","rgba","",new Color(0.8f,0.8f,0.8f,1f));
	
	public ShaderVar AlphaBlendMode = new ShaderVar("Alpha Blend Mode",new string[]{"Blend","Blend Inside","Erase","Replace"},new string[]{"","","",""},4);
	//public ShaderVar MixAmount = new ShaderVar("Mix Amount",1f);
	public ShaderVar MixType = new ShaderVar("Mix Type", new string[]{"Mix","Add","Subtract","Multiply","Divide","Lighten","Darken"},new string[]{"","","","","","",""},3);
	public ShaderVar UseAlpha = new ShaderVar("Use Alpha", true);
	public ShaderLayerList	CustomLightingLayers;
	public ShaderLayerList	CustomAmbientLayers;
	public ShaderVar UseCustomLighting = new ShaderVar("Use Custom Lighting", false);
	public ShaderVar UseCustomAmbient = new ShaderVar("Use Custom Ambient", false);
	public bool CustomLightingEnabled = false;
	public bool CustomAmbientEnabled = false;
	public bool ForceAlphaGenerate = false;
	public bool UseAlphaGenerate = false;
	
	public bool OutputPremultiplied = false;
	public bool OutputsOneMinusAlpha = false;
	
	public ShaderVar CompareDepth = new ShaderVar("Compare Depth", true);
	public ShaderVar NormalizeMix = new ShaderVar("Normalize", true);
	
	public override void BaseAwake(){
		SurfaceLayers = ScriptableObject.CreateInstance<ShaderLayerList>();
		SurfaceLayers.UShaderLayerList("Surface","Surface","A generic surface component (Hopefully this is updated...)","Surface","Surface","rgba","",new Color(0.8f,0.8f,0.8f,1f));
		
		CustomLightingLayers = ScriptableObject.CreateInstance<ShaderLayerList>();
		CustomLightingLayers.UShaderLayerList("Custom Lighting","Custom Lighting","Customize lighting here!","Lighting","Lighting","rgba","",new Color(0.8f,0.8f,0.8f,1f));
		
		CustomAmbientLayers = ScriptableObject.CreateInstance<ShaderLayerList>();
		CustomAmbientLayers.UShaderLayerList("Custom Ambient","Custom Ambient","Customize ambient lighting here!","Ambient","Ambient","rgba","",new Color(0.8f,0.8f,0.8f,1f));
	}
	public override bool IsUsed(){
		return Enabled.On&&(MixAmount.Float!=0f);
	}
	/*public virtual int GetHeight(float Width){
		return 0;
	}
	public virtual void RenderUI(float Width){
		
	}
	public virtual Dictionary<string,string> GeneratePrecalculations(){
		return new Dictionary<string,string>();
	}
	//float4/float3/float (VertexData vd, Precalculations pc, UnityGI gi, Masks mask)
	public virtual string GenerateCode(ShaderData SD){
		return "";
	}*/
	public override void ConstantShaderVars(List<ShaderVar> SVs){
		SVs.Add(UserName);
		SVs.Add(UseCustomLighting);
		SVs.Add(UseCustomAmbient);
		SVs.Add(AlphaBlendMode);
		SVs.Add(MixAmount);
		SVs.Add(MixType);
		SVs.Add(UseAlpha);
		SVs.Add(ShouldLink);
		if (ShaderSurfaceComponent==ShaderSurfaceComponent.Depth)
			SVs.Add(CompareDepth);
	}
	/*public virtual void GetMyShaderVars(List<ShaderVar> SVs){
	}
	public virtual List<ShaderLayerList> GetMyShaderLayerLists(){
		List<ShaderLayerList> SLLs = new List<ShaderLayerList>();
		SLLs.Add(SurfaceLayers);
		return SLLs;
	}*/
	public override string GenerateHeader(ShaderData SD, string surfaceInternals, bool Call,string addon){
		bool UsesVertexData = false;
		bool UsesVertexDataUnchanged = false;
		bool UsesVertexToPixel = false;
		bool UsesPrecalculations = false;
		bool UsesUnityGI = false;
		bool UsesUnityGIInput = false;
		bool UsesMasks = false;
		bool UsesOutputColor = false;
		bool UsesDepth = false;
		bool UsesPreviousBaseColor = false;
		bool UsesDeferredOutput = false;
		bool UsesOneMinusAlpha = false;
		//bool UsesUVOffset = false;
		OutputsOneMinusAlpha = false;
		StringBuilder FunctionHeader = new StringBuilder();
		if (!Call){
			if (SD.ShaderPassIsDeferred&&ShaderSurfaceComponent==ShaderSurfaceComponent.Color){
				FunctionHeader.Append("DeferredOutput ");
			}else{
				if (ShaderSurfaceComponent==ShaderSurfaceComponent.Color)
					FunctionHeader.Append("half4 ");
				else if (ShaderSurfaceComponent==ShaderSurfaceComponent.Normal)
					FunctionHeader.Append("half3 ");
				else//Depth
					FunctionHeader.Append("half3 ");
			}
		}
			
		FunctionHeader.Append(addon);
		FunctionHeader.Append(CodeName);
		FunctionHeader.Append(" (");
		//VertexData vd, Precalculations pc, UnityGI gi, Masks mask
		if (surfaceInternals.IndexOf("vd.")>=0){
			SD.UsesVertexData = true;UsesVertexData = true;}
		if (surfaceInternals.IndexOf("vdUnchanged.")>=0){
			SD.UsesDepth = true;UsesVertexDataUnchanged = true;}
		if (surfaceInternals.IndexOf("vtp")>=0){
			UsesVertexToPixel = true;}
		if (surfaceInternals.IndexOf("pc.")>=0){
			SD.UsesPrecalculations = true;UsesPrecalculations = true;}
		if (surfaceInternals.IndexOf("gi.")>=0){
			SD.UsesUnityGIFragment = true;UsesUnityGI = true;}//Debug.Log(surfaceInternals);Debug.Log(SD.ShaderPass);}
		if (surfaceInternals.IndexOf("giInput")>=0){
			SD.UsesUnityGIInputFragment = true;UsesUnityGIInput = true;}
		if (surfaceInternals.IndexOf("mask.")>=0){
			SD.UsesMasks = true;UsesMasks = true;}
		if (surfaceInternals.IndexOf("outputColor")>=0){
			UsesOutputColor = true;}
		if (surfaceInternals.IndexOf("outputDepth")>=0){
			SD.UsesDepth = true;UsesDepth = true;}
		if (surfaceInternals.IndexOf("previousBaseColor")>=0){
			SD.UsesPreviousBaseColor = true;UsesPreviousBaseColor = true;}
		//if (surfaceInternals.IndexOf("prevDeferredOut")>=0){
		if (SD.ShaderPassIsDeferred)
			UsesDeferredOutput = true;
		if (surfaceInternals.IndexOf("OneMinusAlpha")>=0){
			SD.UsesOneMinusAlpha = true;UsesOneMinusAlpha = true;}
			
		//if (ShaderSurfaceComponent==ShaderSurfaceComponent.Depth&&surfaceInternals.IndexOf("uvOffset")>=0){
		//	SD.UsesUVOffset = true;UsesUVOffset = true;}

		string asd = " ";
		if (Call){
			if (UsesVertexData){FunctionHeader.Append(asd + "vd");asd = ", ";}
			if (UsesVertexDataUnchanged){FunctionHeader.Append(asd + "NonDisplacedvd");asd = ", ";}
			if (UsesVertexToPixel){FunctionHeader.Append(asd + "vtp");asd = ", ";}
			if (UsesPrecalculations){FunctionHeader.Append(asd+"pc");asd = ", ";}
			if (UsesUnityGI){FunctionHeader.Append(asd+"gi");asd = ", ";}
			if (UsesUnityGIInput){FunctionHeader.Append(asd+"giInput");asd = ", ";}
			if (UsesMasks){FunctionHeader.Append(asd+"mask");asd = ", ";}
			if (UsesOutputColor){FunctionHeader.Append(asd+"outputColor");asd = ", ";}
			if (UsesDepth){FunctionHeader.Append(asd+"depth");asd = ", ";}
			if (UsesPreviousBaseColor){FunctionHeader.Append(asd+"previousBaseColor");asd = ", ";}
			if (UsesDeferredOutput){FunctionHeader.Append(asd+"deferredOut");asd = ", ";}
			if (UsesOneMinusAlpha){FunctionHeader.Append(asd+"OneMinusAlpha");asd = ", ";}
			//if (UsesUVOffset){FunctionHeader.Append(asd+"uvOffset");asd = ", ";}
		}
		else{
			if (UsesVertexData){FunctionHeader.Append(asd + "VertexData vd");asd = ", ";}
			if (UsesVertexDataUnchanged){FunctionHeader.Append(asd + "VertexData vdUnchanged");asd = ", ";}
			if (UsesVertexToPixel){FunctionHeader.Append(asd + "VertexToPixel vtp");asd = ", ";}
			if (UsesPrecalculations){FunctionHeader.Append(asd+"Precalculations pc");asd = ", ";}
			if (UsesUnityGI){FunctionHeader.Append(asd+"UnityGI gi");asd = ", ";}
			if (UsesUnityGIInput){FunctionHeader.Append(asd+"UnityGIInput giInput");asd = ", ";}
			if (UsesMasks){FunctionHeader.Append(asd+"Masks mask");asd = ", ";}
			if (UsesOutputColor){FunctionHeader.Append(asd+"half4 outputColor");asd = ", ";}
			if (UsesDepth){FunctionHeader.Append(asd+"half3 outputDepth");asd = ", ";}
			if (UsesPreviousBaseColor){FunctionHeader.Append(asd+"inout half4 previousBaseColor");asd = ", ";}
			if (UsesDeferredOutput){FunctionHeader.Append(asd+"DeferredOutput deferredOut");asd = ", ";}
			if (UsesOneMinusAlpha){FunctionHeader.Append(asd+"out half OneMinusAlpha");asd = ", ";}
			//if (UsesUVOffset){FunctionHeader.Append(asd+"out half3 uvOffset");asd = ", ";}
		}

		FunctionHeader.Append(")");
		if (!Call){
			FunctionHeader.Append("{\n	");
			//if (SD.ShaderPassIsDeferred&&ShaderSurfaceComponent==ShaderSurfaceComponent.Color){
			//	FunctionHeader.Append("DeferredOutput deferredOut;\n	UNITY_INITIALIZE_OUTPUT(DeferredOutput,deferredOut);\n	");
			//}
		}
		OutputsOneMinusAlpha|=UsesOneMinusAlpha;
		return FunctionHeader.ToString();
	}
	public override List<ShaderLayerList> GetMyShaderLayerLists(bool All){
		List<ShaderLayerList> SLLs = new List<ShaderLayerList>();
		SLLs.Add(SurfaceLayers);
		if ((UseCustomLighting.On&&CustomLightingEnabled)||All)
			SLLs.Add(CustomLightingLayers);
		if ((UseCustomAmbient.On&&CustomAmbientEnabled)||All)
			SLLs.Add(CustomAmbientLayers);
		//SurfaceLayers.UsesAlpha = SD.UsingBlending&&(AlphaBlendMode.Type==0||AlphaBlendMode.Type==2);
		return SLLs;
	}
	
	public override void RenderWedgeUI(ShaderPass SP, float Width){
		Rect MixAmountPos = new Rect(0,15+5+4,Width,8);
		if (EMindGUI.MouseDownIn(MixAmountPos,false)){
			SP.selectedIngredient = this;
		}
		ShaderUtil.DrawCoolSlider(MixAmountPos,MixAmount,1,SP.selectedIngredient==this);
	}
	
	public virtual void CalculateTransparency(){
		//foreach(ShaderLayer SL in SurfaceLayers){
		if (ForceAlphaGenerate){
			UseAlphaGenerate = UseAlpha.On;
			return;
		}
		
		UseAlphaGenerate = true;
		if (SurfaceLayers.SLs.Count==0)
			UseAlphaGenerate = false;
		else{
			for(int i = SurfaceLayers.SLs.Count-1;i>=0;i--){
				ShaderLayer SL = SurfaceLayers.SLs[i];
				if (//IS Blocking
					(SL.AlphaBlendMode.Names[SL.AlphaBlendMode.Type]=="Replace"||((!SL.UsesAlpha()||(i==0&&SurfaceLayers.BaseColor == new Color(0,0,0,0)))&&SL.AlphaBlendMode.Names[SL.AlphaBlendMode.Type]=="Blend"))&&
					!SL.UsesAlpha()&&
					((SL.MixAmount.Get()=="1"&&
					SL.Stencil.Obj==null&&
					SL.MixType.Type==0))//||SurfaceLayers.BaseColor != new Color(0,0,0,0))//Mix
				){
					UseAlphaGenerate = false;
					break;
				}
			}
		}
	}
	public override bool IsPremultiplied(ShaderData SD){
		return OutputPremultiplied&&ShaderSurfaceComponent == ShaderSurfaceComponent.Color;
	}
	public void CalculatePremultiplication(){
		int BlendCount = 0;
		//Debug.Log(CodeName);
		for(int i = SurfaceLayers.SLs.Count-1;i>=0;i--){
			ShaderLayer SL = SurfaceLayers.SLs[i];
			SL.IsBlocker = false;
			if (//IS Blocking
				(SL.AlphaBlendMode.Names[SL.AlphaBlendMode.Type]=="Replace"||((!SL.UsesAlpha()||(i==0&&SurfaceLayers.BaseColor == new Color(0,0,0,0)))&&SL.AlphaBlendMode.Names[SL.AlphaBlendMode.Type]=="Blend"))&&
				!SL.UsesAlpha()&&
				SL.MixAmount.Get()=="1"&&
				SL.MixType.Type==0&&//Mix
				SL.Stencil.Obj==null
			){
				SL.IsBlocker = true;
				//Debug.Log("Blocking"+SL.Name.Text);
				BlendCount = 0;
				break;
			}
			//Debug.Log("++?"+SL.Name.Text);
			//Debug.Log(SL.MixAmount.Get()!="0");
			//Debug.Log((!(SL.AlphaBlendMode.Names[SL.AlphaBlendMode.Type]=="Replace"||(((i==0&&SurfaceLayers.BaseColor == new Color(0,0,0,0)))&&SL.AlphaBlendMode.Names[SL.AlphaBlendMode.Type]=="Blend"))&&SL.UseAlpha.On));
			//Debug.Log(SL.MixType.Type!=0);
			//Debug.Log(SL.MixAmount.Get()!="1");
			if (
				/*SL.MixAmount.Get()!="0"&&(
				(!(SL.AlphaBlendMode.Names[SL.AlphaBlendMode.Type]=="Replace"||((!SL.UseAlpha.On||(i==0&&SurfaceLayers.BaseColor == new Color(0,0,0,0)))&&SL.AlphaBlendMode.Names[SL.AlphaBlendMode.Type]=="Blend"))&&
				SL.UseAlpha.On)
				||
				SL.MixType.Type!=0||
				SL.MixAmount.Get()!="1")*/
				!(//ISN'T Blocking
				(SL.AlphaBlendMode.Names[SL.AlphaBlendMode.Type]=="Replace"||((!SL.UsesAlpha()||(i==0&&SurfaceLayers.BaseColor == new Color(0,0,0,0)))&&SL.AlphaBlendMode.Names[SL.AlphaBlendMode.Type]=="Blend"))&&
				!SL.UsesAlpha()&&
				SL.MixAmount.Get()=="1"&&
				SL.MixType.Type==0&&//Mix
				SL.Stencil.Obj==null
			)
			){
				//Debug.Log("++"+SL.Name.Text);
				if (SL.MixType.Type==1&&
				(SL.AlphaBlendMode.Names[SL.AlphaBlendMode.Type]=="Replace"||SL.AlphaBlendMode.Names[SL.AlphaBlendMode.Type]=="Blend"))//Add
				BlendCount++;
				
				if (SL.MixAmount.Get()!="1"||SL.Stencil.Obj!=null)
					BlendCount++;
			
			
				BlendCount++;
			}
		}
		if (BlendCount<=1)
			OutputPremultiplied = false;
	}
	public ShaderFix DeferredBlending = null;
	public ShaderFix DeferredCustomLighting = null;
	public override void GetMyPersonalPrivateForMyEyesOnlyFixes(ShaderBase shader, ShaderPass pass, List<ShaderFix> fixes){
		if (MixAmountAlmostOne==null)
			MixAmountAlmostOne = new ShaderFix(this,"Set the Mix Amount to 1 (from the super close value of x).",ShaderFix.FixType.Optimization,
			()=>{
				MixAmount.Float = 1f;
			});
		if (MixAmountAlmostZero==null)
			MixAmountAlmostZero = new ShaderFix(this,"Set the Mix Amount to 0 (from the super close value of x).",ShaderFix.FixType.Optimization,
			()=>{
				MixAmount.Float = 0f;
			});
		if (DeferredBlending==null)
			DeferredBlending = new ShaderFix(this,"Ingredient blending in deferred isn't really supported much at all - I'd set Mix Amount to 1 and Mix Type to...Mix XD",ShaderFix.FixType.Warning,
			()=>{
				MixAmount.Float = 1f;
				MixType.Type = 0;
			});
		if (DeferredCustomLighting==null)
			DeferredCustomLighting = new ShaderFix(this,"Disable custom lighting, since it isn't supported in deferred.",ShaderFix.FixType.Warning,
			()=>{
				UseCustomLighting.On = false;
			});
	
		GetMyPersonalPrivateForMySubStandardChildEyesOnlyFixes(shader, pass, fixes);
		if (MixAmount.Input==null&&MixAmount.Float!=1f&&MixAmount.Float>0.95f){
			MixAmountAlmostOne.Message = "Set the Mix Amount to 1 (from the super close value of "+MixAmount.Float.ToString()+").";
			fixes.Add(MixAmountAlmostOne);
		}
		if (MixAmount.Input==null&&MixAmount.Float!=0f&&MixAmount.Float<0.05f){
			MixAmountAlmostZero.Message = "Set the Mix Amount to 0 (from the super close value of "+MixAmount.Float.ToString()+").";
			fixes.Add(MixAmountAlmostZero);
		}
		if (shader.RenderingPathTarget.Type==1){//Deferred
		//Debug.Log(Name+MixAmount.SafeOnExport().ToString());
			if (!MixAmount.SafeOnExport()||MixType.Type!=0){
				fixes.Add(DeferredBlending);
			}
			if (CustomLightingEnabled&&UseCustomLighting.On){
				fixes.Add(DeferredCustomLighting);
			}
		}
	}
}
}

