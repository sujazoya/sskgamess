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
public class SLTDot : ShaderLayerType{
	override public void Activate(){
		TypeS = "SLTDot";
		Name = "Procedural/Dot Noise";
		Description = "Procedurally generated dots of varying size.";
		shader = "Hidden/ShaderSandwich/Previews/Types/DotNoise";
		MenuIndex = 16;
		Function = @"
//Some noise code based on the fantastic library by Brian Sharpe, he deserves a ton of credit :)
//brisharpe CIRCLE_A yahoo DOT com
//http://briansharpe.wordpress.com
//https://github.com/BrianSharpe
float DotFalloff( float xsq ) { xsq = 1.0 - xsq; return xsq*xsq*xsq; }
float4 FastHash2D(float2 Pos){
	float2 Offset = float2(26,161);
	float Domain = 71;
	float SomeLargeFloat = 951.135664;
	float4 P = float4(Pos.xy,Pos.xy+1);
	P = P-floor(P*(1.0/Domain))*Domain;
	P += Offset.xyxy;
	P *= P;
	return frac(P.xzxz*P.yyww*(1.0/SomeLargeFloat));
}
float DotNoise2D(float2 P,float3 Rad){
	float radius_low = Rad.x;
	float radius_high = Rad.y;
	float2 Pi = floor(P);
	float2 Pf = P-Pi;

	float3 Hash = FastHash2D(Pi);
	
	float Radius = max(0.0,radius_low+Hash.z*(radius_high-radius_low));
	float Value = Radius/max(radius_high,radius_low);
	
	Radius = 2.0/Radius;
	Pf *= Radius;
	Pf -= (Radius - 1.0);
	Pf += Hash.xy*(Radius - 2);
	Pf = pow(Pf,Rad.z);
	return DotFalloff(min(dot(Pf,Pf),1.0))*Value;
}
float4 FastHash3D(float3 Pos){
	float2 Offset = float2(26,161);
	float Domain = 69;
	float4 SomeLargeFloats = float4( 635.298681, 682.357502, 668.926525, 588.255119 );
	float4 Zinc = float4( 48.500388, 65.294118, 63.934599, 63.279683 );

	Pos = Pos - floor(Pos*(1/Domain))*Domain;
	Pos.xy += Offset;
	Pos.xy *= Pos.xy;
	return frac(Pos.x*Pos.y*(1/(SomeLargeFloats+Pos.zzzz*Zinc) ) );
}
float DotNoise3D(float3 P,float3 Rad){
	P.z+=0.5;
	float3 Pi = floor(P);
	float3 Pf = P-Pi;
	float radius_low = Rad.x;
	float radius_high = Rad.y;	
	float4 Hash = FastHash3D(Pi);

	float Radius = max(0.0,radius_low+Hash.w*(radius_high-radius_low));
	float Value = Radius/max(radius_high,radius_low);
	
	Radius = 2.0/Radius;
	Pf *= Radius;
	Pf -= (Radius - 1.0);
	Pf += Hash.xyz*(Radius - 2);
	Pf = pow(Pf,Rad.z);
	return DotFalloff(min(dot(Pf,Pf),1.0))*Value;	
}
";
	}
	override public SVDictionary GetInputs(){
		Inputs = new SVDictionary();
			Inputs["Noise Dimensions"] = new ShaderVar("Noise Dimensions",new string[]{"2D","3D"},new string[]{"",""});
			Inputs["MinSize"] = new ShaderVar("Min Size",0f);
			Inputs["MaxSize"] = new ShaderVar("Max Size",1f);
			Inputs["Square"] = new ShaderVar("Square",false);
			Inputs["Gamma Correct"] = new ShaderVar("Gamma Correct",true);
		return Inputs;	
	}
	override public string Generate(SVDictionary Data, ShaderLayer SL, ShaderData SD, string Map){
		string PixCol = "";
		string f = "1";
		if (Inputs["Square"].On)
		f = "2";
		if (Data["Noise Dimensions"].Type==0)
			PixCol = "DotNoise2D("+Map+"*3,float3("+Data["MinSize"].Get()+","+Data["MaxSize"].Get()+","+f+"))";
		if (Data["Noise Dimensions"].Type==1)
			PixCol = "DotNoise3D("+Map+"*3,float3("+Data["MinSize"].Get()+","+Data["MaxSize"].Get()+","+f+"))";
		if (Data["Gamma Correct"].On)
			PixCol = ShaderUtil.GammaToLinear(PixCol);
		return PixCol;
	}
	override public int GetDimensions(SVDictionary Data, ShaderLayer SL){
		if ((Data["Noise Dimensions"]).Type==0)
		return 2;
		if ((Data["Noise Dimensions"]).Type==1)
		return 3;
		return 1-1;
	}
	override public ShaderColor Preview(SVDictionary Data, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		return new ShaderColor(DotNoise(UV.x*3f,UV.y*3f,Data["MinSize"].Float,Data["MaxSize"].Float,Data["Square"].On?2f:1f));
	}
	Float4 FastHash2D(Float2 Pos){
		Float2 Offset = new Float2(26,161);
		Float Domain = 71;
		Float SomeLargeFloat = 951.135664f;
		Float4 P = new Float4(Pos.xy,Pos.xy+1);
		//P = P-floor(P*(1.0f/Domain))*Domain;
		P = P-floor((P+0).Mul(1.0f/Domain),true).Mul(Domain);
		P.Add(Offset.xyxy);
		P.Square();
		return frac(P.xzxz.Mul(P.yyww).Mul(1.0f/SomeLargeFloat),false);
	}
	float DotNoise(float X,float Y,float Val1,float Val2,float Val3){
		Float3 Rad = new Float3(Val1,Val2,Val3);
		Float2 P = new Float2(X,Y);
		Float radius_low = Rad.x;
		Float radius_high = Rad.y;
		Float2 Pi = floor(P,true);
		Float2 Pf = P-Pi;

		Float4 Hash = FastHash2D(Pi);
		
		Float Radius = max(0.0f,radius_low+Hash.z*(radius_high-radius_low),true);
		Float Value = Radius/max(radius_high,radius_low,true);
		
		Radius = 2.0f/Radius;
		Pf *= Radius;
		Pf -= (Radius - 1.0f);
		Pf += Hash.xy*(Radius - 2f);
		Pf = pow(Pf,Rad.z);
		return DotFalloff(min(dot(Pf,Pf),1.0f,true))*Value;
	}
	float DotFalloff( Float xsq ) { xsq = 1.0f - xsq; return xsq.Square().Square(); }
}
}