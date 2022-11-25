using UnityEngine;
using System.Collections;
/// <summary>
/// CamerRay casts a ray to the mouse point i put this here for people to create menus in 3Dspace. no need to use onGUI calls with this and you can make any object a button as long at it has a variant of 
/// the Button class on it. I put an on off switch for moments where you dont need this to be on. 
/// </summary>
public class CameraRay : MonoBehaviour 
{
    public int onOffSwitch = 0;
    private SpinButton button;
    public Camera other; 
	
	void Start () 
    {
	
	}
	
	
	void Update () 
    {
        if (onOffSwitch == 1)
        {
           
            Ray ray = other.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            Debug.DrawRay(ray.origin, ray.direction * 100); 
            if(Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject.tag == "button" && Input.GetMouseButton(0))
                {
                    //Debug.Log("HIT" + hit.collider.gameObject.name);
                    hit.collider.gameObject.GetComponent<SpinButton>().pressed = 1;
                }
            } 
        } 
	}
}
