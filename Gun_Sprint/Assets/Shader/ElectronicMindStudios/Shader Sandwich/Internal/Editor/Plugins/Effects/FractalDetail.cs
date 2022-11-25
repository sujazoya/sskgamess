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
public class SSEFractalDetail : ShaderEffect{
	public override void Activate(){///TOoDO: FINISH
		TypeS = "SSEFractalDetail";
		Name = "Blur/Fractal Detail";//+UnityEngine.Random.value.ToString();
	shader = "Hidden/ShaderSandwich/Previews/Effects/FractalDetail";
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("Count",4f));
			Inputs.Add(new ShaderVar("Scale",1.8715f));
			Inputs.Add(new ShaderVar("Weighting",1f/1.8715f));

		Inputs["Count"].NoInputs = true;
		//Inputs["Scale"].NoSlider = true;
		Inputs["Count"].Float = Mathf.Round(Inputs["Count"].Float);
		Inputs["Count"].Range1 = 10;
		Inputs["Scale"].Range1 = 5f;
	}
	public override void GPUPreviewJustBefore(SVDictionary Inputs, ShaderLayer SL, RenderTexture In, RenderTexture Out){
		_Material.SetFloat("_AlphaMode",UseAlpha.Float);
		_Material.SetFloat("Count",Inputs["Count"].Float);
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor, int W, int H){
	
		//ShaderColor OldColor = OldColors[ShaderUtil.FlatArray(X,Y,W,H)];
		
		/*ShaderColor NewColor = new ShaderColor(
		OldColors[ShaderUtil.FlatArray(X+((int)Mathf.Round(W*Inputs["Count"].Vector.r)),Y+((int)Mathf.Round(H*Inputs["Count"].Vector.g)),W,H)].r,
		OldColors[ShaderUtil.FlatArray(X+((int)Mathf.Round(W*Inputs["Scale"].Vector.r)),Y+((int)Mathf.Round(H*Inputs["Scale"].Vector.g)),W,H)].g,
		OldColors[ShaderUtil.FlatArray(X+((int)Mathf.Round(W*Inputs["Weighting"].Vector.r)),Y+((int)Mathf.Round(H*Inputs["Weighting"].Vector.g)),W,H)].b,
		OldColors[ShaderUtil.FlatArray(X+((int)Mathf.Round(W*Inputs[3].Vector.r)),Y+((int)Mathf.Round(H*Inputs[3].Vector.g)),W,H)].a
		);*/
		return OldColor;//OldColors[ShaderUtil.FlatArray(X,Y,W,H)];//Mathf.Sqrt(1-(NewColor.x*NewColor.x)-(NewColor.y*NewColor.y)),1);
		//NewColor/=2f;
		//NewColor+= new Color(0.5f,0.5f,0.5f,0f);
		//else
		//NewColor/=(Inputs[3].Float*4f)+1;

		//return NewColor;
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["Count"].Float<1f)
			return Line;
		string Line1= "("+Line+"+";
		/*if (Inputs["Weighting"].Get()=="1"){
			for(int i = 1;i<=Inputs["Count"].Float;i++){
				Line1 += SL.StartNewBranch(SD,"("+SL.GCUVs(SD)+"*pow("+Inputs["Scale"].Get()+","+i.ToString()+"))",Effect)+((i+1<=Inputs["Count"].Float)?" + ":"");
			}
			Line1 += ")/"+(Inputs["Count"].Float+1).ToString();
		}
		else{
			for(int i = 1;i<=Inputs["Count"].Float;i++){
				Line1 += SL.StartNewBranch(SD,"("+SL.GCUVs(SD)+"*pow("+Inputs["Scale"].Get()+","+i.ToString()+"))",Effect)+"*((1.0/"+(Inputs["Count"].Float+1).ToString()+")*pow("+Inputs["Weighting"].Get()+","+i.ToString()+"))"+((i+1<=Inputs["Count"].Float)?" + ":"");
			}
			Line1 += ")";
		}*/
		///TODO: One Day
		//Line1 = "0;\nfloat Scale = 1;\nfloat Weight = 1;\nfor(float i = 0; i < "+Inputs["Count"].Get()+";i++){\n"+
		//				"	"+Line+" += "+SL.StartNewBranch(SD,"("+SL.GCUVs(SD)+"*Scale)",Effect)+"*Weight;\n"+
		//				"	Scale *= "+Inputs["Scale"].Get()+";\n"+
		//				"	Weight *= "+Inputs["Weighting"].Get()+";\n"+
		//				"}";
		string Scale = Inputs["Scale"].Get();
		string Weight = Inputs["Weighting"].Get();
		for(int i = 0;i<Inputs["Count"].Float;i++){
			ShaderLayerBranch NewBranch = Branch;
			Line1 += SL.StartNewBranch(SD,"("+SL.GCUVs(SD,ref NewBranch)+"*"+Scale+")",NewBranch)+" * "+Weight+((i+1<Inputs["Count"].Float)?" + ":"");
			Scale += " * "+Scale;
			Weight += " * "+Weight;
		}
		Line1 += ")";
		return Line1;
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
	//	string[] Colors = new string[]{"r","g","b","a"};
		//string Line1 = "(float4("+SL.StartNewBranch(SD,SL.GCUVs(SD,Inputs["Count"].Get()+".r",Inputs["Count"].Get()+".g",Inputs["Count"].Get()+".b"),Effect)+".r,0,0,0)+float4(0,"+SL.StartNewBranch(SD,SL.GCUVs(SD,Inputs["Scale"].Get()+".r",Inputs["Scale"].Get()+".g",Inputs["Scale"].Get()+".b"),Effect)+".g,0,0)+float4(0,0,"+SL.StartNewBranch(SD,SL.GCUVs(SD,Inputs["Weighting"].Get()+".r",Inputs["Weighting"].Get()+".g",Inputs["Weighting"].Get()+".b"),Effect)+".b,0)+float4(0,0,0,"+SL.StartNewBranch(SD,SL.GCUVs(SD,Inputs[3].Get()+".r",Inputs[3].Get()+".g",Inputs[3].Get()+".b"),Effect)+".b))";
		//string retVal = "(float3("+Line1+","+Line2+","+"sqrt((1-pow("+Line1+",2)-pow("+Line2+",2)))))";///2+0.5)";
		//string retVal = Line1;///2+0.5)";
		return Line;
	}
}
}