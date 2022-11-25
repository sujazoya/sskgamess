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
public class SSEMathSin : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEMathSin";
		Name = "Maths/Sine";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/Maths/Sine";
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("0-1",true));
		Function = "";
		LinePre = "";
	}
	public override void GPUPreviewJustBefore(SVDictionary Inputs, ShaderLayer SL, RenderTexture In, RenderTexture Out){
		_Material.SetFloat("_SineZeroToOne",Inputs["0-1"].On?1f:0f);
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["0-1"].On)
		return "((sin("+Line+")+1)/2)";
		else
		return "sin("+Line+")";
	}
	public override string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["0-1"].On)
		return "((sin("+Line+")+1)/2)";
		else
		return "sin("+Line+")";
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["0-1"].On)
		return "((sin("+Line+")+1)/2)";
		else
		return "sin("+Line+")";
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		//if (OldColor.r<0)
		//Debug.Log(Mathf.Sin(OldColor.r));
		ShaderColor NewColor;
		if (Inputs["0-1"].On)
		NewColor = ((new ShaderColor(Mathf.Sin(OldColor.r),Mathf.Sin(OldColor.g),Mathf.Sin(OldColor.b),Mathf.Sin(OldColor.a)))+1)/2;
		else
		NewColor = ((new ShaderColor(Mathf.Sin(OldColor.r),Mathf.Sin(OldColor.g),Mathf.Sin(OldColor.b),Mathf.Sin(OldColor.a))));
		//Debug.Log(NewColor.ToString());
		return NewColor;
	}
}
}