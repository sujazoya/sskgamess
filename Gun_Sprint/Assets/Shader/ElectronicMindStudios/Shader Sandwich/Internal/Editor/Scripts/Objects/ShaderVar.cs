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
[System.Serializable]
public enum Types {
	Vec,
	Float,
	Type,
	Toggle,
	Texture,
	Cubemap,
	ObjectArray,
	Text,
	//PluginSelection
};
public enum DrawTypes {Color,Slider01,Int,Float,Type,Toggle,Texture,Cubemap,ObjectArray,Text};

[System.Serializable]
public class ShaderVar{
	public int UID = -1;
	public delegate void ChangeDelegate();
	[XmlIgnore]public ChangeDelegate OnChange;
	
	[DataMember]
	public ShaderColor Vector = new ShaderColor(0f);//{ get; set; }
	public bool VecIsColor = true;
	//public string PluginSelectionType;//{ get; set; }
	public string Plugin;
	//public string Text{
	//	get{return _Text;}
	//	set{if (Name=="EndTag"){Debug.Log(_Text);Debug.Log(value);}_Text = value;}
	//}
	public string Text = "";
	[XmlIgnore,NonSerialized] public Texture2D Image;
	public string ImageGUID = "";
	public Color32[] ImagePixels;
	[XmlIgnore,NonSerialized] public Cubemap Cube;
	public string CubeGUID = "";
	public float Float;
	public ShaderInput Input;
	/*public ShaderInput Input{
		get{return _Input;}
		set{Debug.Log(Name+"oh oh: "+(value==null?"Null":value.ToString()));_Input = value;}
	}*/
	public int ColorComponent = 0;
	public bool RGBAMasks = false;
	public bool LightingMasks = false;
	public int MaskColorComponent = 0;
	public string MaskColorComponentS{
	get{
		if (((ShaderLayerList)Obj).EndTag.Text.Length>1){
			return (new string[]{".r",".g",".b",".a"})[MaskColorComponent];
		}
		return "";
	}
	}
	public bool UseInput = false;
	public bool Hidden = false;
	public bool Use = true;
	public bool Editing = false;
	public bool NoSlider = false;
	public bool NoArrows = false;
	public bool NoBox = false;
	public bool ShowNumbers = false;
	public bool HideAlpha = false;
	public bool ForceGUIChange = false;
	public float EditingPopup = 0f;
	public double EditingPopupStartTime = 0f;
	public string ObjectUpdater;
	public ShaderObjectField ObjField;
	public List<object> ObjFieldObject;
	public List<Texture> ObjFieldImage;
	public List<bool> ObjFieldEnabled;
	public List<string> ObjFieldStorageNames;
	Texture CustomIcon = null;
	[NonSerialized]public object RealObj = null;
	public object Obj{
		set {
			RealObj = value;
			if (ObjFieldObject==null)
				HookIntoObjects();
				//ObjFieldObject= null;
				//HookIntoObjects();
			if (ObjFieldObject!=null&&ObjFieldObject.Contains(RealObj))
				RealSelected = ObjFieldObject.IndexOf(RealObj);
		}
		get {//Debug.Log(RealSelected);
			//if (RealObj!=null){
			//	if (ObjFieldObject.IndexOf(RealObj)!=-1)
			//		return RealObj;
			//}
			//RealObj = null;
			if (ObjFieldObject==null)
				HookIntoObjects();
				//ObjFieldObject= null;
				//Debug.Log(ObjectUpdater);
				//HookIntoObjects();
			if (ObjFieldObject!=null&&ObjFieldObject.Count>RealSelected&&RealSelected>=0){
				RealObj = ObjFieldObject[RealSelected];
				//Debug.Log("It did work at one point ;("+Name);
				return RealObj;
			}
			return null;
		}
	}
	[NonSerialized]public bool ObjIsFalse = true;
	
	[NonSerialized]public object OldObj = null;
	public int RealSelected = -2;
	public int Selected{
		get {
			//Debug.Log(RealSelected);
			//Debug.Log(ObjFieldObject);
			//Debug.Log(ObjFieldObject.Count);
			//return RealSelected;
			if (ObjFieldObject!=null&&ObjFieldObject.Count>RealSelected){
				if (RealSelected<0)
					RealObj = null;
				else
					RealObj = ObjFieldObject[RealSelected];
				return RealSelected;/*RealSelected = ObjFieldObject.IndexOf(Obj);return ObjFieldObject.IndexOf(Obj);*/
			}
			//Debug.Log(RealSelected);
			return -1;
		}
		set {
			//Debug.Log(Name);
			//Debug.Log(GetID());
			//Debug.Log(RealSelected);
			//Debug.Log(value);
			RealSelected = value;
			
			if (ObjFieldObject!=null&&ObjFieldObject.Count>value){
				if (value>=0)
					RealObj = ObjFieldObject[value];
				else
					RealObj = null;
			}
			RealSelected = value;
		}
	}
	public bool ObjFieldOn = false;
	public int Type;
	[NonSerialized]public Rect LastUsedRect;
	[XmlArrayAttribute]public string[] Names;
	[XmlArrayAttribute]public string[] HiddenNames;
	[XmlArrayAttribute]public string[] CodeNames;
	[XmlArrayAttribute]public string[] Descriptions;
	
	
	[XmlArrayAttribute]public string[] ImagePaths;
	[XmlArrayAttribute,XmlIgnore,NonSerialized]public Texture2D[] Images;
	public bool On;
	
	public int TypeDispL = 4;
	public int TypeDispLasdasdas = 4;
	
	public float Range0 = 0;
	public float Range1 = 1;
	
	//public Types CType{
	//	get{return _CType;}
	//	set{if (Name=="Layer Type"){Debug.Log(_CType);Debug.Log(value);}_CType = value;}
	//}
	public Types CType;
	
	public bool NoInputs = false;
	
	public string Name = "";
	
	
	public string WarningTitle = "";
	public string WarningMessage = "";
	public string WarningOption1 = "";
	public string WarningOption2 = "";
	public string WarningOption3 = "";	
	public delegate void WarningDelegateReal(int Option, ShaderVar SV);
	[XmlIgnore]public WarningDelegateReal WarningDelegate;	
	
	public void WarningSetup(string Title, string Message, string Option1,string Option2,string Option3,WarningDelegateReal Delegate){
		WarningTitle = Title;
		WarningMessage = Message;
		WarningOption1 = Option1;
		WarningOption2 = Option2;
		WarningOption3 = Option3;
		WarningDelegate = Delegate;
	}
	public void WarningSetup(string Title, string Message, string Option1,string Option2,WarningDelegateReal Delegate){
		WarningTitle = Title;
		WarningMessage = Message;
		WarningOption1 = Option1;
		WarningOption2 = Option2;
		WarningDelegate = Delegate;
	}
	
	
	[XmlIgnore,NonSerialized]public ShaderLayer Parent_Real;
	[XmlIgnore,NonSerialized]public bool CanConfirmIDontNeedAnyParentalGuidanceAnymore = false;
	public ShaderLayer Parent{
		get{
			if (CanConfirmIDontNeedAnyParentalGuidanceAnymore)
				return null;
			if (Parent_Real==null){
				foreach(ShaderLayer SL in ShaderUtil.GetAllLayers())
				SL.UpdateShaderVars(true);
			}
			if (Parent_Real==null)
				CanConfirmIDontNeedAnyParentalGuidanceAnymore = true;
			//Debug.Log(Parent_Real);
			return Parent_Real;
		}
		set{
			Parent_Real = value;
		}
	
	}
	
	public bool Safe(){
		if (ShaderBase.Current.SD!=null&&ShaderBase.Current.SD.Temp&&ShaderSandwich.Instance!=null&&ShaderSandwich.Instance.ImprovedUpdates&&((CType==Types.Float&&!NoInputs)||(CType==Types.Vec)))
			return false;
		if (Input!=null){
			string asd = Input.Get();
			if (asd!="0"&&asd!="1")
				return false;
		
			return true;
		}
		//string g = Float.ToString();
		//if (Get()!="0"&&Get()!="1")
		if (Float!=0f&&Float!=1f)
			return false;
		
		return true;
	}
	public bool CanBeBaked(){
		if (ShaderBase.Current.SD!=null&&ShaderBase.Current.SD.Temp&&ShaderSandwich.Instance!=null&&ShaderSandwich.Instance.ImprovedUpdates)
			return false;
		if (Input!=null){
			return false;
		}
		if (CType==Types.Texture)
			return false;
		if (CType==Types.Cubemap)
			return false;
		if (CType==Types.ObjectArray)
			return false;
		if (CType==Types.Text)
			return false;
		if (CType==Types.Type)
			return false;
		if (CType==Types.Toggle)
			return false;
		
		return true;
	}
	public bool SafeOnExport(){
		if (Input!=null){
			string asd = Input.Get();
			if (asd!="0"&&asd!="1")
				return false;
			return true;
		}
		if (Float!=0f&&Float!=1f)
			return false;
		
		return true;
	}
	
	public string Get(){
		return Get(0);
	}
	public string Get(float Wrap){
		ShaderData SD = ShaderBase.Current.SD;
		if (Input!=null){
			/*if (Input.SpecialType!=InputSpecialTypes.None&&!Input.InEditor){
				UpdateToInput(false);
				string[] SInputTypes = new string[]{"None",Input.CustomSpecial.Text.Replace("_Time","_SSTime"),Input.Mask.GetMaskName(),"_SSTime.y","_SSTime.z","_SSTime.y/2","_SSTime.y/6","_SSSinTime.w","sin(_SSTime.z)","_SSSinTime.z","_SSCosTime.w","cos(_SSTime.z)","_SSCosTime.z","ShellDepth","d.Depth","((_SSSinTime.w+1)/2)","((sin(_SSTime.z)+1)/2)","((_SSSinTime.z+1)/2)","((_SSCosTime.w+1)/2)","((cos(_SSTime.z)+1)/2)","((_SSCosTime.z+1)/2))"};
				if (SD!=null&&!SD.Temp)
				SInputTypes = new string[]{"None",Input.CustomSpecial.Text,Input.Mask.GetMaskName(),"_Time.y","_Time.z","_Time.y/2","_Time.y/6","_SinTime.w","sin(_Time.z)","_SinTime.z","_CosTime.w","cos(_Time.z)","_CosTime.z","ShellDepth","d.Depth","((_SinTime.w+1)/2)","((sin(_Time.z)+1)/2)","((_SinTime.z+1)/2)","((_CosTime.w+1)/2)","((cos(_Time.z)+1)/2)","((_CosTime.z+1)/2)"};
				
				InputSpecialTypes[] EInputTypes2 = new InputSpecialTypes[]{InputSpecialTypes.None,InputSpecialTypes.Custom,InputSpecialTypes.Mask,InputSpecialTypes.Time,InputSpecialTypes.TimeFast,InputSpecialTypes.TimeSlow,InputSpecialTypes.TimeVerySlow,InputSpecialTypes.SinTime,InputSpecialTypes.SinTimeFast,InputSpecialTypes.SinTimeSlow,InputSpecialTypes.CosTime,InputSpecialTypes.CosTimeFast,InputSpecialTypes.CosTimeSlow,InputSpecialTypes.ShellDepth,InputSpecialTypes.ParallaxDepth,InputSpecialTypes.ClampedSinTime,InputSpecialTypes.ClampedSinTimeFast,InputSpecialTypes.ClampedSinTimeSlow,InputSpecialTypes.ClampedCosTime,InputSpecialTypes.ClampedCosTimeFast,InputSpecialTypes.ClampedCosTimeSlow};
				int Index = Array.IndexOf(EInputTypes2,Input.SpecialType);
				if (Index!=0)
				return SInputTypes[Index];
			}
			else if (Input.InEditor||SD.Temp){*/
				if (!(CType==Types.Float&&(Input.Type==ShaderInputTypes.Color||Input.Type==ShaderInputTypes.Vector)))
					return Input.Get();
				else
					return Input.Get()+"."+(new string[]{"r","g","b","a"}[ColorComponent]);
			//}
		}
		if (SD!=null&&SD.Temp&&ShaderSandwich.Instance!=null&&ShaderSandwich.Instance.ImprovedUpdates&&((CType==Types.Float&&!NoInputs)||(CType==Types.Vec))){
			if (CType==Types.Vec&&VecIsColor)
				return "GammaToLinear(SSTEMPSV"+GetID().ToString()+")";
			else
				return "SSTEMPSV"+GetID().ToString();
		}
		if ((CType==Types.Texture)||(CType==Types.Cubemap)){
				return "SSTEMPSV"+GetID().ToString();
		}
		if (CType==Types.Vec){
			if (VecIsColor)
				return "GammaToLinear(float4("+Vector.r.ToString()+", "+Vector.g.ToString()+", "+Vector.b.ToString()+", "+Vector.a.ToString()+"))";
			else
				return "float4("+Vector.r.ToString()+", "+Vector.g.ToString()+", "+Vector.b.ToString()+", "+Vector.a.ToString()+")";
		}
		
		//string bs = Float>=0?"":"(";
		//string be = Float>=0?"":")";
		
		if (CType==Types.Float&&Wrap==0){
			if (Float<0f)
				return "("+Float.ToString()+")";
			else
				return Float.ToString();
		}
		else if (CType==Types.Float){
			if (Float%Wrap<0f)
				return "("+(Float%Wrap).ToString()+")";
			else
				return (Float%Wrap).ToString();
		}
		
		//if (CType==Types.ObjectsArray)
		//return GetMaskName();
		
		return "SSError";
	}
	public int GetID(){
		if (UID==-1)
		UID = ShaderSandwich.Instance.ShaderVarIds;ShaderSandwich.Instance.ShaderVarIds+=1;
		
		return UID;
	}
	public ShaderVar(){}
	public void Update(string[] N,string[] D){
		Names = N;
		HiddenNames = N;
		Descriptions = D;
		TypeDispL = Names.Length;
	}
	public void Update(string[] N){
		Names = N;
		HiddenNames = N;
		Descriptions = new string[N.Length];
		for(int i = 0; i<N.Length;i++)
			Descriptions[i] = "";
		TypeDispL = Names.Length;
	}
	public void Update(string[] N,string[] D,string[] CN){
		Names = N;
		HiddenNames = N;
		CodeNames = CN;
		Descriptions = D;
		TypeDispL = Names.Length;
	}
	public void Update(string[] N,string[] D,int S){
		Names = N;
		HiddenNames = N;
		Descriptions = D;
		TypeDispL = S;
	}
	///////////////////
	public ShaderVar(string Nam,Vector4 Vec){
		Vector = new ShaderColor(Vec);
		CType = Types.Vec;
		Name = Nam;
		
	}
	public ShaderVar(string Nam,ShaderColor Vec){
		Vector = Vec;
		CType = Types.Vec;
		Name = Nam;
		
	}
	public ShaderVar(string Nam,string s, string s2){
		if (s=="Texture2D")
			CType = Types.Texture;
		else
		if (s=="Cubemap")
			CType = Types.Cubemap;
		else
		if (s=="ListOfObjects"){
			CType = Types.ObjectArray;
			ObjectUpdater = s2;
		}
		else{
			CType = Types.Text;
			Text = s;
		}
		
		Name = Nam;
		
		if (s=="ListOfObjects")
			HookIntoObjects();
	}
	public ShaderVar(string Nam,string s){
		if (s=="Texture2D")
			CType = Types.Texture;
		else
		if (s=="Cubemap")
			CType = Types.Cubemap;
		else
		if (s=="ListOfObjects"){
			CType = Types.ObjectArray;
			ObjectUpdater = "Mask";
		}
		else{
			CType = Types.Text;
			Text = s;
		}
		
		Name = Nam;
		if (s=="ListOfObjects")
			HookIntoObjects();
	}
	public ShaderVar(string Nam,bool b){
		On = b;
		CType = Types.Toggle;
		Name = Nam;
		
	}
	public ShaderVar(string Nam,float flo,float R0,float R1){
		Float = flo;
		CType = Types.Float;
		Name = Nam;
		Range0 = R0;
		Range1 = R1;
		
	}
	public ShaderVar(string Nam,float flo){
		Float = flo;
		CType = Types.Float;
		Name = Nam;
		
	}
	public ShaderVar(string Nam,string[] N,string[] D){
		Names = N;
		HiddenNames = new string[N.Length];
		Array.Copy(N,HiddenNames,N.Length);
		Descriptions = D;
		CType = Types.Type;
		TypeDispL = Names.Length;
		Name = Nam;
	}	
	public ShaderVar(string Nam,string[] N){
		Names = N;
		HiddenNames = new string[N.Length];
		Array.Copy(N,HiddenNames,N.Length);
		CType = Types.Type;
		TypeDispL = Names.Length;
		Name = Nam;
		
		Descriptions = new string[N.Length];
		for(int i = 0; i<N.Length;i++)
			Descriptions[i] = "";
	}	
	public ShaderVar(string Nam,string[] N,string[] IP,string ASD){
		Names = N;
		HiddenNames = new string[N.Length];
		Array.Copy(N,HiddenNames,N.Length);
		ImagePaths = IP;
		CType = Types.Type;
		TypeDispL = Names.Length;
		Name = Nam;
		
	}		
	public ShaderVar(string Nam,string[] N,string[] IP,string ASD,string[] D){
		Names = N;
		HiddenNames = new string[N.Length];
		Array.Copy(N,HiddenNames,N.Length);
		ImagePaths = IP;
		CType = Types.Type;
		TypeDispL = Names.Length;
		Descriptions = D;
		Name = Nam;
		
	}	
	public ShaderVar(string Nam,string[] N,string[] D,string[] CN){
		Names = N;
		HiddenNames = new string[N.Length];
		Array.Copy(N,HiddenNames,N.Length);
		CodeNames = CN;
		Descriptions = D;
		CType = Types.Type;
		TypeDispL = Names.Length;
		Name = Nam;
		
	}	
	public ShaderVar(string Nam,string[] N,string[] D,int S){
		Names = N;
		HiddenNames = new string[N.Length];
		Array.Copy(N,HiddenNames,N.Length);
		Descriptions = D;
		CType = Types.Type;
		TypeDispL = S;
		Name = Nam;
		
	}			
	
	[NonSerialized]public bool AttemptedHook = false;
	public void HookIntoObjects(){
		//Debug.Log("Let's hook?");
		if (CType == Types.ObjectArray&&ObjectUpdater.Length>0){
			//Debug.Log("Let's hook!");
			if (ShaderSandwich.ShaderVarObjectFieldUpdates!=null){
				//Debug.Log("Almost there..."+ObjectUpdater);
				if (ShaderSandwich.ShaderVarObjectFieldUpdates.ContainsKey(ObjectUpdater)){
					//Debug.Log("Hell yeah! :D");
					ShaderSandwich.Instance.CreateHook(this);
					//ShaderSandwich.ShaderVarObjectFieldUpdates[ObjectUpdater] -= UpdateObjects;
					//ShaderSandwich.ShaderVarObjectFieldUpdates[ObjectUpdater] += UpdateObjects;
					//ShaderSandwich.Instance.JustHooked();
					//Debug.Log("froceasda");
					//ShaderSandwich.Instance.ForceUpdateHooks();
				}
			}
			//Debug.Log(Name+ObjectUpdater);
		}
		
		AttemptedHook = true;
	}
	
	public void UpdateObjects(List<object> objects,List<string> storageNames, List<Texture> icons, List<bool> enabled){
		if (ObjFieldEnabled==null)ObjFieldEnabled=new List<bool>();
		
		ObjFieldObject = objects;
		ObjFieldStorageNames = storageNames;
		ObjFieldImage = icons;
		ObjFieldEnabled.Clear();
		
		//Debug.Log(Name);
		//Debug.Log(objects.Count);
		//Debug.Log(RealObj);
		if (!ObjIsFalse&&RealObj!=null)
			Selected = objects.IndexOf(RealObj);
		//Debug.Log(Selected);	
		ObjIsFalse = false;
		bool Enab = true;
		foreach(object obj in objects){
			bool addenab = true;
			ShaderLayerList SLL2 = obj as ShaderLayerList;
			if (SLL2!=null){
				//Debug.Log("hmmmm");
				//Debug.Log(Name);
				if (!CanConfirmIDontNeedAnyParentalGuidanceAnymore&&Parent!=null&&SLL2==Parent.Parent)
					Enab = false;
			
				//addenab = Enab&&((SLL2.EndTag.Text.Length==4&&RGBAMasks)||!RGBAMasks);//!(!Enab||((SLL2.EndTag.Text.Length!=4&&RGBAMasks))||(!LightingMasks&&SLL2.IsLighting.On)||(LightingMasks&&!SLL2.IsLighting.On));//wtf?
				addenab = Enab;//!(!Enab||((SLL2.EndTag.Text.Length!=4&&RGBAMasks))||(!LightingMasks&&SLL2.IsLighting.On)||(LightingMasks&&!SLL2.IsLighting.On));//wtf?
			}
			ObjFieldEnabled.Add(addenab);
		}
		if (Selected>-1){
			if (ObjFieldEnabled[Selected]==false)
				Selected = -1;
		}
	}
	
	public bool Draw(Rect rect){
		if (CType==Types.Vec)
		return Draw_Real(rect,DrawTypes.Color,"");
		if (CType==Types.Float)
		return Draw_Real(rect,DrawTypes.Slider01,"");
		if (CType==Types.Type)
		return Draw_Real(rect,DrawTypes.Type,"");
		if (CType==Types.Toggle)
		return Draw_Real(rect,DrawTypes.Toggle,"");
		if (CType==Types.Texture)
		return Draw_Real(rect,DrawTypes.Texture,"");
		if (CType==Types.Cubemap)
		return Draw_Real(rect,DrawTypes.Cubemap,"");
		if (CType==Types.ObjectArray)
		return Draw_Real(rect,DrawTypes.ObjectArray,"");
		return false;
	}	
	public bool Draw(Rect rect,string S){
				//		Debug.Log(Name);
				//Debug.Log(CType);
		if (CType==Types.Vec)
		return Draw_Real(rect,DrawTypes.Color,S);
		if (CType==Types.Float)
		return Draw_Real(rect,DrawTypes.Slider01,S);
		if (CType==Types.Type)
		return Draw_Real(rect,DrawTypes.Type,S);
		if (CType==Types.Toggle)
		return Draw_Real(rect,DrawTypes.Toggle,S);
		if (CType==Types.Texture)
		return Draw_Real(rect,DrawTypes.Texture,S);
		if (CType==Types.Cubemap)
		return Draw_Real(rect,DrawTypes.Cubemap,S);
		if (CType==Types.ObjectArray)
		return Draw_Real(rect,DrawTypes.ObjectArray,S);
		
		return false;
	}
	public bool Draw(Rect rect,string S,DrawTypes d){
		return Draw_Real(rect,d,S);
	}
	public bool DrawPicType(Rect rect,Texture2D Tex,string Na){
	return DrawPicType_Real(rect,Tex,Na,true);
	}
	public bool DrawPicType(Rect rect,Texture2D Tex,string Na,bool Alp){
	return DrawPicType_Real(rect,Tex,Na,Alp);
	}
	public bool DrawPicType_Real(Rect rect,Texture2D Tex,string Na,bool Alp){
	if (NoInputs==true)
	UseInput = false;
		Rect InitialRect = rect;
		GUI.Box(rect,"",GUI.skin.button);
	//	rect.x+=1;
	//	rect.y+=1;
	//	rect.width-=2;
	//	rect.height-=2;
		GUI.DrawTexture( rect ,Tex);
		if (Alp)
		{
		GUI.DrawTexture( rect ,Tex);
		GUI.DrawTexture( rect ,Tex);
		GUI.DrawTexture( rect ,Tex);
		}
		rect = InitialRect;
		rect.y+=rect.height-18;
		rect.height = 18;
		GUI.Box(rect,"",GUI.skin.button);
		GUI.skin.label.alignment = TextAnchor.UpperCenter;
		EMindGUI.Label(rect,Na,12);
		GUI.skin.label.alignment = TextAnchor.UpperLeft;
		
		rect.y+=17;//rect.height;
		rect.height = 22;
		rect.width /= 2;
		rect.width += 1;
		if(GUI.Button( rect ,ShaderSandwich.LeftArrow))Type-=1;
		
		rect.x+=InitialRect.width-rect.width;
		if(GUI.Button( rect ,ShaderSandwich.RightArrow))Type+=1;
		
		
/*
				rect.y+=rect.height-18;
				rect.height = 18;
				GUI.Box(rect,"",GUI.skin.button);
				GUI.skin.label.alignment = TextAnchor.UpperCenter;
				EMindGUI.Label(rect,Names[Type],12);
				GUI.skin.label.alignment = TextAnchor.UpperLeft;
				
				rect.y+=17;//rect.height;
				rect.height = 22;
				rect.width /= 2;
				rect.width += 1;
				//rect.width = 40;
				if(GUI.Button( rect ,ShaderSandwich.LeftArrow))Type-=1;
				
				rect.x+=InitialRect.width-rect.width;
				if(GUI.Button( rect ,ShaderSandwich.RightArrow))Type+=1;
				
				if (Type>=Names.Length)
				Type = 0;				
				if (Type<0)
				Type = Names.Length-1;		
*/
		
		return false;
	}
	
	
	public void UseEditingMouse(bool Paint){
		Rect rect = LastUsedRect;
		//rect.x-=150-LabelOffset;
		ShaderInput OrigInput = Input;
		int OrigComponent = ColorComponent;
		int PopupWidth = 130;
		if (EditingPopup>0f){
			
			if ((Event.current.type == EventType.MouseDown) &&(
			//!(new Rect(rect.x+110,rect.y,20,20).Contains(Event.current.mousePosition))&&
			!(new Rect(rect.x+PopupWidth+10-20,rect.y-(20*EditingPopup)-20,PopupWidth+10,40).Contains(Event.current.mousePosition))
			)){
				ShaderSandwich.ShaderVarEditing = null;
				GUI.changed = true;
			}
		
			if (Paint==true){
				if (EditingPopup>0f){
				//Debug.Log(rect);
					GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,EditingPopup);
					EMindGUI.BeginGroup(new Rect(rect.x+PopupWidth+10-20,rect.y-(20*EditingPopup)-20,PopupWidth+10,40),GUI.skin.button);

					//CheckGUIStyles();

					List<string> InputNamesList = new List<string>();
					List<int> InputIntsList = new List<int>();
					List<ShaderInput> InputInputsList = new List<ShaderInput>();
					List<int> InputComponentsList = new List<int>();
					InputNamesList.Add("-");
					InputIntsList.Add(0);
					InputInputsList.Add(null);
					InputComponentsList.Add(0);
					int ii = 1;
					foreach(ShaderInput SI in ShaderBase.Current.ShaderInputs){
						string SIType = "";
						if (SI.Type==ShaderInputTypes.Image)
						SIType = "Tex";
						if (SI.Type==ShaderInputTypes.Color)
						SIType = "Color";
						if (SI.Type==ShaderInputTypes.Vector)
						SIType = "Vector";
						if (SI.Type==ShaderInputTypes.Cubemap)
						SIType = "Cube";
						if (SI.Type==ShaderInputTypes.Float)
						SIType = "Float";
						if (SI.Type==ShaderInputTypes.Range)
						SIType = "Range";
						if (
						(CType == Types.Float&&(SI.Type==ShaderInputTypes.Float||SI.Type==ShaderInputTypes.Range||SI.Type==ShaderInputTypes.Color||SI.Type==ShaderInputTypes.Vector))||
						(CType == Types.Vec&&SI.Type==ShaderInputTypes.Color||SI.Type==ShaderInputTypes.Vector)||
						(CType == Types.Texture&&SI.Type==ShaderInputTypes.Image)||
						(CType == Types.Cubemap&&SI.Type==ShaderInputTypes.Cubemap)
						){
							if (CType == Types.Float&&(SI.Type==ShaderInputTypes.Color||SI.Type==ShaderInputTypes.Vector)){
								InputNamesList.Add(SIType+": "+SI.VisName.Replace("-","/")+"/R");
								InputIntsList.Add(ii);
								InputInputsList.Add(SI);
								InputComponentsList.Add(0);
								ii++;
								InputNamesList.Add(SIType+": "+SI.VisName.Replace("-","/")+"/G");
								InputIntsList.Add(ii);
								InputInputsList.Add(SI);
								InputComponentsList.Add(1);
								ii++;
								InputNamesList.Add(SIType+": "+SI.VisName.Replace("-","/")+"/B");
								InputIntsList.Add(ii);
								InputInputsList.Add(SI);
								InputComponentsList.Add(2);
								ii++;
								InputNamesList.Add(SIType+": "+SI.VisName.Replace("-","/")+"/A");
								InputIntsList.Add(ii);
								InputInputsList.Add(SI);
								InputComponentsList.Add(3);
								ii++;
							}
							else{
								InputNamesList.Add(SIType+": "+SI.VisName.Replace("-","/"));
								InputIntsList.Add(ii);
								InputInputsList.Add(SI);
								InputComponentsList.Add(0);
								ii++;
							}
						}
						
					}
					string[] InputNames = InputNamesList.ToArray();
					int[] InputInts = InputIntsList.ToArray();
					//InputNames[ii] = "Add New Input";
					
					int IndexOfInput = 0;
					if (InputInputsList.IndexOf(Input)!=-1)
					IndexOfInput = InputInputsList.IndexOf(Input)+ColorComponent;
					//IndexOfInput = ShaderBase.Current.ShaderInputs.IndexOf(Input)+1+ColorComponent;
					//Debug.Log(IndexOfInput);
					EMindGUI.Label(new Rect(5,5,PopupWidth,15),"Inputs:",11);
					//IndexOfInput = 5;
					//int SIS = InputInts[EditorGUI.Popup(new Rect(5,20,75,15), Array.IndexOf(InputInts,IndexOfInput), InputNames,GUI.skin.GetStyle("MiniPopup"))];
					//IntInput = EditorGUI.Popup(new Rect(5,20,75,15),IntInput, InputNames,GUI.skin.GetStyle("MiniPopup"));
					//int SIS = IntInput;
					int SIS = EditorGUI.IntPopup(new Rect(5,20,PopupWidth-55,15), IndexOfInput, InputNames,InputInts,GUI.skin.GetStyle("MiniPopup"));
					
					if (SIS>=InputInputsList.Count)
					SIS = 0;
					
					if (SIS==0)
					Input = null;
					else{
						Input = InputInputsList[SIS];
						ColorComponent = InputComponentsList[SIS];
					}
					//Debug.Log(SIS);
					//Debug.Log(ColorComponent);
					//Input = ShaderBase.Current.ShaderInputs[SIS-1];
					if (GUI.Button(new Rect(PopupWidth-50,20,15,15),ShaderSandwich.Plus,EMindGUI.ButtonNoBorder)||EMindGUI.MouseUpIn(new Rect(PopupWidth-50,20,15,15))){
						AddInput();
						ShaderSandwich.ShaderVarEditing = null;
						ShaderSandwich.ValueChanged = true;
						GUI.changed = true;
						ShaderSandwich.Instance.RegenShaderPreview();
						ShaderSandwich.Instance.UpdateShaderPreview();
					}
					if (GUI.Button(new Rect(PopupWidth-30,20,15,15),ShaderSandwich.IconMask,EMindGUI.ButtonNoBorder)||EMindGUI.MouseUpIn(new Rect(PopupWidth-30,20,15,15))){
						AddInput();
						Input.Mask.HookIntoObjects();
						ShaderSandwich.ShaderVarEditing = null;
						ShaderSandwich.ValueChanged = true;
						GUI.changed = true;
						ShaderSandwich.Instance.RegenShaderPreview();
						ShaderSandwich.Instance.UpdateShaderPreview();
						ShaderSandwich.Instance.OpenShader.AddMask();
						//ShaderUtil.MoveItem(ref ShaderSandwich.Instance.OpenShader.ShaderLayersMasks,ShaderSandwich.Instance.OpenShader.ShaderLayersMasks.Count-1,0);
						ShaderLayerList Mask = ShaderSandwich.Instance.OpenShader.ShaderLayersMasks[ShaderSandwich.Instance.OpenShader.ShaderLayersMasks.Count-1];
						Mask.Name.Text = Input.VisName;
						//if (Input.Type<=2)
						if (Input.Type==ShaderInputTypes.Image||Input.Type==ShaderInputTypes.Color||Input.Type==ShaderInputTypes.Vector||Input.Type==ShaderInputTypes.Cubemap)
							Mask.EndTag.Text = "rgba";
						
						if (Input.Type==ShaderInputTypes.Color){
							ShaderLayer Sl = ShaderLayer.CreateInstance<ShaderLayer>();
							Mask.Add(Sl);
							Sl.CustomData.D["Color"] = new ShaderVar("Color",new Vector4(Vector.r,Vector.g,Vector.b,Vector.a));
							ShaderSandwich.Instance.SRS.LayerSelection = Sl;
						}
						if (Input.Type==ShaderInputTypes.Vector){
							ShaderLayer Sl = ShaderLayer.CreateInstance<ShaderLayer>();
							Sl.SetType("SLTVector");
							Mask.Add(Sl);
							Sl.CustomData.D["Color"] = new ShaderVar("Color",new Vector4(Vector.r,Vector.g,Vector.b,Vector.a));
							ShaderSandwich.Instance.SRS.LayerSelection = Sl;
						}
						if (Input.Type==ShaderInputTypes.Float||Input.Type==ShaderInputTypes.Range){
							ShaderLayer Sl = ShaderLayer.CreateInstance<ShaderLayer>();
							Mask.Add(Sl);
							Sl.LayerType.Obj = ShaderSandwich.ShaderLayerTypeInstances["SLTNumber"];
							Sl.LayerTypeInputs = ((ShaderLayerType)Sl.LayerType.Obj).GetInputs();
							ShaderUtil.SetupCustomData(Sl.CustomData,Sl.LayerTypeInputs);
							Sl.CustomData.D["Number"] = new ShaderVar("Number",Float);
							ShaderSandwich.Instance.SRS.LayerSelection = Sl;
							//Sl.AddLayerEffect("SSEMathAdd");
							///TOoDO:
							///Sl.LayerEffects[0].Inputs[0].Float = Float;
						}
						Mask.UpdateIcon(new Vector2(70,70));
						Input.Mask.RealObj = Mask;
						ShaderSandwich.Instance.UpdateMaskShaderVars();
						Input.InEditor = false;
						Input.SpecialType = InputSpecialTypes.Mask;
						//Debug.Log(Input.Mask.RealObj);
						//Debug.Log(Input.Mask.Obj);
						
						ShaderSandwich.Instance.CorrectAllLayers();
						ShaderSandwich.Instance.ChangeSaveTemp(null);
						ShaderSandwich.Instance.Goto(ShaderSandwich.GUIType.Layers,ShaderTransition.TransDir.Backward);
						ShaderSandwich.ShaderLayerTab = -1;
						EditorGUIUtility.ExitGUI();
					}
					if (GUI.Button(new Rect(PopupWidth-10,20,15,15),ShaderSandwich.CrossRed,EMindGUI.ButtonNoBorder)||EMindGUI.MouseUpIn(new Rect(PopupWidth-10,20,15,15))){
						/*ShaderBase.Current.ShaderInputs.Remove(Input);
						List<ShaderVar> SVs = new List<ShaderVar>();
						foreach (ShaderLayer SL in ShaderUtil.GetAllLayers()){
							SL.UpdateShaderVars(true);
							SVs.AddRange(SL.ShaderVars);
						}
						SVs.AddRange(ShaderBase.Current.GetMyShaderVars());
						ShaderInput OldInput = Input;
						foreach(ShaderVar SV in SVs){
							if (SV.Input==OldInput)
							SV.Input = null;
						}*/
						if (Input!=null){
						ShaderInput OldInput = Input;
						Input = null;
						List<ShaderVar> SVs = ShaderUtil.GetAllShaderVars();//new List<ShaderVar>();
						/*foreach (ShaderLayer SL in ShaderUtil.GetAllLayers()){
							SL.UpdateShaderVars(true);
							SVs.AddRange(SL.ShaderVars);
						}
						SVs.AddRange(ShaderBase.Current.GetMyShaderVars());*/
						OldInput.UsedCount = 0;
						foreach(ShaderVar SV in SVs){
							if (SV.Input==OldInput)
								OldInput.UsedCount+=1;
						}
						if (OldInput.UsedCount==0){
							ShaderSandwich.Instance.InputDelete(OldInput);
							/*ShaderBase.Current.ShaderInputs.Remove(OldInput);
							foreach(ShaderVar SV in SVs){
								if (SV.Input==OldInput)
									SV.Input = null;
							}*/
							
						}
						//UEObject.DestroyImmediate(SL,false);
						ShaderSandwich.ShaderVarEditing = null;
						EditorGUIUtility.ExitGUI();
						}
					}
					if (OrigInput!=Input||OrigComponent!=ColorComponent){
						UpdateToInput();
						GUI.changed = true;
						ShaderSandwich.Instance.ChangeSaveTemp(Parent);
					}
					EMindGUI.EndGroup();
				}

			}
		}
	}
	public ShaderInput AddInput(){
		UnityEngine.Object o = ShaderSandwich.Instance.OpenShader;
		if (Parent!=null)
			o = Parent;
		Undo.RegisterCompleteObjectUndo(new UnityEngine.Object[]{o,ShaderBase.Current},"Added Input");
		Input = ShaderInput.CreateInstance<ShaderInput>();
		Input.AutoCreated = true;
		//Debug.Log(MyParent);
		if (Parent!=null)
		Input.VisName = Parent.Name.Text+" - "+Name;
		else
		Input.VisName = Name;
		ShaderBase.Current.ShaderInputs.Add(Input);
		ShaderBase.Current.RecalculateAutoInputs();
		Input.Number = Float;
		Input.Range0 = Range0;
		Input.Range1 = Range1;
		Input.Color = Vector;

		if (CType==Types.Vec&&VecIsColor)
			Input.Type=ShaderInputTypes.Color;
		if (CType==Types.Vec&&!VecIsColor)
			Input.Type=ShaderInputTypes.Vector;
		if (CType==Types.Float)
			if (NoSlider==false)
				Input.Type=ShaderInputTypes.Range;
			else
				Input.Type=ShaderInputTypes.Float;
		
		
		if (CType==Types.Texture){
			Input.Image = Image;
			Input.ImageGUID = ImageGUID;
			Input.Type = ShaderInputTypes.Image;
		}
		if (CType==Types.Cubemap){
			Input.Cube = Cube;
			Input.CubeGUID = CubeGUID;
			Input.Type = ShaderInputTypes.Cubemap;
		}
		//if (Input.Type<=2)
		if (Input.Type==ShaderInputTypes.Image||Input.Type==ShaderInputTypes.Color||Input.Type==ShaderInputTypes.Vector||Input.Type==ShaderInputTypes.Cubemap)
			Input.Mask.RGBAMasks = true;
		return Input;
	}
	void DrawEditingInterface(){
	
	}
	public void DrawGear(Rect rect){
//		CheckGUIStyles();
		if (NoInputs==false){
			LastUsedRect = SU.AddRectVector(rect,EMindGUI.RectPos);
			//if (Editing==false)
			//	Editing = GUI.Toggle(new Rect(rect.x+110,rect.y,20,20),Editing,new GUIContent(ShaderSandwich.Gear),EMindGUI.ButtonNoBorder);
			//else
				if ((Event.current.type == EventType.MouseDown) &&(new Rect(rect.x+LabelOffset-30,rect.y,rect.height,rect.height).Contains(Event.current.mousePosition))){
				if (ShaderSandwich.ShaderVarEditing!=this){
				ShaderSandwich.ShaderVarEditing = this;
				EditingPopupStartTime = EditorApplication.timeSinceStartup;
				}
				else
				ShaderSandwich.ShaderVarEditing = null;
				}
				if (Input!=null)
				GUI.Toggle(new Rect(rect.x+LabelOffset-30,rect.y,rect.height,rect.height),ShaderSandwich.ShaderVarEditing==this,new GUIContent(ShaderSandwich.GearLinked),EMindGUI.ButtonNoBorder);
				else
				GUI.Toggle(new Rect(rect.x+LabelOffset-30,rect.y,rect.height,rect.height),ShaderSandwich.ShaderVarEditing==this,new GUIContent(ShaderSandwich.Gear),EMindGUI.ButtonNoBorder);
				

				
				if (Event.current.type==EventType.Repaint){
					if (ShaderSandwich.ShaderVarEditing==this){
						//EditingPopup+=0.03f;
						EditingPopup = Mathf.Min(1,(float)(EditorApplication.timeSinceStartup-EditingPopupStartTime)*6f);
					}
					else{
						//EditingPopup-=0.03f;
						EditingPopup = Mathf.Max(0,(float)(1-(EditorApplication.timeSinceStartup-EditingPopupStartTime))*6f);
					}
				}
				
				if (WarningTitle!=""&&WarningDelegate!=null)
				if (GUI.Button(new Rect(rect.x+LabelOffset-50-5,rect.y,20,20),new GUIContent(ShaderSandwich.Warning),EMindGUI.ButtonNoBorder)){
				
				if (WarningOption3!="")
				WarningDelegate(EditorUtility.DisplayDialogComplex(WarningTitle,WarningMessage,WarningOption1,WarningOption2,WarningOption3),this);
				else
				WarningDelegate(EditorUtility.DisplayDialog(WarningTitle,WarningMessage,WarningOption1,WarningOption2) ? 0 : 1,this);
				}
				
				//GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,1);
		}

	}
	public string GetMaskName(){
		if (Obj!=null)
		return "vd."+((ShaderLayerList)Obj).CodeName;
		return "float4(0.5f,0.5f,0.5f,0.5f)";
	}
	public void SetCustomIcon(Texture cs){
		CustomIcon = cs;
	}
	public int LabelOffset = 150;
	//Vector2 SliderMousePos;
	bool held;
	bool Draw_Real(Rect rect,DrawTypes d,string S){
		//if (!AttemptedHook)
		//	HookIntoObjects();
//		CheckGUIStyles();
		if (ForceGUIChange==true){
			ForceGUIChange = false;
			GUI.changed = true;
		}	
		//LastUsedRect = SU.AddRectVector(rect,EMindGUI.GetGroupVector());
		LastUsedRect = SU.AddRectVector(rect,EMindGUI.RectPos);
		bool RetVal = false;
		Rect InitialRect = rect;
		Color oldCol = GUI.backgroundColor;
		ShaderColor OldVector = Vector;
		RetVal = UpdateToInput(false);
		
		if (d == DrawTypes.Type&&Names!=null&&Descriptions!=null&&Type<Descriptions.Length){
			if (ImagePaths==null||ImagePaths.Length==0){
				//if (Names!=null)
				rect.height*=Mathf.Ceil((float)Names.Length/(float)TypeDispL);
				int oldType =  Type;
				
				GUIStyle ButtonStyle = GUI.skin.button;
				if (ShaderSandwich.Instance.CurInterfaceStyle == InterfaceStyle.Modern){
					ButtonStyle = EMindGUI.SCSkin.window;//new GUIStayle(ShaderSandwich.SCSkin.window);
					//ButtonStyle.fontSize = 12;
					//ButtonStyle.contentOffset = new Vector2(0,0);
					//ButtonStyle.padding = new RectOffset(0,0,0,0);
					//ButtonStyle.font = GUI.skin.button.font;
				}
				//if (Names==null)
				//Debug.Log(Name);
				///if (GUI.enabled&&Event.current.type==EventType.Repaint){
					///GUI.BeginGroup(new Rect(rect.x-1,rect.y,40,rect.height));
					//GUI.Toggle(new Rect(1,0,rect.width/TypeDispL,rect.height),Type==0,Names[0],ButtonStyle);
					///ButtonStyle.Draw(new Rect(1,0,rect.width/TypeDispL,rect.height),false,Type==0,Type==0,Type==0);
					///GUI.EndGroup();
					//GUI.Toggle(new Rect(1,0,rect.width/TypeDispL,rect.height),Type==0,Names[0],ButtonStyle);
					//ButtonStyle.Draw(new Rect(rect.x,rect.y,7,rect.height),false,Type==0,Type==0,Type==0);
					//GUI.Box(new Rect(rect.x,rect.y,rect.width/TypeDispL,rect.height),"");
					GUI.DrawTexture(new Rect(rect.x-6,rect.y-4,6,24),ShaderSandwich.WindowEdge);
				///}
				EditorGUI.BeginChangeCheck();
				Type = GUI.SelectionGrid(rect,Type,Names,TypeDispL,ButtonStyle);
				if(EditorGUI.EndChangeCheck())
					EMindGUI.Defocus();
				if (Type!=oldType)
					RetVal = true;
				rect.y+=rect.height;
				rect.height+=40;
				if (Type<Descriptions.Length)
					EMindGUI.Label(rect,Descriptions[Type],12);
			}
			else{
				//GUI.backgroundColor = new Color(0,0,0,1);
				
				if (Type>=Names.Length)
					Type = 0;				
				if (Type<0)
					Type = Names.Length-1;
				
				//GUI.Box(rect,"",GUI.skin.button);
				if (Images==null)Images = new Texture2D[ImagePaths.Length];
				if (Images[Type]==null)Images[Type] = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/"+ImagePaths[Type]) as Texture2D;
				Rect AspectCorrectedRect = new Rect(rect.x+rect.width/2-rect.height/2,rect.y,rect.height,rect.height);
				Color OldGUI2 = GUI.color;
				if (ImagePaths[Type]!=""){
					AspectCorrectedRect.x+=5;
					AspectCorrectedRect.y+=5;
					AspectCorrectedRect.width-=10;
					AspectCorrectedRect.height-=10;
					AspectCorrectedRect.x += 8;
					AspectCorrectedRect.y += 4;
					
					GUI.color = new Color(0f,0f,0f,0.3f);
					GUI.DrawTexture( AspectCorrectedRect ,Images[Type]);
					GUI.color = OldGUI2;
					AspectCorrectedRect.x -= 8;
					AspectCorrectedRect.y -= 4;
					
					GUI.DrawTexture( AspectCorrectedRect ,Images[Type]);
					//GUI.DrawTexture( AspectCorrectedRect ,Images[Type]);
					//GUI.DrawTexture( AspectCorrectedRect ,Images[Type]);
					//GUI.DrawTexture( AspectCorrectedRect ,Images[Type]);
					//GUI.DrawTexture( rect ,Images[Type]);
					//GUI.DrawTexture( rect ,Images[Type]);
					//GUI.DrawTexture( rect ,Images[Type]);
				}
				
				rect.y+=rect.height;
				//rect.height = 18;
				rect.height = 32;
				//GUI.Box(rect,"",GUI.skin.button);
				GUI.skin.label.alignment = TextAnchor.UpperCenter;
				OldGUI2 = GUI.skin.label.normal.textColor;
				GUI.skin.label.normal.textColor = GUI.color;//EMindGUI.BaseInterfaceColor;
				EMindGUI.Label(rect,Names[Type],12);
				GUI.skin.label.normal.textColor = OldGUI2;
				GUI.skin.label.alignment = TextAnchor.UpperLeft;
				
				//rect.y+=17;//rect.height;
				//rect.height = 22;
				//rect.width /= 2;
				//rect.width += 1;
				Rect sideRect = new Rect(rect.x-1,AspectCorrectedRect.y+32,24,AspectCorrectedRect.height-64);
				//rect.width = 40;
				//Vector2 oldPos = GUI.skin.button.contentOffset;
				//GUI.skin.button.contentOffset = new Vector2(-rect.width/2+14,0);
				GUI.skin.button.stretchHeight = true;
				//
				if (Event.current.type!=EventType.Repaint)
				if(GUI.Button( sideRect ,""))Type-=1;
				
				
				//rect.x+=InitialRect.width-rect.width;
				
				sideRect.x += rect.width-24+1;
				//GUI.skin.button.contentOffset = new Vector2(rect.width/2-14,0);
				if (Event.current.type!=EventType.Repaint)
				if(GUI.Button( sideRect ,""))Type+=1;
				
				Color OldGUI = GUI.color;
				//GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,0.5f);
				sideRect = new Rect(rect.x-1,AspectCorrectedRect.y+32,24,AspectCorrectedRect.height-64);
				float oldy = sideRect.y;
				float oldx = sideRect.x;
				if (sideRect.Contains(Event.current.mousePosition)){
					GUI.color = EMindGUI.BaseInterfaceColor;
					if (EMindGUI.mouse.MouseDrag){
						sideRect.x+=1;
						sideRect.y+=1;
					}
				}
				else
					GUI.color = OldGUI;
				sideRect.y-=32;
				sideRect.height+=64;
				
				GUI.DrawTexture( sideRect ,ShaderSandwich.LeftTriangle);
				sideRect.y = oldy;
				sideRect.height -= 64;
				sideRect.x = oldx;
				sideRect.x += rect.width-24+1;
				//Debug.Log(sideRect.Contains(Event.current.mousePosition));
				if (sideRect.Contains(Event.current.mousePosition)){
					GUI.color = EMindGUI.BaseInterfaceColor;
					if (EMindGUI.mouse.MouseDrag){
						sideRect.x-=1;
						sideRect.y+=1;
					}
				}
				else
					GUI.color = OldGUI;
				
				sideRect.y-=32;
				sideRect.height+=64;
				GUI.DrawTexture( sideRect ,ShaderSandwich.RightTriangle);
				GUI.color = OldGUI;
				//GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,1f);
				GUI.skin.button.stretchHeight = false;
				//GUI.skin.button.contentOffset = oldPos;
				//
				if (Type>=Names.Length)
				Type = 0;				
				if (Type<0)
				Type = Names.Length-1;				
			}
		}
		else
		{
			if (S!="")
			{
				if (d != DrawTypes.Toggle)
				DrawGear(rect);
				
				GUI.backgroundColor = Color.white;
				rect.y+=2;
				EMindGUI.Label(rect,S,12);
				rect.y-=2;
				//UseInput = GUI.Toggle(new Rect(rect.x+120,rect.y+2,20,20),UseInput,"");
				rect.x+=LabelOffset;
				rect.width-=LabelOffset;
			}
			GUI.backgroundColor = Color.white;
			if (d == DrawTypes.Slider01){
				GUI.backgroundColor = new Color(0.2f,0.2f,0.2f,1f);
				
				if (!NoSlider){
					//if (S!="")
					//rect.width-=60-20;
					rect.y+=1;
					rect.width-=48;//68-20;
					EditorGUI.BeginChangeCheck();
					//Float = GUI.HorizontalSlider (rect,Float, Range0, Range1);
					//GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,0.5f);
					if (Event.current.shift)
						EditorGUIUtility.AddCursorRect(rect,MouseCursor.SlideArrow);
					if (Event.current.shift&& held)
						EditorGUIUtility.AddCursorRect(new Rect(Event.current.mousePosition.x-5,Event.current.mousePosition.y-5,10,10),MouseCursor.SlideArrow);
					
					if (Event.current.rawType == EventType.MouseUp){
						held = false;
					}
					if (Event.current.type==EventType.MouseDown){
						held = rect.Contains(Event.current.mousePosition);
						//if (held)
						//	Cursor.lockState = CursorLockMode.Locked;
						//else 
						//	Cursor.lockState = CursorLockMode.None;
					}else{
						//Debug.Log(Event.current.isMouse&&Event.current.type!=EventType.MouseDown);
						if (Event.current.isMouse && Event.current.shift && held){
							EMindGUI.Slider (rect,Float, Range0, Range1);
							Float += (Event.current.delta.x)*(0.05f*(Mathf.Abs(Range1-Range0)/rect.width));
							if (Event.current.delta.x!=0)
								GUI.changed = true;
							//SliderMousePos = Event.current.mousePosition;
						}
						else if (Event.current.isMouse && Event.current.shift){
							held = false;
							//Cursor.lockState = CursorLockMode.None;
						}
					}
					if (!(Event.current.isMouse && Event.current.shift && held)){
						Float = EMindGUI.Slider (rect,Float, Range0, Range1);
					}
					//GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,1);
					if (EditorGUI.EndChangeCheck())
						EditorGUIUtility.editingTextField = false;
					//if (S!="")
					if (!NoBox){
						rect.y-=1;
					//rect.width+=10;
					//Float = EditorGUI.FloatField( new Rect(rect.x+rect.width+10,rect.y,30,20),Float,EMindGUI.EditorFloatInput);
					Float = EditorGUI.FloatField( new Rect(rect.x+rect.width+10,rect.y+2,38,16),Float,EMindGUI.EditorFloatInput);
					}
				}
				else{
					rect.x-=5;
					rect.width+=6;
					EditorGUIUtility.labelWidth = 30;
					if (NoArrows)
						Float = EditorGUI.FloatField( rect,Float,EMindGUI.EditorFloatInput);
					else
						Float = EditorGUI.FloatField( rect,"<>",Float,EMindGUI.EditorFloatInput);
					EditorGUIUtility.labelWidth = 0;
					//GUI.Box(rect,"");
				}
			}
			if (d == DrawTypes.Color){
				
				EditorGUI.BeginChangeCheck();
				Color newcol = EditorGUI.ColorField (rect,Vector.ToColor());
				if (EditorGUI.EndChangeCheck()){
					Vector = new ShaderColor(newcol);
					if (!OldVector.Cmp(Vector))
					RetVal = true;
				}
				
				if (ShowNumbers){
					GUI.backgroundColor = new Color(0.2f,0.2f,0.2f,1f);
					EditorGUI.BeginChangeCheck();
					
						rect = InitialRect;
						rect.y+=rect.height;
						if (HideAlpha){
							rect.width/=3;
							//rect.width-=12;
							
							EditorGUIUtility.labelWidth = 12;
							
							//rect.x+=12;
							Vector.r = EditorGUI.FloatField( rect,"R", Vector.r,EMindGUI.EditorFloatInput);
							
							rect.x+=rect.width;
							//rect.x+=12;
							Vector.g = EditorGUI.FloatField( rect,"G", Vector.g,EMindGUI.EditorFloatInput);
							
							rect.x+=rect.width;
							//rect.x+=12;
							Vector.b = EditorGUI.FloatField( rect,"B", Vector.b,EMindGUI.EditorFloatInput);
						}
						else{
							rect.width/=4;
							//rect.width-=12;
							
							EditorGUIUtility.labelWidth = 12;
							
							//rect.x+=12;
							Vector.r = EditorGUI.FloatField( rect,"x", Vector.r,EMindGUI.EditorFloatInput);
							
							rect.x+=rect.width;
							//rect.x+=12;
							Vector.g = EditorGUI.FloatField( rect,"y", Vector.g,EMindGUI.EditorFloatInput);
							
							rect.x+=rect.width;
							//rect.x+=12;
							Vector.b = EditorGUI.FloatField( rect,"z", Vector.b,EMindGUI.EditorFloatInput);
							
							rect.x+=rect.width;
							//rect.x+=12;
							Vector.a = EditorGUI.FloatField( rect,"w", Vector.a,EMindGUI.EditorFloatInput);
						}
						
						EditorGUIUtility.labelWidth = 0;
						
					if (EditorGUI.EndChangeCheck())
						RetVal = true;
				}
			
			}
			if (d == DrawTypes.Texture){
				if (Image==null)
					Image = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(ImageGUID),typeof(Texture2D));
				
				Texture2D oldImage = Image;
				
				Image = (Texture2D) EditorGUI.ObjectField (rect,Image, typeof (Texture2D),false);
				ImageGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(Image));
				if (oldImage!=Image)
					RetVal = true;
			}
			if (d == DrawTypes.Cubemap){
				if (Cube==null)
					Cube = (Cubemap)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(CubeGUID),typeof(Cubemap));
				
				Cubemap oldCube = Cube;
				
				Cube = (Cubemap) EditorGUI.ObjectField (rect,Cube, typeof (Cubemap),false);
				CubeGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(Cube));
				if (oldCube!=Cube)
					RetVal = true;
			}
			if (d == DrawTypes.Toggle){
				//Debug.Log("Toggle!");
				rect.x-=30;//+=(rect.width-50);
				rect.width = rect.height;
				//rect.height = 17;

				GUI.backgroundColor = new Color(0.2f,0.2f,0.2f,1f);
				
				if (EMindGUI.MouseDownIn(rect,false))
					EMindGUI.Defocus();//I don't know why toggle doesn't take focus, but 'eh...
				
				EditorGUI.BeginChangeCheck();
				Color oldg = GUI.contentColor;
				if (On)
					GUI.contentColor = On?EMindGUI.BaseInterfaceColorComp2:EMindGUI.BaseInterfaceColorComp;
				if (On)
					On = GUI.Toggle(rect,On,ShaderSandwich.Tick,EMindGUI.ButtonNoBorder);
				else
					On = GUI.Toggle(rect,On,ShaderSandwich.Cross,EMindGUI.ButtonNoBorder);
				GUI.contentColor = oldg;
				if (EditorGUI.EndChangeCheck())
					RetVal = true;
				
				
				
				//On = EditorGUI.Toggle (rect,On);
			
			}
			if (d == DrawTypes.ObjectArray){
				if (Event.current.type == EventType.Repaint)
					GUI.skin.GetStyle("ObjectFieldThumb").Draw(rect, false, false, false,ObjFieldOn); 
				if ((ObjField!=null)){
					//GUIUtility.hotControl = 0;
					ObjFieldOn = true;
				}
				if (ShaderSandwich.GUIMouseDown&&(rect.Contains(Event.current.mousePosition)))
					ObjFieldOn = true;
				if (GUIUtility.hotControl != 0&&ObjField==null)
					ObjFieldOn = false;
				//GUI.skin.GetStyle("ObjectFieldThumb").Draw(rect, bool isHover, bool isActive, bool on, bool hasKeyboardFocus); 
				Rect newRect = rect;
				newRect.width-=2;
				newRect.height-=2;
				newRect.y+=1;
				newRect.x+=1;
				if (Obj!=null){
					if (Event.current.type==EventType.Repaint){
						if (CustomIcon!=null)
							Graphics.DrawTexture(newRect,CustomIcon,ShaderUtil.GPUGUIMat.Material);
							//GUI.DrawTexture(newRect,CustomIcon);
						else
							Graphics.DrawTexture(newRect,ObjFieldImage[Selected],ShaderUtil.GPUGUIMat.Material);
							//Graphics.DrawTexture(new Rect(0,0,64,64),ObjFieldImage[Selected],ShaderUtil.GPUGUIMat.Material);
							//GUI.DrawTexture(newRect,ObjFieldImage[Selected]);
					}
				}
				//rect.x+=rect.width-32;
				rect.x+=rect.width-36;
				rect.width = 36;
				rect.y+=rect.height-8;
				rect.height = 8;
				if (GUI.Button(rect,"Select",GUI.skin.GetStyle("ObjectFieldThumbOverlay2"))){
					//Debug.Log("yay");
					HookIntoObjects();
					if (RGBAMasks){
						//ObjField = ShaderObjectField.Show(this,"Select Mask (RGBA Masks Only!)",ObjFieldObject,ObjFieldImage,ObjFieldEnabled,Selected);
						ObjField = ShaderObjectField.Show(this,"Select Mask (RGBA Masks Supported/Suggested)",ObjFieldObject,ObjFieldImage,ObjFieldEnabled,Selected);
						ObjField.SomethingOtherThanAMask = true;
					}
					else{
						ObjField = ShaderObjectField.Show(this,"Select "+ObjectUpdater,ObjFieldObject,ObjFieldImage,ObjFieldEnabled,Selected);
						if (ObjectUpdater!="Mask")
							ObjField.NullIsAllowed = false;
					}
				}
			}
			
		}
		UpdateToVar();
		
		GUI.backgroundColor = oldCol;
		
		if (RetVal&&OnChange!=null)
			OnChange();
		
		return RetVal;
	}

	public bool UpdateToInput(){
		return UpdateToInput(true);
	}
	public bool UpdateToInput(bool Execute){
		bool RetVal = false;
		if (Input!=null){
			
			float OldFloat =  Float;
			if ((Input.Type==ShaderInputTypes.Color||Input.Type==ShaderInputTypes.Vector)&&CType==Types.Float){
				if (ColorComponent==0)
					Float = Input.Color.r;
				if (ColorComponent==1)
					Float = Input.Color.g;
				if (ColorComponent==2)
					Float = Input.Color.b;
				if (ColorComponent==3)
					Float = Input.Color.a;
			}
			else
				Float = Input.Number;
			
			if (!Mathf.Approximately(OldFloat,Float))
				RetVal = true;
			
			Range0 = Input.Range0;
			Range1 = Input.Range1;
			
			if (Vector!=null&&Input.Color!=null&&!Input.Color.Cmp(Vector)){
				RetVal = true;}//Debug.Log("Color");}
			Vector = Input.Color;
			
			if (Input.ImageS()!=Image)//Need to fix blahblah
				RetVal = true;
			Image = Input.Image;
			
			if (Input.CubeS()!=Cube){
				RetVal = true;}//Debug.Log("Cubemap");}
			Cube = Input.Cube;
		}
		if (RetVal&&OnChange!=null&&Execute){
			OnChange();}
		if (RetVal&&Execute){
			ShaderSandwich.ValueChanged = true;
		}
		
		return RetVal;
	}
	public void UpdateToVar(){
		if (Input!=null){
			
			if ((Input.Type==ShaderInputTypes.Color||Input.Type==ShaderInputTypes.Vector)&&CType==Types.Float){
				//Vector = Vector.Copy();
				if (ColorComponent==0)
				Vector.r = Float;
				if (ColorComponent==1)
				Vector.g = Float;
				if (ColorComponent==2)
				Vector.b = Float;
				if (ColorComponent==3)
				Vector.a = Float;
			}
			else
			Input.Number = Float;
			
			Input.Range0 = Range0;
			Input.Range1 = Range1;
			Input.Color = Vector;
			Input.Image = Image;
			Input.Cube = Cube;
			Input.ImageGUID = ImageGUID;
			Input.CubeGUID = CubeGUID;
		}
	}
	public Texture2D ImageS(){
		if (Image==null&&(Input==null||(Input.Image!=null)))
			Image = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(ImageGUID),typeof(Texture2D));
		//Debug.Log(AssetDatabase.GUIDToAssetPath(ImageGUID));
		return Image;
	}
	public Cubemap CubeS(){
		if (Cube==null&&(Input==null||(Input.Cube!=null)))
			Cube = (Cubemap)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(CubeGUID),typeof(Cubemap));
		return Cube;
	}
	public bool NeedLinked(){
		if ((CType==Types.Texture&&Image!=null)||(CType==Types.Cubemap&&Cube!=null)){
			return true;
		}
		return false;
	}
	public Texture2D GetImage(int n){
		if (Images==null)Images = new Texture2D[ImagePaths.Length];
		if (Images[n]==null)
			Images[n] = EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/"+ImagePaths[n]) as Texture2D;
		return Images[n];	
		
	}
/*	public ShaderColor Vector;//{ get; set; }
	public string ImageGUID = "";
	public string CubeGUID = "";
	public float Float;
	public ShaderInput Input;
	public ShaderObjectField ObjField;
	public int Type;
	
	public bool On;
	
	public int TypeDispL = 4;
	
	public float Range0 = 0;
	public float Range1 = 1;
	
	public Types CType;
	
public enum Types {
Vec,
Float,
Type,
Toggle,
Texture,
Cubemap,
ObjectArray};	
	*/	
	public string Save(int tabs){
	//
		string S = ShaderUtil.Tabs[tabs]+Name+"("+CType.ToString()+"): ";
		if (CType == Types.Vec)
			S += Vector.Save();
		if (CType == Types.Float)
			S += Float.ToString();
		if (CType == Types.Type){
			if (HiddenNames!=null){
				if (HiddenNames.Length>Type)
				S += HiddenNames[Type].Replace("\n","\\n");
				else{
					foreach(string s in HiddenNames){
						Debug.LogWarning("Hi there! There was a minor glitch, could you send the following console lines to Ztechnologies.com@gmail.com, thanks :)");
						Debug.LogWarning(Name);
						Debug.LogWarning(s);
					}
				}
			}
			else
				S += "SSN/A";
		}
		if (CType == Types.Toggle)
			S += On.ToString();
		if (CType == Types.Texture)
			S += ImageGUID;
		if (CType == Types.Cubemap)
			S += CubeGUID;
		if (CType == Types.Text)
			S += "\""+Text.Replace("\"","\\q")+"\"";
		if (CType == Types.ObjectArray){
			if (Selected>=0)
				S += ObjFieldStorageNames[Selected];
			else
				S += "SSNone";
		}
		//if (CType == Types.ObjectArray)
		//S += CubeGUID;
		if (Input!=null||CType == Types.Type||CType == Types.ObjectArray){
			S += " - {";
			bool asd = false;
			
			if (CType==Types.Type){
				S += (asd?", ":"")+"TypeID = "+Type.ToString();
				asd = true;
			}
			
			if (CType==Types.ObjectArray){
				S += (asd?", ":"")+"ObjectID = "+Selected.ToString();
				asd = true;
			}
			
			if (Input!=null){
				if (CType==Types.Float&&(Input.Type==ShaderInputTypes.Color||Input.Type==ShaderInputTypes.Vector)){
					S += (asd?", ":"")+"ColorComponent = "+ColorComponent.ToString();
					asd = true;
				}
				//S += "#^CC"+ColorComponent.ToString();
			
				S += (asd?", ":"")+"Input = "+ShaderBase.Current.ShaderInputs.IndexOf(Input).ToString();
				asd = true;
			}
			//S += " #^ "+ShaderBase.Current.ShaderInputs.IndexOf(Input).ToString();
		
		
			S += "}";
		}
		S += "\n";
		//S += "#?"+Name+"\n";

		return S;
	}
	public ShaderVar Load (string S){
		S = ShaderUtil.Sanitize(S);
		//Debug.Log(S);
		//string name = S.Substring(0,S.IndexOf("("));
		//Debug.Log(name);
		//S = S.Replace(" (RGBA)(","(");
		
		string ctype = S.Substring(S.IndexOf("(")+1,S.IndexOf(")")-S.IndexOf("(")-1);
//		Debug.Log(ctype);
		
		CType = (Types)Enum.Parse(typeof(Types),ctype);
		
		if (CType==Types.Text){
			int firstquot = S.IndexOf("\"")+1;
			int secquot = S.IndexOf("\"",firstquot);
			Text = S.Substring(firstquot,secquot-firstquot);
			S = S.Substring(secquot+1);
			//Debug.Log(Text);
		}
		string finalVal = "";
		int FoundTypeID = 0;
		int FoundObjectID = 0;
		string FoundObjectClass = "";
		if (S.Contains(" - {")){//Has Additional Attributes
			string[] attribs = S.Substring(S.IndexOf("{")+1,S.IndexOf("}")-S.IndexOf("{")-1).Split(","[0]);
			//Debug.Log(S.Substring(S.IndexOf("{")+1,S.IndexOf("}")-S.IndexOf("{")-1));
			foreach(string attrib in attribs){
				//Debug.Log(attrib);
				string[] split = attrib.Split("="[0]);
				split[0] = split[0].Trim();
				split[1] = split[1].Trim();
				
				if (split[0]=="Input"){
					Input = ShaderBase.Current.ShaderInputs[int.Parse(split[1])];
					//Debug.Log("yay I  have an uinptus!");
					//Debug.Log(Input.VisName);
				}
				
				if (split[0]=="ColorComponent")
					ColorComponent = int.Parse(split[1]);
				
				if (split[0]=="TypeID")
					FoundTypeID = int.Parse(split[1]);
				
				if (split[0]=="ObjectID")
					FoundObjectID = int.Parse(split[1]);
				if (split[0]=="ObjectClass")
					FoundObjectClass = split[1];
			}//ObjFieldStorageNames
			if (CType!=Types.Text)
				finalVal = S.Substring(S.IndexOf("): ")+3,S.IndexOf(" - ")-S.IndexOf("): ")-3).Trim();
		}else{
			//Debug.Log(S.IndexOf("): "));
			if (CType!=Types.Text&&S.IndexOf("): ")>-1){
				finalVal = S.Substring(S.IndexOf("): ")+3).Trim();
			}
		}
		if (CType!=Types.Text){
			//Debug.Log(finalVal);
			if (CType == Types.Vec)
			Vector.Load(finalVal);
			if (CType == Types.Float)
			float.TryParse(finalVal,out Float);
			if (CType == Types.Type){
				//Debug.Log(Name);
				//Debug.Log(finalVal);
				int i = 0;
				Type = FoundTypeID;
				if (HiddenNames!=null){
					finalVal.Replace("\\n","\n");
					//Debug.Log(finalVal);
					foreach(string nam in HiddenNames){
						//Debug.Log(nam);
						if (nam==finalVal){
							Type = i;
							//Debug.Log("Foudnit!");
						}
						i++;
					}
				}
				//Type = int.Parse(finalVal);
			}
			if (CType == Types.Toggle)
			On = bool.Parse(finalVal);
			if (CType == Types.Texture){
				ImageGUID = finalVal;
				Image = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(ImageGUID),typeof(Texture2D));
			}
			if (CType == Types.Cubemap){
				CubeGUID = finalVal;
				Cube = (Cubemap)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(CubeGUID),typeof(Cubemap));
			}
			//if (CType == Types.ObjectArray){
			//	Selected = int.Parse(finalVal);
			//}
			if (CType == Types.ObjectArray){
				if (FoundObjectClass!="")
					ObjectUpdater = FoundObjectClass;
				else{
					if (ObjectUpdater==null||ObjectUpdater=="")
						ObjectUpdater = "Mask";
				}
					
				//Debug.Log(Name);
				//Debug.Log(S);
				//Debug.Log(finalVal);
				HookIntoObjects();
				//int i = 0;
				Selected = FoundObjectID;
				//if (ObjFieldStorageNames!=null)
				//	Debug.Log(ObjFieldStorageNames);
				//else
				//	Debug.Log("NULL");
				FoundObjectID = ObjFieldStorageNames.IndexOf(finalVal);
				if (FoundObjectID>=0)
					Selected = FoundObjectID;
				ObjIsFalse = true;
				//Debug.Log(Selected);
			}
		}
		
		UpdateToInput();
		//Wrap Color(Vec) : 0.4,0.2,0.2,1
		//ShaderName(Text): "Untitled Shader" - {ColorComponent = 0, Input = ASDASD}
		return this;
	}
	public void LoadLegacy (string S){
		S = ShaderUtil.Sanitize(S);
		//Debug.Log(S);
		string[] parts = S.Split(new string[] { "#^" }, StringSplitOptions.None);
		//Debug.Log(parts[0]);
		int i = 0;
		if (parts[0]=="S2"){
			CType = (Types)Enum.Parse(typeof(Types),parts[1]);
			i = 2;
		}
			
		//Debug.Log(parts[0+i]);
		if (CType == Types.Vec)
		Vector.Load(parts[0+i]);
		if (CType == Types.Float)
		float.TryParse(parts[0+i],out Float);
		if (CType == Types.Type)
		Type = int.Parse(parts[0+i]);
		if (CType == Types.Toggle)
		On = bool.Parse(parts[0+i]);
		if (CType == Types.Text){
		Text = parts[0+i];
		//Debug.Log(Text);
		}
		if (CType == Types.Texture){
			ImageGUID = parts[0+i];
			Image = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(ImageGUID),typeof(Texture2D));
		}
		if (CType == Types.Cubemap){
			CubeGUID = parts[0+i];
			Cube = (Cubemap)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(CubeGUID),typeof(Cubemap));
		}
		//int ExpectedLength = 2;
		if (CType == Types.ObjectArray){
			Selected = int.Parse(parts[0+i]);
		}
		if (parts.Length>1+i){
			if (parts[1+i].Trim()!="AUTO"&&!parts[1+i].Trim().StartsWith("CC")){
				if (ShaderBase.Current.ShaderInputs.Count>int.Parse(parts[1+i].Trim()))
					Input = ShaderBase.Current.ShaderInputs[int.Parse(parts[1+i].Trim())];
			}
			//Debug.Log(parts[1].Trim());
			if (parts[1+i].Trim().StartsWith("CC")){
				ColorComponent = int.Parse(parts[1+i].Trim().Replace("CC",""));
			}
		}
		if (parts.Length==3+i){
			if (parts[2+i].Trim()!="AUTO"&&!parts[2+i].Trim().StartsWith("CC")){
				if (ShaderBase.Current.ShaderInputs.Count>int.Parse(parts[2+i].Trim()))
					Input = ShaderBase.Current.ShaderInputs[int.Parse(parts[2+i].Trim())];
			}
		}
		UpdateToInput();
	}
	public ShaderVar Copy(){
		ShaderVar SV = new ShaderVar();
		SV.Vector = Vector;
		SV.VecIsColor = VecIsColor;
		SV.Text = Text;
		SV.Image = Image;
		SV.ImageGUID = ImageGUID;
		SV.Cube = Cube;
		SV.CubeGUID = CubeGUID;
		SV.Float = Float;
		
		SV.Input = Input;
		SV.ColorComponent = ColorComponent;
		SV.RGBAMasks = RGBAMasks;
		SV.MaskColorComponent = MaskColorComponent;
		SV.UseInput = UseInput;
		SV.Hidden = Hidden;
		SV.Editing = Editing;
		SV.NoSlider = NoSlider;
		SV.NoArrows = NoArrows;
		SV.NoBox = NoBox;
		SV.ShowNumbers = ShowNumbers;
		SV.EditingPopup = EditingPopup;
		SV.ObjField = ObjField;
		SV.ObjFieldOn = ObjFieldOn;
		SV.RealObj = RealObj;
		SV.RealSelected = RealSelected;
		
		SV.Type = Type;
		SV.LastUsedRect = LastUsedRect;
		SV.Names = Names;
		SV.HiddenNames = HiddenNames;
		SV.CodeNames = CodeNames;
		SV.Descriptions = Descriptions;
		
		
		SV.ImagePaths = ImagePaths;
		SV.Images = Images;
		SV.On = On;
		
		SV.TypeDispL = TypeDispL;
		SV.TypeDispLasdasdas = TypeDispLasdasdas;//?
		
		SV.Range0 = Range0;
		SV.Range1 = Range1;
		
		SV.CType = CType;
		SV.ObjectUpdater = ObjectUpdater;
		
		SV.NoInputs = NoInputs;
		
		SV.Name = Name;
		SV.LabelOffset = LabelOffset;
		
		if (SV.CType==Types.ObjectArray){
			SV.HookIntoObjects();
		}
		return SV;
	}
	public void TakeMeta(ShaderVar In){
		Names = In.Names;
		HiddenNames = In.HiddenNames;
		CodeNames = In.CodeNames;
		Descriptions = In.Descriptions;
		
		ImagePaths = In.ImagePaths;
		Images = In.Images;
		
		TypeDispL = In.TypeDispL;
		
		Range0 = In.Range0;
		Range1 = In.Range1;
		VecIsColor = In.VecIsColor;
		
		CType = In.CType;
		string OldObjectUpdater = ObjectUpdater;
		ObjectUpdater = In.ObjectUpdater;
		if (CType==Types.ObjectArray&&OldObjectUpdater!=ObjectUpdater){
			HookIntoObjects();
		}
		NoInputs = In.NoInputs;
		Name = In.Name;
	}
	public void TakeValues(ShaderVar In){
		Type = In.Type;
		Vector = In.Vector.Copy();
		Image = In.Image;
		ImageGUID = In.ImageGUID;
		Cube = In.Cube;
		CubeGUID = In.CubeGUID;
		Float = In.Float;
		Input = In.Input;
		On = In.On;
	}
	public void WarningReset(){
		WarningTitle = "";
		WarningMessage = "";
		WarningOption1 = "";
		WarningOption2 = "";
		WarningOption3 = "";		
	}
	/*public void SetToMasks(ShaderLayerList SLL, int AllowRGB){
		if (ShaderSandwich.Instance!=null&&ShaderSandwich.Instance.OpenShader!=null){
			if (ObjFieldObject==null)ObjFieldObject=new List<object>();
			if (ObjFieldImage==null)ObjFieldImage=new List<Texture2D>();
			if (ObjFieldEnabled==null)ObjFieldEnabled=new List<bool>();
			ObjFieldObject.Clear();
			ObjFieldImage.Clear();
			ObjFieldEnabled.Clear();
			bool Enab = true;
			foreach(ShaderLayerList SLL2 in ShaderSandwich.Instance.OpenShader.ShaderLayersMasks){
				if (SLL2==SLL)
				Enab = false;
				ObjFieldObject.Add(SLL2);
				ObjFieldImage.Add(SLL2.GetIcon());
				ObjFieldEnabled.Add(!(!Enab||((SLL2.EndTag.Text.Length!=4&&RGBAMasks))||(!LightingMasks&&SLL2.IsLighting.On)||(LightingMasks&&!SLL2.IsLighting.On)));
			}
		}
	}*/
	/*
			ShaderVar SV = new ShaderVar();
		SV.Vector = Vector;
		SV.Image = Image;
		SV.ImageGUID = ImageGUID;
		SV.Cube = Cube;
		SV.CubeGUID = CubeGUID;
		SV.Float = Float;
		SV.VecIsColor = VecIsColor;
		SV.Input = Input;
		SV.UseInput = UseInput;
		SV.Editing = Editing;
		SV.EditingPopup = EditingPopup;
		SV.ObjField = ObjField;
		SV.ObjFieldOn = ObjFieldOn;
		SV.Type = Type;
		SV.LastUsedRect = LastUsedRect;
		SV.Names = Names;
		SV.HiddenNames = HiddenNames;
		SV.CodeNames = CodeNames;
		SV.Descriptions = Descriptions;
		
		
		SV.ImagePaths = ImagePaths;
		SV.Images = Images;
		SV.On = On;
		
		SV.TypeDispL = TypeDispL;
		
		SV.Range0 = Range0;
		SV.Range1 = Range1;
		
		SV.CType = CType;
		SV.ObjectUpdater = ObjectUpdater;
		
		SV.NoInputs = NoInputs;
		
		SV.Name = Name;
		return SV;*/
}
}