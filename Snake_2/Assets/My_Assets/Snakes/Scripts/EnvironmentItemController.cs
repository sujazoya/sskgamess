using CBGames;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnvironmentItemController : MonoBehaviour
{
	[Header("Config")]
	[SerializeField]
	private string environmentName = string.Empty;

	[SerializeField]
	private int unlockPrice = 500;

	[SerializeField]
	private int environmentIndex;

	[Header("References")]
	[SerializeField]
	private GameObject lockPanel;

	[SerializeField]
	private Button unlockBtn;

	[SerializeField]
	private Text unlockPriceTxt;

	[SerializeField]
	private Text environmentNameTxt;

	[SerializeField]
	private Sprite background;

	private void Start()
	{
		environmentNameTxt.text = environmentName.ToUpper();
		if (unlockPrice == 0 || PlayerPrefs.GetInt(environmentName, 0) == 1)
		{
			lockPanel.SetActive(value: false);
			return;
		}
		lockPanel.SetActive(value: true);
		StartCoroutine(CRCheckAndEnableUnLockBtn());
	}

	private IEnumerator CRCheckAndEnableUnLockBtn()
	{
		while (base.gameObject.activeInHierarchy && PlayerPrefs.GetInt(environmentName, 0) != 1)
		{
			environmentNameTxt.gameObject.SetActive(value: false);
			if (ServicesManager.Instance.CoinManager.TotalCoins >= unlockPrice)
			{
				unlockBtn.interactable = true;
				unlockPriceTxt.gameObject.SetActive(value: true);
				unlockPriceTxt.text = unlockPrice.ToString();
			}
			else
			{
				unlockBtn.interactable = false;
				unlockPriceTxt.gameObject.SetActive(value: false);
			}
			yield return null;
		}
	}

	public void UnlockBtn()
	{
		ServicesManager.Instance.CoinManager.RemoveTotalCoins(unlockPrice);
		ServicesManager.Instance.SoundManager.PlayOneSound(ServicesManager.Instance.SoundManager.unlock);
		PlayerPrefs.SetInt(environmentName, 1);
		lockPanel.SetActive(value: false);
		environmentNameTxt.gameObject.SetActive(value: true);
	}

	public void SelectBtn()
	{
		IngameManager.SetEnvironmentIndex(environmentIndex);
		SceneLoader.SetTargetScene("Ingame", background);
		ServicesManager.Instance.SoundManager.PlayOneSound(ServicesManager.Instance.SoundManager.button);
		SceneManager.LoadScene("Loading");
	}
}
