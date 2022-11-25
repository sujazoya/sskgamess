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
public class SSEPixelate : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEPixelate";
		Name = "Blur/Pixelate";//+UnityEngine.Random.value.ToString();
		uvshader = "Hidden/ShaderSandwich/Previews/Effects/Blur/Pixelate";
		Function = "";
		LinePre = "";
		
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("Seperate",false));
			Inputs.Add(new ShaderVar("X Size",0.00001f));
			Inputs.Add(new ShaderVar("Y Size",0.00001f));
			Inputs.Add(new ShaderVar("Z Size",0.00001f));
		Inputs["X Size"].Range0 = 0.000001f;
		Inputs["Y Size"].Range0 = 0.000001f;
		Inputs["Z Size"].Range0 = 0.000001f;
		//Inputs["X Size"].NoSlider = true;
		//Inputs["Y Size"].NoSlider = true;
		//Inputs["Z Size"].NoSlider = true;
		
		

		

		IsUVEffect = true;
	}
	public override string GenerateMap(SVDictionary Inputs, ShaderData SD, ShaderLayer SL, string Map, ref int UVDimensions, ref int TypeDimensions){
		if (!Inputs["Seperate"].On){
		Inputs["Y Size"].Float = Inputs["X Size"].Float;
		Inputs["Z Size"].Float = Inputs["X Size"].Float;
		Inputs["Y Size"].Input = Inputs["X Size"].Input;
		Inputs["Z Size"].Input = Inputs["X Size"].Input;
		}
		string Mapping="";
		if (UVDimensions==3)
		Mapping = "float3("+Inputs["X Size"].Get()+","+Inputs["Y Size"].Get()+","+Inputs["Z Size"].Get()+")";
		if (UVDimensions==2)
		Mapping = "float2("+Inputs["X Size"].Get()+","+Inputs["Y Size"].Get()+")";
		if (UVDimensions==1)
		Mapping = "("+Inputs["X Size"].Get()+")";
		
		if (Mapping!="")
		return "(round("+Map+"/"+Mapping+")*"+Mapping+")";
		
		return Map;
	}
	public override Vector2 Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, int width, int height){
		Vector2 NormalUV = new Vector2(UV.x/(float)width,UV.y/(float)height);
		if (!Inputs["Seperate"].On){
		Inputs["Y Size"].Float = Inputs["X Size"].Float;
		Inputs["Z Size"].Float = Inputs["X Size"].Float;
		}
		NormalUV.x=Mathf.Round(NormalUV.x/Inputs["X Size"].Float)*Inputs["X Size"].Float;
		NormalUV.y=Mathf.Round(NormalUV.y/Inputs["Y Size"].Float)*Inputs["Y Size"].Float;
		//NormalUV.y=Inputs["Y Size"].Float;
		
		UV = new Vector2(NormalUV.x*(float)width,NormalUV.y*(float)height);
		return UV;
	}
	public override string GenerateBase(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line,string Map, ref ShaderLayerBranch Branch){
		///TOoDO: FIX FOR CURRENT TYPE SYSTEM AAHH!
		if (SL.LayerType.Obj!=null){
			if ((SL.LayerType.Obj as ShaderPlugin).TypeS == "SLTTexture"){
				if (SD.PipelineStage==PipelineStage.Fragment&&SD.ShaderModel!=ShaderModel.SM2&&SD.ShaderModel!=ShaderModel.SM2_5){
					string UVs = SL.GCUVs(SD,ref Branch);
					Line = "tex2D("+SL.CustomData["Texture"].Get()+","+Map+",ddx("+UVs+"), ddy("+UVs+"))";
				}
				else
					Line = "tex2Dlod("+SL.CustomData["Texture"].Get()+",half4("+Map+",0,0))";
				
			}
			if ((SL.LayerType.Obj as ShaderPlugin).TypeS == "SLTCubemap"){
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
				if (SD.PipelineStage==PipelineStage.Fragment&&SD.ShaderModel!=ShaderModel.SM2&&SD.ShaderModel!=ShaderModel.SM2_5){
					string UVs = SL.GCUVs(SD,ref Branch);
					Line = around1+"texCUBEgrad("+SL.CustomData["Cubemap"].Get()+","+Map+",ddx("+UVs+"), ddy("+UVs+"))"+around2;
				}
				else			
					Line = around1+"texCUBElod("+SL.CustomData["Cubemap"].Get()+",half4("+Map+",0))"+around2;
			}	
		}	
		return Line;
	}
	public override bool DrawJustAfter(Rect rect){
		if (!Inputs["Seperate"].On){
			Inputs["Y Size"].Hidden = true;
			Inputs["Y Size"].Use = false;
			Inputs["Z Size"].Hidden = true;
			Inputs["Z Size"].Use = false;
			Inputs["X Size"].Name = "Size";
			Inputs["Y Size"].Float = Inputs["X Size"].Float;
			Inputs["Y Size"].UseInput = Inputs["X Size"].UseInput;
			Inputs["Z Size"].Float = Inputs["X Size"].Float;
			Inputs["Z Size"].UseInput = Inputs["X Size"].UseInput;
		}
		else{
			Inputs["X Size"].Name = "X Size";
			Inputs["Y Size"].Hidden = false;
			Inputs["Y Size"].Use = true;
			Inputs["Z Size"].Hidden = false;
			Inputs["Z Size"].Use = true;
		}
		return false;
	}
}
}