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
public class SSEGamma : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEGamma";
		Name = "Color/Gamma";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/Gamma";
		Function = "";
		LinePre = "";
		
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("Gamma",2.2f));
			Inputs.Add(new ShaderVar("Luminance Only",false));

		Inputs["Gamma"].Range0 = 0.0001f;
		Inputs["Gamma"].Range1 = 4;
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["Luminance Only"].On)
			return "("+Line+"/max(max("+Line+".r,"+Line+".g),"+Line+".b)) * pow(max(max("+Line+".r,"+Line+".g),"+Line+".b),"+Inputs["Gamma"].Get()+")";
		else
			return "pow("+Line+","+Inputs["Gamma"].Get()+")";
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["Luminance Only"].On)
			return "half4(("+Line+".rgb/max(max("+Line+".r,"+Line+".g),"+Line+".b)) * pow(max(max("+Line+".r,"+Line+".g),"+Line+".b),"+Inputs["Gamma"].Get()+"),"+Line+".a)";
		else
			return "half4(pow("+Line+".rgb,"+Inputs["Gamma"].Get()+"),"+Line+".a)";
	}
	public override string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return Line;
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		ShaderColor NewColor = new ShaderColor(Mathf.Pow(OldColor.r,Inputs["Gamma"].Float),Mathf.Pow(OldColor.g,Inputs["Gamma"].Float),Mathf.Pow(OldColor.b,Inputs["Gamma"].Float),Mathf.Pow(OldColor.a,Inputs["Gamma"].Float));
		return NewColor;
	}
}
}