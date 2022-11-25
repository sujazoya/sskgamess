using UnityEngine;
using System.Collections;
/// <summary>
/// Base class for spinning the wheel if you so choose to make a wheel spinning game. May add more funcitonality in later updates. 
/// Has some follow point functionality. 
/// </summary>
//[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class ApplyForce : MonoBehaviour
{
    
    public int forceAmount = 20;
    public GameObject arrow;
    public bool startOn = false;

    //public float scale; 
    [HideInInspector]
    public Vector3 originalMousePosition = Vector3.zero;
    [HideInInspector]
    public Vector3 originalCollisionPosition = Vector3.zero;
    [HideInInspector]
    public Vector3 radLoc; 
    [HideInInspector]
    public Vector3 lastPoint; 
    [HideInInspector]
    public bool hasSpun = false;
    [HideInInspector]
    public int spinCount;
    [HideInInspector]
    public int spinAgain = 0;

    protected Quaternion originalRotation;

    public bool isSpinning { get; set; }

    #region Unity Methods
    public virtual void Start()
    {
        isSpinning = false;

        if (startOn)
        {
            enabled = true;
        }
        else
        {
            enabled = false;
        }
    }

    public virtual void Update()
    {
        
		if (isSpinning)
			return;

		if (Input.GetMouseButton(0))
		{
            arrow.SetActive(false); 
			Dragging();
			return;
		}
       
        if (Input.GetMouseButtonUp(0))
        {
            Release();
        }

        if (GetComponent<Rigidbody>().angularVelocity.magnitude >= 0.1)
        {
            isSpinning = true;
        }
        

    }
    #endregion

    public virtual void Reset()
    {
        transform.rotation = originalRotation;
    }

    public virtual void FixLook() {}
 
    protected virtual void Dragging() { }

	protected virtual void Release(){}

}
