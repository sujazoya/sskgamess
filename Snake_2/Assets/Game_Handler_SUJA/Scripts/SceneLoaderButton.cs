using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SceneLoaderButton : MonoBehaviour
{
    [SerializeField] string sceneName;
    private Level_Loader _Loader;
    Button button;
    // Start is called before the first frame update
    void Start()
    {
        _Loader = FindObjectOfType<Level_Loader>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

   void OnClick()
    {
        Level_Loader.instance.LoadLevel(sceneName);
    }
}
