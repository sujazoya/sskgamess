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
public class SSEMathMul : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEMathMul";
		Name = "Maths/Multiply";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/Maths/Mul";
		
		Function = "";
		LinePre = "";
		
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("Multiply",1));

		Inputs["Multiply"].NoSlider = true;		
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "("+Line+"*"+Inputs["Multiply"].Get()+")";
	}
	public override string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "("+Line+"*"+Inputs["Multiply"].Get()+")";
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "("+Line+"*"+Inputs["Multiply"].Get()+")";
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		ShaderColor NewColor = new ShaderColor(OldColor.r*Inputs["Multiply"].Float,OldColor.g*Inputs["Multiply"].Float,OldColor.b*Inputs["Multiply"].Float,OldColor.a*Inputs["Multiply"].Float);
		return NewColor;
	}
}
}