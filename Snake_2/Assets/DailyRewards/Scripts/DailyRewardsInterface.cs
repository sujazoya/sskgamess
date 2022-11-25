/***************************************************************************\
Project:      Daily Rewards
Copyright (c) Niobium Studios.
Author:       Guilherme Nunes Barbosa (gnunesb@gmail.com)
\***************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using System;

/* 
 * Daily Rewards Canvas is the User interface to show Daily rewards using Unity 4.6
 */
namespace com.niobiumstudios.dailyrewards
{
    public class DailyRewardsInterface : MonoBehaviour
    {

        // Prefab containing the daily reward
        public GameObject dailyRewardPrefab;

        // Rewards panel
        public GameObject panelReward;
        public Text txtReward;

        // Claim Button
        public Button btnClaim;

        // How long until next claim
        public Text txtTimeDue;

        // The Grid that contains the rewards
        public GridLayoutGroup dailyRewardsGroup;
        [SerializeField] GameObject dalyRewardItem;
        void Start ()
        {
            DailyRewards.instance.CheckRewards();

            DailyRewards.instance.onClaimPrize += OnClaimPrize;
            DailyRewards.instance.onPrizeAlreadyClaimed += OnPrizeAlreadyClaimed;

            UpdateUI();
        }

        void OnDestroy()
        {
            DailyRewards.instance.onClaimPrize -= OnClaimPrize;
            DailyRewards.instance.onPrizeAlreadyClaimed -= OnPrizeAlreadyClaimed;
        }

        // Clicked the claim button
        public void OnClaimClick()
        {
            DailyRewards.instance.ClaimPrize(DailyRewards.instance.availableReward);
            UpdateUI();
            AdmobAdmanager.Instance.ShowInterstitial();
        }

        public void UpdateUI()
        {
            foreach (Transform child in dailyRewardsGroup.transform)
            {
                Destroy(child.gameObject);
            }

            bool isRewardAvailableNow = false;

            for (int i = 0; i < DailyRewards.instance.rewards.Count; i++)
            {
                int reward = DailyRewards.instance.rewards[i];
                int day = i + 1;

                GameObject dailyRewardGo = GameObject.Instantiate(dailyRewardPrefab) as GameObject;

                DailyRewardUI dailyReward = dailyRewardGo.GetComponent<DailyRewardUI>();
                dailyReward.transform.SetParent(dailyRewardsGroup.transform);
                dailyRewardGo.transform.localScale = Vector2.one;

                dailyReward.day = day;
                dailyReward.reward = reward;

                dailyReward.isAvailable = day == DailyRewards.instance.availableReward;
                dailyReward.isClaimed = day <= DailyRewards.instance.lastReward;

                if (dailyReward.isAvailable)
                {
                    isRewardAvailableNow = true;
                }

                dailyReward.Refresh();
            }

            btnClaim.gameObject.SetActive(isRewardAvailableNow);
            dalyRewardItem.SetActive(isRewardAvailableNow);
            txtTimeDue.gameObject.SetActive(!isRewardAvailableNow);
        }

        void Update()
        {
            if (txtTimeDue.IsActive())
            {
                TimeSpan difference = (DailyRewards.instance.lastRewardTime - DailyRewards.instance.timer).Add(new TimeSpan(0, 24, 0, 0));

                // Is the counter below 0? There is a new reward then
                if (difference.TotalSeconds <= 0)
                {
                    DailyRewards.instance.CheckRewards();
                    UpdateUI();
                    return;
                }

                string formattedTs = string.Format("{0:D2}:{1:D2}:{2:D2}", difference.Hours, difference.Minutes, difference.Seconds);

                txtTimeDue.text = "Return in " + formattedTs + " to claim your reward";
            }
        }

        // Delegate
        private void OnClaimPrize(int day)
        {
            panelReward.SetActive(true);
            txtReward.text = "You got " + DailyRewards.instance.rewards[day-1] + " coins!";
            Game.TotalCoins += DailyRewards.instance.rewards[day - 1];
        }

        // Delegate
        private void OnPrizeAlreadyClaimed(int day)
        {
            // Do Something with the prize already claimed
            MessegeManager.insance.ShowPopup("Prize Already Claimed");
        }

        // Close the Rewards panel
        public void OnCloseRewardsClick()
        {
            panelReward.SetActive(false);
        }

        // Resets player preferences
        public void OnResetClick()
        {
            DailyRewards.instance.Reset();
            DailyRewards.instance.lastRewardTime = System.DateTime.MinValue;
            DailyRewards.instance.CheckRewards();
            UpdateUI();
        }

        // Simulates the next day
        public void OnAdvanceDayClick()
        {
            DailyRewards.instance.timer = DailyRewards.instance.timer.AddDays(1);
            DailyRewards.instance.CheckRewards();
            UpdateUI();
        }

        // Simulates the next hour
        public void OnAdvanceHourClick()
        {
            DailyRewards.instance.timer = DailyRewards.instance.timer.AddHours(1);
            DailyRewards.instance.CheckRewards();
            UpdateUI();
        }
        public void ShowRewadPanel()
        {
            dalyRewardItem.SetActive(true);
            AdmobAdmanager.Instance.ShowInterstitial();
        }
        public void CloseRewadPanel()
        {
            dalyRewardItem.SetActive(false);
            AdmobAdmanager.Instance.ShowInterstitial();
        }

    }    

}