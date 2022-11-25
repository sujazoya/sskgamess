using UnityEngine;
using System.Collections;

public class LookAtCam : MonoBehaviour {

    public bool reverseDir = true;

    void Update()
    {
        if (reverseDir)
        {
            transform.LookAt(Camera.main.transform);
            transform.rotation = transform.rotation * Quaternion.Euler(new Vector3(0, 180, 0));
        }
        else
        {
            transform.LookAt(Camera.main.transform);
        }
    }

}
