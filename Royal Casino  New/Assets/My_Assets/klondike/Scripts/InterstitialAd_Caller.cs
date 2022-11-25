using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterstitialAd_Caller : MonoBehaviour
{
    [SerializeField] GameObject popupPanel;
    [SerializeField] Animator popAnim;
    [SerializeField] Text timerText;
    [SerializeField]float intervalTime=250f;
    private void Awake()
    {
        popAnim.enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
        StartCoroutine(ShowRewardedInterstitialStarting_Popup());
    }
   
    bool showRewardIntAd=true;
    IEnumerator ShowRewardedInterstitialStarting_Popup()
    {
        yield return new WaitForSeconds(intervalTime);
        popupPanel.SetActive(true);
        popAnim.enabled = true;
        timerText.text = "05";
        yield return new WaitForSeconds(0.5f);
        popAnim.enabled = false;
        showRewardIntAd = true;
        int t = 5;
        while (t > 0)
        {
            t--;
            timerText.text = ": "
                + t;
            yield return new WaitForSeconds(1);
        }
        popAnim.enabled = true;
        if (showRewardIntAd)
        {
            yield return new WaitForSeconds(0.5f);
            ShowAd();
        }
        StartCoroutine(ShowRewardedInterstitialStarting_Popup());
    }
    void ShowAd()
    {
        popAnim.enabled = false;
        popupPanel.SetActive(false);
        AdmobAdmanager.Instance.ShowInterstitial();
        // FacebookAdManager.Instance.ShowInterstitial();
    }
}
