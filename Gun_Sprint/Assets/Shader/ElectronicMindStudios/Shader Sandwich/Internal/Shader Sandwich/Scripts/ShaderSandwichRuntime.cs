using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
	using UnityEditor;
#endif

[ExecuteInEditMode]
[AddComponentMenu("Shader Sandwich/Base/Shader Sandwich Runtime")]
public class ShaderSandwichRuntime : MonoBehaviour {
	public bool EnablePerlinNoise = true;
	public Texture2D PerlinNoise;
	public Texture2D PerlinNoiseLinear;
	public bool ForceRefresh = false;
	void Awake () {
		if (EnablePerlinNoise){
			#if UNITY_EDITOR
				PerlinNoise = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/ElectronicMindStudios/Shader Sandwich/Internal/Shader Sandwich/Textures/ShaderSandwichPerlinNoise.png",typeof(Texture2D));
				PerlinNoiseLinear = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/ElectronicMindStudios/Shader Sandwich/Internal/Shader Sandwich/Textures/ShaderSandwichPerlinNoiseLinear.png",typeof(Texture2D));
			#endif
		
			Shader.SetGlobalTexture("_ShaderSandwichPerlinTexture",PerlinNoise);
			Shader.SetGlobalTexture("_ShaderSandwichPerlinTextureLinear",PerlinNoiseLinear);
		}
	}
	void DoStuff(){
		if (EnablePerlinNoise){
			#if UNITY_EDITOR
				if (!Application.isPlaying){
					if (PerlinNoise==null)
					PerlinNoise = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/ElectronicMindStudios/Shader Sandwich/Internal/Shader Sandwich/Textures/ShaderSandwichPerlinNoise.png",typeof(Texture2D));
					PerlinNoiseLinear = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/ElectronicMindStudios/Shader Sandwich/Internal/Shader Sandwich/Textures/ShaderSandwichPerlinNoiseLinear.png",typeof(Texture2D));
					ForceRefresh = true;
				}
			#endif
			//if (ForceRefresh)
			Shader.SetGlobalTexture("_ShaderSandwichPerlinTexture",PerlinNoise);
			Shader.SetGlobalTexture("_ShaderSandwichPerlinTextureLinear",PerlinNoiseLinear);
		}
	
		/*Camera cam = gameObject.GetComponent<Camera>();
		if (cam==null)
			cam = Camera.main;
		
		if (cam!=null){
			Transform camtr = cam.transform;
			float camNear = cam.nearClipPlane;
			float camFar = cam.farClipPlane;
			float camFov = cam.fieldOfView;
			float camAspect = cam.aspect;

            Matrix4x4 frustumCorners = Matrix4x4.identity;
            Matrix4x4 frustumCornersNormalized = Matrix4x4.identity;

			float fovWHalf = camFov * 0.5f;

			Vector3 toRight = camtr.right * camNear * Mathf.Tan (fovWHalf * Mathf.Deg2Rad) * camAspect;
			Vector3 toTop = camtr.up * camNear * Mathf.Tan (fovWHalf * Mathf.Deg2Rad);

			Vector3 topLeft = (camtr.forward * camNear - toRight + toTop);
			float camScale = topLeft.magnitude * camFar/camNear;
			float centerScale = camNear;

           
			

			Vector3 topRight = (camtr.forward * camNear + toRight + toTop);
            
			

			Vector3 bottomRight = (camtr.forward * camNear + toRight - toTop);
           
			

			Vector3 bottomLeft = (camtr.forward * camNear - toRight - toTop);



            frustumCornersNormalized.SetRow (0, topLeft/centerScale);
            frustumCornersNormalized.SetRow (1, topRight/centerScale);
            frustumCornersNormalized.SetRow (2, bottomRight/centerScale);
            frustumCornersNormalized.SetRow (3, bottomLeft/centerScale);
			
			topLeft.Normalize();
			topRight.Normalize();
			bottomRight.Normalize();
			bottomLeft.Normalize();
			
			topLeft *= camScale;
			topRight *= camScale;
			bottomRight *= camScale;
			bottomLeft *= camScale;

            frustumCorners.SetRow (0, topLeft);
            frustumCorners.SetRow (1, topRight);
            frustumCorners.SetRow (2, bottomRight);
            frustumCorners.SetRow (3, bottomLeft);

			//var camPos= camtr.position;
            Shader.SetGlobalMatrix ("SS_FrustumCornersWS", frustumCorners);
            Shader.SetGlobalMatrix ("SS_FrustumCornersWSNormalized", frustumCornersNormalized);
		}	*/
		ForceRefresh = false;
	}
	public void MyPreRender(Camera cam){
		Shader.SetGlobalMatrix ("SS_MATRIX_I_VP", (GL.GetGPUProjectionMatrix(cam.projectionMatrix,RenderTexture.active!=null) * cam.worldToCameraMatrix).inverse);
	}

	public void OnEnable(){
		// register the callback when enabling object
		Camera.onPreRender += MyPreRender;
	}

	public void OnDisable(){
		// remove the callback when disabling object
		Camera.onPreRender -= MyPreRender;
	}
	void Update () {
		DoStuff();
	}
	//void OnPreRender() {
	//	DoStuff();
    //}
}
