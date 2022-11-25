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
public class ShaderGeometryModifierWind : ShaderGeometryModifier{
	public ShaderVar TreeOrLeaf = new ShaderVar("TreeOrLeaf",new string[] {"Tree", "Leaf"});
	public override void GetMyShaderVars(List<ShaderVar> SVs){
		SVs.Add(TreeOrLeaf);
	}
	new static public string MenuItem = "Displace/Wind Zone Support";
	public void OnEnable(){
		Name = "Wind Zone Support";
		MenuItem = "Displace/Wind Zone Support";
		SurfaceLayers.BaseColor = new Color(1f,1f,1f,1);
	}
	public override int GetHeight(float Width){
		return 20+45+25+25;
	}
	public override void RenderUI(float Width){
		TreeOrLeaf.Draw(new Rect(0,20,Width,20));
	}
	public override string GenerateVertex(ShaderData SD){
		SD.AdditionalFunctions.Add("#include \"UnityBuiltin3xTreeLibrary.cginc\"\n");
		SD.ShaderAdditionalProperties.Add("	[HideInInspector] _TreeInstanceColor (\"TreeInstanceColor\", Vector) = (1,1,1,1)\n"+
	"	[HideInInspector] _TreeInstanceScale (\"TreeInstanceScale\", Vector) = (1,1,1,1)\n"+
	"	[HideInInspector] _SquashAmount (\"Squash\", Float) = 1\n");
		SD.ShaderAdditionalEnd.Add("Dependency \"OptimizedShader\" = \"Hidden/Nature/Tree Creator Leaves Fast Optimized\"\n");
		string DispCode = "	appdata_full temp"+CodeName+"SpeedTreeVB;"+
		"	UNITY_INITIALIZE_OUTPUT(appdata_full,temp"+CodeName+"SpeedTreeVB);\n"+
		"	temp"+CodeName+"SpeedTreeVB.vertex = v.vertex;\n"+
		"	temp"+CodeName+"SpeedTreeVB.tangent = v.tangent;\n"+
		"	temp"+CodeName+"SpeedTreeVB.normal = v.normal;\n"+
		"	temp"+CodeName+"SpeedTreeVB.texcoord = v.texcoord;\n"+
		"	temp"+CodeName+"SpeedTreeVB.texcoord1 = v.texcoord1;\n"+
		"	temp"+CodeName+"SpeedTreeVB.texcoord2 = v.texcoord2;\n"+
		"	temp"+CodeName+"SpeedTreeVB.texcoord3 = v.texcoord3;\n"+
		"	temp"+CodeName+"SpeedTreeVB.color = v.color;\n";
		
		if (TreeOrLeaf.Type==0)
			DispCode += "\n	TreeVertBark(temp"+CodeName+"SpeedTreeVB);\n"+
			"	v.vertex.xyz = temp"+CodeName+"SpeedTreeVB.vertex.xyz;\n"+
			"	v.color.xyz = temp"+CodeName+"SpeedTreeVB.color.xyz;\n"+
			"	v.normal.xyz = temp"+CodeName+"SpeedTreeVB.normal.xyz;\n"+
			"	v.tangent.xyz = temp"+CodeName+"SpeedTreeVB.tangent.xyz;\n";
		else
		DispCode += "\n	TreeVertLeaf(temp"+CodeName+"SpeedTreeVB);\n"+
		"	v.vertex.xyz = temp"+CodeName+"SpeedTreeVB.vertex.xyz;\n"+
		"	v.color.xyz = temp"+CodeName+"SpeedTreeVB.color.xyz;\n"+
		"	v.normal.xyz = temp"+CodeName+"SpeedTreeVB.normal.xyz;\n"+
		"	v.tangent.xyz = temp"+CodeName+"SpeedTreeVB.tangent.xyz;\n";
		return DispCode;
	}
	public override List<ShaderLayerList> GetMyShaderLayerLists(bool All){
		List<ShaderLayerList> SLLs = new List<ShaderLayerList>();
		return SLLs;
	}
}
}