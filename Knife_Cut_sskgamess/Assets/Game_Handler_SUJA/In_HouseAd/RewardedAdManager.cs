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
        yield return new WaitForSeconds(3);
        yield return new WaitUntil(() => GoogleSheetHandler.googlesheetInitilized);
        show_rewarded = GoogleSheetHandler.show_rewarded;
        show_ad_as_index = GoogleSheetHandler.show_ad_as_index;
        show_rewarded_onrequest_count = int.Parse(GoogleSheetHandler.show_rewarded_onrequest_count);
        showAds = GoogleSheetHandler.showAds;

        if (show_rewarded == true)
        {
            RequestRewarded();
        }
        
    }
    public static void Initialize(Action<InitializationStatus> initCompleteAction) { }
    #region REWARDED
    private static RewardedAd Rewarded_1;
    private static RewardedAd Rewarded_2;
    private static RewardedAd Rewarded_3;
    private static RewardedAd Rewarded_4;
    private static RewardedAd Rewarded_5;
    private static RewardedAd Rewarded_6;
    private static RewardedAd Rewarded_7;
    private static RewardedAd Rewarded_8;
    private static RewardedAd Rewarded_9;
    private static RewardedAd Rewarded_10;



    string rewardedAd_ID1;
    string rewardedAd_ID2;
    string rewardedAd_ID3;

    int totallRewarded = 10;   
    public void RequestRewarded()
    {
        rewardedAd_ID1 = GoogleSheetHandler.g_rewarded1;
        rewardedAd_ID2 = GoogleSheetHandler.g_rewarded2;
        rewardedAd_ID3 = GoogleSheetHandler.g_rewarded3;

        if (show_ad_as_index == true)
        {
            
            Rewarded_1 = RequestRewardedAd(rewardedAd_ID1);
            Rewarded_2 = RequestRewardedAd(rewardedAd_ID2);
            Rewarded_3 = RequestRewardedAd(rewardedAd_ID3);
            Rewarded_4 = RequestRewardedAd(rewardedAd_ID1);
            Rewarded_5 = RequestRewardedAd(rewardedAd_ID2);
            Rewarded_6 = RequestRewardedAd(rewardedAd_ID3);
            Rewarded_7 = RequestRewardedAd(rewardedAd_ID1);
            Rewarded_8 = RequestRewardedAd(rewardedAd_ID2);
            Rewarded_9 = RequestRewardedAd(rewardedAd_ID3);
            Rewarded_10 = RequestRewardedAd(rewardedAd_ID1);
        }
        else
        {
            Rewarded_1 = RequestRewardedAd(CurrentRewardedId());
            Rewarded_2 = RequestRewardedAd(CurrentRewardedId());
            Rewarded_3 = RequestRewardedAd(CurrentRewardedId());
            Rewarded_4 = RequestRewardedAd(CurrentRewardedId());
            Rewarded_5 = RequestRewardedAd(CurrentRewardedId());
            Rewarded_6 = RequestRewardedAd(CurrentRewardedId());
            Rewarded_7 = RequestRewardedAd(CurrentRewardedId());
            Rewarded_8 = RequestRewardedAd(CurrentRewardedId());
            Rewarded_9 = RequestRewardedAd(CurrentRewardedId());
            Rewarded_10 = RequestRewardedAd(CurrentRewardedId());

        }


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
        myRewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
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
        if (CurrentRewardedAd().IsLoaded())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //RewardedAd GetRewardedAd(int count)
    //{
    //    if (count == 1) { return Rewarded_1; }
    //    if (count == 2) { return Rewarded_2; }
    //    if (count == 3) { return Rewarded_3; }
    //    if (count == 4) { return Rewarded_4; }
    //    if (count == 5) { return Rewarded_5; }
    //    if (count == 6) { return Rewarded_6; }
    //    if (count == 7) { return Rewarded_7; }
    //    if (count == 8) { return Rewarded_8; }
    //    if (count == 9) { return Rewarded_9; }
    //    if (count == 10)  { return  Rewarded_10;  }
    //    else return Rewarded_1;
    //}
    /*
    RewardedAd currentRewardedAd;
    
    public RewardedAd CurrentRewardedAd()
    {
        
        if (show_ad_as_index == false)
        {           
            if (GoogleSheetHandler.show_g_rewarded1 == true)
            {
                currentRewardedAd = rewardedAd1;
               
            }
            else if (GoogleSheetHandler.show_g_rewarded2 == true)
            {
                currentRewardedAd = rewardedAd2;
            }
            else
                 if (GoogleSheetHandler.show_g_rewarded3 == true)
            {
                currentRewardedAd = rewardedAd3;
            }
        }
        else
        {
            if (rewIndex == 0)
            {
                currentRewardedAd = rewardedAd1;

            }
            else if (rewIndex == 1)
            {
                currentRewardedAd = rewardedAd2;
            }
            else
                 if (rewIndex == 2)
            {
                currentRewardedAd = rewardedAd2;
            }
            rewIndex++;
            if (rewIndex >= maxRew)
            {
                rewIndex = 0;
            }
        }
        return currentRewardedAd;        
        
    }
    */

    public void ShowRewardedAd()
    {
       
        if (!showAds||!show_rewarded)
            return;
        StartCoroutine(WaitAplayRewardedAd());
       
    }
    static int gIndex;    
    public static  RewardedAd CurrentRewardedAd()
    {       
        if (gIndex == 0)
        {
            return Rewarded_1;
        }
        else
         if (gIndex == 1)
        {
            return Rewarded_2;
        }
        else
             if (gIndex == 2)
        {
            return Rewarded_3;
        }
        else
             if (gIndex == 3)
        {
            return Rewarded_4;
        }
        else
             if (gIndex == 4)
        {
            return Rewarded_5;
        }
        else
             if (gIndex == 5)
        {
            return Rewarded_6;
        }
        else
             if (gIndex == 6)
        {
            return Rewarded_7;
        }
        else
             if (gIndex == 7)
        {
            return Rewarded_8;
        }
        else
             if (gIndex == 8)
        {
            return Rewarded_9;
        }
        else
             if (gIndex == 9)
        {
            return Rewarded_10;
        }
        else
        return Rewarded_1;
    }
    void Gindex()
    {
        gIndex++;
        if (gIndex >= totallRewarded)
        {
            gIndex = 0;
        }
    }

    //string CurrentID;
    //public string CurrentRewardedAd_ID()
    //{
    //    if (GoogleSheetHandler.show_g_rewarded1 == true)
    //    {
    //        CurrentID = GoogleSheetHandler.g_rewarded1;           
    //    }
    //    else if (GoogleSheetHandler.show_g_rewarded2 == true)
    //    {
    //        CurrentID = GoogleSheetHandler.g_rewarded2;            
    //    }
    //    else
    //         if (GoogleSheetHandler.show_g_rewarded3 == true)
    //    {
    //        CurrentID = GoogleSheetHandler.g_rewarded3;
            
    //    }
    //    return CurrentID;
    //}   
    IEnumerator WaitAplayRewardedAd()
    {

        //CurrentRewardedAd().Show();
      
        RewardedAd currentRewardedAd = CurrentRewardedAd();    
        while (!currentRewardedAd.IsLoaded())
        {
            yield return null;
        }        
        currentRewardedAd.Show();
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
    //public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    //{
    //    string type = args.Type;
    //    double amount = args.Amount;
    //    //RequestRewardedAd(CurrentRewardedAd_ID());
    //    //uiManager.RewardTheUser();

    //}

    //public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    //{
    //    //RequestRewardedAd(CurrentRewardedAd_ID());
    //    //uiManager.WarnAdClosed();
    //}
   
    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        //  Debug.Log("Rewarded Video ad loaded successfully");

    }
    #endregion
}