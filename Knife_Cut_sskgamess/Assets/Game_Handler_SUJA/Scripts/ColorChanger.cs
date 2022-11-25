using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ColorChanger : MonoBehaviour {

    ////private Sprite sprite;
    public Color32[] colors;   
   
    public float period = 5f;
    private int lastIndex;
    float t = 0;
    // Use this for initialization
    void Start ()
    {
        //sprite = transform.GetComponent<Sprite>();
        len = colors.Length;
        interval = period;
        interval -= 0.1f;
    }
    int len;
    float interval;
    

	void Update ()
    {
        if (colors == null) return;
        if (len <= 0) return;



      
            //if (lastIndex >= colors.Length)
            //{
            //    lastIndex = 0;
            //}
            if (transform.GetComponent<Image>())
            {
                //transform.GetComponent<Image>().color = colors[lastIndex];

                Image healthImage = transform.GetComponent<Image>();
                Color newColor = colors[lastIndex];
                newColor.a = 1;
                //healthImage.color = newColor;
                healthImage.color = Color.Lerp(healthImage.color, newColor, period*Time.deltaTime);
                t = Mathf.Lerp(t, period, period * Time.deltaTime);
                if (t > interval)
                {
                    t = 0;
                    lastIndex++;
                    lastIndex = (lastIndex >= len) ? 0 : lastIndex;
                }
            //}
            // execute block of code here
        }


        //counter++;
    }
}
