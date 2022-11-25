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
using EMindGUI = ElectronicMind.ElectronicMindStudiosInterfaceUtils;
using System.Xml.Serialization;
using System.Runtime.Serialization;
//using System.Xml;
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
[System.Serializable]
public class SLTNumber : ShaderLayerType{
	override public void Activate(){
		TypeS = "SLTNumber";
		Name = "Basic/Number";
		MenuIndex = 7;
		Description = "A number, plain and simple :)";
		shader = "Hidden/ShaderSandwich/Previews/Types/Number";
	}
	override public SVDictionary GetInputs(){
		Inputs = new SVDictionary();
			Inputs["Number"] = new ShaderVar("Number",0.5f);
		return Inputs;
	}
	override public string Generate(SVDictionary Data, ShaderLayer SL, ShaderData SD, string Map){
		return Data["Number"].Get();
	}
	override public int GetDimensions(SVDictionary Data, ShaderLayer SL){
		return 0;
	}
	override public ShaderColor Preview(SVDictionary Data, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		return new ShaderColor(Data["Number"].Float);
	}
}
}