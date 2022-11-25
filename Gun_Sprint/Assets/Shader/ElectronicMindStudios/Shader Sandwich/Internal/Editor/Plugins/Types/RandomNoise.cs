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
public class SLTRandomNoise : ShaderLayerType{
	override public void Activate(){
		TypeS = "SLTRandomNoise";
		Name = "Procedural/Random Noise";
		Description = "Extremely quick random noise, good for static.";
		shader = "Hidden/ShaderSandwich/Previews/Types/RandomNoise";
		MenuIndex = 17;
		Function = @"
	float RandomNoise3D(float3 co){
		return frac(sin( dot(co.xyz ,float3(12.9898,78.233,45.5432) )) * 43758.5453);
	}
	float RandomNoise2D(float2 co){
		return frac(sin( dot(co.xy ,float2(12.9898,78.233) )) * 43758.5453);
	}
 ";
	}
	override public SVDictionary GetInputs(){
		Inputs = new SVDictionary();
			Inputs["Noise Dimensions"] = new ShaderVar("Noise Dimensions",new string[]{"2D","3D"},new string[]{"",""});
			Inputs["Gamma Correct"] = new ShaderVar("Gamma Correct",true);
		return Inputs;
	}
	override public string Generate(SVDictionary Data, ShaderLayer SL, ShaderData SD, string Map){
		string PixCol = "";
		if (Data["Noise Dimensions"].Type==0)
			PixCol = "RandomNoise2D("+Map+")";
		if (Data["Noise Dimensions"].Type==1)
			PixCol = "RandomNoise3D("+Map+")";
		if (Data["Gamma Correct"].On)
			PixCol = ShaderUtil.GammaToLinear(PixCol);
		return PixCol;
	}
	override public int GetDimensions(SVDictionary Data, ShaderLayer SL){
		if ((Data["Noise Dimensions"]).Type==0)
		return 2;
		if ((Data["Noise Dimensions"]).Type==1)
		return 3;
		return 0;
	}
	override public ShaderColor Preview(SVDictionary Data, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		float NoiseVal = Mathf.Sin(((UV.x*12.9898f)+(UV.y*78.233f))*43758.5453f);
		NoiseVal = NoiseVal-Mathf.Floor(NoiseVal);
		return new ShaderColor(NoiseVal);
	}
}
}