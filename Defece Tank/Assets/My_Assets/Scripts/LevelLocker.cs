using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelLocker : MonoBehaviour
{
    Button myButton;
    [SerializeField] int levelNum;
    GameObject lock_;
    // Start is called before the first frame update
    void Start()
    {
        lock_ = transform.Find("lock").gameObject;
        myButton = GetComponent<Button>();
        CheckLevelStatus();
    }
    void CheckLevelStatus()
    {
        if (!GameManager_Defence.Instance.testBuild)
        {
            if (Game.PassedLevel < levelNum)
            {
                lock_.SetActive(true);
                myButton.interactable = false;
            }
            else
            {
                lock_.SetActive(false);
                myButton.interactable = true;
            }
        }
        else
        {
            lock_.SetActive(false);
        }      
            
    }
   
}
