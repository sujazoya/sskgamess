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
public class SSEHueShift : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEHueShift";
		Name = "Color/Hue Shift";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/HueShift";
		Function = @"
float3 rgb2hsv(float3 c){
	float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
	float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}
float3 hsv2rgb(float3 c){
	c.r = frac(c.r);
	c.gb = saturate(c.gb);
	float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
	float3 pp = c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
	return pp;
}";
		



	
		
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("Hue",0));
			Inputs.Add(new ShaderVar("Saturation",0));
			Inputs.Add(new ShaderVar("Value",0));

		Inputs["Hue"].Range0 = 0;
		Inputs["Hue"].Range1 = 1;		
		Inputs["Saturation"].Range0 = -1;
		Inputs["Saturation"].Range1 = 1;			
		Inputs["Value"].Range0 = -1;
		Inputs["Value"].Range1 = 1;		
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "hsv2rgb(rgb2hsv("+Line+")+float3("+Inputs["Hue"].Get()+","+Inputs["Saturation"].Get()+","+Inputs["Value"].Get()+"))";
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		float Hue = 0;
		float Sat = 0;
		float Val = 0;
		ElectronicMindStudiosInterfaceUtils.RGBToHSV((Color)OldColor,out Hue,out Sat,out Val);
		Hue+=Inputs["Hue"].Float;
		if (Hue<0)
		Hue += 1f;
		if (Hue>1)
		Hue -= 1;
		Sat=Mathf.Clamp01(Sat+Inputs["Saturation"].Float);
		Val=Mathf.Clamp01(Val+Inputs["Value"].Float);
		ShaderColor NewColor = (ShaderColor)ElectronicMindStudiosInterfaceUtils.HSVToRGB(Hue,Sat,Val);
		NewColor.a = OldColor.a;
		return NewColor;
	}
}
}