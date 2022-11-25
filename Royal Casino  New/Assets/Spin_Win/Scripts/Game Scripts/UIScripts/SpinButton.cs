using UnityEngine;
using System.Collections;

public abstract class SpinButton : MonoBehaviour 
{
    public int pressed = 0;

    public GameManager gameManager; 

    public virtual void Update() 
    {
        
        if (pressed == 1)
        {
            Message(); 
        } 
	}

    public virtual void Message()
    {
        pressed = 0;
        gameObject.GetComponent<Renderer>().enabled = false; 
    }
}
