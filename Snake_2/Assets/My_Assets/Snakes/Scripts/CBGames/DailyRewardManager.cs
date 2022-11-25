using UnityEngine;

namespace CBGames
{
	public class DailyRewardManager : MonoBehaviour
	{
		[SerializeField]
		private DailyRewardItem[] dailyRewardItems;

		public DailyRewardItem[] DailyRewardItems => dailyRewardItems;
	}
}
