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
using System.Xml;
//This class is the ugliest class I have ever written. I should rewrite it to be like ShaderLayerTypes at least...time...
//Alright gonna do it!
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
[System.Serializable]
public class ShaderEffect : ShaderPlugin{
	public string Line = "";
	public string LinePre = "";
	
	public int CustomHeight = -1;
	[NonSerialized]public bool HandleAlpha = true;
	public ShaderVar UseAlpha = new ShaderVar("UseAlpha",0);
	public bool IsUVEffect = false;
	public bool ChangeBaseCol = false;
	public bool AutoCreated = false;
	public bool WantsFullLine = false;
	
	public string uvshader = "";
	public Shader _uvShader = null;
	[XmlIgnore]public Material _uvMaterial = null;
	
	public override void GPUPreviewJustBefore(SVDictionary Inputs, ShaderLayer SL, RenderTexture In, RenderTexture Out){
		_Material.SetFloat("_AlphaMode",UseAlpha.Float);
	}
	
	virtual public void UVGPUPreview(SVDictionary Inputs, ShaderLayer SL, RenderTexture In, RenderTexture Out){
		if (uvshader==""){
			Graphics.Blit(In,Out);
			return;
		}
		
		
		if (_uvShader==null)
			_uvShader = Shader.Find(uvshader);
			
		if (_uvShader!=null){
			if (_uvMaterial==null)
				_uvMaterial = new Material(_uvShader);
			
			//Finally update material:
				foreach(KeyValuePair<string,ShaderVar> SVN in Inputs){
					ShaderVar SV = SVN.Value;
					string name = ShaderUtil.CodeName(SVN.Key);
					if (name=="Color")
						name = "_Color";//Grrr Unity....
					if (SV.CType == Types.Vec)
						_uvMaterial.SetColor(name,(Color)SV.Vector);
					if (SV.CType == Types.Float&&!SV.NoInputs)
						_uvMaterial.SetFloat(name,SV.Float);
					if (SV.CType == Types.Texture)
						_uvMaterial.SetTexture(name,SV.ImageS());
					if (SV.CType == Types.Cubemap)
						_uvMaterial.SetTexture(name,SV.CubeS());
					if (SV.CType == Types.Type)
						_uvMaterial.SetFloat(name,SV.Type);
					if (SV.CType == Types.Toggle)
						_uvMaterial.SetFloat(name,SV.On?1f:0f);
					if (SV.CType == Types.ObjectArray){
						if (SV.Obj!=null&&SV.Obj as ShaderLayerList != null)
						_uvMaterial.SetTexture(name,(SV.Obj as ShaderLayerList).GetIcon());
					}
				}
				if (In!=null)
					_uvMaterial.SetTexture("_Previous",In);
			
			Graphics.Blit(In,Out,_uvMaterial);
		}
	}
	
	
	public virtual string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Data, string Line, ShaderLayerBranch Branch){return "";}
	public virtual string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Data, string Line, ShaderLayerBranch Branch){return "";}
	public virtual string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Data, string Line, ShaderLayerBranch Branch){return "";}
	public virtual string GenerateBase(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, string Map, ref ShaderLayerBranch Branch){return Line;}
	public virtual string GenerateStart(ShaderData SD){return "";}
	
	public virtual string GenerateMap(SVDictionary Inputs, ShaderData SD, ShaderLayer SL, string Map, ref int UVDimensions, ref int TypeDimensions){return Map;}
	
	
	public string ToStringDetailed(){
		string Args = "";
		foreach(KeyValuePair<string,ShaderVar> SV in Inputs){
			Args+=SV.Value.Save(0)+"\n";
		}
		return "Name: "+Name+"\n Function:"+Function+"\n Line:"+Line+"\n LinePre:"+LinePre+"\nArguments:\n"+Args;
	}
	public ShaderEffect(){}
	static public System.Reflection.MethodInfo GetMethod(string Ty,string S){
		//Debug.Log(Ty);
		if (Ty=="Hasn't Activated Correctly"){
			Debug.LogWarning("Hi there! Sorry, some sort of internal error happened, I'd highly recommend reloading Shader Sandwich :)");
			return null;
		}
		Type t = Type.GetType("ElectronicMind.Sandwich."+Ty);
		//Debug.Log(t.GetMethod(S)); 
		return t.GetMethod(S);
	}
	public Dictionary<string,ShaderVar> GetSaveLoadDict(){
		Dictionary<string,ShaderVar> D = new Dictionary<string,ShaderVar>();
		
		D.Add(TypeS_Real.Name,TypeS_Real);
		D.Add(IsVisible.Name,IsVisible);
		D.Add(UseAlpha.Name,UseAlpha);
		//foreach(ShaderVar SV in Inputs){
		//	D.Add(SV.Name,SV);
		//}
		Inputs.ToList().ForEach(x => D[x.Key] = x.Value);
		return D;
	}	
	public string Save(int tabs){
		string S = ShaderUtil.Tabs[tabs]+"Begin Shader Effect\n";
		S += ShaderUtil.SaveDict(GetSaveLoadDict(),tabs+1);
		S += ShaderUtil.Tabs[tabs]+"End Shader Effect\n\n"; 
		return S;
	}
public ShaderEffect Copy(){
	string asd = Save(0);
	//UnityEngine.Debug.Log(asd);
	StringReader temp = new StringReader(asd);
	temp.ReadLine();
	return Load(temp);
}
static public ShaderEffect Load(StringReader S){
	//string ShaderEffectTypeS = ShaderUtil.LoadLineExplode(ShaderUtil.Sanitize(S.ReadLine()))[1];
	string ShaderEffectTypeS = ShaderUtil.Sanitize(S.ReadLine());
	//Debug.Log(ShaderEffectTypeS);
//	Debug.Log(ShaderEffectTypeS.Split(new string[] { "#^" },StringSplitOptions.None)[0]);
	//if (ShaderEffectTypeS.Split(new string[] { "#^" },StringSplitOptions.None)[0]=="S2")
	//ShaderEffectTypeS = ShaderEffectTypeS.Split(new string[] { "#^" },StringSplitOptions.None)[2].Trim();
	//else
	//ShaderEffectTypeS = ShaderEffectTypeS.Split(new string[] { "#^" },StringSplitOptions.None)[0].Trim();
	ShaderVar temp = new ShaderVar("TypeS","");
	temp.Load(ShaderEffectTypeS);
	ShaderEffectTypeS = temp.Text;
//	Debug.Log(ShaderEffectTypeS);
	ShaderEffect SE = (ShaderEffect)ScriptableObject.CreateInstance(ShaderSandwich.ShaderLayerEffect[ShaderEffectTypeS]);
	SE.Activate();
	var D = SE.GetSaveLoadDict();
	while(1==1){
		string Line =  S.ReadLine();
		if (Line!=null){
			Line = ShaderUtil.Sanitize(Line);
			if(Line=="End Shader Effect")break;
			
			if (Line.Contains(":")){
				ShaderUtil.LoadLine(D,Line);
				
				D = SE.GetSaveLoadDict();
			}
		}
		else
		break;
	}
	return SE;
}
static public ShaderEffect LoadLegacy(StringReader S){
	string ShaderEffectTypeS = ShaderUtil.LoadLineExplode(ShaderUtil.Sanitize(S.ReadLine()))[1];
	//Debug.Log(ShaderEffectTypeS);
//	Debug.Log(ShaderEffectTypeS.Split(new string[] { "#^" },StringSplitOptions.None)[0]);
	if (ShaderEffectTypeS.Split(new string[] { "#^" },StringSplitOptions.None)[0]=="S2")
	ShaderEffectTypeS = ShaderEffectTypeS.Split(new string[] { "#^" },StringSplitOptions.None)[2].Trim();
	else
	ShaderEffectTypeS = ShaderEffectTypeS.Split(new string[] { "#^" },StringSplitOptions.None)[0].Trim();
	//Debug.Log(ShaderEffectTypeS);
	ShaderEffect SE = (ShaderEffect)ScriptableObject.CreateInstance(ShaderSandwich.ShaderLayerEffect[ShaderEffectTypeS]);
	SE.Activate();
	//SE.ShaderEffectIn(ShaderEffectTypeS);
	var D = SE.GetSaveLoadDict();
	while(1==1){
		string Line =  S.ReadLine();
		if (Line!=null){
			if(Line=="EndShaderEffect")break;
			
			if (Line.Contains("#!")){
				ShaderUtil.LoadLineLegacy(D,Line);
				
				//if (Act.GetParameters().Length==2)
				//Act.Invooke(null,new object[]{SE,false});
				//else
				//Act.Invooke(null,new object[]{SE,false,null});
				
				
				D = SE.GetSaveLoadDict();
			}
		}
		else
		break;
	}
	return SE;
}
	//public static void Activate(bool Inputs){}
	//public static Color PixelChange(Color OldColor){return OldColor;}
}
}