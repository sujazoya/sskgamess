using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudienceNetwork;

public class FacebookAdManager : MonoBehaviour
{
    private AdView adView;

     private InterstitialAd interstitialAd;
    private bool isLoaded;
    public static FacebookAdManager Instance;

    private void Awake(){
        Instance=this;
       //AudienceNetworkAds.Initialize();
    }

    // Start is called before the first frame update
    void Start()
    { 
       StartCoroutine (LoadBanner());      
       StartCoroutine(LoadInterstitial());
    }
#region BANNER
    public IEnumerator LoadBanner()
    {
       
        yield return new WaitUntil(() => GoogleSheetHandler.googlesheetInitilized);
        if(GoogleSheetHandler.show_fb_ad==false)
        yield return null;
        if (this.adView) {
            this.adView.Dispose();
        }

        this.adView = new AdView(GoogleSheetHandler.fb_banner_id, AdSize.BANNER_HEIGHT_50);
        this.adView.Register(this.gameObject);

        // Set delegates to get notified on changes or when the user interacts with the ad.
        this.adView.AdViewDidLoad = (delegate() {
            Debug.Log("Banner loaded.");
            this.adView.Show(AdPosition.BOTTOM);
        });
        adView.AdViewDidFailWithError = (delegate(string error) {
            Debug.Log("Banner failed to load with error: " + error);
        });
        adView.AdViewWillLogImpression = (delegate() {
            Debug.Log("Banner logged impression.");            
        });
        adView.AdViewDidClick = (delegate() {
            Debug.Log("Banner clicked.");            
        });

        // Initiate a request to load an ad.
        adView.LoadAd();
    }
    #endregion
#region Interstitial
     IEnumerator  LoadInterstitial()
    {
         yield return new WaitUntil(() => GoogleSheetHandler.googlesheetInitilized);
         if (this.interstitialAd != null) {
                this.interstitialAd.Dispose();                
            } 
        this.interstitialAd = new InterstitialAd(GoogleSheetHandler.fb_int_id);
        this.interstitialAd.Register(this.gameObject);

        // Set delegates to get notified on changes or when the user interacts with the ad.
        this.interstitialAd.InterstitialAdDidLoad = (delegate() {
            Debug.Log("Interstitial ad loaded.");
            this.isLoaded = true;
        });
        interstitialAd.InterstitialAdDidFailWithError = (delegate(string error) {
            Debug.Log("Interstitial ad failed to load with error: " + error);
        });
        interstitialAd.InterstitialAdWillLogImpression = (delegate() {
            Debug.Log("Interstitial ad logged impression.");
        });
        interstitialAd.InterstitialAdDidClick = (delegate() {
            Debug.Log("Interstitial ad clicked.");  
             StartCoroutine(LoadInterstitial());       
        });
        this.interstitialAd.interstitialAdDidClose = (delegate() {
            Debug.Log("Interstitial ad did close.");
            if (this.interstitialAd != null) {
                this.interstitialAd.Dispose();                
            } 
             StartCoroutine(LoadInterstitial());          
        });
        // Initiate the request to load the ad.
        this.interstitialAd.LoadAd();
    }
    public void ShowInterstitial()
    {
        if(GoogleSheetHandler.show_fb_ad==false)
        return;
        if (this.isLoaded) {
            this.interstitialAd.Show();
            this.isLoaded = false;

        } else {
           // Debug.Log("Interstitial Ad not loaded!");
        }
    }
    #endregion

}
