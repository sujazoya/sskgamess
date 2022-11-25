using CBGames;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverViewController : MonoBehaviour
{
	[SerializeField]
	private RectTransform topBarTrans;

	[SerializeField]
	private RectTransform bottomBarTrans;

	[SerializeField]
	private RectTransform inforViewTrans;

	[SerializeField]
	private Text bestDistanceTxt;

	[SerializeField]
	private Text totalCoinsTxt;

	[SerializeField]
	private Text currentScoreTxt;

	[SerializeField]
	private Text bestScoreTxt;

	[SerializeField]
	private GameObject newBestScoreImg;

	[SerializeField]
	private Text collectedCoinsTxt;

	[SerializeField]
	private GameObject doubleCoinBtnView;

	[SerializeField]
	private GameObject collectCoinsBtnView;

	public void OnShow()
	{
//		doubleCoinBtnView.SetActive(ServicesManager.Instance.AdManager.IsRewardedVideoAdReady());
		collectCoinsBtnView.SetActive(value: true);
		ViewManager.Instance.ScaleRect(inforViewTrans, Vector2.zero, Vector2.one, 1f);
		ViewManager.Instance.MoveRect(topBarTrans, topBarTrans.anchoredPosition, new Vector2(topBarTrans.anchoredPosition.x, 0f), 1f);
		ViewManager.Instance.MoveRect(bottomBarTrans, bottomBarTrans.anchoredPosition, new Vector2(topBarTrans.anchoredPosition.x, 0f), 1f);
		currentScoreTxt.text = ServicesManager.Instance.ScoreManager.CurrentScore.ToString();
		bestScoreTxt.text = ServicesManager.Instance.ScoreManager.BestScore.ToString();
		if (ServicesManager.Instance.ScoreManager.CurrentScore == ServicesManager.Instance.ScoreManager.BestScore)
		{
			newBestScoreImg.SetActive(value: true);
		}
		else
		{
			newBestScoreImg.SetActive(value: false);
		}
		bestDistanceTxt.text = Utilities.FloatToMeters(PlayerController.Instance.GetBestDistance());
	}

	private void Update()
	{
		collectedCoinsTxt.text = "COLLECTED COINS: " + ServicesManager.Instance.CoinManager.CollectedCoins.ToString();
		totalCoinsTxt.text = ServicesManager.Instance.CoinManager.TotalCoins.ToString();
	}

	private void OnDisable()
	{
		inforViewTrans.localScale = Vector2.zero;
		topBarTrans.anchoredPosition = new Vector2(topBarTrans.anchoredPosition.x, 100f);
		bottomBarTrans.anchoredPosition = new Vector2(bottomBarTrans.anchoredPosition.x, -100f);
	}

	public void GetDoubleCoinsBtn()
	{
		ViewManager.Instance.PlayClickButtonSound();
		doubleCoinBtnView.SetActive(value: false);
//		ServicesManager.Instance.AdManager.ShowRewardedVideoAd();
	}

	public void CollectCoinsBtn()
	{
		collectCoinsBtnView.gameObject.SetActive(value: false);
		doubleCoinBtnView.gameObject.SetActive(value: false);
		ViewManager.Instance.PlayClickButtonSound();
		ServicesManager.Instance.RewardCoinManager.RewardTotalCoins(ServicesManager.Instance.CoinManager.CollectedCoins, 0.3f);
		ServicesManager.Instance.RewardCoinManager.RemoveCollectedCoins(0.3f);
	}

	public void NativeShareBtn()
	{
		ViewManager.Instance.PlayClickButtonSound();
		ServicesManager.Instance.ShareManager.NativeShare();
	}

	public void RestartBtn(float delay)
	{
		ViewManager.Instance.PlayClickButtonSound();
		SceneLoader.SetTargetScene(SceneManager.GetActiveScene().name);
		IngameManager.Instance.LoadScene("Loading", 0.2f);
	}

	public void HomeBtn()
	{
		ViewManager.Instance.PlayClickButtonSound();
		SceneLoader.SetTargetScene("Home");
		IngameManager.Instance.LoadScene("Loading", 0.2f);
	}
}
