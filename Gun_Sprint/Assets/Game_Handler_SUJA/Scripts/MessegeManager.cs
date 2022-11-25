using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessegeManager : MonoBehaviour
{
    public GameObject massegePnel;
    public Text header;
    public Text massege;
    public Button closeButton;
    public GameObject massegeEffect;
    public GameObject popupCanvas;
    public Text popupText;    
    public static MessegeManager insance;

    private void Awake()
    {
        if (insance == null) { insance = this; }
    }
    // Start is called before the first frame update
    void Start()
    {
        closeButton.onClick.AddListener(CloseMassegeCanvas);
        CloseMassegeCanvas();
        popupCanvas.SetActive(false);
    }
    public void ShowPopup(string massege)
    {
        StartCoroutine(ShowPopup_(massege));
    }
    IEnumerator ShowPopup_(string massege)
    {
       popupText.text = string.Empty;
       popupCanvas.SetActive(true);
       popupText.text = massege;
        yield return new WaitForSeconds(2.5f);
        popupText.text = string.Empty;
        popupCanvas.SetActive(false);
    }
    void CloseMassegeCanvas()
    {
        massegePnel.SetActive(false);
    }
    public void ShowMassege(string headers, string masseges, bool value)
    {
       massegePnel.SetActive(true);
       header.text = headers;
       massege.text = masseges;
        if (value == false) {if(massegeEffect) massegeEffect.SetActive(false); } 
        else { if (massegeEffect) massegeEffect.SetActive(true); }
    }
}
