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
using System.Text;

using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
[System.Serializable]
public class SSECAntiAliasing : ShaderEffect{
	public override void Activate(){
		TypeS = "SSECAntiAliasing";
		Name = "Blur/Anti Aliasing";//+UnityEngine.Random.value.ToString();

		Inputs = new SVDictionary();
		Inputs.Add(new ShaderVar("Quality",5f));

		Inputs["Quality"].Range0 = 3;
		Inputs["Quality"].Range1 = 20;
		Inputs["Quality"].Float = Mathf.Max(3,Inputs["Quality"].Float);
		Inputs["Quality"].Float = Mathf.Round(Inputs["Quality"].Float);	
		shader = "Hidden/ShaderSandwich/Previews/Effects/AntiAliasing";
	}
	//public static void SetUsed(ShaderData SD){
	//	SG.UsedScreenPos = true;
	//}
	/*public string GetAddBlurString(ShaderData SD,ShaderLayer SL,float Weight, int Effect,float XAdd,float YAdd){
		string XAnti = "AntiAlias.x";//IN.screenPos.z/300";
		string YAnti = "AntiAlias.y";;//"IN.screenPos.z";
		
		
		//if (Inputs[1].Input!=null)
		string XX = ((float)XAdd/Inputs["Quality"].Float).ToString()+"*"+XAnti;
		//if (Inputs[2].Input!=null)
		string YY = ((float)YAdd/Inputs["Quality"].Float).ToString()+"*"+YAnti;
		
		//if (!Inputs["Quality"].On){
		//	if (Inputs[1].Input!=null)
		//	YY = ((float)YAdd/Inputs["Quality"].Float).ToString()+"*"+XAnti;
		//}
		
		return "+("+SL.StartNewBranch(SD,SL.GCUVs(SD,XX,YY,"0"),Effect)+" * "+Weight.ToString()+")";
		//return OldColors[ShaderUtil.FlatArray(XX,YY,W,H)];	
	}*/
	public override void GPUPreviewJustBefore(SVDictionary Inputs, ShaderLayer SL, RenderTexture In, RenderTexture Out){
		_Material.SetFloat("_AlphaMode",UseAlpha.Float);
		_Material.SetFloat("Quality",Inputs["Quality"].Float);
	}
	public string GetAddBlurString(ShaderData SD,ShaderLayer SL,ShaderLayerBranch Branch,float XAdd,float YAdd){
		string XX;
		string YY;
		string XXX;
		string YYY;
		XXX = XAdd.ToString();
		YYY = YAdd.ToString();
		XX = XXX+"*AntiAlias.x";
		
		YY = YYY+"*AntiAlias.y";
		//if(Inputs["Reduce Banding"].On){
		//	XX = "dot(float2("+XX+","+YY+"),PoissonBlurRotation.xz)";
		//	YY = "dot(float2("+XX+","+YY+"),PoissonBlurRotation.yw)";
		//}
		ShaderLayerBranch NewBranch = Branch;
		return "+("+SL.StartNewBranch(SD,SL.GCUVs(SD,XX,YY,"0",true,ref NewBranch),NewBranch)+")";
		//return OldColors[ShaderUtil.FlatArray(XX,YY,W,H)];	
	}
	public Color LerpBlur(float X, float Y, Color Col1,Color Col2,Color Col3,Color Col4){
		return Col1*(1f-X)*(1f-Y) + Col2*X*(1f-Y) + Col3*(1f-X)*Y + Col4*X*Y;
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
	
		//ShaderColor OldColor = OldColors[ShaderUtil.FlatArray(X,Y,W,H)];//GetAddBlur(SE,OldColors,X,Y,W,H,0,0);
		//ShaderColor OldColor = OldColors[ShaderUtil.FlatArray(X,Y,W,H)];//GetAddBlur(SE,OldColors,X,Y,W,H,0,0);
		/*Inputs["Quality"].Float = Mathf.Round(Inputs["Quality"].Float);
		
		
		Color NewColor = OldColor;
		int AddCount = 0;
		
		if (Inputs[3].On==false){
			for(int i = (int)(-Inputs["Quality"].Float);i<=(int)(Inputs["Quality"].Float);i+=2){
			if (i!=0){NewColor+=GetAddBlur(SE,OldColors,X,Y,W,H,i,i);AddCount+=1;}}
			for(int i = (int)(-Inputs["Quality"].Float);i<=(int)(Inputs["Quality"].Float);i+=2){
			if (i!=0){NewColor+=GetAddBlur(SE,OldColors,X,Y,W,H,-i,-i);AddCount+=1;}}	
			for(int i = (int)(-Inputs["Quality"].Float);i<=(int)(Inputs["Quality"].Float);i+=2){
			if (i!=0){NewColor+=GetAddBlur(SE,OldColors,X,Y,W,H,-i,i);AddCount+=1;}}
			for(int i = (int)(-Inputs["Quality"].Float);i<=(int)(Inputs["Quality"].Float);i+=2){
			if (i!=0){NewColor+=GetAddBlur(SE,OldColors,X,Y,W,H,i,-i);AddCount+=1;}}		
		}
		for(int i = (int)(-Inputs["Quality"].Float);i<=(int)(Inputs["Quality"].Float);i++){
		if (i!=0){NewColor+=GetAddBlur(SE,OldColors,X,Y,W,H,i,0);AddCount+=1;}}
		for(int i = (int)(-Inputs["Quality"].Float);i<=(int)(Inputs["Quality"].Float);i++){
		if (i!=0){NewColor+=GetAddBlur(SE,OldColors,X,Y,W,H,0,i);AddCount+=1;}}
		
		

		//if (Inputs[2].On==false)
		NewColor/=(float)((AddCount)+1);
		//else
		//NewColor/=(Inputs[3].Float*4f)+1;
*/
		return OldColor;
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		if (!SL.IsVertex){
			StringBuilder retVal = new StringBuilder("((");
			float AddCount = 0;
			
			float[] points = SSEPoissonBlur.Poisson3Points;
			if (Inputs["Quality"].Float>0)
				points = SSEPoissonBlur.Poisson3Points;
			if (Inputs["Quality"].Float>=4)
				points = SSEPoissonBlur.Poisson4Points;
			if (Inputs["Quality"].Float>=5)
				points = SSEPoissonBlur.Poisson5Points;
			if (Inputs["Quality"].Float>=10)
				points = SSEPoissonBlur.Poisson10Points;
			if (Inputs["Quality"].Float>=15)
				points = SSEPoissonBlur.Poisson15Points;
			if (Inputs["Quality"].Float>=20)
				points = SSEPoissonBlur.Poisson20Points;
			if (Inputs["Quality"].Float>=25)
				points = SSEPoissonBlur.Poisson25Points;
			if (Inputs["Quality"].Float>=30)
				points = SSEPoissonBlur.Poisson30Points;
			if (Inputs["Quality"].Float>=40)
				points = SSEPoissonBlur.Poisson40Points;
			if (Inputs["Quality"].Float>=50)
				points = SSEPoissonBlur.Poisson50Points;
			if (Inputs["Quality"].Float>=100)
				points = SSEPoissonBlur.Poisson100Points;
			if (Inputs["Quality"].Float>=200)
				points = SSEPoissonBlur.Poisson200Points;
		
			//float TotalWeight = 0f;
			/*for( int x = (int)-Inputs["Quality"].Float; x<=Inputs["Quality"].Float;x++){
				for( int y = (int)-Inputs["Quality"].Float; y<=Inputs["Quality"].Float;y++){
					float weight = new Vector2(((float)x/Inputs["Quality"].Float),((float)y/Inputs["Quality"].Float)).magnitude;
					TotalWeight += weight;
				}
			}
			for( int x = (int)-Inputs["Quality"].Float; x<=Inputs["Quality"].Float;x++){
				for( int y = (int)-Inputs["Quality"].Float; y<=Inputs["Quality"].Float;y++){
					float weight = new Vector2(((float)x/Inputs["Quality"].Float),((float)y/Inputs["Quality"].Float)).magnitude;
					retVal+=GetAddBlurString(SD,SL,weight/TotalWeight,Effect,x,y);
					//AddCount+=1;
				}
			}*/
			
			for(int i = 0;i<points.Length;i+=2){
				float px = points[i];
				float py = points[i+1];
				retVal.Append(GetAddBlurString(SD,SL,Branch,px,py));
				AddCount++;
			}
			retVal.Append(")/"+(AddCount).ToString()+")");
			
			return retVal.ToString();
		}
		return Line;
	}
	public override string GenerateStart(ShaderData SD){
		return "half2 AntiAlias = fwidth(vd.genericTexcoord)*0.5;\n";//(vd.screenPos.z*abs((ddx(vd.screenPos.z)+ddy(vd.screenPos.z))/4/vd.screenPos.z)+(vd.screenPos.z/600));\n";
	}
}
}