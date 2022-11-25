using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingAdCaller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CallInterstitial());
    }

   IEnumerator CallInterstitial()
    {
        yield return new WaitForSeconds(2);
        AdmobAdmanager.Instance.ShowInterstitial();
    }
}
