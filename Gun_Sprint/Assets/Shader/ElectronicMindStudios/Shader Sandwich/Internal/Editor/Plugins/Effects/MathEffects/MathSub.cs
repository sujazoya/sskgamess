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
public class SSEMathSub : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEMathSub";
		Name = "Maths/Subtract";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/Maths/Subtract";
		Function = "";
		LinePre = "";
		
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("Subtract",0));

		Inputs["Subtract"].NoSlider = true;


	
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "("+Line+"-("+Inputs["Subtract"].Get()+"))";
	}
	public override string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "("+Line+"-("+Inputs["Subtract"].Get()+"))";
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "("+Line+"-("+Inputs["Subtract"].Get()+"))";
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		ShaderColor NewColor = new ShaderColor(OldColor.r-Inputs["Subtract"].Float,OldColor.g-Inputs["Subtract"].Float,OldColor.b-Inputs["Subtract"].Float,OldColor.a-Inputs["Subtract"].Float);
		return NewColor;
	}
}
}