using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using SimpleJSON;
public class InHouseAdManager : MonoBehaviour
{
    private string spreedSheetApiURL = "https://script.google.com/macros/s/AKfycbyL1BQjJ0bPMvsl99O2eTyHvfl7cLam5z8B3D4ZKHXtBipa_zN1VJHWRVTdKMnxEEIc/exec";
    public delegate void InHouseAd();
    public static InHouseAd onAdCompleted;
    public static InHouseAd onAdClosed;
    public static InHouseAd onAdCancelled;
    public static int ad_counts_we_have;     
    public static InHouseAdManager Instance;

    public List<InHouse_Ad> my_Ads;
    public static bool show_inhouse_ad;
    [SerializeField] int adIndex=0;
    [HideInInspector] public bool apiConfirmed;
    public int iha_starting_count;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckForAPICall());       
    }
    public void CallOnAdCompleted()
    {
        if (onAdCompleted != null)
        {
            onAdCompleted();
        }
    }
    public void CallOnAdClosed()
    {
        if (onAdClosed != null)
        {
            onAdClosed();
        }
    }
    public void ConfirmItems()
    {

    }
    public InHouse_Ad CurrentAd()
    {
        int iND = AdIndex();
        return my_Ads[iND];
    }
    int AdIndex()
    {
        adIndex++;
        if (adIndex >= my_Ads.Count)
        {
            adIndex = 0;
        }
        return adIndex;
    }
    //public int CurrentAdIndex()
    //{

    //}

    #region DATABASE_ANAGER
    IEnumerator CheckForAPICall()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return new WaitUntil(() => Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
            Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork);


            yield return new WaitUntil(() => GoogleSheetHandler.googlesheetInitilized == true);
            RetriveDataFromAPI();
        }
        else
        {
            yield return new WaitUntil(() => GoogleSheetHandler.googlesheetInitilized == true);
            RetriveDataFromAPI();
        }
    }
    //Get Data From Database on App Start(use this method on spash screen to load data
    public void RetriveDataFromAPI()
    {
        StartCoroutine(ApiCallSet(null, SetApiData));
    }

    void SetApiData(string value)
    {
        
        GetVideosFromAPI(value);

       
    }
    private string Checker(string value)
    {
        string tmpstring = value.ToLower();

        return tmpstring;
    }
    IEnumerator ApiCallSet(Action<string> OnFailCallBack, Action<string> OnSuccessCallBack)
    {
        UnityWebRequest request = UnityWebRequest.Get(spreedSheetApiURL);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
           
            yield break;
        }
        OnSuccessCallBack?.Invoke(request.downloadHandler.text);
    }
    #endregion   

    public static string localInfo;
    private static bool BooleanChecker(string value)
    {
        string tmpstring = value.ToLower();
        if (tmpstring == "true") return true;
        else return false;
    }
   
    public void GetVideosFromAPI(string snapshot)
    {
        JSONNode appLink_info = JSON.Parse(snapshot)[0];
        JSONNode itle_info = JSON.Parse(snapshot)[1];
        JSONNode description_info = JSON.Parse(snapshot)[2];
        JSONNode logoLink_info = JSON.Parse(snapshot)[3];
        JSONNode image1Link_info = JSON.Parse(snapshot)[4];
        JSONNode image2Link_info = JSON.Parse(snapshot)[5];
        JSONNode image3Link_info = JSON.Parse(snapshot)[6];
        JSONNode image4Link_info = JSON.Parse(snapshot)[7];
        show_inhouse_ad = GoogleSheetHandler.show_inhouse_ad;
        iha_starting_count = GoogleSheetHandler.iha_starting_count;
        ad_counts_we_have = GoogleSheetHandler.ad_counts_we_have;
        if (show_inhouse_ad == true)
        {
            GameObject InHouseAds = new GameObject();
            InHouseAds.name = "InHouseAds";
            for (int i = 0; i < ad_counts_we_have; i++)
            {
                GameObject myAd = new GameObject();
                int num = i + iha_starting_count;
                string adName = "ad_" + num.ToString();
                myAd.name = "myAd" + num.ToString();
                InHouse_Ad newAd = myAd.AddComponent<InHouse_Ad>();
                newAd.appLink = appLink_info[adName].Value.ToString();
                newAd.appTitle = itle_info[adName].Value.ToString();
                newAd.Description = description_info[adName].Value.ToString();
                newAd.appLogo_Link = logoLink_info[adName].Value.ToString();
                newAd.image1_Link = image1Link_info[adName].Value.ToString();
                newAd.image2_Link = image2Link_info[adName].Value.ToString();
                newAd.image3_Link = image3Link_info[adName].Value.ToString();
                newAd.image4_Link = image4Link_info[adName].Value.ToString();
                myAd.transform.parent = InHouseAds.transform;
                my_Ads.Add(newAd);
            }
            Invoke("ConfirmApi", 2);
        }
    }
    void ConfirmApi()
    {
        if (my_Ads.Count > 0)
        {
            apiConfirmed = true;
        }
       
    }
 }
