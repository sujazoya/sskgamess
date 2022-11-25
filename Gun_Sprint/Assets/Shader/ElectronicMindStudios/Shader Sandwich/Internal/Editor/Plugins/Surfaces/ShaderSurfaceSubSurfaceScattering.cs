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
public class ShaderSurfaceSubSurfaceScattering : ShaderSurface{//Ironic name lololololololol oh god
	public ShaderVar SSSType = new ShaderVar("SSS Type",new string[] {"The Scattering Sandwich :D","Tunable Ad-hoc Translucency"},new string[]{"ImagePreviews/SubSurfaceScattering.png","ImagePreviews/Translucency.png"},"",new string[] {"An expensive but great looking SSS approximation with independant RGB scattering, without any precomputation.","A very fast and decent translucency approximation - good for highly translucent objects like wax, not so much for skin. Works best with Add blend mode I think :)"});
	public ShaderVar UseThickness = new ShaderVar("SSS Use Thickness",true);
	public ShaderVar Ambient = new ShaderVar("SSS Constant Scatter",1f);
	public ShaderVar Density = new ShaderVar("SSS Density",10f);
	public ShaderVar Distortion = new ShaderVar("SSS Fresnel Distortion",0.05f);
	//public ShaderVar Scale = new ShaderVar("SSS Fresnel Brightness",1f);
	public ShaderVar Thinness = new ShaderVar("SSS Fresnel Thinness",10f);
	
	public ShaderVar SSSIterations = new ShaderVar("SSS Iterations",8f);
	public ShaderVar SSSQuality = new ShaderVar("SSS Quality",new string[] {"High", "Medium", "Low"});
	public ShaderVar RGBSeperate = new ShaderVar("SSS RGB Seperate",true);
	public ShaderVar Scale = new ShaderVar("SSS Scale",1f);
	public ShaderVar RGBScale = new ShaderVar("RGB Scale", new Vector4(1f,0.3f,0.3f,1f));
	//public ShaderVar RScale = new ShaderVar("Red Scale",1f);
	//public ShaderVar GScale = new ShaderVar("Blue Scale",0.3f);
	//public ShaderVar BScale = new ShaderVar("Green Scale",0.3f);
	public ShaderVar LightSize = new ShaderVar("Light Size",0f);
	
	public ShaderVar Fringing = new ShaderVar("SSS Fringing",0f);
	
	public ShaderVar ScatterShadows = new ShaderVar("Scatter Shadows",true);
	public ShaderVar ScatterShadowsSamples = new ShaderVar("Scatter Shadows Samples",10f);
	public ShaderVar ScatterShadowsRelativeSize = new ShaderVar("Scatter Shadows Relative Size",0.15f);
	public ShaderVar ScatterShadowsSizeType = new ShaderVar("Scatter Shadows Size Type",new string[]{"Relative Size","World Size"});
	//asdasd
	public ShaderVar UseAmbient = new ShaderVar("Use Ambient",true);
	
	public ShaderLayerList	ThicknessLayers;

	public ShaderFix DeferredSupport = null;
	public ShaderFix MobileSupport= null;
	public override void GetMyPersonalPrivateForMySubStandardChildEyesOnlyFixes(ShaderBase shader, ShaderPass pass, List<ShaderFix> fixes){
		if (MobileSupport==null)
			MobileSupport = new ShaderFix(this,"Switch to Ad-hoc translucency (will look very different!!!); Using real time Sub Surface Scattering on mobile probably isn't a good idea, at least go with the faster option XD",ShaderFix.FixType.Optimization,
			()=>{
				SSSType.Type = 1;
			});
		if (DeferredSupport==null)
			DeferredSupport = new ShaderFix(this,"Sub Surface Scattering isn't supported in deferred mode - it'll just render as Diffuse.",ShaderFix.FixType.Error,
			null);
		
		//if (shader.HardwareTarget.Type==2&&shader.RenderingPathTarget.Type==0){//Mobile Forward
		if (shader.HardwareTarget.Type==2){//Mobile
			fixes.Add(MobileSupport);
		}
		if (shader.RenderingPathTarget.Type==1){//Deferred
			fixes.Add(DeferredSupport);
		}
	}
	public override void GetMyShaderVars(List<ShaderVar> SVs){
		SVs.Add(SSSType);
		SVs.Add(UseThickness);
		SVs.Add(Ambient);
		SVs.Add(Density);
		SVs.Add(Distortion);
		SVs.Add(Thinness);
		SVs.Add(SSSIterations);
		SVs.Add(SSSQuality);
		SVs.Add(RGBSeperate);
		SVs.Add(Scale);
		SVs.Add(RGBScale);
		//SVs.Add(RScale);
		//SVs.Add(GScale);
		//SVs.Add(BScale);
		SVs.Add(LightSize);
		SVs.Add(Fringing);
		SVs.Add(ScatterShadows);
		SVs.Add(ScatterShadowsSamples);
		SVs.Add(ScatterShadowsRelativeSize);
		SVs.Add(ScatterShadowsSizeType);
		SVs.Add(UseAmbient);
		
		
		/*SVs.Add(Ambient);
		SVs.Add(Density);
		SVs.Add(Distortion);
		//SVs.Add(Scale);
		SVs.Add(Thinness);
		SVs.Add(UseAmbient);
		SVs.Add(RGBSeperate);
		SVs.Add(Scale);
		SVs.Add(RGBScale);
		SVs.Add(SSSIterations);
		SVs.Add(SSSQuality);
		SVs.Add(Fringing);
		SVs.Add(ScatterShadows);
		SVs.Add(ScatterShadowsSamples);
		SVs.Add(ScatterShadowsRelativeSize);
		SVs.Add(ScatterShadowsSizeType);
		//SVs.Add(RScale);
		//SVs.Add(GScale);
		//SVs.Add(BScale);
		SVs.Add(LightSize);*/
	}
	new static public string MenuItem = "Lighting/Sub Surface Scattering";
	public void OnEnable(){
		Name = "Sub Surface Scattering";
		MenuItem = "Lighting/Sub Surface Scattering";
		SurfaceLayers.Description = "The color at the surface.";
		SurfaceLayers.NameUnique.Text = "SSS Color";
		SurfaceLayers.Name.Text = "SSS Color";
		SurfaceLayers.BaseColor = new Color(0.8f,0.8f,0.8f,1);
		RGBScale.VecIsColor = false;
		RGBScale.ShowNumbers = false;
		RGBScale.HideAlpha = true;
	}
	public new void Awake(){
		BaseAwake();
		//MixAmount.Float = 0.1f;
		ThicknessLayers = ScriptableObject.CreateInstance<ShaderLayerList>();
		
		ThicknessLayers.UShaderLayerList("Sub Surface Thickness","SSS Thickness","The thickness of the object - typically baked with Vertex Toastie into vertex colors.","Sub Surface Thickness","Thickness","b","",new Color(1f,1f,1f,1f));
		ShaderLayer SL = ScriptableObject.CreateInstance<ShaderLayer>();
		//if (ShaderSandwich.ShaderLayerTypeInstances!=null&&ShaderSandwich.ShaderLayerTypeInstances.ContainsKey("SLTVertexColors"))
		//	SL.LayerType.Obj = ShaderSandwich.ShaderLayerTypeInstances["SLTVertexColors"];//"SLTVertexColors";
		SL.SetType("SLTVertexColors");
		//SL.LayerTypeInputs = ShaderSandwich.PluginList["LayerType"][SL.RealLayerType.Text].GetInputs();
		ThicknessLayers.SLs.Add(SL);
		SL.Parent = ThicknessLayers;
		SL.Name.Text = "Sub Surface Thickness";
		SSSType.Type = 0;
		SSSQuality.Type = 1;
	}
	public override int GetHeight(float Width){
		return 124+18+24+15+25+5+25+25+25+25+25+25+25+25+25+25+25+25+25;
	}
	public override void RenderUI(float Width){
		int y = 20;
		Density.Range0 = 1f;
		Density.Range1 = 5f;
		
		Scale.Range0 = 0f;
		Scale.Range1 = 1f;
		Thinness.Range0 = 1f;
		Thinness.Range1 = 32f;
		Distortion.Range1 = 0.5f;
		
		SSSIterations.Range0 = 1f;
		SSSIterations.Range1 = 10f;
		ScatterShadowsSamples.Range0 = 3f;
		ScatterShadowsSamples.Range1 = 50f;
		
		
		
		GUI.skin.label.alignment = TextAnchor.UpperCenter;
		EMindGUI.Label(new Rect(0,124+18,Width,70),SSSType.Descriptions[SSSType.Type],12);
		GUI.skin.label.alignment = TextAnchor.UpperLeft;
		Color oldColb = GUI.backgroundColor;
		y += 124+18+24;//+15;
		
		if (SSSType.Type==0){
			if (Event.current.type==EventType.Repaint){
				//EMindGUI.SCSkin.window.Draw(new Rect(250/3,YOffset,250/3*2,16*4+20),"",false,true,true,true);
				//EMindGUI.SCSkin.window.Draw(new Rect(250/3+10,YOffset+20,250/3*2-10,16*4),"",false,true,true,true);
				EMindGUI.SCSkin.window.padding.top = 2;
				GUI.backgroundColor = new Color(GUI.backgroundColor.r,GUI.backgroundColor.g,GUI.backgroundColor.b,0.4f);
				EMindGUI.SCSkin.window.Draw(new Rect(0,y,250,14),"Look",false,true,true,true);
				GUI.backgroundColor = oldColb;
				EMindGUI.SCSkin.window.padding.top = 3;
			}
			y += 19;
			/*if (RGBSeperate.Draw(new Rect(0,y,Width,20),"Seperate RGB")){
				if (RGBSeperate.On){
					RGBScale.Vector.r = RScale.Float;
					RGBScale.Vector.g = GScale.Float;
					RGBScale.Vector.b = BScale.Float;
				}
			}
			y+=25;*/
			if (RGBSeperate.On){
				/*RScale.NoInputs = true;
				GScale.NoInputs = true;
				BScale.NoInputs = true;
				RScale.Float = RGBScale.Vector.r;
				GScale.Float = RGBScale.Vector.g;
				BScale.Float = RGBScale.Vector.b;
				
				RScale.Draw(new Rect(0,y,Width,20),"Red Scale");y+=25;
				GScale.Draw(new Rect(0,y,Width,20),"Green Scale");y+=25;
				BScale.Draw(new Rect(0,y,Width,20),"Blue Scale");y+=25;
				RGBScale.Vector = new ShaderColor(RScale.Float,GScale.Float,BScale.Float,0f);*/
				Scale.Draw(new Rect(0,y,Width,20),"Scale");y+=25;
				RGBScale.Draw(new Rect(16,y,Width-16,20),"RGB Scattering");y+=25;//y+=20;
				Fringing.Draw(new Rect(16,y,Width-16,20),"Fringing");y+=25;
			}else{
				Scale.Draw(new Rect(16,y,Width-16,20),"Scale");y+=25;
				RGBScale.Vector = new ShaderColor(1f,1f,1f,0f);
			}
			
			ScatterShadows.Draw(new Rect(0,y,Width,20),"Scatter Shadows");y+=25;
			if (ScatterShadows.On){
				ScatterShadowsRelativeSize.Draw(new Rect(16,y,Width-16,20),ScatterShadowsSizeType.Names[ScatterShadowsSizeType.Type]);y+=25;
				ScatterShadowsSizeType.Draw(new Rect(16,y,Width-16,20),"Relative Size");y+=25;
			}
			y+=10;
			LightSize.Draw(new Rect(0,y,Width,20),"Light Radius");y+=25;
			UseAmbient.Draw(new Rect(0,y,Width,20),"Use Ambient");y+=25;
			y+=10;
			if (Event.current.type==EventType.Repaint){
				//EMindGUI.SCSkin.window.Draw(new Rect(250/3,YOffset,250/3*2,16*4+20),"",false,true,true,true);
				//EMindGUI.SCSkin.window.Draw(new Rect(250/3+10,YOffset+20,250/3*2-10,16*4),"",false,true,true,true);
				EMindGUI.SCSkin.window.padding.top = 2;
				GUI.backgroundColor = new Color(GUI.backgroundColor.r,GUI.backgroundColor.g,GUI.backgroundColor.b,0.4f);
				EMindGUI.SCSkin.window.Draw(new Rect(0,y,250,14),"Quality <> Speed",false,true,true,true);
				GUI.backgroundColor = oldColb;
				EMindGUI.SCSkin.window.padding.top = 3;
			}
			y += 19;
			
			SSSQuality.Draw(new Rect(0,y,Width,20),"High Quality");y+=25;
			SSSIterations.Draw(new Rect(0,y,Width,20),"Surface Quality");y+=25;
			if (ScatterShadows.On){
				ScatterShadowsSamples.Draw(new Rect(0,y,Width,20),"Shadow Quality");y+=25;
			}
			SSSIterations.Float = Mathf.Round(SSSIterations.Float);
			
			if (SSSQuality.Type<=1)
				SSSIterations.Float = Mathf.Max(2f,SSSIterations.Float);
			ScatterShadowsSamples.Float = Mathf.Round(ScatterShadowsSamples.Float);
		}
		if (SSSType.Type==1){
			y+=10;
			Ambient.Draw(new Rect(0,y,Width,20),"Constant Scatttering");y+=25;
			Density.Draw(new Rect(0,y,Width,20),"Density");y+=25;
			Thinness.Draw(new Rect(0,y,Width,20),"Scatter");y+=25;
			Distortion.Draw(new Rect(0,y,Width,20),"Distortion");y+=25;
			UseAmbient.Draw(new Rect(0,y,Width,20),"Use Ambient");y+=25;
		}
		SSSType.Draw(new Rect(0,0,Width,124));
		//Scale.Draw(new Rect(0,y,Width,20),"Fresnel Brightness");y+=25;
		
		if (SSSType.Type==0)
			SurfaceLayers.Description = "The color of the surface";
		else
			SurfaceLayers.Description = "The color underneath the surface";
	}
	public override string GenerateGlobalFunctions(ShaderData SD,List<ShaderIngredient> ingredients){
		string function = "";
		if (SD.ShaderPassOnlyZ)
			return "";
		bool UsedSSSBlurRot = false;
		foreach(ShaderIngredient ingredient in ingredients){
			ShaderSurfaceSubSurfaceScattering ing = ingredient as ShaderSurfaceSubSurfaceScattering;
			if (ing==null)
				continue;
			if (ing.SSSType.Type==1)
				continue;
			if (!ing.ScatterShadows.On)
				continue;
			
			UsedSSSBlurRot = true;
			
			function +=
			"	float ComputeScatteredShadow_"+ing.CodeName+"(VertexData vd,VertexToPixel vtp, float3 lightDir, float blerg){\n"+
			"		float colShadow = 0;\n"+
			"		float3 bitangent = cross(vd.worldTangent,vd.worldNormal);\n"+
			"		float weight = 0;\n"+
			"		\n";

			float[] points = SSEPoissonBlur.Poisson3Points;
			if (ing.ScatterShadowsSamples.Float>0)
				points = SSEPoissonBlur.Poisson3Points;
			if (ing.ScatterShadowsSamples.Float>=4)
				points = SSEPoissonBlur.Poisson4Points;
			if (ing.ScatterShadowsSamples.Float>=5)
				points = SSEPoissonBlur.Poisson5Points;
			if (ing.ScatterShadowsSamples.Float>=10)
				points = SSEPoissonBlur.Poisson10Points;
			if (ing.ScatterShadowsSamples.Float>=15)
				points = SSEPoissonBlur.Poisson15Points;
			if (ing.ScatterShadowsSamples.Float>=20)
				points = SSEPoissonBlur.Poisson20Points;
			if (ing.ScatterShadowsSamples.Float>=25)
				points = SSEPoissonBlur.Poisson25Points;
			if (ing.ScatterShadowsSamples.Float>=30)
				points = SSEPoissonBlur.Poisson30Points;
			if (ing.ScatterShadowsSamples.Float>=40)
				points = SSEPoissonBlur.Poisson40Points;
			if (ing.ScatterShadowsSamples.Float>=50)
				points = SSEPoissonBlur.Poisson50Points;




			function+=
			"		float sampleCount = "+(points.Length/2).ToString()+";\n"+
			"		static float2 asd["+(points.Length/2).ToString()+"] = {\n";
			for(int i = 0;i<points.Length;i+=2){
				//Precompute weights
				Vector2 pos = new Vector2(points[i],points[i+1]);
				//pos = pos*(1.0f-(1.0f/(Mathf.Sqrt(Vector2.Dot(pos,pos))+blerg)));
				if (!(ing.SSSQuality.Type<=1))
					pos = pos*(1.0f-(1.0f/(Mathf.Sqrt(Vector2.Dot(pos,pos)))));
				
				function += "			float2("+pos.x.ToString()+","+pos.y.ToString()+"),\n";
			}
			function+=
			"		};\n";

			if (ing.SSSQuality.Type<=1){
				function+=
				"		static float asdL["+(points.Length/2).ToString()+"] = {\n";
				float weight = 0f;
				for(int i = 0;i<points.Length;i+=2){
					float w = 1.0f/(Mathf.Sqrt(Vector2.Dot(new Vector2(points[i],points[i+1]),new Vector2(points[i],points[i+1]))));//(1.0f/Vector2.Dot(new Vector2(points[i],points[i+1]),new Vector2(points[i],points[i+1])));
					function += "			"+w.ToString()+",\n";//"1.0/dot(float2("+points[i].ToString()+","+points[i+1].ToString()+"),float2("+points[i].ToString()+","+points[i+1].ToString()+")),\n";
					weight += w;
				}
				function+=
				"		};\n";
			}

			function+=
			"		weight = "+(1.0f/(points.Length/2)).ToString()+";\n"+
			"		half PoissonBlurRandRotationAnimated = 6.28*frac(sin(dot(vd.screenPos.xy, float2(12.9898, 78.233))+_Time.w)* 43758.5453);\n"+
			"		half4 PoissonBlurRotationAnimated = half4( SSSBlurRot(half2(1,0),PoissonBlurRandRotationAnimated), SSSBlurRot(half2(0,1),PoissonBlurRandRotationAnimated));\n"+
			"		#if defined (SHADOWS_SCREEN) || defined (SHADOWS_DEPTH)\n"+
			"			float4 shadowCoord = vtp._ShadowCoord;\n"+
			"		#endif\n"+
			"		#if defined (SHADOWS_CUBE)\n"+
			"			float3 shadowCoord = vtp._ShadowCoord;\n"+
			"		#endif\n"+
			"		\n"+
			"		[unroll]\n"+
			"		for(float i = 0; i<sampleCount; i+=1){\n"+
			"			vtp._ShadowCoord = shadowCoord;\n";//tempasd[i] = (asd[i])*(1.0-(1.0/(sqrt(dot(asd[i],asd[i]))+blerg)));

			if (ing.SSSQuality.Type<=1){
				function+=
			"			float2 tempblah = (asd[i])*(1.0-(asdL[i]+blerg));\n";
			}

			string asd="asd[i]";
			if (ing.SSSQuality.Type<=1)
				asd = "tempblah";

				//function+="				vtp._ShadowCoord.xy += float2(dot("+asd+",PoissonBlurRotationAnimated.xz)*blerg,dot("+asd+",PoissonBlurRotationAnimated.yw)*blerg);\n";
			if (!(ing.SSSQuality.Type<=1))
				function+=	
				"			#if defined (SHADOWS_SCREEN)\n"+
				"				vtp._ShadowCoord.xy += float2(dot("+asd+",PoissonBlurRotationAnimated.xz)*blerg,dot("+asd+",PoissonBlurRotationAnimated.yw)*blerg);\n"+
				"			#endif\n"+
				"			#if defined (SHADOWS_DEPTH)\n"+
				"				vtp._ShadowCoord = mul (unity_World2Shadow[0],float4(vd.worldPos+(vd.worldTangent*dot("+asd+",PoissonBlurRotationAnimated.xz)+bitangent*dot("+asd+",PoissonBlurRotationAnimated.yw))*blerg,1));\n"+
				"			#endif\n"+
				"			#if defined (SHADOWS_CUBE)\n"+
				"				vtp._ShadowCoord += (vd.worldTangent*dot("+asd+",PoissonBlurRotationAnimated.xz)+bitangent*dot("+asd+",PoissonBlurRotationAnimated.yw))*blerg;\n"+
				"			#endif\n";
			else
				function+=	
				"			#if defined (SHADOWS_SCREEN)\n"+
				"				float3 u = normalize(unity_World2Shadow[0][0].xyz);\n"+
				"				float3 l = normalize(unity_World2Shadow[0][1].xyz);\n"+
				"				vtp._ShadowCoord = ComputeScreenPos(mul (UNITY_MATRIX_VP,float4(vd.worldPos+(u*dot("+asd+",PoissonBlurRotationAnimated.xz)+l*dot("+asd+",PoissonBlurRotationAnimated.yw))*blerg,1)));\n"+
				"			#endif\n"+
				"			#if defined (SHADOWS_DEPTH)\n"+
				"				float3 u = normalize(cross(float3(1,0,0),lightDir));\n"+
				"				float3 l = cross(u,lightDir);\n"+
				"				vtp._ShadowCoord = mul (unity_World2Shadow[0],float4(vd.worldPos+(u*dot("+asd+",PoissonBlurRotationAnimated.xz)+l*dot("+asd+",PoissonBlurRotationAnimated.yw))*blerg,1));\n"+
				"			#endif\n"+
				"			#if defined (SHADOWS_CUBE)\n"+
				"				float3 u = normalize(cross(float3(1,0,0),lightDir));\n"+
				"				float3 l = cross(u,lightDir);\n"+
				"				vtp._ShadowCoord += (u*dot("+asd+",PoissonBlurRotationAnimated.xz)+l*dot("+asd+",PoissonBlurRotationAnimated.yw))*blerg;\n"+
				"			#endif\n";
			

			function+=
			"			\n"+
			//"			float w = asdL[i];\n"+
			"			colShadow += SHADOW_ATTENUATION(vtp);\n"+//*w;\n"+
			"		}\n"+
			"		colShadow*=weight;\n"+
			"		return colShadow;\n"+
			"	}\n\n";
		}
		if (UsedSSSBlurRot){
			function = 
			"half2 SSSBlurRot( half2 d, float r ) {\n"+
			"	half2 sc = half2(sin(r),cos(r));\n"+
			"	return half2( dot( d, half2(sc.y, -sc.x) ), dot( d, sc.xy ) );\n"+
			"}\n\n"+			"#if defined (SHADOWS_SCREEN) || defined (SHADOWS_DEPTH) || defined (SHADOWS_CUBE)\n"+function+			"#endif\n";;
		}
		return function;
	}
	public override string GenerateCode(ShaderData SD){
		string SSSCode = "half4 Surface = half4(0.8,0.8,0.8,1);\n";
		
		SSSCode += SurfaceLayers.GenerateCode(SD,OutputPremultiplied);
		
		if (SD.ShaderPassOnlyZ){
			SSSCode += "return Surface;\n";
			return SSSCode;
		}
		if (SD.ShaderPassIsDeferred){
			string deffcoeadasd = SSSCode;
			if (UseAmbient.On&&SD.ShaderPassIsBaseLighting){
				deffcoeadasd += 
					"Unity_GlossyEnvironmentData g;\n"+
					"g.roughness = 1;\n"+
					"g.reflUVW = vd.worldViewDir;\n"+
					"gi = UnityGlobalIllumination(giInput, 1, vd.worldNormal,g);\n";
			}
			deffcoeadasd+="deferredOut.Alpha = 1;OneMinusAlpha = 0;\ndeferredOut.Albedo = Surface.rgb;\ndeferredOut.AmbientDiffuse = gi.indirect.diffuse;\nreturn deferredOut;\n";
			return deffcoeadasd;
		}
		
		if (SSSType.Type==1){
			SSSCode += "\n\nhalf Thickness = 1;\n";
			
			SSSCode += ThicknessLayers.GenerateCode(SD,true);
		}
		
		string LightingCode = "";
		if (SSSType.Type==0){
LightingCode = "float3 fLT = 0;\n\n" + 
"\n" + 
"float3 MaxScale = " + RGBScale.Get() + " * " + Scale.Get() + ";\n";

if (LightSize.Get()!="0"){
	LightingCode += "float3 backup_WorldSpaceLightPos0 = _WorldSpaceLightPos0.xyz;\n" +
	"_WorldSpaceLightPos0.xyz += vd.worldNormal * " + LightSize.Get() + ";\n";
}


LightingCode+=
/*"//Should be LinearToGamma I think...\n" + 
"float Distance = Thickness * " + Scale.Get() + ";\n" + 
"float StartDepth = (1-Thickness) * " + Scale.Get() + ";\n" + 
"\n" + 
"_WorldSpaceLightPos0.xyz += vd.worldNormal * " + LightSize.Get() + ";///TOoDO: Undo at end\n" + 
"\n" + 
"float3 v = (vd.worldPos-vd.worldViewDir*StartDepth) - _WorldSpaceLightPos0.xyz;\n" + 
"\n" + 
"float b = dot(vd.worldNormal, v);\n" + 
"float c = dot(v, v);\n" + 
"\n" + 
"float s = 1.0f / (sqrt(c - b*b)+0.5);\n" + 
"\n" + 
"float l = s * (atan( (Distance + b) * s) - atan( b*s ));\n" + 
"\n" + 
"float3 scatter = l * " + RGBScale.Get() + " * " + RGBScale.Get() + ";\n" + */
"\n" + 
"float Quality = "+SSSIterations.Get()+";\n" + 
"float weight = 1.0/(Quality+1);\n" + 
"float iters = 1.0/Quality;\n" + 
"\n" + 
"#ifndef USING_DIRECTIONAL_LIGHT\n" + 
"	float dist = max(1,length(UnityWorldSpaceLightDir(vd.worldPos)));\n";

string BackScatterOffset = "BackOffset";
if (Fringing.Get()!="1"){
	LightingCode+= "	float3 BackOffset = vd.worldNormal*dot(MaxScale,float3(0.33333,0.33333,0.33333))*dist;";
}
if (Fringing.Get()=="1"){
	BackScatterOffset = "Offset";
}
if (!Fringing.Safe()){
	BackScatterOffset = "lerp(BackOffset,Offset,"+Fringing.Get()+")";
}

string nmalize = "";
string nmalizeEnd = "";
if (SSSQuality.Type>0){
	nmalize = "normalize(";
	nmalizeEnd = ")";
}

LightingCode+=
"	//Red\n" + 
"	float3 Offset = vd.worldNormal*MaxScale.r*dist;\n" + 
"	float3 ldinnerR = "+nmalize+"UnityWorldSpaceLightDir(vd.worldPos+"+BackScatterOffset+")" + nmalizeEnd + ";\n" + 
"	float3 ldouterR = "+nmalize+"UnityWorldSpaceLightDir(vd.worldPos-Offset)" + nmalizeEnd + ";\n" + 
"	//Green\n" + 
"	Offset = vd.worldNormal*MaxScale.g*dist;\n" + 
"	float3 ldinnerG = "+nmalize+"UnityWorldSpaceLightDir(vd.worldPos+"+BackScatterOffset+")" + nmalizeEnd + ";\n" + 
"	float3 ldouterG = "+nmalize+"UnityWorldSpaceLightDir(vd.worldPos-Offset)" + nmalizeEnd + ";\n" + 
"	//Blue\n" + 
"	Offset = vd.worldNormal*MaxScale.b*dist;\n" + 
"	float3 ldinnerB = "+nmalize+"UnityWorldSpaceLightDir(vd.worldPos+"+BackScatterOffset+")" + nmalizeEnd + ";\n" + 
"	float3 ldouterB = "+nmalize+"UnityWorldSpaceLightDir(vd.worldPos-Offset)" + nmalizeEnd + ";\n#else\n";

if (Fringing.Get()!="1"){
	LightingCode+= "	float3 BackOffset = vd.worldNormal*dot(MaxScale,float3(0.33333,0.33333,0.33333));";
}
LightingCode+=
"	//Red\n" + 
"	float3 Offset = vd.worldNormal*MaxScale.r;\n" + 
"	float3 ldinnerR = "+nmalize+"UnityWorldSpaceLightDir(vd.worldPos)-"+BackScatterOffset + nmalizeEnd + ";\n" + 
"	float3 ldouterR = "+nmalize+"UnityWorldSpaceLightDir(vd.worldPos)+Offset" + nmalizeEnd + ";\n" + 
"	//Green\n" + 
"	Offset = vd.worldNormal*MaxScale.g;\n" + 
"	float3 ldinnerG = "+nmalize+"UnityWorldSpaceLightDir(vd.worldPos)-"+BackScatterOffset + nmalizeEnd + ";\n" + 
"	float3 ldouterG = "+nmalize+"UnityWorldSpaceLightDir(vd.worldPos)+Offset" + nmalizeEnd + ";\n" + 
"	//Blue\n" + 
"	Offset = vd.worldNormal*MaxScale.b;\n" + 
"	float3 ldinnerB = "+nmalize+"UnityWorldSpaceLightDir(vd.worldPos)-"+BackScatterOffset + nmalizeEnd + ";\n" + 
"	float3 ldouterB = "+nmalize+"UnityWorldSpaceLightDir(vd.worldPos)+Offset" + nmalizeEnd + ";\n#endif\n\n";

if (SSSQuality.Type>0){
	nmalize = "";
	nmalizeEnd = "";
}else{
	nmalize = "normalize(";
	nmalizeEnd = ")";	
}
if (SSSQuality.Type<=1)
 LightingCode+=
"weight = 0;\n";

if (SSSIterations.Get()==SSSIterations.Float.ToString())
LightingCode+=
"[unroll]\n";

if (SSSQuality.Type<=1)
 LightingCode+=
"for(float i = 0;i<=1;i+=iters){\n" + 
"	float w=1-abs(i*2-1);\n" + 
"	float3 a = 0;\n" + 
"	float3 lightDir = "+nmalize+"lerp(ldinnerR,ldouterR,i)" + nmalizeEnd + ";\n" + 
"	a.r = (dot(vd.worldNormal,lightDir))*w;\n" + 
"	lightDir = "+nmalize+"lerp(ldinnerG,ldouterG,i)" + nmalizeEnd + ";\n" + 
"	a.g = (dot(vd.worldNormal,lightDir))*w;\n" + 
"	lightDir = "+nmalize+"lerp(ldinnerB,ldouterB,i)" + nmalizeEnd + ";\n" + 
"	a.b = (dot(vd.worldNormal,lightDir))*w;\n" + 
"	a = saturate(a);\n" + 
"	fLT.rgb += a;\n" + 
"	weight+=w;\n" + 
"}\n" + 
"fLT*=1.0/weight;\n" ;
else
 LightingCode+=
"for(float i = 0;i<=1;i+=iters){\n" + 
"	float3 a = 0;\n" + 
"	float3 lightDir = (lerp(ldinnerR,ldouterR,i));\n" + 
"	a.r = (dot(vd.worldNormal,lightDir));\n" + 
"	lightDir = (lerp(ldinnerG,ldouterG,i));\n" + 
"	a.g = (dot(vd.worldNormal,lightDir));\n" + 
"	lightDir = (lerp(ldinnerB,ldouterB,i));\n" + 
"	a.b = (dot(vd.worldNormal,lightDir));\n" + 
"	a = saturate(a);\n" + 
"	fLT.rgb += a;\n" + 
"}\n" + 
"fLT*=weight;\n" ;

//"fLT.rgb = max(scatter,fLT.rgb);\n" + 

LightingCode+= "float3 scatteredShadow = giInput.atten;\n";

if (ScatterShadows.On&&ScatterShadowsRelativeSize.Get()!="0"){
	LightingCode+=
	"#ifndef SHADOWS_SCREEN\n"+
	"	float4 backup = _LightShadowData;//This is fun XD\n"+
	"	_LightShadowData = 1;\n"+
	"	UNITY_LIGHT_ATTENUATION(atten, vtp, vd.worldPos)\n"+
	"	_LightShadowData = backup;\n"+
	"	scatteredShadow = atten;\n"+
	"#else\n"+
	"	scatteredShadow = 1;\n"+
	"#endif\n"+
	
	//LightingCode+=
	"#if defined (SHADOWS_SCREEN) || defined (SHADOWS_DEPTH) || defined (SHADOWS_CUBE)\n";
	if (ScatterShadowsSizeType.Type==0)
		LightingCode += "	MaxScale *= "+ScatterShadowsRelativeSize.Get()+";\n";
	else
		LightingCode += "	MaxScale = " + RGBScale.Get() + " * "+ScatterShadowsRelativeSize.Get()+";\n";
	
/*BackScatterOffset = "blerg";
if (Fringing.Get()!="1"){
	LightingCode += "	float blerg = dot(MaxScale,float3(0.33333,0.33333,0.33333));\n";
}
if (Fringing.Get()=="1"){
	BackScatterOffset = "MaxScale";
}
if (!Fringing.Safe()){
	BackScatterOffset = "lerp(blerg,MaxScale,"+Fringing.Get()+")";
}*/
	LightingCode+=
	"	\n"+
	"	scatteredShadow.r *= ComputeScatteredShadow_"+CodeName+"(vd, vtp, gi.light.dir, MaxScale.r);\n"+
	"	scatteredShadow.g *= ComputeScatteredShadow_"+CodeName+"(vd, vtp, gi.light.dir, MaxScale.g);\n"+
	"	scatteredShadow.b *= ComputeScatteredShadow_"+CodeName+"(vd, vtp, gi.light.dir, MaxScale.b);\n"+
	//"	scatteredShadow.r = ComputeScatteredShadow(vd,vtp,lerp(blerg,MaxScale.r,"+Fringing.Get()+"));\n"+
	//"	scatteredShadow.g = ComputeScatteredShadow(vd,vtp,lerp(blerg,MaxScale.g,"+Fringing.Get()+"));\n"+
	//"	scatteredShadow.b = ComputeScatteredShadow(vd,vtp,lerp(blerg,MaxScale.b,"+Fringing.Get()+"));\n"+
	"#endif\n";
}


 LightingCode+=
"\n" + 
"fLT.rgb *= gi.light.color * scatteredShadow;\n" + 
"fLT.rgb *= Surface.rgb;\n";

if (LightSize.Get()!="0"){
	LightingCode+="_WorldSpaceLightPos0.xyz = backup_WorldSpaceLightPos0;\n";
}
}
		if (SSSType.Type==1){
			LightingCode = "";
LightingCode = 
			"float3 vLTLight = gi.light.dir + vd.worldNormal * "+Distortion.Get()+";//distortion;\n"+
			"vLTLight = normalize(vLTLight);\n"+
			"float fLTDot = exp2(saturate(dot(vd.worldViewDir,-vLTLight)) * "+Thinness.Get()+" - ("+Thinness.Get()+"));\n"+
			"fLTDot = fLTDot * (("+Thinness.Get()+"+6)*0.125);\n"+
			"float3 tolight = vd.worldPos-_WorldSpaceLightPos0.xyz;\n"+
			"float atten = pow(1.0/(dot(tolight,tolight)+1),"+Density.Get()+");\n"+
			"#ifndef USING_DIRECTIONAL_LIGHT\n"+
			"	float3 fLT = atten * (fLTDot + "+Ambient.Get()+") * Surface.rgb * gi.light.color * Thickness;\n"+
			"#else\n"+
			"	float3 fLT = fLTDot * Surface.rgb * gi.light.color * Thickness;\n"+
			"#endif\n";
		}
			
		if (UseAmbient.On&&SD.ShaderPassIsBaseLighting){
			LightingCode += 
				"Unity_GlossyEnvironmentData g;\n"+
				"g.roughness = 1;\n"+
				"g.reflUVW = vd.worldViewDir;\n"+
				"gi = UnityGlobalIllumination(giInput, 1, vd.worldNormal,g);\n"+
				"fLT += gi.indirect.diffuse*Surface.rgb;\n";
		}
		
		LightingCode += "return float4(fLT,Surface.a);";
		return SSSCode+LightingCode;
	}
	public override List<ShaderLayerList> GetMyShaderLayerLists(bool All){
		List<ShaderLayerList> SLLs = new List<ShaderLayerList>();
		SLLs.Add(SurfaceLayers);
		if (SSSType.Type==1||All)
			SLLs.Add(ThicknessLayers);
		return SLLs;
	}
}
}
/*
function+=
"		float sampleCount = "+(points.Length/2).ToString()+";\n"+
"		static float2 asd["+(points.Length/2).ToString()+"] = {\n";
for(int i = 0;i<points.Length;i+=2){
	function += "			float2("+points[i].ToString()+","+points[i+1].ToString()+"),\n";
}
function+=
"		};\n"+
"		static float asdL["+(points.Length/2).ToString()+"] = {\n";
float weight = 0f;
for(int i = 0;i<points.Length;i+=2){
	float w = (1.0f/Vector2.Dot(new Vector2(points[i],points[i+1]),new Vector2(points[i],points[i+1])));
	function += "			"+w.ToString()+",\n";//"1.0/dot(float2("+points[i].ToString()+","+points[i+1].ToString()+"),float2("+points[i].ToString()+","+points[i+1].ToString()+")),\n";
	weight += w;
}
function+=
"		};\n"+
"		weight = "+(1.0f/weight).ToString()+";\n"+
"		half PoissonBlurRandRotationAnimated = 6.28*frac(sin(dot(vd.worldPos.xy+vd.worldPos.y+_SSTime.xy, float2(12.9898, 78.233)))* 43758.5453);\n"+
"		half4 PoissonBlurRotationAnimated = half4( SSSBlurRot(half2(1,0),PoissonBlurRandRotationAnimated), SSSBlurRot(half2(0,1),PoissonBlurRandRotationAnimated));\n"+
"		float4 shadowCoord = vtp._ShadowCoord;\n"+
"		\n"+
"		[unroll]\n"+
"		for(float i = 0; i<sampleCount; i+=1){\n"+
"			vtp._ShadowCoord = shadowCoord;\n"+
"			#if defined (SHADOWS_SCREEN)\n"+
"			vtp._ShadowCoord.xy += float2(dot(asd[i],PoissonBlurRotationAnimated.xz)*blerg,dot(asd[i],PoissonBlurRotationAnimated.yw)*blerg);\n"+
"			#endif\n"+
"			float2 lentjhing = asd[i];\n"+
"			\n"+
"			float w = asdL[i];\n"+
"			colShadow += SHADOW_ATTENUATION(vtp)*w;\n"+
"		}\n"+
"		colShadow*=weight;\n"+
"		return colShadow;\n"+
"	}\n"+
"#endif\n";
return function;*/