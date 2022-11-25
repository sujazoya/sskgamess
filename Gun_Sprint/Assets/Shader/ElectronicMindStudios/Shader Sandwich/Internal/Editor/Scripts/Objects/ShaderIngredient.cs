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
using System.Reflection;
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
//Base class for all ingredients...pretty much made so I didn't have to copy paste all the interface code for surfaces and geometry modifiers XD
[System.Serializable]
public class ShaderIngredient : ScriptableObject{
	public ShaderVar UserName = new ShaderVar("User Name","Yummy Ingredient");
	public string _Name = "Yummy Ingredient";
	public string Name {
		get{return _Name;}
		set{_Name = value;
		UserName.Text = _Name;}
	}
	//public string MenuItem = "Yum Yums";
	public Type[] TopAnchor = null;
	public Type[] ExistencePreferred = null;
	static public string MenuItem = "Yum Yums";
	string CachedCodename = "";
	string CachedName = "";
	public string CodeName{
		get{
			if (CachedName!=_Name){
				CachedCodename = ShaderUtil.CodeName(_Name);
				CachedName = _Name;
			}
			return CachedCodename;
		}
		set{}
	}
	//public bool Enabled = true;
	public virtual bool IsUsed(){
		return Enabled.On;
	}
	public ShaderVar Enabled = new ShaderVar("Enabled",true);
	public ShaderVar ShouldLink = new ShaderVar("ShouldLink",false);
	[NonSerialized]bool _Linked = false;
	public bool Linked{
		set{}
		get{
			return ShouldLink.On;
		}
	}
	public bool Locked = false;
	public void UpdateLinked(ShaderPass SP){
		SetLinked(SP,ShouldLink.On);
	}
	public void SetLinked(ShaderPass SP, bool l){
		if (l)
			Link(SP);
		else
			Unlink();
	}
	public void Link(ShaderPass SP){
		//Debug.Log(_Linked);
		ShouldLink.On = true;
		if (!_Linked){
			_Linked = true;
			List<ShaderIngredient> SIs = new List<ShaderIngredient>();
			SIs.AddRange(SP.Surfaces);
			SIs.AddRange(SP.GeometryModifiers);
			
			FieldInfo[] myfields = this.GetType().GetFields();
			foreach(ShaderIngredient SI in SIs){
				if (SI!=this&&SI.Linked){
					//Debug.Log(SI.MenuItem);
					FieldInfo[] theirfields = SI.GetType().GetFields();
					foreach(FieldInfo myfield in myfields){
						//Debug.Log(myfield.Name);
						if (
						myfield.Name!="UserName"&&
						myfield.Name!="MixAmount"&&
						myfield.Name!="AlphaBlendMode"&&
						myfield.Name!="MixType"&&
						myfield.Name!="UseAlpha"&&
						myfield.Name!="CompareDepth"&&
						myfield.Name!="ShouldLink"&&
						myfield.Name!="UseCustomLighting"&&
						myfield.Name!="UseCustomAmbient"&&
						myfield.Name!="CustomLightingEnabled"&&
						myfield.Name!="CustomAmbientEnabled"&&
						myfield.GetValue(this) as ShaderVar!=null){
							//Debug.Log("Woo ShaderVar!");
							foreach(FieldInfo theirfield in theirfields){
								//Debug.Log(theirfield.Name);
								if (theirfield.Name==myfield.Name && theirfield.GetValue(SI) as ShaderVar!=null){
									//Debug.Log("Woo it's in both and a ShaderVar!");
									myfield.SetValue(this,theirfield.GetValue(SI));
								}
							}
						}
					}
				}
			}
		}
	}
	public void Unlink(){
		ShouldLink.On = false;
		if (_Linked){
			_Linked = false;
			FieldInfo[] fields = this.GetType().GetFields();
			foreach(FieldInfo field in fields){
				if (field.GetValue(this) as ShaderVar!=null){
					ShaderVar copy = (field.GetValue(this) as ShaderVar).Copy();
					field.SetValue(this,copy);
				}
			}
		}
	}
	
	public ShaderLayerList	SurfaceLayers;
	public ShaderVar MixAmount = new ShaderVar("Mix Amount",1f);
	public int Height = 32;
	
	public bool FirstDisplayed = false;
	
	public virtual void BaseAwake(){
		SurfaceLayers = ScriptableObject.CreateInstance<ShaderLayerList>();
		SurfaceLayers.UShaderLayerList("Surface","Surface","A generic surface component (Hopefully this is updated...)","Surface","Surface","rgba","",new Color(0.8f,0.8f,0.8f,1f));
	}
	public void Awake(){
		BaseAwake();
	}
	
	public virtual int GetHeight(float Width){
		return 0;
	}
	public virtual void RenderUI(float Width){
		
	}
	public virtual void RenderWedgeUI(ShaderPass SP, float Width){
		
	}
	public virtual Dictionary<string,string> GeneratePrecalculations(){
		return new Dictionary<string,string>();
	}
	//float4/float3/float (VertexData vd, Precalculations pc, UnityGI gi, Masks mask)
	public virtual string GenerateHeader(ShaderData SD, string surfaceInternals){
		return GenerateHeader(SD,surfaceInternals,false,"");
	}
	public virtual string GenerateHeader(ShaderData SD, string surfaceInternals,bool Call){
		return GenerateHeader(SD,surfaceInternals,Call,"");
	}
	public virtual string GenerateHeader(ShaderData SD, string surfaceInternals,bool Call,string addon){
		return "";
	}
	public virtual bool IsPremultiplied(ShaderData SD){
		return SD.UsesPremultipliedBlending;
	}
	public virtual string GenerateGlobalFunctions(ShaderData SD, List<ShaderIngredient> ingredients){
		return "";
	}
	public virtual string GenerateEffectsCode(ShaderData SD){
		string surfaceEffects = "";
		foreach(KeyValuePair<string,ShaderPlugin> Ty2 in ShaderSandwich.ShaderLayerEffectInstances){
			string Ty = Ty2.Key;
			bool IsUsed = false;
			foreach (ShaderLayerList SLL in GetMyShaderLayerLists()){
				foreach (ShaderLayer SL in SLL){
					foreach(ShaderEffect SE in SL.LayerEffects){
						if (Ty==SE.TypeS&&SE.Visible)
							IsUsed = true;
					}
				}
			}
			if (IsUsed){
				//ShaderEffect NewEffect = (ShaderEffect)ShaderEffect.CreateInstance(ShaderSandwich.ShaderLayerEffectInstances);
				//NewEffect.ShaderEffectIn(Ty);
				ShaderEffect NewEffect = Ty2.Value as ShaderEffect;
				//if (ShaderEffect.GetMethod(NewEffect.TypeS,"GenerateStart")!=null)
				//	surfaceEffects+= (string)ShaderEffect.GetMethod(NewEffect.TypeS,"GenerateStart").Invooke(null,new object[]{SD})+"\n";
				string dumbasdoorknob = NewEffect.GenerateStart(SD);
				if (dumbasdoorknob.Length>0)
					surfaceEffects+= dumbasdoorknob+"\n";
			}
		}
		return surfaceEffects;
	}
	string GenerateCodeCache = "";
	public virtual string GenerateCodeAndCache(ShaderData SD){
		GenerateCodeCache = GenerateCode(SD);
		return GenerateCodeCache;
	}
	public virtual string GenerateCodeCached(){
		return GenerateCodeCache;
	}
	public virtual string GenerateCode(ShaderData SD){
		return "";
	}
	public virtual void ConstantShaderVars(List<ShaderVar> SVs){
		//SVs.Add(UseAlpha);
		SVs.Add(UserName);
		SVs.Add(MixAmount);
		SVs.Add(ShouldLink);
		//SVs.Add(MixType);
	}
	public virtual void GetMyShaderVars(List<ShaderVar> SVs){
	}
	public List<ShaderLayerList> GetMyShaderLayerLists(){
		return GetMyShaderLayerLists(false);
	}
	public virtual List<ShaderLayerList> GetMyShaderLayerLists(bool All){
		List<ShaderLayerList> SLLs = new List<ShaderLayerList>();
		SLLs.Add(SurfaceLayers);
		return SLLs;
	}
	public void LoadLayerList(StringReader S){
		//foreach(ShaderLayerList SLL in GetMyShaderLayerLists(true)){
		//	ShaderLayerList.LoadSingle(S,SLL);
		//}
		ShaderLayerList.LoadFromGroup(S,GetMyShaderLayerLists(true));
	}
	public bool HackyForceUpdateShaderVars = false;
	static public ShaderIngredient Load(StringReader S){
		Dictionary<string,ShaderVar> SVs = null;
		ShaderIngredient SI = null;
		while(1==1){
			string Line = S.ReadLine();

			if (Line!=null){
				Line = ShaderUtil.Sanitize(Line);
				if(Line=="End Shader Ingredient")break;
				
				if (Line.Contains(":")){
					if (Line.Contains("Type(Text)")){
						ShaderVar asd = new ShaderVar("Type","ShaderIngredient");
						asd.Load(Line);
						if (ShaderSandwich.NonDumbShaderSurfaces.ContainsKey(asd.Text))
							SI = (ShaderIngredient)ScriptableObject.CreateInstance(ShaderSandwich.NonDumbShaderSurfaces[asd.Text]);
							//SI = (ShaderIngredient)Activator.CreateInstance(ShaderSandwich.NonDumbShaderSurfaces[asd.Text]);
						if (ShaderSandwich.NonDumbShaderGeometryModifiers.ContainsKey(asd.Text))
							SI = (ShaderIngredient)ScriptableObject.CreateInstance(ShaderSandwich.NonDumbShaderGeometryModifiers[asd.Text]);
						if (asd.Text=="ShaderIngredientUpdateMask")
							SI = (ShaderIngredient)ScriptableObject.CreateInstance(typeof(ShaderIngredientUpdateMask));
							//SI = (ShaderIngredient)Activator.CreateInstance(ShaderSandwich.NonDumbShaderGeometryModifiers[asd.Text]);
						if (SI!=null){
							//Debug.Log(SI.Name);
							foreach(ShaderLayerList SL in SI.GetMyShaderLayerLists(true)){
								//Debug.Log(SL);
								//Debug.Log(SL.SLs);
								SL.SLs.Clear();
							}
							List<ShaderVar> SV = new List<ShaderVar>();
							SI.ConstantShaderVars(SV);
							SI.GetMyShaderVars(SV);
							SVs = ShaderUtil.ShaderVarListToDictionary(SV);
						}
					}else{
						if (SI!=null){
							if (SI.HackyForceUpdateShaderVars){
								List<ShaderVar> SV = new List<ShaderVar>();
								SI.ConstantShaderVars(SV);
								SI.GetMyShaderVars(SV);
								SVs = ShaderUtil.ShaderVarListToDictionary(SV);
							}
							ShaderUtil.LoadLine(SVs,Line);
						}
					}
				}
				if (Line=="Begin Shader Layer List"){
					SI.LoadLayerList(S);
				}
			}
			else
			break;
		}
		return SI;
	}
	public string Save(){
		string S = "\n			Begin Shader Ingredient\n\n";
			List<ShaderVar> SV = new List<ShaderVar>();
			SV.Add(new ShaderVar("Type",GetType().Name));
			ConstantShaderVars(SV);
			GetMyShaderVars(SV);
			S += ShaderUtil.SaveList(SV,4);
			foreach(ShaderLayerList SLL in GetMyShaderLayerLists()){
				S += SLL.Save(4);
			}
			S += "\n			End Shader Ingredient\n\n";
		return S;
	}
	public ShaderIngredient Copy(){
		string asd = Save();
		return Load(new StringReader(asd));
	}
	public virtual UnityEngine.Object[] GetSerializableStuff(){
		return new UnityEngine.Object[]{this};
	}
	
	public ShaderFix MixAmountAlmostOne = null;
	public ShaderFix MixAmountAlmostZero = null;
	public virtual void GetMyPersonalPrivateForMySubStandardChildEyesOnlyFixes(ShaderBase shader, ShaderPass pass, List<ShaderFix> fixes){
		
	}
	public virtual void GetMyPersonalPrivateForMyEyesOnlyFixes(ShaderBase shader, ShaderPass pass, List<ShaderFix> fixes){
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
	
	
	
		GetMyPersonalPrivateForMySubStandardChildEyesOnlyFixes(shader, pass, fixes);
		if (MixAmount.Get()==MixAmount.Float.ToString()&&MixAmount.Float!=1f&&MixAmount.Float>0.95f){
			MixAmountAlmostOne.Message = "Set the Mix Amount to 1 (from the super close value of "+MixAmount.Float.ToString()+").";
			fixes.Add(MixAmountAlmostOne);
		}
		if (MixAmount.Get()==MixAmount.Float.ToString()&&MixAmount.Float!=0f&&MixAmount.Float<0.05f){
			MixAmountAlmostZero.Message = "Set the Mix Amount to 0 (from the super close value of "+MixAmount.Float.ToString()+").";
			fixes.Add(MixAmountAlmostZero);
		}
	}
	public virtual void GetMyFixes(ShaderBase shader,ShaderPass pass, List<ShaderFix> fixes){
		GetMyPersonalPrivateForMyEyesOnlyFixes(shader,pass, fixes);
		foreach(ShaderLayerList SLL in GetMyShaderLayerLists()){
			foreach(ShaderLayer SL in SLL.SLs){
				SL.GetMyFixes(shader,pass,this,fixes);
			}
		}
	}
}
}