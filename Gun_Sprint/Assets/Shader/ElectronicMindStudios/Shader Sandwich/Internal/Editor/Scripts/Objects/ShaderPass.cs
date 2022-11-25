#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define PRE_UNITY_5
#else
#define UNITY_5
#endif
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using EMindGUI = ElectronicMind.ElectronicMindStudiosInterfaceUtils;
using UEObject = UnityEngine.Object;
using System.Text;
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
[System.Serializable]
public class ShaderPass : ScriptableObject{
//Pass Settings
	public ShaderVar Name = new ShaderVar("Name","");
	public ShaderVar Visible = new ShaderVar("Visible",true);
//Settings

	/*public bool IsPBR{
		get{
			if (DiffuseLightingType.Type==0&&DiffuseOn.On)
			return true;
			
			return false;
		}
		set{
		}
	}
	public bool UsingMetalModel{
		get{
			if (IsPBR&&PBRModel.Type==1)
			return true;
			
			return false;
		}
		set{
		}
	}*/

	public bool Initiated = false;

	public ShaderVar OtherTypes = new ShaderVar("Parallax Type",new string[] {"Off", "On","Off","Off","Off","On"},new string[]{"ImagePreviews/ParallaxOff.png","ImagePreviews/ParallaxOn.png","ImagePreviews/TransparentOff.png","ImagePreviews/EmissionOff.png","ImagePreviews/ShellsOff.png","ImagePreviews/ShellsOn.png","ImagePreviews/TessellationOff.png","ImagePreviews/TessellationOn.png"},"",new string[] {"", "","","","",""});	
	
//LayerLists
	/*public ShaderLayerList	ShaderLayersDiffuse = new ShaderLayerList("Albedo","Albedo","The color of the surface.","Texture","d.Albedo","rgb","",new Color(0.8f,0.8f,0.8f,1f));
	public ShaderLayerList	ShaderLayersAlpha = new ShaderLayerList("Alpha","Alpha","Which parts are see through.","Transparency","d.Alpha","a","",new Color(1f,1f,1f,1f));
	public ShaderLayerList	ShaderLayersSpecular = new ShaderLayerList("Specular","Specular","Where the shine appears.","Gloss","d.Specular","rgb","",new Color(0.3f,0.3f,0.3f,1f));
	public ShaderLayerList	ShaderLayersMetallic = new ShaderLayerList("Metallic","Metallic","How metallic the surface is.","Metallic","d.Metallic","r","",new Color(0.3f,0.3f,0.3f,1f));
	public ShaderLayerList	ShaderLayersNormal = new ShaderLayerList("Normals","Normals","Used to add fake bumps.","NormalMap","d.Normal","rgb","",new Color(0f,0f,1f,1f));
	public ShaderLayerList	ShaderLayersEmission = new ShaderLayerList("Emission","Emission","Where and what color the glow is.","Emission","d.Emission","rgba","",new Color(0f,0f,0f,1f));
	public ShaderLayerList	ShaderLayersHeight = new ShaderLayerList("Height","Height","Which parts of the shader are higher than the others.","Height","d.Height","a","",new Color(1f,1f,1f,1));
	public ShaderLayerList	ShaderLayersVertex = new ShaderLayerList("Vertex","Vertex","Used to move the model's vertices.","Vertex","Vertex","rgba","",new Color(1f,1f,1f,1));
	
	public ShaderLayerList	ShaderLayersLightingDiffuse = new ShaderLayerList("LightingDiffuse","Diffuse","Customize the diffuse lighting.","Lighting","cDiff","rgb","",new Color(0.8f,0.8f,0.8f,1f));
	public ShaderLayerList	ShaderLayersLightingSpecular = new ShaderLayerList("LightingSpecular","Specular","Custom speculars highlights and reflections.","Lighting","cSpec","rgb","",new Color(0.8f,0.8f,0.8f,1f));
	public ShaderLayerList	ShaderLayersLightingAmbient = new ShaderLayerList("LightingIndirect","Ambient","Customize the ambient lighting.","Lighting","cAmb","rgb","",new Color(0.8f,0.8f,0.8f,1f));
	public ShaderLayerList	ShaderLayersLightingAll = new ShaderLayerList("LightingDirect","Direct","Custom both direct diffuse and specular lighting.","Lighting","cTemp","rgb","",new Color(0.8f,0.8f,0.8f,1f));
	
	public ShaderLayerList	ShaderLayersSubsurfaceScattering = new ShaderLayerList("SubsurfaceScatteringColor","SSS Color","The color underneath the surface.","Scatter","d.SubsurfaceColor","rgb","",new Color(1f,0.0f,0.0f,1f));
	
	public ShaderLayerList	ShaderLayersFragmentPoke = new ShaderLayerList("FragmentPoke","Poke Channel 1","Calculated for every individual poke.","PokeChan1","PokeChan1","rgba","",new Color(1f,1f,1f,1f));
	public ShaderLayerList	ShaderLayersVertexPoke = new ShaderLayerList("VertexPoke","Poke Channel 1","Calculated for every individual poke.","PokeChan2","PokeChan2","rgba","",new Color(1f,1f,1f,1f));*/
	
	
	public List<ShaderIngredient> Surfaces = new List<ShaderIngredient>();
	//public ShaderSurface selectedSurface = null;
	public ShaderIngredient selectedIngredient = null;
	public List<ShaderIngredient> GeometryModifiers = new List<ShaderIngredient>();
	
	
	public ShaderFix DeferredNormalsNotFirst = null;
	public ShaderFix DeferredDiffuseAfterSpecular = null;
	public ShaderFix DeferredNoSpecular = null;
	public void GetMyPersonalPrivateForMyEyesOnlyFixes(ShaderBase shader, List<ShaderFix> fixes){
		if (DeferredNoSpecular==null)
			DeferredNoSpecular = new ShaderFix(this,"Deferred mode doesn't have full support for disabling specular lighting, ambient will be gone, but you'll still see some around the rim of the object and won't be able to control roughness/smoothness.",ShaderFix.FixType.Error,
			null);
		if (DeferredDiffuseAfterSpecular==null)
			DeferredDiffuseAfterSpecular = new ShaderFix(this,"Put diffuse ingredient before specular; anything else isn't fully supported in deferred mode and could lead to odd behaviour.",ShaderFix.FixType.Error,
			()=>{
				for(int i = 0;i<Surfaces.Count;i++){
					ShaderSurfaceSpecular ing = Surfaces[i] as ShaderSurfaceSpecular;
					if (ing!=null){
						for(int ii = i;ii<Surfaces.Count;ii++){
							ShaderSurfaceDiffuse ing2 = Surfaces[ii] as ShaderSurfaceDiffuse;
							if (ing2!=null){
								ShaderUtil.MoveItem(ref Surfaces,ii,i);
							}
						}
					}
				}
			});
		
		if (shader.RenderingPathTarget.Type==1){//Deferred
			bool foundSpecular = false;
			bool foundDiffuseAfterSpecular = false;
			
			for(int i = 0;i<Surfaces.Count;i++){
				ShaderSurfaceSpecular ing = Surfaces[i] as ShaderSurfaceSpecular;
				if (ing!=null){
					foundSpecular = true;
					for(int ii = i;ii<Surfaces.Count;ii++){
						ShaderSurfaceDiffuse ing2 = Surfaces[ii] as ShaderSurfaceDiffuse;
						if (ing2!=null)
							foundDiffuseAfterSpecular = true;
					}
				}
			}
			if (!foundSpecular)
				fixes.Add(DeferredNoSpecular);
			if (foundDiffuseAfterSpecular)
				fixes.Add(DeferredDiffuseAfterSpecular);
		}
	}
	[NonSerialized]public List<ShaderFix> ProposedFixes = null;
	public virtual void GetMyFixes(ShaderBase shader, List<ShaderFix> fixes){
		GetMyPersonalPrivateForMyEyesOnlyFixes(shader,fixes);
		foreach(ShaderIngredient ingredient in Surfaces){
			if (!ingredient.IsUsed())continue;
			ingredient.GetMyFixes(shader,this,fixes);
		}
		foreach(ShaderIngredient ingredient in GeometryModifiers){
			if (!ingredient.IsUsed())continue;
			ingredient.GetMyFixes(shader,this,fixes);
		}
	}
	
	
	//public ShaderGeometryModifier selectedGeometryModifier = null;
	
	public bool dragSurface = false;
	public bool hasDraggedSurface = false;
	public Vector2 dragSurfaceOffset = new Vector2(0,0);
	
	public void AddIngredient(object TypeName,List<ShaderIngredient> Ingredients){
		Type t = (Type)TypeName;
		selectedIngredient = (ShaderIngredient)ScriptableObject.CreateInstance(t);
		if (selectedIngredient.TopAnchor!=null){
			int Anchor = 0;
			bool DOOEEET = true;
			foreach(ShaderIngredient SI in Ingredients){
				if (SI.GetType()==selectedIngredient.GetType())
					DOOEEET = false;
			}
			if (DOOEEET){
				foreach(Type type in selectedIngredient.TopAnchor){
					int i = 0;
					foreach(ShaderIngredient SI in Ingredients){
						//if (SI.GetType().Name==type.ToString())
						if (SI.GetType()==type)
							Anchor = i+1;
						i++;
					}
				}
			}
			else{
				Anchor = Ingredients.Count;
			}
			Ingredients.Insert(Anchor,selectedIngredient);
		}
		else
			Ingredients.Add(selectedIngredient);
		
		if (selectedIngredient.ExistencePreferred!=null){
			foreach(Type type in selectedIngredient.ExistencePreferred){
				bool DOOEEETmoredooeetttes = true;
				foreach(ShaderIngredient SI in Surfaces){
					if (SI.GetType()==type)
						DOOEEETmoredooeetttes = false;
				}
				foreach(ShaderIngredient SI in GeometryModifiers){
					if (SI.GetType()==type)
						DOOEEETmoredooeetttes = false;
				}
				if (DOOEEETmoredooeetttes){
					AddIngredient(type);
				}
			}
		}
		EMindGUI.Defocus();
	}
	
	public void AddGeometryModifier(object TypeName){
		//GeometryModifiers.Add((ShaderGeometryModifier)Activator.CreateInstance((Type)TypeName));
		///Type t = (Type)TypeName;
		///GeometryModifiers.Add((ShaderIngredient)ScriptableObject.CreateInstance(t));
		///selectedIngredient = GeometryModifiers[GeometryModifiers.Count-1];
		AddIngredient(TypeName,GeometryModifiers);
	}
	public void DeleteGeometryModifier(object modifier){
		GeometryModifiers.Remove((ShaderGeometryModifier)modifier);
		selectedIngredient = null;
		EMindGUI.Defocus();
	}
	public void AddSurface(object TypeName){
		//Surfaces.Add((ShaderSurface)Activator.CreateInstance((Type)TypeName));
		/*Type t = (Type)TypeName;
		selectedIngredient = (ShaderIngredient)ScriptableObject.CreateInstance(t);
		if (selectedIngredient.TopAnchor!=null){
			int Anchor = 0;
			bool DOOEEET = true;
			foreach(ShaderIngredient SI in Surfaces){
				if (SI.GetType()==selectedIngredient.GetType())
					DOOEEET = false;
			}
			if (DOOEEET){
				foreach(Type type in selectedIngredient.TopAnchor){
					int i = 0;
					foreach(ShaderIngredient SI in Surfaces){
						//if (SI.GetType().Name==type.Name)
						if (SI.GetType()==type)
							Anchor = i+1;
						i++;
					}
				}
			}
			else{
				Anchor = Surfaces.Count;
			}
			Surfaces.Insert(Anchor,selectedIngredient);
		}
		else
			Surfaces.Add(selectedIngredient);
		
		if (selectedIngredient.ExistencePreferred!=null){
			foreach(Type type in selectedIngredient.ExistencePreferred){
				bool DOOEEETmoredooeetttes = true;
				foreach(ShaderIngredient SI in Surfaces){
					if (SI.GetType()==type)
						DOOEEETmoredooeetttes = false;
				}
				if (DOOEEETmoredooeetttes){
					AddIngredient(type);
				}
			}
		}
		//selectedIngredient = Surfaces[Surfaces.Count-1];
		EMindGUI.Defocus();*/
		AddIngredient(TypeName,Surfaces);
	}
	
	public void AddSurfaceMaskUpdate(object Sll){
		//Surfaces.Add((ShaderSurface)Activator.CreateInstance((Type)TypeName));
		ShaderLayerList SLL = (ShaderLayerList)Sll;
		if (Surfaces.Count>0&&Surfaces[Surfaces.Count-1] as ShaderIngredientUpdateMask == null){
			Surfaces.Add((ShaderIngredient)ScriptableObject.CreateInstance(typeof(ShaderIngredientUpdateMask)));
		}
		(Surfaces[Surfaces.Count-1] as ShaderIngredientUpdateMask).Add(SLL);
		selectedIngredient = Surfaces[Surfaces.Count-1];
		EMindGUI.Defocus();
	}
	public void DeleteIngredient(object surface){
		Surfaces.Remove((ShaderIngredient)surface);
		GeometryModifiers.Remove((ShaderIngredient)surface);
		selectedIngredient = null;
		EMindGUI.Defocus();
	}
	public void AddIngredient(object TypeName){
		Type t = (Type)TypeName;
		ShaderIngredient ingredient = (ShaderIngredient)ScriptableObject.CreateInstance(t);
		if (ingredient as ShaderSurface!=null){
			Surfaces.Add(ingredient);
			selectedIngredient = ingredient;
		}else if (ingredient as ShaderGeometryModifier!=null){
			GeometryModifiers.Add(ingredient);
			selectedIngredient = ingredient;
		}
		EMindGUI.Defocus();
	}
	public void AddIngredientDirect(ShaderIngredient ingredient, ShaderIngredient Pos){
		if (Pos==null){
			if (ingredient as ShaderSurface!=null){
				Surfaces.Add(ingredient);
				selectedIngredient = ingredient;
			}else if (ingredient as ShaderGeometryModifier!=null){
				GeometryModifiers.Add(ingredient);
				selectedIngredient = ingredient;
			}
		}
		else{
			if (ingredient as ShaderSurface!=null){
				if (Surfaces.Contains(Pos)){
					Surfaces.Insert(Surfaces.IndexOf(Pos as ShaderSurface)+1,ingredient);
					selectedIngredient = ingredient;
				}
			}else if (ingredient as ShaderGeometryModifier!=null){
				if (GeometryModifiers.Contains(Pos)){
					GeometryModifiers.Insert(GeometryModifiers.IndexOf(Pos as ShaderGeometryModifier)+1,ingredient);
					selectedIngredient = ingredient;
				}
			}else{
				if (Surfaces.Contains(Pos))
					Surfaces.Insert(Surfaces.IndexOf(Pos as ShaderSurface)+1,ingredient);
				else
					GeometryModifiers.Insert(GeometryModifiers.IndexOf(Pos as ShaderGeometryModifier)+1,ingredient);
				selectedIngredient = ingredient;
			}
		}
		EMindGUI.Defocus();
	}
	
	public ShaderPass(){
		//ShaderLayersLightingDiffuse.IsLighting.On = true;
		//ShaderLayersLightingSpecular.IsLighting.On = true;
		//ShaderLayersLightingAmbient.IsLighting.On = true;
		//ShaderLayersLightingAll.IsLighting.On = true;
		//TechCull.Type = 1;
		//TechZTest.Type = 2;
		//TessellationType.Type = 2;
		//IndirectSubsurfaceScattering.Type = 0;
		//PokeInteraction.Type = 1;
		
		//Surfaces.Add(new ShaderSurfaceDiffuse());
		//Surfaces.Add(new ShaderSurfaceSpecular());
	}
	public void Awake(){
		Surfaces.Add((ShaderIngredient)ScriptableObject.CreateInstance(typeof(ShaderSurfaceDiffuse)));
		Surfaces[0].Link(this);
		Surfaces.Add((ShaderIngredient)ScriptableObject.CreateInstance(typeof(ShaderSurfaceSpecular)));
		Surfaces[1].Link(this);
		selectedIngredient = Surfaces[0];
		GeometryModifiers.Add((ShaderIngredient)ScriptableObject.CreateInstance(typeof(ShaderGeometryModifierMisc)));
	}
	public string asdjghoergerg = "";
	public ShaderPass(string N){
		Name.Text = N;
		//ShaderLayersLightingDiffuse.IsLighting.On = true;
		//ShaderLayersLightingSpecular.IsLighting.On = true;
		//ShaderLayersLightingAmbient.IsLighting.On = true;
		//ShaderLayersLightingAll.IsLighting.On = true;
		//TechCull.Type = 1;
		//TechZTest.Type = 2;
		//TessellationType.Type = 2;
		//IndirectSubsurfaceScattering.Type = 0;
		//PokeInteraction.Type = 1;
		
		//Surfaces.Add(new ShaderSurfaceDiffuse());
		//Surfaces.Add(new ShaderSurfaceSpecular());
		Surfaces.Add((ShaderIngredient)ScriptableObject.CreateInstance(typeof(ShaderSurfaceDiffuse)));
		Surfaces[0].Link(this);
		Surfaces.Add((ShaderIngredient)ScriptableObject.CreateInstance(typeof(ShaderSurfaceSpecular)));
		Surfaces[1].Link(this);
		selectedIngredient = Surfaces[0];
		GeometryModifiers.Add((ShaderIngredient)ScriptableObject.CreateInstance(typeof(ShaderGeometryModifierMisc)));
	}
	public void GetMyShaderVars(List<ShaderVar> SVs){
		
		SVs.Add(Name);
		SVs.Add(Visible);
		//SVs.Add(TechLOD);
		//SVs.Add(TechCull);
		//SVs.Add(TechShaderTarget);
		
		foreach(ShaderIngredient surface in Surfaces){
			if (!surface.IsUsed())continue;
			surface.ConstantShaderVars(SVs);
			surface.GetMyShaderVars(SVs);
		}
		
		foreach(ShaderIngredient surface in GeometryModifiers){
			if (!surface.IsUsed())continue;
			surface.ConstantShaderVars(SVs);
			surface.GetMyShaderVars(SVs);
		}
	}
	public string GenerateCode(ShaderData SD){
		string ShaderCode;
		ShaderCode = GeneratePassGroup(SD);
		return ShaderCode;
	}
	public void AddPass(Func<ShaderData,ShaderIngredient[],string> f,Dictionary<string,object> cd){
		Passes.Add(f);
		if (cd==null)
			cd = new Dictionary<string,object>();
		PassesCD.Add(cd);
	}
	public void InsertPass(int index, Func<ShaderData,ShaderIngredient[],string> f,Dictionary<string,object> cd){
		Passes.Insert(index,f);
		if (cd==null)
			cd = new Dictionary<string,object>();
		PassesCD.Insert(index, cd);
	}
	public List<Func<ShaderData,ShaderIngredient[],string>> Passes = new List<Func<ShaderData,ShaderIngredient[],string>>();
	public List<Dictionary<string,object>> PassesCD = new List<Dictionary<string,object>>();
	public string GeneratePassGroup(ShaderData SD){
		//var watch = System.Diagnostics.Stopwatch.StartNew();

		List<ShaderIngredient> SIs = new List<ShaderIngredient>();
		SIs.AddRange(Surfaces);
		SIs.AddRange(GeometryModifiers);
		
		foreach(ShaderIngredient SI in SIs){
			SI.UpdateLinked(this);
		}
		Passes = new List<Func<ShaderData,ShaderIngredient[],string>>();
		PassesCD = new List<Dictionary<string,object>>();
		
		//Kinda dirty but 'eh
		if (GeometryModifiers.Count==0){
			AddGeometryModifier(typeof(ShaderGeometryModifierMisc));
		}
		(GeometryModifiers[0] as ShaderGeometryModifier).GenerateVertex(SD);
		
		if (SD.GenerateForwardBasePass)
			AddPass(GenerateForwardBasePass,null);
		if (SD.GenerateForwardAddPass)
			AddPass(GenerateForwardAddPass,null);
		
		if (SD.Base.RenderingPathTarget.Type==1&&SD.GenerateForwardDeferredPass)
			AddPass(GenerateDeferredPass,null);
		
		ShaderUtil.GetAllShaderVars();
		
		//int iasd = 0;
				
		List<ShaderIngredient> ingredientsList = new List<ShaderIngredient>();
		
		foreach(ShaderIngredient ingredient in Surfaces){
			if (!ingredient.IsUsed())continue;
			ingredientsList.Add(ingredient);
			//ingredients[iasd] = ingredient;
			//iasd++;
		}
		foreach(ShaderIngredient ingredient in GeometryModifiers){
			if (!ingredient.IsUsed())continue;
			ingredientsList.Add(ingredient);
			//ingredients[iasd] = ingredient;
			//iasd++;
		}
		ShaderIngredient[] ingredients = ingredientsList.ToArray();//new ShaderIngredient[Surfaces.Count+GeometryModifiers.Count];
		CheckIngredientNameClashes(ingredients);
		
		//watch.Stop();
		//Debug.Log("Prepare Passes"+watch.ElapsedMilliseconds.ToString());
		//watch = System.Diagnostics.Stopwatch.StartNew();
		foreach(ShaderIngredient ingredient in GeometryModifiers){
			if (!ingredient.IsUsed())continue;
			ShaderGeometryModifier geometryModifier = ingredient as ShaderGeometryModifier;
			if (geometryModifier!=null){
				geometryModifier.GeneratePass(SD,this);
			}
		}
		//watch.Stop();
		//Debug.Log("Prepare Geometry Ingredients"+watch.ElapsedMilliseconds.ToString());
		if (SD.GenerateShadowCaster)
			AddPass(GenerateShadowCasterPass,null);
		
		//watch = System.Diagnostics.Stopwatch.StartNew();
		StringBuilder ShaderCode = new StringBuilder(50000);
		for(int i = 0;i<Passes.Count;i++){
			SD.CustomData = PassesCD[i];
			//Debug.Log(SD.CustomData);
			//foreach(KeyValuePair<string,object> kv in SD.CustomData){
			//	Debug.Log(kv.Key+": "+kv.Value.ToString());
			//}
			ShaderCode.Append(Passes[i](SD,ingredients));
		}
		//watch.Stop();
		//Debug.Log("Generate Passes"+watch.ElapsedMilliseconds.ToString());
		//ShaderCode += GenerateForwardBasePass(SD);
		//ShaderCode += GenerateForwardAddPass(SD);
		return ShaderCode.ToString();
	}
	public void DecidePremultiplication(ShaderData SD){
		/*SD.PipelineStage = PipelineStage.Fragment;
		foreach(ShaderIngredient ingredient in Surfaces){
			ShaderSurface surface = ingredient as ShaderSurface;
			if (surface!=null){
				
				if (surface.ShaderSurfaceComponent == ShaderSurfaceComponent.Depth)
					SD.UsesDepth = true;
				if (surface.AlphaBlendMode.Type==0&&Surfaces.IndexOf(surface)!=0)//TODO: oh oh
					SD.UsesPremultipliedBlending = true;
				if (surface.AlphaBlendMode.Type!=1){
					foreach(ShaderLayerList SLL in surface.GetMyShaderLayerLists()){
						foreach(ShaderLayer SL in SLL){
							if (SL.AlphaBlendMode.Type==0&&SLL.SLs.IndexOf(SL)!=0)
								SD.UsesPremultipliedBlending = true;
						}
					}
				}
			}
		}
		if (SD.ShaderPass == ShaderPasses.ForwardAdd){
			SD.BlendFactorSrc = BlendFactor.One;
			SD.BlendFactorDest = BlendFactor.One;
		}
		//SD.OutputsPremultipliedBlending = SD.UsesPremultipliedBlending;
		foreach(ShaderIngredient ingredient in Surfaces){
			ShaderSurface surface = ingredient as ShaderSurface;
			if (surface!=null){
				if (surface.ShaderSurfaceComponent == ShaderSurfaceComponent.Color){
					string goodVariableName = surface.GenerateCode(SD.Copy());
					if (goodVariableName.Contains("outputColor")&&!goodVariableName.Contains("outputColor.a")&&!goodVariableName.Contains("outputColor. a")&&!goodVariableName.Contains("outputColor .a")&&!goodVariableName.Contains("outputColor . a")){
						bool isLast = true;
						for(int i = Surfaces.Count-1;i>=0;i--){//I know this is all really bad code...but 'eh it's not performance critical I guess...
							if (Surfaces[i]==ingredient)
								break;
							else{
								ShaderSurface surface2 = ingredient as ShaderSurface;
								if (surface2!=null)
									if (surface2.ShaderSurfaceComponent == ShaderSurfaceComponent.Color)
										isLast = false;
							}
						}
						SD.UsesPremultipliedBlending = false;
						//if (isLast)
						//	SD.OutputsPremultipliedBlending = false;
					}
				}
			}
		}*/
		foreach (ShaderInput SI in SD.Base.ShaderInputs){
			if (SI.Mask.Obj!=null){
				SD.UsedMasksVertex.Add(((ShaderLayerList)SI.Mask.Obj));
				SD.UsedMasksFragment.Add(((ShaderLayerList)SI.Mask.Obj));
			}
		}
		
		if (SD.ShaderPass == ShaderPasses.ForwardAdd){
			SD.BlendFactorSrc = BlendFactor.One;
			SD.BlendFactorDest = BlendFactor.One;
		}
		int blerg = -1;
		foreach(ShaderIngredient ingredient in Surfaces){
			foreach(ShaderLayerList SLL in ingredient.GetMyShaderLayerLists()){
				foreach(ShaderLayer SL in SLL){
					if (SL.MixAmount.Get()=="0")continue;
					
					if (SL.Stencil.Obj!=null){
						SD.UsedMasksFragment.Add((ShaderLayerList)SL.Stencil.Obj);
					}
				}
			}
		}
		foreach(ShaderIngredient ingredient in GeometryModifiers){
			foreach(ShaderLayerList SLL in ingredient.GetMyShaderLayerLists()){
				foreach(ShaderLayer SL in SLL){
					if (SL.MixAmount.Get()=="0")continue;
					
					if (SL.Stencil.Obj!=null){
						SD.UsedMasksVertex.Add((ShaderLayerList)SL.Stencil.Obj);
					}
				}
			}
		}
		
		List<ShaderVar> SVs = new List<ShaderVar>();
		foreach(ShaderIngredient surface in Surfaces){
			if (!surface.IsUsed())continue;
			surface.ConstantShaderVars(SVs);
			surface.GetMyShaderVars(SVs);
			foreach(ShaderLayerList SLL in surface.GetMyShaderLayerLists()){
				foreach(ShaderLayer SL in SLL){
					SL.UpdateShaderVars(true);
					SVs.AddRange(SL.ShaderVars);
				}
			}
		}
		foreach(ShaderVar SV in SVs){
			if (SV.Obj!=null){
				ShaderLayerList asd = SV.Obj as ShaderLayerList;
				if (asd!=null){
					SD.UsedMasksFragment.Add(asd);
					NestMasks(SD,asd);
				}
			}
		}
		
		SVs.Clear();
		foreach(ShaderIngredient surface in GeometryModifiers){
			if (!surface.IsUsed())continue;
			surface.ConstantShaderVars(SVs);
			surface.GetMyShaderVars(SVs);
			foreach(ShaderLayerList SLL in surface.GetMyShaderLayerLists()){
				foreach(ShaderLayer SL in SLL){
					SL.UpdateShaderVars(true);
					SVs.AddRange(SL.ShaderVars);
				}
			}
		}
		foreach(ShaderVar SV in SVs){
			if (SV.Obj!=null){
				ShaderLayerList asd = SV.Obj as ShaderLayerList;
				if (asd!=null){
					SD.UsedMasksVertex.Add(asd);
					NestMasks(SD,asd);
				}
			}
		}
		
		
		foreach(ShaderIngredient ingredient in Surfaces){
			ingredient.FirstDisplayed = false;
			if (!ingredient.IsUsed())continue;
			ShaderSurface surface = ingredient as ShaderSurface;
			if (surface!=null){
				if (surface.ShaderSurfaceComponent == ShaderSurfaceComponent.Color){
					if (blerg==-1){
						surface.FirstDisplayed = true;
						blerg = 1;
					}
				}
				surface.CalculateTransparency();
			}
		}
		
		 
		bool ifoundit = false;
		SD.UsesPremultipliedBlending = true;
		for(int i = GeometryModifiers.Count-1; i>=0;i--){
			ShaderGeometryModifier geometryModifier = GeometryModifiers[i] as ShaderGeometryModifier;
			if (geometryModifier!=null){
				geometryModifier.CalculatePremultiplication(SD);
			}
		}
		ifoundit = !SD.UsesPremultipliedBlending;
		int firstColorThingo = -1;
		for(int i = Surfaces.Count-1; i>=0;i--){
			ShaderSurface surface = Surfaces[i] as ShaderSurface;
			if (surface!=null){
				
				if (surface.ShaderSurfaceComponent == ShaderSurfaceComponent.Color){
					if (firstColorThingo==-1)
						firstColorThingo = i;
					string goodVariableName = surface.GenerateCode(SD.Copy());
					if (goodVariableName.Contains("outputColor")&&!goodVariableName.Contains("outputColor.a")&&!goodVariableName.Contains("outputColor. a")&&!goodVariableName.Contains("outputColor .a")&&!goodVariableName.Contains("outputColor . a")){
						//if (i==Surfaces.Count-1)
						if (i==firstColorThingo)
							SD.UsesPremultipliedBlending = false;
						ifoundit = true;
						break;
					}
				}
				surface.OutputPremultiplied = !ifoundit&&surface.UseAlphaGenerate;
			}
		}
		for(int i = 0; i<Surfaces.Count;i++){
			ShaderSurface surface = Surfaces[i] as ShaderSurface;
			if (surface!=null){
				if (surface.ShaderSurfaceComponent == ShaderSurfaceComponent.Depth)
					SD.UsesDepth = true;
				if (surface.OutputPremultiplied){
					surface.CalculatePremultiplication();
				}
			}
		}
		
		foreach(ShaderIngredient ingredient in GeometryModifiers){
			if (!ingredient.IsUsed())continue;
			ShaderGeometryModifier geometryModifier = ingredient as ShaderGeometryModifier;
			if (geometryModifier!=null){
				if (geometryModifier.HasFlag(ShaderGeometryModifierComponent.Tessellation))
					SD.UsesTessellation = true;
			}
		}
	}
	public void NestMasks(ShaderData SD, ShaderLayerList SLL){
		List<ShaderVar> SVs = new List<ShaderVar>();
		SVs.Clear();
		foreach(ShaderLayer SL in SLL){
			SL.UpdateShaderVars(true);
			SVs.AddRange(SL.ShaderVars);
		}
		foreach(ShaderVar SV in SVs){
			if (SV.Obj!=null){
				ShaderLayerList asd = SV.Obj as ShaderLayerList;
				if (asd!=null){
					SD.UsedMasksVertex.Add(asd);
					NestMasks(SD, asd);
				}
			}
		}
	}
	public void CheckIngredientNameClashes(ShaderIngredient[] ingredients){
		foreach(ShaderIngredient ingredient in ingredients){
			bool NameCollision = false;
			while (1==1){
				NameCollision = false;
				foreach (ShaderIngredient ingredient2 in ingredients){
					if (ShaderUtil.CodeName(ingredient._Name)==ShaderUtil.CodeName(ingredient2._Name)&&ingredient!=ingredient2){
						NameCollision = true;
						ingredient2._Name += " 2";
					}
				}
				if (NameCollision == false)
				break;
			}
		}
	}
	public string GenerateIngredientGlobalFunctions(ShaderData SD, ShaderIngredient[] ingredients){
		StringBuilder IngredientGlobalFunctions = new StringBuilder();
		foreach(string TypeS in ShaderSandwich.NonDumbShaderSurfaces.Keys){
			//bool IsUsed = false;
			ShaderIngredient blerg = null;
			List<ShaderIngredient> blergs = new List<ShaderIngredient>();
			foreach (ShaderIngredient ingredient in ingredients){
				if (!ingredient.IsUsed())continue;
				if (ingredient.GetType().Name == TypeS){
					blerg = ingredient;
					blergs.Add(blerg);
				}
			}
			if (blerg!=null){
				string func = blerg.GenerateGlobalFunctions(SD,blergs);
				foreach(ShaderVertexInterpolator i in SD.Interpolators){
					if (func.IndexOf(i.Name)>=0){
						i.InFragment = true;
						if ((i.FragmentCode.Contains("vtp."+i.Name)||i.FragmentCode.Length==0)){
							i.Transfer = true;
							i.InVertex = true;
						}
						foreach(ShaderVertexInterpolator ii in i.ReliesOn){
							ii.InVertex = true;
						}
					}
				}
				IngredientGlobalFunctions.Append(func);
			}
		}
		foreach(string TypeS in ShaderSandwich.NonDumbShaderGeometryModifiers.Keys){
			//bool IsUsed = false;
			ShaderIngredient blerg = null;
			List<ShaderIngredient> blergs = new List<ShaderIngredient>();
			foreach (ShaderIngredient ingredient in ingredients){
				if (!ingredient.IsUsed())continue;
				if (ingredient.GetType().Name == TypeS){
					blerg = ingredient;
					blergs.Add(blerg);
				}
			}
			if (blerg!=null){
				string func = blerg.GenerateGlobalFunctions(SD,blergs);
				foreach(ShaderVertexInterpolator i in SD.Interpolators){
					if (func.IndexOf(i.Name)>=0){
						i.InVertex = true;
						foreach(ShaderVertexInterpolator ii in i.ReliesOn){
							ii.InVertex = true;
						}
					}
				}
				IngredientGlobalFunctions.Append(func);
			}
			//if (blerg!=null)
			//	IngredientGlobalFunctions.Append(blerg.GenerateGlobalFunctions(SD,blergs));
		}
		return IngredientGlobalFunctions.ToString();
	}
	public void DependancyCheckyThing(ShaderData SD, string FragmentShader, string VertexShader){
			foreach(ShaderVertexInterpolator i in SD.Interpolators){
				/*if (SurfaceFunctions.IndexOf(i.Name)>=0){
					i.Used = true;
					i.InVertex = true;
					//Debug.Log(i.Name);
					foreach(ShaderVertexInterpolator ii in i.ReliesOn){
						//Debug.Log(ii.Name);
						ii.InVertex = true;
					}
				}*/
				if (FragmentShader.IndexOf(i.Name)>=0){
					i.InFragment = true;
					if ((i.FragmentCode.Contains("vtp."+i.Name)||i.FragmentCode.Length==0)){
						i.InVertex = true;
						i.Transfer = true;
					}
					//Debug.Log(i.Name);
					foreach(ShaderVertexInterpolator ii in i.ReliesOn){
						if (i.FragmentCode.Contains("vtp."+ii.Name)||i.FragmentCode.Contains("vd."+ii.Name)){
							ii.InFragment = true;
							ii.Transfer = true;
						}
						//Debug.Log(ii.Name);
						ii.InVertex = true;
						foreach(ShaderVertexInterpolator iii in ii.ReliesOn){
							//Debug.Log(iii.Name);
							iii.InVertex = true;		
							foreach(ShaderVertexInterpolator iiii in iii.ReliesOn){
								iiii.InVertex = true;
							}
						}
					}
				}
				if (VertexShader.IndexOf(i.Name)>=0){
					i.InVertex = true;
					//if (SD.ShaderPassIsShadowCaster)
					//Debug.Log(i.Name);
					foreach(ShaderVertexInterpolator ii in i.ReliesOn){
						//if (SD.ShaderPassIsShadowCaster)
						//Debug.Log(ii.Name);
						ii.InVertex = true;
						foreach(ShaderVertexInterpolator iii in ii.ReliesOn){
							iii.InVertex = true;		
							foreach(ShaderVertexInterpolator iiii in iii.ReliesOn){
								iiii.InVertex = true;
							}
						}
					}
				}
			}
	}
	public void CreateInterpolators(ShaderData SD, ref string SurfaceFunctions, ref string FragmentShader, ref string VertexShader, ref string IngredientGlobalFunctions, ShaderIngredient[] ingredients){
		//if (!ShaderSandwich.Instance.ViewLayerNames)
		//return;
		//bool TangentCollection = false;
		//	if (FragmentShader.IndexOf("TtoWSpace")>=0)
		//		TangentCollection = true;
		//	if (SurfaceFunctions.IndexOf("TtoWSpace")>=0)
		//		TangentCollection = true;
		//List<ShaderVertexInterpolator> Interpolators = new List<ShaderVertexInterpolator>();
		//if (TangentCollection)
		SD.Interpolators.Add(new ShaderVertexInterpolator("worldPos", ShaderVertexInterpolatorType.Float3, 
		new Dictionary<CoordSpace,string>{{CoordSpace.Object,"mul(_Object2World, v.vertex).xyz"}}
		, new ShaderVertexInterpolator[0], new string[]{"vertex"}));//0
		SD.Interpolators[SD.Interpolators.Count-1].ParallaxDisplacementVariable = "worldViewDir*-1";///TOODO: Could break old shaders relying on it being offset by worldViewDir
		
		if (!SD.ShaderPassIsShadowCaster)
			SD.Interpolators.Add(new ShaderVertexInterpolator("position", ShaderVertexInterpolatorType.Position, new Dictionary<CoordSpace,string>{{CoordSpace.Object,"mul(UNITY_MATRIX_MVP, v.vertex)"},{CoordSpace.World,"mul(UNITY_MATRIX_VP, float4(vd.worldPos,1))"}}, new ShaderVertexInterpolator[0], new string[]{"vertex"}));//1
		else
			SD.Interpolators.Add(new ShaderVertexInterpolator("position", ShaderVertexInterpolatorType.Position, new Dictionary<CoordSpace,string>{{CoordSpace.Object,"0;\n	TRANSFER_SHADOW_CASTER_NOPOS(vd,vd.position)"}}, new ShaderVertexInterpolator[0], new string[]{"vertex"}));//1
		SD.Interpolators[SD.Interpolators.Count-1].Transfer = true;
		SD.Interpolators[SD.Interpolators.Count-1].InVertex = true;

		SD.Interpolators.Add(new ShaderVertexInterpolator("localPos", ShaderVertexInterpolatorType.Float3, 
		new Dictionary<CoordSpace,string>{{CoordSpace.Object,"v.vertex.xyz"},{CoordSpace.World,"mul(_World2Object,float4(vd.worldPos,1)).xyz"}}
		, new ShaderVertexInterpolator[0], new string[]{"vertex"}));//2
		
		SD.Interpolators.Add(new ShaderVertexInterpolator("worldNormal", ShaderVertexInterpolatorType.Float3, 
		new Dictionary<CoordSpace,string>{{CoordSpace.Object,"UnityObjectToWorldNormalNew(v.normal)"}}, "normalize(vtp.worldNormal)","",new ShaderVertexInterpolator[0], new string[]{"normal"}));//3
		
		SD.Interpolators.Add(new ShaderVertexInterpolator("localNormal", ShaderVertexInterpolatorType.Float3, 
		new Dictionary<CoordSpace,string>{{CoordSpace.Object,"v.normal.xyz"},{CoordSpace.World,"UnityWorldToObjectDir(vd.worldNormal)"}}, "normalize(vtp.localNormal)","", new ShaderVertexInterpolator[0], new string[]{"normal"}));//4
		
		SD.Interpolators.Add(new ShaderVertexInterpolator("worldTangent", ShaderVertexInterpolatorType.Float3, 
		new Dictionary<CoordSpace,string>{{CoordSpace.Object,"UnityObjectToWorldNormalNew(v.tangent)"}}, "normalize(vtp.worldTangent)","", new ShaderVertexInterpolator[0], new string[]{"tangent"}));//5
		
		SD.Interpolators.Add(new ShaderVertexInterpolator("localTangent", ShaderVertexInterpolatorType.Float3, 
		new Dictionary<CoordSpace,string>{{CoordSpace.Object,"v.tangent.xyz"},{CoordSpace.World,"UnityWorldToObjectDir(vd.worldTangent)"}}, "normalize(vtp.localTangent)","", new ShaderVertexInterpolator[0], new string[]{"tangent"}));//6
		
		SD.Interpolators.Add(new ShaderVertexInterpolator("worldBitangent", ShaderVertexInterpolatorType.Float3, 
		new Dictionary<CoordSpace,string>{{CoordSpace.Object,"cross(vd.worldNormal, vd.worldTangent) * v.tangent.w * unity_WorldTransformParams.w"}}, "normalize(vtp.worldBitangent)","", new ShaderVertexInterpolator[]{SD.Interpolators[5],SD.Interpolators[3]}, new string[0]));//7
		
		SD.Interpolators.Add(new ShaderVertexInterpolator("localBitangent", ShaderVertexInterpolatorType.Float3, 
		new Dictionary<CoordSpace,string>{{CoordSpace.Object,"cross(v.normal.xyz, v.tangent.xyz) * v.tangent.w"},{CoordSpace.World,"UnityWorldToObjectDir(cross(vd.worldNor.xyz, vd.worldTangent.xyz) * v.tangent.w)"}}, "normalize(vtp.localBitangent)","", new ShaderVertexInterpolator[0], new string[0]));//8
		
		SD.Interpolators.Add(new ShaderVertexInterpolator("screenPos", ShaderVertexInterpolatorType.Float3, 
		new Dictionary<CoordSpace,string>{{CoordSpace.Object,"ComputeScreenPos (mul (UNITY_MATRIX_MVP, v.vertex)).xyw"},{CoordSpace.World,"ComputeScreenPos (mul (UNITY_MATRIX_VP, float4(vd.worldPos,1))),xyw"}},"float3(vtp.screenPos.xy/vtp.screenPos.z,vtp.screenPos.z)", new ShaderVertexInterpolator[0], new string[]{"v.vertex"}));//9
		//new Dictionary<CoordSpace,string>{{CoordSpace.Object,"ComputeScreenPos (mul (UNITY_MATRIX_MVP, v.vertex))"},{CoordSpace.World,"ComputeScreenPos (mul (UNITY_MATRIX_VP, float4(vd.worldPos,1)))"}},"float4(vtp.screenPos.xyz/vtp.screenPos.w,vtp.screenPos.w)", new ShaderVertexInterpolator[0], new string[]{"v.vertex"}));//9
		if (!SD.ShaderPassIsShadowCaster)
			SD.Interpolators.Add(new ShaderVertexInterpolator("worldViewDir", ShaderVertexInterpolatorType.Float3, 
		new Dictionary<CoordSpace,string>{{CoordSpace.Object,"normalize(UnityWorldSpaceViewDir(vd.worldPos))"},{CoordSpace.World,"normalize(UnityWorldSpaceViewDir(vd.worldPos))"}}, "normalize(UnityWorldSpaceViewDir(vd.worldPos))","", new ShaderVertexInterpolator[]{SD.Interpolators[0]}, new string[0]));//10
		else{
			string StupidWorkaround = 
"				#ifdef SHADOWS_DEPTH\n"+
"					//Sorry, no support for directional lights unless Screen Space Shadows are turned off...too much of a pain :/\n"+
"					vd.worldViewDir = normalize(UnityWorldSpaceLightDir(vd.worldPos));\n"+
"					if (dot(vd.worldViewDir,vd.worldNormal)<0)\n"+
"						vd.worldViewDir *= -1;\n"+
"				#else\n"+
"					vd.worldViewDir = normalize(UnityWorldSpaceLightDir(vd.worldPos));\n"+
"				#endif\n";
			
			
			SD.Interpolators.Add(new ShaderVertexInterpolator("worldViewDir", ShaderVertexInterpolatorType.Float3, 
		new Dictionary<CoordSpace,string>{{CoordSpace.Object,"0;\n"+StupidWorkaround},{CoordSpace.World,"0;\n"+StupidWorkaround}}, "0;\n"+StupidWorkaround,"", new ShaderVertexInterpolator[]{SD.Interpolators[0]}, new string[0]));//10
//"		vd.worldViewDir *= -1;\n"}}, "0;\n"+"	vd.worldViewDir = normalize(UnityWorldSpaceLightDir(vd.worldPos));\n"+
//"	if (dot(vd.worldViewDir,vd.worldNormal)<0)\n"+
//"		vd.worldViewDir *= -1;\n","", new ShaderVertexInterpolator[]{SD.Interpolators[0]}, new string[0]));//10
		}

		SD.Interpolators.Add(new ShaderVertexInterpolator("worldRefl", ShaderVertexInterpolatorType.Float3, 
		new Dictionary<CoordSpace,string>{{CoordSpace.Object,"reflect(-vd.worldViewDir, vd.worldNormal)"},{CoordSpace.World,"reflect(-vd.worldViewDir, vd.worldNormal)"}}, "reflect(-vd.worldViewDir, vd.worldNormal)","", new ShaderVertexInterpolator[]{SD.Interpolators[3],SD.Interpolators[10]}, new string[0]));//11
		
		//TtoWSpace
		SD.Interpolators.Add(new ShaderVertexInterpolator("TtoWSpaceX", ShaderVertexInterpolatorType.Float4, "float4(vd.worldTangent.x, vd.worldBitangent.x, vd.worldNormal.x, vd.worldPos.x)", new ShaderVertexInterpolator[]{SD.Interpolators[5],SD.Interpolators[7],SD.Interpolators[3],SD.Interpolators[0]}, new string[0]));//12
		SD.Interpolators.Add(new ShaderVertexInterpolator("TtoWSpaceY", ShaderVertexInterpolatorType.Float4, "float4(vd.worldTangent.y, vd.worldBitangent.y, vd.worldNormal.y, vd.worldPos.y)", new ShaderVertexInterpolator[]{SD.Interpolators[5],SD.Interpolators[7],SD.Interpolators[3],SD.Interpolators[0]}, new string[0]));//13
		SD.Interpolators.Add(new ShaderVertexInterpolator("TtoWSpaceZ", ShaderVertexInterpolatorType.Float4, "float4(vd.worldTangent.z, vd.worldBitangent.z, vd.worldNormal.z, vd.worldPos.z)", new ShaderVertexInterpolator[]{SD.Interpolators[5],SD.Interpolators[7],SD.Interpolators[3],SD.Interpolators[0]}, new string[0]));//14
		
		SD.Interpolators.Add(new ShaderVertexInterpolator("vertexColor", ShaderVertexInterpolatorType.Color, "v.color", new ShaderVertexInterpolator[]{}, new string[]{"color"}));//15
		SD.Interpolators.Add(new ShaderVertexInterpolator("genericTexcoord", ShaderVertexInterpolatorType.Float2, "v.texcoord", new ShaderVertexInterpolator[]{}, new string[]{"texcoord"}));//16
		SD.Interpolators[SD.Interpolators.Count-1].ParallaxDisplacementVariable = "tangentViewDir.xy";
		SD.Interpolators.Add(new ShaderVertexInterpolator("genericTexcoord2", ShaderVertexInterpolatorType.Float2, "v.texcoord1", new ShaderVertexInterpolator[]{}, new string[]{"texcoord1"}));//17
		SD.Interpolators.Add(new ShaderVertexInterpolator("genericTexcoord3", ShaderVertexInterpolatorType.Float2, "v.texcoord2", new ShaderVertexInterpolator[]{}, new string[]{"texcoord2"}));//17
		SD.Interpolators.Add(new ShaderVertexInterpolator("genericTexcoord4", ShaderVertexInterpolatorType.Float2, "v.texcoord3", new ShaderVertexInterpolator[]{}, new string[]{"texcoord3"}));//17
		
		foreach (ShaderInput SI in ShaderSandwich.Instance.OpenShader.ShaderInputs){
			if (SI.Type==ShaderInputTypes.Image){
				SD.Interpolators.Add(new ShaderVertexInterpolator("uv"+SI.Get(), ShaderVertexInterpolatorType.Float2, "TRANSFORM_TEX(v.texcoord, "+SI.Get()+")", new ShaderVertexInterpolator[]{}, new string[]{"texcoord"}));
				SD.Interpolators[SD.Interpolators.Count-1].ParallaxDisplacementVariable = "tangentViewDir.xy";
			}
		}
		
		SD.Interpolators.Add(new ShaderVertexInterpolator("sh", ShaderVertexInterpolatorType.Float3, "0;\n"+
		"// SH/ambient and vertex lights\n"+
		"#ifdef LIGHTMAP_OFF\n"+
		"	vd.sh = 0;\n"+
		"	// Approximated illumination from non-important point lights\n"+
		"	#ifdef VERTEXLIGHT_ON\n"+
		"		vd.sh += Shade4PointLights (\n"+
		"		unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,\n"+
		"		unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,\n"+
		"		unity_4LightAtten0, vd.worldPos, vd.worldNormal);\n"+
		"	#endif\n"+
		"	vd.sh = ShadeSHPerVertex (vd.worldNormal, vd.sh);\n"+
		"#endif // LIGHTMAP_OFF\n"
		, new ShaderVertexInterpolator[]{SD.Interpolators[0],SD.Interpolators[3]}, new string[0], new string[]{"#if UNITY_SHOULD_SAMPLE_SH"}, new string[]{"#endif"}));
		
		SD.Interpolators.Add(new ShaderVertexInterpolator("tangentViewDir", ShaderVertexInterpolatorType.Float3, 
		new Dictionary<CoordSpace,string>{{CoordSpace.Object,"normalize(mul(ObjSpaceViewDir(v.vertex),float3x3( v.tangent.xyz, cross( normalize(v.normal), normalize(v.tangent.xyz) ) * v.tangent.w, v.normal )))*float3(-1,-1,1)"},{CoordSpace.World,"mul(vd.worldViewDir,float3x3(vd.TtoWSpaceX.xyz,vd.TtoWSpaceY.xyz,vd.TtoWSpaceZ.xyz))*float3(-1,-1,1)"}}, "mul(vd.worldViewDir,float3x3(vd.TtoWSpaceX.xyz,vd.TtoWSpaceY.xyz,vd.TtoWSpaceZ.xyz))*float3(-1,-1,1)","", new ShaderVertexInterpolator[]{SD.Interpolators[12],SD.Interpolators[13],SD.Interpolators[14]}, new string[0]));//10
		
		SD.Interpolators.Add(new ShaderVertexInterpolator("SHADOW_COORDS", ShaderVertexInterpolatorType.Inline, 
		"TRANSFER_SHADOW(vtp);", new ShaderVertexInterpolator[0], new string[0], new string[]{"#define pos position"}, new string[]{"#undef pos"}));//10
		
		SD.Interpolators.Add(new ShaderVertexInterpolator("UNITY_FOG_COORDS", ShaderVertexInterpolatorType.Inline, 
		"UNITY_TRANSFER_FOG(vtp,vtp.pos);", new ShaderVertexInterpolator[0], new string[0], new string[]{"#define pos position"}, new string[]{"#undef pos"}));//10
		//SD.Interpolators[SD.Interpolators.Count-1].Transfer = true;
		//SD.Interpolators[SD.Interpolators.Count-1].InFragment = true;
		//SD.Interpolators[SD.Interpolators.Count-1].InVertex = true;
		
		SD.Interpolators.Add(new ShaderVertexInterpolator("lmap", ShaderVertexInterpolatorType.Float4, 
		"0;\n"+
		"#ifndef DYNAMICLIGHTMAP_OFF\n"+
		"	vd.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;\n"+
		"#endif\n"+
		"#ifndef LIGHTMAP_OFF\n"+
		"	vd.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;\n"+
		"#endif\n"
, new ShaderVertexInterpolator[0], new string[0], new string[]{"#if (!defined(LIGHTMAP_OFF))||(SHADER_TARGET >= 30)"}, new string[]{"#endif"}));//10

		if (SD.ShaderPassIsShadowCaster){
			SD.Interpolators.Add(new ShaderVertexInterpolator("vec", ShaderVertexInterpolatorType.Float3, ""
		, new ShaderVertexInterpolator[]{SD.Interpolators[0],SD.Interpolators[3]}, new string[0], new string[]{"#ifdef SHADOWS_CUBE"}, new string[]{"#endif"}));
			SD.Interpolators[SD.Interpolators.Count-1].Transfer = true;
			SD.Interpolators[SD.Interpolators.Count-1].InVertex = true;
			SD.Interpolators[SD.Interpolators.Count-1].InFragment = true;
			
			SD.Interpolators.Add(new ShaderVertexInterpolator("hpos", ShaderVertexInterpolatorType.Float4, ""
		, new ShaderVertexInterpolator[]{SD.Interpolators[0],SD.Interpolators[3]}, new string[0], new string[]{"#ifndef SHADOWS_CUBE","#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE"}, new string[]{"#endif","#endif"}));
			SD.Interpolators[SD.Interpolators.Count-1].Transfer = true;
			SD.Interpolators[SD.Interpolators.Count-1].InVertex = true;
			SD.Interpolators[SD.Interpolators.Count-1].InFragment = true;
		}
		
		/*foreach(ShaderVertexInterpolator i in SD.Interpolators){
			if (IngredientGlobalFunctions.IndexOf(i.Name)>=0){
				i.Used = true;
				i.InVertex = true;
				foreach(ShaderVertexInterpolator ii in i.ReliesOn){
					ii.InVertex = true;
				}
			}
		}*/
		int asdasdas  = SD.GeometryModifierIndex;
		SurfaceFunctions = GenerateSurfaceFunctions(SD);
		FragmentShader = GenerateFragment(SD);
		VertexShader = 	GenerateVertex(SD);
		IngredientGlobalFunctions = GenerateIngredientGlobalFunctions(SD,ingredients);
		
		for (int iter = 1; iter>0;iter--){//Iterate to solve dependencies, yes I'm lazy...
			DependancyCheckyThing(SD,FragmentShader,VertexShader);
			SurfaceFunctions = GenerateSurfaceFunctions(SD);
			
			//Debug.Log(SD.Interpolators[12].Name);
			//Debug.Log(SD.Interpolators[12].InVertex);
			//Debug.Log(SD.Interpolators[12].Transfer);
			//Debug.Log(SD.Interpolators[12].InFragment);
			if (SD.Interpolators[12].InFragment == true){//TTWoasdja 12,13,14
			//Debug.Log("asd");
				SD.Interpolators[3].FragmentCode = "half3(vtp.TtoWSpaceX.z,vtp.TtoWSpaceY.z,vtp.TtoWSpaceZ.z)";
				SD.Interpolators[3].InVertex = true;
				SD.Interpolators[3].Transfer = false;
				SD.Interpolators[5].FragmentCode = "half3(vtp.TtoWSpaceX.x,vtp.TtoWSpaceY.x,vtp.TtoWSpaceZ.x)";
				SD.Interpolators[5].Transfer = false;
				SD.Interpolators[5].InVertex = true;
				SD.Interpolators[7].FragmentCode = "half3(vtp.TtoWSpaceX.y,vtp.TtoWSpaceY.y,vtp.TtoWSpaceZ.y)";
				SD.Interpolators[7].Transfer = false;
				SD.Interpolators[7].InVertex = true;
				SD.Interpolators[0].FragmentCode = "half3(vtp.TtoWSpaceX.w,vtp.TtoWSpaceY.w,vtp.TtoWSpaceZ.w)";
				SD.Interpolators[0].Transfer = false;
				SD.Interpolators[0].InVertex = true;
			}
			
			
			FragmentShader = GenerateFragment(SD);
			SD.GeometryModifierIndex = asdasdas;
			VertexShader = 	GenerateVertex(SD);
		}
		/*if (SD.ShaderPassIsShadowCaster)
		foreach(ShaderVertexInterpolator i in SD.Interpolators){
			Debug.Log(i.Name);
			Debug.Log(i.InVertex);
			Debug.Log(i.Transfer);
			Debug.Log(i.InFragment);
			//if (i.InFragment||i.Transfer){
			//	if (!(i.FragmentCode.Contains(i.Name)||i.FragmentCode.Length==0)){
			//		i.Transfer = false;
			//	}
			//}
		}*/
		SD.GeometryModifierIndex = asdasdas;
		//string Everything = VertexShader+FragmentShader+SurfaceFunctions;
		string Everything = VertexShader+SurfaceFunctions;
		
		if (Everything.IndexOf("v.vertex")>-1)
			SD.VertexInputs.Add(new ShaderVertexInterpolator("vertex", ShaderVertexInterpolatorType.Float4,"",new ShaderVertexInterpolator[0], new string[]{" : POSITION","float4(vd.worldPos,1)"}));
		if (Everything.IndexOf("v.tangent")>-1)
			SD.VertexInputs.Add(new ShaderVertexInterpolator("tangent", ShaderVertexInterpolatorType.Float4,"",new ShaderVertexInterpolator[0], new string[]{" : TANGENT"}));
		if (Everything.IndexOf("v.normal")>-1)
			SD.VertexInputs.Add(new ShaderVertexInterpolator("normal", ShaderVertexInterpolatorType.Float3,"",new ShaderVertexInterpolator[0], new string[]{" : NORMAL","vd.worldNormal"}));
		if (Everything.IndexOf("v.texcoord")>-1)
			SD.VertexInputs.Add(new ShaderVertexInterpolator("texcoord", ShaderVertexInterpolatorType.Float4,"",new ShaderVertexInterpolator[0], new string[]{" : TEXCOORD0"}));
		if (Everything.IndexOf("v.texcoord1")>-1)
			SD.VertexInputs.Add(new ShaderVertexInterpolator("texcoord1", ShaderVertexInterpolatorType.Float4,"",new ShaderVertexInterpolator[0], new string[]{" : TEXCOORD1"}));
		if (Everything.IndexOf("v.texcoord2")>-1)
			SD.VertexInputs.Add(new ShaderVertexInterpolator("texcoord2", ShaderVertexInterpolatorType.Float4,"",new ShaderVertexInterpolator[0], new string[]{" : TEXCOORD2"}));
		if (Everything.IndexOf("v.texcoord3")>-1)
			SD.VertexInputs.Add(new ShaderVertexInterpolator("texcoord3", ShaderVertexInterpolatorType.Float4,"",new ShaderVertexInterpolator[0], new string[]{" : TEXCOORD3"}));
		if (Everything.IndexOf("v.texcoord4")>-1||Everything.IndexOf("v.texcoord5")>-1){
			///VertexShaderInput+="	#if defined(SHADER_API_XBOX360)\n";
			if (Everything.IndexOf("v.texcoord4")>-1)
				SD.VertexInputs.Add(new ShaderVertexInterpolator("texcoord4", ShaderVertexInterpolatorType.Half4,"",new ShaderVertexInterpolator[0], new string[]{" : TEXCOORD4"}));
			if (Everything.IndexOf("v.texcoord5")>-1)
				SD.VertexInputs.Add(new ShaderVertexInterpolator("texcoord5", ShaderVertexInterpolatorType.Half4,"",new ShaderVertexInterpolator[0], new string[]{" : TEXCOORD5"}));
			//VertexShaderInput+="	#endif\n";
		}
		if (Everything.IndexOf("v.color")>-1)
			SD.VertexInputs.Add(new ShaderVertexInterpolator("color", ShaderVertexInterpolatorType.Fixed4,"",new ShaderVertexInterpolator[0], new string[]{" : COLOR"}));
		
		SD.GeometryModifierIndex = asdasdas;
		VertexShader = 	GenerateVertex(SD);
		
		if (SD.Interpolators[3].InVertex||SD.Interpolators[3].InFragment||SD.Interpolators[5].InVertex||SD.Interpolators[5].InFragment){
			SD.AdditionalFunctions.Add("	//From UnityCG.inc, Unity 2017.01 - works better than any of the earlier ones\n	inline float3 UnityObjectToWorldNormalNew( in float3 norm ){\n"+
				"		#ifdef UNITY_ASSUME_UNIFORM_SCALING\n"+
				"			return UnityObjectToWorldDir(norm);\n"+
				"		#else\n"+
				"			// mul(IT_M, norm) => mul(norm, I_M) => {dot(norm, I_M.col0), dot(norm, I_M.col1), dot(norm, I_M.col2)}\n"+
				"			return normalize(mul(norm, (float3x3)_World2Object));\n"+
				"		#endif\n"+
				"	}");
			}
	}
	public string GenerateVertexShaderInput(ShaderData SD){
		string VertexShaderInput = "struct VertexShaderInput{\n";
		
		foreach(ShaderVertexInterpolator i in SD.VertexInputs){
			if (i.ShaderVertexInterpolatorType!=ShaderVertexInterpolatorType.Inline)
				VertexShaderInput+=i.ImLazy()+i.Name+i.vertexData[0]+";\n";
		}
		
		VertexShaderInput +="};\n";
		return VertexShaderInput;
	}
	public void GenerateTessellation(ShaderData SD, ref string TessellationShaderInput, ref string HullControlPointsShader, ref string HullTessellationFactorsShader, ref string DomainShader, ref string TessellationFactors){
		SD.PipelineStage = PipelineStage.Domain;
		if (SD.UsesTessellation){
			TessellationShaderInput = "struct TessellationShaderInput{\n";
			if (SD.CoordSpace==CoordSpace.World)
				TessellationShaderInput+="	//Sent in world space as an optimization\n";
			
			foreach(ShaderVertexInterpolator i in SD.VertexInputs){
				if (i.Name=="vertex")
					TessellationShaderInput+=i.ImLazy()+i.Name+" : INTERNALTESSPOS;\n";
				else
					TessellationShaderInput+=i.ImLazy()+i.Name+i.vertexData[0]+";\n";
			}
			
			TessellationShaderInput +="};\n";
			
			
			string hcps = "";
			foreach(ShaderGeometryModifier geometryModifier in GeometryModifiers){
				if (geometryModifier.HasFlag(ShaderGeometryModifierComponent.Tessellation)){
					hcps += geometryModifier.GenerateHullControlPoints(SD);
				}
			}
			
			string htfs = "";
			foreach(ShaderGeometryModifier geometryModifier in GeometryModifiers){
				if (geometryModifier.HasFlag(ShaderGeometryModifierComponent.Tessellation)){
					htfs += geometryModifier.GenerateHullTessellationFactors(SD);
				}
			}
			
			string ds = "";
			foreach(ShaderGeometryModifier geometryModifier in GeometryModifiers){
				if (geometryModifier.HasFlag(ShaderGeometryModifierComponent.Tessellation)){
					ds += geometryModifier.GenerateDomainTransfer(SD);
				}
			}
			string Domain = "tri";
			if (SD.TessDomain==TessDomain.Tri)Domain = "tri";
			if (SD.TessDomain==TessDomain.Quad)Domain = "quad";
			if (SD.TessDomain==TessDomain.Isoline)Domain = "isoline";
			
			string Partition = "fractional_odd";
			if (SD.TessPartition==TessPartition.Integer)Partition = "integer";
			if (SD.TessPartition==TessPartition.FractionalEven)Partition = "fractional_even";
			if (SD.TessPartition==TessPartition.FractionalOdd)Partition = "fractional_odd";
			if (SD.TessPartition==TessPartition.Pow2)Partition = "pow2";
			
			string OutputTopo = "triangle_cw";
			if (SD.TessOutputTopology==TessOutputTopology.Point)OutputTopo = "point";
			if (SD.TessOutputTopology==TessOutputTopology.Line)OutputTopo = "line";
			if (SD.TessOutputTopology==TessOutputTopology.TriangleCW)OutputTopo = "triangle_cw";
			if (SD.TessOutputTopology==TessOutputTopology.TriangleCCW)OutputTopo = "triangle_ccw";
			
			HullControlPointsShader = 
			"[UNITY_domain(\""+Domain+"\")]\n"+
			"[UNITY_partitioning(\""+Partition+"\")]\n"+
			"[UNITY_outputtopology(\""+OutputTopo+"\")]\n"+
			"[UNITY_patchconstantfunc(\"HullTessellationFactors\")]\n"+
			"[UNITY_outputcontrolpoints("+SD.TessControlPoints.ToString()+")]\n";
			
			HullControlPointsShader += "TessellationShaderInput Hull (InputPatch<TessellationShaderInput,"+SD.TessControlPoints.ToString()+"> v, uint id : SV_OutputControlPointID) {\n"+
			"	TessellationShaderInput VT = v[id];\n";
			HullControlPointsShader += hcps;
			HullControlPointsShader +=	"	return VT;\n"+
										"}\n";
										

			
			HullTessellationFactorsShader = "TessellationFactors HullTessellationFactors (InputPatch<TessellationShaderInput,"+SD.TessControlPoints.ToString()+"> v) {\n";
			HullTessellationFactorsShader += "	float4 TF = float4(1,1,1,1);\n";
			HullTessellationFactorsShader += htfs;
			
			//HullTessellationFactorsShader += "	TF = max(TF,1);\n"+"	TessellationFactors o;\n";
			HullTessellationFactorsShader += "	TessellationFactors o;\n";
		
			if (SD.TessDomain == TessDomain.Tri)
			HullTessellationFactorsShader +="	o.edge[0] = TF.x;\n"+
			"	o.edge[1] = TF.y;\n"+
			"	o.edge[2] = TF.z;\n"+
			"	o.inside = TF.w;\n";
			else
			if (SD.TessDomain == TessDomain.Isoline)
			HullTessellationFactorsShader +="	o.edge[0] = TF.x;\n"+
			"	o.edge[1] = TF.y;\n";
			///TessQuad still needed
			
			HullTessellationFactorsShader +="	return o;\n"+
			"}\n";
			
			
			DomainShader = 
			"[UNITY_domain(\""+Domain+"\")]\n";
			
			if(SD.TessDomain==TessDomain.Tri)
			DomainShader += "VertexToPixel Domain (TessellationFactors tessFactors, const OutputPatch<TessellationShaderInput,"+SD.TessControlPoints.ToString()+"> vi, float3 bary : SV_DomainLocation) {\n";
			else
			DomainShader += "VertexToPixel Domain (TessellationFactors tessFactors, const OutputPatch<TessellationShaderInput,"+SD.TessControlPoints.ToString()+"> vi, float2 bary : SV_DomainLocation) {\n";
			
			DomainShader += "	VertexShaderInput v;\n";
			
			
			
			
			
			
			
			
			
			DomainShader += "	VertexToPixel vtp;\n";
			DomainShader += "	VertexData vd;\n";
			DomainShader += "	UNITY_INITIALIZE_OUTPUT(VertexData,vd);\n";
			
			foreach(ShaderVertexInterpolator i in SD.VertexInputs){
				if (SD.TessDomain == TessDomain.Tri)
				DomainShader+="	v."+i.Name+" = vi[0]."+i.Name+"*bary.x + vi[1]."+i.Name+"*bary.y + vi[2]."+i.Name+"*bary.z;\n";
				else if (SD.TessDomain == TessDomain.Isoline)
				DomainShader+="	v."+i.Name+" = vi[0]."+i.Name+"*(1-bary.x) + vi[1]."+i.Name+"*bary.x;\n";
			}

			DomainShader += ds;
			
			foreach(ShaderVertexInterpolator i in SD.Interpolators){
				if (i.InVertex){
					string t = "";
					DomainShader += GenerateInterpolatorPredefines(i,ref t,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds);
					if (i.ShaderVertexInterpolatorType!=ShaderVertexInterpolatorType.Inline){
						if (SD.CoordSpace == CoordSpace.World){
							if (i.Name=="worldPos")
								DomainShader += t+"	vd.worldPos = v.vertex.xyz;\n";
							else if (i.Name=="worldNormal")
								DomainShader += t+"	vd.worldNormal = v.normal;\n";
							else
								DomainShader += t+"	vd." + i.Name + " = " + i.GetCode(SD.CoordSpace) + ";\n";
						}
						else
						if (i.GetCode(SD.CoordSpace)!="")
							DomainShader += t+"	vd." + i.Name + " = " + i.GetCode(SD.CoordSpace) + ";\n";
					}else{
						DomainShader += t+"	"+i.GetCode(SD.CoordSpace)+"\n";
					}
				}
			}
			DomainShader += GenerateInterpolatorPredefines(null,ref SD.worthlessref,ref SD.PredefinesDepth, SD.PredefineStarts,SD.PredefineEnds);
			if (SD.CoordSpace == CoordSpace.World){
					DomainShader += "	v.vertex.xyz = mul(_World2Object,v.vertex.xyz);\n";
					DomainShader += "	v.normal = UnityWorldToObjectDir(v.normal);\n";
			}

			//foreach(ShaderLayerList SLL in SD.UsedMasks){///TODO: CheckFragmentOrVertexEtc
				//if (SLL.Used){
		if (SD.UsesUnityGIVertex){
			StringBuilder ImLazy = new StringBuilder(2000);
			GenerateGI(SD,ImLazy);
			DomainShader += ImLazy.ToString();
			//AttenIncrement++;
		}
			foreach(ShaderLayerList SLL in SD.Base.ShaderLayersMasks){
				if (SD.UsedMasksVertex.Contains(SLL)){
					//string surfaceInternals = SLL.GCVariable()+SLL.GenerateCode(SD,false)+"return "+SLL.CodeName+";\n";
					string FunctionHeader = SLL.GenerateHeaderCached(SD,true);///TODO: Used to not use cached, double check if theres a reason why
					DomainShader+=("vd." + SLL.CodeName + " = " + FunctionHeader + ";\n");
				}
			}
//			Debug.Log(SD.GeometryModifierIndex);
			for (;SD.GeometryModifierIndex<GeometryModifiers.Count;SD.GeometryModifierIndex++){
				ShaderGeometryModifier geometryModifier = GeometryModifiers[SD.GeometryModifierIndex] as ShaderGeometryModifier;
				if (geometryModifier!=null){
					//if (geometryModifier.HasFlag(ShaderGeometryModifierComponent.Tessellation)||geometryModifier.ShaderGeometryModifierComponent .HasFlag(ShaderGeometryModifierComponent.Geometry))
					//	continue;
					//if (geometryModifier.ShaderGeometryModifierComponent == ShaderGeometryModifierComponent.Transparency)
					//	continue;
					if (!geometryModifier.HasFlag(ShaderGeometryModifierComponent.Vertex))
						continue;
					string surfaceInternals = geometryModifier.GenerateEffectsCode(SD)+geometryModifier.GenerateVertex(SD);
					if (surfaceInternals.Length==0)
						continue;
					string func = "	"+geometryModifier.GenerateHeader(SD,surfaceInternals,true)+";\n";//GenerateGeometryMoodifier(SD,geometryModifier);
					//string realfunc = geometryModifier.GenerateVertex(SD);
					DomainShader += func;
					bool UpdatedWorldPos = surfaceInternals.IndexOf("vd.worldPos")>-1;
					bool UpdatedWorldNormal = surfaceInternals.IndexOf("vd.worldNormal")>-1;
					bool UpdatedLocalPos = surfaceInternals.IndexOf("v.vertex")>-1;
					bool UpdateLocalNormal = surfaceInternals.IndexOf("v.normal")>-1;
					UpdatedWorldPos = !UpdatedLocalPos&&UpdatedWorldPos;
					UpdatedWorldNormal = !UpdateLocalNormal&&UpdatedWorldNormal;
					if (UpdatedWorldPos)
						DomainShader += "	v.vertex.xyz = mul(_World2Object,float4(vd.worldPos,1)).xyz;\n";
					if (UpdatedWorldNormal)
						DomainShader += "	v.normal.xyz = UnityWorldToObjectDir(vd.worldNormal);\n";
					foreach(ShaderVertexInterpolator i in SD.Interpolators){
						if (i.InVertex&&!((UpdatedWorldPos&&i.Name=="worldPos")||(UpdatedWorldNormal&&i.Name=="worldNormal"))){
							string t = "";
							DomainShader += GenerateInterpolatorPredefines(i,ref t,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds);
							if (i.GetCode(SD.CoordSpace)!=""){
								if (i.ShaderVertexInterpolatorType==ShaderVertexInterpolatorType.Inline)
									DomainShader += t+i.GetCode(SD.CoordSpace) + "\n";
								else
									DomainShader += t+"	vd." + i.Name + " = " + i.GetCode(SD.CoordSpace) + ";\n";
							}
						}
					}
					DomainShader += GenerateInterpolatorPredefines(null,ref SD.worthlessref,ref SD.PredefinesDepth, SD.PredefineStarts,SD.PredefineEnds);
				}
			}
			
			foreach(ShaderVertexInterpolator i in SD.Interpolators){
				if (i.Transfer){
					string t = "";
					DomainShader += GenerateInterpolatorPredefines(i,ref t,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds);
					if (i.ShaderVertexInterpolatorType!=ShaderVertexInterpolatorType.Inline)
						DomainShader += t+"	vtp." + i.Name + " = vd." + i.Name + ";\n";
					else
						DomainShader += t+"	" + i.GetCode(SD.CoordSpace) + "\n";
				}
			}
			DomainShader += GenerateInterpolatorPredefines(null,ref SD.worthlessref,ref SD.PredefinesDepth, SD.PredefineStarts,SD.PredefineEnds);
			
			
			DomainShader +=	"	return vtp;\n"+
										"}\n";
			
			if (SD.TessDomain==TessDomain.Tri)
			TessellationFactors = 	"struct TessellationFactors {\n"+
									"	float edge[3] : SV_TessFactor;\n"+
									"	float inside : SV_InsideTessFactor;\n"+
									"};\n";
			else
			if (SD.TessDomain==TessDomain.Isoline)
			TessellationFactors = 	"struct TessellationFactors {\n"+
									"	float edge[2] : SV_TessFactor;\n"+
									"};\n";
			else//Quads
			TessellationFactors = "struct TessellationFactors {\n"+
									"	float edge[4] : SV_TessFactor;\n"+
									"	float inside[2] : SV_InsideTessFactor;\n"+
									"};\n";
		}
	}
	public string GenerateVertexToPixel(ShaderData SD){
		string VertexToPixel = "struct VertexToPixel{\n";
			//if (OneToOne){
				int TexCoord = 0;
				foreach(ShaderVertexInterpolator i in SD.Interpolators){
					if (i.Transfer){
						string t = "";
						
						VertexToPixel += GenerateInterpolatorPredefines(i,ref t,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds);
						t += i.ImLazy();
						t += i.Name;
						if (i.ShaderVertexInterpolatorType==ShaderVertexInterpolatorType.Position)
							t += " : POSITION;\n";
						else if (i.ShaderVertexInterpolatorType==ShaderVertexInterpolatorType.Color)
							t += " : COLOR;\n";
						else if (i.ShaderVertexInterpolatorType==ShaderVertexInterpolatorType.Inline){
							t += "("+TexCoord.ToString()+")\n";
							TexCoord++;
						}
						else{
							t += " : TEXCOORD"+TexCoord.ToString()+";\n";
							TexCoord++;
						}
						VertexToPixel += t;
					}
					
				}
			//END BRACKET
			VertexToPixel += GenerateInterpolatorPredefines(null,ref SD.worthlessref,ref SD.PredefinesDepth, SD.PredefineStarts,SD.PredefineEnds);
			VertexToPixel += "};\n\n";
			
			if (SD.ShaderPassIsDeferred)
				VertexToPixel+="			struct DeferredOutput{\n"+
					"				half3 Emission;\n"+
					"				half4 SpecSmoothness;\n"+
					"				half3 Albedo;\n"+
					"				half Alpha;\n"+
					"				half  Occlusion;\n"+
					"				half3 Normal;\n"+
					"				half3 AmbientDiffuse;\n"+
					"			};\n"+
					"			\n";
					
			return VertexToPixel;
	}
	public string GenerateVertexData(ShaderData SD){
		string VertexData = "struct VertexData{\n";
		foreach(ShaderVertexInterpolator i in SD.Interpolators){
			//if (i.InFragment&&!(i.ShaderVertexInterpolatorType==ShaderVertexInterpolatorType.Position)&&!(i.ShaderVertexInterpolatorType==ShaderVertexInterpolatorType.Inline)){
			if ((i.InFragment||i.InVertex)&&!(i.ShaderVertexInterpolatorType==ShaderVertexInterpolatorType.Inline)){
				string t = "";
				VertexData += GenerateInterpolatorPredefines(i,ref t,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds);
				t += i.ImLazy();
				t += i.Name+";\n";
				VertexData += t;
			}
			
		}
		VertexData += GenerateInterpolatorPredefines(null,ref SD.worthlessref,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds);
		//foreach(ShaderLayerList SLL in SD.Base.ShaderLayersMasks){
		foreach(ShaderLayerList SLL in SD.Base.ShaderLayersMasks){
			if (SD.UsedMasksVertex.Contains(SLL)||SD.UsedMasksFragment.Contains(SLL)){
				VertexData += "	"+SLL.GCVariableBlahBlah();
			}
		}
		VertexData += "	float Atten;\n";
		VertexData += "};\n";
		return VertexData;
	}
	/*public string GenerateVertexDataVertexShader(ShaderData SD){
		string VertexDataVertexShader = "struct VertexDataVertexShader{\n";
		foreach(ShaderVertexInterpolator i in SD.Interpolators){
			if (i.InVertex){
				if (i.ShaderVertexInterpolatorType!=ShaderVertexInterpolatorType.Inline){
					string t = "";
					VertexDataVertexShader += GenerateInterpolatorPredefines(i,ref t,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds);
					t += i.ImLazy();
					t += i.Name+";\n";
					VertexDataVertexShader += t;
				}
			}
			
		}
		VertexDataVertexShader += GenerateInterpolatorPredefines(null,ref SD.worthlessref,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds);
		VertexDataVertexShader += "float Atten;\n";
		VertexDataVertexShader += "};\n";
		return VertexDataVertexShader;
	}*/
	public string GenerateForwardBasePass(ShaderData SD,ShaderIngredient[] ingredients){
		SD.NewPass();
		SD.ShaderPass = ShaderPasses.ForwardBase;
		return GeneratePass(SD,ingredients);
	}
	public string GenerateForwardAddPass(ShaderData SD,ShaderIngredient[] ingredients){
		SD.NewPass();
		SD.ShaderPass = ShaderPasses.ForwardAdd;
		return GeneratePass(SD,ingredients);
	}
	public string GenerateDeferredPass(ShaderData SD,ShaderIngredient[] ingredients){
		SD.NewPass();
		SD.ShaderPass = ShaderPasses.Deferred;
		return GeneratePass(SD,ingredients);
	}
	public string GenerateShadowCasterPass(ShaderData SD,ShaderIngredient[] ingredients){
		SD.NewPass();
		SD.ShaderPass = ShaderPasses.ShadowCaster;
		return GeneratePass(SD,ingredients);
	}
	public string GenerateMaskPass(ShaderData SD,ShaderIngredient[] ingredients){
		SD.NewPass();
		SD.ShaderPass = ShaderPasses.Mask;
		return GeneratePass(SD,ingredients);
	}

	public string GeneratePass(ShaderData SD,ShaderIngredient[] ingredients){
		string ShaderCode = "";
		string HullControlPointsShader = "";
		string HullTessellationFactorsShader = "";
		string DomainShader = "";
		string TessellationShaderInput = "";
		string VertexToTessellation = "";
		string TessellationFactors = "";
		string IngredientGlobalFunctions = "";
		string SurfaceFunctions = "";
		string FragmentShader = "";
		string VertexShader = "";
		
			DecidePremultiplication(SD);				
			
			IngredientGlobalFunctions = GenerateIngredientGlobalFunctions(SD,ingredients);
			
			//int asdasdas = SD.GeometryModifierIndex;
			
		CreateInterpolators(SD, ref SurfaceFunctions, ref FragmentShader, ref VertexShader, ref IngredientGlobalFunctions, ingredients);
			
			string VertexShaderInput = GenerateVertexShaderInput(SD);	
			
		GenerateTessellation(SD, ref TessellationShaderInput, ref HullControlPointsShader, ref HullTessellationFactorsShader, ref DomainShader, ref TessellationFactors);
		
			string VertexToPixel = GenerateVertexToPixel(SD);
			string VertexData = GenerateVertexData(SD);
		
		string TheCoolPartsOfTheShader = VertexShaderInput+TessellationShaderInput+TessellationFactors+VertexToTessellation+VertexToPixel+VertexData+IngredientGlobalFunctions+SurfaceFunctions+VertexShader+HullTessellationFactorsShader+HullControlPointsShader+DomainShader+FragmentShader+GeneratePassEnd(null);
		if (SD.ShaderPass == ShaderPasses.ForwardBase)
			ShaderCode += GeneratePassStart2(SD,"FORWARD","ForwardBase","multi_compile_fwdbase"+(SD.ForwardShadows?"":" noshadow")+(SD.Lightmaps?"":" nolightmap nodynlightmap")+(SD.ForwardVertexLights?"":" novertexlight"),"UNITY_PASS_FORWARDBASE","")+GenerateUniforms(SD,TheCoolPartsOfTheShader)+GenerateFunctions(SD,TheCoolPartsOfTheShader)+TheCoolPartsOfTheShader;
	
		if (SD.ShaderPass == ShaderPasses.ForwardAdd)
			ShaderCode += GeneratePassStart2(SD,"FORWARD","ForwardAdd","multi_compile_fwdadd_fullshadows"+((SD.ForwardFullShadows)?"":" noshadow")+(SD.Lightmaps?"":" nolightmap nodynlightmap")+(SD.ForwardVertexLights?"":" novertexlight"),"UNITY_PASS_FORWARDADD","")+GenerateUniforms(SD,TheCoolPartsOfTheShader)+GenerateFunctions(SD,TheCoolPartsOfTheShader)+TheCoolPartsOfTheShader;
		
		if (SD.ShaderPass == ShaderPasses.ShadowCaster)
			ShaderCode += GeneratePassStart2(SD,"ShadowCaster","ShadowCaster","multi_compile_shadowcaster","SHADERSANDWICH_SHADOWCASTER","")+GenerateUniforms(SD,TheCoolPartsOfTheShader)+GenerateFunctions(SD,TheCoolPartsOfTheShader)+TheCoolPartsOfTheShader;		
		
		if (SD.ShaderPass == ShaderPasses.Mask)
			ShaderCode += GeneratePassStart2(SD,"ZWritePrePass","","","SHADERSANDWICH_ZWRITEPREPASS","")+GenerateUniforms(SD,TheCoolPartsOfTheShader)+GenerateFunctions(SD,TheCoolPartsOfTheShader)+TheCoolPartsOfTheShader;
		
		if (SD.ShaderPass == ShaderPasses.Deferred)
			//ShaderCode += GeneratePassStart2(SD,"DEFERRED","Deferred","multi_compile_prepassfinal\n				#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2","UNITY_PASS_DEFERRED"," nomrt")+GenerateUniforms(SD,TheCoolPartsOfTheShader)+GenerateFunctions(SD,TheCoolPartsOfTheShader)+TheCoolPartsOfTheShader;
			ShaderCode += GeneratePassStart2(SD,"DEFERRED","Deferred","multi_compile_prepassfinal\n				#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2"+(SD.Lightmaps?"":" nolightmap nodynlightmap")+(SD.ForwardVertexLights?"":" novertexlight"),"UNITY_PASS_DEFERRED"," nomrt")+GenerateUniforms(SD,TheCoolPartsOfTheShader)+GenerateFunctions(SD,TheCoolPartsOfTheShader)+TheCoolPartsOfTheShader;
		//if (SD.ShaderModel == ShaderModel.SM2||SD.ShaderModel==ShaderModel.SM2_5||SD.ShaderModel==ShaderModel.SM3)
		//	ShaderCode = ShaderCode.Replace("vtp.position","floor(vd.screenPos*float4(_ScreenParams.xy,1,1))");
			return ShaderCode;
	}
	public string GenerateInterpolatorPredefines(ShaderVertexInterpolator i, ref string t,ref int PredefinesDepth, string[] PredefineStarts,string[] PredefineEnds){
		string VertexToPixel = "";
		if (i!=null&&i.PredefineStarts!=null){
			int ii = 0;
			foreach(string pred in i.PredefineStarts){
				if (pred!=PredefineStarts[ii]){
					for(;PredefinesDepth>=ii;PredefinesDepth--){
						VertexToPixel += ShaderUtil.Tabs[ii]+PredefineEnds[PredefinesDepth]+"\n";
						PredefineEnds[PredefinesDepth] = "";
						PredefineStarts[PredefinesDepth] = "";
					}
					
					PredefineStarts[ii] = pred;
					PredefineEnds[ii] = i.PredefineEnds[ii];
					VertexToPixel += ShaderUtil.Tabs[ii+1]+PredefineStarts[ii]+"\n";
				}
				ii++;
			}
			PredefinesDepth = i.PredefineStarts.Length-1;
		}else{
			for(;PredefinesDepth>=0;PredefinesDepth--){
				VertexToPixel += ShaderUtil.Tabs[PredefinesDepth+1]+PredefineEnds[PredefinesDepth]+"\n";
				PredefineEnds[PredefinesDepth] = "";
				PredefineStarts[PredefinesDepth] = "";
			}
		}
		if (PredefinesDepth>-1)
			t = ShaderUtil.Tabs[PredefinesDepth+1];
		return VertexToPixel;
	}
	/*public string GenerateGeometryModifier(ShaderData SD, ShaderGeometryModifier geometryModifier){
		string SurfaceMix = "";		

			//string surfaceInternals = geometryModifier.GenerateEffectsCode(SD)+geometryModifier.GenerateCode(SD);
			//Debug.Log(surfaceInternals);
			/*bool UsesVertexData = false;
			bool UsesVertexToPixel = false;
			bool UsesVertex = false;
			bool UsesMasks = false;
			if (surfaceInternals.IndexOf("vd.")>=0)
				UsesVertexData = true;
			if (surfaceInternals.IndexOf("vtp.")>=0)
				UsesVertexToPixel = true;
			if (surfaceInternals.IndexOf("v.")>=0)
				UsesVertex = true;
			if (surfaceInternals.IndexOf("mask.")>=0)
				UsesMasks = true;
			
			string passin = "";
			bool asd = false;
			if (UsesVertexData){passin += (asd?", ":" ") + "vd";asd = true;}
			if (UsesVertexToPixel){passin += (asd?", ":" ") + "vtp";asd = true;}
			if (UsesVertex){passin += (asd?", ":" ") + "v";asd = true;}
			if (UsesMasks){passin += (asd?", ":" ")+"mask";asd = true;}
			
			//if (surface.ShaderGeometryModifierComponent==ShaderGeometryModifierComponent.Normal)
			//SurfaceMix += "				"+geometryModifier.CodeName+"("+passin+");\n";
			SurfaceMix += "	"+geometryModifier.GenerateHeader(SD,geometryModifier.GenerateEffectsCode(SD)+geometryModifier.GenerateCode(SD),true)+";\n";
		
		return SurfaceMix;
	}*/
	public string GenerateSurfaceFunctions(ShaderData SD){
		StringBuilder SurfaceFunctions = new StringBuilder();
		bool UsesPremul = SD.UsesPremultipliedBlending;
		SD.PipelineStage = PipelineStage.Fragment;
		foreach(ShaderIngredient ingredient in Surfaces){
			if (!ingredient.IsUsed())continue;
			string surfaceInternals = ingredient.GenerateEffectsCode(SD)+ingredient.GenerateCodeAndCache(SD);
			if (surfaceInternals.Length==0)
				continue;
			string FunctionHeader = ingredient.GenerateHeader(SD,surfaceInternals,false);
			if (ingredient as ShaderSurface!=null){
				SurfaceFunctions.Append("//OutputPremultiplied: "+(ingredient as ShaderSurface).OutputPremultiplied.ToString()+"\n");
				SurfaceFunctions.Append("//UseAlphaGenerate: "+(ingredient as ShaderSurface).UseAlphaGenerate.ToString()+"\n");
			}
			SurfaceFunctions.Append(FunctionHeader);
			//SurfaceFunctions.Append("{\n	");
			SurfaceFunctions.Append(surfaceInternals.Replace("\n","\n	"));
			SurfaceFunctions.Append("\n}\n");
			foreach(ShaderVertexInterpolator i in SD.Interpolators){
				if (surfaceInternals.IndexOf(i.Name)>=0){
					i.InFragment = true;
					if ((i.FragmentCode.Contains("vtp."+i.Name)||i.FragmentCode.Length==0)){
						i.Transfer = true;
						i.InVertex = true;
					}
					foreach(ShaderVertexInterpolator ii in i.ReliesOn){
						ii.InVertex = true;
					}
				}
			}
		}
		foreach(ShaderIngredient ingredient in GeometryModifiers){
			if (!ingredient.IsUsed())continue;
			ShaderGeometryModifier geometryModifier = ingredient as ShaderGeometryModifier;
			if (geometryModifier!=null){
				string V_ = "";
				string T_ = "";
				if (geometryModifier.HasFlag(ShaderGeometryModifierComponent.Vertex)&&geometryModifier.HasFlag(ShaderGeometryModifierComponent.Transparency)){
					V_ = "V_";
					T_ = "T_";
				}
				
				SD.PipelineStage = PipelineStage.Vertex;
				string surfaceInternals = ingredient.GenerateEffectsCode(SD)+geometryModifier.GenerateVertex(SD);
				if (surfaceInternals.Length>0){
					string FunctionHeader = ingredient.GenerateHeader(SD,surfaceInternals,false,V_);
					surfaceInternals = ProtectSamplings(surfaceInternals);
					SurfaceFunctions.Append(FunctionHeader);
					//SurfaceFunctions.Append("{\n	");
					SurfaceFunctions.Append(surfaceInternals.Replace("\n","\n	"));
					SurfaceFunctions.Append("\n}\n");
					
					foreach(ShaderVertexInterpolator i in SD.Interpolators){
						if (surfaceInternals.IndexOf(i.Name)>=0){
							i.InVertex = true;
							foreach(ShaderVertexInterpolator ii in i.ReliesOn){
								ii.InVertex = true;
							}
						}
					}
				}
				SD.PipelineStage = PipelineStage.Fragment;
				surfaceInternals = ingredient.GenerateEffectsCode(SD)+geometryModifier.GenerateTransparency(SD);
				if (surfaceInternals.Length>0){
					string FunctionHeader = ingredient.GenerateHeader(SD,surfaceInternals,false,T_);
					SurfaceFunctions.Append(FunctionHeader);
					//SurfaceFunctions.Append("{\n	");
					SurfaceFunctions.Append(surfaceInternals.Replace("\n","\n	"));
					SurfaceFunctions.Append("\n}\n");
					foreach(ShaderVertexInterpolator i in SD.Interpolators){
						if (surfaceInternals.IndexOf(i.Name)>=0){
							i.InFragment = true;
							if ((i.FragmentCode.Contains("vtp."+i.Name)||i.FragmentCode.Length==0)){
								i.Transfer = true;
								i.InVertex = true;
							}
							foreach(ShaderVertexInterpolator ii in i.ReliesOn){
								ii.InVertex = true;
							}
						}
					}
				}
			}else{
				//Debug.Log("Yeah I execute...oh");
				SD.PipelineStage = PipelineStage.Fragment;
				string surfaceInternals = ingredient.GenerateEffectsCode(SD)+ingredient.GenerateCodeAndCache(SD);
				if (surfaceInternals.Length==0)
					continue;
				string FunctionHeader = ingredient.GenerateHeader(SD,surfaceInternals,false);
				SurfaceFunctions.Append(FunctionHeader);
				//SurfaceFunctions.Append("{\n	");
				SurfaceFunctions.Append(surfaceInternals.Replace("\n","\n	"));
				SurfaceFunctions.Append("\n}\n");
				foreach(ShaderVertexInterpolator i in SD.Interpolators){
					if (surfaceInternals.IndexOf(i.Name)>=0){
						i.InFragment = true;
						if ((i.FragmentCode.Contains("vtp."+i.Name)||i.FragmentCode.Length==0)){
							i.Transfer = true;
							i.InVertex = true;
						}
						foreach(ShaderVertexInterpolator ii in i.ReliesOn){
							ii.InVertex = true;
						}
					}
				}
			}
		}
		SD.UsesPremultipliedBlending = UsesPremul;
		//foreach(ShaderLayerList SLL in SD.Base.ShaderLayersMasks){
		//foreach(ShaderLayerList SLL in SD.UsedMasks){
		HashSet<ShaderLayerList> Ignore = new HashSet<ShaderLayerList>();
		foreach(ShaderLayerList SLL in SD.UsedMasksVertex){
			Ignore.Add(SLL);
		}
		foreach(ShaderLayerList SLL in SD.Base.ShaderLayersMasks){
			if (SD.UsedMasksFragment.Contains(SLL)){
				SD.PipelineStage = PipelineStage.Fragment;
			//if (SLL.Used){///TOoDO: UsedVertex, UsedFragment etc, for now assume fragment
				string surfaceInternals = SLL.GCVariable()+SLL.GenerateCode(SD,false)+"return "+SLL.CodeName+";\n";
				if (surfaceInternals.IndexOf("tex2D")<0&&surfaceInternals.IndexOf("texCUBE")<0){
					SD.UsedMasksVertex.Remove(SLL);
					SLL.GenerateHeader(SD,surfaceInternals,true);
					SLL.GenerateHeaderCache2 = SLL.GenerateHeaderCache;
				}else{
					SLL.GenerateHeader(SD,surfaceInternals,true);
				}
				string FunctionHeader = SLL.GenerateHeader(SD,surfaceInternals,false);
				SurfaceFunctions.Append(FunctionHeader);
				SurfaceFunctions.Append("{\n	");
				SurfaceFunctions.Append(surfaceInternals.Replace("\n","\n	"));
				SurfaceFunctions.Append("\n}\n");
					foreach(ShaderVertexInterpolator i in SD.Interpolators){
						if (surfaceInternals.IndexOf(i.Name)>=0){
							i.InFragment = true;
							if ((i.FragmentCode.Contains("vtp."+i.Name)||i.FragmentCode.Length==0)){
								i.Transfer = true;
								i.InVertex = true;
							}
							foreach(ShaderVertexInterpolator ii in i.ReliesOn){
								ii.InVertex = true;
							}
						}
					}
			}
			if (SD.UsedMasksVertex.Contains(SLL)){
				SD.PipelineStage = PipelineStage.Vertex;
			//if (SLL.Used){///TOoDO: UsedVertex, UsedFragment etc, for now assume fragment
				string surfaceInternals = SLL.GCVariable()+SLL.GenerateCode(SD,false)+"return "+SLL.CodeName+";\n";
				string FunctionHeader = SLL.GenerateHeader(SD,surfaceInternals,false);
				SLL.GenerateHeader(SD,surfaceInternals,true);
				SurfaceFunctions.Append(FunctionHeader);
				SurfaceFunctions.Append("{\n	");
				SurfaceFunctions.Append(ProtectSamplings(surfaceInternals.Replace("\n","\n	")));
				SurfaceFunctions.Append("\n}\n");
					foreach(ShaderVertexInterpolator i in SD.Interpolators){
						if (surfaceInternals.IndexOf(i.Name)>=0){
							i.InFragment = true;
							if ((i.FragmentCode.Contains("vtp."+i.Name)||i.FragmentCode.Length==0)){
								i.Transfer = true;
								i.InVertex = true;
							}
							foreach(ShaderVertexInterpolator ii in i.ReliesOn){
								ii.InVertex = true;
							}
						}
					}
			}
		}
		SD.UsedMasksVertex = Ignore;
		return SurfaceFunctions.ToString();
	}
	public static string ProtectSamplings(string surfaceInternals){
		if (surfaceInternals.Contains("tex2D"))
			surfaceInternals = "#define tex2D(tex,uv) tex2Dlod(tex,float4(uv,0,0))\n"+surfaceInternals+"#undef tex2D\n";
		if (surfaceInternals.Contains("texCUBE"))
			surfaceInternals = "#define texCUBE(tex,uv) texCUBElod(tex,float4(uv,0))\n"+surfaceInternals+"#undef texCUBE\n";
		return surfaceInternals;
	}
	public string GenerateVertex(ShaderData SD){
		SD.PipelineStage = PipelineStage.Vertex;
		SD.CoordSpace = CoordSpace.Object;
		//var watch = System.Diagnostics.Stopwatch.StartNew();	
		
		StringBuilder VertexShader = new StringBuilder(3000);
		if (!SD.UsesTessellation)
			VertexShader.Append(
			"VertexToPixel Vertex (VertexShaderInput v){\n"+
			"	VertexToPixel vtp;\n"+
			"	UNITY_INITIALIZE_OUTPUT(VertexToPixel,vtp);\n");
		else
			VertexShader.Append(
			"TessellationShaderInput Vertex (VertexShaderInput v){\n"+
			"	TessellationShaderInput vtp;\n"+
			"	UNITY_INITIALIZE_OUTPUT(TessellationShaderInput,vtp);\n");
			
		VertexShader.Append(
			"	VertexData vd;\n"+
			"	UNITY_INITIALIZE_OUTPUT(VertexData,vd);\n");
			//bool DidDisplace = false;
			for (;SD.GeometryModifierIndex<GeometryModifiers.Count;SD.GeometryModifierIndex++){
				//Debug.Log("asdas");
				ShaderGeometryModifier geometryModifier = GeometryModifiers[SD.GeometryModifierIndex] as ShaderGeometryModifier;
				if (geometryModifier!=null){
					//Debug.Log("asdas2222");
					if (geometryModifier.HasFlag(ShaderGeometryModifierComponent.Tessellation)||geometryModifier.HasFlag(ShaderGeometryModifierComponent.Geometry))
						break;
					if (!geometryModifier.HasFlag(ShaderGeometryModifierComponent.Vertex))
						continue;
					string surfaceInternals = geometryModifier.GenerateEffectsCode(SD)+geometryModifier.GenerateVertex(SD);
					//Debug.Log(surfaceInternals);
					if (surfaceInternals.Length==0)
						continue;
					//Debug.Log("vd?");
					string func = "	"+geometryModifier.GenerateHeader(SD,surfaceInternals,true)+";\n";//GenerateGeometryMoodifier(SD,geometryModifier);
					//Debug.Log(func);
					if (func.IndexOf("vd")>=0)
						break;
					//Debug.Log("vd nop");
					//Debug.Log(func);
					VertexShader.Append(func);
					//DidDisplace = true;
				}
			}
			SD.PredefineStarts = new string[]{"","","","","","",""};
			SD.PredefineEnds = new string[]{"","","","","","",""};
			SD.worthlessref = "";
			SD.PredefinesDepth = -1;
			foreach(ShaderVertexInterpolator i in SD.Interpolators){
				if (i.InVertex){
					if (i.GetCode(SD.CoordSpace)!=""){
						if (i.ShaderVertexInterpolatorType!=ShaderVertexInterpolatorType.Inline){
							VertexShader.Append(GenerateInterpolatorPredefines(i,ref SD.worthlessref,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds));
							VertexShader.Append("	vd." + i.Name + " = " + i.GetCode(SD.CoordSpace) + ";\n");
						}
					}
				}
			}
			VertexShader.Append(GenerateInterpolatorPredefines(null,ref SD.worthlessref,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds));
			VertexShader.Append("	\n	\n");
		//watch.Stop();Debugassssss+="\nFirstBit"+watch.Elapsed.ToString();	
		//watch = System.Diagnostics.Stopwatch.StartNew();
		//foreach(ShaderLayerList SLL in SD.UsedMasks){///TODO: CheckFragmentOrVertexEtc
		if (SD.UsesUnityGIVertex){
			GenerateGI(SD,VertexShader);
			//AttenIncrement++;
		}
		foreach(ShaderLayerList SLL in SD.Base.ShaderLayersMasks){
			if (SD.UsedMasksVertex.Contains(SLL)){
			//if (SLL.Used){
				//string surfaceInternals = SLL.GCVariable()+SLL.GenerateCode(SD,false)+"return "+SLL.CodeName+";\n";
				string FunctionHeader = SLL.GenerateHeaderCached(SD,true);
				VertexShader.Append(("vd." + SLL.CodeName + " = " + FunctionHeader + ";\n"));
			}
		}
		//watch.Stop();Debugassssss+="\nMasks"+watch.Elapsed.ToString();	
		//watch = System.Diagnostics.Stopwatch.StartNew();	
			//DidDisplace = false;
			for (;SD.GeometryModifierIndex<GeometryModifiers.Count;SD.GeometryModifierIndex++){
				ShaderGeometryModifier geometryModifier = GeometryModifiers[SD.GeometryModifierIndex] as ShaderGeometryModifier;
				if (geometryModifier!=null){
					if (geometryModifier.HasFlag(ShaderGeometryModifierComponent.Tessellation)||geometryModifier.HasFlag(ShaderGeometryModifierComponent.Geometry))
						break;
					
					if (!geometryModifier.HasFlag(ShaderGeometryModifierComponent.Vertex))
						continue;
					string realfunc = geometryModifier.GenerateVertex(SD);
					string surfaceInternals = geometryModifier.GenerateEffectsCode(SD)+realfunc;//geometryModifier.GenerateVertex(SD);
					if (surfaceInternals.Length==0)
						continue;
					string func = "	"+geometryModifier.GenerateHeader(SD,surfaceInternals,true)+";\n";//GenerateGeometryMoodifier(SD,geometryModifier);
					
					VertexShader.Append(func);
					//DidDisplace = true;
					bool UpdatedWorldPos = realfunc.IndexOf("vd.worldPos")>-1;
					bool UpdatedWorldNormal = realfunc.IndexOf("vd.worldNormal")>-1;
					bool UpdatedLocalPos = realfunc.IndexOf("v.vertex")>-1;
					bool UpdateLocalNormal = realfunc.IndexOf("v.normal")>-1;
					UpdatedWorldPos = !UpdatedLocalPos&&UpdatedWorldPos;
					UpdatedWorldNormal = !UpdateLocalNormal&&UpdatedWorldNormal;
					if (UpdatedWorldPos)
						VertexShader.Append("	v.vertex.xyz = mul(_World2Object,float4(vd.worldPos,1));\n");
					if (UpdatedWorldNormal)
						VertexShader.Append("	v.normal.xyz = UnityWorldToObjectDir(vd.worldNormal);\n");
					foreach(ShaderVertexInterpolator i in SD.Interpolators){
						if (i.InVertex&&!((UpdatedWorldPos&&i.Name=="worldPos")||(UpdatedWorldNormal&&i.Name=="worldNormal"))){
							if (i.GetCode(SD.CoordSpace)!=""){
								if (i.ShaderVertexInterpolatorType!=ShaderVertexInterpolatorType.Inline){
									VertexShader.Append(GenerateInterpolatorPredefines(i,ref SD.worthlessref,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds));
									VertexShader.Append("	vd." + i.Name + " = " + i.GetCode(SD.CoordSpace) + ";\n");
								}
							}
						}
					}
					VertexShader.Append(GenerateInterpolatorPredefines(null,ref SD.worthlessref,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds));
				}
			}
			//watch.Stop();Debugassssss+="\nSecondBit"+watch.Elapsed.ToString();	
			//watch.Stop();Debugassssss+="\nSecondBit"+watch.Elapsed.Milliseconds.ToString();	
			//watch.Stop();Debugassssss+="\nSecondBit"+watch.Elapsed.TotalMilliseconds.ToString();	
			//Debugassssss+="\nSecondBit"+watch.ElapsedMilliseconds.ToString();	
		//watch = System.Diagnostics.Stopwatch.StartNew();
			VertexShader.Append("	\n	\n");
			if (!SD.UsesTessellation){
				foreach(ShaderVertexInterpolator i in SD.Interpolators){
					if (i.Transfer){
						//if(SD.UsesVertexDataVertexShader)
							
							VertexShader.Append(GenerateInterpolatorPredefines(i,ref SD.worthlessref,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds));
							if (i.ShaderVertexInterpolatorType!=ShaderVertexInterpolatorType.Inline)
								VertexShader.Append("	vtp." + i.Name + " = vd." + i.Name + ";\n");
							else
								VertexShader.Append("	" + i.GetCode(SD.CoordSpace) + "\n");
						//else
						//	VertexShader += "	vtp." + i.Name + " = " + i.Name + ";\n";
					}
				}
				VertexShader.Append(GenerateInterpolatorPredefines(null,ref SD.worthlessref,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds));
			}else{
				foreach(ShaderVertexInterpolator i in SD.VertexInputs){
					if (SD.CoordSpace==CoordSpace.World&&i.vertexData.Length>=2){
						VertexShader.Append("	vtp." + i.Name + " = " + i.vertexData[1] + ";\n");
					}else{
						VertexShader.Append("	vtp." + i.Name + " = v." + i.Name + ";\n");//i.ImLazy()+i.Name+i.vertexData[0]+";\n";
					}
				}
			}
			//watch.Stop();Debugassssss+="\nMoreBit"+watch.Elapsed.ToString();	
		//watch = System.Diagnostics.Stopwatch.StartNew();
			
			VertexShader.Append( 	"	return vtp;\n"+
								"}\n");
			//watch.Stop();Debugassssss+="\nFinishing touches"+watch.Elapsed.ToString();	
		return VertexShader.ToString();
	}
	public string GenerateFragment(ShaderData SD){
		SD.PipelineStage = PipelineStage.Fragment;
		StringBuilder ShaderCode;
		string AdditionalOutputs = "";
		if (SD.UsesDepthCorrection)
			AdditionalOutputs = ", out float outputDepth : SV_Depth";
		if (!SD.ShaderPassIsDeferred)
			ShaderCode = new StringBuilder(
				//"			void Pixel (VertexToPixel vtp,out fixed4 outputColor : SV_Target) {\n"+
				"			half4 Pixel (VertexToPixel vtp"+AdditionalOutputs+") : SV_Target {\n"+
				"				half4 outputColor = half4(0,0,0,0);\n"+
				"				half3 outputNormal = half3(0,0,1);\n"+
				"				half3 depth = half3(0,0,0);//Tangent Corrected depth, World Space depth, Normalized depth\n"+
				"				half3 tempDepth = half3(0,0,0);\n"+
				"				VertexData vd;\n"+
				"				UNITY_INITIALIZE_OUTPUT(VertexData,vd);\n");
		else
			ShaderCode = new StringBuilder(
				"			void Pixel (VertexToPixel vtp"+AdditionalOutputs+",\n"+
				"				out half4 outDiffuse : SV_Target0,\n"+
				"				out half4 outSpecSmoothness : SV_Target1,\n"+
				"				out half4 outNormal : SV_Target2,\n"+
				"				out half4 outEmission : SV_Target3) {\n"+
				"					outDiffuse = 0;\n"+
				"					outSpecSmoothness = 0;\n"+
				"					outNormal = 0;\n"+
				"					outEmission = 0;\n"+
				"					half3 depth = half3(0,0,0);//Tangent Corrected depth, World Space depth, Normalized depth\n"+
				"					half3 tempDepth = half3(0,0,0);\n"+
				"					VertexData vd;\n"+
				"						UNITY_INITIALIZE_OUTPUT(VertexData,vd);\n"+
				"					DeferredOutput deferredOut;\n"+
				"						UNITY_INITIALIZE_OUTPUT(DeferredOutput,deferredOut);\n"+
				"						deferredOut.Occlusion = 1;\n");
		//if (SD.UseShellDepth){
		//	ShaderCode.Append("					half shellDepth = 0;");
		//}				
		if (SD.Interpolators!=null){
			SD.PredefineStarts = new string[]{"","","","","","",""};
			SD.PredefineEnds = new string[]{"","","","","","",""};
			SD.worthlessref = "";
			SD.PredefinesDepth = -1;
			foreach(ShaderVertexInterpolator i in SD.Interpolators){
				if (i.InFragment&&!(i.ShaderVertexInterpolatorType==ShaderVertexInterpolatorType.Position)&&!(i.ShaderVertexInterpolatorType==ShaderVertexInterpolatorType.Inline)){
					ShaderCode.Append(GenerateInterpolatorPredefines(i,ref SD.worthlessref,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds));
					if (i.FragmentCode!="")
						ShaderCode.Append("				vd." + i.Name + " = " + i.FragmentCode + ";\n");
					else
						ShaderCode.Append("				vd." + i.Name + " = vtp." + i.Name + ";\n");
				}
			}
			ShaderCode.Append(GenerateInterpolatorPredefines(null,ref SD.worthlessref,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds));
		}
		
		if (SD.UsesDepth)
			ShaderCode.Append("				VertexData NonDisplacedvd = vd;\n");
		
		if (SD.UsesPreviousBaseColor)
			ShaderCode.Append("				half4 previousBaseColor = 0;//Honestly just a quick hack to get Metal specular working XD\n");
		
		if (SD.UsesOneMinusAlpha)
			ShaderCode.Append("				half OneMinusAlpha = 0; //An optimization to avoid redundant 1-a if already calculated in ingredient function :)\n");
		
		//if (SD.UsesUVOffset)
		//	ShaderCode.Append("				half3 uvOffset = 0;\n				half3 tempUvOffset = 0;\n");
		
		
		if (SD.Interpolators!=null&&SD.Interpolators.Count>3&&SD.Interpolators[3].InFragment){
			if (!SD.ShaderPassIsDeferred)
				ShaderCode.Append("				outputNormal = vd.worldNormal;\n");
			else
				ShaderCode.Append("				deferredOut.Normal = vd.worldNormal;\n");
		}
	
	
		StringBuilder SurfaceMix = new StringBuilder();
		
		foreach(ShaderLayerList SLL in SD.Base.ShaderLayersMasks){
			if (SD.UsedMasksFragment.Contains(SLL)){
			//if (SLL.Used){
				//string surfaceInternals = SLL.GCVariable()+SLL.GenerateCode(SD,false)+"return "+SLL.CodeName+";\n";
				string FunctionHeader = SLL.GenerateHeaderCached(SD,true);
				SurfaceMix.Append("vd." + SLL.CodeName + " = " + FunctionHeader + ";\n");
			}
		}
		
		bool InDepthTest = false;
		bool HasSetDepth = false;
		bool UsesPremul = SD.UsesPremultipliedBlending;
		//bool PreviousUsesPremultipliedBlending = SD.UsesPremultipliedBlending;
		int AttenIncrement = 1;
		foreach(ShaderIngredient ingredient in Surfaces){
			if (!ingredient.IsUsed())continue;
			//PreviousUsesPremultipliedBlending = SD.UsesPremultipliedBlending;//Why previous? What is this code...
			ShaderSurface surface = ingredient as ShaderSurface;
			if (surface!=null){
				string surfaceInternals = surface.GenerateCodeCached();
				if (surfaceInternals.Length==0)
					continue;
				
				
				surfaceInternals = ingredient.GenerateEffectsCode(SD)+surfaceInternals;
				
				if (surface.ShaderSurfaceComponent==ShaderSurfaceComponent.Depth){
					if (InDepthTest){
						InDepthTest = false;
						SurfaceMix.Append("				}\n");
					}
					
					SD.PredefineStarts = new string[]{"","","","","","",""};
					SD.PredefineEnds = new string[]{"","","","","","",""};
					SD.worthlessref = "";
					SD.PredefinesDepth = -1;
					
					foreach(ShaderVertexInterpolator i in SD.Interpolators){
						if (i.InFragment&&i.ParallaxDisplacementVariable!=""){
							SurfaceMix.Append(GenerateInterpolatorPredefines(i,ref SD.worthlessref,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds));
							SurfaceMix.Append("				vd."+i.Name+" = NonDisplacedvd."+i.Name+";\n");
						}
					}
					SurfaceMix.Append(GenerateInterpolatorPredefines(null,ref SD.worthlessref,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds));
				}
				string AT = surface.AlphaBlendMode.Names[surface.AlphaBlendMode.Type];
				//if (AT == "Blend"&&Surfaces.IndexOf(surface)==0)//&&surface.MixAmount.Safe())
				if (AT == "Blend"&&surface.FirstDisplayed)//&&surface.MixAmount.Safe())
					AT = "Replace";
				
				string lerpfactor = "";
				string MulAdd = "";
				//if (surface.UseAlpha.On){
				if (SD.ShaderPassIsDeferred&&surface.ShaderSurfaceComponent==ShaderSurfaceComponent.Color){
					string tempfactor = "";
					string tempfactororig = "";
					if (!surface.MixAmount.Safe()){
						tempfactor=surface.MixAmount.Get()+" * ";MulAdd="";
						tempfactororig=surface.MixAmount.Get();MulAdd="";
					}
					lerpfactor = tempfactororig;
					SurfaceMix.Append("				DeferredOutput deferredOut"+surface.CodeName+" = "+ surface.GenerateHeader(SD,surfaceInternals,true) + ";\n");
					//SurfaceMix.Append("				deferredOut = deferredOut"+surface.CodeName+";\n");
					bool setsAlbedo = surfaceInternals.Contains("deferredOut.Albedo");
					bool setsAlpha = surfaceInternals.Contains("deferredOut.Alpha");
					bool setsSpecular = surfaceInternals.Contains("deferredOut.SpecSmoothness");
					bool setsEmission = surfaceInternals.Contains("deferredOut.Emission");
					if (setsAlbedo){
						if (surface.UseAlphaGenerate&&AT == "Blend"){
							lerpfactor=tempfactor+"deferredOut"+surface.CodeName+".Alpha";MulAdd=" * ";}
						SurfaceMix.Append("				"+ShaderUtil.GenerateMixingCode(SD, "deferredOut.Albedo.rgb","deferredOut"+surface.CodeName+".Albedo.rgb", surface.MixType.Names[surface.MixType.Type], AT, lerpfactor, false,false,"rgb").Replace("\n","\n				"));
						//SurfaceMix.Append("				"+ShaderUtil.GenerateMixingCode(SD, "deferredOut.Alpha","deferredOut"+surface.CodeName+".Alpha.rrrr", surface.MixType.Names[surface.MixType.Type], AT, lerpfactor, false,false,"a").Replace("\n","\n				"));
						//SurfaceMix.Append("				"+ShaderUtil.GenerateMixingCode(SD, "deferredOut.AlbedoAlpha.a","1", "Mix", AT, lerpfactor, false,false,"a").Replace("\n","\n				"));
						
						if (setsAlbedo&&!setsSpecular&&surface.MixType.Names[surface.MixType.Type]=="Mix"){
							if (!surface.MixAmount.Safe())
								SurfaceMix.Append("				deferredOut.SpecSmoothness*=1-(deferredOut"+surface.CodeName+".Alpha*"+surface.MixAmount.Get()+");\n");
							else
								SurfaceMix.Append("				deferredOut.SpecSmoothness*=OneMinusAlpha;\n");
						}
						if (setsAlbedo&&!setsEmission&&surface.MixType.Names[surface.MixType.Type]=="Mix"){
							if (!surface.MixAmount.Safe())
								SurfaceMix.Append("				deferredOut.Emission*=1-(deferredOut"+surface.CodeName+".Alpha*"+surface.MixAmount.Get()+");\n");
							else
								SurfaceMix.Append("				deferredOut.Emission*=OneMinusAlpha;\n");
						}
					}
					if (setsAlpha){
						if (surface.UseAlphaGenerate&&AT == "Blend"){
							lerpfactor=tempfactor+"deferredOut"+surface.CodeName+".Alpha";MulAdd=" * ";}
						SurfaceMix.Append("				"+ShaderUtil.GenerateMixingCode(SD, "deferredOut.Alpha","deferredOut"+surface.CodeName+".Alpha.rrrr", surface.MixType.Names[surface.MixType.Type], AT, lerpfactor, false,false,"a").Replace("\n","\n				"));
					}
					lerpfactor = tempfactororig;
					if (setsSpecular){
						if (surface.UseAlphaGenerate){
							lerpfactor=tempfactororig;}//tempfactor+"deferredOut"+surface.CodeName+".Alpha";MulAdd=" * ";}
						//SurfaceMix.Append("				"+ShaderUtil.GenerateMixingCode(SD, "deferredOut.SpecSmoothness.rgb","deferredOut"+surface.CodeName+".SpecSmoothness.rgb", surface.MixType.Names[surface.MixType.Type], AT, lerpfactor, false,false,"rgb").Replace("\n","\n				"));
						SurfaceMix.Append("				"+ShaderUtil.GenerateMixingCode(SD, "deferredOut.SpecSmoothness.rgba","deferredOut"+surface.CodeName+".SpecSmoothness.rgba", surface.MixType.Names[surface.MixType.Type], "Replace", lerpfactor, false,false,"z").Replace("\n","\n				"));
						//lerpfactor=tempfactororig;
						//SurfaceMix.Append("				"+ShaderUtil.GenerateMixingCode(SD, "deferredOut.SpecSmoothness.a","deferredOut"+surface.CodeName+".SpecSmoothness.a", "Mix", AT, lerpfactor, false,false,"r").Replace("\n","\n				"));
					
						if (setsSpecular&&!setsAlbedo&&surface.MixType.Names[surface.MixType.Type]=="Mix"){
							if (!surface.MixAmount.Safe())
								SurfaceMix.Append("				deferredOut.Albedo*=1-(deferredOut"+surface.CodeName+".Alpha*"+surface.MixAmount.Get()+");\n");
							else
								SurfaceMix.Append("				deferredOut.Albedo*=OneMinusAlpha;\n");
						}
						if (setsEmission&&!setsAlbedo&&surface.MixType.Names[surface.MixType.Type]=="Mix"){
							if (!surface.MixAmount.Safe())
								SurfaceMix.Append("				deferredOut.Emission*=1-(deferredOut"+surface.CodeName+".Alpha*"+surface.MixAmount.Get()+");\n");
							else
								SurfaceMix.Append("				deferredOut.Emission*=OneMinusAlpha;\n");
						}
					}
					lerpfactor = tempfactororig;
					if (surfaceInternals.Contains("deferredOut.AmbientDiffuse")){
						if (surface.UseAlphaGenerate)
							lerpfactor=tempfactororig;///+"deferredOut"+surface.CodeName+".AlbedoAlpha.a";MulAdd=" * ";}
						SurfaceMix.Append("				"+ShaderUtil.GenerateMixingCode(SD, "deferredOut.AmbientDiffuse.rgb","deferredOut"+surface.CodeName+".AmbientDiffuse.rgb", "Mix", AT, lerpfactor, false,false,"rgb").Replace("\n","\n				"));
					}
					lerpfactor = tempfactororig;
					if (setsEmission){
						if (surface.UseAlphaGenerate)
							lerpfactor=tempfactororig;///+"deferredOut"+surface.CodeName+".AlbedoAlpha.a";MulAdd=" * ";}
						SurfaceMix.Append("				"+ShaderUtil.GenerateMixingCode(SD, "deferredOut.Emission.rgb","deferredOut"+surface.CodeName+".Emission.rgb", surface.MixType.Names[surface.MixType.Type], AT, lerpfactor, false,false,"rgb").Replace("\n","\n				"));
						if (!setsAlbedo&&surface.MixType.Names[surface.MixType.Type]=="Mix"){
							if (!surface.MixAmount.Safe())
								SurfaceMix.Append("				deferredOut.Albedo*=1-(deferredOut"+surface.CodeName+".Alpha*"+surface.MixAmount.Get()+");\n");
							else
								SurfaceMix.Append("				deferredOut.Albedo*=OneMinusAlpha;\n");
						}
						if (!setsSpecular&&surface.MixType.Names[surface.MixType.Type]=="Mix"){
							if (!surface.MixAmount.Safe())
								SurfaceMix.Append("				deferredOut.SpecSmoothness*=1-(deferredOut"+surface.CodeName+".Alpha*"+surface.MixAmount.Get()+");\n");
							else
								SurfaceMix.Append("				deferredOut.SpecSmoothness*=OneMinusAlpha;\n");
						}
					}
				}
				else{
					if (surface.UseAlphaGenerate&&surface.ShaderSurfaceComponent==ShaderSurfaceComponent.Color){
						lerpfactor+=MulAdd+"output"+surface.CodeName+".a";MulAdd=" * ";}
					if (!surface.MixAmount.Safe()){
						lerpfactor+=MulAdd+surface.MixAmount.Get();MulAdd=" * ";}
					
					string WhereToOutput = "Nowhere to output! Please send an email to ztechnologies.com@gmail.com with your shader and this error :)\n";
					if (surface.ShaderSurfaceComponent==ShaderSurfaceComponent.Color)
						WhereToOutput = "outputColor";
					if (surface.ShaderSurfaceComponent==ShaderSurfaceComponent.Normal){
						if (SD.ShaderPassIsDeferred)
							WhereToOutput = "deferredOut.Normal.xyz";
						else
							WhereToOutput = "outputNormal";
					}
					if (surface.ShaderSurfaceComponent==ShaderSurfaceComponent.Depth&&(surface.CompareDepth.On&&HasSetDepth))
						WhereToOutput = "tempDepth";
					if (surface.ShaderSurfaceComponent==ShaderSurfaceComponent.Depth&&(!surface.CompareDepth.On||!HasSetDepth))
						WhereToOutput = "depth";
					
					if (surface.ShaderSurfaceComponent==ShaderSurfaceComponent.Color)
							SurfaceMix.Append("				half4 output"+surface.CodeName+" = "+ surface.GenerateHeader(SD,surfaceInternals,true) + ";\n");
						else if (surface.ShaderSurfaceComponent==ShaderSurfaceComponent.Normal)
							SurfaceMix.Append("				half3 output"+surface.CodeName+" = "+ surface.GenerateHeader(SD,surfaceInternals,true) + ";\n");
						else if (surface.ShaderSurfaceComponent==ShaderSurfaceComponent.Depth)
							SurfaceMix.Append("				half3 output"+surface.CodeName+" = "+ surface.GenerateHeader(SD,surfaceInternals,true) + ";\n");
					
							//SurfaceMix.Append("output"+surface.CodeName + ".rgb *= " + surface.MixAmount.Get() + ";\n");
					
					string endtag = "rgba";
					if (surface.ShaderSurfaceComponent==ShaderSurfaceComponent.Normal)
						endtag = "rgb";
					else if (surface.ShaderSurfaceComponent==ShaderSurfaceComponent.Depth)
						endtag = "rgb";
					
					string thelerpisalie = lerpfactor;
					if (!surface.MixAmount.Safe()&&surface.IsPremultiplied(SD)&&endtag.Length==4){
						SurfaceMix.Append("output"+surface.CodeName + "."+endtag+" *= " + surface.MixAmount.Get() + ";\n");
						lerpfactor = "";
						if (surface.UseAlphaGenerate)
							lerpfactor+="output"+surface.CodeName+".a";
					}
					//Debug.Log(SD.UsesPremultipliedBlending);
					if (thelerpisalie=="output"+surface.CodeName+".a"&&surface.OutputsOneMinusAlpha)
						SurfaceMix.Append("				"+ShaderUtil.GenerateMixingCode(SD, WhereToOutput,"output"+surface.CodeName, surface.MixType.Names[surface.MixType.Type], AT, lerpfactor,"OneMinusAlpha", surface.IsPremultiplied(SD),SD.UsesPremultipliedBlending,endtag).Replace("\n","\n				"));
					else
						SurfaceMix.Append("				"+ShaderUtil.GenerateMixingCode(SD, WhereToOutput,"output"+surface.CodeName, surface.MixType.Names[surface.MixType.Type], AT, lerpfactor, surface.IsPremultiplied(SD),SD.UsesPremultipliedBlending,endtag).Replace("\n","\n				"));
					if (surface.ShaderSurfaceComponent==ShaderSurfaceComponent.Normal&&surface.NormalizeMix.On&&lerpfactor.Length>0){
						if (SD.ShaderPassIsDeferred)
							SurfaceMix.Append("deferredOut.Normal.xyz = normalize(deferredOut.Normal.xyz);\n");
						else
							SurfaceMix.Append("outputNormal = normalize(outputNormal);\n");
					}
						
					//if (surface.ShaderSurfaceComponent==ShaderSurfaceComponent.Normal&&!SD.ShaderPassIsDeferred){///TOoDO: Why not deferred again?
					if (surface.ShaderSurfaceComponent==ShaderSurfaceComponent.Normal){
						if (SD.ShaderPassIsDeferred)
							SurfaceMix.Append("				vd.worldNormal = deferredOut.Normal.xyz;\n");
						else
							SurfaceMix.Append("				vd.worldNormal = outputNormal;\n");
						
						if (SD.Interpolators[11].InFragment)///TODO: unbake worldRefl
							SurfaceMix.Append("				vd.worldRefl = reflect(-vd.worldViewDir, vd.worldNormal);\n");
					}
				}
				
				if (surface.ShaderSurfaceComponent==ShaderSurfaceComponent.Depth){
					if (surface.CompareDepth.On&&HasSetDepth){
						SurfaceMix.Append("				if (depth.x<=tempDepth.x){\n"+
						"				depth = tempDepth;\n");
						InDepthTest = true;
					}
					SD.PredefineStarts = new string[]{"","","","","","",""};
					SD.PredefineEnds = new string[]{"","","","","","",""};
					SD.worthlessref = "";
					SD.PredefinesDepth = -1;
					
					foreach(ShaderVertexInterpolator i in SD.Interpolators){
						if (i.InFragment&&i.ParallaxDisplacementVariable!=""){
							SurfaceMix.Append(GenerateInterpolatorPredefines(i,ref SD.worthlessref,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds));
							//if (i.ParallaxDisplacementVariable!="tangentViewDir.xy")
							//if (SD.UsesUVOffset&&i.ParallaxDisplacementVariable=="tangentViewDir.xy")
							//	SurfaceMix.Append("				vd."+i.Name+" += uvOffset;\n");
							//else
								SurfaceMix.Append("				vd."+i.Name+" += vd."+i.ParallaxDisplacementVariable+"*depth.x;\n");
							//else
							//	SurfaceMix += "				vd."+i.Name+" += vd."+i.ParallaxDisplacementVariable+"/vd.tangentViewDir.z*depth;\n";
						}
					}
					SurfaceMix.Append(GenerateInterpolatorPredefines(null,ref SD.worthlessref,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds));
					
					
					SD.PredefinesDepth = -1;
					
					foreach(ShaderVertexInterpolator i in SD.Interpolators){
						if (i.InFragment&&!(i.ShaderVertexInterpolatorType==ShaderVertexInterpolatorType.Position)&&!(i.ShaderVertexInterpolatorType==ShaderVertexInterpolatorType.Inline)){
							foreach(ShaderVertexInterpolator eye in i.ReliesOn){
								if (eye.InFragment&&eye.ParallaxDisplacementVariable!=""){
									SurfaceMix.Append(GenerateInterpolatorPredefines(i,ref SD.worthlessref,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds));
									if (i.FragmentCode!="")
										SurfaceMix.Append("				vd." + i.Name + " = " + i.FragmentCode + ";\n");
									else
										SurfaceMix.Append("				vd." + i.Name + " = vtp." + i.Name + ";\n");
								}
							}
						}
					}
					SurfaceMix.Append(GenerateInterpolatorPredefines(null,ref SD.worthlessref,ref SD.PredefinesDepth,SD.PredefineStarts,SD.PredefineEnds));
					
					//Just gonna hard code this here...so done...
					if (ingredient as ShaderSurfaceParallaxOcclusionMapping!=null&&(ingredient as ShaderSurfaceParallaxOcclusionMapping).ShadowCorrection.On){
					if (SD.UsesUnityGIFragment){
						if (SD.ShaderPassIsForward)
								SurfaceMix.Append(
							"				//Lotso fun\n"+
							"				#if defined (SHADOWS_SCREEN)\n"+
							"					#if defined(UNITY_NO_SCREENSPACE_SHADOWS)\n"+
							"						vtp._ShadowCoord = mul( unity_World2Shadow[0], float4(vd.worldPos,1));\n"+
							"					#else\n"+
							"						vtp._ShadowCoord = ComputeScreenPos(mul(UNITY_MATRIX_VP,float4(vd.worldPos,1)));\n"+
							"					#endif\n"+
							"				#endif\n"+
							"				// ---- Spot light shadows\n"+
							"				#if defined (SHADOWS_DEPTH) && defined (SPOT)\n"+
							"					vtp._ShadowCoord = mul (unity_World2Shadow[0], float4(vd.worldPos,1));\n"+
							"				#endif\n"+
							"				// ---- Point light shadows\n"+
							"				#if defined (SHADOWS_CUBE)\n"+
							"					vtp._ShadowCoord = vd.worldPos.xyz - _LightPositionRange.xyz;\n"+
							"				#endif\n"+
							"				#define lightCoord lightCoord"+AttenIncrement.ToString()+"//Oh Unity...\n"+
							"					UNITY_LIGHT_ATTENUATION(atten"+AttenIncrement.ToString()+", vtp, vd.worldPos)\n"+
							"				#undef lightCoord\n"+
							"				vd.Atten = atten"+AttenIncrement.ToString()+";\n"+
							"				giInput.atten = atten"+AttenIncrement.ToString()+";\n"
							);
							AttenIncrement++;
					}
					if (SD.ShaderPassIsShadowCaster)
							SurfaceMix.Append(
						"				//More fun\n"+
						"				#ifdef SHADOWS_CUBE\n"+
						"					vd.vec = vd.worldPos.xyz - _LightPositionRange.xyz;\n"+
						"				#else\n"+
						"					#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE\n"+
						"						vd.hpos = mul(UNITY_MATRIX_VP, vd.worldPos);\n"+
						"						vd.hpos = UnityApplyLinearShadowBias(vd.hpos);\n"+
						"					#endif\n"+
						"				#endif\n"
						);
					}
					HasSetDepth = true;
				}
			}else{
				ShaderIngredientUpdateMask maskupdater = ingredient as ShaderIngredientUpdateMask;
				if (maskupdater!=null){
					foreach(ShaderVar sv in maskupdater.Masks){
						if (sv.Obj!=null){
							ShaderLayerList SLL = (ShaderLayerList)sv.Obj;
							SurfaceMix.Append("				vd." + SLL.CodeName+" = "+SLL.GenerateHeaderCached(SD,true)+";\n");
							//SurfaceMix.Append("				vd." + SLL.CodeName+" = "+SLL.GenerateHeader(SD,SLL.GenerateCode(SD,false),true)+";\n");
						}
					}
				}
			}
		}
		foreach(ShaderIngredient ingredient in GeometryModifiers){
			if (!ingredient.IsUsed())continue;
			ShaderGeometryModifier geometryModifier = ingredient as ShaderGeometryModifier;
			if (geometryModifier!=null){
				if (!geometryModifier.HasFlag(ShaderGeometryModifierComponent.Transparency))
					continue;
				string T_ = "";
				if (geometryModifier.HasFlag(ShaderGeometryModifierComponent.Vertex)&&geometryModifier.HasFlag(ShaderGeometryModifierComponent.Transparency))
					T_ = "T_";
				string surfaceInternals = geometryModifier.GenerateTransparency(SD);
				if (SD.ShaderPassIsDeferred){
					//SurfaceMix.Append("				"+ShaderUtil.GenerateMixingCode(SD, "deferredOut.Alpha","deferredOut"+surface.CodeName+".Alpha", "Mix", AT, lerpfactor, false,false,"r").Replace("\n","\n				"));
					SurfaceMix.Append("				deferredOut.Alpha = " + geometryModifier.GenerateHeader(SD,surfaceInternals,true,T_) + ".Alpha;\n");
				}else{
					SurfaceMix.Append("				outputColor = " + geometryModifier.GenerateHeader(SD,surfaceInternals,true,T_) + ";\n");
				}
			}
		}
		SD.UsesPremultipliedBlending = UsesPremul;
		if (InDepthTest){
			InDepthTest = false;
			SurfaceMix.Append("				}\n");
		}
		
		if (SD.UsesUnityGIFragment){
			GenerateGI(SD,ShaderCode);
			AttenIncrement++;
		}
		ShaderCode.Append(SurfaceMix);
		if (SD.ShaderPassIsDeferred)
		ShaderCode.Append(
			"				outDiffuse = half4(deferredOut.Albedo,deferredOut.Occlusion);\n"+
			"				outSpecSmoothness = deferredOut.SpecSmoothness;\n"+
			"				outNormal.xyz = deferredOut.Normal * 0.5 + 0.5;\n"+
			"				outEmission.rgb = deferredOut.Emission + deferredOut.Albedo * deferredOut.AmbientDiffuse;\n"+
			"				#ifndef UNITY_HDR_ON\n"+
			"					outEmission.rgb = exp2(-outEmission.rgb);\n"+
			"				#endif\n"
		);
		if (SD.UsesDepthCorrection){
			if (SD.ShaderPassIsShadowCaster)
				ShaderCode.Append("				float4 mullde = mul(UNITY_MATRIX_VP,float4(vd.worldPos,1));\n"+
					"				outputDepth = (mullde.z+unity_LightShadowBias.x)/mullde.w;\n");
			else
				ShaderCode.Append("				float4 mullde = mul(UNITY_MATRIX_VP,float4(vd.worldPos,1));\n"+
					"				outputDepth = mullde.z/mullde.w;\n");
		}
		if (SD.ForwardFog&&!ShaderSandwich.Unity4&&!SD.ShaderPassIsDeferred){
			ShaderCode.Append("				UNITY_APPLY_FOG(vtp.fogCoord, outputColor); // apply fog (UNITY_FOG_COORDS));\n");
		}
		
		if (SD.ShaderPassIsShadowCaster)
			ShaderCode.Append("				SHADOW_CASTER_FRAGMENT(vd)\n");
		if (SD.ShaderPassIsMask)
			ShaderCode.Append("				return 0;\n");
		else
		if (SD.ShaderPassIsForward)
			ShaderCode.Append("				return outputColor;\n");
			//ShaderCode.Append("outputColor.rgba = 0;\n");
		
		ShaderCode.Append("\n			}\n");
		return ShaderCode.ToString();
	}
	public void GenerateGI(ShaderData SD, StringBuilder ShaderCode){
			ShaderCode.Append( 
			"				UnityGI gi;\n"+
			"				UNITY_INITIALIZE_OUTPUT(UnityGI,gi);\n");
			ShaderCode.Append(
			"// Setup lighting environment\n"+
			"				#ifndef USING_DIRECTIONAL_LIGHT\n"+
			"					fixed3 lightDir = normalize(UnityWorldSpaceLightDir(vd.worldPos));\n"+
			"				#else\n"+
			"					fixed3 lightDir = _WorldSpaceLightPos0.xyz;\n"+
			"				#endif\n");
			
			if (SD.ShaderPassIsForward&&!SD.ShaderPassIsShadowCaster&&SD.PipelineStage == PipelineStage.Fragment)
				ShaderCode.Append(	
			"				//Uses SHADOW_COORDS\n"+
			"				UNITY_LIGHT_ATTENUATION(atten, vtp, vd.worldPos)\n");
			else
				ShaderCode.Append(	
						"				half atten = 1;\n");
			if (SD.ShaderPassIsForward)
				ShaderCode.Append(
			"				vd.Atten = atten;\n"+
			"				gi.indirect.diffuse = 0;\n"+
			"				gi.indirect.specular = 0;\n"+
			"				#if !defined(LIGHTMAP_ON)\n"+
			"					gi.light.color = _LightColor0.rgb;\n"+
			"					gi.light.dir = lightDir;\n"+
			"					gi.light.ndotl = LambertTerm (vd.worldNormal, gi.light.dir);\n"+
			"				#endif\n");
			else
				ShaderCode.Append(	
			"				vd.Atten = atten;\n"+
			"				gi.indirect.diffuse = 0;\n"+
			"				gi.indirect.specular = 0;\n"+
			"				gi.light.color = 0;\n"+
			"				gi.light.dir = half3(0,1,0);\n"+
			"				gi.light.ndotl = 0;");
			
			ShaderCode.Append(
			"				// Call GI (lightmaps/SH/reflections) lighting function\n"+
			"				UnityGIInput giInput;\n"+
			"				UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);\n"+
			"				giInput.light = gi.light;\n"+
			"				giInput.worldPos = vd.worldPos;\n"+
			"				giInput.worldViewDir = vd.worldViewDir;\n"+
			"				giInput.atten = atten;\n"+
			"				#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)\n"+
			"					giInput.lightmapUV = vd.lmap;\n"+
			"				#else\n"+
			"					giInput.lightmapUV = 0.0;\n"+
			"				#endif\n"+
			"				#if UNITY_SHOULD_SAMPLE_SH\n"+
			"					giInput.ambient = vd.sh;\n"+
			"				#else\n"+
			"					giInput.ambient.rgb = 0.0;\n"+
			"				#endif\n"+
			"				giInput.probeHDR[0] = unity_SpecCube0_HDR;\n"+
			"				giInput.probeHDR[1] = unity_SpecCube1_HDR;\n"+
			"				#if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION\n"+
			"					giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending\n"+
			"				#endif\n"+
			"				#if UNITY_SPECCUBE_BOX_PROJECTION\n"+
			"					giInput.boxMax[0] = unity_SpecCube0_BoxMax;\n"+
			"					giInput.probePosition[0] = unity_SpecCube0_ProbePosition;\n"+
			"					giInput.boxMax[1] = unity_SpecCube1_BoxMax;\n"+
			"					giInput.boxMin[1] = unity_SpecCube1_BoxMin;\n"+
			"					giInput.probePosition[1] = unity_SpecCube1_ProbePosition;\n"+
			"				#endif\n");
	}
	public string GeneratePassStart2(ShaderData SD,string PassName, string LightMode, string MultiCompile, string Define, string Exclude){
		string ShaderCode = "";
		ShaderCode +=
		//"AlphaToMask "+(TransparencyOn.On&&TransparencyType.Type==0&&TransparencyAlphaToCoverage.On?"On":"Off")+"\n"+
		"AlphaToMask "+(SD.AlphaToMask?"On":"Off")+"\n"+
		"	Pass {\n" + 
		"		Name \"" + PassName+"\"\n" + 
		"		Tags { ";
		if (LightMode!="")
			ShaderCode +="\"LightMode\" = \"" + LightMode+"\" ";
		//if (PokeOn.On)
		//	ShaderCode +="\"SSPoke\" = \"Yes\" ";
		ShaderCode +=
		"}\n" + 
		GenerateTags(SD)+"\n"+
		"		\n" + 
		"		CGPROGRAM\n" + 
		"			// compile directives\n";
		if (!SD.UsesTessellation)
		ShaderCode += 
		"				#pragma vertex Vertex\n"+
		"				#pragma fragment Pixel\n";
		else
		ShaderCode += 
		"				#pragma vertex Vertex\n"+
		"				#pragma fragment Pixel\n"+
		"				#pragma hull Hull\n"+
		"				#pragma domain Domain\n";
		///TODO: Double check this define works
		//#if UNITY_5_4_OR_NEWER
		//	string[] SMWeirdmap = new string[]{"2.5",	"2.0",	"2.5","3.0","es3.0","4.0","es3.1","gl4.1","5.0"};
		//#else
			string[] SMWeirdmap = new string[]{"3.0",	"2.0",	"2.5","3.0","es3.0","4.0","es3.1","gl4.1","5.0"};
		//#endif
		//if (SMWeirdmap[(int)SD.ShaderModel]=="2.5")
		//	ShaderCode+="				#if UNITY_VERSION>540\n	";
		if (SD.ShaderModel==ShaderModel.Auto)
			ShaderCode +=
			"				#pragma target "+SMWeirdmap[(int)SD.ShaderModelAuto]+"\n";
		else
			ShaderCode +=
			"				#pragma target "+SMWeirdmap[(int)SD.ShaderModel]+"\n";
		//if (SMWeirdmap[(int)SD.ShaderModel]=="2.5")
		//	ShaderCode+="				#else\n"+
		//				"					#pragma target 3.0\n"+
		//				"				#endif\n";
		if (SD.ForwardFog&&!ShaderSandwich.Unity4&&SD.ShaderPass != ShaderPasses.Deferred)
			ShaderCode +=
			"				#pragma multi_compile_fog\n";
			
		ShaderCode += "				#pragma multi_compile __ UNITY_COLORSPACE_GAMMA\n";
		
		if (ShaderSandwich.Instance.OpenShader.TechExcludeDX9.On)
			Exclude+=" d3d9";
		if (Exclude!="")
			ShaderCode += "				#pragma exclude_renderers"+Exclude+"\n";
		
		/*if (IndirectAmbientOcclusionOn.On&&IndirectAmbientOcclusionControl.Type==0){
		ShaderCode +=
			"				#pragma shader_feature Visualize_AO\n"+
			"				#pragma shader_feature ApplyAOToDiffuse\n";
		}*/
		if (MultiCompile.Length>0)
		ShaderCode +=
		"				#pragma " + MultiCompile+"\n";
		
		ShaderCode+=
		"				#include \"HLSLSupport.cginc\"\n" + 
		"				#include \"UnityShaderVariables.cginc\"\n" + 
		"				#define " + Define+"\n" + 
		"				#include \"UnityCG.cginc\"\n" + 
		"				#include \"Lighting.cginc\"\n";
		if (!ShaderSandwich.Unity4)
		ShaderCode +=
		"				#include \"UnityPBSLighting.cginc\"\n";
		ShaderCode +=
		"				#include \"AutoLight.cginc\"\n" + 
		"\n" + 
		"				#define INTERNAL_DATA\n" + 
		"				#define WorldReflectionVector(data,normal) data.worldRefl\n" + 
		"				#define WorldNormalVector(data,normal) normal\n";
		//ShaderCode +=
		//"				(DEFINEGLSLHERE)\n";
		
		///TOoDO: Double check in U5.3
		//if (!SD.Lightmaps||SD.ShaderPass==ShaderPasses.Mask||SD.ShaderPass == ShaderPasses.ShadowCaster)
		//ShaderCode += "				#define LIGHTMAP_OFF\n";
		//ShaderCode += "				#ifndef LIGHTMAP_OFF\n					#define LIGHTMAP_OFF\n				#endif\n";
		
		if (SD.UsesTessellation)
		ShaderCode+=
		"				#include \"Tessellation.cginc\" //Include some Unity code for tessellation.\n";
		return ShaderCode;	
	}
	public string GenerateTags(ShaderData SD){
		string ShaderCode = "";
		if (!SD.Wireframe){
			if (SD.ForwardFog==false)
				ShaderCode+="	Fog {Mode Off}\n";
			else{
				if (SD.ShaderPass==ShaderPasses.ForwardAdd)
					ShaderCode+="	Fog { Color (0,0,0,0) }\n";
			}
			if (SD.ZTestMode==ZTestMode.Auto)
				SD.ZTestMode = ZTestMode.LEqual;
			ShaderCode+="	ZTest "+SD.ZTestMode.ToString()+"\n";
			
			if (SD.ZWriteMode==ZWriteMode.Auto)
				SD.ZWriteMode = ZWriteMode.On;
			ShaderCode+="	ZWrite "+SD.ZWriteMode.ToString()+"\n";
			if (SD.BlendFactorSrc==BlendFactor.None||SD.BlendFactorDest==BlendFactor.None){
				ShaderCode+="	Blend Off//No transparency\n";
			}else{
				ShaderCode+="	Blend "+SD.BlendFactorSrc.ToString()+" "+SD.BlendFactorDest.ToString()+"\n";
			}
			
			ShaderCode+="	Cull "+SD.CullMode.ToString()+"//Culling specifies which sides of the models faces to hide.\n";
		}
		if (SD.ShaderPass == ShaderPasses.Mask)
			ShaderCode+="	ColorMask 0\n";
		return ShaderCode;
	}
	public string GenerateUniforms(ShaderData SD,string TheCoolPartsOfTheShader){
		StringBuilder ShaderCode = new StringBuilder("			//Make our inputs accessible by declaring them here.\n");
			#if PRE_UNITY_5
				if (IsPBR){
					ShaderCode.Append("				samplerCUBE _Cube;\n");
				}
				if (MiscLightmap.On&&SD.ShaderPass!=ShaderPasses.ForwardAdd){
					ShaderCode.Append("				#ifndef LIGHTMAP_OFF\n"+
								"					float4 unity_LightmapST;\n"+
								"				#endif\n"+
								"				#ifndef LIGHTMAP_OFF\n"+
								"					sampler2D unity_Lightmap;\n"+
								"					#ifndef DIRLIGHTMAP_OFF\n"+
								"						sampler2D unity_LightmapInd;\n"+
								"					#endif\n"+
								"				#endif\n");
				}
			#endif
			/*if (IndirectAmbientOcclusionOn.On){
				if (IndirectAmbientOcclusionControl.Type==0)
					ShaderCode.Append( 
					"				//Vertex Toastie Uniforms\n"+
					"					float _AOPower;\n"+
					"					float _AOStrength;\n");
				
				ShaderCode.Append(
				"					float testpow(float a,float b){\n");
				
				//if (MiscVertexToastieOptimized.On)
				//	ShaderCode+=
				//	"						return a;\n";
				//else
					ShaderCode.Append(
					"						return lerp(a,a*a,b/4);\n");
				
				ShaderCode.Append(
				"					}\n");
			}*/
			//ShaderCode+="				float4 specColor;\n";
			foreach(ShaderInput SI in SD.Base.ShaderInputs){
				if (SI.InEditor||SI.SpecialType==InputSpecialTypes.None||(SD.Temp&&(SI.InEditor||SI.SpecialType==InputSpecialTypes.None))){
					ShaderCode.Append("				");
					if (SI.Type==ShaderInputTypes.Image){
						ShaderCode.Append("sampler2D "+SI.Get()+";\n");
						ShaderCode.Append("float4 "+SI.Get()+"_ST;\n");
						ShaderCode.Append("float4 "+SI.Get()+"_HDR;\n");
					}
					if ((SI.Type==ShaderInputTypes.Color||SI.Type==ShaderInputTypes.Vector)&&(SI.Get()!="_SpecColor"))
						ShaderCode.Append("float4 "+SI.Get()+";\n");
					if (SI.Type==ShaderInputTypes.Cubemap){
						ShaderCode.Append("samplerCUBE " + SI.Get()+";\n");
						ShaderCode.Append("float4 " + SI.Get()+"_HDR;\n");
					}
					if (SI.Type==ShaderInputTypes.Float || SI.Type==ShaderInputTypes.Range)
						ShaderCode.Append("float " + SI.Get()+";\n");
				}
			}
			if (ShaderSandwich.Instance.ImprovedUpdates&&SD.Temp){
				ShaderCode.Append("\n			//Inputs for temporary quicker editing.\n");
			
				foreach(ShaderVar SV in ShaderUtil.GetAllShaderVarsCached()){
					if (SV.Input==null){
						if (SV.CType == Types.Vec)
							ShaderCode.Append("				float4 SSTEMPSV" + SV.GetID().ToString()+";//" + SV.Name+"\n");
						if (SV.CType == Types.Float&&!SV.NoInputs)
							ShaderCode.Append("				float SSTEMPSV" + SV.GetID().ToString()+";//" + SV.Name+"\n");
						
						if (SV.CType == Types.Texture){
							ShaderCode.Append("				sampler2D SSTEMPSV" + SV.GetID().ToString()+";//" + SV.Name+"\n");
							ShaderCode.Append("				float4 SSTEMPSV" + SV.GetID().ToString()+"_HDR;//" + SV.Name+"\n");
						}
						if (SV.CType == Types.Cubemap){
							ShaderCode.Append("				samplerCUBE SSTEMPSV" + SV.GetID().ToString()+";//" + SV.Name+"\n");
							ShaderCode.Append("				float4 SSTEMPSV" + SV.GetID().ToString()+"_HDR;//" + SV.Name+"\n");
						}
					}
				}
			}else{
				foreach(ShaderVar SV in ShaderUtil.GetAllShaderVarsCached()){
					if (SV.Input==null){
						if (SV.CType == Types.Texture&&TheCoolPartsOfTheShader.Contains("SSTEMPSV" + SV.GetID().ToString())){
							ShaderCode.Append("				sampler2D SSTEMPSV" + SV.GetID().ToString()+";//" + SV.Name+"\n");
							ShaderCode.Append("				float4 SSTEMPSV" + SV.GetID().ToString()+"_HDR;//" + SV.Name+"\n");
						}
						if (SV.CType == Types.Cubemap&&TheCoolPartsOfTheShader.Contains("SSTEMPSV" + SV.GetID().ToString())){
							ShaderCode.Append("				samplerCUBE SSTEMPSV" + SV.GetID().ToString()+";//" + SV.Name+"\n");
							ShaderCode.Append("				float4 SSTEMPSV" + SV.GetID().ToString()+"_HDR;//" + SV.Name+"\n");
						}
					}
				}
			}
			if (SD.Temp){
				ShaderCode.Append("			//Setup some time stuff for the Shader Sandwich preview.\n" +
							"				float4 _SSTime;\n" +
							"				float4 _SSSinTime;\n" +
							"				float4 _SSCosTime;\n");
			}
		/*if (PokeCount.Float>0f&&PokeOn.On){
			ShaderCode+=	"			//Poke uniforms\n";
			for(int i = 0;i<PokeCount.Float;i++){
				string si = i.ToString();
				ShaderCode+="				float3 _Poke"+si+"Position;\n" +
							"				float3 _Poke"+si+"Velocity;\n" +
							"				float  _Poke"+si+"TimeNormalized;\n" +
							"				float  _Poke"+si+"Time;\n"+
							"				float  _Poke"+si+"MaxDistance;\n";
				for(int ii = 0; ii<PokeFloats.Float;ii++)
					ShaderCode+="				float  _Poke"+si+"CustomF_"+ii.ToString()+";\n";
				for(int ii = 0; ii<PokeFloat3s.Float;ii++)
					ShaderCode+="				float3 _Poke"+si+"CustomF3_"+ii.ToString()+";\n";
				if (PokeLine.On){
				ShaderCode+="				float3 _Poke"+si+"PreviousPosition;\n" +
							"				float3 _Poke"+si+"PreviousVelocity;\n" +
							"				float  _Poke"+si+"PreviousTimeNormalized;\n" +
							"				float  _Poke"+si+"PreviousTime;\n"+
							"				float  _Poke"+si+"PreviousMaxDistance;\n";
				for(int ii = 0; ii<PokeFloats.Float;ii++)
					ShaderCode+="				float  _Poke"+si+"PreviousCustomF_"+ii.ToString()+";\n";
				for(int ii = 0; ii<PokeFloat3s.Float;ii++)
					ShaderCode+="				float3 _Poke"+si+"PreviousCustomF3_"+ii.ToString()+";\n";
				}
				ShaderCode+="\n";
			}
		}*/
		
		return ShaderCode.ToString();
	}
	public string GeneratePassEnd(ShaderGenerate SG){
		string ShaderCode = "";
			ShaderCode +=
			"		ENDCG\n"+
			"	}\n";
		return ShaderCode;
	}
	public string GetTexCoord(ref int Texcoord){
		Texcoord+=1;
		return (Texcoord-1).ToString();
	}
	public string Save(){
		string S = "		Begin Shader Pass\n";
			S += ShaderUtil.SaveDict(GetSaveLoadDict(),3);
			foreach(ShaderIngredient surface in Surfaces){
				S += surface.Save();
			}
			S += "			Geometry Ingredients\n";
			foreach(ShaderIngredient geometryModifier in GeometryModifiers){
				S += geometryModifier.Save();
			}
			S += "		End Shader Pass\n\n";
		return S;
	}
	/*public void CorrectStuff(){
		SpecularHardness.Range0 = 0.0001f;
		if (IsPBR){
			SpecularOn.On = true;
			SpecularEnergy.On = true;
			SpecularLightingType.Type = 0;
		}

		ShellsCount.NoInputs=true;
		ShellsEase.Range0 = 0f;
		ShellsEase.Range1 = 3f;
		ParallaxHeight.Range1 = 0.4f;
		ParallaxBinaryQuality.NoInputs = true;
		
		TechLOD.NoInputs = true;
		TechLOD.Range0 = 0;
		TechLOD.Range1 = 1000;
		TechLOD.Float = Mathf.Round(TechLOD.Float);
		
		TechShaderTarget.NoInputs = true;
		TechShaderTarget.Range0 = 2;
		TechShaderTarget.Range1 = 5;
		TechShaderTarget.Float = Mathf.Round(TechShaderTarget.Float);
		if (!MiscShadows.On)
			MiscFullShadows.On = false;
		if (ShaderSandwich.Instance.OpenShader.FileVersion<2.2f){
			if (!TransparencyZWrite.On)
			TransparencyZWriteType.Type = 0;
			else
			TransparencyZWriteType.Type += 1;
		}
		if (ShaderSandwich.Instance.OpenShader.FileVersion<2.3f){
			if (TechZTest.Type==0){
				TechZTest.Type = 2;
				TechZWrite.Type = 2;
			}
			TechZTest.Type-=1;
		}
	}*/
	public void Load(StringReader S){
		Surfaces.Clear();
		GeometryModifiers.Clear();
		var D = GetSaveLoadDictLegacy();
		bool geometryyet = false;
		while(1==1){
			string Line =  S.ReadLine();
			if (Line!=null){
				Line = ShaderUtil.Sanitize(Line);
				if(Line=="End Shader Pass")break;
				if(Line=="Geometry Ingredients")geometryyet = true;

				if (Line.Contains(":"))
					ShaderUtil.LoadLine(D,Line);
				else if (Line=="Begin Shader Ingredient"){
					//Debug.Log("Starting shader ingredient!");
					ShaderIngredient SI = ShaderIngredient.Load(S);
					//Debug.Log("new Ingerdeingt!");
					if (SI!=null){
							//Debug.Log(SI.GetType().ToString());
						//if (ShaderSandwich.NonDumbShaderSurfaces.ContainsKey(SI.GetType().Name)){
						if (ShaderSandwich.NonDumbShaderGeometryModifiers.ContainsKey(SI.GetType().Name))
							geometryyet = true;
						if (!geometryyet)
							Surfaces.Add(SI);
						//}
						else//{
							GeometryModifiers.Add(SI);
						//}
					}
				}
			}
			else
			break;
		}
		selectedIngredient = null;
		if (Surfaces.Count>0)
		selectedIngredient = Surfaces[0];
	}
	public void LoadLegacy(StringReader S){
		var D = GetSaveLoadDictLegacy();
		List<ShaderLayerList> legacyLists = GetShaderLayerListsLegacy();
		while(1==1){
			string Line =  S.ReadLine();
			if (Line!=null){
				if(Line=="EndShaderPass")break;

				if (Line.Contains("#!"))
					ShaderUtil.LoadLineLegacy(D,Line);
				else if (Line=="BeginShaderLayerList")
					ShaderLayerList.LoadLegacy(S,legacyLists,null);
			}
			else
			break;
		}
		
		if (ShaderSandwich.Instance.OpenShader.FileVersion<2.2f){
			if (!D["ZWrite"].On)
				D["ZWrite Type"].Type = 0;
			else
				D["ZWrite Type"].Type += 1;
		}
		if (ShaderSandwich.Instance.OpenShader.FileVersion<2.3f){
			if (D["Ztest"].Type==0){
				D["Ztest"].Type = 2;
				D["BaseZWrite"].Type = 2;
			}
			D["Ztest"].Type-=1;
		}
		
		
		Surfaces.Clear();
		
		ShaderGeometryModifierMisc geomMisc = (ShaderGeometryModifierMisc)GeometryModifiers[0];
		geomMisc.ZWriteMode.TakeValues(D["BaseZWrite"]);
		if (D["Cull"].Type==0)geomMisc.CullMode.Type=2;
		if (D["Cull"].Type==1)geomMisc.CullMode.Type=0;
		if (D["Cull"].Type==2)geomMisc.CullMode.Type=1;
		
		if (D["Ztest"].Type==0)geomMisc.ZTestMode.Type=2;
		if (D["Ztest"].Type==1)geomMisc.ZTestMode.Type=4;
		if (D["Ztest"].Type==2)geomMisc.ZTestMode.Type=3;
		if (D["Ztest"].Type==3)geomMisc.ZTestMode.Type=5;
		if (D["Ztest"].Type==4)geomMisc.ZTestMode.Type=6;
		if (D["Ztest"].Type==5)geomMisc.ZTestMode.Type=7;
		if (D["Ztest"].Type==6)geomMisc.ZTestMode.Type=1;
		
		if (D["Tech Shader Target"].Float>=2f)geomMisc.ShaderModel.Type=1;
		if (D["Tech Shader Target"].Float>=3f)geomMisc.ShaderModel.Type=3;
		if (D["Tech Shader Target"].Float>=4f)geomMisc.ShaderModel.Type=5;
		if (D["Tech Shader Target"].Float>=5f)geomMisc.ShaderModel.Type=8;
		
		
		geomMisc.UseFog.TakeValues(D["Use Fog"]);
		geomMisc.UseForwardVertexLights.TakeValues(D["Use Vertex Lights"]);
		geomMisc.UseLightmaps.TakeValues(D["Use Lightmaps"]);
		geomMisc.GenerateDeferredPass.TakeValues(D["Deferred"]);
		geomMisc.GenerateForwardBasePass.TakeValues(D["Forward"]);
		geomMisc.GenerateForwardAddPass.TakeValues(D["Forward Add"]);
		geomMisc.GenerateShadowCasterPass.TakeValues(D["Shadow Caster"]);
		
		geomMisc.UseShadows.TakeValues(D["Shadows"]);
		geomMisc.UseForwardFullShadows.TakeValues(D["Use All Shadows"]);
		//geomMisc.TechZTest.TakeValues(D["Cull"]);
		if (D["Parallax On"].On){
			ShaderSurfaceParallaxOcclusionMapping parallax = (ShaderSurfaceParallaxOcclusionMapping)ScriptableObject.CreateInstance(typeof(ShaderSurfaceParallaxOcclusionMapping));
			Surfaces.Add((ShaderIngredient)parallax);
			parallax.SurfaceLayers.SLs.Clear();
			parallax.Parallax.TakeValues(D["Parallax Height"]);
			parallax.LinearSearch.TakeValues(D["Parallax Quality"]);
			parallax.LinearSearch.Float/=2f;
			parallax.LinearSearch.Float = Mathf.Round(parallax.LinearSearch.Float);
			parallax.LinearSearchShadow.TakeValues(parallax.Parallax);
			if (D["Silhouette Clipping"].On == true)
				parallax.SilhouetteClipping.Type = 1;
			parallax.DepthCorrection.TakeValues(D["Parallax Depth Correction"]);
			parallax.ShadowCorrection.TakeValues(D["Parallax Shadows"]);
			
			parallax.SurfaceLayers.EndTag.Text = legacyLists[7].EndTag.Text;
			foreach(ShaderLayer SL in legacyLists[7]){
				parallax.SurfaceLayers.SLs.Add(SL);
			}
		}
		
		if (legacyLists[4].SLs.Count>0){
			ShaderSurfaceTangentNormals normals = (ShaderSurfaceTangentNormals)ScriptableObject.CreateInstance(typeof(ShaderSurfaceTangentNormals));
			Surfaces.Add((ShaderIngredient)normals);
			normals.SurfaceLayers.SLs.Clear();
			foreach(ShaderLayer SL in legacyLists[4]){
				normals.SurfaceLayers.SLs.Add(SL);
			}
		}
		if (D["Diffuse On"].On&&D["Lighting Type"].Type!=4){
			ShaderSurfaceDiffuse diffuse = (ShaderSurfaceDiffuse)ScriptableObject.CreateInstance(typeof(ShaderSurfaceDiffuse));
			Surfaces.Add((ShaderIngredient)diffuse);
			diffuse.SurfaceLayers.SLs.Clear();
			diffuse.Link(this);
			diffuse.LightingType.Type = D["Lighting Type"].Type+1;
			diffuse.Smoothness.TakeValues(D["Spec Hardness"]);
			diffuse.Roughness.TakeValues(D["Setting1"]);
			diffuse.WrapAmount.TakeValues(D["Setting1"]);
			diffuse.WrapColor.TakeValues(D["Wrap Color"]);
			diffuse.DisableNormals.TakeValues(D["Disable Normals"]);
			diffuse.PBRQuality.TakeValues(D["PBR Quality"]);
			
			if (!D["Use Ambient"].On){
				diffuse.UseAmbient.On = false;
			}
			foreach(ShaderLayer SL in legacyLists[0]){
				diffuse.SurfaceLayers.SLs.Add(SL);
			}
			
			if (legacyLists[9].SLs.Count>0){
				diffuse.UseCustomLighting.On = true;
				//diffuse.CustomLightingLayers.SLs.Clear();
				foreach(ShaderLayer SL in legacyLists[9]){
					diffuse.CustomLightingLayers.SLs.Add(SL);
				}
			}
			if (legacyLists[12].SLs.Count>0){
				diffuse.UseCustomLighting.On = true;
				//diffuse.CustomLightingLayers.SLs.Clear();
				foreach(ShaderLayer SL in legacyLists[12]){
					diffuse.CustomLightingLayers.SLs.Add(SL);
				}
			}
			if (legacyLists[10].SLs.Count>0){
				diffuse.UseCustomAmbient.On = true;
				foreach(ShaderLayer SL in legacyLists[10]){
					diffuse.CustomAmbientLayers.SLs.Add(SL);
				}
			}
		}else{
			ShaderSurfaceUnlit unlit = (ShaderSurfaceUnlit)ScriptableObject.CreateInstance(typeof(ShaderSurfaceUnlit));
			Surfaces.Add((ShaderIngredient)unlit);
			unlit.SurfaceLayers.SLs.Clear();
			unlit.Link(this);
			foreach(ShaderLayer SL in legacyLists[0]){
				unlit.SurfaceLayers.SLs.Add(SL);
			}
		}
		if (D["Shells On"].On){
			ShaderGeometryModifierShells shells = (ShaderGeometryModifierShells)ScriptableObject.CreateInstance(typeof(ShaderGeometryModifierShells));
			GeometryModifiers.Add((ShaderIngredient)shells);
			shells.SurfaceLayers.SLs.Clear();
			shells.ShellCount.TakeValues(D["Shell Count"]);
			shells.ShellDistance.TakeValues(D["Shells Distance"]);
			shells.ShellEase.TakeValues(D["Shell Ease"]);
			///TODO; INV?
			shells.ShellSkipFirst.TakeValues(D["Shell Skip First"]);
			if (D["Shell Front"].On)
				shells.ShellSide.Type = 0;
			else
				shells.ShellSide.Type = 1;
		}
		if (D["Transparency On"].On){
			ShaderSurfaceTransparency surfaceTransparency = (ShaderSurfaceTransparency)ScriptableObject.CreateInstance(typeof(ShaderSurfaceTransparency));
			ShaderGeometryModifierTransparency geomTransparency = (ShaderGeometryModifierTransparency)ScriptableObject.CreateInstance(typeof(ShaderGeometryModifierTransparency));
			Surfaces.Add((ShaderIngredient)surfaceTransparency);
			GeometryModifiers.Add((ShaderIngredient)geomTransparency);
			surfaceTransparency.SurfaceLayers.SLs.Clear();///TOoDO: Update for Alpha To Mask
			geomTransparency.ShadowTypeFade.Type = 0;
			geomTransparency.Transparency.TakeValues(D["Transparency"]);
			if (D["Correct Shadows"].On)
				geomTransparency.ShadowTypeFade.Type = 1;
			if (D["ZWrite Type"].Type==0)
				geomTransparency.TransparencyZWrite.Type = 0;
			if (D["ZWrite Type"].Type==1)
				geomTransparency.TransparencyZWrite.Type = 2;
			if (D["ZWrite Type"].Type==2)
				geomTransparency.TransparencyZWrite.Type = 3;
			if (D["Transparency Type"].Type==0){
				geomTransparency.TransparencyType.Type = 0;
				geomTransparency.CutoutAmount.TakeValues(D["Transparency"]);
				if (D["Alpha To Coverage"].On){
					geomTransparency.TransparencyType.Type = 2;
					geomTransparency.Transparency.Float = 1f-geomTransparency.Transparency.Float;
					geomTransparency.CutoutAmount.TakeValues(D["CutoffZWriteTransparency"]);
				}
			}
			if (D["Transparency Type"].Type==1){
				geomTransparency.TransparencyType.Type = 1;
				geomTransparency.Transparency.Float = 1f-geomTransparency.Transparency.Float;
				geomTransparency.CutoutAmount.TakeValues(D["CutoffZWriteTransparency"]);
			}
			///TOoDO: BlendMode
			if (D["Blend Mode"].Type==0)
				geomTransparency.BlendMode.Type = 0;
			if (D["Blend Mode"].Type==1)
				geomTransparency.BlendMode.Type = 1;
			if (D["Blend Mode"].Type==2)
				geomTransparency.BlendMode.Type = 3;
			
			surfaceTransparency.SurfaceLayers.EndTag.Text = legacyLists[1].EndTag.Text;
			
			
			ShaderLayer baseSL = ScriptableObject.CreateInstance<ShaderLayer>();
			baseSL.SetType("SLTNumber");
			baseSL.CustomData["Number"].Float = 1f;
			baseSL.Parent = surfaceTransparency.SurfaceLayers;
			baseSL.Name.Text = "Legacy Transparency Base";
			surfaceTransparency.SurfaceLayers.SLs.Add(baseSL);
			foreach(ShaderLayer SL in legacyLists[1]){
				surfaceTransparency.SurfaceLayers.SLs.Add(SL);
			}
		}
		if (D["Specular On"].On){
			ShaderSurfaceSpecular specular = (ShaderSurfaceSpecular)ScriptableObject.CreateInstance(typeof(ShaderSurfaceSpecular));
			Surfaces.Add((ShaderIngredient)specular);
			specular.Link(this);
			specular.SurfaceLayers.SLs.Clear();///TOoDO: 0.3,0.3,0.3,0.3
			
			specular.Smoothness.TakeValues(D["Spec Hardness"]);
			specular.PBRModel.TakeValues(D["PBR Model"]);
			specular.PBRQuality.TakeValues(D["PBR Quality"]);
			specular.ConserveEnergy.TakeValues(D["Spec Energy Conserve"]);
			specular.Offset.TakeValues(D["Spec Offset"]);
			specular.SpecularType.Type = D["Specular Type"].Type+1;
			if (specular.SpecularType.Type>1||(D["Transparency On"].On&&D["Transparency Type"].Type==0)||!D["Use PBR"].On)
				specular.AlphaBlendMode.Type = 1;
			
			if (!D["Use Ambient"].On){
				specular.UseAmbient.On = false;
			}
			
			ShaderLayer baseSL = ScriptableObject.CreateInstance<ShaderLayer>();
			baseSL.SetType("SLTNumber");
			baseSL.CustomData["Number"].Float = 0.3f;
			baseSL.Parent = specular.SurfaceLayers;
			baseSL.Name.Text = "Legacy Specular Base";
			
			if (specular.PBRModel.Type==1){
				specular.SurfaceLayers.EndTag.Text = legacyLists[2].EndTag.Text;
			}
			specular.SurfaceLayers.SLs.Add(baseSL);
			foreach(ShaderLayer SL in legacyLists[2]){
				specular.SurfaceLayers.SLs.Add(SL);
			}
			
			if (legacyLists[11].SLs.Count>0){
				specular.UseCustomLighting.On = true;
				//specular.CustomLightingLayers.SLs.Clear();
				foreach(ShaderLayer SL in legacyLists[9]){
					specular.CustomLightingLayers.SLs.Add(SL);
				}
				specular.UseCustomAmbient.On = true;
				//specular.CustomLightingLayers.SLs.Clear();
				foreach(ShaderLayer SL in legacyLists[9]){
					specular.CustomAmbientLayers.SLs.Add(SL);
				}
			}
			if (legacyLists[12].SLs.Count>0){
				specular.UseCustomLighting.On = true;
				//specular.CustomLightingLayers.SLs.Clear();
				foreach(ShaderLayer SL in legacyLists[12]){
					specular.CustomLightingLayers.SLs.Add(SL);
				}
				
				specular.UseCustomAmbient.On = true;
				//specular.CustomLightingLayers.SLs.Clear();
				foreach(ShaderLayer SL in legacyLists[12]){
					specular.CustomAmbientLayers.SLs.Add(SL);
				}
			}
		}
		if (D["Emission On"].On){
			ShaderSurfaceEmission emission = (ShaderSurfaceEmission)ScriptableObject.CreateInstance(typeof(ShaderSurfaceEmission));
			Surfaces.Add((ShaderIngredient)emission);
			emission.SurfaceLayers.SLs.Clear();
			if (D["Emission Type"].Type==0)
				emission.MixType.Type = 1;
			if (D["Emission Type"].Type==2)
				emission.MixType.Type = 0;
			foreach(ShaderLayer SL in legacyLists[5]){
				emission.SurfaceLayers.SLs.Add(SL);
			}
			if (D["Emission Type"].Type==1){//Multiply is really d += s*d
				emission.MixType.Type = 1;
				
				ShaderLayer baseSL = ScriptableObject.CreateInstance<ShaderLayer>();
				baseSL.SetType("SLTCurrentOutputColor");
				baseSL.Parent = emission.SurfaceLayers;
				baseSL.Name.Text = "Legacy Emission Multiply Mode";
				baseSL.MixType.Type = 3;
				emission.SurfaceLayers.SLs.Add(baseSL);
			}
		}
		if (D["Tessellation On"].On){
			ShaderGeometryModifierTessellation tessellation = (ShaderGeometryModifierTessellation)ScriptableObject.CreateInstance(typeof(ShaderGeometryModifierTessellation));
			GeometryModifiers.Add((ShaderIngredient)tessellation);
			tessellation.SurfaceLayers.SLs.Clear();
			tessellation.Type.TakeValues(D["Tessellation Type"]);
			tessellation.Quality.TakeValues(D["Tessellation Quality"]);
			tessellation.ScreenSize.TakeValues(D["Tessellation Quality"]);
			tessellation.ScreenSize.Float = 51f-tessellation.ScreenSize.Float;
			if (tessellation.ScreenSize.Input!=null)
				tessellation.ScreenSize.Input.Number = 51f-tessellation.ScreenSize.Input.Number;
			tessellation.Falloff.TakeValues(D["Tessellation Falloff"]);
			
			if (D["Tessellation Toplogy"].Type==1){
				ShaderGeometryModifierToPoints toPoints = (ShaderGeometryModifierToPoints)ScriptableObject.CreateInstance(typeof(ShaderGeometryModifierToPoints));
				GeometryModifiers.Add((ShaderIngredient)toPoints);
			}
		}
		if (legacyLists[8].Count>0){
			bool usesPosition = false;
			bool usesNormal = false;
			foreach(ShaderLayer SL in legacyLists[8]){
				if (SL.LegacyDictionary["Vertex Mask"].Float==1&&(!SL.IsGrayscale()||!(SL.MixType.Type==1)||!(SL.LegacyDictionary["Vertex Space"].Float==0f)))
					usesNormal = true;
				if (SL.LegacyDictionary["Vertex Mask"].Float==2)
					usesPosition = true;
			}
			///TOoDO; Create right masks
			ShaderLayerList positionMask = null;
			ShaderLayerList normalMask = null;
			ShaderLayer baseSL = null;
			if (usesPosition){
				ShaderSandwich.Instance.OpenShader.AddMask(false);
				positionMask = ShaderSandwich.Instance.OpenShader.ShaderLayersMasks[ShaderSandwich.Instance.OpenShader.ShaderLayersMasks.Count-1];
				positionMask.Name.Text = "Legacy Position Mask";
				positionMask.EndTag.Text = "rgba";
				
					baseSL = ScriptableObject.CreateInstance<ShaderLayer>();
					baseSL.SetType("SLTLiteral");
					baseSL.MapType.Type = 2;
					baseSL.MapSpace.Type = 1;
					baseSL.Parent = positionMask;
					baseSL.Name.Text = "Legacy Object Space Position";
				positionMask.SLs.Add(baseSL);
			}
			if (usesNormal){
				ShaderSandwich.Instance.OpenShader.AddMask(false);
				normalMask = ShaderSandwich.Instance.OpenShader.ShaderLayersMasks[ShaderSandwich.Instance.OpenShader.ShaderLayersMasks.Count-1];
				normalMask.Name.Text = "Legacy Normal Mask";
				normalMask.EndTag.Text = "rgba";
				
					baseSL = ScriptableObject.CreateInstance<ShaderLayer>();
					baseSL.SetType("SLTLiteral");
					baseSL.MapDirection.Type = 1;
					baseSL.MapType.Type = 4;
					baseSL.MapSpace.Type = 1;
					baseSL.Parent = normalMask;
					baseSL.Name.Text = "Legacy Object Space Normal";
				normalMask.SLs.Add(baseSL);
			}
			
			
			int Space = -1;//Local,World
			int VertexMask = -1;//None,Normal,Position
			ShaderGeometryModifier geom = null;
			foreach(ShaderLayer SL in legacyLists[8]){
				int ToVertexMask = (int)SL.LegacyDictionary["Vertex Mask"].Float;
				int ToSpace = (int)SL.LegacyDictionary["Vertex Space"].Float;
				
				if ((ToVertexMask!=VertexMask&&ToSpace==0)||ToSpace!=Space){//Need to create new modifier?
					//if ((ToVertexMask==0||ToVertexMask==1)&&(ToVertexMask!=1||SL.IsGrayscale())&&SL.MixType.Type==1&&SL.LegacyDictionary["Vertex Space"].Float==0f){//possible to recreate with displace
					if ((ToVertexMask==0||ToVertexMask==1)&&SL.MixType.Type==1&&SL.LegacyDictionary["Vertex Space"].Float==0f){//possible to recreate with displace
						ShaderGeometryModifierDisplacement displace = (ShaderGeometryModifierDisplacement)ScriptableObject.CreateInstance(typeof(ShaderGeometryModifierDisplacement));
						geom = displace;
						displace.MidLevel.Float = 0f;
						if (SL.LegacyDictionary["Vertex Mask"].Float==0)
							displace.Direction.Type = 2;
						else
							displace.Direction.Type = 0;
						displace.IndependentXYZ.On = true;
						baseSL = ScriptableObject.CreateInstance<ShaderLayer>();
						baseSL.SetType("SLTNumber");
						baseSL.CustomData["Number"].Float = 0f;
						baseSL.Parent = displace.SurfaceLayers;
						baseSL.Name.Text = "Legacy Displacement Base";
						displace.SurfaceLayers.Add(baseSL);
					}else{
						ShaderGeometryModifierSet set = (ShaderGeometryModifierSet)ScriptableObject.CreateInstance(typeof(ShaderGeometryModifierSet));
						geom = set;
					}
					GeometryModifiers.Add((ShaderIngredient)geom);
				}
				Space = ToSpace;
				VertexMask = ToVertexMask;
				if (!((ToVertexMask==0||ToVertexMask==1)&&SL.MixType.Type==1&&SL.LegacyDictionary["Vertex Space"].Float==0f)){
					SL.AddLayerEffect("SSEMaskMultiply");
					if (ToVertexMask==1){
						SL.LayerEffects[SL.LayerEffects.Count-1].Inputs["Values"].Obj = normalMask;
						SL.LayerEffects[SL.LayerEffects.Count-1].Inputs["Values"].Selected = ShaderSandwich.Instance.OpenShader.ShaderLayersMasks.IndexOf(normalMask);
					}
					if (ToVertexMask==2){
						SL.LayerEffects[SL.LayerEffects.Count-1].Inputs["Values"].Obj = positionMask;
						SL.LayerEffects[SL.LayerEffects.Count-1].Inputs["Values"].Selected = ShaderSandwich.Instance.OpenShader.ShaderLayersMasks.IndexOf(positionMask);
					}
				}
				geom.SurfaceLayers.SLs.Add(SL);
				
			}
		}
		
		if (D["Indirect Subsurface Scattering On"].On){
			ShaderSurfaceSubSurfaceScattering sss = (ShaderSurfaceSubSurfaceScattering)ScriptableObject.CreateInstance(typeof(ShaderSurfaceSubSurfaceScattering));
			Surfaces.Add((ShaderIngredient)sss);
			sss.SSSType.Type = 1;
			sss.SurfaceLayers.SLs.Clear();
			sss.MixAmount.TakeValues(D["SSS Mix"]);
			sss.UseThickness.TakeValues(D["SSS Use Thickness"]);
			sss.Ambient.TakeValues(D["SSS Constant Scatter"]);
			sss.Density.TakeValues(D["SSS Density"]);
			sss.Distortion.TakeValues(D["SSS Fresnel Distortion"]);
			sss.Thinness.TakeValues(D["SSS Fresnel Thinness"]);
			
			foreach(ShaderLayer SL in legacyLists[6]){
				sss.SurfaceLayers.SLs.Add(SL);
			}
		}
		selectedIngredient = null;
		if (Surfaces.Count>0)
			selectedIngredient = Surfaces[0];
		//CorrectStuff();
	}
	public List<ShaderLayerList> GetShaderLayerListsLegacy(){
		ShaderLayerList	ShaderLayersDiffuse = ScriptableObject.CreateInstance<ShaderLayerList>();
		ShaderLayersDiffuse.UShaderLayerList("Albedo","Albedo","The color of the surface.","Texture","d.Albedo","rgb","",new Color(0.8f,0.8f,0.8f,1f));
		ShaderLayerList	ShaderLayersAlpha = ScriptableObject.CreateInstance<ShaderLayerList>();
		ShaderLayersAlpha.UShaderLayerList("Alpha","Alpha","Which parts are see through.","Transparency","d.Alpha","a","",new Color(1f,1f,1f,1f));
		ShaderLayerList	ShaderLayersSpecular = ScriptableObject.CreateInstance<ShaderLayerList>();
		ShaderLayersSpecular.UShaderLayerList("Specular","Specular","Where the shine appears.","Gloss","d.Specular","rgb","",new Color(0.3f,0.3f,0.3f,1f));
		ShaderLayerList	ShaderLayersMetallic = ScriptableObject.CreateInstance<ShaderLayerList>();
		ShaderLayersMetallic.UShaderLayerList("Metallic","Metallic","How metallic the surface is.","Metallic","d.Metallic","r","",new Color(0.3f,0.3f,0.3f,1f));
		ShaderLayerList	ShaderLayersNormal = ScriptableObject.CreateInstance<ShaderLayerList>();
		ShaderLayersNormal.UShaderLayerList("Normals","Normals","Used to add fake bumps.","NormalMap","d.Normal","rgb","",new Color(0f,0f,1f,1f));
		ShaderLayerList	ShaderLayersEmission = ScriptableObject.CreateInstance<ShaderLayerList>();
		ShaderLayersEmission.UShaderLayerList("Emission","Emission","Where and what color the glow is.","Emission","d.Emission","rgba","",new Color(0f,0f,0f,1f));
		ShaderLayerList	ShaderLayersHeight = ScriptableObject.CreateInstance<ShaderLayerList>();
		ShaderLayersHeight.UShaderLayerList("Height","Height","Which parts of the shader are higher than the others.","Height","d.Height","a","",new Color(1f,1f,1f,1));
		ShaderLayerList	ShaderLayersVertex = ScriptableObject.CreateInstance<ShaderLayerList>();
		ShaderLayersVertex.UShaderLayerList("Vertex","Vertex","Used to move the model's vertices.","Vertex","Vertex","rgba","",new Color(1f,1f,1f,1));
		ShaderLayerList	ShaderLayersLightingDiffuse = ScriptableObject.CreateInstance<ShaderLayerList>();
		ShaderLayersLightingDiffuse.UShaderLayerList("LightingDiffuse","Diffuse","Customize the diffuse lighting.","Lighting","cDiff","rgb","",new Color(0.8f,0.8f,0.8f,1f));
		ShaderLayerList	ShaderLayersLightingSpecular = ScriptableObject.CreateInstance<ShaderLayerList>();
		ShaderLayersLightingSpecular.UShaderLayerList("LightingSpecular","Specular","Custom speculars highlights and reflections.","Lighting","cSpec","rgb","",new Color(0.8f,0.8f,0.8f,1f));
		ShaderLayerList	ShaderLayersLightingAmbient = ScriptableObject.CreateInstance<ShaderLayerList>();
		ShaderLayersLightingAmbient.UShaderLayerList("LightingIndirect","Ambient","Customize the ambient lighting.","Lighting","cAmb","rgb","",new Color(0.8f,0.8f,0.8f,1f));
		ShaderLayerList	ShaderLayersLightingAll = ScriptableObject.CreateInstance<ShaderLayerList>();
		ShaderLayersLightingAll.UShaderLayerList("LightingDirect","Direct","Custom both direct diffuse and specular lighting.","Lighting","cTemp","rgb","",new Color(0.8f,0.8f,0.8f,1f));
		ShaderLayerList	ShaderLayersSubsurfaceScattering = ScriptableObject.CreateInstance<ShaderLayerList>();
		ShaderLayersSubsurfaceScattering.UShaderLayerList("SubsurfaceScatteringColor","SSS Color","The color underneath the surface.","Scatter","d.SubsurfaceColor","rgb","",new Color(1f,0.0f,0.0f,1f));
		ShaderLayerList	ShaderLayersFragmentPoke = ScriptableObject.CreateInstance<ShaderLayerList>();
		ShaderLayersFragmentPoke.UShaderLayerList("FragmentPoke","Poke Channel 1","Calculated for every individual poke.","PokeChan1","PokeChan1","rgba","",new Color(1f,1f,1f,1f));
		ShaderLayerList	ShaderLayersVertexPoke = ScriptableObject.CreateInstance<ShaderLayerList>();
		ShaderLayersVertexPoke.UShaderLayerList("VertexPoke","Poke Channel 1","Calculated for every individual poke.","PokeChan2","PokeChan2","rgba","",new Color(1f,1f,1f,1f));

		List<ShaderLayerList> tempList = new List<ShaderLayerList>();
		tempList.Add(ShaderLayersDiffuse);
		tempList.Add(ShaderLayersAlpha);
		tempList.Add(ShaderLayersSpecular);
		tempList.Add(ShaderLayersMetallic);
		tempList.Add(ShaderLayersNormal);
		tempList.Add(ShaderLayersEmission);
		tempList.Add(ShaderLayersSubsurfaceScattering);
		tempList.Add(ShaderLayersHeight);
		tempList.Add(ShaderLayersVertex);
		tempList.Add(ShaderLayersLightingDiffuse);
		tempList.Add(ShaderLayersLightingSpecular);
		tempList.Add(ShaderLayersLightingAmbient);
		tempList.Add(ShaderLayersLightingAll);
		tempList.Add(ShaderLayersFragmentPoke);
		tempList.Add(ShaderLayersVertexPoke);
		return tempList;
	}
	public List<ShaderLayerList> GetShaderLayerLists(){
		List<ShaderLayerList> tempList = new List<ShaderLayerList>();
		/*tempList.Add(ShaderLayersDiffuse);
		tempList.Add(ShaderLayersAlpha);
		tempList.Add(ShaderLayersSpecular);
		tempList.Add(ShaderLayersMetallic);
		tempList.Add(ShaderLayersNormal);
		tempList.Add(ShaderLayersEmission);
		tempList.Add(ShaderLayersSubsurfaceScattering);
		tempList.Add(ShaderLayersHeight);
		tempList.Add(ShaderLayersVertex);
		tempList.Add(ShaderLayersLightingDiffuse);
		tempList.Add(ShaderLayersLightingSpecular);
		tempList.Add(ShaderLayersLightingAmbient);
		tempList.Add(ShaderLayersLightingAll);
		tempList.Add(ShaderLayersFragmentPoke);
		tempList.Add(ShaderLayersVertexPoke);*/
		foreach(ShaderIngredient surface in Surfaces){
			if (!surface.IsUsed())continue;
			foreach(ShaderLayerList list in surface.GetMyShaderLayerLists()){
				tempList.Add(list);
			}
		}
		foreach(ShaderIngredient surface in GeometryModifiers){
			if (!surface.IsUsed())continue;
			foreach(ShaderLayerList list in surface.GetMyShaderLayerLists()){
				tempList.Add(list);
			}
		}
		return tempList;
	}
	public List<List<ShaderLayer>> GetShaderLayers(){
		List<List<ShaderLayer>> tempList = new List<List<ShaderLayer>>();
		/*tempList.Add(ShaderLayersDiffuse.SLs);
		tempList.Add(ShaderLayersAlpha.SLs);
		tempList.Add(ShaderLayersSpecular.SLs);
		tempList.Add(ShaderLayersMetallic.SLs);
		tempList.Add(ShaderLayersNormal.SLs);
		tempList.Add(ShaderLayersEmission.SLs);
		tempList.Add(ShaderLayersSubsurfaceScattering.SLs);
		tempList.Add(ShaderLayersHeight.SLs);
		tempList.Add(ShaderLayersVertex.SLs);
		tempList.Add(ShaderLayersLightingDiffuse.SLs);
		tempList.Add(ShaderLayersLightingSpecular.SLs);
		tempList.Add(ShaderLayersLightingAmbient.SLs);
		tempList.Add(ShaderLayersLightingAll.SLs);
		tempList.Add(ShaderLayersFragmentPoke.SLs);
		tempList.Add(ShaderLayersVertexPoke.SLs);*/
		foreach(ShaderIngredient surface in Surfaces){
			if (!surface.IsUsed())continue;
			foreach(ShaderLayerList list in surface.GetMyShaderLayerLists()){
				tempList.Add(list.SLs);
			}
		}
		foreach(ShaderIngredient surface in GeometryModifiers){
			if (!surface.IsUsed())continue;
			foreach(ShaderLayerList list in surface.GetMyShaderLayerLists()){
				tempList.Add(list.SLs);
			}
		}
		return tempList;
	}
	public Dictionary<string,ShaderVar> GetSaveLoadDict(){
		Dictionary<string,ShaderVar> D = new Dictionary<string,ShaderVar>();
		D.Add(Name.Name,Name);
		D.Add(Visible.Name,Visible);
		//D.Add(TechLOD.Name,TechLOD);
		//D.Add(TechCull.Name,TechCull);
		//D.Add(TechZTest.Name,TechZTest);
		//D.Add(TechZWrite.Name,TechZWrite);
		//D.Add(TechShaderTarget.Name,TechShaderTarget);

		/*D.Add(MiscVertexRecalculation.Name,MiscVertexRecalculation);
		D.Add(MiscFog.Name,MiscFog);
		D.Add(MiscAmbient.Name,MiscAmbient);
		D.Add(MiscVertexLights.Name,MiscVertexLights);
		D.Add(MiscLightmap.Name,MiscLightmap);
		D.Add(MiscFullShadows.Name,MiscFullShadows);
		D.Add(MiscForwardAdd.Name,MiscForwardAdd);
		D.Add(MiscShadowCaster.Name,MiscShadowCaster);
		D.Add(MiscForward.Name,MiscForward);
		D.Add(MiscDeferred.Name,MiscDeferred);
		D.Add(MiscShadows.Name,MiscShadows);
		D.Add(MiscInterpolateView.Name,MiscInterpolateView);
		D.Add(MiscRecalculateAfterVertex.Name,MiscRecalculateAfterVertex);*/

		return D;
	}
	/*public void CorrectOld(){
		if (DiffuseLightingType.Type==4)//PBR
			DiffuseLightingType.Type = 0;
		else
		if (DiffuseLightingType.Type==0)//Standard
			DiffuseLightingType.Type = 1;
		else
		if (DiffuseLightingType.Type==1)//Micro
			DiffuseLightingType.Type = 2;
		else
		if (DiffuseLightingType.Type==2)//Transc
			DiffuseLightingType.Type = 3;
		else
		if (DiffuseLightingType.Type==5)//Custom
			DiffuseLightingType.Type = 4;
		else
		if (DiffuseLightingType.Type==3){//Unlit
			DiffuseLightingType.Type = 1;
			DiffuseOn.On = false;
		}
		if (TransparencyType.Type==1)
		TransparencyAmount.Float = 1f-TransparencyAmount.Float;
	}*/
	public string GenerateFunctions(ShaderData SD, string ShaderCode){
		//if (!ShaderSandwich.Instance.ViewLayerNames)
		//return "";
		StringBuilder shaderCode = new StringBuilder(10000);
		
		foreach(string s in SD.AdditionalFunctions){
			shaderCode.Append("\n");
			shaderCode.Append(s);
		}
		
		/*if (ShaderCode.Contains("SillyFlip"))
		shaderCode.Append("\n"+
		"	float2 SillyFlip(float2 inUV){\n"+
		"		#if UNITY_UV_STARTS_AT_TOP\n"+
		"			if (_ProjectionParams.x > 0);\n"+
		"				inUV.y = 1-inUV.y;\n"+
		"		#endif\n"+
		"		inUV.y = 1-inUV.y;///TODO: Make this function work...\n"+
		"		return inUV;\n"+
		"	}\n");*/
		
		/*if (ShaderCode.Contains("GenerateTriPlanar"))
		shaderCode += "\n"+
		"	float4 GenerateTriPlanar(float4 XAxis,float4 YAxis, float4 ZAxis, float3 Map, float idk){\n"+
		"		half3 blend = pow(abs(Map),idk);\n"+
		"		blend /= blend.x+blend.y+blend.z;\n"+
		"		return XAxis*blend.x + YAxis*blend.y + ZAxis*blend.z;\n"+
		"	}\n";*/
		
		/*if (ShaderCode.Contains("DivideByW_XY"))
		shaderCode.Append("\n"+
		"	float3 DivideByW_XY(float4 blerg){\n"+
		"		return float3(blerg.xy/blerg.w,blerg.z);\n"+
		"	}\n");*/
		
		shaderCode.Append("\n"+
		"	float4 GammaToLinear(float4 col){\n");
		
		if (ShaderSandwich.Instance.OpenShader.MiscColorSpace.Type == 1)
		shaderCode.Append(
		"		#if defined(UNITY_COLORSPACE_GAMMA)\n"+
		"			//Best programming evar XD\n"+
		"		#else\n"+
		"			col.rgb = pow(col,2.2);\n"+
		"		#endif\n");
		
		shaderCode.Append(
		"		return col;\n"+
		"	}\n");
		
		shaderCode.Append("\n"+
		"	float4 GammaToLinearForce(float4 col){\n");
		
		shaderCode.Append(
		"		#if defined(UNITY_COLORSPACE_GAMMA)\n"+
		"			//Best programming evar XD\n"+
		"		#else\n"+
		"			col.rgb = pow(col,2.2);\n"+
		"		#endif\n");
		
		shaderCode.Append(
		"		return col;\n"+
		"	}\n");
		
		shaderCode.Append("\n"+
		"	float4 LinearToGamma(float4 col){\n");
		
		if (ShaderSandwich.Instance.OpenShader.MiscColorSpace.Type == 2)
		shaderCode.Append(
		"		#if defined(UNITY_COLORSPACE_GAMMA)\n"+
		"			//Best programming evar XD\n"+
		"		#else\n"+
		"			col.rgb = pow(col,1/2.2);\n"+
		"		#endif\n");
		
		shaderCode.Append(
		"		return col;\n"+
		"	}\n");
		
		shaderCode.Append("\n"+
		"	float4 LinearToGammaForWeirdSituations(float4 col){\n");
		shaderCode.Append(
		"		#if defined(UNITY_COLORSPACE_GAMMA)\n"+
		"			col.rgb = pow(col,2.2);\n"+
		"		#endif\n");
		shaderCode.Append(
		"		return col;\n"+
		"	}\n");
		
		
		
		shaderCode.Append("\n"+
		"	float3 GammaToLinear(float3 col){\n");
		
		if (ShaderSandwich.Instance.OpenShader.MiscColorSpace.Type == 1)
		shaderCode.Append(
		"		#if defined(UNITY_COLORSPACE_GAMMA)\n"+
		"			//Best programming evar XD\n"+
		"		#else\n"+
		"			col = pow(col,2.2);\n"+
		"		#endif\n");
		
		shaderCode.Append(
		"		return col;\n"+
		"	}\n");

		shaderCode.Append("\n"+
		"	float3 GammaToLinearForce(float3 col){\n");
		
		shaderCode.Append(
		"		#if defined(UNITY_COLORSPACE_GAMMA)\n"+
		"			//Best programming evar XD\n"+
		"		#else\n"+
		"			col = pow(col,2.2);\n"+
		"		#endif\n");
		
		shaderCode.Append(
		"		return col;\n"+
		"	}\n");
		
		shaderCode.Append("\n"+
		"	float3 LinearToGamma(float3 col){\n");
		
		if (ShaderSandwich.Instance.OpenShader.MiscColorSpace.Type == 2)
		shaderCode.Append(
		"		#if defined(UNITY_COLORSPACE_GAMMA)\n"+
		"			//Best programming evar XD\n"+
		"		#else\n"+
		"			col = pow(col,1/2.2);\n"+
		"		#endif\n");
		
		shaderCode.Append(
		"		return col;\n"+
		"	}\n");
		

		
		shaderCode.Append( "\n"+
		"	float GammaToLinear(float col){\n");
		
		if (ShaderSandwich.Instance.OpenShader.MiscColorSpace.Type == 1)
		shaderCode.Append(
		"		#if defined(UNITY_COLORSPACE_GAMMA)\n"+
		"			//Best programming evar XD\n"+
		"		#else\n"+
		"			col = pow(col,2.2);\n"+
		"		#endif\n");
		
		shaderCode.Append(
		"		return col;\n"+
		"	}\n");
		
		#if PRE_UNITY_5
		shaderCode.Append("\n"+
		"#ifndef UNITY_LIGHTING_COMMON_INCLUDED\n"+
"#define UNITY_LIGHTING_COMMON_INCLUDED\n"+
"// Transforms direction from object to world space\n"+
"inline float3 UnityObjectToWorldDir( in float3 dir ){\n"+
"	return normalize(mul((float3x3)_Object2World, dir));\n"+
"}\n"+
"// Transforms direction from world to object space\n"+
"inline float3 UnityWorldToObjectDir( in float3 dir ){\n"+
"	return normalize(mul((float3x3)_World2Object, dir));\n"+
"}\n"+
"// Transforms normal from object to world space\n"+
"inline float3 UnityObjectToWorldNormal( in float3 norm ){\n"+
"	// Multiply by transposed inverse matrix, actually using transpose() generates badly optimized code\n"+
"	return normalize(_World2Object[0].xyz * norm.x + _World2Object[1].xyz * norm.y + _World2Object[2].xyz * norm.z);\n"+
"}\n"+
"// Computes world space light direction, from world space position\n"+
"inline float3 UnityWorldSpaceLightDir( in float3 worldPos ){\n"+
"	#ifndef USING_LIGHT_MULTI_COMPILE\n"+
"		return _WorldSpaceLightPos0.xyz - worldPos * _WorldSpaceLightPos0.w;\n"+
"	#else\n"+
"		#ifndef USING_DIRECTIONAL_LIGHT\n"+
"		return _WorldSpaceLightPos0.xyz - worldPos;\n"+
"		#else\n"+
"		return _WorldSpaceLightPos0.xyz;\n"+
"		#endif\n"+
"	#endif\n"+
"}\n"+
"// Computes world space view direction, from object space position\n"+
"inline float3 UnityWorldSpaceViewDir( in float3 worldPos ){\n"+
"	return _WorldSpaceCameraPos.xyz - worldPos;\n"+
"}\n"+
"inline half DotClamped (half3 a, half3 b){\n"+
"	#if (SHADER_TARGET < 30 || defined(SHADER_API_PS3))\n"+
"		return saturate(dot(a, b));\n"+
"	#else\n"+
"		return max(0.0h, dot(a, b));\n"+
"	#endif\n"+
"}\n"+
"inline half LambertTerm (half3 normal, half3 lightDir){\n"+
"	return DotClamped (normal, lightDir);\n"+
"}\n"+
"\n"+
"\n"+
"\n"+
"struct UnityLight\n"+
"{\n"+
"	half3 color;\n"+
"	half3 dir;\n"+
"	half  ndotl;\n"+
"};\n"+
"\n"+
"struct UnityIndirect\n"+
"{\n"+
"	half3 diffuse;\n"+
"	half3 specular;\n"+
"};\n"+
"\n"+
"struct UnityGI\n"+
"{\n"+
"	UnityLight light;\n"+
"	#ifdef DIRLIGHTMAP_SEPARATE\n"+
"		#ifdef LIGHTMAP_ON\n"+
"			UnityLight light2;\n"+
"		#endif\n"+
"		#ifdef DYNAMICLIGHTMAP_ON\n"+
"			UnityLight light3;\n"+
"		#endif\n"+
"	#endif\n"+
"	UnityIndirect indirect;\n"+
"};\n"+
"\n"+
"struct UnityGIInput \n"+
"{\n"+
"	UnityLight light; // pixel light, sent from the engine\n"+
"\n"+
"	float3 worldPos;\n"+
"	float3 worldViewDir;\n"+
"	half atten;\n"+
"	half3 ambient;\n"+
"	float4 lightmapUV; // .xy = static lightmap UV, .zw = dynamic lightmap UV\n"+
"\n"+
"	float4 boxMax[2];\n"+
"	float4 boxMin[2];\n"+
"	float4 probePosition[2];\n"+
"	float4 probeHDR[2];\n"+
"};\n"+
"\n"+
"#endif\n";
		if (IsPBR){
			shaderCode+="\n"+
"#define SHADER_TARGET (30)\n"+
"#define U4Imposter\n"+
"#ifndef UNITY_PBS_LIGHTING_INCLUDED\n"+
"#define UNITY_PBS_LIGHTING_INCLUDED\n"+

"#include \"UnityShaderVariables.cginc\"\n"+
"#ifndef UNITY_STANDARD_CONFIG_INCLUDED\n"+
"#define UNITY_STANDARD_CONFIG_INCLUDED\n"+

"// Define Specular cubemap constants\n"+
"#define UNITY_SPECCUBE_LOD_EXPONENT (1.5)\n"+
"#define UNITY_SPECCUBE_LOD_STEPS (7) // TODO: proper fix for different cubemap resolution needed. My assumptions were actually wrong!\n"+

"// Energy conservation for Specular workflow is Monochrome. For instance: Red metal will make diffuse Black not Cyan\n"+
"#define UNITY_CONSERVE_ENERGY 1\n"+
"#define UNITY_CONSERVE_ENERGY_MONOCHROME 1\n"+
"\n"+
"// High end platforms support Box Projection and Blending\n"+
"#define UNITY_SPECCUBE_BOX_PROJECTION ( !defined(SHADER_API_MOBILE) && (SHADER_TARGET >= 30) )\n"+
"#define UNITY_SPECCUBE_BLENDING ( !defined(SHADER_API_MOBILE) && (SHADER_TARGET >= 30) )\n"+
"\n"+
"#define UNITY_SAMPLE_FULL_SH_PER_PIXEL 0\n"+
"\n"+
"#define UNITY_GLOSS_MATCHES_MARMOSET_TOOLBAG2 1\n"+
"#define UNITY_BRDF_GGX 0\n"+

@"// Orthnormalize Tangent Space basis per-pixel
// Necessary to support high-quality normal-maps. Compatible with Maya and Marmoset.
// However xNormal expects oldschool non-orthnormalized basis - essentially preventing good looking normal-maps :(
// Due to the fact that xNormal is probably _the most used tool to bake out normal-maps today_ we have to stick to old ways for now.
// 
// Disabled by default, until xNormal has an option to bake proper normal-maps.
"+
"#define UNITY_TANGENT_ORTHONORMALIZE 0\n"+
"\n"+
"#endif // UNITY_STANDARD_CONFIG_INCLUDED\n"+
"#ifndef UNITY_GLOBAL_ILLUMINATION_INCLUDED\n"+
"#define UNITY_GLOBAL_ILLUMINATION_INCLUDED\n"+
"\n"+
"// Functions sampling light environment data (lightmaps, light probes, reflection probes), which is then returned as the UnityGI struct.\n"+
"\n"+
"\n"+
"#ifndef UNITY_STANDARD_BRDF_INCLUDED\n"+
"#define UNITY_STANDARD_BRDF_INCLUDED\n"+
"\n"+
"#include "+"\""+"UnityCG.cginc"+"\""+"\n"+
"\n"+
"//-------------------------------------------------------------------------------------\n"+
"half4 unity_LightGammaCorrectionConsts = float4(100,100,100,100);\n"+
"#define unity_LightGammaCorrectionConsts_PIDiv4 (0.7853975)\n"+
"#define unity_LightGammaCorrectionConsts_PI (3.14159)\n"+
"#define unity_LightGammaCorrectionConsts_HalfDivPI (0.008)\n"+
"#define unity_LightGammaCorrectionConsts_8 (64)\n"+
"#define unity_LightGammaCorrectionConsts_SqrtHalfPI (0.04)\n"+
"#define unity_ColorSpaceDielectricSpec float4(0,0,0,0)\n"+
"#define UNITY_PI (3.14159)\n"+
"\n"+
"half4 DecodeHDR (half4 a, half3 b)\n"+
"{\n"+
"	return a;\n"+
"}\n"+
"\n"+
"inline half Pow4 (half x){\n"+
"	return x*x*x*x;\n"+
"}\n"+
"inline half2 Pow4 (half2 x){\n"+
"	return x*x*x*x;\n"+
"}\n"+
"inline half3 Pow4 (half3 x){\n"+
"	return x*x*x*x;\n"+
"}\n"+
"inline half4 Pow4 (half4 x){\n"+
"	return x*x*x*x;\n"+
"}\n"+
"\n"+
"// Pow5 uses the same amount of instructions as generic pow(), but has 2 advantages:\n"+
"// 1) better instruction pipelining\n"+
"// 2) no need to worry about NaNs\n"+
"half Pow5 (half x)\n"+
"{\n"+
"	return x*x * x*x * x;\n"+
"}\n"+
"\n"+
"half2 Pow5 (half2 x)\n"+
"{\n"+
"	return x*x * x*x * x;\n"+
"}\n"+
"\n"+
"half3 Pow5 (half3 x)\n"+
"{\n"+
"	return x*x * x*x * x;\n"+
"}\n"+
"\n"+
"half4 Pow5 (half4 x)\n"+
"{\n"+
"	return x*x * x*x * x;\n"+
"}\n"+
"\n"+
"half BlinnTerm (half3 normal, half3 halfDir)\n"+
"{\n"+
"	return DotClamped (normal, halfDir);\n"+
"}\n"+
"\n"+
"half3 FresnelTerm (half3 F0, half cosA)\n"+
"{\n"+
"	half t = Pow5 (1 - cosA);	// ala Schlick interpoliation\n"+
"	return F0 + (1-F0) * t;\n"+
"}\n"+
"half3 FresnelLerp (half3 F0, half3 F90, half cosA)\n"+
"{\n"+
"	half t = Pow5 (1 - cosA);	// ala Schlick interpoliation\n"+
"	return lerp (F0, F90, t);\n"+
"}\n"+
"// approximage Schlick with ^4 instead of ^5\n"+
"half3 FresnelLerpFast (half3 F0, half3 F90, half cosA)\n"+
"{\n"+
"	half t = Pow4 (1 - cosA);\n"+
"	return lerp (F0, F90, t);\n"+
"}\n"+
"half3 LazarovFresnelTerm (half3 F0, half roughness, half cosA)\n"+
"{\n"+
"	half t = Pow5 (1 - cosA);	// ala Schlick interpoliation\n"+
"	t /= 4 - 3 * roughness;\n"+
"	return F0 + (1-F0) * t;\n"+
"}\n"+
"half3 SebLagardeFresnelTerm (half3 F0, half roughness, half cosA)\n"+
"{\n"+
"	half t = Pow5 (1 - cosA);	// ala Schlick interpoliation\n"+
"	return F0 + (max (F0, roughness) - F0) * t;\n"+
"}\n"+
"\n"+
"// NOTE: Visibility term here is the full form from Torrance-Sparrow model, it includes Geometric term: V = G / (N.L * N.V)\n"+
"// This way it is easier to swap Geometric terms and more room for optimizations (except maybe in case of CookTorrance geom term)\n"+
"\n"+
"// Cook-Torrance visibility term, doesn't take roughness into account\n"+
"half CookTorranceVisibilityTerm (half NdotL, half NdotV,  half NdotH, half VdotH)\n"+
"{\n"+
"	VdotH += 1e-5f;\n"+
"	half G = min (1.0, min (\n"+
"		(2.0 * NdotH * NdotV) / VdotH,\n"+
"		(2.0 * NdotH * NdotL) / VdotH));\n"+
"	return G / (NdotL * NdotV + 1e-4f);\n"+
"}\n"+
"\n"+
"// Kelemen-Szirmay-Kalos is an approximation to Cook-Torrance visibility term\n"+
"// http://sirkan.iit.bme.hu/~szirmay/scook.pdf\n"+
"half KelemenVisibilityTerm (half LdotH)\n"+
"{\n"+
"	return 1.0 / (LdotH * LdotH);\n"+
"}\n"+
"\n"+
"// Modified Kelemen-Szirmay-Kalos which takes roughness into account, based on: http://www.filmicworlds.com/2014/04/21/optimizing-ggx-shaders-with-dotlh/ \n"+
"half ModifiedKelemenVisibilityTerm (half LdotH, half roughness)\n"+
"{\n"+
"	// c = sqrt(2 / Pi)\n"+
"	half c = unity_LightGammaCorrectionConsts_SqrtHalfPI;\n"+
"	half k = roughness * roughness * c;\n"+
"	half gH = LdotH * (1-k) + k;\n"+
"	return 1.0 / (gH * gH);\n"+
"}\n"+
"\n"+
"// Generic Smith-Schlick visibility term\n"+
"half SmithVisibilityTerm (half NdotL, half NdotV, half k)\n"+
"{\n"+
"	half gL = NdotL * (1-k) + k;\n"+
"	half gV = NdotV * (1-k) + k;\n"+
"	return 1.0 / (gL * gV + 1e-4f);\n"+
"}\n"+
"\n"+
"// Smith-Schlick derived for Beckmann\n"+
"half SmithBeckmannVisibilityTerm (half NdotL, half NdotV, half roughness)\n"+
"{\n"+
"	// c = sqrt(2 / Pi)\n"+
"	half c = unity_LightGammaCorrectionConsts_SqrtHalfPI;\n"+
"	half k = roughness * roughness * c;\n"+
"	return SmithVisibilityTerm (NdotL, NdotV, k);\n"+
"}\n"+
"\n"+
"// Smith-Schlick derived for GGX \n"+
"half SmithGGXVisibilityTerm (half NdotL, half NdotV, half roughness)\n"+
"{\n"+
"	half k = (roughness * roughness) / 2; // derived by B. Karis, http://graphicrants.blogspot.se/2013/08/specular-brdf-reference.html\n"+
"	return SmithVisibilityTerm (NdotL, NdotV, k);\n"+
"}\n"+
"\n"+
"half ImplicitVisibilityTerm ()\n"+
"{\n"+
"	return 1;\n"+
"}\n"+
"\n"+
"half RoughnessToSpecPower (half roughness)\n"+
"{\n"+
"roughness+=0.017;\n"+
"#if UNITY_GLOSS_MATCHES_MARMOSET_TOOLBAG2\n"+
"	// from https://s3.amazonaws.com/docs.knaldtech.com/knald/1.0.0/lys_power_drops.html\n"+
"	half n = 10.0 / log2((1-roughness)*0.968 + 0.03);\n"+
"#if defined(SHADER_API_PS3)\n"+
"	n = max(n,-255.9370);  //i.e. less than sqrt(65504)\n"+
"#endif\n"+
"	return n * n;\n"+
"\n"+
"	// NOTE: another approximate approach to match Marmoset gloss curve is to\n"+
"	// multiply roughness by 0.7599 in the code below (makes SpecPower range 4..N instead of 1..N)\n"+
"#else\n"+
"	half m = roughness * roughness * roughness + 1e-4f;	// follow the same curve as unity_SpecCube\n"+
"	half n = (2.0 / m) - 2.0;							// http://jbit.net/%7Esparky/academic/mm_brdf.pdf\n"+
"	n = max(n, 1e-4f);									// prevent possible cases of pow(0,0), which could happen when roughness is 1.0 and NdotH is zero\n"+
"	return n;\n"+
"#endif\n"+
"}\n"+
"\n"+
"// BlinnPhong normalized as normal distribution function (NDF)\n"+
"// for use in micro-facet model: spec=D*G*F\n"+
"// http://www.thetenthplanet.de/archives/255\n"+
"half NDFBlinnPhongNormalizedTerm (half NdotH, half n)\n"+
"{\n"+
"	// norm = (n+1)/(2*pi)\n"+
"	half normTerm = (n + 1.0) * unity_LightGammaCorrectionConsts_HalfDivPI;\n"+
"\n"+
"	half specTerm = pow (NdotH, n);\n"+
"	return specTerm * normTerm;\n"+
"}\n"+
"\n"+
"// BlinnPhong normalized as reflecδion denγity funcδion (RDF)\n"+
"// ready for use directly as specular: spec=D\n"+
"// http://www.thetenthplanet.de/archives/255\n"+
"half RDFBlinnPhongNormalizedTerm (half NdotH, half n)\n"+
"{\n"+
"	half normTerm = (n + 2.0) / (8.0 * UNITY_PI);\n"+
"	half specTerm = pow (NdotH, n);\n"+
"	return specTerm * normTerm;\n"+
"}\n"+
"\n"+
"half GGXTerm (half NdotH, half roughness)\n"+
"{\n"+
"	half a = roughness * roughness;\n"+
"	half a2 = a * a;\n"+
"	half d = NdotH * NdotH * (a2 - 1.f) + 1.f;\n"+
"	return a2 / (UNITY_PI * d * d);\n"+
"}\n"+
"\n"+
"//-------------------------------------------------------------------------------------\n"+
"/*\n"+
"// https://s3.amazonaws.com/docs.knaldtech.com/knald/1.0.0/lys_power_drops.html\n"+
"\n"+
"const float k0 = 0.00098, k1 = 0.9921;\n"+
"// pass this as a constant for optimization\n"+
"const float fUserMaxSPow = 100000; // sqrt(12M)\n"+
"const float g_fMaxT = ( exp2(-10.0/fUserMaxSPow) - k0)/k1;\n"+
"float GetSpecPowToMip(float fSpecPow, int nMips)\n"+
"{\n"+
"   // Default curve - Inverse of TB2 curve with adjusted constants\n"+
"   float fSmulMaxT = ( exp2(-10.0/sqrt( fSpecPow )) - k0)/k1;\n"+
"   return float(nMips-1)*(1.0 - clamp( fSmulMaxT/g_fMaxT, 0.0, 1.0 ));\n"+
"}\n"+
"\n"+
"	//float specPower = RoughnessToSpecPower (roughness);\n"+
"	//float mip = GetSpecPowToMip (specPower, 7);\n"+
"*/\n"+
"\n"+
"// Decodes HDR textures\n"+
"// handles dLDR, RGBM formats\n"+
"// Modified version of DecodeHDR from UnityCG.cginc\n"+
"/*half3 DecodeHDR_NoLinearSupportInSM2 (half4 data, half4 decodeInstructions)\n"+
"{\n"+
"	// If Linear mode is not supported we can skip exponent part\n"+
"\n"+
"	// In Standard shader SM2.0 and SM3.0 paths are always using different shader variations\n"+
"	// SM2.0: hardware does not support Linear, we can skip exponent part\n"+
"	#if defined(UNITY_NO_LINEAR_COLORSPACE) && (SHADER_TARGET < 30)\n"+
"		return (data.a * decodeInstructions.x) * data.rgb;\n"+
"	#else\n"+
"		return DecodeHDR(data, decodeInstructions);\n"+
"	#endif\n"+
"}*/\n"+
"\n"+
"half3 Unity_GlossyEnvironment (samplerCUBE tex, half3 worldNormal, half roughness){\n"+
"#if !UNITY_GLOSS_MATCHES_MARMOSET_TOOLBAG2 || (SHADER_TARGET < 30)\n"+
"	float mip = roughness * UNITY_SPECCUBE_LOD_STEPS;\n"+
"#else\n"+
"	// TODO: remove pow, store cubemap mips differently\n"+
"	float mip = pow(roughness,3.0/4.0) * UNITY_SPECCUBE_LOD_STEPS;\n"+
"#endif\n"+
"\n"+
"	//half4 rgbm = SampleCubeReflection(tex, worldNormal.xyz, mip);\n"+
"	half4 rgbm = texCUBElod(tex, float4(worldNormal.xyz, mip));\n"+
"	return rgbm.rgb;//DecodeHDR_NoLinearSupportInSM2 (rgbm, hdr);\n"+
"}\n"+
"\n"+
"//-------------------------------------------------------------------------------------\n"+
"\n"+
"// Note: BRDF entry points use oneMinusRoughness (aka "+"\""+"smoothness"+"\""+") and oneMinusReflectivity for optimization\n"+
"// purposes, mostly for DX9 SM2.0 level. Most of the math is being done on these (1-x) values, and that saves\n"+
"// a few precious ALU slots.\n"+
"\n"+
"\n"+
"// Main Physically Based BRDF\n"+
"// Derived from Disney work and based on Torrance-Sparrow micro-facet model\n"+
"//\n"+
"//   BRDF = kD / pi + kS * (D * V * F) / 4\n"+
"//   I = BRDF * NdotL\n"+
"//\n"+
"// * NDF (depending on UNITY_BRDF_GGX):\n"+
"//  a) Normalized BlinnPhong\n"+
"//  b) GGX\n"+
"// * Smith for Visiblity term\n"+
"// * Schlick approximation for Fresnel\n"+
"half4 BRDF1_Unity_PBS (half3 diffColor, half3 specColor, half oneMinusReflectivity, half oneMinusRoughness,\n"+
"	half3 normal, half3 viewDir,\n"+
"	UnityLight light, UnityIndirect gi)\n"+
"{\n"+
"	half roughness = 1-oneMinusRoughness;\n"+
"	half3 halfDir = normalize (light.dir + viewDir);\n"+
"\n"+
"	half nl = light.ndotl;\n"+
"	half nh = BlinnTerm (normal, halfDir);\n"+
"	half nv = DotClamped (normal, viewDir);\n"+
"	half lv = DotClamped (light.dir, viewDir);\n"+
"	half lh = DotClamped (light.dir, halfDir);\n"+
"\n"+
"#if UNITY_BRDF_GGX\n"+
"	half V = SmithGGXVisibilityTerm (nl, nv, roughness);\n"+
"	half D = GGXTerm (nh, roughness);\n"+
"#else\n"+
"	half V = SmithBeckmannVisibilityTerm (nl, nv, roughness);\n"+
"	half D = NDFBlinnPhongNormalizedTerm (nh, RoughnessToSpecPower (roughness));\n"+
"#endif\n"+
"\n"+
"	half nlPow5 = Pow5 (1-nl);\n"+
"	half nvPow5 = Pow5 (1-nv);\n"+
"	half Fd90 = 0.5 + 2 * lh * lh * roughness;\n"+
"	half disneyDiffuse = (1 + (Fd90-1) * nlPow5) * (1 + (Fd90-1) * nvPow5);\n"+
"	\n"+
"	// HACK: theoretically we should divide by Pi diffuseTerm and not multiply specularTerm!\n"+
"	// BUT 1) that will make shader look significantly darker than Legacy ones\n"+
"	// and 2) on engine side "+"\""+"Non-important"+"\""+" lights have to be divided by Pi to in cases when they are injected into ambient SH\n"+
"	// NOTE: multiplication by Pi is part of single constant together with 1/4 now\n"+
"\n"+
"	half specularTerm = max(0, (V * D * nl) * unity_LightGammaCorrectionConsts_PIDiv4);// Torrance-Sparrow model, Fresnel is applied later (for optimization reasons)\n"+
"	half diffuseTerm = disneyDiffuse * nl;\n"+
"	\n"+
"	half grazingTerm = saturate(oneMinusRoughness + (1-oneMinusReflectivity));\n"+
"    half3 color =	diffColor * (gi.diffuse + light.color * diffuseTerm)\n"+
"                    + specularTerm * light.color * FresnelTerm (specColor, lh)\n"+
"					+ gi.specular * FresnelLerp (specColor, grazingTerm, nv);\n"+
"\n"+
"	return half4(color, 1);\n"+
"}\n"+
"\n"+
"// Based on Minimalist CookTorrance BRDF\n"+
"// Implementation is slightly different from original derivation: http://www.thetenthplanet.de/archives/255\n"+
"//\n"+
"// * BlinnPhong as NDF\n"+
"// * Modified Kelemen and Szirmay-?Kalos for Visibility term\n"+
"// * Fresnel approximated with 1/LdotH\n"+
"half4 BRDF2_Unity_PBS (half3 diffColor, half3 specColor, half oneMinusReflectivity, half oneMinusRoughness,\n"+
"	half3 normal, half3 viewDir,\n"+
"	UnityLight light, UnityIndirect gi)\n"+
"{\n"+
"	half3 halfDir = normalize (light.dir + viewDir);\n"+
"\n"+
"	half nl = light.ndotl;\n"+
"	half nh = BlinnTerm (normal, halfDir);\n"+
"	half nv = DotClamped (normal, viewDir);\n"+
"	half lh = DotClamped (light.dir, halfDir);\n"+
"\n"+
"	half roughness = 1-oneMinusRoughness;\n"+
"	half specularPower = RoughnessToSpecPower (roughness);\n"+
"	// Modified with approximate Visibility function that takes roughness into account\n"+
"	// Original ((n+1)*N.H^n) / (8*Pi * L.H^3) didn't take into account roughness \n"+
"	// and produced extremely bright specular at grazing angles\n"+
"\n"+
"	// HACK: theoretically we should divide by Pi diffuseTerm and not multiply specularTerm!\n"+
"	// BUT 1) that will make shader look significantly darker than Legacy ones\n"+
"	// and 2) on engine side "+"\""+"Non-important"+"\""+" lights have to be divided by Pi to in cases when they are injected into ambient SH\n"+
"	// NOTE: multiplication by Pi is cancelled with Pi in denominator\n"+
"\n"+
"	half invV = lh * lh * oneMinusRoughness + roughness * roughness; // approx ModifiedKelemenVisibilityTerm(lh, 1-oneMinusRoughness);\n"+
"	half invF = lh;\n"+
"	half specular = ((specularPower + 1) * pow (nh, specularPower)) / (unity_LightGammaCorrectionConsts_8 * invV * invF + 1e-4f); // @TODO: might still need saturate(nl*specular) on Adreno/Mali\n"+
"\n"+
"	half grazingTerm = saturate(oneMinusRoughness + (1-oneMinusReflectivity));\n"+
"    half3 color =	(diffColor + specular * specColor) * light.color * nl\n"+
"    				+ gi.diffuse * diffColor\n"+
"					+ gi.specular * FresnelLerpFast (specColor, grazingTerm, nv);\n"+
"\n"+
"	return half4(color, 1);\n"+
"}\n"+
"\n"+
"// Old school, not microfacet based Modified Normalized Blinn-Phong BRDF\n"+
"// Implementation uses Lookup texture for performance\n"+
"//\n"+
"// * Normalized BlinnPhong in RDF form\n"+
"// * Implicit Visibility term\n"+
"// * No Fresnel term\n"+
"//\n"+
"// TODO: specular is too weak in Linear rendering mode\n"+
"sampler2D unity_NHxRoughness;\n"+
"half4 BRDF3_Unity_PBS (half3 diffColor, half3 specColor, half oneMinusReflectivity, half oneMinusRoughness,\n"+
"	half3 normal, half3 viewDir,\n"+
"	UnityLight light, UnityIndirect gi)\n"+
"{\n"+
"	half LUT_RANGE = 16.0; // must match range in NHxRoughness() function in GeneratedTextures.cpp\n"+
"\n"+
"	half3 reflDir = reflect (viewDir, normal);\n"+
"	half3 halfDir = normalize (light.dir + viewDir);\n"+
"\n"+
"	half nl = light.ndotl;\n"+
"	half nh = BlinnTerm (normal, halfDir);\n"+
"	half nv = DotClamped (normal, viewDir);\n"+
"\n"+
"	// Vectorize Pow4 to save instructions\n"+
"	half2 rlPow4AndFresnelTerm = Pow4 (half2(dot(reflDir, light.dir), 1-nv));  // use R.L instead of N.H to save couple of instructions\n"+
"	half rlPow4 = rlPow4AndFresnelTerm.x; // power exponent must match kHorizontalWarpExp in NHxRoughness() function in GeneratedTextures.cpp\n"+
"	half fresnelTerm = rlPow4AndFresnelTerm.y;\n"+
"\n"+
"#if 1 // Lookup texture to save instructions\n"+
"	half specular = tex2D(unity_NHxRoughness, half2(rlPow4, 1-oneMinusRoughness)).UNITY_ATTEN_CHANNEL * LUT_RANGE;\n"+
"#else\n"+
"	half roughness = 1-oneMinusRoughness;\n"+
"	half n = RoughnessToSpecPower (roughness) * .25;\n"+
"	half specular = (n + 2.0) / (2.0 * UNITY_PI * UNITY_PI) * pow(dot(reflDir, light.dir), n) * nl;// / unity_LightGammaCorrectionConsts_PI;\n"+
"	//half specular = (1.0/(UNITY_PI*roughness*roughness)) * pow(dot(reflDir, light.dir), n) * nl;// / unity_LightGammaCorrectionConsts_PI;\n"+
"#endif\n"+
"	half grazingTerm = saturate(oneMinusRoughness + (1-oneMinusReflectivity));\n"+
"\n"+
"    half3 color =	(diffColor + specular * specColor) * light.color * nl\n"+
"    				+ gi.diffuse * diffColor\n"+
"					+ gi.specular * lerp (specColor, grazingTerm, fresnelTerm);\n"+
"\n"+
"	return half4(color, 1);\n"+
"}\n"+
"\n"+
"\n"+
"#endif // UNITY_STANDARD_BRDF_INCLUDED\n"+
"\n"+
"#ifndef UNITY_STANDARD_UTILS_INCLUDED\n"+
"#define UNITY_STANDARD_UTILS_INCLUDED\n"+
"\n"+
"#include "+"\""+"UnityCG.cginc"+"\""+"\n"+
"\n"+
"// Helper functions, maybe move into UnityCG.cginc\n"+
"\n"+
"half SpecularStrength(half3 specular)\n"+
"{\n"+
"	#if (SHADER_TARGET < 30)\n"+
"		// SM2.0: instruction count limitation\n"+
"		// SM2.0: simplified SpecularStrength\n"+
"		return specular.r; // Red channel - because most metals are either monocrhome or with redish/yellowish tint\n"+
"	#else\n"+
"		return max (max (specular.r, specular.g), specular.b);\n"+
"	#endif\n"+
"}\n"+
"\n"+
"// Diffuse/Spec Energy conservation\n"+
"half3 EnergyConservationBetweenDiffuseAndSpecular (half3 albedo, half3 specColor, out half oneMinusReflectivity)\n"+
"{\n"+
"	oneMinusReflectivity = 1 - SpecularStrength(specColor);\n"+
"	#if !UNITY_CONSERVE_ENERGY\n"+
"		return albedo;\n"+
"	#elif UNITY_CONSERVE_ENERGY_MONOCHROME\n"+
"		return albedo * oneMinusReflectivity;\n"+
"	#else\n"+
"		return albedo * (half3(1,1,1) - specColor);\n"+
"	#endif\n"+
"}\n"+
"\n"+
"half3 DiffuseAndSpecularFromMetallic (half3 albedo, half metallic, out half3 specColor, out half oneMinusReflectivity)\n"+
"{\n"+
"	specColor = lerp (unity_ColorSpaceDielectricSpec.rgb, albedo, metallic);\n"+
"	// We'll need oneMinusReflectivity, so\n"+
"	//   1-reflectivity = 1-lerp(dielectricSpec, 1, metallic) = lerp(1-dielectricSpec, 0, metallic)\n"+
"	// store (1-dielectricSpec) in unity_ColorSpaceDielectricSpec.a, then\n"+
"	//	 1-reflectivity = lerp(alpha, 0, metallic) = alpha + metallic*(0 - alpha) = \n"+
"	//                  = alpha - metallic * alpha\n"+
"	half oneMinusDielectricSpec = unity_ColorSpaceDielectricSpec.a;\n"+
"	oneMinusReflectivity = oneMinusDielectricSpec - metallic * oneMinusDielectricSpec;\n"+
"	return albedo * oneMinusReflectivity;\n"+
"}\n"+
"\n"+
"half3 PreMultiplyAlpha (half3 diffColor, half alpha, half oneMinusReflectivity, out half outModifiedAlpha)\n"+
"{\n"+
"	#if defined(_ALPHAPREMULTIPLY_ON)\n"+
"		// NOTE: shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)\n"+
"\n"+
"		// Transparency 'removes' from Diffuse component\n"+
" 		diffColor *= alpha;\n"+
" 		\n"+
" 		#if (SHADER_TARGET < 30)\n"+
" 			// SM2.0: instruction count limitation\n"+
" 			// Instead will sacrifice part of physically based transparency where amount Reflectivity is affecting Transparency\n"+
" 			// SM2.0: uses unmodified alpha\n"+
" 			outModifiedAlpha = alpha;\n"+
" 		#else\n"+
"	 		// Reflectivity 'removes' from the rest of components, including Transparency\n"+
"	 		// outAlpha = 1-(1-alpha)*(1-reflectivity) = 1-(oneMinusReflectivity - alpha*oneMinusReflectivity) =\n"+
"	 		//          = 1-oneMinusReflectivity + alpha*oneMinusReflectivity\n"+
"	 		outModifiedAlpha = 1-oneMinusReflectivity + alpha*oneMinusReflectivity;\n"+
" 		#endif\n"+
" 	#else\n"+
" 		outModifiedAlpha = alpha;\n"+
" 	#endif\n"+
" 	return diffColor;\n"+
"}\n"+
"\n"+
"// Same as ParallaxOffset in Unity CG, except:\n"+
"//  *) precision - half instead of float\n"+
"half2 ParallaxOffset1Step (half h, half height, half3 viewDir)\n"+
"{\n"+
"	h = h * height - height/2.0;\n"+
"	half3 v = normalize(viewDir);\n"+
"	v.z += 0.42;\n"+
"	return h * (v.xy / v.z);\n"+
"}\n"+
"\n"+
"half LerpOneTo(half b, half t)\n"+
"{\n"+
"	half oneMinusT = 1 - t;\n"+
"	return oneMinusT + b * t;\n"+
"}\n"+
"\n"+
"half3 LerpWhiteTo(half3 b, half t)\n"+
"{\n"+
"	half oneMinusT = 1 - t;\n"+
"	return half3(oneMinusT, oneMinusT, oneMinusT) + b * t;\n"+
"}\n"+
"\n"+
"half3 UnpackScaleNormal(half4 packednormal, half bumpScale)\n"+
"{\n"+
"	#if defined(UNITY_NO_DXT5nm)\n"+
"		return packednormal.xyz * 2 - 1;\n"+
"	#else\n"+
"		half3 normal;\n"+
"		normal.xy = (packednormal.wy * 2 - 1);\n"+
"		#if (SHADER_TARGET >= 30)\n"+
"			// SM2.0: instruction count limitation\n"+
"			// SM2.0: normal scaler is not supported\n"+
"			normal.xy *= bumpScale;\n"+
"		#endif\n"+
"		normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));\n"+
"		return normal;\n"+
"	#endif\n"+
"}		\n"+
"\n"+
"half3 BlendNormals(half3 n1, half3 n2)\n"+
"{\n"+
"	return normalize(half3(n1.xy + n2.xy, n1.z*n2.z));\n"+
"}\n"+
"\n"+
"half3x3 CreateTangentToWorldPerVertex(half3 normal, half3 tangent, half3 flip)\n"+
"{\n"+
"	half3 binormal = cross(normal, tangent) * flip;\n"+
"	return half3x3(tangent, binormal, normal);\n"+
"}\n"+
"\n"+
"//-------------------------------------------------------------------------------------\n"+
"half3 BoxProjectedCubemapDirection (half3 worldNormal, float3 worldPos, float4 cubemapCenter, float4 boxMin, float4 boxMax)\n"+
"{\n"+
"	// Do we have a valid reflection probe?\n"+
"	\n"+
"	if (cubemapCenter.w > 0.0)\n"+
"	{\n"+
"		half3 nrdir = normalize(worldNormal);\n"+
"\n"+
"		#if 1				\n"+
"			half3 rbmax = (boxMax.xyz - worldPos) / nrdir;\n"+
"			half3 rbmin = (boxMin.xyz - worldPos) / nrdir;\n"+
"\n"+
"			half3 rbminmax = (nrdir > 0.0f) ? rbmax : rbmin;\n"+
"\n"+
"		#else // Optimized version\n"+
"			half3 rbmax = (boxMax.xyz - worldPos);\n"+
"			half3 rbmin = (boxMin.xyz - worldPos);\n"+
"\n"+
"			half3 select = step (half3(0,0,0), nrdir);\n"+
"			half3 rbminmax = lerp (rbmax, rbmin, select);\n"+
"			rbminmax /= nrdir;\n"+
"		#endif\n"+
"\n"+
"		half fa = min(min(rbminmax.x, rbminmax.y), rbminmax.z);\n"+
"\n"+
"		float3 aabbCenter = (boxMax.xyz + boxMin.xyz) * 0.5;\n"+
"		float3 offset = aabbCenter - cubemapCenter.xyz;\n"+
"		float3 posonbox = offset + worldPos + nrdir * fa;\n"+
"\n"+
"		worldNormal = posonbox - aabbCenter;\n"+
"	}\n"+
"	return worldNormal;\n"+
"}\n"+
"\n"+
"\n"+
"//-------------------------------------------------------------------------------------\n"+
"// Derivative maps\n"+
"// http://www.rorydriscoll.com/2012/01/11/derivative-maps/\n"+
"// For future use.\n"+
"\n"+
"// Project the surface gradient (dhdx, dhdy) onto the surface (n, dpdx, dpdy)\n"+
"half3 CalculateSurfaceGradient(half3 n, half3 dpdx, half3 dpdy, half dhdx, half dhdy)\n"+
"{\n"+
"	half3 r1 = cross(dpdy, n);\n"+
"	half3 r2 = cross(n, dpdx);\n"+
"	return (r1 * dhdx + r2 * dhdy) / dot(dpdx, r1);\n"+
"}\n"+
"\n"+
"// Move the normal away from the surface normal in the opposite surface gradient direction\n"+
"half3 PerturbNormal(half3 n, half3 dpdx, half3 dpdy, half dhdx, half dhdy)\n"+
"{\n"+
"	//TODO: normalize seems to be necessary when scales do go beyond the 2...-2 range, should we limit that?\n"+
"	//how expensive is a normalize? Anything cheaper for this case?\n"+
"	return normalize(n - CalculateSurfaceGradient(n, dpdx, dpdy, dhdx, dhdy));\n"+
"}\n"+
"\n"+
"// Calculate the surface normal using the uv-space gradient (dhdu, dhdv)\n"+
"half3 CalculateSurfaceNormal(half3 position, half3 normal, half2 gradient, half2 uv)\n"+
"{\n"+
"	half3 dpdx = ddx(position);\n"+
"	half3 dpdy = ddy(position);\n"+
"\n"+
"	half dhdx = dot(gradient, ddx(uv));\n"+
"	half dhdy = dot(gradient, ddy(uv));\n"+
"\n"+
"	return PerturbNormal(normal, dpdx, dpdy, dhdx, dhdy);\n"+
"}\n"+
"\n"+
"\n"+
"#endif // UNITY_STANDARD_UTILS_INCLUDED\n"+
"\n"+
"\n"+
"half3 DecodeDirectionalSpecularLightmap (half3 color, fixed4 dirTex, half3 normalWorld, bool isRealtimeLightmap, fixed4 realtimeNormalTex, out UnityLight o_light)\n"+
"{\n"+
"	o_light.color = color;\n"+
"	o_light.dir = dirTex.xyz * 2 - 1;\n"+
"\n"+
"	// The length of the direction vector is the light's "+"\""+"directionality"+"\""+", i.e. 1 for all light coming from this direction,\n"+
"	// lower values for more spread out, ambient light.\n"+
"	half directionality = length(o_light.dir);\n"+
"	o_light.dir /= directionality;\n"+
"\n"+
"	#ifdef DYNAMICLIGHTMAP_ON\n"+
"	if (isRealtimeLightmap)\n"+
"	{\n"+
"		// Realtime directional lightmaps' intensity needs to be divided by N.L\n"+
"		// to get the incoming light intensity. Baked directional lightmaps are already\n"+
"		// output like that (including the max() to prevent div by zero).\n"+
"		half3 realtimeNormal = realtimeNormalTex.zyx * 2 - 1;\n"+
"		o_light.color /= max(0.125, dot(realtimeNormal, o_light.dir));\n"+
"	}\n"+
"	#endif\n"+
"\n"+
"	o_light.ndotl = LambertTerm(normalWorld, o_light.dir);\n"+
"\n"+
"	// Split light into the directional and ambient parts, according to the directionality factor.\n"+
"	half3 ambient = o_light.color * (1 - directionality);\n"+
"	o_light.color = o_light.color * directionality;\n"+
"\n"+
"	// Technically this is incorrect, but helps hide jagged light edge at the object silhouettes and\n"+
"	// makes normalmaps show up.\n"+
"	ambient *= o_light.ndotl;\n"+
"	return ambient;\n"+
"}\n"+
"\n"+
"half3 MixLightmapWithRealtimeAttenuation (half3 lightmapContribution, half attenuation, fixed4 bakedColorTex)\n"+
"{\n"+
"	// Let's try to make realtime shadows work on a surface, which already contains\n"+
"	// baked lighting and shadowing from the current light.\n"+
"	// Generally do min(lightmap,shadow), with "+"\""+"shadow"+"\""+" taking overall lightmap tint into account.\n"+
"	half3 shadowLightmapColor = bakedColorTex.rgb * attenuation;\n"+
"	half3 darkerColor = min(lightmapContribution, shadowLightmapColor);\n"+
"\n"+
"	// However this can darken overbright lightmaps, since "+"\""+"shadow color"+"\""+" will\n"+
"	// never be overbright. So take a max of that color with attenuated lightmap color.\n"+
"	return max(darkerColor, lightmapContribution * attenuation);\n"+
"}\n"+
"\n"+
"void ResetUnityLight(out UnityLight outLight)\n"+
"{\n"+
"	outLight.color = 0;\n"+
"	outLight.dir = 0;\n"+
"	outLight.ndotl = 0;\n"+
"}\n"+
"\n"+
"void ResetUnityGI(out UnityGI outGI)\n"+
"{\n"+
"	ResetUnityLight(outGI.light);\n"+
"	#ifdef DIRLIGHTMAP_SEPARATE\n"+
"		#ifdef LIGHTMAP_ON\n"+
"			ResetUnityLight(outGI.light2);\n"+
"		#endif\n"+
"		#ifdef DYNAMICLIGHTMAP_ON\n"+
"			ResetUnityLight(outGI.light3);\n"+
"		#endif\n"+
"	#endif\n"+
"	outGI.indirect.diffuse = 0;\n"+
"	outGI.indirect.specular = 0;\n"+
"}\n"+
"\n"+
"/*UnityGI UnityGlobalIllumination (UnityGIInput data, half occlusion, half oneMinusRoughness, half3 normalWorld, bool reflections)\n"+
"{\n"+
"	UnityGI o_gi;\n"+
"	UNITY_INITIALIZE_OUTPUT(UnityGI, o_gi);\n"+
"\n"+
"	// Explicitly reset all members of UnityGI\n"+
"	ResetUnityGI(o_gi);\n"+
"\n"+
"	#if UNITY_SHOULD_SAMPLE_SH\n"+
"		#if UNITY_SAMPLE_FULL_SH_PER_PIXEL\n"+
"			half3 sh = ShadeSH9(half4(normalWorld, 1.0));\n"+
"		#elif (SHADER_TARGET >= 30)\n"+
"			half3 sh = data.ambient + ShadeSH12Order(half4(normalWorld, 1.0));\n"+
"		#else\n"+
"			half3 sh = data.ambient;\n"+
"		#endif\n"+
"	\n"+
"		o_gi.indirect.diffuse += sh;\n"+
"	#endif\n"+
"\n"+
"	#if !defined(LIGHTMAP_ON)\n"+
"		o_gi.light = data.light;\n"+
"		o_gi.light.color *= data.atten;\n"+
"\n"+
"	#else\n"+
"		// Baked lightmaps\n"+
"		fixed4 bakedColorTex = UNITY_SAMPLE_TEX2D(unity_Lightmap, data.lightmapUV.xy); \n"+
"		half3 bakedColor = DecodeLightmap(bakedColorTex);\n"+
"		\n"+
"		#ifdef DIRLIGHTMAP_OFF\n"+
"			o_gi.indirect.diffuse = bakedColor;\n"+
"\n"+
"			#ifdef SHADOWS_SCREEN\n"+
"				o_gi.indirect.diffuse = MixLightmapWithRealtimeAttenuation (o_gi.indirect.diffuse, data.atten, bakedColorTex);\n"+
"			#endif // SHADOWS_SCREEN\n"+
"\n"+
"		#elif DIRLIGHTMAP_COMBINED\n"+
"			fixed4 bakedDirTex = UNITY_SAMPLE_TEX2D_SAMPLER (unity_LightmapInd, unity_Lightmap, data.lightmapUV.xy);\n"+
"			o_gi.indirect.diffuse = DecodeDirectionalLightmap (bakedColor, bakedDirTex, normalWorld);\n"+
"\n"+
"			#ifdef SHADOWS_SCREEN\n"+
"				o_gi.indirect.diffuse = MixLightmapWithRealtimeAttenuation (o_gi.indirect.diffuse, data.atten, bakedColorTex);\n"+
"			#endif // SHADOWS_SCREEN\n"+
"\n"+
"		#elif DIRLIGHTMAP_SEPARATE\n"+
"			// Left halves of both intensity and direction lightmaps store direct light; right halves - indirect.\n"+
"\n"+
"			// Direct\n"+
"			fixed4 bakedDirTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd, unity_Lightmap, data.lightmapUV.xy);\n"+
"			o_gi.indirect.diffuse += DecodeDirectionalSpecularLightmap (bakedColor, bakedDirTex, normalWorld, false, 0, o_gi.light);\n"+
"\n"+
"			// Indirect\n"+
"			half2 uvIndirect = data.lightmapUV.xy + half2(0.5, 0);\n"+
"			bakedColor = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, uvIndirect));\n"+
"			bakedDirTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd, unity_Lightmap, uvIndirect);\n"+
"			o_gi.indirect.diffuse += DecodeDirectionalSpecularLightmap (bakedColor, bakedDirTex, normalWorld, false, 0, o_gi.light2);\n"+
"		#endif\n"+
"	#endif\n"+
"	\n"+
"	#ifdef DYNAMICLIGHTMAP_ON\n"+
"		// Dynamic lightmaps\n"+
"		fixed4 realtimeColorTex = UNITY_SAMPLE_TEX2D(unity_DynamicLightmap, data.lightmapUV.zw);\n"+
"		half3 realtimeColor = DecodeRealtimeLightmap (realtimeColorTex);\n"+
"\n"+
"		#ifdef DIRLIGHTMAP_OFF\n"+
"			o_gi.indirect.diffuse += realtimeColor;\n"+
"\n"+
"		#elif DIRLIGHTMAP_COMBINED\n"+
"			half4 realtimeDirTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_DynamicDirectionality, unity_DynamicLightmap, data.lightmapUV.zw);\n"+
"			o_gi.indirect.diffuse += DecodeDirectionalLightmap (realtimeColor, realtimeDirTex, normalWorld);\n"+
"\n"+
"		#elif DIRLIGHTMAP_SEPARATE\n"+
"			half4 realtimeDirTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_DynamicDirectionality, unity_DynamicLightmap, data.lightmapUV.zw);\n"+
"			half4 realtimeNormalTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_DynamicNormal, unity_DynamicLightmap, data.lightmapUV.zw);\n"+
"			o_gi.indirect.diffuse += DecodeDirectionalSpecularLightmap (realtimeColor, realtimeDirTex, normalWorld, true, realtimeNormalTex, o_gi.light3);\n"+
"		#endif\n"+
"	#endif\n"+
"	o_gi.indirect.diffuse *= occlusion;\n"+
"\n"+
"	if (reflections)\n"+
"	{\n"+
"		half3 worldNormal = reflect(-data.worldViewDir, normalWorld);\n"+
"\n"+
"		#if UNITY_SPECCUBE_BOX_PROJECTION		\n"+
"			half3 worldNormal0 = BoxProjectedCubemapDirection (worldNormal, data.worldPos, data.probePosition[0], data.boxMin[0], data.boxMax[0]);\n"+
"		#else\n"+
"			half3 worldNormal0 = worldNormal;\n"+
"		#endif\n"+
"\n"+
"		half3 env0 = Unity_GlossyEnvironment (UNITY_PASS_TEXCUBE(unity_SpecCube0), data.probeHDR[0], worldNormal0, 1-oneMinusRoughness);\n"+
"		#if UNITY_SPECCUBE_BLENDING\n"+
"			const float kBlendFactor = 0.99999;\n"+
"			float blendLerp = data.boxMin[0].w;\n"+
"			UNITY_BRANCH\n"+
"			if (blendLerp < kBlendFactor)\n"+
"			{\n"+
"				#if UNITY_SPECCUBE_BOX_PROJECTION\n"+
"					half3 worldNormal1 = BoxProjectedCubemapDirection (worldNormal, data.worldPos, data.probePosition[1], data.boxMin[1], data.boxMax[1]);\n"+
"				#else\n"+
"					half3 worldNormal1 = worldNormal;\n"+
"				#endif\n"+
"\n"+
"				half3 env1 = Unity_GlossyEnvironment (UNITY_PASS_TEXCUBE(unity_SpecCube1), data.probeHDR[1], worldNormal1, 1-oneMinusRoughness);\n"+
"				o_gi.indirect.specular = lerp(env1, env0, blendLerp);\n"+
"			}\n"+
"			else\n"+
"			{\n"+
"				o_gi.indirect.specular = env0;\n"+
"			}\n"+
"		#else\n"+
"			o_gi.indirect.specular = env0;\n"+
"		#endif\n"+
"	}\n"+
"	o_gi.indirect.specular *= occlusion;\n"+
"\n"+
"	return o_gi;\n"+
"}*/\n"+
"\n"+
"/*UnityGI UnityGlobalIllumination (UnityGIInput data, half occlusion, half oneMinusRoughness, half3 normalWorld)\n"+
"{\n"+
"	return UnityGlobalIllumination (data, occlusion, oneMinusRoughness, normalWorld, true);	\n"+
"}*/\n"+
"\n"+
"#endif\n"+
"\n"+
"//-------------------------------------------------------------------------------------\n"+
"// Default BRDF to use:\n"+
"#if !defined (UNITY_BRDF_PBS) // allow to explicitly override BRDF in custom shader\n"+
"	#if (SHADER_TARGET < 30) || defined(SHADER_API_PSP2)\n"+
"		// Fallback to low fidelity one for pre-SM3.0\n"+
"		#define UNITY_BRDF_PBS BRDF3_Unity_PBS\n"+
"	#elif defined(SHADER_API_MOBILE)\n"+
"		// Somewhat simplified for mobile\n"+
"		#define UNITY_BRDF_PBS BRDF2_Unity_PBS\n"+
"	#else\n"+
"		// Full quality for SM3+ PC / consoles\n"+
"		#define UNITY_BRDF_PBS BRDF1_Unity_PBS\n"+
"	#endif\n"+
"#endif\n"+
"\n"+
"//-------------------------------------------------------------------------------------\n"+
"// BRDF for lights extracted from *indirect* directional lightmaps (baked and realtime).\n"+
"// Baked directional lightmap with *direct* light uses UNITY_BRDF_PBS.\n"+
"// For better quality change to BRDF1_Unity_PBS.\n"+
"// No directional lightmaps in SM2.0.\n"+
"\n"+
"#if !defined(UNITY_BRDF_PBS_LIGHTMAP_INDIRECT)\n"+
"	#define UNITY_BRDF_PBS_LIGHTMAP_INDIRECT BRDF2_Unity_PBS\n"+
"#endif\n"+
"#if !defined (UNITY_BRDF_GI)\n"+
"	#define UNITY_BRDF_GI BRDF_Unity_Indirect\n"+
"#endif\n"+
"\n"+
"//-------------------------------------------------------------------------------------\n"+
"\n"+
"\n"+
"half3 BRDF_Unity_Indirect (half3 baseColor, half3 specColor, half oneMinusReflectivity, half oneMinusRoughness, half3 normal, half3 viewDir, half occlusion, UnityGI gi)\n"+
"{\n"+
"	half3 c = 0;\n"+
"	#if defined(DIRLIGHTMAP_SEPARATE)\n"+
"		gi.indirect.diffuse = 0;\n"+
"		gi.indirect.specular = 0;\n"+
"\n"+
"		#ifdef LIGHTMAP_ON\n"+
"			c += UNITY_BRDF_PBS_LIGHTMAP_INDIRECT (baseColor, specColor, oneMinusReflectivity, oneMinusRoughness, normal, viewDir, gi.light2, gi.indirect).rgb * occlusion;\n"+
"		#endif\n"+
"		#ifdef DYNAMICLIGHTMAP_ON\n"+
"			c += UNITY_BRDF_PBS_LIGHTMAP_INDIRECT (baseColor, specColor, oneMinusReflectivity, oneMinusRoughness, normal, viewDir, gi.light3, gi.indirect).rgb * occlusion;\n"+
"		#endif\n"+
"	#endif\n"+
"	return c;\n"+
"}\n"+
"\n"+
"//-------------------------------------------------------------------------------------\n"+
"\n"+
"\n"+
"\n"+
"// Surface shader output structure to be used with physically\n"+
"// based shading model.\n"+
"\n"+
"//-------------------------------------------------------------------------------------\n"+
"// Metallic workflow\n"+
"\n"+
"struct SurfaceOutputStandard\n"+
"{\n"+
"	fixed3 Albedo;		// base (diffuse or specular) color\n"+
"	fixed3 Normal;		// tangent space normal, if written\n"+
"	half3 Emission;\n"+
"	half Metallic;		// 0=non-metal, 1=metal\n"+
"	half Smoothness;	// 0=rough, 1=smooth\n"+
"	half LightSmoothness;	// 0=rough, 1=smooth\n"+
"	half Occlusion;		// occlusion (default 1)\n"+
"	fixed Alpha;		// alpha for transparencies\n"+
"};\n"+
"\n"+
"half4 LightingStandard (SurfaceOutputStandard s, half3 viewDir, UnityGI gi)\n"+
"{\n"+
"	s.Normal = normalize(s.Normal);\n"+
"\n"+
"	half oneMinusReflectivity;\n"+
"	half3 specColor;\n"+
"	s.Albedo = DiffuseAndSpecularFromMetallic (s.Albedo, s.Metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);\n"+
"\n"+
"	// shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)\n"+
"	// this is necessary to handle transparency in physically correct way - only diffuse component gets affected by alpha\n"+
"	half outputAlpha;\n"+
"	s.Albedo = PreMultiplyAlpha (s.Albedo, s.Alpha, oneMinusReflectivity, /*out*/ outputAlpha);\n"+
"\n"+
"	half4 c = UNITY_BRDF_PBS (s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, gi.light, gi.indirect);\n"+
"	c.rgb += UNITY_BRDF_GI (s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, s.Occlusion, gi);\n"+
"	c.a = outputAlpha;\n"+
"	return c;\n"+
"}\n"+
"\n"+
"half4 LightingStandard_Deferred (SurfaceOutputStandard s, half3 viewDir, UnityGI gi, out half4 outDiffuseOcclusion, out half4 outSpecSmoothness, out half4 outNormal)\n"+
"{\n"+
"	half oneMinusReflectivity;\n"+
"	half3 specColor;\n"+
"	s.Albedo = DiffuseAndSpecularFromMetallic (s.Albedo, s.Metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);\n"+
"\n"+
"	half4 c = UNITY_BRDF_PBS (s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, gi.light, gi.indirect);\n"+
"	c.rgb += UNITY_BRDF_GI (s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, s.Occlusion, gi);\n"+
"\n"+
"	outDiffuseOcclusion = half4(s.Albedo, s.Occlusion);\n"+
"	outSpecSmoothness = half4(specColor, s.Smoothness);\n"+
"	outNormal = half4(s.Normal * 0.5 + 0.5, 1);\n"+
"	half4 emission = half4(s.Emission + c.rgb, 1);\n"+
"	return emission;\n"+
"}\n"+
"\n"+
"/*void LightingStandard_GI (\n"+
"	SurfaceOutputStandard s,\n"+
"	UnityGIInput data,\n"+
"	inout UnityGI gi)\n"+
"{\n"+
"#if UNITY_VERSION >= 520\n"+
"UNITY_GI(gi, s, data);\n"+
"#else\n"+
"	gi = UnityGlobalIllumination (data, s.Occlusion, s.Smoothness, s.Normal);\n"+
"#endif\n"+
"}*/\n"+
"\n"+
"//-------------------------------------------------------------------------------------\n"+
"// Specular workflow\n"+
"\n"+
"struct SurfaceOutputStandardSpecular\n"+
"{\n"+
"	fixed3 Albedo;		// diffuse color\n"+
"	fixed3 Specular;	// specular color\n"+
"	fixed3 Normal;		// tangent space normal, if written\n"+
"	half3 Emission;\n"+
"	half Smoothness;	// 0=rough, 1=smooth\n"+
"	half LightSmoothness;	// 0=rough, 1=smooth\n"+
"	half Occlusion;		// occlusion (default 1)\n"+
"	fixed Alpha;		// alpha for transparencies\n"+
"};\n"+
"\n"+
"half4 LightingStandardSpecular (SurfaceOutputStandardSpecular s, half3 viewDir, UnityGI gi)\n"+
"{\n"+
"	s.Normal = normalize(s.Normal);\n"+
"\n"+
"	// energy conservation\n"+
"	half oneMinusReflectivity;\n"+
"	s.Albedo = EnergyConservationBetweenDiffuseAndSpecular (s.Albedo, s.Specular, /*out*/ oneMinusReflectivity);\n"+
"\n"+
"	// shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)\n"+
"	// this is necessary to handle transparency in physically correct way - only diffuse component gets affected by alpha\n"+
"	half outputAlpha;\n"+
"	s.Albedo = PreMultiplyAlpha (s.Albedo, s.Alpha, oneMinusReflectivity, /*out*/ outputAlpha);\n"+
"\n"+
"	half4 c = UNITY_BRDF_PBS (s.Albedo, s.Specular, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, gi.light, gi.indirect);\n"+
"	c.rgb += UNITY_BRDF_GI (s.Albedo, s.Specular, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, s.Occlusion, gi);\n"+
"	c.a = outputAlpha;\n"+
"	return c;\n"+
"}\n"+
"\n"+
"half4 LightingStandardSpecular_Deferred (SurfaceOutputStandardSpecular s, half3 viewDir, UnityGI gi, out half4 outDiffuseOcclusion, out half4 outSpecSmoothness, out half4 outNormal)\n"+
"{\n"+
"	// energy conservation\n"+
"	half oneMinusReflectivity;\n"+
"	s.Albedo = EnergyConservationBetweenDiffuseAndSpecular (s.Albedo, s.Specular, /*out*/ oneMinusReflectivity);\n"+
"\n"+
"	half4 c = UNITY_BRDF_PBS (s.Albedo, s.Specular, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, gi.light, gi.indirect);\n"+
"	c.rgb += UNITY_BRDF_GI (s.Albedo, s.Specular, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, s.Occlusion, gi);\n"+
"\n"+
"	outDiffuseOcclusion = half4(s.Albedo, s.Occlusion);\n"+
"	outSpecSmoothness = half4(s.Specular, s.Smoothness);\n"+
"	outNormal = half4(s.Normal * 0.5 + 0.5, 1);\n"+
"	half4 emission = half4(s.Emission + c.rgb, 1);\n"+
"	return emission;\n"+
"}\n"+
"\n"+
"/*void LightingStandardSpecular_GI (\n"+
"	SurfaceOutputStandardSpecular s,\n"+
"	UnityGIInput data,\n"+
"	inout UnityGI gi)\n"+
"{\n"+
"	gi = UnityGlobalIllumination (data, s.Occlusion, s.Smoothness, s.Normal);\n"+
"}*/\n"+
"\n"+
"#endif // UNITY_PBS_LIGHTING_INCLUDED			\n"+
"			\n");
		}
		#endif
		
		foreach (ShaderPlugin SP in SD.UsedPlugins){
			shaderCode.Append("\n\n");
			shaderCode.Append(SP.Function);
			shaderCode.Append("\n\n");
		}
		
			HashSet<string> usedEffects = new HashSet<string>();
			foreach (ShaderLayer SL in ShaderUtil.GetAllLayers()){
				foreach(ShaderEffect SE in SL.LayerEffects){
					if (SE.Visible){
						usedEffects.Add(SE.TypeS);
					}
				}
			}
			foreach(string e in usedEffects){
				if (e=="Hasn't Activated Correctly"){
					Debug.LogWarning("Hi there! Sorry, some sort of internal error happened, I'd highly reccomend reloading Shader Sandwich :)");
					continue;
				}
				ShaderEffect NewEffect = ShaderSandwich.ShaderLayerEffectInstances[e] as ShaderEffect;		
				shaderCode.Append(NewEffect.Function+"\n");
			}
		/*foreach(KeyValuePair<string,ShaderPlugin> Ty2 in ShaderSandwich.ShaderLayerEffectInstances){
			string Ty = Ty2.Key;
			bool IsUsed = false;
			foreach (ShaderLayer SL in ShaderUtil.GetAllLayers()){
				foreach(ShaderEffect SE in SL.LayerEffects)
				{
					if (Ty==SE.TypeS&&SE.Visible)
					IsUsed = true;
				}
			}
			if (IsUsed){
				//ShaderEffect NewEffect = ShaderEffect.CreateInstance<ShaderEffect>();
				//NewEffect.ShaderEffectIn(Ty);	
				ShaderEffect NewEffect = Ty2.Value as ShaderEffect;		
				shaderCode.Append(NewEffect.Function+"\n");
			}
		}*/
		
		return shaderCode.ToString();
	}
	
	public Dictionary<string,ShaderVar> GetSaveLoadDictLegacy(){
		ShaderVar TechLOD = new ShaderVar("Tech Lod",200);
		ShaderVar TechCull = new ShaderVar("Cull",new string[]{"All","Front","Back"},new string[]{"","",""},new string[]{"Off","Back","Front"}); 
		ShaderVar TechZWrite = new ShaderVar("BaseZWrite",new string[]{"Auto","On","Off"},new string[]{"","",""},new string[]{"On","On","Off"}); 
		ShaderVar TechZTest = new ShaderVar("Ztest",new string[]{"Less","Greater","LEqual","GEqual","Equal","NotEqual","Always"},new string[]{"","","","","","",""},new string[]{"Less","Greater","LEqual","GEqual","Equal","NotEqual","Always"}); 

		ShaderVar TechShaderTarget = new ShaderVar("Tech Shader Target",3f);
		
		ShaderVar MiscVertexRecalculation = new ShaderVar("Vertex Recalculation",false);
		ShaderVar MiscFog = new ShaderVar("Use Fog",true);
		ShaderVar MiscAmbient = new ShaderVar("Use Ambient",true);
		ShaderVar MiscVertexLights = new ShaderVar("Use Vertex Lights",true);
		ShaderVar MiscLightmap = new ShaderVar("Use Lightmaps",true);
		ShaderVar MiscDeferred = new ShaderVar("Deferred",true);
		ShaderVar MiscForward = new ShaderVar("Forward",true);
		ShaderVar MiscForwardAdd = new ShaderVar("Forward Add",true);
		ShaderVar MiscShadowCaster = new ShaderVar("Shadow Caster",true);
		ShaderVar MiscShadows = new ShaderVar("Shadows",true);
		ShaderVar MiscFullShadows = new ShaderVar("Use All Shadows",true);
		ShaderVar MiscInterpolateView = new ShaderVar("Interpolate View",false);
		ShaderVar MiscRecalculateAfterVertex = new ShaderVar("Recalculate After Vertex",true);
		ShaderVar MiscVertexToastie = new ShaderVar("Vertex Toastie Support Type",true);
		ShaderVar MiscVertexToastieOptimized = new ShaderVar("Vertex Toastie Optimized",false);
		ShaderVar IndirectControl = new ShaderVar("Indirect Control",new string[]{"Manual","Auto","Auto Optimized"});
		ShaderVar IndirectAmbientOcclusionOn = new ShaderVar("Indirect Ambient Occlusion On",true);
		ShaderVar IndirectAmbientOcclusionControl = new ShaderVar("Indirect Ambient Occlusion Control",new string[]{"Auto","Manual"});
		ShaderVar IndirectAmbientOcclusionRealtime = new ShaderVar("Indirect Ambient Occlusion Realtime",new string[]{"Baked","Realtime"});
		ShaderVar IndirectAmbientOcclusionOptimizedType = new ShaderVar("Indirect Ambient Occlusion Optimized Type",new string[]{"Optimized","Really Optimized"});
		ShaderVar IndirectAmbientOcclusionOptimized = new ShaderVar("Indirect Ambient Occlusion Optimized",false);
		ShaderVar IndirectAmbientOcclusionStrength = new ShaderVar("Indirect Ambient Occlusion Strength",1f);
		ShaderVar IndirectAmbientOcclusionPushback = new ShaderVar("Indirect Ambient Occlusion Pushback",1f);
		ShaderVar IndirectAmbientOcclusionDisplayType = new ShaderVar("Indirect Ambient Occlusion Display Type",new string[]{"Ambient","Ambient+Diffuse","AO Only"});
		ShaderVar IndirectSubsurfaceScatteringOn = new ShaderVar("Indirect Subsurface Scattering On",false);
		ShaderVar IndirectSubsurfaceScattering = new ShaderVar("Indirect Subsurface Scattering Enabled",new string[]{"Baked","Realtime"});
		ShaderVar IndirectSubsurfaceScatteringMix = new ShaderVar("SSS Mix",0.5f);
		ShaderVar IndirectSubsurfaceScatteringUseThickness = new ShaderVar("SSS Use Thickness",true);
		ShaderVar IndirectSubsurfaceScatteringAmbient = new ShaderVar("SSS Constant Scatter",0.1f);
		ShaderVar IndirectSubsurfaceScatteringDensity = new ShaderVar("SSS Density",40f);
		ShaderVar IndirectSubsurfaceScatteringDistortion = new ShaderVar("SSS Fresnel Distortion",0.25f);
		ShaderVar IndirectSubsurfaceScatteringScale = new ShaderVar("SSS Fresnel Brightness",1f);
		ShaderVar IndirectSubsurfaceScatteringThinness = new ShaderVar("SSS Fresnel Thinness",1f);
		ShaderVar IndirectSubsurfaceScatteringColor = new ShaderVar("SSS Color",new ShaderColor(1f,1f,1f,1f));
		ShaderVar DiffuseOn = new ShaderVar("Diffuse On",true);
		ShaderVar DiffuseLightingType = new ShaderVar("Lighting Type",new string[] {"PBR Standard","Standard", "Microfaceted", "Translucent","Custom"},new string[]{"ImagePreviews/DiffusePBRStandard.png","ImagePreviews/DiffuseStandard.png","ImagePreviews/DiffuseRough.png","ImagePreviews/DiffuseTranslucent.png",""},"",new string[] {"PBR Standard - A physically based version of the Standard option.","Smooth/Lambert - A good approximation of hard, but smooth surfaced objects.\n(Wood,Plastic)", "Rough/Oren-Nayar - Useful for rough surfaces, or surfaces with billions of tiny indents.\n(Carpet,Skin)", "Translucent/Wrap - Good for simulating sub-surface scattering, or translucent objects.\n(Skin,Plants)","Custom - Create your own lighting calculations in the lighting tab."});//);
		ShaderVar DiffuseSetting1 = new ShaderVar("Setting1",0f);
		ShaderVar DiffuseSetting2 = new ShaderVar("Wrap Color",new Vector4(0.4f,0.2f,0.2f,1f));
		ShaderVar PBRQuality = new ShaderVar("PBR Quality",new string[] {"Auto", "High", "Medium", "Low"},new string[] {"","","",""},new string[] {"UNITY_BRDF_PBSSS","BRDF1_Unity_PBSSS", "BRDF2_Unity_PBSSS", "BRDF3_Unity_PBSSS"});
		ShaderVar PBRModel = new ShaderVar("PBR Model",new string[] {"Specular", "Metal"},new string[] {"",""},new string[] {"Specular","Metal"});
		ShaderVar PBRSpecularType = new ShaderVar("PBR Specular Type",new string[] {"GGX", "BlinnPhong"},new string[] {"",""},new string[] {"GGX","BlinnPhong"});
		ShaderVar DiffuseNormals = new ShaderVar("Disable Normals",0f);
		ShaderVar SpecularOn = new ShaderVar("Specular On",true);
		ShaderVar SpecularLightingType = new ShaderVar("Specular Type",new string[] {"Standard", "Circular","Wave"},new string[]{"ImagePreviews/SpecularNormal.png","ImagePreviews/SpecularCircle.png","ImagePreviews/SpecularWave.png"},"",new string[] {"BlinnPhong - Standard specular highlights.", "Circular - Circular specular highlights(Unrealistic)","Wave - A strange wave like highlight."});	
		ShaderVar SpecularHardness = new ShaderVar("Spec Hardness",0.3f);
		ShaderVar MaxLightSpecularHardness = new ShaderVar("Max Light Spec Hardness",0.9f);
		ShaderVar SpecularEnergy = new ShaderVar("Spec Energy Conserve",true);
		ShaderVar SpecularOffset = new ShaderVar("Spec Offset",0f);
		ShaderVar EmissionOn = new ShaderVar("Emission On",false);
//		ShaderVar EmissionColor = new ShaderVar("Emission Color",new Vector4(0f,0f,0f,0f));
		ShaderVar EmissionType = new ShaderVar("Emission Type",new string[]{"Standard","Multiply","Set"},new string[]{"ImagePreviews/EmissionOn.png","ImagePreviews/EmissionMul.png","ImagePreviews/EmissionSet.png"},"",new string[]{"Standard - Simply add the emission on top of the base color.","Multiply - Add the emission multiplied by the base color. An emission color of white adds the base color to itself.","Set - Mixes the shadeless color on top based on the alpha."});
		ShaderVar TransparencyOn = new ShaderVar("Transparency On",false);
		ShaderVar TransparencyType = new ShaderVar("Transparency Type",new string[]{"Cutout","Fade"},new string[]{"ImagePreviews/TransparentCutoff.png","ImagePreviews/TransparentFade.png"},"",new string[]{"Cutout - Only allows alpha to be on or off, fast, but creates sharp edges. This can have aliasing issues and is slower on mobile.","Fade - Can have many levels of transparency, but can have depth sorting issues (Objects can appear in front of each other incorrectly)."}); 
		ShaderVar TransparencyZWrite = new ShaderVar("ZWrite",false); 
		ShaderVar TransparencyCorrectShadows = new ShaderVar("Correct Shadows",true); 
		ShaderVar TransparencyPBR = new ShaderVar("Use PBR",true); 
		ShaderVar TransparencyAlphaToCoverage = new ShaderVar("Alpha To Coverage",false); 
		//ShaderVar TransparencyPreMult = new ShaderVar("Pre-MultiplyOld",new string[]{"Off","In Assets","In Shader"},new string[]{"","",""},new string[]{"Off","In Assets","In Shader"}); 
		//ShaderVar TransparencyPreMult2 = new ShaderVar("Pre-Multiply",false); 
		ShaderVar TransparencyReceive = new ShaderVar("Receive Shadows",false);
		ShaderVar TransparencyZWriteType = new ShaderVar("ZWrite Type",new string[]{"Off","Full","Cutoff"},new string[]{"","","",""},new string[]{"Off","Full","Cutoff"});
		ShaderVar BlendMode = new ShaderVar("Blend Mode",new string[]{"Mix","Add","Mul"},new string[]{"","",""},new string[]{"Mix","Add","Mul"});
		ShaderVar TransparencyAmount = new ShaderVar("Transparency",0f);
		ShaderVar TransparencyZWriteAmount = new ShaderVar("CutoffZWriteTransparency",0f);
		ShaderVar ShellsOn = new ShaderVar("Shells On",false);
		ShaderVar ShellsCount = new ShaderVar("Shell Count",1,0,50);	
		ShaderVar ShellsDistance = new ShaderVar("Shells Distance",0.1f);
		ShaderVar ShellsEase = new ShaderVar("Shell Ease",1,0f,3f);
		ShaderVar ShellsZWrite = new ShaderVar("Shells ZWrite",true);
		ShaderVar ShellsFront = new ShaderVar("Shell Front",true); 
		ShaderVar ShellsSkipFirst = new ShaderVar("Shell Skip First",true); 
		ShaderVar ParallaxOn = new ShaderVar("Parallax On",false);
		ShaderVar ParallaxHeight = new ShaderVar("Parallax Height",0.1f); 
		ShaderVar ParallaxBinaryQuality = new ShaderVar("Parallax Quality",10,0,50);
		ShaderVar ParallaxSilhouetteClipping = new ShaderVar("Silhouette Clipping",false);
		ShaderVar ParallaxDepthCorrection = new ShaderVar("Parallax Depth Correction",false);
		ShaderVar ParallaxShadows = new ShaderVar("Parallax Shadows",false);
		ShaderVar ParallaxShadowSize = new ShaderVar("Parallax Shadow Size",1f,1f,10f);
		ShaderVar ParallaxShadowStrength = new ShaderVar("Parallax Shadow Strength",1f,0f,10f);
		ShaderVar TessellationOn = new ShaderVar("Tessellation On",false);
		ShaderVar TessellationType = new ShaderVar("Tessellation Type",new string[]{"Equal","Size","Distance"},new string[]{"Tessellate all faces the same amount (Not Recommended!).","Tessellate faces based on their size (larger = more tessellation).","Tessellate faces based on distance and screen area."},new string[]{"Equal","Size","Distance"});
		ShaderVar TessellationQuality = new ShaderVar("Tessellation Quality",10,1,50);
		ShaderVar TessellationFalloff = new ShaderVar("Tessellation Falloff",1,1,3);
		ShaderVar TessellationSmoothingAmount = new ShaderVar("Tessellation Smoothing Amount",0f,-3,3);
		ShaderVar TessellationTopology = new ShaderVar("Tessellation Toplogy",new string[]{"Triangles","Point"},new string[]{"",""},new string[]{"triangle_cw","point"});
		ShaderVar PokeOn = new ShaderVar("Poke On",false);
		ShaderVar PokeCount = new ShaderVar("Poke Count",4f);
		ShaderVar PokeLine = new ShaderVar("Poke Line",true);
		ShaderVar PokeInteraction = new ShaderVar("Poke Interaction",new string[]{"None","Collapse","Bounce","Ripple"},new string[]{"","","",""},new string[]{"","","",""});
		ShaderVar PokeFloat3s = new ShaderVar("Poke Float3 Count",0f);
		ShaderVar PokeFloats = new ShaderVar("Poke Float Count",0f);
		ShaderVar PokeCustom = new ShaderVar("Poke Custom",false);
		
		
		
		
		
		Dictionary<string,ShaderVar> D = new Dictionary<string,ShaderVar>();
		D.Add(Name.Name,Name);
		D.Add(Visible.Name,Visible);
		D.Add(TechLOD.Name,TechLOD);
		D.Add(TechCull.Name,TechCull);
		D.Add(TechZTest.Name,TechZTest);
		D.Add(TechZWrite.Name,TechZWrite);
		D.Add(TechShaderTarget.Name,TechShaderTarget);

		D.Add(MiscVertexRecalculation.Name,MiscVertexRecalculation);
		D.Add(MiscFog.Name,MiscFog);
		D.Add(MiscAmbient.Name,MiscAmbient);
		D.Add(MiscVertexLights.Name,MiscVertexLights);
		D.Add(MiscLightmap.Name,MiscLightmap);
		D.Add(MiscFullShadows.Name,MiscFullShadows);
		D.Add(MiscForwardAdd.Name,MiscForwardAdd);
		D.Add(MiscShadowCaster.Name,MiscShadowCaster);
		D.Add(MiscForward.Name,MiscForward);
		D.Add(MiscDeferred.Name,MiscDeferred);
		D.Add(MiscShadows.Name,MiscShadows);
		D.Add(MiscInterpolateView.Name,MiscInterpolateView);
		D.Add(MiscRecalculateAfterVertex.Name,MiscRecalculateAfterVertex);
		D.Add(MiscVertexToastie.Name,MiscVertexToastie);
		D.Add(MiscVertexToastieOptimized.Name,MiscVertexToastieOptimized);

		D.Add(IndirectControl.Name,IndirectControl);
		D.Add(IndirectAmbientOcclusionOn.Name,IndirectAmbientOcclusionOn);
		D.Add(IndirectAmbientOcclusionControl.Name,IndirectAmbientOcclusionControl);
		D.Add(IndirectAmbientOcclusionRealtime.Name,IndirectAmbientOcclusionRealtime);
		D.Add(IndirectAmbientOcclusionOptimized.Name,IndirectAmbientOcclusionOptimized);
		D.Add(IndirectAmbientOcclusionOptimizedType.Name,IndirectAmbientOcclusionOptimizedType);
		D.Add(IndirectAmbientOcclusionStrength.Name,IndirectAmbientOcclusionStrength);
		D.Add(IndirectAmbientOcclusionPushback.Name,IndirectAmbientOcclusionPushback);
		D.Add(IndirectAmbientOcclusionDisplayType.Name,IndirectAmbientOcclusionDisplayType);
		
		D.Add(IndirectSubsurfaceScatteringOn.Name,IndirectSubsurfaceScatteringOn);
		D.Add(IndirectSubsurfaceScattering.Name,IndirectSubsurfaceScattering);
		D.Add(IndirectSubsurfaceScatteringMix.Name,IndirectSubsurfaceScatteringMix);
		D.Add(IndirectSubsurfaceScatteringUseThickness.Name,IndirectSubsurfaceScatteringUseThickness);
		D.Add(IndirectSubsurfaceScatteringAmbient.Name,IndirectSubsurfaceScatteringAmbient);
		D.Add(IndirectSubsurfaceScatteringDensity.Name,IndirectSubsurfaceScatteringDensity);
		D.Add(IndirectSubsurfaceScatteringDistortion.Name,IndirectSubsurfaceScatteringDistortion);
		D.Add(IndirectSubsurfaceScatteringScale.Name,IndirectSubsurfaceScatteringScale);
		D.Add(IndirectSubsurfaceScatteringThinness.Name,IndirectSubsurfaceScatteringThinness);
		D.Add(IndirectSubsurfaceScatteringColor.Name,IndirectSubsurfaceScatteringColor);
		
		D.Add(DiffuseOn.Name,DiffuseOn);
		D.Add(DiffuseLightingType.Name,DiffuseLightingType);
		D.Add(DiffuseSetting1.Name,DiffuseSetting1);
		D.Add(DiffuseSetting2.Name,DiffuseSetting2);
		D.Add(PBRQuality.Name,PBRQuality);
		D.Add(PBRModel.Name,PBRModel);
		D.Add(PBRSpecularType.Name,PBRSpecularType);
		D.Add(DiffuseNormals.Name,DiffuseNormals);

		D.Add(SpecularOn.Name,SpecularOn);
		D.Add(SpecularLightingType.Name,SpecularLightingType);
		D.Add(SpecularHardness.Name,SpecularHardness);
		D.Add(MaxLightSpecularHardness.Name,MaxLightSpecularHardness);
		D.Add(SpecularEnergy.Name,SpecularEnergy);
		D.Add(SpecularOffset.Name,SpecularOffset);

		D.Add(EmissionOn.Name,EmissionOn);
		D.Add(EmissionType.Name,EmissionType);

		D.Add(TransparencyOn.Name,TransparencyOn);
		D.Add(TransparencyType.Name,TransparencyType);
		D.Add(TransparencyZWrite.Name,TransparencyZWrite);
		D.Add(TransparencyCorrectShadows.Name,TransparencyCorrectShadows);
		D.Add(TransparencyPBR.Name,TransparencyPBR);
		D.Add(TransparencyAlphaToCoverage.Name,TransparencyAlphaToCoverage);
		D.Add(TransparencyAmount.Name,TransparencyAmount);
		D.Add(TransparencyZWriteAmount.Name,TransparencyZWriteAmount);
		D.Add(TransparencyReceive.Name,TransparencyReceive);
		D.Add(TransparencyZWriteType.Name,TransparencyZWriteType);
		D.Add(BlendMode.Name,BlendMode);
		//D.Add(TransparencyPreMult2.Name,TransparencyPreMult2);

		D.Add(ShellsOn.Name,ShellsOn);
		D.Add(ShellsCount.Name,ShellsCount);
		D.Add(ShellsDistance.Name,ShellsDistance);
		D.Add(ShellsEase.Name,ShellsEase);
		D.Add(ShellsZWrite.Name,ShellsZWrite);
		D.Add(ShellsFront.Name,ShellsFront);
		D.Add(ShellsSkipFirst.Name,ShellsSkipFirst);

		D.Add(ParallaxOn.Name,ParallaxOn);
		D.Add(ParallaxHeight.Name,ParallaxHeight);
		D.Add(ParallaxBinaryQuality.Name,ParallaxBinaryQuality);
		D.Add(ParallaxSilhouetteClipping.Name,ParallaxSilhouetteClipping);
		D.Add(ParallaxDepthCorrection.Name,ParallaxDepthCorrection);
		D.Add(ParallaxShadows.Name,ParallaxShadows);
		D.Add(ParallaxShadowStrength.Name,ParallaxShadowStrength);
		D.Add(ParallaxShadowSize.Name,ParallaxShadowSize);

		D.Add(TessellationOn.Name,TessellationOn);
		D.Add(TessellationType.Name,TessellationType);
		D.Add(TessellationQuality.Name,TessellationQuality);
		D.Add(TessellationFalloff.Name,TessellationFalloff);
		D.Add(TessellationSmoothingAmount.Name,TessellationSmoothingAmount);
		D.Add(TessellationTopology.Name,TessellationTopology);
		
		D.Add(PokeOn.Name,PokeOn);
		D.Add(PokeCount.Name,PokeCount);
		D.Add(PokeLine.Name,PokeLine);
		D.Add(PokeInteraction.Name,PokeInteraction);
		D.Add(PokeFloats.Name,PokeFloats);
		D.Add(PokeFloat3s.Name,PokeFloat3s);
		D.Add(PokeCustom.Name,PokeCustom);

		return D;
	}
}
}