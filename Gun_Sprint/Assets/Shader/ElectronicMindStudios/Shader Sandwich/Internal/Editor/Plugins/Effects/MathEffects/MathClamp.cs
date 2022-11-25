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
public class SSEMathClamp : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEMathClamp";
		Name = "Maths/Clamp";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/Maths/Clamp";
		
		Function = "";
		LinePre = "";
		
			Inputs = new SVDictionary();
			Inputs["Min"] = new ShaderVar("Min",0);
			Inputs["Max"] = new ShaderVar("Max",1);

		Inputs["Min"].NoSlider = true;
		Inputs["Max"].NoSlider = true;	
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["Min"].Get()=="0"&&Inputs["Min"].Get()=="1")
		return "saturate("+Line+")";
		return "clamp("+Line+","+Inputs["Min"].Get()+","+Inputs["Max"].Get()+")";
	}
	public override string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["Min"].Get()=="0"&&Inputs["Min"].Get()=="1")
		return "saturate("+Line+")";
		return "clamp("+Line+","+Inputs["Min"].Get()+","+Inputs["Max"].Get()+")";
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["Min"].Get()=="0"&&Inputs["Min"].Get()=="1")
		return "saturate("+Line+")";
		return "clamp("+Line+","+Inputs["Min"].Get()+","+Inputs["Max"].Get()+")";
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		ShaderColor NewColor = new ShaderColor(Mathf.Clamp(OldColor.r,Inputs["Min"].Float,Inputs["Max"].Float),Mathf.Clamp(OldColor.g,Inputs["Min"].Float,Inputs["Max"].Float),Mathf.Clamp(OldColor.b,Inputs["Min"].Float,Inputs["Max"].Float),Mathf.Clamp(OldColor.a,Inputs["Min"].Float,Inputs["Max"].Float));
		return NewColor;
	}
}
}