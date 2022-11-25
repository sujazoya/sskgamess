#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#define PRE_MORESTUPIDUNITYDEPRECATIONS
#endif
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using ElectronicMind;
using ElectronicMind.Sandwich;
namespace ElectronicMind{
public enum RectDir{Normal,Diag, Bottom,Right,Middle,MiddleTop};

static public class ElectronicMindStudiosInterfaceUtils{
	static public void TrackMouseStuff(){
		mouse.TrackMouseStuff();
	}
	static public void Label(Rect r,string Text,int S){
		SetLabelSize(S);
		
		GUI.skin.label.wordWrap = true;
		GUI.Label(r,Text);
	}
	static public void SetLabelSize(int S){
		GUI.skin.label.fontSize = GetFontSize(S);
	}
	static public int GetFontSize(int S){
		return (int)((S*((float)InterfaceSize/12f)));
	}
	
	static public Rect Rect2(RectDir dir,float x,float y,float width, float height)
	{
		if (dir==RectDir.Middle)
		return new Rect(x-width/2f,y-height/2f,width,height);	
		if (dir==RectDir.MiddleTop)
		return new Rect(x-width/2f,y,width,height);	
		if (dir==RectDir.Diag)
		return new Rect(x-width,y-height,width,height);		
		if (dir==RectDir.Bottom)
		return new Rect(x,y-height,width,height);		
		if (dir==RectDir.Right)
		return new Rect(x-width,y,width,height);
		
		return new Rect(x,y,width,height);	
	}
	
	static public bool MouseDownIn(Rect rect){
		if (Event.current.type == EventType.MouseDown&&(rect.Contains(Event.current.mousePosition))){
		Event.current.Use();
		return true;
		}
		return false;
	}
	static public bool MouseUpIn(Rect rect){
		if (Event.current.type == EventType.MouseUp&&(rect.Contains(Event.current.mousePosition))){
		Event.current.Use();
		return true;
		}
		return false;
	}
	static public bool MouseDownIn(Rect rect,bool Eat){
		if (Event.current.type == EventType.MouseDown&&(rect.Contains(Event.current.mousePosition))){
		if (Eat==true)
		Event.current.Use();
		return true;
		}
		return false;
	}
	static public bool MouseUpIn(Rect rect,bool Eat){
		if (Event.current.type == EventType.MouseUp&&(rect.Contains(Event.current.mousePosition))){
			if (Eat==true)
				Event.current.Use();
			return true;
		}
		return false;
	}
	static public bool MouseHoldIn(Rect rect,bool Eat){
		if (mouse.MouseDrag&&(rect.Contains(Event.current.mousePosition))){
			if (Eat==true)
				Event.current.Use();
			return true;
		}
		return false;
	}
	
	static public void AddProSkin(Vector2 WinSize){
		AddProSkin_Real(true,WinSize);
	}
	
	static public float BaseInterfaceVal;
	static public Color InterfaceColor;
	static public Color BaseInterfaceColor;
	static public Color BaseInterfaceColorComp;
	static public Color BaseInterfaceColorComp2;
	static public int InterfaceSize;
	
	static public GUISkin SSSkin{
		get{
			if (SSSkin_Real==null)
				SSSkin_Real = EditorGUIUtility.Load("ElectronicMindStudios/Shared/Interface/MainSkin.guiskin") as GUISkin;
			return SSSkin_Real;
		}
		set{}
	}
	static public GUISkin SCSkin{
		get{
			if (SCSkin_Real==null)
				SCSkin_Real = EditorGUIUtility.Load("ElectronicMindStudios/Shared/Interface/UtilGuiSkin.guiskin") as GUISkin;
			return SCSkin_Real;
		}
		set{}
	}
	static public Texture2D searchIcon{
		get{
			if (searchIcon_Real==null)
				searchIcon_Real = EditorGUIUtility.Load("ElectronicMindStudios/Shared/Interface/Icons/SearchStart.png") as Texture2D;
			return searchIcon_Real;
		}
		set{}
	}
	static public GUISkin SSSkin_Real;
	static public GUISkin SCSkin_Real;
	static public GUISkin oldskin;
	static public Texture2D searchIcon_Real;
	
	static public Color EditorLabelNormal;
	static public Color EditorLabelActive;
	static public Color EditorLabelFocused;
	static public Color EditorLabelHover;
	
	static public Color EditorTextFieldNormal;
	static public Color EditorTextFieldActive;
	static public Color EditorTextFieldFocused;
	static public Color EditorTextFieldHover;
	
	
	static public Color EditorHelpBoxNormal;
	
	static public GUIStyle EditorLabel;
	static public GUIStyle OldEditorLabel;
	static public GUIStyle EditorFloatInput;
	static public GUIStyle EditorPopup;
	
	
	
	static public GUIStyle ButtonNoBorder;
	static public GUIStyle BoxNoBorder;
	static public GUIStyle ButtonImage;
	
	static public bool HasCleanedUp = true;
	
	
	static public bool LightInterface = false;
	static public void Defocus(){
		GUI.FocusControl("");
	}
	[NonSerialized]static public bool ProSkinHasBeenUsedOnce = false;
	static public void AddProSkin_Real(bool FI,Vector2 WinSize){
		//if (SSSkin==null)
		//	SSSkin = EditorGUIUtility.Load("ElectronicMindStudios/Shared/Interface/MainSkin.guiskin") as GUISkin;
		//if (SCSkin==null)
		//	SCSkin = EditorGUIUtility.Load("ElectronicMindStudios/Shared/Interface/UtilGuiSkin.guiskin") as GUISkin;
		if(Event.current.type==EventType.Repaint||!ProSkinHasBeenUsedOnce){
			oldskin = GUI.skin;
			GUI.skin = SSSkin;
			//InterfaceBackgroundColorDuckups = 0;
			//InterfaceColorDuckups = 0;
			InterfaceColor = new Color(
				EditorPrefs.GetFloat("ElectronicMindStudiosInterfaceColor_R", 0f),
				EditorPrefs.GetFloat("ElectronicMindStudiosInterfaceColor_G", 39f/255f),
				EditorPrefs.GetFloat("ElectronicMindStudiosInterfaceColor_B", 51f/255f),
				EditorPrefs.GetFloat("ElectronicMindStudiosInterfaceColor_A", 1f)
			);//002733FF
			float Val = GetVal(InterfaceColor);
			LightInterface = Val>=0.7f;
			//HCYColor BaseInterfaceColorHSL = new HCYColor(InterfaceColor);
			//BaseInterfaceColorHSL.c = 1f;
			//BaseInterfaceColorHSL.y = 0.5f;
			BaseInterfaceColor = SetVal(InterfaceColor,1f);
			BaseInterfaceColorComp = SetHue(BaseInterfaceColor,GetHue(BaseInterfaceColor)+0.5f);//SetVal(new Color(1f-BaseInterfaceColor.r,1f-BaseInterfaceColor.g,1f-BaseInterfaceColor.b,1f),1f);
			BaseInterfaceColorComp2 = BaseInterfaceColor;//new Color(1f-BaseInterfaceColor.r,1f-BaseInterfaceColor.g,1f-BaseInterfaceColor.b,1f);
			BaseInterfaceColorComp2 = SetVal(SetHue(BaseInterfaceColorComp,GetHue(BaseInterfaceColorComp)+0.333f),1f);
			
			/*BaseInterfaceColorHSL.h += 0.333f;
			BaseInterfaceColorComp = (Color)BaseInterfaceColorHSL;//SetHue(BaseInterfaceColor,GetHue(BaseInterfaceColor)+0.3333f);
			BaseInterfaceColorHSL.h += 0.333f;
			BaseInterfaceColorComp2 = (Color)BaseInterfaceColorHSL;//SetHue(BaseInterfaceColor,GetHue(BaseInterfaceColor)+0.6666f);*/
			
			InterfaceSize = EditorPrefs.GetInt("ElectronicMindStudiosInterfaceSize",10);
			
			Color TextColor = new Color(0.8f,0.8f,0.8f,1f);
			Color TextColor2 = new Color(0.8f,0.8f,0.8f,1f);
			Color TextColorA = new Color(1f,1f,1f,1f);

			Color BackgroundColor;
			if (Val<0.5f)
				BackgroundColor = new Color(0.6f*Val,0.6f*Val,0.6f*Val,1);
			else
				BackgroundColor = new Color(1f*Val,1f*Val,1f*Val,1);
			
			
			
			SCSkin.window.onNormal.textColor = BaseInterfaceColor;
			
			SCSkin.window.fontSize = InterfaceSize;
			GUI.skin.label.fontSize = InterfaceSize;
			GUI.skin.box.fontSize = InterfaceSize;

			GUI.color = BackgroundColor;
			if (LightInterface)
				GUI.color = new Color(BackgroundColor.r*0.76f,BackgroundColor.g*0.76f,BackgroundColor.b*0.76f,1f);
			
			BaseInterfaceVal = GUI.color.r;
			
			GUI.DrawTexture( new Rect(0,0,WinSize.x,WinSize.y), EditorGUIUtility.whiteTexture );	
			GUI.color = new Color(1f,1f,1f,1f);
			
			if (Val<0.7f){
				GUI.contentColor = new Color(1f,1f,1f,1f);
			}else{
				TextColor = new Color(0f,0f,0f,1f);
				TextColor2 = new Color(0f,0f,0f,1f);
				TextColorA = new Color(0f,0f,0f,1f);
			}
			
			SCSkin.window.normal.textColor = TextColor;
			SCSkin.window.hover.textColor = TextColor;
			SCSkin.window.active.textColor = TextColor;
			//else
			//	GUI.contentColor = new Color(0f,0f,0f,1f);
			
			GUI.backgroundColor = new Color(1f*Val,1f*Val,1f*Val,1f);
			
			
			
			GUI.skin.button.normal.textColor = TextColor;
			GUI.skin.button.active.textColor = TextColorA;
			GUI.skin.label.normal.textColor = TextColor;
			GUI.skin.box.normal.textColor = TextColorA;
			
			GUI.skin.textField.normal.textColor = TextColor2;
			GUI.skin.textField.active.textColor = TextColor2;
			GUI.skin.textField.focused.textColor = TextColor2;
			GUI.skin.textField.wordWrap = false;
			
			GUI.skin.settings.cursorColor = TextColor2;
			
			GUI.skin.textArea.normal.textColor = TextColor2;
			GUI.skin.textArea.active.textColor = TextColor2;
			GUI.skin.textArea.focused.textColor = TextColor2;
			
			GUI.skin.GetStyle("ButtonLeft").normal.textColor = TextColor2;
			GUI.skin.GetStyle("ButtonLeft").active.textColor = TextColor2;
			GUI.skin.GetStyle("ButtonLeft").focused.textColor = TextColor2;
			GUI.skin.GetStyle("ButtonLeft").onFocused.textColor = BaseInterfaceColor;
			GUI.skin.GetStyle("ButtonLeft").onNormal.textColor = BaseInterfaceColor;
			GUI.skin.GetStyle("ButtonLeft").onActive.textColor = BaseInterfaceColor;
			
			GUI.skin.GetStyle("ButtonRight").normal.textColor = TextColor2;
			GUI.skin.GetStyle("ButtonRight").active.textColor = TextColor2;
			GUI.skin.GetStyle("ButtonRight").focused.textColor = TextColor2;
			GUI.skin.GetStyle("ButtonRight").onFocused.textColor = BaseInterfaceColor;
			GUI.skin.GetStyle("ButtonRight").onNormal.textColor = BaseInterfaceColor;
			GUI.skin.GetStyle("ButtonRight").onActive.textColor = BaseInterfaceColor;
			
			GUI.skin.GetStyle("ButtonMid").normal.textColor = TextColor2;
			GUI.skin.GetStyle("ButtonMid").active.textColor = TextColor2;
			GUI.skin.GetStyle("ButtonMid").focused.textColor = TextColor2;
			GUI.skin.GetStyle("ButtonMid").onFocused.textColor = BaseInterfaceColor;
			GUI.skin.GetStyle("ButtonMid").onNormal.textColor = BaseInterfaceColor;
			GUI.skin.GetStyle("ButtonMid").onActive.textColor = BaseInterfaceColor;

			GUI.skin.GetStyle("MiniPopup").normal.textColor = TextColor2;
			GUI.skin.GetStyle("MiniPopup").active.textColor = TextColor2;
			GUI.skin.GetStyle("MiniPopup").focused.textColor = TextColor2;
			
			if (HasCleanedUp){
				EditorTextFieldNormal = EditorStyles.numberField.normal.textColor;
				EditorTextFieldActive = EditorStyles.numberField.active.textColor;
				EditorTextFieldFocused = EditorStyles.numberField.focused.textColor;
				EditorTextFieldHover = EditorStyles.numberField.hover.textColor;
			}
			
			//EditorStyles.numberField.normal.textColor = BaseInterfaceColor;
			//EditorStyles.numberField.active.textColor = BaseInterfaceColor;
			//EditorStyles.numberField.focused.textColor = BaseInterfaceColor;
			//EditorStyles.numberField.hover.textColor = BaseInterfaceColor;
			
			if (EditorFloatInput==null&&FI==true){
				EditorFloatInput = new GUIStyle(EditorStyles.numberField);
					EditorFloatInput.wordWrap = false;
			}
					EditorFloatInput.normal.textColor = TextColor2;
					EditorFloatInput.active.textColor = TextColor2;
					EditorFloatInput.focused.textColor = TextColor2;
					EditorFloatInput.hover.textColor = TextColor2;
					EditorFloatInput.onNormal.textColor = TextColor2;
					EditorFloatInput.onActive.textColor = TextColor2;
					EditorFloatInput.onFocused.textColor = TextColor2;
					EditorFloatInput.onHover.textColor = TextColor2;
			

			if (EditorPopup==null&&FI==true){
				EditorPopup = new GUIStyle(EditorStyles.popup);
			}
					EditorPopup.normal.textColor = TextColor2;
					EditorPopup.active.textColor = TextColor2;
					EditorPopup.focused.textColor = TextColor2;
					EditorPopup.hover.textColor = TextColor2;
			//if (EditorLabel==null&&FI==true){
				EditorLabel = EditorStyles.label;//new GUIStyle(EditorStyles.label);
				if (HasCleanedUp){
					EditorLabelNormal = EditorLabel.normal.textColor;
					EditorLabelActive = EditorLabel.active.textColor;
					EditorLabelFocused = EditorLabel.focused.textColor;
					EditorLabelHover = EditorLabel.hover.textColor;
				}
					
					EditorStyles.label.normal.textColor = TextColor2;
					EditorStyles.label.active.textColor = BaseInterfaceColor;
					EditorStyles.label.focused.textColor = BaseInterfaceColor;
					EditorStyles.label.hover.textColor = TextColor2;
				//OldEditorLabel = EditorStyles.label;
				//EditorStyles.label = EditorLabel;
			//}
			GUI.skin.GetStyle("Toolbar").normal.textColor = TextColor2;
			GUI.skin.GetStyle("Toolbar").active.textColor = TextColor2;
			GUI.skin.GetStyle("Toolbar").focused.textColor = TextColor2;
			
			
			GUIStyle stf = GUI.skin.GetStyle("SearchTextField");
			if (LightInterface){
				GUIStyle stf2 = oldskin.GetStyle("SearchTextField");
				GUI.skin.GetStyle("SearchTextField").normal.background = stf2.normal.background;
				GUI.skin.GetStyle("SearchTextField").active.background = stf2.active.background;
				GUI.skin.GetStyle("SearchTextField").hover.background = stf2.hover.background;
				GUI.skin.GetStyle("SearchTextField").focused.background = stf2.focused.background;
			}else{
				stf.normal.background = searchIcon;
				stf.active.background = searchIcon;
				stf.hover.background = searchIcon;
				stf.focused.background = searchIcon;
			}
			
			stf.normal.textColor = TextColor2;
			stf.active.textColor = TextColor2;
			stf.hover.textColor = TextColor2;
			stf.focused.textColor = TextColor2;
			
			GUI.skin.GetStyle("ProjectBrowserGridLabel").normal.textColor = TextColor2;
			
			if (HasCleanedUp){
				EditorHelpBoxNormal = EditorStyles.helpBox.normal.textColor;
			}
			EditorStyles.helpBox.normal.textColor = TextColor2;
			
			/*GUI.skin.label.fontSize = InterfaceSize;
			GUI.skin.box.fontSize = InterfaceSize;
			
			GUI.skin.button.normal.textColor = TextColor;
			GUI.skin.button.active.textColor = TextColorA;
			GUI.skin.label.normal.textColor = TextColor;
			GUI.skin.box.normal.textColor = TextColorA;*/
			if (ButtonNoBorder!=null){
				ButtonNoBorder.fontSize = InterfaceSize;
				ButtonNoBorder.normal.textColor = TextColor;
				ButtonNoBorder.active.textColor = TextColorA;
			}
			if (BoxNoBorder!=null){
				BoxNoBorder.fontSize = InterfaceSize;
				BoxNoBorder.normal.textColor = TextColor;
				BoxNoBorder.active.textColor = TextColorA;
			}
			if (ButtonImage!=null){
				ButtonImage.fontSize = InterfaceSize;
				ButtonImage.normal.textColor = TextColor;
				ButtonImage.active.textColor = TextColorA;
			}
			ProSkinHasBeenUsedOnce = true;
		}
		
		if (!ProSkinHasBeenUsedOnce){
			ButtonNoBorder=null;
			BoxNoBorder=null;
			ButtonImage=null;
		}
		
		if (ButtonNoBorder==null){
			ButtonNoBorder = new GUIStyle(GUI.skin.button);
			ButtonNoBorder.padding = new RectOffset(0,0,0,0);
			ButtonNoBorder.margin = new RectOffset(0,0,0,0);
		}
		if (BoxNoBorder==null){
			if (ShaderSandwich.Instance!=null&&ShaderSandwich.Instance.CurInterfaceStyle == InterfaceStyle.Modern)
				BoxNoBorder = new GUIStyle(GUI.skin.box);
			else
				BoxNoBorder = new GUIStyle(GUI.skin.button);
			BoxNoBorder.padding = new RectOffset(0,0,0,0);
			BoxNoBorder.margin = new RectOffset(0,0,0,0);
		}
		if (ButtonImage==null){
			ButtonImage = new GUIStyle(GUI.skin.button);
			ButtonImage.stretchHeight = true;
			ButtonImage.fixedHeight = 0;
			ButtonImage.stretchWidth = true;
			ButtonImage.fixedWidth = 0;
		}
		ProSkinHasBeenUsedOnce = true;
		HasCleanedUp = false;
	}
	static public void EndProSkin(){
		if (HasCleanedUp==false){
		EditorStyles.label.normal.textColor = EditorLabelNormal;//new Color(0,0,0,1);
		EditorStyles.label.active.textColor = EditorLabelActive;//new Color(0,0,0,1);
		EditorStyles.label.focused.textColor = EditorLabelFocused;//new Color(0,0,0,1);
		EditorStyles.label.hover.textColor = EditorLabelHover;//new Color(0,0,0,1);
		
		//EditorStyles.numberField.normal.textColor = EditorTextFieldNormal;
		//EditorStyles.numberField.active.textColor = EditorTextFieldActive;
		//EditorStyles.numberField.focused.textColor = EditorTextFieldFocused;
		//EditorStyles.numberField.hover.textColor = EditorTextFieldHover;
		
		EditorStyles.helpBox.normal.textColor = EditorHelpBoxNormal;
		}
		
		GUI.skin = oldskin;
		//HasCleanedUp = true;
	}
	
	public static void RGBToHSV(Color col,out float Hue, out float Sat, out float Val){
		#if PRE_MORESTUPIDUNITYDEPRECATIONS
			EditorGUIUtility.RGBToHSV(col,out Hue,out Sat,out Val);
		#else
			Color.RGBToHSV(col,out Hue,out Sat,out Val);
		#endif
	}
	public static Color HSVToRGB(float Hue, float Sat, float Val){
		#if PRE_MORESTUPIDUNITYDEPRECATIONS
			return EditorGUIUtility.HSVToRGB(Hue,Sat,Val);
		#else
			return Color.HSVToRGB(Hue,Sat,Val);
		#endif
	}
	public static Color SetHue(Color col,float Hue){
		float hue = 0f;
		float sat = 0f;
		float val = 0f;
		RGBToHSV(col, out hue, out sat, out val);
		return HSVToRGB(Hue-Mathf.Floor(Hue),sat,val);
	}
	public static float GetHue(Color col){
		float hue = 0f;
		float sat = 0f;
		float val = 0f;
		RGBToHSV(col, out hue, out sat, out val);
		return hue;
	}
	public static Color SetVal(Color col,float Val){
		float hue = 0f;
		float sat = 0f;
		float val = 0f;
		RGBToHSV(col, out hue, out sat, out val);
		return HSVToRGB(hue,sat,Val);
	}
	public static float GetVal(Color col){
		float hue = 0f;
		float sat = 0f;
		float val = 0f;
		RGBToHSV(col, out hue, out sat, out val);
		return val;
	}
	public static float GetSat(Color col){
		float hue = 0f;
		float sat = 0f;
		float val = 0f;
		RGBToHSV(col, out hue, out sat, out val);
		return sat;
	}
	public static Color SetSat(Color col,float Sat){
		float hue = 0f;
		float sat = 0f;
		float val = 0f;
		RGBToHSV(col, out hue, out sat, out val);
		return HSVToRGB(hue,Sat,val);
	}
	
	static public void BoldText(){
			GUI.skin.label.fontStyle = FontStyle.Bold;
	}
	static public void UnboldText(){
			GUI.skin.label.fontStyle = FontStyle.Normal;
	}
	static public ElectronicMindStudiosWindowInterfaceUtils mouse = new ElectronicMindStudiosWindowInterfaceUtils();
	static public bool Toggle(Rect position, bool toggle){
		SetGUIColor(BaseInterfaceColor);
		SetGUIBackgroundColor(Color.white);
		toggle = EditorGUI.Toggle(position,toggle);
		ResetGUIBackgroundColor();
		ResetGUIColor();
		return toggle;
	}
	static public float Slider(Rect position, string label, float value, float leftValue, float rightValue){
		SetGUIColor(Color.white);
		SetGUIBackgroundColor(Color.white);
			//142x20
			GUI.Label(position, label);
			//5padding
		ResetGUIBackgroundColor();
		ResetGUIColor();
		
		int TextSpace = 100;//142 for the same as Unity
		position.x+=TextSpace+5;
		position.width-=TextSpace+5+50+5;
		EditorGUI.BeginChangeCheck();
		value = GUI.HorizontalSlider(position,value,leftValue,rightValue);
		if (EditorGUI.EndChangeCheck())
			GUIUtility.keyboardControl = -1;

		float backupValue = value;
		if (leftValue>rightValue){
			value = (rightValue-(value-leftValue));
			float blerg = rightValue;
			rightValue = leftValue;
			leftValue = blerg;
		}
		if (position.Contains(Event.current.mousePosition)||(mouse.MouseDrag&&position.Contains(mouse.DragStartPos-RectPos))){
			SetGUIColor(BaseInterfaceColor);
			if (Event.current.type==EventType.Repaint)
				GUI.DrawTexture(new Rect(position.x+((position.width-8)*((Mathf.Min(rightValue,Mathf.Max(leftValue,value))-leftValue)/(rightValue-leftValue)))-8,position.y-2,24,24),GUI.skin.GetStyle("CustomThumb").hover.background);
			ResetGUIColor();
		}
		if (mouse.MouseDrag&&position.Contains(mouse.DragStartPos-RectPos))
			SetGUIColor(new Color(1,1,1,0.8f));
		else
			SetGUIColor(new Color(1,1,1,0.5f));
		if (Event.current.type==EventType.Repaint)
			GUI.DrawTexture(new Rect(position.x+((position.width-8)*((Mathf.Min(rightValue,Mathf.Max(leftValue,value))-leftValue)/(rightValue-leftValue)))-8,position.y-2,24,24),GUI.skin.GetStyle("CustomThumb").normal.background);
		ResetGUIColor();
		
		position.x+=position.width+5;
		position.width = 50;
		
		value = EditorGUI.FloatField(position,value,"textfield");
		return backupValue;
	}
	static public float Slider(Rect position, float value, float leftValue, float rightValue){
		EditorGUI.BeginChangeCheck();
		value = GUI.HorizontalSlider(position,value,leftValue,rightValue);
		if (EditorGUI.EndChangeCheck())
			GUIUtility.keyboardControl = -1;
		
		float backupValue = value;
		if (leftValue>rightValue){
			value = (rightValue-(value-leftValue));
			float blerg = rightValue;
			rightValue = leftValue;
			leftValue = blerg;
		}
///TODO: Fix the mouse positiony stuff (relative vs absolute)
		if (position.Contains(Event.current.mousePosition)||(mouse.MouseDrag&&position.Contains(mouse.DragStartPos-RectPos))){
			SetGUIColor(BaseInterfaceColor*GUI.color);
			if (Event.current.type==EventType.Repaint)
				GUI.DrawTexture(new Rect(position.x+((position.width-8)*((Mathf.Min(rightValue,Mathf.Max(leftValue,value))-leftValue)/(rightValue-leftValue)))-8,position.y-2,24,24),GUI.skin.GetStyle("CustomThumb").hover.background);
			ResetGUIColor();
		}
		if (mouse.MouseDrag&&position.Contains(mouse.DragStartPos-RectPos))
			SetGUIColor(new Color(1,1,1,0.8f)*GUI.color);
		else
			SetGUIColor(new Color(1,1,1,0.5f)*GUI.color);
		if (Event.current.type==EventType.Repaint)
			GUI.DrawTexture(new Rect(position.x+((position.width-8)*((Mathf.Min(rightValue,Mathf.Max(leftValue,value))-leftValue)/(rightValue-leftValue)))-8,position.y-2,24,24),GUI.skin.GetStyle("CustomThumb").normal.background);
		ResetGUIColor();
		return backupValue;
	}
	//static public int InterfaceColorDuckups = 0;
	//static public int InterfaceBackgroundColorDuckups = 0;
	
	static public Color oldGUIColor;
	
	static public void SetGUIColor(Color a){
		oldGUIColor = GUI.color;
		GUI.color = a;
		//InterfaceColorDuckups+=1;
		//if (InterfaceColorDuckups!=1)
		//Debug.Log("Awwww scheeeeeiiit!" + InterfaceColorDuckups.ToString());
	}
	static public void ResetGUIColor(){
		GUI.color = oldGUIColor;
		//InterfaceColorDuckups-=1;
		//if (InterfaceColorDuckups!=0)
		//Debug.Log("Awwww hell naawwh!" + InterfaceColorDuckups.ToString());
	}	
	static public Color oldGUIBackgroundColor;
	
	static public void SetGUIBackgroundColor(Color a){
		oldGUIBackgroundColor = GUI.backgroundColor;
		GUI.backgroundColor = a;
		//InterfaceBackgroundColorDuckups+=1;
		//if (InterfaceBackgroundColorDuckups!=1)
		//Debug.Log("Awwww scheeeeeiiit!" + InterfaceBackgroundColorDuckups.ToString());
	}
	static public void ResetGUIBackgroundColor(){
		GUI.backgroundColor = oldGUIBackgroundColor;
		//InterfaceBackgroundColorDuckups-=1;
		//if (InterfaceBackgroundColorDuckups!=0)
		//Debug.Log("Awwww hell naawwh!" + InterfaceBackgroundColorDuckups.ToString());
	}
	static public void VerticalScrollbar(Rect WindowSize, Vector2 ScrollArea, Rect ContentSize){
		VerticalScrollbar(WindowSize,ScrollArea,ContentSize,false);
	}
	static public void VerticalScrollbar(Rect WindowSize, Vector2 ScrollArea, Rect ContentSize,bool Force){
		if (LightInterface)
		return;
		if (ContentSize.height>WindowSize.height||Force){
			SetGUIColor(new Color(1f,1f,1f,1f));
			float Ratio = ((WindowSize.height-32f-20f)/ContentSize.height);
			float Ratio2 = (WindowSize.height-32f-20f)/ContentSize.height;
			
			Rect ScrollAreaRect = new Rect(WindowSize.x+WindowSize.width-16,WindowSize.y,16,WindowSize.height);
			Rect ScrollThumbRect = new Rect(WindowSize.x+WindowSize.width-15,WindowSize.y+16+(Ratio2*ScrollArea.y),16,Mathf.Min(WindowSize.height-32,Ratio*(float)(WindowSize.height)+20));
			if (Event.current.type==EventType.Repaint){
				//Debug.Log(RectPos);
				//Debug.Log(mouse.DragStartPos-RectPos);
				//Debug.Log(ScrollAreaRect);
				if (mouse.MouseDrag&&ScrollAreaRect.Contains(mouse.DragStartPos-RectPos)){
					SetGUIBackgroundColor(SetSat(BaseInterfaceColor,0.8f));
					SCSkin.verticalScrollbarThumb.Draw(ScrollThumbRect, true, true, true, false);
				}
				else if (ScrollThumbRect.Contains(Event.current.mousePosition)){
					SetGUIBackgroundColor(SetSat(BaseInterfaceColor,0f));
					SCSkin.verticalScrollbarThumb.Draw(ScrollThumbRect, false, false, true, false);
					
					ResetGUIColor();
					ResetGUIBackgroundColor();
					SetGUIBackgroundColor(SetSat(BaseInterfaceColor,0.8f));
					SetGUIColor(new Color(1f,1f,1f,0.3f));
					SCSkin.verticalScrollbarThumb.Draw(ScrollThumbRect, false, false, true, false);
				}
				else{
					SetGUIBackgroundColor(SetSat(BaseInterfaceColor,0f));
					SCSkin.verticalScrollbarThumb.Draw(ScrollThumbRect, false, false, true, false);
				}
				ResetGUIBackgroundColor();
			}
			ResetGUIColor();
		}
	}
	
	static public string[] OldTooltip = new string[]{"",""};
	static public string[] Tooltip = new string[]{"",""};
	static public Vector2[] TooltipPos = new Vector2[]{new Vector2(0,0),new Vector2(0,0)};
	static public float[] TooltipAlpha = new float[]{0f,0f};
	static public double[] TooltipLastUpdate = new double[]{0f,0f};
	static public void MakeTooltip(int ID,Rect rect,string tool){
		if (rect.Contains(Event.current.mousePosition))
			Tooltip[ID] = tool;
	}
	static public void DrawTooltip(int ID,Vector2 WinSize){
		if (Event.current.type==EventType.Repaint){
			GUI.color = new Color(1,1,1,Mathf.Max(0,TooltipAlpha[ID]));
			GUI.backgroundColor = new Color(1,1,1,Mathf.Max(0,TooltipAlpha[ID]));
			if (OldTooltip[ID]!=Tooltip[ID]){
				TooltipAlpha[ID] = -5f;
			}
			if (ID==2){
				//Debug.Log(ID);
				//Debug.Log(Tooltip[ID]);
				//Debug.Log(TooltipAlpha[ID]);
				//TooltipAlpha[ID] = 1;
			}
			OldTooltip[ID] = Tooltip[ID];
			
				TooltipPos[ID] = new Vector2(Event.current.mousePosition.x,Event.current.mousePosition.y);
				float delta = 300*((float)((EditorApplication.timeSinceStartup-TooltipLastUpdate[ID])));
//				Debug.Log(delta);
				TooltipAlpha[ID] += ((Tooltip[ID]=="")?-0.03f:0.03f)*delta;
				TooltipLastUpdate[ID] = EditorApplication.timeSinceStartup;
				TooltipAlpha[ID] = Mathf.Max(Mathf.Min(TooltipAlpha[ID],1f),-5f);
			
			Vector2 MinSize = GUI.skin.GetStyle("Tooltip").CalcSize(new GUIContent(Tooltip[ID]));
			Rect MinSize2 = new Rect(TooltipPos[ID].x,TooltipPos[ID].y-MinSize.y,MinSize.x,MinSize.y);
			//GUI.Box(MinSize2,"","Tooltip");
			if (WinSize.x<MinSize2.x+MinSize2.width&&(MinSize2.x-MinSize2.width+20>0)){
				MinSize2.x-=MinSize2.width;
			}
			GUI.Label(MinSize2,Tooltip[ID],"Tooltip");
			Tooltip[ID] = "";
		}
	}
	static public string StringLimit(string str, int lim){
		if (str.Length>lim){
			str = str.Substring(0,lim-3);
			str += "…";
		}
		return str;
	}
	static public string StringLimit(string str, GUIStyle style, float lim){
		string retStr = "";
		bool OldWordWrap = style.wordWrap;
		style.wordWrap = false;
		GUIContent asd = new GUIContent("…");
		float asdgtrds = style.CalcSize(asd).x+10;
		for(int i = 0;i<str.Length;i++){
			asd.text = retStr;
			if (style.CalcSize(asd).x+asdgtrds<lim)
				retStr+=str[i];
			else
				break;
		}
		if (str!=retStr)
		retStr += "…";
		style.wordWrap = OldWordWrap;
		return retStr;
	}
	
	static public List<Rect> Rects = new List<Rect>();
	static public Vector2 RectPos = new Vector2(0,0);
	static public void BeginGroup(Rect rect, GUIStyle GS){
		GUI.BeginGroup(rect,GS);
		RectPos.x += rect.x;
		RectPos.y += rect.y;
		Rects.Add(rect);
	}
	static public void BeginGroup(Rect rect){
		GUI.BeginGroup(rect);
		RectPos.x += rect.x;
		RectPos.y += rect.y;
		Rects.Add(rect);
	}
	static public void EndGroup(){
		if (Rects.Count>0){
			GUI.EndGroup();
			RectPos.x -= Rects[Rects.Count-1].x;
			RectPos.y -= Rects[Rects.Count-1].y;
			Rects.RemoveAt(Rects.Count-1);
		}
	}
	static public Vector2 BeginScrollView(Rect rect,Vector2 vec,Rect rect2,bool bo1,bool bo2){
		vec = GUI.BeginScrollView(rect,vec,rect2,bo1,bo2);
		Rects.Add(new Rect(vec.x,vec.y,rect.x,rect.y));
		RectPos -= vec;
		RectPos.x += rect.x;
		RectPos.y += rect.y;
		return vec;
	}
	static public void EndScrollView(){
		if (Rects.Count>0){
			GUI.EndScrollView();
			RectPos.x += Rects[Rects.Count-1].x;
			RectPos.y += Rects[Rects.Count-1].y;
			RectPos.x -= Rects[Rects.Count-1].width;
			RectPos.y -= Rects[Rects.Count-1].height;
			Rects.RemoveAt(Rects.Count-1);
		}
	}	
	static public Rect GetGroupRect(){
		float x = 0;
		float y = 0;
		float w = 0;
		float h = 0;
		foreach(Rect rect in Rects){
			x+=rect.x;
			y+=rect.y;
			w=rect.width;
			h=rect.height;
		}
		return new Rect(x,y,w,h);
	}
	static public Vector2 GetGroupVector(){
		float x = 0;
		float y = 0;
		foreach(Rect rect in Rects){
			x+=rect.x;
			y+=rect.y;
		}
		return new Vector2(x,y);
	}
}
}