using UnityEngine;

namespace CBGames
{
	public class ServicesManager : MonoBehaviour
	{
		[SerializeField]
		private ScoreManager scoreManager;

		[SerializeField]
		private CoinManager coinManager;

		[SerializeField]
		private SoundManager soundManager;

		[SerializeField]
		private ShareManager shareManager;

//		[SerializeField]
//		private AdManager adManager;

		[SerializeField]
		private DailyRewardManager dailyRewardManager;

		[SerializeField]
		private RewardCoinManager rewardCoinManager;

		[SerializeField]
		private SkinContainer skinContainer;

		public static ServicesManager Instance
		{
			get;
			private set;
		}

		public ScoreManager ScoreManager => scoreManager;

		public CoinManager CoinManager => coinManager;

		public SoundManager SoundManager => soundManager;

		public ShareManager ShareManager => shareManager;

//		public AdManager AdManager => adManager;

		public DailyRewardManager DailyRewardManager => dailyRewardManager;

		public RewardCoinManager RewardCoinManager => rewardCoinManager;

		public SkinContainer SkinContainer => skinContainer;

		private void Awake()
		{
			if ((bool)Instance)
			{
				UnityEngine.Object.DestroyImmediate(base.gameObject);
				return;
			}
			Instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}
}
