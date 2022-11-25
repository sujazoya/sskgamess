using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Level_Button : MonoBehaviour
{
    private Text levelText;
    [SerializeField] int levelNum;
    private GameObject lock_;
    Button button;
    // Start is called before the first frame update
    void Start()
    {
        levelText = transform.Find("Num").GetComponent<Text>();
        levelText.text = levelNum.ToString();
        lock_ = transform.Find("lock").gameObject;
        button = GetComponent<Button>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        CheckLevelStatus();
    }
    void CheckLevelStatus()
    {
        if (GoogleSheetHandler.test_build == false)
        {
            if (levelNum <= Game.PassedLevel)
            {
                lock_.SetActive(false);
            }
            else
            {
                button.interactable = false;
                lock_.SetActive(true);
            }
        }
        else
        {
            lock_.SetActive(false);
        }
    }  
    void OnClick()
    {
        GameManager_Sprint.Instance.CreateLevel(levelNum-1);
    }
}
