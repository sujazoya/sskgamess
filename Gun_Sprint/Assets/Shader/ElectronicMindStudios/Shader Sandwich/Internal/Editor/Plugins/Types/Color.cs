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
public class SLTColor : ShaderLayerType{
	override public void Activate(){
		TypeS = "SLTColor";
		Name = "Color";
		MenuIndex = 0;
		Description = "A plain color.";
		shader = "Hidden/ShaderSandwich/Previews/Types/Color";
	}
	override public SVDictionary GetInputs(){
		Inputs = new SVDictionary();
			//Inputs["Color"] = new ShaderVar("Color",new Vector4(160f/255f,204f/255f,225/255f,1f));
			Inputs["Color"] = new ShaderVar("Color",new Vector4(158f/255f*158f/255f,223f/255f*223f/255f,255f/255f,1f));
			Inputs["Color"].Vector = (ShaderColor)ElectronicMindStudiosInterfaceUtils.SetSat(ElectronicMindStudiosInterfaceUtils.BaseInterfaceColor,ElectronicMindStudiosInterfaceUtils.GetSat(ElectronicMindStudiosInterfaceUtils.BaseInterfaceColor)*0.5f);
			Inputs["Color"].VecIsColor = true;
		return Inputs;
	}
	override public string Generate(SVDictionary Data, ShaderLayer SL, ShaderData SD, string Map){
		return ShaderUtil.LinearToGamma(Data["Color"].Get());
	}
	override public int GetDimensions(SVDictionary Data, ShaderLayer SL){
		return 0;
	}
	override public ShaderColor Preview(SVDictionary Data, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		return GammaToLinear(Data["Color"].Vector);
	}
}
}