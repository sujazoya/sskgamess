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
public class ShaderGeometryModifierSet : ShaderGeometryModifier{
	public ShaderVar SetWhatAaahh = new ShaderVar("SetWhatAaahh",new string[] {"Local Position","World Position"});

	public override void GetMyShaderVars(List<ShaderVar> SVs){
		SVs.Add(SetWhatAaahh);
	}
	new static public string MenuItem = "Displace/Set";
	public void OnEnable(){
		Name = "Set";
		MenuItem = "Displace/Set";
		SurfaceLayers.Description = "The new vertex positions.";
		SurfaceLayers.Name.Text = "Vertex";
		SurfaceLayers.BaseColor = new Color(1f,1f,1f,1);
		SurfaceLayers.EndTag.Text = "rgb";
	}
	public override int GetHeight(float Width){
		return 20+45+25+25;
	}
	public override void RenderUI(float Width){
		int y = 20;
		SetWhatAaahh.TypeDispL = 2;
		SetWhatAaahh.Draw(new Rect(0,y,Width,20),"SetWhatAaahh");y+=25;
	}
	public override string GenerateVertex(ShaderData SD){
		string DispCode;
		if (SetWhatAaahh.Type==1)
			SD.CoordSpace = CoordSpace.World;
		else
			SD.CoordSpace = CoordSpace.Object;
		if (SetWhatAaahh.Type==1)
			DispCode = "float3 Surface = vd.worldPos;\n";
		else
			DispCode = "float3 Surface = v.vertex.xyz;\n";
		
		DispCode += SurfaceLayers.GenerateCode(SD,false);
		
		string Surface = "Surface";
		//bool local = true;
		//if (Direction.Type>=2)
		//	local = false;
		if (SD.CoordSpace == CoordSpace.Object)
			DispCode += "\nv.vertex.xyz = "+Surface+";\n";
		else
			DispCode += "\nvd.worldPos = "+Surface+";\n";
		return DispCode;
	}
}
}