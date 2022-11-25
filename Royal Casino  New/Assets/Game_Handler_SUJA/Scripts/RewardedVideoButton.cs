/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System;

public class RewardedVideoButton : MonoBehaviour
{
	private const string ACTION_NAME = "rewarded_video";
	public UnityEvent onRewarded;
	public UnityEvent onClose;
	Button button;
	private void Start()
	{	
		//Timer.Schedule(this, 5f, AddEvents);
		StartCoroutine(AddEvent());
	}
	public IEnumerator AddEvent()
	{
		button=transform.GetComponent<Button>();
        if (button)
        {
			button.onClick.AddListener(OnClick);
		}
		
		//button.interactable = false;
		yield return new WaitUntil(() => AdmobAdmanager.readyToShoAd==true);
		AddEvents();
		button.interactable = true; 

	}

	private void AddEvents()
	{
		if (RewardedAdManager.Instance.IsReadyToShowAd())
		{
			RewardedAdManager.CurrentRewardedAd().OnUserEarnedReward += HandleRewardBasedVideoRewarded;
			RewardedAdManager.CurrentRewardedAd().OnAdClosed += HandleRewardedAdClosed;
			
		}
	}

	public void OnClick()
	{		
		RewardedAdManager.Instance.ShowRewardedAd();
		AddEvents();
	}

	public void HandleRewardBasedVideoRewarded(object sender, Reward args)
	{
		onRewarded.Invoke();
	}
	public void HandleRewardedAdClosed(object sender, EventArgs args)
	{
		onClose.Invoke();
	}
	public bool IsAdAvailable()
	{
		if (!AdmobAdmanager.readyToShoAd )
		{
			return false;
		}
		return true;
	}

	private void OnDisable()
	{
		if (RewardedAdManager.Instance.IsReadyToShowAd())
		{
			RewardedAdManager.CurrentRewardedAd().OnUserEarnedReward -= HandleRewardBasedVideoRewarded;
			RewardedAdManager.CurrentRewardedAd().OnAdClosed -= HandleRewardedAdClosed;
		}
	}
}
