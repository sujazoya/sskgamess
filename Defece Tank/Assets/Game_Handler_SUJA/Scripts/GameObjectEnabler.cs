using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectEnabler : MonoBehaviour
{
    [Header("If Not Self")]
    [SerializeField] GameObject[] items;

    public enum EnableMode
    {
        OnAwake,OnStart,OnEnable,OnDisable
    }
    public EnableMode enableMode;
    public Do @do;
    public enum Do
    {
        Enable,Disable
    }
    [SerializeField] float afterTime;
    private void Awake()
    {
       
        if (enableMode == EnableMode.OnAwake)
        {
            DoActive();
        }
        if (afterTime > 0)
        {
            Invoke("DoActive", afterTime);
        }
    }    
    void DoActive()
    {
        if (@do == Do.Enable)
        {
            if (items.Length > 0)
            {
                SetActive(true);
            }
            else
            {
                transform.gameObject.SetActive(true);
            }
        }
        else
                if (@do == Do.Disable)
        {
            if (items.Length > 0)
            {
                SetActive(false);
            }
            else
            {
                transform.gameObject.SetActive(false);
            }
        }
    }
    void SetActive(bool value)
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].SetActive(value);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (enableMode == EnableMode.OnStart)
        {
            DoActive();
        }
    }
   void OnEnable()
    {
        if (enableMode == EnableMode.OnEnable)
        {
            DoActive();
        }
    }
    void OnDisable()
    {
        if (enableMode == EnableMode.OnDisable)
        {
            DoActive();
        }
    }

}
