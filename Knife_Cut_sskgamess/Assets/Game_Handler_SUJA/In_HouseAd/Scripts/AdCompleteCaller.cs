using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdCompleteCaller : MonoBehaviour
{
    [SerializeField] InHouse_Ad_Handler inHouse_Ad;   
    public void CallAdComplete()
    {
        inHouse_Ad.CallAddComplete();
    }
}
