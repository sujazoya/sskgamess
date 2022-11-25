using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAssigner : MonoBehaviour
{
    public RenderMode renderMode = RenderMode.ScreenSpaceCamera;
    // Start is called before the first frame update
    void Start()
    {
        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.renderMode = renderMode;
        canvas.worldCamera = Camera.main;
    }
}
