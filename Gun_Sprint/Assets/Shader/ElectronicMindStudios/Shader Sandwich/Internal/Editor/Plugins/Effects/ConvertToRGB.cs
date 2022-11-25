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
public class SSEConvertToRGB : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEConvertToRGB";
		Name = "Conversion/HSV to RGB";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/ConvertToRGB";
		Function = @"
float3 hsv2rgb2(float3 c){
	c.r = frac(c.r);
	c.gb = saturate(c.gb);
	float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
	float3 pp = c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
	return pp;
}";
		



	
		
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("RGBData",new string[]{"RGB","R","G","B"},new string[]{"","","",""}));
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["RGBData"].Type==0)
			return "hsv2rgb2("+Line+")";
		if (Inputs["RGBData"].Type==1)
			return "hsv2rgb2("+Line+").rrr";
		if (Inputs["RGBData"].Type==2)
			return "hsv2rgb2("+Line+").ggg";
		if (Inputs["RGBData"].Type==3)
			return "hsv2rgb2("+Line+").bbb";
			
		return Line;
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		float R = 0;
		float G = 0;
		float B = 0;
		ShaderColor NewColor = (ShaderColor)ElectronicMindStudiosInterfaceUtils.HSVToRGB(OldColor.r,OldColor.g,OldColor.b);
		R = NewColor.r;
		G = NewColor.g;
		B = NewColor.b;
		if (Inputs["RGBData"].Type==0)
			return new ShaderColor(R,G,B,OldColor.a);
		if (Inputs["RGBData"].Type==1)
			return new ShaderColor(R,R,R,OldColor.a);
		if (Inputs["RGBData"].Type==2)
			return new ShaderColor(G,G,G,OldColor.a);
		if (Inputs["RGBData"].Type==3)
			return new ShaderColor(B,B,B,OldColor.a);
		return NewColor;
	}
}
}