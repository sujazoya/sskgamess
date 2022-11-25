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
using EMindGUI = ElectronicMind.ElectronicMindStudiosInterfaceUtils;
using System.Xml.Serialization;
using System.Runtime.Serialization;
//using System.Xml;
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
[System.Serializable]
public class SLTVector : ShaderLayerType{
	override public void Activate(){
		TypeS = "SLTVector";
		Name = "Basic/Vector";
		Description = "A set of four numbers.";
		MenuIndex = 8;
		shader = "Hidden/ShaderSandwich/Previews/Types/Vector";
	}
	override public SVDictionary GetInputs(){
		Inputs = new SVDictionary();
			Inputs["Color"] = new ShaderVar("Color",new Vector4(160f/255f,204f/255f,225/255f,1f));
			Inputs["Color"].VecIsColor = false;
			Inputs["ColorOrFloat"] = new ShaderVar("ColorOrFloat",new string[]{"Color","Floats"},new string[]{"",""});
			Inputs["FillerInput1"] = new ShaderVar("FillerInput1",false);
			Inputs["FillerInput2"] = new ShaderVar("FillerInput2",false);
			Inputs["FillerInput3"] = new ShaderVar("FillerInput3",false);
		return Inputs;
	}
	override public bool Draw(Rect rect,SVDictionary Data,ShaderLayer SL){
		//if (DrawVars(rect,Data,SL)){
		//	GUI.changed = true;
		//}
		int Y = 0;
		Data["ColorOrFloat"].Draw(new Rect(rect.x+2,rect.y+Y*20,rect.width-4,20),Data["ColorOrFloat"].Name);
		Y++;
		if (Data["ColorOrFloat"].Type==0){
			if (Data["Color"].Draw(new Rect(rect.x+2,rect.y+Y*20,rect.width-4,20),Data["Color"].Name))
				GUI.changed = true;
		}
		if (Data["ColorOrFloat"].Type==1){
			Color OldColor = GUI.color;
			GUI.color = new Color(1,1,1,1);
			GUI.Label(new Rect(rect.x+2,rect.y+Y*20,rect.width-4,20),"X");
			float x = EditorGUI.FloatField(new Rect(rect.x+2,rect.y+Y*20,rect.width-4,20), " ", Data["Color"].Vector.r, EMindGUI.EditorFloatInput);
			
			Y++;
			GUI.Label(new Rect(rect.x+2,rect.y+Y*20,rect.width-4,20),"Y");
			float y = EditorGUI.FloatField(new Rect(rect.x+2,rect.y+Y*20,rect.width-4,20), " ", Data["Color"].Vector.g, EMindGUI.EditorFloatInput);
			
			Y++;
			GUI.Label(new Rect(rect.x+2,rect.y+Y*20,rect.width-4,20),"Z");
			float z = EditorGUI.FloatField(new Rect(rect.x+2,rect.y+Y*20,rect.width-4,20), " ", Data["Color"].Vector.b, EMindGUI.EditorFloatInput);
			
			Y++;
			GUI.Label(new Rect(rect.x+2,rect.y+Y*20,rect.width-4,20),"W");
			float w = EditorGUI.FloatField(new Rect(rect.x+2,rect.y+Y*20,rect.width-4,20), " ", Data["Color"].Vector.a, EMindGUI.EditorFloatInput);
			
			Data["Color"].Vector = new ShaderColor(x,y,z,w);
			if (Data["Color"].Input!=null)
				Data["Color"].Input.Color = new ShaderColor(x,y,z,w);
			
			GUI.color = OldColor;
		}
		
		return false;
	}
	override public string Generate(SVDictionary Data, ShaderLayer SL, ShaderData SD, string Map){
		return Data["Color"].Get();
	}
	override public int GetDimensions(SVDictionary Data, ShaderLayer SL){
		return 0;
	}
	override public ShaderColor Preview(SVDictionary Data, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		return Data["Color"].Vector;
	}
}
}