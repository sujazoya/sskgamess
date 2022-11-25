using UnityEngine;
using System.Collections;
/// <summary>
/// Controls the obj with the win lose Textures. 
/// </summary>
public class WinLoseObj : MonoBehaviour
{
    //public GUITexture win;
    //public GUITexture lose; 
    //public UnityEngine.UI.Image win;
    public UnityEngine.UI.Text lose;

    void Start()
    {
    }

    void Update()
    {
    }

    public void WinLose(int w)
    {
        if (w == 0)
        {
            //win.enabled = true; 
        }
        if (w == 1)
        {
            lose.enabled = true; 
        } 
    }

    internal void Reset()
    {
        //win.enabled = false; 
        lose.enabled = false; 

    }
}
