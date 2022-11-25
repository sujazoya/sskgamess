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
public class SSEUnpackNormal : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEUnpackNormal";
		Name = "Conversion/Unpack Normal";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/UnpackNormals";
		
		Function = "";
		LinePre = "";
		WantsFullLine = true;
		//UseAlpha.Float = 1;
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "(UnpackNormal("+Line+"))";///2+0.5)";
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return "float4(UnpackNormal("+Line+"),"+Line+".a)";///2+0.5),"+Line+".a);";
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		ShaderColor NewColor = new ShaderColor(OldColor.a,OldColor.g,0,0);
		NewColor*=2;
		NewColor -= new ShaderColor(1,1,0,0);
		//NewColor = new Color(NewColor.r,NewColor.g,0,0);
		
		NewColor = new ShaderColor(NewColor.r,NewColor.g,Mathf.Sqrt(1-(NewColor.r*NewColor.r)-(NewColor.g*NewColor.g)),1);
		//NewColor/=2f;
		//NewColor+= new Color(0.5f,0.5f,0.5f,0f);	
		//NewColor.a = OldColor.a;
		return NewColor;
	}
}
}