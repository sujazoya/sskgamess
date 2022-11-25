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
using System.Diagnostics;

[System.Serializable]
public class ShaderShaderAndMaterialBadcooodIthinkdotdotdot : ScriptableObject{
	public string shader = "";
	public Shader _Shader = null;
	[XmlIgnore]public Material _Material = null;
	
	
	public Material Material{
		get{
			if (_Material==null){
				if (_Shader==null){
					_Shader = Shader.Find(shader);
				}
				if (_Shader!=null){
					_Material = new Material(_Shader);
				}
			}
			return _Material;
		}
	}
	
	public ShaderShaderAndMaterialBadcooodIthinkdotdotdot(string s){
		shader = s;
	}
}