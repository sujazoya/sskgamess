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
public class SSESwitchNormalScale : ShaderEffect{
	public override void Activate(){
		TypeS = "SSESwitchNormalScale";
		Name = "Conversion/Squish Normal Map";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/SwitchNormalMapScale";
		Function = "";
		LinePre = "";
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "("+Line+"*0.5+0.5)";///2+0.5)";
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "("+Line+"*0.5+0.5)";
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		ShaderColor NewColor = OldColor;
		//NewColor*=2;
		//NewColor -= new Color(1,1,0,0);
		//NewColor = new Color(NewColor.r,NewColor.g,0,0);
		
		//NewColor = new Color(NewColor.r,NewColor.g,Mathf.Sqrt(1-(NewColor.r*NewColor.r)-(NewColor.g*NewColor.g)),1);
		NewColor*=0.5f;
		NewColor+= new ShaderColor(0.5f,0.5f,0.5f,0f);	
		NewColor.a = OldColor.a;
		//NewColor.a = OldColor.a;
		return NewColor;
	}
}
}