using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Xml;
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
public enum ShaderVertexInterpolatorType{Float,Float2,Float3,Float4,Half4,Fixed4,Color,Position,Inline};
public class ShaderVertexInterpolator{
	public string Name;
	public ShaderVertexInterpolatorType ShaderVertexInterpolatorType = ShaderVertexInterpolatorType.Float3;
	public string GenerationCode = "";
	public Dictionary<CoordSpace,string> GenerationCodeSpecific = new Dictionary<CoordSpace,string>();
	
	public string GetCode(CoordSpace cs){
		if (GenerationCodeSpecific.ContainsKey(cs))
			return GenerationCodeSpecific[cs];
		return GenerationCode;
	}
	
	public string FragmentCode = "";
	public string FasterFragmentCode = "";
	
	public string ParallaxDisplacementVariable = "";
	public ShaderVertexInterpolator[] ReliesOn;// = new List<ShaderVertexInterpolator>();
	
	public string[] PredefineStarts;
	public string[] PredefineEnds;
	
	public string[] vertexData;// = new List<string>();
	public ShaderVertexInterpolator(string n, ShaderVertexInterpolatorType t, string G, ShaderVertexInterpolator[] r, string[] v){
		Name = n;
		ShaderVertexInterpolatorType = t;
		GenerationCode = G;
		ReliesOn = r;
		vertexData = v;
	}
	public ShaderVertexInterpolator(string n, ShaderVertexInterpolatorType t, string G, ShaderVertexInterpolator[] r, string[] v, string[] ps, string[] pe){
		Name = n;
		ShaderVertexInterpolatorType = t;
		GenerationCode = G;
		ReliesOn = r;
		vertexData = v;
		PredefineStarts = ps;
		PredefineEnds = pe;
	}
	public ShaderVertexInterpolator(string n, ShaderVertexInterpolatorType t, string G, string F, ShaderVertexInterpolator[] r, string[] v){
		Name = n;
		ShaderVertexInterpolatorType = t;
		GenerationCode = G;
		FragmentCode = F;
		FasterFragmentCode = F;
		ReliesOn = r;
		vertexData = v;
	}
	public ShaderVertexInterpolator(string n, ShaderVertexInterpolatorType t, string G, string F, string FF, ShaderVertexInterpolator[] r, string[] v){
		Name = n;
		ShaderVertexInterpolatorType = t;
		GenerationCode = G;
		FragmentCode = F;
		FasterFragmentCode = FF;
		ReliesOn = r;
		vertexData = v;
	}
	public ShaderVertexInterpolator(string n, ShaderVertexInterpolatorType t, Dictionary<CoordSpace,string> G, ShaderVertexInterpolator[] r, string[] v){
		Name = n;
		ShaderVertexInterpolatorType = t;
		GenerationCodeSpecific = G;
		GenerationCode = G[CoordSpace.Object];
		ReliesOn = r;
		vertexData = v;
	}
	public ShaderVertexInterpolator(string n, ShaderVertexInterpolatorType t, Dictionary<CoordSpace,string> G, string F, ShaderVertexInterpolator[] r, string[] v){
		Name = n;
		ShaderVertexInterpolatorType = t;
		GenerationCodeSpecific = G;
		GenerationCode = G[CoordSpace.Object];
		FragmentCode = F;
		FasterFragmentCode = F;
		ReliesOn = r;
		vertexData = v;
	}
	public ShaderVertexInterpolator(string n, ShaderVertexInterpolatorType t, Dictionary<CoordSpace,string> G, string F, string FF, ShaderVertexInterpolator[] r, string[] v){
		Name = n;
		ShaderVertexInterpolatorType = t;
		GenerationCodeSpecific = G;
		GenerationCode = G[CoordSpace.Object];
		FragmentCode = F;
		FasterFragmentCode = FF;
		ReliesOn = r;
		vertexData = v;
	}
	public string ImLazy(){
		if (ShaderVertexInterpolatorType==ShaderVertexInterpolatorType.Float)
			return "	float ";
		if (ShaderVertexInterpolatorType==ShaderVertexInterpolatorType.Float2)
			return "	float2 ";
		if (ShaderVertexInterpolatorType==ShaderVertexInterpolatorType.Float3)
			return "	float3 ";
		if (ShaderVertexInterpolatorType==ShaderVertexInterpolatorType.Float4)
			return "	float4 ";
		if (ShaderVertexInterpolatorType==ShaderVertexInterpolatorType.Half4)
			return "	half4 ";
		if (ShaderVertexInterpolatorType==ShaderVertexInterpolatorType.Fixed4)
			return "	fixed4 ";
		if (ShaderVertexInterpolatorType==ShaderVertexInterpolatorType.Color)
			return "	half4 ";
		if (ShaderVertexInterpolatorType==ShaderVertexInterpolatorType.Position)
			return "	float4 ";
		if (ShaderVertexInterpolatorType==ShaderVertexInterpolatorType.Inline)
			return "	";
		return "IHaveNoIdeaWhatIsHappening - Please send an email to Ztechnologies.com@gmail.com with your shader, thanks!";
	}
	//STUPID
	//public bool Used = false;//Calculate and Transfer to Fragment
	//public bool Needed = false;//At least Calculate
	
	
	public bool InVertex = false;
	public bool InFragment = false;
	public bool Transfer = false;
	
	public bool TempBool = false;//For...whatever XD
}
}