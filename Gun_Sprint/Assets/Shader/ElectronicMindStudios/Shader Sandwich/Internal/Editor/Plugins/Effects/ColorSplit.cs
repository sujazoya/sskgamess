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
public class SSEColorSplit : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEColorSplit";
		Name = "Color/Split";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/ColorSplit";

		Inputs = new SVDictionary();
		Inputs.Add(new ShaderVar("R Vector",new ShaderColor(0.6f,0.1f,0,0f)));
		Inputs.Add(new ShaderVar("G Vector",new ShaderColor(1f,0,-0.4f,0f)));
		Inputs.Add(new ShaderVar("B Vector",new ShaderColor(-0.3f,1f,0,0f)));
		Inputs.Add(new ShaderVar("A Vector",new ShaderColor(1f,1f,0,0f)));
		Inputs.Add(new ShaderVar("Advanced",false));
		Inputs.Add(new ShaderVar("Factor",0.2f));
			
		Inputs["R Vector"].ShowNumbers = true;
		Inputs["G Vector"].ShowNumbers = true;
		Inputs["B Vector"].ShowNumbers = true;
		Inputs["A Vector"].ShowNumbers = true;
		Inputs["R Vector"].VecIsColor = false;
		Inputs["G Vector"].VecIsColor = false;
		Inputs["B Vector"].VecIsColor = false;
		Inputs["A Vector"].VecIsColor = false;
		Inputs["Factor"].Range1 = 0.25f;
		
		CustomHeight = 20*10+10;	
	}
	override public bool DrawJustAfter(Rect rect){
		if (!Inputs["Advanced"].On){
			Inputs["R Vector"].Hidden = true;
			Inputs["G Vector"].Hidden = true;
			Inputs["B Vector"].Hidden = true;
			Inputs["A Vector"].Hidden = true;
			Inputs["Factor"].Hidden = false;
			Inputs["R Vector"].Vector = new ShaderColor(Inputs["Factor"].Float,Inputs["Factor"].Float,0f,0f);
			Inputs["G Vector"].Vector = new ShaderColor(0f,Inputs["Factor"].Float,0f,0f);
			Inputs["B Vector"].Vector = new ShaderColor(Inputs["Factor"].Float,0f,0f,0f);
			Inputs["A Vector"].Vector = new ShaderColor(0f,-Inputs["Factor"].Float,0f,0f);
		}
		else{
			Inputs["R Vector"].Hidden = false;
			Inputs["G Vector"].Hidden = false;
			Inputs["B Vector"].Hidden = false;
			Inputs["A Vector"].Hidden = false;
			Inputs["Factor"].Hidden = true;
		}
		return false;
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor, int W, int H){
	
		//ShaderColor OldColor = OldColors[ShaderUtil.FlatArray(X,Y,W,H)];
		
		ShaderColor NewColor = new ShaderColor(
		OldColors[ShaderUtil.FlatArray((int)UV.x+((int)Mathf.Round(W*Inputs["R Vector"].Vector.r)),(int)UV.y+((int)Mathf.Round(H*Inputs["R Vector"].Vector.g)),W,H)].r,
		OldColors[ShaderUtil.FlatArray((int)UV.x+((int)Mathf.Round(W*Inputs["G Vector"].Vector.r)),(int)UV.y+((int)Mathf.Round(H*Inputs["G Vector"].Vector.g)),W,H)].g,
		OldColors[ShaderUtil.FlatArray((int)UV.x+((int)Mathf.Round(W*Inputs["B Vector"].Vector.r)),(int)UV.y+((int)Mathf.Round(H*Inputs["B Vector"].Vector.g)),W,H)].b,
		OldColors[ShaderUtil.FlatArray((int)UV.x+((int)Mathf.Round(W*Inputs["A Vector"].Vector.r)),(int)UV.y+((int)Mathf.Round(H*Inputs["A Vector"].Vector.g)),W,H)].a
		);
		return NewColor;//Mathf.Sqrt(1-(NewColor.x*NewColor.x)-(NewColor.y*NewColor.y)),1);
		//NewColor/=2f;
		//NewColor+= new Color(0.5f,0.5f,0.5f,0f);
		//else
		//NewColor/=(Inputs["A Vector"].Float*4f)+1;

		//return NewColor;
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
//		string[] Colors = new string[]{"r","g","b","a"};
			ShaderLayerBranch Branch1 = Branch;
			ShaderLayerBranch Branch2 = Branch;
			ShaderLayerBranch Branch3 = Branch;
		string Line1;
		if (Inputs["Advanced"].On)
			Line1 = "(float3("+SL.StartNewBranch(SD,SL.GCUVs(SD,Inputs["R Vector"].Get()+".r",Inputs["R Vector"].Get()+".g",Inputs["R Vector"].Get()+".b",ref Branch1),Branch1)+".r,0,0)+float3(0,"+SL.StartNewBranch(SD,SL.GCUVs(SD,Inputs["G Vector"].Get()+".r",Inputs["G Vector"].Get()+".g",Inputs["G Vector"].Get()+".b",ref Branch2),Branch2)+".g,0)+float3(0,0,"+SL.StartNewBranch(SD,SL.GCUVs(SD,Inputs["B Vector"].Get()+".r",Inputs["B Vector"].Get()+".g",Inputs["B Vector"].Get()+".b",ref Branch3),Branch3)+".b))";
		else
			Line1 = "(float3("+SL.StartNewBranch(SD,SL.GCUVs(SD,Inputs["Factor"].Get(),Inputs["Factor"].Get(),"0",ref Branch1),Branch1)+".r,0,0)+float3(0,"+SL.StartNewBranch(SD,SL.GCUVs(SD,"0",Inputs["Factor"].Get(),"0",ref Branch2),Branch2)+".g,0)+float3(0,0,"+SL.StartNewBranch(SD,SL.GCUVs(SD,Inputs["Factor"].Get(),"0","0",ref Branch3),Branch3)+".b))";
		return Line1;
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
	//	string[] Colors = new string[]{"r","g","b","a"};
		//string Line1 = "(float4("+SL.StartNewBranch(SD,SL.GCUVs(SD,Inputs["R Vector"].Get()+".r",Inputs["R Vector"].Get()+".g",Inputs["R Vector"].Get()+".b"),Effect)+".r,0,0,0)+float4(0,"+SL.StartNewBranch(SD,SL.GCUVs(SD,Inputs["G Vector"].Get()+".r",Inputs["G Vector"].Get()+".g",Inputs["G Vector"].Get()+".b"),Effect)+".g,0,0)+float4(0,0,"+SL.StartNewBranch(SD,SL.GCUVs(SD,Inputs["B Vector"].Get()+".r",Inputs["B Vector"].Get()+".g",Inputs["B Vector"].Get()+".b"),Effect)+".b,0)+float4(0,0,0,"+SL.StartNewBranch(SD,SL.GCUVs(SD,Inputs["A Vector"].Get()+".r",Inputs["A Vector"].Get()+".g",Inputs["A Vector"].Get()+".b"),Effect)+".b))";
			ShaderLayerBranch Branch1 = Branch;
			ShaderLayerBranch Branch2 = Branch;
			ShaderLayerBranch Branch3 = Branch;
		string Line1;
		if (Inputs["Advanced"].On)
			Line1 = "(float3("+SL.StartNewBranch(SD,SL.GCUVs(SD,Inputs["R Vector"].Get()+".r",Inputs["R Vector"].Get()+".g",Inputs["R Vector"].Get()+".b",ref Branch1),Branch1)+".r,0,0)+float3(0,"+SL.StartNewBranch(SD,SL.GCUVs(SD,Inputs["G Vector"].Get()+".r",Inputs["G Vector"].Get()+".g",Inputs["G Vector"].Get()+".b",ref Branch2),Branch2)+".g,0)+float3(0,0,"+SL.StartNewBranch(SD,SL.GCUVs(SD,Inputs["B Vector"].Get()+".r",Inputs["B Vector"].Get()+".g",Inputs["B Vector"].Get()+".b",ref Branch3),Branch3)+".b))";
		else
			Line1 = "(float3("+SL.StartNewBranch(SD,SL.GCUVs(SD,Inputs["Factor"].Get(),Inputs["Factor"].Get(),"0",ref Branch1),Branch1)+".r,0,0)+float3(0,"+SL.StartNewBranch(SD,SL.GCUVs(SD,"0",Inputs["Factor"].Get(),"0",ref Branch2),Branch2)+".g,0)+float3(0,0,"+SL.StartNewBranch(SD,SL.GCUVs(SD,Inputs["Factor"].Get(),"0","0",ref Branch3),Branch3)+".b))";
		//string retVal = "(float3("+Line1+","+Line2+","+"sqrt((1-pow("+Line1+",2)-pow("+Line2+",2)))))";///2+0.5)";
		//string retVal = Line1;///2+0.5)";
		return Line1;
	}
}
}