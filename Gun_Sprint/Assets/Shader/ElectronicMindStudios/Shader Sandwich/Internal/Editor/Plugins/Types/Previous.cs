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
public class SLTPrevious : ShaderLayerType{
	override public void Activate(){
		TypeS = "SLTPrevious";
		Name = "Adjustment";
		MenuIndex = 3;
		Description = "The previous layers.";
		shader = "Hidden/ShaderSandwich/Previews/Types/Previous";
	}
	override public SVDictionary GetInputs(){
		Inputs = new SVDictionary();
		return Inputs;
	}
	override public string Generate(SVDictionary Data, ShaderLayer SL, ShaderData SD, string Map){
		string PixCol;
		
		if (SL.Parent.EndTag.Text.Length==1)
			PixCol = "float4("+SL.Parent.CodeName+".rrrr)";
			//PixCol = "float4("+SL.Parent.CodeName+".rrr,1)";
		else
		if (SL.Parent.EndTag.Text.Length==4)
			PixCol = SL.Parent.CodeName;
		else
			PixCol = "float4("+SL.Parent.CodeName+",0)";
		
		return PixCol;
	}
	override public int GetDimensions(SVDictionary Data, ShaderLayer SL){
		return 0;
	}
	override public ShaderColor Preview(SVDictionary Data, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		return null;
	}
}
}