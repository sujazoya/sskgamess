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
[System.Serializable,DataContract]
public class ShaderGenerate{
	public ShaderBase Base;
	public ShaderPass RPass;
	public VertexSpaces VertexSpace = VertexSpaces.Object;
	public bool OutputsDepth = false;
	public bool UsedPoke = false;
	public bool UsedVertexToastie = false;
	public bool InVertex = false;
	public bool SurfaceShader = true;
	public bool GI = true;
	public bool UsedMapType0 = false;
	public bool UsedMapType1 = false;
	public bool UsedMapType6 = false;
	public bool UsedMapGenerate = false;
	
	public bool UsedParallax = false;
	public bool UsedVertex = false;
	public bool UsedHSV = false;
	public bool UsedVertexColors = false;
	
	public bool UsedLocalWorldPos = false;
	public bool UsedWorldPos = false;
	public bool UsedLocalWorldRefl = false;
	public bool UsedWorldRefl = false;
	public bool UsedLocalWorldNormals = false;
	public bool UsedWorldNormals = false;
	public bool UsedScreenPos = false;
	public bool UsedViewDir = false;
	public bool UsedProjected = false;
	
	public bool UsedGrabPass = false;
	public bool UsedDepthTexture = false;
	
	public bool UsedNoise = false;
	public bool UsedNormalBlend = false;
	public bool UsedShellsLighting = false;
	public bool UsedShellsNormals = false;
	public bool UsedShellsEmission = false;
	
	public bool Float2sCombined = false;
	public bool Float3sCombined = false;
	
	public string oNameNormal = "";
	public string oNameViewDir = "";	
	public string oNameWorldRef = "";
	
	public string[] SBCullNamesCode = {"Off","Back","Front"};
	
	public bool StencilOn = false;
	public bool StencilDefined = false;
	
	public bool MapDispOn = false;
	public bool MapDispDefined = false;	
	
	public bool Type2UsedAlbedo = false;
	public bool Type2UsedAlpha = false;
	public bool Type2UsedEmission = false;
	public bool Type2UsedNormal = false;
	public bool Type2UsedShellNormal = false;
	public bool Type2UsedShellEmission = false;
	public bool Type2UsedNormalBlend = false;
	public bool UsedTempNormals = false;
	public bool UsedGenericUV = false;
	
	public bool Type2UsedVertex = false;
	
	public int DiffuseInput = 0;
	public int NormalInput = 0;
	public int EmissionInput = 0;
	public int HeightInput = 0;
	public int AlphaInput = 0;
	public int VertexInput = 0;
	public int SpecularInput = 0;
	
	public bool UsedNormals = false;
	public bool TooManyTexcoords = false;
	
	//public string GeneralUV = "GENUV";
	public string GeneralUV = "genericTexcoord";
	
	public bool Temp = false;
	public bool Wireframe = false;
	
	public Dictionary<string,int> UsedBases = new Dictionary<string,int>();
	public Dictionary<ShaderLayerList,int> UsedMasks = new Dictionary<ShaderLayerList,int>();
	
	public List<ShaderPlugin> UsedPlugins = new List<ShaderPlugin>();
	
	public bool PassSetNormals = false;
	public bool UseTangentSpace = false;
	public bool UseLighting = false;
	public string Pass = "ForwardBase";
	public string Function = "ForwardBase";
	public string PassSubset = "";
	public float Dist = 0f;
	
	public bool U4 = false;
	public void ResetPass(){
		PassSetNormals = false;
		UseTangentSpace = false;
		UseLighting = false;
		Pass = "ForwardBase";
		Function = "Fragment";
		UsedParallax = false;
		VertexSpace = VertexSpaces.Object;
	}
	public void Reset(){
		VertexSpace = VertexSpaces.Object;
		OutputsDepth = false;
		UsedVertexToastie = false;
		InVertex = false;
		UsedPlugins.Clear();
		UsedBases.Clear();
		UsedMapType0 = false;
		UsedMapType1 = false;
		UsedMapType6 = false;
		UsedMapGenerate = false;
		
		UsedParallax = false;
		UsedVertex = false;
		UsedHSV = false;
		UsedVertexColors = false;
		
		UsedLocalWorldPos = false;
		UsedWorldPos = false;
		UsedLocalWorldRefl = false;
		UsedWorldRefl = false;
		UsedLocalWorldNormals = false;
		UsedWorldNormals = false;
		
		UsedNoise = false;
		UsedNormalBlend = false;
		UsedShellsLighting = false;
		UsedShellsNormals = false;
		UsedShellsEmission = false;
		
		
		oNameNormal = "";
		oNameViewDir = "";	
		oNameWorldRef = "";
		
		SBCullNamesCode = new string[]{"Off","Back","Front"};
		
		StencilOn = false;
		StencilDefined = false;
		TooManyTexcoords = false;
		
		MapDispOn = false;
		MapDispDefined = false;	
		
		Type2UsedAlbedo = false;
		Type2UsedAlpha = false;
		Type2UsedEmission = false;
		Type2UsedNormal = false;
		Type2UsedShellNormal = false;
		Type2UsedShellEmission = false;
		Type2UsedNormalBlend = false;
		UsedTempNormals = false;
		UsedGenericUV = false;
		Type2UsedVertex = false;
		UsedNormals = false;
		UsedShellsNormals = false;
		
		Float2sCombined = false;
		Float3sCombined = false;

		GeneralUV = "GENUV";
	
	}
	/*public ShaderGenerate(){
	
		NameInputs =new List<int>();
		NameInputs.Add(0);
		NameInputs.Add(0);
		NameInputs.Add(0);
		NameInputs.Add(0);
		NameInputs.Add(0);
		NameInputs.Add(0);
		NameInputs.Add(0);
		
		NameToInt.Add("Diffuse",0);
		NameToInt.Add("Alpha",1);
		NameToInt.Add("Normals",2);
		NameToInt.Add("Specular",3);
		NameToInt.Add("Emission",4);
		NameToInt.Add("Height",5);
		NameToInt.Add("Vertex",6);
	}*/
	
}
}