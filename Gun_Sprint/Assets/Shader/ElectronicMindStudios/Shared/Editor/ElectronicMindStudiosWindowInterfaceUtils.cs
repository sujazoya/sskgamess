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
public class ElectronicMindStudiosWindowInterfaceUtils{
	public Vector2 DragStartPos = new Vector2(0,0);
	public bool MouseDown = false;
	public bool MouseDrag = false;
	
	public void TrackMouseStuff(){
		if (Event.current.type == EventType.MouseDown){
			MouseDown = true;
			MouseDrag = true;
			DragStartPos = Event.current.mousePosition;
		}
		if (Event.current.type == EventType.MouseDrag){
			if (MouseDown)
			MouseDrag = true;
		}
		if (Event.current.rawType == EventType.MouseUp){
			MouseDown = false;
			MouseDrag = false;
		}
	}
	public void TrackMouseReset(){
		if (Event.current.type == EventType.Repaint){
			//MouseDown = false;
			//MouseDrag = false;
		}
	}
}
}