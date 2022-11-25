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
public class ShaderGeometryModifierShells : ShaderGeometryModifier{
	public ShaderVar ShellCount = new ShaderVar("ShellCount",5f);
	public ShaderVar ShellDistance = new ShaderVar("ShellDistance",0.1f);
	//public ShaderVar ShellDistanceType = new ShaderVar("ShellDistanceType",new string[] {"Local","World","Screen"});
	public ShaderVar ShellDistanceType = new ShaderVar("ShellDistanceType",new string[] {"Local","World"});
	public ShaderVar ShellSide = new ShaderVar("ShellSide",new string[] {"Front","Back"});
	public ShaderVar ShellEase = new ShaderVar("ShellEase",1f);
	public ShaderVar ShellEaseInv = new ShaderVar("ShellEaseInv",true);
	public ShaderVar ShellSkipFirst = new ShaderVar("ShellSkipFirst",false);

	public override void GetMyShaderVars(List<ShaderVar> SVs){
		SVs.Add(ShellCount);
		SVs.Add(ShellDistance);
		SVs.Add(ShellDistanceType);
		SVs.Add(ShellSide);
		SVs.Add(ShellEase);
		SVs.Add(ShellEaseInv);
		SVs.Add(ShellSkipFirst);
	}
	new static public string MenuItem = "Generate/Shells";
	public void OnEnable(){
		Name = "Shells";
		MenuItem = "Generate/Shells";
	}
	public new void Awake(){
		BaseAwake();
		ShellSide.Type = 1;
	}
	public override int GetHeight(float Width){
		return 20+45+25+25;
	}
	public override void RenderUI(float Width){
		int y = 20;
		ShellDistance.Range0 = 0f;
		ShellDistance.Range1 = 1f;
		ShellCount.Range0 = 0.555f;
		ShellCount.Range1 = 50f;
		ShellEase.Range0 = 1f;
		ShellEase.Range1 = 4f;
		ShellCount.NoInputs = true;
		
		ShellDistanceType.Draw(new Rect(0,y,Width,20),"");y+=25;
		ShellCount.Draw(new Rect(0,y,Width,20),"Count");y+=25;
		ShellCount.Float = Mathf.Round(ShellCount.Float);
		ShellDistance.Draw(new Rect(0,y,Width,20),"Distance");y+=25;
		ShellEase.Draw(new Rect(0,y,Width,20),"Easing");y+=25;
		ShellEaseInv.Draw(new Rect(0,y,Width,20),"Invert Easing");y+=25;
		ShellSkipFirst.Draw(new Rect(0,y,Width,20),"Skip Original");y+=25;
		ShellSide.Draw(new Rect(0,y,Width,20),"Side");y+=25;
	}
	public override void GeneratePass(ShaderData SD, ShaderPass SP){
		List<Func<ShaderData,ShaderIngredient[],string>> Passes = new List<Func<ShaderData,ShaderIngredient[],string>>(SP.Passes);
		List<Dictionary<string,object>> PassesCD = new List<Dictionary<string,object>>(SP.PassesCD);
		
		SP.Passes.Clear();
		SP.PassesCD.Clear();
		for(float i = (ShellSide.Type==0&&ShellSkipFirst.On)?1:0;i<=((ShellSide.Type==1&&ShellSkipFirst.On)?ShellCount.Float-1f:ShellCount.Float);i++){
			if (ShellSide.Type==0)
				inni(i,SD,SP,Passes,PassesCD);
			else if (ShellSide.Type==1)
				inni(ShellCount.Float-i,SD,SP,Passes,PassesCD);
		}
	}
	public void inni(float i, ShaderData SD, ShaderPass SP,List<Func<ShaderData,ShaderIngredient[],string>> Passes,List<Dictionary<string,object>> PassesCD){
		for(int ii = 0; ii<Passes.Count;ii++){
			Dictionary<string,object> CD = ShaderData.CopyCD(PassesCD[ii]);
			//string oldShellDepth = "";
			//if (!CD.ContainsKey("ShellDepth"))
			//	oldShellDepth = ((string)CD["ShellDepth"+CodeName])+" + ";
			if (!CD.ContainsKey("ShellDepth"))
				CD["ShellDepth"] = 1f;
			
			if (ShellDistance.Get()==ShellDistance.Float.ToString()){
				if (ShellEase.Get()==ShellEase.Float.ToString()){
					if (ShellEaseInv.On)
						CD["ShellDepth"+CodeName] = ((1f-Mathf.Pow(1f-i/ShellCount.Float,ShellEase.Float)) * ShellDistance.Float).ToString();
					else
						CD["ShellDepth"+CodeName] = (Mathf.Pow(i/ShellCount.Float,ShellEase.Float) * ShellDistance.Float).ToString();
				}
				else{
					if (ShellEaseInv.On)
						CD["ShellDepth"+CodeName] = ("(1.0-pow("+(1f-(i/ShellCount.Float)).ToString()+","+ShellEase.Get()+")) * "+ShellDistance.Get());
					else
						CD["ShellDepth"+CodeName] = ("pow("+(i/ShellCount.Float).ToString()+","+ShellEase.Get()+") * "+ShellDistance.Get());
				}
			}else{
				if (ShellEase.Get()==ShellEase.Float.ToString()){
					if (ShellEaseInv.On)
						CD["ShellDepth"+CodeName] = ((1f-Mathf.Pow(1f-i/ShellCount.Float,ShellEase.Float)).ToString())+" * "+ShellDistance.Get();
					else
						CD["ShellDepth"+CodeName] = ((Mathf.Pow(i/ShellCount.Float,ShellEase.Float)).ToString())+" * "+ShellDistance.Get();
				}
				else{
					if (ShellEaseInv.On)
						CD["ShellDepth"+CodeName] = ("(1.0-pow("+(1f-i/ShellCount.Float).ToString()+","+ShellEase.Get()+")) * "+ShellDistance.Get());
					else
						CD["ShellDepth"+CodeName] = ("pow("+(i/ShellCount.Float).ToString()+","+ShellEase.Get()+") * "+ShellDistance.Get());
				}
			}
			//CD["ShellDepthNormalized"] = ((float)CD["ShellDepthNormalized"])*(i/ShellCount.Float);
			CD["ShellDepthNormalized"] = (i/ShellCount.Float).ToString();
			CD["ShellDepthInvertedNormalized"] = (1f-i/ShellCount.Float).ToString();
			CD["ShellDepth"] = (i/ShellCount.Float).ToString();
			
			SP.AddPass(Passes[ii],CD);
		}
	}
	public override string GenerateVertex(ShaderData SD){
		if (!SD.CustomData.ContainsKey("ShellDepth"+CodeName))
			return "";
		if (ShellDistanceType.Type==0)
			SD.CoordSpace = CoordSpace.Object;
		if (ShellDistanceType.Type==1)
			SD.CoordSpace = CoordSpace.World;///TODOish: make screen work
		
		string Vec = "";
		if (SD.CoordSpace == CoordSpace.Object)
			Vec = "v.normal";
		else
			Vec = "vd.worldNormal";
		string DispCode = "";
		if (SD.CoordSpace == CoordSpace.Object)
			DispCode += "\nv.vertex.xyz = float3(v.vertex.xyz + "+Vec+" * "+((string)SD.CustomData["ShellDepth"+CodeName])+");\n";
		else
			DispCode += "\nvd.worldPos = float3(vd.worldPos + "+Vec+" * "+((string)SD.CustomData["ShellDepth"+CodeName])+");\n";
		return DispCode;
	}
	public override List<ShaderLayerList> GetMyShaderLayerLists(bool All){
		List<ShaderLayerList> SLLs = new List<ShaderLayerList>();
		return SLLs;
	}
}
}