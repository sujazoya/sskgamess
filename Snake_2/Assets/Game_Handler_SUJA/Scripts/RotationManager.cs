using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationManager : MonoBehaviour
{
    [Range(-5,5)]
    [SerializeField] float speed;
    [SerializeField] bool x;
    [SerializeField] bool y;
    [SerializeField] bool z;
    int index;
    // Start is called before the first frame update
    void Start()
    {
        index = Random.Range(0, 1);
        if (index == 0)
        {
            speed = 2;
        }
        else
        {
            speed = -2;
        }
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
