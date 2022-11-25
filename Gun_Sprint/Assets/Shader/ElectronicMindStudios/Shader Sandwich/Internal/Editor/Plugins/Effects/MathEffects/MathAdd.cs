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
public class SSEMathAdd : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEMathAdd";
		Name = "Maths/Add";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/Maths/Add";
		Function = "";
		LinePre = "";
		
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("Add",0));

		Inputs["Add"].NoSlider = true;
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "("+Line+"+"+Inputs["Add"].Get()+")";
	}
	public override string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "("+Line+"+"+Inputs["Add"].Get()+")";
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "("+Line+"+"+Inputs["Add"].Get()+")";
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		ShaderColor NewColor = new ShaderColor(OldColor.r+Inputs["Add"].Float,OldColor.g+Inputs["Add"].Float,OldColor.b+Inputs["Add"].Float,OldColor.a+Inputs["Add"].Float);
		return NewColor;
	}
}
}