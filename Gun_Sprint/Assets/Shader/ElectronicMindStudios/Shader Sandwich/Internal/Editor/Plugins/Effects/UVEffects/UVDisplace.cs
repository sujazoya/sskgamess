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
public class SSEUVDisplace : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEUVDisplace";
		Name = "Mapping/Displace";//+UnityEngine.Random.value.ToString();
		uvshader = "Hidden/ShaderSandwich/Previews/Effects/Mapping/UVDisplace";
		
		Function = "";
		LinePre = "";
			Inputs = new SVDictionary();
			Inputs["Displace Mask"] = (new ShaderVar("Displace Mask (RGBA)","ListOfObjects"));
			Inputs.Add(new ShaderVar("Displace Amount",0));
			Inputs.Add(new ShaderVar("Displace Middle",0.5f));

		
		Inputs["Displace Mask"].NoInputs = true;
		Inputs["Displace Mask"].RGBAMasks = true;
		//if (BInputs)
		//	Inputs["Displace Mask"].HookIntoObjects();	
		Inputs["Displace Amount"].NoSlider = true;
		Inputs["Displace Middle"].Range0 = 0f;
		Inputs["Displace Middle"].Range1 = 1f;

	}
	//public static void SetUsed(ShaderData SD,ShaderEffect SE){
	//Debug.Log(SG.UsedMasks.ContainsKey((ShaderLayerList)Inputs["Displace Mask"].Obj));
		//if (Inputs["Displace Mask"].Obj!=null&&SG.UsedMasks.ContainsKey((ShaderLayerList)Inputs["Displace Mask"].Obj)){
		//SD.UsedMasks[(ShaderLayerList)Inputs["Displace Mask"].Obj]++;
//		Debug.Log(SG.UsedMasks[(ShaderLayerList)Inputs["Displace Mask"].Obj]);
		//}
	//}
	public override string GenerateMap(SVDictionary Inputs, ShaderData SD, ShaderLayer SL, string Map, ref int UVDimensions, ref int TypeDimensions){
		if (TypeDimensions==3){
			if (UVDimensions == 1)
				Map = "float3("+Map+",0,0)";
			if (UVDimensions == 2)
				Map = "float3("+Map+",0)";
			UVDimensions = 3;
			return "("+Map+"+((float3("+Inputs["Displace Mask"].GetMaskName()+".r,"+Inputs["Displace Mask"].GetMaskName()+".g,"+Inputs["Displace Mask"].GetMaskName()+".b)-"+Inputs["Displace Middle"].Get()+")*"+Inputs["Displace Amount"].Get()+"))";
		}
		if (TypeDimensions==2){
			if (UVDimensions == 1)
				Map = "float2("+Map+",0)";
			
			if (UVDimensions<3){
				UVDimensions = 2;
				return "("+Map+"+((float2("+Inputs["Displace Mask"].GetMaskName()+".r,"+Inputs["Displace Mask"].GetMaskName()+".g)-"+Inputs["Displace Middle"].Get()+")*"+Inputs["Displace Amount"].Get()+"))";
			}
			else{
				return "("+Map+"+((float3("+Inputs["Displace Mask"].GetMaskName()+".r,"+Inputs["Displace Mask"].GetMaskName()+".g,0)-"+Inputs["Displace Middle"].Get()+")*"+Inputs["Displace Amount"].Get()+"))";
			}
		}
		if (TypeDimensions==1){
			//if (UVDimensions==1)
			return "("+Map+"+(("+Inputs["Displace Mask"].GetMaskName()+".r-"+Inputs["Displace Middle"].Get()+")*"+Inputs["Displace Amount"].Get()+"))";
		}
		return Map;
	}
	public override Vector2 Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, int width, int height){
		Color Disp = new Color(0.5f,0.5f,0.5f,0.5f);
		if (Inputs["Displace Mask"].Obj!=null)
		Disp = ((Texture2D)((ShaderLayerList)(Inputs["Displace Mask"].Obj)).GetIcon()).GetPixel((int)UV.x,(int)UV.y);
		
		UV.x+=(Disp.r-Inputs["Displace Middle"].Float)*width*Inputs["Displace Amount"].Float;//Mathf.Round(Inputs["Displace Mask"].Float*width);
		UV.y+=(Disp.g-Inputs["Displace Middle"].Float)*height*Inputs["Displace Amount"].Float;//Mathf.Round(Inputs["Displace Amount"].Float*height);
		return UV;
	}
}
}