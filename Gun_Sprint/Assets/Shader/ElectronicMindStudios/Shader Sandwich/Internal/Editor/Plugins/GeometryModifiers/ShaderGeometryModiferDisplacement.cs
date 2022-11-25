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
public class ShaderGeometryModifierDisplacement : ShaderGeometryModifier{
	public ShaderVar Direction = new ShaderVar("Direction",new string[] {"Normal","Tangent","XYZ", "X","Y","Z"});
	
	public ShaderVar CoordinateSpace = new ShaderVar("Coordinate Space",new string[] {"Object","World"});
	public ShaderVar IndependentXYZ = new ShaderVar("Independent XYZ",false);
	public ShaderVar Strength = new ShaderVar("Strength",1f);
	public ShaderVar MidLevel = new ShaderVar("MidLevel",0f);

	public override void GetMyShaderVars(List<ShaderVar> SVs){
		SVs.Add(Direction);
		SVs.Add(CoordinateSpace);
		SVs.Add(Strength);
		SVs.Add(MidLevel);
		SVs.Add(IndependentXYZ);
	}
	new static public string MenuItem = "Displace/Displace";
	public void OnEnable(){
		Name = "Displace";
		MenuItem = "Displace/Displace";
		SurfaceLayers.Description = "The amount of displacement.";
		SurfaceLayers.Name.Text = "Displacement";
		SurfaceLayers.BaseColor = new Color(1f,1f,1f,1);
	}
	public override int GetHeight(float Width){
		return 20+45+25+25;
	}
	public override void RenderUI(float Width){
		int y = 20;
		Strength.Range0 = -2f;
		Strength.Range1 = 2f;
		
		Direction.TypeDispL = 3;
		Direction.Draw(new Rect(0,y,Width,20),"Direction");y+=45;
		CoordinateSpace.Draw(new Rect(0,y,Width,20),"Coordinate Space");y+=25;
		Strength.Draw(new Rect(0,y,Width,20),"Strength");y+=25;
		MidLevel.Draw(new Rect(0,y,Width,20),"Middle Value");y+=25;
		if (Direction.Type<2)
			IndependentXYZ.Draw(new Rect(0,y,Width,20),"Independent XYZ");y+=25;
		
		if (Direction.Type==2||(IndependentXYZ.On&&Direction.Type<2))
			SurfaceLayers.EndTag.Text = "rgb";
		else if (SurfaceLayers.EndTag.Text.Length!=1)
			SurfaceLayers.EndTag.Text = "r";
	}
	public override string GenerateVertex(ShaderData SD){
		///if (Direction.Type>=2)
		///	SD.CoordSpace = CoordSpace.World;
		if (CoordinateSpace.Type==0)
			SD.CoordSpace = CoordSpace.Object;
		if (CoordinateSpace.Type==1)
			SD.CoordSpace = CoordSpace.World;
		//else
		//	SD.CoordSpace = CoordSpace.Object;
		if (Direction.Type==2||(IndependentXYZ.On&&Direction.Type<2))
			SurfaceLayers.EndTag.Text = "rgb";
		else if (SurfaceLayers.EndTag.Text.Length!=1)
			SurfaceLayers.EndTag.Text = "r";
		string DispCode;
		if (Direction.Type==2||(IndependentXYZ.On&&Direction.Type<2))
			DispCode = "half3 Surface = half3(1,1,1);\n";
		else
			DispCode = "half Surface = 1;\n";
		
		DispCode += SurfaceLayers.GenerateCode(SD,false);
		
		string Vec = "";
		if (SD.CoordSpace == CoordSpace.Object){
			if (Direction.Type==0)Vec = "v.normal";
			if (Direction.Type==1)Vec = "v.tangent";
		}else{
			if (Direction.Type==0)Vec = "vd.worldNormal";
			if (Direction.Type==1)Vec = "vd.worldTangent";
		}
		if (Direction.Type==2)Vec = "";
		if (Direction.Type==3)Vec = "float3(1,0,0)";
		if (Direction.Type==4)Vec = "float3(0,1,0)";
		if (Direction.Type==5)Vec = "float3(0,0,1)";
		
		if (Direction.Type!=2){
			//if (!MidLevel.Safe()){
			//	Vec = "(("+Vec+") - "+MidLevel.Get()+") * ";
			//}else{
				Vec = Vec + " * ";
			//}
		}
		string Surface = "Surface";
		if (!MidLevel.Safe()){
			Surface = "("+Surface+" - ("+MidLevel.Get()+"))";
		}
		//bool local = true;
		//if (Direction.Type>=2)
		//	local = false;
		if (SD.CoordSpace == CoordSpace.Object)
			DispCode += "\nv.vertex.xyz = float3(v.vertex.xyz + "+Vec+Surface+" * "+Strength.Get()+");\n";
		else
			DispCode += "\nvd.worldPos = float3(vd.worldPos + "+Vec+Surface+" * "+Strength.Get()+");\n";
		return DispCode;
	}
}
}