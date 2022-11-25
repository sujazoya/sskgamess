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
public class SLTData : ShaderLayerType{
	override public void Activate(){
		TypeS = "SLTData";
		Name = "Resources/Data";
		Description = "Various bits of information, such as light color and direction, the object position, and whatever else I think to add.";
		MenuIndex = 23;
		UsesVertexColors = true;
		AllowInBase = true;
		AllowInVertex = false;
		AllowInLighting = true;
	}
	override public SVDictionary GetInputs(){
		Inputs = new SVDictionary();
		Inputs["Data"] = new ShaderVar("Data",new string[]{
		"Light/Direction",
		"Light/Attenuation",
		"Camera/View Direction",
		"Camera/Position",
		//"Channels/Albedo(Diffuse)",
		//"Channels/Normal",
		//"Channels/Specular",
		//"Channels/Emission",
		//"Channels/Alpha",
		//"Channels/Height",
		"Parallax/Depth",
		"Light/Color * Attenation",
		"Light/Color",
		"Light/NdotL",
		"Parallax/Shadowing",
		"Light/Position",
		"Object/Position",
		"Parallax/Normalized Depth",
		"Last Ingredient Color"
		},new string[]{"","","","","","","","","","","","",""});
		Inputs["Data"].HiddenNames[5] = "Light/Color";//Light/Color * Attenation
		Inputs["Data"].HiddenNames[6] = "Light/RawColor";//Light/Color
		Inputs["Data"].HiddenNames[2] = "View Direction";//Light/Color
		Inputs["Data"].HiddenNames[12] = "Channels/Albedo(Diffuse)";//Light/Color
		return Inputs;
	}
	override public string Generate(SVDictionary Data, ShaderLayer SL, ShaderData SD, string Map){
			string PixCol = "";
			//if (SG.InVertex||SG.ShaderPass==ShaderPasses.ShadowCaster)
			//	PixCol = "float3(1,1,1)";
			//else{
				if (Data["Data"].Type==0)
					PixCol = "gi.light.dir";
				if (Data["Data"].Type==1)
					PixCol = "vd.Atten.rrr";
				if (Data["Data"].Type==2)
					PixCol = "vd.worldViewDir";
				if (Data["Data"].Type==3)
					PixCol = "_WorldSpaceCameraPos";
				/*if (Data["Data"].Type==3)
					PixCol = "d.Albedo";
				if (Data["Data"].Type==4)
					PixCol = "d.Normal";
				if (Data["Data"].Type==5)
					PixCol = "d.Specular";
				if (Data["Data"].Type==6)
					PixCol = "d.Emission";
				if (Data["Data"].Type==7)
					PixCol = "d.Alpha.rrr";
				if (Data["Data"].Type==8)
					PixCol = "d.Height.rrr";*/
				if (Data["Data"].Type==4)
					PixCol = "outputDepth.yyy";
				if (Data["Data"].Type==5)
					PixCol = "gi.light.color";
				if (Data["Data"].Type==6)
					PixCol = "_LightColor0";
				if (Data["Data"].Type==7)
					PixCol = "gi.light.ndotl.rrr";
				if (Data["Data"].Type==8)
					PixCol = "1,1,1";
				if (Data["Data"].Type==9)
					PixCol = "_WorldSpaceLightPos0";
				if (Data["Data"].Type==10)
					PixCol = "_Object2World[0].w,_Object2World[1].w,_Object2World[2].w";
				if (Data["Data"].Type==11)
					PixCol = "outputDepth.zzz";
				if (Data["Data"].Type==12)
					PixCol = "previousBaseColor";
			//}
			if (Data["Data"].Type==6||Data["Data"].Type==9||Data["Data"].Type==12)
				return PixCol;
			else
				return "float4("+PixCol+",0)";
	}
	override public bool Draw(Rect rect,SVDictionary Data,ShaderLayer SL){
		GUI.Label(new Rect(0,rect.y,250,20),"Data");
		Data["Data"].Type=EditorGUI.Popup(new Rect(100,rect.y,150,20),Data["Data"].Type,Data["Data"].Names,ElectronicMindStudiosInterfaceUtils.EditorPopup);
		
		return false;
	}
	override public int GetDimensions(SVDictionary Data, ShaderLayer SL){
		return 0;
	}
	override public ShaderColor Preview(SVDictionary Data, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		return new ShaderColor(UV.x,UV.y,0,1);
	}
}
}