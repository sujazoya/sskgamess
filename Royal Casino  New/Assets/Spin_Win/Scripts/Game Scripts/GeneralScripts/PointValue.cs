using UnityEngine;
using System.Collections;
/// <summary>
/// Assigns a point value to the cube and also has a raycast for sounds in one of the scenes. 
/// </summary>
public class PointValue : MonoBehaviour 
{
    public int pointVal;

    void Update()
    {
        Ray ray = new Ray(gameObject.transform.position, Vector3.back);
		RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            if (hit.collider.tag == "Cube")
            {
                GetComponent<AudioSource>().Play();
            } 
        }
        Debug.DrawRay(ray.origin, ray.direction); 
    } 
}
