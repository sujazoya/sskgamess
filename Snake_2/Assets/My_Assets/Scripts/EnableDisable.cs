using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisable : MonoBehaviour
{
   
    public GameObject[] _EnabeOnEnable;
    [SerializeField] float _EnabeDelayTime;
    [SerializeField] bool enableOnStart;    
    public GameObject[] _EnabeOnDisable;
    [SerializeField] float _DisableDelayTime;
    // Start is called before the first frame update

    void OnEnable()
    {       
      
        if (_EnabeOnEnable.Length > 0)
        {
            StartCoroutine(SetGameObject(_EnabeDelayTime, _EnabeOnEnable, true));
        }
       
        if (_EnabeOnDisable.Length > 0)
        {
            StartCoroutine(SetGameObject(_DisableDelayTime, _EnabeOnDisable, false));
        }

    }
    IEnumerator SetGameObject(float delayTime,GameObject[]items,bool value)
    {
        yield return new WaitForSeconds(delayTime);
        for (int i = 0; i < items.Length; i++)
        {
            items[i].SetActive(value);
        }

    }
    void OnDisable()
    {

       
        if (_EnabeOnEnable.Length > 0)
        {
            StartCoroutine(SetGameObject(_EnabeDelayTime, _EnabeOnEnable, false));
        }
       
        if (_EnabeOnDisable.Length > 0)
        {
            StartCoroutine(SetGameObject(_DisableDelayTime, _EnabeOnDisable, true));
        }

    }
    void Start()
    {
        if (enableOnStart)
        {
            for (int i = 0; i < _EnabeOnEnable.Length; i++)
            {
                _EnabeOnEnable[i].SetActive(false);
            }
            if (_EnabeOnEnable.Length > 0)
            {
                StartCoroutine(SetGameObject(_EnabeDelayTime, _EnabeOnEnable, true));
            }

            if (_EnabeOnDisable.Length > 0)
            {
                StartCoroutine(SetGameObject(_DisableDelayTime, _EnabeOnDisable, false));
            }
        }
    }

}
