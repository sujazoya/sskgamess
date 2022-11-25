using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager_1 : MonoBehaviour
{
    public  float slowDownFactor = 0.05f;
    public  float slowDownLenth = 2f;

    public static TimeManager_1 Instance;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Time.timeScale += (1f / slowDownLenth)*Time.unscaledDeltaTime;
        //Time.timeScale = Mathf.Clamp(Time.timeScale, 0, 1);
    }
    public void ToggleSlowMo(bool value)
    {
        if (value == true)
        {
            Time.timeScale = slowDownFactor;
            //Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
        else
        {
            Time.timeScale = 1;
            //Time.fixedDeltaTime = 1;
        }
        
    }
}
