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
public class ShaderSurfaceTangentNormals : ShaderSurface{
	public ShaderVar Normalize = new ShaderVar("Normalize",true);
	public override void GetMyShaderVars(List<ShaderVar> SVs){
		SVs.Add(Normalize);
	}
	new static public string MenuItem = "Normals/Tangent Normals";
	public void OnEnable(){
		Name = "Tangent Normals";
		MenuItem = "Normals/Tangent Normals";
		SurfaceLayers.NameUnique.Text = "Normals";
		SurfaceLayers.Name.Text = "Normals";
		SurfaceLayers.EndTag.Text = "rgb";
		SurfaceLayers.Description = "Add fake details to the surface of the model.";
		SurfaceLayers.BaseColor = new Color(0,0,1,1);
		ShaderSurfaceComponent = ShaderSurfaceComponent.Normal;
		TopAnchor = new Type[]{typeof(ShaderSurfaceParallaxOcclusionMapping),typeof(ShaderSurfaceDepth)};
	}
	public override int GetHeight(float Width){
		return 50;
	}
	public override void RenderUI(float Width){
		Normalize.Draw(new Rect(0,20,Width,20),"Normalize: ");
	}
	public override Dictionary<string,string> GeneratePrecalculations(){
		Dictionary<string,string> Precalcs = new Dictionary<string,string>();
		return Precalcs;
	}
	public override string GenerateCode(ShaderData SD){
		string NormalsCode = "half3 Surface = half3(0,0,1);\n";
		
		NormalsCode += SurfaceLayers.GenerateCode(SD,OutputPremultiplied);
		if (Normalize.On)
			return NormalsCode+"return normalize(half3(dot(vd.TtoWSpaceX.xyz, Surface),dot(vd.TtoWSpaceY.xyz, Surface),dot(vd.TtoWSpaceZ.xyz, Surface)));\n";
		else
			return NormalsCode+"return half3(dot(vd.TtoWSpaceX.xyz, Surface),dot(vd.TtoWSpaceY.xyz, Surface),dot(vd.TtoWSpaceZ.xyz, Surface));\n";
	}
}
}