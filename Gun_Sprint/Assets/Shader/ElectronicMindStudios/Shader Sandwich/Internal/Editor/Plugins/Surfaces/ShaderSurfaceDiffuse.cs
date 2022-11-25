//#define UnityGGXCorrection
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
using System.Text;
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
public class ShaderSurfaceDiffuse : ShaderSurface{
	public ShaderVar LightingType = new ShaderVar("Lighting Type",new string[] {"The Greatest Sandwich\n...shader model","Unity Standard","Standard", "Microfaceted", "Translucent", "Light Wrap"},new string[]{"ImagePreviews/DiffuseSandwich.png","ImagePreviews/DiffuseStandard.png","ImagePreviews/DiffuseLambert.png","ImagePreviews/DiffuseMicrofaceted.png","ImagePreviews/DiffuseTranslucent.png","ImagePreviews/DiffuseLightwrap.png"},"",new string[] {"Shader Sandwich's own bunch 'o good lookin' hacks :)","A physically based version of the Standard option - based on Disney's BRDF.","A good approximation of hard, but smooth surfaced objects.\n(Wood,Plastic)", "Useful for rough surfaces, or surfaces with billions of tiny indents.\n(Carpet,Skin)", "Good for simulating sub-surface scattering, or translucent objects.\n(Skin,Plants)", "A very fast hack that allows light to wrap around the edges of the model.\n(Particles)"});

	public ShaderVar RoughnessOrSmoothness = new ShaderVar("Roughness Or Smoothness",new string[] {"Smoothness","Roughness"});
	public ShaderVar Smoothness = new ShaderVar("Smoothness",0.3f);
	public ShaderVar Roughness = new ShaderVar("Roughness",0.7f);
	public ShaderVar LightSize = new ShaderVar("Light Size",0f);
	public ShaderVar WrapAmount = new ShaderVar("Wrap Amount",0.7f);
	public ShaderVar WrapColor = new ShaderVar("Wrap Color",new Vector4(0.4f,0.2f,0.2f,1f));
	public ShaderVar PBRQuality = new ShaderVar("PBR Quality",new string[] {"Auto", "High", "Medium", "Low"},new string[] {"","","",""},new string[] {"UNITY_BRDF_PBSSS","BRDF1_Unity_PBSSS", "BRDF2_Unity_PBSSS", "BRDF3_Unity_PBSSS"});
	//public ShaderVar PBRModel = new ShaderVar("PBR Model",new string[] {"Specular", "Metal"},new string[] {"",""},new string[] {"Specular","Metal"});
	//public ShaderVar PBRSpecularType = new ShaderVar("PBR Specular Type",new string[] {"GGX", "BlinnPhong"},new string[] {"",""},new string[] {"GGX","BlinnPhong"});
	public ShaderVar DisableNormals = new ShaderVar("Disable Normals",0f);
	public ShaderVar UseTangents = new ShaderVar("Use Tangents",false);
	public ShaderVar UseAmbient = new ShaderVar("Use Ambient",true);
	//public ShaderVar AmbientRoughness = new ShaderVar("Ambient Roughness",false);
	public ShaderFix DeferredTypeNotSupported = null;
	public ShaderFix DeferredTypeSemiSupported = null;
	public override void GetMyPersonalPrivateForMySubStandardChildEyesOnlyFixes(ShaderBase shader, ShaderPass pass, List<ShaderFix> fixes){
		if (DeferredTypeNotSupported==null)
			DeferredTypeNotSupported = new ShaderFix(this,"Switch to Unity Standard Diffuse; Diffuse types other than Unity Standard aren't supported in deferred mode (The Greatest Sandwich model is sort of...)",ShaderFix.FixType.Error,
			()=>{
				LightingType.Type = 1;
			});
		if (DeferredTypeSemiSupported==null)
			DeferredTypeSemiSupported = new ShaderFix(this,"Switch to Unity Standard Diffuse; The Greatest Sandwich shadel model is only sort of supported in deferred mode - it'll do its fun ambient lighting stuff, but nothing else.",ShaderFix.FixType.Error,
			()=>{
				LightingType.Type = 1;
			});
		
		if (shader.RenderingPathTarget.Type==1){//Deferred
			if (LightingType.Type>1)
				fixes.Add(DeferredTypeNotSupported);
			if (LightingType.Type==0)
				fixes.Add(DeferredTypeSemiSupported);
		}
	}
	public override void GetMyShaderVars(List<ShaderVar> SVs){
		SVs.Add(LightingType);
		SVs.Add(RoughnessOrSmoothness);
		SVs.Add(Smoothness);
		SVs.Add(Roughness);
		SVs.Add(LightSize);
		SVs.Add(WrapAmount);
		SVs.Add(WrapColor);
		SVs.Add(PBRQuality);
		SVs.Add(DisableNormals);
		SVs.Add(UseAmbient);
		//SVs.Add(AmbientRoughness);
		SVs.Add(UseTangents);
	}
	new static public string MenuItem = "Lighting/Diffuse";
	public void OnEnable(){
		Name = "Diffuse";
		MenuItem = "Lighting/Diffuse";
		SurfaceLayers.NameUnique.Text = "Albedo";
		SurfaceLayers.Name.Text = "Albedo";
		SurfaceLayers.Description = "The color of the surface";
		SurfaceLayers.BaseColor = new Color(0,0,0,0);
		CustomLightingLayers.NameUnique.Text = "Diffuse";
		CustomLightingLayers.Name.Text = "Diffuse";
		CustomLightingLayers.InputName = "Diffuse";
		CustomLightingLayers.CodeName = "Lighting";
		CustomLightingLayers.EndTag.Text = "rgb";
		
		CustomAmbientLayers.NameUnique.Text = "Diffuse Indirect";
		CustomAmbientLayers.Name.Text = "Diffuse Indirect";
		CustomAmbientLayers.InputName = "Diffuse Indirect";
		CustomAmbientLayers.CodeName = "gi.indirect.diffuse";
		CustomAmbientLayers.EndTag.Text = "rgb";
		//Roughness.Range0 = 0.002f;
		Roughness.Range0 = 0.014f;
		Smoothness.Range1 = 0.986f;
		CustomLightingEnabled = true;
		CustomAmbientEnabled = true;
		//if (ShaderSandwich.Instance!=null&&ShaderSandwich.Instance.SRS!=null&&ShaderSandwich.Instance.SRS.LayerSelection == null&&SurfaceLayers.SLs.Count>0)
		//	ShaderSandwich.Instance.SRS.LayerSelection = SurfaceLayers.SLs[0];
	}
	public new void Awake(){
		BaseAwake();
		ShaderLayer SL = ScriptableObject.CreateInstance<ShaderLayer>();
		DateTime now = DateTime.Now;
		if (now.Day==1&&now.Month==4)
			SL.CustomData.D["Color"] = new ShaderVar("Color",new ShaderColor(1f,0f,1f,1f));
		else
			SL.CustomData.D["Color"] = new ShaderVar("Color",new ShaderColor(0.8f,0.8f,0.8f,1f));
		//SL.LayerTypeInputs = ShaderSandwich.PluginList["LayerType"][SL.RealLayerType.Text].GetInputs();
		SurfaceLayers.SLs.Add(SL);
		SL.Parent = SurfaceLayers;
		SL.Name.Text = "Albedo";
		LightingType.Type=1;
		
		
		CustomLightingEnabled = true;
		CustomAmbientEnabled = true;
		SL = ScriptableObject.CreateInstance<ShaderLayer>();
		SL.SetType("SLTData");
		SL.CustomData["Data"].Type = 10;
		CustomLightingLayers.SLs.Add(SL);
		SL.Parent = CustomLightingLayers;
		SL.Name.Text = "Light Color";
		SL.MixType.Type = 3;
	}
	public override int GetHeight(float Width){
		return 124+18+24+20+25+25+25+25+25+25+25;
	}
	public override void RenderUI(float Width){
		LightingType.Draw(new Rect(0,0,Width,124));
		
		GUI.skin.label.alignment = TextAnchor.UpperCenter;
		EMindGUI.Label(new Rect(0,124+18+(LightingType.Type==0?10:0),Width,70),LightingType.Descriptions[LightingType.Type],12);
		GUI.skin.label.alignment = TextAnchor.UpperLeft;
		
		Roughness.LabelOffset = 150;
		Smoothness.LabelOffset = 150;
		if (LightingType.Type==0||LightingType.Type==1){
			GUI.Label(new Rect(0,124+18+24+25,Width-124,20),"Quality ");
			PBRQuality.Draw(new Rect(0+16,124+18+24+20+25,Width-16,20),"Quality");
			if (RoughnessOrSmoothness.Type==1)//Lol inverted
				Roughness.Draw(new Rect(0,124+18+24+20+25+25,Width,20),"Roughness");
			else
				Smoothness.Draw(new Rect(0,124+18+24+20+25+25,Width,20),"Smoothness");
			GUI.color = new Color(1f,1f,1f,0f);
			Rect downRect = new Rect(70,124+18+24+20+25+25+2,16,20);
			if (RoughnessOrSmoothness.Type==0)
				downRect.x+=6;
			RoughnessOrSmoothness.Type = EditorGUI.Popup(downRect,"",RoughnessOrSmoothness.Type,new string[]{"Smoothness (Unity)","Roughness   (Everyone else)"},GUI.skin.GetStyle("box"));
			
			downRect.y+=2;
			downRect.width=16;
			downRect.height=16;
			
			if (downRect.Contains(Event.current.mousePosition)){
				GUI.color = EMindGUI.BaseInterfaceColor;
				if (EMindGUI.mouse.MouseDrag){
					downRect.y+=1;
				}
			}
			else
				GUI.color = new Color(1f,1f,1f,1f);
			GUI.DrawTexture(downRect,ShaderSandwich.MiniDownArrow);
			GUI.color = new Color(1f,1f,1f,1f);
			
			LightSize.Draw(new Rect(0,124+18+24+20+25+25+25,Width,20),"Light Radius");
			UseAmbient.Draw(new Rect(0,124+18+24+20+25+25+25+25,Width,20),"Use Ambient");
			//AmbientRoughness.Draw(new Rect(16,124+18+24+20+25+25+25+25,Width,20),"Ambient Roughness");
			UseTangents.Draw(new Rect(0,124+18+24+20+25+25+25+25+25,Width,20),"Use Tangents");
			//GUI.Label(new Rect(122,105,Width-124,20),"Model: ");
			//PBRModel.Draw(new Rect(122+49,100,Width-124-49,20),"Model: ");
		}
			
		if (LightingType.Type==2){
			LightSize.Draw(new Rect(0,124+18+24+25,Width,20),"Light Radius");
			DisableNormals.Draw(new Rect(0,124+18+24+25+25,Width,20),"Disable Normals");
			UseAmbient.Draw(new Rect(0,124+18+24+25+25+25,Width,20),"Use Ambient");
			//AmbientRoughness.Draw(new Rect(16,124+18+24+25+25+25,Width,20),"Ambient Roughness");
			UseTangents.Draw(new Rect(0,124+18+24+25+25+25+25,Width,20),"Use Tangents");
		}
		if (LightingType.Type==3){
			LightSize.Draw(new Rect(0,124+18+24+25,Width,20),"Light Radius");
			Roughness.Draw(new Rect(0,124+18+24+25+25,Width,20),"Roughness");
			UseAmbient.Draw(new Rect(0,124+18+24+25+25+25,Width,20),"Use Ambient");
			//AmbientRoughness.Draw(new Rect(16,124+18+24+25+25+25,Width,20),"Ambient Roughness");
			UseTangents.Draw(new Rect(0,124+18+24+25+25+25+25,Width,20),"Use Tangents");
		}
			
		if (LightingType.Type==4){
			LightSize.Draw(new Rect(0,124+18+24+25,Width,20),"Light Radius");
			WrapAmount.Draw(new Rect(0,124+18+24+25+25,Width,20),"Light Wrap");
			WrapColor.Draw(new Rect(0,124+18+24+25+25+25,Width,20),"Color");
			UseAmbient.Draw(new Rect(0,124+18+24+25+25+25+25,Width,20),"Use Ambient");
			//AmbientRoughness.Draw(new Rect(16,124+18+24+25+25+25+25,Width,20),"Ambient Roughness");
			UseTangents.Draw(new Rect(0,124+18+24+25+25+25+25+25,Width,20),"Use Tangents");
		}
		if (LightingType.Type==5){
			LightSize.Draw(new Rect(0,124+18+24+25,Width,20),"Light Radius");
			WrapAmount.Draw(new Rect(0,124+18+24+25+25,Width,20),"Light Wrap");
			UseAmbient.Draw(new Rect(0,124+18+24+25+25+25,Width,20),"Use Ambient");
			UseTangents.Draw(new Rect(0,124+18+24+25+25+25+25,Width,20),"Use Tangents");
		}
	}
	public override Dictionary<string,string> GeneratePrecalculations(){
		Dictionary<string,string> Precalcs = new Dictionary<string,string>();
		return Precalcs;
	}
	public string GenerateCodeHeader(ShaderData SD, string normals){
		string LightingCode = "";
			///TODO: Remove unncessaries fro mType 0		
		if (LightingType.Type==0||LightingType.Type==1){//Diffuse
			if (PBRQuality.Type==0){
				SD.AdditionalFunctions.Add(
				
"#if !defined (SSUNITY_BRDF_PBS) // allow to explicitly override BRDF in custom shader\n"+
"	// still add safe net for low shader models, otherwise we might end up with shaders failing to compile\n"+
"	#if SHADER_TARGET < 30\n"+
"		#define SSUNITY_BRDF_PBS 3\n"+
"	#elif defined(UNITY_PBS_USE_BRDF3)\n"+
"		#define SSUNITY_BRDF_PBS 3\n"+
"	#elif defined(UNITY_PBS_USE_BRDF2)\n"+
"		#define SSUNITY_BRDF_PBS 2\n"+
"	#elif defined(UNITY_PBS_USE_BRDF1)\n"+
"		#define SSUNITY_BRDF_PBS 1\n"+
"	#elif defined(SHADER_TARGET_SURFACE_ANALYSIS)\n"+
"		// we do preprocess pass during shader analysis and we dont actually care about brdf as we need only inputs/outputs\n"+
"		#define SSUNITY_BRDF_PBS 1\n"+
"	#else\n"+
"		#error something broke in auto-choosing BRDF (Shader Sandwich)\n"+
"	#endif\n"+
"#endif\n"
				
				);
				LightingCode += "#if SSUNITY_BRDF_PBS==1\n";
			}
		
		
			if (PBRQuality.Type==0||PBRQuality.Type==1){
				if (RoughnessOrSmoothness.Type==1)
					LightingCode += "half roughness = "+Roughness.Get()+";\n";
				else
					LightingCode += "half roughness = 1-"+Smoothness.Get()+";\n";
				
				LightingCode += 
				"half3 halfDir = normalize (gi.light.dir + vd.worldViewDir);\n"+
				"\n"+
				#if UnityGGXCorrection
				"#if UNITY_BRDF_GGX \n"+
				"	half shiftAmount = dot("+normals+", vd.worldViewDir);\n"+
				"	"+normals+" = shiftAmount < 0.0f ? "+normals+" + vd.worldViewDir * (-shiftAmount + 1e-5f) : "+normals+";\n"+
				"#endif\n"+
				#else
				#endif
				"half nh = saturate(dot("+normals+", halfDir));\n"+
				"half nl = saturate(dot("+normals+", gi.light.dir));\n"+
				#if UnityGGXCorrection
					"half nv = saturate(dot(vd.worldNormal, vd.worldViewDir));\n"+
				#else
					"half nv = abs(dot(vd.worldNormal, vd.worldViewDir));\n"+
				#endif
				//"half nv = DotClamped ("+normals+", vd.worldViewDir);\n"+
				"half lh = saturate(dot(gi.light.dir, halfDir));\n"+
				"\n"+
				"half nlPow5 = Pow5 (1-nl);\n"+
				"half nvPow5 = Pow5 (1-nv);\n";
			}
			
			if (PBRQuality.Type==0){
				if (LightingType.Type==0){
					LightingCode+="#elif SSUNITY_BRDF_PBS==2\n"+
					"	half nv = saturate(dot(vd.worldNormal, vd.worldViewDir));\n";
				}
				
				LightingCode+="#else\n";
			}
			if (PBRQuality.Type==2){
				if (LightingType.Type==0){
					LightingCode+="half nv = saturate(dot(vd.worldNormal, vd.worldViewDir));\n";
				}
			}
			
			if (PBRQuality.Type==0||PBRQuality.Type>1){
				LightingCode+="Lighting = saturate(dot("+normals+", gi.light.dir));\n";
			}
			
			if (PBRQuality.Type==0)
				LightingCode+="#endif\n";
		}
		if (LightingType.Type==2){//Diffuse
			if (DisableNormals.Get()=="1")
				LightingCode += "half nll = 1; //Disabled using normals, so just set this to 1.\n";
			else if (DisableNormals.Get()=="0")
				LightingCode += "half nll = saturate(dot("+normals+", gi.light.dir)); //Calculate the dot product of the faces normal and the lights direction. This means a lower number the further the angle of the face is from the light source.\n";
			else
				LightingCode += "half nll = lerp(saturate(dot("+normals+", gi.light.dir)),1,"+DisableNormals.Get()+"); //Calculate the dot product of the faces normal and the lights direction. This means a lower number the further the angle of the face is from the light source. Finally, we blend this with the default value of 1 when no normals is turned up.\n";
		}
		if (LightingType.Type==3){//Microfaceted
			string TempStr1 = Roughness.Get();
			LightingCode +=
			"\n"+
			"half roughness2=("+TempStr1+"*2)*("+TempStr1+"*2);\n"+
			"half2 AandB = roughness2/(roughness2 + float2(0.33,0.09));//Computing some constants\n"+
			"half2 oren_nayar = float2(1, 0) + float2(-0.5, 0.45) * AandB;\n"+
			"\n"+
			"//Theta and phi\n"+
			"half2 cos_theta = saturate(float2(dot("+normals+",gi.light.dir),dot("+normals+",vd.worldViewDir)));\n"+
			"half2 cos_theta2 = cos_theta * cos_theta;\n"+
			"half sin_theta = sqrt((1-cos_theta2.x)*(1-cos_theta2.y));\n"+
			"half3 light_plane = normalize(gi.light.dir - cos_theta.x*"+normals+");\n"+
			"half3 view_plane = normalize("+normals+" - cos_theta.y*"+normals+");\n"+///TODO: Does that even do anything??
			"half cos_phi = saturate(dot(light_plane, view_plane));\n";
		}
		if (LightingType.Type==4){//Translucent
			LightingCode +=
			"half3 Surf1 =saturate(dot ("+normals+", gi.light.dir));//Calculate lighting the standard way (See Diffuse lighting mode's comments).\n";

			string TempStr1 = WrapAmount.Get();
			LightingCode+=
			"half3 Surf2 = saturate(dot ("+normals+", gi.light.dir) + "+TempStr1+");//Calculate diffuse lighting while taking the Wrap Amount into consideration.\n";
			//"half3 Surf2 = saturate(dot ("+normals+", gi.light.dir)* "+TempStr1+"/2.0 + "+TempStr1+"/2.0);//Calculate diffuse lighting with inverted normals while taking the Wrap Amount into consideration.\n";
		}
		if (LightingType.Type==5){//Translucent
			string TempStr1 = WrapAmount.Get();
			LightingCode +=
			"Lighting = saturate((dot ("+normals+", gi.light.dir) + " + TempStr1 + ") / ((1+" + TempStr1 + ") * (1+" + TempStr1 + ")));\n";
		}
					
		return LightingCode;
	}
	public string GenerateCodeBody(string normals){
		string LightingCode = "";
		
		if (LightingType.Type==0){//Diffuse
			
			/*"const float PI = 3.14159;\n"+
			"// interpolating normals will change the length of the normal, so renormalize the normal.\n"+
			"// calculate intermediary values float\n"+
			"float NdotL = dot(vd.worldNormal, gi.light.dir);\n"+
			"float NdotV = dot(vd.worldNormal, vd.worldViewDir);\n"+
			"float angleVN = acos(NdotV);\n"+
			"float angleLN = acos(NdotL);\n"+
			"float alpha = max(angleVN, angleLN);\n"+
			"float beta = min(angleVN, angleLN);\n"+
			"float gamma = dot(vd.worldViewDir - vd.worldNormal * dot(vd.worldViewDir, vd.worldNormal), gi.light.dir - vd.worldNormal * dot(gi.light.dir, vd.worldNormal));\n"+
			"float roughnessSquared = roughness * roughness;\n"+
			"// calculate A and B\n"+
			"float A = 1.0 - 0.5 * (roughnessSquared / (roughnessSquared + 0.57));\n"+
			"float B = 0.45 * (roughnessSquared / (roughnessSquared + 0.09));\n"+
			"float C = sin(alpha) * tan(beta);\n"+
			"// put it all together\n"+
			"float L1 = (max(0.0, NdotL)) * (A + B * max(0.0, gamma) * C); // get the final color\n"+
			"Lighting = gi.light.color * L1;\n"+
			"Lighting *= 1-(0.5 + (nv - nvPow5)*roughness)*0.1* roughness;\n";*/
			if (PBRQuality.Type==0)
				LightingCode += "#if SSUNITY_BRDF_PBS==1\n";
			
			if (PBRQuality.Type==0||PBRQuality.Type==1){
				LightingCode +="//Optimized oren nayar by van Ouwerkerk, thankyou! - http://shaderjvo.blogspot.com.au/2011/08/van-ouwerkerks-rewrite-of-oren-nayar.html\n"+
				"float roughness2=roughness*roughness;\n"+
				"float2 oren_nayar_fraction = roughness2/(roughness2 + float2(0.33,0.09));\n"+
				"float2 oren_nayar = float2(1, 0) + float2(-0.5, 0.45) * oren_nayar_fraction;\n"+
				"\n"+
				"//Theta and phi\n"+
				"float2 cos_theta = float2(nl,nv);\n"+
				"float2 cos_theta2 = cos_theta * cos_theta;\n"+
				"float sin_theta = sqrt((1-cos_theta2.x)*(1-cos_theta2.y));\n"+
				"float3 light_plane = normalize(gi.light.dir - cos_theta.x*vd.worldNormal);\n"+
				"float3 view_plane = normalize(vd.worldViewDir - cos_theta.y*vd.worldNormal);\n"+
				"float cos_phi = saturate(dot(light_plane, view_plane));\n"+
				"\n"+
				"//composition\n"+
				"\n"+
				//"float diffuse_oren_nayar = cos_phi * sin_theta / max(cos_theta.x, cos_theta.y);\n"+
				"float diffuse_oren_nayar = cos_phi * sin_theta / max(cos_theta.x+0.00001, cos_theta.y);\n"+
				"\n"+
				"float diffuse = cos_theta.x * (oren_nayar.x + oren_nayar.y * diffuse_oren_nayar);\n"+
				"Lighting = max(0,diffuse);\n";
			}
			if (PBRQuality.Type==0)
				LightingCode += "#endif\n";
		}
		
		if (LightingType.Type==1){//Diffuse
			if (PBRQuality.Type==0)
				LightingCode += "#if SSUNITY_BRDF_PBS==1\n";
			
			if (PBRQuality.Type==0||PBRQuality.Type==1){
				LightingCode +=
				"half Fd90 = 0.5 + 2 * lh * lh * roughness;\n"+
				"half disneyDiffuse = (1 + (Fd90-1) * nlPow5) * (1 + (Fd90-1) * nvPow5);\n"+
				"\n"+
				"half diffuseTerm = disneyDiffuse * nl;\n"+
				"Lighting = diffuseTerm;\n";
			}
			if (PBRQuality.Type==0)
				LightingCode += "#endif\n";
		}
		if (LightingType.Type==2){//Diffuse
			LightingCode = 
			"Lighting = nll; //Output the final RGB color by multiplying the surfaces color with the light color, then by the distance from the light (or some function of it), and finally by the Dot of the normal and the light direction.\n";
		}
		if (LightingType.Type==3){//Microfaceted
			LightingCode =
			"//composition\n"+
			"half diffuse_oren_nayar = cos_phi * sin_theta / max(cos_theta.x, cos_theta.y);\n"+
			"\n"+
			"half diffuse = cos_theta.x * (oren_nayar.x + oren_nayar.y * diffuse_oren_nayar);\n"+
			"Lighting = max(0,diffuse);\n";
		}
		if (LightingType.Type==4){//Translucent
			string TempStr1 = WrapAmount.Get();
			string TempStr2 = WrapColor.Get();
			LightingCode=
			"Lighting =  (Surf1+(Surf2*("+TempStr1+"-abs(dot("+normals+", gi.light.dir)))*"+TempStr1+" * "+TempStr2+".rgb));//Combine the two lightings together, by adding the standard one with the wrapped one.\n";
			//"Lighting =  gi.light.color * (Surf1+(Surf2*(0.8-abs(dot("+normals+", gi.light.dir)))*"+TempStr1+" * "+TempStr2+".rgb));//Combine the two lightings together, by adding the standard one with the inverted one.\n";
		}
		//if (LightingType.Type==5){//Light Wrap
			//LightingCode=
			//"Lighting *= gi.light.color;\n";
		//}
		
		return LightingCode;
	}
	int genid = 0;
	public override string GenerateCode(ShaderData SD){
		StringBuilder AlbedoCode = new StringBuilder("half4 Surface = half4(0,0,0,0);\n");
		genid++;
		AlbedoCode.Append(SurfaceLayers.GenerateCode(SD,OutputPremultiplied));
		
		string normals = "vd.worldNormal";
		if (SD.UsesPreviousBaseColor)
			AlbedoCode.Append("previousBaseColor = Surface;\n");
		//AlbedoCode.Append("//"+genid.ToString()+"\n");
		//AlbedoCode.Append("//"+SD.ShaderPass.ToString()+"\n");
		if (SD.ShaderPassOnlyZ){
			AlbedoCode.Append("return Surface;\n");
			return AlbedoCode.ToString();
		}
		if (SD.ShaderPassIsForward){
			
			//string LightingCode = "float3 Lighting;\n";
			AlbedoCode.Append("float3 Lighting;\n");

			if (UseTangents.On)
				normals = "vd.worldTangent";
			
			if (LightSize.Get()!="0"){
				AlbedoCode.Append(
					"//Spherical lights implementation modified from http://blog.selfshadow.com/publications/s2013-shading-course/karis/s2013_pbs_epic_notes_v2.pdf - thanks guys! :D\n"+
					"float3 L = UnityWorldSpaceLightDir(vd.worldPos);\n"+
					"float3 closestPoint = L + vd.worldNormal * ");
					AlbedoCode.Append( LightSize.Get());
					AlbedoCode.Append( ";\n"+
					///UNITY_LIGHT_ATTENUATION(atten, vd, vd.worldPos+(gi.light.dir * SSTEMPSV69));
					"gi.light.dir = normalize(closestPoint);\n");
			}
		}
		if (!SD.ShaderPassIsDeferred)
			AlbedoCode.Append(GenerateCodeHeader(SD,normals));
		else if (LightingType.Type==0){
			if (PBRQuality.Type==0||PBRQuality.Type==1||PBRQuality.Type==2){
				AlbedoCode.Append(
				#if UnityGGXCorrection
					"half nv = saturate(dot(vd.worldNormal, vd.worldViewDir));\n"
				#else
					"half nv = abs(dot(vd.worldNormal, vd.worldViewDir));\n"
				#endif
				+"half nvPow5 = Pow5 (1-nv);\n"
				);
				
				if (RoughnessOrSmoothness.Type==1)
					AlbedoCode.Append("half roughness = "+Roughness.Get()+";\n");
				else
					AlbedoCode.Append("half roughness = 1-"+Smoothness.Get()+";\n");
			}
		}
		if (UseAmbient.On&&SD.ShaderPassIsBaseLighting){
			AlbedoCode.Append( 
				"Unity_GlossyEnvironmentData g;\n"+
				"g.roughness = ");
			if (RoughnessOrSmoothness.Type==1||LightingType.Type>=2)
				AlbedoCode.Append(Roughness.Get());
			else
				AlbedoCode.Append("(1-"+Smoothness.Get()+")");
			AlbedoCode.Append(";\n");
			AlbedoCode.Append("g.reflUVW = vd.worldViewDir;\n"+
							  "gi = UnityGlobalIllumination(giInput, 1, ");
			AlbedoCode.Append(normals);
			AlbedoCode.Append(",g);\n");
				
			if (LightingType.Type==0){
				if (PBRQuality.Type==0||PBRQuality.Type==1||PBRQuality.Type==2){
					if (PBRQuality.Type==0)
						AlbedoCode.Append("#if SSUNITY_BRDF_PBS==1||SSUNITY_BRDF_PBS==2\n");
			
					AlbedoCode.Append(
						"#if UNITY_SHOULD_SAMPLE_SH\n"+
						"	#if !defined(LIGHTMAP_ON)\n"+
						"		#ifndef DYNAMICLIGHTMAP_ON\n");
					AlbedoCode.Append( 
						"gi.indirect.diffuse = max(half3(0, 0, 0), ShadeSH9(half4(normalize(");
						AlbedoCode.Append( normals);
						AlbedoCode.Append( " + vd.worldViewDir * g.roughness * nv");
						//AlbedoCode.Append( normals);
						AlbedoCode.Append( "), 1.0)));\n"+
						"		#endif\n"+
						"	#endif\n"+
						"#endif\n");
						
					if (PBRQuality.Type==0)
						AlbedoCode.Append("#endif\n");
				}
			}
		}
		else{
			AlbedoCode.Append(
				"UnityGI o_gi;\n"+
				"ResetUnityGI(o_gi);\n"+
				"o_gi.light = giInput.light;\n"+
				"o_gi.light.color *= giInput.atten;\n"+
				"gi = o_gi;\n");
		}
		if (SD.ShaderPassIsForward){
			AlbedoCode.Append( GenerateCodeBody(normals));
			if (UseCustomLighting.On){
				AlbedoCode.Append(CustomLightingLayers.GenerateCode(SD,false));
			}else{
				AlbedoCode.Append("Lighting *= gi.light.color;\n");
			}
		}
		
		if (LightingType.Type==0){
			//LightingCode += "Surface *= 1 - (0.5 + nv * roughness) * 0.2 * roughness;\n";
			if (UseAmbient.On&&SD.ShaderPassIsBaseLighting){
				if (PBRQuality.Type==0||PBRQuality.Type==1){
					if (PBRQuality.Type==0)
						AlbedoCode.Append("#if SSUNITY_BRDF_PBS==1\n");
					
					AlbedoCode.Append( "float roughnessReduction = 1 - (0.5 + (nv-nvPow5) * roughness) * 0.15 * roughness;\n");
					AlbedoCode.Append( "gi.indirect.diffuse *= roughnessReduction;\n");
					
					if (PBRQuality.Type==0)
						AlbedoCode.Append("#endif\n");
				}
			}
		}
		if (UseAmbient.On&&SD.ShaderPassIsBaseLighting&&UseCustomAmbient.On){
			AlbedoCode.Append(CustomAmbientLayers.GenerateCode(SD,false));
		}
		
		if (SD.ShaderPassIsForward){
			if (UseAmbient.On&&SD.ShaderPassIsBaseLighting)
				AlbedoCode.Append( "return half4(gi.indirect.diffuse + Lighting,1)*Surface;");
			else
				AlbedoCode.Append( "return half4(Lighting,1)*Surface;");
		} else if (SD.ShaderPassIsDeferred){
			AlbedoCode.Append("deferredOut.Albedo = Surface.rgb;\nOneMinusAlpha = 0;\ndeferredOut.Alpha = Surface.a;\ndeferredOut.AmbientDiffuse = gi.indirect.diffuse;\nOneMinusAlpha = 1-Surface.a;\nreturn deferredOut;\n");
		}
		return AlbedoCode.ToString();
	}
}
}