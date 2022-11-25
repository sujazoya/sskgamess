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
public class SSEGeometryLineDistance : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEGeometryLineDistance";
		Name = "Geometry/Distance To Line";//+UnityEngine.Random.value.ToString();
		
		Function = @"
			float LineDistance(float P1, float P2, float pos){
				float direction = normalize(P2-P1);
				float startDirection = pos-P1;
				float Projection = saturate(dot(startDirection,direction)/distance(P1,P2));
				float NearestPoint = P2 * Projection + P1 * (1 - Projection);
				return distance(pos,NearestPoint);
			}
			float LineDistance(float2 P1, float2 P2, float2 pos){
				float2 direction = normalize(P2-P1);
				float2 startDirection = pos-P1;
				float Projection = saturate(dot(startDirection,direction)/distance(P1,P2));
				float2 NearestPoint = P2 * Projection + P1 * (1 - Projection);
				return distance(pos,NearestPoint);
			}
			float LineDistance(float3 P1, float3 P2, float3 pos){
				float3 direction = normalize(P2-P1);
				float3 startDirection = pos-P1;
				float Projection = saturate(dot(startDirection,direction)/distance(P1,P2));
				float3 NearestPoint = P2 * Projection + P1 * (1 - Projection);
				return distance(pos,NearestPoint);
			}
			float LineDistance(float4 P1, float4 P2, float4 pos){
				float4 direction = normalize(P2-P1);
				float4 startDirection = pos-P1;
				float Projection = saturate(dot(startDirection,direction)/distance(P1,P2));
				float4 NearestPoint = P2 * Projection + P1 * (1 - Projection);
				return distance(pos,NearestPoint);
			}
			";
			Inputs = new SVDictionary();
			Inputs.Add(new ShaderVar("Dimensions",new string[]{"1D","2D","3D","4D"},new string[]{"","","",""},new string[]{"1D","2D","3D","4D"}));
			Inputs["Dimensions"].Type = 1;
			Inputs.Add(new ShaderVar("Line Start",new ShaderColor(0,0,0,1)));
			Inputs.Add(new ShaderVar("Line End",new ShaderColor(1,1,0,1)));

		Inputs["Line Start"].ShowNumbers = true;
		Inputs["Line End"].ShowNumbers = true;
		CustomHeight = 20*5;
		//if (Inputs["Dimensions"].Type==3)
		//UseAlpha.Float=1f;
		//if (UseAlpha.Float==2f)
		//UseAlpha.Float=0f;
		WantsFullLine = true;

	
	}
	public string GenerateGeneric(string line){
		string Grey = line;
		if (Inputs["Dimensions"].Type==0)
		Grey = "LineDistance("+Inputs["Line Start"].Get()+".x,"+Inputs["Line End"].Get()+".x,"+line+".x)";
		if (Inputs["Dimensions"].Type==1)
		Grey = "LineDistance("+Inputs["Line Start"].Get()+".xy,"+Inputs["Line End"].Get()+".xy,"+line+".xy)";
		if (Inputs["Dimensions"].Type==2)
		Grey = "LineDistance("+Inputs["Line Start"].Get()+".xyz,"+Inputs["Line End"].Get()+".xyz,"+line+".xyz)";
		if (Inputs["Dimensions"].Type==3)
		Grey = "LineDistance("+Inputs["Line Start"].Get()+".xyzw,"+Inputs["Line End"].Get()+".xyzw,"+line+".xyzw)";
		//if (Inputs["Dimensions"].Type==2)
		//Grey = "LineDistance("+line+".xyz,float3("+Inputs["Line Start"].Get()+","+Inputs["Line End"].Get()+","+Inputs[3].Get()+"))";
		//if (Inputs["Dimensions"].Type==3)
		//Grey = "LineDistance("+line+",float4("+Inputs["Line Start"].Get()+","+Inputs["Line End"].Get()+","+Inputs[3].Get()+","+Inputs[4].Get()+"))";
		return "(("+Grey+").rrrr)";	
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return GenerateGeneric(Line);
	}
	public override string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return GenerateGeneric(Line);
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return GenerateGeneric(Line);
	}
	//public static float distance(Float4 asd,Float4 asd2){
	//	return Vector4.Distance(new Vector4(asd.x,asd.y,asd.z,asd.w),new Vector4(asd2.x,asd2.y,asd2.z,asd2.w));
	//}
	//public static float distance(Float3 asd,Float3 asd2){
	//	return Vector3.Distance(new Vector3(asd.x,asd.y,asd.z),new Vector3(asd2.x,asd2.y,asd2.z));
	//}
	//public static float distance(Float2 asd,Float2 asd2){
	//	return Vector2.Distance(new Vector2(asd.x,asd.y),new Vector2(asd2.x,asd2.y));
	//}
	//public static float distance(float asd,float asd2){
	//	return Mathf.Max(asd2,asd)-Mathf.Min(asd2,asd);
	//}
	public Float4 saturate(Float4 asd){
		asd.x = Mathf.Clamp(asd.x,0,1);
		asd.y = Mathf.Clamp(asd.y,0,1);
		asd.z = Mathf.Clamp(asd.z,0,1);
		asd.w = Mathf.Clamp(asd.w,0,1);
		return asd;
	}
	public Float3 saturate(Float3 asd){
		asd.x = Mathf.Clamp(asd.x,0,1);
		asd.y = Mathf.Clamp(asd.y,0,1);
		asd.z = Mathf.Clamp(asd.z,0,1);
		return asd;
	}
	public Float2 saturate(Float2 asd){
		asd.x = Mathf.Clamp(asd.x,0,1);
		asd.y = Mathf.Clamp(asd.y,0,1);
		return asd;
	}
	public float saturate(float asd){
		asd = Mathf.Clamp(asd,0,1);
		return asd;
	}
	//public Float dot(Float4 asd,Float4 asd2){
	//	return asd.x*asd2.x+asd.y*asd2.y+asd.z*asd2.z+asd.w*asd2.w;
	//}
	public Float dot(Float3 asd,Float3 asd2){
		return asd.x*asd2.x+asd.y*asd2.y+asd.z*asd2.z;
	}
	//public Float dot(Float2 asd,Float2 asd2){
	//	return asd.x*asd2.x+asd.y*asd2.y;
	//}
	public Float dot(float asd,float asd2){
		return asd*asd2;
	}
	public Float4 normalize(Float4 asd){
		return new Float4(new Vector4(asd.x,asd.y,asd.z,asd.w).normalized);
	}
	public Float3 normalize(Float3 asd){
		return new Float3(new Vector3(asd.x,asd.y,asd.z).normalized);
	}
	public Float2 normalize(Float2 asd){
		return new Float2(new Vector2(asd.x,asd.y).normalized);
	}
	public float normalize(float asd){
		return 1;
	}
	public float LineDistance(Float4 P1, Float4 P2, Float4 pos){
		Float4 direction = normalize(P2-P1);
		Float4 startDirection = pos-P1;
		float Projection = saturate(dot(startDirection,direction)/distance(P1,P2));
		Float4 NearestPoint = P2 * Projection + P1 * (1f - Projection);
		return distance(pos,NearestPoint);	
	}
	public float LineDistance(Float3 P1, Float3 P2, Float3 pos){
		Float3 direction = normalize(P2-P1);
		Float3 startDirection = pos-P1;
		float Projection = saturate(dot(startDirection,direction)/distance(P1,P2));
		Float3 NearestPoint = P2 * Projection + P1 * (1f - Projection);
		return distance(pos,NearestPoint);	
	}
	public float LineDistance(Float2 P1, Float2 P2, Float2 pos){
		Float2 direction = normalize(P2-P1);
		Float2 startDirection = pos-P1;
		float Projection = saturate(dot(startDirection,direction)/distance(P1,P2));
		Float2 NearestPoint = P2 * Projection + P1 * (1f - Projection);
		return distance(pos,NearestPoint);	
	}
	public float LineDistance(float P1, float P2, float pos){
		float direction = 1;
		float startDirection = pos-P1;
		float Projection = saturate(dot(startDirection,direction)/distance(P1,P2));
		float NearestPoint = P2 * Projection + P1 * (1f - Projection);
		return distance(pos,NearestPoint);	
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		//ShaderColor NewColor = OldColor*new ShaderColor(Inputs["Dimensions"].Float,Inputs["Line Start"].Float,Inputs["Line End"].Float,Inputs[3].Float);// = new ShaderColor(OldColor.r+Inputs["Dimensions"].Float,OldColor.g+Inputs["Dimensions"].Float,OldColor.b+Inputs["Dimensions"].Float,OldColor.a+Inputs["Dimensions"].Float);
		float Grey = 0;
		if (Inputs["Dimensions"].Type==0)
		Grey = LineDistance(Inputs["Line Start"].Vector.r,Inputs["Line End"].Vector.r,OldColor.r);
		if (Inputs["Dimensions"].Type==1)
		Grey = LineDistance(new Float2(Inputs["Line Start"].Vector),new Float2(Inputs["Line End"].Vector),new Float2(OldColor.r,OldColor.g));
		//Grey = Vector2.Distance(new Vector2(OldColor.r,OldColor.g),new Vector2(Inputs["Line Start"].Float,Inputs["Line End"].Float));
		if (Inputs["Dimensions"].Type==2)
		Grey = LineDistance(new Float3(Inputs["Line Start"].Vector),new Float3(Inputs["Line End"].Vector),new Float3(OldColor.r,OldColor.g,OldColor.b));
		if (Inputs["Dimensions"].Type==3)
		Grey = LineDistance(new Float4(Inputs["Line Start"].Vector),new Float4(Inputs["Line End"].Vector),new Float4(OldColor));
		
		
		ShaderColor NewColor = new ShaderColor(Grey,Grey,Grey,Grey);
		return NewColor;
	}
}
}