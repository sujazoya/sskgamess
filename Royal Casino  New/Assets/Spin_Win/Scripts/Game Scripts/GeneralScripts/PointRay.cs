using UnityEngine;
using System.Collections;
/// <summary>
/// This is a Ray to grab the point value of the cube on the Wheel of Triumph
/// </summary>
public class PointRay : MonoBehaviour 
{
    public int pointVal;
    private GameObject hitObj;
    [HideInInspector]
    public RaycastHit hit; 

	public int GetScore()
	{	
		Ray ray = new Ray(gameObject.transform.position, Vector3.forward);
		hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit, 100.0f))
		{
			hitObj = hit.collider.gameObject;
			pointVal = hitObj.GetComponent<PointValue>().pointVal;
		}
		return pointVal;

	}
   
}
