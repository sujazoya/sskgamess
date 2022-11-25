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
public class SLTCubemap : ShaderLayerType{
	override public void Activate(){
		TypeS = "SLTCubemap";
		Name = "Cubemap";
		MenuIndex = 2;
		shader = "Hidden/ShaderSandwich/Previews/Types/Cubemap";
		Description = "A 3D cubemap.";
	}
	public void AddTextureInput(int Option,ShaderVar SV){
		if (Option==0){
				SV.AddInput();
			SV.WarningReset();
		}
	}
	public override void GPUPreviewJustBefore(SVDictionary Inputs, ShaderLayer SL, RenderTexture In, RenderTexture Out){
		if (SL==null||Inputs["Cubemap"].Cube==null){
			_Material.SetTexture("Texture",EditorGUIUtility.ObjectContent(null, typeof(Cubemap)).image);
			_Material.SetFloat("Preview",1f);
		}
		else{
			_Material.SetFloat("Preview",0f);
		}
	}
	//EditorGUIUtility.ObjectContent(null, typeof(Cubemap)).image
	override public SVDictionary GetInputs(){
		Inputs = new SVDictionary();
			Inputs["Cubemap"] = new ShaderVar("Cubemap","Cubemap");
			Inputs["HDR Support"] = new ShaderVar("HDR Support",true);///TOoDO: actually work...
		return Inputs;
	}
	static float ImageRes = 256;
	void UpdateImage(SVDictionary Data,ShaderLayer SL){
		if (ShaderSandwich.Instance.PreviewSystem == PreviewSystem.GPU)
			return;
		
		string path = AssetDatabase.GetAssetPath(Inputs["Cubemap"].CubeS());
		TextureImporter ti =  TextureImporter.GetAtPath(path) as TextureImporter;
		bool OldIsReadable = false;
		if (ti!=null){
				OldIsReadable = ti.isReadable;
				ti.isReadable = true;
				AssetDatabase.ImportAsset(path);
		}
		Color32[] colors = new Color32[(int)(ImageRes*ImageRes)];
		Cubemap UseCube = Data["Cubemap"].Cube;
		try {
			Data["Cubemap"].Cube.GetPixel(CubemapFace.PositiveX,0,0);
		}
		catch {
			UseCube = ShaderSandwich.KitchenCube;
		}
		for(int x = 0;x<ImageRes;x++){
			for(int y = 0;y<(int)ImageRes;y++){
				Color LayerPixel;
				LayerPixel = UseCube.GetPixel(CubemapFace.PositiveX,(int)((float)x/ImageRes*UseCube.width),(int)((1-(float)y/ImageRes)*UseCube.height));
				colors[x+((int)(y*ImageRes))] = (Color32)LayerPixel;
			}
		}	
		Data["Cubemap"].ImagePixels = colors;
		if (ti!=null){
			ti.isReadable = OldIsReadable;
			AssetDatabase.ImportAsset(path);
		}
	}
	override public bool Draw(Rect rect,SVDictionary Data,ShaderLayer SL){
		if (DrawVars(rect,Data,SL)){
			GUI.changed = true;
			UpdateImage(Data,SL);
		}
	
		//if (Data["Cubemap"].Input==null)
		//	Data["Cubemap"].WarningSetup("No Input","Textures and cubemaps require an input to function correctly. Would you like to add one automatically?","Yes","No",AddTextureInput);
		//else
		//	Data["Cubemap"].WarningReset();
		Data["Cubemap"].Use = true;
		
		return false;
	}
	override public string Generate(SVDictionary Data, ShaderLayer SL, ShaderData SD, string Map){
		if (Data["Cubemap"].Get()!="SSError"){
			if (Data["HDR Support"].On){
				SD.AdditionalFunctions.Add(
				"	half4 DecodeHDRWA(half4 col, half4 data){\n"+
				"		return half4(DecodeHDR(col,data),col.a);\n"+
				"	}\n");
				if (SL.IsVertex)
					return "DecodeHDRWA(texCUBElod("+Data["Cubemap"].Get()+",float4("+Map+",0)),"+Data["Cubemap"].Get()+"_HDR)";
				else
					return "DecodeHDRWA(texCUBE("+Data["Cubemap"].Get()+","+Map+"),"+Data["Cubemap"].Get()+"_HDR)";
			}else{
				if (SL.IsVertex)
				return "texCUBElod("+Data["Cubemap"].Get()+",float4("+Map+",0))";
				else
				return "texCUBE("+Data["Cubemap"].Get()+","+Map+")";
			}
		}
		return "float4(1,1,1,1)";
	}
	override public int GetDimensions(SVDictionary Data, ShaderLayer SL){
		return 3;
	}
	override public ShaderColor Preview(SVDictionary Data, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		if (Data["Cubemap"].Get()!="SSError"&&Data["Cubemap"].ImagePixels!=null&&Data["Cubemap"].Cube!=null)
			return new ShaderColor(Data["Cubemap"].ImagePixels[ShaderUtil.FlatArray((int)(UV.x*ImageRes),(int)(UV.y*ImageRes),(int)ImageRes,(int)ImageRes)]);
		else
			return new ShaderColor(1f,1f,1f,1f);
	}
}
}