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
public class SSEUVRotate : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEUVRotate";
		Name = "Mapping/Simple Rotate";//+UnityEngine.Random.value.ToString();
		
		Function = "";
		LinePre = "";
		
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("Rotation Axis",new string[]{"X","Y","Z"},new string[]{"","",""}));
			Inputs.Add(new ShaderVar("X Rotate",new string[]{"0°","90°","180°","270°"},new string[]{"","","",""}));
			Inputs["X Rotate"].HiddenNames = new string[]{"0","90","180","270"};
		
		Inputs["Rotation Axis"].Type = 2;
		IsUVEffect = true;
	}
	public override string GenerateMap(SVDictionary Inputs, ShaderData SD, ShaderLayer SL, string Map, ref int UVDimensions, ref int TypeDimensions){
		//return "float3("+((Inputs["X Rotate"].On)?"1-":"")+")";
		string MapX = "0";
		string MapY = "0";
		//string MapZ = Map+".z";
		//Debug.Log(UVDimensions);
		bool UpSize1 = false;
		if (UVDimensions==1&&TypeDimensions>1)
		UpSize1 = true;
	
	
		string Axis1=".x";
		string Axis2=".y";
		
		if (Inputs["Rotation Axis"].Type==2){
			Axis1=".x";
			if (UVDimensions>1)
				Axis2=".y";
		}
		if (Inputs["Rotation Axis"].Type==1){
			Axis1=".x";
			if (UVDimensions>2)
				Axis2=".z";
		}
		if (Inputs["Rotation Axis"].Type==0){
			if (UVDimensions>1)
				Axis1=".y";
			if (UVDimensions>2)
				Axis2=".z";
		}
		if (Inputs["X Rotate"].Type==0){
			MapX = Map+Axis1;
			if (!UpSize1)
			MapY = Map+Axis2;
		}
		if (Inputs["X Rotate"].Type==1){
			if (!UpSize1){
				MapX = "1-"+Map+Axis2;
				MapY = Map+Axis1;
			}
			else{
				MapY = "1-"+Map+Axis1;
			}
		}
		if (Inputs["X Rotate"].Type==2){
			MapX = "1-"+Map+Axis1;
			if (!UpSize1)
			MapY = "1-"+Map+Axis2;
		}
		if (Inputs["X Rotate"].Type==3){
			if (!UpSize1){
				MapX = Map+Axis2;
				MapY = "1-"+Map+Axis1;
			}
			else{
				MapY = Map+Axis1;
			}
		}
		if (Inputs["Rotation Axis"].Type!=2)
			UVDimensions = 3;
		if (UVDimensions==3){
			if (Inputs["Rotation Axis"].Type==2)
				return "float3("+MapX+","+MapY+","+Map+".z)";
			else if (Inputs["Rotation Axis"].Type==1)
				return "float3("+MapX+","+Map+".y,"+MapY+")";
			else if (Inputs["Rotation Axis"].Type==0)
				return "float3("+Map+".x,"+MapX+","+MapY+")";
		}
		if (UVDimensions==2)
			return "float2("+MapX+","+MapY+")";
		if (UVDimensions==1)
			return "("+MapX+")";
		return Map;
	}
	public override Vector2 Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, int width, int height){
		Vector2 OldUV = new Vector2(UV.x,UV.y);
		if (Inputs["X Rotate"].Type==1){
			UV.x=height-OldUV.y;
			UV.y=OldUV.x;
		}
		if (Inputs["X Rotate"].Type==2){
			UV.x=width-OldUV.x;
			UV.y=height-OldUV.y;
		}
		if (Inputs["X Rotate"].Type==3){
			UV.x=OldUV.y;
			UV.y=width-OldUV.x;
		}
		return UV;
	}
}
}