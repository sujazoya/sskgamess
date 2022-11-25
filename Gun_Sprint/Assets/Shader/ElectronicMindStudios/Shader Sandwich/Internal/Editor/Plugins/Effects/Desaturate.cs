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
public class SSEDesaturate : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEDesaturate";
		Name = "Color/Desaturate";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/Desaturate";
		Function = "";
		Line = "float3(dot(float3(0.3,0.59,0.11),@Line))";
		LinePre = "";
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "float3(dot(float3(0.3,0.59,0.11),"+Line+".rgb),dot(float3(0.3,0.59,0.11),"+Line+".rgb),dot(float3(0.3,0.59,0.11),"+Line+".rgb))";
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		float Grey = Vector3.Dot(new Vector3(0.3f,0.59f,0.11f),new Vector3(OldColor.r,OldColor.g,OldColor.b));
		ShaderColor NewColor = new ShaderColor(Grey,Grey,Grey,OldColor.a);
		//NewColor.a = OldColor.a;
		return NewColor;
	}
}
}