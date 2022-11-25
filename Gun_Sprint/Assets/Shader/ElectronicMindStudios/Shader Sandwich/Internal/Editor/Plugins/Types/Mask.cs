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
public class SLTMask : ShaderLayerType{
	override public void Activate(){
		TypeS = "SLTMask";
		Name = "Basic/Mask";
		MenuIndex = 6;
		Description = "An RGBA mask.";
		shader = "Hidden/ShaderSandwich/Previews/Types/Mask";
	}
	[NonSerialized]public bool HaveHookedGodIHateMyCode = false;
	override public SVDictionary GetInputs(){
		Inputs = new SVDictionary();
		Inputs["RGBMask"] = new ShaderVar("RGBA Mask","ListOfObjects");
		Inputs["RGBMask"].RGBAMasks = true;
		if (!HaveHookedGodIHateMyCode)
			Inputs["RGBMask"].HookIntoObjects();
		
		HaveHookedGodIHateMyCode = true;
		return Inputs;
	}
	override public bool Draw(Rect rect,SVDictionary Data,ShaderLayer SL){
		//if (SL!=null)
		Data["RGBMask"].LightingMasks = false;
		
		//Data["RGBMask"].SetToMasks(SL.Parent,0);
		Data["RGBMask"].NoInputs = true;
		if (DrawVars(rect,Data,SL)){
			GUI.changed = true;
		}
	
		return false;
	}
	override public string Generate(SVDictionary Data, ShaderLayer SL, ShaderData SD, string Map){
		if (Data["RGBMask"].Obj==null||((ShaderLayerList)Data["RGBMask"].Obj).EndTag.Text.Length==4)
			return "("+Data["RGBMask"].GetMaskName()+")";
		else
			return "("+Data["RGBMask"].GetMaskName()+".rrrr)";
	}
	override public int GetDimensions(SVDictionary Data, ShaderLayer SL){
		return 4;
	}
	override public ShaderColor Preview(SVDictionary Data, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		ShaderColor Col = new ShaderColor(0.5f,0.5f,0.5f,0.5f);
		if (Data["RGBMask"].Obj!=null)
		Col = (ShaderColor)((ShaderLayerList)(Data["RGBMask"].Obj)).GetImagePixels()[ShaderUtil.FlatArray((int)(UV.x*70),(int)(UV.y*70),(int)70,(int)70)];//GetIcon().GetPixel((int)(UV.x*70f),(int)(UV.y*70f));
		return Col;
	}
}
}