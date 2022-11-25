using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fan : MonoBehaviour
{
    [Range(-100,100)]
    public float speed;
    [SerializeField] bool x;
    [SerializeField] bool y;
    [SerializeField] bool z;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (x)
        {
            transform.Rotate(speed, 0, 0);
        }else if (y)
        {
            transform.Rotate(0, speed, 0);
        }
        else if (z)
        {
            transform.Rotate(0, 0, speed);
        }     
                          
    }
}
