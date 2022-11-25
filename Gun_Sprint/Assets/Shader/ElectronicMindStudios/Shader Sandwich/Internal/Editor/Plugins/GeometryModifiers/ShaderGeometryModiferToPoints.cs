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
public class ShaderGeometryModifierToPoints : ShaderGeometryModifier{
	new static public string MenuItem = "Convert/Points";
	public void OnEnable(){
		Name = "Points";
		MenuItem = "Convert/Points";
		
		ShaderGeometryModifierComponent = ShaderGeometryModifierComponent.Tessellation;
	}
	public override List<ShaderLayerList> GetMyShaderLayerLists(bool All){
		List<ShaderLayerList> SLLs = new List<ShaderLayerList>();
		return SLLs;
	}
	public override int GetHeight(float Width){
		return 25;
	}
	public override void RenderUI(float Width){
	}
	public override string GenerateHullControlPoints(ShaderData SD){
		SD.TessOutputTopology = TessOutputTopology.Point;
		return "";
	}
}
}