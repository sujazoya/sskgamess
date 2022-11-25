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
public class SLTLiteral : ShaderLayerType{
	override public void Activate(){
		TypeS = "SLTLiteral";
		Name = "Basic/Map (XYZ=RGB)";
		Description = "The mapping coordinates in color form - X is red, Y is green, Z is blue and W is alpha.";
		MenuIndex = 4;
		shader = "Hidden/ShaderSandwich/Previews/Types/Literal";
	}
	override public SVDictionary GetInputs(){
		Inputs = new SVDictionary();
		return Inputs;
	}
	override public string Generate(SVDictionary Data, ShaderLayer SL, ShaderData SD, string Map){
		return "float4("+Map+",1)";
	}
	override public int GetDimensions(SVDictionary Data, ShaderLayer SL){
		return 3;
	}
	override public ShaderColor Preview(SVDictionary Data, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		return new ShaderColor(UV.x,UV.y,0,1);
	}
}
}