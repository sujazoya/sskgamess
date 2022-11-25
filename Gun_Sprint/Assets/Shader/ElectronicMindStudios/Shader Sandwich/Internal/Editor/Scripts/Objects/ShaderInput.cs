using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using SU = ElectronicMind.Sandwich.ShaderUtil;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
//Image Color Cubemap Float Range
public enum ShaderInputTypes{Image,Vector,Color,Cubemap,Float,Range,Header,Space};

public enum InputMainTypes {
None,Custom,

MainColor,Tint,SpecularColor,ReflectColor,EmissionColor,

MainTexture,BumpMap,ParallaxMap,MetallicGlossMap,TerrainControl,TerrainSplat0,TerrainSplat1,TerrainSplat2,TerrainSplat3,TerrainNormal0,TerrainNormal1,TerrainNormal2,TerrainNormal3,OcclusionMap,EmissionMap,DetailMask,DetailAlbedoMap,DetailNormalMap,DecalTex,SpecGlossMap,

Rotation,Exposure,Shininess,Metallic,Glossiness,Parallax,Cutoff,ShellDistance,BumpScale,OcclusionStrength,DetailNormalMapScale,

MainCubemap,
};
public enum InputSpecialTypes {
None,

Time,
TimeStandard,
TimeDiv20,
TimeMul2,
TimeMul3,
SinTime,
SinTimeStandard,
SinTimeDiv2,
SinTimeDiv4,
SinTimeDiv8,
CosTime,
CosTimeStandard,
CosTimeDiv2,
CosTimeDiv4,
CosTimeDiv8,
ClampedSinTime,
ClampedSinTimeStandard,
ClampedSinTimeDiv2,
ClampedSinTimeDiv4,
ClampedSinTimeDiv8,
ClampedCosTime,
ClampedCosTimeStandard,
ClampedCosTimeDiv2,
ClampedCosTimeDiv4,
ClampedCosTimeDiv8,

ShellDepthNormalized,ParallaxDepth,
ShellDepthInvertedNormalized,ParallaxDepthNormalized,

SpecCube0,SpecCube1,
FogColor,FogParamaters,

Lightmap,LightmapInd,
DynamicLightmap,DynamicDirectionality,DynamicNormal,

ScreenParamaters,
ScreenWidth,
ScreenHeight,
ScreenWidthInv,
ScreenHeightInv,
ScreenWidthInvPlusOne,
ScreenHeightInvPlusOne,
ScreenWidthDivScreenHeight,
ScreenHeightDivScreenWidth,

Custom,Mask,
};
public enum DefaultTextures {White,Grey,Black,Red,Bump};
[System.Serializable]
public class ShaderInput : ScriptableObject{// : UnityEngine.Object{
	public enum OldInputMainTypes {None,MainColor,MainTexture,BumpMap,MainCubemap,SpecularColor,Shininess,ReflectColor,Parallax,ParallaxMap,Cutoff,ShellDistance,TerrainControl,TerrainSplat0,TerrainSplat1,TerrainSplat2,TerrainSplat3,TerrainNormal0,TerrainNormal1,TerrainNormal2,TerrainNormal3,Custom};
	public enum OldInputSpecialTypes {None,Time,TimeFast,TimeSlow,TimeVerySlow,SinTime,SinTimeFast,SinTimeSlow,CosTime,CosTimeFast,CosTimeSlow,ShellDepth,ParallaxDepth,ClampedSinTime,ClampedSinTimeFast,ClampedSinTimeSlow,ClampedCosTime,ClampedCosTimeFast,ClampedCosTimeSlow,Custom,Mask};
	public enum OldShaderInputTypes{Image,Color,Cubemap,Float,Range};
	//public int Type;//Why is this not an enum????
	public ShaderInputTypes Type;//Haha suck it!
	//public string Name;//What was this variable even here for?????
	public string VisName = "";
	//public ShaderVar Image = new ShaderVar("Texture2D");
	[XmlIgnore,NonSerialized] public Texture2D Image;
	public int ImageDefault;
	public string[] ImageDefaultNames = {"white","bump"};
	[DataMember]
	public ShaderColor Color =  new ShaderColor(0.8f,0.8f,0.8f,1f);
	//public ShaderVar Cube = new ShaderVar("Cubemap");
	[XmlIgnore,NonSerialized] public Cubemap Cube;
	
	public string ImageGUID = "";
	
	public string CubeGUID = "";
	public float Number; 
	public float Range0; 
	public float Range1; 
	public bool UsedMapType6;
	public bool UsedMapType1;
	public bool UsedMapType0 = true;
	public bool InEditor = true;
	
	public bool NormalMap = false;
	public bool IsHDR;
	public bool IsGamma;
	public DefaultTextures DefaultTexture;
	
	public bool SeeTilingOffset = true;
	public Vector2 Tiling = new Vector2(1f,1f);
	public Vector2 Offset = new Vector2(0,0);
	
	public ShaderVar CustomFallback = new ShaderVar("CustomFallback","");
	public ShaderVar CustomSpecial = new ShaderVar("CustomSpecial","1");
	
	public ShaderVar Mask;
	
	public InputMainTypes MainType = InputMainTypes.None;
	public InputSpecialTypes SpecialType = InputSpecialTypes.None;
	public float InputScale = 1f;

	public bool AutoCreated = false;
	public int UsedCount = 0;
	
	public void Update(){
		float time = (float)EditorApplication.timeSinceStartup;   
		Vector4 vTime = new Vector4(( time / 20f)%1000f, time%1000f, (time*2f)%1000f, (time*3f)%1000f);
		Vector4 vCosTime = new Vector4( Mathf.Cos(time / 8f), Mathf.Cos(time/4f), Mathf.Cos(time/2f), Mathf.Cos(time));
		Vector4 vSinTime = new Vector4( Mathf.Sin(time / 8f), Mathf.Sin(time/4f), Mathf.Sin(time/2f), Mathf.Sin(time));
		float NewNumber = -97.5f;
		if (!InEditor){
			switch(SpecialType){
				case InputSpecialTypes.Time:
					NewNumber = vTime.y*InputScale;break;
				case InputSpecialTypes.TimeStandard:
					NewNumber = vTime.y;break;
				case InputSpecialTypes.TimeDiv20:
					NewNumber = vTime.x;break;
				case InputSpecialTypes.TimeMul2:
					NewNumber = vTime.z;break;
				case InputSpecialTypes.TimeMul3:
					NewNumber = vTime.w;break;
				case InputSpecialTypes.SinTime:
					NewNumber = Mathf.Sin(vTime.y*InputScale);break;
				case InputSpecialTypes.SinTimeStandard:
					NewNumber = vSinTime.w;break;
				case InputSpecialTypes.SinTimeDiv2:
					NewNumber = vSinTime.z;break;
				case InputSpecialTypes.SinTimeDiv4:
					NewNumber = vSinTime.y;break;
				case InputSpecialTypes.SinTimeDiv8:
					NewNumber = vSinTime.x;break;
				case InputSpecialTypes.CosTime:
					NewNumber = Mathf.Cos(vTime.y*InputScale);break;
				case InputSpecialTypes.CosTimeStandard:
					NewNumber = vCosTime.w;break;
				case InputSpecialTypes.CosTimeDiv2:
					NewNumber = vCosTime.z;break;
				case InputSpecialTypes.CosTimeDiv4:
					NewNumber = vCosTime.y;break;
				case InputSpecialTypes.CosTimeDiv8:
					NewNumber = vCosTime.x;break;
					
				case InputSpecialTypes.ClampedSinTime:
					NewNumber = (Mathf.Sin(vTime.y*InputScale)+1f)/2f;break;
				case InputSpecialTypes.ClampedSinTimeStandard:
					NewNumber = (vSinTime.w+1f)/2f;break;
				case InputSpecialTypes.ClampedSinTimeDiv2:
					NewNumber = (vSinTime.z+1f)/2f;break;
				case InputSpecialTypes.ClampedSinTimeDiv4:
					NewNumber = (vSinTime.y+1f)/2f;break;
				case InputSpecialTypes.ClampedSinTimeDiv8:
					NewNumber = (vSinTime.x+1f)/2f;break;
					
				case InputSpecialTypes.ClampedCosTime:
					NewNumber = (Mathf.Cos(vTime.y*InputScale)+1f)/2f;break;
				case InputSpecialTypes.ClampedCosTimeStandard:
					NewNumber = (vCosTime.w+1f)/2f;break;
				case InputSpecialTypes.ClampedCosTimeDiv2:
					NewNumber = (vCosTime.z+1f)/2f;break;
				case InputSpecialTypes.ClampedCosTimeDiv4:
					NewNumber = (vCosTime.y+1f)/2f;break;
				case InputSpecialTypes.ClampedCosTimeDiv8:
					NewNumber = (vCosTime.x+1f)/2f;break;
			}
		}
		if (NewNumber!=-97.5f){
			Number = NewNumber;
			
		}
		if (MainType!=InputMainTypes.Custom)
			CustomFallback.Text = Get();
	}
	
	public Texture2D ImageS(){
		if (Image==null)
			Image = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(ImageGUID),typeof(Texture2D));
		//Debug.Log(AssetDatabase.GUIDToAssetPath(ImageGUID));
		return Image;
	}
	public Cubemap CubeS(){
		if (Cube==null)
			Cube = (Cubemap)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(CubeGUID),typeof(Cubemap));
		return Cube;
	}
	public ShaderInput(){
		//Mask.SetToMasks(null,0);
		//if (Type<=2)
		//	Mask.RGBAMasks = true;
	}
	public void Awake(){
		Mask = new ShaderVar("Mask","ListOfObjects");
	}
	public void OnEnable(){
		//Mask.SetToMasks(null,0);
		Mask.HookIntoObjects();
		Mask.NoInputs = true;
		//if (Type<=2)
		//	Mask.RGBAMasks = true;
	}
	public ShaderInput(bool A){
		AutoCreated = A;
		//Mask.SetToMasks(null,0);
		Mask.HookIntoObjects();
		Mask.NoInputs = true;
		//if (Type<=2)
		//	Mask.RGBAMasks = true;
	}
	public string GetDecorators(){
		string Decorators = "";
		if (!SeeTilingOffset)
			Decorators+="[NoScaleOffset]";
		if (IsGamma)
			Decorators+="[Gamma]";
		if (IsHDR)
			Decorators+="[HDR]";
		if (Decorators.Length>0)
			Decorators+=" ";
		
		return Decorators;
	}
	static public string[] PreExistingUniforms = new string[]{
		"_Time",
		"_SinTime",
		"_CosTime",
		"unity_DeltaTime",
		"_WorldSpaceCameraPos",
		"_ProjectionParams",
		"_ScreenParams",
		"_ZBufferParams",
		"unity_OrthoParams",
		"unity_CameraProjection",
		"unity_CameraInvProjection",
		"_WorldSpaceLightPos0",
		"unity_4LightPosX0",
		"unity_4LightPosY0",
		"unity_4LightPosZ0",
		"unity_4LightAtten0",
		"unity_LightColor",
		"unity_SHAr",
		"unity_SHAg",
		"unity_SHAb",
		"unity_SHBr",
		"unity_SHBg",
		"unity_SHBb",
		"unity_SHC",
		"_SpecColor",
	};
	public bool UsesInputScale(){
			if (SpecialType==InputSpecialTypes.Time)return true;
			if (SpecialType==InputSpecialTypes.SinTime)return true;
			if (SpecialType==InputSpecialTypes.SinTime)return true;
			if (SpecialType==InputSpecialTypes.CosTime)return true;
			if (SpecialType==InputSpecialTypes.CosTime)return true;
			if (SpecialType==InputSpecialTypes.ClampedSinTime)return true;;
			if (SpecialType==InputSpecialTypes.ClampedSinTime)return true;
			if (SpecialType==InputSpecialTypes.ClampedCosTime)return true;
			if (SpecialType==InputSpecialTypes.ClampedCosTime)return true;
		return false;
	}
	public string Get(){
		string NewName;
		if (!PreExistingUniforms.Contains(ShaderUtil.CodeName(VisName)))
			NewName = "_"+ShaderUtil.CodeName(VisName);
		else
			NewName = "_"+ShaderUtil.CodeName("SS"+VisName);
		
		if (InEditor){
			if (MainType==InputMainTypes.MainColor)NewName = "_Color";
			if (MainType==InputMainTypes.Tint)NewName = "_Tint";
			if (MainType==InputMainTypes.SpecularColor)NewName = "_SpecColor";
			if (MainType==InputMainTypes.ReflectColor)NewName = "_ReflectColor";
			if (MainType==InputMainTypes.ReflectColor)NewName = "_EmissionColor";
			
			if (MainType==InputMainTypes.MainTexture)NewName = "_MainTex";
			if (MainType==InputMainTypes.BumpMap)NewName = "_BumpMap";
			if (MainType==InputMainTypes.ParallaxMap)NewName = "_ParallaxMap";
			if (MainType==InputMainTypes.MetallicGlossMap)NewName = "_MetallicGlossMap";
			if (MainType==InputMainTypes.TerrainControl)NewName = "_Control";
			if (MainType==InputMainTypes.TerrainSplat0)NewName = "_Splat0";
			if (MainType==InputMainTypes.TerrainSplat1)NewName = "_Splat1";
			if (MainType==InputMainTypes.TerrainSplat2)NewName = "_Splat2";
			if (MainType==InputMainTypes.TerrainSplat3)NewName = "_Splat3";
			if (MainType==InputMainTypes.TerrainNormal0)NewName = "_Normal0";
			if (MainType==InputMainTypes.TerrainNormal1)NewName = "_Normal1";
			if (MainType==InputMainTypes.TerrainNormal2)NewName = "_Normal2";
			if (MainType==InputMainTypes.TerrainNormal3)NewName = "_Normal3";
			if (MainType==InputMainTypes.OcclusionMap)NewName = "_OcclusionMap";
			if (MainType==InputMainTypes.EmissionMap)NewName = "_EmissionMap";
			if (MainType==InputMainTypes.DetailMask)NewName = "_DetailMask";
			if (MainType==InputMainTypes.DetailAlbedoMap)NewName = "_DetailAlbedoMap";
			if (MainType==InputMainTypes.DetailNormalMap)NewName = "_DetailNormalMap";
			if (MainType==InputMainTypes.DecalTex)NewName = "_DecalTex";
			if (MainType==InputMainTypes.SpecGlossMap)NewName = "_SpecGlossMap";
			
			if (MainType==InputMainTypes.Rotation)NewName = "_Rotation";
			if (MainType==InputMainTypes.Exposure)NewName = "_Exposure";
			if (MainType==InputMainTypes.Metallic)NewName = "_Metallic";
			if (MainType==InputMainTypes.Glossiness)NewName = "_Glossiness";
			if (MainType==InputMainTypes.Shininess)NewName = "_Shininess";
			if (MainType==InputMainTypes.Cutoff)NewName = "_Cutoff";
			if (MainType==InputMainTypes.ShellDistance)NewName = "_ShellDistance";
			if (MainType==InputMainTypes.Parallax)NewName = "_Parallax";
			if (MainType==InputMainTypes.BumpScale)NewName = "_BumpScale";
			if (MainType==InputMainTypes.OcclusionStrength)NewName = "_OcclusionStrength";
			if (MainType==InputMainTypes.DetailNormalMapScale)NewName = "_DetailNormalMapScale";
			
			if (MainType==InputMainTypes.MainCubemap)NewName = "_Cube";

			if (MainType==InputMainTypes.Custom)NewName = CustomFallback.Text;
		}else{//InputScale
			string scale = "";
			if (InputScale!=1f)
				scale = " * "+InputScale.ToString();
			if (SpecialType==InputSpecialTypes.Time)NewName = "_Time.y"+scale;
			if (SpecialType==InputSpecialTypes.SinTime&&scale!="")NewName = "sin(_Time.y"+scale+")";
			if (SpecialType==InputSpecialTypes.SinTime&&scale=="")NewName = "_SinTime.w";
			if (SpecialType==InputSpecialTypes.CosTime&&scale!="")NewName = "cos(_Time.y"+scale+")";
			if (SpecialType==InputSpecialTypes.CosTime&&scale=="")NewName = "_CosTime.w";
			if (SpecialType==InputSpecialTypes.ClampedSinTime&&scale!="")NewName = "(sin(_Time.y"+scale+")+1)*0.5";
			if (SpecialType==InputSpecialTypes.ClampedSinTime&&scale=="")NewName = "(_SinTime.w+1)*0.5";
			if (SpecialType==InputSpecialTypes.ClampedCosTime&&scale!="")NewName = "(cos(_Time.y"+scale+")+1)*0.5";
			if (SpecialType==InputSpecialTypes.ClampedCosTime&&scale=="")NewName = "(_CosTime.w+1)*0.5";
			
			if (SpecialType==InputSpecialTypes.TimeStandard)NewName = "_Time.y";
			if (SpecialType==InputSpecialTypes.TimeDiv20)NewName = "_Time.x";
			if (SpecialType==InputSpecialTypes.TimeMul2)NewName = "_Time.z";
			if (SpecialType==InputSpecialTypes.TimeMul3)NewName = "_Time.w";
			
			if (SpecialType==InputSpecialTypes.SinTimeStandard)NewName = "_SinTime.w";
			if (SpecialType==InputSpecialTypes.SinTimeDiv2)NewName = "_SinTime.z";
			if (SpecialType==InputSpecialTypes.SinTimeDiv4)NewName = "_SinTime.y";
			if (SpecialType==InputSpecialTypes.SinTimeDiv8)NewName = "_SinTime.x";
			
			if (SpecialType==InputSpecialTypes.CosTimeStandard)NewName = "_CosTime.w";
			if (SpecialType==InputSpecialTypes.CosTimeDiv2)NewName = "_CosTime.z";
			if (SpecialType==InputSpecialTypes.CosTimeDiv4)NewName = "_CosTime.y";
			if (SpecialType==InputSpecialTypes.CosTimeDiv8)NewName = "_CosTime.x";
			
			if (SpecialType==InputSpecialTypes.ClampedSinTimeStandard)NewName = "(_SinTime.w+1)*0.5";
			if (SpecialType==InputSpecialTypes.ClampedSinTimeDiv2)NewName = "(_SinTime.z+1)*0.5";
			if (SpecialType==InputSpecialTypes.ClampedSinTimeDiv4)NewName = "(_SinTime.y+1)*0.5";
			if (SpecialType==InputSpecialTypes.ClampedSinTimeDiv8)NewName = "(_SinTime.x+1)*0.5";
			
			if (SpecialType==InputSpecialTypes.ClampedCosTimeStandard)NewName = "(_CosTime.w+1)*0.5";
			if (SpecialType==InputSpecialTypes.ClampedCosTimeDiv2)NewName = "(_CosTime.z+1)*0.5";
			if (SpecialType==InputSpecialTypes.ClampedCosTimeDiv4)NewName = "(_CosTime.y+1)*0.5";
			if (SpecialType==InputSpecialTypes.ClampedCosTimeDiv8)NewName = "(_CosTime.x+1)*0.5";
			
			
			if (SpecialType==InputSpecialTypes.ParallaxDepth)NewName = "outputDepth.y";
			if (SpecialType==InputSpecialTypes.ParallaxDepthNormalized)NewName = "outputDepth.z";
			/*if (SpecialType==InputSpecialTypes.ShellDepth){
				if (ShaderSandwich.Instance.OpenShader!=null&&ShaderSandwich.Instance.OpenShader.SD!=null&&
				ShaderSandwich.Instance.OpenShader.SD.CustomData.ContainsKey("ShellDepth")){
					NewName = (string)ShaderSandwich.Instance.OpenShader.SD.CustomData["ShellDepth"];
				}else{
					NewName = "0";
				}
			}*/
			if (SpecialType==InputSpecialTypes.ShellDepthNormalized){
				if (ShaderSandwich.Instance.OpenShader!=null&&ShaderSandwich.Instance.OpenShader.SD!=null&&
				ShaderSandwich.Instance.OpenShader.SD.CustomData.ContainsKey("ShellDepthNormalized")){
					NewName = (string)ShaderSandwich.Instance.OpenShader.SD.CustomData["ShellDepthNormalized"];
				}else{
					NewName = "0";
				}
			}
			if (SpecialType==InputSpecialTypes.ShellDepthInvertedNormalized){
				if (ShaderSandwich.Instance.OpenShader!=null&&ShaderSandwich.Instance.OpenShader.SD!=null&&
				ShaderSandwich.Instance.OpenShader.SD.CustomData.ContainsKey("ShellDepthInvertedNormalized")){
					NewName = (string)ShaderSandwich.Instance.OpenShader.SD.CustomData["ShellDepthInvertedNormalized"];
				}else{
					NewName = "1";
				}
			}
			//if (SpecialType==InputSpecialTypes.ShellDepth)NewName = "vd.ShellDepth";
			//if (SpecialType==InputSpecialTypes.ShellDepthNormalized)NewName = "vd.ShellDepthNormalized";
			
			if (SpecialType==InputSpecialTypes.SpecCube0)NewName = "UNITY_PASS_TEXCUBE(unity_SpecCube0)";
			if (SpecialType==InputSpecialTypes.SpecCube1)NewName = "UNITY_PASS_TEXCUBE(unity_SpecCube1)";
			
			if (SpecialType==InputSpecialTypes.FogColor)NewName = "unity_FogColor";
			if (SpecialType==InputSpecialTypes.FogParamaters)NewName = "unity_FogParams";
			
			if (SpecialType==InputSpecialTypes.Lightmap)NewName = "unity_Lightmap";
			if (SpecialType==InputSpecialTypes.LightmapInd)NewName = "unity_LightmapInd";
			
			if (SpecialType==InputSpecialTypes.DynamicLightmap)NewName = "unity_DynamicLightmap";
			if (SpecialType==InputSpecialTypes.DynamicDirectionality)NewName = "unity_DynamicDirectionality";
			if (SpecialType==InputSpecialTypes.DynamicNormal)NewName = "unity_DynamicNormal";
			
			if (SpecialType==InputSpecialTypes.ScreenParamaters)NewName = "_ScreenParams";
			if (SpecialType==InputSpecialTypes.ScreenWidth)NewName = "_ScreenParams.x";
			if (SpecialType==InputSpecialTypes.ScreenHeight)NewName = "_ScreenParams.y";
			if (SpecialType==InputSpecialTypes.ScreenWidthInv)NewName = "1/_ScreenParams.x";
			if (SpecialType==InputSpecialTypes.ScreenHeightInv)NewName = "1/_ScreenParams.y";
			if (SpecialType==InputSpecialTypes.ScreenWidthInvPlusOne)NewName = "_ScreenParams.z";
			if (SpecialType==InputSpecialTypes.ScreenHeightInvPlusOne)NewName = "_ScreenParams.w";
			
			if (SpecialType==InputSpecialTypes.ScreenWidthDivScreenHeight)NewName = "_ScreenParams.x/_ScreenParams.y";
			if (SpecialType==InputSpecialTypes.ScreenHeightDivScreenWidth)NewName = "_ScreenParams.y/_ScreenParams.x";
			
			if (SpecialType==InputSpecialTypes.Custom)NewName = CustomSpecial.Text;
			
			if (SpecialType==InputSpecialTypes.Mask)NewName = Mask.GetMaskName();
		}
		
		return NewName;
	}
	public Dictionary<string,ShaderVar> GetSaveLoadDict(bool GeneralLoad){
		Dictionary<string,ShaderVar> D = new Dictionary<string,ShaderVar>();
		//Create some temp Shader Vars (Ammendum 10/5/2017)Because I'm a terrible programmer...bad Sean!
		//Why I'm not just using Shader Vars already I'll never know...
		ShaderVar SVType = new ShaderVar("Type","");
		SVType.Text = Type.ToString();
		D.Add(SVType.Name,SVType);
		
		ShaderVar SVVisName = new ShaderVar("VisName",VisName);
		D.Add(SVVisName.Name,SVVisName);
		
		if (Type == ShaderInputTypes.Image||GeneralLoad){
			ShaderVar SVImageDefault = new ShaderVar("ImageDefault",ImageDefault);
			D.Add(SVImageDefault.Name,SVImageDefault);
			
			ShaderVar SVImage = new ShaderVar("Image",AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(Image)));
			D.Add(SVImage.Name,SVImage);
			
			ShaderVar SVNormalMap = new ShaderVar("NormalMap",NormalMap?1f:0f);
			D.Add(SVNormalMap.Name,SVNormalMap);
			
			ShaderVar SVDefaultTexture = new ShaderVar("DefaultTexture",DefaultTexture.ToString());
			D.Add(SVDefaultTexture.Name,SVDefaultTexture);
		}
		if (Type == ShaderInputTypes.Cubemap||GeneralLoad){
			ShaderVar SVCube = new ShaderVar("Cube",AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(Cube)));
			D.Add(SVCube.Name,SVCube);
		}
		
		if (Type == ShaderInputTypes.Image||Type == ShaderInputTypes.Cubemap||GeneralLoad){
			ShaderVar SVSeeTilingOffset = new ShaderVar("SeeTilingOffset",SeeTilingOffset);
			D[SVSeeTilingOffset.Name] = SVSeeTilingOffset;
			
			ShaderVar SVTilingOffset = new ShaderVar("TilingOffset",new ShaderColor(Tiling.x,Tiling.y,Offset.x,Offset.y));
			D[SVTilingOffset.Name] = SVTilingOffset;
		}
		
		if (Type == ShaderInputTypes.Color||Type == ShaderInputTypes.Vector||GeneralLoad){
			ShaderVar SVColor = new ShaderVar("Color",new Color(0.8f,0.8f,0.8f,1f));
			if (Color!=null)
			SVColor = new ShaderVar("Color",Color.ToColor());
		
			D.Add(SVColor.Name,SVColor);
		}
		if (IsHDR||GeneralLoad){
				ShaderVar SVIsHDR = new ShaderVar("IsHDR",IsHDR);
				D.Add(SVIsHDR.Name,SVIsHDR);
		}
		if (Type == ShaderInputTypes.Float||Type == ShaderInputTypes.Range||GeneralLoad){
			ShaderVar SVNumber = new ShaderVar("Number",Number);
			D.Add(SVNumber.Name,SVNumber);
			
			ShaderVar SVRange0 = new ShaderVar("Range0",Range0);
			D.Add(SVRange0.Name,SVRange0);
			
			ShaderVar SVRange1 = new ShaderVar("Range1",Range1);
			D.Add(SVRange1.Name,SVRange1);
			
			if (IsGamma||GeneralLoad){
				ShaderVar SVIsGamma = new ShaderVar("IsGamma",IsGamma);
				D.Add(SVIsGamma.Name,SVIsGamma);
			}
		}
		
		if (MainType != InputMainTypes.None||GeneralLoad){
			ShaderVar SVMainType = new ShaderVar("MainType",MainType.ToString());
			D.Add(SVMainType.Name,SVMainType);
		}
		
		if (SpecialType != InputSpecialTypes.None||GeneralLoad){
			ShaderVar SVSpecialType = new ShaderVar("SpecialType",SpecialType.ToString());
			D.Add(SVSpecialType.Name,SVSpecialType);
		}
		if (InputScale!=1f||GeneralLoad){
			ShaderVar SVInputScale = new ShaderVar("InputScale",InputScale);
			D.Add(SVInputScale.Name,SVInputScale);
		}
		
		if (InEditor!=true||GeneralLoad){
			ShaderVar SVInEditor = new ShaderVar("InEditor",InEditor?1f:0f);
			D.Add(SVInEditor.Name,SVInEditor);
		}

		
		//Oh NOW I have actual shader vars!? I... ;(
		if (CustomFallback.Text!=""||GeneralLoad)
			D.Add(CustomFallback.Name,CustomFallback);
		if (CustomSpecial.Text!="1"||GeneralLoad)
			D.Add(CustomSpecial.Name,CustomSpecial);
		
		if (Mask.Obj!=null||GeneralLoad)
			D.Add(Mask.Name,Mask);
		
		
		return D;
	}
	public string Save(){
		string S = "\n		Begin Shader Input\n";

		S += ShaderUtil.SaveDict(GetSaveLoadDict(false),3);
		S += "		End Shader Input\n\n";
		return S;
	}
	static public ShaderInput Load(StringReader S){
		ShaderInput SI = ShaderInput.CreateInstance<ShaderInput>();
		var D = SI.GetSaveLoadDict(true);
		while(1==1){
			string Line =  ShaderUtil.Sanitize(S.ReadLine());

			if (Line!=null){
				if(Line=="End Shader Input")break;
				
				if (Line.Contains(":"))
				ShaderUtil.LoadLine(D,Line);
			}
			else
			break;
		}
		if (D["Type"].CType==Types.Float)
			SI.Type = (ShaderInputTypes)(int)D["Type"].Float;
		else
			SI.Type = (ShaderInputTypes)Enum.Parse(typeof(ShaderInputTypes),D["Type"].Text);
		SI.VisName = D["VisName"].Text;
		SI.ImageDefault = (int)D["ImageDefault"].Float;
		SI.ImageGUID = D["Image"].Text;
		SI.Image = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(D["Image"].Text),typeof(Texture2D));
		SI.CubeGUID = D["Cube"].Text;
		SI.Cube = (Cubemap)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(D["Cube"].Text),typeof(Cubemap));
		SI.Color = D["Color"].Vector;
		SI.Number = D["Number"].Float;
		SI.Range0 = D["Range0"].Float;
		SI.Range1 = D["Range1"].Float;
		SI.MainType = (InputMainTypes)Enum.Parse(typeof(InputMainTypes),D["MainType"].Text);
		SI.SpecialType = (InputSpecialTypes)Enum.Parse(typeof(InputSpecialTypes),D["SpecialType"].Text);
		SI.InputScale = D["InputScale"].Float;
		if (D.ContainsKey("InEditor")&&D["InEditor"].Float==0f)
			SI.InEditor = false;
		else
			SI.InEditor = true;
		
		if (D.ContainsKey("NormalMap")&&D["NormalMap"].Float==0f)
			SI.NormalMap = false;
		else
			SI.NormalMap = true;
		
		if (D.ContainsKey("DefaultTexture"))
			SI.DefaultTexture = (DefaultTextures)Enum.Parse(typeof(DefaultTextures),D["DefaultTexture"].Text);
		
		if (D.ContainsKey("IsHDR"))
			SI.IsHDR = D["IsHDR"].On;
		
		if (D.ContainsKey("IsGamma"))
			SI.IsGamma = D["IsGamma"].On;
	
		
		if (D.ContainsKey("SeeTilingOffset"))
			SI.SeeTilingOffset = D["SeeTilingOffset"].On;
	
		if (D.ContainsKey("TilingOffset")){
			SI.Tiling.x = D["TilingOffset"].Vector.r;
			SI.Tiling.y = D["TilingOffset"].Vector.g;
			SI.Offset.x = D["TilingOffset"].Vector.b;
			SI.Offset.y = D["TilingOffset"].Vector.a;
		}
		
		return SI;
	}
	static public ShaderInput LoadLegacy(StringReader S){
		ShaderInput SI = ShaderInput.CreateInstance<ShaderInput>();//UpdateGradient
		var D = SI.GetSaveLoadDict(true);
		while(1==1){
			string Line =  ShaderUtil.Sanitize(S.ReadLine());

			if (Line!=null){
				if(Line=="EndShaderInput")break;
				
				if (Line.Contains("#!"))
				ShaderUtil.LoadLineLegacy(D,Line);
			}
			else
			break;
		}
		
/*		ShaderVar SVType = new ShaderVar("Type",(float)Type);
		ShaderVar SVVisName = new ShaderVar("VisName",VisName);
		ShaderVar SVImageDefault = new ShaderVar("ImageDefault",ImageDefault);
		ShaderVar SVColor = new ShaderVar("Color",Color.ToColor());
		ShaderVar SVNumber = new ShaderVar("Number",Number);
		ShaderVar SVRange0 = new ShaderVar("Range0",Range0);
		ShaderVar SVRange1 = new ShaderVar("Range1",Range1);
		ShaderVar SVMainType = new ShaderVar("MainType",(float)(int)MainType);
		ShaderVar SVSpecialType = new ShaderVar("SpecialType",(float)(int)SpecialType);*/
		SI.Type = (ShaderInputTypes)Enum.Parse(typeof(ShaderInputTypes),((OldShaderInputTypes)(int)D["Type"].Float).ToString());
		SI.VisName = D["VisName"].Text;
		SI.ImageDefault = (int)D["ImageDefault"].Float;
		SI.Image = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(D["Image"].Text),typeof(Texture2D));
		SI.Cube = (Cubemap)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(D["Cube"].Text),typeof(Cubemap));
		SI.Color = D["Color"].Vector;
		SI.Number = D["Number"].Float;
		SI.Range0 = D["Range0"].Float;
		SI.Range1 = D["Range1"].Float;
		
		//Dictionary<OldInputSpecialTypes,InputSpecialTypes> SpecialTypeConversion = new Dictionary<OldInputSpecialTypes,InputSpecialTypes>;
		//SpecialTypeConversion[OldInputSpecialTypes.Time] = InputSpecialTypes.Time;
		//SpecialTypeConversion[OldInputSpecialTypes.TimeFast] = InputSpecialTypes.TimeMul2;
		//SpecialTypeConversion[OldInputSpecialTypes.TimeSlow] = InputSpecialTypes.Time;
		
		
		SI.MainType = (InputMainTypes)Enum.Parse(typeof(InputMainTypes),((OldInputMainTypes)(int)D["MainType"].Float).ToString());
		//SI.SpecialType = (InputSpecialTypes)Enum.Parse(typeof(InputSpecialTypes),((OldInputSpecialTypes)(int)D["MainType"].Float).ToString());//(InputSpecialTypes)(int)D["SpecialType"].Float;
		
		OldInputSpecialTypes old = ((OldInputSpecialTypes)(int)D["SpecialType"].Float);
		if (old==OldInputSpecialTypes.None)SI.SpecialType = InputSpecialTypes.None;
		if (old==OldInputSpecialTypes.Time)SI.SpecialType = InputSpecialTypes.TimeStandard;
		if (old==OldInputSpecialTypes.TimeFast)SI.SpecialType = InputSpecialTypes.TimeMul2;
		if (old==OldInputSpecialTypes.TimeSlow){SI.SpecialType = InputSpecialTypes.Time;SI.InputScale = 0.5f;}
		if (old==OldInputSpecialTypes.TimeVerySlow){SI.SpecialType = InputSpecialTypes.Time;SI.InputScale = 1.0f/6f;}
		if (old==OldInputSpecialTypes.SinTime){SI.SpecialType = InputSpecialTypes.SinTimeStandard;}
		if (old==OldInputSpecialTypes.SinTimeFast){SI.SpecialType = InputSpecialTypes.SinTime;SI.InputScale = 2f;}
		if (old==OldInputSpecialTypes.SinTimeSlow){SI.SpecialType = InputSpecialTypes.SinTimeDiv2;}
		if (old==OldInputSpecialTypes.CosTime){SI.SpecialType = InputSpecialTypes.CosTimeStandard;}
		if (old==OldInputSpecialTypes.CosTimeFast){SI.SpecialType = InputSpecialTypes.CosTime;SI.InputScale = 2f;}
		if (old==OldInputSpecialTypes.CosTimeSlow){SI.SpecialType = InputSpecialTypes.CosTimeDiv2;}
		if (old==OldInputSpecialTypes.ShellDepth){SI.SpecialType = InputSpecialTypes.ShellDepthInvertedNormalized;}
		if (old==OldInputSpecialTypes.ParallaxDepth){SI.SpecialType = InputSpecialTypes.ParallaxDepthNormalized;}
		if (old==OldInputSpecialTypes.ClampedSinTime){SI.SpecialType = InputSpecialTypes.ClampedSinTimeStandard;}
		if (old==OldInputSpecialTypes.ClampedSinTimeFast){SI.SpecialType = InputSpecialTypes.ClampedSinTime;SI.InputScale = 2f;}
		if (old==OldInputSpecialTypes.ClampedSinTimeSlow){SI.SpecialType = InputSpecialTypes.ClampedSinTimeDiv2;}
		if (old==OldInputSpecialTypes.ClampedCosTime){SI.SpecialType = InputSpecialTypes.ClampedCosTimeStandard;}
		if (old==OldInputSpecialTypes.ClampedCosTimeFast){SI.SpecialType = InputSpecialTypes.ClampedCosTime;SI.InputScale = 2f;}
		if (old==OldInputSpecialTypes.ClampedCosTimeSlow){SI.SpecialType = InputSpecialTypes.ClampedCosTimeDiv2;}
		if (old==OldInputSpecialTypes.Custom){SI.SpecialType = InputSpecialTypes.Custom;}
		if (old==OldInputSpecialTypes.Mask){SI.SpecialType = InputSpecialTypes.Mask;}
		
		//if (SI.SpecialType!=InputSpecialTypes.None)
		if (D.ContainsKey("InEditor")&&D["InEditor"].Float==0f)
		SI.InEditor = false;
		else
		SI.InEditor = true;
		
		if (D.ContainsKey("NormalMap")&&D["NormalMap"].Float==0f)
		SI.NormalMap = false;
		else
		SI.NormalMap = true;
		
		//Debug.Log(SI.VisName);
		return SI;
	}
	/*_Color
	_SpecColor
	_Shininess
	_ReflectColor
	_Parallax
	_MainTex
	_Cube
	_BumpMap
	_ParallaxMap*/	
}
}