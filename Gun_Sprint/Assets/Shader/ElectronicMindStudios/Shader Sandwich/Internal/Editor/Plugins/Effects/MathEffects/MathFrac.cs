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
public class SSEMathFrac : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEMathFrac";
		Name = "Maths/Fractional Bit";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/Maths/Frac";
		Function = "";
		LinePre = "";
		
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("Split Number",1));

		Inputs["Split Number"].NoSlider = false;
		Inputs["Split Number"].Range0 = 1f;
		Inputs["Split Number"].Range1 = 20f;
		Inputs["Split Number"].Float = Mathf.Round(Inputs["Split Number"].Float);
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		Inputs["Split Number"].Float = Mathf.Round(Inputs["Split Number"].Float);
		if (Inputs["Split Number"].Get()=="0")
		return "fmod("+Line+",1)";

		return "fmod("+Line+"*"+Inputs["Split Number"].Get()+",1)";
	}
	public override string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		Inputs["Split Number"].Float = Mathf.Round(Inputs["Split Number"].Float);
		if (Inputs["Split Number"].Get()=="0")
		return "fmod("+Line+",1)";

		return "fmod("+Line+"*"+Inputs["Split Number"].Get()+",1)";
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		Inputs["Split Number"].Float = Mathf.Round(Inputs["Split Number"].Float);
		if (Inputs["Split Number"].Get()=="0")
		return "fmod("+Line+",1)";

		return "fmod("+Line+"*"+Inputs["Split Number"].Get()+",1)";
	}
	public static float Frac(float f){
		return f-Mathf.Floor(f);
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		Inputs["Split Number"].Float = Mathf.Round(Inputs["Split Number"].Float);
		ShaderColor NewColor = new ShaderColor(Frac(OldColor.r*Inputs["Split Number"].Float),Frac(OldColor.g*Inputs["Split Number"].Float),Frac(OldColor.b*Inputs["Split Number"].Float),Frac(OldColor.a*Inputs["Split Number"].Float));
		
		return NewColor;
	}
}
}