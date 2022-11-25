using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardedVideoGroup : MonoBehaviour
{
    public GameObject buttonGroup;
    public GameObject textGroup;
    public TimerText timerText;

    private const string ACTION_NAME = "rewarded_video";

    private void Start()
    {
        if (timerText != null) timerText.onCountDownComplete += OnCountDownComplete;

#if UNITY_ANDROID || UNITY_IOS
        Timer.Schedule(this, 0.1f, AddEvents);

        if (!IsAvailableToShow())
        {
            buttonGroup.SetActive(false);
            if (IsAdAvailable() && !IsActionAvailable())
            {
                int remainTime = 10;//(int)(GameConfig.instance.rewardedVideoPeriod - CUtils.GetActionDeltaTime(ACTION_NAME));
                ShowTimerText(remainTime);
            }
        }

        InvokeRepeating("IUpdate", 1, 1);
#else
        buttonGroup.SetActive(false);
#endif
    }

    private void AddEvents()
    {
        if (RewardedAdManager.CurrentRewardedAd() != null)
        {
            RewardedAdManager.CurrentRewardedAd().OnUserEarnedReward += HandleRewardBasedVideoRewarded;
        }
    }

    private void IUpdate()
    {
        buttonGroup.SetActive(IsAvailableToShow());
    }

    public void OnClick()
    {
        RewardedAdManager.Instance.ShowRewardedAd();
        Sound.instance.PlayButton();
    }

    private void ShowTimerText(int time)
    {
        if (textGroup != null)
        {
            textGroup.SetActive(true);
            timerText.SetTime(time);
            timerText.Run();
        }
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        buttonGroup.SetActive(false);
        ShowTimerText(10);
    }

    private void OnCountDownComplete()
    {
        textGroup.SetActive(false);
        if (IsAdAvailable())
        {
            buttonGroup.SetActive(true);
        }
    }

    public bool IsAvailableToShow()
    {
        return IsActionAvailable() && IsAdAvailable();
    }

    private bool IsActionAvailable()
    {
        return CUtils.IsActionAvailable(ACTION_NAME, 10);
    }

    private bool IsAdAvailable()
    {
        if (RewardedAdManager.CurrentRewardedAd() == null) return false;
        bool isLoaded = RewardedAdManager.CurrentRewardedAd().IsLoaded();
        return isLoaded;
    }

    private void OnDestroy()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (RewardedAdManager.CurrentRewardedAd() != null)
        {
            RewardedAdManager.CurrentRewardedAd().OnUserEarnedReward -= HandleRewardBasedVideoRewarded;
        }
#endif
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            if (textGroup != null && textGroup.activeSelf)
            {
                int remainTime = 10;// (int)(GameConfig.instance.rewardedVideoPeriod - CUtils.GetActionDeltaTime(ACTION_NAME));
                ShowTimerText(remainTime);
            }
        }
    }
}
