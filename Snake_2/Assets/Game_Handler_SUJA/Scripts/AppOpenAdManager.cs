using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GoogleMobileAds.Api;

public class AppOpenAdManager : MonoBehaviour
{
    private static AppOpenAdManager instance;

    private AppOpenAd ad;

    private bool isShowingAd = false;

    public static AppOpenAdManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AppOpenAdManager();
            }

            return instance;
        }
    }

    private bool IsAdAvailable
    {
        get
        {
            return ad != null;
        }
    }
    private void Start()
    {
        StartCoroutine (LoadAd()); 
    }
     public int CurrentOpenIndex()
    {
        if (GoogleSheetHandler.show_open_1 == true)
        {
            return 0;
        }
        else if (GoogleSheetHandler.show_open_2 == true)
        {
            return 1;
        }
        else
             if (GoogleSheetHandler.show_open_3 == true)
        {
            return 2;
        }
        else
            return 0;

    }
    int index_int;
     public string CurrentOpenId()
    {
        if(GoogleSheetHandler.show_ad_as_index==false){
            if (GoogleSheetHandler.show_open_1 == true)
            {
                return GoogleSheetHandler.open_id_1;
            }
            else if (GoogleSheetHandler.show_open_2 == true)
            {
                return GoogleSheetHandler.open_id_2;
            }
            else
                if (GoogleSheetHandler.show_open_3 == true)
            {
                return GoogleSheetHandler.open_id_3;
            }
            else
                return GoogleSheetHandler.open_id_1;
        }
        else
        {
          index_int++;
          if(index_int>2){
              index_int=0;
          }
           if (index_int == 0)
            {
                return GoogleSheetHandler.open_id_1;
            }
             if (index_int == 1)
            {
                return GoogleSheetHandler.open_id_2;
            }            
            if (index_int == 2)
            {
                return GoogleSheetHandler.open_id_3;
            }
            else
                return GoogleSheetHandler.open_id_1;
        }


    }
     public IEnumerator LoadAd()
    {
         yield return new WaitUntil(() => GoogleSheetHandler.googlesheetInitilized);        
        AdRequest request = new AdRequest.Builder().Build();
        // Load an app open ad for portrait orientation
        AppOpenAd.LoadAd(CurrentOpenId(), ScreenOrientation.Portrait, request, ((appOpenAd, error) =>
        {
            if (error != null)
            {
                // Handle the error.
                Debug.LogFormat("Failed to load the ad. (reason: {0})", error.LoadAdError.GetMessage());
                return;
            }

            // App open ad is loaded.
            ad = appOpenAd;
        }));
    }
    public void ShowAdIfAvailable()
    {
        if (!IsAdAvailable || isShowingAd)
        {
            return;
        }

        ad.OnAdDidDismissFullScreenContent += HandleAdDidDismissFullScreenContent;
        ad.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresentFullScreenContent;
        ad.OnAdDidPresentFullScreenContent += HandleAdDidPresentFullScreenContent;
        ad.OnAdDidRecordImpression += HandleAdDidRecordImpression;
        ad.OnPaidEvent += HandlePaidEvent;

        ad.Show();
    }
     
    private void HandleAdDidDismissFullScreenContent(object sender, EventArgs args)
    {
        Debug.Log("Closed app open ad");
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        ad = null;
        isShowingAd = false;        
         LoadAd();      
  
    }

    private void HandleAdFailedToPresentFullScreenContent(object sender, AdErrorEventArgs args)
    {
        Debug.LogFormat("Failed to present the ad (reason: {0})", args.AdError.GetMessage());
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        ad = null;
        LoadAd();
    }

    private void HandleAdDidPresentFullScreenContent(object sender, EventArgs args)
    {
        Debug.Log("Displayed app open ad");
        isShowingAd = true;
    }

    private void HandleAdDidRecordImpression(object sender, EventArgs args)
    {
        Debug.Log("Recorded ad impression");
    }

    private void HandlePaidEvent(object sender, AdValueEventArgs args)
    {
        Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
                args.AdValue.CurrencyCode, args.AdValue.Value);
    }
      public void OnApplicationPause(bool paused)
    {
        // Display the app open ad when the app is foregrounded
        if (!paused)
        {
            AppOpenAdManager.Instance.ShowAdIfAvailable();
        }
    }
}
