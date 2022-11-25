using UnityEngine;
using UnityEngine.UI;

public class LoadingViewController : MonoBehaviour
{
	[SerializeField]
	private Text loadingTxt;

	[SerializeField]
	private Image loadingImg;

	public void OnShow()
	{
	}

	public void SetLoadingText(string text)
	{
		loadingTxt.text = text;
	}

	public void SetLoadingSprite(Sprite sp)
	{
		loadingImg.sprite = sp;
	}
}
