using CBGames;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayingViewController : MonoBehaviour
{
	[SerializeField]
	private RectTransform topBarTrans;

	[SerializeField]
	private RectTransform bottomBarTrans;

	[SerializeField]
	private Text collectedCoinsTxt;

	[SerializeField]
	private GameObject unPauseBtnView;

	[SerializeField]
	private GameObject pauseBtnView;

	[SerializeField]
	private Text currentDistanceTxt;

	[SerializeField]
	private Text currentScoreTxt;

	[SerializeField]
	private GameObject unPauseCountDownView;

	[SerializeField]
	private Text unPauseCountDownTxt;

	[SerializeField]
	private Image magnetSliderImg;

	public void OnShow()
	{
		unPauseBtnView.SetActive(value: false);
		unPauseCountDownView.SetActive(value: false);
		magnetSliderImg.gameObject.SetActive(value: false);
		ViewManager.Instance.MoveRect(topBarTrans, topBarTrans.anchoredPosition, new Vector2(topBarTrans.anchoredPosition.x, 0f), 1f);
		ViewManager.Instance.MoveRect(bottomBarTrans, bottomBarTrans.anchoredPosition, new Vector2(bottomBarTrans.anchoredPosition.x, 20f), 1f);
	}

	private void OnDisable()
	{
		topBarTrans.anchoredPosition = new Vector2(topBarTrans.anchoredPosition.x, 80f);
		bottomBarTrans.anchoredPosition = new Vector2(bottomBarTrans.anchoredPosition.x, -85f);
	}

	private void Update()
	{
		currentScoreTxt.text = ServicesManager.Instance.ScoreManager.CurrentScore.ToString();
		collectedCoinsTxt.text = ServicesManager.Instance.CoinManager.CollectedCoins.ToString();
		currentDistanceTxt.text = Utilities.FloatToMeters(PlayerController.Instance.transform.position.z);
	}

	private IEnumerator CRCountDownUnPauseText()
	{
		unPauseCountDownView.SetActive(value: true);
		int t = 3;
		while (t > 0)
		{
			unPauseCountDownTxt.text = t.ToString();
			t--;
			yield return new WaitForSeconds(1f);
		}
		unPauseCountDownView.SetActive(value: false);
		IngameManager.Instance.UnPauseGame();
	}

	private IEnumerator CRCountDownMagnetMode(float time)
	{
		float t = 0f;
		while (t < time)
		{
			t += Time.deltaTime;
			float t2 = t / time;
			magnetSliderImg.fillAmount = Mathf.Lerp(1f, 0f, t2);
			yield return null;
		}
		magnetSliderImg.gameObject.SetActive(value: false);
		PlayerController.Instance.SetMagetMode(isActive: false);
	}

	public void PauseBtn()
	{
		ViewManager.Instance.PlayClickButtonSound();
		unPauseBtnView.SetActive(value: true);
		pauseBtnView.SetActive(value: false);
		IngameManager.Instance.PauseGame();
	}

	public void UnPauseBtn()
	{
		ViewManager.Instance.PlayClickButtonSound();
		unPauseBtnView.SetActive(value: false);
		pauseBtnView.SetActive(value: true);
		StartCoroutine(CRCountDownUnPauseText());
	}

	public void CountDownMagnetMode(float time)
	{
		magnetSliderImg.fillAmount = 1f;
		magnetSliderImg.gameObject.SetActive(value: true);
		StartCoroutine(CRCountDownMagnetMode(time));
	}
}
