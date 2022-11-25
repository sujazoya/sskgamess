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
public class SSEConvertToHSV : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEConvertToHSV";
		Name = "Conversion/RGB to HSV";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/ConvertToHSV";
		Function = @"
float3 rgb2hsv2(float3 c){
	float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
	float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}";
		



	
		
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("HSVData",new string[]{"HSV","Hue","Sat","Val"},new string[]{"","","",""}));
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["HSVData"].Type==0)
			return "rgb2hsv2("+Line+")";
		if (Inputs["HSVData"].Type==1)
			return "rgb2hsv2("+Line+").rrr";
		if (Inputs["HSVData"].Type==2)
			return "rgb2hsv2("+Line+").ggg";
		if (Inputs["HSVData"].Type==3)
			return "rgb2hsv2("+Line+").bbb";
			
		return Line;
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		float Hue = 0;
		float Sat = 0;
		float Val = 0;
		ElectronicMindStudiosInterfaceUtils.RGBToHSV((Color)OldColor,out Hue,out Sat,out Val);
		ShaderColor NewColor = OldColor;
		if (Inputs["HSVData"].Type==0)
			return new ShaderColor(Hue,Sat,Val,OldColor.a);
		if (Inputs["HSVData"].Type==1)
			return new ShaderColor(Hue,Hue,Hue,OldColor.a);
		if (Inputs["HSVData"].Type==2)
			return new ShaderColor(Sat,Sat,Sat,OldColor.a);
		if (Inputs["HSVData"].Type==3)
			return new ShaderColor(Val,Val,Val,OldColor.a);
		return NewColor;
	}
}
}