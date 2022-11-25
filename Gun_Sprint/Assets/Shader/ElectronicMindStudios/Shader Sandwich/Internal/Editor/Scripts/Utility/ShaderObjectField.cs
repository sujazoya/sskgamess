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
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
#pragma warning disable 0618
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
[System.Serializable]
public class ShaderObjectField : EditorWindow {

	public static ShaderObjectField Instance {
	get { return (ShaderObjectField)GetWindow(typeof (ShaderObjectField),false,"",false); }
	}
	public GUISkin SSSkin;
	public Vector2 Scroll;
	
	public string SearchText = "";
	public int Selected = -1;
	public List<object> ObjFieldObject;
	public List<Texture> ObjFieldImage;
	public List<bool> ObjFieldEnabled;
	public ShaderVar Caller = null;
	public bool SomethingOtherThanAMask = false;
	public bool NullIsAllowed = true;
	static public ShaderObjectField Show(ShaderVar SV,string Name,List<object> Objects,List<Texture> Images,List<bool> Bools,int Sel){
		ShaderObjectField windowG = (ShaderObjectField)ScriptableObject.CreateInstance(typeof(ShaderObjectField));//new ShaderObjectField();
		windowG.ShowAuxWindow();
		windowG.Caller = SV;
		//windowG.ShowUtility();
		windowG.title = Name.Replace("LayerType","Layer Type");
		windowG.ObjFieldObject = Objects;
		windowG.ObjFieldImage = Images;
		windowG.ObjFieldEnabled = Bools;
		
		/*int X = -1;
		foreach(object obj in windowG.ObjFieldObject){
			X+=1;
			if (obj==Sel){
			windowG.Selected = X;
			Debug.Log(obj);
			}
		}*/
		windowG.Selected = Sel;
		return windowG;
	}
	//[MenuItem ("Window/ObjFieldTest")]
	public static void Init () {
		//ShaderObjectField windowG = (ShaderObjectField)EditorWindow.GetWindow (typeof (ShaderObjectField));
		ShaderObjectField windowG = new ShaderObjectField();
		windowG.ShowAuxWindow();
		//windowG.wantsMouseMove = true;
		//windowG.minSize = new Vector2(750,360);
	}
		//Color TextColor = new Color(0.8f,0.8f,0.8f,1f);
		//Color TextColorA = new Color(1f,1f,1f,1f);
		//Color BackgroundColor = new Color(0.18f,0.18f,0.18f,1);
	public float Size = 0.66625f;
	bool HasntFocussedYet = true;
	int LastFrameHeight = 500;
	public bool sRGB = false;
    void OnGUI() {
		if (Event.current.type == EventType.ValidateCommand){
			if ((Event.current.commandName == "UndoRedoPerformed")){
				GUI.changed = true;
				Repaint();
			}
		}
		//ShaderObjectField windowG = (ShaderObjectField)EditorWindow.GetWindow (typeof (ShaderObjectField),true,"",false);
		EMindGUI.TrackMouseStuff();
		Vector2 WinSize = new Vector2(position.width,position.height);
		EMindGUI.AddProSkin(WinSize);
		
		GUI.Box(new Rect(0,0,WinSize.x,30),"");
		//GUI.backgroundColor = new Color(1f,1f,1f,1f);
		EMindGUI.SetGUIBackgroundColor(new Color(1f,1f,1f,1f));
		GUI.SetNextControlName("Search Field");
		SearchText = GUI.TextField(new Rect(10,8,WinSize.x-20,18),SearchText,GUI.skin.GetStyle("SearchTextField"));
		
		if (HasntFocussedYet){
			GUI.FocusControl("Search Field");
			HasntFocussedYet = false;
		}
		
		EMindGUI.ResetGUIBackgroundColor();
		//GUI.backgroundColor = new Color(0.25f,0.25f,0.25f,1f);
		if (Event.current.type == EventType.Repaint)
		GUI.skin.GetStyle("ObjectPickerTab").Draw(new Rect(0,30,56,1), false, true, true,false);
	
		Size = EMindGUI.Slider(new Rect(WinSize.x-5-55,32+16-20,55,20),Size,0f,1f);
		int IconSize = (int)Mathf.Lerp(28,92,Size);//0.65625,0.671875
		int IconPad = (int)Mathf.Lerp(14,22,Size);
		int IconTotal = IconSize+IconPad;
		
		GUI.Label(new Rect(6,30,56,16),"Assets");
		Scroll = GUI.BeginScrollView(new Rect(0,32+16,WinSize.x,WinSize.y-32-24-16),Scroll,new Rect(0,0,WinSize.x-15,LastFrameHeight),false,false);
		
		int X = -1;
		int VisX = -1;
		int VisY = 0;
		
		
		
		
		//ObjectPickerGroupHeader
		//Draw(Rect position, bool isHover, bool isActive, bool on, bool hasKeyboardFocus);
		if (Event.current.type==EventType.Repaint)
			Selected = Caller.Selected;
		if (ObjFieldObject!=null){
			//Process Selections to groups
			List<List<int>> Groups = new List<List<int>>();
			List<string> GroupNames = new List<string>();
			
			int i = 0;
			foreach(object obj in ObjFieldObject){
				if (obj==null)
					continue;
				string n = obj.ToString();
				if (SearchText!=""&&!(n.IndexOf(SearchText,StringComparison.OrdinalIgnoreCase)>=0)){
					i++;
					continue;
				}
				if (n.Contains("/")){
					string substr = n.Substring(0,n.IndexOf("/"));
					if (!GroupNames.Contains(substr)){
						GroupNames.Add(substr);
						Groups.Add(new List<int>());
					}
					Groups[GroupNames.IndexOf(substr)].Add(i);
				}else{
					if (!GroupNames.Contains("")){
						GroupNames.Insert(0,"");
						Groups.Insert(0,new List<int>());
					}
					Groups[0].Add(i);
				}
				i++;
			}
			//foreach(object obj in ObjFieldObject){
			//	if (!obj.ToString().Contains("/"))
			//		Groups[0].Add(i);
			//	i++;
			//}
			
			foreach(List<int> group in Groups){
				X+=1;
				//foreach(object obj in ObjFieldObject){
				Color asdasdasd = GUI.skin.label.normal.textColor;
				Color asdasdasd2 = GUI.skin.GetStyle("ProjectBrowserGridLabel").normal.textColor;
				if (GroupNames[X]!=""){
					Color oldg = GUI.color;
					GUI.color = EMindGUI.BaseInterfaceColor;
						GUI.skin.label.normal.textColor = Color.white;
						GUI.Label(new Rect(20,20+VisY,WinSize.x,20),GroupNames[X]);
						GUI.skin.label.normal.textColor = asdasdasd;
						VisY += 20;
						GUI.DrawTexture( new Rect(20,VisY+2+EMindGUI.InterfaceSize,WinSize.x,1f), EditorGUIUtility.whiteTexture );	
					GUI.color = oldg;
				}
				foreach(int item in group){
					object obj = ObjFieldObject[item];
					
					//if (SearchText!=""&&!(obj.ToString().IndexOf(SearchText,StringComparison.OrdinalIgnoreCase)>=0))
					//	continue;
					
					VisX+=1;
					if (20+VisX*IconTotal+IconTotal>WinSize.x){
						VisX = 0;
						VisY += IconTotal;
					}
					//Debug.Log(obj.ToString()+","+ObjFieldEnabled[X].ToString());
					if (ObjFieldEnabled[item]==false)
						GUI.enabled = false;
					if (Event.current.type == EventType.Repaint){
						if (Selected==item){
							EMindGUI.SetGUIBackgroundColor(EMindGUI.BaseInterfaceColor);
							GUI.skin.GetStyle("ProjectBrowserTextureIconDropShadow").Draw(new Rect(20+VisX*IconTotal,20+VisY,IconSize,IconSize),false,false,true,false);
							GUI.skin.GetStyle("ProjectBrowserGridLabel").Draw(new Rect(20+VisX*IconTotal,20+IconSize+VisY+3,IconSize,20),false,false,true,true);
							EMindGUI.ResetGUIBackgroundColor();
						}
						else{
							GUI.Box(new Rect(20+VisX*IconTotal,20+VisY,IconSize,IconSize),"","ProjectBrowserTextureIconDropShadow");
						}
						if (sRGB)
							GUI.DrawTexture(new Rect(20+VisX*IconTotal,20+VisY,IconSize,IconSize),ObjFieldImage[item],ScaleMode.StretchToFill,false);
						else{
							//ShaderUtil.GPUGUIMat.Material.SetFloat("Transparent",UseAlpha.On?1f:0f);
							ShaderUtil.GPUGUIMat.Material.SetFloat("Transparent",0f);
							Graphics.DrawTexture(new Rect(20+VisX*IconTotal,20+VisY,IconSize,IconSize),ObjFieldImage[item],ShaderUtil.GPUGUIMat.Material);
							//GUI.DrawTexture(new Rect(20+VisX*IconTotal,20+VisY,IconSize,IconSize),ObjFieldImage[item],false);
						}
						//GUI.Label(new Rect(20+VisX*IconTotal,IconSize+20+VisY,IconSize,20),obj.ToString().Substring(Math.Max(0,obj.ToString().IndexOf("/")+1)),"ProjectBrowserGridLabel");
						GUI.skin.GetStyle("ProjectBrowserGridLabel").normal.textColor = Color.white;
						GUI.Label(new Rect(20+VisX*IconTotal,IconSize+20+VisY+3,IconSize,20),obj.ToString().Substring(Math.Max(0,obj.ToString().IndexOf("/")+1)),"ProjectBrowserGridLabel");
						GUI.skin.GetStyle("ProjectBrowserGridLabel").normal.textColor = asdasdasd2;
					}
					else{
						//if (GUI.Button(new Rect(20+X*100,20,100,100),""))
						if (EMindGUI.MouseDownIn(new Rect(20+VisX*IconTotal-IconPad/2,20+VisY,IconTotal,IconSize+20))){
							UnityEngine.Object o = ShaderSandwich.Instance.OpenShader;
							if (Caller.Parent!=null)
								o = Caller.Parent;
							Undo.RegisterCompleteObjectUndo(o,"Selected Mask");
							GUI.changed = true;
							Caller.ForceGUIChange = true;
							ShaderSandwich.Instance.Repaint();
							Selected = item;
							Caller.Selected = Selected;
							//Debug.Log(Event.current.clickCount);
							if (Event.current.clickCount==2)
							Close();
						}
					}
					GUI.enabled = true;
				}
				VisX = -1;
				VisY += IconTotal;
			}
		}
		else{
			GUI.Label(new Rect(100,100,200,400),"Sorry, there's been a weird bug :(. Try saving and reloading this file, and if that doesn't work please send a bug report :).");
		}
			//if (Event.current.type != EventType.Repaint){
			if (EMindGUI.MouseDownIn(new Rect(0,0,WinSize.x,WinSize.y))&&NullIsAllowed){
				//if (GUI.Button(new Rect(0,0,WinSize.x,800),""))
					UnityEngine.Object o = ShaderSandwich.Instance.OpenShader;
					if (Caller.Parent!=null)
						o = Caller.Parent;
					Undo.RegisterCompleteObjectUndo(o,"Unselected Mask");
					Selected = -1;
					Caller.Selected = Selected;
					GUI.changed = true;
					Caller.ForceGUIChange = true;
					ShaderSandwich.Instance.Repaint();
					//Debug.Log(Event.current.clickCount);
					if (Event.current.clickCount==2)
						Close();	
			}		
		
		GUI.EndScrollView();
		EMindGUI.VerticalScrollbar(new Rect(0,32+16,WinSize.x,WinSize.y-32-24-16),Scroll,new Rect(0,0,WinSize.x-15,LastFrameHeight));//Rect WindowSize, Vector2 ScrollArea, Rect ContentSize)
		LastFrameHeight = IconSize+20+(VisY-IconTotal)+3+20;
		GUI.Box(new Rect(0,WinSize.y-24,WinSize.x,24),"");
		//if (!SomethingOtherThanAMask&&Caller.Selected!=-1&&Caller.Obj!=null){
		if (Caller.Obj as ShaderLayerList !=null && Caller.Selected!=-1&&Caller.Obj!=null){
			if (((ShaderLayerList)Caller.Obj).EndTag.Text.Length>1){
				EditorGUI.BeginChangeCheck();
				int col = GUI.SelectionGrid(new Rect(0,WinSize.y-24,150,24),Caller.MaskColorComponent,new string[]{"R","G","B","A"},4);
				if (EditorGUI.EndChangeCheck()){
					UnityEngine.Object o = ShaderSandwich.Instance.OpenShader;
					if (Caller.Parent!=null)
						o = Caller.Parent;
					Undo.RegisterCompleteObjectUndo(o,"Selected Mask");
					Caller.MaskColorComponent = col;
					GUI.changed = true;
					Caller.ForceGUIChange = true;
				}
			}
		}
		EMindGUI.EndProSkin();
    }
	void Nothing(){}
}
}