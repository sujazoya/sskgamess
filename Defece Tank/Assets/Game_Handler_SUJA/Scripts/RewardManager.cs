using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    public Button rewardButton;
    public Button closeButton;  
    public GameObject rewardPanel;
    public Text diamondCount;
     public Text reward_Count;
     public Slider RewardedSlider;

     #region SINGLETON
     public static RewardManager _instance;
     private void Awake() {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
     }
     #endregion

    // Start is called before the first frame update
    void Start()
    {
        if(rewardButton)
        {
            rewardButton.onClick.AddListener(OpenRewardPanel);
        }
        if(closeButton)
        {
            closeButton.onClick.AddListener(CloseRewardPanel);
        }
        if(RewardedSlider)
        {
            RewardedSlider.minValue=0;
            RewardedSlider.maxValue=10;
            RewardedSlider.value=RewardCount;
        }
        CloseRewardPanel();
        UpdateStatus();
    }
    void OpenRewardPanel()
    {
        if(rewardPanel)
        {
            rewardPanel.SetActive(true);
        }
    }
     void CloseRewardPanel()
    {
        if(rewardPanel)
        {
            rewardPanel.SetActive(false);
        }
    }
    public void GiveReward()
    {
        Game.TotalDiemonds++;
        RewardCount++;
        if(RewardCount>10)
        {
            RewardCount=0;
        }
        UpdateStatus() ;
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void UpdateStatus() 
    {
         if(diamondCount)
        {
            diamondCount.text=Game.TotalDiemonds.ToString();
        }
       if(reward_Count)
        {
            reward_Count.text = RewardCount.ToString()+" / "+10;
        }
         RewardedSlider.value=RewardCount;
        
    }
    public static int RewardCount
	{
		get { return PlayerPrefs.GetInt("RewardCount", 0); }
		set { PlayerPrefs.SetInt("RewardCount", value); }
	}
    public void ShowRewardedAd ()
    {
        
    }
}
