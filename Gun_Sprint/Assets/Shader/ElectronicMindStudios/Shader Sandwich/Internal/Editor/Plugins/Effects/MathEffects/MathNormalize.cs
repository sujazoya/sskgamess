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
public class SSENormalize : ShaderEffect{
	public override void Activate(){
		TypeS = "SSENormalize";
		Name = "Maths/Normalize";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/Maths/Normalize";
		Function = "";
		LinePre = "";
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "(normalize("+Line+"))";
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "(normalize("+Line+"))";
	}
	public override string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "("+Line+")";
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		//rsqrt(dot(v,v))*v
		ShaderColor NewColor = new ShaderColor(OldColor.r,OldColor.g,OldColor.b,OldColor.a);
		float dot;
		if (UseAlpha.Float==2f)
		dot = OldColor.a*OldColor.a;
		else
		if (UseAlpha.Float==1f)
		dot = OldColor.r*OldColor.r+OldColor.g*OldColor.g+OldColor.b*OldColor.b+OldColor.a*OldColor.a;
		else
		dot = OldColor.r*OldColor.r+OldColor.g*OldColor.g+OldColor.b*OldColor.b;
		
		dot = Mathf.Pow(dot,-0.5f);
		NewColor *= dot;
		return NewColor;
	}
}
}