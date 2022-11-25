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
public class SSEInvert : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEInvert";
		Name = "Color/Invert";
		shader = "Hidden/ShaderSandwich/Previews/Effects/Invert";
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "(float3(1,1,1)-"+Line+")";
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "(float4(1,1,1,1)-"+Line+")";
	}
	public override string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "(1-"+Line+")";
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		ShaderColor NewColor = new ShaderColor(1f-OldColor.r,1f-OldColor.g,1f-OldColor.b,1f-OldColor.a);
		return NewColor;
	}
}
}