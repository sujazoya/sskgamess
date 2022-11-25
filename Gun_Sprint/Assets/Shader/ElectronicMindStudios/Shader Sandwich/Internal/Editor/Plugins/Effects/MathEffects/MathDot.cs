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
public class SSEMathDot : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEMathDot";
		Name = "Maths/Dot";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/Maths/Dot";
		Function = "";
		LinePre = "";
		
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("R",0.333f));
			Inputs.Add(new ShaderVar("G",0.333f));
			Inputs.Add(new ShaderVar("B",0.333f));
			Inputs.Add(new ShaderVar("A",0.333f));

		Inputs["R"].NoSlider = true;
		Inputs["G"].NoSlider = true;
		Inputs["B"].NoSlider = true;
		Inputs["A"].NoSlider = true;
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		string a= "dot("+Line+",float3("+Inputs["R"].Get()+","+Inputs["G"].Get()+","+Inputs["B"].Get()+"))";
		return "float4("+a+","+a+","+a+","+a+")";
	}
	public override string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		string a= "dot("+Line+","+Inputs["R"].Get()+")";
		return "float4("+a+","+a+","+a+","+a+")";
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		string a= "dot("+Line+",float3("+Inputs["R"].Get()+","+Inputs["G"].Get()+","+Inputs["B"].Get()+","+Inputs["A"].Get()+"))";
		return "float4("+a+","+a+","+a+","+a+")";
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		ShaderColor NewColor = OldColor*new ShaderColor(Inputs["R"].Float,Inputs["G"].Float,Inputs["B"].Float,Inputs["A"].Float);// = new ShaderColor(OldColor.r+Inputs["R"].Float,OldColor.g+Inputs["R"].Float,OldColor.b+Inputs["R"].Float,OldColor.a+Inputs["R"].Float);
		float Grey = NewColor.r+NewColor.g+NewColor.b;
		if (UseAlpha.Float==2f)
		Grey = NewColor.a;
		if (UseAlpha.Float==1f)
		Grey = NewColor.r+NewColor.g+NewColor.b+NewColor.a;
		NewColor = new ShaderColor(Grey,Grey,Grey,Grey);
		return NewColor;
	}
}
}