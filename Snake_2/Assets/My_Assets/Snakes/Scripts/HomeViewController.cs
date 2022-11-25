using CBGames;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeViewController : MonoBehaviour
{
	[SerializeField]
	private RectTransform topBarTrans;

	[SerializeField]
	private RectTransform bottomBarTrans;

	[SerializeField]
	private RectTransform settingBtnsViewTrans;

	[SerializeField]
	private RectTransform environmentViewTrans;

	[SerializeField]
	private RectTransform gameNameTrans;

	[SerializeField]
	private GameObject soundOnBtnView;

	[SerializeField]
	private GameObject soundOffBtnView;

	[SerializeField]
	private GameObject musicOnBtnView;

	[SerializeField]
	private GameObject musicOffBtnView;

	[SerializeField]
	private GameObject warning;

	[SerializeField]
	private Text bestScoreTxt;

	[SerializeField]
	private Text totalCoinsTxt;

	[SerializeField]
	private DailyRewardViewController dailyRewardViewControl;

	private bool isLocked;

	public void OnShow()
	{
		ViewManager.Instance.MoveRect(topBarTrans, topBarTrans.anchoredPosition, new Vector2(topBarTrans.anchoredPosition.x, 0f), 0.5f);
		ViewManager.Instance.MoveRect(bottomBarTrans, bottomBarTrans.anchoredPosition, new Vector2(bottomBarTrans.anchoredPosition.x, 0f), 0.5f);
		bestScoreTxt.text = ServicesManager.Instance.ScoreManager.BestScore.ToString();
		isLocked = false;
		if (ServicesManager.Instance.SoundManager.IsSoundOff())
		{
			soundOnBtnView.gameObject.SetActive(value: false);
			soundOffBtnView.gameObject.SetActive(value: true);
		}
		else
		{
			soundOnBtnView.gameObject.SetActive(value: true);
			soundOffBtnView.gameObject.SetActive(value: false);
		}
		if (ServicesManager.Instance.SoundManager.IsMusicOff())
		{
			musicOffBtnView.gameObject.SetActive(value: true);
			musicOnBtnView.gameObject.SetActive(value: false);
		}
		else
		{
			musicOffBtnView.gameObject.SetActive(value: false);
			musicOnBtnView.gameObject.SetActive(value: true);
		}
		Invoke("CheckAndEnableWarning", 0.5f);
	}

	private void OnDisable()
	{
		topBarTrans.anchoredPosition = new Vector2(topBarTrans.anchoredPosition.x, 150f);
		bottomBarTrans.anchoredPosition = new Vector2(topBarTrans.anchoredPosition.x, -150f);
		environmentViewTrans.anchoredPosition = new Vector2(environmentViewTrans.anchoredPosition.x, 600f);
		gameNameTrans.anchoredPosition = new Vector2(gameNameTrans.anchoredPosition.x, 0f);
	}

	private void Update()
	{
		totalCoinsTxt.text = ServicesManager.Instance.CoinManager.TotalCoins.ToString();
		if (Input.GetMouseButtonDown(0) && !isLocked && EventSystem.current.currentSelectedGameObject == null)
		{
			isLocked = true;
			ViewManager.Instance.PlayClickButtonSound();
			ViewManager.Instance.MoveRect(environmentViewTrans, environmentViewTrans.anchoredPosition, new Vector2(environmentViewTrans.anchoredPosition.x, -40f), 0.75f);
			ViewManager.Instance.MoveRect(bottomBarTrans, bottomBarTrans.anchoredPosition, new Vector2(bottomBarTrans.anchoredPosition.x, -150f), 0.5f);
		}
	}

	public void OnDailyRewardViewClose()
		{
			isLocked = false;
			ViewManager.Instance.MoveRect(gameNameTrans, gameNameTrans.anchoredPosition, new Vector2(gameNameTrans.anchoredPosition.x, 0f), 0.5f);
			ViewManager.Instance.MoveRect(bottomBarTrans, bottomBarTrans.anchoredPosition, new Vector2(bottomBarTrans.anchoredPosition.x, 0f), 0.5f);
			CheckAndEnableWarning();
		}

	private void CheckAndEnableWarning()
	{
		/*			if (ServicesManager.Instance.AdManager.IsRewardedVideoAdReady())
							{
								warning.SetActive(value: true);
							}
							else if (dailyRewardViewControl.gameObject.activeInHierarchy)
							{
								if (dailyRewardViewControl.IsRewardAvailable)
								{
									warning.SetActive(value: true);
								}
								else
								{
									warning.SetActive(value: false);
								}
							}
							else
							{
								warning.SetActive(value: false);
							}
						}
		*/
	}
	public void NativeShareBtn()
	{
		ViewManager.Instance.PlayClickButtonSound();
		ServicesManager.Instance.ShareManager.NativeShare();
	}

	public void CharacterBtn()
	{
		ViewManager.Instance.PlayClickButtonSound();
		SceneLoader.SetTargetScene("Skin");
		SceneManager.LoadScene("Loading");
	}

	public void RewardBtn()
	{
		isLocked = true;
		ViewManager.Instance.PlayClickButtonSound();
		ViewManager.Instance.MoveRect(bottomBarTrans, bottomBarTrans.anchoredPosition, new Vector2(bottomBarTrans.anchoredPosition.x, -150f), 0.5f);
		ViewManager.Instance.MoveRect(gameNameTrans, gameNameTrans.anchoredPosition, new Vector2(gameNameTrans.anchoredPosition.x, 100f), 0.5f);
		dailyRewardViewControl.gameObject.SetActive(value: true);
		dailyRewardViewControl.OnShow();
		warning.SetActive(value: false);
	}

	public void SettingBtn()
	{
		isLocked = true;
		ViewManager.Instance.PlayClickButtonSound();
		ViewManager.Instance.MoveRect(settingBtnsViewTrans, settingBtnsViewTrans.anchoredPosition, new Vector2(settingBtnsViewTrans.anchoredPosition.x, 10f), 0.5f);
	}

	public void ToggleSound()
	{
		ViewManager.Instance.PlayClickButtonSound();
		ServicesManager.Instance.SoundManager.ToggleSound();
		if (ServicesManager.Instance.SoundManager.IsSoundOff())
		{
			soundOnBtnView.gameObject.SetActive(value: false);
			soundOffBtnView.gameObject.SetActive(value: true);
		}
		else
		{
			soundOnBtnView.gameObject.SetActive(value: true);
			soundOffBtnView.gameObject.SetActive(value: false);
		}
	}

	public void ToggleMusic()
	{
		ViewManager.Instance.PlayClickButtonSound();
		ServicesManager.Instance.SoundManager.ToggleMusic();
		if (ServicesManager.Instance.SoundManager.IsMusicOff())
		{
			musicOffBtnView.gameObject.SetActive(value: true);
			musicOnBtnView.gameObject.SetActive(value: false);
		}
		else
		{
			musicOffBtnView.gameObject.SetActive(value: false);
			musicOnBtnView.gameObject.SetActive(value: true);
		}
	}

	public void FacebookShareBtn()
	{
		ViewManager.Instance.PlayClickButtonSound();
		ServicesManager.Instance.ShareManager.FacebookShare();
	}

	public void TwitterShareBtn()
	{
		ViewManager.Instance.PlayClickButtonSound();
		ServicesManager.Instance.ShareManager.TwitterShare();
	}

	public void RateAppBtn()
	{
		ViewManager.Instance.PlayClickButtonSound();
		Application.OpenURL(ServicesManager.Instance.ShareManager.AppUrl);
	}

	public void CloseSettingViewBtn()
	{
		isLocked = false;
		ViewManager.Instance.PlayClickButtonSound();
		ViewManager.Instance.MoveRect(settingBtnsViewTrans, settingBtnsViewTrans.anchoredPosition, new Vector2(settingBtnsViewTrans.anchoredPosition.x, -350f), 0.5f);
	}
}
