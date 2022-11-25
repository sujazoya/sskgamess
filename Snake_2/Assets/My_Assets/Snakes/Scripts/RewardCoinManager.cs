using CBGames;
using System.Collections;
using UnityEngine;

public class RewardCoinManager : MonoBehaviour
{
	public void RewardTotalCoins(int amount, float delay)
	{
		StartCoroutine(CRRewardingTotalCoins(amount, delay));
	}

	public void RewardCollectedCoins(int amount, float delay)
	{
		StartCoroutine(CRRewardingCollectedCoins(amount, delay));
	}

	public void RemoveCollectedCoins(float delay)
	{
		StartCoroutine(CRRemoveCollectedCoins(delay));
	}

	private IEnumerator CRRewardingTotalCoins(int amount, float delay)
	{
		ServicesManager.Instance.SoundManager.PlayOneSound(ServicesManager.Instance.SoundManager.rewarded);
		yield return new WaitForSeconds(delay);
		for (int i = 1; i <= amount; i++)
		{
			ServicesManager.Instance.CoinManager.AddTotalCoins(1);
			yield return new WaitForSeconds(0.02f);
		}
	}

	private IEnumerator CRRewardingCollectedCoins(int amount, float delay)
	{
		ServicesManager.Instance.SoundManager.PlayOneSound(ServicesManager.Instance.SoundManager.rewarded);
		yield return new WaitForSeconds(delay);
		for (int i = 1; i <= amount; i++)
		{
			ServicesManager.Instance.CoinManager.AddCollectedCoins(1);
			yield return new WaitForSeconds(0.02f);
		}
	}

	private IEnumerator CRRemoveCollectedCoins(float delay)
	{
		yield return new WaitForSeconds(delay);
		int amount = ServicesManager.Instance.CoinManager.CollectedCoins;
		for (int i = 1; i <= amount; i++)
		{
			ServicesManager.Instance.CoinManager.RemoveCollectedCoins(1);
			yield return new WaitForSeconds(0.02f);
		}
	}
}
