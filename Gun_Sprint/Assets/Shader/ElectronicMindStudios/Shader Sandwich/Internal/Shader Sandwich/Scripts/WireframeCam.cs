using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class WireframeCam : MonoBehaviour {

	// Use this for initialization
    void Update() {
		//Debug.Log("lolrofl");
	}
    void OnPreRender() {
		//Debug.Log("asd");
        GL.wireframe = true;
		GL.Color(new Color(0,0,0,0));
    }
    void OnPostRender() {
        GL.wireframe = false;
    }
}
