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
public class SSEThreshold : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEThreshold";
		Name = "Color/Threshold";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/Threshold";
		
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("Threshold",0.3f));
		Inputs["Threshold"].Range0 = 0;
		Inputs["Threshold"].Range1 = 1;
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		float Grey = Vector3.Dot(new Vector3(0.3f,0.59f,0.11f),new Vector3(OldColor.r,OldColor.g,OldColor.b));
		ShaderColor NewColor = OldColor;
		if (Grey<Inputs["Threshold"].Float)
		NewColor = new ShaderColor(0,0,0,OldColor.a);
		if (NewColor.a<Inputs["Threshold"].Float)
		NewColor.a = 0;
		return NewColor;
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		//return "lerp(float3(0,0,0),"+Line+",ceil(dot(float3(0.3,0.59,0.11),"+Line+")-"+Inputs["Threshold"].Get()+"))";
		return Line+" * ceil(saturate(dot(float3(0.3,0.59,0.11),"+Line+")-"+Inputs["Threshold"].Get()+"))";
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		//return "lerp(float4(0,0,0,0),"+Line+",ceil(dot(float3(0.3,0.59,0.11),"+Line+".rgb)-"+Inputs["Threshold"].Get()+")*ceil("+Line+".a-"+Inputs["Threshold"].Get()+"))";
		//return "lerp(float4(0,0,0,0),"+Line+",ceil(dot(float3(0.3,0.59,0.11),"+Line+".rgb)-"+Inputs["Threshold"].Get()+")*ceil("+Line+".a-"+Inputs["Threshold"].Get()+"))";
		//return "lerp(float4(0,0,0,0),"+Line+",ceil(dot(float3(0.3,0.59,0.11),"+Line+".rgb)-"+Inputs["Threshold"].Get()+")*ceil("+Line+".a-"+Inputs["Threshold"].Get()+"))";
		return "float4("+Line+".rgb * ceil(saturate(dot(float3(0.3,0.59,0.11),"+Line+")-"+Inputs["Threshold"].Get()+")),"+Line+".a * ceil(saturate("+Line+".a-"+Inputs["Threshold"].Get()+")))";
		///TOoDO: Wanted to fix like the others but I have no idea what this one does...
	}
	public override string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		//return "lerp(0,"+Line+",ceil("+Line+"-"+Inputs["Threshold"].Get()+"))";
		return Line+" * ceil(saturate("+Line+"-"+Inputs["Threshold"].Get()+"))";
	}
}
}