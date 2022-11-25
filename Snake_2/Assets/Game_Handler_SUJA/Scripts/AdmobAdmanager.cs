

using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using System.Collections;
using System;
using suja;
public class AdmobAdmanager : MonoBehaviour
{  

    private int bannerIndex = 2;
    [HideInInspector] public BannerView[] bannerView;
    public string[] bannerID;
    public GoogleMobileAds.Api.AdPosition bannerPose = GoogleMobileAds.Api.AdPosition.Bottom;


    private int intIndex = 3;
    public GoogleMobileAds.Api.InterstitialAd[] interstitialAd;
    public string[] interstitiaAdID;   
    private int intRewIndex = 3;
    public string[] rewardedIntadUnitId;   
    [HideInInspector]public AdRequest[] requestInterstitial;
    [HideInInspector] public AdRequest[] requestRewarded;
    [HideInInspector] public AdRequest[] requestRewardedInterstitialAd;
    public RewardedInterstitialAd rewardedInterstitialAd;
    [SerializeField] Text timerText;
    [SerializeField] Button noThanks_Button;
    [SerializeField] Animator popAnim;
    [SerializeField] GameObject popupPanel;
    private bool showAds;
    public static bool readyToShoAd;
    bool show_ad_as_index;
    bool show_unity_ad; 
    RewardedAdManager rewardedAdManager;  
    #region GestAdIDsAndRequestAds()   

    public void GestAdIDsAndRequestAds()
    {
        showAds = GoogleSheetHandler.showAds;
        //Set refrencess here
        bannerView = new BannerView[2];
        interstitialAd = new GoogleMobileAds.Api.InterstitialAd[3];      
        requestInterstitial = new AdRequest[3];       
        requestRewarded = new AdRequest[3];
        requestRewardedInterstitialAd = new AdRequest[3];
       


        // BANNER SECTION
        bannerID = new string[bannerIndex];
        bannerID[0] = GoogleSheetHandler.g_banner1;

        //Debug.Log("Banner1 ->" + bannerID[0]);

        RequestBanner(bannerID[0], 0);
        bannerID[1] = GoogleSheetHandler.g_banner2;
        RequestBanner(bannerID[1], 1);
        //bannerView = new BannerView[bannerIndex];

        // interstitia SECTION

        interstitiaAdID = new string[intIndex];
        interstitiaAdID[0] = GoogleSheetHandler.g_inter1;
        RequestInterstitialAdAd(interstitiaAdID[0], 0);
        interstitiaAdID[1] = GoogleSheetHandler.g_inter2;
        RequestInterstitialAdAd(interstitiaAdID[1], 1);
        interstitiaAdID[2] = GoogleSheetHandler.g_inter3;
        RequestInterstitialAdAd(interstitiaAdID[2], 2);

        // rewardedIntadUnit SECTION
        rewardedIntadUnitId = new string[intRewIndex];
        rewardedIntadUnitId[0] = GoogleSheetHandler.g_rewardedint1;
        RequestRewardedInterstitialAd(rewardedIntadUnitId[0], 0);
        rewardedIntadUnitId[1] = GoogleSheetHandler.g_rewardedint2;
        RequestRewardedInterstitialAd(rewardedIntadUnitId[1], 1);
        rewardedIntadUnitId[2] = GoogleSheetHandler.g_rewardedint3;
        RequestRewardedInterstitialAd(rewardedIntadUnitId[2], 2);       

        show_ad_as_index = GoogleSheetHandler.show_ad_as_index;
          //Debug.Log($"---{rewardedAdID[0]}\n{rewardedAdID[1]}\n{rewardedAdID[2]}");
        readyToShoAd = true;       
        StartCoroutine(ShowRewardedInt());
        Invoke("ShowInterstitial", 3);
        show_unity_ad = GoogleSheetHandler.show_unity_ad;

    }
    #endregion
   


    #region SHOW AD IN GAP   
    IEnumerator ShowRewardedInt()
    {
        yield return new WaitForSeconds(5);
        ShowRewardedInterstitialAd();
        //StartCoroutine(ShowRewarded());
    }

    private static AdmobAdmanager _instance;

    public static AdmobAdmanager Instance { get { return _instance; } }

    #endregion
    private void Awake()
    {
       
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        StartCoroutine(TryInitAds());        
    }
    //private bool loaded()
    //{
    //    return string.IsNullOrEmpty(bannerID) && string.IsNullOrEmpty(interstitiaAdID) && string.IsNullOrEmpty(rewardedAdID);
    //}
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
      
    }
    private void Start()
    {       
        if (noThanks_Button)
        {
            noThanks_Button.onClick.AddListener(DontShowAd);
        }
        if (popupPanel)
        {
            popupPanel.SetActive(false);
        }         
        rewardedAdManager=FindObjectOfType<RewardedAdManager>();    
        //ShowInterstitial();
        StartCoroutine(ShowBanner());               
    }

    public void InitAds()
    {
        MobileAds.Initialize(initStatus => { });    
 
        //Debug.Log("Admob Initializes");
      
    }   
    void RequestRewardedInterstitialAd(string rewardedIntadUnitId,int rewIntIndex)
    {
        RewardedInterstitialAd.LoadAd(rewardedIntadUnitId, requestRewardedInterstitialAd[rewIntIndex], adLoadCallback);
       
    }  

    #region REWARDED INTERTITIL

    bool showRewardIntAd;
    IEnumerator ShowRewardedInterstitialStarting_Popup()
    {
        popupPanel.SetActive(true);
        popAnim.enabled = true;
        timerText.text = "00.5";
        yield return new WaitForSeconds(0.5f);
        popAnim.enabled = false;
        showRewardIntAd = true;
        int t = 5;
        while (t > 0)
        {
            t--;
            timerText.text = "00: "
                + t;
            yield return new WaitForSeconds(1);
        }
        popAnim.enabled = true;
        if (showRewardIntAd)
        {
            ContineuShowRewardedInterstitialAd();
        }
        StartCoroutine(popup());
    }
    void DontShowAd()
    {
        showRewardIntAd = false;
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
        //if (FindObjectOfType<UIManager>())
        //{
        //    FindObjectOfType<UIManager>().RewardTheUser_Half();
        //}
        Game.TotalCoins += 50;
        MessegeManager.insance.ShowMassege
                ("Congratulation"
                , "You Won 50 Credits", true);
    }  
    private void adLoadCallback(RewardedInterstitialAd ad, AdFailedToLoadEventArgs error)
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
        RewardedInterstitialAd.LoadAd(rewardedIntadUnitId[CurrentRewIntIndex()], requestRewardedInterstitialAd[CurrentRewIntIndex()], adLoadCallback);
    }

    private void HandlePaidEvent(object sender, AdValueEventArgs args)
    {
        //MonoBehaviour.print(
        //    "Rewarded interstitial ad has received a paid event.");
        RewardedInterstitialAd.LoadAd(rewardedIntadUnitId[CurrentRewIntIndex()], requestRewardedInterstitialAd[CurrentRewIntIndex()], adLoadCallback);

    }
    #endregion   

   public int CurrentIntIndex()
    {
        if (GoogleSheetHandler.show_g_inter1 == true)
        {
            return 0;
        }
        else if (GoogleSheetHandler.show_g_inter2 == true)
        {
            return 1;
        }else
             if (GoogleSheetHandler.show_g_inter3 == true)
        {
            return 2;
        }else
            return 0;

    }
    public int CurrentRewIntIndex()
    {
        if (GoogleSheetHandler.show_g_rewardedint1 == true)
        {
            return 0;
        }
        else if (GoogleSheetHandler.show_g_rewardedint1 == true)
        {
            return 1;
        }
        else
             if (GoogleSheetHandler.show_g_rewardedint1 == true)
        {
            return 2;
        }
        else
            return 0;

    }
    public static int Banner_Index
    {
        get { return PlayerPrefs.GetInt("Banner_Index", 0); }
        set { PlayerPrefs.SetInt("Banner_Index", value); }
    }
    public static int Int_Index
    {
        get { return PlayerPrefs.GetInt("Int_Index", 0); }
        set { PlayerPrefs.SetInt("Int_Index", value); }
    }
    public static int Rew_Index
    {
        get { return PlayerPrefs.GetInt("Rew_Index", 0); }
        set { PlayerPrefs.SetInt("Rew_Index", value); }
    }
    private static int maxBannerIndex = 2;
    private static int maxIntIndex = 3;
    private static int maxRewIndex = 3;

    void CheckBannerIndex()
    {
        Banner_Index++;
        if(Banner_Index >= maxBannerIndex)
        {
            Banner_Index=0;           
        }       
    }
    void CheckIntIndex()
    {
        Int_Index++;
        if (Int_Index >= maxIntIndex)
        {
            Int_Index = 0;
        }
    }
     public  IEnumerator WaitAndShowInterstitial(float wait)
    {
        yield return new WaitForSeconds(wait);
        if (!showAds)
           yield return null;
        StartCoroutine(WaitAplayInterstitialAd());
    }
    public void ShowInterstitial()
    {
        if (!showAds)
            return;
            StartCoroutine(WaitAplayInterstitialAd());           
            
    }
    int myRequestCount=1;
    IEnumerator WaitAplayInterstitialAd()
    {       
        if (myRequestCount == ad_show_onrequest_count())
        {
            if (show_ad_as_index == false)
            {
                while (!interstitialAd[CurrentIntIndex()].IsLoaded())
                {
                    yield return null;
                }
                interstitialAd[CurrentIntIndex()].Show();
            }
            else
            {
                while (!interstitialAd[Int_Index].IsLoaded())
                {
                    yield return null;
                }
                interstitialAd[Int_Index].Show();                   
            }
            myRequestCount = 1;
        }
        else
        {
            myRequestCount++;
        }               
    
    }
   public void ShowInterstitial_Instant()
    {
        if (show_ad_as_index == false)
        {
            if (!interstitialAd[CurrentIntIndex()].IsLoaded())
                 return;
            interstitialAd[CurrentIntIndex()].Show();
        }
        else
        {
            if (!interstitialAd[Int_Index].IsLoaded())
                return;            
            interstitialAd[Int_Index].Show();
            CheckIntIndex();
        }
    }
    int ad_show_onrequest_count()
    {
        if (GoogleSheetHandler.ad_show_onrequest_count == "1")
        {
            return 1;
        }
        else
             if (GoogleSheetHandler.ad_show_onrequest_count == "2")
        {
            return 2;
        }
        else
             if (GoogleSheetHandler.ad_show_onrequest_count == "3")
        {
            return 3;
        }
        else
             if (GoogleSheetHandler.ad_show_onrequest_count == "4")
        {
            return 4;
        }
        else
             if (GoogleSheetHandler.ad_show_onrequest_count == "5")
        {
            return 5;
        }
        else
            return 1;
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

    #region BANNER

    public void RequestBanner(string bannerID ,int bannerIndex)
    {
        bannerView[bannerIndex] = new BannerView(bannerID,GoogleMobileAds.Api.AdSize.Banner, bannerPose);

        AdRequest request = new AdRequest.Builder().Build();

        bannerView[bannerIndex].LoadAd(request);
        
       
      }
    int BannerIndex()
    {
        if (GoogleSheetHandler.show_g_banner1 == true)
        {
            return 0;
        }
        else if (GoogleSheetHandler.show_g_banner2 == true)
        {
            return 1;
        }
        else
        return 0;

    }
    public IEnumerator ShowBanner()
    {
        if(GoogleSheetHandler.show_fb_ad==false)
           {
                yield return new WaitUntil(() => readyToShoAd);
        
                if (show_ad_as_index == false)
                {
                    if (showAds == true)
                    {
                        bannerView[BannerIndex()].Show();
                    }
                }
                else
                {
                    if (showAds == true)
                    {
                        bannerView[Banner_Index].Show();
                        CheckBannerIndex();
                    }
                }    
            }           
                  
    }

    public void HideBanner()
    {
        bannerView[BannerIndex()].Hide();

    }
    #endregion

    
    public void RequestInterstitialAdAd(string interstitiaAdID,int int_index)
    {
        interstitialAd[int_index] = new GoogleMobileAds.Api.InterstitialAd(interstitiaAdID);

        requestInterstitial[int_index] = new AdRequest.Builder().Build();

        this.interstitialAd[int_index].OnAdClosed += HandleOnAdClosed;
        this.interstitialAd[int_index].OnAdClosed += HandleOnAdOpened;
        // Called when the ad click caused the user to leave the application.
        this.interstitialAd[int_index].OnAdDidRecordImpression += HandleOnAdLeavingApplication;

        interstitialAd[int_index].LoadAd(requestInterstitial[int_index]);
        

    }
    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }
    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        //adManager.CallRewardedAdClickedEvent();
        //MonoBehaviour.print("HandleAdLeavingApplication event received");
          if(show_ad_as_index==false)
        {
            interstitialAd[CurrentIntIndex()].LoadAd(requestInterstitial[CurrentIntIndex()]);
        }
        else
        {
         interstitialAd[Int_Index].LoadAd(requestInterstitial[Int_Index]);
         CheckIntIndex();
        }        
    }  

    public int CurrentRewIndex()
    {
        if (GoogleSheetHandler.show_g_rewarded1 == true)
        {
            return 0;
        }
        else if (GoogleSheetHandler.show_g_rewarded2 == true)
        {
            return 1;
        }
        else
             if (GoogleSheetHandler.show_g_rewarded3 == true)
        {
            return 2;
        }
        else
            return 0;

    }

    public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        //MonoBehaviour.print(
        //    "HandleRewardedAdFailedToLoad event received with message: "
        //                     + args.Message);
    }
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        // MonoBehaviour.print("HandleAdClosed event received");
        //adManager.CallRewardedAdClosedEvent();       
        
        if(show_ad_as_index==false)
        {
            interstitialAd[CurrentIntIndex()].LoadAd(requestInterstitial[CurrentIntIndex()]);
        }
        else
        {
         interstitialAd[Int_Index].LoadAd(requestInterstitial[Int_Index]);
         CheckIntIndex();
        }         
    }

    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        //  Debug.Log("Rewarded Video ad loaded successfully");

    }

    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        //MonoBehaviour.print(
        //    "HandleRewardedAdFailedToLoad event received with message: "
        //                     + args.Message);
    }  


    private void OnDestroy()
    {
        //ConfigManager.FetchCompleted -= GetAdId;
    }
   
}
