using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLoader : MonoBehaviour
{
    [SerializeField] GameObject[] items;
    public static BulletLoader Instance;
    Animator animator;
    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < items.Length; i++)
        {
            items[i].SetActive(false);
        }
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }
    public void ShowLoader(float loadTime)
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].SetActive(true);
        }
        animator.enabled = true;
        StartCoroutine(CloseLoader(loadTime));
    }
     IEnumerator CloseLoader(float loadTime)
    {
        yield return new WaitForSeconds(loadTime);
        for (int i = 0; i < items.Length; i++)
        {
            items[i].SetActive(false);
        }
        animator.enabled = false;
    }
   
}
