#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define PRE_UNITY_5
#else
#define UNITY_5
#endif
#pragma warning disable 0618
//Yes the code in here is an absolute pile of crap...I just wanted to be done XD
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;
using SU = ElectronicMind.Sandwich.ShaderUtil;
using EMindGUI = ElectronicMind.ElectronicMindStudiosInterfaceUtils;
using UnityEngine.Rendering;
using ElectronicMind.Sandwich;
namespace ElectronicMind.Sandwich{
[System.Serializable]
public class ShaderPreview : EditorWindow, ISerializationCallbackReceiver {
	
	public GameObject previewParent;
	public GameObject previewCamAnchor;
	public GameObject previewCamAnchorTarget;
	public GameObject previewCam;
	public GameObject previewLightAnchor;
	public GameObject previewObject;
	public GameObject previewBackdrop;
	public GameObject Light1;
	//public LightType  Light1Type = LightType.Directional;
	//public Color	  Light1Color = new Color(1f*1.1f,0.9f*1.1f,0.75f*1.1f,1f);
	public GameObject Light2;
	//public LightType  Light2Type = LightType.Directional;
	//public Color	  Light2Color = new Color(0.4f,0.4f,0.4f,1f);
	public GameObject Light3;
	//public LightType  Light3Type = LightType.Point;
	//public Color	  Light3Color = new Color(1f,0f,0f,1f);
	public Mesh previewMesh;
	public Material previewMat;
	public Material previewMat2;
	public Material previewMatW;
	public Material backMat;
	public Material light1Mat;
	public Material light2Mat;
	public Material light3Mat;
	
	public Cubemap defaultBackCube;
	
	public Mesh customObject;
	public Cubemap backCube;
	public Cubemap CurCube{
		get{
			if (backCube!=null)
				return backCube;
			#if UNITY_5
				return ShaderSandwich.DayCube;
			#else
				return ShaderSandwich.KitchenCube;
			#endif
		}
		set{}
	}
	public Cubemap BlackCube;
	public SphericalHarmonicsL2 NewAmbient;
	
	public int MeshType;
	public List<Mesh> Meshes;
	public bool InDrag = false;
	public bool Wireframe = false;
	public bool Shadows = false;
	static public ShaderPreview Instance;
	
	public static void Init () {
		ShaderPreview windowG = (ShaderPreview)EditorWindow.GetWindow (typeof (ShaderPreview));
		Instance = windowG;
		windowG.previewCam = null;
		#if UNITY_5
		windowG.defaultBackCube = ShaderSandwich.DayCube;
		#else
		windowG.defaultBackCube = ShaderSandwich.KitchenCube;
		
		EditorUtility.DisplayDialog("Unity 4 Free","Hi! Unity 4 Free does not support render textures, so the preview window will render using a technique called handles. However, this does not always look correct and can have z-writing issues. If you want, I'd recommend opening the scene ShaderSandwichPreviewScene.unity and using that as the preview window :).","Ok");
		#endif
		windowG.Setup();
		windowG.wantsMouseMove = true;
		windowG.minSize = new Vector2(270,200);
		//windowG.maxSize = new Vector2(400,400);
		windowG.title = "Preview";
//		Debug.Log(windowG.previewMat);

		//windowG.Meshes = null;
		
	}
	int closeDelay = 2;
	void OnDestroy(){
		Cleanup();
	}
	void Update(){
		if (ShaderSandwich.Instance==null){
			closeDelay-=1;
			if (closeDelay<0)
				Close();
		}
		else
		closeDelay = 2;

		#if UNITY_5
		defaultBackCube = ShaderSandwich.DayCube;
		#else
		defaultBackCube = ShaderSandwich.KitchenCube;
		#endif
	}
	float ToGreyscale(Color Col){
		return (Col.r+Col.g+Col.b)/3f;
	}
	ShaderShaderAndMaterialBadcooodIthinkdotdotdot readCubemap;
	void CalcAmbientLightFace(ref SphericalHarmonicsL2 l, Cubemap c, CubemapFace face, ref float total){
		RenderTexture preRT = RenderTexture.active;
		
		RenderTexture FaceGPU = RenderTexture.GetTemporary(32,32,0,RenderTextureFormat.DefaultHDR,RenderTextureReadWrite.Linear);
		Texture2D FaceCPU = new Texture2D(32,32,TextureFormat.RGBAHalf,false,true);
		
		readCubemap.Material.SetTexture("Cubemap",c);
		
		int fi = 0;
		if (face == CubemapFace.PositiveX)fi = 0;
		if (face == CubemapFace.NegativeX)fi = 1;
		if (face == CubemapFace.PositiveY)fi = 2;
		if (face == CubemapFace.NegativeY)fi = 3;
		if (face == CubemapFace.PositiveZ)fi = 4;
		if (face == CubemapFace.NegativeZ)fi = 5;
		
		readCubemap.Material.SetFloat("Face",fi);
		GL.sRGBWrite = false;
		
		Graphics.Blit(null,FaceGPU,readCubemap.Material);
		
		RenderTexture.active = FaceGPU;
			FaceCPU.ReadPixels(new Rect(0,0,32,32),0,0,false);
			FaceCPU.Apply();
		RenderTexture.active = preRT;
		
		RenderTexture.ReleaseTemporary(FaceGPU);
		Color[] cols = FaceCPU.GetPixels(0);
		double sqrt = 32;
		int res = (int)sqrt;
		for(int x = 0;x<res;x++){
			float u = ((float)x/((float)res/2f))-1f;
			//Debug.Log(cols[x]);
			for(int y = 0;y<res;y++){
				float v = ((float)y/((float)res/2f))-1f;
				Vector3 Dir = new Vector3(0f,0f,0f);
				
				if (face==CubemapFace.PositiveX)Dir = new Vector3(1f,-v,-u);
				if (face==CubemapFace.NegativeX)Dir = new Vector3(-1f,-v,u);
				if (face==CubemapFace.PositiveZ)Dir = new Vector3(u,-v,1f);
				if (face==CubemapFace.NegativeZ)Dir = new Vector3(-u,-v,-1f);
				if (face==CubemapFace.PositiveY)Dir = new Vector3(u,1f,v);
				if (face==CubemapFace.NegativeY)Dir = new Vector3(u,-1f,-v);
				
				
				//
				Color col = cols[x+y*res];
				float scale = 4f/((1f+u*u+v*v)*Mathf.Sqrt(1f+u*u+v*v));
	
				Dir.Normalize();
				total += scale;
				l.AddDirectionalLight(Dir,col,scale);
			}
		}
		UnityEngine.Object.DestroyImmediate(FaceCPU,false);
	}
	void CalcAmbientLight(Cubemap TempCube){
			//AMBIENT LIGHT CALCULATION
			Cubemap UseCube = TempCube;
			
			#if UNITY_5
				SphericalHarmonicsL2 L = new SphericalHarmonicsL2();
				float total = 0f;
				CalcAmbientLightFace(ref L, UseCube, CubemapFace.PositiveX, ref total);
				CalcAmbientLightFace(ref L, UseCube, CubemapFace.NegativeX, ref total);
				CalcAmbientLightFace(ref L, UseCube, CubemapFace.PositiveY, ref total);
				CalcAmbientLightFace(ref L, UseCube, CubemapFace.NegativeY, ref total);
				CalcAmbientLightFace(ref L, UseCube, CubemapFace.PositiveZ, ref total);
				CalcAmbientLightFace(ref L, UseCube, CubemapFace.NegativeZ, ref total);
				total = 1f/total;
				L *= total;
				NewAmbient = L;
				//(previewCam.GetComponent("PreviewCameraSet") as PreviewCameraSet).NewAmbient = L;
			#endif
			///TODO:
			///Color C = (PosX/6f)+(NegX/6f)+(PosY/6f)+(NegY/6f)+(PosZ/6f)+(NegZ/6f);
			///(previewCam.GetComponent("PreviewCameraSet") as PreviewCameraSet).NewAmbientColor = C;
			if (ShaderSandwich.Instance!=null)
				ShaderSandwich.Instance.UpdateShaderPreview();
	}
	public bool HasDoneASerializeThingo = true;
	public void Ser(){
		//Setup();
		//CalcAmbientLight(CurCube);
		HasDoneASerializeThingo = true;
	}
	public void OnBeforeSerialize(){}
	public void OnAfterDeserialize(){
		HasDoneASerializeThingo = true;
	}
	public void OnEnable(){
		readCubemap = ScriptableObject.CreateInstance<ShaderShaderAndMaterialBadcooodIthinkdotdotdot>();
		readCubemap.shader = "Hidden/ShaderSandwich/Previews/ReadCubemap";
		
		EditorApplication.playmodeStateChanged += Ser;
		//if (ShaderSandwich.Instance!=null)
		//	Ser();
	}
	public bool Hidden = true;
	public void Cleanup(){
		Transform[] obs = Resources.FindObjectsOfTypeAll(typeof(Transform)) as Transform[];
		for(int i=0;i<obs.Length;i++){
			if (obs[i]!=null){
				GameObject ob = obs[i].gameObject;
				if (ob!=null){
					if (ob.name=="Shader Sandwich Preview"||ob.name=="Shader Sandwich Preview Camera"||ob.name=="Shader Sandwich Preview Object"||ob.name=="Shader Sandwich Preview Light 1"||ob.name=="Shader Sandwich Preview Light 2"||ob.name=="Shader Sandwich Preview Light 3"||ob.name=="Shader Sandwich Preview Backdrop"||ob.name=="Shader Sandwich Camera Anchor"||ob.name=="Shader Sandwich Camera Anchor Target"||ob.name=="Shader Sandwich Light Anchor"){
						GameObject.DestroyImmediate(ob,true);
					}
				}
			}
		}
	}
	void UpdateHideFlags(){
		HideFlags hf = HideFlags.DontSaveInEditor|HideFlags.DontSaveInBuild;
		if (Hidden)
			hf = hf|HideFlags.HideInHierarchy;
		
		previewParent.hideFlags = hf;
		
		previewCamAnchor.hideFlags = hf;
		previewCamAnchorTarget.hideFlags = hf;
		previewCam.hideFlags = hf;

		previewLightAnchor.hideFlags = hf;

		previewObject.hideFlags = hf;

		previewBackdrop.hideFlags = hf;

		Light1.hideFlags = hf;
		Light2.hideFlags = hf;
		Light3.hideFlags = hf;
		EditorApplication.DirtyHierarchyWindowSorting();
		EditorApplication.RepaintHierarchyWindow();
	}
	void Setup(){
		if (Meshes==null){
			Meshes = new List<Mesh>();
			GameObject Prim = (GameObject)GameObject.Instantiate( EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Models/SimpleCube.fbx") as GameObject);
			Meshes.Add(((MeshFilter)(Prim.GetComponent("MeshFilter"))).sharedMesh);
			GameObject.DestroyImmediate(Prim,true);
			Prim = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			Meshes.Add(((MeshFilter)(Prim.GetComponent("MeshFilter"))).sharedMesh);
			GameObject.DestroyImmediate(Prim,true);
			Prim = (GameObject)GameObject.Instantiate( EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Models/Monkey.fbx") as GameObject);
			Meshes.Add(((MeshFilter)(Prim.GetComponent("MeshFilter"))).sharedMesh);
			GameObject.DestroyImmediate(Prim,true);
			Prim = (GameObject)GameObject.Instantiate( EditorGUIUtility.Load("ElectronicMindStudios/Shader Sandwich/Models/TesselatedCube.fbx") as GameObject);
			Meshes.Add(((MeshFilter)(Prim.GetComponent("MeshFilter"))).sharedMesh);
			GameObject.DestroyImmediate(Prim,true);
		}
		if ((previewParent==null||previewCam==null||previewObject==null||previewCamAnchor==null||previewCamAnchorTarget==null||backMat==null||previewBackdrop==null||Light1==null||Light2==null||Light3==null||previewLightAnchor==null)){///TOoDO: Add Light 3
			
//Debug.Log("We must spread!!!");
			//Cleanup previous objects
			Cleanup();
			backMat = new Material (Shader.Find("Hidden/SSCubemapSkybox2"));
			backMat.SetTexture("_Cube",defaultBackCube);
			light1Mat = new Material (Shader.Find("Unlit/Color Transparent"));
			light2Mat = new Material (Shader.Find("Unlit/Color Transparent"));
			light3Mat = new Material (Shader.Find("Unlit/Color Transparent"));
			
			HideFlags hf = HideFlags.DontSaveInEditor|HideFlags.DontSaveInBuild;
			if (Hidden)
				hf = hf|HideFlags.HideInHierarchy;
			
			previewParent = new GameObject();
			previewParent.hideFlags = hf;
			previewParent.name = "Shader Sandwich Preview";
			
			previewCamAnchor = new GameObject();
				previewCamAnchor.name = "Shader Sandwich Camera Anchor";
				previewCamAnchor.hideFlags = hf;
				previewCamAnchor.transform.eulerAngles = new Vector3(0f,0,0);
				previewCamAnchor.transform.parent = previewParent.transform;
			
			previewCamAnchorTarget = new GameObject();
				previewCamAnchorTarget.name = "Shader Sandwich Camera Anchor Target";
				previewCamAnchorTarget.hideFlags = hf;
				previewCamAnchorTarget.transform.eulerAngles = new Vector3(0f,0,0);
				previewCamAnchorTarget.transform.parent = previewParent.transform;

			previewCam = new GameObject();
				previewCam.name = "Shader Sandwich Preview Camera";
				previewCam.hideFlags = hf;
				//previewCam.transform.position = new Vector3(0,0,-2.175257f);
				previewCam.transform.position = new Vector3(0,0,-2.175257f);
				previewCam.transform.parent = previewCamAnchor.transform;

			previewLightAnchor = new GameObject();
				previewLightAnchor.name = "Shader Sandwich Preview Light Anchor";
				previewLightAnchor.hideFlags = hf;
				previewLightAnchor.transform.position = new Vector3(0,0,-2.175257f);
				previewLightAnchor.transform.parent = previewCamAnchor.transform;
				//PreviewCameraSet PCS = previewCam.AddComponent<PreviewCameraSet>();
				//PCS.NewCubemap = defaultBackCube;
				
				Camera CamComp = previewCam.AddComponent<Camera>() as Camera;
					CamComp.depth=-20;
					#if !UNITY_5_0
						CamComp.clearFlags = CameraClearFlags.SolidColor;
						CamComp.backgroundColor = new Color(12f/255f,12f/255f,12f/255f,0);
					#endif
					CamComp.renderingPath = RenderingPath.Forward;
			
			previewObject = new GameObject();
				previewObject.name = "Shader Sandwich Preview Object";
				previewObject.hideFlags = hf;//|HideFlags.HideInHierarchy;
				previewObject.transform.eulerAngles = new Vector3(0f,0,0);
				MeshFilter MeshFilComp = previewObject.AddComponent<MeshFilter>() as MeshFilter;
					MeshFilComp.sharedMesh = Meshes[0];
				MeshRenderer MeshRendComp = previewObject.AddComponent<MeshRenderer>() as MeshRenderer;
				previewObject.transform.parent = previewParent.transform;

			previewBackdrop = new GameObject();
				previewBackdrop.name = "Shader Sandwich Preview Backdrop";
				previewBackdrop.hideFlags = hf;
				previewBackdrop.transform.eulerAngles = new Vector3(0f,0,0);
				previewBackdrop.transform.localScale = new Vector3(40,40,40);
				previewBackdrop.transform.parent = previewParent.transform;
				MeshFilComp = previewBackdrop.AddComponent<MeshFilter>() as MeshFilter;
					MeshFilComp.sharedMesh = Meshes[1];
				MeshRendComp = previewBackdrop.AddComponent<MeshRenderer>() as MeshRenderer;
					MeshRendComp.material = backMat;
			
			
			Light1 = new GameObject();
			Light2 = new GameObject();
			Light3 = new GameObject();
			
			int eye = 0;
			foreach(GameObject Licht in new GameObject[]{Light1,Light2,Light3}){
				Licht.name = "Shader Sandwich Preview Light "+(eye+1).ToString();
				Licht.hideFlags = hf;//|HideFlags.HideInHierarchy;
				if (eye==0)		Licht.transform.position = new Vector3(-2f,2f,-2f);
				else if (eye==1)	Licht.transform.position = new Vector3(2f,-2f,2f);
				else			Licht.transform.position = new Vector3(0f,1f,0f);
				Licht.transform.localScale = new Vector3(0.25f,0.25f,0.25f);
				//Light1.transform.eulerAngles = new Vector3(38f,428f,-48f);
				Light Light1Light = Licht.AddComponent<Light>() as Light;
				Licht.transform.parent = previewParent.transform;
					Light1Light.intensity = (eye==0)?1.1f:0f;
					Light1Light.color = new Color(1f,0.9f,0.75f,1f);
					Light1Light.type = (eye==0)?LightType.Directional:LightType.Point;
					Light1Light.spotAngle = 60;
				MeshFilComp = Licht.AddComponent<MeshFilter>() as MeshFilter;
					MeshFilComp.sharedMesh = Meshes[1];
				MeshRendComp = Licht.AddComponent<MeshRenderer>() as MeshRenderer;
					MeshRendComp.material = new Material[]{light1Mat,light2Mat,light3Mat}[eye];
					
				if (eye==0){
					Light1Light.shadows = LightShadows.Soft;
					Light1Light.shadowNearPlane = 0.3f;
					Light1Light.shadowNormalBias = 0f;
					Light1Light.shadowBias = 0.02f;
				}
				eye++;
			}
			previewCamAnchorTarget.transform.eulerAngles = new Vector3(0f,180f,0);
					
			CalcAmbientLight(defaultBackCube);
			
			previewCam.GetComponent<Camera>().farClipPlane = 40F;
			previewCam.GetComponent<Camera>().nearClipPlane = 0.05F;
		}
		if (previewMat==null){
			if (Shader.Find("Shader Sandwich/Shader Preview")!=null){
				previewMat = new Material (Shader.Find("Shader Sandwich/Shader Preview"));
				previewMat.SetTexture("_Cube",defaultBackCube);
				AssetDatabase.CreateAsset(previewMat,"Assets/ElectronicMindStudios/Shader Sandwich/Internal/Shader Sandwich/Shader Sandwich Preview Material.mat");
			}
		}
		if (previewMatW==null){
			if (Shader.Find("Hidden/SSTempWireframe")!=null)
				previewMatW = new Material (Shader.Find("Hidden/SSTempWireframe"));
		}
		if(previewMat2!=null){
			previewMat2.renderQueue = -1;
		}
		previewObject.GetComponent<MeshRenderer>().sharedMaterial = previewMat;
		
		
	}
	RenderTexture NormalRenderTexture;
	RenderTexture WireframeRenderTexture;
	
	public int SelectedLight = 0;
	//Rect WinRectTest = new Rect(20,20,200,200);
	void OnGUI(){
		if (Event.current.type==EventType.Repaint)
			deltaTime = (float)(EditorApplication.timeSinceStartup-lastTime);
		Vector2 WinSize = new Vector2(position.width,position.height);
		if (ShaderSandwich.Instance!=null){
			Instance = this;
			Repaint();
			Setup();
			
			//if (SmoothDamp)
			//	Light1.GetComponent<Light>().intensity = 0.9f;
			//else
			//	Light1.GetComponent<Light>().intensity = 0f;
			
			Vector2 WindowSize = new Vector2(position.width,position.height);
			if(Event.current.type==EventType.Repaint)
				EMindGUI.AddProSkin(WindowSize);
			
			Color OldCol = GUI.color;
			Color OldBGCol = GUI.backgroundColor;
			Rect PreviewRect = new Rect(0,0,WindowSize.x,WindowSize.y-64);
			
			//EMindGUI.MakeTooltip(1,WireframeRect,"Enable wireframe mode. Doesn't work\nsometimes (I think on Windows 10?), sorry :(.");
			GUI.backgroundColor = new Color(1,1,1,1);
			Rect CubemapRect = new Rect(0,PreviewRect.height,64,64);
				EMindGUI.MakeTooltip(1,CubemapRect,"Choose the background cubemap.");
				EditorGUI.BeginChangeCheck();
				backCube = (Cubemap)EditorGUI.ObjectField(CubemapRect,backCube,typeof(Cubemap),false);
				if (EditorGUI.EndChangeCheck()){
					CalcAmbientLight(CurCube);
					
					backMat.SetTexture("_Cube",CurCube);
					previewMat.SetTexture("_Cube",CurCube);
					//(previewCam.GetComponent("PreviewCameraSet") as PreviewCameraSet).NewCubemap = CurCube;
					//Debug.Log(CurCube);
				}
			
				//Rect CustomRect = EditorGUILayout.GetControlRect(false,GUILayout.MaxWidth(108));
				/*if (customObject!=null)
				GUI.DrawTexture(new Rect(64,PreviewRect.height,44,44),AssetPreview.GetAssetPreview(customObject));
				Rect CustomRect = new Rect(64,PreviewRect.height,64,10);
				EMindGUI.MakeTooltip(1,CustomRect,"Choose a custom preview object.");
				customObject = (Mesh)EditorGUI.ObjectField(CustomRect,customObject,typeof(Mesh),false);
				//customObject = (Mesh)EditorGUILayout.ObjectField(customObject,typeof(Mesh),false,GUILayout.MaxWidth(108));
				if (customObject){
					MeshType = 4;
					previewObject.GetComponent<MeshFilter>().sharedMesh = customObject;
				}*/
			GUI.backgroundColor = OldBGCol;
			
			
			
			
			int w = (int)((WinSize.x-128-10)/2f);
			
			float left = 3;
			int eyeyeyeyeyey = 0;
			foreach(GameObject go in new GameObject[]{Light1,Light2,Light3}){
				if (EMindGUI.MouseDownIn(new Rect(64+64,PreviewRect.height+left,PreviewRect.width-64+64,14),false))
					SelectedLight = eyeyeyeyeyey;
				Light l = go.GetComponent<Light>();
				//GUI.Box(new Rect(64+left,PreviewRect.height+19,WinSize.x-64,WinSize.y-19),"");
				//GUI.Label(new Rect(64+12,PreviewRect.height+19,w,20),"Light 1");
				
				GUI.backgroundColor = new Color(1,1,1,1);
					Color newcol = EditorGUI.ColorField(new Rect(64+64+w+10,PreviewRect.height+left,w-10,14),new GUIContent(""),l.color*l.intensity,true,true,true,new ColorPickerHDRConfig(0,99,0.01010101f,3));
				GUI.backgroundColor = OldBGCol;
				
				LightType prevType = l.type;
				l.type = (LightType)EditorGUI.EnumPopup(new Rect(64+64+10,PreviewRect.height+left,w-10,20),"",l.type,EMindGUI.EditorPopup);
				if (prevType!=l.type&&prevType==LightType.Directional){
					go.GetComponent<Transform>().position = go.GetComponent<Transform>().position.normalized*4f;
				}
					
				if (newcol.maxColorComponent!=0f)
					l.color = newcol/newcol.maxColorComponent;
				else
					l.color = new Color(0f,0f,0f,0f);
				l.intensity = newcol.maxColorComponent;
				left += 21;
				eyeyeyeyeyey++;
			}
			
			
			
			
			SelectedLight = GUI.SelectionGrid(new Rect(64,PreviewRect.height,64,64),SelectedLight,new string[]{"Lamp 1","Lamp 2","Lamp 3"},1,EMindGUI.SCSkin.window);
			//GUI.Label(new Rect(64,PreviewRect.height+19+20,100,20),"Light 2");
			//GUI.Label(new Rect(64,PreviewRect.height+19+40,100,20),"Light 3");
				/*
				Tesselated
				Subdivided
				
				High Poly
				Detailed
				Complex
				Sliced
				*/
				
			
				
			float time = (float)EditorApplication.timeSinceStartup;   
			Vector4 vTime = new Vector4(( time / 20f)%1000f, time%1000f, (time*2f)%1000f, (time*3f)%1000f);
			Vector4 vCosTime = new Vector4( Mathf.Cos(time / 8f), Mathf.Cos(time/4f), Mathf.Cos(time/2f), Mathf.Cos(time));
			Vector4 vSinTime = new Vector4( Mathf.Sin(time / 8f), Mathf.Sin(time/4f), Mathf.Sin(time/2f), Mathf.Sin(time));
			//(t/8, t/4, t/2, t)
			Shader.SetGlobalVector( "_SSTime", vTime );
			Shader.SetGlobalVector( "_SSCosTime", vCosTime );
			Shader.SetGlobalVector( "_SSSinTime", vSinTime );
			
			
			if (Event.current.type==EventType.Repaint){
				ShaderSandwich.Instance.UpdateShaderPreview();
				if (HasDoneASerializeThingo){
					HasDoneASerializeThingo = false;
					CalcAmbientLight(CurCube);
				}
				Camera PC = previewCam.GetComponent<Camera>();
				//Camera PCW = previewCamW.GetComponent<Camera>();
				
				PC.backgroundColor = new Color(0,0,0,0);
				//PCW.backgroundColor = new Color(0,0,0,0);
				#if UNITY_5||UNITY_PRO_LICENSE
					if (PC.targetTexture==null||PC.targetTexture.width != (int)PreviewRect.width||PC.targetTexture.height != (int)PreviewRect.height){
					
						if (NormalRenderTexture!=null)
							NormalRenderTexture.Release();
						if (WireframeRenderTexture!=null)
							WireframeRenderTexture.Release();
						
						
						NormalRenderTexture = new RenderTexture((int)PreviewRect.width,(int)PreviewRect.height,16,RenderTextureFormat.DefaultHDR);
						WireframeRenderTexture = new RenderTexture((int)PreviewRect.width,(int)PreviewRect.height,16);
						
						//PCW.targetTexture = NormalRenderTexture;
						NormalRenderTexture.antiAliasing = 8;
						NormalRenderTexture.depth = 24;
						WireframeRenderTexture.antiAliasing = 4;
						NormalRenderTexture.Create();
						WireframeRenderTexture.Create();
						
						PC.targetTexture = NormalRenderTexture;
					}
					PC.enabled = false;
					PC.depthTextureMode = DepthTextureMode.Depth;
					//PCW.enabled = false;
					CamStart();
					
						//previewBackdrop.GetComponent<MeshRenderer>().enabled = false;
						if (ShaderSandwich.Instance.OpenShader!=null){
							if (ShaderSandwich.Instance.OpenShader.RenderingPathTarget.Type==0)
								PC.renderingPath = RenderingPath.Forward;
							if (ShaderSandwich.Instance.OpenShader.RenderingPathTarget.Type==1)
								PC.renderingPath = RenderingPath.DeferredShading;
						}
							
						PC.clearFlags = CameraClearFlags.SolidColor;
						PC.targetTexture = NormalRenderTexture;
						PC.Render();
						if (Wireframe){
							previewBackdrop.GetComponent<MeshRenderer>().enabled = false;
							PC.clearFlags = CameraClearFlags.SolidColor;
							PC.targetTexture = WireframeRenderTexture;
							Light1.GetComponent<MeshRenderer>().enabled = false;
							Light2.GetComponent<MeshRenderer>().enabled = false;
							Light3.GetComponent<MeshRenderer>().enabled = false;
							GL.wireframe = true;
							if (ShaderSandwich.Instance.OpenShader!=null){
								if (ShaderSandwich.Instance.OpenShader.TransparencyType.Type==1&&ShaderSandwich.Instance.OpenShader.TransparencyOn.On)
									previewObject.GetComponent<MeshRenderer>().material = previewMatW;
							}
							PC.Render();
							previewObject.GetComponent<MeshRenderer>().material = previewMat;
							GL.wireframe = false;
							previewBackdrop.GetComponent<MeshRenderer>().enabled = true;
							
						}
					CamEnd();
					//GUI.DrawTexture(PreviewRect,NormalRenderTexture,ScaleMode.StretchToFill,false);
					Graphics.DrawTexture(PreviewRect,NormalRenderTexture,ShaderUtil.GetMaterial("Mix",new Color(1,1,1,1),false));
					if (Wireframe){
						Graphics.DrawTexture(PreviewRect,WireframeRenderTexture,ShaderUtil.GetMaterial("Mix",new Color(0,0,0,1),true));
					}
				#else
				Handles.BeginGUI();
					CamStart();
						Handles.DrawCamera(PreviewRect,previewCam.GetComponent<Camera>());
					CamEnd();
				Handles.EndGUI();
				#endif
				//
			}
			
			
			if (!ShaderSandwich.Instance.RealtimePreviewUpdates){
				
				GUI.color = new Color(1,1,1,1);
				GUI.skin.label.fontStyle = FontStyle.Bold;
				EMindGUI.Label(new Rect(PreviewRect.x+1,+1+5,PreviewRect.width,PreviewRect.height),"Realtime Updates are turned off...",15);
				EMindGUI.Label(new Rect(PreviewRect.x-1,-1+5,PreviewRect.width,PreviewRect.height),"Realtime Updates are turned off...",15);
				EMindGUI.Label(new Rect(PreviewRect.x,-1+5,PreviewRect.width,PreviewRect.height),"Realtime Updates are turned off...",15);
				EMindGUI.Label(new Rect(PreviewRect.x,+1+5,PreviewRect.width,PreviewRect.height),"Realtime Updates are turned off...",15);
				EMindGUI.Label(new Rect(PreviewRect.x-1,5,PreviewRect.width,PreviewRect.height),"Realtime Updates are turned off...",15);
				EMindGUI.Label(new Rect(PreviewRect.x+1,5,PreviewRect.width,PreviewRect.height),"Realtime Updates are turned off...",15);
				GUI.color = new Color(1,0,0,1);
				EMindGUI.Label(new Rect(PreviewRect.x,5,PreviewRect.width,PreviewRect.height),"Realtime Updates are turned off...",15);
				GUI.color = OldCol;
				GUI.skin.label.fontStyle = FontStyle.Normal;
				EMindGUI.Label(new Rect(0,0,0,0),"Realtime Updates are turned off...",12);
			}
			
			GUI.backgroundColor = new Color(OldBGCol.r,OldBGCol.g,OldBGCol.b,0.5f);
			EditorGUI.BeginChangeCheck();
			MeshType = GUI.SelectionGrid(new Rect(0,PreviewRect.height-20,WinSize.x,20),MeshType,new string[]{"Cube","Sphere","Suzanne","Custom"},4,EMindGUI.SCSkin.window);
			if (EditorGUI.EndChangeCheck()){
				if (MeshType!=3){
					if (previewObject.GetComponent<MeshFilter>().sharedMesh!=Meshes[MeshType]){
						previewObject.GetComponent<MeshFilter>().sharedMesh = Meshes[MeshType];
						previewObject.GetComponent<Transform>().position-=previewObject.GetComponent<Renderer>().bounds.center;
					}
					customObject = null;
				}
				else if (MeshType==3){
					EditorGUIUtility.ShowObjectPicker<Mesh>(customObject, true, "", 23);
				}
			}
			if (Event.current.type==EventType.ExecuteCommand&&Event.current.commandName=="ObjectSelectorUpdated"){
				customObject = (Mesh)EditorGUIUtility.GetObjectPickerObject();
				if (previewObject.GetComponent<MeshFilter>().sharedMesh!=customObject){
					previewObject.GetComponent<MeshFilter>().sharedMesh = customObject;
					previewObject.GetComponent<Transform>().position-=previewObject.GetComponent<Renderer>().bounds.center;
				}
			}
			Wireframe = GUI.Toggle(new Rect(0,PreviewRect.height-20-18,80,18),Wireframe,new GUIContent("Wireframe",Wireframe?ShaderSandwich.Tick:ShaderSandwich.Cross),EMindGUI.SCSkin.window);
			EditorGUI.BeginChangeCheck();
			Hidden = !GUI.Toggle(new Rect(PreviewRect.width-80,PreviewRect.height-20-18,80,18),!Hidden,new GUIContent("Expose",!Hidden?ShaderSandwich.Tick:ShaderSandwich.Cross),EMindGUI.SCSkin.window);
			if (EditorGUI.EndChangeCheck()){
				UpdateHideFlags();
			}
			GUI.backgroundColor = OldBGCol;
			
			/*		BeginWindows();
			EndWindows();*/
			EditorGUIUtility.AddCursorRect(new Rect(0,PreviewRect.y-3,WinSize.x,6),MouseCursor.ResizeVertical);
			
			bool InDrag2 = InDrag;
			if (InDrag2&&Event.current.button == 0)//!Event.current.shift)
				EditorGUIUtility.AddCursorRect(PreviewRect,MouseCursor.Pan);
			//else if (InDrag2&&Event.current.button == 1)
			else if (InDrag2&&Event.current.button == 1)//Event.current.shift)
				EditorGUIUtility.AddCursorRect(PreviewRect,MouseCursor.ResizeVertical);

			GUI.Box(new Rect(0,PreviewRect.height-4,WinSize.x,4),"","Button");
			WindowPreviewUpdate(PreviewRect);
			//GUI.skin = oldskin;
			EMindGUI.EndProSkin();
			EMindGUI.DrawTooltip(1,WinSize);
	//		Debug.Log("FMYlAG");
		}
		if (Event.current.type==EventType.Repaint)
			lastTime = EditorApplication.timeSinceStartup;
	}

	float previewCamAnchorScale = 1f;
	float previewLightAnchorScale = 1f;
	//Quaternion CameraRot = new Quaternion();
	public Vector2 camlookTarget = new Vector2(0,0);
	public Vector2 camlook = new Vector2(0,0);
	
	
	public Vector3 light1lookTarget = new Vector3(0,0,2);
	public Vector3 light1look = new Vector3(0,0,2);
	public bool setLightScaleDir;
	public Vector3 LightScaleDir;
	
	float deltaTime = 0f;
	double lastTime = -1d;
	void WindowPreviewUpdate(Rect PreviewRect){
		//PreviewRectSmaller = PreviewRect.Instatiate();
		PreviewRect.height-=3;
		PreviewRect.y+=3;
		if (EMindGUI.MouseDownIn(PreviewRect,false))
			InDrag = true;
		if (Event.current.type == EventType.MouseUp)
			InDrag = false;
		
		bool InDrag2 = Event.current.type == EventType.MouseDrag&&InDrag;
		//if (Event.current.type == EventType.MouseDrag&&Event.current.button == 0)
		if (InDrag2&&!Event.current.shift){
			Light1.GetComponent<Transform>().parent = null;
			if (Event.current.button == 1){
				previewCamAnchorScale += Event.current.delta.y*-0.02f;
				previewCamAnchorScale = Mathf.Min(Mathf.Max(previewCamAnchorScale,0.5f),2f);
			}else{
				previewCamAnchorTarget.transform.Rotate(Vector3.up * Event.current.delta.x, Space.World);//World x/World Up
				previewCamAnchorTarget.transform.Rotate(Vector3.left * -Event.current.delta.y, Space.Self);//Local z/
			}
		}
		GameObject l = new GameObject[]{Light1,Light2,Light3}[SelectedLight];
		if (InDrag2&&Event.current.shift){
			/*if (Event.current.button == 1){
				light1lookTarget.z += Event.current.delta.y*-0.02f;
				light1lookTarget.z = Mathf.Min(Mathf.Max(light1lookTarget.z,0f),7f);
			}else{
				light1lookTarget.y += Event.current.delta.x*-0.5f*(6.28f/360f);
				light1lookTarget.x += Event.current.delta.y*-0.5f*(6.28f/360f);
				Debug.Log(light1lookTarget.x);
			}*/
			l.GetComponent<Transform>().parent = previewLightAnchor.transform;
			/*if (Event.current.button == 1){
				previewCamAnchorScale += Event.current.delta.y*-0.02f;
				previewCamAnchorScale = Mathf.Min(Mathf.Max(previewCamAnchorScale,0.5f),2f);
			}else{
				previewCamAnchorTarget.transform.Rotate(Vector3.up * Event.current.delta.x, Space.World);//World x/World Up
				previewCamAnchorTarget.transform.Rotate(Vector3.left * -Event.current.delta.y, Space.Self);//Local z/
			}*/
			if (Event.current.button == 1){
				//previewLightAnchorScale += Event.current.delta.y*-0.02f;
				//previewLightAnchorScale = Mathf.Min(Mathf.Max(previewLightAnchorScale,0.5f),2f);
				if (!setLightScaleDir){
					setLightScaleDir = true;
					LightScaleDir = (l.GetComponent<Transform>().position).normalized;
				}
				l.GetComponent<Transform>().position += LightScaleDir * Event.current.delta.y*-0.02f;//Vector3.Lerp(Light1.GetComponent<Transform>().localPosition,new Vector3(0f,0f,0f),Event.current.delta.y);
			}else{
				camlookTarget.y += Event.current.delta.x*0.15f;
				camlookTarget.x += Event.current.delta.y*0.15f;
			}
		}
		if (!Event.current.shift){
			setLightScaleDir = false;
			camlookTarget = new Vector2(0f,0f);
			camlook = camlookTarget;
			l.GetComponent<Transform>().parent = previewParent.transform;;
			previewLightAnchorScale = 1f;
			previewLightAnchor.transform.localScale = new Vector3(previewLightAnchorScale,previewLightAnchorScale,previewLightAnchorScale);
			
		}
		/*if (InDrag2&&Event.current.button == 2){
			//previewCamAnchorScale += Event.current.delta.y*-0.02f;
			//previewCamAnchorScale = Mathf.Min(Mathf.Max(previewCamAnchorScale,0.5f),2f);
			//Yeah I'm a terrible programmer...
			camlookTarget.y += Event.current.delta.x*-0.5f;
			camlookTarget.x += Event.current.delta.y*-0.5f;
		}*/
		if (Event.current.type == EventType.ScrollWheel){
			previewCamAnchorScale += Event.current.delta.y*0.04f;
			previewCamAnchorScale = Mathf.Min(Mathf.Max(previewCamAnchorScale,0.5f),2f);
		}
		if (Event.current.type==EventType.Repaint){
			float NewScale = Mathf.Lerp(previewCamAnchor.transform.localScale.x,previewCamAnchorScale,deltaTime*20f);
			previewCamAnchor.transform.localScale = new Vector3(NewScale,NewScale,NewScale);
			
			NewScale = Mathf.Lerp(previewLightAnchor.transform.localScale.x,previewLightAnchorScale,deltaTime*20f);
			previewLightAnchor.transform.localScale = new Vector3(NewScale,NewScale,NewScale);
			
			previewCamAnchor.transform.rotation = Quaternion.Slerp(previewCamAnchor.transform.rotation,previewCamAnchorTarget.transform.rotation,deltaTime*20f);
			camlook = Vector2.Lerp(camlook,camlookTarget,deltaTime*20f);
			previewLightAnchor.transform.localRotation = Quaternion.Euler(0f,camlook.y,0f) * Quaternion.Euler(camlook.x,0f,0f);
			
			
			//light1look = Vector3.Lerp(light1look,light1lookTarget,deltaTime*15f);
			//Light1.GetComponent<Transform>().position = new Vector3(Mathf.Sin(light1look.y),Mathf.Sin(light1look.x),Mathf.Cos(light1look.y)) * light1look.z;
			///Light1.GetComponent<Transform>().position = new Vector3(Mathf.Cos(light1look.y)*Mathf.Sin(light1look.x),Mathf.Cos(light1look.x),Mathf.Sin(light1look.x)*Mathf.Sin(light1look.y)) * light1look.z;
			Color l1col = Light1.GetComponent<Light>().color;
			l1col = new Color(l1col.r/l1col.maxColorComponent,l1col.g/l1col.maxColorComponent,l1col.b/l1col.maxColorComponent,Light1.GetComponent<Light>().intensity);
				light1Mat.SetColor("_Color",l1col);
			l1col = Light2.GetComponent<Light>().color;
			l1col = new Color(l1col.r/l1col.maxColorComponent,l1col.g/l1col.maxColorComponent,l1col.b/l1col.maxColorComponent,Light2.GetComponent<Light>().intensity);
				light2Mat.SetColor("_Color",l1col);
			l1col = Light3.GetComponent<Light>().color;
			l1col = new Color(l1col.r/l1col.maxColorComponent,l1col.g/l1col.maxColorComponent,l1col.b/l1col.maxColorComponent,Light3.GetComponent<Light>().intensity);
				light3Mat.SetColor("_Color",l1col);
		}
		Light1.GetComponent<Transform>().LookAt(previewObject.GetComponent<Transform>());
		Light2.GetComponent<Transform>().LookAt(previewObject.GetComponent<Transform>());
		Light3.GetComponent<Transform>().LookAt(previewObject.GetComponent<Transform>());
		if (Light1.GetComponent<Transform>().position.magnitude>18f||Light1.GetComponent<Light>().type==LightType.Directional)
			Light1.GetComponent<Transform>().position = Light1.GetComponent<Transform>().position.normalized*18f;
		if (Light2.GetComponent<Transform>().position.magnitude>18f||Light2.GetComponent<Light>().type==LightType.Directional)
			Light2.GetComponent<Transform>().position = Light2.GetComponent<Transform>().position.normalized*18f;
		if (Light3.GetComponent<Transform>().position.magnitude>18f||Light3.GetComponent<Light>().type==LightType.Directional)
			Light3.GetComponent<Transform>().position = Light3.GetComponent<Transform>().position.normalized*18f;
		
		//if (!InDrag2){
			
			float dir = (Light1.GetComponent<Light>().type==LightType.Directional?1f:0.25f);
			if (Light1.transform.parent!=previewParent.transform&&Light1.transform.parent!=null)
				dir/=Light1.transform.parent.parent.localScale.x;
			Light1.transform.localScale = new Vector3(dir,dir,dir);
			dir = Light2.GetComponent<Light>().type==LightType.Directional?1f:0.25f;
			if (Light2.transform.parent!=previewParent.transform&&Light1.transform.parent!=null)
				dir/=Light2.transform.parent.parent.localScale.x;
			Light2.transform.localScale = new Vector3(dir,dir,dir);
			dir = Light3.GetComponent<Light>().type==LightType.Directional?1f:0.25f;
			if (Light3.transform.parent!=previewParent.transform&&Light1.transform.parent!=null)
				dir/=Light3.transform.parent.parent.localScale.x;
			Light3.transform.localScale = new Vector3(dir,dir,dir);
		//}
	}
	
	
	
	
    public Light[] lights;
	public bool[] LightIntensities;
    public MeshRenderer[] meshRenderers;
    public SkinnedMeshRenderer[] skinnedMeshRenderers;
	public bool[] renderers;	
	public bool[] skinnedrenderers;
	
	public Color AmbientLight;
	public Cubemap OldCubemap;
	#if UNITY_5
		public SphericalHarmonicsL2 OldAmbient;
		public DefaultReflectionMode OldCubemapSettings;
		public AmbientMode OldAmbientMode;
	#endif
	public Color OldAmbientColor;
	public float OldAmbientIntensity = 0f;
	public float OldReflectionIntensity = 0f;
	public void CamStart() {
//		Debug.Log(CurCube);
		#if UNITY_5
			OldAmbientIntensity = RenderSettings.ambientIntensity;
			RenderSettings.ambientIntensity = 1f;
			OldReflectionIntensity = RenderSettings.reflectionIntensity;
			RenderSettings.reflectionIntensity = 1f;
		#else
			OldAmbientColor = RenderSettings.ambientLight;
			RenderSettings.ambientLight = NewAmbientColor;
		#endif
		//
		
		
		lights = FindObjectsOfType(typeof(Light)) as Light[];
		LightIntensities = new bool[lights.Length];
		int i = 0;
		Light1.GetComponent<Light>().enabled = true;
		Light2.GetComponent<Light>().enabled = true;
		Light3.GetComponent<Light>().enabled = true;
		Light1.GetComponent<MeshRenderer>().enabled = true;
		Light2.GetComponent<MeshRenderer>().enabled = true;
		Light3.GetComponent<MeshRenderer>().enabled = true;
		previewObject.GetComponent<MeshRenderer>().enabled = true;
		previewBackdrop.GetComponent<MeshRenderer>().enabled = true;
		foreach(Light light in lights){
			if (light.gameObject.name!="Shader Sandwich Preview Light 1"&&light.gameObject.name!="Shader Sandwich Preview Light 2"&&light.gameObject.name!="Shader Sandwich Preview Light 3"&&light.gameObject.name!="Shader Sandwich Preview Light 3"){
				LightIntensities[i] = light.enabled;
				light.enabled = false;
//				Debug.Log(light.gameObject.name);
			}
			i+=1;
		}
		meshRenderers = FindObjectsOfType(typeof(MeshRenderer)) as MeshRenderer[];
		skinnedMeshRenderers = FindObjectsOfType(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer[];
		renderers = new bool[meshRenderers.Length];
		skinnedrenderers = new bool[skinnedMeshRenderers.Length];
		i = 0;
		foreach(MeshRenderer meshRenderer in meshRenderers){
			if (meshRenderer.gameObject.name!="Shader Sandwich Preview Object"&&meshRenderer.gameObject.name!="Shader Sandwich Preview Backdrop"){
				renderers[i] = meshRenderer.enabled;
				meshRenderer.enabled = false;
			}
			else{
				renderers[i] = false;
				meshRenderer.enabled = true;
			}
			i+=1;
		}
		i = 0;
		foreach(SkinnedMeshRenderer meshRenderer in skinnedMeshRenderers){
			if (meshRenderer.gameObject.name!="Shader Sandwich Preview Object"&&meshRenderer.gameObject.name!="Shader Sandwich Preview Backdrop"){
				skinnedrenderers[i] = meshRenderer.enabled;
				meshRenderer.enabled = false;
			}
			else{
				skinnedrenderers[i] = false;
				meshRenderer.enabled = true;
			}
			i+=1;
		}
		#if UNITY_5
		OldCubemap = RenderSettings.customReflection;
		OldCubemapSettings = RenderSettings.defaultReflectionMode;
		RenderSettings.customReflection = CurCube;
		RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
		
		OldAmbient = RenderSettings.ambientProbe;
		RenderSettings.ambientProbe = NewAmbient;
		OldAmbientMode = RenderSettings.ambientMode;
		RenderSettings.ambientMode = AmbientMode.Skybox;
		//#else
		//OldAmbientColor = RenderSettings.ambientLight;
		//RenderSettings.ambientLight = NewAmbientColor;
		#endif

		//RenderSettings.ambientProbe = new SphericalHarmonicsL2();
    }
    public void CamEnd() {
		
		#if UNITY_5
		RenderSettings.ambientIntensity = OldAmbientIntensity;
		RenderSettings.reflectionIntensity = OldReflectionIntensity;
		RenderSettings.ambientMode = OldAmbientMode;
		#else
		RenderSettings.ambientLight = OldAmbientColor;
		#endif
		//RenderSettings.ambientLight = AmbientLight;
		int i = 0;
		Light1.GetComponent<Light>().enabled = false;
		Light2.GetComponent<Light>().enabled = false;
		Light3.GetComponent<Light>().enabled = false;
		Light1.GetComponent<MeshRenderer>().enabled = false;
		Light2.GetComponent<MeshRenderer>().enabled = false;
		Light3.GetComponent<MeshRenderer>().enabled = false;
		previewObject.GetComponent<MeshRenderer>().enabled = false;
		previewBackdrop.GetComponent<MeshRenderer>().enabled = false;
		foreach(Light light in lights){
			light.enabled = LightIntensities[i];
			
			i+=1;
		}
		i=0;
		foreach(MeshRenderer meshRenderer in meshRenderers){
			meshRenderer.enabled = renderers[i];
			i+=1;
		}
		i=0;
		foreach(SkinnedMeshRenderer meshRenderer in skinnedMeshRenderers){
			meshRenderer.enabled = skinnedrenderers[i];
			i+=1;
		}
		#if UNITY_5
		RenderSettings.customReflection = OldCubemap;
		RenderSettings.ambientProbe = OldAmbient;
		RenderSettings.defaultReflectionMode = OldCubemapSettings;
		#endif
    }
	void Nothing(){}
}
}