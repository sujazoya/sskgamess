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
public class SLTPerlinNoise : ShaderLayerType{
	override public void Activate(){
		TypeS = "SLTPerlinNoise";
		MenuIndex = 10;
		Name = "Procedural/Perlin Noise";
		Description = "Fast, non repeating procedural noise.";
		shader = "Hidden/ShaderSandwich/Previews/Types/PerlinNoise";
		
/*		Function = @"
sampler2D _ShaderSandwichPerlinTexture;
//Some noise code based on the fantastic library by Brian Sharpe, he deserves a ton of credit :)
//brisharpe CIRCLE_A yahoo DOT com
//http://briansharpe.wordpress.com
//https://github.com/BrianSharpe
float2 PerlinInterpolation_C2( float2 x ) { return x * x * x * (x * (x * 6.0 - 15.0) + 10.0); }
void PerlinFastHash2D(float2 Pos,out float4 hash_0, out float4 hash_1){
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
float PerlinNoise2D(float2 P)
{
	float2 Pi = floor(P);
	float4 Pf_Pfmin1 = P.xyxy-float4(Pi,Pi+1);
	float4 HashX, HashY;
	PerlinFastHash2D(Pi,HashX,HashY);
	float4 GradX = HashX-0.499999;
	float4 GradY = HashY-0.499999;
	float4 GradRes = rsqrt(GradX*GradX+GradY*GradY+0.00001)*(GradX*Pf_Pfmin1.xzxz+GradY*Pf_Pfmin1.yyww);
	
	GradRes *= 1.4142135623730950488016887242097;
	float2 blend = PerlinInterpolation_C2(Pf_Pfmin1.xy);
	float4 blend2 = float4(blend,float2(1.0-blend));
	return (dot(GradRes,blend2.zxzx*blend2.wwyy));
}
float3 PerlinInterpolation_C2( float3 x ) { return x * x * x * (x * (x * 6.0 - 15.0) + 10.0); }
void PerlinFastHash3D(float3 Pos,out float4 hash_0, out float4 hash_1,out float4 hash_2, out float4 hash_3,out float4 hash_4, out float4 hash_5){
	float2 Offset = float2(50,161);
	float Domain = 69;
	float3 SomeLargeFloats = float3(635.298681, 682.357502, 668.926525 );
	float3 Zinc = float3( 48.500388, 65.294118, 63.934599 );
	
	Pos = Pos-floor(Pos*(1.0/Domain))*Domain;
	float3 Pos_Inc1 = step(Pos,float(Domain-1.5).rrr)*(Pos+1);
	
	float4 P = float4(Pos.xy,Pos_Inc1.xy)+Offset.xyxy;
	P *= P;
	P = P.xzxz*P.yyww;
	
	float3 lowz_mod = float3(1/(SomeLargeFloats+Pos.zzz*Zinc));//Pos.zzz
	float3 highz_mod = float3(1/(SomeLargeFloats+Pos_Inc1.zzz*Zinc));//Pos_Inc1.zzz
	
	hash_0 = frac(P*lowz_mod.xxxx);
	hash_1 = frac(P*lowz_mod.yyyy);
	hash_2 = frac(P*lowz_mod.zzzz);
	hash_3 = frac(P*highz_mod.xxxx);
	hash_4 = frac(P*highz_mod.yyyy);
	hash_5 = frac(P*highz_mod.zzzz);
}
float PerlinNoise3D(float3 P)
{
	float3 Pi = floor(P);
	float3 Pf = P-Pi;
	float3 Pf_min1 = Pf-1.0;
	
	float4 HashX0, HashY0, HashZ0, HashX1, HashY1, HashZ1;
	PerlinFastHash3D(Pi, HashX0, HashY0, HashZ0, HashX1, HashY1, HashZ1);
	
	float4 GradX0 = HashX0-0.49999999;
	float4 GradX1 = HashX1-0.49999999;
	float4 GradY0 = HashY0-0.49999999;
	float4 GradY1 = HashY1-0.49999999;
	float4 GradZ0 = HashZ0-0.49999999;
	float4 GradZ1 = HashZ1-0.49999999;

	float4 GradRes = rsqrt( GradX0 * GradX0 + GradY0 * GradY0 + GradZ0 * GradZ0+0.00001) * ( float2( Pf.x, Pf_min1.x ).xyxy * GradX0 + float2( Pf.y, Pf_min1.y ).xxyy * GradY0 + Pf.zzzz * GradZ0 );
	float4 GradRes2 = rsqrt( GradX1 * GradX1 + GradY1 * GradY1 + GradZ1 * GradZ1+0.00001) * ( float2( Pf.x, Pf_min1.x ).xyxy * GradX1 + float2( Pf.y, Pf_min1.y ).xxyy * GradY1 + Pf_min1.zzzz * GradZ1 );
	
	float3 Blend = PerlinInterpolation_C2(Pf);
	
	float4 Res = lerp(GradRes,GradRes2,Blend.z);
	float4 Blend2 = float4(Blend.xy,float2(1.0-Blend.xy));
	float Final = dot(Res,Blend2.zxzx*Blend2.wwyy);
	Final *= 1.1547005383792515290182975610039;
	return Final;
}";*/
	}
	override public SVDictionary GetInputs(){
		Inputs = new SVDictionary();
			Inputs["Noise Dimensions"] = new ShaderVar("Noise Dimensions",new string[]{"2D","3D"},new string[]{"",""});
			Inputs["Image Based"] = new ShaderVar("Image Based",false);
			Inputs["Gamma Correct"] = new ShaderVar("Gamma Correct",true);
		return Inputs;
	}
	override public string Generate(SVDictionary Data, ShaderLayer SL, ShaderData SD, string Map){
		string PixCol = "";
		if (!Data["Image Based"].On){//0.05265
		SD.AdditionalFunctions.Add(
@"//Some noise code based on the fantastic library by Brian Sharpe, he deserves a ton of credit :)
//brisharpe CIRCLE_A yahoo DOT com
//http://briansharpe.wordpress.com
//https://github.com/BrianSharpe
"
		);
			if (Data["Noise Dimensions"].Type==0){
				SD.AdditionalFunctions.Add("float2 PerlinInterpolation_C2( float2 x ) { return x * x * x * (x * (x * 6.0 - 15.0) + 10.0); }\n");
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
SD.AdditionalFunctions.Add(
@"float PerlinNoise2D(float2 P){
	float2 Pi = floor(P);
	float4 Pf_Pfmin1 = P.xyxy-float4(Pi,Pi+1);
	float4 HashX, HashY;
	PerlinFastHash2D(Pi,HashX,HashY);
	float4 GradX = HashX-0.499999;
	float4 GradY = HashY-0.499999;
	float4 GradRes = rsqrt(GradX*GradX+GradY*GradY+0.00001)*(GradX*Pf_Pfmin1.xzxz+GradY*Pf_Pfmin1.yyww);
	
	GradRes *= 1.4142135623730950488016887242097;
	float2 blend = PerlinInterpolation_C2(Pf_Pfmin1.xy);
	float4 blend2 = float4(blend,float2(1.0-blend));
	return (dot(GradRes,blend2.zxzx*blend2.wwyy));
}
"
				);
				PixCol = "PerlinNoise2D("+Map+"*3)";
			}
			if (Data["Noise Dimensions"].Type==1){
				SD.AdditionalFunctions.Add("float3 PerlinInterpolation_C2( float3 x ) { return x * x * x * (x * (x * 6.0 - 15.0) + 10.0); }\n");
				SD.AdditionalFunctions.Add(
@"void PerlinFastHash3D(float3 Pos,out float4 hash_0, out float4 hash_1,out float4 hash_2, out float4 hash_3,out float4 hash_4, out float4 hash_5){
	float2 Offset = float2(50,161);
	float Domain = 69;
	float3 SomeLargeFloats = float3(635.298681, 682.357502, 668.926525 );
	float3 Zinc = float3( 48.500388, 65.294118, 63.934599 );
	
	Pos = Pos-floor(Pos*(1.0/Domain))*Domain;
	float3 Pos_Inc1 = step(Pos,float(Domain-1.5).rrr)*(Pos+1);
	
	float4 P = float4(Pos.xy,Pos_Inc1.xy)+Offset.xyxy;
	P *= P;
	P = P.xzxz*P.yyww;
	
	float3 lowz_mod = float3(1/(SomeLargeFloats+Pos.zzz*Zinc));//Pos.zzz
	float3 highz_mod = float3(1/(SomeLargeFloats+Pos_Inc1.zzz*Zinc));//Pos_Inc1.zzz
	
	hash_0 = frac(P*lowz_mod.xxxx);
	hash_1 = frac(P*lowz_mod.yyyy);
	hash_2 = frac(P*lowz_mod.zzzz);
	hash_3 = frac(P*highz_mod.xxxx);
	hash_4 = frac(P*highz_mod.yyyy);
	hash_5 = frac(P*highz_mod.zzzz);
}
float PerlinNoise3D(float3 P)
{
	float3 Pi = floor(P);
	float3 Pf = P-Pi;
	float3 Pf_min1 = Pf-1.0;
	
	float4 HashX0, HashY0, HashZ0, HashX1, HashY1, HashZ1;
	PerlinFastHash3D(Pi, HashX0, HashY0, HashZ0, HashX1, HashY1, HashZ1);
	
	float4 GradX0 = HashX0-0.49999999;
	float4 GradX1 = HashX1-0.49999999;
	float4 GradY0 = HashY0-0.49999999;
	float4 GradY1 = HashY1-0.49999999;
	float4 GradZ0 = HashZ0-0.49999999;
	float4 GradZ1 = HashZ1-0.49999999;

	float4 GradRes = rsqrt( GradX0 * GradX0 + GradY0 * GradY0 + GradZ0 * GradZ0+0.00001) * ( float2( Pf.x, Pf_min1.x ).xyxy * GradX0 + float2( Pf.y, Pf_min1.y ).xxyy * GradY0 + Pf.zzzz * GradZ0 );
	float4 GradRes2 = rsqrt( GradX1 * GradX1 + GradY1 * GradY1 + GradZ1 * GradZ1+0.00001) * ( float2( Pf.x, Pf_min1.x ).xyxy * GradX1 + float2( Pf.y, Pf_min1.y ).xxyy * GradY1 + Pf_min1.zzzz * GradZ1 );
	
	float3 Blend = PerlinInterpolation_C2(Pf);
	
	float4 Res = lerp(GradRes,GradRes2,Blend.z);
	float4 Blend2 = float4(Blend.xy,float2(1.0-Blend.xy));
	float Final = dot(Res,Blend2.zxzx*Blend2.wwyy);
	Final *= 1.1547005383792515290182975610039;
	return Final;
}
"
				);
				PixCol = "PerlinNoise3D("+Map+"*3)";
			}
			PixCol = "("+PixCol+"+1)/2";
			if (Data["Gamma Correct"].On)
				PixCol = ShaderUtil.GammaToLinear(PixCol);
		}else{
			string tex = "_ShaderSandwichPerlinTexture";
			if (!Data["Gamma Correct"].On)
				tex = "_ShaderSandwichPerlinTextureLinear";
			SD.AdditionalFunctions.Add("sampler2D "+tex+";\n");
			if (SD.PipelineStage==PipelineStage.Fragment){
				if (Data["Noise Dimensions"].Type==0)
					PixCol = "tex2D("+tex+", "+Map+".xy*0.05265-0.5)";
				if (Data["Noise Dimensions"].Type==1)
					PixCol = "lerp(tex2D("+tex+", ("+Map+"%2000).xy*0.05265-0.5+floor(("+Map+"%2000).z*5)*0.365),tex2D("+tex+", ("+Map+"%2000).xy*0.05265-0.5+floor(("+Map+"%2000).z*5+1)*0.365),frac(("+Map+"%2000).z*5))";
			}else{
				if (Data["Noise Dimensions"].Type==0)
					PixCol = "tex2Dlod("+tex+", float4("+Map+".xy*0.05265-0.5,0,0))";
				if (Data["Noise Dimensions"].Type==1)
					PixCol = "lerp(tex2Dlod("+tex+", float4(("+Map+"%2000).xy*0.05265-0.5+floor(("+Map+"%2000).z*5)*0.365,0,0)),tex2Dlod("+tex+", float4(("+Map+"%2000).xy*0.05265-0.5+floor(("+Map+"%2000).z*5+1)*0.365,0,0)),frac(("+Map+"%2000).z*5))";
			}
			//PixCol = PixCol;
		}

		
		return PixCol;
	}
	override public bool Draw(Rect rect,SVDictionary Data,ShaderLayer SL){
		bool ret = Data["Noise Dimensions"].Draw(new Rect(rect.x+2,rect.y,rect.width-4,20),"Noise Dimensions");
		if (Data["Image Based"].Draw(new Rect(rect.x+2,rect.y+25,rect.width-4,20),"Image Based")){
			ret = true;
			//Debug.Log("Huh?");
			if (Data["Image Based"].On){
				//Debug.Log(UnityEngine.Object.FindObjectOfType(typeof(ShaderSandwichRuntime)));
				if (UnityEngine.Object.FindObjectOfType(typeof(ShaderSandwichRuntime))==null){
					EditorUtility.DisplayDialog("Shader Sandwich Runtime needed!","To use Image Based noise an object in the scene needs to have the Shader Sandwich Runtime script applied. It'll work for now as long as Shader Sandwich is open, but make sure to add it at some point :).","Aye :)");
				}
			}
		}
		Data["Gamma Correct"].Hidden = Data["Image Based"].On;
		ret |= Data["Gamma Correct"].Draw(new Rect(rect.x+2,rect.y+50,rect.width-4,20),"Gamma Correct");
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