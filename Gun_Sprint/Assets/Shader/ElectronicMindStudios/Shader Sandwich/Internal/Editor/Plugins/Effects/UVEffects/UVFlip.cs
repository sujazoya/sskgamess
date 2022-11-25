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
public class SSEUVFlip : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEUVFlip";
		Name = "Mapping/Flip";//+UnityEngine.Random.value.ToString();
		uvshader = "Hidden/ShaderSandwich/Previews/Effects/Mapping/Flip";
		Function = "";
		LinePre = "";
		
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("X Flip",true));
			Inputs.Add(new ShaderVar("Y Flip",false));
			Inputs.Add(new ShaderVar("Z Flip",false));
			
		IsUVEffect = true;
	}
	public override string GenerateMap(SVDictionary Inputs, ShaderData SD, ShaderLayer SL, string Map, ref int UVDimensions, ref int TypeDimensions){
		//return "float3("+((Inputs[0].On)?"1-":"")+")";
		if (UVDimensions==3)
		return "float3("+((Inputs["X Flip"].On)?"1-":"")+Map+".x,"+((Inputs["Y Flip"].On)?"1-":"")+Map+".y,"+((Inputs["Z Flip"].On)?"1-":"")+Map+".z)";
		if (UVDimensions==2)
		return "float2("+((Inputs["X Flip"].On)?"1-":"")+Map+".x,"+((Inputs["Y Flip"].On)?"1-":"")+Map+".y)";
		if (UVDimensions==1)
		return ""+(((Inputs["X Flip"].On)?"1-":"")+Map);
		return Map;
	}
	public override Vector2 Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, int width, int height){
		if (Inputs["X Flip"].On)
			UV.x=width-UV.x;
		if (Inputs["Y Flip"].On)
			UV.y=height-UV.y;
		return UV;
	}
}
}