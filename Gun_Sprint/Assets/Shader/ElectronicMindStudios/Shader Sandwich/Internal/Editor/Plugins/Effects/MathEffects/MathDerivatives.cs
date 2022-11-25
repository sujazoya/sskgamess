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
public class SSEMathDerivatives : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEMathDerivatives";
		Name = "Maths/Derivatives";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/Maths/Derivatives";
		Function = "";
		LinePre = "";
		
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("Derivative",new string[]{"x","y", "abs(x)+abs(y)"}));
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["Derivative"].Type==0)
			return "ddx("+Line+")";
		if (Inputs["Derivative"].Type==1)
			return "ddy("+Line+")";
		if (Inputs["Derivative"].Type==2)
			return "fwidth("+Line+")";
		return Line;
	}
	public override string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["Derivative"].Type==0)
			return "ddx("+Line+")";
		if (Inputs["Derivative"].Type==1)
			return "ddy("+Line+")";
		if (Inputs["Derivative"].Type==2)
			return "fwidth("+Line+")";
		return Line;
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["Derivative"].Type==0)
			return "ddx("+Line+")";
		if (Inputs["Derivative"].Type==1)
			return "ddy("+Line+")";
		if (Inputs["Derivative"].Type==2)
			return "fwidth("+Line+")";
		return Line;
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		ShaderColor NewColor = new ShaderColor(OldColor.r/Inputs["Divide"].Float,OldColor.g/Inputs["Divide"].Float,OldColor.b/Inputs["Divide"].Float,OldColor.a/Inputs["Divide"].Float);
		return NewColor;
	}
}
}