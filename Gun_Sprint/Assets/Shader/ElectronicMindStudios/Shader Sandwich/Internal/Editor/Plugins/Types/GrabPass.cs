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
public class SLTGrabPass : ShaderLayerType{
	override public void Activate(){
		TypeS = "SLTGrabPass";
		Name = "Screen/Behind";
		MenuIndex = 20;
		Description = "The screen before any objects with this shader is rendered.";
		Function = @"		//Setup inputs for the grab pass texture and some meta information about it.
				sampler2D _GrabTexture;
				float4 _GrabTexture_TexelSize;";
		ExternalFunction = "GrabPass {}";
		shader = "Hidden/ShaderSandwich/Previews/Types/Texture";
	}
	///TODO Warning in deferred
	public override void GPUPreviewJustBefore(SVDictionary Inputs, ShaderLayer SL, RenderTexture In, RenderTexture Out){
		_Material.SetTexture("Texture",ShaderSandwich.GrabPass);
	}
	public Color[] Image;
	override public SVDictionary GetInputs(){
		Inputs = new SVDictionary();
		return Inputs;
	}
	override public string Generate(SVDictionary Data, ShaderLayer SL, ShaderData SD, string Map){
		SD.BlendsWeirdlyAnywhere = true;
		SD.AdditionalFunctions.Add("	float2 SillyFlip(float2 inUV){\n"+
		"		#if UNITY_UV_STARTS_AT_TOP\n"+
		"			if (_ProjectionParams.x > 0);\n"+
		"				inUV.y = 1-inUV.y;\n"+
		"		#endif\n"+
		"		inUV.y = 1-inUV.y;///TODO: Make this function work...\n"+
		"		return inUV;\n"+
		"	}\n");
		return "tex2D( _GrabTexture, SillyFlip("+Map+"))";
	}
	override public int GetDimensions(SVDictionary Data, ShaderLayer SL){
		return 2;
	}
	override public ShaderColor Preview(SVDictionary Data, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		if (Image!=null)
			return (ShaderColor)Image[ShaderUtil.FlatArray((int)(UV.x*70),(int)(UV.y*70),70,70)];
		if (ShaderSandwich.GrabPass!=null)
			Image = ShaderSandwich.GrabPass.GetPixels(0);
		
		return new ShaderColor(0,0,0,1);
	}
}
}