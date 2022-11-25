using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using SimpleJSON;
public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;

    private string apiURL = "https://script.google.com/macros/s/AKfycbz3FVCJZ0FfWZIfkW8U5WCsYqMZYDf2ck5ifmxJKtyuq1yKkj_avZrRhnhII6F_ox4g2A/exec";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        //UIController.Instance.CheckInternet(RetriveDataFromAPI);
        StartCoroutine(CheckForAPICall());

    }
    IEnumerator CheckForAPICall()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return new WaitUntil(() => Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
            Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork);
            RetriveDataFromAPI();
        }
        else
        {
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
        GoogleSheetHandler.GetDataFromAPI(value);
       
        //AdsManager.Instance.LoadInterstitialAd(0);
        //AdsManager.Instance.LoadInterstitialAd(1);
        //AdsManager.Instance.LoadInterstitialAd(2);

        //AdsManager.Instance.LoadRewardedAd(0);
        //AdsManager.Instance.LoadRewardedAd(1);
    }   
    private string Checker(string value)
    {
        string tmpstring = value.ToLower();
       
        return tmpstring;
    }
    IEnumerator ApiCallSet(Action<string> OnFailCallBack, Action<string> OnSuccessCallBack)
    {
        UnityWebRequest request = UnityWebRequest.Get(apiURL);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            /*if (internetConnectivity())
            {
                RetriveDataFromAPI();
            }*/
            yield break;
        }
        OnSuccessCallBack?.Invoke(request.downloadHandler.text);
    }
   
    ///Its Check Internet Connection
/*
    public bool internetConnectivity()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://www.google.com/");
        request.SendWebRequest();

        if (request.error != null)
        {
           
            Debug.LogWarning("No internet connection");
            return false;
        }
        else
        {
            return true;
        }
    }*/

    ///

}