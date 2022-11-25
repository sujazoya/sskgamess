using GoogleMobileAds.Api;
using UnityEngine;

public class RewardedVideoCallBack : MonoBehaviour {

    private void Start()
    {
        Timer.Schedule(this, 0.1f, AddEvents);
    }

    private void AddEvents()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (RewardedAdManager.Instance.CurrentRewardedId() != null)
        {
            RewardedAdManager.CurrentRewardedAd().OnUserEarnedReward += HandleRewardBasedVideoRewarded;
        }
#endif
    }

    private const string ACTION_NAME = "rewarded_video";
    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        
    }

    private void OnDestroy()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (RewardedAdManager.Instance.CurrentRewardedId() != null)
        {
            RewardedAdManager.CurrentRewardedAd().OnUserEarnedReward -= HandleRewardBasedVideoRewarded;
        }
#endif
    }
}
