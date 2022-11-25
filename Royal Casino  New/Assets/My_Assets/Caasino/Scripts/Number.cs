using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Number : MonoBehaviour
{
    Casino casino;
    int myNumber;
    // Start is called before the first frame update
    void Start()
    {
        casino = FindObjectOfType<Casino>();
        myNumber = int.Parse(transform.name);
    }
    
    //private void OnTriggerEnter(Collider other)
    //{
       
    //}
    private void OnTriggerStay(Collider other)
    {
        if (casino.casinoStatus == Casino.CasinoStatus.Stopped)
        {
            if (other.tag == "pin")
            {
                casino.ShowResult(myNumber);
            }

        }
    }
}
