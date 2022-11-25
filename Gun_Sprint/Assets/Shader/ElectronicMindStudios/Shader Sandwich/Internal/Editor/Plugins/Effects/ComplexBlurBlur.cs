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
public class SSEComplexBlur : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEComplexBlur";
		Name = "Blur/Useless Blur";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/UselessBlur";
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("Separate",false));
			Inputs.Add(new ShaderVar("Blur X",0.1f));
			Inputs["Blur"] = Inputs["Blur X"];
			Inputs.Add(new ShaderVar("Blur Y",0.1f));
			Inputs.Add(new ShaderVar("Fast",true));
			Inputs.Add(new ShaderVar("Quality",3f));

		Inputs["Blur X"].Range0 = 0;
		Inputs["Blur X"].Range1 = 0.2f;
		Inputs["Blur Y"].Range0 = 0;
		Inputs["Blur Y"].Range1 = 0.2f;
		Inputs["Quality"].NoInputs = true;
		Inputs["Quality"].Range0 = 1;
		Inputs["Quality"].Range1 = 20;
		
	}
	public override bool DrawJustAfter(Rect rect){
		Inputs.Remove("Blur");
		Inputs["Quality"].Float = Mathf.Max(1,Inputs["Quality"].Float);
		Inputs["Quality"].Float = Mathf.Round(Inputs["Quality"].Float);
		
		if (!Inputs["Separate"].On){
			Inputs["Blur Y"].Hidden = true;
			Inputs["Blur Y"].Use = false;
			Inputs["Blur X"].Name = "Blur";
			Inputs["Blur Y"].Float = Inputs["Blur X"].Float;
			Inputs["Blur Y"].UseInput = Inputs["Blur X"].UseInput;
		}
		else{
			Inputs["Blur X"].Name = "Blur X";
			Inputs["Blur Y"].Hidden = false;
			Inputs["Blur Y"].Use = true;
		}
		return false;
	}
	public override void GPUPreviewJustBefore(SVDictionary Inputs, ShaderLayer SL, RenderTexture In, RenderTexture Out){
		_Material.SetFloat("_AlphaMode",UseAlpha.Float);
		_Material.SetFloat("Quality",Inputs["Quality"].Float);
		Inputs.Remove("Blur");
	}
	public ShaderColor GetAddBlur(ShaderColor[] OldColors,int X,int Y,int W,int H,int XAdd,int YAdd){
		int XX = (int)((((float)X/(float)W)+((float)XAdd*Inputs["Blur X"].Float/Inputs["Quality"].Float))*(float)W);
		int YY = (int)((((float)Y/(float)H)+((float)YAdd*Inputs["Blur Y"].Float/Inputs["Quality"].Float))*(float)H);
		return OldColors[ShaderUtil.FlatArray(XX,YY,W,H)];	
	}
	public string GetAddBlurString(ShaderData SD,ShaderLayer SL,ShaderLayerBranch Branch,int XAdd,int YAdd){
		string XX = ((float)XAdd*Inputs["Blur X"].Float/Inputs["Quality"].Float).ToString();
		string YY = ((float)YAdd*Inputs["Blur Y"].Float/Inputs["Quality"].Float).ToString();
		if (!Inputs["Separate"].On){
			YY = ((float)YAdd*Inputs["Blur X"].Float/Inputs["Quality"].Float).ToString();
		}
		
		if (Inputs["Blur X"].Input!=null)
		XX = ((float)XAdd/Inputs["Quality"].Float).ToString()+"*"+Inputs["Blur X"].Get();
		if (Inputs["Blur Y"].Input!=null)
		YY = ((float)YAdd/Inputs["Quality"].Float).ToString()+"*"+Inputs["Blur Y"].Get();
		
		if (!Inputs["Separate"].On){
			if (Inputs["Blur X"].Input!=null)
			YY = ((float)YAdd/Inputs["Quality"].Float).ToString()+"*"+Inputs["Blur X"].Get();
		}
		
		return "+("+SL.StartNewBranch(SD,SL.GCUVs(SD,XX,YY,"0",ref Branch),Branch)+")";
		//return OldColors[ShaderUtil.FlatArray(XX,YY,W,H)];	
	}
	public Color LerpBlur(float X, float Y, Color Col1,Color Col2,Color Col3,Color Col4){
		return Col1*(1f-X)*(1f-Y) + Col2*X*(1f-Y) + Col3*(1f-X)*Y + Col4*X*Y;
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor, int W, int H){
		Inputs.Remove("Blur");
		int X = (int)UV.x;
		int Y = (int)UV.y;
	
		Inputs["Quality"].Float = Mathf.Round(Inputs["Quality"].Float);
		//OldColor = OldColor;//GetAddBlur(OldColors,X,Y,W,H,0,0);
		
		ShaderColor NewColor = OldColor;
		int AddCount = 0;
		
		if (Inputs["Fast"].On==false){
			for(int i = (int)(-Inputs["Quality"].Float);i<=(int)(Inputs["Quality"].Float);i+=2){
			if (i!=0){NewColor+=GetAddBlur(OldColors,X,Y,W,H,i,i);AddCount+=1;}}
			for(int i = (int)(-Inputs["Quality"].Float);i<=(int)(Inputs["Quality"].Float);i+=2){
			if (i!=0){NewColor+=GetAddBlur(OldColors,X,Y,W,H,-i,-i);AddCount+=1;}}	
			for(int i = (int)(-Inputs["Quality"].Float);i<=(int)(Inputs["Quality"].Float);i+=2){
			if (i!=0){NewColor+=GetAddBlur(OldColors,X,Y,W,H,-i,i);AddCount+=1;}}
			for(int i = (int)(-Inputs["Quality"].Float);i<=(int)(Inputs["Quality"].Float);i+=2){
			if (i!=0){NewColor+=GetAddBlur(OldColors,X,Y,W,H,i,-i);AddCount+=1;}}		
		}
		for(int i = (int)(-Inputs["Quality"].Float);i<=(int)(Inputs["Quality"].Float);i++){
		if (i!=0){NewColor+=GetAddBlur(OldColors,X,Y,W,H,i,0);AddCount+=1;}}
		for(int i = (int)(-Inputs["Quality"].Float);i<=(int)(Inputs["Quality"].Float);i++){
		if (i!=0){NewColor+=GetAddBlur(OldColors,X,Y,W,H,0,i);AddCount+=1;}}
		
		

		//if (Inputs["Blur Y"].On==false)
		NewColor/=(float)((AddCount)+1);
		//else
		//NewColor/=(Inputs["Fast"].Float*4f)+1;

		return NewColor;
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		Inputs.Remove("Blur");
		string retVal = "((("+Line+")";
		int AddCount = 0;
		
		if (Inputs["Fast"].On==false){
			for(int i = (int)(-Inputs["Quality"].Float);i<=(int)(Inputs["Quality"].Float);i+=2){
			if (i!=0){retVal+=GetAddBlurString(SD,SL,Branch,i,i);AddCount+=1;}}
			for(int i = (int)(-Inputs["Quality"].Float);i<=(int)(Inputs["Quality"].Float);i+=2){
			if (i!=0){retVal+=GetAddBlurString(SD,SL,Branch,-i,-i);AddCount+=1;}}	
			for(int i = (int)(-Inputs["Quality"].Float);i<=(int)(Inputs["Quality"].Float);i+=2){
			if (i!=0){retVal+=GetAddBlurString(SD,SL,Branch,-i,i);AddCount+=1;}}
			for(int i = (int)(-Inputs["Quality"].Float);i<=(int)(Inputs["Quality"].Float);i+=2){
			if (i!=0){retVal+=GetAddBlurString(SD,SL,Branch,i,-i);AddCount+=1;}}		
		}
		if (Inputs["Blur X"].Float!=0f)
		for(int i = (int)(-Inputs["Quality"].Float);i<=(int)(Inputs["Quality"].Float);i++){
		if (i!=0){retVal+=GetAddBlurString(SD,SL,Branch,i,0);AddCount+=1;}}
		
		if (Inputs["Blur Y"].Float!=0f)
		for(int i = (int)(-Inputs["Quality"].Float);i<=(int)(Inputs["Quality"].Float);i++){
		if (i!=0){retVal+=GetAddBlurString(SD,SL,Branch,0,i);AddCount+=1;}}
		
		retVal += ")/"+(AddCount+1).ToString()+")";

		//+("+SL.GetSubPixel(SG,SL.GCUVs(SG,"0.01","0","0"),Effect)+"))/2)";
		return retVal;
	}
}
}