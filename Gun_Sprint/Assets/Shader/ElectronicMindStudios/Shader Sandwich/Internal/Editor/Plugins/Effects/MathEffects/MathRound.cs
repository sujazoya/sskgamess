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
public class SSEMathRound : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEMathRound";
		Name = "Maths/Round";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/Maths/Round";
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("Split Number",1));
		Inputs["Split Number"].NoSlider = false;
		Inputs["Split Number"].Range0 = 1f;
		Inputs["Split Number"].Range1 = 40f;
		Inputs["Split Number"].Float = Mathf.Round(Inputs["Split Number"].Float);
		
		Function = "";
		LinePre = "";
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "(round("+Line+"*"+Inputs["Split Number"].Get()+")/"+Inputs["Split Number"].Get()+")";
	}
	public override string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "(round("+Line+"*"+Inputs["Split Number"].Get()+")/"+Inputs["Split Number"].Get()+")";
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "(round("+Line+"*"+Inputs["Split Number"].Get()+")/"+Inputs["Split Number"].Get()+")";
	}
	public static float Round(float f){
		return Mathf.Round(f);
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		ShaderColor NewColor = new ShaderColor(Round(OldColor.r*Inputs["Split Number"].Float)/Inputs["Split Number"].Float,Round(OldColor.g*Inputs["Split Number"].Float)/Inputs["Split Number"].Float,Round(OldColor.b*Inputs["Split Number"].Float)/Inputs["Split Number"].Float,Round(OldColor.a*Inputs["Split Number"].Float)/Inputs["Split Number"].Float);
		
		return NewColor;
	}
}
}