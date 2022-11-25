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
public class SSEMathStretch : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEMathStretch";
		Name = "Maths/Stretch";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/Maths/Stretch";
		Function = "";
		LinePre = "";
		
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("From: Start",0));
			Inputs.Add(new ShaderVar("From: End",1));
			Inputs.Add(new ShaderVar("To: Start",-1));
			Inputs.Add(new ShaderVar("To: End",1));

		Inputs["From: Start"].NoSlider = true;
		Inputs["From: End"].NoSlider = true;
		Inputs["To: Start"].NoSlider = true;
		Inputs["To: End"].NoSlider = true;


	
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["From: Start"].Get()=="0"&&Inputs["From: End"].Get()=="1")
		return "("+Line+"*("+Inputs["To: End"].Get()+"-("+Inputs["To: Start"].Get()+"))+"+Inputs["To: Start"].Get()+")";
		else
		return "((("+Line+"-("+Inputs["From: Start"].Get()+"))/("+Inputs["From: End"].Get()+"-("+Inputs["From: Start"].Get()+")))*("+Inputs["To: End"].Get()+"-("+Inputs["To: Start"].Get()+"))+"+Inputs["To: Start"].Get()+")";
	}
	public override string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["From: Start"].Get()=="0"&&Inputs["From: End"].Get()=="1")
		return "("+Line+"*("+Inputs["To: End"].Get()+"-("+Inputs["To: Start"].Get()+"))+"+Inputs["To: Start"].Get()+")";
		else
		return "((("+Line+"-("+Inputs["From: Start"].Get()+"))/("+Inputs["From: End"].Get()+"-("+Inputs["From: Start"].Get()+")))*("+Inputs["To: End"].Get()+"-("+Inputs["To: Start"].Get()+"))+"+Inputs["To: Start"].Get()+")";
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["From: Start"].Get()=="0"&&Inputs["From: End"].Get()=="1")
		return "("+Line+"*("+Inputs["To: End"].Get()+"-("+Inputs["To: Start"].Get()+"))+"+Inputs["To: Start"].Get()+")";
		else
		return "((("+Line+"-("+Inputs["From: Start"].Get()+"))/("+Inputs["From: End"].Get()+"-("+Inputs["From: Start"].Get()+")))*("+Inputs["To: End"].Get()+"-("+Inputs["To: Start"].Get()+"))+"+Inputs["To: Start"].Get()+")";
	}
	public float stretch( float f){
		return ((f - Inputs["From: Start"].Float)/(Inputs["From: End"].Float-Inputs["From: Start"].Float))*(Inputs["To: End"].Float-Inputs["To: Start"].Float)+Inputs["To: Start"].Float;
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		ShaderColor NewColor = new ShaderColor(stretch( OldColor.r),stretch( OldColor.g),stretch( OldColor.b),stretch( OldColor.a));
		return NewColor;
	}
}
}