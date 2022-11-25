using CBGames;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ViewManager : MonoBehaviour
{
	[SerializeField]
	private HomeViewController homeViewControl;

	[SerializeField]
	private LoadingViewController loadingViewControl;

	[SerializeField]
	private IngameViewController ingameViewControl;

	[SerializeField]
	private SkinViewController skinViewControl;

	public static ViewManager Instance
	{
		get;
		private set;
	}

	public HomeViewController HomeViewController => homeViewControl;

	public LoadingViewController LoadingViewController => loadingViewControl;

	public IngameViewController IngameViewController => ingameViewControl;

	public SkinViewController SkinViewController => skinViewControl;

	private void Awake()
	{		
		if ((bool)Instance)
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
			return;
		}
		Instance = this;
		Object.DontDestroyOnLoad(base.gameObject);		
	}	
	
	private IEnumerator CRMovingRect(RectTransform rect, Vector2 startPos, Vector2 endPos, float movingTime)
	{
		if (!new Vector2(Mathf.RoundToInt(rect.anchoredPosition.x), Mathf.RoundToInt(rect.anchoredPosition.y)).Equals(endPos))
		{
			rect.anchoredPosition = startPos;
			float t = 0f;
			while (t < movingTime)
			{
				t += Time.deltaTime;
				float t2 = EasyType.MatchedLerpType(LerpType.EaseInOutQuart, t / movingTime);
				rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t2);
				yield return null;
			}
		}
	}

	private IEnumerator CRScalingRect(RectTransform rect, Vector2 startScale, Vector2 endScale, float scalingTime)
	{
		rect.localScale = startScale;
		float t = 0f;
		while (t < scalingTime)
		{
			t += Time.deltaTime;
			float t2 = EasyType.MatchedLerpType(LerpType.EaseInOutQuart, t / scalingTime);
			rect.localScale = Vector2.Lerp(startScale, endScale, t2);
			yield return null;
		}
	}

	public void PlayClickButtonSound()
	{
		ServicesManager.Instance.SoundManager.PlayOneSound(ServicesManager.Instance.SoundManager.button);
	}

	public void MoveRect(RectTransform rect, Vector2 startPos, Vector2 endPos, float movingTime)
	{
		StartCoroutine(CRMovingRect(rect, startPos, endPos, movingTime));
	}

	public void ScaleRect(RectTransform rect, Vector2 startScale, Vector2 endScale, float scalingTime)
	{
		StartCoroutine(CRScalingRect(rect, startScale, endScale, scalingTime));
	}

	public void OnLoadingSceneDone(string sceneName)
	{
		if (sceneName.Equals("Home"))
		{
			homeViewControl.gameObject.SetActive(value: true);
			homeViewControl.OnShow();
			loadingViewControl.gameObject.SetActive(value: false);
			ingameViewControl.gameObject.SetActive(value: false);
			skinViewControl.gameObject.SetActive(value: false);
		}
		else if (sceneName.Equals("Ingame"))
		{
			ingameViewControl.gameObject.SetActive(value: true);
			ingameViewControl.OnShow();
			loadingViewControl.gameObject.SetActive(value: false);
			homeViewControl.gameObject.SetActive(value: false);
			skinViewControl.gameObject.SetActive(value: false);
		}
		else if (sceneName.Equals("Skin"))
		{
			skinViewControl.gameObject.SetActive(value: true);
			skinViewControl.OnShow();
			loadingViewControl.gameObject.SetActive(value: false);
			homeViewControl.gameObject.SetActive(value: false);
			ingameViewControl.gameObject.SetActive(value: false);
		}
		else if (sceneName.Equals("Loading"))
		{
			loadingViewControl.gameObject.SetActive(value: true);
			loadingViewControl.OnShow();
			ingameViewControl.gameObject.SetActive(value: false);
			homeViewControl.gameObject.SetActive(value: false);
			skinViewControl.gameObject.SetActive(value: false);
		}
		if (sceneName.Equals("GameMenu"))
		{
			loadingViewControl.gameObject.SetActive(value: false);
			loadingViewControl.OnShow();
			ingameViewControl.gameObject.SetActive(value: false);
			homeViewControl.gameObject.SetActive(value: false);
			skinViewControl.gameObject.SetActive(value: false);
		}
	}

}
