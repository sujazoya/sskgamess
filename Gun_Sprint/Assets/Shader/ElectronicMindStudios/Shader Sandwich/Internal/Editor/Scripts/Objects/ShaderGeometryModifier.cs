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
using System.Text;
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
//Base class for all geometry modifiers, vertex, tessellation, geometry shader stuff, etc.
//Only 1 tesselation and geometry allowed per pass.
[Flags]
public enum ShaderGeometryModifierComponent{None = 0,Vertex = 1,Tessellation = 2,Geometry = 4,Transparency = 8, Pass = 16};
public class ShaderGeometryModifier : ShaderIngredient{
	public ShaderGeometryModifier(){
		Name = "GenericModifier";
		MenuItem = "Generic/GenericModifier";
	}
	public ShaderGeometryModifierComponent ShaderGeometryModifierComponent = ShaderGeometryModifierComponent.Vertex;
	public bool HasFlag(ShaderGeometryModifierComponent s){
		return ((ShaderGeometryModifierComponent&s)==s);
	}
	public virtual string GenerateHullControlPoints(ShaderData SD){
		return "";
	}
	public virtual string GenerateHullTessellationFactors(ShaderData SD){
		return "";
	}
	public virtual string GenerateDomainTransfer(ShaderData SD){
		return "";
	}
	public virtual string GenerateVertex(ShaderData SD){
		return "";
	}
	public virtual string GenerateTransparency(ShaderData SD){
		return "";
	}
	public virtual void CalculatePremultiplication(ShaderData SD){
	}
	public virtual void GeneratePass(ShaderData SD,ShaderPass SP){
	}
	
	public override string GenerateHeader(ShaderData SD, string surfaceInternals,bool Call,string addon){
		bool UsesVertexToPixel = false;
		bool UsesVertex = false;
		bool UsesVertexData = false;
		bool UsesMasks = false;
		
		//Primarily for transparency
		bool UsesPrecalculations = false;
		bool UsesUnityGI = false;
		bool UsesUnityGIInput = false;
		bool UsesOutputColor = false;
		bool UsesDeferredOutput = false;
	
		StringBuilder FunctionHeader = new StringBuilder();
		if (!Call){
			if (SD.ShaderPassIsDeferred&&ShaderGeometryModifierComponent == ShaderGeometryModifierComponent.Transparency){
				//FunctionHeader.Append("DeferredOutput ");
				if (SD.ShaderPassIsDeferred){
					FunctionHeader.Append("DeferredOutput ");
				}else{
					FunctionHeader.Append("float ");
				}
			}else{
				if (ShaderGeometryModifierComponent==ShaderGeometryModifierComponent.Vertex)
					FunctionHeader.Append("void ");
				if (ShaderGeometryModifierComponent==ShaderGeometryModifierComponent.Tessellation)
					FunctionHeader.Append("float4 ");
				if (ShaderGeometryModifierComponent==ShaderGeometryModifierComponent.Transparency)
					FunctionHeader.Append("float4 ");
			}
		}
			
		FunctionHeader.Append(addon);
		FunctionHeader.Append(CodeName);
		FunctionHeader.Append(" (");
		//VertexData vd, Precalculations pc, UnityGI gi, Masks mask
		if (surfaceInternals.IndexOf("vd.")>=0){
			SD.UsesVertexData = true;UsesVertexData = true;}
		if (surfaceInternals.IndexOf("vtp.")>=0){
			UsesVertexToPixel = true;}
		if (surfaceInternals.IndexOf("v.")>=0){
			UsesVertex = true;}
		if (surfaceInternals.IndexOf("mask.")>=0){
			SD.UsesMasks = true;UsesMasks = true;}
			
		if (surfaceInternals.IndexOf("pc.")>=0){
			SD.UsesPrecalculations = true;UsesPrecalculations = true;}
		if (surfaceInternals.IndexOf("gi.")>=0){
			SD.UsesUnityGIVertex = true;UsesUnityGI = true;}
		if (surfaceInternals.IndexOf("giInput")>=0){
			SD.UsesUnityGIInputVertex = true;UsesUnityGIInput = true;}
		if (surfaceInternals.IndexOf("outputColor")>=0){
			UsesOutputColor = true;}
		if (surfaceInternals.IndexOf("deferredOut")>=0)
			UsesDeferredOutput = true;
		
		string asd = " ";
		
		if (Call){
			if (ShaderGeometryModifierComponent==ShaderGeometryModifierComponent.Tessellation){FunctionHeader.Append(asd + "v0, v1, v2");asd = ", ";}
			if (UsesVertexData){FunctionHeader.Append(asd + "vd");asd = ", ";}
			if (UsesVertexToPixel){FunctionHeader.Append(asd + "vtp");asd = ", ";}
			if (UsesVertex){FunctionHeader.Append(asd + "v");asd = ", ";}
			if (UsesMasks){FunctionHeader.Append(asd+"mask");asd = ", ";}
			
			if (UsesPrecalculations){FunctionHeader.Append(asd+"pc");asd = ", ";}
			if (UsesUnityGI){FunctionHeader.Append(asd+"gi");asd = ", ";}
			if (UsesUnityGIInput){FunctionHeader.Append(asd+"giInput");asd = ", ";}
			if (UsesOutputColor){FunctionHeader.Append(asd+"outputColor");asd = ", ";}
			if (UsesDeferredOutput){FunctionHeader.Append(asd+"deferredOut");asd = ", ";}
		}else{
			if (ShaderGeometryModifierComponent==ShaderGeometryModifierComponent.Tessellation){FunctionHeader.Append(asd + "TessellationShaderInput v0, TessellationShaderInput v1, TessellationShaderInput v2");asd = ", ";}
			if (UsesVertexData){FunctionHeader.Append(asd + "inout VertexData vd");asd = ", ";}
			if (UsesVertexToPixel){FunctionHeader.Append(asd + "inout VertexToPixel vtp");asd = ", ";}
			if (UsesVertex){FunctionHeader.Append(asd + "inout VertexShaderInput v");asd = ", ";}
			if (UsesMasks){FunctionHeader.Append(asd+"Masks mask");asd = ", ";}
			
			if (UsesPrecalculations){FunctionHeader.Append(asd+"Precalculations pc");asd = ", ";}
			if (UsesUnityGI){FunctionHeader.Append(asd+"UnityGI gi");asd = ", ";}
			if (UsesUnityGIInput){FunctionHeader.Append(asd+"UnityGIInput giInput");asd = ", ";}
			if (UsesOutputColor){FunctionHeader.Append(asd+"float4 outputColor");asd = ", ";}
			if (UsesDeferredOutput){FunctionHeader.Append(asd+"DeferredOutput deferredOut");asd = ", ";}
		}
		
		FunctionHeader.Append(")");
		if (!Call){
			FunctionHeader.Append("{\n	");
			//if (SD.ShaderPassIsDeferred){
			//	FunctionHeader.Append("DeferredOutput deferredOut;\n	UNITY_INITIALIZE_OUTPUT(DeferredOutput,deferredOut);\n	");
			//}
		}
		return FunctionHeader.ToString();
	}
}
}