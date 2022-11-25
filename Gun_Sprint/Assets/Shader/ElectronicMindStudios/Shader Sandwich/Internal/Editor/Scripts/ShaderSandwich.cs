#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define PRE_UNITY_5
#else
#define UNITY_5
#endif
#pragma warning disable 0618
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
using UEObject = UnityEngine.Object;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Reflection;
using System.Threading;

using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
public enum InterfaceStyle{Old,Flat,Modern};
public enum PreviewSystem{CPU,GPU};
[System.Serializable]
public class ShaderSandwich : EditorWindow{//, ISerializationCallbackReceiver {
	[NonSerialized]public bool DEBUGMODE = false;
	public string SerializeProtection = "";
	//public void OnBeforeSerialize(){
		//Ser();
		//asd
	//}
	[NonSerialized]public int DelayedSerLoad = 0;//
	public void Ser(){
		//Debug.Log("PlaymodeStateChanged"+EditorApplication.isPlaying.ToString()+EditorApplication.isPlayingOrWillChangePlaymode.ToString()+EditorApplication.isPaused.ToString());
		if (OpenShader!=null&&(!EditorApplication.isPlaying&&EditorApplication.isPlayingOrWillChangePlaymode)){//Transitioning to Play Mode
			SerializeProtection = "\n\n/*\n"+OpenShader.Save()+"\n*/"	;
			DelayedSerLoad = 1;
			//Debug.Log("GO serialize!"+SerializeProtection);
		}
		if (EditorApplication.isPlaying&&EditorApplication.isPlayingOrWillChangePlaymode&&!EditorApplication.isPaused){//In Play Mode
			//Debug.Log("In Play Mode");
			DelayedSerLoad = 2;
		}
		
		if (OpenShader!=null&&(EditorApplication.isPlaying&&!EditorApplication.isPlayingOrWillChangePlaymode)){//Transitioning out of Play Mode
			SerializeProtection = "\n\n/*\n"+OpenShader.Save()+"\n*/";
			DelayedSerLoad = 1;
			//Debug.Log("GO serialize!"+SerializeProtection);
		}
		if (!EditorApplication.isPlaying&&!EditorApplication.isPlayingOrWillChangePlaymode){//Out of Play Mode
			//Debug.Log("Out Of Play Mode");
			DelayedSerLoad = 2;
			//LoadFromSerializedProtectNonsense();
			//SerializeProtection = "";
			//ForceHookUpdate = true;
		}
		
	}
	public void OnEnable(){
		//Debug.Log("Enabled"+EditorApplication.isPlayingOrWillChangePlaymode.ToString());
		EditorApplication.playmodeStateChanged += Ser;
		InitialUndoGroup = UnityEditor.Undo.GetCurrentGroup();
	}
	/*public void Start(){
		Debug.Log("Started"+EditorApplication.isPlayingOrWillChangePlaymode.ToString());
	}
	public void OnDestroy(){
		Debug.Log("Destroyed"+EditorApplication.isPlayingOrWillChangePlaymode.ToString());
	}
	public void OnDisable(){
		Debug.Log("Disabled"+EditorApplication.isPlayingOrWillChangePlaymode.ToString());
	}*/
	public void OnDestroy(){
		Instance = null;
		DragAreaTex = null;
		DragAreaTex2 = null;
		UPArrow = null;
		DOWNArrow = null;
		LeftArrow = null;
		RightArrow = null;
		LeftTriangle = null;
		RightTriangle = null;
		QuestionMarkOff = null;
		QuestionMarkOn = null;
		LightBulbOff = null;
		LightBulbOn = null;
		Warning = null;
		Plus = null;
		ReloadIcon = null;
		Tick = null;
		Cross = null;
		CrossRed = null;
		Logo = null;
		Banner = null;
		BannerLight = null;
		Literal = null;
		PerlNoise = null;
		BlockNoise = null;
		CubistNoise = null;
		CellNoise = null;
		DotNoise = null;
		GrabPass = null;
		ThisThingo = null;
		ThisDepthThing = null;
		DepthPass = null;
		KitchenCube = null;
		BlackCube = null;
		DayCube = null;
		SunsetCube = null;
		IconShell = null;
		IconBase = null;
		IconMask = null;
		EditableOn = null;
		EditableOff = null;
		EyeClose = null;
		EyeOpen = null;
		AlphaOn = null;
		AlphaOff = null;
		Gear = null;
		GearLinked = null;
		ChainLinked = null;
		ChainNotLinked = null;
		SCSkin = null;
		SSSkin = null;
		
		//EffectsList = null;
		//ShaderSurfaces = null;
		//ShaderGeometryModifiers = null;
		//NonDumbShaderSurfaces = null;
		//NonDumbShaderGeometryModifiers = null;
		//ShaderLayerTypeInstances = null;
		//PluginList = null;
		
		if (ShaderPreview.Instance!=null)
			ShaderPreview.Instance.Close();
		ShaderPreview.Instance = null;
		
		//if (InitialUndoGroup!=-100)
		//	Undo.CollapseUndoOperations(InitialUndoGroup);
		foreach (ShaderLayerList go in Resources.FindObjectsOfTypeAll(typeof(ShaderLayerList)) as ShaderLayerList[]){Undo.ClearUndo(go);UnityEngine.Object.DestroyImmediate(go,true);}
		foreach (ShaderLayer go in Resources.FindObjectsOfTypeAll(typeof(ShaderLayer)) as ShaderLayer[]){Undo.ClearUndo(go);UnityEngine.Object.DestroyImmediate(go,true);}
		foreach (ShaderPass go in Resources.FindObjectsOfTypeAll(typeof(ShaderPass)) as ShaderPass[]){Undo.ClearUndo(go);UnityEngine.Object.DestroyImmediate(go,true);}
		foreach (ShaderBase go in Resources.FindObjectsOfTypeAll(typeof(ShaderBase)) as ShaderBase[]){Undo.ClearUndo(go);UnityEngine.Object.DestroyImmediate(go,true);}
		foreach (ShaderInput go in Resources.FindObjectsOfTypeAll(typeof(ShaderInput)) as ShaderInput[]){Undo.ClearUndo(go);UnityEngine.Object.DestroyImmediate(go,true);}
		foreach (ShaderIngredient go in Resources.FindObjectsOfTypeAll(typeof(ShaderIngredient)) as ShaderIngredient[]){Undo.ClearUndo(go);UnityEngine.Object.DestroyImmediate(go,true);}
		foreach (ShaderSurface go in Resources.FindObjectsOfTypeAll(typeof(ShaderSurface)) as ShaderSurface[]){Undo.ClearUndo(go);UnityEngine.Object.DestroyImmediate(go,true);}
		foreach (ShaderGeometryModifier go in Resources.FindObjectsOfTypeAll(typeof(ShaderGeometryModifier)) as ShaderGeometryModifier[]){Undo.ClearUndo(go);UnityEngine.Object.DestroyImmediate(go,true);}
		foreach (ShaderEffect go in Resources.FindObjectsOfTypeAll(typeof(ShaderEffect)) as ShaderEffect[]){Undo.ClearUndo(go);UnityEngine.Object.DestroyImmediate(go,true);}
		foreach (ShaderRandomStuffForSerialization go in Resources.FindObjectsOfTypeAll(typeof(ShaderRandomStuffForSerialization)) as ShaderRandomStuffForSerialization[]){Undo.ClearUndo(go);UnityEngine.Object.DestroyImmediate(go,true);}
		Undo.FlushUndoRecordObjects ();
		
		EMindGUI.EndProSkin();
	}
	public int InitialUndoGroup = -100;
	public int DeserializedLoadThing = -1000;
	public bool PreviousPlayState = false;
	/*public void OnAfterDeserialize(){
//		Debug.Log("It's ok.");
		if (PreviousPlayState!=EditorApplication.isPlaying){
			DeserializedLoadThing = 2;
			PreviousPlayState = EditorApplication.isPlaying;
		}
	}*/
	public void LoadFromSerializedProtectNonsense(){
		//Debug.Log("Plez work ;("+((SerializeProtection!="nope")?"yay!":"Oh ;(")+((EditorApplication.isPlaying)?"yay!":"Oh ;("));
		//Debug.Log(SerializeProtection);
		if (SerializeProtection!=""){
			StringReader theReader = new StringReader(SerializeProtection);
			string line = "";
			string FileString = "";
		//using (theReader)
		//{
			bool inParse = false;
			bool didParse = false;
			bool LegacyLoad = false;
			while(true==true){
			line = theReader.ReadLine();
				//Debug.Log(line);
				if (line != null){
					line = ShaderUtil.Sanitize(line);
					line = line.Trim();
					if (line.Contains("BeginShaderParse")){
						inParse = true;
						didParse = true;
						LegacyLoad = true;
					}
					if (line.Contains("Shader Sandwich Shader")){
						inParse = true;
						didParse = true;
					}
					if (LegacyLoad){
						if (line.IndexOf("#?")>=0)
							line = line.Substring(0,line.IndexOf("#?"));
					}
					if (inParse==true&&(line!=""))
						FileString+=line+"\n";
					
					if (LegacyLoad&&line.Contains("EndShaderParse"))
						inParse = false;
					if (line.Contains("End Shader Sandwich Shader"))
						inParse = false;
				}
				else{
					break;
				}
			}
			
			theReader.Close();
			if (didParse){
				OpenShader = ShaderBase.Load(new StringReader(FileString),false);
				RegenShaderPreview();
				GUI.changed = true;
				ChangeSaveTemp(null);
			}else{
				EditorUtility.DisplayDialog("Oh no!","Sorry, something really weird happened when transfering into Playmode :(","Ok");
			}
			
			if (OpenShader.ShaderPasses.Count>0&&OpenShader.ShaderPasses[0].Surfaces.Count>0&&OpenShader.ShaderPasses[0].Surfaces[0].GetMyShaderLayerLists().Count>0&&OpenShader.ShaderPasses[0].Surfaces[0].GetMyShaderLayerLists()[0].SLs.Count>0)
				SRS.LayerSelection = OpenShader.ShaderPasses[0].Surfaces[0].GetMyShaderLayerLists()[0].SLs[0];
		}
		else{
			//Debug.Log("What is life?");
		}
		ChangeSaveTemp(null);
	}
	static public bool Unity4{
		get{
			#if PRE_UNITY_5
				return true;
			#else
				return false;
			#endif
		}
		set{}
	}
	
	public ShaderBase OpenShader;
	public enum GUIType{Start,Presets,Layers,Configure,Inputs,Preview};
	public GUIType GUIStage = GUIType.Start;
	public int ShaderVarIds = 0;
	
	static public ShaderTransition GUITrans;
	static public ShaderTransition StartNewTrans;
	static public ShaderVar ShaderVarEditing;
	public GUIType GUITransition = GUIType.Start;
	
    public int selGridInt = 0;
	public bool SeeShells = false;
    public string[] selStrings = new string[] {"Diffuse", "Normal Mapped", "Specular", "Normal Mapped Specular"};
	//public ShaderSandwich windowG;
	//Start Specific
	public Vector2 StartScroll;
	public Vector2 ConfigScroll;
	public Vector2 LayerScroll;
	public Vector2 LayerListScroll;
	public Vector2 InputScroll;
	
	
	public Vector2 ShellsScroll;
	
	public ShaderRandomStuffForSerialization SRS;
	public string CurrentFilePath = "";
	public int GUIChangedTimer = 5;
	public string OldShaderGenerate = "";
	public static ShaderSandwichSettings SSSettings = null;
	public string StatusReal = "No Shader Loaded";

	public string Status{
		get{return StatusReal;}
		set{
		
		StatusReal = value;
		//Debug.Log(StatusReal);
		//Repaint();
		//Event.current.type=EventType.Layout;
		//DrawMenu();
		//Event.current.type=EventType.Repaint;
		//DrawMenu();
		
		}
	}
	public void SetStatus(string Title, string Stat,float perc){
		Status = Stat;
		//EditorUtility.DisplayProgressBar(Title, Stat, perc);
		//if (perc==1f)
		//EditorUtility.ClearProgressBar();
	}
	static public Texture2D DragAreaTex;
	static public Texture2D DragAreaTex2;
	static public Texture2D UPArrow;
	static public Texture2D DOWNArrow;
	static public Texture2D LeftArrow;
	static public Texture2D RightArrow;
	static public Texture2D LeftTriangle;
	static public Texture2D RightTriangle;
	static public Texture2D QuestionMarkOff;
	static public Texture2D QuestionMarkOn;
	static public Texture2D LightBulbOff;
	static public Texture2D LightBulbOn;
	static public Texture2D Warning;
	static public Texture2D Plus;
	static public Texture2D ReloadIcon;
	static public Texture2D Tick;
	static public Texture2D Cross;
	static public Texture2D Bin;
	static public Texture2D Bin2;
	static public Texture2D Bin3;
	static public Texture2D StarOn;
	static public Texture2D StarOff;
	static public Texture2D CrossRed;
	static public Texture2D Logo;
	static public Texture2D Banner;
	static public Texture2D BannerLight;
	static public Texture2D Literal;
	static public Texture2D PerlNoise;
	static public Texture2D ImageBasedPerlinNoiseTemp;
	static public Texture2D ImageBasedPerlinNoiseTempLinear;
	static public Texture2D BlockNoise;
	static public Texture2D CubistNoise;
	static public Texture2D CellNoise;
	static public Texture2D DotNoise;
	static public Texture2D GrabPass;
	static public Texture2D ThisThingo;
	static public Texture2D ThisDepthThing;
	static public Texture2D DepthPass;
	static public Cubemap KitchenCube;
	static public Cubemap BlackCube;
	static public Cubemap DayCube;
	static public Cubemap SunsetCube;
	static public Texture2D IconShell;
	static public Texture2D IconBase;
	static public Texture2D IconMask;
	static public Texture2D EditableOn;
	static public Texture2D EditableOff;
	static public Texture2D EyeClose;
	static public Texture2D EyeOpen;
	static public Texture2D AlphaOn;
	static public Texture2D AlphaOff;
	static public Texture2D Gear;
	static public Texture2D GearLinked;
	static public Texture2D ChainLinked;
	static public Texture2D ChainNotLinked;
	static public Texture2D WindowEdge;
	static public Texture2D Happy;
	static public Texture2D MiniDownArrow;
	static public GUISkin SCSkin;
	static public GUISkin SSSkin;
	
	public bool RealtimePreviewUpdates = true;
	public bool AnimateInputs = true;
	public bool ViewLayerNames = true;
	public bool ImprovedUpdates = true;
	
	public InterfaceStyle CurInterfaceStyle = InterfaceStyle.Modern;
	public PreviewSystem PreviewSystem = PreviewSystem.GPU;
	public bool UseOldInputs = false;
	public bool ShowMixAmountValues = false;
	//public Color InterfaceColor = new Color(109f/255f,215f/255f,1,1);
	//public Color InterfaceColor = new Color(0f,97f/255f,0.5f,1);
	public Color InterfaceColor = new Color(0f,39f/255f,51f/255f,1);
	public int InterfaceSize = 10;
	public bool BlendLayers = false;
	
	public ShaderLayer ClipboardLayer = null;
	public ShaderIngredient ClipboardIngredient = null;
	public List<ShaderEffect> ClipboardEffects = null;
	
	//static public Dictionary<string,Delegate> ShaderLayerTypePreviewList = new Dictionary<string,Delegate>();
	//static public Dictionary<string,Delegate> ShaderLayerTypeGetDimensionsList = new Dictionary<string,Delegate>();
	//static public Dictionary<string,ShaderLayerType> ShaderLayerTypeInstances = new Dictionary<string,ShaderLayerType>();
	static public Dictionary<string,Type> ShaderSurfaces = new Dictionary<string,Type>();
	static public Dictionary<string,Type> ShaderGeometryModifiers = new Dictionary<string,Type>();
	static public Dictionary<string,Type> NonDumbShaderSurfaces = new Dictionary<string,Type>();
	static public Dictionary<string,Type> NonDumbShaderGeometryModifiers = new Dictionary<string,Type>();
	static public Dictionary<string,ShaderPlugin> ShaderLayerTypeInstances = new Dictionary<string,ShaderPlugin>();
	static public Dictionary<string,ShaderPlugin> ShaderLayerEffectInstances = new Dictionary<string,ShaderPlugin>();
	static public Dictionary<string,Type> ShaderLayerEffect = new Dictionary<string,Type>();
	static public Dictionary<string,ShaderPlugin> NonDumbShaderLayerTypeInstances = new Dictionary<string,ShaderPlugin>();
	static public Dictionary<string,ShaderPlugin> NonDumbShaderLayerEffectInstances = new Dictionary<string,ShaderPlugin>();
	static public Dictionary<string,string> ToNonDumbShaderLayerEffectInstances = new Dictionary<string,string>();
	static public Dictionary<string,Dictionary<string,ShaderPlugin>> PluginList = new Dictionary<string,Dictionary<string,ShaderPlugin>>();
	
	public delegate void ShaderVarObjectFieldUpdate(List<object> objects, List<string> storageNames, List<Texture> icons, List<bool> enabled);
	//static public ShaderVarObjectFieldUpdate MaskObjectsUpdate;
	[NonSerialized]public bool DereBeARefreshHere = true;
	[NonSerialized]public bool ForceHookUpdate = true;
	//public bool ForceHookUpdate{
	//	get{return _ForceHookUpdate;}
	//	set{Debug.Log(value.ToString());_ForceHookUpdate = value;}
	//}
	public void JustHooked(){
		ForceHookUpdate = true;
		ForceHookUpdate = true;
	}
	//public void ForceUpdateHooks(){
		//UpdateMaskShaderVars();
		//UpdateMaskShaderVars();
	//	UpdateLayerTypeShaderVars();
	//}
	public void CreateHook(ShaderVar SV){
		List<object> objects;
		List<Texture> icons;
		List<bool> enabled;
		List<string> storageNames;
		ShaderSandwich.ShaderVarObjectFieldUpdates[SV.ObjectUpdater] -= SV.UpdateObjects;
		ShaderSandwich.ShaderVarObjectFieldUpdates[SV.ObjectUpdater] += SV.UpdateObjects;
		//JustHooked();
		if (SV.ObjectUpdater=="Mask"){
			UpdateMaskShaderVarsRaw(out objects,out storageNames,out icons,out enabled);
			if (objects!=null)
				SV.UpdateObjects(objects,storageNames,icons,enabled);
			else{
				if (SV.ObjFieldObject==null)
					SV.ObjFieldObject = new List<object>();
				//Debug.Log("Hook failed");
				ForceHookUpdate = true;
			}
		}
		if (SV.ObjectUpdater=="LayerType"){
			UpdateLayerTypeShaderVarsRaw(out objects,out storageNames,out icons,out enabled);
			if (objects!=null)
				SV.UpdateObjects(objects,storageNames,icons,enabled);
			else{
				if (SV.ObjFieldObject==null)
					SV.ObjFieldObject = new List<object>();
				//Debug.Log("Hook failed");
				ForceHookUpdate = true;
			}
		}
	}
	public void UpdateMaskShaderVarsRaw(out List<object> objects,
		out List<string> storageNames,
		out List<Texture> icons,
		out List<bool> enabled){
			
		objects = new List<object>();
		icons = new List<Texture>();
		enabled = new List<bool>();
		storageNames = new List<string>();
		if (OpenShader!=null){
			foreach(ShaderLayerList SLL2 in OpenShader.ShaderLayersMasks){
				objects.Add(SLL2);
				//Debug.Log(SLL2.GetIcon());
				icons.Add(SLL2.GetIcon());
				enabled.Add(true);//!(!Enab||((SLL2.EndTag.Text.Length!=4&&RGBAMasks))||(!LightingMasks&&SLL2.IsLighting.On)||(LightingMasks&&!SLL2.IsLighting.On)));
				storageNames.Add(ShaderUtil.CodeName(SLL2.Name.Text));
			}
		}
	}
	public void UpdateMaskShaderVars(){
		List<object> objects;
		List<Texture> icons;
		List<bool> enabled;
		List<string> storageNames;
		UpdateMaskShaderVarsRaw(out objects,out storageNames,out icons,out enabled);
		if (ShaderSandwich.PluginList.ContainsKey("Mask")&&OpenShader!=null){
			if (ShaderVarObjectFieldUpdates["Mask"]!=null)
				ShaderVarObjectFieldUpdates["Mask"](objects,storageNames,icons,enabled);
			//else
				//Debug.Log("Not a single mask using shader variable...probably a bug");
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
	Shader _DefaultUVsShader;
	Material _DefaultUVsMat;
	public void UpdateLayerTypeShaderVarsRaw(out List<object> objects,
		out List<string> storageNames,
		out List<Texture> icons,
		out List<bool> enabled){
		//if (ShaderSandwich.Instance!=null&&ShaderSandwich.Instance.OpenShader!=null&&ShaderSandwich.PluginList.Contains("LayerType")){
		if (ShaderSandwich.PluginList.ContainsKey("LayerType")){
			if (ShaderSandwich.Instance.PreviewSystem == PreviewSystem.GPU){
				if (_DefaultUVsShader==null)
					_DefaultUVsShader = Shader.Find("Hidden/ShaderSandwich/Previews/DefaultUVs");
					
				if (_DefaultUVsShader!=null){
					if (_DefaultUVsMat==null)
						_DefaultUVsMat = new Material(_DefaultUVsShader);
				}
				
				Shader.SetGlobalTexture("_UVs",RTTempUV);
				Graphics.Blit(null,RTTempUV,_DefaultUVsMat);
			}
			
			objects = new List<object> ( new object[ShaderSandwich.PluginList["LayerType"].Count] );
			icons = new List<Texture> ( new Texture[ShaderSandwich.PluginList["LayerType"].Count] );;
			enabled = new List<bool> ( new bool[ShaderSandwich.PluginList["LayerType"].Count] );
			storageNames = new List<string> ( new string[ShaderSandwich.PluginList["LayerType"].Count] );
			var PVals = ShaderSandwich.PluginList["LayerType"].Values.ToArray();
			for(int i = 0;i<ShaderSandwich.PluginList["LayerType"].Count;i++){
				if (PVals[i].MenuIndex>-1){
					objects[PVals[i].MenuIndex] = PVals[i];
					icons[PVals[i].MenuIndex] = PVals[i].GetPreviewIcon(PVals[i].GetInputs(),null,null);
					enabled[PVals[i].MenuIndex] = true;
					storageNames[PVals[i].MenuIndex] = PVals[i].TypeS;
				}
			}
			int placement = 0;
			for(int i = 0;i<ShaderSandwich.PluginList["LayerType"].Count;i++){
				if (PVals[i].MenuIndex<=-1){
					while(objects[placement]!=null){placement++;}
					objects[placement] = PVals[i];
					icons[placement] = PVals[i].GetPreviewIcon(PVals[i].GetInputs(),null,null);
					enabled[placement] = true;
					storageNames[PVals[i].MenuIndex] = PVals[i].TypeS;
				}
			}
		}else{
			objects = null;
			icons = null;
			enabled = null;
			storageNames = null;
		}
	}
	public void UpdateLayerTypeShaderVars(){
		//if (ShaderSandwich.Instance!=null&&ShaderSandwich.Instance.OpenShader!=null&&ShaderSandwich.PluginList.Contains("LayerType")){
		if (ShaderSandwich.PluginList.ContainsKey("LayerType")){
			List<object> objects;
			List<Texture> icons;
			List<bool> enabled;
			List<string> storageNames;
			
			UpdateLayerTypeShaderVarsRaw(out objects,out storageNames,out icons,out enabled);
			if (objects!=null){
				if (ShaderVarObjectFieldUpdates["LayerType"]!=null)
					ShaderVarObjectFieldUpdates["LayerType"](objects,storageNames,icons,enabled);
			}else{
				ForceHookUpdate = true;
			}
			//else
				//Debug.Log("Not a single mask using shader variable...probably a bug");
		}else{
			ForceHookUpdate = true;
		}
	}
	
	static public Dictionary<string,ShaderVarObjectFieldUpdate> ShaderVarObjectFieldUpdates = new Dictionary<string,ShaderVarObjectFieldUpdate>();
	
	static public void LoadPlugins(){
		//Surfaces
		ShaderSurfaces.Clear();
		Assembly targetAssembly = Assembly.GetAssembly(typeof(ShaderSurface));
		foreach(Type Ty in targetAssembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ShaderSurface)))){
			//ShaderSurfaces[((ShaderSurface)ScriptableObject.CreateInstance(Ty)).MenuItem] = Ty;
			ShaderSurfaces[(string)(Ty.GetField("MenuItem").GetValue(null))] = Ty;
			NonDumbShaderSurfaces[Ty.Name] = Ty;
		}
		//Geometry Modifiers
		ShaderGeometryModifiers.Clear();
		targetAssembly = Assembly.GetAssembly(typeof(ShaderGeometryModifier));
		foreach(Type Ty in targetAssembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ShaderGeometryModifier)))){
			//ShaderGeometryModifiers[((ShaderGeometryModifier)ScriptableObject.CreateInstance(Ty)).MenuItem] = Ty;
			ShaderGeometryModifiers[(string)(Ty.GetField("MenuItem").GetValue(null))] = Ty;
			NonDumbShaderGeometryModifiers[Ty.Name] = Ty;
		}
		
		//Effects
		targetAssembly = Assembly.GetAssembly(typeof(ShaderPlugin));
		foreach(Type Ty in targetAssembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ShaderEffect)))){
			ShaderPlugin Ins = (ShaderPlugin)ScriptableObject.CreateInstance(Ty);
			Ins.Activate();
			if (Ins.TypeS!="Hasn't Activated Correctly"){
				ShaderLayerEffect[Ins.TypeS] = Ty;
				ShaderLayerEffectInstances[Ins.TypeS] = Ins;
				NonDumbShaderLayerEffectInstances[Ins.Name] = Ins;
				ToNonDumbShaderLayerEffectInstances[Ins.TypeS] = Ins.Name;
			}
			else{
				Debug.LogWarning("The plugin "+Ty.Name+" is not activated correctly, please check whether it has been overriden.");
			}
		}
		PluginList["LayerEffect"] = ShaderLayerEffectInstances;
		string str = "Loading "+ShaderLayerEffectInstances.Count.ToString()+" Layer Effects:\n";
		
		foreach(KeyValuePair<string,ShaderPlugin> kv in ShaderLayerEffectInstances){
			str+=kv.Key+"\n";
		}
		
		//Layer Types
		targetAssembly = Assembly.GetAssembly(typeof(ShaderPlugin));
		foreach(Type Ty in targetAssembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ShaderLayerType)))){
			ShaderPlugin Ins = (ShaderPlugin)ScriptableObject.CreateInstance(Ty);
			Ins.Activate();
			if (Ins.TypeS!="Hasn't Activated Correctly"){
				ShaderLayerTypeInstances[Ins.TypeS] = Ins;
				NonDumbShaderLayerTypeInstances[Ins.Name] = Ins;
			}
			else{
				Debug.LogWarning("The plugin "+Ty.Name+" is not activated correctly, please check whether it has been overriden.");
			}
		}
		PluginList["LayerType"] = ShaderLayerTypeInstances;
		str += "Loading "+ShaderLayerTypeInstances.Count.ToString()+" Layer Types:\n";
		
		foreach(KeyValuePair<string,ShaderPlugin> kv in ShaderLayerTypeInstances){
			str+=kv.Key+"\n";
		}
		//Debug.Log(str);
	}
	public string MOTD = "Loading message of the day...";
	public static bool ValueChanged = false;
	public int changedTimer = 100;
	public static ShaderSandwich Instance;// {
	//get { return (ShaderSandwich)GetWindow(typeof (ShaderSandwich),false,"",false); }
	//}
	WWW www;
	double wwwStart;
	void getAnswerFromOutside(string url) {
		wwwStart = EditorApplication.timeSinceStartup;
		www = new WWW(url);
		/*while (1==1)
		{
			if (www.isDone==true||(EditorApplication.timeSinceStartup-Start>5))
			break;
		}
		if (www.isDone==true)
		return www.text;
		
		return "Unable to connect to www.Electronic-Mind.org for the message of the day/year, sorry!";*/
	}
	
	[MenuItem ("Window/Shader Sandwich")]
	public static void Init () {
		
		ShaderSandwich windowG = (ShaderSandwich)EditorWindow.GetWindow (typeof (ShaderSandwich));
		Instance = windowG;
		Instance.InterfaceColor = new Color(
			EditorPrefs.GetFloat("ElectronicMindStudiosInterfaceColor_R", 0f),
			EditorPrefs.GetFloat("ElectronicMindStudiosInterfaceColor_G", 39f/255f),
			EditorPrefs.GetFloat("ElectronicMindStudiosInterfaceColor_B", 51f/255f),
			EditorPrefs.GetFloat("ElectronicMindStudiosInterfaceColor_A", 1f)
		);
		Instance.InterfaceSize = EditorPrefs.GetInt("ElectronicMindStudiosInterfaceSize", 10);
		LoadPlugins();
		LoadAssets();
		windowG.wantsMouseMove = true;
		//windowG.minSize = new Vector2(950,460);//460
		//windowG.minSize = new Vector2(800,450);//460
		windowG.minSize = new Vector2(600,350);//460
		//Rect tempPos = windowG.position;
		//Debug.Log(tempPos);
		//tempPos.width = 950;
		//tempPos.height = 460;
		windowG.position = new Rect(200,200,950,460);
		windowG.title = "Sandwich! :D"; 
		
		//LoadAssets();
		windowG.getAnswerFromOutside("http://electronic-mind.org/ShaderSandwich/MOTD.txt");
		windowG.MOTD = "Loading message of the day...";//
		string tempStr = "";
		bool commentedOut = false;
		foreach (char c in windowG.MOTD){
			if (c.ToString()=="~")
			commentedOut=true;
			
			if (commentedOut==false)
			tempStr+=c;
			
			if (c.ToString()=="~")
			commentedOut=false;			
		}
		windowG.MOTD = tempStr;
		
		EditorApplication.playmodeStateChanged += Instance.Ser;
		//Debug.Log((Texture2D)AssetDatabase.LoadAssetAtPath("Assets/ElectronicMindStudios/Shader Sandwich/Internal/Shader Sandwich/Textures/ShaderSandwichPerlinNoise.png",typeof(Texture2D)));
		//Debug.Log(EditorGUIUtility.FindTexture("ElectronicMindStudios/Shader Sandwich/Internal/Shader Sandwich/Textures/ShaderSandwichPerlinNoise.png") as Texture2D);
		//Shader.SetGlobalTexture("_ShaderSandwichPerlinTexture",(Texture2D)AssetDatabase.LoadAssetAtPath("Assets/ElectronicMindStudios/Shader Sandwich/Internal/Shader Sandwich/Textures/ShaderSandwichPerlinNoise.png",typeof(Texture2D)));
		//if (windowS.QuestionMarkOff==null)
		//windowG.OnEnable();
		
		//ShaderSandwich.Instance.OpenShader = new ShaderBase();
	}
	public ShaderSandwich(){
		Instance = this;
		if (!ShaderVarObjectFieldUpdates.ContainsKey("Mask"))
			ShaderVarObjectFieldUpdates["Mask"] = null;//MaskObjectsUpdate;
		if (!ShaderVarObjectFieldUpdates.ContainsKey("LayerType"))
			ShaderVarObjectFieldUpdates["LayerType"] = null;
		//if (ShaderLayerEffectInstances.Count==0)
		//LoadPlugins();
		ForceHookUpdate = true;
	}
	public static void LoadAssets(){
		if (SSSettings==null){
			SSSettings = (ShaderSandwichSettings)AssetDatabase.LoadAssetAtPath("Assets/ElectronicMindStudios/Shader Sandwich/Internal/Editor/SSSettings.asset",typeof(ShaderSandwichSettings));
			if (SSSettings==null){
				SSSettings = (ShaderSandwichSettings)ScriptableObject.CreateInstance<ShaderSandwichSettings>();
				if (SSSettings!=null)
				AssetDatabase.CreateAsset(SSSettings,"Assets/ElectronicMindStudios/Shader Sandwich/Internal/Editor/SSSettings.asset");
				else
					Debug.LogWarning("No idea why this keeps happening but it makes Unity act all funny if it does, so instead here's a message...");
			}
		}
		
		if (GUITrans==null)
			GUITrans = new ShaderTransition();
		if (StartNewTrans==null)
			StartNewTrans = new ShaderTransition();
		if (DragAreaTex==null)
			DragAreaTex = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Movement/DragArea.fw.png") as Texture2D;
		if (DragAreaTex2==null)
			DragAreaTex2 = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Movement/DragArea2.fw.png") as Texture2D;
		if (UPArrow==null)
			UPArrow = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Movement/UPArrow.fw.png") as Texture2D;
		//Debug.Log(UPArrow);
		if (DOWNArrow==null)
			DOWNArrow = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Movement/DOWNArrow.fw.png") as Texture2D;
		if (QuestionMarkOff==null)
			QuestionMarkOff = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/QuestionMarkOff.png") as Texture2D;
		if (QuestionMarkOn==null)
			QuestionMarkOn = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/QuestionMarkOn.png") as Texture2D;
		if (LightBulbOff==null)
			LightBulbOff = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/LightBulbOff.fw.png") as Texture2D;
		if (LightBulbOn==null)
			LightBulbOn = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/LightBulbOn.fw.png") as Texture2D;	
		if (Warning==null)
			Warning = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/Warning.fw.png") as Texture2D;	
		if (ReloadIcon==null)
			ReloadIcon = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/Reload.fw.png") as Texture2D;	
		if (Plus==null)
			Plus = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Movement/Plus.fw.png") as Texture2D;	
		if (Tick==null)
			Tick = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Movement/On.fw.png") as Texture2D;
		if (Cross==null)
			Cross = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Movement/Off.fw.png") as Texture2D;	
		if (Bin==null)
			Bin = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/ActualBin.png") as Texture2D;	
		if (Bin2==null)
			Bin2 = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/zBin.png") as Texture2D;	
		if (Bin3==null)
			Bin3 = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/zBin2.png") as Texture2D;	
		if (StarOn==null)
			StarOn = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/StarOn.png") as Texture2D;	
		if (StarOff==null)
			StarOff = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/StarOff.png") as Texture2D;	
		if (CrossRed==null)
			CrossRed = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Movement/Cross.fw.png") as Texture2D;	
		if (Banner==null)
			Banner = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/LineBannerPro.fw.png") as Texture2D;
		if (Logo==null){
			DateTime now = DateTime.Now;
			if ((now.Month==11&&now.Day>=23)||(now.Month==12))
				Logo = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/LogoChristmas.fw.png") as Texture2D;
			else
				Logo = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/Logo.fw.png") as Texture2D;
		}
		if (BannerLight==null)
			BannerLight = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/LineBanner.png") as Texture2D;
		if (RightArrow==null)
			RightArrow = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Movement/RightArrow.fw.png") as Texture2D;
		if (LeftArrow==null)
			LeftArrow = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Movement/LEFTArrow.fw.png") as Texture2D;
		if (RightTriangle==null)
			RightTriangle = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Movement/RightTriangle.png") as Texture2D;
		if (LeftTriangle==null)
			LeftTriangle = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Movement/LeftTriangle.png") as Texture2D;
		if (Literal==null)
			Literal = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Resources/Literal.png") as Texture2D;
		if (ImageBasedPerlinNoiseTempLinear==null)
			ImageBasedPerlinNoiseTempLinear = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/ElectronicMindStudios/Shader Sandwich/Internal/Shader Sandwich/Textures/ShaderSandwichPerlinNoiseLinear.png",typeof(Texture2D));
		if (ImageBasedPerlinNoiseTemp==null)
			ImageBasedPerlinNoiseTemp = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/ElectronicMindStudios/Shader Sandwich/Internal/Shader Sandwich/Textures/ShaderSandwichPerlinNoise.png",typeof(Texture2D));
		if (PerlNoise==null)
			PerlNoise = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Resources/PerlNoise.png") as Texture2D;
		if (BlockNoise==null)
			BlockNoise = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Resources/BlockNoise.png") as Texture2D;
		if (GrabPass==null)
			GrabPass = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Resources/GrabPass.png") as Texture2D;
		if (ThisThingo==null)
			ThisThingo = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Resources/ThisThingo.png") as Texture2D;
		if (ThisDepthThing==null)
			ThisDepthThing = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Resources/ThisDepthThing.png") as Texture2D;
		if (DepthPass==null)
			DepthPass = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Resources/DepthPass.png") as Texture2D;
		if (CubistNoise==null)
			CubistNoise = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Resources/CubistNoise.png") as Texture2D;
		if (CellNoise==null)
			CellNoise = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Resources/CellNoise.png") as Texture2D;
		if (DotNoise==null)
			DotNoise = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Resources/DotNoise.png") as Texture2D;
		if (KitchenCube==null)
			KitchenCube = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Resources/KitchenCube.png") as Cubemap;
		if (BlackCube==null)
			BlackCube = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Resources/BlackCube.png") as Cubemap;
		if (DayCube==null)
			DayCube = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Resources/SSDaySky.exr") as Cubemap;
		if (SunsetCube==null)
			SunsetCube = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Resources/SSSunsetSky.exr") as Cubemap;
		if (IconBase==null)
			IconBase = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/IconBase.png") as Texture2D;
		if (IconShell==null)
			IconShell = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/IconShell.png") as Texture2D;
		if (IconMask==null)
			IconMask = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/IconMask.png") as Texture2D;
		if (EditableOn==null)
			EditableOn = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/EditableOn.fw.png") as Texture2D;
		if (EditableOff==null)
			EditableOff = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/EditableOff.fw.png") as Texture2D;
		if (EyeClose==null)
			EyeClose = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/EyeClose.fw.png") as Texture2D;
		if (EyeOpen==null)
			EyeOpen = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/EyeOpen.fw.png") as Texture2D;
		if (AlphaOn==null)
			AlphaOn = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/Alpha.fw.png") as Texture2D;
		if (AlphaOff==null)
			AlphaOff = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/AlphaOff.fw.png") as Texture2D;
		if (Gear==null)
			Gear = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/Gear.fw.png") as Texture2D;
		if (GearLinked==null)
			GearLinked = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/GearLinked.fw.png") as Texture2D;
		if (ChainLinked==null)
			ChainLinked = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/Linked.png") as Texture2D;
		if (ChainNotLinked==null)
			ChainNotLinked = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Icons/NotLinked.png") as Texture2D;
		if (WindowEdge==null)
			WindowEdge = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Misc/Interface/Window5.png") as Texture2D;
		//if (Happy==null)
		//	Happy = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Resources/Happy.png") as Texture2D;
		if (MiniDownArrow==null)
			MiniDownArrow = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Movement/MiniDownArrow.fw.png") as Texture2D;

		
		if (!ShaderVarObjectFieldUpdates.ContainsKey("Mask"))
			ShaderVarObjectFieldUpdates["Mask"] = null;//MaskObjectsUpdate;
		if (!ShaderVarObjectFieldUpdates.ContainsKey("LayerType"))
			ShaderVarObjectFieldUpdates["LayerType"] = null;
		//if (ShaderSandwich.Instance.OpenShader==null)
		if (ShaderLayerEffectInstances.Count==0)
			LoadPlugins();
	}
		//Color TextColor = new Color(0.8f,0.8f,0.8f,1f);
		//Color TextColorA = new Color(1f,1f,1f,1f);
		Color BackgroundColor = new Color(0.18f,0.18f,0.18f,1);
	[NonSerialized]public bool HasUsedRepaint = false;
	
	/*public GameObject UndoHack;
	public List<ShaderHistory> UndoHistory = new List<ShaderHistory>();
	public int UndoHistoryIndex = -1;
	static public void Undo(ShaderHistory SH){
		if (ShaderSandwich.Instance.UndoHistoryIndex<ShaderSandwich.Instance.UndoHistory.Count-1){
			ShaderSandwich.Instance.UndoHistory.RemoveRange(ShaderSandwich.Instance.UndoHistoryIndex+1,ShaderSandwich.Instance.UndoHistory.Count-(ShaderSandwich.Instance.UndoHistoryIndex+1));
		}
		ShaderSandwich.Instance.UndoHistory.Add(SH);
		ShaderSandwich.Instance.UndoHistoryIndex++;
		
		//UnityEditor.Undo.IncrementCurrentGroup();
		//ShaderSandwich.Instance.UndoHack.transform.position = new Vector3(UnityEngine.Random.value,78,2);
		//UnityEditor.Undo.RecordObject(ShaderSandwich.Instance.UndoHack.transform, "Shader Sandwich: "+SH.ToString());
		//UnityEditor.Undo.SetCurrentGroupName("IDDISOMETHINGISWEAR23!!!");
		//UnityEditor.Undo.IncrementCurrentGroup();
		UnityEditor.Undo.IncrementCurrentGroup();
		UnityEditor.Undo.RecordObject(ShaderSandwich.Instance.UndoHack.transform, "Shader Sandwich: "+SH.ToString()+UnityEngine.Random.value.ToString());
		ShaderSandwich.Instance.UndoHack.transform.position = new Vector3(UnityEngine.Random.value,78,2);
		//UnityEditor.Undo.IncrementCurrentGroup();
		//UnityEditor.Undo.SetCurrentGroupName("IDDISOMETHINGISWEAR!!!");
//		Debug.Log("Added Undo");
	}
	//string LastUndoName = 
	string LastGroupName = "";
	string GroupName = "";
	int GroupID;*/
    void OnGUI() {
		Instance = this;
		/*if (UnityEditor.Undo.GetCurrentGroup()!=GroupID){
			GroupID = UnityEditor.Undo.GetCurrentGroup();
			LastGroupName = GroupName;
			GroupName = UnityEditor.Undo.GetCurrentGroupName();
			//Debug.Log(GroupName);
		}
		
		if (UndoHack==null)
			UndoHack = new GameObject();//(ShaderLayer)ScriptableObject.CreateInstance<ShaderLayer>();
		//if (Event.current.type == EventType.ExecuteCommand){
		//	Debug.Log(Event.current.commandName);
		//	Debug.Log(UnityEditor.Undo.GetCurrentGroupName());
		//}
		if (Event.current.type == EventType.ValidateCommand){
			//Debug.Log(Event.current.commandName);
			//Event.current.Use();
			if ((Event.current.commandName == "UndoRedoPerformed")&&LastGroupName.Contains("Shader Sandwich: ")){
				//Debug.Log("Yay it registered undo :D" );
				Event.current.Use();
				
				//Debug.Log(UndoHistoryIndex);
				//Debug.Log(UndoHistory.Count);
				if (UndoHistoryIndex>=0&&UndoHistory.Count>UndoHistoryIndex){
					UndoHistory[UndoHistoryIndex].Undo();
					UndoHistoryIndex-=1;
				}
				//UndoHack.transform.position = new Vector3(0,38,49);
				//Undo.RecordObject(UndoHack.transform, "Shader Sandwich Undo2!");
			}
		}*/
		
		
		
		if (SRS==null){
			SRS = ScriptableObject.CreateInstance<ShaderRandomStuffForSerialization>();
		}
		if (DereBeARefreshHere){
			SRS.LayerSelection = null;
		}
		

		
		
		
		if (Event.current.type == EventType.ValidateCommand){
			if ((Event.current.commandName == "UndoRedoPerformed")){
				GUI.changed = true;
			}
		}
		
		if(Event.current.type==EventType.Repaint){
			//DeserializedLoadThing--;
			//if (DeserializedLoadThing==0){
			//	LoadFromSerializedProtectNonsense();
			//}
		
			Instance.InterfaceColor = new Color(
				EditorPrefs.GetFloat("ElectronicMindStudiosInterfaceColor_R", 0f),
				EditorPrefs.GetFloat("ElectronicMindStudiosInterfaceColor_G", 39f/255f),
				EditorPrefs.GetFloat("ElectronicMindStudiosInterfaceColor_B", 51f/255f),
				EditorPrefs.GetFloat("ElectronicMindStudiosInterfaceColor_A", 1f)
			);
			Instance.InterfaceSize = EditorPrefs.GetInt("ElectronicMindStudiosInterfaceSize", 10);
		}
		ValueChanged = false;
		//RunHotkeys();
		EMindGUI.TrackMouseStuff();
		EMindGUI.Rects.Clear();
		EMindGUI.RectPos = new Vector2(0,0);
		//ShaderSandwich windowG = this;//(ShaderSandwich)EditorWindow.GetWindow (typeof (ShaderSandwich),false,"",false);
		
		int MenuPad = 15;
		
		Vector2 WinSize = new Vector2(position.width,position.height);
		Vector2 WinSize2 = new Vector2(position.width,position.height-MenuPad);
		
		GUIMouseDown = false;
		GUIMouseUp = false;
		
		if ((Event.current.type == EventType.MouseDown))
		GUIMouseDown = true;
		if ((Event.current.type == EventType.MouseUp))
		GUIMouseUp = true;
		
		if (GUIMouseDown)
		GUIMouseHold = true;		
		if (GUIMouseUp)
		GUIMouseHold = false;
		
		LoadAssets();
		
		if (DelayedSerLoad==2){
			DelayedSerLoad = 0;
			LoadFromSerializedProtectNonsense();
			SerializeProtection = "";
			ForceHookUpdate = true;
		}
		
		SSSettings.CurPath = CurrentFilePath;
//		GUISkin oldskin = GUI.skin;
		//if(Event.current.type==EventType.Repaint){
		EMindGUI.AddProSkin(WinSize);
		if (!HasUsedRepaint&&Event.current.type==EventType.Repaint)
			ShaderUtil.ClearGUIStyles();
		HasUsedRepaint = true;
		//}
	
		//ShaderUtil.ClearGUIStyles();
		ShaderUtil.CheckGUIStyles();

		if (ShaderVarEditing!=null)
		ShaderVarEditing.UseEditingMouse(false);
		
		bool SVMD = false;
		if (ShaderVarEditing!=null&&(Event.current.type!=EventType.Repaint))//.type== EventType.MouseDown))
		SVMD = true;
		
		if (SVMD)
		EMindGUI.BeginGroup(new Rect(0,0,0,0));
		if (OpenShader!=null&&AnimateInputs&&Event.current.type==EventType.Repaint){
			foreach(ShaderInput SI in OpenShader.ShaderInputs)
			SI.Update();
		}		
			//if (EditorWindow.focusedWindow==this)
			Repaint();
			//TransitionTime = Mathf.Min(TransitionTime,1);
			if (GUITrans.DoneHit())
			{
				GUIStage = GUITransition;
			}

			if (OpenShader!=null&&OpenShader.ShaderPasses!=null&&OpenShader.ShaderPasses.Count>0&&CurInterfaceStyle==InterfaceStyle.Modern){
				
				/*if (GUI.Button( EMindGUI.Rect2(RectDir.Bottom,1,WinSize.y,66,30),"Home",ShaderUtil.BoxMiddle15)&&GUIStage!=GUIType.Start)
					Goto(GUIType.Start,ShaderTransition.TransDir.Backward);
				if (GUI.Button( EMindGUI.Rect2(RectDir.Bottom,67,WinSize.y,66,30),"Layers",ShaderUtil.BoxMiddle15)&&GUIStage!=GUIType.Layers)
					Goto(GUIType.Layers,(GUITransition>GUIType.Layers)?ShaderTransition.TransDir.Backward:ShaderTransition.TransDir.Forward);	
				if (GUI.Button( EMindGUI.Rect2(RectDir.Bottom,67+66,WinSize.y,66,30),"Settings",ShaderUtil.BoxMiddle15)&&GUIStage!=GUIType.Configure)
					Goto(GUIType.Configure,(GUITransition>GUIType.Configure)?ShaderTransition.TransDir.Backward:ShaderTransition.TransDir.Forward);	
				if (GUI.Button( EMindGUI.Rect2(RectDir.Bottom,67+66+66,WinSize.y,66,30),"Inputs",ShaderUtil.BoxMiddle15)&&GUIStage!=GUIType.Inputs)
					Goto(GUIType.Inputs,ShaderTransition.TransDir.Forward);*/
				
				if (GUI.Button( EMindGUI.Rect2(RectDir.Bottom,1,WinSize.y,50,30),"Home",ShaderUtil.BoxMiddle15)&&GUIStage!=GUIType.Start)
					Goto(GUIType.Start,ShaderTransition.TransDir.Backward);
				if (GUI.Button( EMindGUI.Rect2(RectDir.Bottom,50,WinSize.y,56,30),"Layers",ShaderUtil.BoxMiddle15)&&GUIStage!=GUIType.Layers)
					Goto(GUIType.Layers,(GUITransition>GUIType.Layers)?ShaderTransition.TransDir.Backward:ShaderTransition.TransDir.Forward);	
				if (GUI.Button( EMindGUI.Rect2(RectDir.Bottom,50+56,WinSize.y,100,30),"Settings",ShaderUtil.BoxMiddle15)&&GUIStage!=GUIType.Configure)
					Goto(GUIType.Configure,(GUITransition>GUIType.Configure)?ShaderTransition.TransDir.Backward:ShaderTransition.TransDir.Forward);	
				if (GUI.Button( EMindGUI.Rect2(RectDir.Bottom,50+56+100,WinSize.y,58,30),"Inputs",ShaderUtil.BoxMiddle15)&&GUIStage!=GUIType.Inputs)
					Goto(GUIType.Inputs,ShaderTransition.TransDir.Forward);
			}
			if (OpenShader==null||OpenShader.ShaderPasses==null||OpenShader.ShaderPasses.Count==0){
				if (OpenShader!=null)
				EditorUtility.DisplayDialog("Error!","The currently open shader has no passes! This is only caused by loading a buggy file or some really bad internal error, I'm really sorry :(","Ok");
				if (SerializeProtection=="")
					GUIStage = GUIType.Start;
				OpenShader = null;
			}
//			Debug.Log(GUIStage);
//			Debug.Log(GUITransition);
			float Movement = 0f;
			if (GUITrans.Transitioning==ShaderTransition.TransDir.Backward)
			Movement = WinSize.x;
			
			if (GUIStage==GUIType.Start)
				GUIStart(WinSize2,new Vector2(-GUITrans.Get()*WinSize.x+Movement,MenuPad));
			//else if (GUIStage==GUIType.Presets)
			//	GUIPresets(WinSize2,new Vector2(-GUITrans.Get()*WinSize.x+Movement,MenuPad));
			else if (GUIStage==GUIType.Configure)
				GUIConfigure(WinSize2,new Vector2(-GUITrans.Get()*WinSize.x+Movement,MenuPad));
			else if (GUIStage==GUIType.Layers)
				GUILayers(WinSize2,new Vector2(-GUITrans.Get()*WinSize.x+Movement,MenuPad));
			else if (GUIStage==GUIType.Inputs){
				if (!UseOldInputs)GUIInputs(WinSize2,new Vector2(-GUITrans.Get()*WinSize.x+Movement,MenuPad));
				if (UseOldInputs)GUIInputs2(WinSize2,new Vector2(-GUITrans.Get()*WinSize.x+Movement,MenuPad));
			}
			
			if (GUITrans.Done()!=true){
				Movement = WinSize2.x;
				if (GUITrans.Transitioning==ShaderTransition.TransDir.Backward)
				Movement = 0*WinSize2.x;
				
				if (GUITransition==GUIType.Start)
					GUIStart(WinSize2,new Vector2(-GUITrans.Get()*WinSize.x+Movement,MenuPad));
				//else if (GUITransition==GUIType.Presets)
				//	GUIPresets(WinSize2,new Vector2(-GUITrans.Get()*WinSize.x+Movement,MenuPad));
				else if (GUITransition==GUIType.Configure)
					GUIConfigure(WinSize2,new Vector2(-GUITrans.Get()*WinSize.x+Movement,MenuPad));
				else if (GUITransition==GUIType.Layers)
					GUILayers(WinSize2,new Vector2(-GUITrans.Get()*WinSize.x+Movement,MenuPad));
				else if (GUITransition==GUIType.Inputs){
					if (!UseOldInputs)GUIInputs(WinSize2,new Vector2(-GUITrans.Get()*WinSize.x+Movement,MenuPad));
					if (UseOldInputs)GUIInputs2(WinSize2,new Vector2(-GUITrans.Get()*WinSize.x+Movement,MenuPad));
				}
			}
			//if (GUIStage!=GUIType.Start){
			if (OpenShader!=null&&CurInterfaceStyle==InterfaceStyle.Modern){
				GUI.Toggle( EMindGUI.Rect2(RectDir.Bottom,1,WinSize.y,50,30),GUITransition==GUIType.Start,"Home",ShaderUtil.ButtonMiddle15);//ShaderUtil.ButtonMiddle15
				GUI.Toggle( EMindGUI.Rect2(RectDir.Bottom,50,WinSize.y,56,30),GUITransition==GUIType.Layers,"Layers",ShaderUtil.ButtonMiddle15);
				GUI.Toggle( EMindGUI.Rect2(RectDir.Bottom,50+56,WinSize.y,100,30),GUITransition==GUIType.Configure,"Ingredients",ShaderUtil.ButtonMiddle15);
				GUI.Toggle( EMindGUI.Rect2(RectDir.Bottom,50+56+100,WinSize.y,58,30),GUITransition==GUIType.Inputs,"Bread",ShaderUtil.ButtonMiddle15);
				
				bool NonHiddenFixes = false;
				foreach(ShaderFix sf in OpenShader.ProposedFixes){
					if (sf.Ignore!=true)
						NonHiddenFixes = true;
				}
				if (NonHiddenFixes)
					GUI.DrawTexture(EMindGUI.Rect2(RectDir.Bottom,50+56+100+45,WinSize.y-18,26,20),Warning);
				/*GUI.Toggle( EMindGUI.Rect2(RectDir.Bottom,1,WinSize.y,66,22),GUITransition==GUIType.Start,"Home",ShaderUtil.ButtonMiddle15);//ShaderUtil.ButtonMiddle15
				GUI.Toggle( EMindGUI.Rect2(RectDir.Bottom,67,WinSize.y,66,22),GUITransition==GUIType.Layers,"Layers",ShaderUtil.ButtonMiddle15);
				GUI.Toggle( EMindGUI.Rect2(RectDir.Bottom,67+66,WinSize.y,66,22),GUITransition==GUIType.Configure,"Ingredients",ShaderUtil.ButtonMiddle15);
				GUI.Toggle( EMindGUI.Rect2(RectDir.Bottom,67+66+66,WinSize.y,66,22),GUITransition==GUIType.Inputs,"Bread",ShaderUtil.ButtonMiddle15);*/
				
				EditorUtility.SetDirty(OpenShader);
			}
			
		
		if (SVMD)
		EMindGUI.EndGroup();
		
		if (ShaderVarEditing!=null)
		ShaderVarEditing.UseEditingMouse(true);
		
		

		//Debug.Log(GUI.tooltip=="");
		//Debug.Log(TooltipAlpha);
		//GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,TooltipAlpha);

		EMindGUI.EndProSkin();
		GUI.color = new Color(1f,1f,1f,1f);
		GUI.backgroundColor = new Color(1f,1f,1f,1f);
		DrawMenu();
		EMindGUI.DrawTooltip(0,WinSize);
		if (GUI.changed==true){
			changedTimer = 100;
		}
		changedTimer -=1;
		/*if (changedTimer>-100&&changedTimer<=0)
		{
			if (OpenShader!=null)
			OpenShader.RecalculateAutoInputs();
			changedTimer = -200;
		}*/
		
		if (ForceHookUpdate == true&&OpenShader!=null){
			ForceHookUpdate = false;
			
			foreach (ShaderVar SV in ShaderUtil.GetAllShaderVars()){
				SV.HookIntoObjects();
			}
			ForceHookUpdate = false;
			//UpdateMaskShaderVars();
			//UpdateLayerTypeShaderVars();
			//Debug.Log("Wasteful hook");
		}
		
		if (PleaseReimport!=""&&Event.current.type==EventType.Repaint){
			GiveMeAFrame+=1;
			if (GiveMeAFrame==4){
				AssetDatabase.ImportAsset(PleaseReimport,ImportAssetOptions.ForceSynchronousImport);
				UpdateShaderPreview();
				//SetStatus("Updating Preview Window","Preview Updated in " + CompileTime + " milliseconds.",1f);
				SetStatus("Updating Preview Window","Preview Updated",1f);
				PleaseReimport = "";
				GiveMeAFrame = 0;
			}
		}
		DereBeARefreshHere = false;
		//if (Event.current.type == EventType.Repaint)
		//	EMindGUI.TrackMouseReset();
    }
	Vector2 GUIConfigureBoxStart(string Name,ShaderVar Tickbox,Vector2 WinSize,float X,float Y){
		float BoxWidth = (WinSize.x-15f)/2f;
		float BoxHeight = 190f;	
		Vector2 RetHW = new Vector2(BoxWidth-15f,BoxHeight-15f);
		
		if (Name!="")
		{
		if (CurInterfaceStyle==InterfaceStyle.Flat)
		EMindGUI.BeginGroup(new Rect((int)((BoxWidth)*(float)X+15f),(BoxHeight)*(float)Y+15f+144f+BaseSettingsHeight,BoxWidth-15f,BoxHeight-15f),GUI.skin.box);
		else
		EMindGUI.BeginGroup(new Rect((int)((BoxWidth)*(float)X+15f),(BoxHeight)*(float)Y+15f+144f+BaseSettingsHeight,BoxWidth-15f,BoxHeight-15f),GUI.skin.button);
		
		
		BoxWidth-=15f;
		BoxHeight-=15f;
		GUI.skin.label.alignment = TextAnchor.UpperLeft;
		GUI.skin.label.fontSize = 20;
		GUI.Label(new Rect(10,4,BoxWidth,50),Name);
		
		if (Tickbox!=null){
		if (Tickbox.On)
		{
			Tickbox.On = GUI.Toggle(new Rect(BoxWidth-34,4,30,30),Tickbox.On,new GUIContent(Tick),"Button");
		}
		else
		{
			Tickbox.On = GUI.Toggle(new Rect(BoxWidth-34,4,30,30),Tickbox.On,new GUIContent(Cross),"Button");
		}
		GUI.enabled = true;
		if (!Tickbox.On)
		GUI.enabled = false;
		}
		}
		return RetHW;//new Vector2(BoxWidth,BoxHeight);
	}
	void GUIConfigureBoxEnd(){
		GUI.enabled = true;
		EMindGUI.EndGroup();
	}
	/*void OnSceneGUI(){
		RunHotkeys();
	}
	void RunHotkeys(){
		if ( Event.current.type == EventType.keyDown )
		{
			if ( Event.current.isKey && Event.current.control)
			{
				if (Event.current.keyCode == KeyCode.S){
					Save();
					Event.current.Use();
				}
			}
		}
	}*/
				/*float Min = 2;
			float Max = 5;
				float MinL = 0;
			float MaxL = 10;*/
	public void DrawSurfaceRowThingy(ShaderPass SP, ShaderIngredient ingredient, GUIStyle surfaceButtonStyle, RectOffset oldpadding,int WinSize, int x, int y,ShaderSurface surface, ShaderGeometryModifier geometryModifier){
		
		int Height = ingredient.Height;
		
		//if (surface!=null&&surface.ShaderSurfaceComponent==ShaderSurfaceComponent.Mask)
		//	Height = 16;
		bool initDrag = SP.dragSurface&&SP.selectedIngredient==ingredient;
		if (initDrag){
			//GUI.BeginGroup(new Rect(Event.current.mousePosition.x-SP.dragSurfaceOffset.x,Event.current.mousePosition.y-SP.dragSurfaceOffset.y,(WinSize.x-30-16)-264+1,32));
			if (new Vector2(Event.current.mousePosition.x-SP.dragSurfaceOffset.x,Event.current.mousePosition.y-SP.dragSurfaceOffset.y).magnitude>2){
				SP.hasDraggedSurface = true;
			}
			if (SP.hasDraggedSurface)
				GUI.BeginGroup(new Rect(Event.current.mousePosition.x-SP.dragSurfaceOffset.x,Event.current.mousePosition.y-SP.dragSurfaceOffset.y,10000,10000));
			else
				initDrag = false;
		}
		Color asdasd = GUI.color;
		if (Event.current.type==EventType.Repaint){
			if (EMindGUI.MouseHoldIn(new Rect(x+16,153+BaseSettingsHeight+y,WinSize,Height),false)&&SP.selectedIngredient!=ingredient)
				GUI.color = new Color(0.9f,0.9f,0.9f,1f);
			//if (!ingredient.Locked)
				surfaceButtonStyle.Draw(new Rect(x+16,153+BaseSettingsHeight+y,WinSize,Height),ingredient.UserName.Text,false, y>40,SP.selectedIngredient==ingredient,new Rect(x+16,153+BaseSettingsHeight+y,WinSize,Height).Contains(Event.current.mousePosition));
			//else{
			//	surfaceButtonStyle.Draw(new Rect(x+16,153+BaseSettingsHeight+y,WinSize,Height),ingredient.UserName.Text,false, y>40,SP.selectedIngredient==ingredient,new Rect(x+16,153+BaseSettingsHeight+y,WinSize,Height).Contains(Event.current.mousePosition));
				//Color oldbg = GUI.backgroundColor;
				//GUI.backgroundColor = new Color(oldbg.r,oldbg.g,oldbg.b,0f);
				//surfaceButtonStyle.Draw(new Rect(x+16,153+BaseSettingsHeight+y,WinSize,Height),ingredient.UserName.Text,false, y>40,SP.selectedIngredient==ingredient,new Rect(x+16,153+BaseSettingsHeight+y,WinSize,Height).Contains(Event.current.mousePosition));
				//GUI.backgroundColor = oldbg;
			//}
		}
		//surfaceButtonStyle.Draw(new Rect(16,153+BaseSettingsHeight+y,WinSize,32),ingredient.Name,false, EMindGUI.MouseDownIn(new Rect(16,153+BaseSettingsHeight+y,WinSize,32),false),SP.selectedIngredient==ingredient,SP.selectedIngredient==ingredient);
	
		//Rect dragArea = new Rect(x+16,153+BaseSettingsHeight+y,64,Height);
		//Rect linkArea = new Rect(16+WinSize-16+x,153+BaseSettingsHeight+y,16,Height);
		Rect linkArea = new Rect(16+x,153+BaseSettingsHeight+y,16,Height);
		Rect dragArea = new Rect(x+16+linkArea.width,153+BaseSettingsHeight+y,WinSize-linkArea.width,Height-8);
		if (geometryModifier!=null)
			dragArea.height+=8;
		
		if (!ingredient.Locked){
				//if (SP.selectedIngredient==ingredient||(dragArea.Contains(Event.current.mousePosition)&&!SP.dragSurface))
				//	GUI.color = EMindGUI.BaseInterfaceColor;
				//else
				//	GUI.color = new Color(0.4f,0.4f,0.4f,1f);
			
			
			///GUI.DrawTexture(dragArea,DragAreaTex,ScaleMode.ScaleAndCrop);
			if (initDrag)
			EditorGUIUtility.AddCursorRect(dragArea,MouseCursor.Pan);
			
		}
		GUI.color = asdasd;
		if (!SP.dragSurface&&EMindGUI.MouseUpIn(new Rect(x+16,153+BaseSettingsHeight+y,WinSize,Height),false)){
			SP.selectedIngredient = ingredient;
			EMindGUI.Defocus();
			if (Event.current.button == 1 ){
				GenericMenu toolsMenu = new GenericMenu();
				if (!ingredient.Locked)
					toolsMenu.AddItem(new GUIContent("Copy"), false, IngredientCopy,ingredient);
					toolsMenu.AddItem(new GUIContent("Paste"), false, IngredientPaste,new object[]{(object)SP,ingredient});
				if (!ingredient.Locked)
					toolsMenu.AddItem(new GUIContent("Delete"), false, SP.DeleteIngredient,ingredient);
				toolsMenu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0));
				GUI.changed = true;				
			}
		}
		
		
		
		//ingredient.ShouldLink.On = GUI.Toggle(linkArea,ingredient.ShouldLink.On,"L",EMindGUI.ButtonImage);
		EMindGUI.MakeTooltip(0,linkArea,"Link? - Makes the ingredient share similar properties (like roughness and light radius) with other ingredients.");
		if (!ingredient.Locked){
			ingredient.ShouldLink.On = GUI.Toggle(linkArea,ingredient.ShouldLink.On,new GUIContent("",ingredient.ShouldLink.On?ChainLinked:ChainNotLinked,"Link? - Makes the ingredient share similar properties (like roughness and light radius) with other ingredients."),EMindGUI.ButtonNoBorder);
			//ingredient.ShouldLink.On = GUI.Toggle(linkArea,ingredient.ShouldLink.On,new GUIContent("",ingredient.ShouldLink.On?ChainLinked:ChainNotLinked,"Link? - Makes the ingredient share similar properties (like roughness and light radius) with other ingredients."),EMindGUI.ButtonNoBorder);
		
			ingredient.UpdateLinked(SP);
		}
		
		if (Event.current.button != 1 && !ingredient.Locked && EMindGUI.MouseDownIn(dragArea,true)){
			SP.dragSurface = true;
			SP.dragSurfaceOffset = Event.current.mousePosition;
			SP.selectedIngredient = ingredient;
			EMindGUI.Defocus();
		}
		//EMindGUI.BeginGroup(new Rect(dragArea.x+dragArea.width,153+BaseSettingsHeight+y,WinSize-dragArea.width,Height));
		//	ingredient.RenderWedgeUI(SP, WinSize-dragArea.width-linkArea.width);
		EMindGUI.BeginGroup(new Rect(dragArea.x,153+BaseSettingsHeight+y,WinSize,Height));
			ingredient.RenderWedgeUI(SP, WinSize-linkArea.width);
		EMindGUI.EndGroup();
		/*if (surface!=null){
			Rect MixAmountPos = new Rect(dragArea.x+dragArea.width,153+BaseSettingsHeight+y+15+5+4,WinSize-dragArea.width,8);
			if (EMindGUI.MouseDownIn(MixAmountPos,false)){
				SP.selectedIngredient = ingredient;
			}
			ShaderUtil.DrawCoolSlider(MixAmountPos,surface.MixAmount,SP.selectedIngredient==ingredient);
		}
		ShaderIngredientUpdateMask updatemask = ingredient as ShaderIngredientUpdateMask;
		if (updatemask!=null){
			foreach()
		}*/
		
		if (!SP.dragSurface&&EMindGUI.MouseUpIn(new Rect(x+16+linkArea.width,153+BaseSettingsHeight+y,WinSize-linkArea.width,Height))){
			SP.selectedIngredient = ingredient;
			EMindGUI.Defocus();
		}
		//EMindGUI.Label(new Rect(15,154+BaseSettingsHeight+y,(WinSize.x-30-16)-264,32),surface.Name,24);
		if (initDrag){
			GUI.EndGroup();
		}
	}
			
	int BaseSettingsHeight = 72;//50;
	
	void GUIConfigure(Vector2 WinSize,Vector2 Position){
		//RunHotkeys();
		ShaderBase SSIO = OpenShader;

		if (SSIO==null){
			if (SerializeProtection==""){
				GUIStage = GUIType.Start;
				GUITrans.Reset();
			}
		}
		else{
			FixScan(100);
			
			int OldShaderLayerTab = ShaderLayerTab;
			bool ChangeShaderLayerTab = false;
			if (ShaderLayerTab<0)
				ShaderLayerTab = 0;
			if (ShaderLayerTab>SSIO.ShaderPasses.Count-1)
				ShaderLayerTab = SSIO.ShaderPasses.Count-1;
			ShaderPass SP = SSIO.ShaderPasses[ShaderLayerTab];
			if (Event.current.type!=EventType.Repaint&&Event.current.type!=EventType.Layout)
				UnityEditor.Undo.RecordObject((UnityEngine.Object)SP,"Change Shader Pass");
			EMindGUI.BeginGroup(new Rect(Position.x,Position.y,WinSize.x,WinSize.y));

			if (CurInterfaceStyle!=InterfaceStyle.Modern)
			if (GUI.Button( EMindGUI.Rect2(RectDir.Diag,WinSize.x,WinSize.y,100,30),"Layers",ShaderUtil.BoxMiddle20))
			Goto(GUIType.Layers,ShaderTransition.TransDir.Forward);
			
			//Vector2 BoxSize = GUIConfigureBoxStart("",SP.DiffuseOn,WinSize,0,0);
			
			bool VertexToastieInstalled = Type.GetType("VertexToastie")!=null;
			bool ShaderSandwichPokeInstalled = Type.GetType("ShaderSandwichPokeExistsYup")!=null;//PokeOn
			float AdditionalHeight2 = 5f;
			AdditionalHeight2 += VertexToastieInstalled?1.2f:0f;
			AdditionalHeight2 += ShaderSandwichPokeInstalled?1.2f:0f;
			
			BaseSettingsHeight = 72;
			//(BoxHeight)*(float)Y+15f+144f+BaseSettingsHeight
			//float height = (BoxSize.y+15f)*(float)AdditionalHeight2+144f+BaseSettingsHeight+29f;
			//ConfigScroll = EMindGUI.BeginScroollView(new Rect(0,0,WinSize.x,WinSize.y),ConfigScroll,new Rect(0,0,WinSize.x-16,(BoxSize.y+15f)*6.2f+144f+29f+BaseSettingsHeight),false,true);
			int SettingsBarWidth = 264;
			int h = 496;//434+75;
			if (SP.selectedIngredient!=null)
				h = Math.Max(h,SP.selectedIngredient.GetHeight((WinSize.x-30-16)-SettingsBarWidth)+75);
			
			
			
			h = Math.Max(h,31+31+31+31+SP.Surfaces.Count*31+SP.GeometryModifiers.Count*31);
			
			int scrllbarheight = 153+h-66-(129-115);
			ConfigScroll = EMindGUI.BeginScrollView(new Rect(0,0,WinSize.x,WinSize.y),ConfigScroll,new Rect(0,0,WinSize.x-16,scrllbarheight),false,true);

			/*if (CurInterfaceStyle==InterfaceStyle.Flat)
			GUI.Box(new Rect(0,0,WinSize.x,100+BaseSettingsHeight),"");
			else
			GUI.Box(new Rect(0,0,WinSize.x,100+BaseSettingsHeight),"","button");
			GUI.skin.label.fontSize = 18;
			//GUI.Label(new Rect(15,30,200,40),"Shader Name: ");
			
			SSIO.ShaderName.Text = GUI.TextField(new Rect(15,30,WinSize.x-30-16,25),SSIO.ShaderName.Text,ShaderUtil.TextField18);

			//BaseSettingsHeight = 72;
			
			if (CurInterfaceStyle==InterfaceStyle.Flat)
			GUI.Box(new Rect(15,75,WinSize.x-30-16,BaseSettingsHeight-15),"");
			else
			GUI.Box(new Rect(15,75,WinSize.x-30-16,BaseSettingsHeight-15),"","button");

			SSIO.TechLOD.NoInputs = true;
			SSIO.TechLOD.Range0 = 0;
			SSIO.TechLOD.Range1 = 1000;
			SSIO.TechLOD.Float = Mathf.Round(SSIO.TechLOD.Float);
			SSIO.TechLOD.LabelOffset = 30;
			SSIO.TechLOD.Draw(new Rect(24+(230)+(200)+21,75+8,220-16,20),"LOD: ");
			
			
			SSIO.TechExcludeDX9.Draw(new Rect(24+(230)+(200)+21,75+8+24,220-16,20),"Exclude DX9: ");
			
			//TechFallback
			GUI.Label(new Rect(24+36,75+8,220-16,20),"Queue:");
			GUI.enabled = !SSIO.TechQueueAuto.On;
			SSIO.TechQueue.Type=EditorGUI.Popup(new Rect(24+36+45,75+8,220-16-45-32,20),SSIO.TechQueue.Type,SSIO.TechQueue.Names,EMindGUI.EditorPopup);
			GUI.enabled = true;
			SSIO.TechQueueAuto.Draw(new Rect(24+36 + (220-16),75+8,220-16,20),"");
			
			
			if (SSIO.TechReplacementAuto.On&&(SSIO.TechReplacement.Type != SSIO.TechQueue.Type)){
				SSIO.TechReplacement.Type = SSIO.TechQueue.Type;
				GUI.changed = true;
			}
			
			//TechFallback
			GUI.Label(new Rect(24+1,75+8+24,220-16,20),"Render Type:");
			GUI.enabled = !SSIO.TechReplacementAuto.On;
			SSIO.TechReplacement.Type=EditorGUI.Popup(new Rect(24+36+45,75+8+24,220-16-45-32,20),SSIO.TechReplacement.Type,SSIO.TechReplacement.Names,EMindGUI.EditorPopup);
			GUI.enabled = true;
			SSIO.TechReplacementAuto.Draw(new Rect(24+36 + (220-16),75+8+24,220-16,20),"");
			
			GUI.Label(new Rect(24+(240),75+8,220-16,20),"Color Space:");
			SSIO.MiscColorSpace.Type=EditorGUI.Popup(new Rect(24+(240)+74,75+8,220-16-45-32,20),SSIO.MiscColorSpace.Type,SSIO.MiscColorSpace.Names,EMindGUI.EditorPopup);
			
			//SSIO.TechQueue.Draw(new Rect(WinSize.x-220,30+25,220-16,20),"Queue: ");
			GUI.Label(new Rect(24+(240)+21,75+8+24,220-16,20),"Fallback:");
			if (SSIO.TechFallback.CodeNames[SSIO.TechFallback.Type]!="Custom"){
				SSIO.TechFallback.Type = EditorGUI.Popup(new Rect(24+(240)+74,75+8+24,220-16-45-32,20),SSIO.TechFallback.Type,SSIO.TechFallback.Names,EMindGUI.EditorPopup);
			}else{
				SSIO.TechFallback.Type = EditorGUI.Popup(new Rect(24+(240)+74,75+8+24,20,20),SSIO.TechFallback.Type,SSIO.TechFallback.Names,EMindGUI.EditorPopup);
				SSIO.TechCustomFallback.Text = GUI.TextField(new Rect(24+(240)+74+20,75+8+24,220-16-45-32,20),SSIO.TechCustomFallback.Text);
			}*/
			
			
			BaseSettingsHeight = -50;
			
			int X = 15;
			int i = -1;
			GUIStyle style = GUI.skin.GetStyle("ButtonLeft");
			int MoveID = -1;
			int DeleteID = -1;
			int MoveDir = 0;
			foreach (ShaderPass SPd in OpenShader.ShaderPasses){
				i++;
				if (ShaderLayerTab != i){
					//if (GUI.Button(new Rect(X,75+BaseSettingsHeight,100,25),new GUIContent(SPd.Name.Text,SPd.Visible.On?(SPd.ShellsOn.On?IconShell:IconBase):CrossRed),style)){
					if (GUI.Button(new Rect(X,75+BaseSettingsHeight,100,25),new GUIContent(SPd.Name.Text,SPd.Visible.On?(IconBase):CrossRed),style)){
							ShaderLayerTab = i;
							ChangeShaderLayerTab = true;
					}
				}
				else{
					//GUI.Toggle(new Rect(X,75+BaseSettingsHeight,100,25),true,new GUIContent(SPd.Name.Text,SPd.Visible.On?(SPd.ShellsOn.On?IconShell:IconBase):CrossRed),style);
					GUI.Toggle(new Rect(X,75+BaseSettingsHeight,100,25),true,new GUIContent(SPd.Name.Text,SPd.Visible.On?(IconBase):CrossRed),style);
				}
				if (i==ShaderLayerTab){
					GUI.enabled = i>0;
					if (GUI.Button(new Rect(X+100-50,60+BaseSettingsHeight,25,15),LeftArrow)){
						MoveID = i;
						MoveDir = -1;
					}
					GUI.enabled = i<SSIO.ShaderPasses.Count-1;
					if (GUI.Button(new Rect(X+100-25,60+BaseSettingsHeight,25,15),RightArrow)){
						MoveID = i;
						MoveDir = 1;
					}
					GUI.enabled = SSIO.ShaderPasses.Count>1;
					if (GUI.Button(new Rect(X+100-75,60+BaseSettingsHeight,25,15),CrossRed)){
						if (EditorUtility.DisplayDialog("Delete Pass \""+SPd.Name.Text+"\"?", "Are you sure you want to delete this entire pass? It can't be undone!", "Yes", "No"))
						DeleteID = i;
					}
					//GUI.enabled = true;
					if (GUI.Button(new Rect(X,60+BaseSettingsHeight,25,15),SPd.Visible.On?EyeOpen:EyeClose)){
						SPd.Visible.On = !SPd.Visible.On;
					}
					if (SSIO.ShaderPasses.Count==1)
					SPd.Visible.On = true;
				}
				GUI.enabled = true;
				X += 100;
				style = GUI.skin.GetStyle("ButtonMid");
			}
			
			if (MoveID!=-1){
				UnityEditor.Undo.RegisterCompleteObjectUndo((UnityEngine.Object)OpenShader,"Move Pass");
				ShaderUtil.MoveItem(ref SSIO.ShaderPasses,MoveID,MoveID+MoveDir);
				ShaderLayerTab = SSIO.ShaderPasses.IndexOf(SP);
				ChangeShaderLayerTab = true;
			}
			if (DeleteID!=-1){
				UnityEditor.Undo.RegisterCompleteObjectUndo((UnityEngine.Object)OpenShader,"Delete Pass");
				SSIO.ShaderPasses.RemoveAt(DeleteID);
			}
			Color oldg = GUI.contentColor;
			GUI.contentColor = EMindGUI.BaseInterfaceColorComp2;
			if (GUI.Button(new Rect(X,75+BaseSettingsHeight,25,25),new GUIContent("",Plus),GUI.skin.GetStyle("ButtonRight"))){
				UnityEditor.Undo.RegisterCompleteObjectUndo((UnityEngine.Object)OpenShader,"Add Pass");
				ShaderPass SPn = ScriptableObject.CreateInstance<ShaderPass>();
				SPn.Name.Text = "Pass "+(OpenShader.ShaderPasses.Count+1).ToString();
				OpenShader.ShaderPasses.Add(SPn);
			}
			GUI.contentColor = oldg;
			BaseSettingsHeight = -66;
			//int h = 575;
			
			//int SettingsBarWidth = 280;
			//GUI.Box(new Rect(15,115+BaseSettingsHeight,WinSize.x-30-16,h+38),"","window");
			GUI.Box(new Rect(15,115+BaseSettingsHeight,WinSize.x-30-16,h+25),"","window");
			//GUI.skin.label.fontSize = 18;
			//GUI.Label(new Rect(25,132+BaseSettingsHeight,WinSize.x-25-15,25),"Pass Group Name: ");
			//TextFieldStyle.fontSize = 18;
			
			//SP.Name.Text = GUI.TextField(new Rect(15,129+BaseSettingsHeight,WinSize.x-30-15,25),SP.Name.Text,ShaderUtil.TextField18);
			BaseSettingsHeight-=(129-115);
			SP.Name.Text = GUI.TextField(new Rect(16,129+BaseSettingsHeight,WinSize.x-30-16,25),SP.Name.Text,ShaderUtil.TextField18);
			
			//Shader Surfaces
			int y = 31;
			int width = (int)(WinSize.x-30-16);
			//int width = (int)(WinSize.x-30-16);
			//if (SP.selectedIngredient!=null)
			
				width-=SettingsBarWidth;//+1;
			
			int ingredwidth = width;
			
			GUIStyle surfaceButtonStyle = EMindGUI.SCSkin.window;//new GUIStayle(EMindGUI.SCSkin.window);
			//surfaceButtonStyle.fontSize = EMindGUI.GetFontSize(12);
			
			RectOffset oldpadding = surfaceButtonStyle.padding;
			//surfaceButtonStyle.padding = new RectOffset(4,4,0,0);
			int selectedY = 0;
			int selectedX = 0;
			
			Color asdasd = GUI.skin.label.normal.textColor;
			GUI.skin.label.normal.textColor = EMindGUI.BaseInterfaceColor;
			
			//EMindGUI.Label(new Rect(16,153+BaseSettingsHeight+y-32,width,32),"Surface",24);
			EditorGUI.LabelField(new Rect(16,153+BaseSettingsHeight+y-6-32,width+2,2),"",GUI.skin.horizontalSlider);
			
			EMindGUI.Label(new Rect(15,153+BaseSettingsHeight+y-24,width,32),"Surface",24);
			GUI.skin.label.normal.textColor = asdasd;
			///EditorGUI.LabelField(new Rect(16,153+BaseSettingsHeight+y-11,width,2),"",GUI.skin.horizontalSlider);
			
			bool Hit = false;
			int x = 0;
			//int lastusedx = 0;
			bool Nors = false;
			bool POM = false;
			if (SP.selectedIngredient!=null&&(Event.current.type!=EventType.Repaint&&Event.current.type!=EventType.Layout))
				UnityEditor.Undo.RecordObjects(SP.selectedIngredient.GetSerializableStuff(),"Change Ingredient");
			foreach(ShaderIngredient ingredient in SP.Surfaces){
				//UnityEditor.Undo.RecordObject((UnityEngine.Object)ingredient,"Change Ingredient");
				//foreach(UnityEngine.Object asd in EditorUtility.CollectDeepHierarchy(new UnityEngine.Object[]{ingredient}))
				//	Debug.Log(asd);
				
				ShaderSurface surface = ingredient as ShaderSurface;
				if (!SP.dragSurface||!SP.hasDraggedSurface||ingredient!=SP.selectedIngredient){
					DrawSurfaceRowThingy(SP,ingredient,surfaceButtonStyle,oldpadding,ingredwidth,x,y,surface,null);
				}
				else{
					selectedY = y;
					selectedX = x;
					if (ingredient==SP.selectedIngredient){
						Color bgc = GUI.backgroundColor;
						GUI.backgroundColor = EMindGUI.BaseInterfaceColor*EMindGUI.BaseInterfaceColor;
						GUI.Box(new Rect(x+16,153+BaseSettingsHeight+y,ingredwidth-1,31),"",GUI.skin.GetStyle("SelectionRect"));
						GUI.backgroundColor = bgc;
					}
				}
				//lastusedx = x;
				if (Event.current.type==EventType.MouseDown&&new Rect(16,153+BaseSettingsHeight+y,ingredwidth,32).Contains(Event.current.mousePosition))
					Hit = true;
				
				//if (ingredient.ShaderSurfaceComponent!=ShaderSurfaceComponent.Mask)
				//	y+=31;
					y+=ingredient.Height-1;
				//else
				//	y+=15;
				if (surface!=null){
					if ((surface.ShaderSurfaceComponent==ShaderSurfaceComponent.Normal&&!Nors)||(surface.ShaderSurfaceComponent==ShaderSurfaceComponent.Depth&&!POM)){
						if (surface.ShaderSurfaceComponent==ShaderSurfaceComponent.Normal)
							Nors = true;
						else
							POM = true;
						x += 31;
						ingredwidth -= 31;
					}
				}
			}
			GUI.contentColor = EMindGUI.BaseInterfaceColorComp2;
			//if (GUI.Button(new Rect(lastusedx+15,153+BaseSettingsHeight+y,66,16),ShaderSandwich.Plus,ShaderUtil.BoxMiddle20)){
			if (GUI.Button(new Rect(width-66+15,153+BaseSettingsHeight+y,66,16),Plus,ShaderUtil.BoxMiddle20)){
				GenericMenu toolsMenu = new GenericMenu();
				foreach(KeyValuePair<string,Type> Ty in ShaderSurfaces){
					toolsMenu.AddItem(new GUIContent(Ty.Key), false, SP.AddSurface,Ty.Value);
				}
				toolsMenu.DropDown(new Rect(width-66+15,153+BaseSettingsHeight+y,66,16));//EMindGUI.Rect2(RectDir.Right,rect.width,YOffset-20,20,20));
				//EditorGUIUtility.ExitGUI();
			}
			int nexty = y;
			if ((y+32)>(h/2))
				nexty+=31+31;
			else 
				nexty = (h/2);
			if (EMindGUI.MouseDownIn(new Rect(0,153+BaseSettingsHeight+y+16,width-66+15,nexty-y-32-16))&&Event.current.button == 1){
				GenericMenu toolsMenu = new GenericMenu();
				foreach(KeyValuePair<string,Type> Ty in ShaderSurfaces){
					toolsMenu.AddItem(new GUIContent(Ty.Key), false, SP.AddSurface,Ty.Value);
				}
				toolsMenu.DropDown(new Rect(Event.current.mousePosition.x,Event.current.mousePosition.y,66,0));//EMindGUI.Rect2(RectDir.Right,rect.width,YOffset-20,20,20));
				//EditorGUIUtility.ExitGUI();
			}
			
			GUI.contentColor = EMindGUI.BaseInterfaceColorComp;
			//if (GUI.Button(new Rect(lastusedx+15,153+BaseSettingsHeight+y,66,16),ShaderSandwich.Plus,ShaderUtil.BoxMiddle20)){
			Rect Bin1Yay = new Rect(16,153+BaseSettingsHeight+y,66,16);
			if (GUI.Button(Bin1Yay,CurrentBin)){
				if (SP.selectedIngredient!=null&&!SP.selectedIngredient.Locked&&SP.selectedIngredient as ShaderSurface!=null)
					SP.DeleteIngredient(SP.selectedIngredient);
			}
			
			GUI.contentColor = oldg;
			//if (GUI.Button(new Rect(lastusedx+15+66,153+BaseSettingsHeight+y,66,16),ShaderSandwich.IconMask,ShaderUtil.BoxMiddle20)){
			if (GUI.Button(new Rect(width-66-65+15,153+BaseSettingsHeight+y,66,16),IconMask,ShaderUtil.BoxMiddle20)){
				GenericMenu toolsMenu = new GenericMenu();
				foreach(ShaderLayerList SLL in OpenShader.ShaderLayersMasks){
					toolsMenu.AddItem(new GUIContent(SLL.Name.Text), false, SP.AddSurfaceMaskUpdate,SLL);
				}
				toolsMenu.DropDown(new Rect(width-66-65+15,153+BaseSettingsHeight+y,66,16));//EMindGUI.Rect2(RectDir.Right,rect.width,YOffset-20,20,20));
				//EditorGUIUtility.ExitGUI();
			}
			ingredwidth = width;
				x = 0;
			//if ((y+32)>(h/2))
			//	y+=31+31;
			//else 
			//	y = (h/2);
			y = nexty;
			//GUI.Box(new Rect(16,153+BaseSettingsHeight+y-2,(WinSize.x-30-16)-264+1,2),"");
			GUI.skin.label.normal.textColor = EMindGUI.BaseInterfaceColor;
			EMindGUI.Label(new Rect(17,153+BaseSettingsHeight+y-24,width,32),"Mesh",24);
			GUI.skin.label.normal.textColor = asdasd;
			EditorGUI.LabelField(new Rect(15,153+BaseSettingsHeight+y-32-6,width+2,2),"",GUI.skin.horizontalSlider);
			//EditorGUI.LabelField(new Rect(16,153+BaseSettingsHeight+y-11,width,2),"",GUI.skin.horizontalSlider);
			foreach(ShaderIngredient ingredient in SP.GeometryModifiers){
				//UnityEditor.Undo.RecordObject((UnityEngine.Object)ingredient,"Change Ingredient");
				//foreach(UnityEngine.Object asd in EditorUtility.CollectDeepHierarchy(new UnityEngine.Object[]{ingredient}))
				//	Debug.Log(asd);
				//UnityEditor.Undo.RecordObjects(ingredient.GetSerializableStuff(),"Change Ingredient");
				if (!SP.dragSurface||!SP.hasDraggedSurface||ingredient!=SP.selectedIngredient){
					DrawSurfaceRowThingy(SP,ingredient,surfaceButtonStyle,oldpadding,ingredwidth,x,y,null,ingredient as ShaderGeometryModifier);
				}
				else{
					selectedY = y;
					selectedX = x;
					if (ingredient==SP.selectedIngredient){
						Color bgc = GUI.backgroundColor;
						GUI.backgroundColor = EMindGUI.BaseInterfaceColor*EMindGUI.BaseInterfaceColor;
						GUI.Box(new Rect(x+16,153+BaseSettingsHeight+y,ingredwidth-1,31),"",GUI.skin.GetStyle("SelectionRect"));
						GUI.backgroundColor = bgc;
					}
				}
				
				if (Event.current.type==EventType.MouseDown&&new Rect(16,153+BaseSettingsHeight+y,ingredwidth,32).Contains(Event.current.mousePosition))
					Hit = true;
				y+=31;
			}
			ShaderSurface selectedS = SP.selectedIngredient as ShaderSurface;
			ShaderGeometryModifier selectedG = SP.selectedIngredient as ShaderGeometryModifier;
			
			GUI.contentColor = EMindGUI.BaseInterfaceColorComp2;
			if (GUI.Button(new Rect(width-66+15,153+BaseSettingsHeight+y,66,16),ShaderSandwich.Plus,ShaderUtil.BoxMiddle20)){
				GenericMenu toolsMenu = new GenericMenu();
				foreach(KeyValuePair<string,Type> Ty in ShaderSandwich.ShaderGeometryModifiers){
					if (!Ty.Key.StartsWith("Hidden"))
						toolsMenu.AddItem(new GUIContent(Ty.Key), false, SP.AddGeometryModifier,Ty.Value);
				}
				toolsMenu.DropDown(new Rect(width-66+15,153+BaseSettingsHeight+y,66,16));//EMindGUI.Rect2(RectDir.Right,rect.width,YOffset-20,20,20));
				//EditorGUIUtility.ExitGUI();
			}
			
			GUI.contentColor = EMindGUI.BaseInterfaceColorComp;
			//if (GUI.Button(new Rect(lastusedx+15,153+BaseSettingsHeight+y,66,16),ShaderSandwich.Plus,ShaderUtil.BoxMiddle20)){
			Rect Bin2Yay = new Rect(16,153+BaseSettingsHeight+y,66,16);
			if (GUI.Button(Bin2Yay,CurrentBin)){
				if (SP.selectedIngredient!=null&&!SP.selectedIngredient.Locked&&SP.selectedIngredient as ShaderGeometryModifier!=null)
					SP.DeleteIngredient(SP.selectedIngredient);
			}
			
			GUI.contentColor = oldg;
			
			if (EMindGUI.MouseDownIn(new Rect(0,153+BaseSettingsHeight+y+16,width-66+15,10000))&&Event.current.button == 1){
				GenericMenu toolsMenu = new GenericMenu();
				foreach(KeyValuePair<string,Type> Ty in ShaderSandwich.ShaderGeometryModifiers){
					if (!Ty.Key.StartsWith("Hidden"))
						toolsMenu.AddItem(new GUIContent(Ty.Key), false, SP.AddGeometryModifier,Ty.Value);
				}
				toolsMenu.DropDown(new Rect(Event.current.mousePosition.x,Event.current.mousePosition.y,66,0));//EMindGUI.Rect2(RectDir.Right,rect.width,YOffset-20,20,20));
				//EditorGUIUtility.ExitGUI();
			}
			

			if (SP.Surfaces.Contains(SP.selectedIngredient)){
				if (SP.dragSurface&&Event.current.mousePosition.y-SP.dragSurfaceOffset.y>16&&(SP.Surfaces.IndexOf(SP.selectedIngredient)>-1&&SP.Surfaces.IndexOf(selectedS)+1<SP.Surfaces.Count)){
					ShaderUtil.MoveItem(ref SP.Surfaces, SP.Surfaces.IndexOf(SP.selectedIngredient),SP.Surfaces.IndexOf(SP.selectedIngredient)+1);
					//if (SP.Surfaces[SP.Surfaces.IndexOf(selectedS)-1].ShaderSurfaceComponent==ShaderSurfaceComponent.Mask)
					//	SP.dragSurfaceOffset.y+=15;
					//else
					//	SP.dragSurfaceOffset.y+=31;
					SP.dragSurfaceOffset.y+=SP.Surfaces[SP.Surfaces.IndexOf(SP.selectedIngredient)-1].Height-1;
				}
				if (SP.dragSurface&&Event.current.mousePosition.y-SP.dragSurfaceOffset.y<-16&&(SP.Surfaces.IndexOf(SP.selectedIngredient)>-1&&SP.Surfaces.IndexOf(SP.selectedIngredient)-1>=0)){
					ShaderUtil.MoveItem(ref SP.Surfaces, SP.Surfaces.IndexOf(SP.selectedIngredient),SP.Surfaces.IndexOf(SP.selectedIngredient)-1);
					//SP.dragSurfaceOffset.y-=31;
					SP.dragSurfaceOffset.y-=SP.Surfaces[SP.Surfaces.IndexOf(SP.selectedIngredient)+1].Height-1;
				}
			}
			else if (SP.GeometryModifiers.Contains(SP.selectedIngredient)){
				if (SP.dragSurface&&Event.current.mousePosition.y-SP.dragSurfaceOffset.y>16&&(SP.GeometryModifiers.IndexOf(SP.selectedIngredient)>-1&&SP.GeometryModifiers.IndexOf(SP.selectedIngredient)+1<SP.GeometryModifiers.Count)){
					ShaderUtil.MoveItem(ref SP.GeometryModifiers, SP.GeometryModifiers.IndexOf(SP.selectedIngredient),SP.GeometryModifiers.IndexOf(SP.selectedIngredient)+1);
					SP.dragSurfaceOffset.y+=31;
				}
				if (SP.dragSurface&&Event.current.mousePosition.y-SP.dragSurfaceOffset.y<-16&&(SP.GeometryModifiers.IndexOf(SP.selectedIngredient)>-1&&SP.GeometryModifiers.IndexOf(SP.selectedIngredient)-1>=0&&!SP.GeometryModifiers[SP.GeometryModifiers.IndexOf(SP.selectedIngredient)-1].Locked)){
					ShaderUtil.MoveItem(ref SP.GeometryModifiers, SP.GeometryModifiers.IndexOf(SP.selectedIngredient),SP.GeometryModifiers.IndexOf(SP.selectedIngredient)-1);
					SP.dragSurfaceOffset.y-=31;
				}
			}
			surfaceButtonStyle.padding = oldpadding;
			
			
			if (Event.current.type==EventType.MouseDown){
				if (SP.dragSurface == false&&!new Rect((WinSize.x-30)-SettingsBarWidth,15+153+BaseSettingsHeight,SettingsBarWidth,h).Contains(Event.current.mousePosition)&&!Hit){
					SP.selectedIngredient = null;
					EMindGUI.Defocus();
				}
			}
			if (SP.dragSurface&&Event.current.type==EventType.MouseUp){
				if (Bin1Yay.Contains(Event.current.mousePosition)||Bin2Yay.Contains(Event.current.mousePosition)){
					SP.DeleteIngredient(SP.selectedIngredient);
				}
				SP.dragSurface = false;
				SP.hasDraggedSurface = false;
				GUI.changed = true;
			}
			
			if (SP.selectedIngredient!=null){
				//GUI.Toggle(new Rect((WinSize.x-30)-264,153+BaseSettingsHeight,264,h),true,SP.selectedIngredient.Name + " : Settings",EMindGUI.SCSkin.window);
				//GUI.Box(new Rect((WinSize.x-30)-264,153+BaseSettingsHeight,264,h),SP.selectedIngredient.Name + " : Settings",EMindGUI.SCSkin.window);
				EMindGUI.SetLabelSize(12);
				if (Event.current.type==EventType.Repaint)
					EMindGUI.SCSkin.window.Draw(new Rect((WinSize.x-30)-SettingsBarWidth,153+BaseSettingsHeight,SettingsBarWidth,h),SP.selectedIngredient.Name + " : Settings",false,true,true,true);
				int SettingsOffset = 0;
				EMindGUI.BeginGroup(new Rect((WinSize.x-30)-SettingsBarWidth,15+153+BaseSettingsHeight+SettingsOffset,SettingsBarWidth,h));
				int xxxxxxx = 0;
				if (selectedS!=null&&selectedS.CustomAmbientEnabled){
					EMindGUI.MakeTooltip(0,new Rect(SettingsBarWidth-32-xxxxxxx,0,32,32),"Custom Indirect Lighting? - Exposes another layer channel that can be used to customize indirect lighting (such as with Adjustment layers).");
					//selectedS.UseCustomAmbient.On = GUI.Toggle(new Rect(SettingsBarWidth-32-xxxxxxx,0,32,32),selectedS.UseCustomAmbient.On,new GUIContent(selectedS.UseCustomAmbient.On?LightBulbOn:LightBulbOff),ShaderUtil.Button2Border);
					selectedS.UseCustomAmbient.On = GUI.Toggle(new Rect(SettingsBarWidth-32-xxxxxxx,0,32,32),selectedS.UseCustomAmbient.On,new GUIContent(selectedS.UseCustomAmbient.On?EditorGUIUtility.ObjectContent(null, typeof(LightingDataAsset)).image:EditorGUIUtility.ObjectContent(null, typeof(LightingDataAsset)).image),ShaderUtil.Button2Border);
					xxxxxxx+=32;
				}
				if (selectedS!=null&&selectedS.CustomLightingEnabled){
					EMindGUI.MakeTooltip(0,new Rect(SettingsBarWidth-32-xxxxxxx,0,32,32),"Custom Direct Lighting? - Exposes another layer channel that can be used to customize direct lighting (such as with Adjustment layers).");
					selectedS.UseCustomLighting.On = GUI.Toggle(new Rect(SettingsBarWidth-32-xxxxxxx,0,32,32),selectedS.UseCustomLighting.On,new GUIContent(selectedS.UseCustomLighting.On?EditorGUIUtility.ObjectContent(null, typeof(Light)).image:EditorGUIUtility.ObjectContent(null, typeof(Light)).image),ShaderUtil.Button2Border);
					xxxxxxx+=32;
				}
				SP.selectedIngredient.RenderUI(SettingsBarWidth);
				EMindGUI.EndGroup();
				SettingsOffset += h-100;
				if (selectedS!=null){
					if (selectedS.ShaderSurfaceComponent==ShaderSurfaceComponent.Depth){
						selectedS.CompareDepth.Draw(new Rect((WinSize.x-30)-SettingsBarWidth,153+BaseSettingsHeight+h-25-70,SettingsBarWidth,20),"Surface Z Test: ");
					}
					if (selectedS.ShaderSurfaceComponent==ShaderSurfaceComponent.Normal){
						selectedS.NormalizeMix.Draw(new Rect((WinSize.x-30)-SettingsBarWidth,153+BaseSettingsHeight+h-25-70,SettingsBarWidth,20),"Normalize After Mix: ");
					}
					if (selectedS.ForceAlphaGenerate)
						EMindGUI.BeginGroup(new Rect((WinSize.x-30)-SettingsBarWidth,153+BaseSettingsHeight+SettingsOffset,SettingsBarWidth,100));
					else
						EMindGUI.BeginGroup(new Rect((WinSize.x-30)-SettingsBarWidth,153+BaseSettingsHeight+SettingsOffset+25,SettingsBarWidth,100-25));
						if (Event.current.type==EventType.Repaint)
							EMindGUI.SCSkin.window.Draw(new Rect(0,0,SettingsBarWidth,100),"Blending",false,true,true,true);
						
						//selectedS.MixType.Draw(new Rect(0,20,264,20),"Blend Mode:");
						ShaderUtil.DrawCoolSlider(new Rect(0,15,SettingsBarWidth,8),selectedS.MixAmount,0,true);
						GUI.Label(new Rect(0,15+8+5,SettingsBarWidth,20),"Mix Type");
						selectedS.MixType.Type = EditorGUI.Popup(new Rect(120,15+8+5,SettingsBarWidth-120,20),selectedS.MixType.Type,selectedS.MixType.Names,EMindGUI.EditorPopup);
						//selectedS.UseAlpha.Draw(new Rect(0,15+8+5+25,SettingsBarWidth,20),"Use Alpha");
						selectedS.AlphaBlendMode.Type = EditorGUI.Popup(new Rect(120,15+8+5+25,SettingsBarWidth-120,20),selectedS.AlphaBlendMode.Type,selectedS.AlphaBlendMode.Names,EMindGUI.EditorPopup);
						GUI.Label(new Rect(0,15+8+5+25,SettingsBarWidth,20),"Alpha Mode");
						if (selectedS.ForceAlphaGenerate)
							selectedS.UseAlpha.Draw(new Rect(0,15+8+5+25+25,SettingsBarWidth,20),"Transparent");
					EMindGUI.EndGroup();
					SettingsOffset += 100;
				}
			}else{
				if (Event.current.type==EventType.Repaint)
					EMindGUI.SCSkin.window.Draw(new Rect((WinSize.x-30)-SettingsBarWidth,153+BaseSettingsHeight,SettingsBarWidth,h),"No Ingredient Selected",false,true,true,true);
			}
			surfaceButtonStyle.padding = oldpadding;
			
			if (SP.dragSurface&&SP.hasDraggedSurface){
				if (Bin1Yay.Contains(Event.current.mousePosition)||Bin2Yay.Contains(Event.current.mousePosition))
					GUI.enabled = false;
				DrawSurfaceRowThingy(SP,SP.selectedIngredient,surfaceButtonStyle,oldpadding,ingredwidth,selectedX,selectedY,selectedS,selectedG);
				GUI.enabled = true;
			}
			
			EMindGUI.EndScrollView();
			EMindGUI.VerticalScrollbar(new Rect(0,0,WinSize.x,WinSize.y),ConfigScroll,new Rect(0,0,WinSize.x-16,scrllbarheight),true);
			
			EMindGUI.EndGroup();	
			
			ChangeSaveTemp(null);
			if (OldShaderLayerTab<0&&!ChangeShaderLayerTab)
			ShaderLayerTab = OldShaderLayerTab;
		}
	}
	static public bool GUIMouseDown = false;
	static public bool GUIMouseUp = false;
	static public bool GUIMouseHold = false;
	
	public List<string> GUIStartShortName = new List<string>();
	public List<string> GUIStartName = new List<string>();
	public List<string> GUIStartFileName = new List<string>();
	public List<string> GUIStartFilePath = new List<string>();
	public int LastRefresh = -1;
	public void RefreshPreviousFiles(){
		GUIStartShortName.Clear();
		GUIStartName.Clear();
		GUIStartFileName.Clear();
		GUIStartFilePath.Clear();
		
		foreach(string File in SSSettings.RecentFilesList){
			if (File!=""&&File!=null){
				if (!System.IO.Path.IsPathRooted(File)){
					Shader TheActualFile = null;
					//try{
					TheActualFile = ((Shader)AssetDatabase.LoadAssetAtPath(File,typeof(Shader)));
					//}catch{};
					if (TheActualFile!=null){
						FileInfo file = new FileInfo(Application.dataPath+File);
						//Debug.Log();
						string ShortPath = TheActualFile.name.Substring(Math.Max(0,TheActualFile.name.LastIndexOf("/")+1));
						GUIStartName.Add(TheActualFile.name);
						GUIStartFileName.Add(ShortPath);
						GUIStartFilePath.Add(File);
						GUIStartShortName.Add(file.Name);
					}
				}else{
					FileInfo file = new FileInfo(Application.dataPath+File);
					string ShortPath = file.Name;//TheActualFile.name.Substring(Math.Max(0,TheActualFile.name.LastIndexOf("/")+1));
					GUIStartName.Add(file.Name);
					GUIStartFileName.Add(ShortPath);
					GUIStartFilePath.Add(File);
					GUIStartShortName.Add(file.Name);
				}
			}
		}
	}
	public Vector2 LogoPosition = new Vector2(0,0);
	public Vector2 LogoVelocity = new Vector2(0,0);
	void GUIStart(Vector2 WinSize,Vector2 Position){
		if (www==null)
			getAnswerFromOutside("http://electronic-mind.org/ShaderSandwich/MOTD.txt");
		if (www!=null){
			if (www.isDone==true)
				MOTD = www.text;
			else if (EditorApplication.timeSinceStartup-wwwStart>10)
				MOTD =  "Unable to connect to www.Electronic-Mind.org for the message of the day/year, sorry!";
		}
		
		ShaderVarEditing = null;
		EMindGUI.BeginGroup(new Rect(Position.x,Position.y,WinSize.x,WinSize.y));
		float BannerHeight = Mathf.Round((144f/1006f)*WinSize.x);
		if (CurInterfaceStyle==InterfaceStyle.Modern){
			if (EMindGUI.GetVal(InterfaceColor)<0.5)
				GUI.DrawTexture(new Rect(00,0,WinSize.x,BannerHeight), Banner, ScaleMode.ScaleToFit);
			else
				GUI.DrawTexture(new Rect(00,0,WinSize.x,BannerHeight), BannerLight, ScaleMode.ScaleToFit);
			Matrix4x4 curMatrix = GUI.matrix;
			
			if (EMindGUI.MouseDownIn(new Rect(LogoPosition.x,LogoPosition.y,BannerHeight*(184f/144f),BannerHeight)))
				LogoVelocity += new Vector2((UnityEngine.Random.value-0.5f)*9f,(UnityEngine.Random.value-0.5f)*9f);
			
			if (Event.current.type==EventType.Repaint){
				Vector2 newvel = (new Vector2(0,0)-LogoPosition);
				LogoVelocity = Vector2.Lerp(LogoVelocity,newvel,(newvel.y<0)?0.02f:0.015f);
				LogoPosition += LogoVelocity;
			}
			//GUIUtility.RotateAroundPivot(rotAngle, pivotPoint);
			GUI.DrawTexture(new Rect(LogoPosition.x,LogoPosition.y,BannerHeight*(184f/144f),BannerHeight), Logo, ScaleMode.ScaleToFit);
			EMindGUI.MakeTooltip(0,new Rect(LogoPosition.x,LogoPosition.y,BannerHeight*(184f/144f),BannerHeight),"Hai! :)");
			GUI.matrix = curMatrix;
		}
		else{
			GUI.DrawTexture(new Rect(00,20,WinSize.x,BannerHeight), Banner, ScaleMode.ScaleToFit);
			BannerHeight+=20;
			BannerHeight+=24;
		}
		BannerHeight+=8;
		
		if (CurInterfaceStyle==InterfaceStyle.Modern){
			
			
			int MenuWidth = 150;
			int MenuItemWidth = 75;
			int MenuCenter = (int)(WinSize.x/2);
			
			ShaderUtil.ButtonLeft.fontSize = (int)((float)ShaderSandwich.Instance.InterfaceSize*1.4f);
			float RecentlyOpenedSize = Mathf.Round(WinSize.x*0.5f+MenuWidth/2);//-300;
			GUI.Box(new Rect(0,BannerHeight,RecentlyOpenedSize,WinSize.y-BannerHeight),"");
			
			EMindGUI.SetLabelSize(14);
			int BoxHeight = ShaderUtil.ButtonLeft.fontSize+GUI.skin.label.fontSize+10;
			int Y = 0;
			
			//GUI.Box(new Rect(WinSize.x-(WinSize.x-RecentlyOpenedSize+1),BannerHeight+BoxHeight,WinSize.x-RecentlyOpenedSize+1,WinSize.y-BannerHeight-BoxHeight),MOTD,ShaderUtil.BoxUpperLeftRich);
			GUI.Box(new Rect(WinSize.x-(WinSize.x-RecentlyOpenedSize+1),BannerHeight+BoxHeight,WinSize.x-RecentlyOpenedSize+1,WinSize.y-BannerHeight-BoxHeight),"",ShaderUtil.BoxUpperLeftRich);
			EditorGUI.SelectableLabel(new Rect(WinSize.x-(WinSize.x-RecentlyOpenedSize+1),BannerHeight+BoxHeight,WinSize.x-RecentlyOpenedSize+1,WinSize.y-BannerHeight-BoxHeight),"",ShaderUtil.BoxUpperLeftRich);
			GUI.skin.label.richText = true;
			EditorGUI.SelectableLabel(new Rect(WinSize.x-(WinSize.x-RecentlyOpenedSize+1)+7,BannerHeight+BoxHeight+4,WinSize.x-RecentlyOpenedSize+1-14,WinSize.y-BannerHeight-BoxHeight),MOTD,GUI.skin.label);
			GUI.skin.label.richText = false;
			ShaderUtil.ButtonLeft.alignment = TextAnchor.MiddleLeft;
			//if (GUI.Button(new Rect(0,(BannerHeight)+Y,RecentlyOpenedSize,BoxHeight),"New Shader!",ShaderUtil.ButtonLeft )){
			//	NewAdvanced();
			//}
			GUI.Box(new Rect(-1,BannerHeight+Y,WinSize.x+2,BoxHeight),"","box");
			

			//MenuCenter = (int)((WinSize.x-300)/2);
			
			ShaderUtil.ButtonLeft.normal.textColor = Color.white;
			ShaderUtil.ButtonLeft.active.textColor = Color.white;
			ShaderUtil.ButtonLeft.hover.textColor = Color.white;
			Color oldg = GUI.contentColor;
			GUI.contentColor = EMindGUI.BaseInterfaceColorComp2;
			if (GUI.Button(new Rect(MenuCenter-MenuWidth-MenuItemWidth/2,BannerHeight+Y+5,MenuItemWidth,BoxHeight-10),new GUIContent("New!",Plus),ShaderUtil.ButtonLeft)){
				NewAdvanced();
			}
			GUI.contentColor = EMindGUI.BaseInterfaceColor;
			if (GUI.Button(new Rect(MenuCenter-MenuItemWidth/2,BannerHeight+Y+5,MenuItemWidth,BoxHeight-10),new GUIContent("Open",ReloadIcon),ShaderUtil.ButtonLeft)){
				if (Load())
					Goto(GUIType.Layers,ShaderTransition.TransDir.Forward);
			}
			GUI.contentColor = EMindGUI.BaseInterfaceColorComp;
			if (GUI.Button(new Rect(MenuCenter+MenuWidth-MenuItemWidth/2,BannerHeight+Y+5,MenuItemWidth,BoxHeight-10),new GUIContent("Help!",Warning),ShaderUtil.ButtonLeft)){
				OpenHelp();
			}
			GUI.contentColor = oldg;
			ShaderUtil.ButtonLeft.normal.textColor = EMindGUI.BaseInterfaceColor;
			ShaderUtil.ButtonLeft.active.textColor = EMindGUI.BaseInterfaceColor;
			ShaderUtil.ButtonLeft.hover.textColor = EMindGUI.BaseInterfaceColor;
			
			ShaderUtil.ButtonLeft.alignment = TextAnchor.UpperLeft;
						
			Y += BoxHeight;
			
			//if (GUI.Button(new Rect(0,(BannerHeight)+Y,RecentlyOpenedSize,BoxHeight),"Continue",ButtonStyle2 ))
			//Goto(GUIType.Layers,ShaderTransition.TransDir.Forward);
			
			//Y += BoxHeight;
			int Count = 0;
			if (Event.current.type==EventType.Repaint){
				LastRefresh-=1;
			}
			if (LastRefresh<0){
				//LastRefresh = 120;
				LastRefresh = 60;
				RefreshPreviousFiles();
			}
			for(int i = 0; i < GUIStartName.Count; i++){
				if (GUI.Button(new Rect(0,(BannerHeight)+Y,RecentlyOpenedSize,BoxHeight),GUIStartShortName[i],ShaderUtil.ButtonLeft )){
					if (Load(GUIStartFilePath[i]))
						Goto(GUIType.Layers,ShaderTransition.TransDir.Forward);
				}
				
				EMindGUI.Label(new Rect(10,(BannerHeight)+Y+BoxHeight-GUI.skin.label.fontSize-5,RecentlyOpenedSize,BoxHeight),GUIStartName[i]+" - "+GUIStartFileName[i],14);
				Y+=BoxHeight;
				Count++;
			}
			if (Count==0){
				GUI.Button(new Rect(0,(BannerHeight)+Y,RecentlyOpenedSize,BoxHeight),"No Recent Files!",ShaderUtil.ButtonLeft );
				
				EMindGUI.Label(new Rect(10,(BannerHeight)+Y+BoxHeight-GUI.skin.label.fontSize-5,RecentlyOpenedSize,BoxHeight),"Come on, get to it! :)",14);
			}
			if (CurInterfaceStyle!=InterfaceStyle.Modern)
				if (OpenShader!=null)
					if (GUI.Button( EMindGUI.Rect2(RectDir.Diag,WinSize.x,WinSize.y,100,30),"Layers",ShaderUtil.Button20))
						Goto(GUIType.Layers,ShaderTransition.TransDir.Forward);
		}
		else{
			int RecentlyOpenedSize = (int)(WinSize.x/3.5714f);//210;
			if (!SSSettings.HasAny())
				GUI.Box(EMindGUI.Rect2(RectDir.Bottom,0,WinSize.y,RecentlyOpenedSize,WinSize.y-(BannerHeight)),"Recently Opened\n__________________________\n - None, get to it already! :)",ShaderUtil.BoxUpperLeftRich);
			else{
				GUI.Box(EMindGUI.Rect2(RectDir.Bottom,0,WinSize.y,RecentlyOpenedSize,WinSize.y-(BannerHeight)),"Recently Opened\n__________________________",ShaderUtil.BoxUpperLeftRich);
				int Y = 0;
				foreach(string File in SSSettings.RecentFilesList){
					if (File!=""&&File!=null){
						Shader TheActualFile = null;
						//try{
							TheActualFile = ((Shader)AssetDatabase.LoadAssetAtPath(File,typeof(Shader)));
						//}catch{};
						if (TheActualFile!=null){
							FileInfo file = new FileInfo(Application.dataPath+File);
							if (GUI.Button(new Rect(10,(BannerHeight)+Y+30,RecentlyOpenedSize-10,20),TheActualFile.name+"("+file.Name+")",ShaderUtil.ButtonLeft )){
								if (Load(File))
									Goto(GUIType.Layers,ShaderTransition.TransDir.Forward);
							}
							Y+=20;
						}
					}
				}
				if (GUI.Button(EMindGUI.Rect2(RectDir.Bottom,0,WinSize.y,RecentlyOpenedSize,20),"Clear"))
				SSSettings.RecentFiles = null;
			}
			
			
			GUI.Box(EMindGUI.Rect2(RectDir.Diag,WinSize.x,WinSize.y,RecentlyOpenedSize,WinSize.y-(BannerHeight)),MOTD,ShaderUtil.BoxUpperLeftRich);
			
			float ButtonOrder = 0;
			int ButtonWidth = (int)((float)RecentlyOpenedSize*0.85f);
			if (OpenShader!=null)
			{
				if (GUI.Button( EMindGUI.Rect2(RectDir.MiddleTop,WinSize.x/2f,BannerHeight+ButtonOrder*28f,ButtonWidth,24),"Continue"))
				Goto(GUIType.Layers,ShaderTransition.TransDir.Forward);
				
				ButtonOrder+=1;
				
				if (GUI.Button( EMindGUI.Rect2(RectDir.MiddleTop,WinSize.x/2f,BannerHeight+ButtonOrder*28f,ButtonWidth,24),"Save As"))
				{
					SaveAs();
				}
				ButtonOrder+=2;
			}
			
			if(StartNewTrans.Get()!=0)
			{
				GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,StartNewTrans.Get());
				if (GUI.Button( EMindGUI.Rect2(RectDir.MiddleTop,WinSize.x/2f,BannerHeight+(StartNewTrans.Get()*28f)+ButtonOrder*28f,ButtonWidth/1.25f,24),"Start from Scratch!"))
				{
					NewAdvanced();
				}
				
				if (GUI.Button( EMindGUI.Rect2(RectDir.MiddleTop,WinSize.x/2f,BannerHeight+(StartNewTrans.Get()*56f)+ButtonOrder*28f,ButtonWidth/1.25f,24),"Start from a Preset!"))
				{
					NewSimple();
				}
				
				GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,1);
			}
			
			
			if (GUI.Button( EMindGUI.Rect2(RectDir.MiddleTop,WinSize.x/2f,BannerHeight+ButtonOrder*28f,ButtonWidth,24),"New Shader"))
			//NewAdvanced();
			StartNewTrans.Start(ShaderTransition.TransDir.Reverse);
			
			ButtonOrder+=1;
			if (GUI.Button( EMindGUI.Rect2(RectDir.MiddleTop,WinSize.x/2f,BannerHeight+ButtonOrder*28f+(StartNewTrans.Get()*56f),ButtonWidth,24),"Open Shader"))
			{
				if (Load())
					Goto(GUIType.Layers,ShaderTransition.TransDir.Forward);
			}
			
			ButtonOrder+=1;
			//GUI.Button( EMindGUI.Rect2(RectDir.MiddleTop,WinSize.x/2f,BannerHeight+ButtonOrder*28f+(StartNewTrans.Get()*56f),ButtonWidth,24),"Get Shaders");
			
			ButtonOrder=8;
			if (GUI.Button( EMindGUI.Rect2(RectDir.MiddleTop,WinSize.x/2f,BannerHeight+ButtonOrder*28f,ButtonWidth,24),"Help!")){
				OpenHelp();
			}
			//if (GUI.Button( EMindGUI.Rect2(RectDir.MiddleTop,WinSize.x/2f,BannerHeight+84f+(StartNewTrans.Get()*56f),150,24),"Temp Config"))
			//Goto(GUIType.Configure,ShaderTransition.TransDir.Forward);
			
			//GUIStyle ButtonStyle = new GUIStayle(GUI.skin.button);
			//ButtonStyle.fontSize = 30;
		}
		//if (GUI.Button( EMindGUI.Rect2(RectDir.Diag,WinSize.x,WinSize.y,210,50),"New Shader",ButtonStyle))
		//Goto(GUIType.Presets,ShaderTransition.TransDir.Forward);
		
		EMindGUI.EndGroup();	
	}
	public void Goto(GUIType GUI,ShaderTransition.TransDir dir)
	{
		//if (GUI==GUIType.Presets)
		//GUI = GUIType.Layers;
		GUITrans.Start(dir);
		GUITransition = GUI;
		if (GUI == GUIType.Start)
		{
			StartNewTrans.Reset();
		}
	}
	public void CorrectAllLayers(){
		foreach(ShaderLayer SL in ShaderUtil.GetAllLayers()){
			SL.BugCheck();
		}
	}
	//ShaderLayer LayerDragSelected = null;
	ShaderLayerList LayerDragSelectedLastPar = null;
	int LayerDragSelectedLastParIndex = 0;
	bool LayerDrag = false;
	bool LayerHasDragged = false;
	Vector2 LayerDragOffset;
	Vector2 LayerDragOriginal;
	float LayerDragWidth = 0f;
	float LayerDragLeft = 0f;
	bool LayerIsOverBin = false;
	
	
	bool GUILayersBox(ShaderPass SP,ShaderLayerList list, int X,Vector2 WinSize,GUIStyle ButtonStyle){
		if (DereBeARefreshHere){
			if (SRS.LayerSelection == null&&list.Count>0)
				SRS.LayerSelection = list.SLs[0];
		}
		//UnityEditor.Undo.RecordObject((UnityEngine.Object)list,"Change Layer List");
		list.LayerCatagory = list.Name.Text;
		list.FixParents();
		//if (!LayerDrag)
		//	UnityEditor.Undo.RecordObject((UnityEngine.Object)list,"Change Layer List");//Uselessly slow...going in manually.
		WinSize.y-=30;
		int HeightOfLayerName = 48;
		if (CurInterfaceStyle==InterfaceStyle.Modern)
			HeightOfLayerName = 26;
			
		int WidthOfAdd = 32;
		if (CurInterfaceStyle==InterfaceStyle.Modern)
			WidthOfAdd = 24;
		Vector2 newPos = LayerDragOriginal + Event.current.mousePosition-LayerDragOffset;
		EMindGUI.BeginGroup(new Rect(150*X,30,150,WinSize.y));
		
			int totalwidth = 150-16;
			int height = 80;
			int heightpad = height+10;
			
			int left = (150-height-32)/2+16-15/2;
			int heightstart = 0;//left;		
		
		
		GUI.Box(new Rect(0,0,150,WinSize.y),"",ButtonStyle);
		Vector2 tempundoscroll = EMindGUI.BeginScrollView(new Rect(0,HeightOfLayerName,150,WinSize.y-HeightOfLayerName-100-WidthOfAdd),list.Scroll,new Rect(0,0,130,list.Count*heightpad),false,false);
		if (tempundoscroll!=list.Scroll)
			UnityEditor.Undo.RegisterCompleteObjectUndo((UnityEngine.Object)list,"Scrolled Layer List");

		list.Scroll = tempundoscroll;
			

			

			
			int Pos = -1;

			bool Hit = false;
			
			
			//if (SRS.LayerSelection.x==X)
			//if (SRS.LayerSelection.y<list.Count)
				
			if ((LayerDrag||LayerHasDragged)&&SRS.LayerSelection!=null){
				Vector2 AbsIshPos = new Vector2(left,0)-list.Scroll+new Vector2(150*X,30+HeightOfLayerName);
				if ((newPos.x+(height/3)*2)>(AbsIshPos.x)&&(newPos.x+height/3)<(AbsIshPos.x+height)){
					if (!list.SLs.Contains(SRS.LayerSelection)){
						if (SRS.LayerSelection.Parent!=null)
							SRS.LayerSelection.Parent.SLs.Remove(SRS.LayerSelection);
						list.SLs.Add(SRS.LayerSelection);
						SRS.LayerSelection.Parent = list;
						LayerDragSelectedLastPar = list;
						LayerDragSelectedLastParIndex = list.SLs.Count-1;
						list.UpdateIcon(new Vector2(70,70));
					}
				}else{
					if (list.SLs.Contains(SRS.LayerSelection)){
						SRS.LayerSelection.Parent = null;
						list.SLs.Remove(SRS.LayerSelection);
						list.UpdateIcon(new Vector2(70,70));
					}
					
				}
			}
				
			if ((LayerDrag||LayerHasDragged)&&SRS.LayerSelection!=null&&list.SLs.Contains(SRS.LayerSelection)){
				
				int i = 0;
				//foreach(ShaderLayer SL in list){
				for(int ii = 0; ii<list.Count;ii++){
					Pos+=1;
					Vector2 AbsIshPos = new Vector2(left,Pos*heightpad+heightstart)-list.Scroll+new Vector2(150*X,30+HeightOfLayerName);
					if (newPos.y>(AbsIshPos.y-heightpad/2))
						i = Pos;
				}
				if (list.SLs.IndexOf(SRS.LayerSelection)!=i){
					list.MoveItem(list.SLs.IndexOf(SRS.LayerSelection),i);
					LayerDragSelectedLastParIndex = i;
					list.UpdateIcon(new Vector2(70,70));
				}
			}
			if (list.Count*heightpad-10<=WinSize.y-HeightOfLayerName-100-WidthOfAdd){
				left = (150-height-32)/2+16;//-15/2;
				totalwidth += 16;
			}
			
			Pos = -1;
			foreach(ShaderLayer SL in list){
				if (SRS.LayerSelection==SL&&(LayerDrag||LayerHasDragged)&&((newPos-LayerDragOriginal).magnitude>2||LayerHasDragged)){
					Pos+=1;
					Color bgc = GUI.backgroundColor;
					GUI.backgroundColor = EMindGUI.BaseInterfaceColor*EMindGUI.BaseInterfaceColor;
					GUI.Box(new Rect(left+8-1,Pos*heightpad+heightstart+8-1,height-16+2,height-16+2),"",GUI.skin.GetStyle("SelectionRect"));
					GUI.backgroundColor = bgc;
					
					continue;
				}
				//if (Event.current.type==EventType.Repaint)
				//EMindGUI.BeginGroup(new Rect(-9000,0,0,0));
				//SL.DrawGUI();
				//EMindGUI.EndGroup();
				
				if (SL.Name.Text=="")
				SL.Name.Text = SL.GetLayerCatagory();
				if (SL.BugCheck())
				ChangeSaveTemp(SL);
				Pos+=1;
				bool OldGUIChanged = GUI.changed;
				Color asdasd = GUI.color;
				Rect dragArea = new Rect(left,Pos*heightpad+heightstart,height,height);
				if (SRS.LayerSelection==SL||(dragArea.Contains(Event.current.mousePosition)&&!(LayerDrag||LayerHasDragged)))
					GUI.color = EMindGUI.BaseInterfaceColor;
				else
					GUI.color = new Color(0.4f,0.4f,0.4f,1f);
				
				//GUI.DrawTexture(dragArea,DragAreaTex2,ScaleMode.ScaleAndCrop);
				//dragArea.x-=height+16;
				//GUI.DrawTexture(dragArea,DragAreaTex2,ScaleMode.ScaleAndCrop);
				//EditorGUIUtility.AddCursorRect(dragArea,MouseCursor.Pan);
				GUI.color = asdasd;
				
				if (EMindGUI.MouseDownIn(dragArea,false)){
					if (SRS.LayerSelection!=SL){
						UnityEditor.Undo.SetCurrentGroupName("Selected Layer");
						UnityEditor.Undo.RegisterCompleteObjectUndo((UnityEngine.Object)SRS,"Select Layer");
					}
					SRS.LayerSelection = SL;
					EMindGUI.Defocus();
					
					if (Event.current.button == 0){
						LayerDrag = true;
						LayerHasDragged = false;
						LayerDragWidth = totalwidth;
						LayerDragLeft = -left;
						
						LayerDragSelectedLastPar = SL.Parent;
						LayerDragSelectedLastParIndex = SL.Parent.SLs.IndexOf(SL);
						LayerDragOffset = Event.current.mousePosition-list.Scroll+new Vector2(150*X,30+HeightOfLayerName);
						LayerDragOriginal = new Vector2(left,Pos*heightpad+heightstart)-list.Scroll+new Vector2(150*X,30+HeightOfLayerName);
						
						float r = UnityEngine.Random.value;
						//Debug.Log(r);
						CurrentBin = Bin;
						LastFunBin--;
						if (LastFunBin<=0){
							if (r<0.1f)
								CurrentBin = Bin2;
							if (r<0.05f)
								CurrentBin = Bin3;
						}
						if (r<0.1f)
							LastFunBin = 5;
					}
				}
				//Color asd = GUI.color;
				Color asd = GUI.backgroundColor;
				
				if ((Pos*heightpad+heightstart+80-list.Scroll.y)>0&&(Pos*heightpad+heightstart-list.Scroll.y<(WinSize.y-150))){
					//if ((X)==SRS.LayerSelection.x&&(Pos)==SRS.LayerSelection.y){
					if (SL==SRS.LayerSelection){
						//GUI.color = new Color(0.4f,0.4f,0.4f,1f);
						//if (Event.current.type==EventType.Repaint)
						//	GUI.skin.button.Draw(new Rect(0,Pos*heightpad+heightstart,150,heightpad+1),false,true,true,true);
						//GUI.color = asd;
						SL.DrawIcon(new Rect(left,Pos*heightpad+heightstart,height,height),true);
						
					}
					else{
						GUI.backgroundColor = new Color(0.9f,0.9f,0.9f,1f);
						//if (Event.current.type==EventType.Repaint)
						//GUI.skin.button.Draw(new Rect(0,Pos*heightpad+heightstart,150,heightpad+1),false,false,false,false);
					
						GUI.backgroundColor = asd;
						if (SL.DrawIcon(new Rect(left,Pos*heightpad+heightstart,height,height),false)){
							//SRS.LayerSelection = new Vector2(X,Pos);
							SRS.LayerSelection = SL;
							EMindGUI.Defocus();
						}
					}
				}
				if (SRS.LayerSelection == SL){
					Color bgc2 = GUI.backgroundColor;
						GUI.backgroundColor = new Color(EMindGUI.BaseInterfaceColor.r*EMindGUI.BaseInterfaceColor.r,EMindGUI.BaseInterfaceColor.g*EMindGUI.BaseInterfaceColor.g,EMindGUI.BaseInterfaceColor.b*EMindGUI.BaseInterfaceColor.b,0.5f);
						GUI.Box(new Rect(left+8-1,Pos*heightpad+heightstart+8-1,height-16+2,height-16+2),"",GUI.skin.GetStyle("SelectionRect"));
					GUI.backgroundColor = bgc2;
				}
				GUI.skin.label.alignment = TextAnchor.LowerCenter;
				
				if (ViewLayerNames){
					
					GUI.color = new Color(0f,0f,0f,0.3f);
					
					EMindGUI.SetLabelSize(11);
					float h = GUI.skin.label.CalcHeight(new GUIContent(SL.Name.Text),totalwidth);
					GUI.Box(new Rect(0,Pos*heightpad+heightpad-h+heightstart,150,h),"");
					GUI.color = asdasd;
					
					if (SRS.LayerSelection==SL)
						GUI.color = EMindGUI.BaseInterfaceColor;
					EMindGUI.Label(new Rect(0,Pos*heightpad+heightstart,totalwidth,heightpad),SL.Name.Text,11);
					GUI.color = asdasd;
					
					EMindGUI.SetLabelSize(12);
				}
				GUI.skin.label.alignment = TextAnchor.UpperLeft;
				GUI.changed = OldGUIChanged;
				if (Event.current.type == EventType.MouseDown && new Rect(left,Pos*heightpad+heightstart,height,height).Contains(Event.current.mousePosition))
				Hit = true;
				
				//Rect dragArea = new Rect(left+height,Pos*heightpad,16,100);

				/*if (Pos==0)
				GUI.enabled = false;
				EMindGUI.MakeTooltip(0,new Rect(120-(15),Pos*110,30,30),"Move Layer Up");
				if (GUI.Button(new Rect(120-(15),Pos*110,30,30),new GUIContent(UPArrow)))
				{
					list.MoveItem(Pos,Pos-1);//MoveLayerUp = Pos;
					list.UpdateIcon(new Vector2(70,70));
					ChangeSaveTemp(null);
					//RegenShaderPreview();
					EditorGUIUtility.ExitGUI();
				}
				GUI.enabled = true;
				EMindGUI.MakeTooltip(0,new Rect(120-(15),Pos*110+30,30,30),"Delete Layer");
				if (GUI.Button(new Rect(120-(15),Pos*110+30,30,30),new GUIContent(CrossRed))){
					list.RemoveAt(Pos);
					//UEObject.DestroyImmediate(SL,false);
					Undo.DestroyObjectImmediate(SL);
					list.UpdateIcon(new Vector2(70,70));
					ChangeSaveTemp(null);
					//RegenShaderPreview();
					UnityEditor.Undo.IncrementCurrentGroup();
					EditorGUIUtility.ExitGUI();
					
				}
				if (Pos>=list.Count-1)
				GUI.enabled = false;
				EMindGUI.MakeTooltip(0,new Rect(120-(15),Pos*110+60,30,30),"Move Layer Down");
				if (GUI.Button(new Rect(120-(15),Pos*110+60,30,30),new GUIContent(DOWNArrow,"Move Layer Down"))){
					//Pos2+=1;	
					list.MoveItem(Pos,Pos+1);//MoveLayerDown = Pos;
					list.UpdateIcon(new Vector2(70,70));
					ChangeSaveTemp(null);
					//RegenShaderPreview();
					UnityEditor.Undo.IncrementCurrentGroup();
					EditorGUIUtility.ExitGUI();
				}
				GUI.enabled = true;*/
				
				//GUI.Label(new Rect(30,48+Pos*110+75,60,30),"Editor: ");
				//GUI.color = new Color(1f,1f,1f,1f);
				//GUI.backgroundColor = new Color(1f,1f,1f,1f);
				//SL.LayerType.UseInput = GUI.Toggle(new Rect(80,48+Pos*110+75,30,30),SL.LayerType.UseInput,"");
				
				//GUI.color = oldCol;
				//GUI.backgroundColor = oldColB;
			}

			
			
			

			
		EMindGUI.EndScrollView();
		EMindGUI.VerticalScrollbar(new Rect(0,HeightOfLayerName,150,WinSize.y-HeightOfLayerName-100-WidthOfAdd),list.Scroll,new Rect(0,0,130,list.Count*heightpad),false);
			for(ButtonStyle.fontSize=20;ButtonStyle.fontSize>10;ButtonStyle.fontSize-=1){
				if (ButtonStyle.CalcSize(new GUIContent(list.Name.Text)).x<=124)
				break;
			}
			GUI.Box(new Rect(0,0,150,HeightOfLayerName),"",ButtonStyle);
			if (OpenShader.ShaderLayersMasks.Contains(list)){
				string tempundoname;
				if(CurInterfaceStyle==InterfaceStyle.Modern)
					tempundoname = GUI.TextField(new Rect(0,0,150-26,HeightOfLayerName),list.Name.Text,ButtonStyle);
				else
					tempundoname = GUI.TextField(new Rect(0,10,150-26,HeightOfLayerName-20),list.Name.Text,ButtonStyle);
				if (list.Name.Text!=tempundoname)
					UnityEditor.Undo.RegisterCompleteObjectUndo((UnityEngine.Object)list,"Changed Mask's Name");
				list.Name.Text = tempundoname;
			}
			else
			GUI.Label(new Rect(0,0,150,HeightOfLayerName),list.Name.Text,ButtonStyle);
			
			if (list.Count==0){
				if(CurInterfaceStyle==InterfaceStyle.Modern)
					EMindGUI.Label(new Rect(5,24,150,300),list.Description,12);
				else
					EMindGUI.Label(new Rect(5,48,150,300),list.Description,12);
			}
			
			GUI.Box(EMindGUI.Rect2(RectDir.Diag,150,WinSize.y-100,150,WidthOfAdd),"");
			Color oldg = GUI.contentColor;
			GUI.contentColor = EMindGUI.BaseInterfaceColorComp2;
			EMindGUI.MakeTooltip(0,EMindGUI.Rect2(RectDir.Diag,150,WinSize.y-100,WidthOfAdd,WidthOfAdd),"Add Layer");
			if (GUI.Button(EMindGUI.Rect2(RectDir.Diag,150,WinSize.y-100,WidthOfAdd,WidthOfAdd),new GUIContent(Plus,"Add Layer"))){
				UnityEditor.Undo.IncrementCurrentGroup();
				UnityEditor.Undo.RegisterCompleteObjectUndo((UnityEngine.Object)list,"Add Layer");
				UnityEditor.Undo.RegisterCompleteObjectUndo((UnityEngine.Object)SRS,"Add Layer");
				ShaderLayer Sl = ShaderLayer.CreateInstance<ShaderLayer>();
				
				list.Add(Sl);
				//Undo(ShaderHistory.AddLayer(Sl));
				//if (list.CodeName == "d.Normal"){
				if (list.BaseColor == new Color(0f,0f,1f,1f)){
					//ShaderUtil.SetupCustomData(Sl.CustomData,ShaderSandwich.PluginList["LayerType"][Sl.RealLayerType.Text].GetInputs());
					Sl.CustomData.D["Color"] = new ShaderVar("Color",new Vector4(0f,0f,1f,1f));
				}
				if (list.EndTag.Text.Length==1){
					Sl.SetType("SLTNumber");
				}
				//SRS.LayerSelection = new Vector2(X,list.Count-1);
				SRS.LayerSelection = Sl;//new Vector2(X,list.Count-1);
				
			}
			GUI.contentColor = EMindGUI.BaseInterfaceColorComp;
			EMindGUI.MakeTooltip(0,EMindGUI.Rect2(RectDir.Diag,0,WinSize.y-100,WidthOfAdd,WidthOfAdd),"Delete Layer");
			if (EMindGUI.Rect2(RectDir.Bottom,0,WinSize.y-100,WidthOfAdd,WidthOfAdd).Contains(Event.current.mousePosition)){
				//if (list.SLs.Contains(SRS.LayerSelection)){
					//SRS.LayerSelection.Parent = null;
					//list.SLs.Remove(SRS.LayerSelection);
					//list.UpdateIcon(new Vector2(70,70));
					LayerIsOverBin = true;
				//}
			}
			
			if (GUI.Button(EMindGUI.Rect2(RectDir.Bottom,0,WinSize.y-100,WidthOfAdd,WidthOfAdd),new GUIContent(CurrentBin,"Delete Layer"))){
				if (SRS.LayerSelection!=null&&list.SLs.Contains(SRS.LayerSelection))
					LayerDelete((object)SRS.LayerSelection);
			}
			GUI.contentColor = oldg;
			if (OpenShader.ShaderLayersMasks.Contains(list)||list==OpenShader.ShaderLayersMaskTemp){
				int ColorSelection = 0;
				if (list.EndTag.Text=="r")
				ColorSelection = 0;
				if (list.EndTag.Text=="g")
				ColorSelection = 1;
				if (list.EndTag.Text=="b")
				ColorSelection = 2;
				if (list.EndTag.Text=="a")
				ColorSelection = 3;
				if (list.EndTag.Text=="rgba")
				ColorSelection = 4;
				string[] ColorTag = new string[]{"Red","Green","Blue",
				"Alpha","RGBA"};
				string[] ColorTag2 = new string[]{"r","g","b",
				"a","rgba"};
					/*if (GUI.Button(EMindGUI.Rect2(RectDir.Bottom,1,WinSize.y-100,(150-33)/2,32),ColorTag[ColorSelection],"button")){
						GenericMenu toolsMenu = new GenericMenu();
						toolsMenu.AddItem(new GUIContent("Red"), false, MashMakeRed,list);
						toolsMenu.AddItem(new GUIContent("Green"), false, MashMakeGreen,list);
						toolsMenu.AddItem(new GUIContent("Blue"), false, MashMakeBlue,list);
						toolsMenu.AddItem(new GUIContent("Alpha"), false, MashMakeAlpha,list);
						toolsMenu.AddItem(new GUIContent("RGBA"), false, MashMakeRGBA,list);
						toolsMenu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0));
						list.UpdateIcon(new Vector2(70,70));
						ChangeSaveTemp(null);
						EditorGUIUtility.ExitGUI();
					}*/
					//VertexMask.Float = (float)(int)(VertexMasks)EditorGUI.EnumPopup(new Rect(0,YOffset,250,20)," ",(VertexMasks)(int)VertexMask.Float,EMindGUI.EditorPopup);
					EditorGUI.BeginChangeCheck();
					EMindGUI.MakeTooltip(0,new Rect(23+26+26,WinSize.y-124,100-23-26-26+28,24),"The channel the mask uses.");
					ColorSelection = EditorGUI.IntPopup(new Rect(23+26+26,WinSize.y-124,100-23-26-26+28,24),ColorSelection,ColorTag,new int[]{0,1,2,3,4},"button");
					if (EditorGUI.EndChangeCheck()){
						UnityEditor.Undo.RegisterCompleteObjectUndo((UnityEngine.Object)list,"Changed Mask's Component");
						list.UpdateIcon(new Vector2(70,70));
						ChangeSaveTemp(null);
						list.EndTag.Text = ColorTag2[ColorSelection];
						UpdateMaskShaderVars();
					}
					/*EMindGUI.MakeTooltip(0,new Rect(0,WinSize.y-132,32,32),"Set the mask to be a lighting mask.");
					EditorGUI.BeginChangeCheck();
					GUI.Toggle(new Rect(0,WinSize.y-132,32,32),list.IsLighting.On,list.IsLighting.On?LightBulbOn:LightBulbOff,ButtonStyle);
					if (EditorGUI.EndChangeCheck())
					{
						if (EditorUtility.DisplayDialog(list.IsLighting.On?"Switch back to normal mask?":"Switch to lighting mask?", "Switching between normal and lighting masks will remove all layers within the mask. Are you sure you want to do this?", "Yes", "No")){
							list.SLs.Clear();
							list.IsLighting.On = !list.IsLighting.On;
							//OpenShader.ShaderLayersMasks.Remove(list);
							CorrectAllLayers();
							list.UpdateIcon(new Vector2(70,70));
							ChangeSaveTemp(null);
							EditorGUIUtility.ExitGUI();
						}
					}*/
					//EMindGUI.MakeTooltip(0,new Rect(0,WinSize.y-124,24,24),"Delete the Mask?");
					EMindGUI.MakeTooltip(0,new Rect(150-26,0,26,26),"Delete the Mask?");
					if (GUI.Button(new Rect(150-26,0,26,26),CrossRed)){
						if (EditorUtility.DisplayDialog("Delete Mask?", "Are you sure you want to delete this mask? It could be in use by other layers and effects (whose links won't be restored with an undo).", "Yes", "No")){
							UnityEditor.Undo.RegisterCompleteObjectUndo(OpenShader,"Delete Mask ("+list.Name.Text+")");
							OpenShader.ShaderLayersMasks.Remove(list);
							UpdateMaskShaderVars();
							//Remove mask from everything...
							/*foreach (ShaderLayer SL in ShaderUtil.GetAllLayers()){
								if (SL.Stencil.Obj==list){
									SL.Stencil.Selected = -1;
								}
							}
							foreach (ShaderVar SV in ShaderUtil.GetAllShaderVars()){
								if (SV.Obj==list){
									SV.Selected = -1;
								}
							}
							foreach (ShaderInput SI in OpenShader.ShaderInputs){
								//SI.Mask.SetToMasks(null,0);
								SI.Mask.HookIntoObjects();
								if (SI.Mask.Obj==list){
									SI.Mask.Selected = -1;
								}
							}*/
							
							CorrectAllLayers();
							ChangeSaveTemp(null);
							EditorGUIUtility.ExitGUI();
						}
					}
					EMindGUI.MakeTooltip(0,new Rect(23,WinSize.y-124,27,24),"Move the mask to the left.");
					if (GUI.Button(new Rect(23,WinSize.y-124,27,24),LeftArrow)){
						UnityEditor.Undo.RegisterCompleteObjectUndo(OpenShader,"Move Mask");
						ShaderUtil.MoveItem(ref OpenShader.ShaderLayersMasks,OpenShader.ShaderLayersMasks.IndexOf(list),OpenShader.ShaderLayersMasks.IndexOf(list)-1);
						CorrectAllLayers();
						ChangeSaveTemp(null);
						UpdateMaskShaderVars();
						EditorGUIUtility.ExitGUI();
					}
					EMindGUI.MakeTooltip(0,new Rect(23+26,WinSize.y-124,27,24),"Move the mask to the right.");
					if (GUI.Button(new Rect(23+26,WinSize.y-124,27,24),RightArrow)){
						UnityEditor.Undo.RegisterCompleteObjectUndo(OpenShader,"Move Mask");
						ShaderUtil.MoveItem(ref OpenShader.ShaderLayersMasks,OpenShader.ShaderLayersMasks.IndexOf(list),OpenShader.ShaderLayersMasks.IndexOf(list)+1);
						CorrectAllLayers();
						ChangeSaveTemp(null);
						UpdateMaskShaderVars();
						EditorGUIUtility.ExitGUI();
					}
			}
			else
			if (list.EndTag.Text.Length==1){//||OpenShader.ShaderLayersMasks.Contains(list)){
				int ColorSelection = 0;
				if (list.EndTag.Text=="r")
				ColorSelection = 0;
				if (list.EndTag.Text=="g")
				ColorSelection = 1;
				if (list.EndTag.Text=="b")
				ColorSelection = 2;
				if (list.EndTag.Text=="a")
				ColorSelection = 3;
				string[] ColorTag = new string[]{"r","g","b",
				"a"};
				string OldEndTag = list.EndTag.Text;
				//if (CurInterfaceStyle == InterfaceStyle.Modern)
				//	ColorSelection = GUI.SelectionGrid(EMindGUI.Rect2(RectDir.Bottom,1,WinSize.y-100,150-33,24),ColorSelection,new string[]{"R","G","B","A"},4);
				//else
				//	ColorSelection = GUI.SelectionGrid(EMindGUI.Rect2(RectDir.Bottom,1,WinSize.y-100,150-33,32),ColorSelection,new string[]{"R","G","B","A"},4);
				EMindGUI.MakeTooltip(0,new Rect(23,WinSize.y-124,100-23+28,24),"The color channel to be used.");
				ColorSelection = EditorGUI.IntPopup(new Rect(23,WinSize.y-124,100-23+28,24),ColorSelection,new string[]{"Red","Green","Blue","Alpha"},new int[]{0,1,2,3},"button");
				
				
				if (ColorTag[ColorSelection]!=OldEndTag){
					UnityEditor.Undo.RegisterCompleteObjectUndo((UnityEngine.Object)list,"Changed Layer Channel's Component");
					list.EndTag.Text = ColorTag[ColorSelection];
					list.UpdateIcon(new Vector2(70,70));
					UpdateMaskShaderVars();
				}
			}

		if (CurInterfaceStyle==InterfaceStyle.Flat)
			GUI.Box(EMindGUI.Rect2(RectDir.Bottom,0,WinSize.y,150,100),"");//,ButtonStyle);
		else
			GUI.Box(EMindGUI.Rect2(RectDir.Bottom,0,WinSize.y,150,100),"",ButtonStyle);
		if (CurInterfaceStyle==InterfaceStyle.Modern)
			EMindGUI.MakeTooltip(0,new Rect(150-16,WinSize.y-16,16,16),"Reload layer preview (In case it hasn't updated).");
		else
			EMindGUI.MakeTooltip(0,new Rect(150-16,WinSize.y-100,16,16),"Reload layer preview (In case it hasn't updated).");
		//GUIStyle ButtonStyle2 = new GUIStayle(ButtonStyle);
		//ButtonStyle2.padding = new RectOffset(2,2,2,2);
		//ButtonStyle2.margin = new RectOffset(2,2,2,2);
		if (CurInterfaceStyle==InterfaceStyle.Modern){
			if (GUI.Button(new Rect(150-16,WinSize.y-16,16,16),ReloadIcon,ShaderUtil.Button2Border))
				GUI.changed = true;
		}
		else
		if (GUI.Button(new Rect(150-16,WinSize.y-100,16,16),ReloadIcon,ShaderUtil.Button2Border))
			GUI.changed = true;
			
		list.DrawIcon(EMindGUI.Rect2(RectDir.Bottom,25+15,WinSize.y-15,100-30,100-30),GUI.changed);
		if (Event.current.button == 1&&EMindGUI.MouseUpIn(new Rect(0,0,EMindGUI.GetGroupRect().width,EMindGUI.GetGroupRect().height-100-30)) ){
			GenericMenu toolsMenu = new GenericMenu();
			toolsMenu.AddItem(new GUIContent("Paste"), false, LayerPaste,list);
			toolsMenu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0));
			EditorGUIUtility.ExitGUI();					
		}							
		EMindGUI.EndGroup();

		return Hit;
	}
	public void MashMakeRed(object List){((ShaderLayerList)List).EndTag.Text = "r";}
	public void MashMakeGreen(object List){((ShaderLayerList)List).EndTag.Text = "g";}
	public void MashMakeBlue(object List){((ShaderLayerList)List).EndTag.Text = "b";}
	public void MashMakeAlpha(object List){((ShaderLayerList)List).EndTag.Text = "a";}
	public void MashMakeRGBA(object List){((ShaderLayerList)List).EndTag.Text = "rgba";}
	Dictionary<string,bool> InputSwitch;
	Material blah = null;
	
	
	public void SetSelectedInputFallback(object ty){
		InputMainTypes tyblerg = (InputMainTypes)ty;
		SRS.InputSelection.MainType = tyblerg;
		ShaderSandwich.Instance.RegenShaderPreview();
	}
	public void SetSelectedInputReplacement(object ty){
		InputSpecialTypes tyblerg = (InputSpecialTypes)ty;
		SRS.InputSelection.SpecialType = tyblerg;
		ShaderSandwich.Instance.RegenShaderPreview();
	}
	
	
	//public ShaderInput SRS.InputSelection = null;
	public bool InputDrag = false;
	public bool InputDragged = false;
	public bool InputEdit = false;
	public Vector2 InputDragOffset = new Vector2(0,0);
	public float InputDragY = 54;
	public bool VisibleInputsWhenDragStarted = false;
	public int InputIndex = 0;
	public bool InputInEditor = false;
	
	public Vector2 InputsScroll = new Vector2(0,0);
	//SI, y, SIHeight, initDrag, initEdit, WinSize, width, left
	float DrawInput(ShaderInput SI, float y, float SIheight, bool initDrag, bool initEdit, Vector2 WinSize, float width, float left, bool HasEnteredVisibles){
					if (EMindGUI.MouseDownIn(new Rect(0,y,width,SIheight+1),false)&&!InputDrag){
						//Debug.Log("Cliedk in woot");
						if (SRS.InputSelection != SI){
							InputEdit = false;
							EMindGUI.Defocus();
						}
						SRS.InputSelection = SI;
						//Debug.Log(InputEdit);
					}
					
		//Rect dragArea = new Rect(0,y,16,16);
		Rect dragArea = new Rect(left,y,117,19);
		Rect editArea = new Rect(0,y,width,SIheight);
		//Color asdasd = GUI.color;
		
		//if (SRS.InputSelection==SI||(dragArea.Contains(Event.current.mousePosition)))
		//	GUI.color = EMindGUI.BaseInterfaceColor;
		//else
		//	GUI.color = new Color(0.4f,0.4f,0.4f,1f);
		
		//GUI.DrawTexture(dragArea,DragAreaTex,ScaleMode.ScaleAndCrop);
		if (SRS.InputSelection!=SI||!InputEdit)
			EditorGUIUtility.AddCursorRect(dragArea,MouseCursor.Pan);
		//GUI.color = asdasd;
		
		//if (InputEdit&&SRS.InputSelection==SI)
		//	SI.VisName = GUI.TextField(new Rect(left,y,117,20),SI.VisName,EditorStyles.label);
		//else
		//	GUI.Label(new Rect(left,y,117,20),SI.VisName,EditorStyles.label);
		if (EMindGUI.MouseDownIn(editArea,false)&&!InputEdit){
			if (Event.current.clickCount==1){
				if (EMindGUI.MouseDownIn(dragArea,false)){
					EMindGUI.Defocus();
					InputDragOffset = Event.current.mousePosition;
					InputDragY = y+2;//-InputsScroll.y;
					InputDrag = true;
					InputIndex = OpenShader.ShaderInputs.IndexOf(SI);
					InputInEditor = SI.InEditor;
					InputDragged = false;
					SRS.InputSelection = SI;
					Event.current.Use();
					InputEdit = false;
					VisibleInputsWhenDragStarted = HasEnteredVisibles;
					//Debug.Log(VisibleInputsWhenDragStarted);
				}
			}else {
				//Debug.Log(Event.current.clickCount);
				if (!InputEdit)
				EMindGUI.Defocus();
			
				Event.current.clickCount=1;
				InputEdit = true;
			}
		}
		
		if (EMindGUI.MouseDownIn(new Rect(0,y,left,left))){
			SRS.InputSelection = SI;
			GenericMenu menu = new GenericMenu();
			if (SI.InEditor){
				menu.AddItem(new GUIContent("None"), SI.MainType==InputMainTypes.None, SetSelectedInputFallback, InputMainTypes.None);
				if (SI.Type==ShaderInputTypes.Color||SI.Type==ShaderInputTypes.Vector){
					menu.AddItem(new GUIContent("Main/Color"), SI.MainType==InputMainTypes.MainColor, SetSelectedInputFallback, InputMainTypes.MainColor);
					menu.AddItem(new GUIContent("Main/Specular Color"), SI.MainType==InputMainTypes.SpecularColor, SetSelectedInputFallback, InputMainTypes.SpecularColor);
					menu.AddItem(new GUIContent("Main/Reflection Color"), SI.MainType==InputMainTypes.ReflectColor, SetSelectedInputFallback, InputMainTypes.ReflectColor);
					menu.AddItem(new GUIContent("Main/Emission Color"), SI.MainType==InputMainTypes.EmissionColor, SetSelectedInputFallback, InputMainTypes.EmissionColor);
					menu.AddItem(new GUIContent("Misc/Tint"), SI.MainType==InputMainTypes.Tint, SetSelectedInputFallback, InputMainTypes.Tint);
				}
				if (SI.Type==ShaderInputTypes.Image){
					menu.AddItem(new GUIContent("Main/Texture"), SI.MainType==InputMainTypes.MainTexture, SetSelectedInputFallback, InputMainTypes.MainTexture);
					menu.AddItem(new GUIContent("Main/Normal Map"), SI.MainType==InputMainTypes.BumpMap, SetSelectedInputFallback, InputMainTypes.BumpMap);
					menu.AddItem(new GUIContent("Standard Shader/Parallax Map"), SI.MainType==InputMainTypes.ParallaxMap, SetSelectedInputFallback, InputMainTypes.ParallaxMap);
					menu.AddItem(new GUIContent("Standard Shader/Metallic\\Gloss Map"), SI.MainType==InputMainTypes.MetallicGlossMap, SetSelectedInputFallback, InputMainTypes.MetallicGlossMap);
					menu.AddItem(new GUIContent("Standard Shader/Specular\\Gloss Map"), SI.MainType==InputMainTypes.SpecGlossMap, SetSelectedInputFallback, InputMainTypes.SpecGlossMap);
					menu.AddItem(new GUIContent("Standard Shader/Occlusion Map"), SI.MainType==InputMainTypes.OcclusionMap, SetSelectedInputFallback, InputMainTypes.OcclusionMap);
					menu.AddItem(new GUIContent("Standard Shader/Emission Map"), SI.MainType==InputMainTypes.EmissionMap, SetSelectedInputFallback, InputMainTypes.EmissionMap);
					menu.AddItem(new GUIContent("Standard Shader/Detail/Mask"), SI.MainType==InputMainTypes.DetailMask, SetSelectedInputFallback, InputMainTypes.DetailMask);
					menu.AddItem(new GUIContent("Standard Shader/Detail/Texture"), SI.MainType==InputMainTypes.DecalTex, SetSelectedInputFallback, InputMainTypes.DecalTex);
					menu.AddItem(new GUIContent("Standard Shader/Detail/Normal Map"), SI.MainType==InputMainTypes.DetailNormalMap, SetSelectedInputFallback, InputMainTypes.DetailNormalMap);
					menu.AddItem(new GUIContent("Standard Shader/Detail/Albedo Map"), SI.MainType==InputMainTypes.DetailAlbedoMap, SetSelectedInputFallback, InputMainTypes.DetailAlbedoMap);
					menu.AddItem(new GUIContent("Terrain/Control"), SI.MainType==InputMainTypes.TerrainControl, SetSelectedInputFallback, InputMainTypes.TerrainControl);
					menu.AddItem(new GUIContent("Terrain/Splat 0"), SI.MainType==InputMainTypes.TerrainSplat0, SetSelectedInputFallback, InputMainTypes.TerrainSplat0);
					menu.AddItem(new GUIContent("Terrain/Splat 1"), SI.MainType==InputMainTypes.TerrainSplat1, SetSelectedInputFallback, InputMainTypes.TerrainSplat1);
					menu.AddItem(new GUIContent("Terrain/Splat 2"), SI.MainType==InputMainTypes.TerrainSplat2, SetSelectedInputFallback, InputMainTypes.TerrainSplat2);
					menu.AddItem(new GUIContent("Terrain/Splat 3"), SI.MainType==InputMainTypes.TerrainSplat3, SetSelectedInputFallback, InputMainTypes.TerrainSplat3);
					menu.AddItem(new GUIContent("Terrain/Normal 0"), SI.MainType==InputMainTypes.TerrainNormal0, SetSelectedInputFallback, InputMainTypes.TerrainNormal0);
					menu.AddItem(new GUIContent("Terrain/Normal 1"), SI.MainType==InputMainTypes.TerrainNormal1, SetSelectedInputFallback, InputMainTypes.TerrainNormal1);
					menu.AddItem(new GUIContent("Terrain/Normal 2"), SI.MainType==InputMainTypes.TerrainNormal2, SetSelectedInputFallback, InputMainTypes.TerrainNormal2);
					menu.AddItem(new GUIContent("Terrain/Normal 3"), SI.MainType==InputMainTypes.TerrainNormal3, SetSelectedInputFallback, InputMainTypes.TerrainNormal3);
				}
				if (SI.Type==ShaderInputTypes.Cubemap){
					menu.AddItem(new GUIContent("Main/Reflection Map"), SI.MainType==InputMainTypes.MainCubemap, SetSelectedInputFallback, InputMainTypes.MainCubemap);
				}
				if (SI.Type==ShaderInputTypes.Float||SI.Type==ShaderInputTypes.Range){
					menu.AddItem(new GUIContent("Main/Shininess"), SI.MainType==InputMainTypes.Shininess, SetSelectedInputFallback, InputMainTypes.Shininess);
					menu.AddItem(new GUIContent("Main/Transparency Cutoff"), SI.MainType==InputMainTypes.Cutoff, SetSelectedInputFallback, InputMainTypes.Cutoff);
					menu.AddItem(new GUIContent("Standard Shader/Glossiness"), SI.MainType==InputMainTypes.Glossiness, SetSelectedInputFallback, InputMainTypes.Glossiness);
					menu.AddItem(new GUIContent("Standard Shader/Metallic"), SI.MainType==InputMainTypes.Metallic, SetSelectedInputFallback, InputMainTypes.Metallic);
					menu.AddItem(new GUIContent("Standard Shader/Parallax"), SI.MainType==InputMainTypes.Parallax, SetSelectedInputFallback, InputMainTypes.Parallax);
					menu.AddItem(new GUIContent("Standard Shader/Normal Map Height"), SI.MainType==InputMainTypes.BumpScale, SetSelectedInputFallback, InputMainTypes.BumpScale);
					menu.AddItem(new GUIContent("Standard Shader/Occlusion Strength"), SI.MainType==InputMainTypes.OcclusionStrength, SetSelectedInputFallback, InputMainTypes.OcclusionStrength);
					menu.AddItem(new GUIContent("Standard Shader/Detail/Normal Map Height"), SI.MainType==InputMainTypes.DetailNormalMapScale, SetSelectedInputFallback, InputMainTypes.DetailNormalMapScale);
					menu.AddItem(new GUIContent("Standard Shader/Detail/Normal Map Height"), SI.MainType==InputMainTypes.DetailNormalMapScale, SetSelectedInputFallback, InputMainTypes.DetailNormalMapScale);
					menu.AddItem(new GUIContent("Misc/Rotation"), SI.MainType==InputMainTypes.Rotation, SetSelectedInputFallback, InputMainTypes.Rotation);
					menu.AddItem(new GUIContent("Misc/Exposure"), SI.MainType==InputMainTypes.Exposure, SetSelectedInputFallback, InputMainTypes.Exposure);
					menu.AddItem(new GUIContent("Misc/Shell Distance"), SI.MainType==InputMainTypes.ShellDistance, SetSelectedInputFallback, InputMainTypes.ShellDistance);
				}
				menu.AddItem(new GUIContent("Custom"), SI.MainType==InputMainTypes.Custom, SetSelectedInputFallback, InputMainTypes.Custom);
			}else{
				menu.AddItem(new GUIContent("None"), SI.SpecialType==InputSpecialTypes.None, SetSelectedInputReplacement, InputSpecialTypes.None);
				if (SI.Type==ShaderInputTypes.Color||SI.Type==ShaderInputTypes.Vector){
					menu.AddItem(new GUIContent("Fog/Color"), SI.SpecialType==InputSpecialTypes.FogColor, SetSelectedInputReplacement, InputSpecialTypes.FogColor);
					menu.AddItem(new GUIContent("Fog/Paramaters"), SI.SpecialType==InputSpecialTypes.FogParamaters, SetSelectedInputReplacement, InputSpecialTypes.FogParamaters);
					menu.AddItem(new GUIContent("Screen/Paramaters(w,h,1\\w,1\\h)"), SI.SpecialType==InputSpecialTypes.ScreenParamaters, SetSelectedInputReplacement, InputSpecialTypes.ScreenParamaters);
					//FogColor,FogParamaters
				}
				if (SI.Type==ShaderInputTypes.Image){
					menu.AddItem(new GUIContent("Lightmap"), SI.SpecialType==InputSpecialTypes.Lightmap, SetSelectedInputReplacement, InputSpecialTypes.Lightmap);
					menu.AddItem(new GUIContent("Lightmap Ind"), SI.SpecialType==InputSpecialTypes.LightmapInd, SetSelectedInputReplacement, InputSpecialTypes.LightmapInd);
					menu.AddItem(new GUIContent("Dynamic Lightmap"), SI.SpecialType==InputSpecialTypes.DynamicLightmap, SetSelectedInputReplacement, InputSpecialTypes.DynamicLightmap);
					menu.AddItem(new GUIContent("Dynamic Directionality"), SI.SpecialType==InputSpecialTypes.DynamicDirectionality, SetSelectedInputReplacement, InputSpecialTypes.DynamicDirectionality);
					menu.AddItem(new GUIContent("Dynamic Normal"), SI.SpecialType==InputSpecialTypes.DynamicNormal, SetSelectedInputReplacement, InputSpecialTypes.DynamicNormal);
				}
				if (SI.Type==ShaderInputTypes.Cubemap){
					menu.AddItem(new GUIContent("Reflection Cube 0"), SI.SpecialType==InputSpecialTypes.SpecCube0, SetSelectedInputReplacement, InputSpecialTypes.SpecCube0);
					menu.AddItem(new GUIContent("Reflection Cube 1"), SI.SpecialType==InputSpecialTypes.SpecCube1, SetSelectedInputReplacement, InputSpecialTypes.SpecCube1);
				}
				if (SI.Type==ShaderInputTypes.Float||SI.Type==ShaderInputTypes.Range||SI.Type==ShaderInputTypes.Color){
					menu.AddItem(new GUIContent("Time/Linear"), SI.SpecialType==InputSpecialTypes.Time, SetSelectedInputReplacement, InputSpecialTypes.Time);
					menu.AddItem(new GUIContent("Time/Sine (0-1)"), SI.SpecialType==InputSpecialTypes.ClampedSinTime, SetSelectedInputReplacement, InputSpecialTypes.ClampedSinTime);
					menu.AddItem(new GUIContent("Time/Cosine (0-1)"), SI.SpecialType==InputSpecialTypes.ClampedCosTime, SetSelectedInputReplacement, InputSpecialTypes.ClampedCosTime);
					menu.AddItem(new GUIContent("Time/Sine"), SI.SpecialType==InputSpecialTypes.SinTime, SetSelectedInputReplacement, InputSpecialTypes.SinTime);
					menu.AddItem(new GUIContent("Time/Cosine"), SI.SpecialType==InputSpecialTypes.CosTime, SetSelectedInputReplacement, InputSpecialTypes.CosTime);
					
					menu.AddItem(new GUIContent("Time/Optimized/Linear/Realtime"), SI.SpecialType==InputSpecialTypes.TimeStandard, SetSelectedInputReplacement, InputSpecialTypes.TimeStandard);
					menu.AddItem(new GUIContent("Time/Optimized/Linear/20x Slower"), SI.SpecialType==InputSpecialTypes.TimeDiv20, SetSelectedInputReplacement, InputSpecialTypes.TimeDiv20);
					menu.AddItem(new GUIContent("Time/Optimized/Linear/2x Faster"), SI.SpecialType==InputSpecialTypes.TimeMul2, SetSelectedInputReplacement, InputSpecialTypes.TimeMul2);
					menu.AddItem(new GUIContent("Time/Optimized/Linear/3x Faster"), SI.SpecialType==InputSpecialTypes.TimeMul3, SetSelectedInputReplacement, InputSpecialTypes.TimeMul3);
					
					menu.AddItem(new GUIContent("Time/Optimized/Sine (0-1)/Realtime"), SI.SpecialType==InputSpecialTypes.ClampedSinTimeStandard, SetSelectedInputReplacement, InputSpecialTypes.ClampedSinTimeStandard);
					menu.AddItem(new GUIContent("Time/Optimized/Sine (0-1)/2x slower"), SI.SpecialType==InputSpecialTypes.ClampedSinTimeDiv2, SetSelectedInputReplacement, InputSpecialTypes.ClampedSinTimeDiv2);
					menu.AddItem(new GUIContent("Time/Optimized/Sine (0-1)/4x slower"), SI.SpecialType==InputSpecialTypes.ClampedSinTimeDiv4, SetSelectedInputReplacement, InputSpecialTypes.ClampedSinTimeDiv4);
					menu.AddItem(new GUIContent("Time/Optimized/Sine (0-1)/8x slower"), SI.SpecialType==InputSpecialTypes.ClampedSinTimeDiv8, SetSelectedInputReplacement, InputSpecialTypes.ClampedSinTimeDiv8);
					
					menu.AddItem(new GUIContent("Time/Optimized/Cosine (0-1)/Realtime"), SI.SpecialType==InputSpecialTypes.ClampedCosTimeStandard, SetSelectedInputReplacement, InputSpecialTypes.ClampedCosTimeStandard);
					menu.AddItem(new GUIContent("Time/Optimized/Cosine (0-1)/2x Slower"), SI.SpecialType==InputSpecialTypes.ClampedCosTimeDiv2, SetSelectedInputReplacement, InputSpecialTypes.ClampedCosTimeDiv2);
					menu.AddItem(new GUIContent("Time/Optimized/Cosine (0-1)/4x Slower"), SI.SpecialType==InputSpecialTypes.ClampedCosTimeDiv4, SetSelectedInputReplacement, InputSpecialTypes.ClampedCosTimeDiv4);
					menu.AddItem(new GUIContent("Time/Optimized/Cosine (0-1)/8x Slower"), SI.SpecialType==InputSpecialTypes.ClampedCosTimeDiv8, SetSelectedInputReplacement, InputSpecialTypes.ClampedCosTimeDiv8);
					
					menu.AddItem(new GUIContent("Time/Optimized/Sine/Realtime"), SI.SpecialType==InputSpecialTypes.SinTimeStandard, SetSelectedInputReplacement, InputSpecialTypes.SinTimeStandard);
					menu.AddItem(new GUIContent("Time/Optimized/Sine/2x slower"), SI.SpecialType==InputSpecialTypes.SinTimeDiv2, SetSelectedInputReplacement, InputSpecialTypes.SinTimeDiv2);
					menu.AddItem(new GUIContent("Time/Optimized/Sine/4x slower"), SI.SpecialType==InputSpecialTypes.SinTimeDiv4, SetSelectedInputReplacement, InputSpecialTypes.SinTimeDiv4);
					menu.AddItem(new GUIContent("Time/Optimized/Sine/8x slower"), SI.SpecialType==InputSpecialTypes.SinTimeDiv8, SetSelectedInputReplacement, InputSpecialTypes.SinTimeDiv8);
					
					menu.AddItem(new GUIContent("Time/Optimized/Cosine/Realtime"), SI.SpecialType==InputSpecialTypes.CosTimeStandard, SetSelectedInputReplacement, InputSpecialTypes.CosTimeStandard);
					menu.AddItem(new GUIContent("Time/Optimized/Cosine/2x Slower"), SI.SpecialType==InputSpecialTypes.CosTimeDiv2, SetSelectedInputReplacement, InputSpecialTypes.CosTimeDiv2);
					menu.AddItem(new GUIContent("Time/Optimized/Cosine/4x Slower"), SI.SpecialType==InputSpecialTypes.CosTimeDiv4, SetSelectedInputReplacement, InputSpecialTypes.CosTimeDiv4);
					menu.AddItem(new GUIContent("Time/Optimized/Cosine/8x Slower"), SI.SpecialType==InputSpecialTypes.CosTimeDiv8, SetSelectedInputReplacement, InputSpecialTypes.CosTimeDiv8);
					
					//menu.AddItem(new GUIContent("Depth/Shell"), SI.SpecialType==InputSpecialTypes.ShellDepth, SetSelectedInputReplacement, InputSpecialTypes.ShellDepth);
					menu.AddItem(new GUIContent("Depth/Shell Normalized"), SI.SpecialType==InputSpecialTypes.ShellDepthNormalized, SetSelectedInputReplacement, InputSpecialTypes.ShellDepthNormalized);
					menu.AddItem(new GUIContent("Depth/Shell Normalized Inverted"), SI.SpecialType==InputSpecialTypes.ShellDepthInvertedNormalized, SetSelectedInputReplacement, InputSpecialTypes.ShellDepthInvertedNormalized);
					menu.AddItem(new GUIContent("Depth/Parallax"), SI.SpecialType==InputSpecialTypes.ParallaxDepth, SetSelectedInputReplacement, InputSpecialTypes.ParallaxDepth);
					menu.AddItem(new GUIContent("Depth/Parallax Normalized"), SI.SpecialType==InputSpecialTypes.ParallaxDepthNormalized, SetSelectedInputReplacement, InputSpecialTypes.ParallaxDepthNormalized);
					
					menu.AddItem(new GUIContent("Screen/Width"), SI.SpecialType==InputSpecialTypes.ScreenWidth, SetSelectedInputReplacement, InputSpecialTypes.ScreenWidth);
					menu.AddItem(new GUIContent("Screen/Height"), SI.SpecialType==InputSpecialTypes.ScreenHeight, SetSelectedInputReplacement, InputSpecialTypes.ScreenHeight);
					
					menu.AddItem(new GUIContent("Screen/1\\Width"), SI.SpecialType==InputSpecialTypes.ScreenWidthInv, SetSelectedInputReplacement, InputSpecialTypes.ScreenWidthInv);
					menu.AddItem(new GUIContent("Screen/1\\Height"), SI.SpecialType==InputSpecialTypes.ScreenHeightInv, SetSelectedInputReplacement, InputSpecialTypes.ScreenHeightInv);
					
					menu.AddItem(new GUIContent("Screen/1 + 1\\Width"), SI.SpecialType==InputSpecialTypes.ScreenWidthInvPlusOne, SetSelectedInputReplacement, InputSpecialTypes.ScreenWidthInvPlusOne);
					menu.AddItem(new GUIContent("Screen/1 + 1\\Height"), SI.SpecialType==InputSpecialTypes.ScreenHeightInvPlusOne, SetSelectedInputReplacement, InputSpecialTypes.ScreenHeightInvPlusOne);
					
					menu.AddItem(new GUIContent("Screen/Width:Height"), SI.SpecialType==InputSpecialTypes.ScreenWidthDivScreenHeight, SetSelectedInputReplacement, InputSpecialTypes.ScreenWidthDivScreenHeight);
					menu.AddItem(new GUIContent("Screen/Height:Width"), SI.SpecialType==InputSpecialTypes.ScreenHeightDivScreenWidth, SetSelectedInputReplacement, InputSpecialTypes.ScreenHeightDivScreenWidth);
				}
				menu.AddItem(new GUIContent("Mask"), SI.SpecialType==InputSpecialTypes.Mask, SetSelectedInputReplacement, InputSpecialTypes.Mask);
				menu.AddItem(new GUIContent("Custom"), SI.SpecialType==InputSpecialTypes.Custom, SetSelectedInputReplacement, InputSpecialTypes.Custom);
			}
			
			menu.ShowAsContext();
		}
		GUI.Toggle(new Rect(0,y,left,left),(SI.InEditor&&SI.MainType!=InputMainTypes.None)||(!SI.InEditor&&SI.SpecialType!=InputSpecialTypes.None),new GUIContent(((SI.InEditor&&SI.MainType!=InputMainTypes.None)||(!SI.InEditor&&SI.SpecialType!=InputSpecialTypes.None))?StarOn:StarOff),GUI.skin.label);
					
					SI.VisName = GUI.TextField(new Rect(left,y,117,20),SI.VisName,EditorStyles.label);
					///TODO: Custom codename
					if (!SI.InEditor&&SI.SpecialType == InputSpecialTypes.Custom){
						SI.CustomSpecial.Text = EditorGUI.TextField(new Rect(117+left,y,width-117-left-6,16),SI.CustomSpecial.Text,GUI.skin.textField);
					}
					else if (!SI.InEditor&&SI.SpecialType == InputSpecialTypes.Mask){
						SI.Mask.Draw(new Rect(width-68,y,64,16),"");
					}else{
						if (SI.Type==ShaderInputTypes.Float){//Float
							SI.Number = EditorGUI.FloatField( new Rect(width-68,y,64,16),"",SI.Number,GUI.skin.textField);
						}
						if (SI.Type==ShaderInputTypes.Range){//Range
							//SI.Range0 = EditorGUI.FloatField(new Rect(550,10,30,20),"",SI.Range0,GUI.skin.textField);
							if (!initEdit)
								SI.Number = EMindGUI.Slider(new Rect(117+left,y,width-117-left-68-6,16),SI.Number,SI.Range0,SI.Range1);
							else
								SI.Number = EMindGUI.Slider(new Rect(117+left+16,y,width-117-left-68-6-16,16),SI.Number,SI.Range0,SI.Range1);
							SI.Number = EditorGUI.FloatField( new Rect(width-68,y,64,16),SI.Number,EMindGUI.EditorFloatInput);
							
							if (SI.Range0<SI.Range1)
								SI.Number = Mathf.Clamp(SI.Number,SI.Range0,SI.Range1);
							else
								SI.Number = Mathf.Clamp(SI.Number,SI.Range1,SI.Range0);
							//SI.Range1 = EditorGUI.FloatField(new Rect(640,10,30,20),"",SI.Range1,GUI.skin.textField);
						}
						if (SI.Type==ShaderInputTypes.Float||SI.Type==ShaderInputTypes.Range){
							int offscreen = initEdit?0:1000;
							//if (initEdit){
								if (SI.Type == ShaderInputTypes.Range){
									if (GUI.Button(new Rect(left+117-offscreen,y,10,20),"")){
										SI.Type = ShaderInputTypes.Float;
									}
									GUI.DrawTexture(new Rect(left+117-offscreen,y-10,10,32+8),RightTriangle);
									if (initEdit)
										y+=19;
									SI.Range0 = EditorGUI.FloatField(new Rect(left+117-offscreen+16,y,(width-(left+117+68+6+16))/2,16),SI.Range0,EMindGUI.EditorFloatInput);
									SI.Range1 = EditorGUI.FloatField(new Rect(117+left-offscreen+16+(width-(left+117+68+6+16))/2,y,(width-(left+117+68+6+16))/2,16),SI.Range1,EMindGUI.EditorFloatInput);
								}else{
									if (GUI.Button(new Rect(width-68-10-offscreen,y,10,20),"")){
										SI.Type = ShaderInputTypes.Range;
									}
									GUI.DrawTexture(new Rect(width-68-10-offscreen,y-10,10,32+8),LeftTriangle);
									if (initEdit)
										y+=19;
								}
								
							//}
							if (SI.UsesInputScale()){
								GUI.Label(new Rect(left-offscreen,y,50,60),"Speed");
								SI.InputScale = EditorGUI.FloatField(new Rect(50+left-offscreen,y,(width-117-left-68-6)/3,16),SI.InputScale,EMindGUI.EditorFloatInput);
							}
							///SI.Type = GUI.Toggle(new Rect(117+left-offscreen+(width-117-left-68-6)/3,y,(width-117-left-68-6)/3,16),SI.Type==ShaderInputTypes.Range,"",EMindGUI.ButtonImage)?ShaderInputTypes.Range:ShaderInputTypes.Float;
							///SI.Range0 = EditorGUI.FloatField(new Rect(117+left-offscreen,y,(width-117-left-68-6)/3,16),SI.Range0,EMindGUI.EditorFloatInput);
							///SI.Range1 = EditorGUI.FloatField(new Rect(117+left-offscreen+((width-117-left-68-6)/3)*2,y,(width-117-left-68-6)/3,16),SI.Range1,EMindGUI.EditorFloatInput);
							SI.IsGamma = GUI.Toggle(new Rect(width-68-offscreen,y,64,16),SI.IsGamma,"Gamma",EMindGUI.SCSkin.window);
						}
						
						Color OldColor = GUI.backgroundColor;
							GUI.backgroundColor = new Color(1,1,1,1);
							if (SI.Type==ShaderInputTypes.Image){//Image
								SI.Image = (Texture2D) EditorGUI.ObjectField (new Rect(width-68,y,64,64),SI.ImageS(), typeof (Texture2D),false);
							}
							if (SI.Type==ShaderInputTypes.Cubemap){//Cubemap
								SI.Cube = (Cubemap) EditorGUI.ObjectField (new Rect(width-68,y,64,64),SI.CubeS(), typeof (Cubemap),false);
							}
							if (SI.Type==ShaderInputTypes.Color){//Color
								if (SI.IsHDR)
									SI.Color = (ShaderColor)EditorGUI.ColorField(new Rect(width-68,y,64,16),new GUIContent(""),(Color)SI.Color,true,true,true,new ColorPickerHDRConfig(0,99,0.01010101f,3));
								else
									SI.Color = (ShaderColor)EditorGUI.ColorField(new Rect(width-68,y,64,16),"",(Color)SI.Color);
							}
						GUI.backgroundColor = OldColor;
						
						if (SI.Type==ShaderInputTypes.Color||SI.Type==ShaderInputTypes.Vector){
							int offscreen = initEdit?0:1000;
							bool washDR = SI.IsHDR;
							//SI.IsHDR = GUI.Toggle(new Rect(117+left-offscreen,y,width-117-left-68-6,16),SI.IsHDR,"HDR",EMindGUI.ButtonImage);
							int blub = 0;
							if (SI.IsHDR)blub = 1;
							if (SI.Type==ShaderInputTypes.Vector)blub = 2;
							blub = EditorGUI.IntPopup(new Rect(117+left-offscreen,y,width-117-left-68-6,16), blub, new string[]{"LDR Color","HDR Color","Vector"}, new int[]{0,1,2}, EMindGUI.EditorPopup);
							
							if (blub==0){
								SI.Type=ShaderInputTypes.Color;
								SI.IsHDR = false;
							}
							if (blub==1){
								SI.Type=ShaderInputTypes.Color;
								SI.IsHDR = true;
							}
							if (blub==2){
								SI.Type=ShaderInputTypes.Vector;
								SI.IsHDR = true;
							}
								
							if (washDR!=SI.IsHDR&&SI.IsHDR==false){
								SI.Color.r = Mathf.Min(1f,SI.Color.r);
								SI.Color.g = Mathf.Min(1f,SI.Color.g);
								SI.Color.b = Mathf.Min(1f,SI.Color.b);
								SI.Color.a = Mathf.Min(1f,SI.Color.a);
							}
						}
						if (SI.Type==ShaderInputTypes.Vector){
							y+=19;
							float[] afawfdaw = new float[]{SI.Color.r,SI.Color.g,SI.Color.b,SI.Color.a};
							EditorGUI.MultiFloatField(new Rect(left,y,width-left-4,16), new GUIContent[]{new GUIContent("X"),new GUIContent("Y"),new GUIContent("Z"),new GUIContent("W")},afawfdaw);
							SI.Color.r = afawfdaw[0];
							SI.Color.g = afawfdaw[1];
							SI.Color.b = afawfdaw[2];
							SI.Color.a = afawfdaw[3];
						}
						
						if (SI.Type==ShaderInputTypes.Image||SI.Type==ShaderInputTypes.Cubemap){
							int offscreen = initEdit?0:1000;
							SI.IsHDR = GUI.Toggle(new Rect(117+left-offscreen,y,width-117-left-68-6,16),SI.IsHDR,"Expects HDR",EMindGUI.ButtonImage);
							if (SI.Type==ShaderInputTypes.Image){
								GUI.Label(new Rect(117+left-offscreen-48,y+16,48,16),"Default");
								SI.DefaultTexture = (DefaultTextures)EditorGUI.EnumPopup(new Rect(117+left-offscreen,y+16,width-117-left-68-6,16),SI.DefaultTexture,EMindGUI.EditorPopup);
							}
							if (SI.SeeTilingOffset){
								if (initEdit){
									if (GUI.Button(new Rect(left+2,y+32,12,32),"")){
										SI.SeeTilingOffset = false;
									}
									GUI.DrawTexture(new Rect(left+2,y+32-8,12,32+16),RightTriangle);
								}else{GUI.Button(new Rect(-1000,y+32,12,32),"");}
								float[] afawfdaw = new float[]{SI.Tiling.x,SI.Tiling.y};
								GUI.Label(new Rect(left+18,y+32,width-left-18-70-97,16),"Tiling");
								EditorGUI.MultiFloatField(new Rect(left+81,y+32,width-left-81-70,16), new GUIContent[]{new GUIContent("X"),new GUIContent("Y")},afawfdaw);
								SI.Tiling.x = afawfdaw[0];
								SI.Tiling.y = afawfdaw[1];
								afawfdaw[0] = SI.Offset.x;
								afawfdaw[1] = SI.Offset.y;
								GUI.Label(new Rect(left+18,y+32+17,width-34-70-left-18,16),"Offset");
								EditorGUI.MultiFloatField(new Rect(left+81,y+32+17,width-left-81-70,16), new GUIContent[]{new GUIContent("X"),new GUIContent("Y")},afawfdaw);
								SI.Offset.x = afawfdaw[0];
								SI.Offset.y = afawfdaw[1];
							}else{
								if (initEdit){
									if (GUI.Button(new Rect(width-68-16,y+32,12,32),"")){
										SI.SeeTilingOffset = true;
									}
									GUI.DrawTexture(new Rect(width-68-16,y+32-8,12,32+16),LeftTriangle);
								}else{GUI.Button(new Rect(-1000,y+32,12,32),"");}
							}
							y+=73-19;
						}
					}
					
					y+=19;
					
					EMindGUI.MouseDownIn(new Rect(0,y-SIheight,width,SIheight+1),true);
					return y;
	}
	Vector2 BreadScroll = new Vector2(0,0);
	Vector2 MessagesScroll = new Vector2(0,0);
	void GUIInputs(Vector2 WinSize,Vector2 Position){
		EMindGUI.BeginGroup(new Rect(Position.x,Position.y,WinSize.x,WinSize.y));
		if (OpenShader==null){
			if (SerializeProtection==""){
				GUIStage = GUIType.Start;
				GUITrans.Reset();
			}
		}
		else{
			float width = 296;
			//EMindGUI.BeginGroup(new Rect(Mathf.Round(WinSize.x/2f-296/2f),Position.y,WinSize.x,WinSize.y));
			EMindGUI.BeginGroup(new Rect(WinSize.x-width,69f,WinSize.x,WinSize.y));
				GUI.Box(new Rect(0,0,width,WinSize.y),"");
				GUI.Box(new Rect(1,0,width-1,44),"","IN BigTitle");
				EMindGUI.Label(new Rect(46,5,width-46,30),"My Example Material! :)",16);
				EMindGUI.Label(new Rect(46,26,width-46,30),"Shader",14);
				
				EMindGUI.MouseDownIn(new Rect(46,5,width,20),true);
				GUI.BeginGroup(new Rect(width-64,5,64,30));
				if (blah==null){
					blah = new Material(Shader.Find("Unlit/Color"));
				}
				EditorGUI.InspectorTitlebar(new Rect(-300,0,300+64-3,30),false,blah,false);
				GUI.EndGroup();
				
				GUI.DrawTexture(new Rect(3,0,46,46),Logo,ScaleMode.ScaleToFit);
				
				//EditorGUI.IntPopup(new Rect(94,26,width-94,15), 0, new string[]{OpenShader.ShaderName.Text,"There aren't any other shaders oh nooo"}, new int[]{0,0}, EMindGUI.EditorPopup);
				EditorGUI.IntPopup(new Rect(94,26,width-94-9,15), 0, new string[]{OpenShader.ShaderName.Text,"You're in a simulation :)"}, new int[]{0,0}, EMindGUI.EditorPopup);
				
				float y = 1;
				//int left = 16;
				float left = 16;
				bool HasEnteredInvisibles = false;
				bool HasEnteredVisibles = false;
				bool ScheduleSortFix = false;
				
				
				int totalheight = 20;
				
				foreach(ShaderInput SI in OpenShader.ShaderInputs){
					if (SI.InEditor){
						HasEnteredVisibles = true;
					}
					if (!SI.InEditor&&!HasEnteredInvisibles){
						HasEnteredInvisibles = true;
						if ((!HasEnteredVisibles&&(!InputDrag||!VisibleInputsWhenDragStarted))||(InputDrag&&!VisibleInputsWhenDragStarted))
							totalheight+=60;//For info
						totalheight+=20;
						totalheight+=20;
					}
					if (SI.Type==ShaderInputTypes.Image||SI.Type==ShaderInputTypes.Cubemap)
						totalheight+=73-19;
					if (SI.Type==ShaderInputTypes.Vector)
						totalheight+=19;
					totalheight+=19;
				}
				if (OpenShader.ShaderInputs.Count>0&&!HasEnteredInvisibles){
					if ((!HasEnteredVisibles&&(!InputDrag||!VisibleInputsWhenDragStarted))||(InputDrag&&!VisibleInputsWhenDragStarted))
							totalheight+=40;//For info
					totalheight+=20;
					if (!(InputDrag&&!VisibleInputsWhenDragStarted))
						totalheight += 70;
				}
				if (OpenShader.ShaderInputs.Count>0&&(!HasEnteredVisibles&&(!InputDrag||!VisibleInputsWhenDragStarted))){
					totalheight += 60;
				}
				if (OpenShader.ShaderInputs.Count==0){
					totalheight += 120;
				}
				float rwidth = width;
				if (WinSize.y-54-69<totalheight){
					width-=16;
				}
				InputsScroll = EMindGUI.BeginScrollView(new Rect(0,54,rwidth,WinSize.y-54-69-24),InputsScroll,new Rect(0,0,width,totalheight),false,false);
				if (InputDragged&&SRS.InputSelection!=null&&Event.current.type==EventType.MouseDrag){
					int newPos = 0;
					//int HiddenPos = OpenShader.ShaderInputs.Count;
					float HiddenY = 100000;
					int i = 0;
					HasEnteredInvisibles = false;
					HasEnteredVisibles = false;
					foreach(ShaderInput SI in OpenShader.ShaderInputs){
						i++;
						if (SI.InEditor){
							HasEnteredVisibles = true;
						}
						if (!SI.InEditor&&!HasEnteredInvisibles){
							if ((!HasEnteredVisibles&&(!InputDrag||!VisibleInputsWhenDragStarted))||(InputDrag&&!VisibleInputsWhenDragStarted))
								y+=60;//For info
							HasEnteredInvisibles = true;
							y+=20;
							HiddenY = y;
							y+=20;
							//HiddenPos = OpenShader.ShaderInputs.IndexOf(SI);
							//if ((HiddenY>(InputDragY+(Event.current.mousePosition.y-InputDragOffset.y)-9)))
							//	newPos--;
						}
						int hiddenoffset = 0;
						if ((i<OpenShader.ShaderInputs.Count&&(OpenShader.ShaderInputs[i]!=SRS.InputSelection)&&!OpenShader.ShaderInputs[i].InEditor&&!HasEnteredInvisibles)){
							hiddenoffset = 40;
							if ((!HasEnteredVisibles&&(!InputDrag||!VisibleInputsWhenDragStarted))||(InputDrag&&!VisibleInputsWhenDragStarted))
								hiddenoffset+=60;//For info
						}
						//if ((i>=2&&OpenShader.ShaderInputs[i-2].InEditor))
						//	hiddenoffset = -40;
						if (y<=(InputDragY+(Event.current.mousePosition.y-InputDragOffset.y)-19-hiddenoffset))
							newPos++;
						
						if (SI.Type==ShaderInputTypes.Image||SI.Type==ShaderInputTypes.Cubemap)
							y+=73-19;
						if (SI.Type==ShaderInputTypes.Vector)
							y+=19;
						y+=19;
					}
					if (!HasEnteredInvisibles){
						HiddenY = y+20;
						if ((!HasEnteredVisibles&&(!InputDrag||!VisibleInputsWhenDragStarted))||(InputDrag&&!VisibleInputsWhenDragStarted))
								HiddenY+=60;//For info
					}
					//Debug.Log(HiddenY);
					//Debug.Log((InputDragY+(Event.current.mousePosition.y-InputDragOffset.y)));
					//Debug.Log(newPos);
					//if (newPos>HiddenPos||((newPos>=HiddenPos)&&!HasEnteredInvisibles))
					if ((HiddenY<=(InputDragY+(Event.current.mousePosition.y-InputDragOffset.y))))//||((newPos>=HiddenPos)&&!HasEnteredInvisibles))
						SRS.InputSelection.InEditor = false;
					else
						SRS.InputSelection.InEditor = true;
					//if (newPos>OpenShader.ShaderInputs.IndexOf(SRS.InputSelection))
					//	newPos-=1;
					ShaderUtil.MoveItem(ref OpenShader.ShaderInputs,OpenShader.ShaderInputs.IndexOf(SRS.InputSelection),newPos);
					y = 1;
				}
				
				
				y = 2;
				//float oldY=y;
				
				HasEnteredInvisibles = false;
				HasEnteredVisibles = false;
				
				foreach(ShaderInput SI in OpenShader.ShaderInputs){
					//if (SI==null)
					//	Debug.Log("wtf");
					if (SI==null)
						continue;
					if (SI.InEditor){
						HasEnteredVisibles = true;
						if (HasEnteredInvisibles)
								ScheduleSortFix = true;
					}
					if (!SI.InEditor&&!HasEnteredInvisibles){
						if ((!HasEnteredVisibles&&(!InputDrag||!VisibleInputsWhenDragStarted))||(InputDrag&&!VisibleInputsWhenDragStarted))
							y+=60;//For info
						HasEnteredInvisibles = true;
						GUI.skin.label.alignment = TextAnchor.UpperCenter;
						y+=20;
						GUI.Label(new Rect(0,y,width,20),"Hidden Inputs");
						GUI.skin.label.alignment = TextAnchor.UpperLeft;
						y+=20;
					}
					
					if (InputDrag&&SRS.InputSelection==SI&&(Event.current.mousePosition-InputDragOffset).magnitude>2){
						if (!InputDragged)
							InputDragY = y+2;//-InputsScroll.y;
						InputDragged = true;
					}
					
					bool initDrag = InputDrag&&InputDragged&&SRS.InputSelection==SI;
					bool initEdit = InputEdit&&SRS.InputSelection==SI;
					
					int SIheight = 19;
					if ((SI.Type==ShaderInputTypes.Float||SI.Type==ShaderInputTypes.Range)&&initEdit)
						SIheight+=20;
					if (SI.Type==ShaderInputTypes.Image||SI.Type==ShaderInputTypes.Cubemap)
						SIheight=73;
					if (SI.Type==ShaderInputTypes.Vector)
						SIheight+=19;
					
					if (initDrag||initEdit){
						Color bgc = GUI.backgroundColor;
						GUI.backgroundColor = EMindGUI.BaseInterfaceColor*EMindGUI.BaseInterfaceColor;
						//GUI.Box(new Rect(left,y-2,width-left,(SI.Type==ShaderInputTypes.Image||SI.Type==ShaderInputTypes.Cubemap)?73:19),"",GUI.skin.GetStyle("SelectionRect"));
						if (Event.current.type==EventType.Repaint)
						GUI.skin.GetStyle("SelectionRect").Draw(new Rect(left,y-2,width-left,SIheight),false,false,false,false);
						GUI.backgroundColor = bgc;
					}
					if (!initDrag&&InputDrag&&SRS.InputSelection==SI){
						GUI.Box(new Rect(left,y-2,width-left,SIheight),"");
					}
					if (!initDrag)
						y = DrawInput(SI, y, SIheight, initDrag, initEdit, WinSize, width, left, HasEnteredVisibles);
					else
						y += SIheight;
				}
				
				if (OpenShader.ShaderInputs.Count>0&&!HasEnteredInvisibles){
					if ((!HasEnteredVisibles&&(!InputDrag||!VisibleInputsWhenDragStarted))||(InputDrag&&!VisibleInputsWhenDragStarted))
							y+=40;//For info
					GUI.skin.label.alignment = TextAnchor.UpperCenter;
					y+=20;
					GUI.Label(new Rect(0,y,width,20),"Hidden Inputs");
					GUI.skin.label.alignment = TextAnchor.UpperLeft;
					if (!(InputDrag&&!VisibleInputsWhenDragStarted))
					EditorGUI.HelpBox(new Rect(5,y+20,width-10,70), "Hidden inputs can be used to link properties together, animate them, replace them with masks, and probably some other fun stuff :)", MessageType.Info);
				}
				if (OpenShader.ShaderInputs.Count>0&&(!HasEnteredVisibles&&(!InputDrag||!VisibleInputsWhenDragStarted))){
					GUI.skin.label.alignment = TextAnchor.UpperCenter;
					GUI.Label(new Rect(0,0,width,20),"Visible Inputs");
					GUI.skin.label.alignment = TextAnchor.UpperLeft;
					EditorGUI.HelpBox(new Rect(5,0+20,width-10,40), "Visible inputs are what end up getting shown in Unity's material inspector :)", MessageType.Info);
				}
				
				
				if (OpenShader.ShaderInputs.Count==0){
					EditorGUI.HelpBox(new Rect(5,y,width-10,120), "No inputs have been added yet :(\n\nInputs primarily allow you to show and change properties of your shader in Unity's material inspector, but can also be used to animate and replace them.\n\nYou can add them to almost any setting by clicking the accompanying blue gear to bring up the inputs menu, and then the plus button :)", MessageType.Info);
				}
				EMindGUI.EndScrollView();
				EMindGUI.VerticalScrollbar(new Rect(0,54,rwidth,WinSize.y-54-69-24),InputsScroll,new Rect(0,0,width,totalheight),false);
				
				GUI.Box(new Rect(0,WinSize.y-69-24,rwidth,24),"");
				EMindGUI.MakeTooltip(0,new Rect(0,WinSize.y-69-24,24,24),"Deletes all unused inputs.");
				if (GUI.Button(new Rect(rwidth-100,WinSize.y-69-24,100,24),"Clean Inputs")){
					List<ShaderVar> SVs = ShaderUtil.GetAllShaderVars();
					foreach(ShaderInput SI in OpenShader.ShaderInputs){
						SI.UsedCount = 0;
					}
					foreach(ShaderVar SV in SVs){
						if (SV.Input!=null)
							SV.Input.UsedCount+=1;
					}
					for (int i = OpenShader.ShaderInputs.Count-1;i>-1;i--){
						ShaderInput SI = OpenShader.ShaderInputs[i];
						if (SI.UsedCount==0){
							InputDelete(SI);
						}
					}
					EditorGUIUtility.ExitGUI();
				}
				EMindGUI.MakeTooltip(0,new Rect(0,WinSize.y-69-24,24,24),"Delete Input");
				if (GUI.Button(new Rect(0,WinSize.y-69-24,24,24),new GUIContent(CurrentBin,"Delete Input"),EMindGUI.ButtonNoBorder)){
					//Debug.Log(SRS.InputSelection);
					//Debug.Log(InputEdit);
					if (SRS.InputSelection!=null&&InputEdit){
						InputDelete(SRS.InputSelection);
					}
				}
				EMindGUI.MouseDownIn(new Rect(0,WinSize.y-69-24,24,24));
			EMindGUI.EndGroup();
			

			
			if (Event.current.type==EventType.MouseDown){
				SRS.InputSelection = null;
			}
			
			ShaderBase SSIO = OpenShader;
			
			BaseSettingsHeight = 72;
			BaseSettingsHeight = 500;

			float BaseStuffWidth = WinSize.x-width-30;
			float BaseSettingsHeightforeal = WinSize.y-80-14;
			
			//(BoxHeight)*(float)Y+15f+144f+BaseSettingsHeight
			//float height = (BoxSize.y+15f)*(float)AdditionalHeight2+144f+BaseSettingsHeight+29f;
			//ConfigScroll = EMindGUI.BeginScroollView(new Rect(0,0,WinSize.x,WinSize.y),ConfigScroll,new Rect(0,0,WinSize.x-16,(BoxSize.y+15f)*6.2f+144f+29f+BaseSettingsHeight),false,true);
			//ConfigScroll = EMindGUI.BeginScroollView(new Rect(0,0,WinSize.x,WinSize.y),ConfigScroll,new Rect(0,0,WinSize.x-16,height),false,true);
			
			GUI.Box(new Rect(0,0,WinSize.x,70),"");
			SSIO.ShaderName.Text = EditorGUI.TextField(new Rect(15,30,WinSize.x-30,25),SSIO.ShaderName.Text,ShaderUtil.TextField18);
			
			
			

			//if (CurInterfaceStyle==InterfaceStyle.Flat)
			//	GUI.Box(new Rect(0,0,BaseStuffWidth,100+BaseSettingsHeight),"");
			//else
			//	GUI.Box(new Rect(0,0,BaseStuffWidth,100+BaseSettingsHeight),"","button");
			//GUI.skin.label.fontSize = 18;
			//GUI.Label(new Rect(15,30,200,40),"Shader Name: ");
			
			//SSIO.ShaderName.Text = GUI.TextField(new Rect(15,30,BaseStuffWidth-30-16,25),SSIO.ShaderName.Text,ShaderUtil.TextField18);

			//BaseSettingsHeight = 72;
			
			if (CurInterfaceStyle==InterfaceStyle.Flat)
				GUI.Box(new Rect(14,80,BaseStuffWidth+2,BaseSettingsHeightforeal-15+2),"");
			else
				GUI.Box(new Rect(14,80,BaseStuffWidth+2,BaseSettingsHeightforeal-15+2),"","button");
			
			BreadScroll = EMindGUI.BeginScrollView(new Rect(15,81,BaseStuffWidth,BaseSettingsHeightforeal-15),BreadScroll,new Rect(0,0,380+16,BaseSettingsHeightforeal-15-15),false,false);
			
			y = 16;//75+16;
			//GUI.Label(new Rect(16, y, 320,20),"How/where is the shader intented to be used?");y+=20;
			EMindGUI.SetLabelSize(22);
			Vector2 s = GUI.skin.label.CalcSize(new GUIContent("Shader Mode!"));
			//EMindGUI.Label(new Rect((int)((BaseStuffWidth-s.x)/2), y, 320,40),"Shader Mode!",22);y+=s.y+5;
			EMindGUI.Label(new Rect(16, y, 320,40),"Shader Mode!",22);y+=s.y+5;
			//SSIO.RenderingPathTarget.Draw(new Rect((int)((BaseStuffWidth-260)/2), y, 260,20));y+=25;
			SSIO.RenderingPathTarget.Draw(new Rect(16, y, 260,20));y+=25+5;
			
			int HW = (int)((BaseStuffWidth-32)/2-8);
			int HWL = HW+16;
			//GUI.Label(new Rect(16, y, 320,20),"When should this shader render?");y+=20;
			GUI.Label(new Rect(16, y, 80,20),"Queue");
				if (SSIO.TechQueue.Type==6){
					SSIO.TechQueue.Type=EditorGUI.Popup(new Rect(16+80, y, 32,16),SSIO.TechQueue.Type,SSIO.TechQueue.Names,EMindGUI.EditorPopup);
					SSIO.TechCustomQueue.Range0 = 0;
					SSIO.TechCustomQueue.Range1 = 5000;
					SSIO.TechCustomQueue.Draw(new Rect(16+32+16+80, y, HW-32-16-80,20));
					SSIO.TechCustomQueue.Float = Mathf.Round(SSIO.TechCustomQueue.Float);
				}else{
					SSIO.TechCustomQueue.Float = new float[]{2000,1000,2000,2450,3000,4000,2000}[SSIO.TechQueue.Type];
					SSIO.TechQueue.Type=EditorGUI.Popup(new Rect(16+80, y, HW-80,16),SSIO.TechQueue.Type,SSIO.TechQueue.Names,EMindGUI.EditorPopup);
				}
			//y+=25;
			
			//GUI.Label(new Rect(16, y, 380,20),"What type of object does this shader represent?");y+=20;
			GUI.Label(new Rect(HWL+16, y, 80,20),"Render Type");
				if (SSIO.TechReplacement.Type==11){
					SSIO.TechReplacement.Type=EditorGUI.Popup(new Rect(HWL+16+80, y, 32,16),SSIO.TechReplacement.Type,SSIO.TechReplacement.Names,EMindGUI.EditorPopup);
					SSIO.TechCustomReplacement.Text = EditorGUI.TextField(new Rect(HWL+16+32+16+80, y, HW-32-16-80,16),SSIO.TechCustomReplacement.Text,GUI.skin.textField);
				}else{
					SSIO.TechCustomReplacement.Text = SSIO.TechReplacement.CodeNames[SSIO.TechReplacement.Type];
					SSIO.TechReplacement.Type=EditorGUI.Popup(new Rect(HWL+16+80, y, HW-80,16),SSIO.TechReplacement.Type,SSIO.TechReplacement.Names,EMindGUI.EditorPopup);
				}
			y+=25;
			
			//-GUI.Label(new Rect(16, y, 420,20),"Which shader should be used if this one can't load?");y+=20;
			//GUI.Label(new Rect(16, y, 420,20),"Which shader should be used if this can't load?");y+=20;
			GUI.Label(new Rect(16, y, 80,20),"Fallback");
				if (SSIO.TechFallback.Type==3){
					SSIO.TechFallback.Type=EditorGUI.Popup(new Rect(16+80, y, 32,16),SSIO.TechFallback.Type,SSIO.TechFallback.Names,EMindGUI.EditorPopup);
					SSIO.TechCustomFallback.Text = EditorGUI.TextField(new Rect(16+80+32+16, y, HW-32-16-80,16),SSIO.TechCustomFallback.Text,GUI.skin.textField);
				}else{
					SSIO.TechCustomFallback.Text = SSIO.TechFallback.CodeNames[SSIO.TechFallback.Type];
					SSIO.TechFallback.Type=EditorGUI.Popup(new Rect(16+80, y, HW-80,16),SSIO.TechFallback.Type,SSIO.TechFallback.Names,EMindGUI.EditorPopup);
				}
			//y+=25;
			
			//GUI.Label(new Rect(16, y, 420,20),"LOD");y+=20;
				//SSIO.TechCustomFallback.Text = SSIO.TechFallback.CodeNames[SSIO.TechFallback.Type];
			GUI.Label(new Rect(HWL+16, y, 80,20),"LOD");
				SSIO.TechLOD.Range0 = 0;
				SSIO.TechLOD.Range1 = 1500;
				SSIO.TechLOD.Draw(new Rect(HWL+16+80, y, HW-80,16));
				SSIO.TechLOD.Float = Mathf.Round(SSIO.TechLOD.Float);
			y+=25;
			
			
			//if (TechQueue.Type<4&&SSIO.TechReplacementAuto.On&&(SSIO.TechReplacement.Type != SSIO.TechQueue.Type)){
			//	SSIO.TechReplacement.Type = SSIO.TechQueue.Type;
			//	GUI.changed = true;
			//}
			
			//TechFallback
			//GUI.Label(new Rect(24+1,75+8+24,220-16,20),"Render Type:");
			//GUI.enabled = !SSIO.TechReplacementAuto.On;
			//SSIO.TechReplacement.Type=EditorGUI.Popup(new Rect(24+36+45,75+8+24,220-16-45-32,20),SSIO.TechReplacement.Type,SSIO.TechReplacement.Names,EMindGUI.EditorPopup);
			//GUI.enabled = true;
			//SSIO.TechReplacementAuto.Draw(new Rect(24+36 + (220-16),75+8+24,220-16,20),"");
			
			FixScan(30);
			float oldy = y;
			//y+=25;
			//GUI.Box(new Rect(24+16, y, BaseStuffWidth-(24+16)*2,200),"Optimizations/Warnings",EMindGUI.SCSkin.window);
			if (Event.current.type==EventType.Repaint)
					EMindGUI.SCSkin.window.Draw(new Rect(16, y, BaseStuffWidth-(16+16)*2+16,220),"Optimizations/Warnings",false,true,true,true);
			MessagesScroll = EMindGUI.BeginScrollView(new Rect(16, y, BaseStuffWidth-(16+16)*2+16,200),MessagesScroll,new Rect(0, 0, BaseStuffWidth-(16+16)*2+16-15,OpenShader.ProposedFixes.Count*30+30),false,true);
			y = 20+5;
			Color oldcol = GUI.color;
			bool therewasoneyay = false;
			foreach(ShaderFix fix in OpenShader.ProposedFixes){
				if (fix.Ignore){
					if (ShowHiddenFixes)
						GUI.color = new Color(1f,1f,1f,0.5f);
					else
						continue;
				}
				GUI.skin.label.alignment = TextAnchor.MiddleLeft;
				//EditorGUI.SelectableLabel(new Rect(48, y-11, BaseStuffWidth-(24+16)*2-48-15,40),fix.MessageWName,GUI.skin.label);
				GUI.Label(new Rect(48, y-11, BaseStuffWidth-(16+16)*2-48-15+16,40),fix.MessageWName,GUI.skin.label);
				GUI.skin.label.alignment = TextAnchor.UpperLeft;
				
				Color oldg = GUI.contentColor;
				GUI.contentColor = EMindGUI.BaseInterfaceColorComp2;
				if (fix.Fix!=null&&GUI.Button(new Rect(0, y, 24,20),new GUIContent(Tick))){
					fix.Fix();LastFixesReloadTimeTemp = -1;
				}
				GUI.contentColor = EMindGUI.BaseInterfaceColorComp;
				if (GUI.Button(new Rect(23, y, 24,20),new GUIContent((fix.Ignore)?EyeOpen:CrossRed))){
					fix.Ignore = !fix.Ignore;LastFixesReloadTimeTemp = -1;
				}
				GUI.contentColor = oldg;
				GUI.color = oldcol;
				y+=30;
				therewasoneyay = true;
			}
			if (!therewasoneyay)
				GUI.Label(new Rect(16, y+5, BaseStuffWidth-(24+16)*2-16-15+16,120),
				"No optimizations or warnings found! This doesn't mean there aren't any, but the infinite wisdom of poorly written code couldn't find any :)");
			EMindGUI.EndScrollView();
			y = oldy;//75+16+25;
			//y+=25;
			EMindGUI.VerticalScrollbar(new Rect(16, y, BaseStuffWidth-(16+16)*2+16,200),MessagesScroll,new Rect(0, 0, BaseStuffWidth-(16+16)*2+16-15,OpenShader.ProposedFixes.Count*30+30),true);
			y+=200;
			SSIO.HardwareTarget.Draw(new Rect(16, y, Mathf.Min(320,BaseStuffWidth-(24+16)*2-110+16+16),20));
			//ShowHiddenFixes = GUI.Toggle(new Rect(24+16+BaseStuffWidth-(24+16)*2-110, y, 110,20),ShowHiddenFixes,new GUIContent("Show Hidden",ShowHiddenFixes?ShaderSandwich.Tick:ShaderSandwich.Cross),EMindGUI.SCSkin.window);
			ShowHiddenFixes = GUI.Toggle(new Rect(16+16+BaseStuffWidth-(24+16)*2-110+16, y, 110,20),ShowHiddenFixes,"Show Hidden",EMindGUI.SCSkin.window);
			
			EMindGUI.EndScrollView();
			EMindGUI.VerticalScrollbar(new Rect(15,75,BaseStuffWidth,BaseSettingsHeightforeal-15),BreadScroll,new Rect(0,0,BaseStuffWidth-15,BaseSettingsHeightforeal-15-15));
			/*SSIO.TechLOD.NoInputs = true;
			SSIO.TechLOD.Range0 = 0;
			SSIO.TechLOD.Range1 = 1000;
			SSIO.TechLOD.Float = Mathf.Round(SSIO.TechLOD.Float);
			SSIO.TechLOD.LabelOffset = 30;
			SSIO.TechLOD.Draw(new Rect(24+(230)+(200)+21,75+8,220-16,20),"LOD: ");
			
			
			SSIO.TechExcludeDX9.Draw(new Rect(24+(230)+(200)+21,75+8+24,220-16,20),"Exclude DX9: ");
			
			//TechFallback
			GUI.Label(new Rect(24+36,75+8,220-16,20),"Queue:");
			GUI.enabled = !SSIO.TechQueueAuto.On;
			SSIO.TechQueue.Type=EditorGUI.Popup(new Rect(24+36+45,75+8,220-16-45-32,20),SSIO.TechQueue.Type,SSIO.TechQueue.Names,EMindGUI.EditorPopup);
			GUI.enabled = true;
			SSIO.TechQueueAuto.Draw(new Rect(24+36 + (220-16),75+8,220-16,20),"");
			
			
			if (SSIO.TechReplacementAuto.On&&(SSIO.TechReplacement.Type != SSIO.TechQueue.Type)){
				SSIO.TechReplacement.Type = SSIO.TechQueue.Type;
				GUI.changed = true;
			}
			
			//TechFallback
			GUI.Label(new Rect(24+1,75+8+24,220-16,20),"Render Type:");
			GUI.enabled = !SSIO.TechReplacementAuto.On;
			SSIO.TechReplacement.Type=EditorGUI.Popup(new Rect(24+36+45,75+8+24,220-16-45-32,20),SSIO.TechReplacement.Type,SSIO.TechReplacement.Names,EMindGUI.EditorPopup);
			GUI.enabled = true;
			SSIO.TechReplacementAuto.Draw(new Rect(24+36 + (220-16),75+8+24,220-16,20),"");
			
			GUI.Label(new Rect(24+(240),75+8,220-16,20),"Color Space:");
			SSIO.MiscColorSpace.Type=EditorGUI.Popup(new Rect(24+(240)+74,75+8,220-16-45-32,20),SSIO.MiscColorSpace.Type,SSIO.MiscColorSpace.Names,EMindGUI.EditorPopup);
			
			//SSIO.TechQueue.Draw(new Rect(WinSize.x-220,30+25,220-16,20),"Queue: ");
			GUI.Label(new Rect(24+(240)+21,75+8+24,220-16,20),"Fallback:");
			if (SSIO.TechFallback.CodeNames[SSIO.TechFallback.Type]!="Custom"){
				SSIO.TechFallback.Type = EditorGUI.Popup(new Rect(24+(240)+74,75+8+24,220-16-45-32,20),SSIO.TechFallback.Type,SSIO.TechFallback.Names,EMindGUI.EditorPopup);
			}else{
				SSIO.TechFallback.Type = EditorGUI.Popup(new Rect(24+(240)+74,75+8+24,20,20),SSIO.TechFallback.Type,SSIO.TechFallback.Names,EMindGUI.EditorPopup);
				SSIO.TechCustomFallback.Text = GUI.TextField(new Rect(24+(240)+74+20,75+8+24,220-16-45-32,20),SSIO.TechCustomFallback.Text);
			}*/
			if (Event.current.type==EventType.MouseDown){
				EMindGUI.Defocus();
			}
			
			if (ScheduleSortFix){
				List<ShaderInput> oldorder = OpenShader.ShaderInputs;
				OpenShader.ShaderInputs = new List<ShaderInput>();
				foreach(ShaderInput SI in oldorder){
					if (SI.InEditor)
						OpenShader.ShaderInputs.Add(SI);
				}
				foreach(ShaderInput SI in oldorder){
					if (!SI.InEditor)
						OpenShader.ShaderInputs.Add(SI);
				}
			}
			if (SRS.InputSelection!=null&&InputDrag&&InputDragged){
				ShaderInput SI = SRS.InputSelection;
				
				int SIheight = 19;
				//if ((SI.Type==ShaderInputTypes.Float||SI.Type==ShaderInputTypes.Range))
				//	SIheight+=20;
				if (SI.Type==ShaderInputTypes.Image||SI.Type==ShaderInputTypes.Cubemap)
					SIheight=73;
				if (SI.Type==ShaderInputTypes.Image||SI.Type==ShaderInputTypes.Vector)
					SIheight+=19;
				
				bool InBin = false;
				if (Event.current.mousePosition.x<WinSize.x-rwidth)
					InBin = true;
				if (new Rect(WinSize.x-rwidth,WinSize.y-24,24,24).Contains(Event.current.mousePosition))
					InBin = true;
				
				GUI.enabled = !InBin;
				EMindGUI.BeginGroup(new Rect(Event.current.mousePosition.x-InputDragOffset.x,Event.current.mousePosition.y-InputDragOffset.y-2,10000,10000));
				GUI.Box(new Rect(left,InputDragY-2,width-left,SIheight),"");
				DrawInput(SI, InputDragY, SIheight, true, false, WinSize, width, left, HasEnteredVisibles);
				EMindGUI.EndGroup();
				GUI.enabled = true;
				
				if (Event.current.type==EventType.MouseUp){
					if (InBin){
						ShaderUtil.MoveItem(ref OpenShader.ShaderInputs,OpenShader.ShaderInputs.IndexOf(SRS.InputSelection),InputIndex);
						SRS.InputSelection.InEditor = InputInEditor;
						InputDelete(SRS.InputSelection);
					}
				}
			}
			if (Event.current.type==EventType.MouseUp){
				InputDrag = false;
				InputDragged = false;
			}
			ChangeSaveTemp(null,true);
		}
			
		EMindGUI.EndGroup();
	}
	public void FixScan(int t){
		if (Event.current.type==EventType.Repaint)
			LastFixesReloadTimeTemp-=1;
		if (LastFixesReloadTimeTemp<0||OpenShader.ProposedFixes==null){
			LastFixesReloadTimeTemp = t;
			if (OpenShader.ProposedFixes==null)
				OpenShader.ProposedFixes = new List<ShaderFix>();
			OpenShader.ProposedFixes.Clear();
			OpenShader.GetMyFixes(OpenShader.ProposedFixes);
		}
	}
	public bool ShowHiddenFixes = false;
	[NonSerialized]public int LastFixesReloadTimeTemp = 3;
	//None shall survive!!!!!
	void GUIInputs2(Vector2 WinSize,Vector2 Position){
		EMindGUI.BeginGroup(new Rect(Position.x,Position.y,WinSize.x,WinSize.y));
		if (OpenShader==null)
		{
			if (SerializeProtection==""){
				GUIStage = GUIType.Start;
				GUITrans.Reset();
			}
		}
		else
		{
		if (InputSwitch==null||InputSwitch.Count!=6){
			InputSwitch = new Dictionary<string,bool>();
			InputSwitch.Add("Type",false);
			InputSwitch.Add("Type2",false);
			InputSwitch.Add("Name",false);
			InputSwitch.Add("In Editor",false);
			InputSwitch.Add("Replacement",false);
			InputSwitch.Add("Value",false);
		}
		
		
		if (CurInterfaceStyle!=InterfaceStyle.Modern)
		if (GUI.Button( EMindGUI.Rect2(RectDir.Diag,WinSize.x,WinSize.y,100,30),"Layers",ShaderUtil.Button20))
			Goto(GUIType.Layers,ShaderTransition.TransDir.Forward);
		
//		int XX;
//		int YY;
		int X = -1;
//		int PerWidth = 3;
		GUI.skin.GetStyle("ButtonLeft").alignment = TextAnchor.MiddleLeft;
		GUI.skin.GetStyle("ButtonRight").alignment = TextAnchor.MiddleLeft;
		GUI.skin.GetStyle("ButtonMid").alignment = TextAnchor.MiddleLeft;
		
		//SeeShells = !GUI.Toggle(new Rect(251,10,100,25),!SeeShells,new GUIContent("Base",IconBase),GUI.skin.GetStyle("ButtonLeft"));
		//bool GUIChanged = GUI.changed;
		if (GUI.Button(new Rect(20,6,100,25),new GUIContent("Type"),GUI.skin.GetStyle("ButtonLeft"))){
			if (!InputSwitch["Type"])
			OpenShader.ShaderInputs = OpenShader.ShaderInputs.OrderBy(o=>o.Type).ToList();
			else
			OpenShader.ShaderInputs = OpenShader.ShaderInputs.OrderByDescending(o=>o.Type).ToList();
			InputSwitch["Type"] = !InputSwitch["Type"];
		}
		if (GUI.Button(new Rect(120,6,90,25),new GUIContent("Fallback"),GUI.skin.GetStyle("ButtonMid"))){
			if (!InputSwitch["Type2"])
			OpenShader.ShaderInputs = OpenShader.ShaderInputs.OrderBy(o=>o.Type).ToList();
			else
			OpenShader.ShaderInputs = OpenShader.ShaderInputs.OrderByDescending(o=>o.Type).ToList();
			InputSwitch["Type2"] = !InputSwitch["Type2"];
		}
		if (GUI.Button(new Rect(210,6,150,25),new GUIContent("Name"),GUI.skin.GetStyle("ButtonMid"))){
			if (!InputSwitch["Name"])
			OpenShader.ShaderInputs = OpenShader.ShaderInputs.OrderBy(o=>o.VisName).ToList();
			else
			OpenShader.ShaderInputs = OpenShader.ShaderInputs.OrderByDescending(o=>o.VisName).ToList();
			InputSwitch["Name"] = !InputSwitch["Name"];
		}
		if (GUI.Button(new Rect(360,6,60,25),new GUIContent("Visible"),GUI.skin.GetStyle("ButtonMid"))){
			if (!InputSwitch["In Editor"])
			OpenShader.ShaderInputs = OpenShader.ShaderInputs.OrderBy(o=>o.InEditor).ToList();
			else
			OpenShader.ShaderInputs = OpenShader.ShaderInputs.OrderByDescending(o=>o.InEditor).ToList();
			InputSwitch["In Editor"] = !InputSwitch["In Editor"];
		}
		if (GUI.Button(new Rect(420,6,140,25),new GUIContent("Replacement"),GUI.skin.GetStyle("ButtonRight"))){
			if (!InputSwitch["Replacement"])
			OpenShader.ShaderInputs = OpenShader.ShaderInputs.OrderBy(o=>o.SpecialType).ToList();
			else
			OpenShader.ShaderInputs = OpenShader.ShaderInputs.OrderByDescending(o=>o.SpecialType).ToList();
			InputSwitch["Replacement"] = !InputSwitch["Replacement"];
		}
		if (GUI.Button(new Rect(WinSize.x-160,6,140,25),"Clean Inputs")){
			List<ShaderVar> SVs = new List<ShaderVar>();
			foreach (ShaderLayer SL in ShaderUtil.GetAllLayers()){
				SL.UpdateShaderVars(true);
				SVs.AddRange(SL.ShaderVars);
			}
			SVs.AddRange(OpenShader.GetMyShaderVars());
			foreach(ShaderInput SI in OpenShader.ShaderInputs){
				SI.UsedCount = 0;
				foreach(ShaderVar SV in SVs){
					if (SV.Input==SI)
						SI.UsedCount+=1;
				}
			}
			for (int i = OpenShader.ShaderInputs.Count-1;i>-1;i--){
				ShaderInput SI = OpenShader.ShaderInputs[i];
				if (SI.UsedCount==0){
					OpenShader.ShaderInputs.Remove(SI);
					foreach(ShaderVar SV in SVs){
						if (SV.Input==SI)
							SV.Input = null;
					}
				}
			}
			EditorGUIUtility.ExitGUI();
		}
	//	GUI.changed = GUIChanged;
		//ButtonStyle = new GUIStayle(GUI.skin.button);
		//ButtonStyle.padding = new RectOffset(0,0,0,0);
		//ButtonStyle.margin = new RectOffset(0,0,0,0);
		if ((OpenShader.ShaderInputs.Count*50+36)>WinSize.y)
		InputScroll = EMindGUI.BeginScrollView(new Rect(0,36,WinSize.x,WinSize.y-36),InputScroll,new Rect(0,0,230,OpenShader.ShaderInputs.Count*50),false,true);
		else
		EMindGUI.BeginGroup(new Rect(0,36,WinSize.x,WinSize.y-36));
		foreach(ShaderInput SI in OpenShader.ShaderInputs){
			X++;//5
			//XX = X % PerWidth;//1 0,1,2,0,1
			//YY = (int)Mathf.Floor((float)X/(float)PerWidth);//1
			//ShaderUtil.DrawEffects(new Rect(0,0,350,350),SI.InputEffects,SelectedEffect);
			//if (SI.AutoCreated)
			//GUI.enabled = false;
			EMindGUI.BeginGroup(new Rect(20,X*50,WinSize.x-40,40),"button");
				if (SI.Type!=ShaderInputTypes.Float&&SI.Type!=ShaderInputTypes.Range){
				GUI.enabled = false;
				SI.Type = (ShaderInputTypes)EditorGUI.IntPopup(new Rect(10,10,80,20),(int)SI.Type,new string[]{"Color","Image","Cubemap","Float","Range"},new int[]{(int)ShaderInputTypes.Color,(int)ShaderInputTypes.Image,(int)ShaderInputTypes.Cubemap,(int)ShaderInputTypes.Float,(int)ShaderInputTypes.Range},GUI.skin.GetStyle("MiniPopup"));
				GUI.enabled = true;
				}
				else{
				SI.Type = (ShaderInputTypes)EditorGUI.IntPopup(new Rect(10,10,80,20),(int)SI.Type,new string[]{"Float","Range"},new int[]{(int)ShaderInputTypes.Float,(int)ShaderInputTypes.Range},GUI.skin.GetStyle("MiniPopup"));
				}
				//None,MainColor,MainTexture,BumpMap,MainCubemap,SpecularColor,Shininess,ReflectColor,Parallax,ParallaxMap
				string[] SInputTypes = new string[0];
				InputMainTypes[] EInputTypes = new InputMainTypes[0];
				SInputTypes = new string[]{"None","Custom"};
				EInputTypes = new InputMainTypes[]{InputMainTypes.None,InputMainTypes.Custom};
				if (SI.Type==ShaderInputTypes.Image){
					SInputTypes = new string[]{"None","Custom","Main Texture","Normal Map","Parallax Map","Terrain/Control","Terrain/Splat 0","Terrain/Splat 1","Terrain/Splat 2","Terrain/Splat 3","Terrain/Normal 0","Terrain/Normal 1","Terrain/Normal 2","Terrain/Normal 3"};
					EInputTypes = new InputMainTypes[]{InputMainTypes.None,InputMainTypes.Custom,InputMainTypes.MainTexture,InputMainTypes.BumpMap,InputMainTypes.ParallaxMap,InputMainTypes.TerrainControl,InputMainTypes.TerrainSplat0,InputMainTypes.TerrainSplat1,InputMainTypes.TerrainSplat2,InputMainTypes.TerrainSplat3,InputMainTypes.TerrainNormal0,InputMainTypes.TerrainNormal1,InputMainTypes.TerrainNormal2,InputMainTypes.TerrainNormal3};
				}
				if (SI.Type==ShaderInputTypes.Color){
					SInputTypes = new string[]{"None","Custom","Main Color","Specular Color","Reflect Color"};
					EInputTypes = new InputMainTypes[]{InputMainTypes.None,InputMainTypes.Custom,InputMainTypes.MainColor,InputMainTypes.SpecularColor,InputMainTypes.ReflectColor};
				}
				if (SI.Type==ShaderInputTypes.Cubemap){
					SInputTypes = new string[]{"None","Custom","Main Cubemap"};
					EInputTypes = new InputMainTypes[]{InputMainTypes.None,InputMainTypes.Custom,InputMainTypes.MainCubemap};
				}
				if (SI.Type==ShaderInputTypes.Float||SI.Type==ShaderInputTypes.Range){
					SInputTypes = new string[]{"None","Custom","Shininess","Parallax","Cutoff","Shell Distance"};
					EInputTypes = new InputMainTypes[]{InputMainTypes.None,InputMainTypes.Custom,InputMainTypes.Shininess,InputMainTypes.Parallax,InputMainTypes.Cutoff,InputMainTypes.ShellDistance};
				}
				int IndexOfType=0;
				if (Array.IndexOf(EInputTypes,SI.MainType)!=-1)
				IndexOfType = Array.IndexOf(EInputTypes,SI.MainType);
				
				if (SI.MainType == InputMainTypes.Custom){
					SI.MainType = EInputTypes[EditorGUI.Popup(new Rect(100,10,20,20),"",IndexOfType,SInputTypes,GUI.skin.GetStyle("MiniPopup"))];
					SI.CustomFallback.Text = GUI.TextField(new Rect(120,10,60,20),SI.CustomFallback.Text);
				}
				else{
					SI.MainType = EInputTypes[EditorGUI.Popup(new Rect(100,10,80,20),"",IndexOfType,SInputTypes,GUI.skin.GetStyle("MiniPopup"))];
				}
				//SI.MainType = (InputMainTypes)EditorGUI.EnumPopup(new Rect(100,10,80,20),"",SI.MainType);
				SI.VisName = GUI.TextField(new Rect(190,10,140,20),SI.VisName);
				//if (Event.current.type==EventType.Repaint){
				if (SI.Type==ShaderInputTypes.Image||SI.Type==ShaderInputTypes.Cubemap){
				SI.InEditor = true;
				GUI.enabled = false;
				}
					if (SI.InEditor)
						SI.InEditor = GUI.Toggle(new Rect(340,10,20,20),SI.InEditor,ShaderSandwich.Tick,EMindGUI.ButtonNoBorder);
					else
						SI.InEditor = GUI.Toggle(new Rect(340,10,20,20),SI.InEditor,ShaderSandwich.Cross,EMindGUI.ButtonNoBorder);
				if (SI.Type==ShaderInputTypes.Image||SI.Type==ShaderInputTypes.Cubemap){
					GUI.enabled = true;
				}
				//None,Time,TimeFast,TimeSlow,SinTime,SinTimeFast,SinTimeSlow,CosTime,CosTimeFast,CosTimeSlow
				InputSpecialTypes[] EInputTypes2 = new InputSpecialTypes[0];
				SInputTypes = new string[]{"None","Custom","Mask"};
				EInputTypes2 = new InputSpecialTypes[]{InputSpecialTypes.None,InputSpecialTypes.Custom,InputSpecialTypes.Mask};
				if (SI.Type==ShaderInputTypes.Float||SI.Type==ShaderInputTypes.Range){
					SInputTypes = new string[]{"None","Custom","Mask","Time/Basic/Standard","Time/Basic/Fast","Time/Basic/Slow","Time/Basic/Very Slow","Time/Sine/Standard","Time/Sine/Fast","Time/Sine/Slow","Time/Cosine/Standard","Time/Cosine/Fast","Time/Cosine/Slow","Time/Clamped Sine/Standard","Time/Clamped Sine/Fast","Time/Clamped Sine/Slow","Time/Clamped Cosine/Standard","Time/Clamped Cosine/Fast","Time/Clamped Cosine/Slow","Depth/Shell","Depth/Parallax Occlusion Mapping"};
					EInputTypes2 = new InputSpecialTypes[]{};//InputSpecialTypes.None,InputSpecialTypes.Custom,InputSpecialTypes.Mask,InputSpecialTypes.Time,InputSpecialTypes.TimeFast,InputSpecialTypes.TimeSlow,InputSpecialTypes.TimeVerySlow,InputSpecialTypes.SinTime,InputSpecialTypes.SinTimeFast,InputSpecialTypes.SinTimeSlow,InputSpecialTypes.CosTime,InputSpecialTypes.CosTimeFast,InputSpecialTypes.CosTimeSlow, InputSpecialTypes.ClampedSinTime,InputSpecialTypes.ClampedSinTimeFast,InputSpecialTypes.ClampedSinTimeSlow,InputSpecialTypes.ClampedCosTime,InputSpecialTypes.ClampedCosTimeFast,InputSpecialTypes.ClampedCosTimeSlow,InputSpecialTypes.ShellDepth,InputSpecialTypes.ParallaxDepth};
				}
				
				IndexOfType=0;
				if (Array.IndexOf(EInputTypes2,SI.SpecialType)!=-1)
				IndexOfType = Array.IndexOf(EInputTypes2,SI.SpecialType);
				
				if (SI.InEditor){
					SI.SpecialType = InputSpecialTypes.None;
					GUI.enabled = false;
					EditorGUI.Popup(new Rect(400,10,140,20),"",IndexOfType,SInputTypes,GUI.skin.GetStyle("MiniPopup"));
					GUI.enabled = true;
				}
				else{
					if (SI.SpecialType == InputSpecialTypes.Custom){
						SI.SpecialType = EInputTypes2[EditorGUI.Popup(new Rect(400,10,20,20),"",IndexOfType,SInputTypes,GUI.skin.GetStyle("MiniPopup"))];
						SI.CustomSpecial.Text = GUI.TextField(new Rect(420,10,120,20),SI.CustomSpecial.Text);
					}
					else
					if (SI.SpecialType == InputSpecialTypes.Mask){
						SI.SpecialType = EInputTypes2[EditorGUI.Popup(new Rect(400,10,20,20),"",IndexOfType,SInputTypes,GUI.skin.GetStyle("MiniPopup"))];
						//SI.Mask.SetToMasks(null,0);
						//SI.Mask.HookIntoObjects();
						SI.Mask.Draw(new Rect(420,10,120,20),"");
					}
					else{
						SI.SpecialType = EInputTypes2[EditorGUI.Popup(new Rect(400,10,140,20),"",IndexOfType,SInputTypes,GUI.skin.GetStyle("MiniPopup"))];
					}
					
				}
				
				GUI.enabled = true;
				
				if (SI.Type==ShaderInputTypes.Float){//Float
					SI.Number = EditorGUI.FloatField(new Rect(550,10,120,20),"",SI.Number,GUI.skin.textField);
				}
				if (SI.Type==ShaderInputTypes.Range){//Range
					SI.Range0 = EditorGUI.FloatField(new Rect(550,10,30,20),"",SI.Range0,GUI.skin.textField);
					SI.Number = GUI.HorizontalSlider(new Rect(590,10,40,20),SI.Number,SI.Range0,SI.Range1);
					SI.Range1 = EditorGUI.FloatField(new Rect(640,10,30,20),"",SI.Range1,GUI.skin.textField);
				}
				Color OldColor = GUI.backgroundColor;
				//GUI.color = new Color(1,1,1,1);
				GUI.backgroundColor = new Color(1,1,1,1);
				if (SI.Type==ShaderInputTypes.Image){//Image
					SI.Image = (Texture2D) EditorGUI.ObjectField (new Rect(550,10,120,20),SI.ImageS(), typeof (Texture2D),false);
				}
				if (SI.Type==ShaderInputTypes.Color){//Color
					SI.Color = (ShaderColor)EditorGUI.ColorField(new Rect(550,10,140,20),"",(Color)SI.Color);
				}
				if (SI.Type==ShaderInputTypes.Cubemap){//Cubemap
					SI.Cube = (Cubemap) EditorGUI.ObjectField (new Rect(550,10,120,20),SI.CubeS(), typeof (Cubemap),false);
				}
				GUI.backgroundColor = OldColor;
				
				
				if (SI.Type==ShaderInputTypes.Image){
					GUI.Label(new Rect(680,10,200,20),"Normal Map: ");
						if (SI.NormalMap)
						SI.NormalMap = GUI.Toggle(new Rect(770,10,20,20),SI.NormalMap,ShaderSandwich.Tick,EMindGUI.ButtonNoBorder);
					else
						SI.NormalMap = GUI.Toggle(new Rect(770,10,20,20),SI.NormalMap,ShaderSandwich.Cross,EMindGUI.ButtonNoBorder);				
				}
				//InputSpecialTypes
				//UnityEngine.Debug.Log("ASD");
				if (X==0)
				GUI.enabled = false;
				if (GUI.Button(new Rect(WinSize.x-40-75,10,20,20),UPArrow,EMindGUI.ButtonNoBorder))
				{
					ShaderUtil.MoveItem(ref OpenShader.ShaderInputs,X,X-1);//MoveLayerUp = Pos;
					EditorGUIUtility.ExitGUI();
				}
				GUI.enabled = true;
				if (GUI.Button(new Rect(WinSize.x-40-50,10,20,20),CrossRed,EMindGUI.ButtonNoBorder))
				{
					OpenShader.ShaderInputs.RemoveAt(X);
					List<ShaderVar> SVs = new List<ShaderVar>();
					foreach (ShaderLayer SL in ShaderUtil.GetAllLayers()){
						SL.UpdateShaderVars(true);
						SVs.AddRange(SL.ShaderVars);
					}
					SVs.AddRange(OpenShader.GetMyShaderVars());
					foreach(ShaderVar SV in SVs){
						if (SV.Input==SI)
						SV.Input = null;
					}
					//UEObject.DestroyImmediate(SL,false);
					EditorGUIUtility.ExitGUI();
				}
				if (X>=OpenShader.ShaderInputs.Count-1)
				GUI.enabled = false;
				if (GUI.Button(new Rect(WinSize.x-40-25,10,20,20),DOWNArrow,EMindGUI.ButtonNoBorder))
				{
					//Pos2+=1;	
					ShaderUtil.MoveItem(ref OpenShader.ShaderInputs,X,X+1);//MoveLayerDown = Pos;
					EditorGUIUtility.ExitGUI();
				}
				GUI.enabled = true;				
				//SI.InEditor = GUI.Toggle(new Rect(250,10,20,20),SI.InEditor);
			EMindGUI.EndGroup();
			GUI.enabled = true;
		}
		if ((OpenShader.ShaderInputs.Count*50+36)>WinSize.y){
			EMindGUI.EndScrollView();
			EMindGUI.VerticalScrollbar(new Rect(0,36,WinSize.x,WinSize.y-36),InputScroll,new Rect(0,0,230,OpenShader.ShaderInputs.Count*50));
		}
		else
		EMindGUI.EndGroup();
		/*Features:
		Editor Inputs
		Inputs: (Composed of editor inputs and custom values)
			Can apply effects to inputs
			
		Interface:
		Two Tabs: Primary and secondary inputs.
		
		*/
		ChangeSaveTemp(null,true);
		if (CurInterfaceStyle!=InterfaceStyle.Modern)
		if (GUI.Button( EMindGUI.Rect2(RectDir.Diag,WinSize.x,WinSize.y,100,30),"Layers",ShaderUtil.Button20))
			Goto(GUIType.Layers,ShaderTransition.TransDir.Forward);
		//GUI.Toggle( EMindGUI.Rect2(RectDir.Diag,WinSize.x,WinSize.y,100,30),GUIMouseHold&&(EMindGUI.Rect2(RectDir.Diag,WinSize.x,WinSize.y,100,30).Contains(Event.current.mousePosition)),"Layers",ButtonStyle);
		}
		EMindGUI.EndGroup();
	}
	
	static public int ShaderLayerTab = 0;
	
	//public ShaderSource SourceTest
	//void GUISourceEditor(Vector2 WinSize,Vector2 Position){
		
	//}
	
	
	float binAlp = 0f;
	Texture2D CurrentBin = null;
	int LastFunBin = 0;//What am I doing...
	void GUILayers(Vector2 WinSize,Vector2 Position){
//		Debug.Log(SRS.LayerSelection);
		//RunHotkeys();
		ShaderBase SSIO = OpenShader;
		if (SSIO==null){
			if (SerializeProtection==""){
				GUIStage = GUIType.Start;
				GUITrans.Reset();
			}
		}
		else{
			if (!LayerDrag)
				Undo.RecordObject(SRS,"Selected Layer");
			
			FixScan(100);
			
			EMindGUI.BeginGroup(new Rect(Position.x,Position.y,WinSize.x,WinSize.y));

			int ListCount = 0;
			ShaderPass SP = null;
			if (ShaderLayerTab > SSIO.ShaderPasses.Count-1)
			ShaderLayerTab = SSIO.ShaderPasses.Count-1;
			if (ShaderLayerTab > -1&&SSIO.ShaderPasses.Count>ShaderLayerTab)
			SP = SSIO.ShaderPasses[ShaderLayerTab];
			if (SP!=null){
				foreach(ShaderIngredient surface in SP.Surfaces){
					ListCount += surface.GetMyShaderLayerLists().Count;
				}
				foreach(ShaderIngredient geometryModifier in SP.GeometryModifiers){
					ListCount += geometryModifier.GetMyShaderLayerLists().Count;
				}
				//ListCount+=1;
				//if (SP.SpecularOn.On==true)
				//	ListCount+=1;
				//if (SP.EmissionOn.On==true)
				//	ListCount+=1;
				//if (SP.IndirectSubsurfaceScatteringOn.On==true)
				//	ListCount+=1;
				//if (SP.TransparencyOn.On==true)
				//	ListCount+=1;
				//if (SP.ParallaxOn.On==true)
				//	ListCount+=1;
				//ListCount+=1;
				
				//ListCount+=1;
				//ListCount+=1;
				//ListCount+=1;
				//ListCount+=1;
			}
			if (ShaderLayerTab == -1){
				//foreach(ShaderLayerList SLL in SSIO.ShaderLayersMasks){
				//	ListCount+=1;
				//}
				ListCount += SSIO.ShaderLayersMasks.Count;
				ListCount+=1;
			}
			
			LayerListScroll = EMindGUI.BeginScrollView(new Rect(264,0,WinSize.x-264,WinSize.y),LayerListScroll,new Rect(0,0,ListCount*150,WinSize.y-20),false,false);
			
			if (LayerHasDragged&&LayerDrag&&Event.current.mousePosition.x>ListCount*150){
				if (Event.current.type==EventType.Repaint)
					binAlp = Mathf.Lerp(binAlp,0.5f,0.2f);
				LayerIsOverBin = true;
			}else{
				if (Event.current.type==EventType.Repaint)
					binAlp = Mathf.Lerp(binAlp,0f,0.2f);
			}
			if (binAlp>0.0001f){
				Color asd = GUI.color;
				GUI.color = new Color(1f,1f,1f,binAlp);
				if (CurrentBin!=null)
					GUI.DrawTexture(new Rect(ListCount*150+(WinSize.x-264-ListCount*150)/2-64,WinSize.y/2-64,128,128), CurrentBin);
				GUI.color = asd;
			}else if (!LayerDrag){CurrentBin = Bin;}
			/*GUIStyle ButtonStyle;
			if(CurInterfaceStyle==InterfaceStyle.Flat||CurInterfaceStyle==InterfaceStyle.Modern){
				ButtonStyle = new GUIStayle(GUI.skin.box);
				ButtonStyle.alignment = TextAnchor.MiddleCenter;
			}
			else
				ButtonStyle = new GUIStayle(GUI.skin.button);*/
			
			ShaderUtil.BoxMiddle20.fontSize = 20;
			
			List<List<ShaderLayer>> SLList = new List<List<ShaderLayer>>();
			GUI.color = BackgroundColor;
			//GUI.DrawTexture( new Rect(264,30,300,25), EditorGUIUtility.whiteTexture );
			GUI.color = new Color(1f,1f,1f,1f);
			if (Event.current.type==EventType.MouseUp&&LayerDrag){
				//Debug.Log(LayerDrag);
				//Debug.Log(LayerDragSelectedLastPar);
				//Debug.Log(SRS.LayerSelection);
				//if (SRS.LayerSelection!=null)
				//	Debug.Log(SRS.LayerSelection.Parent);
				if (SSIO.ShaderLayersMaskTemp.SLs.Count>0){//Some weirdo dragged a layer onto it I guess...
					List<ShaderLayer> tempmaskslayers = OpenShader.ShaderLayersMaskTemp.SLs;
					OpenShader.AddMask();
					OpenShader.ShaderLayersMasks[OpenShader.ShaderLayersMasks.Count-1].SLs = tempmaskslayers;
				}
				if (LayerIsOverBin){
					//Debug.Log("asasd");
					LayerDelete(SRS.LayerSelection);
				}else
				if (SRS.LayerSelection!=null&&SRS.LayerSelection.Parent==null){
					//if (!(Event.current.mousePosition.x>ListCount*150-LayerListScroll.x)){
						//Debug.Log("oops");
						SRS.LayerSelection.Parent = LayerDragSelectedLastPar;
						LayerDragSelectedLastPar.SLs.Insert(LayerDragSelectedLastParIndex,SRS.LayerSelection);//Double check if it isnt already there//Actually don't since it's already removed...
						CurrentBin = Bin;
					//}else{
					//	Debug.Log("asasd");
					//	LayerDelete(SRS.LayerSelection);
					//}
				}
					
					
				LayerDrag = false;
				LayerHasDragged = false;
				GUI.changed = true;
				
			}
			LayerIsOverBin = false;
			int Pos = 0;
			bool Hit = false;
			if (SP!=null){
				//if (!LayerDrag&&SRS.LayerSelection!=null&&(Event.current.type==EventType.MouseDown||Event.current.type==EventType.MouseUp||Event.current.type==EventType.MouseDrag||Event.current.type==EventType.KeyDown||Event.current.type==EventType.KeyUp||Event.current.type==EventType.ContextClick))///TOoDO:SuPER SLOW!!!
				if (!LayerDrag&&SRS.LayerSelection!=null&&(Event.current.type!=EventType.Repaint&&Event.current.type!=EventType.Layout))///TOoDO:SuPER SLOW!!!
				UnityEditor.Undo.RecordObject((UnityEngine.Object)(SRS.LayerSelection),"Change Layer");
				foreach(ShaderIngredient surface in SP.Surfaces){
					foreach(ShaderLayerList SLL in surface.GetMyShaderLayerLists()){
						if(GUILayersBox(SP,SLL,Pos,WinSize,ShaderUtil.BoxMiddle20))Hit = true;
						SLList.Add(SLL.SLs);
						Pos+=1;
					}
				}
				foreach(ShaderIngredient geometryModifier in SP.GeometryModifiers){
					foreach(ShaderLayerList SLL in geometryModifier.GetMyShaderLayerLists()){
						if(GUILayersBox(SP,SLL,Pos,WinSize,ShaderUtil.BoxMiddle20))Hit = true;
						SLList.Add(SLL.SLs);
						Pos+=1;
					}
				}
			}
			if (ShaderLayerTab == -1){
				if ((Event.current.type == EventType.MouseDown&&new Rect(150*(Pos+SSIO.ShaderLayersMasks.Count),30,150,WinSize.y).Contains(Event.current.mousePosition)))
				OpenShader.AddMask();
				foreach(ShaderLayerList SLL in SSIO.ShaderLayersMasks){
					if(GUILayersBox(null,SLL,Pos,WinSize,ShaderUtil.BoxMiddle20))Hit = true;
					SLList.Add(SLL.SLs);
					
					Pos+=1;
				}
				if (!(Event.current.type == EventType.Repaint&&new Rect(150*Pos,30,150,WinSize.y).Contains(Event.current.mousePosition)))GUI.enabled = false;
				GUILayersBox(null,SSIO.ShaderLayersMaskTemp,Pos,WinSize,ShaderUtil.BoxMiddle20);
				
				//if (SSIO.ShaderLayersMaskTemp.SLs.Count==0)// Hasn't dragged a layer onto it
				if (!GUI.enabled||!LayerDrag)// Hasn't dragged a layer onto it
					GUI.Label(new Rect(150*Pos+18,120,150,WinSize.y),"Click to Add Mask");
				
				GUI.enabled = true;
				Pos+=1;
			}
				if ((LayerDrag||LayerHasDragged)&&SRS.LayerSelection!=null&&((Event.current.mousePosition-LayerDragOffset).magnitude>2||LayerHasDragged)){
					if (!LayerHasDragged){
						//if (SRS.LayerSelection!=SL){
							UnityEditor.Undo.IncrementCurrentGroup();
							UnityEditor.Undo.SetCurrentGroupName("Drag Layer");
						//	UnityEditor.Undo.RegisterCompleteObjectUndo((UnityEngine.Object)SRS,"Drag Layer");
						//}
						
						if (Event.current.button == 0){
							if (ShaderLayerTab != -1){
								foreach(ShaderIngredient surface in SP.Surfaces){
									foreach(ShaderLayerList SLL in surface.GetMyShaderLayerLists()){
										UnityEditor.Undo.RegisterCompleteObjectUndo((UnityEngine.Object)SLL,"Drag Layer");
									}
								}
								foreach(ShaderIngredient geometryModifier in SP.GeometryModifiers){
									foreach(ShaderLayerList SLL in geometryModifier.GetMyShaderLayerLists()){
										UnityEditor.Undo.RegisterCompleteObjectUndo((UnityEngine.Object)SLL,"Drag Layer");
									}
								}
							}else{
								foreach(ShaderLayerList SLL in OpenShader.ShaderLayersMasks){
									UnityEditor.Undo.RegisterCompleteObjectUndo((UnityEngine.Object)SLL,"Drag Layer");
								}
							}
						}
					}
					LayerHasDragged = true;
					int height = 80;
					int heightpad = height+10;
					
					//int left = (150-height-32)/2+16-15/2;
					//if ((X)==SRS.LayerSelection.x&&(Pos)==SRS.LayerSelection.y){
					Vector2 newPos;
						//if ((Event.current.mousePosition-LayerDragOffset).magnitude>2)
							newPos = LayerDragOriginal + Event.current.mousePosition-LayerDragOffset;
						//else
						//	newPos = LayerDragOriginal;
										float alp = 1f;
					if (SRS.LayerSelection.Parent==null||LayerIsOverBin)
						alp = 0.3f;
					Color asdasd = GUI.color;
					GUI.color = asdasd*new Color(1f,1f,1f,alp);
					
					SRS.LayerSelection.DrawIcon(new Rect(newPos.x,newPos.y,height,height),true);
					GUI.skin.label.alignment = TextAnchor.UpperCenter;
					
					if (ViewLayerNames){
						GUI.color = new Color(0f,0f,0f,0.3f*alp);
						
						if (Event.current.type==EventType.Repaint){
							LayerDragWidth = Mathf.Lerp(LayerDragWidth,height-16,0.2f);
							LayerDragLeft = Mathf.Lerp(LayerDragLeft,8,0.2f);
						}
						
						EMindGUI.SetLabelSize(11);
						float h = GUI.skin.label.CalcHeight(new GUIContent(SRS.LayerSelection.Name.Text),LayerDragWidth);
						//Why -4?
						GUI.Box(new Rect(newPos.x+LayerDragLeft,newPos.y+height-4,LayerDragWidth,h),"");
						GUI.color = asdasd*new Color(1f,1f,1f,alp);
						
						GUI.color = EMindGUI.BaseInterfaceColor;
						EMindGUI.Label(new Rect(newPos.x+LayerDragLeft,newPos.y+height-4,LayerDragWidth,heightpad),SRS.LayerSelection.Name.Text,11);
						EMindGUI.SetLabelSize(12);
					}
					GUI.skin.label.alignment = TextAnchor.UpperLeft;
					
					
					GUI.color = EMindGUI.BaseInterfaceColor*new Color(1f,1f,1f,alp);
					
					//Rect dragArea = new Rect(newPos.x+90,newPos.y,16,100);
					Rect dragArea = new Rect(newPos.x,newPos.y,height,height);

					//GUI.DrawTexture(dragArea,DragAreaTex2,ScaleMode.ScaleAndCrop);
					if ((Event.current.mousePosition-LayerDragOffset).magnitude>2)
					EditorGUIUtility.AddCursorRect(dragArea,MouseCursor.Pan);
					GUI.color = asdasd;
					//LayerDragOriginal;
				}
			
			
			EMindGUI.EndScrollView();
			
			GUI.skin.GetStyle("ButtonLeft").alignment = TextAnchor.MiddleLeft;
			GUI.skin.GetStyle("ButtonRight").alignment = TextAnchor.MiddleLeft;
			GUI.skin.GetStyle("ButtonMid").alignment = TextAnchor.MiddleLeft;
			
			float Val = EMindGUI.GetVal(InterfaceColor);
			Color BackgroundColor2;
			if (CurInterfaceStyle==InterfaceStyle.Modern){
				if (Val<0.5f)
					BackgroundColor2 = new Color(0.6f*Val,0.6f*Val,0.6f*Val,1);
				else
					BackgroundColor2 = new Color(1f*Val,1f*Val,1f*Val,1);
			}
			else
			BackgroundColor2 = new Color(0.18f,0.18f,0.18f,1);
			Color GUIColor = GUI.color;
			GUI.color = BackgroundColor2;
			GUI.DrawTexture( new Rect(250+16,0,WinSize.x-250-16,32), EditorGUIUtility.whiteTexture );
			GUI.color = GUIColor;
			int X = 265;
			int i = -1;
			foreach (ShaderPass SPd in OpenShader.ShaderPasses){
				i++;
				if (ShaderLayerTab != i){
					if (GUI.Button(new Rect(X,6,100,25),new GUIContent(SPd.Name.Text,SPd.Visible.On?(IconBase):CrossRed),GUI.skin.GetStyle("ButtonMid"))){
						ShaderLayerTab = i;
						if (OpenShader.ShaderPasses[i].Surfaces.Count>0&&OpenShader.ShaderPasses[i].Surfaces[0].GetMyShaderLayerLists().Count>0&&OpenShader.ShaderPasses[i].Surfaces[0].GetMyShaderLayerLists()[0].SLs.Count>0)
							SRS.LayerSelection = OpenShader.ShaderPasses[i].Surfaces[0].GetMyShaderLayerLists()[0].SLs[0];
						else
							SRS.LayerSelection = null;
					}
					//if (GUI.Button(new Rect(X,6,100,25),new GUIContent(SPd.Name.Text,SPd.Visible.On?(SPd.ShellsOn.On?IconShell:IconBase):CrossRed),GUI.skin.GetStyle("ButtonMid")))ShaderLayerTab = i;
				}
				else{
					GUI.Toggle(new Rect(X,6,100,25),true,new GUIContent(SPd.Name.Text,SPd.Visible.On?(IconBase):CrossRed),GUI.skin.GetStyle("ButtonMid"));
					//GUI.Toggle(new Rect(X,6,100,25),true,new GUIContent(SPd.Name.Text,SPd.Visible.On?(SPd.ShellsOn.On?IconShell:IconBase):CrossRed),GUI.skin.GetStyle("ButtonMid"));
				}
				X += 100;
			}
			
			GUI.enabled = true;
			
			if (ShaderLayerTab != -1){
				EMindGUI.MakeTooltip(0,new Rect(X+6,6+2,18,18),"Not sure why I'm still using this icon...XD");
				if (GUI.Button(new Rect(X,6,100,25),new GUIContent("Mask",IconMask),GUI.skin.GetStyle("ButtonRight"))){
					ShaderLayerTab = -1;
					if (OpenShader.ShaderLayersMasks.Count>0&&OpenShader.ShaderLayersMasks[0].SLs.Count>0)
						SRS.LayerSelection = OpenShader.ShaderLayersMasks[0].SLs[0];
					else
						SRS.LayerSelection = null;
				}
			}
			else{
				EMindGUI.MakeTooltip(0,new Rect(X+6,6+2,18,18),"Not sure why I'm still using this icon...XD");
				GUI.Toggle(new Rect(X,6,100,25),true,new GUIContent("Mask",IconMask),GUI.skin.GetStyle("ButtonRight"));
			}
			GUI.enabled = true;
			
			GUI.Box(new Rect(0,0,264,WinSize.y),"",ShaderUtil.BoxMiddle20);
			
			
			if (!LayerDrag&&Hit==false&&Event.current.type == EventType.MouseDown&&!(new Rect(0,0,264,WinSize.y).Contains(Event.current.mousePosition))){
				UnityEditor.Undo.SetCurrentGroupName("Deselect Layer");
				SRS.LayerSelection = null;//new Vector2(20,20);
			}
			
			//ShaderLayer DrawLayer = null;
			//if (LayerSelection.x==0)
			//{
				//if (SRS.LayerSelection.x<SLList.Count)
				//if (SRS.LayerSelection.y<SLList[(int)SRS.LayerSelection.x].Count)
				//if (SLList[(int)SRS.LayerSelection.x][(int)SRS.LayerSelection.y]!=null)
				//DrawLayer = SLList[(int)SRS.LayerSelection.x][(int)SRS.LayerSelection.y];
			//}
			
			int LayerSettingsHeight = 48;
			if (ShaderSandwich.Instance.CurInterfaceStyle==InterfaceStyle.Modern)
				LayerSettingsHeight = 31;
			GUI.Box(new Rect(0,0,265,LayerSettingsHeight),"Layer Settings",ShaderUtil.BoxMiddle20);
			int ScrollHeight = 660;
			if (SRS.LayerSelection!=null)
				ScrollHeight = SRS.LayerSelection.LastGUIHeightImSoLazy;
			else
				ScrollHeight = 10;
			
			EMindGUI.BeginGroup(new Rect(0,LayerSettingsHeight,264,WinSize.y-LayerSettingsHeight));
			if (!((WinSize.y-LayerSettingsHeight-30)>ScrollHeight))
			LayerScroll = EMindGUI.BeginScrollView(new Rect(0,0,264,WinSize.y-LayerSettingsHeight-30),LayerScroll,new Rect(0,0,230,ScrollHeight),false,true);
			else{
			EMindGUI.BeginScrollView(new Rect(0,0,264,WinSize.y-LayerSettingsHeight-30),new Vector2(0,0),new Rect(0,0,230,ScrollHeight),false,true);
			EMindGUI.EndScrollView();
			EMindGUI.VerticalScrollbar(new Rect(0,0,264,WinSize.y-LayerSettingsHeight-30),new Vector2(0,0),new Rect(0,0,230,ScrollHeight),true);
			}
			
			
			if (AnimateInputs&&Event.current.type==EventType.Repaint&&SRS.LayerSelection!=null&&SRS.LayerSelection.Parent!=null){
				foreach(ShaderLayer SL in SRS.LayerSelection.Parent.SLs){
					SL.UpdateShaderVars(true);
					foreach(ShaderVar SV in SL.ShaderVars)
					SV.UpdateToInput(true);
				}
			}
			
			if (SRS.LayerSelection!=null)
				SRS.LayerSelection.DrawGUI();
			else
				ShaderLayer.DrawGUIGen(true);
			//if ((WinSize.y-48)<=ScrollHeight)
			if (!((WinSize.y-LayerSettingsHeight-30)>ScrollHeight)){
				EMindGUI.EndScrollView();
				EMindGUI.VerticalScrollbar(new Rect(0,0,264,WinSize.y-LayerSettingsHeight-30),LayerScroll,new Rect(0,0,230,ScrollHeight));
			}
			EMindGUI.EndGroup();
			ChangeSaveTemp(SRS.LayerSelection);
			
			if (CurInterfaceStyle!=InterfaceStyle.Modern){
				if (GUI.Button( EMindGUI.Rect2(RectDir.Bottom,0,WinSize.y,64,30),"Back",ShaderUtil.BoxMiddle20))
					Goto(GUIType.Start,ShaderTransition.TransDir.Backward);
				if (GUI.Button( EMindGUI.Rect2(RectDir.Bottom,64,WinSize.y,110,30),"Settings",ShaderUtil.BoxMiddle20))
					Goto(GUIType.Configure,ShaderTransition.TransDir.Backward);	
				if (GUI.Button( EMindGUI.Rect2(RectDir.Bottom,174,WinSize.y,100-24,30),"Inputs",ShaderUtil.BoxMiddle20))
					Goto(GUIType.Inputs,ShaderTransition.TransDir.Backward);
			}
			
			EMindGUI.EndGroup();
		}
		
	}
	public void ChangeSaveTemp(ShaderLayer Up){
		ChangeSaveTemp_Real(Up,true);
	}
	public void ChangeSaveTemp(ShaderLayer Up,bool UpdateInputs){
		ChangeSaveTemp_Real(Up,UpdateInputs);
	}
	public void RegenShaderPreview(){
		if (ShaderPreview.Instance!=null&&RealtimePreviewUpdates)
			SaveTemp();
	}
	public void UpdateShaderPreview(){
		if (ShaderPreview.Instance!=null&&ShaderPreview.Instance.previewMat!=null&&RealtimePreviewUpdates&&OpenShader!=null){
			//Debug.Log("Updating Inputs");
			Shader.SetGlobalTexture("_ShaderSandwichPerlinTextureLinear",ImageBasedPerlinNoiseTempLinear);
			Shader.SetGlobalTexture("_ShaderSandwichPerlinTexture",ImageBasedPerlinNoiseTemp);
			
			ShaderPreview.Instance.previewMat.SetTexture("_Cube",ShaderPreview.Instance.CurCube);
			if (ShaderPreview.Instance.previewMat2!=null)
				ShaderPreview.Instance.previewMat2.SetTexture("_Cube",ShaderPreview.Instance.CurCube);
			foreach(ShaderInput SI in OpenShader.ShaderInputs){
				if (SI.Type==ShaderInputTypes.Image)
					ShaderPreview.Instance.previewMat.SetTexture(SI.Get(),SI.ImageS());
				if (SI.Type==ShaderInputTypes.Color)
					ShaderPreview.Instance.previewMat.SetColor(SI.Get(),SI.Color.ToColor());
				if (SI.Type==ShaderInputTypes.Vector)
					ShaderPreview.Instance.previewMat.SetVector(SI.Get(),SI.Color.ToColor());
				if (SI.Type==ShaderInputTypes.Cubemap)
					ShaderPreview.Instance.previewMat.SetTexture(SI.Get(),SI.CubeS());
				
				if (SI.Type==ShaderInputTypes.Image||SI.Type==ShaderInputTypes.Cubemap){
					if (SI.SeeTilingOffset){
						ShaderPreview.Instance.previewMat.SetTextureOffset(SI.Get(),SI.Offset);
						ShaderPreview.Instance.previewMat.SetTextureScale(SI.Get(),SI.Tiling);
					}
					else{
						ShaderPreview.Instance.previewMat.SetTextureOffset(SI.Get(),new Vector2(0,0));
						ShaderPreview.Instance.previewMat.SetTextureScale(SI.Get(),new Vector2(1,1));
					}
				}
				
				if (SI.Type==ShaderInputTypes.Float||SI.Type==ShaderInputTypes.Range)
					ShaderPreview.Instance.previewMat.SetFloat(SI.Get(),SI.Number);
				if (ShaderPreview.Instance.previewMat2!=null){
					if (SI.Type==ShaderInputTypes.Image)
						ShaderPreview.Instance.previewMat2.SetTexture(SI.Get(),SI.ImageS());
					if (SI.Type==ShaderInputTypes.Color)
						ShaderPreview.Instance.previewMat2.SetColor(SI.Get(),SI.Color.ToColor());
					if (SI.Type==ShaderInputTypes.Vector)
						ShaderPreview.Instance.previewMat2.SetVector(SI.Get(),SI.Color.ToColor());
					if (SI.Type==ShaderInputTypes.Cubemap)
						ShaderPreview.Instance.previewMat2.SetTexture(SI.Get(),SI.CubeS());
					if (SI.Type==ShaderInputTypes.Float||SI.Type==ShaderInputTypes.Range)
						ShaderPreview.Instance.previewMat2.SetFloat(SI.Get(),SI.Number);
					
					if (SI.Type==ShaderInputTypes.Image||SI.Type==ShaderInputTypes.Cubemap){
						if (SI.SeeTilingOffset){
							ShaderPreview.Instance.previewMat2.SetTextureOffset(SI.Get(),SI.Offset);
							ShaderPreview.Instance.previewMat2.SetTextureScale(SI.Get(),SI.Tiling);
						}
						else{
							ShaderPreview.Instance.previewMat2.SetTextureOffset(SI.Get(),new Vector2(0,0));
							ShaderPreview.Instance.previewMat2.SetTextureScale(SI.Get(),new Vector2(1,1));
						}
					}
				}
			}
			if (ShaderSandwich.Instance.ImprovedUpdates){
				foreach(ShaderVar SV in ShaderUtil.GetAllShaderVars()){
					if (SV.Input==null){
						//Debug.Log(SV.Name);
						if (SV.CType == Types.Vec)
							ShaderPreview.Instance.previewMat.SetColor("SSTEMPSV"+SV.GetID().ToString(),(Color)SV.Vector);
						if (SV.CType == Types.Float&&!SV.NoInputs)
							ShaderPreview.Instance.previewMat.SetFloat("SSTEMPSV"+SV.GetID().ToString(),SV.Float);
					
						if (SV.CType == Types.Texture)
							ShaderPreview.Instance.previewMat.SetTexture("SSTEMPSV"+SV.GetID().ToString(),SV.ImageS());
						if (SV.CType == Types.Cubemap)
							ShaderPreview.Instance.previewMat.SetTexture("SSTEMPSV"+SV.GetID().ToString(),SV.CubeS());
						
						if (ShaderPreview.Instance.previewMat2!=null){
							if (SV.CType == Types.Vec)
								ShaderPreview.Instance.previewMat2.SetColor("SSTEMPSV"+SV.GetID().ToString(),(Color)SV.Vector);
							if (SV.CType == Types.Float)
								ShaderPreview.Instance.previewMat2.SetFloat("SSTEMPSV"+SV.GetID().ToString(),SV.Float);
						
							if (SV.CType == Types.Texture)
								ShaderPreview.Instance.previewMat2.SetTexture("SSTEMPSV"+SV.GetID().ToString(),SV.ImageS());
							if (SV.CType == Types.Cubemap)
								ShaderPreview.Instance.previewMat2.SetTexture("SSTEMPSV"+SV.GetID().ToString(),SV.CubeS());
						}
					}
				}
			}
		}
		if (ShaderPreview.Instance!=null&&RealtimePreviewUpdates&&OpenShader!=null&&OpenShader.TransparencyType.Type==1&&OpenShader.TransparencyOn.On&&ShaderPreview.Instance.previewMatW!=null){
			//Debug.Log("Updating Inputs");
			foreach(ShaderInput SI in OpenShader.ShaderInputs){
				if (SI.Type==ShaderInputTypes.Image)
				ShaderPreview.Instance.previewMatW.SetTexture(SI.Get(),SI.ImageS());
				if (SI.Type==ShaderInputTypes.Color)
				ShaderPreview.Instance.previewMatW.SetColor(SI.Get(),SI.Color.ToColor());
				if (SI.Type==ShaderInputTypes.Vector)
				ShaderPreview.Instance.previewMatW.SetVector(SI.Get(),SI.Color.ToColor());
				if (SI.Type==ShaderInputTypes.Cubemap)
				ShaderPreview.Instance.previewMatW.SetTexture(SI.Get(),SI.CubeS());
				if (SI.Type==ShaderInputTypes.Float||SI.Type==ShaderInputTypes.Range)
				ShaderPreview.Instance.previewMatW.SetFloat(SI.Get(),SI.Number);
			}
		}
	}
	public void ChangeSaveTemp_Real(ShaderLayer Up,bool UpdateInputs){
		if (GUI.changed)
		ValueChanged = true;
		if (ValueChanged&&OpenShader!=null){//DrawLayer!=null&&
			if (Up!=null&&Up.Parent!=null){
			Up.Parent.UpdateIcon(new Vector2(70,70));
			//Debug.Log("UpdatedIcon");
			}
			
			if (UpdateInputs){
			OpenShader.RecalculateAutoInputs();}
			
			UpdateShaderPreview();
		}
		if (!GUI.changed&&OpenShader!=null){
			GUIChangedTimer-=1;
			if (GUIChangedTimer==0){
				RegenShaderPreview();
			}
		}
		else{
			GUIChangedTimer=30;
		}
	}
	List<GUIContent> PresetsGUIContents;
	List<string> PresetsPaths;
	/*void GUIPresets(Vector2 WinSize,Vector2 Position)
	{
		ShaderVarEditing = null;
		EMindGUI.BeginGroup(new Rect(Position.x,Position.y,WinSize.x,WinSize.y));
		/*GUILayout.BeginVertical(SCSkin.box,GUILayout.Height(200));
		
		GUILayout.EndVertical();*/
		
		//string[] BaseVarients = {"Plain","Plant","Fur","Grass","Skin"};
		//string[] ExtraVarients = {"","Specular","Normal Mapped","Specular Normal Mapped","Rim Lit","Normal Mapped Rim Lit","POM","Specular POM","Normal Mapped POM","Specular Normal Mapped POM"};
		/*string[] selStrings = new string[BaseVarients.Length*ExtraVarients.Length];
		int i = 0;
		foreach (string Var2 in ExtraVarients)
		{
			foreach (string Var in BaseVarients)
			{
				selStrings[i] = Var+" "+Var2;
				i+=1;
			}
		}*
		if (PresetsGUIContents==null||PresetsGUIContents.Count==0||PresetsPaths==null||PresetsPaths.Count==0){
			PresetsGUIContents = new List<GUIContent>(); 
			PresetsPaths = new List<string>();
			//DirectoryInfo info = null;
			//try{
				//---------info = new DirectoryInfo(Application.dataPath+"/Shader Sandwich/Internal/Editor/Presets/");//ScriptFileFolder);
			//}catch{}
			/*if (info!=null&&info.Exists){
				FileInfo[] fileInfo = info.GetFiles();
				//List<GUIContent> selStrings = new List<GUIContent>();
				//List<Texture2D> selImages = new List<Texture2D>();
				foreach (FileInfo file in fileInfo){
					//List<string> FileText = new List<string>();
					if (file.Extension==".shader"){
						//StreamReader theReader = new StreamReader(file.ToString());
						//Debug.Log(file.Name);
						
						//Debug.Log(file.FullName);
						if (System.IO.File.Exists(file.DirectoryName+"\\"+file.Name.Replace(".shader",".png").Substring(1))){
						var bytes = System.IO.File.ReadAllBytes(file.DirectoryName+"\\"+file.Name.Replace(".shader",".png").Substring(1));
						var tex = new Texture2D(1, 1);
						tex.LoadImage(bytes);
						//PresetsGUIContents.Add(new GUIContent(tex));
						PresetsGUIContents.Add(new GUIContent(file.Name.Replace(".shader","").Substring(1),tex));
						PresetsPaths.Add(file.DirectoryName+"\\"+file.Name);
						//selStrings.Add(new GUIContent(tex));
						}
					}
				}
			}*
			string[] guids2 = AssetDatabase.FindAssets ("t:Shader", new string[]{"Assets/ElectronicMindStudios/Shader Sandwich/Internal/Editor/Presets"});
			//for (string guid in guids2)
				//Debug.Log (AssetDatabase.GUIDToAssetPath(guid));
			//FileInfo[] fileInfo = info.GetFiles();
			//List<GUIContent> selStrings = new List<GUIContent>();
			//List<Texture2D> selImages = new List<Texture2D>();
			foreach (string guid in guids2){
				//Debug.Log (AssetDatabase.GUIDToAssetPath(guid));
				string ImgPath = AssetDatabase.GUIDToAssetPath(guid).Replace(".shader",".png");
				ImgPath = ImgPath.Remove(ImgPath.LastIndexOf("/")+1,1);
				//Debug.Log(ImgPath);
				Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(ImgPath,typeof(Texture2D));
				if (tex!=null){
					PresetsGUIContents.Add(new GUIContent(new FileInfo(AssetDatabase.GUIDToAssetPath(guid)).Name.Replace(".shader","").Substring(1),tex));
					PresetsPaths.Add(AssetDatabase.GUIDToAssetPath(guid));
				}
				//Debug.Log("YAY!");
				/*if (System.IO.File.Exists(file.DirectoryName+"\\"+file.Name.Replace(".shader",".png").Substring(1))){
					var bytes = System.IO.File.ReadAllBytes(file.DirectoryName+"\\"+file.Name.Replace(".shader",".png").Substring(1));
					var tex = new Texture2D(1, 1);
					tex.LoadImage(bytes);
					//PresetsGUIContents.Add(new GUIContent(tex));
					PresetsGUIContents.Add(new GUIContent(file.Name.Replace(".shader","").Substring(1),tex));
					PresetsPaths.Add(file.DirectoryName+"\\"+file.Name);
				}	*
			}
		}
		//List<string> selStrings = new string[]{"Legacy Diffuse","PBR Diffuse","PBR Normal Mapped","PBR Height Mapped","Parallax Occlusion Mapped","Displacement Mapped","Glow","Cartoon"};
		int XCount = (int)Mathf.Floor((WinSize.x-120f)/192f);
		Rect gridRect = new Rect(0, 0, XCount*192, Mathf.Ceil(PresetsGUIContents.Count/Mathf.Floor((WinSize.x-120f)/256f))*192);
		GUI.Box(new Rect(100,32,XCount*192+20,WinSize.y-64),"",GUI.skin.button);
		StartScroll = EMindGUI.BeginScroollView(new Rect(100,32,XCount*192+20,WinSize.y-64),StartScroll,new Rect(0,0,gridRect.width,Mathf.Max((int)(PresetsGUIContents.Count/XCount)*192,gridRect.height-180)),false,false);
		
		GUIStyle GridStyle = new GUIStayle(GUI.skin.button);
		//GridStyle.margin = new RectOffset(GridStyle.margin.left+20,GridStyle.margin.right+20,GridStyle.margin.top+20,GridStyle.margin.bottom+20); 
		GridStyle.wordWrap = true;
		GridStyle.imagePosition = ImagePosition.ImageAbove;
		GridStyle.fixedHeight =192;
		
		foreach(GUIContent GC in PresetsGUIContents){
			if (GUI.skin.label.CalcHeight(new GUIContent(GC.text), 192)<20){
				GC.text+="\n";
			}
		}
		
		selGridInt = GUI.SelectionGrid(gridRect, selGridInt, PresetsGUIContents.ToArray(), XCount,GridStyle);
		if (PresetsGUIContents.Count==0)
		GUI.Label(new Rect(20,20,500,300),"Sorry, the presets can't be loaded. Make sure Shader Sandwich has been imported properly, and if it has, please file a bug report :).");
		EMindGUI.EndScroollView();
		
		GUIStyle ButtonStyle = new GUIStayle(GUI.skin.button);
		ButtonStyle.fontSize = 20;		
		if (GUI.Button( EMindGUI.Rect2(RectDir.Bottom,0,WinSize.y,100,30),"Back",ButtonStyle))
			Goto(GUIType.Start,ShaderTransition.TransDir.Backward);	
		if (PresetsGUIContents.Count>0){
			if (GUI.Button( EMindGUI.Rect2(RectDir.Diag,WinSize.x,WinSize.y,100,30),"Next",ButtonStyle)||Event.current.clickCount==2){
				//Debug.Log(PresetsPaths[selGridInt]);
				SimpleLoad(PresetsPaths[selGridInt]);
				OpenShader.ShaderName.Text = "Untitled Shader";
				Goto(GUIType.Layers,ShaderTransition.TransDir.Forward);
			}
		}
		
		EMindGUI.EndGroup();		
	}*/
	
	void DrawMenu() {
		//if (GUILayout.Button("Create...", EditorStyles.toolbarButton))
		Color OldColor = GUI.backgroundColor;
		GUI.backgroundColor = new Color(1,1,1,1);
		GUILayout.BeginHorizontal(GUI.skin.GetStyle("Toolbar"));
		if (GUILayout.Button("File", GUI.skin.GetStyle("ToolbarDropDown"))){
			GenericMenu toolsMenu = new GenericMenu();

			//if (Selection.activeGameObject != null)
			//toolsMenu.AddItem(new GUIContent("New/Simple"), false, NewSimple);
			toolsMenu.AddItem(new GUIContent("New"), false, NewAdvanced);
			toolsMenu.AddItem(new GUIContent("Open"), false, LoadWhoCares);
			toolsMenu.AddSeparator("");
			if (CurrentFilePath!="")
			toolsMenu.AddItem(new GUIContent("Save"), false, Save);
			else
			toolsMenu.AddDisabledItem(new GUIContent("Save"));
			toolsMenu.AddItem(new GUIContent("Save as..."), false, SaveAs);

			toolsMenu.AddSeparator("");
			toolsMenu.AddItem(new GUIContent("Help"), false, OpenHelp);
			toolsMenu.AddSeparator("");
			//List<string> lst = new List<string>();
			/*lst.AddRange(GlobalSettings.RecentFiles);
			string[] lstArray = lst.Distinct().ToArray();
			for(int i=0;i<(lstArray.Length);i+=1)
			{
				toolsMenu.AddItem(new GUIContent("Open Recent/"+Path.GetFileName(lstArray[i])), false, Nothing,i);
			}*/
			//else
			//toolsMenu.AddDisabledItem(new GUIContent("Optimize Selected"));

			//toolsMenu.AddSeparator("");

			//toolsMenu.AddItem(new GUIContent("Help..."), false, Nothing);

			// Offset menu from right of editor window
			toolsMenu.DropDown(new Rect(5, 0, 0, 16));
			EditorGUIUtility.ExitGUI();
		}
		if (GUILayout.Button("View", GUI.skin.GetStyle("ToolbarDropDown"))){
			GenericMenu toolsMenu = new GenericMenu();

			//if (Selection.activeGameObject != null)
			//toolsMenu.AddItem(new GUIContent("New/Simple"), false, NewSimple);
			toolsMenu.AddItem(new GUIContent("Layer Names"), ViewLayerNames, SetViewLayerNames);
			toolsMenu.AddItem(new GUIContent("Mix Amount Values"), ShowMixAmountValues, SetShowMixAmountValues);
			//
			///toolsMenu.AddItem(new GUIContent("Blend Layer Icons"), BlendLayers, SetBlendLayers);
			
			///toolsMenu.AddItem(new GUIContent("Interface/Original"), CurInterfaceStyle==InterfaceStyle.Old, SetInterfaceOriginal);
			///toolsMenu.AddItem(new GUIContent("Interface/Flat"), CurInterfaceStyle==InterfaceStyle.Flat, SetInterfaceFlat);
			///toolsMenu.AddItem(new GUIContent("Interface/New"), CurInterfaceStyle==InterfaceStyle.Modern, SetInterfaceNew);
			

			///toolsMenu.AddItem(new GUIContent("Inputs/Terrible Old Interface"), UseOldInputs, SetOldInputs);
			toolsMenu.DropDown(new Rect(41, 0, 0, 16));
			EditorGUIUtility.ExitGUI();
		}
		if (GUILayout.Button("Previews",GUI.skin.GetStyle("ToolbarDropDown"))){
			GenericMenu toolsMenu = new GenericMenu();

			//if (Selection.activeGameObject != null)
			toolsMenu.AddItem(new GUIContent("Open Preview Window"), false, OpenPreview);
			//toolsMenu.AddItem(new GUIContent("Improved Updates"), ImprovedUpdates, SetImprovedUpdates);
			//toolsMenu.AddItem(new GUIContent("Realtime Preview Updates"), RealtimePreviewUpdates, SetRealtimePreviewUpdates);
			toolsMenu.AddItem(new GUIContent("Compiled Previews/On Fast(Less recompiles, slower shader)"), RealtimePreviewUpdates&&ImprovedUpdates, SetPreviewUpdatesOnFast);
			toolsMenu.AddItem(new GUIContent("Compiled Previews/On Accurate(More recompiles, faster shader)"), RealtimePreviewUpdates&&!ImprovedUpdates, SetPreviewUpdatesOn);
			toolsMenu.AddItem(new GUIContent("Compiled Previews/Off"), !RealtimePreviewUpdates&&!ImprovedUpdates, SetPreviewUpdatesOff);
			
			toolsMenu.AddItem(new GUIContent("Mini Previews/Animate"), AnimateInputs, SetAnimateInputs);
			toolsMenu.AddItem(new GUIContent("Mini Previews/System/GPU"), PreviewSystem==PreviewSystem.GPU, SetPreviewSystemGPU);
			//toolsMenu.AddItem(new GUIContent("Mini Previews/System/CPU (Slow and Less Accurate!)"), PreviewSystem==PreviewSystem.CPU, SetPreviewSystemCPU);

			toolsMenu.DropDown(new Rect(83, 0, 0, 16));
			EditorGUIUtility.ExitGUI();
		}
		if (GUILayout.Button("Help",GUI.skin.GetStyle("ToolbarDropDown"))){
			GenericMenu toolsMenu = new GenericMenu();
			toolsMenu.AddItem(new GUIContent("About"), false, OpenAbout);
			toolsMenu.AddItem(new GUIContent("Open Online Documentation"), false, OpenHelp);
			toolsMenu.AddItem(new GUIContent("Report Bug or Suggest Feature!"), false, SendFeedback);
			toolsMenu.AddItem(new GUIContent("See Bug and Feature Reports"), false, OpenFeedback);

			toolsMenu.DropDown(new Rect(146, 0, 0, 17));
			EditorGUIUtility.ExitGUI();
		}
//		Debug.Log("DrawMenu:"+Status);
		GUILayout.Button("Status: "+Status,GUI.skin.GetStyle("ToolbarButton"));
		
		//GUILayout.Label(Path.GetFileName(AssetDatabase.GetAssetPath(OpenShader)));
		GUILayout.FlexibleSpace();
		if (ShaderSandwich.Instance.CurInterfaceStyle==InterfaceStyle.Modern){
			//InterfaceSize = EditorGUILayout.IntField(new GUIContent(""),InterfaceSize,GUILayout.Width(24));
			EditorGUI.BeginChangeCheck();
			InterfaceSize = EditorPrefs.GetInt("ElectronicMindStudiosInterfaceSize", 10);
			InterfaceSize = EditorGUILayout.IntSlider(new GUIContent(""),InterfaceSize,8,14,GUILayout.Width(140));
			if (EditorGUI.EndChangeCheck()){
				EditorPrefs.SetInt("ElectronicMindStudiosInterfaceSize", InterfaceSize);
			}
			Color newCol = EditorGUILayout.ColorField(new GUIContent(""),InterfaceColor,GUILayout.Width(100));
			if (newCol!=InterfaceColor){
				if (EMindGUI.GetVal(newCol)>0){
					InterfaceColor = newCol;
					
					EditorPrefs.SetFloat("ElectronicMindStudiosInterfaceColor_R", InterfaceColor.r);
					EditorPrefs.SetFloat("ElectronicMindStudiosInterfaceColor_G", InterfaceColor.g);
					EditorPrefs.SetFloat("ElectronicMindStudiosInterfaceColor_B", InterfaceColor.b);
					EditorPrefs.SetFloat("ElectronicMindStudiosInterfaceColor_A", InterfaceColor.a);
				}
			}
		}
		GUILayout.EndHorizontal();
		GUI.backgroundColor = OldColor;
	}	
	//void OpenOnlineDocumentation(){
	//	Help.BrowseURL("http://electronic-mind.org/pages/things/SSDoc/Main.html");
	//}
	void OpenAbout(){
		ShaderSandwichAboutBox blah = ScriptableObject.CreateInstance<ShaderSandwichAboutBox>();
		blah.ShowUtility();
	}
	void OpenHelp(){
		int option = EditorUtility.DisplayDialogComplex(
				"Online Help",
				"Hi! Do you want to open the online help, or just copy the link to your clipboard?",
				"Open Online Help",
				"Copy to Clipboard",
				"Do Nothing");
		switch (option) {
			// Save Scene
			case 0:
				Help.BrowseURL("http://electronic-mind.org/ShaderSandwichDocumentation/");
				break;
			// Quit Without saving.
			case 1:
				EditorGUIUtility.systemCopyBuffer = "http://electronic-mind.org/ShaderSandwichDocumentation/";
				EditorUtility.DisplayDialog("Copied!", "The URL:\nhttp://electronic-mind.org/ShaderSandwichDocumentation/ has been copied!", "Ok");
				break;
		}
	
	}
	void OpenFeedback(){
		int option = EditorUtility.DisplayDialogComplex(
				"Bug/Feature Suggestion Tracker",
				"Hello :). Do you want to open the Bug/Feature Suggestion tracker in your browser, or just have a link to it copied to your clipboard?",
				"Open Tracker",
				"Copy to Clipboard",
				"Do Nothing");
		switch (option) {
			// Save Scene
			case 0:
				Help.BrowseURL("http://electronic-mind.org/ShaderSandwichFeedback/");
				break;
			// Quit Without saving.
			case 1:
				EditorGUIUtility.systemCopyBuffer = "http://electronic-mind.org/ShaderSandwichFeedback/";
				EditorUtility.DisplayDialog("Copied!", "The URL:\nhttp://electronic-mind.org/ShaderSandwichFeedback/ has been copied!", "Ok");
				break;
		}
	}
	void SendFeedback(){
		ShaderBugReport.Init();
	}
	void OpenPreview(){
		ShaderPreview.Init();
		ChangeSaveTemp(null,true);
		UpdateShaderPreview();
	}
	void SetRealtimePreviewUpdates(){
		RealtimePreviewUpdates = !RealtimePreviewUpdates;
		ChangeSaveTemp(null,true);
		UpdateShaderPreview();
	}
	void SetViewLayerNames(){
		ViewLayerNames = !ViewLayerNames;
	}
	void SetShowMixAmountValues(){
		ShowMixAmountValues = !ShowMixAmountValues;
	}
	void SetPreviewUpdatesOff(){
		ImprovedUpdates = false;
		RealtimePreviewUpdates = false;
		ChangeSaveTemp(null,true);
		UpdateShaderPreview();
	}
	void SetPreviewUpdatesOn(){
		ImprovedUpdates = false;
		RealtimePreviewUpdates = true;
		ChangeSaveTemp(null,true);
		UpdateShaderPreview();
	}
	void SetPreviewUpdatesOnFast(){
		ImprovedUpdates = true;
		RealtimePreviewUpdates = true;
		ChangeSaveTemp(null,true);
		UpdateShaderPreview();
	}
	void SetImprovedUpdates(){
		ImprovedUpdates = !ImprovedUpdates;
	}
	void SetInterfaceOriginal(){
		CurInterfaceStyle = InterfaceStyle.Old;
	}
	void SetInterfaceFlat(){
		CurInterfaceStyle = InterfaceStyle.Flat;
	}
	void SetInterfaceNew(){
		CurInterfaceStyle = InterfaceStyle.Modern;
	}
	void SetPreviewSystemCPU(){
		PreviewSystem = PreviewSystem.CPU;
	}
	void SetPreviewSystemGPU(){
		PreviewSystem = PreviewSystem.GPU;
	}
	void SetOldInputs(){
		UseOldInputs = !UseOldInputs;
	}
	void SetBlendLayers(){
		BlendLayers = !BlendLayers;
	}
	void SetAnimateInputs(){
		AnimateInputs = !AnimateInputs;
		ChangeSaveTemp(null,true);
	}
	void NewAdvanced(){
		if (OpenShader!=null)
			OpenShader.CleanUp();
		OpenShader = ShaderBase.CreateInstance<ShaderBase>();
		OpenShader.Prepare();
		ShaderSandwich.ShaderLayerTab = 0;
		Goto(GUIType.Layers,ShaderTransition.TransDir.Forward);
		GUI.changed = true;
		ChangeSaveTemp(null);
		CurrentFilePath = "";
		ShaderSandwich.Instance.UpdateMaskShaderVars();
		SRS.LayerSelection = OpenShader.ShaderPasses[0].Surfaces[0].SurfaceLayers.SLs[0];
	}
	void NewSimple(){
		if (OpenShader!=null)
			OpenShader.CleanUp();
		OpenShader = ShaderBase.CreateInstance<ShaderBase>();
		OpenShader.Prepare();
		ShaderSandwich.ShaderLayerTab = 0;
		Goto(GUIType.Presets,ShaderTransition.TransDir.Forward);
		ChangeSaveTemp(null);
		CurrentFilePath = "";
	}
	void SaveAs(){
		string path = EditorUtility.SaveFilePanelInProject("Save Shader",//"/Assets/ElectronicMindStudios/ASD123.asset";//
								"",
								"shader",
								"Please enter a file name to save the shader to");
		if (path!="")
		{
			CurrentFilePath = path;
			//if (OpenShader.ShaderName.Text == "Untitled Shader")
			OpenShader.ShaderName.Text = (new FileInfo(CurrentFilePath)).Name.Replace(".shader","").Replace(".Shader","");
			Save();
		}
		
	}
	void Save(){
		if (CurrentFilePath=="")
		SaveAs();
		else{
			Status = "Saving Shader Code...";
			ShaderUtil.SaveString(CurrentFilePath,OpenShader.GenerateCode()+"\n\n/*\n"+OpenShader.Save()+"\n*/");
			//AssetDatabase.Refresh();
			//Debug.Log(CurrentFilePath);
			//Debug.Log(Application.dataPath);
			Status = "Compiling Shader Code...";
			AssetDatabase.ImportAsset(CurrentFilePath,ImportAssetOptions.ForceSynchronousImport);
			Status = "Shader Saved.";
			SSSettings.AddNew(CurrentFilePath);
			UpdateShaderPreview();
		}
	}
	string PleaseReimport = "";
	int GiveMeAFrame = 0;
	//double CompileTime = 0;
	void SaveTemp(){
		var watch = System.Diagnostics.Stopwatch.StartNew();
		string NewGenerateCode = "";
		//for(int i = 0;i<5;i++){
			NewGenerateCode = OpenShader.GenerateCode(true);
		//}
		watch.Stop();
		//CompileTime = watch.ElapsedMilliseconds;///5f;
		if (OldShaderGenerate!=NewGenerateCode){
			SetStatus("Updating Preview Window","Saving Shader Code...",0.1f);
			
			ShaderUtil.SaveString(Application.dataPath+"/ElectronicMindStudios/Shader Sandwich/Internal/Shader Sandwich/SSTemp.shader",NewGenerateCode+"\n\n/*\n"+OpenShader.Save()+"\n*/");
			OldShaderGenerate = NewGenerateCode;
			SetStatus("Updating Preview Window","Compiling Shader Code...",0.3f);

			PleaseReimport = "Assets/ElectronicMindStudios/Shader Sandwich/Internal/Shader Sandwich/SSTemp.shader";
			/*if (OpenShader.TransparencyType.Type==1&&OpenShader.TransparencyOn.On){
				Status = "Saving Shader Code...";
				ShaderUtil.SaveString(Application.dataPath+"/ElectronicMindStudios/Shader Sandwich/Internal/Shader Sandwich/SSTempWireframe.shader",OpenShader.GenerateCode(true,true));
				Status = "Compiling Shader Code...";
				AssetDatabase.ImportAsset("Assets/ElectronicMindStudios/Shader Sandwich/Internal/Shader Sandwich/SSTempWireframe.shader",ImportAssetOptions.ForceSynchronousImport);
			}*/
			//EditorGUIUtility.ExitGUI();
		}
		else{
			//SetStatus("Updating Preview Window","No Code Change, Stopping Update."+CompileTime.ToString(),1f);
			SetStatus("Updating Preview Window","No Code Change, Stopping Update.",1f);
		}
	}
	void LoadWhoCares(){
	Load();
	}
	bool Load(){
		
		return Load(EditorUtility.OpenFilePanel("Open Shader",
								"Assets/",
								"shader"));
	}
	void SimpleLoad(string path){
		SetStatus("Loading File","Loading Shader...",0.1f);
		string line = "";
		string FileString = "";
		

		StreamReader theReader = new StreamReader(path);

		//using (theReader)
		//{
			bool inParse = false;
			bool didParse = false;
			bool LegacyLoad = false;
			while(true==true){
				line = theReader.ReadLine();
				//Debug.Log(line);
				if (line != null){
					line = ShaderUtil.Sanitize(line);
					line = line.Trim();
					if (line.Contains("BeginShaderParse")){
						inParse = true;
						didParse = true;
						LegacyLoad = true;
					}
					if (line.Contains("Shader Sandwich Shader")){
						inParse = true;
						didParse = true;
					}
					if (LegacyLoad){
						if (line.IndexOf("#?")>=0)
							line = line.Substring(0,line.IndexOf("#?"));
					}
					if (inParse==true&&(line!=""))
						FileString+=line+"\n";
					
					if (LegacyLoad&&line.Contains("EndShaderParse"))
						inParse = false;
					if (line.Contains("End Shader Sandwich Shader"))
						inParse = false;
				}
				else{
					break;
				}
			}
			//Debug.Log(FileString);
			theReader.Close();
			if (didParse){
				OpenShader = ShaderBase.Load(new StringReader(FileString),LegacyLoad);
				SRS.LayerSelection = null;
				RegenShaderPreview();
				GUI.changed = true;
				ChangeSaveTemp(null);
			}else{
				EditorUtility.DisplayDialog("Oh no!","Sorry, the shader you chose wasn't made with Shader Sandwich, so it can't be opened :(.","Ok");
			}
			SetStatus("Loading File","Shader Loaded.",1f);
			UpdateMaskShaderVars();
			
			if (OpenShader.ShaderPasses.Count>0&&OpenShader.ShaderPasses[0].Surfaces.Count>0&&OpenShader.ShaderPasses[0].Surfaces[0].GetMyShaderLayerLists().Count>0&&OpenShader.ShaderPasses[0].Surfaces[0].GetMyShaderLayerLists()[0].SLs.Count>0)
				SRS.LayerSelection = OpenShader.ShaderPasses[0].Surfaces[0].GetMyShaderLayerLists()[0].SLs[0];
	}
	bool Load(string path){
		//Debug.Log("\""+path+"\"");
		if (path!=""){
			if (path.StartsWith(Application.dataPath))
			path = "Assets"+path.Substring(Application.dataPath.Length);
			AssetDatabase.Refresh();
			CurrentFilePath = path;
			SSSettings.AddNew(CurrentFilePath);
			SimpleLoad(path);
			return true;
		}
		return false;
	}
	void Nothing(){}
	public void LayerCopy(object SL){ClipboardLayer = ((ShaderLayer)SL).Copy();}//Debug.Log(((ShaderLayer)SL).Save(0));}
	public void LayerPaste(object SLL){
		if (ClipboardLayer!=null){
			ShaderLayer SL = ClipboardLayer.Copy();
			SL.Name.Text+=" Copy";
			((ShaderLayerList)SLL).AddC(SL);
		}
		RegenShaderPreview();
		((ShaderLayerList)SLL).UpdateIcon(new Vector2(70,70));
	}
	public void LayerDelete(object sl){
		//UnityEditor.Undo.IncrementCurrentGroup();
		ShaderLayer SL = (ShaderLayer)sl;
		UnityEditor.Undo.SetCurrentGroupName("Delete Layer");
		ShaderLayerList list = SL.Parent;
		//Debug.Log(SRS.LayerSelection);
		if (list!=null){
			UnityEditor.Undo.RegisterCompleteObjectUndo(new UnityEngine.Object[]{(UnityEngine.Object)SRS,(UnityEngine.Object)list},"Delete Layer");
		}else{
			UnityEditor.Undo.RegisterCompleteObjectUndo((UnityEngine.Object)SRS,"Delete Layer");
		}
		UnityEditor.Undo.IncrementCurrentGroup();
		if (list!=null){
			list.SLs.Remove(SL);
			list.UpdateIcon(new Vector2(70,70));
		}
		GUI.changed = true;
		ChangeSaveTemp(null);
		SRS.LayerSelection = null;
		EMindGUI.Defocus();
		//Undo.DestroyObjectImmediate(SL);
		
		//EditorGUIUtility.ExitGUI();
	}
	public void InputDelete(object si){
		ShaderInput SI = (ShaderInput)si;
		UnityEditor.Undo.SetCurrentGroupName("Delete Input");
		//ShaderLayerList list = SL.Parent;
		//Debug.Log(SRS.LayerSelection);
		//if (list!=null){
			
		//}else{
		//	UnityEditor.Undo.RegisterCompleteObjectUndo((UnityEngine.Object)SRS,"Delete Layer");
		//}
		
		
		
		/*foreach (ShaderLayer SL in ShaderUtil.GetAllLayers()){
			SL.UpdateShaderVars(true);
			SVs.AddRange(SL.ShaderVars);
		}
		SVs.AddRange(OpenShader.GetMyShaderVars());*/
		List<ShaderVar> SVs = new List<ShaderVar>();
		List<UnityEngine.Object> undoObjects = new List<UnityEngine.Object>();
		undoObjects.Add((UnityEngine.Object)OpenShader);
		undoObjects.Add((UnityEngine.Object)SRS);
		foreach(ShaderVar SV in ShaderUtil.GetAllShaderVars()){
			if (SV.Input==SI){
				if (SV.Parent!=null)
					undoObjects.Add(SV.Parent);
				SVs.Add(SV);
			}
		}
		UnityEditor.Undo.RegisterCompleteObjectUndo(undoObjects.ToArray(),"Delete Input");
		UnityEditor.Undo.IncrementCurrentGroup();
		OpenShader.ShaderInputs.Remove(SI);
		foreach(ShaderVar SV in SVs){
			SV.Input = null;
		}
		
		ChangeSaveTemp(null);
		SRS.InputSelection = null;
		EMindGUI.Defocus();
	}
	public void IngredientCopy(object Ingredient){ClipboardIngredient = ((ShaderIngredient)Ingredient).Copy();}
	public void IngredientPaste(object Objects){
		if (ClipboardIngredient!=null){
			object[] objects = (object[])Objects;
			ShaderPass SP = (ShaderPass)objects[0];
			ShaderIngredient Pos = (ShaderIngredient)objects[1];
			ShaderIngredient I = ClipboardIngredient.Copy();
			SP.AddIngredientDirect(I,Pos);
		}
		RegenShaderPreview();
	}
	public void EffectCopy(object SEs){
		List<ShaderEffect> ToCopy = (List<ShaderEffect>)SEs;
		List<ShaderEffect> Copied = new List<ShaderEffect>();
		foreach(ShaderEffect SE in ToCopy){
			Copied.Add(SE.Copy());
		}
		ClipboardEffects = Copied;
	}
}
}