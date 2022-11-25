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
public class SLTCurrentOutputColor : ShaderLayerType{
	override public void Activate(){
		TypeS = "SLTCurrentOutputColor";
		Name = "Screen/This";
		MenuIndex = 18;
		Description = "The current output color this pixel.";
		shader = "Hidden/ShaderSandwich/Previews/Types/Texture";
	}
	public override void GPUPreviewJustBefore(SVDictionary Inputs, ShaderLayer SL, RenderTexture In, RenderTexture Out){
		_Material.SetTexture("Texture",ShaderSandwich.ThisThingo);
	}
	public Color[] Image;
	override public SVDictionary GetInputs(){
		Inputs = new SVDictionary();
		return Inputs;
	}
	override public string Generate(SVDictionary Data, ShaderLayer SL, ShaderData SD, string Map){
		return "outputColor";
	}
	override public int GetDimensions(SVDictionary Data, ShaderLayer SL){
		return 0;
	}
	override public ShaderColor Preview(SVDictionary Data, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		if (Image!=null)
			return (ShaderColor)Image[ShaderUtil.FlatArray((int)(UV.x*70),(int)(UV.y*70),70,70)];
		if (ShaderSandwich.ThisThingo!=null)
			Image = ShaderSandwich.ThisThingo.GetPixels(0);
		
		return new ShaderColor(0,0,0,1);
	}
}
}