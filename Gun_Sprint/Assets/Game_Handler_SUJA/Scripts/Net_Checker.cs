using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Net_Checker : MonoBehaviour
{
    [SerializeField] GameObject warningPanel;
    public Button closeButton;
    // Start is called before the first frame update

    private void Awake()
    {
        Net_Checker[] objs = FindObjectsOfType<Net_Checker>();
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        warningPanel.SetActive(false);
    }
    void Start()
    {
        StartCoroutine(WarnConnection(0));
        if (closeButton)
        {
            closeButton.onClick.AddListener(Close);
        }
    }
    public  IEnumerator  WarnConnection(float wait)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //Debug.Log("Error. Check internet connection!");
            warningPanel.SetActive(true);
        }
        yield return new WaitForSeconds(wait);
        StartCoroutine(WarnConnection(120));
    }
    void Close()
    {
        warningPanel.SetActive(false);
    }
    
}
