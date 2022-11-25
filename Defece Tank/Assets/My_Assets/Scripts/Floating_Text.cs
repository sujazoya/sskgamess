using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating_Text : MonoBehaviour
{
    public float destroyTime = 3f;
    Vector3 offset = new Vector3(0, 2, 0);
    Vector3 RandomizeIntenct= new Vector3(0.5f, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyTime);
        transform.localPosition += offset;
        transform.localPosition += new Vector3(Random.Range(-RandomizeIntenct.x, RandomizeIntenct.x),
            Random.Range(-RandomizeIntenct.y, RandomizeIntenct.y),
            Random.Range(-RandomizeIntenct.z, RandomizeIntenct.z));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
