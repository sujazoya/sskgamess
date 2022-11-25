using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_Model : MonoBehaviour
{
    public AudioClip shootClip;
    public enum GunType
    {
        Normal,SemiAuto,Auto
    }
    public GunType gunType;

    // Start is called before the first frame update
    void Start()
    {
        
    }
   
}
