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
public class ShaderIngredientUpdateMask : ShaderIngredient{///TODO; FINISH!
	public List<ShaderVar> Masks = new List<ShaderVar>();//("Mask","ListOfObjects");
	public ShaderVar MaskCount = new ShaderVar("Mask Count",0);//("Mask","ListOfObjects");
	new static public string MenuItem = "Update Masks";
	public ShaderIngredientUpdateMask(){
		Name = "";
		MenuItem = "Update Masks";
		HackyForceUpdateShaderVars = true;
		//Height = 16;
	}
	public override int GetHeight(float Width){
		return 50;
	}
	//public override void RenderUI(float Width){
		//Mask.SetToMasks(null,0);
		//Mask.Draw(new Rect(0,20,Width,20),"Mask");
		//if (Mask.Obj!=null)
		//	Name = "Update Mask: " + ((ShaderLayerList)Mask.Obj).Name.Text;
	//}
	public override string GenerateCode(ShaderData SD){
		return "";
	}
	
	public override void GetMyShaderVars(List<ShaderVar> SVs){
//		Debug.Log("GteMaskVars");
		//Debug.Log(MaskCount.Float.ToString());
		if (MaskCount.Float < Masks.Count)
			MaskCount.Float = Masks.Count;
		SVs.Add(MaskCount);
		for(int i = Masks.Count;i<MaskCount.Float;i++){
				Add(null);
		}
		
		
		SVs.AddRange(Masks);
	}
	public override List<ShaderLayerList> GetMyShaderLayerLists(bool All){
		List<ShaderLayerList> SLLs = new List<ShaderLayerList>();
		return SLLs;
	}
	public void Add(ShaderLayerList SLL){
		ShaderVar sv = new ShaderVar("Mask"+Masks.Count.ToString(),"ListOfObjects");
		//sv.SetToMasks(null,0);
		sv.HookIntoObjects();
		sv.Obj = SLL;
		Masks.Add(sv);
		MaskCount.Float = Masks.Count;
	}
	public override void RenderWedgeUI(ShaderPass SP, float Width){
		MaskCount.Float = Masks.Count;
		GUI.Box(new Rect(3,3,Width-6,32-6),"","box");
		
		Rect MixAmountPos = new Rect(0,0,Width,31);
		if (EMindGUI.MouseDownIn(MixAmountPos,false)){
			SP.selectedIngredient = this;
		}
		for(int i = Masks.Count-1;i>=0;i--){
			ShaderVar sv = Masks[i];
			//sv.SetToMasks(null,0);
			sv.HookIntoObjects();
			if (sv.Obj==null){
				Masks.Remove(sv);
			}
		}
		int x = 4;
		foreach(ShaderVar sv in Masks){
			ShaderLayerList SLL = (ShaderLayerList)sv.Obj;
			GUI.DrawTexture(new Rect(x,4,24,24),SLL.GetIcon());
			x+=24;
		}
	}
}
}