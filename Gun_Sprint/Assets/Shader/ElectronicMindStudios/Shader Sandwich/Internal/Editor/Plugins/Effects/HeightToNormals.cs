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
public class SSENormalMap : ShaderEffect{
	public override void Activate(){
		TypeS = "SSENormalMap";
		Name = "Conversion/Height to Normals";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/HeightToNormalMap";
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("Size",0.02f));
			Inputs.Add(new ShaderVar("Height",1f));
			Inputs.Add(new ShaderVar( "Channel",new string[] {"R", "G", "B","A"},new string[] {"","","",""},4));
			Inputs.Add(new ShaderVar("Normalize",true));
			Inputs["Channel"].Type = 3;

		Inputs["Size"].Range0 = 0;
		Inputs["Size"].Range1 = 0.1f;
		Inputs["Height"].Range0 = 0;
		Inputs["Height"].Range1 = 3;
		WantsFullLine = true;
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor, int W, int H){
		int X = (int)UV.x;
		int Y = (int)UV.y;
		//ShaderColor OldColor = OldColors[ShaderUtil.FlatArray(X,Y,W,H)];
		
		Vector2 NewColor = new Vector2(0,0);
		
		if (Inputs["Channel"].Type==0)
		NewColor = new Vector2(OldColors[ShaderUtil.FlatArray(X,Y,W,H)].r-OldColors[ShaderUtil.FlatArray(X+((int)Mathf.Round(Inputs["Size"].Float*W)),Y,W,H)].r,
		OldColors[ShaderUtil.FlatArray(X,Y,W,H)].r-OldColors[ShaderUtil.FlatArray(X,Y+((int)Mathf.Round(Inputs["Size"].Float*H)),W,H)].r);
		if (Inputs["Channel"].Type==1)
		NewColor = new Vector2(OldColors[ShaderUtil.FlatArray(X,Y,W,H)].g-OldColors[ShaderUtil.FlatArray(X+((int)Mathf.Round(Inputs["Size"].Float*W)),Y,W,H)].g,
		OldColors[ShaderUtil.FlatArray(X,Y,W,H)].g-OldColors[ShaderUtil.FlatArray(X,Y+((int)Mathf.Round(Inputs["Size"].Float*H)),W,H)].g);
		if (Inputs["Channel"].Type==2)
		NewColor = new Vector2(OldColors[ShaderUtil.FlatArray(X,Y,W,H)].b-OldColors[ShaderUtil.FlatArray(X+((int)Mathf.Round(Inputs["Size"].Float*W)),Y,W,H)].b,
		OldColors[ShaderUtil.FlatArray(X,Y,W,H)].b-OldColors[ShaderUtil.FlatArray(X,Y+((int)Mathf.Round(Inputs["Size"].Float*H)),W,H)].b);
		if (Inputs["Channel"].Type==3)
		NewColor = new Vector2(OldColors[ShaderUtil.FlatArray(X,Y,W,H)].a-OldColors[ShaderUtil.FlatArray(X+((int)Mathf.Round(Inputs["Size"].Float*W)),Y,W,H)].a,
		OldColors[ShaderUtil.FlatArray(X,Y,W,H)].a-OldColors[ShaderUtil.FlatArray(X,Y+((int)Mathf.Round(Inputs["Size"].Float*H)),W,H)].a);
		
		
		
		NewColor *= Inputs["Height"].Float;
		return new ShaderColor(NewColor.x,NewColor.y,1,1);//Mathf.Sqrt(1-(NewColor.x*NewColor.x)-(NewColor.y*NewColor.y)),1);
		//NewColor/=2f;
		//NewColor+= new Color(0.5f,0.5f,0.5f,0f);
		//else
		//NewColor/=(Inputs[3].Float*4f)+1;

		//return NewColor;
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		
		ShaderLayerBranch BranchX = Branch;
		ShaderLayerBranch BranchY = Branch;
		
		string[] Colors = new string[]{"r","g","b","a"};
		string Line1 = "(("+Line+"."+Colors[Inputs["Channel"].Type]+"-"+SL.StartNewBranch(SD,SL.GCUVs(SD,Inputs["Size"].Get(),"0","0", ref BranchX),BranchX)+"."+Colors[Inputs["Channel"].Type]+")*"+Inputs["Height"].Get()+")";
		string Line2 = "(("+Line+"."+Colors[Inputs["Channel"].Type]+"-"+SL.StartNewBranch(SD,SL.GCUVs(SD,"0",Inputs["Size"].Get(),"0", ref BranchY),BranchY)+"."+Colors[Inputs["Channel"].Type]+")*"+Inputs["Height"].Get()+")";
		//string retVal = "(float3("+Line1+","+Line2+","+"sqrt((1-pow("+Line1+",2)-pow("+Line2+",2)))))";///2+0.5)";
		string retVal = "normalize(float3("+Line1+","+Line2+",1))";///2+0.5)";
		if (!Inputs["Normalize"].On)
			retVal = "float3("+Line1+","+Line2+",1)";
		return retVal;
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		
		ShaderLayerBranch BranchX = Branch;
		ShaderLayerBranch BranchY = Branch;
		
		string[] Colors = new string[]{"r","g","b","a"};
		string Line1 = "(("+Line+"."+Colors[Inputs["Channel"].Type]+"-"+SL.StartNewBranch(SD,SL.GCUVs(SD,Inputs["Size"].Get(),"0","0", ref BranchX),BranchX)+"."+Colors[Inputs["Channel"].Type]+")*"+Inputs["Height"].Get()+")";
		string Line2 = "(("+Line+"."+Colors[Inputs["Channel"].Type]+"-"+SL.StartNewBranch(SD,SL.GCUVs(SD,"0",Inputs["Size"].Get(),"0", ref BranchY),BranchY)+"."+Colors[Inputs["Channel"].Type]+")*"+Inputs["Height"].Get()+")";
		//string retVal = "(float4("+Line1+","+Line2+","+"sqrt((1-pow("+Line1+",2)-pow("+Line2+",2))),"+Line+".a))";///2+0.5";
		string retVal = "(float4(normalize(float3("+Line1+","+Line2+",1)),"+Line+".a))";///2+0.5";
		if (!Inputs["Normalize"].On)
			retVal = "(float4("+Line1+","+Line2+",1,"+Line+".a))";
		return retVal;
	}
	//public override bool DrawJustAfter(Rect rect){
		//if (Inputs["Channel"].Type==3)
		//	UseAlpha.Float = 1;
	//	return false;
	//}
}
}