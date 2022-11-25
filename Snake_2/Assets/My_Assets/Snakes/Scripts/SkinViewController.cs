using CBGames;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkinViewController : MonoBehaviour
{
	[Header("References UI")]
	[SerializeField]
	private RectTransform topBarTrans;

	[SerializeField]
	private RectTransform bottomBarTrans;

	[SerializeField]
	private RectTransform scrollLeftBtnTrans;

	[SerializeField]
	private RectTransform scrollRightBtnTrans;

	[SerializeField]
	private Text totalCoinsTxt;

	[SerializeField]
	private Text skinPriceTxt;

	[SerializeField]
	private GameObject selectBtnView;

	[SerializeField]
	private GameObject unlockBtnView;

	[SerializeField]
	private Button unlockBtn;

	private SkinController skinController;

	private SkinInfor currentSkinInfor;

	public void OnShow()
	{
		ViewManager.Instance.MoveRect(topBarTrans, topBarTrans.anchoredPosition, new Vector2(topBarTrans.anchoredPosition.x, 0f), 1f);
		ViewManager.Instance.MoveRect(bottomBarTrans, bottomBarTrans.anchoredPosition, new Vector2(bottomBarTrans.anchoredPosition.x, 0f), 1f);
		ViewManager.Instance.MoveRect(scrollLeftBtnTrans, scrollLeftBtnTrans.anchoredPosition, new Vector2(0f, scrollLeftBtnTrans.anchoredPosition.y), 1f);
		ViewManager.Instance.MoveRect(scrollRightBtnTrans, scrollRightBtnTrans.anchoredPosition, new Vector2(0f, scrollRightBtnTrans.anchoredPosition.y), 1f);
		if (skinController == null)
		{
			skinController = UnityEngine.Object.FindObjectOfType<SkinController>();
		}
	}

	private void OnDisable()
	{
		topBarTrans.anchoredPosition = new Vector2(topBarTrans.anchoredPosition.x, 100f);
		bottomBarTrans.anchoredPosition = new Vector2(topBarTrans.anchoredPosition.x, -100f);
		scrollLeftBtnTrans.anchoredPosition = new Vector2(-100f, scrollLeftBtnTrans.anchoredPosition.y);
		scrollRightBtnTrans.anchoredPosition = new Vector2(100f, scrollRightBtnTrans.anchoredPosition.y);
	}

	private void Update()
	{
		totalCoinsTxt.text = ServicesManager.Instance.CoinManager.TotalCoins.ToString();
	}

	public void UpdateUI(SkinInfor skinInfor)
	{
		currentSkinInfor = skinInfor;
		if (!skinInfor.IsUnlocked)
		{
			selectBtnView.SetActive(value: false);
			unlockBtnView.SetActive(value: true);
			skinPriceTxt.text = skinInfor.SkinPrice.ToString();
			if (ServicesManager.Instance.CoinManager.TotalCoins >= skinInfor.SkinPrice)
			{
				unlockBtn.interactable = true;
			}
			else
			{
				unlockBtn.interactable = false;
			}
		}
		else
		{
			unlockBtnView.SetActive(value: false);
			selectBtnView.SetActive(value: true);
		}
	}

	public void UnlockBtn()
	{
		currentSkinInfor.Unlock();
		UpdateUI(currentSkinInfor);
	}

	public void SelectBtn()
	{
		ViewManager.Instance.PlayClickButtonSound();
		ServicesManager.Instance.SkinContainer.SetSelectedSkinIndex(currentSkinInfor.SequenceNumber);
		BackBtn();
	}

	public void BackBtn()
	{
		ViewManager.Instance.PlayClickButtonSound();
		SceneLoader.SetTargetScene("Home");
		SceneManager.LoadScene("Loading");
	}

	public void ScrollForwardBtn()
	{
		ViewManager.Instance.PlayClickButtonSound();
		skinController.MoveAllSkins(Vector3.forward);
	}

	public void ScrollBackBtn()
	{
		ViewManager.Instance.PlayClickButtonSound();
		skinController.MoveAllSkins(Vector3.back);
	}
}
