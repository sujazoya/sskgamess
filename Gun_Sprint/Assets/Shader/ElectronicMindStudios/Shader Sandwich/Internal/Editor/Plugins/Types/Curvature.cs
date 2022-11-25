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
public class SLTCurvature : ShaderLayerType{
	override public void Activate(){
		TypeS = "SLTCurvature";
		Name = "Resources/Curvature";
		MenuIndex = 22;
		Description = "The curvature of the surface (1/(the radius of the turning circle...or something))";
		shader = "Hidden/ShaderSandwich/Previews/Types/Curvature";
	}
	override public SVDictionary GetInputs(){
		Inputs = new SVDictionary();
			Inputs["Scale"] = new ShaderVar("Scale",15f);
			Inputs["Scale"].Range0 = 1f;
			Inputs["Scale"].Range1 = 100f;
		return Inputs;
	}
	override public string Generate(SVDictionary Data, ShaderLayer SL, ShaderData SD, string Map){
		return "length(fwidth(vd.worldNormal)) / max(length(fwidth(vd.worldPos) * " + Data["Scale"].Get() + "),0.0001)";
	}
	override public int GetDimensions(SVDictionary Data, ShaderLayer SL){
		return 1;
	}
	override public ShaderColor Preview(SVDictionary Data, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		return new ShaderColor(UV.x);
	}
}
}