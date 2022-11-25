using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using System.Collections;
using System;
using SimpleJSON;
using suja;
//using Unity.RemoteConfig;
[RequireComponent(typeof(DatabaseManager))]
public class AdsManager : MonoBehaviour
{
    public struct userAttributes { }
    public struct appAttributes { }
    private BannerView bannerView;
    public static string bannerIDA;
    public  string bannerID = "ca-app-pub-6263037668585399/9811257595";
    private InterstitialAd interstitialAd;
    private string interstitiaAdID = "ca-app-pub-6263037668585399/8306604236";
    private RewardBasedVideoAd rewardedAd;
    private string rewardedAdID = "ca-app-pub-6263037668585399/9236542527";

    private string adUnitId = "ca-app-pub-6263037668585399/8321882268";

    //private Unity_AdManager unity_AdManager;
    //AdManagerAll managerAll;
    [SerializeField] bool releaseBuild;
    bool updateAdmobIDS;

    //AdManagerAll adManager;
    AdRequest requestInterstitial;
    AdRequest requestRewarded;
    AdRequest requestRewardedInterstitialAd;
    private RewardedInterstitialAd rewardedInterstitialAd;
    [SerializeField] Text timerText;
    [SerializeField] Button noThanks_Button;
    [SerializeField] Animator popAnim;
    [SerializeField] GameObject popupPanel;

    public static AdsManager Instance;
    public Text testText;

    #region SHOW AD IN GAP
    private float WaitForNextAd = 200;
    IEnumerator ShowRewarded()
    {
        yield return new WaitForSeconds(WaitForNextAd);
        ShowRewardedInterstitialAd();
        StartCoroutine(ShowRewarded());
    }
    #endregion
    private void Awake()
    {
        StartCoroutine(ShowRewarded());
        AdsManager[] objs = FindObjectsOfType<AdsManager>();
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        Instance = this;

        // if (ConfigManager.requestStatus == ConfigRequestStatus.Success)
        // {
        //ConfigManager.FetchCompleted += GetAdId;
        //ConfigManager.FetchConfigs<userAttributes, appAttributes>(new userAttributes(), new appAttributes());
        //}
        // if (!releaseBuild)
        // {
        //bannerID = "ca-app-pub-3940256099942544/6300978111";
        //interstitiaAdID = "ca-app-pub-3940256099942544/1033173712";
        //rewardedAdID = "ca-app-pub-3940256099942544/5224354917";
        //adUnitId = "ca-app-pub-3940256099942544/5354046379";



        //           RequestBanner();


        rewardedAd = RewardBasedVideoAd.Instance;
        //managerAll = GetComponent<AdManagerAll>();       
        StartCoroutine(TryInitAds());
        //}
    }
    public void ShowAPIResult()
    {
        if (testText)
        {
            testText.text=string.Empty;
            testText.text = GoogleSheetHandler.localInfo;
        }
        bannerID = null;
        bannerID = bannerIDA;
    }

    private int admobInterstitialCounter;
    private int admobBannerCounter;
    private int admobRewardedCounter;

    private int counter;

  
  GameHandler.AdShowGameState adShowGameState;
    private void HandleStateAfterAdClosedOrFailed()
    {
        switch (adShowGameState)
        {
            case GameHandler.AdShowGameState.GameOver:
                break;
            case GameHandler.AdShowGameState.Exit:
                //Show Popup
                break;
            case GameHandler.AdShowGameState.BackToHome:
                //Open Menu
                break;
            case GameHandler.AdShowGameState.ShopClose:
                //Show Menu
                break;
            case GameHandler.AdShowGameState.reload:
                //Reload Level
                break;

            default:
                break;

        }
    }

    public void LoadInterstitialAd(int index)
    {
        if (!showAd)      
            return;
        LoadInterstitial(index);
    }
    string interstitialAdID(int index)
    {
        if (index == 0)
        {
            return GoogleSheetHandler. g_inter1;
        }
        else if (index == 1)
        {
            return GoogleSheetHandler.g_inter2;
        }
        else if (index == 2)
        {
            return GoogleSheetHandler.g_inter3;
        }
        else
            return null;

    } 
    public void LoadInterstitial(int index)
    {
        string adID = interstitialAdID(index);
        interstitialAd = new InterstitialAd(adID);

        requestInterstitial = new AdRequest.Builder().Build();

        this.interstitialAd.OnAdClosed += HandleOnAdClosed;
        this.interstitialAd.OnAdClosed += HandleOnAdOpened;
        // Called when the ad click caused the user to leave the application.
        this.interstitialAd.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        interstitialAd.LoadAd(requestInterstitial);

    }
    string rewardedsAdID(int index)
    {
        if (index == 0)
        {
            return GoogleSheetHandler.g_rewarded1;
        }
        else if (index == 1)
        {
            return GoogleSheetHandler.g_rewarded2;
        }
        else
            return null;

    }

    public void LoadRewardedAd(int index)
    {
        string adID = rewardedsAdID(index);        
        rewardedAd.OnAdLoaded += HandleRewardBasedVideoLoaded;

        rewardedAd.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;

        rewardedAd.OnAdRewarded += HandleRewardBasedVideoRewarded;

        rewardedAd.OnAdClosed += HandleRewardBasedVideoClosed;

        requestRewarded = new AdRequest.Builder().Build();

        rewardedAd.LoadAd(requestRewarded, adID);

    }

    

    //void GetAdId(ConfigResponse response)
    //{
    //    updateAdmobIDS = ConfigManager.appConfig.GetBool("updateAdmobIDS");
    //    //if (ConfigManager.appConfig.GetBool("releaseBuild"))
    //    //{
    //    releaseBuild = ConfigManager.appConfig.GetBool("releaseBuild");
    //    //}       
    //    //if (!releaseBuild)
    //    //{
    //    //    bannerID = "ca-app-pub-3940256099942544/6300978111";
    //    //    fullScreenAdID = "ca-app-pub-3940256099942544/1033173712";
    //    //    rewardedAdID = "ca-app-pub-3940256099942544/5224354917";
    //    //}
    //    // else 
    //    if (updateAdmobIDS)
    //    {
    //        bannerID = ConfigManager.appConfig.GetString("bannerId");
    //        interstitiaAdID = ConfigManager.appConfig.GetString("interstitilaId");
    //        rewardedAdID = ConfigManager.appConfig.GetString("rewardedId");
    //        adUnitId = ConfigManager.appConfig.GetString("adUnitId");
    //        StartCoroutine(TryInitAds());
    //    }

    //    //StartCoroutine(TryInitAds());
    //    //  text.text = rewId;
    //}
    private bool loaded()
    {
        return string.IsNullOrEmpty(bannerID) && string.IsNullOrEmpty(interstitiaAdID) && string.IsNullOrEmpty(rewardedAdID);
    }
    public IEnumerator TryInitAds()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return new WaitUntil(() => Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
            Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork);
            InitAds();
        }
        else
        {
            InitAds();
        }
        //yield return new WaitUntil(() => loaded());
        //InitAds();
    }
    private void Start()
    {
        //adManager = GetComponent<AdManagerAll>();
        // Initialize the Google Mobile Ads SDK.
        //MobileAds.Initialize(initStatus => { });
        //RequestBanner();
        //rewardedAd = RewardBasedVideoAd.Instance;
        ////managerAll = GetComponent<AdManagerAll>();       
        //InitAds();
        if (noThanks_Button)
        {
            noThanks_Button.onClick.AddListener(DontShowAd);
        }
        if (popupPanel)
        {
            popupPanel.SetActive(false);
        }
    }


    public void InitAds()
    {
        MobileAds.Initialize(initStatus => { });

        RequestFullScreenAd();
        RequestRewardedAd();

        rewardedAd.OnAdLoaded += HandleRewardBasedVideoLoaded;

        rewardedAd.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;

        rewardedAd.OnAdRewarded += HandleRewardBasedVideoRewarded;

        rewardedAd.OnAdClosed += HandleRewardBasedVideoClosed;

        //unity_AdManager = GetComponent<Unity_AdManager>();
        Debug.Log("Admob Initializes");

        // Create an empty ad request.
        requestRewardedInterstitialAd = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        RewardedInterstitialAd.LoadAd(adUnitId, requestRewardedInterstitialAd, adLoadCallback);

    }
    #region REWARDED INTERTITIL

    bool showAd;
    IEnumerator ShowRewardedInterstitialStarting_Popup()
    {
        popupPanel.SetActive(true);
        popAnim.enabled = true;
        timerText.text = "Your Ad Begins In 5";
        yield return new WaitForSeconds(0.5f);
        popAnim.enabled = false;
        showAd = true;
        int t = 5;
        while (t > 0)
        {
            t--;
            timerText.text = "Your Ad Begins In "
                + t;
            yield return new WaitForSeconds(1);
        }
        popAnim.enabled = true;
        if (showAd)
        {
            ContineuShowRewardedInterstitialAd();
        }
        StartCoroutine(popup());
    }
    void DontShowAd()
    {
        showAd = false;
        popAnim.enabled = true;
        StartCoroutine(popup());
    }
    IEnumerator popup()
    {
        yield return new WaitForSeconds(1);
        popupPanel.SetActive(false);
    }
    public void ShowRewardedInterstitialAd()
    {
        StartCoroutine(ShowRewardedInterstitialStarting_Popup());

    }
    private void ContineuShowRewardedInterstitialAd()
    {
        if (rewardedInterstitialAd != null)
        {
            rewardedInterstitialAd.Show(userEarnedRewardCallback);
        }
    }
    private void userEarnedRewardCallback(Reward reward)
    {
        // TODO: Reward the user.
    }
    private void adLoadCallback(RewardedInterstitialAd ad, string error)
    {
        if (error == null)
        {
            rewardedInterstitialAd = ad;

            rewardedInterstitialAd.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresent;
            rewardedInterstitialAd.OnAdDidPresentFullScreenContent += HandleAdDidPresent;
            rewardedInterstitialAd.OnAdDidDismissFullScreenContent += HandleAdDidDismiss;
            rewardedInterstitialAd.OnPaidEvent += HandlePaidEvent;
        }
    }
    private void HandleAdFailedToPresent(object sender, AdErrorEventArgs args)
    {
        //MonoBehavior.print("Rewarded interstitial ad has failed to present.");
    }

    private void HandleAdDidPresent(object sender, EventArgs args)
    {
        //MonoBehavior.print("Rewarded interstitial ad has presented.");
    }

    private void HandleAdDidDismiss(object sender, EventArgs args)
    {
        //MonoBehavior.print("Rewarded interstitial ad has dismissed presentation.");
        RewardedInterstitialAd.LoadAd(adUnitId, requestRewardedInterstitialAd, adLoadCallback);
    }

    private void HandlePaidEvent(object sender, AdValueEventArgs args)
    {
        //MonoBehaviour.print(
        //    "Rewarded interstitial ad has received a paid event.");
        RewardedInterstitialAd.LoadAd(adUnitId, requestRewardedInterstitialAd, adLoadCallback);

    }
    #endregion

    IEnumerator CheckForReward()
    {
        yield return new WaitForSeconds(300f);
        ShowRewardedAd();
        StartCoroutine(CheckForReward());
    }
    IEnumerator WaitAplayInterstitialAd()
    {
        while (!interstitialAd.IsLoaded())
        {
            yield return null;
        }
        ShowInterstitial();
    }

    #region Native Ad Mehods ------------------------------------------------
    /*
    private void RequestNativeAd()
    {
        AdLoader adLoader = new AdLoader.Builder(idNative).ForUnifiedNativeAd().Build();
        adLoader.OnUnifiedNativeAdLoaded += this.HandleOnUnifiedNativeAdLoaded;
        adLoader.LoadAd(AdRequestBuild());
    }



    //events
    private void HandleOnUnifiedNativeAdLoaded(object sender, UnifiedNativeAdEventArgs args)
    {
        this.adNative = args.nativeAd;
        nativeLoaded = true;
    }

  

    //------------------------------------------------------------------------
    AdRequest AdRequestBuild()
    {
        return new AdRequest.Builder().Build();
    }
    private void HandleNativeAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("Native ad failed to load: " + args.Message);
    }
    void Update()
    {
        if (nativeLoaded)
        {
            nativeLoaded = false;

            Texture2D iconTexture = this.adNative.GetIconTexture();
            Texture2D iconAdChoices = this.adNative.GetAdChoicesLogoTexture();
            string headline = this.adNative.GetHeadlineText();
            string cta = this.adNative.GetCallToActionText();
            string advertiser = this.adNative.GetAdvertiserText();
           nativeItem.adIcon.texture = iconTexture;
           nativeItem.adChoices.texture = iconAdChoices;
           nativeItem.adHeadline.text = headline;
           nativeItem.adAdvertiser.text = advertiser;
           nativeItem.adCallToAction.text = cta;

            //register gameobjects
            adNative.RegisterIconImageGameObject(nativeItem.adIcon.gameObject);
            adNative.RegisterAdChoicesLogoGameObject(nativeItem.adChoices.gameObject);
            adNative.RegisterHeadlineTextGameObject(nativeItem.adHeadline.gameObject);
            adNative.RegisterCallToActionGameObject(nativeItem.adCallToAction.gameObject);
            adNative.RegisterAdvertiserTextGameObject(nativeItem.adAdvertiser.gameObject);

            nativeItem.adNativePanel.SetActive(true); //show ad panel
        }
    }
    */
    #endregion
    public static void Initialize(Action<InitializationStatus> initCompleteAction) { }

    public void RequestBanner()
    {
        bannerView = new BannerView(bannerID, AdSize.Banner, AdPosition.TopLeft);

        AdRequest request = new AdRequest.Builder().Build();

        bannerView.LoadAd(request);

        bannerView.Show();
    }

    public void HideBanner()
    {
        bannerView.Hide();

    }

    public void RequestFullScreenAd()
    {
        interstitialAd = new InterstitialAd(interstitiaAdID);

        requestInterstitial = new AdRequest.Builder().Build();

        this.interstitialAd.OnAdClosed += HandleOnAdClosed;
        this.interstitialAd.OnAdClosed += HandleOnAdOpened;
        // Called when the ad click caused the user to leave the application.
        this.interstitialAd.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        interstitialAd.LoadAd(requestInterstitial);

    }

    public void ShowInterstitial()
    {
        // RequestFullScreenAd();       
        if (!interstitialAd.IsLoaded())
        {
            //unity_AdManager.ShowInterstitialVideo();
        }
        else
        {
            interstitialAd.Show();
        }

    }
    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        //adManager.CallRewardedAdClickedEvent();
        //MonoBehaviour.print("HandleAdLeavingApplication event received");
    }
    public void RequestRewardedAd()
    {
        rewardedAd.OnAdLoaded += HandleRewardBasedVideoLoaded;

        rewardedAd.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;

        rewardedAd.OnAdRewarded += HandleRewardBasedVideoRewarded;

        rewardedAd.OnAdClosed += HandleRewardBasedVideoClosed;

        requestRewarded = new AdRequest.Builder().Build();

        rewardedAd.LoadAd(requestRewarded, rewardedAdID);
    }

    public void ShowRewardedAd()
    {
        if (!rewardedAd.IsLoaded())
        {
            //unity_AdManager.ShowRewardedVideo();

        }
        else
        {
            rewardedAd.Show();
        }
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        // MonoBehaviour.print("HandleAdClosed event received");
        //adManager.CallRewardedAdClosedEvent();       
        interstitialAd.LoadAd(requestInterstitial);
    }

    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        //  Debug.Log("Rewarded Video ad loaded successfully");

    }

    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        //  Debug.Log("Failed to load rewarded video ad : " + args.Message);


    }
    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        //MonoBehaviour.print("HandleAdOpened event received");
        ////interstitialAd.Destroy();
        ////RequestFullScreenAd();
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        // Debug.Log("You have been rewarded with  " + amount.ToString() + " " + type); 
        //adManager.CallRewardedAdClickedEvent();
        //  UIManager.instance.GameOverUI.SetActive(false);       
        //  RequestRewardedAd();


    }


    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        // Debug.Log ("Rewarded video has closed");
        //adManager.CallRewardedAdClosedEvent();
        rewardedAd.LoadAd(requestRewarded, rewardedAdID);

    }
    private void OnDestroy()
    {
        //ConfigManager.FetchCompleted -= GetAdId;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ShowRewardedInterstitialAd();
        }
    }
}
