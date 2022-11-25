using CBGames;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardItemController : MonoBehaviour
{
	[Header("Item References")]
	[SerializeField]
	private Text rewardDayTxt;

	[SerializeField]
	private Text rewardedCoinTxt;

	[SerializeField]
	private GameObject lockPanel;

	private DayItem dayItem;

	private int rewardedCoins;

	public bool IsClaimed => PlayerPrefs.GetInt(IS_CLAIMED_PPK, 0) == 1;

	private string IS_CLAIMED_PPK => "CLAIMED_" + dayItem.ToString();

	private string SAVED_DAY_TIME_PPK => "TIME_" + dayItem.ToString();

	public void SetValues(DayItem dayItem, int rewardedCoins)
	{
		this.dayItem = dayItem;
		this.rewardedCoins = rewardedCoins;
		rewardDayTxt.text = ("DAY " + this.dayItem.ToString().Split('_')[1]).ToUpper();
		rewardedCoinTxt.text = rewardedCoins.ToString();
		if (!IsClaimed)
		{
			lockPanel.SetActive(value: false);
		}
		else
		{
			lockPanel.SetActive(value: true);
		}
	}

	public void SetDayTimeValue()
	{
		int year = DateTime.Now.Year;
		int month = DateTime.Now.Month;
		int day = DateTime.Now.Day;
		int hour = DateTime.Now.Hour;
		int minute = DateTime.Now.Minute;
		int second = DateTime.Now.Second;
		string value = year + ":" + month + ":" + day + ":" + hour + ":" + minute + ":" + second;
		PlayerPrefs.SetString(SAVED_DAY_TIME_PPK, value);
	}

	public void SetClaimReward()
	{
		PlayerPrefs.SetInt(IS_CLAIMED_PPK, 1);
		lockPanel.SetActive(value: true);
	}

	public void DeleteDayTimePlayerPrefs()
	{
		PlayerPrefs.DeleteKey(SAVED_DAY_TIME_PPK);
	}

	public double TimeRemains()
	{
		string @string = PlayerPrefs.GetString(SAVED_DAY_TIME_PPK);
		int year = int.Parse(@string.Split(':')[0]);
		int month = int.Parse(@string.Split(':')[1]);
		int day = int.Parse(@string.Split(':')[2]);
		int hour = int.Parse(@string.Split(':')[3]);
		int minute = int.Parse(@string.Split(':')[4]);
		int second = int.Parse(@string.Split(':')[5]);
		DateTime value = new DateTime(year, month, day, hour, minute, second);
		TimeSpan timeSpan = DateTime.Now.Subtract(value);
		double num = ((dayItem == DayItem.DAY_1) ? 0.0 : (24.0 - timeSpan.TotalHours)) * 60.0 * 60.0;
		if (num <= 0.0)
		{
			num = 0.0;
		}
		return num;
	}
}
