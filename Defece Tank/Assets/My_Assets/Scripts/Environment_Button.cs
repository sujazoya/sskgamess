using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Environment_Button : MonoBehaviour
{
    Button button_;
    [SerializeField] int envIndex;
    GameObject lock_;
    [SerializeField] int envValue;
    [SerializeField] Button buyButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        button_ = GetComponent<Button>();
        lock_= transform.Find("lock").gameObject;
        CheckEmvIndex();
        if (buyButton) { buyButton.onClick.AddListener(BuyEnv); }
    }
   void CheckEmvIndex()
    {
        if (!GameManager_Defence.Instance.testBuild)
        {
            if (Game.AvailableEnvironment < envIndex)
            {
                lock_.SetActive(true);
                button_.interactable = false;
            }
            else
            {
                lock_.SetActive(false);
                button_.interactable = true;
            }
        }
        else
        {
            lock_.SetActive(false);
        }
    }
     void BuyEnv()
    {
        if(Game.TotalCoins>= envValue)
        {
            Game.TotalCoins -= envValue;
            Game.AvailableEnvironment = envIndex;
            lock_.SetActive(false);
            button_.interactable = true;
        }
        else
        {
            MessegeManager.insance.ShowPopup("You Don't Have Enogh Coins");
        }
    }

}
