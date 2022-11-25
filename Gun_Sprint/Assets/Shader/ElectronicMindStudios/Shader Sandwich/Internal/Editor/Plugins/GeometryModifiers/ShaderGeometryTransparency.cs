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
public class ShaderGeometryModifierTransparency : ShaderGeometryModifier{
	public ShaderVar TransparencyType = new ShaderVar(
	"Transparency Type",new string[]{"Cutout","Fade","Fade by MSAA\n(Alpha to Coverage)"},new string[]{"ImagePreviews/TransparencyCutout.png","ImagePreviews/TransparencyFade.png","ImagePreviews/TransparencyAlphaToMask.png"},"",new string[]{"Cutout - Cuts holes in the mesh, fast, but creates sharp edges. This can have aliasing issues and is slower on some phones.","Fade - Supports proper transparency, but can have depth sorting issues (Objects can appear in front of each other incorrectly) and is usually slower than cutout.","\nFade by MSAA - uses hardware multi-sample anti aliasing to create results similar to Fade mode but quicker and with better z-writing."}
	); 
	public ShaderVar TransparencyZWrite = new ShaderVar("Transparency ZWrite",new string[]{"Off","On","Full","Cutout"}); 
	public ShaderVar ShadowTypeFade = new ShaderVar("Shadow Type Fade",new string[]{"Full","Cutout","Dithered"}); 
	public ShaderVar CutoutAmount = new ShaderVar("Cutout Amount",0.5f); 
	public ShaderVar Transparency = new ShaderVar("Transparency",0.5f);
	public ShaderVar BlendMode = new ShaderVar("Blend Mode",new string[]{"Mix","Add","Soft Add","Multiply"},new string[]{"","","",""},new string[]{"Mix","Add","Soft Add","Multiply"});
	public ShaderFix DeferredTransparency = null;
	public override void GetMyPersonalPrivateForMySubStandardChildEyesOnlyFixes(ShaderBase shader, ShaderPass pass, List<ShaderFix> fixes){
		if (DeferredTransparency==null)
			DeferredTransparency = new ShaderFix(this,"Transparency in deferred mode behaves very different to forward mode - I hope you know what you're doing :P",ShaderFix.FixType.Warning,
			null);
		
		if (shader.RenderingPathTarget.Type==1&&TransparencyType.Type!=0){//Deferred
			fixes.Add(DeferredTransparency);
		}
	}
	public override void GetMyShaderVars(List<ShaderVar> SVs){
		SVs.Add(TransparencyType);
		SVs.Add(TransparencyZWrite);
		SVs.Add(ShadowTypeFade);
		SVs.Add(CutoutAmount);
		SVs.Add(Transparency);
		SVs.Add(BlendMode);
	}
	new static public string MenuItem = "Transparency/Transparency";
	public void OnEnable(){
		Name = "Transparency";
		MenuItem = "Transparency/Transparency";
		
		ShaderGeometryModifierComponent = ShaderGeometryModifierComponent.Transparency;
	}
	public override List<ShaderLayerList> GetMyShaderLayerLists(bool All){
		List<ShaderLayerList> SLLs = new List<ShaderLayerList>();
		return SLLs;
	}
	public override int GetHeight(float Width){
		return 20+25;
	}
	public override string GenerateGlobalFunctions(ShaderData SD,List<ShaderIngredient> ingredients){
		bool Dithered = false;
		if (SD.ShaderPassIsShadowCaster){
			foreach(ShaderIngredient ingredient in ingredients){
				if (ingredient as ShaderGeometryModifierTransparency!=null){
					Dithered |= (ingredient as ShaderGeometryModifierTransparency).TransparencyType.Type>=1&&(ingredient as ShaderGeometryModifierTransparency).ShadowTypeFade.Type==2;
				}
			}
			if (Dithered){
				return "				sampler3D _DitherMaskLOD;\n";
			}
		}
		return "";
	}
	
	public override void RenderUI(float Width){
		int y = 0;
		TransparencyType.Draw(new Rect(0,0,Width,124));
		
		GUI.skin.label.alignment = TextAnchor.UpperCenter;
		EMindGUI.Label(new Rect(0,124+18,Width,70),TransparencyType.Descriptions[TransparencyType.Type],12);
		GUI.skin.label.alignment = TextAnchor.UpperLeft;
		
		y += 124+18+24+25;
		if (TransparencyType.Type==2)
			y+=20;
			
		if (TransparencyType.Type==0)
			Transparency.Draw(new Rect(0,y,Width,20),"Cutoff");
		else
			Transparency.Draw(new Rect(0,y,Width,20),"Opacity");
		y+=25;
		
				
		if (TransparencyType.Type==1){
			GUI.Label(new Rect(0,y,Width,20),"Z Writing");y+=20;
			TransparencyZWrite.Draw(new Rect(16,y,Width-16,20),"Cutoff");y+=25;
			if (ShadowTypeFade.Type!=1&&(TransparencyType.Type>=1&&TransparencyZWrite.Type==3)){
				CutoutAmount.Draw(new Rect(16,y,Width-16,20),"Cutoff");y+=25;
			}
		}
		
		GUI.Label(new Rect(0,y,Width,20),"Shadow Casting");y+=20;
		if (TransparencyType.Type==0){
			ShadowTypeFade.Names = new string[]{"Full","Cutout"};
			if (ShadowTypeFade.Type>1)
				ShadowTypeFade.Type = 1;
			ShadowTypeFade.TypeDispL = 2;
		}else{
			ShadowTypeFade.Names = new string[]{"Full","Cutout","Dithered"};
			ShadowTypeFade.TypeDispL = 3;
		}
		
		ShadowTypeFade.Draw(new Rect(16,y,Width-16,20),"Shadow Type");y+=25;
		//if (ShadowTypeFade.Type==1||(TransparencyType.Type==1&&TransparencyZWrite.Type==3)){
		if (ShadowTypeFade.Type==1){
			CutoutAmount.Draw(new Rect(16,y,Width-16,20),"Cutoff");y+=25;
		}
		if (TransparencyType.Type>=1)
			BlendMode.Draw(new Rect(0,y,Width,20),"Blend Mode");y+=25;
	}
	public override void CalculatePremultiplication(ShaderData SD){
		if (TransparencyType.Type==0||TransparencyType.Type==2)
			SD.UsesPremultipliedBlending = false;
		if (TransparencyType.Type==2)
			SD.AlphaToMask = true;
	}
	
	public override void GeneratePass(ShaderData SD, ShaderPass SP){
		//Debug.Log("GOooO??/");
		if (TransparencyType.Type==1&&TransparencyZWrite.Type>=2){
			//Debug.Log("GOooO!");
			if (SP.PassesCD.Count>0)
				SP.InsertPass(0,SP.GenerateMaskPass,ShaderData.CopyCD(SP.PassesCD[0]));
			else
				SP.InsertPass(0,SP.GenerateMaskPass,null);
			//SP.PassesCD[0]["TransparencyZFillType"] = "Full";
		}
	}
	public override string GenerateVertex(ShaderData SD){
		if (SD.ZWriteMode==ZWriteMode.Auto){
			if (!SD.ShaderPassIsShadowCaster){
				if (TransparencyType.Type==0||TransparencyType.Type==2)
					SD.ZWriteMode=ZWriteMode.On;
				if (TransparencyType.Type==1){
					if (TransparencyZWrite.Type==1)//On
						SD.ZWriteMode=ZWriteMode.On;
					else
						SD.ZWriteMode=ZWriteMode.Off;
				}
			}
			if (SD.ShaderPassIsMask)
				SD.ZWriteMode=ZWriteMode.On;
		}
		return "";
	}
	public override string GenerateTransparency(ShaderData SD){
		if (SD.ShaderPassIsDeferred){
			if (TransparencyType.Type==0){
				return "deferredOut.Alpha -= "+Transparency.Get()+";\nclip(deferredOut.Alpha);\nreturn deferredOut;\n";
			}
			if (TransparencyType.Type==1){
				return "deferredOut.Alpha *= "+Transparency.Get()+";\nreturn deferredOut;\n";
			}
		}
		if (TransparencyType.Type==0){
			SD.ClipsAnywhere = true;
			if (SD.ShaderPassIsShadowCaster){
				if (ShadowTypeFade.Type==1){
					return "clip (outputColor.a - " + CutoutAmount.Get() + ");\nreturn outputColor;\n";
				}else
					return "return outputColor;\n";
			}
			else{
				return "outputColor.a -= "+Transparency.Get()+";\nclip(outputColor.a);\n"+"return outputColor;\n";
			}
		}
		else{
			SD.BlendsWeirdlyAnywhere = true;
			if (SD.ShaderPassIsMask){
				if (TransparencyZWrite.Type==3){
					return "clip (outputColor.a - " + CutoutAmount.Get() + ");\n"+"return outputColor;\n";
				}
			}
			if (!SD.ShaderPassIsShadowCaster&&TransparencyType.Type==1){
				if (SD.UsesPremultipliedBlending)
					SD.BlendFactorSrc = BlendFactor.One;
				else
					SD.BlendFactorSrc = BlendFactor.SrcAlpha;
				
				if (SD.ShaderPass==ShaderPasses.ForwardAdd)
					SD.BlendFactorDest = BlendFactor.One;
				else
					SD.BlendFactorDest = BlendFactor.OneMinusSrcAlpha;
				
				SD.IsTransparent = true;
			}
				
			if (SD.ShaderPass==ShaderPasses.Deferred){
				SD.BlendFactorSrc = BlendFactor.SrcAlpha;
				SD.BlendFactorDest = BlendFactor.OneMinusSrcAlpha;
				return "deferredOut.AlbedoAlpha.a *= "+Transparency.Get()+";\nreturn deferredOut.AlbedoAlpha.a;\n";
			}
			if (!SD.ShaderPassIsShadowCaster){
				if (BlendMode.Type==1){
					SD.BlendFactorSrc = BlendFactor.One;
					SD.BlendFactorDest = BlendFactor.One;
				}
				if (BlendMode.Type==2){
					SD.BlendFactorSrc = BlendFactor.OneMinusDstColor;
					SD.BlendFactorDest = BlendFactor.One;
				}
				if (BlendMode.Type==3){
					SD.BlendFactorSrc = BlendFactor.DstColor;
					SD.BlendFactorDest = BlendFactor.Zero;
				}
			}
			
			string ret = "";
			if (SD.ShaderPassIsShadowCaster){
				if (ShadowTypeFade.Type==1){
					ret += "clip (outputColor.a - " + CutoutAmount.Get() + ");\n";
				}
			}
			if (SD.UsesPremultipliedBlending&&TransparencyType.Type==1){
				ret += "outputColor *= "+Transparency.Get()+";\n";
				if (BlendMode.Type==3)
					ret += "outputColor += (1-outputColor.a);\n";
			}
			else{
				if (BlendMode.Type==1||BlendMode.Type==2)
					ret += "outputColor *= "+Transparency.Get()+";\n"+
					"outputColor.rgb *= outputColor.a;\n";
				else if (BlendMode.Type==3)
					ret += "outputColor = lerp(1,outputColor,outputColor.a);\n";
				else
					ret += "outputColor.a *= "+Transparency.Get()+";\n";
			}
			
			if (SD.ShaderPassIsShadowCaster){
				if (ShadowTypeFade.Type==2){
					//if ((SD.ShaderModel == ShaderModel.Auto&&(SD.ShaderModelAuto == ShaderModel.SM2||SD.ShaderModelAuto==ShaderModel.SM2_5||SD.ShaderModelAuto==ShaderModel.SM3))||SD.ShaderModel == ShaderModel.SM2||SD.ShaderModel==ShaderModel.SM2_5||SD.ShaderModel==ShaderModel.SM3)
					//	ret += "outputColor.a = tex3D(_DitherMaskLOD, float3(floor(vd.screenPos.xy*_ScreenParams.xy)*0.25,outputColor.a*0.9375)).a;\n";
					//else
					//	ret += "outputColor.a = tex3D(_DitherMaskLOD, float3(vtp.position.xy*0.25,outputColor.a*0.9375)).a;\n";
						
						
					ret += "#if SHADER_TARGET <= 30\n	outputColor.a = tex3D(_DitherMaskLOD, float3(floor(vd.screenPos.xy*_ScreenParams.xy)*0.25,outputColor.a*0.9375)).a;\n#else\n";
					ret += "	outputColor.a = tex3D(_DitherMaskLOD, float3(vtp.position.xy*0.25,outputColor.a*0.9375)).a;\n#endif\n";
					
					
					ret += "clip (outputColor.a - 0.01);\n";
				}
			}
			
			ret += "return outputColor;\n";
			return ret;
		}
	}
}
}