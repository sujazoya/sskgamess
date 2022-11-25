using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;
using SU = ElectronicMind.Sandwich.ShaderUtil;
using UnityEngine.Rendering;
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
[System.Serializable]
public class ShaderSource  : ScriptableObject, ISerializationCallbackReceiver{
	public SSDictionary CodeBits = new SSDictionary();
	public SBDictionary CodeEndings = new SBDictionary();
	public SSDictionary CodeComments = new SSDictionary();
	public SSDictionary CustomCodeBits = new SSDictionary();
	public List<string> CodeOrder = new List<string>();
	
	public void OnBeforeSerialize(){
		CodeBits.Serialize();
		CodeEndings.Serialize();
		CodeComments.Serialize();
		CustomCodeBits.Serialize();
	}
	public void OnAfterDeserialize(){
		CodeBits.Deserialize();
		CodeEndings.Deserialize();
		CodeComments.Deserialize();
		CustomCodeBits.Deserialize();
	}
	
	public void ResetBits(){
		CodeBits = new SSDictionary();
		CodeOrder = new List<string>();
		//CodeOrder = new List<string>();
	}
	public void AddBit(string ID,string Val){
		if (!CodeBits.D.ContainsKey(ID)){
			CodeOrder.Add(ID);
		}
		CodeBits.D[ID] = Val;//.Replace(";","");
		//CodeEndings.D[ID] = Val.IndexOf(";")>-1;
	}
	public void AddBit(string Val){
		if (!CodeBits.D.ContainsKey(Val)){
			CodeOrder.Add(Val);
		}
		CodeBits.D[Val] = Val;//.Replace(";","");
		CodeEndings.D[Val] = Val.IndexOf(";")>-1;
	}
	public void AddBit(string ID,string Val,string Comment){
		if (!CodeBits.D.ContainsKey(ID)){
			CodeOrder.Add(ID);
		}
		CodeBits.D[ID] = Val+" //"+Comment;//.Replace(";","");
		//CodeComments.D[ID] = Comment;
		//CodeEndings.D[ID] = Val.IndexOf(";")>-1;
	}
	public void SBracket(){
		CodeOrder.Add("{");
	}
	public void EBracket(){
		CodeOrder.Add("}");
	}
	public string GetBit(string ID){
		if (ID!="{"&&ID!="}"){
			string End="";
			//if (CodeComments.D.ContainsKey(ID))
			//End = " //"+CodeComments.D[ID];
			
			//if (CodeEndings.D[ID])
			//End = ";"+End;
			
			if (CustomCodeBits.D.ContainsKey(ID))
				return CustomCodeBits.D[ID]+End;
			if (CodeBits.D.ContainsKey(ID))
				return CodeBits.D[ID]+End;
			return "";
		}
		return ID;
	}
	override public string ToString(){
		string Total = "";
		int Depth = 0;
		foreach(string ID in CodeOrder){
			if (ID=="}")
				Depth--;
			
			Total+=ShaderUtil.Pad(Depth)+GetBit(ID)+"\n";
			
			if (ID=="{")
				Depth++;
		}
		return Total;
	}
}
}