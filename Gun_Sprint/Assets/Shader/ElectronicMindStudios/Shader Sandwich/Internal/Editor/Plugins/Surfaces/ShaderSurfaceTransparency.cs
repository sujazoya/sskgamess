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
public class ShaderSurfaceTransparency : ShaderSurface{
	public override void GetMyShaderVars(List<ShaderVar> SVs){
	}
	new static public string MenuItem = "Transparency/Set Alpha Channel";
	public void OnEnable(){
		Name = "Set Alpha Channel";
		MenuItem = "Transparency/Set Alpha Channel";
		SurfaceLayers.Description = "The transparency of the surface.";
		SurfaceLayers.NameUnique.Text = "Transparency";
		SurfaceLayers.Name.Text = "Transparency";
		SurfaceLayers.BaseColor = new Color(0,0,0,0);
		ShaderSurfaceComponent = ShaderSurfaceComponent.Color;
		ExistencePreferred = new Type[]{typeof(ShaderGeometryModifierTransparency)};
	}
	public new void Awake(){
		BaseAwake();
		AlphaBlendMode.Type=3;
		SurfaceLayers.EndTag.Text = "a";
	}
	public override int GetHeight(float Width){
		return 50;
	}
	public override void RenderUI(float Width){
	}
	public override string GenerateCode(ShaderData SD){
		//SD.UsesPremultipliedBlending = SD.OutputsPremultipliedBlending;
		string POMCode = "";
		POMCode+="float Surface = 0;\n";
		POMCode+=SurfaceLayers.GenerateCode(SD,OutputPremultiplied)+"\n";
		
		if (SD.ShaderPassIsDeferred){
			return POMCode+"deferredOut.Alpha = Surface;\nreturn deferredOut;\n";
		}
		
		//if (SD.UsesPremultipliedBlending)
		//	POMCode+="return float4(outputColor.rgb*Surface,Surface);\n";
		//else
			POMCode+="return float4(outputColor.rgb,Surface);\n";
		return POMCode;
	}
	public override void CalculateTransparency(){
		UseAlphaGenerate = true;
	}
}
}