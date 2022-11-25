using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Level_Loader : MonoBehaviour
{
    public GameObject loadingScreen;
    public Image loadingSlider;
    public Text percetangeText;
    [SerializeField] bool loadingScene;
    [Header("If Loading Scene Assing Menu Scene Name")]
    [SerializeField] string sceneNameToLoad;

    public static Level_Loader instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        loadingScreen.SetActive(false);
        if (loadingScene)
        {
            LoadLevel(sceneNameToLoad);
        }
    }
    public void LoadLevel(string scene)
    {
        StartCoroutine(LoadAsyncroneusly(scene));
    }

    IEnumerator LoadAsyncroneusly(string scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        loadingScreen.SetActive(true);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingSlider.fillAmount = progress;
            //percetangeText.text = progress * 100f + "%";
            yield return null;
        }
       AdmobAdmanager.Instance.ShowInterstitial();
       FacebookAdManager.Instance.ShowInterstitial();
    }
}
