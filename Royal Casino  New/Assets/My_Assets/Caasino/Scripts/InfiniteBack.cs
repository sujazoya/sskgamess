using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteBack : MonoBehaviour
{
   
    [SerializeReference] float speed;
    float x;
    float y;
	void Start()
    {
        x = transform.position.x;
        y = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.z >= -57)
        {
            transform.Translate(Vector3.right* speed * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3(x, y, 130);
        }
        
    }
	

}
