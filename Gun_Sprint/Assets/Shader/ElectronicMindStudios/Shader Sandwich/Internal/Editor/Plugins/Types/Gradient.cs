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
public class SLTGradient : ShaderLayerType{
	override public void Activate(){
		TypeS = "SLTGradient";
		Name = "Procedural/Gradient";
		MenuIndex = 9;
		Description = "An interpolation between two colors.";
		shader = "Hidden/ShaderSandwich/Previews/Types/Gradient";
	}
	override public SVDictionary GetInputs(){
		Inputs = new SVDictionary();
			Inputs["Color"] = new ShaderVar("Color A",new Vector4(160f/255f,204f/255f,225/255f,1f));
			Inputs["Color"].Vector = (ShaderColor)ElectronicMindStudiosInterfaceUtils.SetSat(ElectronicMindStudiosInterfaceUtils.BaseInterfaceColor,ElectronicMindStudiosInterfaceUtils.GetSat(ElectronicMindStudiosInterfaceUtils.BaseInterfaceColor)*0.5f);
			Inputs["Color 2"] = new ShaderVar("Color B",new Vector4(0,0,0,1f));
			Inputs["Color 2"].Vector = (ShaderColor)ElectronicMindStudiosInterfaceUtils.SetSat(ElectronicMindStudiosInterfaceUtils.BaseInterfaceColorComp,ElectronicMindStudiosInterfaceUtils.GetSat(ElectronicMindStudiosInterfaceUtils.BaseInterfaceColorComp)*0.5f);
			Inputs["Cheap Gamma Correct"] = new ShaderVar("Gamma Correct",false);
		return Inputs;
	}
	override public string Generate(SVDictionary Data, ShaderLayer SL, ShaderData SD, string Map){
		//Debug.Log(Data["Color 2"].Get());
		//Debug.Log(Data["Color 2"].Get().StartsWith("float4(0, 0, 0"));
		//Debug.Log(Data["Color"].Get());
		if (Data["Cheap Gamma Correct"].On)
			Map = ShaderUtil.GammaToLinear(Map);
		if (Data["Color 2"].Get().StartsWith("float4(0, 0, 0")&&Data["Color"].Get().StartsWith("float4(1, 1, 1"))
		return "float4(("+Map+").xxxx)";
		else
		if (Data["Color 2"].Get().StartsWith("float4(1, 1, 1")&&Data["Color"].Get().StartsWith("float4(0, 0, 0"))
		return "float4((1-"+Map+").xxxx)";
		else
		if (Data["Color 2"].Get()==Data["Color"].Get())
		return Data["Color 2"].Get();
		else
		return "lerp("+Data["Color 2"].Get()+", "+Data["Color"].Get()+", "+Map+")";
	}
	override public int GetDimensions(SVDictionary Data, ShaderLayer SL){
		return 1;
	}
	override public ShaderColor Preview(SVDictionary Data, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		return ShaderColor.Lerp(Data["Color 2"].Vector,Data["Color"].Vector,UV.x);
	}
}
}