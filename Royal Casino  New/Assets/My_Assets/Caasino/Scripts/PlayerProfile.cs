using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System;
public class PlayerProfile : MonoBehaviour
{
    public Text playerName;
    public Text playerBalance;
    public Text playerDiemonds;
    public Text playerCoins;
    public Button buyDiemondButton;
    public Button buyCoinsButton;
    public Button watchAdButton;
    PlayerManager playerManager;

    Casino casino;
    [HideInInspector] public int playerIndex;

    // Start is called before the first frame update
    private void OnEnable()
    {
        buyDiemondButton.onClick.     AddListener(BuyDiemond);
        buyCoinsButton.onClick.       AddListener(BuyCoins);
        watchAdButton.onClick.        AddListener(EarnMoney);
        playerManager = FindObjectOfType<PlayerManager>();
        casino = FindObjectOfType<Casino>();
        UpdatePlayerStatus();
    }

    void UpdatePlayerStatus()
    {
        playerName.text = playerManager.curentPlayer.playerName;
        playerBalance.text = playerManager.curentPlayer.balance.ToString();
        playerDiemonds.text = playerManager.curentPlayer.diemond.ToString();
        playerCoins.text = playerManager.curentPlayer.coin.ToString();
        casino.UpdatePlayerStatus(playerManager.curentPlayer, playerIndex);
    }
    public void BuyDiemond()
    {
        if (playerManager.curentPlayer.balance >= 500)
        {
            playerManager.curentPlayer.balance -= 500;
            playerManager.curentPlayer.diemond += 10;
            playerManager.SaveCurrentPlayer(playerManager.curentPlayer.playerName);
            UpdatePlayerStatus();

        }
    }
    public void BuyCoins()
    {
        if (playerManager.curentPlayer.balance >= 200)
        {
            playerManager.curentPlayer.balance -= 200;
            playerManager.curentPlayer.coin += 10;
            playerManager.SaveCurrentPlayer(playerManager.curentPlayer.playerName);
            UpdatePlayerStatus();
        }
    }
    public void GiveRewards()
    {            
            playerManager.curentPlayer.balance += 100;
            playerManager.SaveCurrentPlayer(playerManager.curentPlayer.playerName);
            UpdatePlayerStatus();
            Casino.Instance.ShowMassege(playerManager.curentPlayer.playerName+" "+"You Got 100 Credits");
    }
    public void EarnMoney()
    {
        // ShowAd
        RewardedAdManager.Instance.ShowRewardedAd();
        AddEvents();
    }
    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        //onRewarded.Invoke();
        GiveRewards();
    }
    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        //onClose.Invoke();
    }
    private void AddEvents()
    {
        if (RewardedAdManager.Instance.IsReadyToShowAd())
        {
            RewardedAdManager.CurrentRewardedAd().OnUserEarnedReward += HandleRewardBasedVideoRewarded;
            RewardedAdManager.CurrentRewardedAd().OnAdClosed += HandleRewardedAdClosed;

        }
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
