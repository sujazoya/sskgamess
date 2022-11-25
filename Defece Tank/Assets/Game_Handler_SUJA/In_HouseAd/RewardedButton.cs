using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System;
[RequireComponent(typeof(Button))]
public class RewardedButton : MonoBehaviour
{
    Button button;
    [SerializeField]  RewardedAdManager rewardedAdManager;
	[SerializeField] private InHouse_Ad_Handler inHouse_Ad;
	public UnityEvent onRewarded;
	public UnityEvent onClose;
	[Space]
	public UnityEvent OnInHouseAdComplete;
	public UnityEvent OnInHouseAdClosed;
	
	string rewardedID;

	private void OnEnable()
	{
		button = GetComponent<Button>();		
		button.onClick.AddListener(OnClick);	
		button.interactable = true;
		//StartCoroutine("AddEvent", 3);
		InHouseAdManager.onAdCompleted += OnAdCompleted;
		InHouseAdManager.onAdClosed += OnAdClosed;
		if(!rewardedAdManager)	{
			rewardedAdManager=FindObjectOfType<RewardedAdManager>();
		}
		if(!inHouse_Ad)	{
			inHouse_Ad=FindObjectOfType<InHouse_Ad_Handler>();
		}
		
	}     
    
	void ShowInHouseAd()
	{
		inHouse_Ad.ShowInHouseAd();
	}
	void OnAdCompleted()
	{
			OnInHouseAdComplete.Invoke();
	}
	void OnAdClosed()
	{
			OnInHouseAdClosed.Invoke();
	}
	//Timer.Schedule(this, 5f, AddEvents);
	//public void CallAddEvent()
	//   {
	//	StartCoroutine(AddEvent());
	//}
    public IEnumerator AddEvent()
	{		
		//button.interactable = false;
		yield return new WaitUntil(() => rewardedAdManager.IsReadyToShowAd()&&gameObject.activeSelf);		
		button.interactable = true;
		//AddEvents();

	}
	RewardedAd rewardedAd()
    {
		return RewardedAdManager.CurrentRewardedAd();     

	}
	//private void AddEvents()
	//{
	//	if (rewardedAdManager.IsReadyToShowAd())
	//	{
	//		rewardedAd().OnUserEarnedReward += HandleRewardBasedVideoRewarded;
	//		rewardedAd().OnAdClosed += HandleRewardedAdClosed;

	//	}
	//}
	int adCount;
	public void OnClick()
	{
		adCount++;
		if(adCount> RewardedAdManager.show_rewarded_onrequest_count)
        {
			adCount = 1;
		}
		if(GoogleSheetHandler.show_fb_ad==false&&GoogleSheetHandler.show_inhouse_ad==false){

			if (RewardedAdManager.show_rewarded && adCount == RewardedAdManager.show_rewarded_onrequest_count)
			{
				if (rewardedAdManager.IsReadyToShowAd())
				{
					rewardedAd().OnUserEarnedReward -= HandleRewardBasedVideoRewarded;
					rewardedAd().OnAdClosed -= HandleRewardedAdClosed;
					rewardedAd().OnUserEarnedReward += HandleRewardBasedVideoRewarded;
					rewardedAd().OnAdClosed += HandleRewardedAdClosed;
					rewardedAdManager.ShowRewardedAd();
				}
				
			}
		}		 
		 if (GoogleSheetHandler.show_inhouse_ad==true)
        {
			ShowInHouseAd();
		}		
	}	
	public void HandleRewardBasedVideoRewarded(object sender, Reward args)
	{
		onRewarded.Invoke();
		//rewardedAdManager.RequestRewardedAd_Shop(rewardedAdManager.CurrentRewardedAd_ID());

	}
	public void HandleRewardedAdClosed(object sender, EventArgs args)
	{
		onClose.Invoke();
		//rewardedAdManager.RequestRewardedAd_Shop(rewardedAdManager.CurrentRewardedAd_ID());
	}
	private void OnDisable()
	{		
		InHouseAdManager.onAdCompleted -= OnAdCompleted;
		InHouseAdManager.onAdClosed -= OnAdClosed;
	}
}


