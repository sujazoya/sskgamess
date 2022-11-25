using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Positions : MonoBehaviour
{
    public List<Transform> pos=new List<Transform>();
    public static Positions instance;
    int indexR;
    private void Awake()
    {
        instance = this;
        foreach (Transform item in transform)
        {
            pos.Add(item);

        }
       
    }
    public Vector3 NewPos()
    {
        indexR++;
        if (indexR >= pos.Count) { indexR = 0; }
        return pos[indexR].transform.position;       
    }
}
