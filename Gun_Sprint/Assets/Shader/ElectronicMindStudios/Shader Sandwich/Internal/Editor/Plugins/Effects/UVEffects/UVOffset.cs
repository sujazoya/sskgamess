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
public class SSEUVOffset : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEUVOffset";
		Name = "Mapping/Offset";//+UnityEngine.Random.value.ToString();
		uvshader = "Hidden/ShaderSandwich/Previews/Effects/Mapping/Offset";
		
		Function = "";
		LinePre = "";
			Inputs = new SVDictionary();
			Inputs["X Offset"] = new ShaderVar("X Offset",0);
			Inputs["Y Offset"] = new ShaderVar("Y Offset",0);
			Inputs["Z Offset"] = new ShaderVar("Z Offset",0);
		Inputs["X Offset"].NoSlider = true;
		Inputs["Y Offset"].NoSlider = true;
		Inputs["Z Offset"].NoSlider = true;
		
		IsUVEffect = true;
	}
	public override string GenerateMap(SVDictionary Inputs, ShaderData SD, ShaderLayer SL, string Map, ref int UVDimensions, ref int TypeDimensions){
		/*if (TypeDimensions==3){
			if (UVDimensions == 1)
				Map = "float3("+Map+",0,0)";
			if (UVDimensions == 2)
				Map = "float3("+Map+",0)";
			UVDimensions = 3;
			return "("+Map+"+float3("+Inputs["X Offset"].Get()+","+Inputs["Y Offset"].Get()+","+Inputs["Z Offset"].Get()+"))";
		}
		if (TypeDimensions==2){
			if (UVDimensions == 1)
				Map = "float2("+Map+",0)";
			
			if (UVDimensions<3){
				UVDimensions = 2;
				return "("+Map+"+float2("+Inputs["X Offset"].Get()+","+Inputs["Y Offset"].Get()+"))";
			}
			else{
				return "("+Map+"+float3("+Inputs["X Offset"].Get()+","+Inputs["Y Offset"].Get()+",0))";
			}
		}
		if (TypeDimensions==1)
			return "("+Map+"+("+Inputs["X Offset"].Get()+"))";*/
		
		if (UVDimensions==1)
			return "("+Map+"+("+Inputs["X Offset"].Get()+"))";
		
		if (UVDimensions==2)
			return "("+Map+"+float2("+Inputs["X Offset"].Get()+","+Inputs["Y Offset"].Get()+"))";
		
		if (UVDimensions==3)
			return "("+Map+"+float3("+Inputs["X Offset"].Get()+","+Inputs["Y Offset"].Get()+","+Inputs["Z Offset"].Get()+"))";
		
		return Map;
	}
	public override Vector2 Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, int width, int height){
		UV.x+=Mathf.Round(Inputs["X Offset"].Float*width);
		UV.y+=Mathf.Round(Inputs["Y Offset"].Float*height);
		return UV;
	}
}
}