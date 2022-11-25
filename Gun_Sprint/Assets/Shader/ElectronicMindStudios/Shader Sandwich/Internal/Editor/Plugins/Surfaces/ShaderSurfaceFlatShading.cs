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
public class ShaderSurfaceFlatShading : ShaderSurface{
	public override void GetMyShaderVars(List<ShaderVar> SVs){
	}
	new static public string MenuItem = "Normals/Flat Shading";
	public ShaderFix MobileSupport= null;
	public override void GetMyPersonalPrivateForMySubStandardChildEyesOnlyFixes(ShaderBase shader, ShaderPass pass, List<ShaderFix> fixes){
		if (MobileSupport==null)
			MobileSupport = new ShaderFix(this,"Sorry, as far as I know the Flat Shading ingredient won't work on most mobile phones.",ShaderFix.FixType.Error,
			null);
			
		if (shader.HardwareTarget.Type==2){//Mobile
			fixes.Add(MobileSupport);
		}
	}
	public void OnEnable(){
		Name = "Flat Shading";
		MenuItem = "Normals/Flat Shading";
		ShaderSurfaceComponent = ShaderSurfaceComponent.Normal;
		TopAnchor = new Type[]{typeof(ShaderSurfaceParallaxOcclusionMapping),typeof(ShaderSurfaceDepth)};
	}
	public override int GetHeight(float Width){
		return 50;
	}
	public override List<ShaderLayerList> GetMyShaderLayerLists(bool All){
		List<ShaderLayerList> SLLs = new List<ShaderLayerList>();
		return SLLs;
	}
	public override string GenerateCode(ShaderData SD){
		//return "return normalize(cross(ddy(vd.worldPos), ddx(vd.worldPos)));\n";
		return 
		"half3 flatNormals = normalize(cross(ddy(vd.worldPos), ddx(vd.worldPos)));\n"+
		//"#if UNITY_UV_STARTS_AT_TOP//Not sure if this is necessary, but I don't have enough devices to test it out so yeah.\n"+
		//"	if (_ProjectionParams.x > 0)\n"+
		"flatNormals *= _ProjectionParams.x;\n"+
		"#ifdef UNITY_UV_STARTS_AT_TOP\n"+
		"	flatNormals = -flatNormals;\n"+
		"#endif\n"+
		//"#endif\n"+
		"return flatNormals;\n";
	}
}
}