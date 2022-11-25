using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudienceNetwork;
using UnityEngine.Events;
public class FBRewardedAd : MonoBehaviour
{  
    public UnityEvent onRewarded;
	public UnityEvent onClose; 
    private RewardedVideoAd  rewardedVideoAd;
    bool isLoaded;
    void Start()
    {
         StartCoroutine(LoadRewardedVideo());  
    }   
    
    public IEnumerator LoadRewardedVideo()
    {
        yield return new WaitUntil(() => GoogleSheetHandler.googlesheetInitilized);
        if (this.rewardedVideoAd != null) {
                this.rewardedVideoAd.Dispose();
            }
        // Create the rewarded video unit with a placement ID (generate your own on the Facebook app settings).
        // Use different ID for each ad placement in your app.
        this.rewardedVideoAd = new RewardedVideoAd(GoogleSheetHandler.fb_rewarded_id);

        this.rewardedVideoAd.Register(this.gameObject);

        // Set delegates to get notified on changes or when the user interacts with the ad.
        this.rewardedVideoAd.RewardedVideoAdDidLoad = (delegate() {
            Debug.Log("RewardedVideo ad loaded.");
            this.isLoaded = true;
        });
        this.rewardedVideoAd.RewardedVideoAdDidFailWithError = (delegate(string error) {
            Debug.Log("RewardedVideo ad failed to load with error: " + error);
        });
        this.rewardedVideoAd.RewardedVideoAdWillLogImpression = (delegate() {
           // Debug.Log("RewardedVideo ad logged impression.");
            StartCoroutine(LoadRewardedVideo()); 
           onRewarded.Invoke();
        });
        this.rewardedVideoAd.RewardedVideoAdDidClick = (delegate() {
            //Debug.Log("RewardedVideo ad clicked.");
            StartCoroutine(LoadRewardedVideo()); 
            onRewarded.Invoke();
        });

        this.rewardedVideoAd.RewardedVideoAdDidClose = (delegate() {
            //Debug.Log("Rewarded video ad did close.");
            if (this.rewardedVideoAd != null) {
                this.rewardedVideoAd.Dispose();
            }
            StartCoroutine(LoadRewardedVideo()); 
        });

        // Initiate the request to load the ad.
        this.rewardedVideoAd.LoadAd();
    }
    public void ShowRewardedVideo_FB()
    {
        if(GoogleSheetHandler.show_fb_ad==false)
        return;
        if (this.isLoaded) {
            this.rewardedVideoAd.Show();
            this.isLoaded = false;
        } else {
           // Debug.Log("Ad not loaded. Click load to request an ad.");
        }
    }
   
}
