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
public class ShaderSurfaceEmission : ShaderSurface{
	new static public string MenuItem = "Lighting/Emission";
	public void OnEnable(){
		Name = "Emission";
		MenuItem = "Lighting/Emission";
		SurfaceLayers.Description = "Where and what color the glow is.";
		SurfaceLayers.NameUnique.Text = "Emission";
		SurfaceLayers.Name.Text = "Emission";
		SurfaceLayers.BaseColor = new Color(0,0,0,1);
	}
	public new void Awake(){
		BaseAwake();
		MixType.Type = 1;
		AlphaBlendMode.Type = 1;
	}
	public override int GetHeight(float Width){
		return 50;
	}
	public override void RenderUI(float Width){
	}
	public override string GenerateCode(ShaderData SD){
		if (!SD.ShaderPassIsBaseLighting)
			return "";
		string EmissionCode = "half4 Surface = half4(0,0,0,0);\n";
		
		EmissionCode += SurfaceLayers.GenerateCode(SD,OutputPremultiplied);
		if (SD.ShaderPassIsForward){
			EmissionCode += "return Surface;";
		}else if (SD.ShaderPassIsDeferred){
			EmissionCode += "\ndeferredOut.Alpha = Surface.a;\nOneMinusAlpha = 0;\ndeferredOut.Emission = Surface;\nreturn deferredOut;\n";
		}
		return EmissionCode;
	}
}
}