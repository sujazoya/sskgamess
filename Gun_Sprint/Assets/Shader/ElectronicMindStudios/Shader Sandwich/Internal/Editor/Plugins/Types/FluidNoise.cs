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
public class SLTFluidNoise : ShaderLayerType{
	override public void Activate(){
		TypeS = "SLTFluidNoise";
		MenuIndex = 11;
		Name = "Procedural/Fluid Noise";
		Description = "2D Perlin noise that swirls.";
		shader = "Hidden/ShaderSandwich/Previews/Types/FluidNoise";
		
		Function = @"
float FluidNoise2D(float3 P){
	float2 Pi = floor(P);
	float4 Pf_Pfmin1 = P.xyxy-float4(Pi,Pi+1);
	float4 HashX, HashY;
	PerlinFastHash2D(Pi,HashX,HashY);
	
	float4 GradX = HashX-0.499999;
	float4 GradY = HashY-0.499999;
	
	float2 scx = half2(sin(P.z),cos(P.z));
	float2 scy = half2(scx.y,-scx.x);
	
	float4 GradX2 = GradX*scy.x;
	float4 GradX3 = GradX*scx.x;
	float4 GradY2 = GradY*scy.y;
	float4 GradY3 = GradY*scx.y;
	
	GradX = GradX2+GradY2;
	GradY = GradX3+GradY3;
	
	float4 GradRes = rsqrt(GradX*GradX+GradY*GradY+0.00001)*(GradX*Pf_Pfmin1.xzxz+GradY*Pf_Pfmin1.yyww);
	
	GradRes *= 1.4142135623730950488016887242097;
	
	float2 blend = PerlinInterpolation_C2(Pf_Pfmin1.xy);
	float4 blend2 = float4(blend,float2(1.0-blend));
	
	return (dot(GradRes,blend2.zxzx*blend2.wwyy));
}";
	}
	override public SVDictionary GetInputs(){
		Inputs = new SVDictionary();
			Inputs["Noise Dimensions"] = new ShaderVar("Noise Dimensions",new string[]{"Swirl Value","Mapping Z"},new string[]{"",""});
			Inputs["Swirl"] = new ShaderVar("Swirl",1f);
			Inputs["Swirl"].Range0 = 0f;
			Inputs["Swirl"].Range1 = 6.28f;
			Inputs["Gamma Correct"] = new ShaderVar("Gamma Correct",true);
		return Inputs;
	}
	override public string Generate(SVDictionary Data, ShaderLayer SL, ShaderData SD, string Map){
		string PixCol = "";
		SD.AdditionalFunctions.Add("float2 PerlinInterpolation_C2( float2 x ) { return x * x * x * (x * (x * 6.0 - 15.0) + 10.0); }\n");
				SD.AdditionalFunctions.Add(
@"//Some noise code based on the fantastic library by Brian Sharpe, he deserves a ton of credit :)
//brisharpe CIRCLE_A yahoo DOT com
//http://briansharpe.wordpress.com
//https://github.com/BrianSharpe
"
);
				SD.AdditionalFunctions.Add(
@"void PerlinFastHash2D(float2 Pos,out float4 hash_0, out float4 hash_1){
	float2 Offset = float2(26,161);
	float Domain = 71;
	float2 SomeLargeFloats = float2(951.135664,642.9478304);
	float4 P = float4(Pos.xy,Pos.xy+1);
	P = P-floor(P*(1.0/Domain))*Domain;
	P += Offset.xyxy;
	P *= P;
	P = P.xzxz*P.yyww;
	hash_0 = frac(P*(1/SomeLargeFloats.x));
	hash_1 = frac(P*(1/SomeLargeFloats.y));
}
"
);
			if (Data["Noise Dimensions"].Type==0)
				PixCol = "FluidNoise2D(float3("+Map+"*3,"+Data["Swirl"].Get()+"))";
			if (Data["Noise Dimensions"].Type==1)
				PixCol = "FluidNoise2D("+Map+"*3)";
			PixCol = "("+PixCol+"+1)/2";

		if (Data["Gamma Correct"].On)
			PixCol = ShaderUtil.GammaToLinear(PixCol);
		return PixCol;
	}
	override public bool Draw(Rect rect,SVDictionary Data,ShaderLayer SL){
		bool ret = Data["Noise Dimensions"].Draw(new Rect(rect.x+2,rect.y,rect.width-4,20),"Swirl Value");rect.y+=25;
		if (Data["Noise Dimensions"].Type==0){
			ret |= Data["Swirl"].Draw(new Rect(rect.x+2,rect.y,rect.width-4,20),"Swirl");rect.y+=25;
		}
		ret |= Data["Gamma Correct"].Draw(new Rect(rect.x+2,rect.y,rect.width-4,20),"Gamma Correct");
		//Debug.Log(ret);
		return ret;
	}
	override public int GetDimensions(SVDictionary Data, ShaderLayer SL){
		if ((Data["Noise Dimensions"]).Type==0)
		return 2;
		if ((Data["Noise Dimensions"]).Type==1)
		return 3;
		return 0;
	}
	override public ShaderColor Preview(SVDictionary Data, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		return new ShaderColor(Mathf.PerlinNoise(UV.x*3f,UV.y*3f));
	}
}
}