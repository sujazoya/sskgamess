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
public class SLTVertexColors : ShaderLayerType{
	override public void Activate(){
		TypeS = "SLTVertexColors";
		MenuIndex = 5;
		Name = "Basic/Vertex Colors";
		Description = "The vertex colors.";
		UsesVertexColors = true;
		shader = "Hidden/ShaderSandwich/Previews/Types/VertexColors";
	}
	override public SVDictionary GetInputs(){
		Inputs = new SVDictionary();
		return Inputs;
	}
	override public string Generate(SVDictionary Data, ShaderLayer SL, ShaderData SD, string Map){
			if (SL.IsVertex)
			return "v.color";
			else
			return "vd.vertexColor";
	}
	override public int GetDimensions(SVDictionary Data, ShaderLayer SL){
		return 0;
	}
	override public ShaderColor Preview(SVDictionary Data, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		return new ShaderColor(UV.x,UV.y,0,1);
	}
}
}