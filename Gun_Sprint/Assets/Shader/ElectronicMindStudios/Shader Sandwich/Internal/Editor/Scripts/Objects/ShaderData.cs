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
public enum ShaderPasses{
Mask,
Meta,
ShadowCaster,
ForwardBase,
ForwardAdd,
Deferred,
};
public enum CoordSpace{
	World,
	Object
}
public enum PipelineStage{
	Vertex,Hull,Domain,Fragment,Geometry
}
public enum TessDomain{
	Tri,Quad,Isoline
}
public enum TessPartition{
	Integer, FractionalEven, FractionalOdd, Pow2
}
public enum TessOutputTopology{
	Point, Line, TriangleCW, TriangleCCW
}
public enum BlendFactor{
	None,
	One,				Zero,
	SrcColor,			SrcAlpha,
	DstColor,			DstAlpha,
	OneMinusSrcColor,	OneMinusSrcAlpha,
	OneMinusDstColor,	OneMinusDstAlpha
}
public enum ZWriteMode{
	Auto, On, Off
}
public enum ZTestMode{
	Auto, Always, Less, LEqual, Greater, GEqual, Equal, NotEqual
}
public enum CullMode{
	Back, Front, Off
}
public enum ShaderModel{
	Auto, SM2,SM2_5,SM3,SM3_5,SM4,SM4_5,SM4_6,SM5
}
[System.Serializable,DataContract]
public class ShaderData{
	public ShaderBase Base;
	public ShaderPass Pass;
	
	public bool Temp = false;
	public bool Wireframe = false;
	
	public ShaderPasses ShaderPass = ShaderPasses.ForwardBase;
	
	public CoordSpace CoordSpace = CoordSpace.Object;
	public int GeometryModifierIndex = 0;
	
	public bool ShaderPassIsBaseLighting{
		get{return (ShaderPass == ShaderPasses.ForwardBase||ShaderPass == ShaderPasses.Deferred);}
		set{}
	}
	public bool ShaderPassIsDeferred{
		get{return (ShaderPass == ShaderPasses.Deferred);}
		set{}
	}
	public bool ShaderPassIsForward{
		get{return (ShaderPass == ShaderPasses.ForwardBase||ShaderPass == ShaderPasses.ForwardAdd||ShaderPass == ShaderPasses.ShadowCaster||ShaderPass == ShaderPasses.Mask);}
		set{}
	}
	public bool ShaderPassIsShadowCaster{
		get{return (ShaderPass == ShaderPasses.ShadowCaster);}
		set{}
	}
	public bool ShaderPassIsMask{
		get{return (ShaderPass == ShaderPasses.Mask);}
		set{}
	}
	public bool ShaderPassOnlyZ{
		get{return (ShaderPass == ShaderPasses.ShadowCaster||ShaderPass == ShaderPasses.Mask);}
		set{}
	}
	public bool UsesVertexDataVertexShader = true;
	public bool UsesVertexData = false;
	public bool UsesPrecalculations = false;
	public bool UsesUnityGIFragment = false;
	public bool UsesUnityGIInputFragment = false;
	public bool UsesUnityGIVertex = false;
	public bool UsesUnityGIInputVertex = false;
	public bool UsesMasks = false;
	public bool UsesTessellation = false;
	public bool UsesDepth = false;
	public bool UsesShellsDepth = false;
	public bool UsesPreviousBaseColor = false;
	public bool UsesOneMinusAlpha = false;
	public bool UsesUVOffset = false;
	public bool UsesDepthCorrection = false;
	
	
	public bool Lightmaps = true;
	public bool ForwardShadows = true;
	public bool ForwardFullShadows = true;
	public bool ForwardVertexLights = true;
	public bool AlphaToMask = false;
	public bool ForwardFog = true;
	
	public bool	GenerateForwardBasePass = true;
	public bool	GenerateForwardAddPass = true;
	public bool	GenerateForwardDeferredPass = true;
	public bool	GenerateShadowCaster = true;
	
	public List<ShaderVertexInterpolator> Interpolators = new List<ShaderVertexInterpolator>();
	public List<ShaderVertexInterpolator> VertexInputs = new List<ShaderVertexInterpolator>();
	
	public HashSet<ShaderPlugin> UsedPlugins = new HashSet<ShaderPlugin>();
	public HashSet<ShaderLayerList> UsedMasksVertex = new HashSet<ShaderLayerList>();
	public HashSet<ShaderLayerList> UsedMasksFragment = new HashSet<ShaderLayerList>();
	
	public HashSet<string> AdditionalFunctions = new HashSet<string>();
	public HashSet<string> ShaderAdditionalProperties = new HashSet<string>();
	public HashSet<string> ShaderAdditionalEnd = new HashSet<string>();

	public PipelineStage PipelineStage = PipelineStage.Vertex;
	
	public TessDomain TessDomain = TessDomain.Tri;
	public TessPartition TessPartition = TessPartition.FractionalOdd;
	public TessOutputTopology TessOutputTopology = TessOutputTopology.TriangleCW;
	public int TessControlPoints = 3;
	public BlendFactor BlendFactorSrc = BlendFactor.None;
	public BlendFactor BlendFactorDest = BlendFactor.None;
	
	public ZWriteMode ZWriteMode = ZWriteMode.Auto;
	public ZTestMode ZTestMode = ZTestMode.Auto;
	public CullMode CullMode = CullMode.Back;
	public bool ForceOrigVDMappingHack = false;
	public ShaderModel ShaderModel = ShaderModel.Auto;
	#if UNITY_5_4_OR_NEWER
		public ShaderModel ShaderModelAuto = ShaderModel.SM2_5;
	#else
		public ShaderModel ShaderModelAuto = ShaderModel.SM3;
	#endif
	public bool UsesPremultipliedBlending = true;
	//public bool OutputsPremultipliedBlending = true;
	public bool SelectivePremultipledBlending = false;
	public bool IsTransparent = false;
	
	
	public bool BlendsWeirdlyAnywhere = false;
	public bool ClipsAnywhere = false;
	
	public bool CorrectAlphaBlending = true;
	
	public string[] PredefineStarts = new string[]{"","","","","","",""};
	public string[] PredefineEnds = new string[]{"","","","","","",""};
	public string worthlessref = "";
	public int PredefinesDepth = -1;
	
	public Dictionary<string, object> CustomData = new Dictionary<string, object>();
	
	public bool UsesBlending{
		get{return (BlendFactorSrc == BlendFactor.None||BlendFactorDest == BlendFactor.None);}
		set{}
	}
	public void NewPass(){
		ShaderPass = ShaderPasses.ForwardBase;
		CoordSpace = CoordSpace.Object;
		GeometryModifierIndex = 0;
		
		UsesVertexDataVertexShader = false;
		UsesVertexData = false;
		UsesPrecalculations = false;
		UsesUnityGIFragment = false;
		UsesUnityGIInputFragment = false;
		UsesUnityGIVertex= false;
		UsesUnityGIInputVertex = false;
		UsesMasks = false;
		UsesTessellation = false;
		UsesPreviousBaseColor = false;
		UsesOneMinusAlpha = false;
		UsesDepth = false;
		UsesShellsDepth = false;
		UsesUVOffset = false;
		UsesDepthCorrection = false;
		
		Lightmaps = true;
		ForwardShadows = true;
		ForwardFullShadows = true;
		ForwardVertexLights = true;
		AlphaToMask = false;
		ForwardFog = true;
		
		
		
		GenerateForwardBasePass = true;
		GenerateForwardAddPass = true;
		GenerateForwardDeferredPass = true;
		GenerateShadowCaster = true;
		
		Interpolators.Clear();
		VertexInputs.Clear();
		
		UsedMasksVertex.Clear();
		UsedMasksFragment.Clear();
		
		TessDomain = TessDomain.Tri;
		TessPartition = TessPartition.FractionalOdd;
		TessOutputTopology = TessOutputTopology.TriangleCW;
		TessControlPoints = 3;
		
		BlendFactorSrc = BlendFactor.None;
		BlendFactorDest = BlendFactor.None;
		ZWriteMode = ZWriteMode.Auto;
		ZTestMode = ZTestMode.Auto;
		CullMode = CullMode.Back;
		ShaderModel = ShaderModel.Auto;
		ForceOrigVDMappingHack = false;
		#if UNITY_5_4_OR_NEWER
			ShaderModelAuto = ShaderModel.SM2_5;
		#else
			ShaderModelAuto = ShaderModel.SM3;
		#endif
		
		UsesPremultipliedBlending = true;
		IsTransparent = false;
		//OutputsPremultipliedBlending = true;
		SelectivePremultipledBlending = false;
		CorrectAlphaBlending = true;
		
		PredefineStarts = new string[]{"","","","","","",""};
		PredefineEnds = new string[]{"","","","","","",""};
		worthlessref = "";
		PredefinesDepth = -1;
		AdditionalFunctions = new HashSet<string>();
		//CustomData = new Dictionary<string, object>();
	}
	public static Dictionary<string, object> CopyCD(Dictionary<string, object> CustomData){
		Dictionary<string, object> SCustomData = new Dictionary<string, object>();
		foreach(KeyValuePair<string,object> kv in CustomData){
			SCustomData[kv.Key] = kv.Value;
		}
		return SCustomData;
	}
	public ShaderData Copy(){
		ShaderData SD = new ShaderData();
		SD.Base = Base;
		SD.Pass = Pass;
		
		SD.Temp = Temp;
		SD.Wireframe = Wireframe;
		
		SD.ShaderPass = ShaderPass;
		
		SD.CoordSpace = CoordSpace;
		SD.GeometryModifierIndex = GeometryModifierIndex;
		

		SD.UsesVertexDataVertexShader = UsesVertexDataVertexShader;
		SD.UsesVertexData = UsesVertexData;
		SD.UsesPrecalculations = UsesPrecalculations;
		SD.UsesUnityGIFragment = UsesUnityGIFragment;
		SD.UsesUnityGIInputFragment = UsesUnityGIInputFragment;
		SD.UsesUnityGIVertex = UsesUnityGIVertex;
		SD.UsesUnityGIInputVertex = UsesUnityGIInputVertex;
		SD.UsesMasks = UsesMasks;
		SD.UsesTessellation = UsesTessellation;
		SD.UsesPreviousBaseColor = UsesPreviousBaseColor;
		SD.UsesOneMinusAlpha = UsesOneMinusAlpha;
		//SD.UsesUVOffset = UsesUVOffset;
		SD.UsesDepth = UsesDepth;
		SD.UsesShellsDepth = UsesShellsDepth;
		SD.UsesDepthCorrection = UsesDepthCorrection;
		
		SD.Lightmaps = Lightmaps;
		SD.ForwardShadows = ForwardShadows;
		SD.ForwardFullShadows = ForwardFullShadows;
		SD.ForwardVertexLights = ForwardVertexLights;
		SD.AlphaToMask = AlphaToMask;
		SD.ForwardFog = ForwardFog;
		
		SD.GenerateForwardBasePass = GenerateForwardBasePass;
		SD.GenerateForwardAddPass = GenerateForwardAddPass;
		SD.GenerateForwardDeferredPass = GenerateForwardDeferredPass;
		SD.GenerateShadowCaster = GenerateShadowCaster;
		
		SD.Interpolators = Interpolators;
		SD.VertexInputs = VertexInputs;
		
		SD.UsedPlugins = UsedPlugins;
		SD.UsedMasksVertex = UsedMasksVertex;
		SD.UsedMasksFragment = UsedMasksFragment;
		SD.AdditionalFunctions = AdditionalFunctions;
		SD.ShaderAdditionalProperties = ShaderAdditionalProperties;
		SD.ShaderAdditionalEnd = ShaderAdditionalEnd;
		SD.PipelineStage = PipelineStage;
		
		SD.TessDomain = TessDomain;
		SD.TessPartition = TessPartition;
		SD.TessOutputTopology = TessOutputTopology;
		SD.TessControlPoints = TessControlPoints;
		SD.BlendFactorSrc = BlendFactorSrc;
		SD.BlendFactorDest = BlendFactorDest;
		SD.ZWriteMode = ZWriteMode;
		SD.ZTestMode = ZTestMode;
		SD.CullMode = CullMode;
		SD.ShaderModel = ShaderModel;
		SD.ShaderModelAuto = ShaderModelAuto;
		
		SD.ForceOrigVDMappingHack = ForceOrigVDMappingHack;
		
		SD.UsesPremultipliedBlending = UsesPremultipliedBlending;
		//SD.OutputsPremultipliedBlending = OutputsPremultipliedBlending;
		SD.SelectivePremultipledBlending = SelectivePremultipledBlending;
		SD.IsTransparent = IsTransparent;
		
		SD.BlendsWeirdlyAnywhere = BlendsWeirdlyAnywhere;
		SD.ClipsAnywhere = ClipsAnywhere;
		
		SD.CorrectAlphaBlending = CorrectAlphaBlending;
		
		foreach(KeyValuePair<string,object> kv in CustomData){
			SD.CustomData[kv.Key] = kv.Value;
		}
		
		return SD;
	}
}
}