using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
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
public class SLTDepthPass : ShaderLayerType{
	override public void Activate(){
		TypeS = "SLTDepthPass";
		Name = "Screen/Behind Depth";
		MenuIndex = 21;
		Description = "The screen depth before any objects with this shader is rendered.";
		//Function = "//Setup inputs for the depth texture.\n"+
		//			"sampler2D_float _CameraDepthTexture;\n";
		shader = "Hidden/ShaderSandwich/Previews/Types/DepthPass";
	}
	public Color[] Image;
	override public SVDictionary GetInputs(){
		Inputs = new SVDictionary();
		Inputs["Linearize"] = new ShaderVar("Linearize",true);
		return Inputs;
	}
	override public string Generate(SVDictionary Data, ShaderLayer SL, ShaderData SD, string Map){
		SD.BlendsWeirdlyAnywhere = true;
		SD.AdditionalFunctions.Add("	float2 SillyFlip(float2 inUV){\n"+
		"		#if UNITY_UV_STARTS_AT_TOP\n"+
		"			if (_ProjectionParams.x > 0);\n"+
		"				inUV.y = 1-inUV.y;\n"+
		"		#endif\n"+
		"		inUV.y = 1-inUV.y;///TODO: Make this function work...\n"+
		"		return inUV;\n"+
		"	}\n");
		
		SD.AdditionalFunctions.Add(
		"		//Setup inputs for the depth texture.\n"+
		"		sampler2D_float _CameraDepthTexture;\n");
		if (Data["Linearize"].On)
			return "(LinearEyeDepth(tex2D(_CameraDepthTexture, SillyFlip("+Map+")).r).rrrr)";
		else
			return "(tex2D(_CameraDepthTexture, SillyFlip("+Map+")).rrrr)";
	}
	override public int GetDimensions(SVDictionary Data, ShaderLayer SL){
		return 2;
	}
	override public ShaderColor Preview(SVDictionary Data, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		return ShaderColor.Lerp(new ShaderColor(0,0,0,0),new ShaderColor(1,1,1,1),UV.x);
	}
}
}