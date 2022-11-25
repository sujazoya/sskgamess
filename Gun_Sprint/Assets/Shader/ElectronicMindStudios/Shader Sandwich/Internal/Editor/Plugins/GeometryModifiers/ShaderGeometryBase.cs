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
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
public class ShaderGeometryModifierMisc : ShaderGeometryModifier{
	public ShaderVar ZWriteMode = new ShaderVar("ZWriteMode",new string[] {"Auto","Yes","No"});
	public ShaderVar ZTestMode = new ShaderVar("ZTestMode",new string[] {"Auto", "Always", "Closer", "Closer or Equal", "Farther", "Farther or Equal", "Equal", "Not Equal"});
	public ShaderVar CullMode = new ShaderVar("CullMode",new string[] {"Back Faces","Front Faces", "None"});
	public ShaderVar ShaderModel = new ShaderVar("ShaderModel",new string[] {"Auto", "Shader Model 2.0","Shader Model 2.5","Shader Model 3.0","Shader Model ES 3.0","Shader Model 4.0 DX11","Shader Model ES 3.1","Shader Model GL 4.1","Shader Model 5.0"},new string[] {
		"Shader Sandwich will attempt to select the minimum shader model - it may select one higher than necessary though, so for mobile feel free to set it manually.", 
		"+ Runs everywhere\n- Can't do much",
		"+ Runs almost everywhere\n+ Decent shader complexity allowed\n- No texture LOD sampling\n- Unity 5.4 +",
		"(Recommended :D)\n+ Wide support\n+ Very good shader complexity allowed\n+ Texture LOD sampling\n- Doesn't work on Windows phones",
		"+ Decent support\n+ Very good shader complexity allowed\n+ Texture LOD sampling and arrays\n- Not supported on DX9, Windows phones and OpenGL ES 2.0 devices",
		"+ Decent support\n+ Very good shader complexity allowed\n+ Geometry shaders\n+ Texture LOD sampling and arrays\n- Not supported on DX9, Windows phones and OpenGL ES 2.0 devices",
		"+ Incredibly large shader complexity allowed\n+ Compute shaders\n+ Texture LOD sampling and arrays\n+ Random access texture writes\n- Poor support, no DX9, no DX11 before SM5.0, no Macs and most mobiles.",
		"(Recommended-ish)\n+ Incredibly large shader complexity allowed\n+ Tessellation and Geometry shaders\n+ Texture LOD sampling and arrays\n+ Random access texture writes\n- Poor support, no DX9, no DX11 before SM5.0, and pretty much any mobiles.",
		"+ Incredibly large shader complexity allowed\n+ Tessellation, Compute and Geometry shaders\n+ Texture LOD sampling and arrays\n+ Random access texture writes\n- Poor support, no DX9, no DX11 before SM5.0, no Macs and pretty much any mobiles."});
		
		public ShaderVar UseFog = new ShaderVar("Use Fog",true);
		public ShaderVar UseLightmaps = new ShaderVar("Use Lightmaps",true);
		public ShaderVar UseForwardFullShadows = new ShaderVar("Use Forward Full Shadows",true);
		public ShaderVar UseForwardVertexLights = new ShaderVar("Use Forward Vertex Lights",true);
		public ShaderVar UseShadows = new ShaderVar("Use Shadows",true);
		
		public ShaderVar GenerateForwardBasePass = new ShaderVar("Generate Forward Base Pass",true);
		public ShaderVar GenerateForwardAddPass = new ShaderVar("Generate Forward Add Pass",true);
		public ShaderVar GenerateDeferredPass = new ShaderVar("Generate Deferred Pass",true);
		public ShaderVar GenerateShadowCasterPass = new ShaderVar("Generate Shadow Caster Pass",true);
	//public ShaderVar ShaderModel = new ShaderVar("ShaderModel",new string[] {"Auto", "Shader Model 2.0","Shader Model 2.5","Shader Model 3.0","Shader Model ES 3.0","Shader Model 4.0 DX11","Shader Model ES 3.1","Shader Model GL 4.1","Shader Model 5.0"},new string[] {"Shader Sandwich will attempt to select the minimum shader model - it may select one higher than necessary though, so for mobile feel free to set it manually.", "Incredibly restrictive for shader complexity, works on all platforms.","Similar to Shader Model 3.0 just with a bit wider support and a few missing features.","Widely supported and will fufill most shader needs - won't work on most Windows Phones though.","Haven't used this one myself, no idea what it's good for...","DX11 ","Shader Model ES 3.1","Shader Model GL 4.1","Shader Model 5.0"});
	public ShaderFix MobileShaderTarget = null;
	public ShaderFix GraphicsAPIWrong = null;
	
	public override void GetMyPersonalPrivateForMySubStandardChildEyesOnlyFixes(ShaderBase shader, ShaderPass pass, List<ShaderFix> fixes){
		if (MobileShaderTarget==null)
			MobileShaderTarget = new ShaderFix(this,"Change the Shader Model to 3.0 - anything above 3.0 limits phone support considerably.",ShaderFix.FixType.Warning,
			()=>{
				ShaderModel.Type = 3;
			});
		
		if (GraphicsAPIWrong==null)
			GraphicsAPIWrong = new ShaderFix(this,"You should save, then switch the current Graphics API (Project Settings\\Player) to at least DirectX 11 or OpenGL Core; These is the lowest the current Shader Model is supported in.",ShaderFix.FixType.Error,
			null);
		
		if (shader.HardwareTarget.Type==2){//&&shader.RenderingPathTarget.Type==0){//Mobile Forward
			if (ShaderModel.Type>3)
				fixes.Add(MobileShaderTarget);
		}else if (ShaderModel.Type>4){
			if (SystemInfo.graphicsDeviceType!=UnityEngine.Rendering.GraphicsDeviceType.Direct3D11&&
				SystemInfo.graphicsDeviceType!=UnityEngine.Rendering.GraphicsDeviceType.Direct3D12&&
				SystemInfo.graphicsDeviceType!=UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore)
					fixes.Add(GraphicsAPIWrong);
		}
	}
	
	public override void GetMyShaderVars(List<ShaderVar> SVs){
		SVs.Add(ZWriteMode);
		SVs.Add(ZTestMode);
		SVs.Add(CullMode);
		SVs.Add(ShaderModel);
		SVs.Add(UseFog);
		SVs.Add(UseLightmaps);
		SVs.Add(UseForwardFullShadows);
		SVs.Add(UseForwardVertexLights);
		SVs.Add(UseShadows);
		SVs.Add(GenerateForwardBasePass);
		SVs.Add(GenerateForwardAddPass);
		SVs.Add(GenerateDeferredPass);
		SVs.Add(GenerateShadowCasterPass);
	}
	new static public string MenuItem = "Hidden/Misc";
	public void OnEnable(){
		Name = "Misc Settings";
		MenuItem = "Hidden/Misc";
		Locked = true;
		Unlink();
	}
	public override int GetHeight(float Width){
		return 380+200;
	}
	public override void RenderUI(float Width){
		int y = 20;
		UseFog.LabelOffset = (int)Width/2;
		UseForwardFullShadows.LabelOffset = (int)Width;
		UseForwardVertexLights.LabelOffset = (int)Width/2;
		GenerateForwardBasePass.LabelOffset = (int)Width;
		GenerateForwardAddPass.LabelOffset = (int)Width;
		GenerateDeferredPass.LabelOffset = (int)Width;
		UseLightmaps.LabelOffset = (int)Width/2;
		UseShadows.LabelOffset = (int)Width/2;
		GenerateShadowCasterPass.LabelOffset = (int)Width;
		
		UseForwardFullShadows.Draw(new Rect(0,y,Width,20),"All Forward Shadows");y+=20;
		UseFog.Draw(new Rect(0,y,Width,20),"Fog");
		UseForwardVertexLights.Draw(new Rect((int)(Width/2f),y,Width,20),"Vertex Lights");y+=20;
		UseLightmaps.Draw(new Rect(0,y,Width,20),"Lightmaps");
		UseShadows.Draw(new Rect((int)(Width/2f),y,Width,20),"Base Shadows");y+=20;
		
		y+=20;
		
		ZTestMode.TypeDispL = 2;
		//GUI.Label(new Rect(0,y,Width,30),"Z(Distance) Write\n - Should the object write to the Z buffer?");y+=30;
		GUI.Label(new Rect(0,y,Width,30),"Should the object write to the Z(distance) buffer?");y+=30;
		ZWriteMode.Draw(new Rect(16,y,Width-16,20),"");y+=20+20;
		GUI.Label(new Rect(0,y,Width,50),"Compared to the previous Z(distance) buffer value, when should the mesh get drawn?");y+=30;
		ZTestMode.Draw(new Rect(16,y,Width-16,20));y+=20*4+20;
		//ZTestMode.Type = 3;
		GUI.Label(new Rect(0,y,Width,30),"Which faces should be culled for speed?");y+=20;
		CullMode.Draw(new Rect(16,y,Width-16,20));y+=20+20;
		GUI.Label(new Rect(0,y,Width,60),"Shader Model Target\n - Changes what capabilities the shader can have, and what hardware it can run on.");y+=40;
		//ShaderModel.Draw(new Rect(16,y,Width-16,20));y+=20+20;
		
		//float[] SMmap = new float[]{		1f,	2f,	3f,	 3f,4f,	 4f,4.6f,4.6f,5f};
		//float[] SMWeirdmap = new float[]{	1f,	2f,	2.5f,3f,3.5f,4f,4.5f,4.6f,5f};
		///TOoDO: Double check this define works
		//Actually just handle it in shader - NOPE NO PREPRPCOEESSORS FOR PRAGAMA HAHSHSH
		//#if UNITY_5_4_OR_NEWER
			//float[] SMWeirdmap = new float[]{	1f,	1.5f,	2f,2.5f,3f,3.5f,4f,4.5f,5f};
		//#else
			float[] SMWeirdmap = new float[]{	1f,	1.5f,	2f,2.5f,3f,3.5f,4f,4.5f,5f};
		//#endif
		ShaderModel.Float = SMWeirdmap[ShaderModel.Type];
		//float SMslider = ShaderModel.Float;//SMmap[ShaderModel.Type];
		//if (Event.current.type==EventType.MouseUp)
		
		ShaderModel.Float = EMindGUI.Slider(new Rect(16,y,Width-16,20),ShaderModel.Float,1f,5f);y+=20;
		for(int i = 0;i<SMWeirdmap.Length;i++){
			if (SMWeirdmap[i]<=ShaderModel.Float)
				ShaderModel.Type = i;
		}
		GUI.Label(new Rect(16,y,Width-16,60),ShaderModel.Names[ShaderModel.Type]);y+=20;
		GUI.Label(new Rect(16,y,Width-16,140),ShaderModel.Descriptions[ShaderModel.Type]);y+=105;
		
		GenerateForwardBasePass.Draw(new Rect(0,y,Width,20),"Base Pass (First directional)");y+=20;
		GenerateForwardAddPass.Draw(new Rect(0,y,Width,20),"Add Pass (Other lights)");y+=20;
		GenerateDeferredPass.Draw(new Rect(0,y,Width,20),"Deferred Pass (yup)");y+=20;
		GenerateShadowCasterPass.Draw(new Rect(0,y,Width,20),"Shadow Pass (Casts custom shadows)");y+=20;
		//Debug.Log(ZTestMode.Type);
		//Direction.Draw(new Rect(0,y,Width,20),"Ambient Scatttering");y+=45;
		//Strength.Draw(new Rect(0,y,Width,20),"Strength");y+=25;
		//MidLevel.Draw(new Rect(0,y,Width,20),"Middle Value");y+=25;
	}
	public override string GenerateVertex(ShaderData SD){
		//Debug.Log("does this work at all?");
		//Debug.Log(ZTestMode.Type);
		//Debug.Log(ZTestMode.Float);
		//Debug.Log((ZTestMode)(int)ZTestMode.Type);
		//I'm an idiot...
		if (ZWriteMode.Type!=0)
		SD.ZWriteMode = (ZWriteMode)(int)ZWriteMode.Type;
		if (ZTestMode.Type!=0)
		SD.ZTestMode = (ZTestMode)(int)ZTestMode.Type;
		SD.CullMode = (CullMode)(int)CullMode.Type;
		if (ShaderModel.Type!=0)
		SD.ShaderModel = (ShaderModel)(int)ShaderModel.Type;
	
	
		SD.ForwardFog = UseFog.On;
		SD.ForwardFullShadows = UseForwardFullShadows.On;
		SD.ForwardVertexLights = UseForwardVertexLights.On;
		SD.GenerateForwardBasePass = GenerateForwardBasePass.On;
		SD.GenerateForwardAddPass = GenerateForwardAddPass.On;
		SD.GenerateForwardDeferredPass = GenerateDeferredPass.On;
		SD.Lightmaps = UseLightmaps.On;
		SD.ForwardShadows = UseShadows.On;
		SD.GenerateShadowCaster = GenerateShadowCasterPass.On;
	
	
		return "";
	}
	public override List<ShaderLayerList> GetMyShaderLayerLists(bool All){
		List<ShaderLayerList> SLLs = new List<ShaderLayerList>();
		return SLLs;
	}
}
}