//Turns out Unity's undo handling system was a lot better than I thought... still I'll keep this here in case it decides to turn on me XD
//...Couldn't make it undo mask selection, guess it's time to do it manually XD. I suck...
//Actually I can't distinguish between Undo or Redo thanks to Unity being weird...hfff fine I'll try again the normal way...
/*
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
public enum ShaderHistoryType{
	LayerAdd,
	LayerDelete,
	LayerMove,
	EffectAdd,
	EffectDelete,
	EffectMove,
	MaskAdd,
	MaskMove,
	MaskDelete,
	MaskSettings,
	ShaderVarChange,
	IngredientAdd,
	IngredientMove,
	IngredientDelete,
	InputAdd,
	InputMove,
	InputDelete,
	InputChange
};
public class ShaderHistory : ScriptableObject{
	public int GroupID;
	
	public ShaderHistoryType Type;
	public ShaderLayer SL;
	public ShaderLayerList PSLL;
	public int PSLPos;
	public int PSLLPos;
	public ShaderLayerList NSLL;
	public int NSLPos;
	public int NSLLPos;
	
	public ShaderEffect SE;
	public int PSEPos;
	public int NSEPos;
	
	public ShaderVar SV;
	public ShaderVar PSV;
	public ShaderVar NSV;
	
	public ShaderIngredient SING;
	public bool PSurfaceIngredient;
	public bool NSurfaceIngredient;
	public int PSINGPos;
	public int NSINGPos;
	
	public ShaderInput SI;
	public ShaderInput PSI;
	public ShaderInput NSI;
	public int PSIPos;
	public int NSIPos;
	
	static public ShaderHistory AddLayer(ShaderLayer SL){
		ShaderHistory SH = (ShaderHistory)ScriptableObject.CreateInstance<ShaderHistory>();
		SH.Type = ShaderHistoryType.LayerAdd;
		SH.SL = SL;
		SH.NSLL = SL.Parent;
		return SH;
	}
	static public ShaderHistory DeleteLayer(ShaderLayer SL){
		ShaderHistory SH = (ShaderHistory)ScriptableObject.CreateInstance<ShaderHistory>();
		SH.Type = ShaderHistoryType.LayerDelete;
		SH.SL = SL;
		SH.NSLL = SL.Parent;
		return SH;
	}
	public override string ToString(){
		return Type.ToString();
	}
	public void Undo(){
		//Debug.Log(Type);
		if (Type == ShaderHistoryType.LayerAdd){
			NSLL.SLs.Remove(SL);
			NSLL.Inputs-=1;
			//Debug.Log("Idunnaundo");
		}
	}
	public void Redo(){
		if (Type == ShaderHistoryType.LayerAdd){
			NSLL.Add(SL);
			NSLL.Inputs+=1;
			//Debug.Log("Idunnaundo");
		}
	}
}

*/
