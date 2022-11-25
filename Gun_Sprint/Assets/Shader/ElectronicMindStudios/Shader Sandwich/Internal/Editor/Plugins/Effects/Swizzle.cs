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
public class SSESwizzle : ShaderEffect{
	public override void Activate(){
		TypeS = "SSESwizzle";
		Name = "Color/Swizzle";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/Swizzle";
		
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar( "Channel R",new string[] {"R", "G", "B","A"},new string[] {"","","",""},4));
			Inputs["Channel R"].Type = 0;
			Inputs.Add(new ShaderVar( "Channel G",new string[] {"R", "G", "B","A"},new string[] {"","","",""},4));
			Inputs["Channel G"].Type = 1;
			Inputs.Add(new ShaderVar( "Channel B",new string[] {"R", "G", "B","A"},new string[] {"","","",""},4));
			Inputs["Channel B"].Type = 2;
			Inputs.Add(new ShaderVar( "Channel A",new string[] {"R", "G", "B","A"},new string[] {"","","",""},4));
			Inputs["Channel A"].Type = 3;
		//if (Inputs["Channel R"].Type==3||Inputs["Channel G"].Type==3||Inputs["Channel B"].Type==3||Inputs["Channel A"].Type==3)
		//UseAlpha.Float = 1;
		WantsFullLine = true;
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		float R = 0;
		if (Inputs["Channel R"].Type == 0)
		R = OldColor.r;
		if (Inputs["Channel R"].Type == 1)
		R = OldColor.g;
		if (Inputs["Channel R"].Type == 2)
		R = OldColor.b;
		if (Inputs["Channel R"].Type == 3)
		R = OldColor.a;
		
		float G = 0;
		if (Inputs["Channel G"].Type == 0)
		G = OldColor.r;
		if (Inputs["Channel G"].Type == 1)
		G = OldColor.g;
		if (Inputs["Channel G"].Type == 2)
		G = OldColor.b;
		if (Inputs["Channel G"].Type == 3)
		G = OldColor.a;	
		
		float B = 0;
		if (Inputs["Channel B"].Type == 0)
		B = OldColor.r;
		if (Inputs["Channel B"].Type == 1)
		B = OldColor.g;
		if (Inputs["Channel B"].Type == 2)
		B = OldColor.b;
		if (Inputs["Channel B"].Type == 3)
		B = OldColor.a;
		
		float A = OldColor.a;
		if (Inputs["Channel A"].Type == 0)
		A = OldColor.r;
		if (Inputs["Channel A"].Type == 1)
		A = OldColor.g;
		if (Inputs["Channel A"].Type == 2)
		A = OldColor.b;
		if (Inputs["Channel A"].Type == 3)
		A = OldColor.a;
		
		ShaderColor NewColor = new ShaderColor(R,G,B,A);
		return NewColor;
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return Line+"."+((Inputs["Channel R"].Names[Inputs["Channel R"].Type]+Inputs["Channel G"].Names[Inputs["Channel G"].Type]+Inputs["Channel B"].Names[Inputs["Channel B"].Type]).ToLower());//+Inputs["Channel A"].Names[Inputs["Channel A"].Type];
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return Line+"."+(Inputs["Channel R"].Names[Inputs["Channel R"].Type]+Inputs["Channel G"].Names[Inputs["Channel G"].Type]+Inputs["Channel B"].Names[Inputs["Channel B"].Type]+Inputs["Channel A"].Names[Inputs["Channel A"].Type]).ToLower();//+Inputs["Channel A"].Names[Inputs["Channel A"].Type];
	}
	public override string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return Line+"."+Inputs["Channel A"].Names[Inputs["Channel A"].Type].ToLower();//+Inputs["Channel A"].Names[Inputs["Channel A"].Type];
	}
}
}