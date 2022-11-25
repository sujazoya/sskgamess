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
public class SLTCellNoise : ShaderLayerType{
	override public void Activate(){
		TypeS = "SLTCellNoise";
		Name = "Procedural/Cell Noise";
		Description = "Cell based procedural noise.";
		shader = "Hidden/ShaderSandwich/Previews/Types/CellNoise";
		MenuIndex = 15;
		Function = @"
//Some noise code based on the fantastic library by Brian Sharpe, he deserves a ton of credit :)
//brisharpe CIRCLE_A yahoo DOT com
//http://briansharpe.wordpress.com
//https://github.com/BrianSharpe
float4 CellularWeightSamples( float4 Samples )
{
	Samples = Samples * 2.0 - 1;
	//return (1.0 - Samples * Samples) * sign(Samples);
	return (Samples * Samples * Samples) - sign(Samples);
}
void CellFastHash2D(float2 Pos,out float4 hash_0, out float4 hash_1){
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
float CellNoise2D(float2 P,float Jitter)
{
	float2 Pi = floor(P);
	float2 Pf = P-Pi;
	float4 HashX, HashY;
	CellFastHash2D(Pi,HashX,HashY);
	HashX = CellularWeightSamples(HashX)*Jitter+float4(0,1,0,1);
	HashY = CellularWeightSamples(HashY)*Jitter+float4(0,0,1,1);
	float4 dx = Pf.xxxx - HashX;
	float4 dy = Pf.yyyy - HashY;
	float4 d = dx*dx+dy*dy;
	d.xy = min(d.xy,d.zw);
	return min(d.x,d.y)*(1.0/1.125);
}
void CellFastHash3D(float3 Pos,out float4 hash_0, out float4 hash_1,out float4 hash_2, out float4 hash_3,out float4 hash_4, out float4 hash_5){
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
float CellNoise3D(float3 P,float Jitter)
{
	float3 Pi = floor(P);
	float3 Pf = P-Pi;
	
	float4 HashX0, HashY0, HashZ0, HashX1, HashY1, HashZ1;
	CellFastHash3D(Pi, HashX0, HashY0, HashZ0, HashX1, HashY1, HashZ1);
	
	HashX0 = CellularWeightSamples(HashX0)*Jitter+float4(0,1,0,1);
	HashY0 = CellularWeightSamples(HashY0)*Jitter+float4(0,0,1,1);
	HashZ0 = CellularWeightSamples(HashZ0)*Jitter+float4(0,0,0,0);
	HashX1 = CellularWeightSamples(HashX1)*Jitter+float4(0,1,0,1);
	HashY1 = CellularWeightSamples(HashY1)*Jitter+float4(0,0,1,1);
	HashZ1 = CellularWeightSamples(HashZ1)*Jitter+float4(1,1,1,1);
	
	float4 dx1 = Pf.xxxx - HashX0;
	float4 dy1 = Pf.yyyy - HashY0;
	float4 dz1 = Pf.zzzz - HashZ0;
	float4 dx2 = Pf.xxxx - HashX1;
	float4 dy2 = Pf.yyyy - HashY1;
	float4 dz2 = Pf.zzzz - HashZ1;
	float4 d1 = dx1 * dx1 + dy1 * dy1 + dz1 * dz1;
	float4 d2 = dx2 * dx2 + dy2 * dy2 + dz2 * dz2;
	d1 = min(d1, d2);
	d1.xy = min(d1.xy, d1.wz);
	return min(d1.x, d1.y) * ( 9.0 / 12.0 );
}
";
	}
	override public SVDictionary GetInputs(){
		Inputs = new SVDictionary();
			Inputs["Noise Dimensions"] = new ShaderVar("Noise Dimensions",new string[]{"2D","3D"},new string[]{"",""});
			Inputs["Jitter"] = new ShaderVar("Jitter",0.1f);
			Inputs["Gamma Correct"] = new ShaderVar("Gamma Correct",true);
		
			Inputs["Jitter"].Range0 = 0;
			Inputs["Jitter"].Range1 = 0.5f;
		return Inputs;	
	}
	override public string Generate(SVDictionary Data, ShaderLayer SL, ShaderData SD, string Map){
		string PixCol = "";
		if (Data["Noise Dimensions"].Type==0)
			PixCol = "CellNoise2D(("+Map+")*3,"+Data["Jitter"].Get()+")";
		if (Data["Noise Dimensions"].Type==1)
			PixCol = "CellNoise3D(("+Map+")*3,"+Data["Jitter"].Get()+")";
		if (Data["Gamma Correct"].On)
			PixCol = ShaderUtil.GammaToLinear(PixCol);
		return PixCol;
	}
	override public int GetDimensions(SVDictionary Data, ShaderLayer SL){
		if (((ShaderVar)Data["Noise Dimensions"]).Type==0)
		return 2;
		if (((ShaderVar)Data["Noise Dimensions"]).Type==1)
		return 3;
		return 0;
	}
	override public ShaderColor Preview(SVDictionary Data, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		return new ShaderColor(CellNoise(UV.x*3f,UV.y*3f,Data["Jitter"].Float));
	}
	
	public float CellNoise(float X,float Y,float Jitter){
		Float2 P = new Float2(X,Y);
		Float2 Pi = floor(P,true);
		Float2 Pf = P.Sub(Pi);
		Float4 HashX, HashY;
		FastHash2D(Pi,out HashX,out HashY);
		HashX = (CellularWeightSamples(HashX).Mul(Jitter)).Add(0,1,0,1);
		HashY = (CellularWeightSamples(HashY).Mul(Jitter)).Add(0,0,1,1);
		Float4 dx = Pf.xxxx.Sub(HashX);
		Float4 dy = Pf.yyyy.Sub(HashY);
		Float4 d = dx.Square()+dy.Square();
		d.xy = min(d.xy,d.zw,false);
		return min(d.x,d.y,false).Mul(1.0f/1.125f);
	}
	public void FastHash2D(Float2 Pos,out Float4 hash_0, out Float4 hash_1){
		//Float2 Offset = new Float2(26,161);
		//Float2 Offset = new Float2(26,161);
		float Domain = 71;
		Float2 SomeLargeFloats = new Float2(951.135664f,642.9478304f);
		/*Float4 P = new Float4(Pos.x,Pos.y,Pos.x+1,Pos.y+1);
		P = P-floor(P*(1.0f/Domain),false)*Domain;
		P.x += 26;
		P.y += 161;
		P.z += 26;
		P.w += 161;*/
		
		Float4 P = new Float4(
			Pos.x-(Mathf.Floor(Pos.x*(1.0f/Domain))*Domain)+26,
			Pos.y-(Mathf.Floor(Pos.y*(1.0f/Domain))*Domain)+161,
			(Pos.x+1)-(Mathf.Floor((Pos.x+1)*(1.0f/Domain))*Domain)+26,
			(Pos.y+1)-(Mathf.Floor((Pos.y+1)*(1.0f/Domain))*Domain)+161
		);
		P.Mul(P);
		P = P.xzxz.Mul(P.yyww);
		hash_0 = frac(P*(1/SomeLargeFloats.x),false);
		hash_1 = frac(P*(1/SomeLargeFloats.y),false);
	}
	public Float4 CellularWeightSamples( Float4 Samples )
	{
		Samples = Samples.Mul(2.0f).Sub(1);
		//return (1.0 - Samples * Samples) * sign(Samples);
		return ((Samples+0).Square().Square()).Sub(sign(Samples,false));
	}
}
}