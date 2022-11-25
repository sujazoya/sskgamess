using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using SU = ElectronicMind.Sandwich.ShaderUtil;
using System.Xml.Serialization;
using System.Runtime.Serialization;
//using System.Xml;
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
[System.Serializable]
public class SSEContrast : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEContrast";
		Name = "Color/Contrast";
		shader = "Hidden/ShaderSandwich/Previews/Effects/Contrast";
		
		Line = "lerp(float3(0.5),@Line,@Arg1)";
		
		Inputs = new SVDictionary();//new Dictionary<string,ShaderVar>();
		Inputs["Contrast"] = new ShaderVar("Contrast",1);
			
		Inputs["Contrast"].Range0 = 0;
		Inputs["Contrast"].Range1 = 3;
	}
	public static float Lerp(float a, float b, float t){
		return a+((b-a)*t);
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["Contrast"].Get()=="1")
			return Line;
		
		return "lerp(float3(0.5,0.5,0.5),"+Line+","+Inputs["Contrast"].Get()+")";
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["Contrast"].Get()=="1")
			return Line;
		
		return "lerp(float4(0.5,0.5,0.5,0.5),"+Line+","+Inputs["Contrast"].Get()+")";
	}
	public override string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["Contrast"].Get()=="1")
			return Line;
		
		return "lerp(0.5,"+Line+","+Inputs["Contrast"].Get()+")";
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		ShaderColor Grey = new ShaderColor(0.5f,0.5f,0.5f,0.5f);
		ShaderColor NewColor = new ShaderColor(Lerp(Grey.r,OldColor.r,Inputs["Contrast"].Float),Lerp(Grey.g,OldColor.g,Inputs["Contrast"].Float),Lerp(Grey.b,OldColor.b,Inputs["Contrast"].Float),Lerp(Grey.a,OldColor.a,Inputs["Contrast"].Float));
		return NewColor;
	}
}
}