using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using SU = ElectronicMind.Sandwich.ShaderUtil;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using System.Diagnostics;


//delegate ShaderColor ShaderLayerTypePreview(ShaderLayerType SLT,Vector2 UV);
//delegate int ShaderLayerTypeGetDimensions(ShaderGenerate SG,ShaderLayerType SLT, ShaderLayer SL);
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
[System.Serializable]
public class ShaderLayerType : ShaderPlugin{
	
	//public void Register(Dictionary<string,Delegate> ShaderLayerTypePreviewList, Dictionary<string,Delegate> ShaderLayerTypeGetDimensionsList){
	//	ShaderLayerTypePreviewList[TypeS] =  new ShaderLayerTypePreview(Preview);
	//	ShaderLayerTypeGetDimensionsList[TypeS] = new ShaderLayerTypeGetDimensions(GetDimensions);
	//}
	virtual public int GetDimensions(SVDictionary Data, ShaderLayer SL){return 0;}
	virtual public string Generate(SVDictionary Data, ShaderLayer SL, ShaderData SD, string Map){return "";}
	//virtual public List<ShaderVar> Activate(){return null;}
}
}