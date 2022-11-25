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
public class ShaderSurfaceParallaxOcclusionMapping : ShaderSurface{
	public ShaderVar Displacement = new ShaderVar("Displacement",new string[] {"Down","Up"},new string[]{"ImagePreviews/POMDownNone.png","ImagePreviews/POMUpNone.png"},"",new string[] {"",""});
	public ShaderVar DisplacementOffImageLazy = new ShaderVar("DisplacementOffImageLazy",new string[] {"Off"},new string[]{"ImagePreviews/POMOff.png"},"",new string[] {""});
	public ShaderVar Parallax = new ShaderVar("Parallax",0.25f,0f,1f);
	public ShaderVar LinearSearch = new ShaderVar("LinearSearch",20,0,100);
	public ShaderVar LinearSearchShadow = new ShaderVar("LinearSearchShadow",20,0,100);
	public ShaderVar BinarySearch = new ShaderVar("BinarySearch",5,0,15);
	public ShaderVar CurveCorrection = new ShaderVar("Curve Correction",new string[] {"None","Basic","Manual"},new string[] {"Warping artifacts visible in the deeper parts and completely incorrect curvature, probably not an issue for things like bricks on a cube - flat and mostly surface level.","Corrects warping artifacts for flat surfaces and some for surved surfaces - good for flat surfaces with large deep areas.","Presents a second layer channel for curvature data (can be baked in Vertex Toastie) - almost completely corrects all artifacts."});
	public ShaderVar SilhouetteClipping = new ShaderVar("Silhouette Clipping",new string[] {"None","UVs","Curve"});
	public ShaderVar DepthCorrection = new ShaderVar("Depth Correction",false);
	public ShaderVar ShadowCorrection = new ShaderVar("Shadows Correction",false);
	public ShaderVar HighQualityMode = new ShaderVar("High Quality Mode",false);
	public ShaderVar LowestPart = new ShaderVar("LowestPart",new string[] {"Black","White"},new string[] {"",""});
	public ShaderVar CorrectMipMaps = new ShaderVar("CorrectMipMaps",true);
	///TOoDO LowestPart optimization
	///TOoDO Shadows warning
	public ShaderFix LowestPartSlow = null;
	public ShaderFix ShadowsWeird = null;
	public override void GetMyPersonalPrivateForMySubStandardChildEyesOnlyFixes(ShaderBase shader, ShaderPass pass, List<ShaderFix> fixes){
		if (LowestPartSlow==null)
			LowestPartSlow = new ShaderFix(this,"Reverse Crevice setting (and change art assets); the current setting causes an invert of the height texture every pixel.",ShaderFix.FixType.Optimization,
			()=>{
				if (Displacement.Type==0)
					LowestPart.Type = 1;
				if (Displacement.Type==1)
					LowestPart.Type = 0;
			});
		if (ShadowsWeird==null)
			ShadowsWeird = new ShaderFix(this,"Sorry, Unity's directional screen space shadows will cause glitches with POM shadows - you can either disable them (if you haven't already), or turn off this shader recieving them (in Misc settings)",ShaderFix.FixType.Error,
			null);
			
			if ((Displacement.Type==0&&LowestPart.Type==0)||(Displacement.Type==1&&LowestPart.Type==1))
				fixes.Add(LowestPartSlow);
			if (ShadowCorrection.On)
				fixes.Add(ShadowsWeird);
	}
	public ShaderLayerList	CurvatureLayers;
	public override void GetMyShaderVars(List<ShaderVar> SVs){
		SVs.Add(Displacement);
		SVs.Add(Parallax);
		SVs.Add(LinearSearch);
		SVs.Add(LinearSearchShadow);
		SVs.Add(BinarySearch);
		SVs.Add(CurveCorrection);
		SVs.Add(SilhouetteClipping);
		SVs.Add(DepthCorrection);
		SVs.Add(ShadowCorrection);
		SVs.Add(HighQualityMode);
		SVs.Add(LowestPart);
		SVs.Add(CorrectMipMaps);
	}
	new static public string MenuItem = "Depth/Parallax Occlusion Mapping";
	public void OnEnable(){
		Name = "Parallax Occlusion Mapping";
		MenuItem = "Depth/Parallax Occlusion Mapping";
		SurfaceLayers.Description = "The depth of the surface (darker is lower).";///TOoDO: Change depending on settings
		SurfaceLayers.NameUnique.Text = "Depth";
		SurfaceLayers.Name.Text = "Depth";
		
		SurfaceLayers.CodeName = "newDepth";
		SurfaceLayers.BaseColor = new Color(1,1,1,1);
		ShaderSurfaceComponent = ShaderSurfaceComponent.Depth;
		TopAnchor = new Type[]{};
	}
	public new void Awake(){
		BaseAwake();
		CurvatureLayers = ScriptableObject.CreateInstance<ShaderLayerList>();
		CurvatureLayers.UShaderLayerList("Curvature Co-efficients","Curvature","(!Red and Green  channels used only)\nTwo co-efficients used to calculate curvature for parallax occlusion mapping - typically  baked with Vertex Toastie into 2nd UV map.","Curvature","Curvature","rgb","",new Color(1f,1f,0f,0f));
		CurveCorrection.Type = 1;
		SurfaceLayers.EndTag.Text = "a";
	}
	public override int GetHeight(float Width){
		return 124+18+20+25+25+25+25+25+25+25+25+25+25+25+25+25+25+25;
	}
	public override void RenderUI(float Width){
		int y = 0;
		Displacement.Draw(new Rect(0,y,Width,124));y+=124+18;
		
		//GUI.skin.label.alignment = TextAnchor.UpperCenter;
		//EMindGUI.Label(new Rect(0,y,Width,70),Displacement.Descriptions[Displacement.Type],12);y+=20;
		//GUI.skin.label.alignment = TextAnchor.UpperLeft;
		
		GUI.Label(new Rect(0,y,Width-124,20),"Crevices");y+=20;
		LowestPart.Draw(new Rect(16,y,Width-16,20),"");y+=25;
		
		if (LowestPart.Type==0)
			SurfaceLayers.Description = "The depth of the surface (darker is lower).";
		else if (LowestPart.Type==1)
			SurfaceLayers.Description = "The depth of the surface (brighter is lower).";
		
		if(Displacement.Type==0){
			if (Parallax.Range0==-1f)
				Parallax.Float = -1f*Parallax.Float;
			Parallax.Range0 = 0f;
			Parallax.Range1 = 1f;
		}else{
			if (Parallax.Range0==0f)
				Parallax.Float = -1f*Parallax.Float;
			Parallax.Range0 = -1f;
			Parallax.Range1 = 0f;
		}
		
		Parallax.Draw(new Rect(0,y,Width,20),"Parallax");y+=25;
		//HighQualityMode.Draw(new Rect(0,y,Width,20),"High Quality Mode");y+=25;
		LinearSearch.Draw(new Rect(0,y,Width,20),"Search Quality");y+=25;
		
		GUI.enabled = !HighQualityMode.On;
			BinarySearch.Draw(new Rect(0,y,Width,20),"Search Refinement");y+=25;
		GUI.enabled = true;
		GUI.Label(new Rect(0,y,Width-124,20),"Curve Correction");y+=25;

		CurveCorrection.Draw(new Rect(16,y,Width-16,20),"Search Refinement");y+=70;
		
		GUI.Label(new Rect(0,y,Width-124,20),"Silhouette");y+=20;
		SilhouetteClipping.Names = (CurveCorrection.Type==2)?(new string[] {"None","UVs","Curve"}):(new string[] {"None","UVs"});
		SilhouetteClipping.Draw(new Rect(16,y,Width-16,20),"");y+=25;
		
		//GUI.Label(new Rect(0,y,Width-124,20),"Silhouette");y+=20;
		CorrectMipMaps.Draw(new Rect(0,y,Width,20),"Mip Map Correction");y+=25;
		DepthCorrection.Draw(new Rect(0,y,Width,20),"Depth Correction");y+=25;
		ShadowCorrection.Draw(new Rect(0,y,Width,20),"Shadow Correction");y+=25;
		if (ShadowCorrection.On)
			LinearSearchShadow.Draw(new Rect(16,y,Width-16,20),"Shadow Quality");y+=25;
			
	}
	public override string GenerateGlobalFunctions(ShaderData SD,List<ShaderIngredient> ingredients){
		string function = "";
		bool UsedCurveRay = false;
		foreach(ShaderIngredient ingredient in ingredients){
			ShaderSurfaceParallaxOcclusionMapping ing = ingredient as ShaderSurfaceParallaxOcclusionMapping;
			if (ing==null)
				continue;
			if (ing.CurveCorrection.Type==2)
				UsedCurveRay = true;
		}
		if (UsedCurveRay){
			function = 
			"half3 CurveRay(half t,half3 v,half curve){\n"+
			"	half3 r=v*t;\n"+
			"	r.z-=t*t*curve;\n"+
			"	return r;\n"+
			"}\n";
			;
		}
		return function;
	}
	public override string GenerateCode(ShaderData SD){
		if (!ShadowCorrection.On&&SD.ShaderPassIsShadowCaster)
			return "";
		SD.ForceOrigVDMappingHack = false;
		string binserach = BinarySearch.Get();
		string POMCode =
		"float linearSearch = "+LinearSearch.Get()+";\n"+
		"float binarySearch = "+binserach+";\n";
		if (SD.ShaderPassIsShadowCaster){
			POMCode =
			"float linearSearch = "+LinearSearchShadow.Get()+";\n"+
			"float binarySearch = "+binserach+";\n";
		}
		if (CurveCorrection.Type==0)
			POMCode+="float MaxParallax = "+Parallax.Get()+";\n";
		else
			POMCode+="float MaxParallax = "+Parallax.Get()+"/vd.tangentViewDir.z;\n";

		if (CurveCorrection.Type==2){
			POMCode+=
			"half3 Curvature = 0;\n"+
			CurvatureLayers.GenerateCode(SD,OutputPremultiplied)+
				"half curve=Curvature.x*vd.tangentViewDir.x*vd.tangentViewDir.x + Curvature.y*vd.tangentViewDir.y*vd.tangentViewDir.y;\n"+
				"float safeCurve = max(abs(curve),0.001);\n"+
				"if (curve<0)\n"+
				"	safeCurve = -safeCurve;\n"+
				"\n"+
				"float3 mapping = float3(1, 1, 1.0/"+Parallax.Get()+");\n"+
				"\n"+
				"half d = vd.tangentViewDir.z*vd.tangentViewDir.z - 4*safeCurve*"+Parallax.Get()+";\n"+
				"half depthMax = 100;\n"+
				"if (d > 0)\n"+
				"	depthMax = min(depthMax,(-vd.tangentViewDir.z+sqrt(d))/(-2*safeCurve));\n"+
				"\n"+
				"d = vd.tangentViewDir.z / safeCurve;\n"+
				"\n"+
				"if (d > 0)\n"+
				"	depthMax = min(depthMax,d);\n"+
				"\n"+
				"float size = (depthMax+0.001)/linearSearch; // stepping size\n"+
				"safeCurve *= mapping.z;\n"+
				"vd.tangentViewDir *= mapping;\n"+
				"vd.worldViewDir *= mapping;\n";
		}else{
			POMCode+=
			//"float zCorrect = vd.tangentViewDir.z;\n"+
			"float size = 1.0/(linearSearch+1); // stepping size\n"+
			"vd.tangentViewDir *= MaxParallax;\n"+
			"vd.worldViewDir *= MaxParallax;\n";
		}
		if (Displacement.Type==0)
			POMCode += "half depth = 0;\n";
		else
			POMCode += "half depth = 1;\n";
		
		string potdepth = "newDepth";
		string depth = "depth";
		
		if (LowestPart.Type==0){
			if (Displacement.Type==0)
				potdepth = "1-newDepth";
		}
		else{
			if (Displacement.Type==1)
				potdepth = "1-newDepth";
		}
		if (CurveCorrection.Type==2)
			depth = "CurveRay(depth,vd.tangentViewDir,safeCurve).z";
		
		POMCode+=
		"half newDepth = 0;\n"+
		"VertexData vdorig = vd;\n"+
		"VertexData vd2causehlsl = vd;\n"+
		"half i = 0;\n";
		
		if (LinearSearch.Get()!="0"){
			POMCode+=
			"for(i = 0; i < linearSearch; i++){// search until it steps over (Front to back)\n"+
			"	vd = vdorig;\n"+
			"	half newDepth = 0;\n";
			foreach(ShaderVertexInterpolator i in SD.Interpolators){
				if (i.InFragment&&i.ParallaxDisplacementVariable!=""){
					POMCode += "		vd."+i.Name+" += vd."+i.ParallaxDisplacementVariable+"*depth;\n";
				}
			}
			SD.PipelineStage = PipelineStage.Vertex;
			POMCode+=
				ShaderPass.ProtectSamplings(SurfaceLayers.GenerateCode(SD,OutputPremultiplied))+"\n"+
			"	\n";
			SD.PipelineStage = PipelineStage.Fragment;
			if (Displacement.Type==0)
			POMCode+=
			"	if("+depth+" < "+potdepth+")\n"+
			"		depth += size;\n";
			else
			POMCode+=
			"	if("+depth+" > "+potdepth+")\n"+
			"		depth -= size;\n";
			
			POMCode+=
			"}\n";
		}
		
		if (BinarySearch.Get()!="0"){
			POMCode+=
			"//HLSL throws a fit if I don't do this weird define work around - \"cannot use casts on l-values\"\n"+
			"#define vd vd2causehlsl\n"+
			"for(i = 0; i < binarySearch; i++){// look around for a closer match\n"+
			"	vd = vdorig;\n"+
			"	half newDepth = 0;\n";
			//POMCode += "	vd.worldPos += vd.worldViewDir*depth;\n";
			//foreach (ShaderInput SI in ShaderSandwich.Instance.OpenShader.ShaderInputs){
			//	if (SI.Type==0)
			//		POMCode += "	vd.uv"+SI.Get()+" += vd.tangentViewDir.xy*depth;\n";
			//}
			foreach(ShaderVertexInterpolator i in SD.Interpolators){
				if (i.InFragment&&i.ParallaxDisplacementVariable!=""){
					POMCode += "		vd."+i.Name+" += vd."+i.ParallaxDisplacementVariable+"*depth;\n";
				}
			}
			SD.PipelineStage = PipelineStage.Vertex;
			POMCode+=
			"	size*=0.5;\n"+
			"	\n"+
				ShaderPass.ProtectSamplings(SurfaceLayers.GenerateCode(SD,OutputPremultiplied))+"\n"+
			"	\n";
			SD.PipelineStage = PipelineStage.Fragment;
			
			if (Displacement.Type==0)
			POMCode+="	if("+depth+" < "+potdepth+")\n"+
			"		depth += (2*size);\n"+
			"	\n"+
			"	depth -= size;\n";
			else
			POMCode+="	if("+depth+" > "+potdepth+")\n"+
			"		depth -= (2*size);\n"+
			"	\n"+
			"	depth += size;\n";
			
			
			
			POMCode+=
			"}\n"+
			"#undef vd\n";
		}
		if (SilhouetteClipping.Type==1&&(!SD.ShaderPassIsShadowCaster||ShadowCorrection.On))
				POMCode+=
				"clip(vd.genericTexcoord.x);\n"+
				"clip(vd.genericTexcoord.y);\n"+
				"clip(1-vd.genericTexcoord.x);\n"+
				"clip(1-vd.genericTexcoord.y);\n";
		//if (CurveCorrection.Type==2)
		//	POMCode+=
		//	"uvOffset = CurveRay(depth,vd.tangentViewDir,curve);\n";
			//"uvOffset = vd.tangentViewDir.xy * depth * MaxParallax;\n";
		if (CurveCorrection.Type==2){
			if (SilhouetteClipping.Type==2&&(!SD.ShaderPassIsShadowCaster||ShadowCorrection.On))
				POMCode+=
				"if (depth>depthMax)\n"+
				"	discard;\n";
			POMCode+=
			"return half3(depth,depth*vd.tangentViewDir.z,depth/MaxParallax);\n";
		}
		else
			POMCode+=
			"return half3(depth * MaxParallax,depth * " + Parallax.Get() + ",depth);\n";
			
		if (DepthCorrection.On&&(!SD.ShaderPassIsShadowCaster||ShadowCorrection.On))
			SD.UsesDepthCorrection = true;
		
		SD.ForceOrigVDMappingHack = CorrectMipMaps.On;
		//"return depth * "+Parallax.Get()+" / zCorrect;\n";
		return POMCode;
	}
	public override List<ShaderLayerList> GetMyShaderLayerLists(bool All){
		List<ShaderLayerList> SLLs = new List<ShaderLayerList>();
		SLLs.Add(SurfaceLayers);
		if (CurveCorrection.Type==2||All)
			SLLs.Add(CurvatureLayers);
		return SLLs;
	}
}
}