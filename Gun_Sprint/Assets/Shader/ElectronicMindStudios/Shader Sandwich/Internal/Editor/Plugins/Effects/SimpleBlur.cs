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
public class SSESimpleBlur : ShaderEffect{
	public override void Activate(){
		TypeS = "SSESimpleBlur";
		Name = "Blur/Simple Blur";//+UnityEngine.Random.value.ToString();
		ChangeBaseCol = true;
		
		Function = "";

		
		Inputs = new SVDictionary();
		Inputs.Add(new ShaderVar("Blur",0));

		Inputs["Blur"].Range0 = 0;
		Inputs["Blur"].Range1 = 10;		
		
		IsUVEffect = true;
	}
	public ShaderColor GetAddBlur(SVDictionary Inputs, ShaderColor[] OldColors,int X,int Y,int W,int H,int XAdd,int YAdd){
		int XX = (int)(Mathf.Floor((float)X/(Inputs["Blur"].Float*3+1)+XAdd)*(Inputs["Blur"].Float*3+1));
		int YY = (int)(Mathf.Floor((float)Y/(Inputs["Blur"].Float*3+1)+YAdd)*(Inputs["Blur"].Float*3+1));
		return OldColors[ShaderUtil.FlatArray(XX,YY,W,H)];	
	}
	public ShaderColor LerpBlur(float X, float Y, ShaderColor Col1,ShaderColor Col2,ShaderColor Col3,ShaderColor Col4){
		return Col1*(1f-X)*(1f-Y) + Col2*X*(1f-Y) + Col3*(1f-X)*Y + Col4*X*Y;
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		//Color OldColor = GetAddBlur(SE,OldColors,X,Y,W,H,0,0);
		int X = (int)UV.x;
		int Y = (int)UV.y;
		int W = 70;
		int H = 70;
		float OldSE = Inputs["Blur"].Float;
		Inputs["Blur"].Float = Mathf.Max(0,(Mathf.Round(Inputs["Blur"].Float))-1);//Mathf.Pow(Inputs["Blur"].Float/5f,2f)*8f;
		float DXX = ((float)X/(Inputs["Blur"].Float*3+1))-Mathf.Floor((float)X/(Inputs["Blur"].Float*3+1));
		float DYY = (((float)Y/(Inputs["Blur"].Float*3+1))-Mathf.Floor((float)Y/(Inputs["Blur"].Float*3+1)));
		
		ShaderColor NewColor = LerpBlur(DXX,DYY,GetAddBlur(Inputs,OldColors,X,Y,W,H,0,0),GetAddBlur(Inputs,OldColors,X,Y,W,H,1,0),GetAddBlur(Inputs,OldColors,X,Y,W,H,0,1),GetAddBlur(Inputs,OldColors,X,Y,W,H,1,1));
		Inputs["Blur"].Float = OldSE;
		//OldColor = new Color(DXX,DYY,0,1);
		
		//Color NewColor = OldColor;
		//NewColor.a = OldColor.a;
		return NewColor;
	}
	
	public override string GenerateBase(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line,string Map, ref ShaderLayerBranch Branch){
		if (SL.LayerType.Obj!=null){
			if (SL.LayerType.Obj.GetType().Name == "SLTTexture"){
				Line = "tex2Dlod("+SL.CustomData["Texture"].Get()+",float4("+Map+",0,"+Inputs["Blur"].Get()+"))";
			}
			if (SL.LayerType.Obj.GetType().Name == "SLTCubemap"){
				string around1 = "";
				string around2 = "";
				if (SL.CustomData["HDR Support"].On){
					SD.AdditionalFunctions.Add(
				"	half4 DecodeHDRWA(half4 col, half4 data){\n"+
				"		return half4(DecodeHDR(col,data),col.a);\n"+
				"	}\n");
					around1 = "DecodeHDRWA(";
					around2 = ","+SL.CustomData["Cubemap"].Get()+"_HDR)";
				}
				Line = around1+"texCUBElod("+SL.CustomData["Cubemap"].Get()+",float4("+Map+","+Inputs["Blur"].Get()+"))"+around2;		
			}
			if (SL.LayerType.Obj.GetType().Name == "SLTGrabPass"){
				Line = "tex2Dlod( _GrabTexture, float4("+Map+",0,"+Inputs["Blur"].Get()+"))";
			}
			if (SL.LayerType.Obj.GetType().Name == "SLTDepthPass"){
				if (SL.CustomData["Linearize"].On)
					Line = "(LinearEyeDepth(tex2Dlod(_CameraDepthTexture, float4("+Map+",0,"+Inputs["Blur"].Get()+")).r).rrrr)";
				else
					Line = "(tex2Dlod(_CameraDepthTexture, float4("+Map+",0,"+Inputs["Blur"].Get()+")).rrrr)";
			}
		}
		return Line;
	}
}
}