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
using EMindGUI = ElectronicMind.ElectronicMindStudiosInterfaceUtils;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using SDebug = System.Diagnostics;
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
public enum LayerTypes{
Color,
Gradient,
VertexColors,
Texture,
Cubemap,
Noise,
Previous,
Literal,
GrabDepth};

public enum VertexMasks{
None = 0,
Normal = 1,
Position = 2
//View = 3
};

public enum VertexSpaces{
Object = 0,
World = 1,
};

//public enum ShaderMapType{UVMap1,UVMap2,Reflection,Direction,RimLight,Generate,View,Position};
//public enum LayerMappingType{UV,Position,Direction,Generate};
//public enum LayerMappingType{UV1,UV2,UV3,UV4,ReflectionWorld,ReflectionLocal,ReflectionView,DirectionWorld,DirectionLocal,DirectionView,RimLight,GenerateLocal,GenerateWorld,ViewPosition01,ViewPositionNeg11,PositionLocal,PositionWorld,PositionView,ViewDirectionWorld,ViewDirectionLocal};
/*public enum LayerMappingType{
UV,// - TilingOffsetInput(?), UV Index
Generate,// World/Local
Position,// World/Local/Texture/View/Projection
Reflection,// World/Local/Texture/View/Projection
Direction,// World/Local/Texture/View/Projection
Tangent,// World/Local/Texture/View/Projection
RimLight,// Inverted
ScreenPosition,// 
ViewDirection//World/Local/Texture/View/Projection
}; */
public enum LayerMappingType{
UV,// - TilingOffsetInput(?), UV Index
Generate,// World/Local*/Rim/RimInverted
Position,// World/Local/Texture/View/Projection
Reflection,// World/Local/Texture/View/Projection
Direction,// Normal/Tangent World/Local/Texture/View/Projection
View,// World/Local/Texture/View/Projection*
}; 

[System.Serializable]
public class ShaderLayer : ScriptableObject{
	public List<ShaderVar> ShaderVars = new List<ShaderVar>();
	public SVDictionary CustomData = new SVDictionary();
	public int LayerID;

	//[NonSerialized]public ShaderLayerList Parent = null;//
	[NonSerialized]private ShaderLayerList Parent2;//
	public ShaderLayerList Parent{
		get{
			if (Parent2==null){
				foreach(ShaderLayerList SLL in ShaderSandwich.Instance.OpenShader.GetShaderLayerLists()){
					SLL.FixParents();
				}
			}
			
			return Parent2;
		}
		set{
			Parent2 = value;
		}
	}
	public bool IsVertex{
		get{
			return false;//(Parent.Name.Text=="Vertex"||(ShaderBase.Current.SG!=null&&ShaderBase.Current.SG.InVertex));
		}
		set{
		
		}
	}
	public bool IsPoke{
		get{
			return false;//(Parent.Name.Text=="Pixel Poke"||Parent.Name.Text=="Vertex Poke");
		}
		set{
		
		}
	}
	public bool IsLighting{
		get{
			return false;//(Parent.IsLighting.On||(ShaderBase.Current.SG!=null&&ShaderBase.Current.SG.InVertex));
		}
		set{
		
		}
	}
	public ShaderVar OldLayerType = new ShaderVar();
	public ShaderVar LayerType = new ShaderVar();
	public bool LayerTypeHelp;
	public ShaderVar Name = new ShaderVar();
	public bool Selected = true;
	public bool Enabled = true;
	
	[XmlIgnore,NonSerialized] public Texture2D GradTex;
	[XmlIgnore,NonSerialized] public Texture2D ColTex;


	//[XmlIgnore,NonSerialized] public Texture2D PreviewTex;
	[XmlIgnore,NonSerialized]public Texture2D PreviewTex;

	[NonSerialized]RenderTexture _RTIcon;
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
	//Vertex Colors 2
	//Texture 3 
	//public Texture2D Image;
	//public int ImageInput = -1;
	public bool IsNormalMap = false;
	public enum ImageTypes {Normal,Greyscale,NormalMap,ScaledNormalMap,PackedNormalMap}
	public ImageTypes ImageType = ImageTypes.Normal;
	public bool NoiseClamp;
		
	public ShaderVar UseAlpha = new ShaderVar();
	public ShaderVar AlphaBlendMode = new ShaderVar();
	
	public bool GeneratePremultiply = false;

	public ShaderVar Stencil = new ShaderVar();
	
	[XmlIgnore,NonSerialized]static public string[] ColorChannelNames = {"Red","Green","Blue","Alpha"};
	[XmlIgnore,NonSerialized]static public string[] ColorChannelCodeNames = {"r","g","b","a"};


	//Mapping
	public bool MapTypeOpen;
	public bool MapTypeHelp;
	public ShaderVar MapType;
	public ShaderVar MapLocal;
	public ShaderVar MapSpace;
	public ShaderVar MapGenerateSpace;
	public ShaderVar MapInverted;
	public ShaderVar MapUVIndex;
	public ShaderVar MapDirection;
	public ShaderVar MapScreenSpace;
	//1"UVs/UV1",
	//2"UVs/UV2",
	//3"World/Normals/Normal",
	//4"World/Normals/Reflection",
	//5"World/Normals/Edge",
	//6"World/Generated/From View","
	//7World/Generated/World Position",
	//8"World/Generated/Cube",
	//9"Local/Normals/Normal",
	//10"Local/Normals/Reflection",
	//11"Local/Normals/Edge",/
	//12"Local/Generated/From View",
	//13"Local/Generated/Local Position",
	//14"Local/Generated/Cube"
	public ShaderInput UVTexture;

	//[XmlIgnore,NonSerialized]static public string[] MapTypeDescriptions = {"Uses the 1st UV map. This is used in almost every shader.","Uses the 2nd UV map.","Used with cubemaps to simulate reflections","Based on the direction between the view and the normal of the object, it will use the first row of the texture.","The texture will be the same from any angle, based on the view.","The texture will be placed based on the world position.","The texture will be placed on each side of the model separately to eliminate seams.","Used with cubemaps to simulate Image Based Lighting."};

	/*asd*/[NonSerialized]public int MappingX = 0;
	/*asd*/[NonSerialized]public int MappingY = 1;
	/*asd*/[NonSerialized]public int MappingZ = 2;
	static public string[] MappingNames = {"X","Y","Z"};
	static public string[] MappingCodeNames = {"x","y","z"};

	public ShaderVar MixAmount = new ShaderVar();

	public ShaderVar UseFadeout = new ShaderVar();
	public ShaderVar FadeoutMinL = new ShaderVar();
	public ShaderVar FadeoutMaxL = new ShaderVar();
	public ShaderVar FadeoutMin = new ShaderVar();
	public ShaderVar FadeoutMax = new ShaderVar();

	/*asd*/[NonSerialized]public string[] AStencilChannelNames = {"Red","Green","Blue","Alpha"};
	/*asd*/[NonSerialized]public string[] AStencilChannelCodeNames = {"r","g","b","a"};

	static public string[] AVertexTypeNames = {"Normal","Local","World","View Space"};
	static public string[] AVertexAxisTypeNames = {"X","Y","Z"};

	public ShaderVar MixType = new ShaderVar();

	public ShaderVar VertexMask = new ShaderVar();
	public ShaderVar VertexSpace = new ShaderVar();

	public bool Initiated = false;

	public int SampleCount = 0;
	public string ShaderCodeSamplers;
	public string ShaderCodeEffects;
	public List<string> SpawnedBranches;

	//Effects
	public List<ShaderEffect> LayerEffects = new List<ShaderEffect>();
	public ShaderEffect FirstSelectedEffect = null;
	public List<ShaderEffect> SelectedEffect = new List<ShaderEffect>();

	//Generation Vars
	public int InfluenceCount;

	public override string ToString(){
		return Name.Text+" (ShaderLayer)";
	}
	public void SetType(string Type){
		LayerType.CType = Types.ObjectArray;
		if (CustomData==null)
			CustomData = new SVDictionary();
		if (ShaderSandwich.ShaderLayerTypeInstances!=null&&ShaderSandwich.ShaderLayerTypeInstances.ContainsKey(Type))
			LayerType.Obj = (object)ShaderSandwich.ShaderLayerTypeInstances[Type];
		if (LayerType.Obj!=null){
			LayerTypeInputs = ((ShaderLayerType)LayerType.Obj).GetInputs();
			ShaderUtil.SetupCustomData(CustomData,LayerTypeInputs);
		}
		//Debug.Log("I am a giant octopus");
		//Debug.Log(LayerType.Obj);
	}
	public void SetType(ShaderLayerType type){
		LayerType.CType = Types.ObjectArray;
		if (type!=null){
			LayerType.Obj = type;
			LayerTypeInputs = ((ShaderLayerType)LayerType.Obj).GetInputs();
			ShaderUtil.SetupCustomData(CustomData,LayerTypeInputs);
		}
	}
	//public ShaderLayer(){
	public void Awake(){
		Name = new ShaderVar("Layer Name","");
		LayerType = new ShaderVar("Layer Type","ListOfObjects","LayerType");
		LayerType.HookIntoObjects();
		if (ShaderSandwich.Instance!=null)
		ShaderSandwich.Instance.UpdateLayerTypeShaderVars();
		SetType("SLTColor");
		LayerTypeInputs = null;
		ShaderVars.Add(LayerType);

		UseAlpha = new ShaderVar("Use Alpha",false);
		//Previously  -  Replace, Dom't change, Blend, Erase
		AlphaBlendMode = new ShaderVar("Alpha Blend Mode",new string[]{"Blend","Blend Inside","Erase","Replace"},new string[]{"","","",""},4);
		Stencil = new ShaderVar("Stencil","ListOfObjects");
		Stencil.LightingMasks = false;//Parent.IsLighting.On;
		Stencil.NoInputs = true;

		MixAmount = new ShaderVar( "Mix Amount",1f);ShaderVars.Add(MixAmount);

		UseFadeout = new ShaderVar("Use Fadeout",false);
		FadeoutMinL = new ShaderVar("Fadeout Limit Min",0f);
		FadeoutMaxL = new ShaderVar("Fadeout Limit Max",10f);
		FadeoutMin = new ShaderVar("Fadeout Start",3f);
		FadeoutMax = new ShaderVar("Fadeout End",5f);

		MixType = new ShaderVar("Mix Type", new string[]{"Mix","Add","Subtract","Multiply","Divide","Lighten","Darken","Normals Mix","Dot"},new string[]{"","","","","","","","",""},3);
		ShaderVars.Add(MixType);
		
		MapType = new ShaderVar("UV Map",new string[]{
"UV Map",
"Generate",
"Position",
"Reflection",
"Direction",
"View",
},new string[]{"","","","","","","","",""},3);
		MapLocal = new ShaderVar("Map Local",false);
		MapDirection = new ShaderVar("Map Direction",new string[]{"Normal","Tangent","Bitangent"}, new string[]{"","",""});
		MapSpace = new ShaderVar("Map Space",new string[]{"World","Object","View","Projection","Texture"}, new string[]{"","","","",""});
		MapScreenSpace = new ShaderVar("Map Screen Space",new string[]{"World Space View Direction","Object Space View Direction","View Space View Direction (yup)","Texture View Direction","Screen Position","Pixel Screen Position"}, new string[]{"","","","",""});
		MapGenerateSpace = new ShaderVar("Map Generate Space",new string[]{"World","Object","Rim"}, new string[]{"","",""});
		MapInverted = new ShaderVar("Map Inverted",true);
		MapUVIndex = new ShaderVar("Map UV Index",1f);
		MapUVIndex.NoInputs = true;
		MapGenerateSpace.Type=1;
		MapScreenSpace.Type=4;
	}
	//public void Awake(){
	//	Stencil.HookIntoObjects();
	//}
	public void UpdateShaderVars(bool Link){
		ShaderVars.Clear();
		/*var fieldValues = this.GetType()
							 .GetFields()
							 .Select(field => field.GetValue(this))
							 .ToList();
		foreach(object obj in fieldValues)
		{
			ShaderVar SV = obj as ShaderVar;
			if (SV!=null)
			{
				SV.MyParent = this;
				ShaderVars.Add(SV);
			}
		}*/
		ShaderVar[] fieldValues = new ShaderVar[]{
			//OldLayerType,
			LayerType,
			Name,

			/*Color,
			Color2,
			Image,
			Cube,
			NoiseDim,
			NoiseType,
			NoiseA,
			NoiseB,
			NoiseC,
			LightData,
			SpecialType,
			LinearizeDepth,*/

			UseAlpha,
			AlphaBlendMode,
			Stencil,
			MapType,
			MapLocal,
			MapSpace,
			MapGenerateSpace,
			MapInverted,
			MapUVIndex,
			MapDirection,
			MapScreenSpace,
			MixAmount,
			UseFadeout,
			FadeoutMinL,
			FadeoutMaxL,
			FadeoutMin,
			FadeoutMax,
			MixType,
			//VertexMask,
			//VertexSpace
		};
		foreach(ShaderVar obj in fieldValues){
			ShaderVar SV = obj;
			if (SV!=null)
			{
				SV.Parent = this;
				ShaderVars.Add(SV);
			}
		}

		foreach(var CV in CustomData){
			CV.Value.Parent = this;
			ShaderVars.Add(CV.Value);
		}
		foreach(ShaderEffect SE in LayerEffects){
			foreach (KeyValuePair<string,ShaderVar> SV in SE.Inputs){
				SV.Value.Parent = this;
				ShaderVars.Add(SV.Value);
			}
		}
		
	}
	public string GetLayerCatagory(){
	return GetLayerCatagory_Real(true);
	}
	public string GetLayerCatagory(bool Add){
	return GetLayerCatagory_Real(Add);
	}
	public string BetterLayerCatagory(){
	string LC = Parent.LayerCatagory;

	if (LC=="Diffuse")
	LC = "Texture";
	if (LC=="Normals")
	LC = "Normal Map";

	return LC;
	}
	public string GetLayerCatagory_Real(bool Add){
		string LC = BetterLayerCatagory();

		if (Add)
			if (Parent.Inputs!=0)
				LC+=""+(Parent.Inputs+1).ToString();

		if (Add)
			Parent.Inputs+=1;
		return LC;
	}
		public void OnEnable(){
			Stencil.HookIntoObjects();
			//UnityEngine.Debug.Log("Good Layer");
		}
	public bool BugCheck(){
		if (CustomData==null)
			CustomData = new SVDictionary();
		//Stencil.SetToMasks(Parent,0);
		//Stencil.HookIntoObjects();
		//if (IsLighting){
		//	OldLayerType.Update(new string[] {"Color", "Gradient", "Light Data","Texture","Cubemap","Noise","Previous","Literal","Grab/Depth"},new string[] {"Color - A plain color.", "Gradient - A color that fades.", "Light Data - Access Lighting layer specific data.","Texture - A 2d texture.","Cubemap - A 3d reflection map.","Noise - Perlin Noise","Previous - The previous value.","Literal - Uses the value directly from the mapping.","Grab/Depth - Access the entire screen or the screen's depth buffer."},3);	
		//}
		//MixType.Update(new string[]{"Mix","Add","Subtract","Multiply","Divide","Lighten","Darken","Normals Mix","Dot"},new string[]{"","","","","","","","",""},3);

		//MapType.Update(new string[]{"UV Map","UV Map2","Reflection","Direction","Rim Light","Generate","View","Position"},new string[]{"The first UV Map","The second UV Map (Lightmapping)","The reflection based on the angle of the face.","Simply the angle of the face.","Maps from the edge to the center of the model, highlighting the rim.","Created the UVMap using tri-planar mapping.(Good for generated meshes)","Plaster the layer onto it from whatever view you are facing from.","Maps using the world position."},3);
		bool RetVal = false;
		//if (Color.UpdateToInput())
		//RetVal = true;
		//if (Color2.UpdateToInput())
		//RetVal = true;
		
		return RetVal;
	}

	public int GetDimensions(){
		return GetDimensions_Real(true);
	}
	public int GetDimensions(bool O){
		return GetDimensions_Real(O);
	}
	public bool UsesMap(){
		return (GetDimensions_Real(false)!=0);
	}
	public int GetDimensions_Real(bool O){
		return ((ShaderLayerType)LayerType.Obj).GetDimensions(CustomData,this);
	}

	public bool DrawIcon(Rect rect, bool Down){
		return DrawIcon_Real(rect,Down,true);
	}
	public bool DrawIcon(Rect rect){
		return DrawIcon_Real(rect,false,false);
	}
	public void PrintCustomData(){
		foreach(KeyValuePair<string,ShaderVar> CD in CustomData){
			UnityEngine.Debug.Log(CD.Key+" : "+CD.Value.Save(0));
		}
	}
	public void PrintJson(){
		try{
			UnityEngine.Debug.Log(JsonUtility.ToJson(this));
		}
		catch{
			UnityEngine.Debug.Log("As usual it errored...");
		}
	}
	public void DestroyCustomData(){
		CustomData = new SVDictionary();
	}
	public void DestroyCustomDataRecord(){
		SDebug.Stopwatch zxc = new SDebug.Stopwatch();
		foreach(string CD in CustomData.Keys.ToList()){
			zxc.Reset();
			zxc.Start();
				for(int i = 0;i<10000;i++){
					UnityEditor.Undo.RecordObject(this,"SHADER SANDWICH TEST");
				}
			zxc.Stop();
			
			long Before = zxc.ElapsedMilliseconds;
			
			CustomData.D.Remove(CD);
			
			zxc.Reset();
			zxc.Start();
				for(int i = 0;i<10000;i++){
					UnityEditor.Undo.RecordObject(this,"SHADER SANDWICH TEST");
				}
			zxc.Stop();
			
			long After = zxc.ElapsedMilliseconds;
			
			
			UnityEngine.Debug.Log(CD + " : "+Before.ToString()+"ms to "+After.ToString()+"ms, saving "+ (After-Before).ToString()+"ms");
		}
		//UnityEditor.Undo.RecordObject((UnityEngine.Object)(this,"SHADER SANDWICH TEST");
	}
	bool DrawIcon_Real(Rect rect,bool Down,bool Button){
		bool ret = false;
		if (Button==true){
			//if (Down==false){
				Color asd = GUI.color;
				GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,GUI.color.a*GUI.color.a);
				/*if (GUI.Button(rect,"",EMindGUI.ButtonImage)){
					ret=true;
					if (Event.current.button == 1 ){
						GenericMenu toolsMenu = new GenericMenu();
						toolsMenu.AddItem(new GUIContent("Copy"), false, ShaderSandwich.Instance.LayerCopy,this);
						toolsMenu.AddItem(new GUIContent("Paste"), false, ShaderSandwich.Instance.LayerPaste,Parent);
						if (ShaderSandwich.Instance.DEBUGMODE){
							toolsMenu.AddItem(new GUIContent("Debug/Print Custom Data"), false, PrintCustomData);
							toolsMenu.AddItem(new GUIContent("Debug/Print JSon"), false, PrintJson);
							toolsMenu.AddItem(new GUIContent("Debug/Destroy Custom Data"), false, DestroyCustomData);
							toolsMenu.AddItem(new GUIContent("Debug/Destroy Custom Data While Recording"), false, DestroyCustomDataRecord);
						}
						
						toolsMenu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0));
						EditorGUIUtility.ExitGUI();					
					}
				}*/
			//}
				if (EMindGUI.MouseUpIn(rect,false)){
					//ret=true;
					//Debug.Log("hmmm ;(");
					if (Event.current.button == 1 ){
						GenericMenu toolsMenu = new GenericMenu();
						toolsMenu.AddItem(new GUIContent("Copy"), false, ShaderSandwich.Instance.LayerCopy,this);
						toolsMenu.AddItem(new GUIContent("Paste"), false, ShaderSandwich.Instance.LayerPaste,Parent);
						toolsMenu.AddItem(new GUIContent("Delete"), false, ShaderSandwich.Instance.LayerDelete,this);
						if (ShaderSandwich.Instance.DEBUGMODE){
							toolsMenu.AddItem(new GUIContent("Debug/Print Custom Data"), false, PrintCustomData);
							toolsMenu.AddItem(new GUIContent("Debug/Print JSon"), false, PrintJson);
							toolsMenu.AddItem(new GUIContent("Debug/Destroy Custom Data"), false, DestroyCustomData);
							toolsMenu.AddItem(new GUIContent("Debug/Destroy Custom Data While Recording"), false, DestroyCustomDataRecord);
						}
						
						toolsMenu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0));
						EditorGUIUtility.ExitGUI();					
					}
				}
			//}
			//else
			///if (Down==true)
			///if (Event.current.type == EventType.Repaint)
			//ret = GUI.Toggle(rect,Down,"",EMindGUI.ButtonImage);
			///EMindGUI.ButtonImage.Draw(rect,false,true,true,true);
			
			GUI.color = asd;
		}
		//rect.x+=15;
		//rect.y+=15;
		//rect.width-=30;
		//rect.height-=30;
		rect.x+=8;
		rect.y+=8;
		rect.width-=16;
		rect.height-=16;
		
		//string mT = MixType.Names[MixType.Type];
		Material mT;
		
		//if (ShaderSandwich.Instance.BlendLayers)
		//mT = ShaderUtil.GetMaterial(MixType.Names[MixType.Type], new Color(1f,1f,1f,MixAmount.Float),UseAlpha.On);
		//else
		mT = ShaderUtil.GetMaterial("Mix", new Color(1f,1f,1f,GUI.color.a),false);
		
		Texture2D Tex = null;
		Tex = PreviewTex;//GetTexture();
		
		if (Event.current.type==EventType.Repaint){
			if (ShaderSandwich.Instance.PreviewSystem == PreviewSystem.CPU){
				if (Tex!=null)
				Graphics.DrawTexture( rect ,Tex ,mT);
			}else{
				if (_RTIcon!=null)
					Graphics.DrawTexture( rect ,RTIcon ,mT);
				//if (_RTIcon!=null)
				//	GUI.DrawTexture( rect ,RTIcon ,ScaleMode.StretchToFill,false);
			}
		}
		//GUI.DrawTexture(rect,Tex);
		GUI.color = new Color(1,1,1,1);
		//if (Down&&hasJSoned==500){
			//UnityEngine.Debug.Log(JsonUtility.ToJson(this));
			//UnityEngine.Debug.Log(Save(0));
			//System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(this.GetType());

            //TextWriter txtW = new StringWriter();
            //x.Serialize(txtW,this);
			//UnityEngine.Debug.Log(txtW.ToString());
		//}
		//hasJSoned++;
		return ret;
	}
//[NonSerialized]public int hasJSoned = 0;
	public ShaderColor GetSample(float X,float Y){
		return ((ShaderLayerType)LayerType.Obj).Preview(CustomData,this,new Vector2(X,Y),null,null,70,70);
	}
	bool MoreOrLess(float A1, float A2, float D){
		if ((A1<A2+D)&&(A1>A2-D))
		return true;
		
		return false;
	}
	bool MoreOrLess(int A1, int A2, int D){
		if ((A1<A2+D)&&(A1>A2-D))
		return true;
		
		return false;
	}
	[NonSerialized]public SVDictionary LayerTypeInputs = null;
	
	
	public Texture GetIcon(){
		if (ShaderSandwich.Instance.PreviewSystem == PreviewSystem.CPU){
			if (PreviewTex==null)
				if (Parent!=null)
					Parent.UpdateIcon(new Vector2(70,70));
			if (PreviewTex!=null)
				return PreviewTex;
		}else{
			if (_RTIcon==null)
				if (Parent!=null)
					Parent.UpdateIcon(new Vector2(70,70));
			if (_RTIcon!=null)
				return RTIcon;
		}
		return null;
	}
	
	//int MappingCat = 0;
	Vector2 InputDragMouseOffset;
	//ShaderEffect DragStart;
	bool InDrag = false;
	bool Dragged = false;
	
	public int LastGUIHeightImSoLazy = 300;
	public void DrawGUI(){
		//DrawGUI2();
		//return;
		if (CustomData==null)
		CustomData = new SVDictionary();
		
		//Color oldCol = GUI.color;
		Color oldColb = GUI.backgroundColor;
		
		string[] Options = new string[ShaderSandwich.PluginList["LayerType"].Count];
		string[] OptionsNames = new string[ShaderSandwich.PluginList["LayerType"].Count];
		bool[] OptionsEnabled = new bool[ShaderSandwich.PluginList["LayerType"].Count];
		var PVals = ShaderSandwich.PluginList["LayerType"].Values.ToArray();
		for(int i = 0;i<ShaderSandwich.PluginList["LayerType"].Count;i++){
			Options[i] = PVals[i].TypeS;
			OptionsNames[i] = PVals[i].Name;
			OptionsEnabled[i] = PVals[i].AllowInBase;
			if (IsVertex)
				OptionsEnabled[i] = PVals[i].AllowInVertex;
			if (IsPoke)
				OptionsEnabled[i] = PVals[i].AllowInPoke;
				
			//if (IsLighting)
			//	OptionsEnabled[i] = PVals[i].AllowInLighting;
			if (OptionsEnabled[i]==false)
			OptionsNames[i] = "";
		}
		//GUI.Box(new Rect(0,0,64,64),"");
		ShaderUtil.GPUGUIMat.Material.SetFloat("Transparent",UseAlpha.On?1f:0f);
		//Graphics.DrawTexture(new Rect(0,0,64,64),GetIcon(),ShaderUtil.GPUGUIMat.Material);
		EditorGUI.BeginChangeCheck();
		//LayerType.Text = Options[EditorGUI.Popup(new Rect(0,64-14,65,14),"",Mathf.Max(0,Array.IndexOf(Options,LayerType.Text)),OptionsNames,GUI.skin.GetStyle("MiniPopup"))];
		LayerType.SetCustomIcon(GetIcon());
		//Debug.Log("ohgod");
		if (EMindGUI.MouseUpIn(new Rect(72-36,72-8,36,8),false)){
			//Debug.Log("inyay");
			Parent.UpdateIcon(new Vector3(70,70),this);
		
			LayerType.Draw(new Rect(0,0,72,72),"");
			
			Parent.UpdateIcon(new Vector3(70,70),null);
		}else{
			LayerType.Draw(new Rect(0,0,72,72),"");
		}
		if (EditorGUI.EndChangeCheck()||LayerTypeInputs == null){
			//LayerTypeInputs = ShaderSandwich.PluginList["LayerType"][LayerType.Text].GetInputs();
			if (LayerType.Obj!=null)
			LayerTypeInputs = ((ShaderLayerType)LayerType.Obj).GetInputs();
		}
		int YOffset = 0;
		
		Name.Text = GUI.TextField(new Rect(72,0,250-72,20),Name.Text);YOffset += 20;
		
		//GUI.Label(new Rect(64,YOffset,250-64,64-YOffset),ShaderSandwich.PluginList["LayerType"][LayerType.Text].Description);
		if (LayerType.Obj!=null)
			GUI.Label(new Rect(72,YOffset,250-72,72-YOffset),"("+LayerType.Obj.ToString()+")\n"+((ShaderLayerType)LayerType.Obj).Description);
		if (LayerTypeInputs!=null)
			ShaderUtil.SetupCustomData(CustomData,LayerTypeInputs);YOffset += 20;
		
		YOffset = 72+8;
		
		if (Event.current.type==EventType.Repaint){
			//EMindGUI.SCSkin.window.Draw(new Rect(250/3,YOffset,250/3*2,16*4+20),"",false,true,true,true);
			//EMindGUI.SCSkin.window.Draw(new Rect(250/3+10,YOffset+20,250/3*2-10,16*4),"",false,true,true,true);
			EMindGUI.SCSkin.window.padding.top = 2;
			GUI.backgroundColor = new Color(GUI.backgroundColor.r,GUI.backgroundColor.g,GUI.backgroundColor.b,0.4f);
			EMindGUI.SCSkin.window.Draw(new Rect(0,YOffset,250,14),"Settings",false,true,true,true);
			GUI.backgroundColor = oldColb;
			EMindGUI.SCSkin.window.padding.top = 3;
		}
		YOffset+=19;
		
		//SVDictionary Trick = new SVDictionary();;
		//if (ShaderSandwich.PluginList["LayerType"][LayerType.Text].Draw(new Rect(6,YOffset,250-6,200),CustomData.D,this))
		if (LayerType.Obj!=null&&((ShaderLayerType)LayerType.Obj).Draw(new Rect(6,YOffset,250-6,200),CustomData,this))
			GUI.changed = true;
		
		if (LayerTypeInputs!=null)
			YOffset += (LayerTypeInputs.Count)*25;
		
		
		UseAlpha.Draw(new Rect(8,YOffset,250-16,20),"Transparent");YOffset += 25;
		YOffset += 10;
		//UV1,UV2,UV3,UV4,ReflectionWorld,ReflectionLocal,ReflectionView,DirectionWorld,DirectionLocal,DirectionView,RimLight,GenerateLocal,GenerateWorld,ViewPosition01,ViewPositionNeg11,PositionLocal,PositionWorld,PositionView,ViewDirectionWorld,ViewDirectionLocal
		//UVs 		- 	UV 1 - Textures, UV 2 - Lightmaps, UV 3, UV 4
		//Positions	-	World Position, Screen Position (0 to 1), Object Position, Screen Position (-1 to 1), View Space World Position
		//Directions-	World Normal(Direction), World Reflection, World View Direction, Local Normal, Local Reflection, View Normal, View Reflection, Local View Direction
		//Generate  -	Rim Light, Generate Local, Generate World
		
		TextAnchor oldalign = EMindGUI.SCSkin.window.alignment;
		int oldpadl = EMindGUI.SCSkin.window.padding.left;
		int oldpadt = EMindGUI.SCSkin.window.padding.top;
		int oldpadb = EMindGUI.SCSkin.window.padding.bottom;
		//int oldborderr = EMindGUI.SCSkin.window.border.right;
		//int oldoverflowr = EMindGUI.SCSkin.window.overflow.right;
		
		//EMindGUI.SCSkin.window.alignment = TextAnchor.UpperLeft;
		
		//EMindGUI.SCSkin.window.padding.left = 8;
		
		if (LayerType.Obj==null||((ShaderLayerType)LayerType.Obj).GetDimensions(CustomData,this)==0)
			GUI.enabled = false;
		int YOffsetblerg = YOffset;
		if (LayerType.Obj!=null&&((ShaderLayerType)LayerType.Obj).GetDimensions(CustomData,this)>0){
			
			if (Event.current.type==EventType.Repaint){
				//EMindGUI.SCSkin.window.Draw(new Rect(250/3,YOffset,250/3*2,16*4+20),"",false,true,true,true);
				//EMindGUI.SCSkin.window.Draw(new Rect(250/3+10,YOffset+20,250/3*2-10,16*4),"",false,true,true,true);
				EMindGUI.SCSkin.window.padding.top = 2;
				GUI.backgroundColor = new Color(GUI.backgroundColor.r,GUI.backgroundColor.g,GUI.backgroundColor.b,0.4f);
				EMindGUI.SCSkin.window.Draw(new Rect(0,YOffset,250,14),"Mapping",false,true,true,true);
				GUI.backgroundColor = oldColb;
				EMindGUI.SCSkin.window.padding.top = 3;
			}
			//EMindGUI.SCSkin.window.padding.left = 4;
			//EMindGUI.SCSkin.window.padding.top = 2;
			//EMindGUI.SCSkin.window.padding.bottom = 0;
			EMindGUI.SCSkin.window.alignment = TextAnchor.UpperCenter;
			
			YOffset += 19;
			
				MapType.Draw(new Rect(0,YOffset,250,20),"asdasdasd");
				//Debug.Log(MapType.Type);
				//Debug.Log(MapType.Float);
				YOffset += 20*2+5;
			
	//UV,// - TilingOffsetInput(?), UV Index
	//Generate,// World/Local*/Rim/RimInverted
	//Position,// World/Local/Texture/View/Projection
	//Reflection,// World/Local/Texture/View/Projection
	//Direction,// Normal/Tangent World/Local/Texture/View/Projection
	//Screen,// World/Local/Texture/View/Projection*
	//{"The first UV map (generally used for textures)","The second UV map (generally used for lightmaps)","The third UV map","The fourth UV map","The direction light would reflect","The direction light would reflect in world space, re-oriented to object space","The direction light would reflect in world space, re-oriented to view space","The direction the face points (normal)","The object space normal","The view space normal","Maps from the edge to the center of the model","Generates UVs using tri-planar mapping relative to the object - the UVs won't change when the object moves","Generates UVs using tri-planar mapping based on the world space position - when the object moves the UVs will slide oddly...","Plasters the layer onto the model from the camera's view","The position in view space, with (0,0) in the center","The object space position","The world space position","The view space position","The direction from the camera to the pixel","The direction from the camera to the pixel, re-oriented to object space"}
			LayerMappingType Mapping = (LayerMappingType)MapType.Type;
			if (Mapping==LayerMappingType.UV){
				MapUVIndex.LabelOffset = 250/2;
				MapUVIndex.Range0 = 1f;
				MapUVIndex.Range1 = 4f;
				MapUVIndex.Draw(new Rect(8,YOffset,250-16,20),"UV Map");YOffset+=25;
				MapUVIndex.Float = Mathf.Round(MapUVIndex.Float);
				MapUVIndex.NoInputs = true;
				if (MapUVIndex.Float>=1f&&MapUVIndex.Float<=4f)
					GUI.Label(new Rect(8,YOffset,250-16,16*3),new string[]{"The first UV map, usually used for texturing.","The second UV map, usually for lightmaps.","The third UV map, for...stuff...","And the fourth UV map, for other stuff...?"}[(int)MapUVIndex.Float-1]);
				else
					GUI.Label(new Rect(8,YOffset,250-16,16*3),"An as of yet unsupported UV map...you could say I future proofed this XD");
			}
			if (Mapping==LayerMappingType.Generate){
				MapGenerateSpace.LabelOffset = 250/2;
				MapInverted.LabelOffset = 250/2+30;
				GUI.Label(new Rect(8,YOffset,250/2-16,20),"Mode");
				MapGenerateSpace.Type = EditorGUI.Popup(new Rect(250/2+8,YOffset,250/2-16,20),MapGenerateSpace.Type,MapGenerateSpace.Names,EMindGUI.EditorPopup);YOffset+=25;
				if (MapGenerateSpace.Type<2)
					GUI.Label(new Rect(8,YOffset,250-16,16*3),new string[]{"Maps using tri-planar mapping based on the world space position - when the object moves the UVs will slide oddly...","Generates UVs using tri-planar mapping relative to the object - the UVs won't change when the object moves."}[(int)MapGenerateSpace.Type]);
				else{
					MapInverted.Draw(new Rect(8,YOffset,250-16,20),"Inverted");YOffset+=25;
					if (MapInverted.On)
						GUI.Label(new Rect(8,YOffset,250-16,16*3),"Maps from the center to the edge of the model.");///TODoO: Change based on Inverted
					else
						GUI.Label(new Rect(8,YOffset,250-16,16*3),"Maps from the edge to the center of the model.");
				}
			}
			if (Mapping==LayerMappingType.Position){
				MapSpace.LabelOffset = 250/2;
				GUI.Label(new Rect(8,YOffset,250/2-16,20),"Space");
				MapSpace.Type = EditorGUI.Popup(new Rect(250/2+8,YOffset,250/2-16,20),MapSpace.Type,MapSpace.Names,EMindGUI.EditorPopup);YOffset+=25;
				GUI.Label(new Rect(8,YOffset,250-16,16*3),new string[]{"Maps using the world position.","Maps using the local/object space position.","Maps using the world position rotated into the camera's view space.","Maps using the world position rotated into the camera's view space then squished into projection space.","Maps using the world position rotated into texture space - where the normal is up and tangent is left."}[(int)MapSpace.Type]);
			}
			if (Mapping==LayerMappingType.Reflection){
				MapSpace.LabelOffset = 250/2;
				GUI.Label(new Rect(8,YOffset,250/2-16,20),"Space");
				MapSpace.Type = EditorGUI.Popup(new Rect(250/2+8,YOffset,250/2-16,20),MapSpace.Type,MapSpace.Names,EMindGUI.EditorPopup);YOffset+=25;
				GUI.Label(new Rect(8,YOffset,250-16,16*3),new string[]{"The direction light would reflect off the surface - good for cubemap reflections and whatnot.","The direction light would reflect off the surface, re-oriented into object space.","The direction light would reflect off the surface, re-oriented into view space - useful for refractive effects.","The direction light would reflect, re-oriented into object space then squished into projection space.","The direction light would reflect off the surface, re-oriented into texture space - where the normal is up and tangent is left."}[(int)MapSpace.Type]);
			}
			if (Mapping==LayerMappingType.Direction){
				MapDirection.LabelOffset = 250/2;
				GUI.Label(new Rect(8,YOffset,250/2-16,20),"Direction");
				MapDirection.Type = EditorGUI.Popup(new Rect(250/2+8,YOffset,250/2-16,20),MapDirection.Type,MapDirection.Names,EMindGUI.EditorPopup);YOffset+=25;
				MapSpace.LabelOffset = 250/2;
				GUI.Label(new Rect(8,YOffset,250/2-16,20),"Space");
				MapSpace.Type = EditorGUI.Popup(new Rect(250/2+8,YOffset,250/2-16,20),MapSpace.Type,MapSpace.Names,EMindGUI.EditorPopup);YOffset+=25;
				if (MapDirection.Type==0)
					GUI.Label(new Rect(8,YOffset,250-16,16*3),new string[]{"The direction the surface points (normal).","The object space normal.","The view space normal.","The projection space normal.","Mostly float3(0,0,1), although it's much faster to just use that!"}[(int)MapSpace.Type]);
				else if (MapDirection.Type==1)
					GUI.Label(new Rect(8,YOffset,250-16,16*3),new string[]{"The direction the texture's x-axis points along the surface (tangent).","The object space tangent.","The view space tangent.","The projection space tangent.","Just float3(1,0,0), from memory...would be quicker to just use that though XD"}[(int)MapSpace.Type]);
				else if (MapDirection.Type==2)
					GUI.Label(new Rect(8,YOffset,250-16,16*3),new string[]{"The direction the texture's y-axis points along the surface (bitangent).","The object space bitangent.","The view space bitangent.","The projection space bitangent.","Just float3(0,1,0), I think...would be quicker to just use that though XD"}[(int)MapSpace.Type]);
			}//MapScreenSpace
			if (Mapping==LayerMappingType.View){
				MapScreenSpace.LabelOffset = 250/2;
				GUI.Label(new Rect(8,YOffset,250/2-16,20),"Space");
				MapScreenSpace.Type = EditorGUI.Popup(new Rect(250/2+8,YOffset,250/2-16,20),MapScreenSpace.Type,MapScreenSpace.Names,EMindGUI.EditorPopup);YOffset+=25;
				if (MapScreenSpace.Type<4){
					MapInverted.Draw(new Rect(8,YOffset,250-16,20),"Inverted");YOffset+=25;
				}
				if (MapInverted.On)
					GUI.Label(new Rect(8,YOffset,250-16,16*3),new string[]{"The view direction to the surface.","The view direction to the surface, re-oriented into object space.","Pretty colors...","The view direction to the surface, re-oriented into tangent space.","The surface's position and depth on screen.","The surface's position and depth on screen, per pixel"}[(int)MapScreenSpace.Type]);
				else{
					GUI.Label(new Rect(8,YOffset,250-16,16*3),new string[]{"The view direction from the surface to the camera.","The view direction from the surface to the camera, re-oriented into object space.","Pretty colors...","The view direction from the surface to the camera, re-oriented into tangent space.","The surface's position and depth on screen.","The surface's position and depth on screen, per pixel"}[(int)MapScreenSpace.Type]);
				}
				
			}//WorldObjectViewProjectionTexture
				//EMindGUI.SCSkin.window.Draw(new Rect(0,YOffset,250,16*4+20),"Mapping",false,true,true,true);
			YOffset = YOffsetblerg+20*3+40+19+20+10;
		}
		
		GUI.backgroundColor = oldColb;
		EMindGUI.SCSkin.window.padding.left = oldpadl;
		EMindGUI.SCSkin.window.padding.top = oldpadt;
		EMindGUI.SCSkin.window.padding.bottom = oldpadb;
		EMindGUI.SCSkin.window.alignment = oldalign;

		
		
		
		GUI.enabled = true;
		
		
		
		
		
		
		
		
		
		YOffsetblerg = YOffset;
		if (Event.current.type==EventType.Repaint){
			EMindGUI.SCSkin.window.padding.top = 2;
			GUI.backgroundColor = new Color(GUI.backgroundColor.r,GUI.backgroundColor.g,GUI.backgroundColor.b,0.4f);
			EMindGUI.SCSkin.window.Draw(new Rect(0,YOffset,250,14),"Effects",false,true,true,true);
			GUI.backgroundColor = oldColb;
			EMindGUI.SCSkin.window.padding.top = 3;
		}
		Rect rect = new Rect(8,YOffset,250-16,10);
		int SEBoxHeight = 60;
		foreach(ShaderEffect SE in LayerEffects){
			if (SEBoxHeight<(SE.Inputs.D.Count*25+25))
			SEBoxHeight = (SE.Inputs.D.Count*25+25);
			if (SE.CustomHeight>=0)
				if (SEBoxHeight<(SE.CustomHeight+25))
					SEBoxHeight = (SE.CustomHeight+25);
		}
		SEBoxHeight+=(LayerEffects.Count*15);
		rect.height = SEBoxHeight;
		//EMindGUI.BeginGroup(rect);
		if (Event.current.type==EventType.Repaint){
			EMindGUI.SCSkin.window.Draw(new Rect(rect.x,YOffset+20,rect.width,rect.height-20),"",false,true,true,true);
		}
		//GUI.Box(new Rect(0,YOffset,rect.width,20),"Effects");
		YOffset+=19;

		//if (SelectedEffect.Count==0&&LayerEffects.Count>=1){
		//	SelectedEffect.Add(LayerEffects[0]);
		//}
			
		//SelectedEffect = Mathf.Max(0,SelectedEffect);
		if (InDrag){
			if (InputDragMouseOffset.y-Event.current.mousePosition.y>15){
				Dragged = true;
				InputDragMouseOffset.y-=15;
				List<ShaderEffect> Sorted = new List<ShaderEffect>();
				for(int i = 0; i < LayerEffects.Count;i++){
					if (SelectedEffect.Contains(LayerEffects[i])){
						Sorted.Add(LayerEffects[i]);
					}
				}
				int NewIndex = Mathf.Max(0,LayerEffects.IndexOf(Sorted[0])-1);
				foreach(ShaderEffect se in Sorted){
					LayerEffects.Remove(se);
				}
				foreach(ShaderEffect se in Sorted){
					LayerEffects.Insert(NewIndex,se);
					NewIndex++;
				}
			}else if (Event.current.mousePosition.y-InputDragMouseOffset.y>15){
				Dragged = true;
				InputDragMouseOffset.y+=15;
				List<ShaderEffect> Sorted = new List<ShaderEffect>();
				for(int i = 0; i < LayerEffects.Count;i++){
					if (SelectedEffect.Contains(LayerEffects[i])){
						Sorted.Add(LayerEffects[i]);
					}
				}
				int NewIndex = Mathf.Min(LayerEffects.Count-Sorted.Count,LayerEffects.IndexOf(Sorted[0])+1);
				foreach(ShaderEffect se in Sorted){
					LayerEffects.Remove(se);
				}
				foreach(ShaderEffect se in Sorted){
					LayerEffects.Insert(NewIndex,se);
					NewIndex++;
				}
			}
		}
		int y = -1;
		foreach(ShaderEffect SE in LayerEffects){
			//SE.Activate(false);
			y+=1;
			bool Selected = false;
			if (SelectedEffect.Contains(SE))
				Selected = true;
			
			
			if (!InDrag&&EMindGUI.MouseDownIn(new Rect(rect.x,YOffset+y*15,rect.width-90,15),false)&&Event.current.button != 1){
				InDrag = true;
				Dragged = false;
				InputDragMouseOffset = new Vector3(rect.x,YOffset+y*15+7);//Event.current.mousePosition;
				//InputDragMouseOffset = Event.current.mousePosition;///TOoDO: Remove
				//DragStart = SE;
				if (Event.current.modifiers==EventModifiers.Control){
					EMindGUI.Defocus();
					if (!SelectedEffect.Contains(SE))
						SelectedEffect.Add(SE);
					else
						SelectedEffect.Remove(SE);
				}
				else if (Event.current.modifiers==EventModifiers.Shift&&SelectedEffect.Count>0){
					//if (!SelectedEffect.Contains(SE))
					//	SelectedEffect.Add(SE);
					//else
					//	SelectedEffect.Remove(SE);
					ShaderEffect Base = SelectedEffect[0];
					SelectedEffect.Clear();
					for(int i = Math.Min(LayerEffects.IndexOf(Base),y);i <= Math.Max(LayerEffects.IndexOf(Base),y);i++){
						SelectedEffect.Add(LayerEffects[i]);
					}
					//InDrag = true;
					//InputDragMouseOffset = Event.current.mousePosition;
					//DragStart = SE;
				}else if (SelectedEffect.Count<=1||!SelectedEffect.Contains(SE)){
					if (SelectedEffect.Count>1||(SelectedEffect.Count==1&&SelectedEffect[0]!=SE))
						EMindGUI.Defocus();
					SelectedEffect.Clear();
					SelectedEffect.Add(SE);
					//InDrag = true;
					//InputDragMouseOffset = Event.current.mousePosition;
					//DragStart = SE;
				}
			}
			if (!Dragged&&EMindGUI.MouseUpIn(new Rect(rect.x,YOffset+y*15,rect.width-90,15),false)){
				if ((Event.current.button != 1||!SelectedEffect.Contains(SE))&&Event.current.modifiers!=EventModifiers.Shift&&Event.current.modifiers!=EventModifiers.Control){
					if (SelectedEffect.Count>1||(SelectedEffect.Count==1&&SelectedEffect[0]!=SE))
						EMindGUI.Defocus();
					SelectedEffect.Clear();
					SelectedEffect.Add(SE);
				}
				if (Event.current.button == 1){
					GenericMenu toolsMenu = new GenericMenu();
						toolsMenu.AddItem(new GUIContent("Copy"), false, ShaderSandwich.Instance.EffectCopy,SelectedEffect);
						if (ShaderSandwich.Instance.ClipboardEffects!=null&&ShaderSandwich.Instance.ClipboardEffects.Count>0)
							toolsMenu.AddItem(new GUIContent("Paste"), false, EffectPaste,SE);
						else
							toolsMenu.AddDisabledItem(new GUIContent("Paste"));
						//toolsMenu.AddItem(new GUIContent("Delete"), false, SP.DeleteIngredient,ingredient);
					toolsMenu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0));
					GUI.changed = true;
					Event.current.Use();
				}
			}
			GUI.skin.button.alignment = TextAnchor.MiddleRight;
			Selected = GUI.Toggle(new Rect(rect.x,YOffset+y*15,rect.width-80,15),Selected,SE.Name,GUI.skin.button);
			GUI.skin.button.alignment = TextAnchor.MiddleCenter;
			///////////////////////////////////////
			bool GUIEN = GUI.enabled;
			GUI.enabled = false;
			//UnityEngine.Debug.Log(SE.TypeS+":"+SE.HandleAlpha.ToString());
			if (SE.HandleAlpha==true)
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
			///TODO:
				/*GUI.Toggle(new Rect(rect.x+rect.width-90+100,YOffset+y*15,15,15),SE.UseAlpha.Float==1||SE.UseAlpha.Float==2,(SE.UseAlpha.Float==1) ? ShaderSandwich.AlphaOn: ShaderSandwich.AlphaOff,ShaderUtil.Button2Border);
				if (GUI.Button(new Rect(rect.x+rect.width-90,YOffset+y*15,15,15),""))
					SE.UseAlpha.Float+=1;
				if (SE.UseAlpha.Float>2)
					SE.UseAlpha.Float = 0;*/
			SE.UseAlpha.Float = EditorGUI.Popup(new Rect(rect.x+rect.width-80,YOffset+y*15,50,15),(int)SE.UseAlpha.Float,new string[]{"RGB","RGBA","Alpha"},EMindGUI.EditorPopup);
			
			GUI.enabled = GUIEN;
			////////////////////////////////////////
			SE.Visible = GUI.Toggle(new Rect(rect.x+rect.width-30,YOffset+y*15,30,15),SE.Visible,SE.Visible ? ShaderSandwich.EyeOpen: ShaderSandwich.EyeClose,ShaderUtil.Button2Border);
			//if (GUI.Button(new Rect(rect.x+rect.width-15,YOffset+y*15,15,15),ShaderSandwich.CrossRed,ShaderUtil.Button2Border))
			//	Delete = SE;
		}

		y+=1;
		
		if (!Dragged&&EMindGUI.MouseUpIn(new Rect(rect.x,YOffsetblerg,rect.width,(int)rect.height),false)){
			if (Event.current.button == 1){
				GenericMenu toolsMenu = new GenericMenu();
					if (ShaderSandwich.Instance.ClipboardEffects!=null&&ShaderSandwich.Instance.ClipboardEffects.Count>0)
						toolsMenu.AddItem(new GUIContent("Paste"), false, EffectPaste,null);
					else
						toolsMenu.AddDisabledItem(new GUIContent("Paste"));
					//toolsMenu.AddItem(new GUIContent("Delete"), false, SP.DeleteIngredient,ingredient);
				toolsMenu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0));
				GUI.changed = true;
				Event.current.Use();
			}
		}
		
		if (ShaderSandwich.GUIMouseUp){
			if (InDrag)GUI.changed = true;
			InDrag = false;
			Dragged = false;
		}
		
		Color oldg = GUI.contentColor;
		GUI.contentColor = EMindGUI.BaseInterfaceColorComp2;
		//if (GUI.Button(EMindGUI.Rect2(RectDir.Right,rect.width,YOffset-20,20,20),"+",Button2Border)){
		if (GUI.Button(EMindGUI.Rect2(RectDir.Right,rect.x+rect.width,YOffset-19,17,17),ShaderSandwich.Plus,ShaderUtil.Button2Border)){
			GenericMenu toolsMenu = new GenericMenu();
			foreach(KeyValuePair<string,ShaderPlugin> Ty2 in ShaderSandwich.ShaderLayerEffectInstances){
				string Ty = Ty2.Key;
				//ShaderEffect SE = ShaderEffect.CreateInstance<ShaderEffect>();
				//SE.ShaderEffectIn(Ty,SL);
				toolsMenu.AddItem(new GUIContent(ShaderSandwich.ToNonDumbShaderLayerEffectInstances[Ty]), false, AddLayerEffect,Ty);
			}
			toolsMenu.DropDown(EMindGUI.Rect2(RectDir.Right,rect.x+rect.width,YOffset-20,20,20));
			//EditorGUIUtility.ExitGUI();
		}
		GUI.contentColor = EMindGUI.BaseInterfaceColorComp;
		if (GUI.Button( new Rect(rect.x,YOffset-19,17,17),ShaderSandwich.Bin,ShaderUtil.Button2Border)){
			//ShaderEffect ToRemove = SelectedEffect;
			ShaderEffect ToSelect = null;
			if (SelectedEffect.Count==1&&LayerEffects.IndexOf(SelectedEffect[0])+1<LayerEffects.Count)
				ToSelect=LayerEffects[LayerEffects.IndexOf(SelectedEffect[0])+1];
			else if (SelectedEffect.Count==1&&LayerEffects.IndexOf(SelectedEffect[0])-1>=0)
				ToSelect=LayerEffects[LayerEffects.IndexOf(SelectedEffect[0])-1];
		
			for(int i = LayerEffects.Count-1;i>=0;i--){
				if (SelectedEffect.Contains(LayerEffects[i]))
					LayerEffects.RemoveAt(i);
			}
			
			SelectedEffect.Clear();
			if (ToSelect!=null)SelectedEffect.Add(ToSelect);
		}
		GUI.contentColor=  oldg;
		if (SelectedEffect.Count == 1)
			SelectedEffect[0].Draw(new Rect(rect.x,YOffset+y*15,rect.width,SEBoxHeight));
		else if (SelectedEffect.Count > 1)
			GUI.Label(new Rect(rect.x,YOffset+y*15,rect.width,SEBoxHeight),"Multi effect editing not supported yet, sorry :(");
		
		
		
		
		
		///TOoDO: Trigger recompile on effect move
		
		
		
		
		
		
		//EMindGUI.EndGroup();
		//YOffset += ShaderUtil.DrawEffects(new Rect(8,YOffset,250-16,10),this,LayerEffects,ref SelectedEffect);
		YOffset = YOffsetblerg+(int)rect.height;
		
		if (Event.current.type==EventType.Repaint){
			//EMindGUI.SCSkin.window.Draw(new Rect(250/3,YOffset,250/3*2,16*4+20),"",false,true,true,true);
			//EMindGUI.SCSkin.window.Draw(new Rect(250/3+10,YOffset+20,250/3*2-10,16*4),"",false,true,true,true);
			EMindGUI.SCSkin.window.padding.top = 2;
			GUI.backgroundColor = new Color(GUI.backgroundColor.r,GUI.backgroundColor.g,GUI.backgroundColor.b,0.4f);
			EMindGUI.SCSkin.window.Draw(new Rect(0,YOffset,250,14),"Blending",false,true,true,true);
			GUI.backgroundColor = oldColb;
			EMindGUI.SCSkin.window.padding.top = 3;
		}
		//if (Event.current.type==EventType.Repaint)
		//	EMindGUI.SCSkin.window.Draw(new Rect(0,YOffset,250,100),"Blending",false,true,true,true);
		YOffset += 19;
		//MixType.Draw(new Rect(0,20,250,20),"Blend Mode:");
		ShaderUtil.DrawCoolSlider(new Rect(0,YOffset,250,10),MixAmount,0,true);YOffset += 8+10;
		GUI.Label(new Rect(8,YOffset,250-16,20),"Blend Mode");
		MixType.Type = EditorGUI.Popup(new Rect(120+8,YOffset,250-120-16,20),MixType.Type,MixType.Names,EMindGUI.EditorPopup);YOffset += 25;
		//UseAlpha.Draw(new Rect(0,15+8+5+25,250,20),"Use Alpha");
		//AlphaBlendMode.Type = EditorGUI.Popup(new Rect(120,15+8+5+25,250-120,20),AlphaBlendMode.Type,AlphaBlendMode.Names,EMindGUI.EditorPopup);
		if (Parent==null||Parent.EndTag.Text!="a"){
			if (EMindGUI.MouseDownIn(new Rect(120+8,YOffset,250-120-16,20))){
				GenericMenu menu = new GenericMenu();
				menu.AddItem(new GUIContent("Blend"), AlphaBlendMode.Type==0, SetAlphaMode, 0);
				if (Parent.EndTag.Text.Length==1){
					menu.AddDisabledItem(new GUIContent("Blend Inside"));
					menu.AddDisabledItem(new GUIContent("Erase"));
				}else{
					menu.AddItem(new GUIContent("Blend Inside"), AlphaBlendMode.Type==1, SetAlphaMode, 1);
					menu.AddItem(new GUIContent("Erase"), AlphaBlendMode.Type==2, SetAlphaMode, 2);
				}
				menu.AddItem(new GUIContent("Replace"), AlphaBlendMode.Type==3, SetAlphaMode, 3);
				menu.DropDown(new Rect(120,YOffset,250-120,20));
			}
			GUI.Box(new Rect(120+8,YOffset,250-120-16,20),new GUIContent(AlphaBlendMode.Names[AlphaBlendMode.Type]),EMindGUI.EditorPopup);
			GUI.Label(new Rect(8,YOffset,250-16,20),"Alpha Mode");YOffset += 25;
		}
		Stencil.LabelOffset = 250-120-10;
		Stencil.Draw(new Rect(8,YOffset,250-16,20),"Mask");YOffset += 25;
		LastGUIHeightImSoLazy = YOffset;
		
	}
	public void EffectPaste(object se){
		ShaderEffect SE = se as ShaderEffect;
		int i = LayerEffects.Count;
		if (SE!=null)
			i = LayerEffects.IndexOf(SE)+1;
		foreach(ShaderEffect sE in ShaderSandwich.Instance.ClipboardEffects){
			LayerEffects.Insert(i,sE.Copy());
			i++;
		}
	}
	public void DrawGUI2(){
		//return;
		if (CustomData==null)
		CustomData = new SVDictionary();
		
		Color oldCol = GUI.color;
		Color oldColb = GUI.backgroundColor;
		
		Name.Text = GUI.TextField(new Rect(0,0,250,20),Name.Text);
		
		string[] Options = new string[ShaderSandwich.PluginList["LayerType"].Count];
		string[] OptionsNames = new string[ShaderSandwich.PluginList["LayerType"].Count];
		bool[] OptionsEnabled = new bool[ShaderSandwich.PluginList["LayerType"].Count];
		var PVals = ShaderSandwich.PluginList["LayerType"].Values.ToArray();
		for(int i = 0;i<ShaderSandwich.PluginList["LayerType"].Count;i++){
			Options[i] = PVals[i].TypeS;
			OptionsNames[i] = PVals[i].Name;
			OptionsEnabled[i] = PVals[i].AllowInBase;
			if (IsVertex)
				OptionsEnabled[i] = PVals[i].AllowInVertex;
			if (IsPoke)
				OptionsEnabled[i] = PVals[i].AllowInPoke;
				
			//if (IsLighting)
			//	OptionsEnabled[i] = PVals[i].AllowInLighting;
			if (OptionsEnabled[i]==false)
			OptionsNames[i] = "";
		}
			
		
		EditorGUI.BeginChangeCheck();
		LayerType.Text = Options[EditorGUI.Popup(new Rect(0,20,250,20),"",Mathf.Max(0,Array.IndexOf(Options,LayerType.Text)),OptionsNames,GUI.skin.GetStyle("MiniPopup"))];
		if (EditorGUI.EndChangeCheck()||LayerTypeInputs == null||LayerTypeInputs.Count == 0){
			LayerTypeInputs = ShaderSandwich.PluginList["LayerType"][LayerType.Text].GetInputs();
		}
		int YOffset = 40;
		
		GUI.Label(new Rect(0,YOffset,250,20),ShaderSandwich.PluginList["LayerType"][LayerType.Text].Description);
		ShaderUtil.SetupCustomData(CustomData,LayerTypeInputs);
		
		//SVDictionary Trick = new SVDictionary();;
		if (ShaderSandwich.PluginList["LayerType"][LayerType.Text].Draw(new Rect(0,YOffset+20,250,200),CustomData,this))
		GUI.changed = true;
		
		YOffset+=(LayerTypeInputs.Count+1)*20+10;
		
		if (IsVertex){
			VertexMask.Float = (float)(int)(VertexMasks)EditorGUI.EnumPopup(new Rect(0,YOffset,250,20)," ",(VertexMasks)(int)VertexMask.Float,EMindGUI.EditorPopup);
			GUI.Label(new Rect(0,YOffset,250,20),"Vertex Base Angle ");
			YOffset+=20;
			
			VertexSpace.Float = (float)(int)(VertexSpaces)EditorGUI.EnumPopup(new Rect(0,YOffset,250,20)," ",(VertexSpaces)(int)VertexSpace.Float,EMindGUI.EditorPopup);
			GUI.Label(new Rect(0,YOffset,250,20),"Coordinate Space ");
			YOffset+=20;
		}
		///TODO:
		YOffset-=100;
		if (ShaderSandwich.Instance.CurInterfaceStyle==InterfaceStyle.Modern)
			YOffset-=70;
		else
			YOffset-=20;
		//if (OldLayerType.Type!=(int)LayerTypes.Previous&&OldLayerType.Type!=(int)LayerTypes.Literal&&OldLayerType.Type!=(int)LayerTypes.VertexColors)
		//YOffset+=50;
		
		int MixAmountHeight = 30;
		if (ShaderSandwich.Instance.CurInterfaceStyle==InterfaceStyle.Modern)
			MixAmountHeight = 20;
			
		//int MixAmountX = 0;
		//if (ShaderSandwich.Instance.CurInterfaceStyle==InterfaceStyle.Modern)
		//	MixAmountX = 20;
			
			if (Event.current.type == EventType.Repaint){
				GUI.skin.GetStyle("Button").Draw(new Rect(0,170+YOffset,250,MixAmountHeight),"",false,true,false,false);
				GUI.skin.label.wordWrap = false;
				//GUI.skin.label.alignment = TextAnchor.UpperCenter;
				//if (ShaderSandwich.Instance.CurInterfaceStyle==InterfaceStyle.Modern)
				GUI.color = EMindGUI.SetVal(EMindGUI.InterfaceColor,1f)*EMindGUI.SetVal(EMindGUI.InterfaceColor,1f);
				//else
				//	GUI.color = new Color(0.3f,0.8f,0.7f,1);
				
				GUI.backgroundColor = new Color(1,1,1,1);
					if (ShaderSandwich.Instance.CurInterfaceStyle==InterfaceStyle.Modern){
						if (MixAmount.Float>0.01f)
						EMindGUI.SCSkin.button.Draw(new Rect(20,170+YOffset,(250-MixAmountHeight)*MixAmount.Float,MixAmountHeight),"",false,true,false,false);
					}
					else
						GUI.skin.GetStyle("ProgressBarBar").Draw(new Rect(0,170+YOffset,(250)*MixAmount.Float,MixAmountHeight),"",false,true,false,false);
					
					GUI.color = new Color(1,1,1,1);
					GUI.Label(new Rect(80,170+YOffset,250,MixAmountHeight),MixType.Names[MixType.Type]+" Amount");
					GUI.color = new Color(0,0,0,1);
					GUI.Label(new Rect(80,170+YOffset,(250-MixAmountHeight)*MixAmount.Float-59,MixAmountHeight),MixType.Names[MixType.Type]+" Amount");
					
				GUI.skin.label.alignment = TextAnchor.UpperLeft;	
				GUI.skin.label.wordWrap = true;	
				//EditorGUI.ProgressBar(new Rect(0,200,250,30),MixAmount.Float,"Mix Amount");	
			}
			EditorGUIUtility.AddCursorRect(new Rect(20,170+YOffset,230,MixAmountHeight),MouseCursor.SlideArrow);
			MixAmountHeight = 20;
			
			MixAmount.DrawGear(new Rect(-290/2+26,170+YOffset,290,MixAmountHeight));
			GUI.color = new Color(1,1,1,0);
			//GUI.color = new Color(1,1,1,1);
			GUI.backgroundColor = oldColb;
			//	MixAmount.NoInputs = true;
				if (ShaderSandwich.Instance.CurInterfaceStyle==InterfaceStyle.Modern)
					MixAmount.Draw(new Rect(-5+20,170+YOffset,295-MixAmountHeight,MixAmountHeight),"");
				else
					MixAmount.Draw(new Rect(-5,170+YOffset,295,MixAmountHeight),"");
				MixAmount.LastUsedRect.x-=110;
				MixAmount.LastUsedRect.y+=10;
			GUI.color = oldCol;
			//MixAmount.NoInputs = false;
			MixAmount.DrawGear(new Rect(-290/2+26,170+YOffset,20,MixAmountHeight));
				
			if (ShaderSandwich.Instance.CurInterfaceStyle==InterfaceStyle.Modern)
				YOffset-=10;
		
		MixType.Draw(new Rect(0,200+YOffset,250,20));
		
		UseAlpha.Draw(new Rect(0,260+YOffset,210,20),"Transparent");
		YOffset+=20;
		//AlphaBlendMode.Type = EditorGUI.Popup(new Rect(120,260+YOffset,250-120,20),AlphaBlendMode.Type,AlphaBlendMode.Names,EMindGUI.EditorPopup);
		//"Blend","Blend Inside","Erase","Replace"
		
		if (EMindGUI.MouseDownIn(new Rect(120,260+YOffset,250-120,20))){
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Blend"), AlphaBlendMode.Type==0, SetAlphaMode, 0);
			if (Parent.EndTag.Text.Length==1){
				menu.AddDisabledItem(new GUIContent("Blend Inside"));
				menu.AddDisabledItem(new GUIContent("Erase"));
			}else{
				menu.AddItem(new GUIContent("Blend Inside"), AlphaBlendMode.Type==1, SetAlphaMode, 1);
				menu.AddItem(new GUIContent("Erase"), AlphaBlendMode.Type==2, SetAlphaMode, 2);
			}
			menu.AddItem(new GUIContent("Replace"), AlphaBlendMode.Type==3, SetAlphaMode, 3);
			menu.DropDown(new Rect(120,260+YOffset,250-120,20));
		}
		GUI.Box(new Rect(120,260+YOffset,250-120,20),new GUIContent(AlphaBlendMode.Names[AlphaBlendMode.Type]),EMindGUI.EditorPopup);
		GUI.Label(new Rect(0,260+YOffset,250,20),"Alpha Mode");
		YOffset+=20;
		//Fadeout
		if (!IsLighting){
		UseFadeout.Draw(new Rect(0,260+YOffset,210,20),"Fadeout ");
		YOffset+=20;
		if (UseFadeout.On){
			EditorGUI.BeginChangeCheck();
			EditorGUI.MinMaxSlider(new Rect(40,280+YOffset,180,20), ref FadeoutMin.Float, ref FadeoutMax.Float, FadeoutMinL.Float, FadeoutMaxL.Float);
			if (EditorGUI.EndChangeCheck()){
				GUI.changed = true;
				EditorGUIUtility.editingTextField = false;
				FadeoutMin.Float = Mathf.Max(FadeoutMinL.Float,Mathf.Round(FadeoutMin.Float*100)/100);
				FadeoutMax.Float = Mathf.Min(FadeoutMaxL.Float,Mathf.Round(FadeoutMax.Float*100)/100);
				FadeoutMin.UpdateToVar();
				FadeoutMax.UpdateToVar();
			}
			//MinL = EditorGUI.FloatField(new Rect(142,130,40,20),MinL);
			//MaxL = EditorGUI.FloatField(new Rect(BoxSize.x-40,130,40,20),MaxL);
			//FadeoutMin.Float = EditorGUI.FloatField(new Rect(180,240+YOffset,40,20),FadeoutMin.Float);
			FadeoutMax.NoSlider = true;
			FadeoutMax.NoArrows = true;
			
			FadeoutMin.NoSlider = true;
			FadeoutMin.NoArrows = true;
			
			FadeoutMinL.NoSlider = true;
			FadeoutMinL.NoArrows = true;
			
			FadeoutMaxL.NoSlider = true;
			FadeoutMaxL.NoArrows = true;
			
			FadeoutMin.LabelOffset = 70;
			FadeoutMax.LabelOffset = 70;
			
			//EditorGUI.BeginChangeCheck();
			FadeoutMin.Draw(new Rect(0,260+YOffset,120,17),"Start");
			FadeoutMax.Draw(new Rect(130,260+YOffset,120,17),"End");
			
			FadeoutMinL.Draw(new Rect(10,280+YOffset,20,17),"");
			FadeoutMaxL.Draw(new Rect(230,280+YOffset,20,17),"");
			//FadeoutMax.Float = EditorGUI.FloatField(new Rect(180,240+YOffset,40,20),FadeoutMax.Float);
			YOffset+=40;
		}
		}else{UseFadeout.On=false;}
		YOffset-=20;
		
		//if (Parent.Parallax||Parent.Name.Text=="Height"){
		//GUI.enabled = false;Stencil.Selected = -1;//}
		
		
		Stencil.Draw(new Rect(0,280+YOffset,250,20),"Mask ");

		
		GUI.enabled = true;
		
		YOffset+=20;
		if (((ShaderLayerType)ShaderSandwich.PluginList["LayerType"][LayerType.Text]).GetDimensions(CustomData,this)==0)
			GUI.enabled = false;
		
		
		MapSpace.Draw(new Rect(0,300+YOffset,250,20));
		YOffset+=20;
		MapType.Draw(new Rect(0,300+YOffset,250,20));
		GUI.enabled = true;
		
		YOffset+=380;
		YOffset+=15;
		//ShaderUtil.DrawEffects(new Rect(10,YOffset,230,10),this,LayerEffects,ref SelectedEffect);
	}
	public void SetAlphaMode(object i){
		AlphaBlendMode.Type = (int)i;
		GUI.changed = true;
		ShaderSandwich.Instance.RegenShaderPreview();
	}
	public void WarningFixDelegate(int Option,ShaderVar SV){
		if (Option==1){
			AddLayerEffect("SSENormalMap");///TODO:
			///LayerEffects[LayerEffects.Count-1].Inputs[2].Type = 0;
			///LayerEffects[LayerEffects.Count-1].AutoCreated = true;
		}
		if (Option==0){
			AddLayerEffect("SSESwizzle");
			///LayerEffects[LayerEffects.Count-1].AutoCreated = true;
			///LayerEffects[LayerEffects.Count-1].Inputs[0].Type = 3;
			///LayerEffects[LayerEffects.Count-1].Inputs[1].Type = 1;
			///LayerEffects[LayerEffects.Count-1].Inputs[2].Type = 0;
			///LayerEffects[LayerEffects.Count-1].Inputs[3].Type = 0;
		}
		//Image.WarningReset();
	}
	/*public void AddTextureOrCubemapInput(int Option,ShaderVar SV){
		if (Option==0){
			if (OldLayerType.Type==(int)LayerTypes.Texture){
				Image.AddInput();
			}
			if (OldLayerType.Type==(int)LayerTypes.Cubemap){
				Cube.AddInput();
			}
			Image.WarningReset();
			Cube.WarningReset();
		}
	}*/
	public void AddLayerEffect(object TypeName){
			ShaderEffect NewEffect = (ShaderEffect)ScriptableObject.CreateInstance(ShaderSandwich.ShaderLayerEffect[(string)TypeName]);
			NewEffect.Activate();
			//NewEffect.ShaderEffectIn((string)TypeName);//ShaderSandwich.EffectsList[0]);
			LayerEffects.Add(NewEffect);
			SelectedEffect.Clear();
			SelectedEffect.Add(NewEffect);
			if (Parent!=null)
				Parent.UpdateIcon(new Vector2(70,70));
	}
	static public void DrawGUIGen(bool Text){
		//Color oldCol = GUI.color;
		//Color oldColb = GUI.backgroundColor;
		GUI.Box(new Rect(0,0,250,21),"No layer selected!");
		/*if (Text)
		GUI.Box(new Rect(0,0,250,21),"The layers name");
		else
		GUI.Box(new Rect(0,0,250,21),"");
		
		int YOffset = 20;
		if (Text)
		GUI.Box(new Rect(0,0+YOffset,250,41),"The layer's type. This changes what the layer itself looks like.");
		else
		GUI.Box(new Rect(0,0+YOffset,250,41),"");
		if (Text)
		GUI.Box(new Rect(0,40+YOffset,250,31),"Some layer specific properties, such as the color of the layer or the texture it uses.");else
		GUI.Box(new Rect(0,40+YOffset,250,31),"");
		
		if (Text)
		GUI.Box(new Rect(0,70+YOffset,250,21),"How much the layer overwrites previous layers.");
		else
		GUI.Box(new Rect(0,70+YOffset,250,21),"");
		
		if (Text)
		GUI.Box(new Rect(0,90+YOffset,250,61),"How the layers blend. Mix is standard blending. Add on the other hand, adds the colors different components (Red, Green, Blue, Alpha) seperately.");
		else
		GUI.Box(new Rect(0,90+YOffset,250,61),"");	
		
		
		if (Text)
		GUI.Box(new Rect(0,150+YOffset,250,21),"Fadeout the layer by distance.");
		else
		GUI.Box(new Rect(0,150+YOffset,250,21),"");		
		
		if (Text)
		GUI.Box(new Rect(0,170+YOffset,250,21),"Use the layer's alpha when mixing.");
		else
		GUI.Box(new Rect(0,170+YOffset,250,21),"");		
		
		if (Text)
		GUI.Box(new Rect(0,190+YOffset,250,41),"The mask the layer uses.");
		else
		GUI.Box(new Rect(0,190+YOffset,250,41),"");		
		
		if (Text)
		GUI.Box(new Rect(0,230+YOffset,250,91),"How the layer is placed on the object. The layer needs to know how to choose what colour from it's texture or gradient to put at what point on the model.");
		else
		GUI.Box(new Rect(0,230+YOffset,250,91),"");
		
		if (Text)
		GUI.Box(new Rect(0,320+YOffset,250,91),"Layer effects which can alter what the layer looks like and how it's mapped.");
		else
		GUI.Box(new Rect(0,320+YOffset,250,91),"");*/

	}










	////////////////Code Gen Stuff
	//public string GCUVs(ShaderData SD,string OffsetX,string OffsetY, string OffsetZ){
	//return GCUVs_Real(SD,OffsetX,OffsetY,OffsetZ,true,-1);
	//}
	public string GCUVs(ShaderData SD,ref ShaderLayerBranch Branch){
	return GCUVs_Real(SD,"","","",true,ref Branch);
	}
	public string GCUVs(ShaderData SD,bool UseEffects,ref ShaderLayerBranch Branch){
	return GCUVs_Real(SD,"","","",UseEffects,ref Branch);
	}
	public string GCUVs(ShaderData SD,string OffsetX,string OffsetY, string OffsetZ,ref ShaderLayerBranch Branch){
	return GCUVs_Real(SD,OffsetX,OffsetY,OffsetZ,true,ref Branch);
	}
	public string GCUVs(ShaderData SD,string OffsetX,string OffsetY, string OffsetZ,bool UseEffects,ref ShaderLayerBranch Branch){
	return GCUVs_Real(SD,OffsetX,OffsetY,OffsetZ,UseEffects,ref Branch);
	}
	public string GCBaseMapping(ShaderData SD){
		string Map = "float3(0,0,0)";
		
		LayerMappingType Mapping = (LayerMappingType)MapType.Type;
		
		if (LazyGenerateTriplanarIndex!=-1){
			//string normal = "vd.worldNormal";
			string pos = "vd.worldPos";
			if (MapGenerateSpace.Type==1){
				//normal = "vd.localNormal";
				pos = GetSampleNameFirst()+"_GeneratePos";
			}
			if (GetDimensions(false)==1){
				if (LazyGenerateTriplanarIndex==0)
					Map = pos+".z";
				if (LazyGenerateTriplanarIndex==1)
					Map = pos+".z";
				if (LazyGenerateTriplanarIndex==2)
					Map = pos+".x";
			}
			if (GetDimensions(false)==2){
				if (LazyGenerateTriplanarIndex==0)
					Map = pos+".zy";
				if (LazyGenerateTriplanarIndex==1)
					Map = pos+".zx";
				if (LazyGenerateTriplanarIndex==2)
					Map = pos+".xy";
			}
			if (GetDimensions(false)==3){
				if (LazyGenerateTriplanarIndex==0)
					Map = "float3("+pos+".zy,0)";
				if (LazyGenerateTriplanarIndex==1)
					Map = "float3("+pos+".zx,0)";
				if (LazyGenerateTriplanarIndex==2)
					Map = "float3("+pos+".xy,0)";
			}
			return Map;
		}
		
		if (Mapping==LayerMappingType.UV){
			if (((ShaderLayerType)LayerType.Obj).TypeS=="SLTTexture")///TODO: Make less hardcoded
				UVTexture = ((ShaderVar)CustomData.D["Texture"]).Input;
			else
				UVTexture = null;
			
			string Index = "";
			if (MapUVIndex.Float>1f)Index = (MapUVIndex.Float).ToString();
			
			string UV = "";
			
			if (UVTexture==null||MapUVIndex.Float>1f)
				UV = "vd.genericTexcoord"+Index;
			else
				UV = "vd.uv"+Index+UVTexture.Get();
			
			Map = UV;//+".xy";
			//if (OldLayerType.Type == (int)LayerTypes.Cubemap)
			//	Map = "normalize(float3(1," + UV + ".y*-2-1," + UV + ".x*-2-1))";
		}
		if (Mapping==LayerMappingType.Reflection){///TODO: Double check makes any sense
			if (MapSpace.Type==0)
				Map = "vd.worldRefl";
			else if (MapSpace.Type==1)
				Map = "mul((float3x3)_World2Object, vd.worldRefl)";
			else if (MapSpace.Type==2)
				Map = "mul((float3x3)UNITY_MATRIX_V, vd.worldRefl)";
			else if (MapSpace.Type==3)
				Map = "mul((float3x3)UNITY_MATRIX_VP, vd.worldRefl)";
			else if (MapSpace.Type==4)
				Map = "mul(vd.worldRefl,float3x3(vd.TtoWSpaceX.xyz,vd.TtoWSpaceY.xyz,vd.TtoWSpaceZ.xyz))";
		}
		if (Mapping==LayerMappingType.Direction){///TODO: Double check makes any sense
			string normal = "vd.worldNormal";
			if (MapDirection.Type==1)
				normal = "vd.worldTangent";
			if (MapDirection.Type==2)
				normal = "vd.worldBitangent";
			if (MapSpace.Type==0)
				Map = normal;
			else if (MapSpace.Type==1)
				Map = "mul((float3x3)_World2Object, "+normal+")";
			else if (MapSpace.Type==2)
				Map = "mul((float3x3)UNITY_MATRIX_V, "+normal+")";
			else if (MapSpace.Type==3)
				Map = "mul((float3x3)UNITY_MATRIX_VP, "+normal+")";
			else if (MapSpace.Type==4)
				Map = "mul("+normal+",float3x3(vd.TtoWSpaceX.xyz,vd.TtoWSpaceY.xyz,vd.TtoWSpaceZ.xyz))";
		}
		if (Mapping==LayerMappingType.Position){///TODO: Double check makes any sense
			if (MapSpace.Type==0)
				Map = "vd.worldPos";
			else if (MapSpace.Type==1)
				Map = "mul(_World2Object, float4(vd.worldPos,1)).xyz";
			else if (MapSpace.Type==2)
				Map = "DivideByW_XY(mul(UNITY_MATRIX_V, float4(vd.worldPos,1)))";
			else if (MapSpace.Type==3)
				Map = "DivideByW_XY(mul(UNITY_MATRIX_VP, float4(vd.worldPos,1)))";
			else if (MapSpace.Type==4)
				Map = "mul(vd.worldPos,float3x3(vd.TtoWSpaceX.xyz,vd.TtoWSpaceY.xyz,vd.TtoWSpaceZ.xyz))";
			
			if (MapSpace.Type==2||MapSpace.Type==3){
				SD.AdditionalFunctions.Add(
		"	float3 DivideByW_XY(float4 blerg){\n"+
		"		return float3(blerg.xy/blerg.w,blerg.z);\n"+
		"	}\n");
			}
		}
		if (Mapping==LayerMappingType.Generate){
			if (MapGenerateSpace.Type == 2){//RimLight
				if (MapInverted.On)
					Map = "(1-dot(vd.worldNormal, vd.worldViewDir))";
				else
					Map = "dot(vd.worldNormal, vd.worldViewDir)";
			}
			if (MapGenerateSpace.Type==1){//Object
				Map = "mul(_World2Object, float4(vd.worldPos,1)).xyz";
			}
			if (MapGenerateSpace.Type==0){//World
				Map = "vd.worldPos";
			}
		}
		if (Mapping==LayerMappingType.View){///TODO: Double check makes any sense
			string inv = MapInverted.On?"-":"";
			if (MapScreenSpace.Type==0)
				Map = inv+"vd.worldViewDir";
			else if (MapScreenSpace.Type==1)
				Map = "mul((float3x3)_World2Object, "+inv+"vd.worldViewDir)";
			else if (MapScreenSpace.Type==2)
				Map = "mul((float3x3)UNITY_MATRIX_V, "+inv+"vd.worldViewDir)";
			else if (MapScreenSpace.Type==3)
				Map = inv+"vd.tangentViewDir";
			else if (MapScreenSpace.Type==4)
				Map = "vd.screenPos.xyz";
			else if (MapScreenSpace.Type==5){
				Map = "vtp.position.xyz";
				if (SD.ShaderModel == ShaderModel.SM2||SD.ShaderModel==ShaderModel.SM2_5||SD.ShaderModel==ShaderModel.SM3)
					Map = "floor(vd.screenPos.xyz*half3(_ScreenParams.xy,1))";
			}
		}
		return Map;
	}
	public string blerg(){
		return "blerg";
	}
	public string GCAlterMapping(ShaderData SD, string Map, int UVDimensions, int TypeDimensions,string OffsetX,string OffsetY, string OffsetZ,bool UseEffects,ref ShaderLayerBranch Branch){
		//LayerMappingType Mapping = (LayerMappingType)MapType.Type;
		//if (Mapping==LayerMappingType.Generate&&MapGenerateSpace.Type<2)
		//	TypeDimensions=3;
		if (TypeDimensions==2){
			if (UVDimensions == 1)
				Map = "float2("+Map+".rr)";
		}
		if (TypeDimensions==3){
			if (UVDimensions == 1)
				Map = "float3("+Map+".rrr)";
			if (UVDimensions == 2)
				Map = "float3("+Map+",0)";
		}
		if (TypeDimensions>UVDimensions)
			UVDimensions = TypeDimensions;
		if (LazyGenerateTriplanarIndex!=-1)
			UVDimensions = TypeDimensions;
		
		int effectIter = 0;
		if (UseEffects){
			//LayerEffects.Reverse();
			//foreach(ShaderEffect SE in LayerEffects){
			for(effectIter = 0;effectIter<LayerEffects.Count;effectIter++){
				ShaderEffect SE = LayerEffects[effectIter];
				if (effectIter==Branch.Effect)
					break;
				if (SE.Visible){
					
					//Debug.Log(Name);
					//Debug.Log(SE.Name);
					var Meth = ShaderEffect.GetMethod(SE.TypeS,"GenerateMap");
					if (Meth!=null){
						Map = SE.GenerateMap(SE.Inputs, SD, this, Map, ref UVDimensions, ref TypeDimensions);
					}
				}
			}
			//LayerEffects.Reverse();
		}
		/*if (Branch.MapXOffset.Length>0)
			OffsetX += OffsetX.Length>0?" + ":"" + Branch.MapXOffset;
		if (Branch.MapYOffset.Length>0)
			OffsetY += OffsetY.Length>0?" + ":"" + Branch.MapYOffset;
		if (Branch.MapZOffset.Length>0)
			OffsetZ += OffsetZ.Length>0?" + ":"" + Branch.MapZOffset;*/
		//Debug.Log(OffsetX);
		//Debug.Log(Branch.MapXOffset);
		if (OffsetX.Length>0)
			Branch.MapXOffset += (Branch.MapXOffset.Length>0?" + ":"") + OffsetX;
		if (OffsetY.Length>0)
			Branch.MapYOffset += (Branch.MapYOffset.Length>0?" + ":"") + OffsetY;
		if (OffsetZ.Length>0)
			Branch.MapZOffset += (Branch.MapZOffset.Length>0?" + ":"") + OffsetZ;
		
		//Debug.Log(Branch.MapXOffset);
		if (UVDimensions==1){
			if (Branch.MapXOffset!="")
				Map="(" + Map + " + "+Branch.MapXOffset + ")";
			//if (SG.MapDispOn==true)
			//Map="("+Map+"+MapDisp.r)";
		}
		if (UVDimensions==2){
			if (Branch.MapXOffset!="")
				Map="(" + Map + " + float2("+Branch.MapXOffset+", "+Branch.MapYOffset+"))";
			//if (SG.MapDispOn==true)
			//Map="("+Map+"+MapDisp.rg)";
		}
		if (UVDimensions==3){
			if (Branch.MapXOffset!="")
				Map="(" + Map + " + float3("+Branch.MapXOffset+", "+Branch.MapYOffset+", "+Branch.MapZOffset+"))";
			//if (SG.MapDispOn==true)
			//Map="("+Map+"+MapDisp.rgb)";			
		}
		
		if (UseEffects&&Branch.Effect>-1){
			//LayerEffects.Reverse();
			for(;effectIter<LayerEffects.Count;effectIter++){
				ShaderEffect SE = LayerEffects[effectIter];
				if (SE.Visible){
					var Meth = ShaderEffect.GetMethod(SE.TypeS,"GenerateMap");
					if (Meth!=null){
						//object[] Vars = new object[]{SD,SE,this,Map,UVDimensions,TypeDimensions};
						//Map = (string)Meth.Invooke(null,Vars);
						//UVDimensions = (int)Vars[4];
						//TypeDimensions = (int)Vars[5];
						//Dictionary<string,ShaderVar> Inputs, ShaderData SD, ShaderLayer SL, string Map, ref int UVDimensions, ref int TypeDimensions
						Map = SE.GenerateMap(SE.Inputs, SD, this, Map, ref UVDimensions, ref TypeDimensions);
					}
				}
			}
			//LayerEffects.Reverse();
		}
		if (TypeDimensions==1){
			if (UVDimensions == 2)
				Map = Map+".x";
			if (UVDimensions == 3)
				Map = Map+".x";
		}
		if (TypeDimensions==2){
			if (UVDimensions == 1)
				Map = "float2("+Map+".rr)";
			if (UVDimensions == 3)
				Map = Map+".xy";
		}
		if (TypeDimensions==3){
			if (UVDimensions == 1)
				Map = "float3("+Map+".rrr)";
			if (UVDimensions == 2)
				Map = "float3("+Map+",0)";
		}
		UVDimensions = TypeDimensions;
		
		return Map;
	}
	public string GCUVs_Real(ShaderData SD,string OffsetX,string OffsetY, string OffsetZ,bool UseEffects,ref ShaderLayerBranch Branch){
		int TypeDimensions = GCTypeDimensions();
		int UVDimensions = GCUVDimensions();
		if (TypeDimensions==0)
			return "0";
				
		string Map = GCBaseMapping(SD);
		Map = GCAlterMapping(SD,Map,UVDimensions,TypeDimensions, OffsetX, OffsetY,  OffsetZ, UseEffects, ref Branch);

		return Map;
	}
	public string GCOrigUVs_Real(ShaderData SD,string OffsetX,string OffsetY, string OffsetZ,bool UseEffects,ref ShaderLayerBranch Branch){//Yeah this is hacky as hell but I have no time...
		int TypeDimensions = GCTypeDimensions();
		int UVDimensions = GCUVDimensions();
		if (TypeDimensions==0)
			return "0";
				
		string Map = GCBaseMapping(SD);
		
		Map = Map.Replace("vd.","vdUnchanged.");
		
		Map = GCAlterMapping(SD,Map,UVDimensions,TypeDimensions, OffsetX, OffsetY,  OffsetZ, UseEffects, ref Branch);
		
		
		return Map;
	}

	public int GCDimensions(){
		return GetDimensions();
	}
	public int GCTypeDimensions(){
		return GetDimensions();
	}
	public int GCUVDimensions(){
		int Dim = 2;
		LayerMappingType Mapping = (LayerMappingType)MapType.Type;
		
		
		if (Mapping==LayerMappingType.UV)
			Dim = 2;
			
		if (Mapping==LayerMappingType.Reflection)
			Dim = 3;
		if (Mapping==LayerMappingType.View)
			Dim = 3;
		if (Mapping==LayerMappingType.Direction)
			Dim = 3;
		if (Mapping==LayerMappingType.Generate&&MapGenerateSpace.Type==2)//RimLight
			Dim = 1;
		
		if (Mapping==LayerMappingType.Position||(Mapping==LayerMappingType.Generate&&MapGenerateSpace.Type<2))
			Dim = 3;
		return Dim;
	}
	public string GCPixelBase(ShaderData SD,string Map, ref ShaderLayerBranch Branch){
		return GCPixelBase_Real(SD,Map,ref Branch);
	}
	public string GCPixelBase_Real(ShaderData SD,string Map, ref ShaderLayerBranch Branch){
		string PixCol = "";
		PixCol = ((ShaderLayerType)LayerType.Obj).Generate(CustomData, this, SD, Map);
		//if (UsesAlpha()&&!UseAlpha.On){
		//	PixCol = "half4("+PixCol+".rgb,1)";
		//}
		//if ((LayerType.Text!="SLTPrevious")&&(!Map.Contains("*(depth)"))){
			///if (!SG.UsedBases.ContainsKey(PixCol))
			///SG.UsedBases.Add(PixCol,1);
			///else
			///SG.UsedBases[PixCol]+=1;
		//}
		//foreach(ShaderEffect SE2 in LayerEffects){
		for(int i = LayerEffects.Count-1;i>=0;i--){
			ShaderEffect SE = LayerEffects[i];
			if (SE.Visible){
				/*if (ShaderEffect.GetMethod(SE2.TypeS,"GenerateBase")!=null){
					if (ShaderEffect.GetMethod(SE2.TypeS,"GenerateBase").GetParameters().Length==4)
					PixCol = (string)ShaderEffect.GetMethod(SE2.TypeS,"GenerateBase").Invoke(null,new object[]{SE2,this,PixCol,Map});
					else
					PixCol = (string)ShaderEffect.GetMethod(SE2.TypeS,"GenerateBase").Invoke(null,new object[]{SD,SE2,this,PixCol,Map});
				}*/
				PixCol = SE.GenerateBase(SD,this,SE.Inputs,PixCol,Map,ref Branch);
			}
		}
		return PixCol;
	}
	//Nope taken the lazy way instead XD
	//public string StartNewBranch(ShaderData SD,int Effect){
	//	return StartNewBranch(SD,"","","",true,Effect);
	//}
	//public string StartNewBranch(ShaderData SD,string OffsetX,string OffsetY, string OffsetZ,bool UseEffects,int Effect){
	public string StartNewBranch(ShaderData SD,string Map,ShaderLayerBranch Branch){
		SampleCount+=1;
		int myBranch = SampleCount;
	//	UnityEngine.Debug.Log(Branch.Effect.ToString()+": " + Map);
		//Branch.Effect++;
		int AlphaResetIndex = -1;
		for(int i = Branch.Effect;i<LayerEffects.Count;i+=1){
			
			Branch.Effect++;
			ShaderEffect SE = LayerEffects[i];
			//UnityEngine.Debug.Log(Effect.ToString()+": "+SE.TypeS);
			if (!SE.Visible)
			continue;
			
			//var Meth = ShaderEffect.GetMethod(SE.TypeS,"Generate");
			//var Meth2 = ShaderEffect.GetMethod(SE.TypeS,"GenerateAlpha");
			//var Meth3 = ShaderEffect.GetMethod(SE.TypeS,"GenerateWAlpha");
			string EffectString = "";
			int OldShaderCodeEffectsLength = ShaderCodeEffects.Length;
			
			ShaderLayerBranch NextBranch = Branch;
			if (SE.UseAlpha.Float==0){
				string gen = SE.Generate(SD,this,SE.Inputs,GetSampleName(myBranch)+(SE.WantsFullLine?"":".rgb"),NextBranch);
				if (gen.Length>0)
					EffectString = GetSampleName(myBranch)+".rgb = "+gen+";\n";
			}
			if (SE.UseAlpha.Float==1){
				string gen = SE.GenerateWAlpha(SD,this,SE.Inputs,GetSampleName(myBranch),NextBranch);
				if (gen.Length>0)
					EffectString = GetSampleName(myBranch)+" = "+gen+";\n";
			}
			if (SE.UseAlpha.Float==2){
				string gen = SE.GenerateAlpha(SD,this,SE.Inputs,GetSampleName(myBranch)+".a",NextBranch);
				if (gen.Length>0)
					EffectString = GetSampleName(myBranch)+".a = "+gen+";\n";
			}
			if (EffectString.Length>0){
				if (SE.UseAlpha.Float>0){
					if (SE.WantsFullLine)
						AlphaResetIndex = OldShaderCodeEffectsLength;
					else
						AlphaResetIndex = OldShaderCodeEffectsLength+EffectString.Length;
				}
				//ShaderCodeEffects = ShaderCodeEffects.Insert(0,EffectString);
				ShaderCodeEffects = ShaderCodeEffects.Insert(ShaderCodeEffects.Length-OldShaderCodeEffectsLength,EffectString);
			}
		}
		if (AlphaResetIndex>-1&&UsesAlpha()&&!UseAlpha.On)
			ShaderCodeEffects = ShaderCodeEffects.Insert(ShaderCodeEffects.Length-AlphaResetIndex,GetSampleName(myBranch)+".a = 1;\n");

		string PixCol = GCPixelBase(SD,Map,ref Branch);
		if (PixCol.Length>0)
			ShaderCodeSamplers += "				half4 "+GetSampleName(myBranch)+" = "+PixCol+";\n";
		else
			ShaderCodeSamplers = "";
		return GetSampleName(myBranch);
	}
	/*public string GetSubPixel(ShaderGenerate SG,string Map,int Effect,int Branch){
		
		ShaderEffect SE = null;
		if (LayerEffects.Count>Effect){
			SE = LayerEffects[Effect];
			if (!SE.Visible)
			return GetSubPixel(SG,Map,Effect+1,Branch);
			
			var Meth = ShaderEffect.GetMethod(SE.TypeS,"Generate");
			var Meth2 = ShaderEffect.GetMethod(SE.TypeS,"GenerateAlpha");
			var Meth3 = ShaderEffect.GetMethod(SE.TypeS,"GenerateWAlpha");
			string EffectString = "";
			if (SE.UseAlpha.Float==0)
				EffectString = GetSampleName(Branch)+".rgb = "+(string)Meth.Invooke(null,new object[]{SG,SE,this,GetSampleName(Branch)+".rgb",Effect+1})+";\n";
			if (SE.UseAlpha.Float==1)
				EffectString = GetSampleName(Branch)+" = "+(string)Meth3.Invooke(null,new object[]{SG,SE,this,GetSampleName(Branch),Effect+1})+";\n";
			if (SE.UseAlpha.Float==2)
				EffectString = GetSampleName(Branch)+".a = "+(string)Meth2.Invooke(null,new object[]{SG,SE,this,GetSampleName(Branch)+".a",Effect+1})+";\n";
			ShaderCodeEffects=ShaderCodeEffects+EffectString;
			return GetSubPixel(SG,Map,Effect+1,Branch);
		}
		if (!SpawnedBranches.Contains(GetSampleName(Branch))){
			//SpawnedBranches.Add(GetSampleName(Branch));

			
			string PixCol = GCPixelBase(SG,Map);
			foreach(ShaderEffect SE2 in LayerEffects){
				if (ShaderEffect.GetMethod(SE2.TypeS,"GenerateBase")!=null){
					if (ShaderEffect.GetMethod(SE2.TypeS,"GenerateBase").GetParameters().Length==4)
						return (string)ShaderEffect.GetMethod(SE2.TypeS,"GenerateBase").Invooke(null,new object[]{SE2,this,PixCol,Map});
					else
						return (string)ShaderEffect.GetMethod(SE2.TypeS,"GenerateBase").Invooke(null,new object[]{SG,SE2,this,PixCol,Map});
				}
			}
			ShaderCodeSamplers = "				half4 "+GetSampleName(Branch)+" = "+PixCol+";\n"+ShaderCodeSamplers;
		}
		string RetName = GetSampleName(Branch);
		return RetName;
	}*/
	public string GetSampleName(int Branch){
		return ShaderUtil.CodeName(Name.Text+Parent.NameUnique.Text)+"_Sample"+Branch.ToString();//SampleCount.ToString();
	}
	public string GetSampleNameFirst(){
		return ShaderUtil.CodeName(Name.Text+Parent.NameUnique.Text)+"_Sample1";
	}
	public int LazyGenerateTriplanarIndex = 0;//x,y,z duh
	public string GCPixel(ShaderData SD){
		if (LayerType.Obj==null)
			return "";
		ShaderCodeSamplers = "";
		ShaderCodeEffects = "";
		SampleCount = 0;
		LazyGenerateTriplanarIndex = -1;
		SpawnedBranches = new List<string>();
	//	string PixCol;
		//if (UseEffects){
		LayerEffects.Reverse();
		if (MapType.Type==(int)LayerMappingType.Generate&&MapGenerateSpace.Type<2&&GCTypeDimensions()>0){
			string normal = "vd.worldNormal";
			string pos = "vd.worldPos";
			if (MapGenerateSpace.Type==1){
				normal = "vd.localNormal";
				ShaderCodeSamplers += "				float3 "+GetSampleNameFirst()+"_GeneratePos = mul(_World2Object, float4(vd.worldPos,1)).xyz;\n";
				pos = GetSampleNameFirst()+"_GeneratePos";
			}
			
			string XAxis = "1";
			string YAxis = "1";
			string ZAxis = "1";
			//Debug.Log(GetDimensions(false));
			ShaderLayerBranch BranchX = new ShaderLayerBranch(false);
			ShaderLayerBranch BranchY = new ShaderLayerBranch(false);
			ShaderLayerBranch BranchZ = new ShaderLayerBranch(false);
			if (GetDimensions(false)==1){
				LazyGenerateTriplanarIndex = 0;
				XAxis = StartNewBranch(SD,GCAlterMapping(SD,pos+".z",1,1, "","","", true, ref BranchX),BranchX);
				LazyGenerateTriplanarIndex = 1;
				YAxis = StartNewBranch(SD,GCAlterMapping(SD,pos+".z",1,1, "","","", true, ref BranchY),BranchY);
				LazyGenerateTriplanarIndex = 2;
				ZAxis = StartNewBranch(SD,GCAlterMapping(SD,pos+".x",1,1, "","","", true, ref BranchZ),BranchZ);
			}
			else if (GetDimensions(false)==2){
				LazyGenerateTriplanarIndex = 0;
				XAxis = StartNewBranch(SD,GCAlterMapping(SD,pos+".zy",2,2, "","","", true, ref BranchX),BranchX);
				LazyGenerateTriplanarIndex = 1;
				YAxis = StartNewBranch(SD,GCAlterMapping(SD,pos+".zx",2,2, "","","", true, ref BranchY),BranchY);
				LazyGenerateTriplanarIndex = 2;
				ZAxis = StartNewBranch(SD,GCAlterMapping(SD,pos+".xy",2,2, "","","", true, ref BranchZ),BranchZ);
			}
			else if (GetDimensions(false)==3){
				LazyGenerateTriplanarIndex = 0;
				XAxis = StartNewBranch(SD,GCAlterMapping(SD,"float3("+pos+".zy,0)",3,3, "","","", true, ref BranchX),BranchX);
				LazyGenerateTriplanarIndex = 1;
				YAxis = StartNewBranch(SD,GCAlterMapping(SD,"float3("+pos+".zx,0)",3,3, "","","", true, ref BranchY),BranchY);
				LazyGenerateTriplanarIndex = 2;
				ZAxis = StartNewBranch(SD,GCAlterMapping(SD,"float3("+pos+".xy,0)",3,3, "","","", true, ref BranchZ),BranchZ);
			}
			//Debug.Log(ShaderCodeSamplers);
			SD.AdditionalFunctions.Add(
		"	float4 GenerateTriPlanar(float4 XAxis,float4 YAxis, float4 ZAxis, float3 Map, float idk){\n"+
		"		half3 blend = pow(abs(Map),idk);\n"+
		"		blend /= blend.x+blend.y+blend.z;\n"+
		"		return XAxis*blend.x + YAxis*blend.y + ZAxis*blend.z;\n"+
		"	}\n");
			ShaderCodeEffects += GetSampleNameFirst()+" = GenerateTriPlanar("+XAxis+", "+YAxis+", "+ZAxis+","+normal+",5);\n";
			//if (GetDimensions(false)==1)
			//	return "GenerateTriPlanar("+GCPixelBase_Real(SD,"("+Map+").z")+", "+GCPixelBase_Real(SD,"("+Map+").z")+", "+GCPixelBase_Real(SD,"("+Map+").x")+", " + normal + ",5)";
			//if (GetDimensions(false)==2)
			//	return "GenerateTriPlanar("+GCPixelBase_Real(SD,"("+Map+").zy")+", "+GCPixelBase_Real(SD,"("+Map+").zx")+", "+GCPixelBase_Real(SD,"("+Map+").xy")+", " + normal + ",5)";
			//if (GetDimensions(false)==3)
			//	return "GenerateTriPlanar("+GCPixelBase_Real(SD,"float3(("+Map+").zy,0)")+", "+GCPixelBase_Real(SD,"float3(("+Map+").zx,0)")+", "+GCPixelBase_Real(SD,"float3(("+Map+").xy,0)")+", " + normal + ",5)";
		}else{
			LazyGenerateTriplanarIndex = -1;
			//GCUVs(SD,new ShaderLayerBranch(false));
			ShaderLayerBranch OriginBranch = new ShaderLayerBranch(false);
			StartNewBranch(SD,GCUVs(SD,ref OriginBranch),OriginBranch);
		}
		LayerEffects.Reverse();
		//}
		bool NoEffects = true;
		foreach(ShaderEffect SE in LayerEffects){
			if (SE.Visible&&!SE.IsUVEffect)
			NoEffects = false;
		}
		if (ShaderCodeSamplers.Length>0)
		return "		//Generate Layer: "+Name.Text+"\n			//Sample parts of the layer:\n"+ShaderCodeSamplers+(NoEffects?"":"\n			//Apply Effects:\n")+ReplaceLastOccurrence("				"+ShaderCodeEffects.Replace("\n","\n				"),"\n				","\n");
		return "";
	}
	public static string ReplaceLastOccurrence(string Source, string Find, string Replace){
		int place = Source.LastIndexOf(Find);

		if(place == -1)
		   return string.Empty;

		string result = Source.Remove(place, Find.Length).Insert(place, Replace);
		return result;
	}//return src*a + dest*(1-a);
	//return src + dest*(1-a);
	public bool IsBlocker = false;
	public bool UsesAlpha(){
		if (UseAlpha.On)
			return true;
		foreach(ShaderEffect SE in LayerEffects){
			if (SE.Visible&&SE.UseAlpha.Float>0)//1=RGBA,2=A1
				return true;
		}
		return false;
	}
	public string CalculateMix(ShaderData SD, string CodeName,string PixelColor,bool Premultiplied){
		string Code = "";
			if (MixAmount.Get()!="0"){
				string lerpfactor = "";
				string MulAdd = "";
				if (UsesAlpha()||(Parent!=null&&Parent.EndTag.Text=="a")){
					lerpfactor+=MulAdd+PixelColor+".a";MulAdd=" * ";}
				if (!MixAmount.Safe()){
					lerpfactor+=MulAdd+MixAmount.Get();MulAdd=" * ";}
				if (Stencil.Obj!=null){
					lerpfactor+=MulAdd+"vd."+(((ShaderLayerList)Stencil.Obj).CodeName+Stencil.MaskColorComponentS);MulAdd="*";}
				if (UseFadeout.On){
					lerpfactor+=MulAdd+("(1-saturate((vd.screenPos.z-("+FadeoutMin.Get()+"))/("+FadeoutMax.Get()+"-"+FadeoutMin.Get()+")))");MulAdd=" * ";}
				
				string AT = AlphaBlendMode.Names[AlphaBlendMode.Type];
				
				//Debug.Log(AT);
				//Debug.Log(Parent);
				//Debug.Log(Parent.SLs.IndexOf(this));
				//Debug.Log(MixAmount.Safe());
				//Debug.Log(Parent.BaseColor == new Color(0,0,0,0));
				if (Parent!=null&&Parent.EndTag.Text=="a"){
					///if (UseAlpha.On)
					if (UsesAlpha())
						AT = "Blend";
					else
						AT = "Replace";
				}
				if (AT == "Blend"&&Parent!=null&&Parent.SLs.IndexOf(this)==0&&Parent.BaseColor == new Color(0,0,0,0))//&&MixAmount.Safe()
					AT = "Replace";
				Code += ShaderUtil.GenerateMixingCode(SD, CodeName,PixelColor, MixType.Names[MixType.Type], AT, lerpfactor,false,Premultiplied,Parent.EndTag.Text);
				//Code += ShaderUtil.GenerateMixingCode(SD, CodeName,PixelColor, MixType.Names[MixType.Type], AT, lerpfactor,false,SD.UsesPremultipliedBlending,Parent.EndTag.Text);
			}
			return Code;
	}
	public Dictionary<string,ShaderVar> GetSaveLoadDict(bool CD){
		Dictionary<string,ShaderVar> D = new Dictionary<string,ShaderVar>();

		D.Add(Name.Name,Name);
		D.Add(LayerType.Name,LayerType);
		//D.Add(OldLayerType.Name,OldLayerType);
		/*D.Add(Color.Name,Color);
		D.Add(Color2.Name,Color2);
		D.Add(Image.Name,Image);
		if (ShaderSandwich.Instance.OpenShader.FileVersion<2f)
			D.Add("Cubemap",Cube);
		else
			D.Add(Cube.Name,Cube);
		D.Add(NoiseType.Name,NoiseType);
		if (ShaderSandwich.Instance.OpenShader.FileVersion<2f)
			D.Add("Noise Dimensions",NoiseDim);
		else
			D.Add(NoiseDim.Name,NoiseDim);
		D.Add(NoiseA.Name,NoiseA);
		D.Add(NoiseB.Name,NoiseB);
		D.Add(NoiseC.Name,NoiseC);
		D.Add(LightData.Name,LightData);
		D.Add(SpecialType.Name,SpecialType);
		D.Add(LinearizeDepth.Name,LinearizeDepth);*/
		
		///D.Add(MapType.Name,MapType);
		///D.Add(MapLocal.Name,MapLocal);
		///D.Add(MapSpace.Name,MapSpace);

			D.Add(MapType.Name,MapType);
			D.Add(MapLocal.Name,MapLocal);
			D.Add(MapSpace.Name,MapSpace);
			D.Add(MapGenerateSpace.Name,MapGenerateSpace);
			D.Add(MapInverted.Name,MapInverted);
			D.Add(MapUVIndex.Name,MapUVIndex);
			D.Add(MapDirection.Name,MapDirection);
			D.Add(MapScreenSpace.Name,MapScreenSpace);
			D.Add(UseFadeout.Name,UseFadeout);
			D.Add(FadeoutMinL.Name,FadeoutMinL);
			D.Add(FadeoutMaxL.Name,FadeoutMaxL);
			D.Add(FadeoutMin.Name,FadeoutMin);
			D.Add(FadeoutMax.Name,FadeoutMax);
	
		D.Add(UseAlpha.Name,UseAlpha);
		D.Add(AlphaBlendMode.Name,AlphaBlendMode);
		D.Add(MixAmount.Name,MixAmount);

		/*D.Add(UseFadeout.Name,UseFadeout);
		D.Add(FadeoutMinL.Name,FadeoutMinL);
		D.Add(FadeoutMaxL.Name,FadeoutMaxL);
		D.Add(FadeoutMin.Name,FadeoutMin);
		D.Add(FadeoutMax.Name,FadeoutMax);*/
		
		D.Add(MixType.Name,MixType);
		D.Add(Stencil.Name,Stencil);
		//Stencil.SetToMasks(Parent,0);
		//Stencil.HookIntoObjects();
		//D.Add(VertexMask.Name,VertexMask);
		//D.Add(VertexSpace.Name,VertexSpace);
		if (CD&&CustomData!=null&&CustomData.D!=null){
			foreach(var CV in CustomData){
				D[CV.Key] = CV.Value;
			}
		}
		return D;
	}
	public ShaderLayer Copy(){
		string asd = Save(0);
		//UnityEngine.Debug.Log(asd);
		return Load(new StringReader(asd));
	}
	public string Save(int tabs){
		string S = "\n"+ShaderUtil.Tabs[tabs]+"Begin Shader Layer\n";

		S += ShaderUtil.SaveDict(GetSaveLoadDict(true),tabs+1);
		foreach(ShaderEffect SE in LayerEffects){
			S += SE.Save(tabs+1);
		}
		S += ShaderUtil.Tabs[tabs]+"End Shader Layer\n";
		//UnityEngine.Debug.Log(S);
		return S;
	}
	/*static public ShaderLayer LoadOld(StringReader S){
		ShaderLayer SL = Load(S);
		if (SL.OldLayerType.Type==(int)LayerTypes.Color)
			SL.LayerType.Text = "SLTColor";
		if (SL.OldLayerType.Type==(int)LayerTypes.Gradient)
			SL.LayerType.Text = "SLTGradient";
		if (SL.OldLayerType.Type==(int)LayerTypes.VertexColors)
			SL.LayerType.Text = "SLTVertexColors";
		if (SL.OldLayerType.Type==(int)LayerTypes.Texture)
			SL.LayerType.Text = "SLTTexture";
		if (SL.OldLayerType.Type==(int)LayerTypes.Cubemap)
			SL.LayerType.Text = "SLTCubemap";
		if (SL.OldLayerType.Type==(int)LayerTypes.Previous)
			SL.LayerType.Text = "SLTPrevious";
		if (SL.OldLayerType.Type==(int)LayerTypes.Literal)
			SL.LayerType.Text = "SLTLiteral";
		if (SL.OldLayerType.Type==(int)LayerTypes.GrabDepth){
			if (SL.SpecialType.Type==0)
				SL.LayerType.Text = "SLTGrabPass";
			else
				SL.LayerType.Text = "SLTDepthPass";
		}
		if (SL.OldLayerType.Type==(int)LayerTypes.Noise){
			if (SL.NoiseType.Type==0)
				SL.LayerType.Text = "SLTPerlinNoise";
			else if (SL.NoiseType.Type==1)
				SL.LayerType.Text = "SLTCloudNoise";
			else if (SL.NoiseType.Type==2)
				SL.LayerType.Text = "SLTCubistNoise";
			else if (SL.NoiseType.Type==3)
				SL.LayerType.Text = "SLTCellNoise";
			else if (SL.NoiseType.Type==4)
				SL.LayerType.Text = "SLTDotNoise";
		}
		ShaderUtil.SetupCustomData(SL.CustomData,ShaderSandwich.PluginList["LayerType"][SL.LayerType.Text].GetInputs());
		/*D.Add(Name.Name,Name);
		D.Add(LayerType.Name,LayerType);
		D.Add(OldLayerType.Name,OldLayerType);
		D.Add(Color.Name,Color);
		D.Add(Color2.Name,Color2);
		D.Add(Image.Name,Image);
		D.Add(Cube.Name,Cube);
		D.Add(NoiseType.Name,NoiseType);
		D.Add(NoiseDim.Name,NoiseDim);
		D.Add(NoiseA.Name,NoiseA);
		D.Add(NoiseB.Name,NoiseB);
		D.Add(NoiseC.Name,NoiseC);
		D.Add(LightData.Name,LightData);
		D.Add(SpecialType.Name,SpecialType);
		D.Add(LinearizeDepth.Name,LinearizeDepth);
		
		D.Add(MapType.Name,MapType);
		D.Add(MapLocal.Name,MapLocal);
		
		D.Add(UseAlpha.Name,UseAlpha);
		D.Add(MixAmount.Name,MixAmount);

		D.Add(UseFadeout.Name,UseFadeout);
		D.Add(FadeoutMinL.Name,FadeoutMinL);
		D.Add(FadeoutMaxL.Name,FadeoutMaxL);
		D.Add(FadeoutMin.Name,FadeoutMin);
		D.Add(FadeoutMax.Name,FadeoutMax);
		
		D.Add(MixType.Name,MixType);
		D.Add(Stencil.Name,Stencil);
		Stencil.SetToMasks(Parent,0);
		D.Add(VertexMask.Name,VertexMask);*

		Dictionary<string,ShaderVar> NewData = new Dictionary<string,ShaderVar>();
		NewData["Color"] = SL.Color;
		NewData["Color 2"] = SL.Color2;
		NewData["Texture"] = SL.Image;
		NewData["Cubemap"] = SL.Cube;
		NewData["Noise Dimensions"] = SL.NoiseDim;
		NewData["Jitter"] = SL.NoiseA;
		NewData["Fill"] = SL.NoiseA;
		NewData["MinSize"] = SL.NoiseA;
		NewData["Edge"] = SL.NoiseB;
		NewData["MaxSize"] = SL.NoiseB;
		NewData["Square"] = SL.NoiseC;
		ShaderUtil.SetupOldData(SL.CustomData,NewData);
		
		///if (SL.MapType.Type==(int)LayerMappingType.RimLight)
		///	SL.AddLayerEffect("SSEUVFlip");
		return SL;
	}*/
	static public ShaderLayer Load(StringReader S){
		ShaderLayer SL = ShaderLayer.CreateInstance<ShaderLayer>();
		var D = SL.GetSaveLoadDict(false);
//		Debug.Log(D["Stencil"].ObjectUpdater);
		SL.CustomData = new SVDictionary();
		while(1==1){
			string Line = S.ReadLine();
//Debug.Log("AHHHAHAHAHAHHAAHAHHAHAHAA");
			if (Line!=null){
				Line = ShaderUtil.Sanitize(Line);
				if(Line=="End Shader Layer")break;
				
				if (Line.Contains(":")){
					//if (Line.Contains("Layer Type"))
					//	SL.SetType(((ShaderLayerType)SL.LayerType.Obj));
						//ShaderUtil.SetupCustomData(SL.CustomData,((ShaderLayerType)SL.LayerType.Obj).GetInputs());
					ShaderUtil.LoadLine(D,Line,SL.CustomData);
					
					if (Line.Contains("Layer Type"))
						SL.SetType(((ShaderLayerType)SL.LayerType.Obj));
				}
				else
				if (Line=="Begin Shader Effect")
					SL.LayerEffects.Add(ShaderEffect.Load(S));
			}
			else
			break;
		}
		//foreach(KeyValuePair<string,ShaderVar> asd in SL.CustomData.D){
		//	Debug.Log(asd.Key+", "+asd.Value.Name+", "+(asd.Value.Input==null?"Null":asd.Value.Input.ToString()));
		//}
		SL.SetType(((ShaderLayerType)SL.LayerType.Obj));
		ShaderUtil.SetupCustomData(SL.CustomData,((ShaderLayerType)SL.LayerType.Obj).GetInputs());
		//SL.GetTexture();
		return SL;
	}
	public Dictionary<string,ShaderVar> LegacyDictionary = null;
	static public ShaderLayer LoadLegacy(StringReader S){
		ShaderLayer SL = ShaderLayer.CreateInstance<ShaderLayer>();//UpdateGradient
		var D = SL.GetSaveLoadDictLegacy(false);
		SL.LegacyDictionary = D;
		SL.CustomData = new SVDictionary();
		while(1==1){
			string Line = ShaderUtil.Sanitize(S.ReadLine());

			if (Line!=null){
				if(Line=="EndShaderLayer")break;
				
				if (Line.Contains("#!"))
					ShaderUtil.LoadLineLegacy(D,Line,SL.CustomData);
				else
				if (Line=="BeginShaderEffect")
					SL.LayerEffects.Add(ShaderEffect.LoadLegacy(S));
			}
			else
			break;
		}
	//	ShaderUtil.SetupCustomData(SL.CustomData,ShaderSandwich.PluginList["LayerType"][D["Real Layer Type"].Text].GetInputs());
		SL.SetType(D["Real Layer Type"].Text);
		
		if (D["UV Map"].Type==0){
			SL.MapType.Type = 0;
			SL.MapUVIndex.Float = 1f;
		}
		if (D["UV Map"].Type==1){
			SL.MapType.Type = 0;
			SL.MapUVIndex.Float = 2f;
		}
		if (D["UV Map"].Type==2){
			SL.MapType.Type = 3;
			if (D["Map Local"].On)
				SL.MapSpace.Type = 1;
			else
				SL.MapSpace.Type = 0;
		}
		if (D["UV Map"].Type==3){
			SL.MapType.Type = 4;
			if (D["Map Local"].On)
				SL.MapSpace.Type = 1;
			else
				SL.MapSpace.Type = 0;
		}
		if (D["UV Map"].Type==4){
			SL.MapType.Type = 1;
			SL.MapGenerateSpace.Type = 2;
			SL.MapInverted.On = false;
		}
		if (D["UV Map"].Type==5){
			SL.MapType.Type = 1;
			if (D["Map Local"].On)
				SL.MapGenerateSpace.Type = 1;
			else
				SL.MapGenerateSpace.Type = 0;
		}
		if (D["UV Map"].Type==6){
			SL.MapType.Type = 5;
		}
		if (D["UV Map"].Type==7){
			SL.MapType.Type = 2;
			if (D["Map Local"].On)
				SL.MapSpace.Type = 1;
			else
				SL.MapSpace.Type = 0;
		}
		List<ShaderEffect> NewOrder = new List<ShaderEffect>();// Time to reverse mapping effects
		foreach(ShaderEffect SE in SL.LayerEffects){
			if (SE.IsUVEffect){
				NewOrder.Insert(0,SE);
			}else{
				NewOrder.Add(SE);
			}
		}
		SL.LayerEffects = NewOrder;
		//SL.GetTexture();
		return SL;
	}
	public bool IsGrayscale(){//I know this is ridiculous but anyway
		ShaderLayerType SLT = LayerType.Obj as ShaderLayerType;
		bool SoFarSoGood = true;
		if (SLT!=null){
			if (SLT.TypeS=="SLTColor"){
				if (!(CustomData["Color"].Input==null&&CustomData["Color"].Vector.r==CustomData["Color"].Vector.g&&CustomData["Color"].Vector.r==CustomData["Color"].Vector.b))
					SoFarSoGood = false;
			}
			if (SLT.TypeS=="SLTVector"){
				if (!(CustomData["Vector"].Input==null&&CustomData["Vector"].Vector.r==CustomData["Vector"].Vector.g&&CustomData["Vector"].Vector.r==CustomData["Vector"].Vector.b))
					SoFarSoGood = false;
			}
			if (!(SLT.TypeS=="SLTCellNoise"||SLT.TypeS=="SLTCloudNoise"||SLT.TypeS=="SLTCubistNoise"||SLT.TypeS=="SLTCurvature"||SLT.TypeS=="SLTDepthPass"||SLT.TypeS=="SLTDotNoise"||SLT.TypeS=="SLTFluidNoise"||SLT.TypeS=="SLTGradient"||SLT.TypeS=="SLTMask"||SLT.TypeS=="SLTNumber"||SLT.TypeS=="SLTPerlinNoise"||SLT.TypeS=="SLTRandomNoise"||SLT.TypeS=="SLTSquareNoise"||SLT.TypeS=="SLTVector"||SLT.TypeS=="SLTColor")){
					SoFarSoGood = false;
			}
		}
		foreach(ShaderEffect SE in LayerEffects){
			if (!SE.Visible)
				continue;
			if (SE.TypeS=="SSESwizzle")
				SoFarSoGood = false;
			if (SE.TypeS=="SSETint")
				SoFarSoGood = false;
			if (SE.TypeS=="SSEHueShift")
				SoFarSoGood = false;
			if (SE.TypeS=="SSEUnpackNormal")
				SoFarSoGood = false;
		}
		//Debug.Log(Name.Text);
		//Debug.Log(SoFarSoGood);
		return SoFarSoGood;
	}
public Dictionary<string,ShaderVar> GetSaveLoadDictLegacy(bool CD){
	
ShaderVar RealLayerType = new ShaderVar("Real Layer Type","SLTColor");
ShaderVar LayerType = new ShaderVar("Layer Type","SLTColor");
ShaderVar Color = new ShaderVar("Main Color", new Vector4(160f/255f,204f/255f,225/255f,1f));
ShaderVar Color2 = new ShaderVar("Second Color", new Vector4(0.0f,0.0f,0.0f,1f));
ShaderVar Image = new ShaderVar("Main Texture", "Texture2D");
ShaderVar Cube = new ShaderVar( "NCubemap","Cubemap");

ShaderVar UseFadeout = new ShaderVar("Use Fadeout",false);
ShaderVar FadeoutMinL = new ShaderVar("Fadeout Limit Min",0f);
ShaderVar FadeoutMaxL = new ShaderVar("Fadeout Limit Max",10f);
ShaderVar FadeoutMin = new ShaderVar("Fadeout Start",3f);
ShaderVar FadeoutMax = new ShaderVar("Fadeout End",5f);

ShaderVar MapType = new ShaderVar("UV Map",new string[]{"UV Map","UV Map2","Reflection","Direction","Rim Light","Generate","View","Position"},new string[]{"The first UV Map","The second UV Map (Lightmapping)","The reflection based on the angle of the face.","Simply the angle of the face.","Maps from the edge to the center of the model.","Created the UVMap using tri-planar mapping.(Good for generated meses)","Plaster the layer onto it from whatever view you are facing from.","Maps using the world position."});
ShaderVar MapLocal = new ShaderVar("Map Local",false);

ShaderVar NoiseDim = new ShaderVar("NNoise Dimensions",new string[]{"2D","3D"},new string[]{"",""});
ShaderVar NoiseType = new ShaderVar("Noise Type",new string[]{"Perlin","Cloud","Cubist","Cell","Dot"},new string[]{"","","","",""});
ShaderVar NoiseA = new ShaderVar("Noise A",0f);
ShaderVar NoiseB = new ShaderVar("Noise B",1f);
ShaderVar NoiseC = new ShaderVar("Noise C",false);

ShaderVar LightData = new ShaderVar("Light Data",new string[]{"Light/Direction","Light/Attenuation","View Direction","Channels/Albedo(Diffuse)","Channels/Normal","Channels/Specular","Channels/Emission","Channels/Alpha","Light/Color"},new string[]{"","","","","","","","",""});
ShaderVar SpecialType = new ShaderVar("Special Type",new string[]{"Grab","Depth"},new string[]{"",""});
ShaderVar LinearizeDepth = new ShaderVar("Linearize Depth",false);
ShaderVar VertexMask = new ShaderVar("Vertex Mask",2f);
ShaderVar VertexSpace = new ShaderVar("Vertex Space",0f);
	
	Dictionary<string,ShaderVar> D = new Dictionary<string,ShaderVar>();

	D.Add(Name.Name,Name);
	D.Add(RealLayerType.Name,RealLayerType);
	D.Add(LayerType.Name,LayerType);
	D.Add(Color.Name,Color);
	D.Add(Color2.Name,Color2);
	D.Add(Image.Name,Image);
	if (ShaderSandwich.Instance.OpenShader.FileVersion<2f)
		D.Add("Cubemap",Cube);
	else
		D.Add(Cube.Name,Cube);
	D.Add(NoiseType.Name,NoiseType);
	if (ShaderSandwich.Instance.OpenShader.FileVersion<2f)
		D.Add("Noise Dimensions",NoiseDim);
	else
		D.Add(NoiseDim.Name,NoiseDim);
	D.Add(NoiseA.Name,NoiseA);
	D.Add(NoiseB.Name,NoiseB);
	D.Add(NoiseC.Name,NoiseC);
	D.Add(LightData.Name,LightData);
	D.Add(SpecialType.Name,SpecialType);
	D.Add(LinearizeDepth.Name,LinearizeDepth);
	
	D.Add(MapType.Name,MapType);
	D.Add(MapLocal.Name,MapLocal);
	
	D.Add(UseAlpha.Name,UseAlpha);
	D.Add(MixAmount.Name,MixAmount);

	D.Add(UseFadeout.Name,UseFadeout);
	D.Add(FadeoutMinL.Name,FadeoutMinL);
	D.Add(FadeoutMaxL.Name,FadeoutMaxL);
	D.Add(FadeoutMin.Name,FadeoutMin);
	D.Add(FadeoutMax.Name,FadeoutMax);
	
	D.Add(MixType.Name,MixType);
	D.Add(Stencil.Name,Stencil);
	D.Add(VertexMask.Name,VertexMask);
	D.Add(VertexSpace.Name,VertexSpace);
	if (CD&&CustomData!=null&&CustomData.D!=null){
		foreach(var CV in CustomData){
			D[CV.Key] = CV.Value;
		}
	}
	return D;
}
	public void UpdateIcon(RenderTexture RTUV, RenderTexture RTPrevious, RenderTexture RTPing){
		bool ontoIcon = false;
		//foreach(ShaderEffect SE in LayerEffects){
		for(int i = LayerEffects.Count-1;i>=0;i--){
			ShaderEffect SE = LayerEffects[i];
			if (SE.Visible&&SE.uvshader!=""){
				ontoIcon = !ontoIcon;
				SE.UVGPUPreview(SE.Inputs,this,ontoIcon?RTUV:RTIcon,ontoIcon?RTIcon:RTUV);
			}
		}
		if (ontoIcon)
			Graphics.Blit(RTIcon,RTUV);
		
		//((ShaderLayerType)ShaderSandwich.PluginList["LayerType"][LayerType.Text]).GPUPreview(CustomData.D,this,RTPrevious,RTPing);
		if (LayerType.Obj!=null)
		((ShaderLayerType)LayerType.Obj).GPUPreview(CustomData,this,RTPrevious,RTPing);
		
		ontoIcon = false;
		foreach(ShaderEffect SE in LayerEffects){
			if (SE.Visible&&SE.shader!=""){
				ontoIcon = !ontoIcon;
				SE.GPUPreview(SE.Inputs,this,ontoIcon?RTPing:RTIcon,ontoIcon?RTIcon:RTPing);
			}
		}
		
		if (!ontoIcon)
			Graphics.Blit(RTPing,RTIcon);
	}
	public ShaderFix MixAmountAlmostOne = null;
	public ShaderFix MixAmountAlmostZero = null;
	public virtual void GetMyFixes(ShaderBase shader, ShaderPass pass, ShaderIngredient	ingredient, List<ShaderFix> fixes){
		//Debug.Log(Name.Text);
		if (MixAmountAlmostOne==null)
			MixAmountAlmostOne = new ShaderFix(this,"Set the Mix Amount to 1 (from the super close value of x).",ShaderFix.FixType.Optimization,
			()=>{
				MixAmount.Float = 1f;
			});
		if (MixAmountAlmostZero==null)
			MixAmountAlmostZero = new ShaderFix(this,"Set the Mix Amount to 0 (from the super close value of x).",ShaderFix.FixType.Optimization,
			()=>{
				MixAmount.Float = 0f;
			});
	
	
		if (MixAmount.Input==null&&MixAmount.Float!=1f&&MixAmount.Float>0.95f){
			MixAmountAlmostOne.Message = "Set the Mix Amount to 1 (from the super close value of "+MixAmount.Float.ToString()+").";
			fixes.Add(MixAmountAlmostOne);
		}
		if (MixAmount.Input==null&&MixAmount.Float!=0f&&MixAmount.Float<0.05f){
			MixAmountAlmostZero.Message = "Set the Mix Amount to 0 (from the super close value of "+MixAmount.Float.ToString()+").";
			fixes.Add(MixAmountAlmostZero);
		}
	}
}
}