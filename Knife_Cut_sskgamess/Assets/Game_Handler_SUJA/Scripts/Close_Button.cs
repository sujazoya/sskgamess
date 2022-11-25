using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Close_Button : MonoBehaviour
{
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;
    [SerializeField] Button closeButton;
    [SerializeField] GameObject areYouSure;
    bool yesOpened;
    // Start is called before the first frame update
    void Start()
    {

        //closeButton = GetComponent<Button>();
        if (closeButton) { closeButton.onClick.AddListener(OnPressCloseButton); }      
        yesButton.onClick.AddListener(Yes);
        noButton.onClick.AddListener(No);
        areYouSure.SetActive(false);
    }
    public void Quit()
    {
        areYouSure.SetActive(true);
        yesOpened = true;
       
    }
    int buttonPress;
    void OnPressCloseButton()
    {
        if (buttonPress == 0)
        {
            StartCoroutine(ContineueNo());
        }
        else
        {
            areYouSure.SetActive(true);
            yesOpened = true;
            buttonPress = 0;
        }
       
    }
   
    void Yes()
    {
        Application.Quit();
    }
    void No()
    {       
        areYouSure.SetActive(false);
        yesOpened = false;        
    }
    IEnumerator ContineueNo()
    {
        if (AdmobAdmanager.Instance.interstitialAd[AdmobAdmanager.Instance.CurrentIntIndex()].IsLoaded())
        {
            AdmobAdmanager.Instance.ShowInterstitial_Instant();
        }
        else
        {
            buttonPress++;
            areYouSure.SetActive(true);
            yesOpened = true;
        }
        yield return new WaitForSeconds(1.2f);
        buttonPress++;
        areYouSure.SetActive(true);
        yesOpened = true;

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnPressCloseButton();
        }

    }
}
