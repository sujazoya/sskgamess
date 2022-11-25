using UnityEngine;

namespace CBGames
{
	public class CoinManager : MonoBehaviour
	{
		private const string PPK_TOTALCOINS = "TOTALCOINS";

		[SerializeField]
		private int initialCoins;

		public int CollectedCoins
		{
			get;
			private set;
		}

		public int TotalCoins
		{
			get
			{
				return PlayerPrefs.GetInt("TOTALCOINS", initialCoins);
			}
			private set
			{
				PlayerPrefs.SetInt("TOTALCOINS", value);
			}
		}

		public void AddTotalCoins(int amount)
		{
			TotalCoins += amount;
			PlayerPrefs.SetInt("TOTALCOINS", TotalCoins);
		}

		public void RemoveTotalCoins(int amount)
		{
			TotalCoins -= amount;
			PlayerPrefs.SetInt("TOTALCOINS", TotalCoins);
		}

		public void AddCollectedCoins(int amount)
		{
			CollectedCoins += amount;
		}

		public void RemoveCollectedCoins(int amount)
		{
			CollectedCoins -= amount;
		}

		public void ResetCollectedCoins()
		{
			CollectedCoins = 0;
		}
	}
}
