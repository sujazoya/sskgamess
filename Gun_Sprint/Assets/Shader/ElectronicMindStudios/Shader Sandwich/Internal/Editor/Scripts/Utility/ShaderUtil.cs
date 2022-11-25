#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#define PRE_MORESTUPIDUNITYDEPRECATIONS
#endif
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System;
using EMindGUI = ElectronicMind.ElectronicMindStudiosInterfaceUtils;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
public enum Frag{Surf, VertFrag};
public enum Vert{Surf, VertFrag};

static public class ShaderUtil{
	static ShaderUtil(){
		GPUChecker = ScriptableObject.CreateInstance<ShaderShaderAndMaterialBadcooodIthinkdotdotdot>();
		GPUChecker.shader = "Hidden/ShaderSandwich/Previews/TransparencyChecker";
		GPUMixMat = ScriptableObject.CreateInstance<ShaderShaderAndMaterialBadcooodIthinkdotdotdot>();
		GPUMixMat.shader = "Hidden/ShaderSandwich/Previews/Mix";
		GPUAddMat = ScriptableObject.CreateInstance<ShaderShaderAndMaterialBadcooodIthinkdotdotdot>();
		GPUAddMat.shader = "Hidden/ShaderSandwich/Previews/Add";
		GPUSubMat = ScriptableObject.CreateInstance<ShaderShaderAndMaterialBadcooodIthinkdotdotdot>();
		GPUSubMat.shader = "Hidden/ShaderSandwich/Previews/Sub";
		GPUGUIMat = ScriptableObject.CreateInstance<ShaderShaderAndMaterialBadcooodIthinkdotdotdot>();
		GPUGUIMat.shader = "Hidden/ShaderSandwich/GUITextureThingo";
	}
	static public void TimerDebug(System.Diagnostics.Stopwatch sw){
		string ExecutionTimeTaken = string.Format("Minutes :{0}\nSeconds :{1}\n Mili seconds :{2}",sw.Elapsed.Minutes,sw.Elapsed.Seconds,sw.Elapsed.TotalMilliseconds);
		UnityEngine.Debug.Log(ExecutionTimeTaken);	
	}
	static public void TimerDebug(System.Diagnostics.Stopwatch sw,string Add){
		string ExecutionTimeTaken = string.Format(Add+":\nMinutes :{0}\nSeconds :{1}\n Mili seconds :{2}",sw.Elapsed.Minutes,sw.Elapsed.Seconds,sw.Elapsed.TotalMilliseconds);
		UnityEngine.Debug.Log(ExecutionTimeTaken);	
		//sw.Reset();
	}
	static public int FlatArray(int X,int Y,int W,int H){
		return FlatArray(X,Y,W,H,null);
	}
	static public int FlatArray(int X,int Y,int W,int H,ShaderLayer SL){
		int ArrayPosX=0;
		int ArrayPosY=0;
		if (SL==null||SL.LayerType.Type!=(int)LayerTypes.Gradient){
			if (X!=0&&(W!=0))
			ArrayPosX = Mathf.Abs(X) % (W);//Mathf.Max(0,Mathf.Min(X,W-1));//Mathf.Floor(((float)X));
			if (Y!=0&&(H!=0))
			ArrayPosY = Mathf.Abs(Y) % (H);
		}
		else{
			ArrayPosX = Mathf.Max(0,Mathf.Min(X,W-1));
			ArrayPosY = Mathf.Max(0,Mathf.Min(Y,H-1));
		}

		
		//ArrayPosX = Mathf.Max(0,Mathf.Min(X,W-1));//Mathf.Floor(((float)Y));
		//ArrayPosY = Mathf.Max(0,Mathf.Min(Y,H-1));//Mathf.Floor(((float)Y));
		int ArrayPos = (ArrayPosY*(W))+ArrayPosX;
		return ArrayPos;
	}
	static public List<ShaderVar> GetAllShaderVars(){
		/*List<ShaderVar> SVs = new List<ShaderVar>();
		if (ShaderSandwich.Instance.OpenShader!=null){
			foreach (ShaderLayer SL in ShaderUtil.GetAllLayers()){
				SL.UpdateShaderVars(true);
				SVs.AddRange(SL.ShaderVars);
			}
			foreach (ShaderInput SI in ShaderBase.Current.ShaderInputs){
				SVs.Add(SI.Mask);
			}
			SVs.AddRange(ShaderBase.Current.GetMyShaderVars());
		}*/
		ShaderVarsCache = GetAllShaderVarsEnumerator().ToList();
		return ShaderVarsCache;
	}
	static public List<ShaderVar> GetAllShaderVarsCached(){
		return ShaderVarsCache;
	}
	static public List<ShaderVar> ShaderVarsCache;
	static public IEnumerable<ShaderVar> GetAllShaderVarsEnumerator(){///TODO: Make work properly :P
		List<ShaderVar> SVs = new List<ShaderVar>();
		if (ShaderSandwich.Instance.OpenShader!=null){
			foreach (ShaderLayer SL in ShaderUtil.GetAllLayers()){
				SL.UpdateShaderVars(true);
				SVs.AddRange(SL.ShaderVars);
			}
			foreach (ShaderInput SI in ShaderBase.Current.ShaderInputs){
				SVs.Add(SI.Mask);
			}
			SVs.AddRange(ShaderBase.Current.GetMyShaderVars());
		}
		return SVs.Distinct();
	}
	
	static public List<ShaderLayer> GetAllLayers(){
		List<ShaderLayer> tempList = new List<ShaderLayer>();
		if (ShaderSandwich.Instance.OpenShader!=null){
			foreach (List<ShaderLayer> SLL in ShaderSandwich.Instance.OpenShader.GetShaderLayers())
			tempList.AddRange(SLL);
		}
		
		return tempList;
	}
	static public List<ShaderLayer> GetAllLayers(ShaderPass SP){
		List<ShaderLayer> tempList = new List<ShaderLayer>();
		foreach (List<ShaderLayer> SLL in SP.GetShaderLayers())
			tempList.AddRange(SLL);
		if (ShaderSandwich.Instance!=null){
			foreach(ShaderLayerList SLL in ShaderSandwich.Instance.OpenShader.ShaderLayersMasks)
				tempList.AddRange(SLL.SLs);
		}
		
		return tempList;
	}
	
	static Material MixMaterial;
	static Material AddMaterial;
	static Material SubMaterial;
	static Material MulMaterial;
	static Material DivMaterial;
	static Material DarMaterial;
	static Material LigMaterial;	
	
	public static Material NAMixMaterial;
	static Material NAAddMaterial;
	static Material NASubMaterial;
	static Material NAMulMaterial;
	static Material NADivMaterial;
	static Material NADarMaterial;
	static Material NALigMaterial;
	
	
	static public ShaderShaderAndMaterialBadcooodIthinkdotdotdot GPUChecker;
	static public ShaderShaderAndMaterialBadcooodIthinkdotdotdot GPUMixMat;
	static public ShaderShaderAndMaterialBadcooodIthinkdotdotdot GPUAddMat;
	static public ShaderShaderAndMaterialBadcooodIthinkdotdotdot GPUSubMat;
	static public ShaderShaderAndMaterialBadcooodIthinkdotdotdot GPUGUIMat;
	
	static public string something;
	
	static public Material GetGPUBlend(string MixType, string EndTag, float Amount,bool Transparent,int AlphaBlendMode,ShaderLayerList Mask, bool CreatesPremul, bool IsPremul){
		Material BlendMat = null;
		int BlendMode = 0;
		if (MixType=="Checker")
			BlendMat = ShaderUtil.GPUChecker.Material;
		else{
			if (MixType=="Mix")BlendMode = 0; else
			if (MixType=="Add")BlendMode = 1; else
			if (MixType=="Subtract")BlendMode = 2; else
			if (MixType=="Multiply")BlendMode = 3; else
			if (MixType=="Divide")BlendMode = 4; else
			if (MixType=="Lighten")BlendMode = 5; else
			if (MixType=="Darken")BlendMode = 6; else
			if (MixType=="Normals Mix")BlendMode = 7; else
			if (MixType=="Dot")BlendMode = 8;
			BlendMat = ShaderUtil.GPUMixMat.Material;
		}
		if (BlendMat==null)
			Debug.LogWarning("Sorry, something went wrong rendering the GPU preview - '"+MixType+"'");
		BlendMat.SetFloat("_Amount",Amount);
		BlendMat.SetFloat("_AlphaMode",AlphaBlendMode);
		BlendMat.SetFloat("_UseMask",Mask!=null?1f:0f);
		BlendMat.SetFloat("_CreatesPremul",CreatesPremul?1f:0f);
		BlendMat.SetFloat("_IsPremul",IsPremul?1f:0f);
		BlendMat.SetFloat("_Transparent",Transparent?1f:0f);
		BlendMat.SetTexture("_Mask",Mask!=null?Mask.GetIcon():null);
		BlendMat.SetFloat("_BlendMode",BlendMode);
		
		if (EndTag=="rgba")
			BlendMat.SetFloat("_EndTag",0f);
		if (EndTag=="rgb")
			BlendMat.SetFloat("_EndTag",1f);
		if (EndTag=="r")
			BlendMat.SetFloat("_EndTag",2f);
		if (EndTag=="g")
			BlendMat.SetFloat("_EndTag",3f);
		if (EndTag=="b")
			BlendMat.SetFloat("_EndTag",4f);
		if (EndTag=="a")
			BlendMat.SetFloat("_EndTag",5f);
		
		return BlendMat;
	}
	static public Material GetMaterial(string S,Color col,bool UseAlpha){
		if (MixMaterial==null||AddMaterial==null||SubMaterial==null||NASubMaterial==null||NASubMaterial==null){
			AddMaterial = new Material( Shader.Find("Hidden/ShaderSandwich/Alpha Additive") );
			LigMaterial = new Material( Shader.Find("Hidden/ShaderSandwich/Alpha Lighten") );
			DarMaterial = new Material( Shader.Find("Hidden/ShaderSandwich/Alpha Darken") );	
			SubMaterial = new Material( Shader.Find("Hidden/ShaderSandwich/Alpha Subtract") );	
			MulMaterial = new Material( Shader.Find("Hidden/ShaderSandwich/Alpha Multiply") );
			DivMaterial = new Material( Shader.Find("Hidden/ShaderSandwich/Alpha Divide") );
			MixMaterial = new Material( Shader.Find("Hidden/ShaderSandwich/Alpha Standard") );
			
			NAAddMaterial = new Material( Shader.Find("Hidden/ShaderSandwich/Additive") );
			NALigMaterial = new Material( Shader.Find("Hidden/ShaderSandwich/Lighten") );
			NADarMaterial = new Material( Shader.Find("Hidden/ShaderSandwich/Darken") );	
			NASubMaterial = new Material( Shader.Find("Hidden/ShaderSandwich/Subtract") );
			NAMulMaterial = new Material( Shader.Find("Hidden/ShaderSandwich/Multiply") );
			NADivMaterial = new Material( Shader.Find("Hidden/ShaderSandwich/Divide") );
			NAMixMaterial = new Material( Shader.Find("Hidden/ShaderSandwich/Standard") );
		}
		Material ReturnMat = null;
		if (UseAlpha){
			switch (S){
				case "Add":
					ReturnMat = AddMaterial;AddMaterial.SetColor("_Color",col);break;
				case "Mix":
					ReturnMat = MixMaterial;MixMaterial.SetColor("_Color",col);break;
				case "Subtract":
					ReturnMat = SubMaterial;SubMaterial.SetColor("_Color",col);break;
				case "Multiply":
					ReturnMat = MulMaterial;MulMaterial.SetColor("_Color",new Color(col.r,col.g,col.b,1f-col.a));break;
				case "Divide":
					ReturnMat = DivMaterial;DivMaterial.SetColor("_Color",col);break;
				case "Lighten":
					ReturnMat = LigMaterial;LigMaterial.SetColor("_Color",col);break;
				case "Darken":
					ReturnMat = DarMaterial;DarMaterial.SetColor("_Color",col);break;
				case "Normals Mix":
					ReturnMat = LigMaterial;LigMaterial.SetColor("_Color",col);break;
			}
		}
		else{
			switch (S){
				case "Add":
					ReturnMat = NAAddMaterial;NAAddMaterial.SetColor("_Color",col);break;
				case "Mix":
					ReturnMat = NAMixMaterial;NAMixMaterial.SetColor("_Color",col);break;
				case "Subtract":
					ReturnMat = NASubMaterial;NASubMaterial.SetColor("_Color",col);break;
				case "Multiply":
					ReturnMat = NAMulMaterial;NAMulMaterial.SetColor("_Color",new Color(col.r,col.g,col.b,1f-col.a));break;
				case "Divide":
					ReturnMat = NADivMaterial;NADivMaterial.SetColor("_Color",col);break;
				case "Lighten":
					ReturnMat = NALigMaterial;NALigMaterial.SetColor("_Color",col);	break;
				case "Darken":
					ReturnMat = NADarMaterial;NADarMaterial.SetColor("_Color",col);break;
				case "Normals Mix":
					ReturnMat = NALigMaterial;NALigMaterial.SetColor("_Color",col);	break;
			}
		}
		return ReturnMat;
	}
	

	static public void MoveItem<T>(ref List<T> list,int OldIndex,int NewIndex){
		if (NewIndex<list.Count&&NewIndex>=0){
			T item = list[OldIndex];
			list.RemoveAt(OldIndex);
			//if (NewIndex > OldIndex)
			//	NewIndex -= 1;
			
			list.Insert(NewIndex,item);
		}
	}
	static public string Ser<T>(T obj2){
	//Debug.Log(obj2);
		//XmlSerializer xsSubmit = new XmlSerializer(typeof(T));
		
		
		DataContractSerializer serializer = new DataContractSerializer(typeof(T), null, 
		2000, //maxItemsInObjectGraph
		false, //ignoreExtensionDataObject
		true, //preserveObjectReferences : this is where the magic happens 
		null); //dataContractSurrogate
			
		
		
		/*StringWriter sww = new StringWriter();
		XmlWriter writer = new XmlTextWriter(sww);//.Create(sww);
		serializer.WriteObject(writer, obj2);	
		//xsSubmit.Serialize(sww, obj2);
		return sww.GetStringBuilder().ToString();//.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>",""); // Your xml	*/
		using (StringWriter output = new StringWriter())
		using (XmlTextWriter writer = new XmlTextWriter(output) {Formatting = Formatting.Indented})
		{
			serializer.WriteObject(writer, obj2);
			//Debug.Log(output.GetStringBuilder().ToString());
			return output.GetStringBuilder().ToString();
		}		
	}
	static public void SaveString(string fileName,string text){
		StreamWriter  sr = File.CreateText(fileName);
		sr.NewLine = "\n";
        sr.WriteLine (text);
        sr.Close();
	}
	static public Rect AddRect(Rect rect,Rect rect2){
		return new Rect(rect.x+rect2.x,rect.y+rect2.y,rect.width+rect2.width,rect.height+rect2.height);
	}
	static public Rect AddRectVector(Rect rect,Vector2 rect2){
		return new Rect(rect.x+rect2.x,rect.y+rect2.y,rect.width,rect.height);
	}
	static public string SaveDict(Dictionary<string,ShaderVar> D){
		return SaveDict(D,1);
	}
	static public string[] Tabs = new string[]{"","	","		","			","				","					","						","							"};
	static public string SaveDict(Dictionary<string,ShaderVar> D, int tabs){
		string S = "";
		foreach(KeyValuePair<string, ShaderVar> entry in D){
			string OldName = entry.Value.Name;
			entry.Value.Name = entry.Key;
				S += entry.Value.Save(tabs);
			entry.Value.Name = OldName;
		}
		return S;
	}
	static public string SaveList(List<ShaderVar> L,int tabs){
		string S = "";
		foreach(ShaderVar sv in L){
			S += sv.Save(tabs);
		}
		return S;
	}
	static public void LoadLine(Dictionary<string,ShaderVar> D,string Line){
		Line = Sanitize(Line);
		Line = Line.Replace(" (RGBA)(","(");
		string name = Line.Substring(0,Line.IndexOf("("));
		if (D.ContainsKey(name))
		D[name].Load(Line);
	}
	static public void LoadLineLegacy(Dictionary<string,ShaderVar> D,string Line){
		Line = Line.Replace(" (RGBA)(","(");
		string[] parts = Line.Replace("#^CC0","").Replace(" #^ CC0","").Split(new string[] { "#!" },StringSplitOptions.None);
		if (D.ContainsKey(parts[0].Trim()))
		D[parts[0].Trim()].LoadLegacy(parts[1].Trim());
	}
	static public void LoadLine(Dictionary<string,ShaderVar> D,string Line,SVDictionary CD){
		//string[] parts = Line.Replace("#^CC0","").Replace(" #^ CC0","").Split(new string[] { "#!" },StringSplitOptions.None);
//		Debug.Log(parts[0].Trim());
		Line = Sanitize(Line);
		Line = Line.Replace(" (RGBA)(","(");
		string name = Line.Substring(0,Line.IndexOf("("));
		if (D.ContainsKey(name))
		D[name].Load(Line);
		else{
			if (!CD.ContainsKey(name))
				CD[name] = new ShaderVar(name,false);
			CD[name].Load(Line);
		}
	}
	static public void LoadLineLegacy(Dictionary<string,ShaderVar> D,string Line,SVDictionary CD){
		string[] parts = Line.Replace("#^CC0","").Replace(" #^ CC0","").Split(new string[] { "#!" },StringSplitOptions.None);
//		Debug.Log(parts[0].Trim());
		if (D.ContainsKey(parts[0].Trim()))
		D[parts[0].Trim()].LoadLegacy(parts[1].Trim());
		else
		{//if (CD.ContainsKey(parts[0].Trim())){
			CD[parts[0].Trim()] = new ShaderVar(parts[0].Trim(),false);
			CD[parts[0].Trim()].LoadLegacy(parts[1].Trim());
		}
	}
	static public string[] LoadLineExplode(string Line){
		return LoadLineExplode(Line,true);
	}
	static public string[] LoadLineExplode(string Line,bool RemoveCC){
		if (RemoveCC)
		return Line.Replace("#^CC0","").Split(new string[] { "#!" },StringSplitOptions.None);
		else
		return Line.Split(new string[] { "#!" },StringSplitOptions.None);
	}
	static public string GetValueFromExplode(string Line){
		return Line.Split(new string[] { "#^" },StringSplitOptions.None)[2];
	}
	static public string GetValueFromExplodeOld(string Line){
		return Line.Split(new string[] { "#^" },StringSplitOptions.None)[0].Trim();
	}
	static public string Sanitize(string S){
		if (S.IndexOf("#?")>=0)
		S = S.Substring(0,S.IndexOf("#?"));	
		S = S.Replace("	","");
		return S;
	}
	static public GUIStyle Button2Border;
	static public GUIStyle Button20;
	static public GUIStyle ButtonMiddle15;
	static public GUIStyle ButtonLeft;
	static public GUIStyle BoxMiddle15;
	static public GUIStyle BoxMiddle20;
	static public GUIStyle BoxUpperLeftRich;
	static public GUIStyle TextField18;
	static public void ClearGUIStyles(){
		Button2Border = null;
		Button20 = null;
		ButtonMiddle15 = null;
		ButtonLeft = null;
		BoxMiddle15 = null;
		BoxMiddle20 = null;
		BoxUpperLeftRich = null;
		TextField18 = null;
	}
	static public void CheckGUIStyles(){
		if (Button2Border==null){
			Button2Border = new GUIStyle(GUI.skin.button);
			Button2Border.padding = new RectOffset(2,2,2,2);
			Button2Border.margin = new RectOffset(2,2,2,2);
		}
		Button2Border.normal.textColor = GUI.skin.button.normal.textColor;
		Button2Border.active.textColor = GUI.skin.button.active.textColor;
		Button2Border.fontSize = EMindGUI.InterfaceSize;
		if (Button20==null){
			Button20 = new GUIStyle(GUI.skin.button);
			Button20.fontSize = 20;
		}
		Button20.normal.textColor = GUI.skin.button.normal.textColor;
		Button20.active.textColor = GUI.skin.button.active.textColor;
		//Button20.fontSize = EMindGUI.InterfaceSize;
		if (ButtonMiddle15==null){
			if(ShaderSandwich.Instance.CurInterfaceStyle==InterfaceStyle.Flat||ShaderSandwich.Instance.CurInterfaceStyle==InterfaceStyle.Modern){
				ButtonMiddle15 = new GUIStyle(EMindGUI.SCSkin.window);
				ButtonMiddle15.alignment = TextAnchor.MiddleCenter;
			}
			else
				ButtonMiddle15 = new GUIStyle(GUI.skin.button);
			//ButtonMiddle15.fontSize = 15;
			ButtonMiddle15.fontSize = 13;
		}
		ButtonMiddle15.onNormal.textColor = EMindGUI.SCSkin.window.onNormal.textColor;
		//ButtonMiddle15.fontSize = EMindGUI.InterfaceSize;
		//ButtonMiddle15.active.textColor = GUI.skin.button.active.textColor;
		if (BoxMiddle15==null){
			if(ShaderSandwich.Instance.CurInterfaceStyle==InterfaceStyle.Flat||ShaderSandwich.Instance.CurInterfaceStyle==InterfaceStyle.Modern){
				BoxMiddle15 = new GUIStyle(GUI.skin.box);
				BoxMiddle15.alignment = TextAnchor.MiddleCenter;
			}
			else
				BoxMiddle15 = new GUIStyle(GUI.skin.button);
			
			BoxMiddle15.fontSize = 15;
		}
		BoxMiddle15.normal.textColor = GUI.skin.button.normal.textColor;
		BoxMiddle15.active.textColor = GUI.skin.button.active.textColor;
		//BoxMiddle15.fontSize = EMindGUI.InterfaceSize;
		if (BoxMiddle20==null){
			if(ShaderSandwich.Instance.CurInterfaceStyle==InterfaceStyle.Flat||ShaderSandwich.Instance.CurInterfaceStyle==InterfaceStyle.Modern){
				BoxMiddle20 = new GUIStyle(GUI.skin.box);
				BoxMiddle15.alignment = TextAnchor.MiddleCenter;
			}
			else
				BoxMiddle20 = new GUIStyle(GUI.skin.button);
			
			BoxMiddle20.fontSize = 20;
		}
		BoxMiddle20.normal.textColor = GUI.skin.button.normal.textColor;
		BoxMiddle20.active.textColor = GUI.skin.button.active.textColor;
		//BoxMiddle20.fontSize = EMindGUI.InterfaceSize;
		if (TextField18==null){
			TextField18 = new GUIStyle(GUI.skin.textField);
			TextField18.fontSize = 18;
		}
		TextField18.normal.textColor = GUI.skin.button.normal.textColor;
		TextField18.active.textColor = GUI.skin.button.active.textColor;
		//TextField18.fontSize = EMindGUI.InterfaceSize;
		if (BoxUpperLeftRich==null){
			BoxUpperLeftRich = new GUIStyle(GUI.skin.box);
			BoxUpperLeftRich.alignment = TextAnchor.UpperLeft;
			BoxUpperLeftRich.richText = true;
		}
		BoxUpperLeftRich.normal.textColor = GUI.skin.button.normal.textColor;
		BoxUpperLeftRich.active.textColor = GUI.skin.button.active.textColor;
		BoxUpperLeftRich.fontSize = EMindGUI.InterfaceSize;
		if (ButtonLeft==null){
			if (ShaderSandwich.Instance.CurInterfaceStyle==InterfaceStyle.Modern){
				ButtonLeft = new GUIStyle(EMindGUI.SCSkin.window);
				ButtonLeft.alignment = TextAnchor.UpperLeft;
			}
			else{
				ButtonLeft = new GUIStyle(GUI.skin.button);
				ButtonLeft.alignment = TextAnchor.MiddleLeft;
			}
		}
		ButtonLeft.normal.textColor = GUI.skin.button.normal.textColor;
		ButtonLeft.active.textColor = GUI.skin.button.active.textColor;
		ButtonLeft.fontSize = EMindGUI.InterfaceSize;
	}
	static public int DrawEffects(Rect rect,ShaderLayer SL,List<ShaderEffect> LayerEffects,ref int SelectedEffect){
		
		int SEBoxHeight = 60;
		foreach(ShaderEffect SE in LayerEffects){
			if (SEBoxHeight<(SE.Inputs.D.Count*20+20))
			SEBoxHeight = (SE.Inputs.D.Count*20+20);
			if (SE.CustomHeight>=0)
				if (SEBoxHeight<(SE.CustomHeight+20))
					SEBoxHeight = (SE.CustomHeight+20);
		}
		SEBoxHeight+=(LayerEffects.Count*15);
		rect.height = SEBoxHeight;
		EMindGUI.BeginGroup(rect);
		int YOffset = 0;
		if (Event.current.type==EventType.Repaint){
			EMindGUI.SCSkin.window.Draw(new Rect(0,YOffset+20,rect.width,rect.height-20),"",false,true,true,true);
		}
		//GUI.Box(new Rect(0,YOffset,rect.width,20),"Effects");
		YOffset+=19;

		ShaderEffect Delete = null;
		ShaderEffect MoveUp = null;
		ShaderEffect MoveDown = null;
		SelectedEffect = Mathf.Max(0,SelectedEffect);
		int y = -1;
		foreach(ShaderEffect SE in LayerEffects){
			//SE.Activate(false);
			y+=1;
			bool Selected = false;
			if (SelectedEffect<LayerEffects.Count&&LayerEffects[SelectedEffect]==SE)
				Selected = true;
			
			Selected = GUI.Toggle(new Rect(0,YOffset+y*15,rect.width-90,15),Selected,SE.Name,GUI.skin.button);
			if (Selected==true){
				if (SelectedEffect!=y)
					EMindGUI.Defocus();
				SelectedEffect = y;
			}
			///////////////////////////////////////
			bool GUIEN = GUI.enabled;
			GUI.enabled = false;
			//UnityEngine.Debug.Log(SE.TypeS+":"+SE.HandleAlpha.ToString());
			if (SE.HandleAlpha==true&&SL.LayerType.Type!=(int)LayerTypes.Previous)
				GUI.enabled = GUIEN;
			/*if(Event.current.type==EventType.Repaint){
				GUI.Toggle(new Rect(rect.width-90,YOffset+y*15,15,15),SE.UseAlpha.Float==1||SE.UseAlpha.Float==2,(SE.UseAlpha.Float==1) ? ShaderSandwich.AlphaOn: ShaderSandwich.AlphaOff,Button2Border);
				GUI.Button(new Rect(rect.width-90+100,YOffset+y*15,15,15),"");
			}
			else{
				GUI.Toggle(new Rect(rect.width-90+100,YOffset+y*15,15,15),false,"");
				if (GUI.Button(new Rect(rect.width-90,YOffset+y*15,15,15),""))
					SE.UseAlpha.Float+=1;
				if (SE.UseAlpha.Float>2)
					SE.UseAlpha.Float = 0;
			}*/
			///TOoDO:
				GUI.Toggle(new Rect(rect.width-90+100,YOffset+y*15,15,15),SE.UseAlpha.Float==1||SE.UseAlpha.Float==2,(SE.UseAlpha.Float==1) ? ShaderSandwich.AlphaOn: ShaderSandwich.AlphaOff,Button2Border);
				if (GUI.Button(new Rect(rect.width-90,YOffset+y*15,15,15),""))
					SE.UseAlpha.Float+=1;
				if (SE.UseAlpha.Float>2)
					SE.UseAlpha.Float = 0;
			
			GUI.enabled = GUIEN;
			////////////////////////////////////////
			SE.Visible = GUI.Toggle(new Rect(rect.width-75,YOffset+y*15,30,15),SE.Visible,SE.Visible ? ShaderSandwich.EyeOpen: ShaderSandwich.EyeClose,Button2Border);
			if (GUI.Button(new Rect(rect.width-45,YOffset+y*15,15,15),ShaderSandwich.UPArrow,Button2Border))
				MoveUp = SE;
			if (GUI.Button(new Rect(rect.width-30,YOffset+y*15,15,15),ShaderSandwich.DOWNArrow,Button2Border))
				MoveDown = SE;
			if (GUI.Button(new Rect(rect.width-15,YOffset+y*15,15,15),ShaderSandwich.CrossRed,Button2Border))
				Delete = SE;
		}
		if (Delete!=null){
			if (LayerEffects.IndexOf(Delete)<SelectedEffect)
				SelectedEffect-=1;
		
			LayerEffects.Remove(Delete);
		}
		if (MoveUp!=null){
			if (LayerEffects.IndexOf(Delete)<SelectedEffect)
				SelectedEffect-=1;
			
			ShaderUtil.MoveItem(ref LayerEffects,LayerEffects.IndexOf(MoveUp),LayerEffects.IndexOf(MoveUp)-1);
		}
		if (MoveDown!=null){
			if (LayerEffects.IndexOf(Delete)<SelectedEffect)
				SelectedEffect+=1;
			
			ShaderUtil.MoveItem(ref LayerEffects,LayerEffects.IndexOf(MoveDown),LayerEffects.IndexOf(MoveDown)+1);
		}
		y+=1;
		
		Color oldg = GUI.contentColor;
		GUI.contentColor = EMindGUI.BaseInterfaceColorComp;
		//if (GUI.Button(EMindGUI.Rect2(RectDir.Right,rect.width,YOffset-20,20,20),"+",Button2Border)){
		if (GUI.Button(EMindGUI.Rect2(RectDir.Right,rect.width,YOffset-20,18,18),ShaderSandwich.Plus,Button2Border)){
			GenericMenu toolsMenu = new GenericMenu();
			foreach(KeyValuePair<string,ShaderPlugin> Ty2 in ShaderSandwich.ShaderLayerEffectInstances){
				string Ty = Ty2.Key;
				//ShaderEffect SE = ShaderEffect.CreateInstance<ShaderEffect>();
				//SE.ShaderEffectIn(Ty,SL);
				toolsMenu.AddItem(new GUIContent(ShaderSandwich.ToNonDumbShaderLayerEffectInstances[Ty]), false, SL.AddLayerEffect,Ty);
			}
			toolsMenu.DropDown(EMindGUI.Rect2(RectDir.Right,rect.width,YOffset-20,20,20));
			//EditorGUIUtility.ExitGUI();
		}
		GUI.contentColor=  oldg;
		SelectedEffect = Mathf.Max(0,SelectedEffect);
		if (SelectedEffect<LayerEffects.Count&&LayerEffects[SelectedEffect]!=null)
			LayerEffects[SelectedEffect].Draw(new Rect(0,YOffset+y*15,rect.width,SEBoxHeight));
		EMindGUI.EndGroup();
		return (int)rect.height;
	}
	static public void DrawEffects2(Rect rect,ShaderLayer SL,List<ShaderEffect> LayerEffects,ref int SelectedEffect){
		
		int SEBoxHeight = 60;
		foreach(ShaderEffect SE in LayerEffects){
			if (SEBoxHeight<(SE.Inputs.D.Count*20+20))
			SEBoxHeight = (SE.Inputs.D.Count*20+20);
			if (SE.CustomHeight>=0)
				if (SEBoxHeight<(SE.CustomHeight+20))
					SEBoxHeight = (SE.CustomHeight+20);
		}
		SEBoxHeight+=(LayerEffects.Count*15);
		rect.height = SEBoxHeight;
		EMindGUI.BeginGroup(rect);
		int YOffset = 0;
		GUI.Box(new Rect(0,YOffset,rect.width,rect.height),"","button");
		GUI.Box(new Rect(0,YOffset,rect.width,20),"Effects");
		YOffset+=20;

		ShaderEffect Delete = null;
		ShaderEffect MoveUp = null;
		ShaderEffect MoveDown = null;
		SelectedEffect = Mathf.Max(0,SelectedEffect);
		int y = -1;
		foreach(ShaderEffect SE in LayerEffects){
			//SE.Activate(false);
			y+=1;
			bool Selected = false;
			if (SelectedEffect<LayerEffects.Count&&LayerEffects[SelectedEffect]==SE)
				Selected = true;
			
			Selected = GUI.Toggle(new Rect(0,YOffset+y*15,rect.width-90,15),Selected,SE.Name,GUI.skin.button);
			if (Selected==true){
				if (SelectedEffect!=y)
					EMindGUI.Defocus();
				SelectedEffect = y;
			}
			///////////////////////////////////////
			bool GUIEN = GUI.enabled;
			GUI.enabled = false;
			//UnityEngine.Debug.Log(SE.TypeS+":"+SE.HandleAlpha.ToString());
			if (SE.HandleAlpha==true&&SL.LayerType.Type!=(int)LayerTypes.Previous)
				GUI.enabled = GUIEN;
			/*if(Event.current.type==EventType.Repaint){
				GUI.Toggle(new Rect(rect.width-90,YOffset+y*15,15,15),SE.UseAlpha.Float==1||SE.UseAlpha.Float==2,(SE.UseAlpha.Float==1) ? ShaderSandwich.AlphaOn: ShaderSandwich.AlphaOff,Button2Border);
				GUI.Button(new Rect(rect.width-90+100,YOffset+y*15,15,15),"");
			}
			else{
				GUI.Toggle(new Rect(rect.width-90+100,YOffset+y*15,15,15),false,"");
				if (GUI.Button(new Rect(rect.width-90,YOffset+y*15,15,15),""))
					SE.UseAlpha.Float+=1;
				if (SE.UseAlpha.Float>2)
					SE.UseAlpha.Float = 0;
			}*/
			///TOoDO:
				GUI.Toggle(new Rect(rect.width-90+100,YOffset+y*15,15,15),SE.UseAlpha.Float==1||SE.UseAlpha.Float==2,(SE.UseAlpha.Float==1) ? ShaderSandwich.AlphaOn: ShaderSandwich.AlphaOff,Button2Border);
				if (GUI.Button(new Rect(rect.width-90,YOffset+y*15,15,15),""))
					SE.UseAlpha.Float+=1;
				if (SE.UseAlpha.Float>2)
					SE.UseAlpha.Float = 0;
			
			GUI.enabled = GUIEN;
			////////////////////////////////////////
			SE.Visible = GUI.Toggle(new Rect(rect.width-75,YOffset+y*15,30,15),SE.Visible,SE.Visible ? ShaderSandwich.EyeOpen: ShaderSandwich.EyeClose,Button2Border);
			if (GUI.Button(new Rect(rect.width-45,YOffset+y*15,15,15),ShaderSandwich.UPArrow,Button2Border))
				MoveUp = SE;
			if (GUI.Button(new Rect(rect.width-30,YOffset+y*15,15,15),ShaderSandwich.DOWNArrow,Button2Border))
				MoveDown = SE;
			if (GUI.Button(new Rect(rect.width-15,YOffset+y*15,15,15),ShaderSandwich.CrossRed,Button2Border))
				Delete = SE;
		}
		if (Delete!=null){
			if (LayerEffects.IndexOf(Delete)<SelectedEffect)
				SelectedEffect-=1;
		
			LayerEffects.Remove(Delete);
		}
		if (MoveUp!=null){
			if (LayerEffects.IndexOf(Delete)<SelectedEffect)
				SelectedEffect-=1;
			
			ShaderUtil.MoveItem(ref LayerEffects,LayerEffects.IndexOf(MoveUp),LayerEffects.IndexOf(MoveUp)-1);
		}
		if (MoveDown!=null){
			if (LayerEffects.IndexOf(Delete)<SelectedEffect)
				SelectedEffect+=1;
			
			ShaderUtil.MoveItem(ref LayerEffects,LayerEffects.IndexOf(MoveDown),LayerEffects.IndexOf(MoveDown)+1);
		}
		y+=1;
		if (GUI.Button(EMindGUI.Rect2(RectDir.Right,rect.width,YOffset-20,20,20),ShaderSandwich.Plus,Button2Border)){
			GenericMenu toolsMenu = new GenericMenu();
			foreach(KeyValuePair<string,ShaderPlugin> Ty2 in ShaderSandwich.ShaderLayerEffectInstances){
				string Ty = Ty2.Key;
				//ShaderEffect SE = ShaderEffect.CreateInstance<ShaderEffect>();
				//SE.ShaderEffectIn(Ty,SL);
				toolsMenu.AddItem(new GUIContent(ShaderSandwich.ToNonDumbShaderLayerEffectInstances[Ty]), false, SL.AddLayerEffect,Ty);
			}
			toolsMenu.DropDown(EMindGUI.Rect2(RectDir.Right,rect.width,YOffset-20,20,20));
			//EditorGUIUtility.ExitGUI();
		}
		SelectedEffect = Mathf.Max(0,SelectedEffect);
		if (SelectedEffect<LayerEffects.Count&&LayerEffects[SelectedEffect]!=null)
			LayerEffects[SelectedEffect].Draw(new Rect(0,YOffset+y*15,rect.width,SEBoxHeight));
		EMindGUI.EndGroup();
	}
	public static string CodeName(string NewName){
		NewName = NewName.Replace(" ","_");
		//NewName = Regex.Replace(NewName, "[^a-zA-Z0-9_]","");
		char[] arr = NewName.Where(c => (char.IsLetterOrDigit(c) || 
									 c == '_')).ToArray(); 

		NewName = new string(arr);
		//2017 What is the point of these?
		//Answer - OpenGL compatability...oops XD
		NewName = NewName.Replace("__","_GL");
		NewName = NewName.Replace("__","_GL");
		///NewName = NewName.Replace("__","_a");
		///NewName = NewName.Replace("__","_a");
		return NewName;
	}
	public static string Pad(int Amount){
		return Pad(Amount,"	");
	}
	public static string Pad(int Amount,string Pad){
		string Tot = "";
		for (int i = 0;i<Amount;i++)
		Tot+=Pad;
		return Tot;
	}
	public static string RegexKeywordMatch(string[] Keywords){
		return RegexKeywordMatch(Keywords,false,false);
	}
	public static string RegexKeywordMatch(string[] Keywords,bool CaseSensitive){
		return RegexKeywordMatch(Keywords,CaseSensitive,false);
	}
	public static string RegexKeywordMatch(string[] Keywords,bool CaseSensitive,bool CheckComments){
		string Reg = "";
		string Start = "";
		if (CheckComments)
		Start+="(?<!\\/.*?)";
		//Start+="(?<!\\/\\/(.{0,1}))";
		//Start+="(?<!\\/.*?)";
		if (!CaseSensitive)
		Start+="(?i)";
		
		Reg = "("+Start+Keywords[0];
		foreach(string k in Keywords){
			Reg+="|"+Start+k;
		}
		Reg+=")";
		
		return Reg;
	}
	public static string RandomString(string[] Keywords){
		return Keywords[(int)Mathf.Round(UnityEngine.Random.Range(0f,Keywords.Length-1))];
	}


	public static void SetupCustomData(SVDictionary CustomData,SVDictionary Inputs){
		if (CustomData!=null){
			foreach(KeyValuePair<string,ShaderVar> Input in Inputs){
				if (Input.Value!=null){
					if (!CustomData.ContainsKey(Input.Key)){
						CustomData[Input.Key] = Input.Value.Copy();
					}
					CustomData[Input.Key].TakeMeta(Input.Value);
				}
			}
		}
		//else{
		//	Debug.Log("Custom Data is NULL aahhh!!!");
		//}
		//Debug.Log("Setting up Custom Data");
	}
	public static void SetupOldData(SVDictionary CustomData,SVDictionary Inputs){
		if (CustomData!=null){
			foreach(KeyValuePair<string,ShaderVar> Input in Inputs){
				if (Input.Value!=null){
					if (!CustomData.ContainsKey(Input.Key)){
						CustomData[Input.Key] = Input.Value.Copy();
					}
					CustomData[Input.Key].TakeValues(Input.Value);
				}
			}
		}
	}
	static public bool MoreOrLess(float A1, float A2, float D){
		if ((A1<A2+D)&&(A1>A2-D))
		return true;
		
		return false;
	}
	static public bool MoreOrLess(int A1, int A2, int D){
		if ((A1<A2+D)&&(A1>A2-D))
		return true;
		
		return false;
	}
	static public string LinearToGamma(string blah){
		if (ShaderSandwich.Instance.OpenShader.MiscColorSpace.Type == 2)
			return "LinearToGamma("+blah+")";
		return blah;
	}
	static public string GammaToLinear(string blah){
			return "GammaToLinear("+blah+")";
		//return blah;
	}
	static public void DrawCoolSlider(Rect MixAmountPos, ShaderVar MixAmount,int FloatField, bool Selected){
		Color oldCol = GUI.color;
		Color oldColb = GUI.backgroundColor;
			float MixAmountHeight = MixAmountPos.height;
			//Rect MixAmountPos = new Rect(dragArea.x+dragArea.width,153+BaseSettingsHeight+y+15+5+4,WinSize-dragArea.width,MixAmountHeight);
			//EditorGUIUtility.AddCursorRect(new Rect(MixAmountPos.x+MixAmountHeight,MixAmountPos.y-10,MixAmountPos.width-MixAmountHeight,MixAmountPos.height+15),MouseCursor.SlideArrow);
			MixAmount.NoBox = true;
			//MixAmountPos.width-=38;
			if (ShaderSandwich.Instance.ShowMixAmountValues)
				MixAmountPos.width-=30;
			EditorGUIUtility.AddCursorRect(new Rect(MixAmountPos.x+MixAmountHeight,MixAmountPos.y,MixAmountPos.width-MixAmountHeight,MixAmountPos.height),MouseCursor.SlideArrow);
			
			float newflaot = MixAmount.Float;
			EditorGUI.BeginChangeCheck();
			if (ShaderSandwich.Instance.ShowMixAmountValues){
				if (FloatField==0)
					newflaot = EditorGUI.FloatField( new Rect(MixAmountPos.x+MixAmountPos.width,MixAmountPos.y,30,16),newflaot,EMindGUI.EditorFloatInput);
				else if (FloatField==1)
					newflaot = EditorGUI.FloatField( new Rect(MixAmountPos.x+MixAmountPos.width,MixAmountPos.y-16+MixAmountPos.height,30,16),newflaot,EMindGUI.EditorFloatInput);
			}
			if (EditorGUI.EndChangeCheck()){
				MixAmount.Float = newflaot;
			}
			if (Event.current.type == EventType.Repaint){
				Color bgc = GUI.backgroundColor;
				GUI.backgroundColor*=0.75f;
				GUI.skin.GetStyle("Button").Draw(MixAmountPos,"",false,true,false,false);
				GUI.skin.label.wordWrap = false;
				GUI.backgroundColor = bgc;
				if (Selected){//||(MixAmountPos.Contains(Event.current.mousePosition)&&!SP.dragSurface)){
					GUI.backgroundColor = new Color(1,1,1,1);
					GUI.color = EMindGUI.BaseInterfaceColor*EMindGUI.BaseInterfaceColor;
				}
				else{
					//GUI.color = GUI.color*0.5f;
					GUI.backgroundColor *= 1.5f;
				}

				
					///if (MixAmount.Float>0.01f)
						///EMindGUI.SCSkin.button.Draw(new Rect(MixAmountPos.x+MixAmountHeight,MixAmountPos.y,(MixAmountPos.width-MixAmountHeight)*MixAmount.Float,MixAmountHeight),"",false,true,false,false);
					if (MixAmount.Float>MixAmount.Range0+0.005f)
						EMindGUI.SCSkin.button.Draw(new Rect(MixAmountPos.x+MixAmountHeight-1,MixAmountPos.y,(MixAmountPos.width-MixAmountHeight+2)*((MixAmount.Float-MixAmount.Range0)/(MixAmount.Range1-MixAmount.Range0)),MixAmountHeight),"",false,true,false,false);

				//GUI.color = new Color(1,1,1,1);
				//GUI.Label(new Rect(80,153+BaseSettingsHeight+y+15,250,MixAmountHeight),MixType.Names[MixType.Type]+" Amount");
				//GUI.color = new Color(0,0,0,1);
				//GUI.Label(new Rect(80,153+BaseSettingsHeight+y+15,(250-MixAmountHeight)*surface.MixAmount.Float-59,MixAmountHeight),MixType.Names[MixType.Type]+" Amount");

				//GUI.skin.label.alignment = TextAnchor.UpperLeft;	
				//GUI.skin.label.wordWrap = true;	
			}
			

			MixAmount.DrawGear(new Rect(MixAmountPos.x-150+30,MixAmountPos.y,20,MixAmountHeight));
			GUI.color = new Color(1,1,1,0);
			
			if (ShaderSandwich.Instance.ShowMixAmountValues)
				MixAmount.Draw(new Rect(MixAmountPos.x+( -3+MixAmountHeight),MixAmountPos.y-2,(MixAmountPos.width+28+MixAmountHeight*2),MixAmountHeight),"");
			else
				MixAmount.Draw(new Rect(MixAmountPos.x+( -3+MixAmountHeight),MixAmountPos.y-1,(MixAmountPos.width+26+MixAmountHeight*2),MixAmountHeight),"");
			GUI.backgroundColor = oldColb;
			MixAmount.LastUsedRect.x-=130;
			MixAmount.LastUsedRect.y+=10;
			GUI.color = oldCol;
			MixAmount.DrawGear(new Rect(MixAmountPos.x-150+30,MixAmountPos.y,20,MixAmountHeight));
	}
	static public Dictionary<string,ShaderVar> ShaderVarListToDictionary(List<ShaderVar> svlist){
		Dictionary<string,ShaderVar> D = new Dictionary<string,ShaderVar>();
			foreach(ShaderVar sv in svlist){
				D[sv.Name] = sv;
			}
		return D;
	}
	static public string GenerateMixingLerp(string Start,string End,string PremulEnd, string Alpha, string OneMinusAlpha, bool Premul){
		if (Premul){
			return "(("+Start+") * " + OneMinusAlpha + " + ("+PremulEnd+"))";
			//return "(("+Start+") * (1 - (" + Alpha + ")) + ("+PremulEnd+"))";
		}else{
			return "lerp("+Start+","+End+","+Alpha+")";
		}
	}
	//static public string GeneratePremulCorrection(string Output, string Start,string End,string PremulEnd, string Alpha, bool Premul){
	//	return Output + " = " + GenerateMixingLerp(Start,End,PremulEnd,Alpha,Premul)+";\n";
	//}
	static public string GenerateMixingLerp(string Start,string End, string Alpha, string OneMinusAlpha, bool Premul){
		return GenerateMixingLerp(Start,End,End,Alpha,OneMinusAlpha,Premul);
	}
	
	static public string GenerateMixingCodeRGBA(string Dest, string Src, string BlendedSrcRGBA, string BlendedSrcRGB, string AlphaMode, string Alpha, string OneMinusAlpha, bool IsPremul, bool CreatesPremul, string EndTag){
		string Code = "";
		
		//if (!IsPremul && CreatesPremul){//NOOOOOPE
		//	Code += Src + ".rgb *= " + Alpha + ";//Premultiply the layer by its alpha (including masks etc)\n";
		//	IsPremul = true;
		if (IsPremul){
			BlendedSrcRGBA = BlendedSrcRGBA.Replace(Dest,Alpha+" * "+Dest);
			BlendedSrcRGB = BlendedSrcRGB.Replace(Dest,Alpha+" * "+Dest);
		}
		//}
		string AlphaNoA = Alpha.Replace( Src + ".a * ", "").Replace( Src + ".a", "");
		string AlphaA = (Alpha.Contains(Src))?" * "+Src+".a":"";
		
		string OneMinusAlphaNoA = OneMinusAlpha.Replace( Src + ".a * ", "").Replace( Src + ".a", "");
		
		if (EndTag=="rgba")
			EndTag = "";
		else
			EndTag = "."+EndTag;
		
		if (AlphaMode == "Blend"){
			if (Alpha == "")
				Code += Dest + " = half4(" + BlendedSrcRGB + ",1) " + EndTag + ";//0\n";
			else{
				//Debug.Log(Alpha);
				//if (IsPremul&&AlphaA != "")
				if (IsPremul)
					Code += Dest + " = " + GenerateMixingLerp(Dest, "half4(" + BlendedSrcRGB + ", " + Alpha + ")", Alpha, OneMinusAlpha, IsPremul) + EndTag + ";//1\n";
				else
					Code += Dest + " = " + GenerateMixingLerp(Dest, "half4(" + BlendedSrcRGB + ", 1)", Alpha, OneMinusAlpha, IsPremul) + EndTag + ";//2\n";
				//else
				//	Code += Dest + " = " + GenerateMixingLerp(Dest, "float4(" + BlendedSrcRGB + ",1)", Alpha, IsPremul) + ";\n";
			}
		}
		if (AlphaMode == "Blend Inside"){
			if (Alpha == ""){
				if (CreatesPremul)
					Code += Dest + ".rgb = " + BlendedSrcRGB + " * " + Dest + ".a;//3\n";
				else
					Code += Dest + ".rgb = " + BlendedSrcRGB + ";//4\n";
			}
			else{
				//Code += Dest + ".rgb = " + GenerateMixingLerp(Dest, BlendedSrcRGB + " * " + Src + ".a", Alpha.Replace( Src + ".a * ", "").Replace( Src, ""), IsPremul) + ";\n";
				if (CreatesPremul)
					Code += Dest + ".rgb = " + GenerateMixingLerp(Dest + ".rgb", BlendedSrcRGB + " * " + Dest + ".a", Alpha,"(1 - ("+Alpha+"))", IsPremul) + ";//5\n";
					//Code += Dest + ".rgb = " + GenerateMixingLerp(Dest + ".rgb", BlendedSrcRGB + " * " + Dest + ".a", Alpha + " * " + Dest + ".a","(1 - ("+Alpha + " * " + Dest + ".a))", IsPremul) + ";//5\n";
				else
					Code += Dest + ".rgb = " + GenerateMixingLerp(Dest + ".rgb", BlendedSrcRGB, Alpha, OneMinusAlpha, IsPremul) + ";//6\n";
			}
		}
		if (AlphaMode == "Replace"){
			if (Alpha == ""){
				Code += Dest + " = half4(" + BlendedSrcRGB + ",1)" + EndTag + ";//7\n";
			}
			else if (AlphaA == ""){
				if (CreatesPremul)
					Code += Dest + " = " + GenerateMixingLerp(Dest, "half4(" + BlendedSrcRGB + ", 1)", AlphaNoA, OneMinusAlphaNoA, IsPremul) + EndTag + ";//8\n";
				else
					Code += Dest + " = " + GenerateMixingLerp(Dest, "half4(" + BlendedSrcRGB + ", 1)", AlphaNoA, OneMinusAlphaNoA, IsPremul) + EndTag + ";//9\n";
			}
			else if (AlphaNoA == ""){
				if (IsPremul||!CreatesPremul)
					Code += Dest + " = " + BlendedSrcRGBA + EndTag + ";//10\n";
				else
					Code += Dest + " = half4(" + BlendedSrcRGB + " * " + Src + ".a, " + Src + ".a)" + EndTag + ";//11\n";
			}
			else{
				if (IsPremul||!CreatesPremul)
					Code += Dest + " = " + GenerateMixingLerp(Dest, BlendedSrcRGBA, AlphaNoA, OneMinusAlphaNoA, IsPremul) + EndTag + ";//13\n";
				else
					Code += Dest + " = " + GenerateMixingLerp(Dest, "half4("+BlendedSrcRGB + " * " + Src + ".a, " + Src + ".a)", AlphaNoA, OneMinusAlphaNoA, IsPremul) + EndTag + ";//12\n";
			}
		}
		if (AlphaMode == "Erase"){
			if (Alpha == ""){
				if (CreatesPremul)
					Code += Dest + " = 0;//14\n";
				else
					Code += Dest + ".a = 0;//15\n";
			}
			else{
				if (CreatesPremul)
					Code += Dest + " *= " + OneMinusAlpha + ";//16\n";
					//Code += Dest + " *= 1 - " + Alpha + ";//16\n";
				else
					Code += Dest + ".a *= " + OneMinusAlpha + ";//17\n";
					//Code += Dest + ".a *= 1 - " + Alpha + ";//17\n";
			}
		}
		return Code;
	}
	static public string GenerateMixingCodeSingle(string Dest, string Src, string BlendedSrc, string AlphaMode, string Alpha, string OneMinusAlpha, bool IsPremul, bool CreatesPremul, string EndTag){
		string Code = "";
		//string BlendedSrcRGB = "Yoo dumb";
		//string BlendedSrcRGBA = "Yoo dumber";
		
		EndTag = "."+EndTag;
		
		if (AlphaMode == "Blend"){
			if (Alpha == "")
				Code += Dest + " = " + BlendedSrc + ";//0\n";
			else{
				Code += Dest + " = " + GenerateMixingLerp(Dest, BlendedSrc, Alpha, OneMinusAlpha, IsPremul) + ";//1\n";
			}
		}
		if (AlphaMode == "Replace"){
			string AlphaNoA = Alpha.Replace( Src + ".a * ", "").Replace( Src + ".a", "");
			
			if (Alpha == ""||AlphaNoA.Length==0){
				Code += Dest + " = " + BlendedSrc + ";//2\n";
			}else{
				string OneMinusAlphaNoA = OneMinusAlpha.Replace( Src + ".a * ", "").Replace( Src + ".a", "");
				Code += Dest + " = " + GenerateMixingLerp(Dest, BlendedSrc, AlphaNoA, OneMinusAlphaNoA, IsPremul) + ";//3\n";
			}
		}
		return Code;
	}
	static public string GenerateMixingCode(ShaderData SD, string Dest, string Src, string BlendMode, string AlphaMode, string Alpha, bool IsPremul, bool CreatesPremul, string EndTag){
		return GenerateMixingCode(SD, Dest, Src, BlendMode, AlphaMode, Alpha, "(1 - ("+Alpha+"))", IsPremul, CreatesPremul, EndTag);
	}
	static public string GenerateMixingCode(ShaderData SD, string Dest, string Src, string BlendMode, string AlphaMode, string Alpha, string OneMinusAlpha, bool IsPremul, bool CreatesPremul, string EndTag){
		string Code = "";
		//ShaderData SD, string Dest, string Src, string BlendedSrcRGBA, string BlendedSrcRGB, string AlphaMode, string Alpha, bool IsPremul, bool CreatesPremul, string EndTag
		if (EndTag.Length==4){
			if(BlendMode=="Mix")
					Code += GenerateMixingCodeRGBA(Dest,Src,
					Src,
					Src+".rgb",
					AlphaMode,Alpha,OneMinusAlpha,IsPremul,CreatesPremul,EndTag);
					
			if(BlendMode=="Add"){//Debug.Log(SD.UsesPremultipliedBlending);
				if (AlphaMode=="Blend"&&SD.UsesPremultipliedBlending){
					AlphaMode = "Blend Inside";
					Code += GenerateMixingCodeRGBA(Dest,Src,
					"half4("+Dest+".rgb + "+Src+".rgb,"+Src+".a)",
					"(" + Dest+".rgb + "+Src+".rgb)",
					AlphaMode,Alpha,OneMinusAlpha,IsPremul,false,EndTag);
				}
				else if (AlphaMode=="Replace"&&SD.UsesPremultipliedBlending){
					AlphaMode = "Blend Inside";
					Code += GenerateMixingCodeRGBA(Dest,Src,
					"half4("+Dest+".rgb + "+Src+".rgb,"+Src+".a)",
					"(" + Dest+".rgb + "+Src+".rgb)",
					AlphaMode,Alpha,OneMinusAlpha,IsPremul,false,EndTag);
				}
				else
					Code += GenerateMixingCodeRGBA(Dest,Src,
					"half4("+Dest+".rgb + "+Src+".rgb,"+Src+".a)",
					"(" + Dest+".rgb + "+Src+".rgb)",
					AlphaMode,Alpha,OneMinusAlpha,IsPremul,CreatesPremul,EndTag);
			}
			if(BlendMode=="Subtract")
				Code += GenerateMixingCodeRGBA(Dest,Src,
				"half4("+Dest+".rgb - "+Src+".rgb,"+Src+".a)",
				"(" + Dest+".rgb - "+Src+".rgb)",
				AlphaMode,Alpha,OneMinusAlpha,IsPremul,CreatesPremul,EndTag);
			if(BlendMode=="Multiply")
				Code += GenerateMixingCodeRGBA(Dest,Src,
				"half4("+Dest+".rgb * "+Src+".rgb,"+Src+".a)",
				"(" + Dest+".rgb * "+Src+".rgb)",
				AlphaMode,Alpha,OneMinusAlpha,IsPremul,false,EndTag);
			if(BlendMode=="Divide")
				Code += GenerateMixingCodeRGBA(Dest,Src,
				"half4("+Dest+".rgb / "+Src+".rgb,"+Src+".a)",
				"(" + Dest+".rgb / "+Src+".rgb)",
				AlphaMode,Alpha,OneMinusAlpha,IsPremul,false,EndTag);
			if(BlendMode=="Lighten")
				Code += GenerateMixingCodeRGBA(Dest,Src,
				"half4(max("+Dest+".rgb,"+Src+".rgb),"+Src+".a)",
				"max("+Dest+".rgb,"+Src+".rgb)",
				AlphaMode,Alpha,OneMinusAlpha,IsPremul,CreatesPremul,EndTag);
			if(BlendMode=="Darken")
				Code += GenerateMixingCodeRGBA(Dest,Src,
				"half4(min("+Dest+".rgb,"+Src+".rgb),"+Src+".a)",
				"min("+Dest+".rgb,"+Src+".rgb)",
				AlphaMode,Alpha,OneMinusAlpha,IsPremul,CreatesPremul,EndTag);
			if(BlendMode=="Normals Mix")
				Code += GenerateMixingCodeRGBA(Dest,Src,
				"half4(normalize(float3("+Dest+".xy+"+Src+".xy,"+Src+".z)),"+Src+".a)",
				"normalize(float3("+Dest+".xy+"+Src+".xy,"+Src+".z))",
				AlphaMode,Alpha,OneMinusAlpha,IsPremul,CreatesPremul,EndTag);
			if(BlendMode=="Dot")
				Code += GenerateMixingCodeRGBA(Dest,Src,
				"half4(dot("+Dest+".rgb,"+Src+".rgb).rrr,"+Src+".a)",
				"dot("+Dest+".rgb,"+Src+".rgb).rrr",
				AlphaMode,Alpha,OneMinusAlpha,IsPremul,CreatesPremul,EndTag);
		}
		if (EndTag.Length==1||EndTag.Length==3){
			string EndTagEnd = "."+EndTag;
			
			if (EndTag=="z")
				EndTagEnd = "";
			
			if(BlendMode=="Mix"){
				if (EndTag=="a"&&AlphaMode=="Blend"){
					EndTagEnd = "";
					Src = "1";
				}
				Code += GenerateMixingCodeSingle(Dest,Src,
				Src+EndTagEnd,
				AlphaMode,Alpha,OneMinusAlpha,IsPremul,CreatesPremul,EndTag);
			}
					
			if(BlendMode=="Add")
				Code += GenerateMixingCodeSingle(Dest,Src,
				"(" + Dest+" + "+Src+EndTagEnd+")",
				AlphaMode,Alpha,OneMinusAlpha,IsPremul,CreatesPremul,EndTag);
				
			if(BlendMode=="Subtract")
				Code += GenerateMixingCodeSingle(Dest,Src,
				"(" + Dest+" - "+Src+EndTagEnd+")",
				AlphaMode,Alpha,OneMinusAlpha,IsPremul,CreatesPremul,EndTag);
				
			if(BlendMode=="Multiply")
				Code += GenerateMixingCodeSingle(Dest,Src,
				"(" + Dest+" * "+Src+EndTagEnd+")",
				AlphaMode,Alpha,OneMinusAlpha,IsPremul,false,EndTag);
				
			if(BlendMode=="Divide")
				Code += GenerateMixingCodeSingle(Dest,Src,
				"(" + Dest+" / "+Src+EndTagEnd+")",
				AlphaMode,Alpha,OneMinusAlpha,IsPremul,false,EndTag);
				
			if(BlendMode=="Lighten")
				Code += GenerateMixingCodeSingle(Dest,Src,
				"max(" + Dest+", "+Src+EndTagEnd+")",
				AlphaMode,Alpha,OneMinusAlpha,IsPremul,CreatesPremul,EndTag);
				
			if(BlendMode=="Darken")
				Code += GenerateMixingCodeSingle(Dest,Src,
				"min(" + Dest+", "+Src+EndTagEnd+")",
				AlphaMode,Alpha,OneMinusAlpha,IsPremul,CreatesPremul,EndTag);
				
			if(BlendMode=="Normals Mix")
				Code += GenerateMixingCodeSingle(Dest,Src,
				"half4(normalize(half3("+Dest+".xy+"+Src+".xy,"+Src+".z)),"+Src+".a)"+EndTagEnd,
				AlphaMode,Alpha,OneMinusAlpha,IsPremul,CreatesPremul,EndTag);
				
			if(BlendMode=="Dot")
				Code += GenerateMixingCodeSingle(Dest,Src,
				"dot("+Dest+".xxx,"+Src+".rgb)",
				AlphaMode,Alpha,OneMinusAlpha,IsPremul,CreatesPremul,EndTag);
		}
				
		return Code;
	}
	
	public static string ReplaceTempVariables(ShaderData SD,string str){
		if (SD.Temp){
			str = str.Replace("_Time.","_SSTime.");
			str = str.Replace("_Time,","_SSTime,");
			str = str.Replace("_Time ","_SSTime ");
			str = str.Replace("_Time)","_SSTime)");
			str = str.Replace("_CosTime.","_SSCosTime.");
			str = str.Replace("_CosTime,","_SSCosTime,");
			str = str.Replace("_CosTime ","_SSTime ");
			str = str.Replace("_CosTime)","_SSTime)");
			str = str.Replace("_SinTime.","_SSSinTime.");
			str = str.Replace("_SinTime,","_SSSinTime,");
			str = str.Replace("_SinTime ","_SSSinTime ");
			str = str.Replace("_SinTime)","_SSSinTime)");
		}
		return str;
	}
	/*static public string GenerateMixingCode(ShaderData SD, string Src, string BlendedSrcRGBA, string BlendedSrcRGB, string BlendedSrcA, string Dest,  string DestAlt, string AlphaMode, string BlendFac, bool Premul, bool IsPremul,bool CreatesPremul){
		string Code = "";
		if (BlendFac=="")
			BlendFac = "1";
		
		if (AlphaMode=="Replace"){
			if (!IsPremul&&CreatesPremul){
				if (BlendedSrcA[BlendedSrcA.Length-1]=='a')
					Code += Src + " *= float4("+BlendedSrcA+"aa,1);\n";
				else
					Code += Src + " *= float4("+BlendedSrcA+".rrr,1);\n";
			}
			Code += Dest + " = " + GenerateMixingLerp(DestAlt,BlendedSrcRGBA,BlendedSrcRGBA,BlendFac,false) + ";\n";
		}
		string FinishPremul = "";
		string MultiplyBlendFac = BlendFac + " * ";
		if (IsPremul){
			FinishPremul = Src + " = " + BlendedSrcRGBA.Replace(DestAlt+".rgb","("+DestAlt+".rgb * "+Src+".a)") +" * " + BlendFac + ";\n";
			BlendedSrcRGBA = Src;
			BlendedSrcRGB = Src+".rgb";
			BlendedSrcA = Src+".a";
			MultiplyBlendFac = "";
		}
		if (AlphaMode=="Blend"){
			Code += FinishPremul + Dest + " = " + GenerateMixingLerp(DestAlt,"float4("+BlendedSrcRGB+",1)",BlendedSrcRGBA,MultiplyBlendFac + BlendedSrcA,IsPremul) + ";\n";
		}
		if (AlphaMode=="Erase"){
			if (CreatesPremul)
				Code += FinishPremul + Dest + " = " + GenerateMixingLerp(DestAlt,"float4(0,0,0,0)","float4(0,0,0,0)",MultiplyBlendFac + BlendedSrcA,IsPremul) + ";\n";
			else
				Code += FinishPremul + Dest + " = " + GenerateMixingLerp(DestAlt,"float4("+DestAlt+".rgb,0)","float4("+DestAlt+".rgb,0)",MultiplyBlendFac + BlendedSrcA,IsPremul) + ";\n";
		}
		if (AlphaMode=="Keep"){//FIXED: Might need alteration for premultipled mode, check later
			if (CreatesPremul){
				Code += Src + " *= float4("+Dest+".aaa,1);\n";
			}
			Code += FinishPremul + Dest + ".rgb = " + GenerateMixingLerp(DestAlt+".rgb",BlendedSrcRGB,BlendedSrcRGB,MultiplyBlendFac + BlendedSrcA,IsPremul) + ";\n";
		}
		
		return Code;
	}
	static public string GenerateMixingCode(ShaderData SD, string Dest,string Src, string BlendMode, string AlphaMode, string BlendFac, bool Premul, bool IsPremul, bool CreatesPremul){/*That last ones weird
		string Code = "";
		string DestAlt = Dest;
		if (AlphaMode=="Don't Change")AlphaMode = "Keep";
		if (BlendFac!="0"){
			if (!Premul&&IsPremul){
				//Code += Dest + ".rgb *= " + BlendFac + ";\n";
				Code += Dest + ".rgb *= " + Dest + ".a;\n";
				//DestAlt = Dest+"*"+Dest+".a";
			}
			if(BlendMode=="Mix")
				Code += GenerateMixingCode(SD,Src,
				Src,
				Src+".rgb",
				Src+".a",Dest,DestAlt,AlphaMode,BlendFac,Premul,IsPremul,CreatesPremul);
			if(BlendMode=="Add"){
				//if (AlphaMode=="Blend"&&SD.UsesPremultipliedBlending)
				//	AlphaMode = "BlendKeepAlpha";
				if (AlphaMode=="Blend"&&SD.UsesPremultipliedBlending){
					AlphaMode = "Keep";
					Code += GenerateMixingCode(SD,Src,
					"float4("+Dest+".rgb + "+Src+".rgb,"+Src+".a)",
					//"float4("+Dest+".rgb + "+Src+".rgb,"+Src+".a)",
					"(" + Dest+".rgb + "+Src+".rgb)",
					Src+".a",
					Dest,DestAlt,AlphaMode,BlendFac,Premul,IsPremul,false);
				}
				else
					Code += GenerateMixingCode(SD,Src,
					"float4("+Dest+".rgb + "+Src+".rgb,"+Src+".a)",
					//"float4("+Dest+".rgb + "+Src+".rgb,"+Src+".a)",
					"(" + Dest+".rgb + "+Src+".rgb)",
					Src+".a",
					Dest,DestAlt,AlphaMode,BlendFac,Premul,IsPremul,CreatesPremul);
			}
			if(BlendMode=="Subtract")
				Code += GenerateMixingCode(SD,Src,
				"float4("+Dest+".rgb - "+Src+".rgb,"+Src+".a)",
				"(" + Dest+".rgb - "+Src+".rgb)",
				Src+".a",
				Dest,DestAlt,AlphaMode,BlendFac,Premul,IsPremul,CreatesPremul);
			if(BlendMode=="Multiply")
				Code += GenerateMixingCode(SD,Src,
				"float4("+Dest+".rgb * "+Src+".rgb,"+Src+".a)",
				"(" + Dest+".rgb * "+Src+".rgb)",
				Src+".a",
				Dest,DestAlt,AlphaMode,BlendFac,Premul,IsPremul,false);
			if(BlendMode=="Divide")
				Code += GenerateMixingCode(SD,Src,
				"float4("+Dest+".rgb / "+Src+".rgb,"+Src+".a)",
				"(" + Dest+".rgb / "+Src+".rgb)",
				Src+".a",
				Dest,DestAlt,AlphaMode,BlendFac,Premul,IsPremul,false);
			if(BlendMode=="Lighten")
				Code += GenerateMixingCode(SD,Src,
				"float4(max("+Dest+".rgb,"+Src+".rgb),"+Src+".a)",
				"max("+Dest+".rgb,"+Src+".rgb)",
				Src+".a",
				Dest,DestAlt,AlphaMode,BlendFac,Premul,IsPremul,CreatesPremul);
			if(BlendMode=="Darken")
				Code += GenerateMixingCode(SD,Src,
				"float4(min("+Dest+".rgb,"+Src+".rgb),"+Src+".a)",
				"min("+Dest+".rgb,"+Src+".rgb)",
				Src+".a",
				Dest,DestAlt,AlphaMode,BlendFac,Premul,IsPremul,CreatesPremul);
		}
		return Code;
	}*/
}
}
/*	static public string GenerateMixingCode(ShaderData SD, string Src, string BlendedSrcRGBA, string BlendedSrcRGB, string BlendedSrcA, string Dest, string AlphaMode, string BlendFac, bool Premul, bool IsPremul){
		string Code = "";
		//Debug.Log("BlendFac+:"+BlendFac);
		if (BlendFac=="")
			BlendFac = "1";
		/*if (AlphaMode=="Replace"&&SD.UsesPremultipliedBlending)
			Code += PixelColor + ".rgb *= " + PixelColor + ".a;\n";
		if (AlphaMode=="Keep"&&SD.UsesPremultipliedBlending)
			Code += PixelColor + ".rgb *= " + CodeName + ".a;\n";
		if (AlphaMode=="Erase"&&SD.UsesPremultipliedBlending)
			Code += PixelColor + ".rgb = 0;\n";
		else if (AlphaMode=="Erase"&&!SD.UsesPremultipliedBlending)
			Code += PixelColor + ".rgb = " + CodeName + ".rgb;\n";*
		
		if (AlphaMode=="Replace"&&SD.UsesPremultipliedBlending&&!IsPremul){
			BlendedSrcRGB = BlendedSrcRGB+" * "+BlendedSrcA;
			BlendedSrcRGBA = BlendedSrcRGBA+" * float4("+BlendedSrcA+".rrr,1)";
		}
		if (AlphaMode=="Keep"&&SD.UsesPremultipliedBlending&&!IsPremul){
			BlendedSrcRGB = BlendedSrcRGB+" * "+Dest+".a";
			BlendedSrcRGBA = BlendedSrcRGBA+" * float4("+Dest+".rrr,1)";
		}
		if (AlphaMode=="Erase"&&SD.UsesPremultipliedBlending&&!IsPremul){
			BlendedSrcRGB = "float3(0,0,0)";
			BlendedSrcRGBA = "float4(0,0,0,"+BlendedSrcA+")";
		}
		else if (AlphaMode=="Erase"&&!SD.UsesPremultipliedBlending&&!IsPremul){
			BlendedSrcRGB = Dest+".rgb";
			BlendedSrcRGBA = Dest;
		}
		
		string unpremulBlendFac = BlendFac;
		if (AlphaMode=="Blend"||AlphaMode=="BlendKeepAlpha"||AlphaMode=="Keep"||AlphaMode=="Erase"){
			if (BlendFac=="1")
				BlendFac = BlendedSrcA;
			else
				BlendFac += " * " + BlendedSrcA;
		}
		if (Premul&&!(AlphaMode=="Replace")){
			BlendFac = Src + ".a";
			Code += Src + " = " + BlendedSrcRGBA + " * " + unpremulBlendFac + ";\n";
			BlendedSrcRGBA = Src;
			BlendedSrcRGB = Src + ".rgb";
			BlendedSrcA = Src + ".a";
		}
		
		if (AlphaMode=="Keep"){
			if (BlendFac=="1")
				Code += Dest + ".rgb = " + BlendedSrcRGB + ";\n";
			else
				Code += Dest + ".rgb = " + GenerateMixingLerp(Dest+".rgb",BlendedSrcRGB,BlendedSrcRGB,BlendFac,Premul) + ";\n";
				//Code += Dest + ".rgb = lerp(" + Dest + ".rgb, " + BlendedSrcRGB + ", " + BlendFac +");\n";
		}
		else if (AlphaMode=="Replace"){
			if (BlendFac=="1")
				Code += Dest + " = " + BlendedSrcRGBA + ";\n";
			else
				Code += Dest + " = " + GenerateMixingLerp(Dest,BlendedSrcRGBA,BlendFac,false) + ";\n";
				//Code += Dest + " = lerp(" + Dest + ", " + BlendedSrcRGBA + ", " + BlendFac + ");\n";
		}
		else if (AlphaMode=="Blend")
			Code += Dest + " = "+GenerateMixingLerp(Dest,"float4(" + BlendedSrcRGB + ",1)",BlendedSrcRGBA,BlendFac,Premul)+";\n";
		else if (AlphaMode=="BlendKeepAlpha")
			Code += Dest + " = "+GenerateMixingLerp(Dest,"float4(" + BlendedSrcRGB + ","+Dest+".a)",BlendFac,Premul)+";\n";
		else if (AlphaMode=="Erase")
			Code += Dest + " = "+GenerateMixingLerp(Dest,"float4(" + BlendedSrcRGB + ",0)",BlendFac,Premul)+";\n";
		else Code += Dest + " = " + BlendedSrcRGBA + ";\n";
		
		return Code;
	}*/
/*				if (BlendFac=="1")
					BlendFac = Src + ".a";
				else
					BlendFac += " * " + Src + ".a";
				
				if (AlphaMode=="Keep"){
					if (BlendFac=="1")
						Code += Dest + ".rgb = " + Src + ".rgb;\n";
					else
						Code += Dest + ".rgb = lerp(" + Dest + ".rgb, " + Src + ".rgb, " + BlendFac +");\n";
				}
				if (AlphaMode=="Replace"){
					if (BlendFac=="1")
						Code += Dest + " = " + Src + ";\n";
					else
						Code += Dest + " = lerp(" + Dest + ", " + Src + ", " + BlendFac + ");\n";
				}
				if (AlphaMode=="Blend")
					Code += Dest + " = "+GenerateMixingLerp(Dest,"float4(" + Src + ".rgb,1)",BlendFac,Premul)+";\n";
				if (AlphaMode=="Erase")
					Code += Dest + " = "+GenerateMixingLerp(Dest,"float4(" + Src + ".rgb,0)",BlendFac,Premul)+";\n";*/