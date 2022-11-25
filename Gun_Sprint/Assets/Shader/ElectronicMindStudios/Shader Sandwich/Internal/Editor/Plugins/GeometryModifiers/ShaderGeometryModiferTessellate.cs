using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using SU = ElectronicMind.Sandwich.ShaderUtil;
using EMindGUI = ElectronicMind.ElectronicMindStudiosInterfaceUtils;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
public class ShaderGeometryModifierTessellation : ShaderGeometryModifier{
	public ShaderVar Type = new ShaderVar("Type",new string[]{"Equal","Size","Distance"},new string[]{"Tessellate all faces the same amount (Not Recommended!).","Tessellate faces based on their size (larger = more tessellation).","Tessellate faces based on screen area."},new string[]{"Equal","Size","Distance"});
	public ShaderVar Quality = new ShaderVar("Quality",10,0,64);
	public ShaderVar ScreenSize = new ShaderVar("ScreenSize",16,1,300);
	public ShaderVar Falloff = new ShaderVar("Falloff",1,1,3);
	public ShaderVar Snap = new ShaderVar("Snap",false);
	//public ShaderVar HighQualityDistanceCalc = new ShaderVar("High Quality Distance Calc",true);

	public override void GetMyShaderVars(List<ShaderVar> SVs){
		SVs.Add(Type);
		SVs.Add(Quality);
		SVs.Add(ScreenSize);
		SVs.Add(Falloff);
		SVs.Add(Snap);
		//SVs.Add(HighQualityDistanceCalc);
	}
	public new void Awake(){
		BaseAwake();
		Type.Type = 2;
	}
	public ShaderFix ShaderModelTooLow = null;
	public ShaderFix GraphicsAPIWrong = null;
	public override void GetMyPersonalPrivateForMySubStandardChildEyesOnlyFixes(ShaderBase shader, ShaderPass pass, List<ShaderFix> fixes){
		if (ShaderModelTooLow==null)
			ShaderModelTooLow = new ShaderFix(this,"Set the shader model to GL 4.1; this is the lowest one tessellation(subdivision) is supported in.",ShaderFix.FixType.Error,
			()=>{
				(pass.GeometryModifiers[0] as ShaderGeometryModifierMisc).ShaderModel.Type = 7;
			});
		if (GraphicsAPIWrong==null)
			GraphicsAPIWrong = new ShaderFix(this,"You should save, then switch the current Graphics API (Project Settings\\Player) to at least DirectX 11 or OpenGL Core; These is the lowest tessellation(subdivision) is supported in.",ShaderFix.FixType.Error,
			null);
		
		if ((pass.GeometryModifiers[0] as ShaderGeometryModifierMisc).ShaderModel.Type!=0&&(pass.GeometryModifiers[0] as ShaderGeometryModifierMisc).ShaderModel.Type<7)
				fixes.Add(ShaderModelTooLow);
			
		if (SystemInfo.graphicsDeviceType!=UnityEngine.Rendering.GraphicsDeviceType.Direct3D11&&
			SystemInfo.graphicsDeviceType!=UnityEngine.Rendering.GraphicsDeviceType.Direct3D12&&
			SystemInfo.graphicsDeviceType!=UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore)
				fixes.Add(GraphicsAPIWrong);
	}
	
	new static public string MenuItem = "Generate/Subdivide";
	public void OnEnable(){
		Name = "Subdivide";
		MenuItem = "Generate/Subdivide";
		SurfaceLayers.Description = "The number of subdivisions.";
		SurfaceLayers.Name.Text = "Subdivision";
		SurfaceLayers.BaseColor = new Color(1f,1f,1f,1f);
		
		ShaderGeometryModifierComponent = ShaderGeometryModifierComponent.Tessellation;
	}
	public override List<ShaderLayerList> GetMyShaderLayerLists(bool All){
		List<ShaderLayerList> SLLs = new List<ShaderLayerList>();
		return SLLs;
	}
	public override int GetHeight(float Width){
		return 20+50+25+25;
	}
	public override void RenderUI(float Width){
		int y = 20;
		
		Type.Draw(new Rect(0,y,Width,20),"Type");y+=50;
		if (Type.Type==2)
			ScreenSize.Draw(new Rect(0,y,Width,20),"Edge Size");
		else
			Quality.Draw(new Rect(0,y,Width,20),"Subdivisions");
		y+=25;
	
		Falloff.Draw(new Rect(0,y,Width,20),"Distance Falloff");y+=25;
		
		//HighQualityDistanceCalc.Draw(new Rect(0,y,Width,20),"High Quality");y+=25;
		Snap.Draw(new Rect(0,y,Width,20),"Force Snap");y+=25;
	}
	public override string GenerateHullControlPoints(ShaderData SD){
		if (Snap.On)
			SD.TessPartition = TessPartition.Pow2;
		return "";
	}
	public override string GenerateGlobalFunctions(ShaderData SD,List<ShaderIngredient> ingredients){
		SD.ShaderModelAuto = ShaderModel.SM4_6;
		bool LengthBased = false;
		bool LengthBasedHQ = false;
		foreach(ShaderIngredient ingredient in ingredients){
			if (ingredient as ShaderGeometryModifierTessellation!=null){
				LengthBased |= (ingredient as ShaderGeometryModifierTessellation).Type.Type==2;//&&!(ingredient as ShaderGeometryModifierTessellation).HighQualityDistanceCalc.On;
				LengthBasedHQ |= (ingredient as ShaderGeometryModifierTessellation).Type.Type==2;//&&(ingredient as ShaderGeometryModifierTessellation).HighQualityDistanceCalc.On;
			}
		}
		if (LengthBased){
			return //"#ifndef SubdivisionLengthBased\n"+
			"float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen){\n"+
			"	// distance to edge center\n"+
			"	float dist = distance (0.5 * (wpos0+wpos1), _WorldSpaceCameraPos);\n"+
			"	// length of the edge\n"+
			"	float len = distance(wpos0, wpos1);\n"+
			"	// edgeLen is approximate desired size in pixels\n"+
			"	float f = len * _ScreenParams.y / (edgeLen * dist);\n"+
			"	return f;\n"+
			"}\n"+
			"float4 EdgeLengthBasedTess (float3 v0, float3 v1, float3 v2, float edgeLength){\n"+
			"	float4 tess;\n"+
			"	tess.x = CalcEdgeTessFactor (v1, v2, edgeLength);\n"+
			"	tess.y = CalcEdgeTessFactor (v2, v0, edgeLength);\n"+
			"	tess.z = CalcEdgeTessFactor (v0, v1, edgeLength);\n"+
			"	tess.xyz = max(1,tess.xyz);\n"+
			"	tess.w = (tess.x + tess.y + tess.z) / 3.0f;\n"+
			"	return tess;\n"+
			"}\n";
			//"#define SubdivisionLengthBased\n"+
			//"#endif\n";
		}
		if (LengthBasedHQ){
			return
			"float CalcEdgeTessFactorHQ(float3 v0, float3 v1, float ClipW, float edgeLength){\n"+
			"	float length = distance(v0,v1);\n"+
			"	return (_ScreenParams.y/edgeLength) * abs(length  * 0.5 * UNITY_MATRIX_P[1][1] / ClipW);\n"+
			"}\n"+
			"\n"+
			"float4 EdgeLengthBasedTessHQLocal (float3 v0, float3 v1, float3 v2, float edgeLength){\n"+
			"	float4 tess;\n"+
			"	tess.x = CalcEdgeTessFactorHQ (v1, v2, mul(UNITY_MATRIX_MVP, float4((v1 + v2)*0.5, 1.0 ) ).w, edgeLength);\n"+
			"	tess.y = CalcEdgeTessFactorHQ (v2, v0, mul(UNITY_MATRIX_MVP, float4((v2 + v0)*0.5, 1.0 ) ).w, edgeLength);\n"+
			"	tess.z = CalcEdgeTessFactorHQ (v0, v1, mul(UNITY_MATRIX_MVP, float4((v0 + v1)*0.5, 1.0 ) ).w, edgeLength);\n"+
			"	tess.xyz = max(1,tess.xyz);\n"+
			"	tess.w = (tess.x + tess.y + tess.z) * 0.333333333;\n"+
			"	return tess;\n"+
			"}\n"+
			"\n"+
			"float4 EdgeLengthBasedTessHQWorld (float3 v0, float3 v1, float3 v2, float edgeLength){\n"+
			"	float4 tess;\n"+
			"	tess.x = CalcEdgeTessFactorHQ (v1, v2, mul(UNITY_MATRIX_VP, float4((v1 + v2)*0.5, 1.0 ) ).w, edgeLength);\n"+
			"	tess.y = CalcEdgeTessFactorHQ (v2, v0, mul(UNITY_MATRIX_VP, float4((v2 + v0)*0.5, 1.0 ) ).w, edgeLength);\n"+
			"	tess.z = CalcEdgeTessFactorHQ (v0, v1, mul(UNITY_MATRIX_VP, float4((v0 + v1)*0.5, 1.0 ) ).w, edgeLength);\n"+
			"	tess.xyz = max(1,tess.xyz);\n"+
			"	tess.w = (tess.x + tess.y + tess.z) * 0.333333333;\n"+
			"	return tess;\n"+
			"}\n";
		}
		return "";
	}
	public override string GenerateHullTessellationFactors(ShaderData SD){
		string DispCode = "";
		if (Type.Type==0){
			DispCode+=
			"					TF *= max(1,"+Quality.Get()+"-1);\n";
		}
		if (Type.Type==1){
			if (SD.CoordSpace==CoordSpace.World)
				DispCode+=
				"					float3 "+CodeName+"pos0 = v[0].vertex.xyz;\n"+
				"					float3 "+CodeName+"pos1 = v[1].vertex.xyz;\n"+
				"					float3 "+CodeName+"pos2 = v[2].vertex.xyz;\n";
			else
				DispCode+=
				"					float3 "+CodeName+"pos0 = mul(_Object2World,v[0].vertex).xyz;\n"+
				"					float3 "+CodeName+"pos1 = mul(_Object2World,v[1].vertex).xyz;\n"+
				"					float3 "+CodeName+"pos2 = mul(_Object2World,v[2].vertex).xyz;\n";
			
			DispCode+=
			"					float4 "+CodeName+"tessfac = 0;\n"+
			"					"+CodeName+"tessfac.x = distance("+CodeName+"pos1, "+CodeName+"pos2)*"+Quality.Get()+";\n"+
			"					"+CodeName+"tessfac.y = distance("+CodeName+"pos2, "+CodeName+"pos0)*"+Quality.Get()+";\n"+
			"					"+CodeName+"tessfac.z = distance("+CodeName+"pos0, "+CodeName+"pos1)*"+Quality.Get()+";\n"+
			"					"+CodeName+"tessfac.xyz = max(1,"+CodeName+"tessfac.xyz);\n"+
			"					"+CodeName+"tessfac.w = ("+CodeName+"tessfac.x + "+CodeName+"tessfac.y + "+CodeName+"tessfac.z) / 3.0f;\n";
			
			if (Falloff.Get()!="1")
			DispCode+="					TF *= pow("+CodeName+"tessfac/64,"+Falloff.Get()+")*64;\n";
			else
			DispCode+="					TF *=  "+CodeName+"tessfac;\n";
		}
		if (Type.Type==2){
			string func;
			/*if (HighQualityDistanceCalc.On){
				if (SD.CoordSpace==CoordSpace.World)
					func = "EdgeLengthBasedTessHQWorld(";
				else
					func = "EdgeLengthBasedTessHQLocal(";
			}
			else*/
				func = "EdgeLengthBasedTess(";
			if (SD.CoordSpace==CoordSpace.World){//||HighQualityDistanceCalc.On){
				if (Falloff.Get()!="1")
					DispCode +=	"					TF *= pow("+func+"v[0].vertex, v[1].vertex, v[2].vertex, "+ScreenSize.Get()+")/64,"+Falloff.Get()+")*64;\n";
				else
					DispCode +=	"					TF *= "+func+"v[0].vertex, v[1].vertex, v[2].vertex, "+ScreenSize.Get()+");\n";
			}else{
				if (Falloff.Get()!="1")
					DispCode +=	"					TF *= pow("+func+"mul(_Object2World,v[0].vertex), mul(_Object2World,v[1].vertex), mul(_Object2World,v[2].vertex), "+ScreenSize.Get()+")/64,"+Falloff.Get()+")*64;\n";
				else
					DispCode +=	"					TF *= "+func+"mul(_Object2World,v[0].vertex), mul(_Object2World,v[1].vertex), mul(_Object2World,v[2].vertex), "+ScreenSize.Get()+");\n";
			}
		}
		return DispCode;
	}
}
}