using System;
using UnityEngine;

namespace CBGames
{
	[Serializable]
	public class DailyRewardItem
	{
		[SerializeField]
		private DayItem DayItem;

		[SerializeField]
		private int RewardedCoins;

		public DayItem GetDayItem => DayItem;

		public int GetRewardedCoins => RewardedCoins;
	}
}
