using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Sprint : MonoBehaviour
{   
    [SerializeField] GameObject[] Dancers;
    [SerializeField] int DancersCounts;
    int DancersIndex=0;
    [SerializeField] GameObject[] items;
    [SerializeField] int ItemsCounts;
    public int levelTarget;
    int itemIndex=0;
    public void CreateLevel()
    {       
        StartCoroutine (TryToCreateLevel());
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        CreateLevel();
    }

    IEnumerator TryToCreateLevel()
    {
        for (int i = 0; i < DancersCounts; i++)
        {
            Vector3 nPos=Positions.instance.NewPos();
            Vector3 currrentPos = new Vector3(nPos.x, -0.8f, nPos.z);
            Instantiate(Dancers[DancersIndex++], currrentPos, Quaternion.Euler(0,180,0));
            yield return new WaitForSeconds(0.2f);
            DancersIndex++;
            if (DancersIndex >= Dancers.Length) { DancersIndex = 0; }
        }
        for (int i = 0; i < ItemsCounts; i++)
        {
            Vector3 nPos = Positions.instance.NewPos();
            Vector3 currrentPos = new Vector3(nPos.x, 0.2f, nPos.z);
            Instantiate(items[itemIndex], currrentPos, Quaternion.Euler(-90, -90, 0));
            yield return new WaitForSeconds(0.2f);
            itemIndex++;
            if (itemIndex >= items.Length) { itemIndex = 0; }
        }
       
    }
}
