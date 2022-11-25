using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using UnityEngine.Networking;

namespace suja
{

    [System.Serializable]
    public class GameHandler_items
    {
        #region AD APP NOTIFICATION
        public GameObject ad_app_Panel;
        public Text ad_app_name;
        public Text ad_app_short_desc;
        public Image app_icon;
        public Button ad_App_buton;
        public Button close_buton;
        public Image app_banner_image;
        public Text ad_dialog_title;
        public Text ad_message;
        #endregion
        #region UPDATE NOTIFICATION
        public GameObject notification_Panel;
       public Image app_new_icon;
       public Button UpdateButton;
       public Button cancelButton;
       public Text update_dialog_title;
       public Text update_message;
       public Text update_title;
       public Text button_text;
       public Text update_version_name;
        #endregion

        [Header("Net Checker Iems")]
        public GameObject warningPanel;
        public Button closeButton;
        public GameObject termAndConditionPanel;  
    }

    public class GameHandler : MonoBehaviour
    {
        [SerializeField] GameHandler_items items;       
              
        public static bool isNetworkAvailable;
        public static bool isGoogleSheetDataConfirmed;
        
        

       

        public bool haveTermAndCondition;
       [HideInInspector] public bool termAndConditionAccepted;
        private string popup_messege = "popup_messege";
        public GameObject[] myObjects;
        private static GameHandler _instance;
        [SerializeField] Button otherGameButton;

        void Start()
        {
            if (items.closeButton) { items.closeButton.onClick.AddListener(CloseNetWarnPanel); }
            if (otherGameButton) { otherGameButton.onClick.AddListener(GoForOtherGames); }
            //PlayerPrefs.DeleteAll();
        }
        void GoForOtherGames()
        {
            string link = GoogleSheetHandler.other_games;
            Application.OpenURL(link);
        }

        public static GameHandler Instance { get { return _instance; } }
        public void ShowMassege(string header, string massege)
        {
            //items.massege_animator.SetTrigger(popup_messege);
            //items.massege_header.text = header;
            //items.massege.text = massege;
        }




        #region CHECKING UPDATE
        public void CheckForUpdate()
        {
            //Debug.Log("Application Version: " + Application.version);
            if (GoogleSheetHandler.isUpdate==false || GoogleSheetHandler.update_version_name == Application.version)
                return;
            GetUpdateItems();
            items.notification_Panel.SetActive(true);
           items.update_dialog_title.text = GoogleSheetHandler.update_dialog_title;
           items.update_message.text = GoogleSheetHandler.update_message;           
           items.update_title.text = GoogleSheetHandler.update_title;
           items.button_text.text = GoogleSheetHandler.button_text;
           items.update_version_name.text = GoogleSheetHandler.update_version_name;
           StartCoroutine(GetImage(GoogleSheetHandler.update_icon_url, items.app_new_icon));
            items.UpdateButton.onClick.AddListener(OpenApp);
           items. cancelButton.onClick.AddListener(CncelNotif);
            if (GoogleSheetHandler.update_show_cancel == true)
            {
                items.cancelButton.gameObject.SetActive(true);
            }
            items.notification_Panel.transform.GetComponent<Animator>().SetTrigger("show");      
           
        }
        void GetUpdateItems()
        {

           
            if (!items.notification_Panel) { items.notification_Panel = GameObject.Find("Canvas/Update_Notication"); }
            if (!items.update_dialog_title) { items.update_dialog_title = items.notification_Panel.transform.Find("update_dialog_title").GetComponent<Text>(); }
            if (!items.update_version_name) { items.update_version_name = items.notification_Panel.transform.Find("update_version_name").GetComponent<Text>(); }
            if (!items.update_message) { items.update_message = items.notification_Panel.transform.Find("update_message").GetComponent<Text>(); } 
            if (!items.app_new_icon) { items.app_new_icon = items.notification_Panel.transform.Find("Logo__/mask/logo").GetComponent<Image>(); } 
            if (!items.UpdateButton) { items.UpdateButton = items.notification_Panel.transform.Find("bottons/Update_Button").GetComponent<Button>(); }
            if (!items.cancelButton) { items.cancelButton = items.notification_Panel.transform.Find("bottons/Cancel_Button").GetComponent<Button>(); }           
            if (!items.update_title) { items.update_title = items.notification_Panel.transform.Find("update_title").GetComponent<Text>(); } 
            if (!items.button_text) { items.button_text = items.UpdateButton.transform.Find("Text").GetComponent<Text>(); }            

        }
        #endregion
        public void ShowTerms()
        {
            if (GoogleSheetHandler.has_terms&& Confirmed==0)
            {
                items.termAndConditionPanel.SetActive(true);
                items.termAndConditionPanel.GetComponent<Animator>().SetTrigger("show");
            }
        }
        #region CheckForNtification        
        public void CheckForNtification()
        {
            if (!GoogleSheetHandler.isNotification)
                return;
            GetNotificationItems();
            items.ad_app_Panel.SetActive(true);
            showNotification = true;
            items.ad_app_name.text = GoogleSheetHandler.ad_app_name;
            items.ad_app_short_desc.text = GoogleSheetHandler.ad_app_short_desc;
            string ad_icon_url = GoogleSheetHandler.ad_icon_url;
            items.close_buton.onClick.AddListener(CloseNotification);
            //StartCoroutine(GetTexture(items.app_icon, ad_icon_url)) ;
            StartCoroutine(GetImage(ad_icon_url, items.app_icon));
            items.ad_App_buton.onClick.AddListener(Ad_App_Button);
            string ad_banner_url = GoogleSheetHandler.ad_banner_url;
            StartCoroutine(GetImage(ad_banner_url, items.app_banner_image));
            items.ad_dialog_title.text= GoogleSheetHandler.ad_dialog_title;
            items.ad_message.text = GoogleSheetHandler.ad_message;
            if (!GoogleSheetHandler.ad_cancel_buton)
            {
                items.close_buton.gameObject.SetActive(false);
            }
            else
            {
                items.close_buton.gameObject.SetActive(true);
            }
            //StartCoroutine(CompleteNtification());
        }
        void GetNotificationItems()
        {            
           if (!items.ad_app_Panel) { items.ad_app_Panel = GameObject.Find("Canvas/AdApp_Panel"); } 
           if (!items.ad_app_name) { items.ad_app_name = items.ad_app_Panel.transform.Find("app_name").GetComponent<Text>(); }  
           if (!items.ad_app_short_desc) { items.ad_app_short_desc = items.ad_app_Panel.transform.Find("short_description").GetComponent<Text>(); } 
           if (!items.app_icon) { items.app_icon = items.ad_app_Panel.transform.Find("Logo__/mask/logo").GetComponent<Image>(); } 
           if (!items.ad_App_buton) { items.ad_App_buton = items.ad_app_Panel.transform.Find("bottons/ad_App_buton").GetComponent<Button>(); }
           if (!items.close_buton) { items.close_buton = items.ad_app_Panel.transform.Find("bottons/Cancel_Button").GetComponent<Button>(); } 
           if (!items.app_banner_image) { items.app_banner_image = items.ad_app_Panel.transform.Find("banner/Scroll View/Viewport/Content/banner_img").GetComponent<Image>(); }
           if (!items.ad_message) { items.ad_message = items.ad_app_Panel.transform.Find("Massege_View/Text_Container/massege").GetComponent<Text>(); }
           if (!items.ad_dialog_title) { items.ad_app_Panel.transform.Find("ad_dialog_title_Button/ad_dialog_title").GetComponent<Text>(); }
        }
       
        bool showNotification;
        bool logo_Goted()
        {
            if (items.app_icon.sprite != null)
                return true;
            else
                return false;
        }
        bool banner_gotted()
        {
            if (items.app_icon.sprite != null)
                return true;
            else
                return false;
        }
       void CloseNotification()
        {
            items.ad_app_Panel.GetComponent<Animator>().SetTrigger("off");
        }
        #endregion

        //IEnumerator CompleteNtification()
        //{

        //    while (ad_AppIconGot == false)
        //    {
        //        yield return null;
        //    }           
        //    //items.app_icon.sprite = ad_AppIcon;
        //    while (ad_bannerGot == false)
        //    {
        //        yield return null;
        //    }
        //    //yield return new WaitUntil(() => ad_bannerGot == true);
        //    //items.app_banner_image.sprite = ad_banner;
        //}


        #region Getting Google Drive Json Data

        //public struct Data
        //{
        //    public string Name;
        //    public string ImageURL;
        //}
        //public string dataName;
        //IEnumerator GetString(string url)
        //{
        //    UnityWebRequest request = UnityWebRequest.Get(url);
        //    yield return request.SendWebRequest();
        //    if (request.isNetworkError || request.isHttpError)
        //    {
        //        Debug.Log(request.error);
        //    }
        //    else
        //    {
        //        Data data = JsonUtility.FromJson<Data>(request.downloadHandler.text);
        //        dataName = data.Name;
        //        StartCoroutine(GetImage(url,items.app_icon));
        //    }
        //    request.Dispose();
        //}

        #endregion

        int callIndex;
        IEnumerator GetImage(string url ,Image image)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Texture2D webTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                image.overrideSprite = SpriteFromTexture2D(webTexture);
                callIndex++;
                if (callIndex == 2)
                {
                    callIndex = 0;
                    StartCoroutine(TryToShowNotification());
                }                
            }           
            request.Dispose();
        }
        IEnumerator TryToShowNotification()
        {
            while (!showNotification && !logo_Goted() && !banner_gotted())
            {
                yield return null;
            }
            items.ad_app_Panel.GetComponent<Animator>().SetTrigger("show");
        }
        void Ad_App_Button()
        {
            string ad_app_url;
            ad_app_url = GoogleSheetHandler.ad_app_url;
            Application.OpenURL(ad_app_url);
            items.ad_app_Panel.GetComponent<Animator>().SetTrigger("off");
        }
            
        Sprite SpriteFromTexture2D(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
        void OpenApp()
        {
            string update_app_url;
            update_app_url = GoogleSheetHandler.update_app_url;
            Application.OpenURL(update_app_url);
            items.notification_Panel.transform.GetComponent<Animator>().SetTrigger("off");

        }
        void CncelNotif()
        {
            items.notification_Panel.transform.GetComponent<Animator>().SetTrigger("off");
        }

        public enum AdShowGameState
        {
            HowToPlay,
            GameOver,
            Exit,
            BackToHome,
            ShopClose,
            reload
        }
      
        private void Awake()
        {
            if (myObjects.Length > 0)
            {
                for (int i = 0; i < myObjects.Length; i++)
                {
                    myObjects[i].SetActive(true);
                }
            }
            
            #region Instance
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            #endregion Instance

            DeActivate_Items();
            StartCoroutine(CheckNetworks(0));


            if (Confirmed > 0)
            {
                termAndConditionAccepted = true;
            }
            if (haveTermAndCondition && !termAndConditionAccepted)
            {
                items.termAndConditionPanel.SetActive(true);
            }
            else
            {
                items.termAndConditionPanel.SetActive(false);
            }
        }
        public void OpenTerms()
        {
            Application.OpenURL(GoogleSheetHandler.terms_url);
        }
        public void AcceptTermAndConditiions()
        {
            termAndConditionAccepted = true;
            Confirmed++;
            items.termAndConditionPanel.GetComponent<Animator>().SetTrigger("off");
            StartCoroutine(ClosetermsPanel());
        }
        IEnumerator ClosetermsPanel()
        {
            yield return new WaitForSeconds(1);
                items.termAndConditionPanel.SetActive(false);            
        }
        public static float RecordDistance
        {
            get { return PlayerPrefs.GetFloat("RecordDistance", 0.0f); }
            set { PlayerPrefs.SetFloat("RecordDistance", value); }
        }       
        public static int Confirmed
        {
            get { return PlayerPrefs.GetInt("Confirmed", 0); }
            set { PlayerPrefs.SetInt("Confirmed", value); }
        }        
        void DeActivate_Items()
        {
            items.warningPanel.SetActive(false);
        }
        public IEnumerator CheckNetworks(float wait)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                items.warningPanel.SetActive(true);              
                items.warningPanel.GetComponent<Animator>().SetTrigger("show");
                //if (GoogleSheetHandler.net_warn_cancel_buton == true)
                //{
                //    items.closeButton.gameObject.SetActive(true);
                //}
                yield return new WaitUntil(() => Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
                Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork);
                items.warningPanel.GetComponent<Animator>().SetTrigger("off");
                //If Nenwork Availed
                yield return new WaitForSeconds(0.7f);
                items.warningPanel.SetActive(false);
                isNetworkAvailable = true;
            }
            else
            {
                items.warningPanel.SetActive(false);
                isNetworkAvailable = true;
            }
            yield return new WaitForSeconds(wait);
            StartCoroutine(CheckNetworks(120));
        }
        void CloseNetWarnPanel()
        {
            StartCoroutine(ActCloseNetWarnPanel());            
        }
        IEnumerator ActCloseNetWarnPanel()
        {
            items.warningPanel.GetComponent<Animator>().SetTrigger("off");
            //If Nenwork Availed
            yield return new WaitForSeconds(0.7f);
            //isNetworkAvailable = true;
            items.warningPanel.SetActive(false);           
        }
        // Start is called before the first frame update
       
        #region googleShetapi

        #endregion googleShetapi

        // Update is called once per frame
        void Update()
        {

        }
    }
}
