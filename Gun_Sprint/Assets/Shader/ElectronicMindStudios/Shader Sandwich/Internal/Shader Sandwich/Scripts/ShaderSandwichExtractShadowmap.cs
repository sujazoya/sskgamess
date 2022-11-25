using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

[ExecuteInEditMode]
public class ShaderSandwichExtractShadowmap : MonoBehaviour {

	public RenderTexture shadowMap;

	void Start(){
		RenderTargetIdentifier shadowmap = BuiltinRenderTextureType.CurrentActive;
		shadowMap = new RenderTexture(4096, 4096, 0,RenderTextureFormat.RFloat);
		CommandBuffer cb = new CommandBuffer();
		
		cb.SetShadowSamplingMode(shadowmap, ShadowSamplingMode.RawDepth);

		cb.Blit(shadowmap, new RenderTargetIdentifier(shadowMap));
		
		GetComponent<Light>().AddCommandBuffer(LightEvent.AfterShadowMap, cb);
	}
	void Update(){
		Shader.SetGlobalTexture("ShaderSandwichShadowMapIfOnlyUnityGaveUsAccessToTheDirectionalShadowMapThatItAlreadyHasThatWouldBeNiceHuh",shadowMap);
	}
}
