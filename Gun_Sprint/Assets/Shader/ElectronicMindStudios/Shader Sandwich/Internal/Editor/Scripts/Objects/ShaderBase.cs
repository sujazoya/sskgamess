#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define PRE_UNITY_5
#else
#define UNITY_5
#endif
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
//using System.Diagnostics;
using UEObject = UnityEngine.Object;
using System.Text;
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
//This code is such a mess :(
[System.Serializable]
public enum ShaderPassOLD{Base,Light,Parallax,ShellBase,ShellLight,Mask,MaskLight,MaskBase}

[System.Serializable]
public class ShaderBase : ScriptableObject{// : UnityEngine.Object{

public static ShaderBase Current{
		get{
			if (ShaderSandwich.Instance!=null&&ShaderSandwich.Instance.OpenShader!=null)
			return ShaderSandwich.Instance.OpenShader;
			else
			return null;
		}
		set{
		}
	}
/*public bool IsPBR{
		get{
			foreach(ShaderPass SP in ShaderPasses){
				if (SP.IsPBR)
				return true;
			}
			
			return false;
		}
		set{
		}
	}*/

	public ShaderVar ShaderName=new ShaderVar("ShaderName","Untitled Shader");
	public List<ShaderInput> ShaderInputs = new List<ShaderInput>();
	public List<int> ShaderInputsOrder;// = new List<int>();
	public int ShaderInputCount;
	
	public ShaderGenerate SG;
	public ShaderData SD;
	public List<ShaderPass> ShaderPasses = new List<ShaderPass>();
	public ShaderLayerList ShaderLayersMaskTemp;
	public List<ShaderLayerList> ShaderLayersMasks = new List<ShaderLayerList>();

	public ShaderLayerList AddMask(){
		return AddMask(true);
	}
	public ShaderLayerList AddMask(bool Undoable){
		if (Undoable)
			UnityEditor.Undo.RegisterCompleteObjectUndo(this,"Add Mask");
		ShaderLayerList SLL = ScriptableObject.CreateInstance<ShaderLayerList>();
		SLL.UShaderLayerList("Mask"+ShaderLayersMasks.Count.ToString(),"Mask"+ShaderLayersMasks.Count.ToString(),"Mask"+ShaderLayersMasks.Count.ToString(),"Mask"+ShaderLayersMasks.Count.ToString(),"r","",new Color(0f,0f,0f,0f));
		SLL.IsMask.On=true;
		ShaderLayersMasks.Add(SLL);
		
		ShaderLayersMaskTemp = ScriptableObject.CreateInstance<ShaderLayerList>();
		ShaderLayersMaskTemp.UShaderLayerList("Mask"+ShaderLayersMasks.Count.ToString(),"Mask"+ShaderLayersMasks.Count.ToString(),"Mask"+ShaderLayersMasks.Count.ToString(),"Mask"+ShaderLayersMasks.Count.ToString(),"r","",new Color(0f,0f,0f,0f));
		SLL.IsMask.On=true;
		
		ShaderSandwich.Instance.UpdateMaskShaderVars();
		
		return SLL;
	}
	public void Prepare(){
		AddMask(false);
		ShaderPasses = new List<ShaderPass>();
		ShaderPasses = new List<ShaderPass>();
		ShaderPass SP = ScriptableObject.CreateInstance<ShaderPass>();
		SP.Name.Text = "Pass "+(ShaderPasses.Count+1).ToString();
		ShaderPasses.Add(SP);
		TechCull.Type = 1;
		TechQueue.Type = 0;
		MiscColorSpace.Type = 1;
		
	}

	public List<List<ShaderLayer>> GetShaderLayers(){
		List<List<ShaderLayer>> tempList = new List<List<ShaderLayer>>();
		foreach(ShaderLayerList SLL in ShaderLayersMasks)
		tempList.Add(SLL.SLs);
		
		foreach(ShaderPass SP in ShaderPasses){
			foreach(ShaderIngredient surface in SP.Surfaces){
				if (!surface.IsUsed())continue;
				foreach(ShaderLayerList list in surface.GetMyShaderLayerLists()){
					tempList.Add(list.SLs);
				}
			}
			foreach(ShaderIngredient surface in SP.GeometryModifiers){
				if (!surface.IsUsed())continue;
				foreach(ShaderLayerList list in surface.GetMyShaderLayerLists()){
					tempList.Add(list.SLs);
				}
			}
		}
		return tempList;
	}	
	public List<ShaderLayerList> GetShaderLayerLists(){
		List<ShaderLayerList> tempList = new List<ShaderLayerList>();
		foreach(ShaderLayerList SLL in ShaderLayersMasks)
			tempList.Add(SLL);		
		foreach(ShaderPass SP in ShaderPasses){
			foreach(ShaderIngredient surface in SP.Surfaces){
				if (!surface.IsUsed())continue;
				foreach(ShaderLayerList list in surface.GetMyShaderLayerLists()){
					tempList.Add(list);
				}
			}
			foreach(ShaderIngredient surface in SP.GeometryModifiers){
				if (!surface.IsUsed())continue;
				foreach(ShaderLayerList list in surface.GetMyShaderLayerLists()){
					tempList.Add(list);
				}
			}
		}
		return tempList;
	}
	public void GetMyPersonalPrivateForMyEyesOnlyFixes(ShaderBase shader, List<ShaderFix> fixes){
		
	}
	[NonSerialized]public List<ShaderFix> ProposedFixes = new List<ShaderFix>();
	public virtual void GetMyFixes(List<ShaderFix> fixes){
		GetMyPersonalPrivateForMyEyesOnlyFixes(this,fixes);
		List<ShaderPass> Plants = ShaderPasses;
		foreach(ShaderPass BeautifulPlant in Plants){
			BeautifulPlant.GetMyFixes(this,fixes);
		}
	}
	//SHADER BASE
	//public ShaderVar RenderingPathTarget = new ShaderVar("Rendering Path Target",new string[]{"Forward", "Deferred", "Image Effect"});///COMING SOON!
	public ShaderVar RenderingPathTarget = new ShaderVar("Rendering Path Target",new string[]{"Forward", "Deferred"});
	public ShaderVar HardwareTarget = new ShaderVar("Hardware Target",new string[]{"Desktop", "Console", "Mobile"});

	//TECHNICAL
	public bool TechDropDown;
	public bool TechHelp;
	public ShaderVar TechLOD = new ShaderVar("Tech Lod",200);
	public ShaderVar TechQueue = new ShaderVar("Queue",new string[]{"Auto","Background (1000)", "Geometry (2000)", "Alpha Test (2450)", "Transparent (3000)", "Overlay (4000)", "Custom"},new string[]{"","","","","","",""},new string[]{"Geometry", "Background", "Geometry", "AlphaTest", "Transparent", "Overlay", "Custom"});
	public ShaderVar TechCustomQueue = new ShaderVar("Custom Queue",2000);
	public ShaderVar TechReplacement = new ShaderVar("Replacement",new string[]{"Auto","Background", "Opaque", "Transparent Cutout", "Transparent", "Overlay", "Tree Opaque", "Tree Transparent Cutout", "Tree Billboard","Grass","Grass Billboard","Custom"},new string[]{"","","","","","","","","","","",""},new string[]{"Opaque","Background", "Opaque", "TransparentCutout", "Transparent", "Overlay","TreeOpaque","TreeTransparentCutout","TreeBillboard","Grass","GrassBillboard","Opaque"});
	public ShaderVar TechCustomReplacement = new ShaderVar("Custom Replacement","Opaque");
	
	public ShaderVar TechFallback = new ShaderVar("Fallback",new string[]{"Diffuse", "Specular", "VertexLit", "Custom", "Off"},new string[]{"","","",""},new string[]{"\"Legacy Shaders/Diffuse\"", "\"Legacy Shaders/Specular\"", "\"Legacy Shaders/VertexLit\"", "Custom", "Off"});
	public ShaderVar TechCustomFallback = new ShaderVar("CustomFallback","Legacy Shaders/VertexLit");
	public ShaderVar TechQueueAuto = new ShaderVar("QueueAuto",true);
	public ShaderVar TechReplacementAuto = new ShaderVar("ReplacementAuto",true);
	public ShaderVar TechCull = new ShaderVar("Cull",new string[]{"All","Front","Back"},new string[]{"","",""},new string[]{"Off","Back","Front"}); 

	public ShaderVar TechShaderTarget = new ShaderVar("Tech Shader Target",3f);
	
	public ShaderVar TechExcludeDX9 = new ShaderVar("Exclude DX9",false);

	//Misc
	public bool MiscDropDown;
	public bool MiscHelp;
	public ShaderVar MiscVertexRecalculation = new ShaderVar("Vertex Recalculation",false);
	public ShaderVar MiscFog = new ShaderVar("Use Fog",true);
	public ShaderVar MiscAmbient = new ShaderVar("Use Ambient",true);
	public ShaderVar MiscVertexLights = new ShaderVar("Use Vertex Lights",true);
	public ShaderVar MiscLightmap = new ShaderVar("Use Lightmaps",true);
	public ShaderVar MiscForwardAdd = new ShaderVar("Forward Add",true);
	public ShaderVar MiscShadows = new ShaderVar("Shadows",true);
	public ShaderVar MiscFullShadows = new ShaderVar("Use All Shadows",true);
	public ShaderVar MiscInterpolateView = new ShaderVar("Interpolate View",false);
	public ShaderVar MiscHalfView = new ShaderVar("Half as View",false);
	
	public ShaderVar MiscColorSpace = new ShaderVar("Color Space",new string[]{"Mixed","Linear Correct","Same as Gamma"},new string[]{"","",""},new string[]{"Mixed","Linear Correct","Same as Gamma"});

	//DIFFUSE
	public bool DiffuseDropDown;
	public bool DiffuseHelp;
	public ShaderVar DiffuseOn = new ShaderVar("Diffuse On",true);

	public ShaderVar DiffuseLightingType = new ShaderVar("Lighting Type",new string[] {"Standard", "Microfaceted", "Translucent","Unlit","PBR Standard","Custom"},new string[]{"ImagePreviews/DiffuseStandard.png","ImagePreviews/DiffuseRough.png","ImagePreviews/DiffuseTranslucent.png","ImagePreviews/DiffuseUnlit.png","ImagePreviews/DiffusePBRStandard.png",""},"",new string[] {"Smooth/Lambert - A good approximation of hard, but smooth surfaced objects.\n(Wood,Plastic)", "Rough/Oren-Nayar - Useful for rough surfaces, or surfaces with billions of tiny indents.\n(Carpet,Skin)", "Translucent/Wrap - Good for simulating sub-surface scattering, or translucent objects.\n(Skin,Plants)","Unlit/Shadeless - No lighting, full brightness.\n(Sky,Globe)","PBR Standard - A physically based version of the Standard option (Unity 5+)","Custom - Create your own lighting calculations in the lighting tab."});//);

	public ShaderVar DiffuseSetting1 = new ShaderVar("Setting1",0f);
	public ShaderVar DiffuseSetting2 = new ShaderVar("Wrap Color",new Vector4(0.4f,0.2f,0.2f,1f));
	public ShaderVar DiffuseNormals = new ShaderVar("Use Normals",0f);

	//SPECULAR
	public bool SpecularDropDown;
	public bool SpecularHelp;
	public ShaderVar SpecularOn = new ShaderVar("Specular On",false);
	public ShaderVar SpecularLightingType = new ShaderVar("Specular Type",new string[] {"Standard", "Circular","Wave"},new string[]{"ImagePreviews/SpecularNormal.png","ImagePreviews/SpecularCircle.png","ImagePreviews/SpecularWave.png"},"",new string[] {"BlinnPhong - Standard specular highlights.", "Circular - Circular specular highlights(Unrealistic)","Wave - A strange wave like highlight."});	

	public ShaderVar SpecularHardness = new ShaderVar("Spec Hardness",0.5f);
	public ShaderVar SpecularEnergy = new ShaderVar("Spec Energy Conserve",true);
	public ShaderVar SpecularOffset = new ShaderVar("Spec Offset",0f);

	//EMISSION
	public bool EmissionDropDown;
	public bool EmissionHelp;
	public ShaderVar EmissionOn = new ShaderVar("Emission On",false);
	public ShaderVar EmissionType = new ShaderVar(
	"Emission Type",new string[]{"Standard","Multiply","Set"},new string[]{"ImagePreviews/EmissionOn.png","ImagePreviews/EmissionMul.png","ImagePreviews/EmissionSet.png"},"",new string[]{"Standard - Simply add the emission on top of the base color.","Multiply - Add the emission multiplied by the base color. An emission color of white adds the base color to itself.","Set - Mixes the shadeless color on top based on the alpha."}
	); 	

	//Transparency
	public bool TransparencyDropDown;
	public bool TransparencyHelp;
	public ShaderVar TransparencyOn = new ShaderVar("Transparency On",false);
	public ShaderVar TransparencyType = new ShaderVar(
	"Transparency Type",new string[]{"Cutout","Fade"},new string[]{"ImagePreviews/TransparentCutoff.png","ImagePreviews/TransparentFade.png"},"",new string[]{"Cutout - Only allows alpha to be on or off, fast, but creates sharp edges. This can have aliasing issues and is slower on mobile.","Fade - Can have many levels of transparency, but can have depth sorting issues (Objects can appear in front of each other incorrectly)."}
	); 
	public ShaderVar TransparencyZWrite = new ShaderVar("ZWrite",false); 
	public ShaderVar TransparencyPBR = new ShaderVar("Use PBR",true); 
	public ShaderVar TransparencyReceive = new ShaderVar("Receive Shadows",false);
	public ShaderVar TransparencyZWriteType = new ShaderVar("ZWrite Type",new string[]{"Full","Cutoff"},new string[]{"","",""},new string[]{"Full","Cutoff"});
	public bool TransparencyOnOff;
	public ShaderVar BlendMode = new ShaderVar("Blend Mode",new string[]{"Mix","Add","Mul"},new string[]{"","",""},new string[]{"Mix","Add","Mul"});

	public ShaderVar TransparencyAmount = new ShaderVar("Transparency",1f);

	//Blend Mode
	public bool BlendModeDropDown;
	public bool BlendModeHelp;
	public int BlendModeType = 0;


	//Shells
	public bool ShellsDropDown;
	public bool ShellsHelp;
	public ShaderVar ShellsOn = new ShaderVar("Shells On",false);
	//public int ShellsCount = 0;
	public ShaderVar ShellsCount = new ShaderVar("Shell Count",1,0,50);	
	public ShaderVar ShellsDistance = new ShaderVar("Shells Distance",0.1f);
	public ShaderVar ShellsEase = new ShaderVar("Shell Ease",1,0f,3f);	
	//public float ShellsEase = 1f;

	//Shells Transparency

	public bool Initiated = false;
	//public string[] CullNames = {"All","Front","Back"};

	[XmlIgnore,NonSerialized]public List<int> NameInputs;
	[XmlIgnore,NonSerialized]public Dictionary<string, int> NameToInt;

	public ShaderVar OtherTypes = new ShaderVar("Parallax Type",new string[] {"Off", "On","Off","Off","Off","On"},new string[]{"ImagePreviews/ParallaxOff.png","ImagePreviews/ParallaxOn.png","ImagePreviews/TransparentOff.png","ImagePreviews/EmissionOff.png","ImagePreviews/ShellsOff.png","ImagePreviews/ShellsOn.png","ImagePreviews/TessellationOff.png","ImagePreviews/TessellationOn.png","ImagePreviews/DiffuseUnlit.png"},"",new string[] {"", "","","","","",""});	
	public string GCTop(ShaderData SD){
		if (SD.Wireframe)
		return "Shader \"Hidden/SSTempWireframe\" {//The Shaders Name\n";
		else
		if (SD.Temp){
			//if (ShaderPreview.Instance!=null&&ShaderPreview.Instance.Expose)
			return "Shader \"Shader Sandwich/Shader Preview\" {//The Shaders Name\n";
			//else
			//return "Shader \"Hidden/SSTemp\" {//The Shaders Name\n";
		}
		else
		return "Shader \""+ShaderName.Text+"\" {//The Shaders Name\n";
	}
	public string GCSubShader(ShaderData SD){
		StringBuilder ShaderCode = new StringBuilder(100000);
		
		//ShaderCode+=GCGrabPass(SG);
		
		foreach (ShaderPlugin SP in SD.UsedPlugins){
			//Debug.Log(SP.TypeS);
			if (SP.ExternalFunction!="")
			ShaderCode.Append("\n"+SP.ExternalFunction+"\n");
		}
		
		/*if (!SG.Wireframe)
		ShaderCode+=GCPassMask(SG,ShaderPassOLD.Base);
		
		if (!ShellsFront.On)
		ShaderCode+=GCShells(SG);

		ShaderCode+=GCPass(SG,ShaderPassOLD.Base);

		if (ShellsFront.On)
		ShaderCode+=GCShells(SG);*/
		var watch = System.Diagnostics.Stopwatch.StartNew();
		foreach(ShaderPass SP in ShaderPasses){
			string SC = "";
			if (SP.Visible.On){
				SC = SP.GenerateCode(SD);
				//if (SC.IndexOf("lod")>0)
				//	SC = SC.Replace("(DEFINEGLSLHERE)","#pragma glsl");
				//else
				//	SC = SC.Replace("(DEFINEGLSLHERE)","");
				ShaderCode.Append(SC);
			}
		}
		watch.Stop();
//		Debug.Log("Generate SubShader Passes"+watch.ElapsedMilliseconds.ToString());
		
		int TechQueueType = TechQueue.Type;
		int TechReplacementType = TechReplacement.Type;
		if (TechQueue.Type==0){
			TechQueueType = 2;
			//if ((( ShaderCode.Length - ShaderCode.Replace("Blend Off","").Length ) / 9)!=(( ShaderCode.Length - ShaderCode.Replace("Blend ","").Length ) / 6))//ShaderCode.Contains("Blend Off"))
			if (SD.BlendsWeirdlyAnywhere)
				TechQueueType = 4;
			//else if (ShaderCode.Contains("discard;")||ShaderCode.Contains("clip("))
			if (SD.ClipsAnywhere)
				TechQueueType = 3;
		}
		//if (TechReplacementAuto.On){///TOoDO:
		//	TechReplacementType = TechQueueType;
		//}
		string Tags = "";
		if (TechReplacement.Type==11)
			Tags = "\"RenderType\"=\""+TechCustomReplacement.Text+"\"";
		else
			Tags = "\"RenderType\"=\""+TechReplacement.CodeNames[TechReplacementType]+"\"";
		
		if (TechQueue.Type==6){
			if (TechCustomQueue.Float<1000)
			Tags+=" \"Queue\"=\"Background-"+(1000f-TechCustomQueue.Float).ToString()+"\"";
			else if (TechCustomQueue.Float>=1000&&TechCustomQueue.Float<2000)
			Tags+=" \"Queue\"=\"Background+"+(TechCustomQueue.Float-1000f).ToString()+"\"";
			else if (TechCustomQueue.Float>=2000&&TechCustomQueue.Float<2450)
			Tags+=" \"Queue\"=\"Geometry+"+(TechCustomQueue.Float-2000f).ToString()+"\"";
			else if (TechCustomQueue.Float>=2450&&TechCustomQueue.Float<3000)
			Tags+=" \"Queue\"=\"AlphaTest+"+(TechCustomQueue.Float-2450f).ToString()+"\"";
			else if (TechCustomQueue.Float>=3000&&TechCustomQueue.Float<4000)
			Tags+=" \"Queue\"=\"Transparent+"+(TechCustomQueue.Float-3000f).ToString()+"\"";
			else if (TechCustomQueue.Float>=4000)
			Tags+=" \"Queue\"=\"Overlay+"+(TechCustomQueue.Float-4000f).ToString()+"\"";
		}
		else
			Tags+=" \"Queue\"=\""+TechQueue.CodeNames[TechQueueType]+"\"";
		///if (SG.UsedPoke)
		///	Tags +="\"SSPoke\" = \"Yes\" ";
		string ShaderCodeString="SubShader {\n"+
		"	Tags { "+Tags+" }//A bunch of settings telling Unity a bit about the shader.\n"+
		"	LOD "+((int)TechLOD.Float).ToString()+"\n"+ShaderCode.ToString()+"}\n";
		

		//ShaderCode += "}\n";

		return ShaderCodeString;
	}
	public string GCLabTags(ShaderPassOLD SP)
	{
		string ShaderCode = "";
		/*ShaderCode += "	blend off //Disabled blending (No Transparency)\n";
		if (!SG.Wireframe){
			if (MiscFog.On==false)
			ShaderCode+="Fog {Mode Off}\n";	
			
			if (TransparencyReceive.On&&TransparencyOn.On)
			ShaderCode+="blend SrcAlpha OneMinusSrcAlpha//Standard Transparency\nZWrite Off\n";
			
			if (ShaderPassShells(SP)&&ShellsBlendMode.Type==1)
			ShaderCode+="blend One One//Add Blend Mode\n";
			if (ShaderPassShells(SP)&&ShellsBlendMode.Type==2)
			ShaderCode+="blend DstColor Zero//Multiply Blend Mode\n";
			
			if (ShaderPassStandard(SP)&&BlendMode.Type==1&&TransparencyType.Type==1&&TransparencyOn.On)
			ShaderCode+="blend One One//Add Blend Mode\n";
			if (ShaderPassStandard(SP)&&BlendMode.Type==2&&TransparencyType.Type==1&&TransparencyOn.On)
			ShaderCode+="blend DstColor Zero//Multiply Blend Mode\n";
		}
*/
		return ShaderCode;
	}
	public List<ShaderVar> GetMyShaderVars(){
		List<ShaderVar> SVs = new List<ShaderVar>();
		
		SVs.Add(TechLOD);
		SVs.Add(TechFallback);
		SVs.Add(TechCustomFallback);
		SVs.Add(TechQueue);
		SVs.Add(TechQueueAuto);
		SVs.Add(TechCustomQueue);
		SVs.Add(TechReplacement);
		SVs.Add(TechReplacementAuto);
		SVs.Add(TechCustomReplacement);
		SVs.Add(TechCull);
		SVs.Add(TechShaderTarget);
		SVs.Add(TechExcludeDX9);

		SVs.Add(DiffuseOn);
		SVs.Add(DiffuseLightingType);
		SVs.Add(DiffuseSetting1);
		SVs.Add(DiffuseSetting2);
		SVs.Add(DiffuseNormals);

		SVs.Add(SpecularOn);
		SVs.Add(SpecularLightingType);
		SVs.Add(SpecularHardness);
		SVs.Add(SpecularEnergy);
		SVs.Add(SpecularOffset);

		SVs.Add(EmissionOn);
		SVs.Add(EmissionType);

		SVs.Add(TransparencyOn);
		SVs.Add(TransparencyType);
		SVs.Add(TransparencyZWrite);
		SVs.Add(TransparencyPBR);
		SVs.Add(TransparencyAmount);
		SVs.Add(TransparencyReceive);
		SVs.Add(TransparencyZWriteType);
		SVs.Add(BlendMode);
		foreach(ShaderPass SP in ShaderPasses){
			SP.GetMyShaderVars(SVs);
		}

		return SVs;
	}
	public void RecalculateAutoInputs(){
		foreach (ShaderInput SI in ShaderInputs){
		
			
			bool NameCollision = false;
			while (1==1){
				NameCollision = false;
				//foreach (ShaderInput SI2 in ShaderInputs){
				for(int i = ShaderInputs.Count - 1; i > -1; i--){
				ShaderInput SI2 = ShaderInputs[i];
					if (ShaderUtil.CodeName(SI.VisName)==ShaderUtil.CodeName(SI2.VisName)&&SI!=SI2){
						NameCollision = true;
						//new String(text.Where(Char.IsDigit).ToArray());
						if (char.IsDigit(SI2.VisName[SI2.VisName.Length-1])){
							SI2.VisName =  SI2.VisName.Substring(0, SI2.VisName.Length - 1)+(int.Parse(SI2.VisName.Substring(SI2.VisName.Length - 1,1))+1).ToString();
						}
						else{
							SI2.VisName += " 2";
						}
					}
				}
				if (NameCollision == false)
				break;
			}
		}
	}

	public string CGProperties(ShaderData SD,string SubShader){
		string ShaderCode = "//The inputs shown in the material panel\nProperties {\n";
		//if (SG.GeneralUV=="uvTexcoord"&&SG.UsedGenericUV)
		//ShaderCode+="	[HideInInspector]Texcoord (\"Generic UV Coords (You shouldn't be seeing this aaaaah!)\", 2D) = \"white\" {}\n";
		
		#if PRE_UNITY_5
		if (IsPBR){
			ShaderCode+="_Cube (\"Reflection Cubemap\", Cube) = \"_Skybox\" {}\n";
		}
		#endif

		if (SD.Temp){
			foreach (ShaderInput SI in ShaderInputs){
				if (SI.InEditor){
					if (SI.Type==ShaderInputTypes.Image){
						if (SI.NormalMap)
							ShaderCode+="	"+"[Normal]"+SI.GetDecorators()+SI.Get()+" (\""+SI.VisName+"\", 2D) = \"bump\" {}\n";
						else
							ShaderCode+="	"+SI.GetDecorators()+SI.Get()+" (\""+SI.VisName+"\", 2D) = \"white\" {}\n";
					}
					if (SI.Type==ShaderInputTypes.Color)
					ShaderCode+="	"+SI.GetDecorators()+SI.Get()+" (\""+SI.VisName+"\", Color) = (1,1,1,1)\n";
					if (SI.Type==ShaderInputTypes.Vector)
					ShaderCode+="	"+SI.GetDecorators()+SI.Get()+" (\""+SI.VisName+"\", Vector) = (1,1,1,1)\n";
					if (SI.Type==ShaderInputTypes.Cubemap)
					ShaderCode+="	"+SI.GetDecorators()+SI.Get()+" (\""+SI.VisName+"\", Cube) = \"white\"{}\n";
					if (SI.Type==ShaderInputTypes.Float)
					ShaderCode+="	"+SI.GetDecorators()+SI.Get()+" (\""+SI.VisName+"\", Float) = 0\n";
					if (SI.Type==ShaderInputTypes.Range)
					ShaderCode+="	"+SI.GetDecorators()+SI.Get()+" (\""+SI.VisName+"\", Range(-1000,1000)) = 0\n";
				}
			}
		}
		else{
			foreach (ShaderInput SI in ShaderInputs){
				if (SI.InEditor){
					if (SI.Type==ShaderInputTypes.Image){
						if (SI.NormalMap)
							ShaderCode+="	"+"[Normal]"+SI.GetDecorators()+SI.Get()+" (\""+SI.VisName+"\", 2D) = \"bump\" {}\n";
						else
							ShaderCode+="	"+SI.GetDecorators()+SI.Get()+" (\""+SI.VisName+"\", 2D) = \"white\" {}\n";
					}
					if (SI.Type==ShaderInputTypes.Color)
					ShaderCode+="	"+SI.GetDecorators()+SI.Get()+" (\""+SI.VisName+"\", Color) = ("+SI.Color.ToString()+")\n";
					if (SI.Type==ShaderInputTypes.Vector)
					ShaderCode+="	"+SI.GetDecorators()+SI.Get()+" (\""+SI.VisName+"\", Vector) = ("+SI.Color.ToString()+")\n";
					if (SI.Type==ShaderInputTypes.Cubemap)
					ShaderCode+="	"+SI.GetDecorators()+SI.Get()+" (\""+SI.VisName+"\", Cube) = \"_Skybox\"{}\n";
					if (SI.Type==ShaderInputTypes.Float)
					ShaderCode+="	"+SI.GetDecorators()+SI.Get()+" (\""+SI.VisName+"\", Float) = "+SI.Number.ToString("F9")+"\n";
					if (SI.Type==ShaderInputTypes.Range)
					ShaderCode+="	"+SI.GetDecorators()+SI.Get()+" (\""+SI.VisName+"\", Range("+SI.Range0.ToString("F9")+","+SI.Range1.ToString("F9")+")) = "+SI.Number.ToString("F9")+"\n";
				}
			}
		}
		foreach(ShaderVar SV in ShaderUtil.GetAllShaderVars()){
			if (SV.Input==null){
				if (SV.CType == Types.Texture&&SubShader.Contains("SSTEMPSV"+SV.GetID().ToString()))
					//ShaderCode+="				sampler2D SSTEMPSV" + SV.GetID().ToString()+";//" + SV.Name+"\n";
					ShaderCode+="	SSTEMPSV"+SV.GetID().ToString()+" (\""+SV.Name+"\", 2D) = \"white\" {}\n";
				if (SV.CType == Types.Cubemap&&SubShader.Contains("SSTEMPSV"+SV.GetID().ToString()))
					//ShaderCode+="				samplerCUBE SSTEMPSV" + SV.GetID().ToString()+";//" + SV.Name+"\n";
					ShaderCode+="	SSTEMPSV"+SV.GetID().ToString()+" (\""+SV.Name+"\", Cube) = \"_Skybox\"{}\n";
			}
		}
		
		foreach(string s in SD.ShaderAdditionalProperties){
			ShaderCode+=("\n");
			ShaderCode+=(s);
		}
		//if (SG.UsedVertexToastie){
		//	ShaderCode+=
		//	"	_AOStrength(\"Occlusion Strength\", Range(0,1)) = 1\n"+
		//	"	_AOPower(\"Occlusion Pushback\", Range(0,4)) = 1\n";
		//}
		
		ShaderCode+="}\n\n";
		return ShaderCode;
	}
	public void GCResetInputs(){
		foreach (ShaderLayer SL in ShaderUtil.GetAllLayers()){
			foreach(ShaderVar SV in SL.ShaderVars){
				if (SV.UseInput==false&&(SV.CType==Types.Texture||SV.CType==Types.Cubemap)){
					SV.Input = null;
				}
			}
		}	

	}
	public void GenerateUsed(ShaderGenerate SG,ShaderData SD){
		SG.U4 = false;
		#if PRE_UNITY_5
		SG.U4 = true;
		#endif
		//UsedScreenPos
		foreach (ShaderInput SI in ShaderInputs){
			SI.UsedMapType0 = false;
			SI.UsedMapType1 = false;
		}
		
		/*if (ParallaxHeight.Get()!="0"&&ParallaxOn.On){
			SG.UsedParallax = true;
			TechShaderTarget.Float = Mathf.Max(TechShaderTarget.Float,3);
			
			if (ParallaxSilhouetteClipping.On)
			SG.UsedGenericUV = true;
			
			SG.UsedNormals = true;
		}*/
		///foreach (ShaderLayerList SL in ShaderLayersMasks){
			//SL.GCUses = 0;
		///	SG.UsedMasks.Add(SL,0);
		///}
		foreach (ShaderLayerList SL in ShaderLayersMasks){
			foreach (ShaderLayerList SL2 in ShaderLayersMasks){
				if (SL!=SL2){
					if (ShaderUtil.CodeName(SL.Name.Text)==ShaderUtil.CodeName(SL2.Name.Text)){
						SL.Name.Text += " 2";
					}
				}
			}
		}
		foreach (ShaderLayer SL in ShaderUtil.GetAllLayers()){
			//Debug.Log(SL.Name.Text);
			if (SL.Stencil.Obj!=null){
				//((ShaderLayerList)SL.Stencil.Obj).GCUses+=1;
				//if (SG.UsedMasks.ContainsKey(((ShaderLayerList)SL.Stencil.Obj)))
				//SG.UsedMasks[((ShaderLayerList)SL.Stencil.Obj)]++;
				SD.UsedMasksVertex.Add((ShaderLayerList)SL.Stencil.Obj);
			}
			/*foreach(ShaderEffect SE in SL.LayerEffects){
				if (ShaderEffect.GetMethod(SE.TypeS,"SetUsed")!=null&&SE.Visible){
					var meth = ShaderEffect.GetMethod(SE.TypeS,"SetUsed");
					
					if (meth.GetParameters().Length==1)
						meth.Invooke(null,new object[]{SD});
					else
						meth.Invooke(null,new object[]{SD,SE});
				}
			}*/
		}
		foreach (ShaderVar SV in ShaderUtil.GetAllShaderVars()){
			if (SV.Obj!=null){
				ShaderLayerList asd = SV.Obj as ShaderLayerList;
				if (asd!=null){
					///SG.UsedMasks[asd]++;
					SD.UsedMasksVertex.Add(asd);
				}
			}
		}
		foreach (ShaderInput SI in ShaderInputs){
			//SI.Mask.SetToMasks(null,0);
			SI.Mask.HookIntoObjects();
			if (SI.Mask.Obj!=null){
				///SG.UsedMasks[((ShaderLayerList)SI.Mask.Obj)]++;
				SD.UsedMasksVertex.Add(((ShaderLayerList)SI.Mask.Obj));
			}
		}
		foreach (ShaderLayer SL in ShaderUtil.GetAllLayers()){
			//Debug.Log(SL.Name.Text);
			//UnityEngine.Debug.Log(SL.Parent);
			//if (SG.UsedMasks.ContainsKey(SL.Parent))
			//UnityEngine.Debug.Log(SG.UsedMasks[SL.Parent]);
			if (!SL.Parent.IsMask.On||
			//SG.UsedMasks[SL.Parent]>0){
			SD.UsedMasksVertex.Contains(SL.Parent)){
			//UnityEngine.Debug.Log("WentAhead");
				//if (SD.UsedPlugins.IndexOf((ShaderPlugin)SL.LayerType.Obj)<0){
					SL.LayerType.Obj.GetType();
					//if ((SL.LayerType.Obj as ShaderPlugin)!=null){///TODO: Figure out why this overload doesn't work after coming out of play mode
					//	Debug.Log("help!wtf!");
						//SG.UsedPlugins.Add((ShaderPlugin)SL.LayerType.Obj);
						SD.UsedPlugins.Add((ShaderPlugin)SL.LayerType.Obj);
					/*}else{
						Debug.Log(SL.LayerType.Obj.GetType());
						Debug.Log(SL.LayerType.Obj);
						Debug.Log(SL.LayerType.Obj as ShaderPlugin);
						Debug.Log((SL.LayerType.Obj as ShaderPlugin) != null);
						Debug.Log("help!!!!!!!");
					}*/
				//}
			
			
				bool NameCollision = false;
				while (1==1){
					NameCollision = false;
					foreach (ShaderLayer SL2 in ShaderUtil.GetAllLayers()){
						if (ShaderUtil.CodeName(SL.Name.Text)==ShaderUtil.CodeName(SL2.Name.Text)&&SL!=SL2){
							NameCollision = true;
							SL.Name.Text += " 2";
						}
					}
					if (NameCollision == false)
					break;
				}
				
				SL.SampleCount = 0;

				//if (SL.IsVertex&&(VertexMasks)(int)SL.VertexMask.Float==VertexMasks.View)
				//	SG.UsedWorldPos = true;
				if (SG.UsedParallax&&SL.GetDimensions()==3)
					SG.UsedWorldNormals = true;
					
			}
		}
		///foreach (ShaderPlugin SP in SG.UsedPlugins){
		///	if (SP.UsesVertexColors)
		///	SG.UsedVertex = true;
		///}
		/*foreach (ShaderPass SP in ShaderPasses){
			if (SP.IndirectAmbientOcclusionOn.On||SP.IndirectSubsurfaceScatteringOn.On)
				SG.UsedVertexToastie = true;
			if (SP.PokeOn.On)
				SG.UsedPoke = true;
		}*/
		
		//if (ShaderLayersShellNormal.Count>0)
		//SG.UsedShellsNormals = true;
		//if (ShaderLayersNormal.Count>0)
		//SG.UsedNormals = true;
		
		///if (SpecularOn.On)
		///SG.UsedViewDir = true;

		//SG.UsedViewDir = true;
		
		//if (ShaderLayersNormal.Count>0&&SG.UsedWorldRefl)
		//SG.UsedWorldNormals = true;
		///string GenTex = "uvTexcoord";
		///float CheckIfTexcoord = 0;
		///foreach (ShaderInput SI in ShaderInputs){
		///	if (SI.Type==ShaderInputTypes.Image)
		///		CheckIfTexcoord+=0.5f;
		///}
		///if (SG.UsedGenericUV)
		///SG.GeneralUV = GenTex;
		
		
		///if (SG.UsedWorldRefl==true)
		///	CheckIfTexcoord+=1;
		///if (SG.UsedScreenPos==true)
		///	CheckIfTexcoord+=1;
		///if (SG.UsedViewDir==true)
		///	CheckIfTexcoord+=1;
		#if PRE_UNITY_5
		///if (DiffuseLightingType.Type==4){
		///	SG.UsedWorldRefl = true;
		///}
		#endif
		
		///if (CheckIfTexcoord>3){
		///	SG.TooManyTexcoords = true;
		///	SG.UsedGenericUV = true;
		///	SG.GeneralUV = GenTex;
		///}
	}
	/*public string GenerateBlendMode(ShaderPass SP){
		string shaderCode = "";
		if (ShaderPassBase(SP)||(ShaderPassShells(SP)&&ShellsBlendMode.Type==0))
		{
			if (TransparencyOn.On==false)
			{
				if (ShaderPassBase(SP))
				shaderCode+="";	
				else
				shaderCode+="Blend One One";	
			}
			if (TransparencyType.Type == 1&&TransparencyOn.On)
			{
				if (ShaderPassBase(SP))
				shaderCode+="Blend SrcAlpha OneMinusSrcAlpha";	
				else
				shaderCode+="Blend SrcAlpha One";	
			}
		}
		if (ShaderPassBase(SP)||(ShaderPassShells(SP)&&ShellsBlendMode.Type==1))
		{
			if (ShaderPassLight(SP))
			shaderCode+="Blend One One";	
			else
			shaderCode+="Blend One One";
		}		
		return shaderCode;		
	}*/
	public string GCGrabPass(ShaderGenerate SG){
		if (SG.UsedGrabPass){
			//sampler2D _GrabTexture;
			//float4 _GrabTexture_TexelSize;
			return "\nGrabPass {}\n";
		}
		return "";
	}
	public string GenerateCode(){
		return GenerateCode(false);
	}
	//public string GenerateCode(bool Temp){
	//	return GenerateCode_Real(Temp);
	//}
	public string GenerateCode(bool Temp){
	return GenerateCode(Temp,false);
	}
	public string GenerateCode(bool Temp,bool Wireframe){
		/*Stopwatch sw = new Stopwatch();
sw.Start();*/
		
		ShaderSandwich.Instance.Status = "Compiling Shader...";//"Generating Shader Code...";
		RecalculateAutoInputs();
		//TechCull.Update(new string[]{"All","Front","Back"},new string[]{"","",""},new string[]{"Off","Back","Front"}); 
		//TechCull.Type = 1;
		StringBuilder ShaderCode = new StringBuilder(100000);
		SG = new ShaderGenerate();
		SD = new ShaderData();
		SD.Temp = Temp;
		SD.Wireframe = Wireframe;
		SD.Base = this;
		GenerateUsed(SG,SD);

		ShaderCode.Append(GCTop(SD));

		ShaderUtil.GetAllShaderVars();
		string SubShader = GCSubShader(SD);
		ShaderCode.Append(CGProperties(SD,SubShader));
		
		ShaderCode.Append(SubShader);

		//if (TransparencyReceive.On&&TransparencyOn.On)
		//ShaderCode+="\nFallback Off\n}";
		//else
		foreach(string s in SD.ShaderAdditionalEnd){
			ShaderCode.Append("\n");
			ShaderCode.Append(s);
		}
		if (TechFallback.CodeNames[TechFallback.Type]=="Custom")
			ShaderCode.Append("\nFallback "+TechCustomFallback.Text+"\n");
			//ShaderCode.Append("\nFallback \""+TechCustomFallback.Text+"\"\n");
		else
			ShaderCode.Append("\nFallback "+TechFallback.CodeNames[TechFallback.Type]+"\n");
		
		ShaderCode.Append("}\n");
		/*sw.Stop();
string ExecutionTimeTaken = string.Format("Minutes :{0}\nSeconds :{1}\n Mili seconds :{2}",sw.Elapsed.Minutes,sw.Elapsed.Seconds,sw.Elapsed.TotalMilliseconds);
UnityEngine.Debug.Log(ExecutionTimeTaken);*/
		///while (ShaderCode != ShaderCode.Replace(".rgb.rgb",".rgb"))
		///ShaderCode = ShaderCode.Replace(".rgb.rgb",".rgb");
		///while (ShaderCode != ShaderCode.Replace(".a.a",".a"))
		///ShaderCode = ShaderCode.Replace(".a.a",".a");
		///while (ShaderCode != ShaderCode.Replace(".xy.xy",".xy"))
		///ShaderCode = ShaderCode.Replace(".xy.xy",".xy");
		///sdfsdf;
		string ShaderCodeString = ShaderCode.ToString();
		ShaderCodeString = ShaderCodeString.Replace("\r\n","\n");
		ShaderCodeString = ShaderCodeString.Replace("\r","\n");
		///TODO: Add the ReplaceTempVariables to all areas the user can input/inpust themselves XD
		if (Temp){
			ShaderCodeString = Regex.Replace(ShaderCodeString, @"\b_Time\b","_SSTime");
			ShaderCodeString = Regex.Replace(ShaderCodeString, @"\b_CosTime\b","_SSCosTime");
			ShaderCodeString = Regex.Replace(ShaderCodeString, @"\b_SinTime\b","_SSSinTime");
		}
		return ShaderCodeString;
	}
	public Dictionary<string,ShaderVar> GetSaveLoadDict(){
		Dictionary<string,ShaderVar> D = new Dictionary<string,ShaderVar>();

		D.Add(ShaderName.Name,ShaderName);

		D.Add(TechLOD.Name,TechLOD);
		D.Add(TechFallback.Name,TechFallback);
		D.Add(TechCustomFallback.Name,TechCustomFallback);
		D.Add(TechQueue.Name,TechQueue);
		D.Add(TechCustomQueue.Name,TechCustomQueue);
		D.Add(TechQueueAuto.Name,TechQueueAuto);
		D.Add(TechReplacement.Name,TechReplacement);
		D.Add(TechReplacementAuto.Name,TechReplacementAuto);
		D.Add(TechShaderTarget.Name,TechShaderTarget);
		D.Add(TechExcludeDX9.Name,TechExcludeDX9);

		return D;
	}
	public string Save(){
		string S = "Shader Sandwich Shader\n	File Format Version(Float): 3.0\n	Begin Shader Base\n";
		foreach (ShaderInput SI in ShaderInputs){
			S+=SI.Save();
		}
		S += ShaderUtil.SaveDict(GetSaveLoadDict(),2);
		
		S += "\n		Begin Masks\n";
			foreach (ShaderLayerList SLL in ShaderLayersMasks){
				S += SLL.Save(3);
			}
		S += "\n		End Masks\n\n";
		foreach (ShaderPass SP in ShaderPasses){
			S+=SP.Save();
		}
		S += "	End Shader Base\n";
		S += "End Shader Sandwich Shader";

		return S;
	}
	public float FileVersion = 3f;
	public void NewLoad(Dictionary<string,ShaderVar> D, StringReader S){
		while(1==1){
			string Line = S.ReadLine();
			if (Line!=null){
				Line = Line.Replace("	","");
				if(Line=="End Shader Base")break;

				if (Line.Contains(":")){
					ShaderUtil.LoadLine(D,Line);
				}
				else if (Line=="Begin Shader Layer List"){
					
					//ShaderLayerList.Load(S,new List<ShaderLayerList>(),ShaderLayersMasks);
					ShaderLayerList.LoadSingle(S,AddMask(false));
				}
				else if (Line=="Begin Masks"){
					//LoadingMasks = true;
				}
				else if (Line=="End Masks"){
					//LoadingMasks = false;
				}
				else if (Line=="Begin Shader Pass"){
					ShaderPass SP = ScriptableObject.CreateInstance<ShaderPass>();
					SP.Name.Text = "Pass "+ShaderPasses.Count.ToString();
					ShaderPasses.Add(SP);
					SP.Load(S);
				}
				else if (Line=="Begin Shader Input")
					ShaderInputs.Add(ShaderInput.Load(S));
			}
			else
			break;
		}
		foreach(ShaderLayerList SLL in ShaderLayersMasks){
			SLL.UpdateIcon(new Vector2(70,70));
		}
		foreach(ShaderPass SP in ShaderPasses){
			foreach(ShaderLayerList SLL in SP.GetShaderLayerLists()){
				SLL.UpdateIcon(new Vector2(70,70));
			}
		}
	}
	public void OldNewLoad(Dictionary<string,ShaderVar> D, StringReader S){
//		bool LoadingMasks = false;
		while(1==1){
			string Line = S.ReadLine();
//			UnityEngine.Debug.Log(Line);
			if (Line!=null){
				if(Line=="EndShaderBase")break;

				if (Line.Contains("#!")){
					ShaderUtil.LoadLineLegacy(D,Line);
				}
				else if (Line=="BeginShaderLayerList"){
					
					ShaderLayerList.LoadLegacy(S,new List<ShaderLayerList>(),ShaderLayersMasks);
				}
				else if (Line=="BeginMasks"){
					//LoadingMasks = true;
				}
				else if (Line=="EndMasks"){
					//LoadingMasks = false;
				}
				else if (Line=="BeginShaderPass"){
					ShaderPass SP = ScriptableObject.CreateInstance<ShaderPass>();
					SP.Name.Text = "Pass "+ShaderPasses.Count.ToString();
					ShaderPasses.Add(SP);
					SP.LoadLegacy(S);
				}
				else if (Line=="BeginShaderInput")
					ShaderInputs.Add(ShaderInput.LoadLegacy(S));
			}
			else
			break;
		}
		TechQueue.Type+=1;
		if (D["QueueAuto"].On)
			TechQueue.Type = 0;
		TechReplacement.Type+=1;
		if (D["ReplacementAuto"].On)
			TechReplacement.Type = 0;
	}
	public void OldLoad(Dictionary<string,ShaderVar> D, StringReader S){
		ShaderPass SPShell = ScriptableObject.CreateInstance<ShaderPass>();
		SPShell.Name.Text = "Shells";
		ShaderPass SPBase = ScriptableObject.CreateInstance<ShaderPass>();
		SPBase.Name.Text = "Base";
		ShaderPasses.Add(SPBase);
		var SP0D = SPBase.GetSaveLoadDictLegacy();
		var SP1D = SPShell.GetSaveLoadDictLegacy();
		while(1==1){
			string Line = S.ReadLine();
			if (Line!=null){
				if(Line=="EndShaderBase")break;

				if (Line.Contains("#!")){
					ShaderUtil.LoadLineLegacy(D,Line);
					ShaderUtil.LoadLineLegacy(SP0D,Line);
					ShaderUtil.LoadLineLegacy(SP1D,Line);
				}
				else if (Line=="BeginShaderLayerList"){
					Line = S.ReadLine();
					ShaderLayerList.LoadLegacy(S,((Line.IndexOf("Shell")<0)?SPBase:SPShell).GetShaderLayerLists(),ShaderLayersMasks,Line);
				}
				else if (Line=="BeginShaderInput")
					ShaderInputs.Add(ShaderInput.LoadLegacy(S));
			}
			else
			break;
		}
		//SPBase.CorrectOld();
		//SPShell.CorrectOld();
		
		/*if (ShellsOn.On){
			if (ShellsFront.On)
				ShaderPasses.Add(SPShell);
			else
				ShaderPasses.Insert(0,SPShell);
			SPBase.ShellsOn.On = false;
			SPShell.BlendMode.Type = ShellsBlendMode.Type;
			SPShell.TechCull.Type = ShellsCull.Type;
			SPShell.DiffuseOn.On = ShellsLighting.On;
		
			SPShell.CorrectStuff();
		}*/
		//SPBase.CorrectStuff();
	}
	static public ShaderBase Load(StringReader S,bool Legacy){
		ShaderBase SB = ShaderBase.CreateInstance<ShaderBase>();
		SB.Prepare();
		SB.ShaderPasses.Clear();
		SB.ShaderLayersMasks.Clear();
		ShaderSandwich.Instance.OpenShader = SB;
		S.ReadLine();
		if (Legacy)
			SB.FileVersion = float.Parse(S.ReadLine());
		else
			SB.FileVersion = (new ShaderVar("File Format Version",3f).Load(S.ReadLine())).Float;
//		UnityEngine.Debug.Log(SB.FileVersion.ToString());
		S.ReadLine();
		
		ShaderSandwich.Instance.OpenShader = SB;
		var D = SB.GetSaveLoadDict();
		var DL = SB.GetSaveLoadDictLegacy();
		
		if (SB.FileVersion<2f){
			EditorUtility.DisplayDialog("Old File","This file was saved with a much older version of Shader Sandwich, so it has to be converted to the new format. It probably won't look the same (or even work/load), sorry!","Ok");
			try{
				SB.OldLoad(DL,S);
			}catch{
				Debug.LogWarning("Sorry, an error occured while loading the file; feel free to send it ztechnologies.com@gmail.com and I can take a look :)");
			}
		}else if (SB.FileVersion<3f){
			EditorUtility.DisplayDialog("Old File","This file was saved with an older version of Shader Sandwich, so it has to be converted to the new format. It might not look exactly the same, sorry!\n\nIf you'd like any help with the conversion feel free to send me an email at ztechnologies.com@gmail.com :)","Ok");
			SB.OldNewLoad(DL,S);
		}else if (SB.FileVersion==3f){
			SB.NewLoad(D,S);
		}else if (SB.FileVersion>3f){
			EditorUtility.DisplayDialog("Newer File","This file was saved in a new version of Shader Sandwich. I'll try and load it anyway but it could load completely wrong - you should update! :)","Ok");
			SB.NewLoad(D,S);
		}

		if (SB.TechShaderTarget.Float==1f)
		SB.TechShaderTarget.Float = 3f;
		return SB;
	}
	
	public void CleanUp(){
		foreach (ShaderLayer SL in ShaderUtil.GetAllLayers()){
			UEObject.DestroyImmediate(SL);
		}
		UEObject.DestroyImmediate(this);
	}
	
	
	
	
	public Dictionary<string,ShaderVar> GetSaveLoadDictLegacy(){
		//bool ShellsTransparencyDropDown;
		//bool ShellsTransparencyHelp;
		ShaderVar ShellsTransparencyType = new ShaderVar(
		"Shell Transparency Type",new string[]{"Cutout","Fade"},new string[]{"ImagePreviews/TransparentCutoff.png","ImagePreviews/TransparentFade.png"},"",new string[]{"Cutout - Only allows alpha to be on or off, fast, but creates sharp edges. This can have aliasing issues and is slower on mobile.","Fade - Can have many levels of transparency, but can have depth sorting issues (Objects can appear in front of each other incorrectly)."}
		); 	
		//bool ShellsTransparencyOnOff; 

		ShaderVar ShellsTransparencyZWrite = new ShaderVar("Shell Transparency ZWrite",false); 
		ShaderVar ShellsCull = new ShaderVar("Shell Cull",new string[]{"All","Front","Back"},new string[]{"","",""},new string[]{"Off","Back","Front"});
		ShaderVar ShellsBlendMode = new ShaderVar("Shell Blend Mode",new string[]{"Mix","Add","Multiply"},new string[]{"","",""},new string[]{"Mix","Add","Mul"});
		ShaderVar ShellsTransparencyAmount = new ShaderVar("Shells Transparency",1f);
		ShaderVar ShellsZWrite = new ShaderVar("Shells ZWrite",true);
		ShaderVar ShellsUseTransparency = new ShaderVar("Shells Use Transparency",true);

		ShaderVar ShellsLighting = new ShaderVar("Shell Lighting",true); 
		ShaderVar ShellsFront = new ShaderVar("Shell Front",true); 

		//Parallax Occlusion
		//bool ParallaxDropDown;
		//bool ParallaxHelp;
		ShaderVar ParallaxOn = new ShaderVar("Parallax On",false);

		ShaderVar ParallaxHeight = new ShaderVar("Parallax Height",0.1f); 
		//int ParallaxLinearQuality = 5;
		ShaderVar ParallaxBinaryQuality = new ShaderVar("Parallax Quality",10,0,50);
		ShaderVar ParallaxSilhouetteClipping = new ShaderVar("Silhouette Clipping",false);
		
		
		//Tessellation
		ShaderVar TessellationOn = new ShaderVar("Tessellation On",false);
		ShaderVar TessellationType = new ShaderVar("Tessellation Type",new string[]{"Equal","Size","Distance"},new string[]{"Tessellate all faces the same amount (Not Recommended!).","Tessellate faces based on their size (larger = more tessellation).","Tessellate faces based on distance and screen area."},new string[]{"Equal","Size","Distance"});
		ShaderVar TessellationQuality = new ShaderVar("Tessellation Quality",10,1,50);
		ShaderVar TessellationFalloff = new ShaderVar("Tessellation Falloff",1,1,3);
		ShaderVar TessellationSmoothingAmount = new ShaderVar("Tessellation Smoothing Amount",0f,-3,3);
		
		
		Dictionary<string,ShaderVar> D = new Dictionary<string,ShaderVar>();

		D.Add(ShaderName.Name,ShaderName);

		D.Add(TechLOD.Name,TechLOD);
		D.Add(TechCull.Name,TechCull);
		D.Add(TechFallback.Name,TechFallback);
		D.Add(TechCustomFallback.Name,TechCustomFallback);
		D.Add(TechQueue.Name,TechQueue);
		D.Add(TechQueueAuto.Name,TechQueueAuto);
		D.Add(TechReplacement.Name,TechReplacement);
		D.Add(TechReplacementAuto.Name,TechReplacementAuto);
		D.Add(TechShaderTarget.Name,TechShaderTarget);
		D.Add(TechExcludeDX9.Name,TechExcludeDX9);

		D.Add(MiscVertexRecalculation.Name,MiscVertexRecalculation);
		D.Add(MiscFog.Name,MiscFog);
		D.Add(MiscAmbient.Name,MiscAmbient);
		D.Add(MiscVertexLights.Name,MiscVertexLights);
		D.Add(MiscLightmap.Name,MiscLightmap);
		D.Add(MiscFullShadows.Name,MiscFullShadows);
		D.Add(MiscForwardAdd.Name,MiscForwardAdd);
		D.Add(MiscShadows.Name,MiscShadows);
		D.Add(MiscInterpolateView.Name,MiscInterpolateView);
		D.Add(MiscHalfView.Name,MiscHalfView);
		D.Add(MiscColorSpace.Name,MiscColorSpace);

		D.Add(DiffuseOn.Name,DiffuseOn);
		D.Add(DiffuseLightingType.Name,DiffuseLightingType);
		D.Add(DiffuseSetting1.Name,DiffuseSetting1);
		D.Add(DiffuseSetting2.Name,DiffuseSetting2);
		D.Add(DiffuseNormals.Name,DiffuseNormals);

		D.Add(SpecularOn.Name,SpecularOn);
		D.Add(SpecularLightingType.Name,SpecularLightingType);
		D.Add(SpecularHardness.Name,SpecularHardness);
		D.Add(SpecularEnergy.Name,SpecularEnergy);
		D.Add(SpecularOffset.Name,SpecularOffset);

		D.Add(EmissionOn.Name,EmissionOn);
		D.Add(EmissionType.Name,EmissionType);

		D.Add(TransparencyOn.Name,TransparencyOn);
		D.Add(TransparencyType.Name,TransparencyType);
		D.Add(TransparencyZWrite.Name,TransparencyZWrite);
		D.Add(TransparencyPBR.Name,TransparencyPBR);
		D.Add(TransparencyAmount.Name,TransparencyAmount);
		D.Add(TransparencyReceive.Name,TransparencyReceive);
		D.Add(TransparencyZWriteType.Name,TransparencyZWriteType);
		D.Add(BlendMode.Name,BlendMode);

		D.Add(ShellsOn.Name,ShellsOn);
		D.Add(ShellsCount.Name,ShellsCount);
		D.Add(ShellsDistance.Name,ShellsDistance);
		D.Add(ShellsEase.Name,ShellsEase);
		D.Add(ShellsTransparencyType.Name,ShellsTransparencyType);
		D.Add(ShellsTransparencyZWrite.Name,ShellsTransparencyZWrite);
		D.Add(ShellsCull.Name,ShellsCull);
		D.Add(ShellsZWrite.Name,ShellsZWrite);
		D.Add(ShellsUseTransparency.Name,ShellsUseTransparency);
		D.Add(ShellsBlendMode.Name,ShellsBlendMode);
		
		D.Add(ShellsTransparencyAmount.Name,ShellsTransparencyAmount);
		D.Add(ShellsLighting.Name,ShellsLighting);
		D.Add(ShellsFront.Name,ShellsFront);

		D.Add(ParallaxOn.Name,ParallaxOn);
		D.Add(ParallaxHeight.Name,ParallaxHeight);
		D.Add(ParallaxBinaryQuality.Name,ParallaxBinaryQuality);
		D.Add(ParallaxSilhouetteClipping.Name,ParallaxSilhouetteClipping);

		D.Add(TessellationOn.Name,TessellationOn);
		D.Add(TessellationType.Name,TessellationType);
		D.Add(TessellationQuality.Name,TessellationQuality);
		D.Add(TessellationFalloff.Name,TessellationFalloff);
		D.Add(TessellationSmoothingAmount.Name,TessellationSmoothingAmount);

		return D;
	}
}
}