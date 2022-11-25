using UnityEngine;

namespace CBGames
{
	public class ScoreManager : MonoBehaviour
	{
		private const string BESTSCORE = "BESTSCORE";

		public int CurrentScore
		{
			get;
			private set;
		}

		public int BestScore
		{
			get
			{
				return PlayerPrefs.GetInt("BESTSCORE", 0);
			}
			private set
			{
				PlayerPrefs.SetInt("BESTSCORE", value);
			}
		}

		private void Start()
		{
			CurrentScore = 0;
		}

		public void AddCurrentScore(int amount)
		{
			CurrentScore += amount;
			if (CurrentScore > BestScore)
			{
				UpdateBestScore(CurrentScore);
			}
		}

		public void ResetCurrentScore()
		{
			CurrentScore = 0;
		}

		private void UpdateBestScore(int newBestScore)
		{
			BestScore = newBestScore;
		}
	}
}
