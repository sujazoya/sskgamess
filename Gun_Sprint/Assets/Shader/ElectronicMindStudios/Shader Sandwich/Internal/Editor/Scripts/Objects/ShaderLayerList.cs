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
[System.Serializable]
public class ShaderLayerList : ScriptableObject, IEnumerable<ShaderLayer>{
	public ShaderVar Name = new ShaderVar("LayerListName","");
	public ShaderVar NameUnique = new ShaderVar("LayerListUniqueName","");
	public string Description = "";
	public List<ShaderLayer> SLs = new List<ShaderLayer>();
	public string CodeName = "";
	//public string CodeName{
	//	get{return ShaderUtil.CodeName(Name.Text);}
	//	set{}
	//}
	public string InputName = "";//UnityEditor.Undo.RecordObject((UnityEngine.Object)list,"Add Layer");
	public ShaderVar EndTag = new ShaderVar("EndTag","rgb");
	public string Function = "";
	public bool UsesAlpha = false;
	public int GCUses = 0;
	public ShaderVar IsMask = new ShaderVar("Is Mask",false);
	public ShaderVar IsLighting = new ShaderVar("Is Lighting",false);
	public bool Parallax = false;
	public int Inputs = 0;
	public string LayerCatagory = "";
	public Color BaseColor;
	public Vector2 Scroll = new Vector2(0,0);
	public int Count{
		get{
			return SLs.Count;
		}
		set{
			
		}
	}
	
	[NonSerialized]Texture2D Icon;
	RenderTexture _RTIcon;
	public RenderTexture RTIcon{
		get{
			if (_RTIcon==null){
				_RTIcon = new RenderTexture(70,70,0,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Default);
				_RTIcon.wrapMode = TextureWrapMode.Repeat;
			}
			if (!_RTIcon.IsCreated()){
				_RTIcon.Create();
			}
			return _RTIcon;
		}
	}
	RenderTexture _RTPing;
	public RenderTexture RTPing{
		get{
			if (_RTPing==null){
				_RTPing = new RenderTexture(70,70,0,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Default);
				_RTPing.wrapMode = TextureWrapMode.Repeat;
			}
			if (!_RTPing.IsCreated()){
				_RTPing.Create();
			}
			return _RTPing;
		}
	}
	RenderTexture _RTTempUV;
	public RenderTexture RTTempUV{
		get{
			if (_RTTempUV==null){
				_RTTempUV = new RenderTexture(70,70,0,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Default);
				_RTTempUV.wrapMode = TextureWrapMode.Repeat;
			}
			if (!_RTTempUV.IsCreated()){
				_RTTempUV.Create();
			}
			return _RTTempUV;
		}
	}

	public bool UsedShell = false;
	public bool UsedBase = false;
	public bool Used = true;
	public string GCVariable(){
		string ShaderCode = "";
		
		if (EndTag.Text.Length==1){
			if (EndTag.Text=="a")
				ShaderCode="float "+CodeName+" = 0";
			else
				ShaderCode="float "+CodeName+" = 0";
		}
		if (EndTag.Text.Length==2)
		ShaderCode="float2 "+CodeName+" = float2(0,0)";
		if (EndTag.Text.Length==3)
		ShaderCode="float3 "+CodeName+" = float3(0,0,0)";
		if (EndTag.Text.Length==4)
		ShaderCode="float4 "+CodeName+" = float4(0,0,0,0)";

		ShaderCode+=";\n";
		return "	//Set default mask color\n		"+ShaderCode;
	}
	public string GCVariableBlahBlah(){
		string ShaderCode = "";
		
		if (EndTag.Text.Length==1)
		ShaderCode+="float "+CodeName;
		if (EndTag.Text.Length==2)
		ShaderCode+="float2 "+CodeName;
		if (EndTag.Text.Length==3)
		ShaderCode+="float3 "+CodeName;
		if (EndTag.Text.Length==4)
		ShaderCode+="float4 "+CodeName;

		ShaderCode+=";\n";
		return ShaderCode;
	}
	public string GCType(){
		string ShaderCode = "";
		
		if (EndTag.Text.Length==1)
		ShaderCode+="float";
		if (EndTag.Text.Length==2)
		ShaderCode+="float2";
		if (EndTag.Text.Length==3)
		ShaderCode+="float3";
		if (EndTag.Text.Length==4)
		ShaderCode+="float4";
		return ShaderCode;
	}
	public string GenerateHeaderCache = "";//Fragm/Genmeral
	public string GenerateHeaderCache2 = "";//Vertex
	public string GenerateHeaderCached(ShaderData SD, bool Call){
		if (Call){
			if (!SD.UsedMasksVertex.Contains(this)||!SD.UsedMasksFragment.Contains(this)||(SD.PipelineStage==PipelineStage.Fragment))
				return GenerateHeaderCache;
			else
				return GenerateHeaderCache2;
		}
		else
			return GenerateHeader(SD,GenerateCode(SD,false),false);
	}
	public string GenerateHeader(ShaderData SD, string surfaceInternals, bool Call){
		bool UsesVertexData = false;
		bool UsesVertexToPixel = false;
		bool UsesPrecalculations = false;
		bool UsesUnityGI = false;
		bool UsesUnityGIInput = false;
		bool UsesMasks = false;
		bool UsesOutputColor = false;
		
		string FunctionHeader = "";
		if (!Call){
			FunctionHeader = GCType()+" ";
		}
			
		FunctionHeader += ((SD.UsedMasksFragment.Contains(this)&&SD.UsedMasksVertex.Contains(this))?(SD.PipelineStage==PipelineStage.Fragment?"Pixel_":"Vertex_"):"")+"Mask_"+CodeName+" (";
		//VertexData vd, Precalculations pc, UnityGI gi, Masks mask
		if (surfaceInternals.IndexOf("vd.")>=0){
			SD.UsesVertexData = true;UsesVertexData = true;}
		if (surfaceInternals.IndexOf("vtp")>=0){
			UsesVertexToPixel = true;}
		if (surfaceInternals.IndexOf("pc.")>=0){
			SD.UsesPrecalculations = true;UsesPrecalculations = true;}
		if (surfaceInternals.IndexOf("gi.")>=0){
			if (SD.PipelineStage==PipelineStage.Fragment)SD.UsesUnityGIFragment = true;
			if (SD.PipelineStage==PipelineStage.Vertex)SD.UsesUnityGIVertex = true;
			UsesUnityGI = true;}
		if (surfaceInternals.IndexOf("giInput")>=0){
			if (SD.PipelineStage==PipelineStage.Fragment)SD.UsesUnityGIInputFragment = true;
			if (SD.PipelineStage==PipelineStage.Vertex)SD.UsesUnityGIInputVertex = true;
			UsesUnityGIInput = true;}
		if (surfaceInternals.IndexOf("mask.")>=0){
			SD.UsesMasks = true;UsesMasks = true;}
		if (surfaceInternals.IndexOf("outputColor")>=0){
			UsesOutputColor = true;}

		bool asd = false;
		if (Call){
			if (UsesVertexData){FunctionHeader += (asd?", ":" ") + "vd";asd = true;}
			if (UsesVertexToPixel){FunctionHeader += (asd?", ":" ") + "vtp";asd = true;}
			if (UsesPrecalculations){FunctionHeader += (asd?", ":" ")+"pc";asd = true;}
			if (UsesUnityGI){FunctionHeader += (asd?", ":" ")+"gi";asd = true;}
			if (UsesUnityGIInput){FunctionHeader += (asd?", ":" ")+"giInput";asd = true;}
			if (UsesMasks){FunctionHeader += (asd?", ":" ")+"mask";asd = true;}
			if (UsesOutputColor){FunctionHeader += (asd?", ":" ")+"outputColor";asd = true;}
		}
		else{
			if (UsesVertexData){FunctionHeader += (asd?", ":" ") + "VertexData vd";asd = true;}
			if (UsesVertexToPixel){FunctionHeader += (asd?", ":" ") + "VertexToPixel vtp";asd = true;}
			if (UsesPrecalculations){FunctionHeader += (asd?", ":" ")+"Precalculations pc";asd = true;}
			if (UsesUnityGI){FunctionHeader += (asd?", ":" ")+"UnityGI gi";asd = true;}
			if (UsesUnityGIInput){FunctionHeader += (asd?", ":" ")+"UnityGIInput giInput";asd = true;}
			if (UsesMasks){FunctionHeader += (asd?", ":" ")+"Masks mask";asd = true;}
			if (UsesOutputColor){FunctionHeader += (asd?", ":" ")+"float4 outputColor";asd = true;}
		}

		FunctionHeader+=")";
		if (Call){
			if (!SD.UsedMasksVertex.Contains(this)||!SD.UsedMasksFragment.Contains(this)||(SD.PipelineStage==PipelineStage.Fragment))
				GenerateHeaderCache = FunctionHeader;
			else
				GenerateHeaderCache2 = FunctionHeader;
		}
		//Debug.Log(GenerateHeaderCache);
		//Debug.Log(GenerateHeaderCache2);
		return FunctionHeader;
	}
	public void FixParents(){
		foreach(ShaderLayer SL in SLs)
			SL.Parent = this;
	}
    public IEnumerator<ShaderLayer> GetEnumerator()
    {
		//Debug.Log(EndTag.Text+Name.Text);
        return SLs.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
		//Debug.Log(EndTag.Text+Name.Text);
        return GetEnumerator();
    }
	public void UShaderLayerList(string UN,string N,string IN,string CN,string ET,string F,Color BS){
		Name.Text = N;
		NameUnique.Text = UN;
		InputName = IN;
		CodeName = CN;
		EndTag.Text = ET;
		Function = F;
		BaseColor = BS;
		//Debug.Log(ET+N);
	}
	public void UShaderLayerList(string UN,string N,string D,string IN,string CN,string ET,string F,Color BS){
	//Debug.Log(ET+N);
		Name.Text = N;
		NameUnique.Text = UN;
		Description = D;
		InputName = IN;
		CodeName = CN;
		EndTag.Text = ET;
		Function = F;
		BaseColor = BS;
	}
	public ShaderLayerList(){
	}
	public void Add(ShaderLayer SL){
		SLs.Add(SL);
		//if (Name.Text!="Diffuse"&&Name.Text!="Emission")
		//	SL.Color.Vector = new ShaderColor(BaseColor);
		//else
		//if (Name.Text=="Emission")
		//	SL.Color.Vector = new ShaderColor(0f,0.2f,0.6f,1f);
		SL.Parent = this;
	}
	public void AddC(ShaderLayer SL){
		SLs.Add(SL);
		SL.Parent = this;
	}
	public void RemoveAt(int Pos){
		SLs.RemoveAt(Pos);
	}
	public void MoveItem(int OldIndex,int NewIndex){
		if (NewIndex<SLs.Count&&NewIndex>=0){
			ShaderLayer item = SLs[OldIndex];
			SLs.RemoveAt(OldIndex);
			//if (NewIndex > OldIndex)
			//	NewIndex -= 1;
			
			SLs.Insert(NewIndex,item);
		}
	}
	
	public void OnDisable(){
		if (_RTIcon!=null)
			_RTIcon.Release();
		if (_RTPing!=null)
			_RTPing.Release();
	}
	Shader _DefaultUVsShader;
	Material _DefaultUVsMat;
	public void UpdateIcon(Vector2 rect){
		UpdateIcon(rect,null);
	}
	public void UpdateIcon(Vector2 rect,ShaderLayer stopAt){
		if (ShaderSandwich.Instance.PreviewSystem == PreviewSystem.GPU){
			if (_DefaultUVsShader==null)
				_DefaultUVsShader = Shader.Find("Hidden/ShaderSandwich/Previews/DefaultUVs");
				
			if (_DefaultUVsShader!=null){
				if (_DefaultUVsMat==null)
					_DefaultUVsMat = new Material(_DefaultUVsShader);
			}
			RenderTexture EditorWindowRT = RenderTexture.active;
			
			Graphics.SetRenderTarget(RTIcon);
			GL.Clear(true,true,BaseColor,1f);
			
			Shader.SetGlobalTexture("_UVs",RTTempUV);
			
			Material BlendMat;
			
			bool OnRTIcon = true;
			foreach(ShaderLayer SL in SLs){
				if (SL==stopAt)
					break;
				OnRTIcon = !OnRTIcon;
				
				Shader.SetGlobalTexture("_Previous",OnRTIcon?RTPing:RTIcon);
				Graphics.Blit(null,RTTempUV,_DefaultUVsMat);
				SL.UpdateIcon(RTTempUV,OnRTIcon?RTPing:RTIcon,OnRTIcon?RTIcon:RTPing);
				
				int AT = SL.AlphaBlendMode.Type;
				if (SL.Parent!=null&&SL.Parent.EndTag.Text=="a"){
					if (SL.UsesAlpha())
						AT = 0;
					else
						AT = 3;
				}
				BlendMat = ShaderUtil.GetGPUBlend(SL.MixType.Names[SL.MixType.Type],EndTag.Text,SL.MixAmount.Float,SL.UsesAlpha(),AT,SL.Stencil.Obj as ShaderLayerList,true,false);
				
				BlendMat.SetTexture("_Previous",OnRTIcon?RTPing:RTIcon);
				BlendMat.SetTexture("_New",SL.RTIcon);
				
				Graphics.Blit(SL.RTIcon,OnRTIcon?RTIcon:RTPing,BlendMat);
			}
			BlendMat = ShaderUtil.GetGPUBlend("Checker",EndTag.Text,1f,true,1,null,true,true);
			BlendMat.SetTexture("_Previous",OnRTIcon?RTIcon:RTPing);
			if (!OnRTIcon){
				//Graphics.Blit(RTIcon,RTIcon,ShaderUtil.GPUChecker.Material);
				Graphics.Blit(RTPing,RTIcon,BlendMat);
			}else {
				Graphics.Blit(RTIcon,RTPing,BlendMat);
				
				Graphics.Blit(RTPing,RTIcon);
			}
		
			Graphics.SetRenderTarget(EditorWindowRT);
			return;
		}else{
			//Debug.Log("Update!");
			Color[] colors = new Color[(int)(rect.x*rect.y)];
			for(int i = 0;i<colors.Length;i++){
				colors[i] = BaseColor;//new Color(0.8f,0.8f,0.8f,1f);
				//0.87
				//8
				Color checker1 = new Color(1f,1f,1f,1f);
				Color checker2 = new Color(0.87f,0.87f,0.87f,1f);
				if (UsesAlpha){
					//float ArrayPosX = Mathf.Floor(((float)i/rect.x));
					//float ArrayPosY = Mathf.Floor(((float)i/rect.y));
					float ArrayPosX = i%rect.x;
					float ArrayPosY = Mathf.Floor(i/rect.x);
					//float total = Mathf.Floor(ArrayPosX*8f) +Mathf.Floor(ArrayPosY*8f);
					//bool isEven = ((ArrayPosX/8f)%2.0f)<1f||(((ArrayPosY+8f)/8f)%2.0f)<1f;
					bool isEven = (((ArrayPosX+Mathf.Floor(ArrayPosY/8f)*8f)/8f)%2.0f)<1f;
					colors[i] = (isEven)? checker1:checker2;// new Color(ArrayPosX/rect.x,ArrayPosY/rect.y,0,1);//
					
				}
				if (EndTag.Text=="r")
					colors[i] = new Color(colors[i].r,colors[i].r,colors[i].r,colors[i].a);
				if (EndTag.Text=="g")
					colors[i] = new Color(colors[i].g,colors[i].g,colors[i].g,colors[i].a);
				if (EndTag.Text=="b")
					colors[i] = new Color(colors[i].b,colors[i].b,colors[i].b,colors[i].a);
				if (EndTag.Text=="a")
					colors[i] = new Color(colors[i].a,colors[i].a,colors[i].a,1);
			}
			if (Icon==null||(Icon.width!=(int)rect.x||Icon.height!=(int)rect.y))
			Icon = new Texture2D((int)rect.x,(int)rect.y,TextureFormat.ARGB32,false);
			
			//List<System.Reflection.MethodInfo> Meths = new List<System.Reflection.MethodInfo>();
			//List<int> MethTypes = new List<int>();

			foreach(ShaderLayer SL in SLs){
				/*Meths.Clear();
				MethTypes.Clear();
				foreach(ShaderEffect SE in SL.LayerEffects){
								Meths.Add(ShaderEffect.GetMethod(SE.TypeS,"Preview"));
								int MS = 0;
								if (Meths[Meths.Count-1]!=null){
								if (Meths[Meths.Count-1].GetParameters().Length==6)
								MS = 1;
								else
								if (Meths[Meths.Count-1].ReturnType==typeof(Vector2))
								MS = 2;
								}
								MethTypes.Add(MS);
				}*/
				
				//if (LayerTex!=null||(SL.LayerType.Type==(int)LayerTypes.Texture&&SL.Image.Image==null)||SL.LayerType.Type == (int)LayerTypes.Previous){
				if (1==1){
					int mipWidth = 0;
					int mipHeight = 0;
					//Color[] OrigLayerPixels = null;
					ShaderColor[] LayerPixels = null;
					if (SL.LayerType.Type != (int)LayerTypes.Previous){
							LayerPixels = new ShaderColor[(int)(rect.x*rect.y)];
							mipWidth = (int)rect.x;
							mipHeight = (int)rect.y;
		//				int i = -1;
						/*foreach(Color TC in TempLayerPixels){
							i++;
							LayerPixels[i] = (ShaderColor)TC;
						}*/
						
						if (SL.PreviewTex==null)
						SL.PreviewTex = new Texture2D(70,70,TextureFormat.ARGB32,false);
						
						int XX = 0;
						int YY = 0;	
						int SEI = -1;
						for(int x = 0;x<(int)mipWidth;x++){
							for(int y = 0;y<(int)mipHeight;y++){
								SEI = -1;
								XX = x;
								YY = y;
								//SL.LayerEffects.Reverse();
								//Meths.Reverse();
								//MethTypes.Reverse();
								foreach(ShaderEffect SE in SL.LayerEffects){
									SEI+=1;
									
									//var Meth = Meths[SEI];
									//int MethType = MethTypes[SEI];	
									if (SE.Visible){
										//if (Meth!=null){
											//if (MethType==2&&SL.LayerType.Type != (int)LayerTypes.Previous){
											if (SL.LayerType.Type != (int)LayerTypes.Previous){
												//Vector2 XY = (Vector2)Meth.Invooke(null, new object[]{SE,new Vector2(XX,YY),mipWidth,mipHeight});
												Vector2 XY = SE.Preview(SE.Inputs,SL,new Vector2(XX,YY),mipWidth,mipHeight);
												XX = (int)(XY.x);
												YY = (int)(XY.y);
											}
										//}
									}
								}
								//SL.LayerEffects.Reverse();
								//Meths.Reverse();
								//MethTypes.Reverse();
								//if (!SL.GetSample())
								//	LayerPixels[ShaderUtil.FlatArray(x,y,mipWidth,mipHeight,SL)] = (ShaderColor)OrigLayerPixels[ShaderUtil.FlatArray(XX,YY,mipWidth,mipHeight,SL)];
								//else
								//	LayerPixels[ShaderUtil.FlatArray(x,y,mipWidth,mipHeight,SL)] = SL.GetSample((float)XX/((float)mipWidth),(float)YY/((float)mipHeight));
								ShaderColor Sample = SL.GetSample((float)XX/((float)mipWidth),(float)YY/((float)mipHeight));
								if (Sample==null)
									Sample = (ShaderColor)colors[ShaderUtil.FlatArray(x,y,mipWidth,mipHeight,SL)];
								
								LayerPixels[ShaderUtil.FlatArray(x,y,mipWidth,mipHeight,SL)] = Sample;
								
								SL.PreviewTex.SetPixel(x,y,(Color)Sample);
							}
						}
						
						
						//if (EndTag.Text!="a"){
						XX = 0;
						YY = 0;
						SEI = -1;
							
						foreach(ShaderEffect SE in SL.LayerEffects){
							SEI+=1;
							if (SE.Visible){
								//var Meth = Meths[SEI];
								//int MethType = MethTypes[SEI];					
								for(int x = 0;x<(int)mipWidth;x++){
									for(int y = 0;y<(int)mipHeight;y++){
										XX = x;
										YY = y;
										ShaderColor OldCol = new ShaderColor(LayerPixels[ShaderUtil.FlatArray(XX,YY,mipWidth,mipHeight)].r,LayerPixels[ShaderUtil.FlatArray(XX,YY,mipWidth,mipHeight)].g,LayerPixels[ShaderUtil.FlatArray(XX,YY,mipWidth,mipHeight)].b,LayerPixels[ShaderUtil.FlatArray(XX,YY,mipWidth,mipHeight)].a);
										//if (Meth!=null){
											//if (MethType==1&&SL.LayerType.Type != (int)LayerTypes.Previous)
											//LayerPixels[ShaderUtil.FlatArray(x,y,mipWidth,mipHeight)] = (ShaderColor)SE.Preview(SE.Inputs.D, SL, new Vector2(XX,YY), LayerPixels, LayerPixels[ShaderUtil.FlatArray(XX,YY,mipWidth,mipHeight)]);
											//LayerPixels[ShaderUtil.FlatArray(x,y,mipWidth,mipHeight)] = (ShaderColor)Meth.Invooke(null, new object[]{SE, LayerPixels,XX,YY,mipWidth,mipHeight});
											
											//if (MethType==0)
											//Debug.Log((Color)Meth.Invooke(null, new object[]{SE, (Color)LayerPixels[ShaderUtil.FlatArray(XX,YY,mipWidth,mipHeight)]}));
											//if (MethType==0)
											LayerPixels[ShaderUtil.FlatArray(x,y,mipWidth,mipHeight)] = (ShaderColor)SE.Preview(SE.Inputs, SL, new Vector2(XX,YY), LayerPixels, LayerPixels[ShaderUtil.FlatArray(XX,YY,mipWidth,mipHeight)]);
											
											if (SE.UseAlpha.Float==0)
												LayerPixels[ShaderUtil.FlatArray(x,y,mipWidth,mipHeight)].a = OldCol.a;
											if (SE.UseAlpha.Float==2){
												float A = LayerPixels[ShaderUtil.FlatArray(x,y,mipWidth,mipHeight)].a;
												LayerPixels[ShaderUtil.FlatArray(x,y,mipWidth,mipHeight)] = OldCol;
												LayerPixels[ShaderUtil.FlatArray(x,y,mipWidth,mipHeight)].a = A;
											}
											
											SL.PreviewTex.SetPixel(x,y,(Color)LayerPixels[ShaderUtil.FlatArray(x,y,mipWidth,mipHeight)]);
										//}
									}
								}
							}
						}
						SL.PreviewTex.Apply();
					}
					//}
					//SL.LayerType.Type != (int)LayerTypes.Previous
					//Debug.Log(LayerPixels.Length);
					for(int x = 0;x<(int)rect.x;x++){
						for(int y = 0;y<(int)rect.y;y++){
						
							//float Progress = 0.5f;
							//EditorUtility.DisplayProgressBar("Updating Preview", "", Progress);
						
							ShaderColor OldColor = (ShaderColor)colors[x+(y*(int)rect.x)];
							ShaderColor LayerPixel = OldColor;
							ShaderColor LayerPixelOrig = OldColor;
							if (SL.LayerType.Type != (int)LayerTypes.Previous){
								float ArrayPosX = Mathf.Floor(((float)x/rect.x)*((float)mipWidth));
								float ArrayPosY = Mathf.Floor(((float)y/rect.y)*((float)mipHeight));
							
								float ArrayPos = ShaderUtil.FlatArray((int)ArrayPosX,(int)ArrayPosY,mipWidth,mipHeight);
								LayerPixel = LayerPixels[(int)ArrayPos];
							}
							/*else{
								//LayerPixel = new Color(1,0,0,1);
								int SEI = -1;
								foreach(ShaderEffect SE in SL.LayerEffects){
									SEI+=1;
									if (SE.Visible){
										///int MethType = MethTypes[SEI];
										///var Meth = Meths[SEI];
										
										///if (MethType == 0){
										///LayerPixel = (ShaderColor)Meth.Invooke(null, new object[]{SE, LayerPixel});}
										///TODO: not sure what this part does anymore...
									}
								}
							}*/
							
							
							
							
							
								LayerPixelOrig =LayerPixel;
								if (SL.MixType.Names[SL.MixType.Type]=="Add")
								LayerPixel += OldColor;
								if (SL.MixType.Names[SL.MixType.Type]=="Subtract")
								LayerPixel = OldColor-LayerPixel;
								if (SL.MixType.Names[SL.MixType.Type]=="Divide")
								//LayerPixel = OldColor-LayerPixel;
								LayerPixel = new ShaderColor(OldColor.r/LayerPixel.r,OldColor.g/LayerPixel.g,OldColor.b/LayerPixel.b,OldColor.a/LayerPixel.a);
								if (SL.MixType.Names[SL.MixType.Type]=="Multiply")
								LayerPixel *= OldColor;
								if (SL.MixType.Names[SL.MixType.Type]=="Lighten")
								LayerPixel = new ShaderColor(Mathf.Max(OldColor.r,LayerPixel.r),Mathf.Max(OldColor.g,LayerPixel.g),Mathf.Max(OldColor.b,LayerPixel.b),Mathf.Max(OldColor.a,LayerPixel.a));
								if (SL.MixType.Names[SL.MixType.Type]=="Darken")
								LayerPixel = new ShaderColor(Mathf.Min(OldColor.r,LayerPixel.r),Mathf.Min(OldColor.g,LayerPixel.g),Mathf.Min(OldColor.b,LayerPixel.b),Mathf.Min(OldColor.a,LayerPixel.a));
								if (SL.MixType.Names[SL.MixType.Type]=="Normals Mix"){
									ShaderColor BOldColor = (OldColor);//-new ShaderColor(0.5f,0.5f,0.5f,0.5f))*2f;
									ShaderColor BLayerPixel = (LayerPixel);//-new ShaderColor(0.5f,0.5f,0.5f,0.5f))*2f;
									Vector3 NormalBlend = new Vector3(BOldColor.r+BLayerPixel.r,BOldColor.g+BLayerPixel.g,OldColor.b);
									//LayerPixel /=  Mathf.Max(LayerPixel.b,Mathf.Max(LayerPixel.r,LayerPixel.g));//(Color)(((Vector4)LayerPixel).normalized);
									NormalBlend.Normalize();
									//NormalBlend = NormalBlend/2f+new Vector3(0.5f,0.5f,0.5f);
									LayerPixel = new ShaderColor(NormalBlend.x,NormalBlend.y,NormalBlend.z,LayerPixel.a);
								}
								if (SL.MixType.Names[SL.MixType.Type]=="Dot")
								LayerPixel = new ShaderColor(Vector3.Dot(new Vector3(OldColor.r,OldColor.g,OldColor.b),new Vector3(LayerPixelOrig.r,LayerPixelOrig.g,LayerPixelOrig.b)));
								
								if (SL.AlphaBlendMode.Type==1&&SL.Stencil.Obj!=null){
								LayerPixel = ShaderColor.Lerp(OldColor,LayerPixel,
								LayerPixelOrig.a*
								SL.MixAmount.Float*
								(((ShaderLayerList)(SL.Stencil.Obj)).GetCPUIcon().GetPixel(x,y).r));
								}
								else
								if (SL.AlphaBlendMode.Type==1)
								LayerPixel = ShaderColor.Lerp(OldColor,LayerPixel,LayerPixelOrig.a*SL.MixAmount.Float);
								else
								if (SL.Stencil.Obj!=null)
								LayerPixel = ShaderColor.Lerp(OldColor,LayerPixel,
								SL.MixAmount.Float*
								(((ShaderLayerList)(SL.Stencil.Obj)).GetCPUIcon().GetPixel(x,y).r));
								else
								LayerPixel = ShaderColor.Lerp(OldColor,LayerPixel,SL.MixAmount.Float);
							//}
							//if (!SL.UseAlpha.On){
							
							if (EndTag.Text=="r")
								LayerPixel = new ShaderColor(LayerPixel.r,LayerPixel.r,LayerPixel.r,1);
							if (EndTag.Text=="g")
								LayerPixel = new ShaderColor(LayerPixel.g,LayerPixel.g,LayerPixel.g,1);
							if (EndTag.Text=="b")
								LayerPixel = new ShaderColor(LayerPixel.b,LayerPixel.b,LayerPixel.b,1);
							if (EndTag.Text=="a")
								LayerPixel = new ShaderColor(LayerPixel.a,LayerPixel.a,LayerPixel.a,1);
							
							//if (EndTag.Text.Length!=1)
							LayerPixel.a = 1;//OldColor.a;
							//}

							colors[x+(y*(int)rect.x)] = (Color)LayerPixel;
						}
					}
					
				}
			}
			if (CodeName=="d.Normal"){
				for(int x = 0;x<(int)rect.x;x++){
					for(int y = 0;y<(int)rect.y;y++){
						ShaderColor LayerPixel2 = (ShaderColor)colors[ShaderUtil.FlatArray(x,y,(int)rect.x,(int)rect.y)];
						LayerPixel2/=2f;
						LayerPixel2+= new ShaderColor(0.5f,0.5f,0.5f,0f);	
						LayerPixel2.a = 1f;
						colors[ShaderUtil.FlatArray(x,y,(int)rect.x,(int)rect.y)] = (Color)LayerPixel2;
					}
				}
			}
			/*if (Function!=""){
				ShaderEffect SE = new ShaderEffect(Function);
				Meth = ShaderEffect.GetMethod(Function,"Preview");
				for(int x = 0;x<(int)rect.x;x++){
					for(int y = 0;y<(int)rect.y;y++){
						Color LayerPixel2 = colors[ShaderUtil.FlatArray(x,y,(int)rect.x,(int)rect.y)];
						LayerPixel2 = (Color)Meth.Invooke(null, new object[]{SE, LayerPixel2});
						LayerPixel2.a = 1;
						colors[ShaderUtil.FlatArray(x,y,(int)rect.x,(int)rect.y)] = LayerPixel2;
					}
				}
			}		*/
			ImagePixels = colors;
			Icon.SetPixels(colors);
			Icon.Apply(false);	
			//EditorUtility.ClearProgressBar();
		}
	}
	[NonSerialized]public Color[] ImagePixels;
	public void DrawIcon(Rect rect, bool Update){
		if (Event.current.type==EventType.Repaint){
			if (ShaderSandwich.Instance.PreviewSystem==PreviewSystem.CPU){
				if (Icon==null){
					UpdateIcon(new Vector2(rect.width,rect.height));
				}
				Graphics.DrawTexture(rect,Icon,ShaderUtil.GetMaterial("Mix", new Color(1f,1f,1f,1f),false));
			}else{
				if (_RTIcon==null){
					//Debug.Log("aaahhh!");
					UpdateIcon(new Vector2(rect.width,rect.height));
				}
				//Graphics.DrawTexture(rect,RTIcon,ShaderUtil.GPUChecker.Material);
				//Graphics.Blit(RTPing,RTPing,ShaderUtil.GPUChecker.Material);
				//Material BlendMat = ShaderUtil.GetGPUBlend("Mix",1f,1,null,true);
				//BlendMat.SetTexture("_Previous",RTPing);
				//Graphics.DrawTexture(rect,RTIcon,BlendMat);//ShaderUtil.GetMaterial("Mix", new Color(1f,1f,1f,1f),true));
				Graphics.DrawTexture(rect,RTIcon);//ShaderUtil.GetMaterial("Mix", new Color(1f,1f,1f,1f),true));
			}
		}
	}
	public Color[] GetImagePixels(){
		if (ImagePixels==null||ImagePixels.Length<10){
			UpdateIcon(new Vector2(70,70));
		}
		return ImagePixels;
	}
	public Texture2D GetCPUIcon(){
		if (Icon==null){
			UpdateIcon(new Vector2(70,70));
		}
		return Icon;
	}
	public Texture GetIcon(){
		Texture icon;
		if (ShaderSandwich.Instance.PreviewSystem == PreviewSystem.GPU)
			icon = RTIcon;
		else
			icon = Icon;
		
		if (icon==null){
			UpdateIcon(new Vector2(70,70));
		}
		return icon;
	}
	public Dictionary<string,ShaderVar> GetSaveLoadDict(){
		Dictionary<string,ShaderVar> D = new Dictionary<string,ShaderVar>();
		
		D.Add(NameUnique.Name,NameUnique);
		D.Add(Name.Name,Name);
		D.Add(IsMask.Name,IsMask);
		//D.Add(IsLighting.Name,IsLighting);
		D.Add(EndTag.Name,EndTag);
		return D;
	}	
	public string Save(int tabs){
		string S = "\n"+ShaderUtil.Tabs[tabs]+"Begin Shader Layer List\n\n";
		S+=ShaderUtil.SaveDict(GetSaveLoadDict(),tabs+1);

		foreach (ShaderLayer SL in SLs){
			S+=SL.Save(tabs+1);
		}
		S += "\n"+ShaderUtil.Tabs[tabs]+"End Shader Layer List\n";
		//public List<ShaderLayer> SLs = new List<ShaderLayer>();
		return S;
	}
	static public void LoadLegacy(StringReader S,List<ShaderLayerList> SLLs,List<ShaderLayerList> SLMs){
		LoadLegacy(S,SLLs,SLMs,"#(%*&");
	}
	static public ShaderLayerList LoadSingle(StringReader S,ShaderLayerList SLL){
		if (SLL!=null){
			//Debug.Log(SLL.Name.Text);
			var D = SLL.GetSaveLoadDict();
			while(1==1){
				string Line =  S.ReadLine();
				if (Line!=null){
					Line = ShaderUtil.Sanitize(Line);
					if(Line=="End Shader Layer List")break;
					
					if (Line.Contains(":")){
						ShaderUtil.LoadLine(D,Line);
					}
					else
					if (Line=="Begin Shader Layer"){
						if (ShaderSandwich.Instance.OpenShader.FileVersion>=2f)
							SLL.SLs.Add(ShaderLayer.Load(S));
						//else
						//	SLL.SLs.Add(ShaderLayer.LoadOld(S));
					}
				}
				else
				break;
			}
			//SLL.IsLighting.On = ShaderLayerIsLighting;
			SLL.UpdateIcon(new Vector2(70,70));
		}
		return SLL;
	}
	static public ShaderLayerList LoadFromGroup(StringReader S,List<ShaderLayerList> SLLs){
		if (SLLs!=null){
			//Debug.Log(SLL.Name.Text);
			bool StillSearching = true;
			ShaderLayerList SLL = ScriptableObject.CreateInstance<ShaderLayerList>();
			var D = SLL.GetSaveLoadDict();
			D["EndTag"].Text = "";
			while(1==1){
				string Line =  S.ReadLine();
				if (Line!=null){
					Line = ShaderUtil.Sanitize(Line);
					if(Line=="End Shader Layer List")break;
					
					if (Line.Contains(":")){
						ShaderUtil.LoadLine(D,Line);
						if (StillSearching&&((D["LayerListUniqueName"].Text!=""&&D["LayerListUniqueName"].Text!="Surface")||D["LayerListName"].Text!="")){
							foreach(ShaderLayerList sll in SLLs){
								if ((sll.NameUnique.Text==D["LayerListUniqueName"].Text&&D["LayerListUniqueName"].Text!="Surface")||sll.Name.Text==D["LayerListName"].Text){
									var tD = sll.GetSaveLoadDict();
									StillSearching = false;
									if (D["EndTag"].Text!="")tD["EndTag"].Text = D["EndTag"].Text;
									D = tD;
									sll.SLs = SLL.SLs;
									SLL = sll;
									break;
								}
							}
						}
					}
					else
					if (Line=="Begin Shader Layer"){
						if (ShaderSandwich.Instance.OpenShader.FileVersion>=2f)
							SLL.SLs.Add(ShaderLayer.Load(S));
						//else
						//	SLL.SLs.Add(ShaderLayer.LoadOld(S));
					}
				}
				else
				break;
			}
			//SLL.IsLighting.On = ShaderLayerIsLighting;
			SLL.UpdateIcon(new Vector2(70,70));
			return SLL;
		}
		Debug.Log("Hi! There was a very very odd error when loading, could you please send a message to Ztechnologies.com@gmail.com with the shader file? Thanks :)");
		return null;
	}
	static public void LoadUnused(StringReader S,List<ShaderLayerList> SLLs,List<ShaderLayerList> SLMs){
		ShaderLayerList SLL = null;
		//if (Line=="#(%*&")
		string Line = S.ReadLine();
//		Debug.Log(Line);
		string ShaderLayerUniqueName = ShaderUtil.LoadLineExplode(Line)[1].Trim();
		string ShaderLayerName = ShaderUtil.LoadLineExplode(S.ReadLine())[1].Trim();
		string ShaderLayerIsMaskb = ShaderUtil.LoadLineExplode(S.ReadLine())[1].Trim();
		if (ShaderSandwich.Instance.OpenShader.FileVersion>=2f){
			ShaderLayerUniqueName = ShaderUtil.GetValueFromExplode(ShaderLayerUniqueName);
			ShaderLayerName = ShaderUtil.GetValueFromExplode(ShaderLayerName);
			ShaderLayerIsMaskb = ShaderUtil.GetValueFromExplode(ShaderLayerIsMaskb);
		}
		else{
			ShaderLayerUniqueName = ShaderUtil.GetValueFromExplodeOld(ShaderLayerUniqueName);
			ShaderLayerName = ShaderUtil.GetValueFromExplodeOld(ShaderLayerName);
			ShaderLayerIsMaskb = ShaderUtil.GetValueFromExplodeOld(ShaderLayerIsMaskb);
		}
		//Debug.Log(ShaderLayerUniqueName);
		//Debug.Log(ShaderLayerIsMaskb);
		bool ShaderLayerIsMask = bool.Parse(ShaderLayerIsMaskb);
//		bool ShaderLayerIsLighting = false;
		if (ShaderSandwich.Instance.OpenShader.FileVersion>0.9f){
			string ShaderLayerIsLightingb = ShaderUtil.LoadLineExplode(S.ReadLine())[1].Trim();
			if (ShaderSandwich.Instance.OpenShader.FileVersion>=2f)
				ShaderLayerIsLightingb = ShaderUtil.GetValueFromExplode(ShaderLayerIsLightingb);
			else
				ShaderLayerIsLightingb = ShaderUtil.GetValueFromExplodeOld(ShaderLayerIsLightingb);
			//ShaderLayerIsLighting = bool.Parse(ShaderLayerIsLightingb);
		}
		foreach(ShaderLayerList SLL2 in SLLs){
			if (SLL2.NameUnique.Text==ShaderLayerUniqueName||(SLL2.NameUnique.Text=="Albedo"&&ShaderLayerUniqueName=="Diffuse"))
			SLL = SLL2;
			if (ShaderSandwich.Instance.OpenShader.FileVersion<2f){
				ShaderLayerUniqueName = ShaderLayerUniqueName.Replace("Shell","");
				if (SLL2.NameUnique.Text==ShaderLayerUniqueName||(SLL2.NameUnique.Text=="Albedo"&&ShaderLayerUniqueName=="Diffuse"))
				SLL = SLL2;
			}
		}
		if (SLL==null){
			SLL = ScriptableObject.CreateInstance<ShaderLayerList>();
			SLL.UShaderLayerList("Mask"+SLMs.Count.ToString(),"Mask"+SLMs.Count.ToString(),"Mask"+SLMs.Count.ToString(),"Mask"+SLMs.Count.ToString(),"rgb","",new Color(1f,1f,1f,1f));
			SLL.IsMask.On=true;
			SLMs.Add(SLL);
		}
		if (SLL!=null){
			SLL.Name.Text = ShaderLayerName;
			var D = SLL.GetSaveLoadDict();
			while(1==1){
				Line =  S.ReadLine();
				if (Line!=null){
					if(Line=="EndShaderLayerList")break;
					
					if (Line.Contains("#!")){
						if (ShaderUtil.LoadLineExplode(Line)[0]!="EndTag"||SLL.EndTag.Text.Length==1||(ShaderUtil.LoadLineExplode(Line)[0]=="EndTag"&&ShaderLayerIsMask==true))
							ShaderUtil.LoadLine(D,Line);
					}
					else
					if (Line=="BeginShaderLayer"){
						if (ShaderSandwich.Instance.OpenShader.FileVersion>=2f)
							SLL.SLs.Add(ShaderLayer.Load(S));
						//else
						//	SLL.SLs.Add(ShaderLayer.LoadOld(S));
					}
				}
				else
				break;
			}
			//SLL.IsLighting.On = ShaderLayerIsLighting;
			SLL.UpdateIcon(new Vector2(70,70));
		}
		
	}
	static public void LoadLegacy(StringReader S,List<ShaderLayerList> SLLs,List<ShaderLayerList> SLMs,string Line){
		ShaderLayerList SLL = null;
		if (Line=="#(%*&")
		Line = S.ReadLine();
//		Debug.Log(Line);
		string ShaderLayerUniqueName = ShaderUtil.LoadLineExplode(Line)[1].Trim();
		string ShaderLayerName = ShaderUtil.LoadLineExplode(S.ReadLine())[1].Trim();
		string ShaderLayerIsMaskb = ShaderUtil.LoadLineExplode(S.ReadLine())[1].Trim();
		if (ShaderSandwich.Instance.OpenShader.FileVersion>=2f){
			ShaderLayerUniqueName = ShaderUtil.GetValueFromExplode(ShaderLayerUniqueName);
			ShaderLayerName = ShaderUtil.GetValueFromExplode(ShaderLayerName);
			ShaderLayerIsMaskb = ShaderUtil.GetValueFromExplode(ShaderLayerIsMaskb);
		}
		else{
			ShaderLayerUniqueName = ShaderUtil.GetValueFromExplodeOld(ShaderLayerUniqueName);
			ShaderLayerName = ShaderUtil.GetValueFromExplodeOld(ShaderLayerName);
			ShaderLayerIsMaskb = ShaderUtil.GetValueFromExplodeOld(ShaderLayerIsMaskb);
		}
		//Debug.Log(ShaderLayerUniqueName);
		//Debug.Log(ShaderLayerIsMaskb);
		bool ShaderLayerIsMask = bool.Parse(ShaderLayerIsMaskb);
//		bool ShaderLayerIsLighting = false;
		if (ShaderSandwich.Instance.OpenShader.FileVersion>0.9f){
			string ShaderLayerIsLightingb = ShaderUtil.LoadLineExplode(S.ReadLine())[1].Trim();
			if (ShaderSandwich.Instance.OpenShader.FileVersion>=2f)
				ShaderLayerIsLightingb = ShaderUtil.GetValueFromExplode(ShaderLayerIsLightingb);
			else
				ShaderLayerIsLightingb = ShaderUtil.GetValueFromExplodeOld(ShaderLayerIsLightingb);
			//ShaderLayerIsLighting = bool.Parse(ShaderLayerIsLightingb);
		}
		foreach(ShaderLayerList SLL2 in SLLs){
			if (SLL2.NameUnique.Text==ShaderLayerUniqueName||(SLL2.NameUnique.Text=="Albedo"&&ShaderLayerUniqueName=="Diffuse"))
			SLL = SLL2;
			if (ShaderSandwich.Instance.OpenShader.FileVersion<2f){
				ShaderLayerUniqueName = ShaderLayerUniqueName.Replace("Shell","");
				if (SLL2.NameUnique.Text==ShaderLayerUniqueName||(SLL2.NameUnique.Text=="Albedo"&&ShaderLayerUniqueName=="Diffuse"))
				SLL = SLL2;
			}
		}
		if (SLL==null){
			SLL = ScriptableObject.CreateInstance<ShaderLayerList>();
			SLL.UShaderLayerList("Mask"+SLMs.Count.ToString(),"Mask"+SLMs.Count.ToString(),"Mask"+SLMs.Count.ToString(),"Mask"+SLMs.Count.ToString(),"rgb","",new Color(1f,1f,1f,1f));
			SLL.IsMask.On=true;
			SLMs.Add(SLL);
		}
		if (SLL!=null){
			SLL.Name.Text = ShaderLayerName;
			//Debug.Log(ShaderLayerName);
			var D = SLL.GetSaveLoadDict();
			while(1==1){
				Line =  S.ReadLine();
				if (Line!=null){
					if(Line=="EndShaderLayerList")break;
					
					if (Line.Contains("#!")){
						if (ShaderUtil.LoadLineExplode(Line)[0]!="EndTag"||SLL.EndTag.Text.Length==1||(ShaderUtil.LoadLineExplode(Line)[0]=="EndTag"&&ShaderLayerIsMask==true)){
							//Debug.Log("goitaleg");
							ShaderUtil.LoadLineLegacy(D,Line);
						}
					}
					else
					if (Line=="BeginShaderLayer"){
						//Debug.Log("ShaderLayer");
						if (ShaderSandwich.Instance.OpenShader.FileVersion>=2f)
							SLL.SLs.Add(ShaderLayer.LoadLegacy(S));
						//else
						//	SLL.SLs.Add(ShaderLayer.LoadOld(S));
					}
				}
				else
				break;
			}
			//SLL.IsLighting.On = ShaderLayerIsLighting;
			SLL.UpdateIcon(new Vector2(70,70));
		}
		
	}
	public override string ToString(){
		return Name.Text;
	}
	public string GenerateCode(ShaderData SD,bool Premultiplied){
		string ShaderCode = "";

		if (SLs.Count>0){
			ShaderCode += "	//Generate layers for the " + Name.Text + " channel.\n";
			
			int ShaderNumber = 0;
			foreach (ShaderLayer SL in SLs){
				/*if (SG.InVertex){
					if (SG.VertexSpace != (VertexSpaces)SL.VertexSpace.Float){
						if (SG.VertexSpace == VertexSpaces.Object&&SL.VertexSpace.Float == (int)VertexSpaces.World){
							ShaderCode+="	//Transform into world space\n	Vertex.xyz = mul(_Object2World,Vertex.xyz);\n";
							SG.VertexSpace = VertexSpaces.World;
						}
						if (SG.VertexSpace == VertexSpaces.World&&SL.VertexSpace.Float == (int)VertexSpaces.Object){
							ShaderCode+="	//Transform back into object space\n	Vertex.xyz = mul(_World2Object,Vertex.xyz);\n";
							SG.VertexSpace = VertexSpaces.Object;
						}
					}
				}*/
				
				//if (SL.MapType.Type!=(int)ShaderMapType.RimLight||SG.Pass!="ShadowCaster"){
					ShaderNumber+=1;
					
					if (SL.MixAmount.Get()!="0"){
						//SL.LayerEffects.Reverse();
						//string Map = SL.GCUVs(SD);
						//SL.LayerEffects.Reverse();
						SL.Parent = this;
						
						string PixelColor = SL.GCPixel(SD);
						if (PixelColor.Length>0){
							PixelColor+="\n";
							//PixelColor += SL.GCCalculateMix(SD,CodeName,SL.GetSampleNameFirst(),Function,ShaderNumber)+"\n\n";
							PixelColor += SL.CalculateMix(SD,CodeName,SL.GetSampleNameFirst(),Premultiplied)+"\n\n";
							ShaderCode += PixelColor;
						}
					}
					else
					ShaderCode += "		//Generate Layer: "+Name.Text+"\n			//The layer has a Mix Amount of 0, which means forget about it :)\n";
				//}
			}
			/*if (SG.InVertex){
				if (SG.VertexSpace != VertexSpaces.Object){
					ShaderCode+="	//Transform back into object space\n	Vertex.xyz = mul(_World2Object,Vertex.xyz);\n";
					SG.VertexSpace = VertexSpaces.Object;
				}
			}*/
		}

		return ShaderCode;
	}
}
}