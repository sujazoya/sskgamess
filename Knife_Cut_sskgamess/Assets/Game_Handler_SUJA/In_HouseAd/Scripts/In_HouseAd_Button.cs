using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Button))]
public class In_HouseAd_Button : MonoBehaviour
{
    public UnityEvent OnAdComplete;
    public UnityEvent OnAdClosed_;
    private InHouse_Ad_Handler inHouse_Ad;
    Button myButton;
    // Start is called before the first frame update
    void Start()
    {
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(ShowInHouseAd);
        inHouse_Ad = FindObjectOfType<InHouse_Ad_Handler>();
        InHouseAdManager.onAdCompleted += OnAdCompleted;
        InHouseAdManager.onAdClosed += OnAdClosed;
    }
    void ShowInHouseAd()
    {
        inHouse_Ad.ShowInHouseAd();
    }
    void OnAdCompleted()
    {
        OnAdComplete.Invoke();
    }
    void OnAdClosed()
    {
        OnAdClosed_.Invoke();
    }

}
