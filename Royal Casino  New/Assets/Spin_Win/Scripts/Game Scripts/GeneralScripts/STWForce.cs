using UnityEngine;
using System.Collections;
/// <summary>
/// Spin to WIn Wheel Force
/// </summary>
public class STWForce : ApplyForce
{
	protected bool firstClick = true;
	protected Vector3 originalEuler;
	protected Vector3 rotationOffset;
	protected float angle;

    public override void Start()
    {
        base.Start();
		originalRotation = transform.rotation;
		originalEuler = transform.rotation.eulerAngles;
    }

    protected override void Dragging()
    {
        originalMousePosition = Input.mousePosition;

        if (!isSpinning)
        {
            
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 10000f))
            {
                originalCollisionPosition = hit.point;
                LookAtThis(hit.point);
            }
        }
    }

    protected override void Release()
    {
        Vector3 rotationForce = Input.mousePosition - originalMousePosition;
        float distance = Vector3.Distance(Input.mousePosition, originalMousePosition); 
        rotationForce = new Vector3(rotationForce.x * forceAmount, 0, rotationForce.y * forceAmount);
        Debug.Log("FORCE: " +rotationForce + "DIST:" + distance);
        if (distance <= 40)
        {
            // did this so even if you spin very little the wheel still goes arround atleast once. 
            rotationForce = new Vector3(-5850, 0,400);
            Debug.Log("ALT");
            spinCount++;
            hasSpun = true;
            GetComponent<Rigidbody>().AddForceAtPosition(rotationForce, originalCollisionPosition);
            
        }
        else
        {
            spinCount++;
            hasSpun = true;
            GetComponent<Rigidbody>().AddForceAtPosition(rotationForce, originalCollisionPosition);
            MusicManager.PauseMusic(0.2f);
        }
       
    }

    private void LookAtThis(Vector3 point)
    {
		point.x -= transform.position.x;
		point.y = 0;
		point.z -= transform.position.z;

		if (firstClick)
		{
			rotationOffset = point;
			firstClick = false;
		}

		transform.rotation = Quaternion.FromToRotation(rotationOffset, point); 
    }
}
