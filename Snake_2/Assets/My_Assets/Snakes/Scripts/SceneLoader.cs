using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	private static string targetScene = string.Empty;

	private void Start()
	{
		StartCoroutine(LoadingScene());
		ViewManager.Instance.OnLoadingSceneDone(SceneManager.GetActiveScene().name);
	}

	private IEnumerator LoadingScene()
	{
		int temp = 0;
		AsyncOperation asyn = SceneManager.LoadSceneAsync(targetScene);
		while (!asyn.isDone)
		{
			temp++;
			switch (temp)
			{
			case 1:
				ViewManager.Instance.LoadingViewController.SetLoadingText("LOADING.");
				break;
			case 2:
				ViewManager.Instance.LoadingViewController.SetLoadingText("LOADING..");
				break;
			default:
				ViewManager.Instance.LoadingViewController.SetLoadingText("LOADING...");
				temp = 0;
				break;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

	public static void SetTargetScene(string sceneName, Sprite sp = null)
	{
		targetScene = sceneName;
		if (sp != null)
		{
			ViewManager.Instance.LoadingViewController.SetLoadingSprite(sp);
		}
	}
}
