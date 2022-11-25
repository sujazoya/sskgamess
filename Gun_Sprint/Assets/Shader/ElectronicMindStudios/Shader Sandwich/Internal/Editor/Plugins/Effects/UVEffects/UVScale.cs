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
public class SSEUVScale : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEUVScale";
		Name = "Mapping/Scale";//+UnityEngine.Random.value.ToString();
		uvshader = "Hidden/ShaderSandwich/Previews/Effects/Mapping/Scale";
		Function = "";
		LinePre = "";
		
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("Seperate",true));
			Inputs["Scale"] = new ShaderVar("X Scale",1);
			Inputs["X Scale"] = Inputs["Scale"];
			Inputs.Add(new ShaderVar("Y Scale",1));
			Inputs.Add(new ShaderVar("Z Scale",1));

		Inputs["X Scale"].NoSlider = true;
		Inputs["Y Scale"].NoSlider = true;
		Inputs["Z Scale"].NoSlider = true;
		
		IsUVEffect = true;
	}
	public override string GenerateMap(SVDictionary Inputs, ShaderData SD, ShaderLayer SL, string Map, ref int UVDimensions, ref int TypeDimensions){
		Inputs.Remove("Scale");
		if (!Inputs["Seperate"].On){
			if (UVDimensions==3)
			return "("+Map+"*float3("+Inputs["X Scale"].Get()+","+Inputs["X Scale"].Get()+","+Inputs["X Scale"].Get()+"))";
			if (UVDimensions==2)
			return "("+Map+"*float2("+Inputs["X Scale"].Get()+","+Inputs["X Scale"].Get()+"))";
			if (UVDimensions==1)
			return "("+Map+"*("+Inputs["X Scale"].Get()+"))";
		}
		else{
			if (UVDimensions==3)
			return "("+Map+"*float3("+Inputs["X Scale"].Get()+","+Inputs["Y Scale"].Get()+","+Inputs["Z Scale"].Get()+"))";
			if (UVDimensions==2)
			return "("+Map+"*float2("+Inputs["X Scale"].Get()+","+Inputs["Y Scale"].Get()+"))";
			if (UVDimensions==1)
			return "("+Map+"*("+Inputs["X Scale"].Get()+"))";
		}
		return Map;
	}
	public override bool DrawJustAfter(Rect rect){
		Inputs.Remove("Scale");
		if (!Inputs["Seperate"].On){
			Inputs["Y Scale"].Hidden = true;
			Inputs["Y Scale"].Use = false;
			Inputs["Z Scale"].Hidden = true;
			Inputs["Z Scale"].Use = false;
			Inputs["X Scale"].Name = "Scale";
			Inputs["Y Scale"].Float = Inputs["X Scale"].Float;
			Inputs["Y Scale"].Input = Inputs["X Scale"].Input;
			Inputs["Z Scale"].Float = Inputs["X Scale"].Float;
			Inputs["Z Scale"].Input = Inputs["X Scale"].Input;
		}
		else{
			Inputs["X Scale"].Name = "X Scale";
			Inputs["Y Scale"].Hidden = false;
			Inputs["Y Scale"].Use = true;
			Inputs["Z Scale"].Hidden = false;
			Inputs["Z Scale"].Use = true;
		}
		return false;
	}
	public override Vector2 Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, int width, int height){
		Inputs.Remove("Scale");
		if (!Inputs["Seperate"].On)
		Inputs["Y Scale"].Float = Inputs["X Scale"].Float;
		if (!Inputs["Seperate"].On)
		Inputs["Z Scale"].Float = Inputs["X Scale"].Float;
		UV.x*=Inputs["X Scale"].Float;
		UV.y*=Inputs["Y Scale"].Float;
		return UV;
	}
}
}