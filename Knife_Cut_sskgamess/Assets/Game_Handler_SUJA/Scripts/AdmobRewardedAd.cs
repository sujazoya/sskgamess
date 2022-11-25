using UnityEngine.Events;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
public class AdmobRewardedAd : MonoBehaviour
{
    private RewardedAd rewardedAd1;
    private RewardedAd rewardedAd2;
    private RewardedAd rewardedAd3;
    string adUnitId= "ca-app-pub-3940256099942544/5224354917";

    public Image rew1;
    public Image rew2;
    public Image rew3;

    private void Awake()
    {
        StartCoroutine(TryInitAds());
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

    }
    public void InitAds()
    {
        MobileAds.Initialize(initStatus => { });

        //Debug.Log("Admob Initializes");

    }
    public void Start()
    {
       rewardedAd1 = RequestRewardedAd(adUnitId);
       rewardedAd2 = RequestRewardedAd(adUnitId);
       rewardedAd3 = RequestRewardedAd(adUnitId);
       StartCoroutine(CheckAdLoaded());
    }
    public RewardedAd RequestRewardedAd(string adUnitId)
    {
        RewardedAd rewardedAd = new RewardedAd(adUnitId);

        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        rewardedAd.LoadAd(request);
        return rewardedAd;
    }

    IEnumerator  CheckAdLoaded()
    {
        rew1.color = Color.red;
        while (rewardedAd1.IsLoaded())
        {
            yield return null;
        }
        //yield return new WaitUntil(() => AdmobAdmanager.Instance.interstitialAd[AdmobAdmanager.currentIntIndex].IsLoaded());
        rew1.color = Color.green;
        rew2.color = Color.red;
        while (rewardedAd2.IsLoaded())
        {
            yield return null;
        }
        //yield return new WaitUntil(() => AdmobAdmanager.Instance.interstitialAd[AdmobAdmanager.currentIntIndex].IsLoaded());
        rew2.color = Color.green;
        rew3.color = Color.red;
        while (rewardedAd3.IsLoaded())
        {
            yield return null;
        }
        //yield return new WaitUntil(() => AdmobAdmanager.Instance.interstitialAd[AdmobAdmanager.currentIntIndex].IsLoaded());
        rew3.color = Color.green;
    }

    public void ShowRewarded(int index)
    {
        if (index == 1)
        {
            rewardedAd1.Show();
        }else
             if (index == 2)
        {
            rewardedAd2.Show();
        }
        else if (index == 3)
        {
            rewardedAd3.Show();
        }     

    }
    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToLoad event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdClosed event received");
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardedAdRewarded event received for "
                        + amount.ToString() + " " + type);
    }
}

