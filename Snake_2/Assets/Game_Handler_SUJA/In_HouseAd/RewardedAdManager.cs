using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using System.Collections;
using System;
using UnityEngine.Events;

public class RewardedAdManager : MonoBehaviour
{   
   
    bool show_ad_as_index;
    private bool showAds;    
    public static bool show_rewarded;
    public static int show_rewarded_onrequest_count;  
    public static RewardedAdManager Instance;
    public RewardedButton[] rewardedButtons;  
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
      
    }  
    void Start()
    {
        MobileAds.Initialize(initStatus => { });
        StartCoroutine(TryToFetch());           
    }
    IEnumerator TryToFetch()
    {
        yield return new WaitForSeconds(1.5f);
        yield return new WaitUntil(() => GoogleSheetHandler.googlesheetInitilized);
        show_rewarded = GoogleSheetHandler.show_rewarded;
        show_ad_as_index = GoogleSheetHandler.show_ad_as_index;
        show_rewarded_onrequest_count = int.Parse(GoogleSheetHandler.show_rewarded_onrequest_count);
        showAds = GoogleSheetHandler.showAds;
        RequestRewarded();            
             
    }
    public static void Initialize(Action<InitializationStatus> initCompleteAction) { }  

    
    public static RewardedAd rewarded;


    string rewardedAd_ID1;
    string rewardedAd_ID2;
    string rewardedAd_ID3;

    int totallRewarded = 10;   
    public void RequestRewarded()
    {
        rewardedAd_ID1 = GoogleSheetHandler.g_rewarded1;
        rewardedAd_ID2 = GoogleSheetHandler.g_rewarded2;
        rewardedAd_ID3 = GoogleSheetHandler.g_rewarded3;
       
        rewarded = RequestRewardedAd(CurrentRewardedId());         

    }
    public string CurrentRewardedId()
    {
        if (GoogleSheetHandler.show_g_rewarded1 == true)
        {
            return rewardedAd_ID1;
        }
        else if (GoogleSheetHandler.show_g_rewarded2 == true)
        {
            return rewardedAd_ID2;
        }
        else
             if (GoogleSheetHandler.show_g_rewarded3 == true)
        {
            return rewardedAd_ID3;
        }
        else
            return rewardedAd_ID1;

    }
    public RewardedAd RequestRewardedAd(string adUnitId)
    {
        RewardedAd myRewardedAd;       
        myRewardedAd = new RewardedAd(adUnitId);

        myRewardedAd.OnAdLoaded += HandleRewardBasedVideoLoaded;
        //myRewardedAd.OnUserEarnedReward += HandleRewardBasedVideoRewarded;
        //myRewardedAd.OnAdClosed += HandleRewardBasedVideoClosed;
        //myRewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        //if (rewardedButtons.Length > 0)
        //{
        //    for (int i = 0; i < rewardedButtons.Length; i++)
        //    {
        //        myRewardedAd.OnUserEarnedReward += rewardedButtons[i].HandleRewardBasedVideoRewarded;
        //        myRewardedAd.OnAdClosed += rewardedButtons[i].HandleRewardedAdClosed;
        //    }
        //}
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        myRewardedAd.LoadAd(request);
        return myRewardedAd;
    }
    public  bool IsReadyToShowAd()
    {
        if (rewarded.IsLoaded())
        {
            return true;
        }
        else
        {
            return false;
        }
    }   

    public void ShowRewardedAd()
    {
       
        if (!showAds||!show_rewarded)
            return;
        StartCoroutine(WaitAplayRewardedAd());
       
    }
    static int gIndex;   
    
    void Gindex()
    {
        gIndex++;
        if (gIndex >= totallRewarded)
        {
            gIndex = 0;
        }
    }
  
    IEnumerator WaitAplayRewardedAd()
    {

        //CurrentRewardedAd().Show();   
            
            while (!rewarded.IsLoaded())
            {
                yield return null;
            }
            rewarded.Show();
            Gindex();       
    }
   
    //public void ShowRewardedAd()
    //{
       
    //}

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        //MonoBehaviour.print(
        //    "HandleRewardedAdFailedToLoad event received with message: "
        //                     + args.Message);
        //RequestRewardedAd(CurrentRewardedAd_ID());
    }
    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        //MonoBehaviour.print("HandleAdOpened event received");
    }
  
   
    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        //  Debug.Log("Rewarded Video ad loaded successfully");

    }
   
    
}