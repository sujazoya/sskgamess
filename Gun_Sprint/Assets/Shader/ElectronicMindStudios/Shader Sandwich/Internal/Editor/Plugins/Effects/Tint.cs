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
public class SSETint : ShaderEffect{
	public override void Activate(){
		TypeS = "SSETint";
		Name = "Color/Tint";
		shader = "Hidden/ShaderSandwich/Previews/Effects/Tint";
		
		Function = "";
		Line = "";
		LinePre = "";
		
			Inputs = new SVDictionary();
			Inputs["Tint"] = new ShaderVar("Tint",new Color(1f,0f,0f,1f));
			
		Inputs["Tint"].Range0 = 0;
		Inputs["Tint"].Range1 = 3;
	}
	public static float Lerp(float a, float b, float t){
		return a+((b-a)*t);
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "("+Line+"*"+Inputs["Tint"].Get()+")";
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "("+Line+"*"+Inputs["Tint"].Get()+")";
	}
	public override string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "("+Line+"*"+Inputs["Tint"].Get()+".a)";
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
//		ShaderColor Grey = new ShaderColor(0.5f,0.5f,0.5f,0.5f);
		ShaderColor NewColor = OldColor*Inputs["Tint"].Vector;//new ShaderColor(Lerp(Grey.r,OldColor.r,Inputs["Tint"].Float),Lerp(Grey.g,OldColor.g,Inputs["Tint"].Float),Lerp(Grey.b,OldColor.b,Inputs["Tint"].Float),Lerp(Grey.a,OldColor.a,Inputs["Tint"].Float));
		return NewColor;
	}
}
}