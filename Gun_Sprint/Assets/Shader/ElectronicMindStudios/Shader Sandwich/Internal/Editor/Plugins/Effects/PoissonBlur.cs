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
//using System.Xml;
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
[System.Serializable]
public class SSEPoissonBlur : ShaderEffect{
	public override void Activate(){
		TypeS = "SSEPoissonBlur";
		Name = "Blur/Poisson(Disc) Blur";//+UnityEngine.Random.value.ToString();
		shader = "Hidden/ShaderSandwich/Previews/Effects/PoissonBlur";
		Inputs = new SVDictionary();
		Inputs.Add(new ShaderVar("Separate",false));
		Inputs.Add(new ShaderVar("Blur X",0.01f));
		Inputs.Add(new ShaderVar("Blur Y",0.01f));
		Inputs.Add(new ShaderVar("Quality",15f));
		Inputs.Add(new ShaderVar("Reduce Banding",true));
		Inputs.Add(new ShaderVar("Animate Noise",true));
		Inputs.Add(new ShaderVar("Weighting",false));
		Inputs.Add(new ShaderVar("Factor",0.5f));
		Inputs.Add(new ShaderVar("Weighting Inverse",false));

		Inputs["Blur X"].Range0 = 0;
		Inputs["Blur X"].Range1 = 0.1f;
		Inputs["Blur Y"].Range0 = 0;
		Inputs["Blur Y"].Range1 = 0.1f;
		Inputs["Quality"].NoInputs = true;
		Inputs["Quality"].Range0 = 3;
		Inputs["Quality"].Range1 = 50;
		Inputs["Quality"].Float = Mathf.Max(3,Inputs["Quality"].Float);
		Inputs["Quality"].Float = Mathf.Round(Inputs["Quality"].Float);
		
		Inputs["Factor"].NoInputs = true;
		
		Inputs["Factor"].Range1 = 2f;
		Inputs["Factor"].Range0 = 0.5f;
		
		Function = @"
		half2 PoissonBlurRot( half2 d, float r ) {
			half2 sc = half2(sin(r),cos(r));
			return half2( dot( d, half2(sc.y, -sc.x) ), dot( d, sc.xy ) );
		}";		
	}
	public override void GPUPreviewJustBefore(SVDictionary Inputs, ShaderLayer SL, RenderTexture In, RenderTexture Out){
		_Material.SetFloat("_AlphaMode",UseAlpha.Float);
		_Material.SetFloat("Quality",Inputs["Quality"].Float);
		_Material.SetFloat("Factor",Inputs["Weighting"].On?Inputs["Factor"].Float:1f);
	}
	public ShaderColor GetAddBlur(ShaderColor[] OldColors,int X,int Y,int W,int H,int XAdd,int YAdd){
		int XX = (int)((((float)X/(float)W)+((float)XAdd*Inputs["Blur X"].Float/Inputs["Quality"].Float))*(float)W);
		int YY = (int)((((float)Y/(float)H)+((float)YAdd*Inputs["Blur Y"].Float/Inputs["Quality"].Float))*(float)H);
		return OldColors[ShaderUtil.FlatArray(XX,YY,W,H)];	
	}
	public string GetAddBlurString(ShaderData SD,ShaderLayer SL,ShaderLayerBranch Branch,float XAdd,float YAdd,float weight,string endtag){
		string XX;// = (XAdd*Inputs["Blur X"].Float).ToString();
		string YY;// = (YAdd*Inputs["Blur Y"].Float).ToString();
		//if (!Inputs["Separate"].On){
		//	YY = ((float)YAdd*Inputs["Blur X"].Float/Inputs["Quality"].Float).ToString();
		//}
		string XXX;
		string YYY;
		if(Inputs["Reduce Banding"].On){
			if (Inputs["Animate Noise"].On){
				XXX = "dot(float2("+XAdd.ToString()+","+YAdd.ToString()+"),PoissonBlurRotationAnimated.xz)";
				YYY = "dot(float2("+XAdd.ToString()+","+YAdd.ToString()+"),PoissonBlurRotationAnimated.yw)";
			}else{
				XXX = "dot(float2("+XAdd.ToString()+","+YAdd.ToString()+"),PoissonBlurRotation.xz)";
				YYY = "dot(float2("+XAdd.ToString()+","+YAdd.ToString()+"),PoissonBlurRotation.yw)";
			}
		}else{
			XXX = XAdd.ToString();
			YYY = YAdd.ToString();
		}
		//if (Inputs["Blur X"].Input!=null)
		XX = XXX+"*"+Inputs["Blur X"].Get();
		//if (Inputs["Blur Y"].Input!=null)
		
		if (!Inputs["Separate"].On){
			//if (Inputs["Blur X"].Input!=null)
			YY = YYY+"*"+Inputs["Blur X"].Get();
		}else{
			YY = YYY+"*"+Inputs["Blur Y"].Get();
		}
		//if(Inputs["Reduce Banding"].On){
		//	XX = "dot(float2("+XX+","+YY+"),PoissonBlurRotation.xz)";
		//	YY = "dot(float2("+XX+","+YY+"),PoissonBlurRotation.yw)";
		//}
		if (weight==1f)
		return "+("+SL.StartNewBranch(SD,SL.GCUVs(SD,XX,YY,"0",true,ref Branch),Branch)+endtag+")";
		else
		return "+("+SL.StartNewBranch(SD,SL.GCUVs(SD,XX,YY,"0",true,ref Branch),Branch)+endtag+") * "+weight.ToString();
		//return OldColors[ShaderUtil.FlatArray(XX,YY,W,H)];	
	}
	public override ShaderColor Preview(SVDictionary Inputs, ShaderLayer SL, Vector2 UV, ShaderColor[] OldColors, ShaderColor OldColor){
		return new ShaderColor(1f,1f,1f,1f);
	}
	public string GenerateGeneric(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch, string endtag){
		StringBuilder retVal = new StringBuilder("((");
		float AddCount = 0;
		float[] points = Poisson3Points;
		if (Inputs["Quality"].Float>0)
			points = Poisson3Points;
		if (Inputs["Quality"].Float>=4)
			points = Poisson4Points;
		if (Inputs["Quality"].Float>=5)
			points = Poisson5Points;
		if (Inputs["Quality"].Float>=10)
			points = Poisson10Points;
		if (Inputs["Quality"].Float>=15)
			points = Poisson15Points;
		if (Inputs["Quality"].Float>=20)
			points = Poisson20Points;
		if (Inputs["Quality"].Float>=25)
			points = Poisson25Points;
		if (Inputs["Quality"].Float>=30)
			points = Poisson30Points;
		if (Inputs["Quality"].Float>=40)
			points = Poisson40Points;
		if (Inputs["Quality"].Float>=50)
			points = Poisson50Points;
		if (Inputs["Quality"].Float>=100)
			points = Poisson100Points;
		if (Inputs["Quality"].Float>=200)
			points = Poisson200Points;
		
		for(int i = 0;i<points.Length;i+=2){
			float weight = 1f;
			float px = points[i];
			float py = points[i+1];
			if (Inputs["Weighting"].On){
				Vector2 vec = new Vector2(px,py);
					if (Inputs["Weighting Inverse"].On)
						vec = vec.normalized * (Mathf.Pow(vec.magnitude,Inputs["Factor"].Float));
					else
						vec = vec.normalized * (1f-Mathf.Pow(1f-vec.magnitude,Inputs["Factor"].Float));
			
				px = vec.x;
				py = vec.y;
			}
			if (new Vector2(px,py).magnitude>0.01){
				retVal.Append(GetAddBlurString(SD,SL,Branch,px,py,weight,endtag));
				AddCount+=weight;
			}
		}
		
		retVal.Append(")/"+(AddCount).ToString()+")");
		
		return retVal.ToString();
	}
	public override string Generate(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return GenerateGeneric(SD, SL, Inputs, Line, Branch,".rgb");
	}
	public override string GenerateWAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return GenerateGeneric(SD, SL, Inputs, Line, Branch,"");
	}
	public override string GenerateAlpha(ShaderData SD, ShaderLayer SL, SVDictionary Inputs, string Line, ShaderLayerBranch Branch){
		return GenerateGeneric(SD, SL, Inputs, Line, Branch,".a");
	}
	public override string GenerateStart(ShaderData SD){
		string seed = "vtp.position.xy";
		//Debug.Log(SD.ShaderModel);
		//Debug.Log(SD.ShaderModelAuto);
		if ((SD.ShaderModel == ShaderModel.Auto&&(SD.ShaderModelAuto == ShaderModel.SM2||SD.ShaderModelAuto==ShaderModel.SM2_5||SD.ShaderModelAuto==ShaderModel.SM3))||SD.ShaderModel == ShaderModel.SM2||SD.ShaderModel==ShaderModel.SM2_5||SD.ShaderModel==ShaderModel.SM3)
			seed = "floor(vd.screenPos.xy*_ScreenParams.xy)";
		string rnd =  
		"half PoissonBlurRandRotation = 6.28*frac(sin(dot("+seed+", float2(12.9898, 78.233)))* 43758.5453);\n";///TODO: Add support for custom banding - and correct (PoissonBlurRandRotation/=sqrt(Quality))
		
		if (SD.Temp)
			rnd += "half PoissonBlurRandRotationAnimated = 6.28*frac(sin(dot("+seed+", float2(12.9898, 78.233))+_SSTime.x)* 43758.5453);\n";
		else
			rnd += "half PoissonBlurRandRotationAnimated = 6.28*frac(sin(dot("+seed+", float2(12.9898, 78.233))+_Time.x)* 43758.5453);\n";
		
		return rnd+
		"half4 PoissonBlurRotation = half4( PoissonBlurRot(half2(1,0),PoissonBlurRandRotation), PoissonBlurRot(half2(0,1),PoissonBlurRandRotation));\n"+
		"half4 PoissonBlurRotationAnimated = half4( PoissonBlurRot(half2(1,0),PoissonBlurRandRotationAnimated), PoissonBlurRot(half2(0,1),PoissonBlurRandRotationAnimated));\n";
	}
	public override bool Draw(Rect rect){
		int Y = 0;
		bool update = false;
		int i = 0;
		
		if (Inputs["Reduce Banding"].On)
			Inputs["Animate Noise"].Hidden = false;
		else
			Inputs["Animate Noise"].Hidden = true;
		
		if (Inputs["Weighting"].On){
			Inputs["Factor"].Hidden = false;
			Inputs["Weighting Inverse"].Hidden = false;
		}
		else{
			Inputs["Factor"].Hidden = true;
			Inputs["Weighting Inverse"].Hidden = true;
		}
		
		foreach(KeyValuePair<string,ShaderVar> a in Inputs){
			ShaderVar SV = a.Value;
			if (SV.Hidden == false){
				if (i==1||i==2||i==5||i==7||i==8){
					if (SV.Draw(new Rect(rect.x+2+20,rect.y+Y*20,rect.width-4-16,20),SV.Name))update = true;
				}
				else{
					if (SV.Draw(new Rect(rect.x+2,rect.y+Y*20,rect.width-4,20),SV.Name))update = true;
				}
				Y+=1;
				if (SV.ShowNumbers)
					Y+=1;
			}
			i++;
		}
		if (!Inputs["Separate"].On){
			Inputs["Blur Y"].Hidden = true;
			Inputs["Blur Y"].Use = false;
			Inputs["Blur X"].Name = "Blur";
			Inputs["Blur Y"].Float = Inputs["Blur X"].Float;
			Inputs["Blur Y"].UseInput = Inputs["Blur X"].UseInput;
		}
		else{
			Inputs["Blur X"].Name = "Blur X";
			Inputs["Blur Y"].Hidden = false;
			Inputs["Blur Y"].Use = true;
		}
		return update;
	}
	public static float[] Poisson3Points = new float[]{
		0.5724562f, 0.6826031f,
		0.01501155f, -0.915989f,
		-0.8965766f, -0.06154215f
	};
	public static float[] Poisson4Points = new float[]{
		0.02476075f, -0.7779286f,
		0.3943906f, 0.6101655f,
		-0.8852497f, 0.1672804f,
		0.8505912f, -0.3281441f
	};
	public static float[] Poisson5Points = new float[]{
		-0.7957063f, 0.3076356f,
		0.09538598f, 0.8763882f,
		0.229684f, -0.6201293f,
		-0.7097354f, -0.4950111f,
		0.3737206f, 0.1264346f
	};
	public static float[] Poisson10Points = new float[]{
		0.3361086f, -0.7454419f,
		-0.3396147f, -0.232551f,
		0.1044865f, 0.1541135f,
		0.7479108f, -0.1854571f,
		-0.4358546f, -0.8037816f,
		-0.5230139f, 0.4908966f,
		-0.9737176f, 0.2253013f,
		0.01432822f, 0.6359013f,
		-0.8122727f, -0.4197346f,
		0.656606f, 0.6030211f
	};
	public static float[] Poisson15Points = new float[]{
		-0.3014517f, 0.4104693f,
		0.122612f, 0.3694764f,
		-0.5772369f, 0.7581095f,
		0.1277625f, -0.2439991f,
		-0.2503848f, -0.02185887f,
		0.04606917f, 0.9134792f,
		-0.7149873f, 0.1212395f,
		-0.3735067f, -0.4221394f,
		-0.7762218f, -0.4957251f,
		-0.4232179f, -0.8786449f,
		0.591468f, -0.6616971f,
		0.7475024f, -0.1394626f,
		0.1937208f, -0.8706161f,
		0.6234847f, 0.304923f,
		0.5161396f, 0.8536832f
	};
	public static float[] Poisson20Points = new float[]{
		-0.5420936f, -0.06769659f,
		-0.5578253f, -0.583697f,
		-0.02640542f, -0.5568427f,
		-0.9909379f, -0.115223f,
		-0.758519f, 0.2848054f,
		-0.2363301f, 0.288436f,
		-0.103763f, -0.08932879f,
		-0.6585842f, 0.6582814f,
		0.5345013f, -0.4915428f,
		0.2108035f, 0.5521463f,
		0.533279f, -0.04395336f,
		0.9272754f, 0.1125228f,
		0.6202674f, 0.4349916f,
		0.8812137f, -0.3336655f,
		0.2979943f, -0.9476216f,
		0.1870839f, 0.1591433f,
		-0.2876338f, -0.9391012f,
		-0.1694376f, 0.688916f,
		0.5204541f, 0.8427336f,
		0.1538422f, 0.9681873f
	};
	public static float[] Poisson25Points = new float[]{
		0.2080095f, -0.01085258f,
		0.2431835f, -0.6473321f,
		-0.2630148f, -0.03314619f,
		0.706383f, -0.01909894f,
		0.2649457f, 0.319762f,
		-0.1366887f, -0.5147343f,
		0.6449754f, -0.4467505f,
		-0.06845222f, 0.3660467f,
		0.677255f, 0.3592614f,
		-0.4845913f, -0.6542463f,
		-0.5686948f, -0.1560421f,
		-0.2901484f, -0.946431f,
		0.06065856f, -0.9272438f,
		0.6195858f, -0.7732947f,
		0.9591783f, -0.2774893f,
		0.9708589f, 0.205323f,
		0.7125145f, 0.6999037f,
		0.2239271f, 0.6519736f,
		-0.2834446f, 0.6219321f,
		-0.1253776f, 0.9499886f,
		-0.4140132f, 0.3006884f,
		-0.6702861f, 0.522322f,
		-0.709277f, 0.151078f,
		0.1577622f, -0.335809f,
		-0.8429466f, -0.3235372f
	};
	public static float[] Poisson30Points = new float[]{
		0.5011483f, 0.04521868f,
		0.1465597f, 0.08051669f,
		0.4249199f, -0.3267138f,
		0.4148951f, 0.3668412f,
		0.8471819f, -0.3454683f,
		0.1121189f, -0.2301473f,
		0.7479299f, 0.2963868f,
		-0.02526122f, 0.3329114f,
		-0.2069537f, 0.06547579f,
		-0.2425406f, -0.2733826f,
		0.057284f, 0.6228352f,
		-0.3816113f, 0.3154401f,
		0.7328869f, 0.6222063f,
		0.4785986f, 0.7927536f,
		0.904448f, 0.02076665f,
		-0.4010861f, 0.8992082f,
		-0.5566451f, 0.04093836f,
		-0.9160525f, 0.2224667f,
		-0.7013909f, 0.5131071f,
		-0.2446982f, 0.6357487f,
		0.1219466f, 0.9327087f,
		-0.9268347f, -0.2508305f,
		-0.5423443f, -0.3187264f,
		-0.7674371f, -0.520185f,
		0.4785696f, -0.6505225f,
		0.1735249f, -0.566444f,
		-0.247582f, -0.7004845f,
		-0.063352f, -0.9795408f,
		-0.5582952f, -0.776435f,
		0.276051f, -0.8835434f
	};
	public static float[] Poisson40Points = new float[]{
		0.7256591f, -0.1042184f,
		0.4492062f, -0.4431369f,
		0.7055052f, -0.3740624f,
		0.5323756f, 0.3323394f,
		0.9846258f, -0.1362978f,
		0.7877364f, 0.3699379f,
		0.2854357f, 0.05240316f,
		0.9336613f, 0.1215896f,
		0.2809109f, -0.2491614f,
		0.3638776f, 0.6816204f,
		0.08544681f, 0.2493392f,
		0.6221807f, 0.7307564f,
		0.287935f, 0.4152373f,
		0.0205787f, -0.2755264f,
		0.1613114f, -0.6803955f,
		-0.1412761f, -0.4816247f,
		-0.04645349f, 0.0317595f,
		-0.07534885f, 0.4957601f,
		-0.2535167f, 0.2566531f,
		0.1584422f, 0.8409653f,
		-0.1671611f, 0.7797599f,
		-0.4729695f, -0.3225064f,
		-0.09659555f, -0.732605f,
		-0.6943507f, 0.1025151f,
		-0.631149f, 0.5491853f,
		-0.3866712f, 0.02117324f,
		-0.3471077f, 0.4888262f,
		-0.4647051f, -0.800525f,
		-0.2439562f, -0.2132379f,
		0.209526f, -0.9264317f,
		-0.1374217f, -0.9810445f,
		-0.8579679f, -0.1124868f,
		-0.8993123f, 0.3334761f,
		0.6261228f, -0.655448f,
		0.4670109f, -0.8765142f,
		-0.4122593f, 0.8613442f,
		-0.6350462f, -0.5527102f,
		-0.8742716f, -0.3821971f,
		-0.5496528f, 0.3086437f,
		0.5342344f, 0.0770176f
	};
	public static float[] Poisson50Points = new float[]{
		0.01191647f, -0.3766487f,
		0.06530184f, -0.06239028f,
		-0.1811882f, -0.5388901f,
		0.1055802f, -0.710745f,
		0.3280199f, -0.2674491f,
		-0.193547f, -0.2725269f,
		0.2221897f, -0.4697671f,
		-0.1753743f, 0.002348582f,
		-0.151774f, -0.77955f,
		-0.3858232f, -0.4307106f,
		0.354765f, -0.8052159f,
		0.4152598f, -0.5817062f,
		-0.05505833f, -0.9784528f,
		0.08306476f, 0.2260627f,
		-0.5666506f, 0.05787798f,
		-0.1841481f, 0.3785738f,
		-0.4438635f, -0.2119619f,
		-0.4176045f, 0.3660236f,
		0.5945581f, -0.7927471f,
		0.6253349f, -0.3254496f,
		0.6937959f, -0.5451365f,
		0.1498304f, 0.4665151f,
		-0.4727286f, 0.6839205f,
		-0.2536181f, 0.7588404f,
		-0.0238042f, 0.6838953f,
		0.7665393f, -0.125378f,
		0.8741308f, -0.4184473f,
		0.2488604f, 0.06121285f,
		0.4464028f, -0.05704774f,
		-0.3928106f, -0.8157101f,
		0.1660396f, -0.9463302f,
		-0.5493619f, -0.5980057f,
		-0.7476863f, -0.4808375f,
		0.4309874f, 0.3841074f,
		0.6658426f, 0.186349f,
		0.7253714f, 0.3993621f,
		0.3989984f, 0.6901026f,
		0.6660151f, 0.7267773f,
		-0.67133f, -0.1480678f,
		0.9698753f, 0.1371034f,
		0.2569785f, 0.884242f,
		-0.05084391f, 0.9498577f,
		-0.6316729f, 0.3139676f,
		-0.6975635f, 0.6446213f,
		-0.8742073f, -0.2880245f,
		0.9943928f, -0.08217984f,
		-0.8804912f, 0.3471478f,
		-0.919705f, -0.05283951f,
		-0.7785192f, 0.1181058f,
		-0.3565525f, 0.1414825f
	};
	public static float[] Poisson100Points = new float[]{
		0.02612813f, 0.7111971f,
		0.06356252f, 0.4564041f,
		0.314371f, 0.7428148f,
		0.07856653f, 0.9525216f,
		-0.08024029f, 0.6025564f,
		-0.2116409f, 0.8006228f,
		0.1751725f, 0.6666564f,
		-0.0831019f, 0.9148592f,
		0.234324f, 0.9107763f,
		-0.2319461f, 0.6063895f,
		-0.3309645f, 0.4458523f,
		-0.1278362f, 0.3520162f,
		0.2858385f, 0.3819727f,
		0.13461f, 0.2689093f,
		-0.04546513f, 0.2188616f,
		0.2975675f, 0.5381553f,
		-0.4370645f, 0.6307747f,
		-0.4606068f, 0.8351763f,
		-0.3047653f, 0.9264637f,
		0.4936808f, 0.7792125f,
		0.4465164f, 0.6285586f,
		0.6941953f, 0.6068887f,
		0.5718168f, 0.4932616f,
		0.2761671f, 0.1484971f,
		0.465151f, 0.1821229f,
		0.5670059f, 0.3097807f,
		-0.6079761f, 0.6393369f,
		0.8695297f, 0.4149915f,
		0.3418659f, -0.098874f,
		0.09636671f, 0.07422215f,
		0.2024577f, -0.03939712f,
		0.4778357f, -0.02515303f,
		0.6519207f, 0.09603161f,
		0.449419f, -0.3046562f,
		0.6352493f, -0.1637099f,
		0.7306239f, -0.03473042f,
		0.2590996f, -0.3714755f,
		0.0974424f, -0.2088683f,
		-0.5785699f, 0.4706898f,
		0.8820369f, 0.0193822f,
		0.9212467f, -0.1280063f,
		0.7314345f, 0.2454951f,
		0.7881542f, -0.3200015f,
		-0.1090654f, -0.181218f,
		0.05921827f, -0.3733233f,
		-0.06098822f, 0.003453207f,
		-0.1169817f, -0.370774f,
		0.1524362f, -0.5014747f,
		0.4331696f, 0.4181646f,
		0.7182284f, 0.3972321f,
		0.9607215f, 0.250033f,
		0.6569387f, -0.5055159f,
		0.4263377f, -0.4956872f,
		-0.2451997f, 0.06142167f,
		-0.3027266f, -0.1013021f,
		0.6016681f, -0.3118193f,
		0.3491679f, -0.6293834f,
		0.1917028f, -0.6543837f,
		-0.2974075f, 0.2940082f,
		0.3859811f, 0.9089845f,
		0.8338471f, -0.5004334f,
		0.9380691f, -0.2854305f,
		-0.2817636f, -0.2596802f,
		0.6700584f, -0.6709729f,
		-0.1696652f, -0.5980157f,
		-0.3413851f, -0.4495297f,
		-0.03763558f, -0.5195722f,
		-0.7721545f, 0.4735696f,
		-0.05514608f, -0.7573578f,
		0.2294462f, -0.9435279f,
		0.4690702f, -0.7222106f,
		0.1332518f, -0.7939733f,
		-0.4662409f, -0.147762f,
		-0.538831f, 0.07134102f,
		-0.4659495f, -0.3148045f,
		-0.3975342f, 0.1643098f,
		-0.6605445f, -0.1311508f,
		-0.6832516f, -0.3322007f,
		-0.5056108f, 0.3030522f,
		-0.6732994f, 0.1992185f,
		0.3440178f, -0.8266987f,
		-0.139351f, -0.9023094f,
		0.00706915f, -0.9414073f,
		-0.2827425f, -0.7810215f,
		-0.7232173f, 0.02067918f,
		-0.8197325f, 0.1569949f,
		-0.3172434f, -0.9440079f,
		-0.4235322f, -0.8345857f,
		-0.455768f, -0.5990983f,
		-0.8159465f, -0.2578754f,
		-0.9391537f, -0.07390571f,
		-0.7668179f, -0.5423347f,
		-0.5870424f, -0.4900112f,
		-0.88137f, -0.3955164f,
		-0.7592999f, 0.6309071f,
		-0.9686231f, -0.2234427f,
		-0.5873713f, -0.7311168f,
		-0.8316625f, 0.3077838f,
		-0.9647833f, 0.09376922f,
		-0.6756103f, 0.3515598f
	};
	public static float[] Poisson200Points = new float[]{
		-0.4300194f, -0.8232805f,
		-0.3532497f, -0.9043159f,
		-0.5488741f, -0.6736406f,
		-0.3322435f, -0.7188908f,
		-0.4377239f, -0.6165856f,
		-0.5463068f, -0.8322428f,
		-0.2222687f, -0.8359885f,
		-0.1434195f, -0.9079297f,
		-0.2438721f, -0.9401876f,
		-0.6710027f, -0.7022732f,
		-0.2514291f, -0.5283233f,
		-0.5821077f, -0.5045745f,
		-0.4285045f, -0.4486101f,
		-0.7675182f, -0.6241673f,
		-0.7058468f, -0.5260714f,
		-0.5309218f, -0.3809995f,
		-0.652706f, -0.4085552f,
		-0.09432106f, -0.7789069f,
		-0.1846865f, -0.6408336f,
		-0.04000707f, -0.5835513f,
		-0.1292696f, -0.5213372f,
		-0.0102499f, -0.6985342f,
		0.1016956f, -0.7651058f,
		0.1949179f, -0.677435f,
		0.1002946f, -0.5223247f,
		-0.009587185f, -0.8827381f,
		0.09160227f, -0.6483216f,
		-0.3599415f, -0.5406205f,
		-0.7677161f, -0.2568234f,
		-0.6306562f, -0.216694f,
		-0.8123513f, -0.3673907f,
		-0.8424847f, -0.4748852f,
		-0.5282617f, -0.2538913f,
		0.05255795f, -0.9850015f,
		0.1797176f, -0.957186f,
		0.1015371f, -0.8837067f,
		-0.06686588f, -0.9899041f,
		0.2580813f, -0.7714363f,
		0.2890108f, -0.6239019f,
		0.241842f, -0.4817588f,
		0.3823929f, -0.6750894f,
		-0.5757254f, -0.1233289f,
		-0.6607841f, -0.05517112f,
		-0.4336417f, -0.1931663f,
		-0.7696177f, -0.05996107f,
		-0.3224425f, -0.3663101f,
		-0.4172152f, -0.297139f,
		0.4689032f, -0.5281132f,
		0.4567594f, -0.841938f,
		0.4863799f, -0.7336988f,
		0.3552412f, -0.5324292f,
		0.5839065f, -0.6534222f,
		0.3226688f, -0.865271f,
		-0.7060623f, 0.0531872f,
		-0.5603448f, 0.04786197f,
		-0.4605265f, -0.06535586f,
		-0.8403611f, -0.1619193f,
		-0.3588874f, 0.04860171f,
		-0.2576052f, -0.02922012f,
		-0.4629863f, 0.1126492f,
		-0.3448579f, -0.1353905f,
		-0.1770282f, -0.3804683f,
		-0.2299885f, -0.181361f,
		-0.3201392f, -0.2505975f,
		-0.1711641f, -0.2700651f,
		-0.1011729f, -0.03366323f,
		-0.08406874f, -0.1378533f,
		-0.05725834f, -0.2578407f,
		0.07950238f, -0.3878196f,
		0.09053519f, -0.1913269f,
		-0.01837593f, -0.4408491f,
		0.01798752f, -0.07334862f,
		0.1919285f, -0.3491443f,
		-0.664445f, 0.2146145f,
		-0.5724746f, 0.1528808f,
		-0.485481f, 0.2417346f,
		-0.5118445f, 0.3969557f,
		-0.3648259f, 0.3649585f,
		-0.3181389f, 0.1734806f,
		-0.6299918f, 0.3216011f,
		-0.2949454f, 0.2804613f,
		-0.9364432f, -0.2058524f,
		-0.9218219f, -0.3839279f,
		0.4805858f, -0.4051972f,
		0.3036608f, -0.3403535f,
		0.2264931f, -0.07091318f,
		0.1014651f, 0.09701405f,
		-0.01325773f, 0.1128113f,
		-0.9334975f, 0.003764372f,
		-0.9750683f, -0.09694421f,
		0.3301312f, -0.1704718f,
		0.3139763f, 0.08205834f,
		0.1939827f, 0.03069827f,
		0.3393303f, -0.06169503f,
		0.2363506f, -0.2267291f,
		0.5142484f, -0.11951f,
		0.4256587f, -0.2757486f,
		0.4666369f, -0.02485169f,
		0.5226315f, -0.230487f,
		0.6549175f, -0.02227225f,
		0.662641f, -0.1298296f,
		0.60511f, -0.2992909f,
		0.5356961f, 0.05485426f,
		0.7238904f, -0.2427726f,
		0.6521644f, -0.4540447f,
		0.573104f, -0.8037529f,
		0.4552868f, 0.2194632f,
		0.5901135f, 0.2456087f,
		0.666732f, 0.09564237f,
		0.428567f, 0.08773036f,
		0.7047076f, 0.2017768f,
		0.8122398f, 0.06553438f,
		0.762117f, -0.04483939f,
		0.7380367f, -0.3597661f,
		0.2403716f, 0.1846801f,
		0.9186118f, -0.1780628f,
		0.8715287f, -0.02396156f,
		0.7968711f, -0.1648925f,
		0.09112613f, 0.2179989f,
		-0.01069296f, 0.2729714f,
		0.664299f, -0.7455587f,
		0.694644f, -0.6398293f,
		-0.151206f, 0.1303467f,
		0.7810943f, 0.3117752f,
		0.7121242f, 0.4076564f,
		0.8484358f, 0.1653219f,
		0.6024355f, 0.3826512f,
		0.07381826f, 0.384279f,
		0.249753f, 0.3417315f,
		0.8797276f, -0.3803534f,
		0.8090919f, -0.4983877f,
		0.850359f, -0.2787458f,
		-0.1675517f, 0.2753795f,
		0.603202f, -0.5478277f,
		-0.8612179f, 0.1034852f,
		0.9707161f, -0.05899495f,
		0.9303575f, 0.08965855f,
		0.9305522f, 0.2403214f,
		-0.9575073f, 0.1858024f,
		-0.0402343f, 0.4493434f,
		-0.1641507f, 0.4141553f,
		0.9597358f, -0.2758986f,
		-0.2749175f, 0.4528748f,
		0.3760393f, -0.418005f,
		-0.824387f, 0.254888f,
		-0.9231886f, 0.2967436f,
		-0.7553941f, 0.154358f,
		-0.6821348f, 0.42188f,
		-0.7946661f, 0.3610871f,
		0.3464009f, 0.2266916f,
		-0.004352365f, 0.5503966f,
		-0.1598784f, 0.524071f,
		-0.2663215f, 0.5615039f,
		0.4992443f, 0.3398259f,
		0.3859025f, 0.3793141f,
		-0.5723174f, 0.5126302f,
		-0.7830616f, 0.5449733f,
		-0.8805085f, 0.4451858f,
		-0.6654414f, 0.5734451f,
		0.790196f, 0.4923933f,
		0.895328f, 0.3960455f,
		0.7385762f, 0.610567f,
		0.5173746f, 0.4633362f,
		0.597513f, 0.5604374f,
		-0.1433942f, 0.6746541f,
		-0.3838659f, 0.6297825f,
		-0.3682111f, 0.7399001f,
		-0.4406835f, 0.5196377f,
		-0.2275333f, 0.7421693f,
		0.6414837f, 0.6624897f,
		0.5356449f, 0.662882f,
		0.4504466f, 0.5937693f,
		0.3089396f, 0.4548273f,
		0.1923594f, 0.5167028f,
		0.3296289f, 0.6036482f,
		0.2240058f, 0.6387789f,
		-0.6431097f, 0.6910599f,
		-0.5152129f, 0.6204265f,
		-0.05903374f, 0.8253719f,
		-0.1643782f, 0.8355314f,
		-0.2694809f, 0.8843475f,
		-0.08506642f, 0.9422758f,
		-0.4150844f, 0.8778806f,
		0.613295f, 0.7760562f,
		-0.5309106f, 0.7335504f,
		0.08070014f, 0.7297148f,
		0.3695894f, 0.7767942f,
		0.2485088f, 0.7813126f,
		0.09627372f, 0.6182019f,
		0.1478538f, 0.8173704f,
		0.4579559f, 0.834309f,
		-0.03651099f, 0.7140694f,
		0.031206f, 0.8956615f,
		-0.1898512f, 0.9716009f,
		-0.5230921f, 0.8487129f,
		-0.7463782f, 0.6494836f,
		0.1192722f, 0.9617193f,
		0.2856058f, 0.9350459f,
		-0.2579955f, 0.08361518f,
		0.4092501f, 0.4901597f
	};
}
}