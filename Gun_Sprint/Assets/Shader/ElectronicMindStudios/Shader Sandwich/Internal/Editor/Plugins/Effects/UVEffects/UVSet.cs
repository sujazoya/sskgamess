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
public class SSEUVSet : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEUVSet";
		Name = "Mapping/Set";//+UnityEngine.Random.value.ToString();
		uvshader = "Hidden/ShaderSandwich/Previews/Effects/Mapping/UVSet";
		Function = "";
		LinePre = "";
			Inputs = new SVDictionary();
			Inputs["Values"] = (new ShaderVar("Values (RGBA)","ListOfObjects"));
			Inputs.Add(new ShaderVar("Scale",1));
		
		//Inputs["Values"].SetToMasks(null,0);

		Inputs["Values"].NoInputs = true;
		Inputs["Values"].RGBAMasks = true;
		Inputs["Values"].HookIntoObjects();		
		Inputs["Scale"].NoSlider = true;
		
		IsUVEffect = true;
	}
	public override string GenerateMap(SVDictionary Inputs, ShaderData SD, ShaderLayer SL, string Map, ref int UVDimensions, ref int TypeDimensions){
		if (TypeDimensions==3){
			if (UVDimensions == 1)
				Map = "float3("+Map+",0,0)";
			if (UVDimensions == 2)
				Map = "float3("+Map+",0)";
			UVDimensions = 3;
			return "(((float3("+Inputs["Values"].GetMaskName()+".r,"+Inputs["Values"].GetMaskName()+".g,"+Inputs["Values"].GetMaskName()+".b))*"+Inputs["Scale"].Get()+"))";
		}
		if (TypeDimensions==2){
			if (UVDimensions == 1)
				Map = "float2("+Map+",0)";
			
			if (UVDimensions<3){
				UVDimensions = 2;
				return "(((float2("+Inputs["Values"].GetMaskName()+".r,"+Inputs["Values"].GetMaskName()+".g))*"+Inputs["Scale"].Get()+"))";
			}
			else{
				return "(((float3("+Inputs["Values"].GetMaskName()+".r,"+Inputs["Values"].GetMaskName()+".g,0))*"+Inputs["Scale"].Get()+"))";
			}
		}
		if (TypeDimensions==1){
			return "((("+Inputs["Values"].GetMaskName()+".r)*"+Inputs["Scale"].Get()+"))";
		}
		return Map;
	}
	public override Vector2 Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, int width, int height){
		Color Disp = new Color(0.5f,0.5f,0.5f,0.5f);
		if (Inputs["Values"].Obj!=null&&(((ShaderLayerList)(Inputs["Values"].Obj)).GetIcon() as Texture2D !=null))
		Disp = (((ShaderLayerList)(Inputs["Values"].Obj)).GetIcon() as Texture2D).GetPixel((int)UV.x,(int)UV.y);
		
		UV.x=(Disp.r)*width*Inputs["Scale"].Float;
		UV.y=(Disp.g)*height*Inputs["Scale"].Float;
		return UV;
	}
}
}