using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot_Piece : MonoBehaviour
{    
    //public bool fixRotation;
    float yVelocity = 0.0f;
    float fixTime;
    SlotManager manager;
    public enum GotItem
    {
        None, Bonus, Wild, Bar, Seven
    }
    public GotItem gotItem;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<SlotManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (fixRotation)
        //{
        //    FixRotation();
        //    fixTime += Time.deltaTime;
        //}
        //if (fixTime > 2f)
        //{
        //    fixRotation = false;
        //    fixTime = 0;
        //}
    }

    public void FixRotation()
    {
        
        switch (gotItem)
        {
            case GotItem.Bonus:
                manager.Bonus+=1;
                FoxRot(180);                
                break;
            case GotItem.Bar:
                manager.Bar += 1;
                FoxRot(0);                
                break;
            case GotItem.Seven:
                manager.Seven += 1;
                FoxRot(90);
                break;
            case GotItem.Wild:
                manager.Wild += 1;
                FoxRot(270);
                break;
        }
      
    }
    void FoxRot(float z)
    {
        float newPosition = Mathf.SmoothDamp(transform.position.z, z, ref yVelocity, 2f);
        transform.rotation = Quaternion.Euler(0, -90f, z);        
        //if (transform.rotation.z == z)
        //{
        //    fixRotation = false;
        //}
    }
    public void ResetRotation()
    {
        transform.rotation = Quaternion.Euler(0, -90f, 0);
    }
}
