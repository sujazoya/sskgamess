using CBGames;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardViewController : MonoBehaviour
{
	[SerializeField]
	private RectTransform dailyRewardPanelTrans;

	[SerializeField]
	private GameObject blackPanel;

	[SerializeField]
	private Text nextRewardCoinTxt;

	[SerializeField]
	private Text nextRewardCountTxt;

	[SerializeField]
	private GameObject claimBtnView;

	[SerializeField]
	private GameObject freeCoinsBtnView;

	[SerializeField]
	private DailyRewardItemController[] dailyRewardItemControls;

	private int currentIndex = -1;

	public bool IsRewardAvailable
	{
		get;
		private set;
	}

	private void Start()
	{
		for (int i = 0; i < ServicesManager.Instance.DailyRewardManager.DailyRewardItems.Length; i++)
		{
			DailyRewardItem dailyRewardItem = ServicesManager.Instance.DailyRewardManager.DailyRewardItems[i];
			dailyRewardItemControls[i].SetValues(dailyRewardItem.GetDayItem, dailyRewardItem.GetRewardedCoins);
		}
		dailyRewardItemControls[0].SetDayTimeValue();
	}

	public void OnShow()
	{
		blackPanel.SetActive(value: true);
		ViewManager.Instance.MoveRect(dailyRewardPanelTrans, dailyRewardPanelTrans.anchoredPosition, new Vector2(dailyRewardPanelTrans.anchoredPosition.x, 60f), 0.75f);
		int num = 0;
		while (true)
		{
			if (num < dailyRewardItemControls.Length)
			{
				if (!dailyRewardItemControls[num].IsClaimed)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		currentIndex = num;
	}

	private void OnDisable()
	{
		dailyRewardPanelTrans.anchoredPosition = new Vector2(dailyRewardPanelTrans.anchoredPosition.x, -375f);
	}

	private void Update()
	{
		if (currentIndex != -1)
		{
			double num = dailyRewardItemControls[currentIndex].TimeRemains();
			nextRewardCountTxt.text = Utilities.SecondsToTimeFormat(num);
			nextRewardCoinTxt.text = ServicesManager.Instance.DailyRewardManager.DailyRewardItems[currentIndex].GetRewardedCoins.ToString();
			if (num > 0.0)
			{
				IsRewardAvailable = false;
				claimBtnView.SetActive(value: false);
			}
			else
			{
				IsRewardAvailable = true;
				claimBtnView.SetActive(value: true);
			}
		}
	}

	public void ClaimBtn()
	{
		ViewManager.Instance.PlayClickButtonSound();
		int getRewardedCoins = ServicesManager.Instance.DailyRewardManager.DailyRewardItems[currentIndex].GetRewardedCoins;
		ServicesManager.Instance.RewardCoinManager.RewardTotalCoins(getRewardedCoins, 0.2f);
		claimBtnView.SetActive(value: false);
		dailyRewardItemControls[currentIndex].SetClaimReward();
		if (currentIndex == dailyRewardItemControls.Length - 1)
		{
			DailyRewardItemController[] array = dailyRewardItemControls;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].DeleteDayTimePlayerPrefs();
			}
			currentIndex = 0;
			dailyRewardItemControls[currentIndex].SetDayTimeValue();
		}
		else
		{
			currentIndex++;
			dailyRewardItemControls[currentIndex].SetDayTimeValue();
		}
		nextRewardCoinTxt.text = ServicesManager.Instance.DailyRewardManager.DailyRewardItems[currentIndex].GetRewardedCoins.ToString();
	}

	public void FreeCoinsBtn()
	{
		ViewManager.Instance.PlayClickButtonSound();
		freeCoinsBtnView.SetActive(value: false);
//		ServicesManager.Instance.AdManager.ShowRewardedVideoAd();
	}

	public void CloseBtn()
	{
		ViewManager.Instance.PlayClickButtonSound();
		blackPanel.SetActive(value: false);
		ViewManager.Instance.HomeViewController.OnDailyRewardViewClose();
		base.gameObject.SetActive(value: false);
	}
}
