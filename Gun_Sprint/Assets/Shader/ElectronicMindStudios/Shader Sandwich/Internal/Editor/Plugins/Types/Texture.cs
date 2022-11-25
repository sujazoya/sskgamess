using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using SU = ElectronicMind.Sandwich.ShaderUtil;
using System.Xml.Serialization;
using System.Runtime.Serialization;
//using System.Xml;
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
[System.Serializable]
public class SLTTexture : ShaderLayerType{
	override public void Activate(){
		TypeS = "SLTTexture";
		MenuIndex = 1;
		Name = "Texture";
		Description = "A 2D image texture.";
		shader = "Hidden/ShaderSandwich/Previews/Types/Texture";
	}
	public void AddTextureInput(int Option,ShaderVar SV){
		if (Option==0){
			SV.AddInput();
			SV.WarningReset();
		}
	}
	public override void GPUPreviewJustBefore(SVDictionary Inputs, ShaderLayer SL, RenderTexture In, RenderTexture Out){
		if (SL==null||Inputs["Texture"].Image==null){
			_Material.SetTexture("Texture",EditorGUIUtility.ObjectContent(null, typeof(Texture2D)).image);
			_Material.SetFloat("Preview",1f);
		}else{
			_Material.SetFloat("Preview",0f);
		}
	}
	override public SVDictionary GetInputs(){
		Inputs = new SVDictionary();
			Inputs["Texture"] = new ShaderVar("Texture","Texture2D");
		return Inputs;
	}
	static float ImageRes = 256;
	void UpdateImage(SVDictionary Data,ShaderLayer SL,bool CreateEffects){
		if (ShaderSandwich.Instance.PreviewSystem == PreviewSystem.GPU){
			if (!CreateEffects)
				return;
			string pathh = AssetDatabase.GetAssetPath(Data["Texture"].ImageS());
			TextureImporter tii = (TextureImporter) TextureImporter.GetAtPath(pathh);
			if (tii!=null){
				string ImageType = "";
				#if UNITY_5_5_OR_NEWER
				if (tii.textureType==TextureImporterType.NormalMap)
				#else
				if (tii.textureType==TextureImporterType.Bump)
				#endif
					ImageType = "PackedNormalMap";
				/*else if (IsGreyscale)
					ImageType = "Greyscale";
				else if (IsNormalMapScaled)
					ImageType = "ScaledNormalMap";
				else if (IsNormalMap)
					ImageType = "NormalMap";*/
				else
					ImageType = "Normal";
				
				bool AlreadyFixed = false;
				bool AlreadyFixed2 = false;
				bool AlreadyFixed3 = false;
				foreach(ShaderEffect SE in SL.LayerEffects){
					if(SE.TypeS=="SSEUnpackNormal")
					AlreadyFixed = true;
					if(SE.TypeS=="SSENormalMap")
					AlreadyFixed2 = true;
					if(SE.TypeS=="SSESwitchNormalScale2")
					AlreadyFixed3 = true;
				}
				
				int Added = 0;
				if (ImageType == "PackedNormalMap"){
					if (!AlreadyFixed){
						SL.AddLayerEffect("SSEUnpackNormal");
						SL.LayerEffects[SL.LayerEffects.Count-1].AutoCreated = true;
					}
					Added = 1;
				}
				if (SL.Parent.CodeName=="d.Normal"){
					if (ImageType == "ScaledNormalMap"){
						if (!AlreadyFixed3){
							SL.AddLayerEffect("SSESwitchNormalScale2");
							SL.LayerEffects[SL.LayerEffects.Count-1].AutoCreated = true;
							
						}
						Added = 3;
					}
					if (ImageType == "Greyscale"||ImageType == "Normal"){
						if (!AlreadyFixed2){
							///TOoDO:
							SL.AddLayerEffect("SSENormalMap");
							SL.LayerEffects[SL.LayerEffects.Count-1].Inputs["Channel"].Type = 0;
							SL.LayerEffects[SL.LayerEffects.Count-1].AutoCreated = true;
						}
						Added = 2;
					}
				}
				
				for(int i = SL.LayerEffects.Count - 1; i > -1; i--){
					if(SL.LayerEffects[i].TypeS=="SSENormalMap"&&Added!=2){
						if (SL.LayerEffects[i].AutoCreated == true)
						SL.LayerEffects.RemoveAt(i);
					}
					else
					if(SL.LayerEffects[i].TypeS=="SSESwitchNormalScale2"&&Added!=3){
						if (SL.LayerEffects[i].AutoCreated == true)
						SL.LayerEffects.RemoveAt(i);
					}
					else
					if(SL.LayerEffects[i].TypeS=="SSEUnpackNormal"&&Added!=1){
						if (SL.LayerEffects[i].AutoCreated == true)
						SL.LayerEffects.RemoveAt(i);
					}
				}
			}
			
			return;
		}
			
		
		
		string path = AssetDatabase.GetAssetPath(Data["Texture"].ImageS());
		TextureImporter ti = (TextureImporter) TextureImporter.GetAtPath(path);
		bool OldIsReadable = false;
		
		if (ti!=null){
			OldIsReadable = ti.isReadable;
			int OldMaxSize = ti.maxTextureSize;
			ti.isReadable = true;
			ti.maxTextureSize = 256;
			AssetDatabase.ImportAsset(path);
			//ImageResize = new Texture2D(70,70,TextureFormat.ARGB32,false);
			bool IsGreyscale = true;
			bool IsNormalMap = true;
			int BadNormalBlues = 0;
			int BadNormalBlues2 = 0;
			bool IsNormalMapScaled = true;
			
			int Size = (int)ImageRes;
			Color32[] colors = new Color32[(int)(Size*Size)];
			int mipLevel = 0;
			
			Color32[] ImageColors = Data["Texture"].Image.GetPixels32(mipLevel);
			int MipmapWidth = (int)Mathf.Max(1,Data["Texture"].Image.width>>mipLevel);///(mipLevel*2));//>>mipLevel);
			int MipmapHeight = (int)Mathf.Max(1,Data["Texture"].Image.height>>mipLevel);
			
			for(int x = 0;x<Size;x++){
				for(int y = 0;y<(int)Size;y++){
					Color32 LayerPixel = ImageColors[ShaderUtil.FlatArray((int)  ((float)x/Size*MipmapWidth),(int)((float)y/Size*MipmapHeight),MipmapWidth,MipmapHeight)];
					
					if ((!ShaderUtil.MoreOrLess(LayerPixel.r,LayerPixel.b,30))||(!ShaderUtil.MoreOrLess(LayerPixel.r,LayerPixel.g,10))||(!ShaderUtil.MoreOrLess(LayerPixel.g,LayerPixel.b,30)))
						IsGreyscale = false;
					if (LayerPixel.r<114)
						BadNormalBlues+=1;
					//if (LayerPixel.b<240)
					//UnityEngine.Debug.Log(LayerPixel.b.ToString());
					if (LayerPixel.b<245)
						BadNormalBlues+=1;
						
					
					colors[x+(y*Size)] = LayerPixel;
				}
			}
			Data["Texture"].ImagePixels = colors;
			
			if (BadNormalBlues>((Size*Size)/2))
				IsNormalMap = false;
			if (BadNormalBlues2>((Size*Size)/2))
				IsNormalMapScaled = false;
			
			string ImageType = "";
			//if (ti.normalmap)
			#if UNITY_5_5_OR_NEWER
			if (ti.textureType==TextureImporterType.NormalMap)
			#else
			if (ti.textureType==TextureImporterType.Bump)
			#endif
			ImageType = "PackedNormalMap";
			else
			if (IsGreyscale)
			ImageType = "Greyscale";
			else
			if (IsNormalMapScaled)
			ImageType = "ScaledNormalMap";
			else
			if (IsNormalMap)
			ImageType = "NormalMap";
			else
			ImageType = "Normal";
			
			if (CreateEffects){
				bool AlreadyFixed = false;
				bool AlreadyFixed2 = false;
				bool AlreadyFixed3 = false;
				foreach(ShaderEffect SE in SL.LayerEffects){
					if(SE.TypeS=="SSEUnpackNormal")
					AlreadyFixed = true;
					if(SE.TypeS=="SSENormalMap")
					AlreadyFixed2 = true;
					if(SE.TypeS=="SSESwitchNormalScale2")
					AlreadyFixed3 = true;
				}
				
				if (ti!=null){
					int Added = 0;
					if (ImageType == "PackedNormalMap"){
						if (AlreadyFixed == false){
							SL.AddLayerEffect("SSEUnpackNormal");
							SL.LayerEffects[SL.LayerEffects.Count-1].AutoCreated = true;
							
						}
						Added = 1;
						//Image.WarningReset();
					}
					if (SL.Parent.CodeName=="d.Normal"){
						if (ImageType == "ScaledNormalMap"){
							if (!AlreadyFixed3){
								SL.AddLayerEffect("SSESwitchNormalScale2");
								SL.LayerEffects[SL.LayerEffects.Count-1].AutoCreated = true;
								
							}
							Added = 3;
						}
						if (ImageType == "Greyscale"||ImageType == "Normal"){
							if (!AlreadyFixed2){
								///TODO:
								///SL.AddLayerEffect("SSENormalMap");
								///SL.LayerEffects[SL.LayerEffects.Count-1].Inputs[2].Type = 0;
								///SL.LayerEffects[SL.LayerEffects.Count-1].AutoCreated = true;
							}
							Added = 2;
						}
					}
					
					//if (AlreadyFixed == true||AlreadyFixed2 == true||AlreadyFixed3 == true){
					for(int i = SL.LayerEffects.Count - 1; i > -1; i--){
						if(SL.LayerEffects[i].TypeS=="SSENormalMap"&&Added!=2){
							if (SL.LayerEffects[i].AutoCreated == true)
							SL.LayerEffects.RemoveAt(i);
						}
						else
						if(SL.LayerEffects[i].TypeS=="SSESwitchNormalScale2"&&Added!=3){
							if (SL.LayerEffects[i].AutoCreated == true)
							SL.LayerEffects.RemoveAt(i);
						}
						else
						if(SL.LayerEffects[i].TypeS=="SSEUnpackNormal"&&Added!=1){
							if (SL.LayerEffects[i].AutoCreated == true)
							SL.LayerEffects.RemoveAt(i);
						}
					}
				}
			}
			ti.isReadable = OldIsReadable;
			ti.maxTextureSize = OldMaxSize;
			AssetDatabase.ImportAsset(path);
		}	
	}
	override public bool Draw(Rect rect,SVDictionary Data,ShaderLayer SL){
		if (DrawVars(rect,Data,SL)){
			GUI.changed = true;
			UpdateImage(Data,SL,true);
		}
	
		//if (Data["Texture"].Input==null)
		//	Data["Texture"].WarningSetup("No Input","Textures and cubemaps require an input to function correctly. Would you like to add one automatically?","Yes","No",AddTextureInput);
		//else
		//	Data["Texture"].WarningReset();
		Data["Texture"].Use = true;
		
		return false;
	}
	override public string Generate(SVDictionary Data, ShaderLayer SL, ShaderData SD, string Map){
		if (Data["Texture"].Get()!="SSError"){
			if (SL.IsVertex)
			return ShaderUtil.LinearToGamma("tex2Dlod("+Data["Texture"].Get()+",float4("+Map+",0,0))");
			else{
				if (SD.ForceOrigVDMappingHack){///TODO: make semi decent
					ShaderLayerBranch worthlessBranch = new ShaderLayerBranch(false);
					return ShaderUtil.LinearToGamma("tex2Dgrad("+Data["Texture"].Get()+","+Map+",ddx("+SL.GCOrigUVs_Real(SD,"","","",true,ref worthlessBranch)+"),ddy("+SL.GCOrigUVs_Real(SD,"","","",true,ref worthlessBranch)+"))");
				}
				else
					return ShaderUtil.LinearToGamma("tex2D("+Data["Texture"].Get()+","+Map+")");
			}
		}	
		return "float4(1,1,1,1)";
	}
	override public int GetDimensions(SVDictionary Data, ShaderLayer SL){
		return 2;
	}
	override public ShaderColor Preview(SVDictionary Data, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		if (Data["Texture"].Get()!="SSError"&&Data["Texture"].ImagePixels!=null&&Data["Texture"].Image!=null)
		return LinearToGamma(new ShaderColor(Data["Texture"].ImagePixels[ShaderUtil.FlatArray((int)(UV.x*ImageRes),(int)(UV.y*ImageRes),(int)ImageRes,(int)ImageRes)]));
		
		if (Data["Texture"].Image!=null&&(Data["Texture"].ImagePixels==null||Data["Texture"].ImagePixels.Length<10))
			UpdateImage(Data,SL,false);
		
		if (Data["Texture"].Input!=null&&Data["Texture"].Input.NormalMap)
		return new ShaderColor(0.5f,0.5f,1,0.5f);
		else
		return new ShaderColor(1f,1f,1f,1f);
		
	}
}
}