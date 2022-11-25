using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCI_fi_Trans : MonoBehaviour
{   
    float transitionTime = 0.6f;  
    float cphase = 0f;
    Renderer r;  
   
    private void OnEnable()
    {
        r = GetComponent<Renderer>();
        StartCoroutine(PlayTrans());
    }
    IEnumerator PlayTrans()
    {
        while (cphase < 1f)
        {
            cphase += Time.deltaTime / transitionTime;
            r.sharedMaterial.SetFloat("_Phase", cphase);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.8f);
        while (cphase > 0f)
        {
            cphase -= Time.deltaTime / transitionTime;
            r.sharedMaterial.SetFloat("_Phase", cphase);
            yield return new WaitForEndOfFrame();
        }
       
    }
}
