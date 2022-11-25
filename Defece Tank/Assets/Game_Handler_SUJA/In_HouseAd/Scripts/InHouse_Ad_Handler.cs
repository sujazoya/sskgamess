using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.Networking;
using UnityEngine.UI;
[System.Serializable]
public class AdItems
{
    public GameObject body;
    public Image logo;
    public Image image1;
    public Image image2;
    public Image image3;
    public Image image4;
    public Text app_name;
    public Text description;
    public Button install_Button;
    public Button close_Button;
    public Button cancel_Button;
    public GameObject loading;
    public Text timerText;
    public RawImage Video_Image;
    public GameObject enableOnPlay;
}
public class InHouse_Ad_Handler : MonoBehaviour
{
    [SerializeField] AdItems items;
   
    float videoLenth;
    bool adCompleted;  
    InHouseAdManager inHouseAdManager;
    private void Awake()
    {
        if (items.body) { items.body.SetActive(false); }
        inHouseAdManager = GetComponent<InHouseAdManager>();
        adCompleted = false;
        if (items.enableOnPlay) { items.enableOnPlay.SetActive(false); }
    }
    void Start()
    {
        InHouseAdManager.onAdCompleted += OnAdCompleted;
        InHouseAdManager.onAdClosed += OnAdClosed;
        InHouseAdManager.onAdCancelled += OnAdCancelled;
        items.close_Button.onClick.AddListener(CloseAd);
        StartCoroutine(CheckForApi());
    }
    IEnumerator CheckForApi()
    {
        yield return new WaitUntil(() => inHouseAdManager.apiConfirmed);
        CreateNewAd();
        yield return new WaitUntil(() => isReadyToShow);
        //ShowInHouseAd();
    }
    void OnAdCompleted()
    {
        DestroyAd();
        adCompleted = true;
        if (items.enableOnPlay) { items.enableOnPlay.SetActive(false); }
        if (items.body) { items.body.SetActive(false); }
        isReadyToShow = false;
        CreateNewAd();
        //GiveRewardTheUser
    }
   void DestroyAd()
    {
        currentAd = null;
        if (items.logo) { items.logo.sprite = null; }
        if (items.image1) { items.image1.sprite = null; }
        if (items.image2) { items.image2.sprite = null; }
        if (items.image3) { items.image3.sprite = null; }
        if (items.image4) { items.image4.sprite = null; }
        if (items.app_name) { items.app_name.text = string.Empty; }
        if (items.description) { items.description.text = string.Empty; }
        appLink = string.Empty;
    }
    void OnAdClosed()
    {
        if (this!=null&& PopupIsRunning) { StopCoroutine("ShowRewardedInterstitialStarting_Popup"); }
        DestroyAd();
        if (items.enableOnPlay) { items.enableOnPlay.SetActive(false); }
        if (items.body) { items.body.SetActive(false); }
        isReadyToShow = false;
         videoLenth = 0;
        CreateNewAd();


        //Warn Ad Closed
    }
    void OnAdCancelled()
    {

    }
    void CloseAd()
    {
       
        inHouseAdManager.CallOnAdClosed();

    }
    public bool isReadyToShow;
    InHouse_Ad currentAd = null;
    public void CreateNewAd()
    {
        if (inHouseAdManager.my_Ads.Count > 0)
        {
            currentAd = inHouseAdManager.CurrentAd();
            if (currentAd)
            {
                StartCoroutine(GetImage(currentAd.appLogo_Link, items.logo));
                StartCoroutine(GetImage(currentAd.image1_Link, items.image1));
                StartCoroutine(GetImage(currentAd.image2_Link, items.image2));
                StartCoroutine(GetImage(currentAd.image3_Link, items.image3));
                StartCoroutine(GetImage(currentAd.image4_Link, items.image4));
                items.app_name.text = currentAd.appTitle;
                items.description.text = currentAd.Description;
                appLink = currentAd.appLink;
                items.install_Button.onClick.RemoveAllListeners();
                items.install_Button.onClick.AddListener(OnClickInstall_Button);

            }
            isReadyToShow = true;
        }
    }
    string appLink;
    void OnClickInstall_Button()
    {
        if (appLink != string.Empty)
        {
            Application.OpenURL(appLink);
        }       
        CallAddComplete();
    }

    IEnumerator GetImage(string url, Image image)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Texture2D webTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            image.overrideSprite = SpriteFromTexture2D(webTexture);

        }
        request.Dispose();
    }
    Sprite SpriteFromTexture2D(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
    }
    public void ShowInHouseAd()
    {
        if (PopupIsRunning) { StopCoroutine("ShowRewardedInterstitialStarting_Popup"); }       
        if (isReadyToShow)
        {
            if (items.body) { items.body.SetActive(true); }
            adCompleted = false;
            isReadyToShow = false;
            StartCoroutine(ShoAd());
        }       
    }  
    IEnumerator ShoAd()
    {       
        if (items.loading)  { items.loading.SetActive(true); }
        yield return new WaitForSeconds(3);
        if (items.loading) { items.loading.SetActive(false); }
        if (items.enableOnPlay) { items.enableOnPlay.SetActive(true); }              
        StartCoroutine ("ShowRewardedInterstitialStarting_Popup");
    }
    public void CallAddComplete()
    {
        inHouseAdManager.CallOnAdCompleted();
    }
    bool PopupIsRunning;
    IEnumerator ShowRewardedInterstitialStarting_Popup()
    {
        PopupIsRunning = true;
        videoLenth = 16;
        adCompleted = false;
        items.timerText.text = videoLenth.ToString();
        yield return new WaitForSeconds(0.5f);

        int t = 0;
        t = (int)videoLenth;
        while (t > 0&&!adCompleted)
        {
            t--;
            items.timerText.text = "Remaining Time : "
                + t;
            yield return new WaitForSeconds(1);           
        }
        if (t == 0) { adCompleted = true; }
        videoLenth = 0;
        PopupIsRunning = false;
    }
}
