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
public class ShaderSurfaceDepth : ShaderSurface{
	public ShaderVar Parallax = new ShaderVar("Parallax",0.025f,-0.1f,0.1f);
	public ShaderVar Displacement = new ShaderVar("Displacement",new string[] {"Down","Up"},new string[]{"ImagePreviews/DepthDown.png","ImagePreviews/DepthUp.png"},"",new string[] {"",""});
	public ShaderVar LowestPart = new ShaderVar("LowestPart",new string[] {"Black","White"},new string[] {"",""});
	public ShaderVar DepthLowQuality = new ShaderVar("DepthLowQuality",false);
	public override void GetMyShaderVars(List<ShaderVar> SVs){
		SVs.Add(Parallax);
		SVs.Add(Displacement);
		SVs.Add(LowestPart);
		SVs.Add(DepthLowQuality);
	}
	new static public string MenuItem = "Depth/Displacement Mapping (Fast)";
	public void OnEnable(){
		Name = "Displacement Mapping";
		MenuItem = "Depth/Displacement Mapping (Fast)";
		SurfaceLayers.Description = "The depth of the surface.";
		SurfaceLayers.NameUnique.Text = "Depth";
		SurfaceLayers.Name.Text = "Depth";
		SurfaceLayers.CodeName = "potdepth";
		SurfaceLayers.BaseColor = new Color(1,1,1,1);
		ShaderSurfaceComponent = ShaderSurfaceComponent.Depth;
		TopAnchor = new Type[]{};
	}
	public new void Awake(){
		BaseAwake();
		SurfaceLayers.EndTag.Text = "a";
	}
	public override int GetHeight(float Width){
		return 124+18+20+25+25+25+25+25;
	}
	public override void RenderUI(float Width){
		int y = 0;
		Displacement.Draw(new Rect(0,y,Width,124));y+=124+18;
		
		GUI.skin.label.alignment = TextAnchor.UpperCenter;
		EMindGUI.Label(new Rect(0,y,Width,70),Displacement.Descriptions[Displacement.Type],12);y+=20;
		GUI.skin.label.alignment = TextAnchor.UpperLeft;
		
		GUI.Label(new Rect(0,y,Width-124,20),"Crevices ");y+=25;
		LowestPart.Draw(new Rect(16,y,Width-16,20),"");y+=25;
		
		//if(Displacement.Type==0){
		//	if (Parallax.Range0==0f)
		//		Parallax.Float = -1f*Parallax.Float;
		//	Parallax.Range0 = -0.1f;
		//	Parallax.Range1 = 0f;
		//}else{
		//	if (Parallax.Range0==-0.1f)
		//		Parallax.Float = -1f*Parallax.Float;
			Parallax.Range0 = 0f;
			Parallax.Range1 = 0.1f;
		//}
		
		Parallax.Draw(new Rect(0,y,Width,20),"Parallax");y+=50;
		DepthLowQuality.Draw(new Rect(0,y,Width,20),"Low Quality");y+=50;
		
	}
	public override string GenerateCode(ShaderData SD){
		if (SD.ShaderPassIsShadowCaster)
			return "";
		SD.ForceOrigVDMappingHack = false;
		string POMCode = "";
		POMCode+="float potdepth = 0;\n";
		POMCode+=SurfaceLayers.GenerateCode(SD,OutputPremultiplied)+"\n";
		
		string potdepth = "potdepth";
		if (LowestPart.Type==0){
			if (Displacement.Type==0)
				potdepth = "(1-potdepth)";
		}
		else{
			if (Displacement.Type==1)
				potdepth = "(1-potdepth)";
		}
		if (!DepthLowQuality.On)
			potdepth += "/vd.tangentViewDir.z";
		if(Displacement.Type==1)
			POMCode+="return "+potdepth+" * -"+Parallax.Get()+";\n";
		else
			POMCode+="return "+potdepth+" * "+Parallax.Get()+";\n";
		return POMCode;
	}
}
}