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
public class ShaderSurfaceUnlit : ShaderSurface{
	new static public string MenuItem = "Unlit";
	public ShaderVar RenderForEachLight = new ShaderVar("RenderForEachLight",false);///TODO: Not working in Deferred - no intention to make it either just putting this downa ahaahhh
	public override void GetMyShaderVars(List<ShaderVar> SVs){
		SVs.Add(RenderForEachLight);
	}
	public void OnEnable(){
		Name = "Unlit";
		MenuItem = "Unlit";
		SurfaceLayers.Description = "";
		SurfaceLayers.Name.Text = "Unlit";
		SurfaceLayers.BaseColor = new Color(0.8f,0.8f,0.8f,1f);
	}
	public override int GetHeight(float Width){
		return 50;
	}
	public override void RenderUI(float Width){
		RenderForEachLight.Draw(new Rect(0,20,Width,20),"Apply For Every Light");
	}
	public override string GenerateCode(ShaderData SD){
		//if (!RenderForEachLight.On && !SD.ShaderPassIsBaseLighting)
		//	return "";
		string EmissionCode = "half4 Surface = half4(0.8,0.8,0.8,1.0);\n";
		
		EmissionCode += SurfaceLayers.GenerateCode(SD,OutputPremultiplied);
		
		if (!RenderForEachLight.On && !SD.ShaderPassIsBaseLighting)
			EmissionCode += "Surface.rgb = 0;\n";
		
		if (SD.ShaderPassIsForward){
			EmissionCode += "return Surface;";
		}
		else if (SD.ShaderPassIsDeferred){
			EmissionCode += "OneMinusAlpha = 0;\ndeferredOut.Alpha = 1;\ndeferredOut.Emission = Surface;\nreturn deferredOut;\n";
		}
		return EmissionCode;
	}
}
}