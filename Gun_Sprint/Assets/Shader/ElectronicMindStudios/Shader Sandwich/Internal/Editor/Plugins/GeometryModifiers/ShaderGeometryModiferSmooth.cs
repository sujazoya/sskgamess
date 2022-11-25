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
public class ShaderGeometryModifierSmooth : ShaderGeometryModifier{
	//public ShaderVar Type = new ShaderVar("Type",new string[]{"Quick","Quads","Tris"},new string[]{"Fast, but misses a few edges.","Show only quads (may still skip some edges, use with caution).","Shows all edges."},new string[]{"Quick","Quads","Tris"});
	public ShaderVar Smoothness = new ShaderVar("Smoothness",0.5f,-1f,1f);
	public override void GetMyShaderVars(List<ShaderVar> SVs){
		//SVs.Add(Type);
		SVs.Add(Smoothness);
	}
	new static public string MenuItem = "Displace/Smooth";
	public void OnEnable(){
		Name = "Smooth";
		MenuItem = "Displace/Smooth";
		
		ShaderGeometryModifierComponent = ShaderGeometryModifierComponent.Tessellation;
		ExistencePreferred = new Type[]{typeof(ShaderGeometryModifierTessellation)};
	}
	public override List<ShaderLayerList> GetMyShaderLayerLists(bool All){
		List<ShaderLayerList> SLLs = new List<ShaderLayerList>();
		return SLLs;
	}
	public override int GetHeight(float Width){
		return 25;
	}
	public override void RenderUI(float Width){
		//Type.Draw(new Rect(0,20,Width,20),"");
		GUI.Label(new Rect(0,20,Width,40),"Designed to be used with the Subdivision ingredient, won't do much without it :)");
		Smoothness.Draw(new Rect(0,50+20,Width,20),"Smoothness");
	}
	public override string GenerateDomainTransfer(ShaderData SD){
		//if (Type.Type==1){
		if (SD.TessDomain == TessDomain.Tri)
			return 
				"	float3 pp[3];\n"+
				"	for (int i = 0; i < 3; ++i)\n"+
				"		pp[i] = v.vertex.xyz - vi[i].normal * (dot(v.vertex.xyz, vi[i].normal) - dot(vi[i].vertex.xyz, vi[i].normal));\n"+
				"	v.vertex.xyz = "+Smoothness.Get()+" * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-("+Smoothness.Get()+")) * v.vertex.xyz;\n";
		else if (SD.TessDomain == TessDomain.Isoline)
			return 
				"	float3 pp[2];\n"+
				"	for (int i = 0; i < 2; ++i)\n"+
				"		pp[i] = v.vertex.xyz - vi[i].normal * (dot(v.vertex.xyz, vi[i].normal) - dot(vi[i].vertex.xyz, vi[i].normal));\n"+
				"	v.vertex.xyz = "+Smoothness.Get()+" * (pp[0]*bary.x + pp[1]*bary.y) + (1.0f-("+Smoothness.Get()+")) * v.vertex.xyz;\n";
		//}
		return "";
	}
}
}