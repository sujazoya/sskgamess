using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu_Spin : MonoBehaviour
{
    public Text balanceText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (balanceText) { balanceText.text = Game.TotalCoins.ToString(); }
    }
}
