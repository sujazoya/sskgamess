using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using SU = ElectronicMind.Sandwich.ShaderUtil;
using System.Xml.Serialization;
using System.Runtime.Serialization;
//using System.Xml;
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
[System.Serializable]
public class SSEDistanceFade : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEDistanceFade";
		Name = "Color/Distance Fade";
		
		Function = "";
		Line = "";
		LinePre = "";
	
		Inputs = new SVDictionary();
		Inputs["Fadeout Limit Min"] = new ShaderVar("Fadeout Limit Min",0f);
		Inputs["Fadeout Limit Max"] = new ShaderVar("Fadeout Limit Max",10f);
		Inputs["Fadeout Start"] = new ShaderVar("Fadeout Start",3f);
		Inputs["Fadeout End"] = new ShaderVar("Fadeout End",5f);
		Inputs["Fadeout Color"] = new ShaderVar("Fadeout Color",new Color(0f,0f,0f,0f));
		Inputs["Fadeout Invert"] = new ShaderVar("Fadeout Invert",true);
		
		UseAlpha.Float=2;
	}
	public override bool Draw(Rect rect){
		rect.x += 5;
		rect.width -= 6;
		float YOffset = rect.y;
		bool changed = false;
			Inputs["Fadeout Start"].NoSlider = true;
			Inputs["Fadeout Start"].NoArrows = true;
			
			Inputs["Fadeout End"].NoSlider = true;
			Inputs["Fadeout End"].NoArrows = true;
			
			Inputs["Fadeout Limit Min"].NoSlider = true;
			Inputs["Fadeout Limit Min"].NoArrows = true;
			
			Inputs["Fadeout Limit Max"].NoSlider = true;
			Inputs["Fadeout Limit Max"].NoArrows = true;
			
			Inputs["Fadeout Start"].LabelOffset = 70;
			Inputs["Fadeout End"].LabelOffset = 70;
			
			//EditorGUI.BeginChangeCheck();
			changed |= Inputs["Fadeout Start"].Draw(new Rect(rect.x,YOffset,(rect.width-10)/2,17),"Start");
			changed |= Inputs["Fadeout End"].Draw(new Rect(rect.x+(rect.width-10)/2+10,YOffset,(rect.width-10)/2,17),"End");YOffset+=20;

			EditorGUI.BeginChangeCheck();
			Color oldc = GUI.backgroundColor;
			GUI.backgroundColor = Color.Lerp(GUI.backgroundColor,new Color(1f,1f,1f,1f),0.4f);
			EditorGUI.MinMaxSlider(new Rect(rect.x+40,YOffset,rect.width-80-4,20), ref Inputs["Fadeout Start"].Float, ref Inputs["Fadeout End"].Float, Inputs["Fadeout Limit Min"].Float, Inputs["Fadeout Limit Max"].Float);
			GUI.backgroundColor = oldc;
			if (EditorGUI.EndChangeCheck()){
				GUI.changed = true;
				EditorGUIUtility.editingTextField = false;
				Inputs["Fadeout Start"].Float = Mathf.Max(Inputs["Fadeout Limit Min"].Float,Mathf.Round(Inputs["Fadeout Start"].Float*100)/100);
				Inputs["Fadeout End"].Float = Mathf.Min(Inputs["Fadeout Limit Max"].Float,Mathf.Round(Inputs["Fadeout End"].Float*100)/100);
				Inputs["Fadeout Start"].UpdateToVar();
				Inputs["Fadeout End"].UpdateToVar();
				changed = true;
			}
			
			changed |= Inputs["Fadeout Limit Min"].Draw(new Rect(rect.x,YOffset,35,17),"");
			changed |= Inputs["Fadeout Limit Max"].Draw(new Rect(rect.x+rect.width-35,YOffset,35,17),"");
			//Inputs["Fadeout Limit Max"].Float = EditorGUI.FloatField(new Rect(180,240+YOffset,40,20),Inputs["Fadeout Limit Max"].Float);
			YOffset+=20;
		changed |= Inputs["Fadeout Color"].Draw(new Rect(rect.x,YOffset,rect.width,17),"Color ");	YOffset+=20;
		changed |= Inputs["Fadeout Invert"].Draw(new Rect(rect.x,YOffset,rect.width,17),"Invert ");	
		return changed;
	}
	public static float Lerp(float a, float b, float t){
		return a+((b-a)*t);
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["Fadeout Color"].Vector.r == 0f&&Inputs["Fadeout Color"].Vector.g == 0f&&Inputs["Fadeout Color"].Vector.b == 0f&&Inputs["Fadeout Color"].CanBeBaked()){
			if (Inputs["Fadeout Invert"].On)
				return "("+Line+"*"+"(1-saturate((vd.screenPos.z-("+Inputs["Fadeout Start"].Get()+"))/("+Inputs["Fadeout End"].Get()+"-"+Inputs["Fadeout Start"].Get()+")))"+")";
			else
				return "("+Line+"*"+"(saturate((vd.screenPos.z-("+Inputs["Fadeout Start"].Get()+"))/("+Inputs["Fadeout End"].Get()+"-"+Inputs["Fadeout Start"].Get()+")))"+")";
		}
		else{
			if (Inputs["Fadeout Invert"].On)
				return "lerp("+Line+","+Inputs["Fadeout Color"].Get()+".rgb,"+"(saturate((vd.screenPos.z-("+Inputs["Fadeout Start"].Get()+"))/("+Inputs["Fadeout End"].Get()+"-"+Inputs["Fadeout Start"].Get()+")))"+")";
			else
				return "lerp("+Inputs["Fadeout Color"].Get()+".rgb,"+Line+","+"(saturate((vd.screenPos.z-("+Inputs["Fadeout Start"].Get()+"))/("+Inputs["Fadeout End"].Get()+"-"+Inputs["Fadeout Start"].Get()+")))"+")";
		}
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["Fadeout Color"].Vector.r == 0f&&Inputs["Fadeout Color"].Vector.g == 0f&&Inputs["Fadeout Color"].Vector.b == 0f&&Inputs["Fadeout Color"].Vector.a == 0f&&Inputs["Fadeout Color"].CanBeBaked()){
			if (Inputs["Fadeout Invert"].On)
				return "("+Line+"*"+"(1-saturate((vd.screenPos.z-("+Inputs["Fadeout Start"].Get()+"))/("+Inputs["Fadeout End"].Get()+"-"+Inputs["Fadeout Start"].Get()+")))"+")";
			else
				return "("+Line+"*"+"(saturate((vd.screenPos.z-("+Inputs["Fadeout Start"].Get()+"))/("+Inputs["Fadeout End"].Get()+"-"+Inputs["Fadeout Start"].Get()+")))"+")";
		}
		else{
			if (Inputs["Fadeout Invert"].On)
				return "lerp("+Line+","+Inputs["Fadeout Color"].Get()+","+"(saturate((vd.screenPos.z-("+Inputs["Fadeout Start"].Get()+"))/("+Inputs["Fadeout End"].Get()+"-"+Inputs["Fadeout Start"].Get()+")))"+")";
			else
				return "lerp("+Inputs["Fadeout Color"].Get()+","+Line+","+"(saturate((vd.screenPos.z-("+Inputs["Fadeout Start"].Get()+"))/("+Inputs["Fadeout End"].Get()+"-"+Inputs["Fadeout Start"].Get()+")))"+")";
		}
	}
	public override string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (Inputs["Fadeout Color"].Vector.a == 0f&&Inputs["Fadeout Color"].CanBeBaked()){
			if (Inputs["Fadeout Invert"].On)
				return "("+Line+"*"+"(1-saturate((vd.screenPos.z-("+Inputs["Fadeout Start"].Get()+"))/("+Inputs["Fadeout End"].Get()+"-"+Inputs["Fadeout Start"].Get()+")))"+")";
			else
				return "("+Line+"*"+"(saturate((vd.screenPos.z-("+Inputs["Fadeout Start"].Get()+"))/("+Inputs["Fadeout End"].Get()+"-"+Inputs["Fadeout Start"].Get()+")))"+")";
		}
		else{
			if (Inputs["Fadeout Invert"].On)
				return "lerp("+Line+","+Inputs["Fadeout Color"].Get()+".a,"+"(saturate((vd.screenPos.z-("+Inputs["Fadeout Start"].Get()+"))/("+Inputs["Fadeout End"].Get()+"-"+Inputs["Fadeout Start"].Get()+")))"+")";
			else
				return "lerp("+Inputs["Fadeout Color"].Get()+".a,"+Line+","+"(saturate((vd.screenPos.z-("+Inputs["Fadeout Start"].Get()+"))/("+Inputs["Fadeout End"].Get()+"-"+Inputs["Fadeout Start"].Get()+")))"+")";
		}
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		return OldColor;
	}
}
}