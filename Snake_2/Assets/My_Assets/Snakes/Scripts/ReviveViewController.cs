using CBGames;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ReviveViewController : MonoBehaviour
{
	[SerializeField]
	private RectTransform filterImgTrans;

	[SerializeField]
	private RectTransform reviveBtnTrans;

	[SerializeField]
	private RectTransform closeReviveViewTrans;

	[SerializeField]
	private Image filterImg;

	public void OnShow()
	{
		ViewManager.Instance.MoveRect(closeReviveViewTrans, closeReviveViewTrans.anchoredPosition, new Vector2(closeReviveViewTrans.anchoredPosition.x, 50f), 1f);
		ViewManager.Instance.ScaleRect(filterImgTrans, Vector2.zero, Vector2.one, 0.5f);
		StartCoroutine(CRReviveCountDown(0.6f));
		StartCoroutine(CRScaleReviveButton(0.6f));
	}

	private void OnDisable()
	{
		closeReviveViewTrans.anchoredPosition = new Vector2(closeReviveViewTrans.anchoredPosition.x, -100f);
		filterImgTrans.localScale = Vector2.zero;
	}

	private IEnumerator CRReviveCountDown(float delay)
	{
		float waitingTime = IngameManager.Instance.ReviveWaitTime;
		filterImg.fillAmount = 1f;
		yield return new WaitForSeconds(delay);
		float t = 0f;
		while (t < waitingTime)
		{
			t += Time.deltaTime;
			float t2 = t / waitingTime;
			filterImg.fillAmount = Mathf.Lerp(1f, 0f, t2);
			yield return null;
		}
		IngameManager.Instance.GameOver();
	}

	private IEnumerator CRScaleReviveButton(float delay)
	{
		yield return new WaitForSeconds(delay);
		float time = 0.3f;
		Vector2 startScale = Vector2.one;
		Vector2 endScale = Vector2.one * 1.15f;
		reviveBtnTrans.localScale = startScale;
		while (base.gameObject.activeInHierarchy)
		{
			float t2 = 0f;
			while (t2 < time)
			{
				t2 += Time.deltaTime;
				float t3 = t2 / time;
				reviveBtnTrans.localScale = Vector2.Lerp(startScale, endScale, t3);
				yield return null;
			}
			t2 = 0f;
			while (t2 < time)
			{
				t2 += Time.deltaTime;
				float t4 = t2 / time;
				reviveBtnTrans.localScale = Vector2.Lerp(endScale, startScale, t4);
				yield return null;
			}
		}
	}

	public void ReviveBtn()
	{
		ViewManager.Instance.PlayClickButtonSound();
//		ServicesManager.Instance.AdManager.ShowRewardedVideoAd();
	}

	public void CloseReviveViewBtn()
	{
		ViewManager.Instance.PlayClickButtonSound();
		IngameManager.Instance.GameOver();
	}
}
