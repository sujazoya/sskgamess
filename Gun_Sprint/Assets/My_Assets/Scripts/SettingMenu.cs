using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    [SerializeField] Button[] settingButtons;
    [SerializeField] Button closeButton;
    [SerializeField] GameObject setting_panel;
    // Start is called before the first frame update
    void Start()
    {
        if (settingButtons.Length > 0)
        {
            for (int i = 0; i < settingButtons.Length; i++)
            {
                settingButtons[i].onClick.AddListener(OpenSetting);
            }
        }       
        if (closeButton)   { closeButton.onClick.AddListener(CloseSetting);  }
        CloseSetting();
    }
    public void OpenSetting()
    {
        setting_panel.SetActive(true);
    }
    void CloseSetting()
    {
        setting_panel.SetActive(false);
    }
   
}
