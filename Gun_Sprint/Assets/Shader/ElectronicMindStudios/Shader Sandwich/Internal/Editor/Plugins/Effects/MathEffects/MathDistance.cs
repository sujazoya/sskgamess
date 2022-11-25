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
public class SSEMathDistance : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEMathDistance";
		Name = "Maths/Distance";//+UnityEngine.Random.value.ToString();
		shader="Hidden/ShaderSandwich/Previews/Effects/Maths/Distance";
		Function = "";
		LinePre = "";
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("Dimensions",new string[]{"1D","2D","3D","4D"},new string[]{"","","",""},new string[]{"1D","2D","3D","4D"}));
			Inputs.Add(new ShaderVar("X",0));
			Inputs.Add(new ShaderVar("Y",0));
			Inputs.Add(new ShaderVar("Z",0));
			Inputs.Add(new ShaderVar("W",0));

		Inputs["X"].NoSlider = true;
		Inputs["Y"].NoSlider = true;
		Inputs["Z"].NoSlider = true;
		Inputs["W"].NoSlider = true;
		//if (Inputs["Dimensions"].Type==3)
		//UseAlpha.Float=1f;
		//if (UseAlpha.Float==2f)
		//UseAlpha.Float=0f;
WantsFullLine = true;

	
	}
	public string GenerateGeneric(string line){
		string Grey = line;
		if (Inputs["Dimensions"].Type==0)
		Grey = line+".r-"+Inputs["X"].Get();
		if (Inputs["Dimensions"].Type==1)
		Grey = "distance("+line+".xy,float2("+Inputs["X"].Get()+","+Inputs["Y"].Get()+"))";
		if (Inputs["Dimensions"].Type==2)
		Grey = "distance("+line+".xyz,float3("+Inputs["X"].Get()+","+Inputs["Y"].Get()+","+Inputs["Z"].Get()+"))";
		if (Inputs["Dimensions"].Type==3)
		Grey = "distance("+line+",float4("+Inputs["X"].Get()+","+Inputs["Y"].Get()+","+Inputs["Z"].Get()+","+Inputs["W"].Get()+"))";
		return "(("+Grey+").rrrr)";	
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return GenerateGeneric(Line);
	}
	public override string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return GenerateGeneric(Line);
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return GenerateGeneric(Line);
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		//ShaderColor NewColor = OldColor*new ShaderColor(Inputs["Dimensions"].Float,Inputs["X"].Float,Inputs["Y"].Float,Inputs["Z"].Float);// = new ShaderColor(OldColor.r+Inputs["Dimensions"].Float,OldColor.g+Inputs["Dimensions"].Float,OldColor.b+Inputs["Dimensions"].Float,OldColor.a+Inputs["Dimensions"].Float);
		float Grey = 0;
		if (Inputs["Dimensions"].Type==0)
		Grey = OldColor.r-Inputs["X"].Float;
		if (Inputs["Dimensions"].Type==1)
		Grey = Vector2.Distance(new Vector2(OldColor.r,OldColor.g),new Vector2(Inputs["X"].Float,Inputs["Y"].Float));
		if (Inputs["Dimensions"].Type==2)
		Grey = Vector3.Distance(new Vector3(OldColor.r,OldColor.g,OldColor.b),new Vector3(Inputs["X"].Float,Inputs["Y"].Float,Inputs["Z"].Float));
		if (Inputs["Dimensions"].Type==3)
		Grey = Vector4.Distance(new Vector4(OldColor.r,OldColor.g,OldColor.b,OldColor.a),new Vector4(Inputs["X"].Float,Inputs["Y"].Float,Inputs["Z"].Float,Inputs["W"].Float));
		
		
		ShaderColor NewColor = new ShaderColor(Grey,Grey,Grey,Grey);
		return NewColor;
	}
}
}