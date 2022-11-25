using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion1 : MonoBehaviour {

 
     float explosionForce = 200f;
    // float explosionRadius = 5f;  

    // Use this for initialization
    void Awake() {

       
        createPiece();
    }    
    // Update is called once per frame
    void Update() {

    }
   

    void createPiece() {

        //create piece      
        int childCount = transform.childCount;
        for (int t = childCount - 1; t >= 0; t--)
        {

            var child = transform.GetChild(t).gameObject;
            // piece = GameObject.CreatePrimitive(PrimitiveType.Cube);


            //set piece position and scale
           // child.transform.position = transform.position;// + new Vector3(cubeSize * x, cubeSize * y, cubeSize * z) - cubesPivot;
           // child.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

            //add rigidbody and set mass
            child.AddComponent<Rigidbody>();
            child.GetComponent<Rigidbody>().mass = 1;
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb != null)
            {
                //add explosion force to this body with given parameters
                rb.AddExplosionForce(explosionForce, transform.position,30,1f);
            }
            child.transform.parent = null;
            Destroy(child.gameObject, 5);
            Destroy(transform.gameObject,2f);


        }
      
    }

}
