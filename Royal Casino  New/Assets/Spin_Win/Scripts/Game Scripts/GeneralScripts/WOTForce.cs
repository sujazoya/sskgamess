using UnityEngine;
using System.Collections;
/// <summary>
/// Wheel of Triumphs wheel force
/// </summary>
public class WOTForce : ApplyForce
{
    private bool firstClick = true;

    public int first = 0; 

    protected Vector3 rotationOffset;
    
    public float t;
    

    public override void Start()
    {
        base.Start();
        originalRotation = transform.rotation;
        
    }
    
    protected override void Dragging()
    {
        
        if (!isSpinning)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000f))
            {
                originalCollisionPosition = hit.point;
                
                LookAtThis(hit.point); 

            }
        }
    }

    protected override void Release()
    {
         
        Vector3 rotationForce = originalMousePosition - Input.mousePosition;
        float y = rotationForce.y;
        if (y > 0)
        {
            spinCount++;
            hasSpun = true;
            GetComponent<Rigidbody>().AddForceAtPosition(-rotationForce * forceAmount, originalCollisionPosition);
        }
        else
        {
            spinCount++;
            hasSpun = true;
            GetComponent<Rigidbody>().AddForceAtPosition(rotationForce * forceAmount, originalCollisionPosition);
            MusicManager.PauseMusic(0.2f);
        }
        lastPoint = Vector3.zero;
    }

    /// <summary>
    /// Makes the object rotate with the mouse movement. 
    /// </summary>
    /// <param name="point"></param>
    private void LookAtThis(Vector3 point)
    {
        point.x = 0;
        point.y -= transform.position.y;
        point.z -= transform.position.z;

        if (firstClick)
        {
            rotationOffset = point;
            originalMousePosition = Input.mousePosition;
            firstClick = false;
        }
        Quaternion blarg = Quaternion.FromToRotation(rotationOffset, point);
        transform.rotation = Quaternion.Slerp(transform.rotation, blarg, t);
    }
    
}
