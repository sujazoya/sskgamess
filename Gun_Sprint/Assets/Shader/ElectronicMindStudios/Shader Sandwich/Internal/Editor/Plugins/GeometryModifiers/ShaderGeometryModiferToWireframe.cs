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
public class ShaderGeometryModifierToWireframe : ShaderGeometryModifier{
	public ShaderVar Type = new ShaderVar("Type",new string[]{"Quick","Quads","Tris"},new string[]{"Fast, but misses a few edges.","Show only quads (may still skip some edges, use with caution).","Shows all edges."},new string[]{"Quick","Quads","Tris"});
	public override void GetMyShaderVars(List<ShaderVar> SVs){
		SVs.Add(Type);
	}
	new static public string MenuItem = "Convert/Wireframe";
	public void OnEnable(){
		Name = "Wireframe";
		MenuItem = "Convert/Wireframe";
		
		ShaderGeometryModifierComponent = ShaderGeometryModifierComponent.Tessellation;
	}
	public override List<ShaderLayerList> GetMyShaderLayerLists(bool All){
		List<ShaderLayerList> SLLs = new List<ShaderLayerList>();
		return SLLs;
	}
	public override int GetHeight(float Width){
		return 25;
	}
	public override void RenderUI(float Width){
		Type.Draw(new Rect(0,20,Width,20),"");
	}
	public override string GenerateHullControlPoints(ShaderData SD){
		SD.TessOutputTopology = TessOutputTopology.Line;
		SD.TessDomain = TessDomain.Isoline;
		return "";
	}
	public override string GenerateHullTessellationFactors(ShaderData SD){
		if (Type.Type>0)
			return "	TF.xy = float2(3,1);\n";
		return "";
	}
	public override string GenerateDomainTransfer(ShaderData SD){
		if (Type.Type==1){
			return 
				"		v.vertex = vi[1].vertex*(1-bary.x) + vi[2].vertex*bary.x;\n";
		}
		if (Type.Type==2){
			return 
				"	if (bary.y>=0.666)\n"+
				"		v.vertex = vi[0].vertex*(1-bary.x) + vi[2].vertex*bary.x;\n"+
				"	else\n"+
				"	if (bary.y>=0.333)\n"+
				"		v.vertex = vi[0].vertex*(1-bary.x) + vi[1].vertex*bary.x;\n"+
				"	else\n"+
				"		v.vertex = vi[1].vertex*(1-bary.x) + vi[2].vertex*bary.x;\n";
		}
		return "";
	}
}
}