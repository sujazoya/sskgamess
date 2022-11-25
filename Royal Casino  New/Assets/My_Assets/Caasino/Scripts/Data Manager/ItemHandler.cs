using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    //public ItemType itemType;

    public CasinoPlayer itemData;
    // Start is called before the first frame update
    public void Start()
    {
        if (string.IsNullOrEmpty(itemData.playerName))
        {
            itemData.playerName = System.DateTime.Now.ToLongDateString() + System.DateTime.Now.ToLongDateString() + Random.Range(0, int.MaxValue).ToString();
            //itemData.itemType = itemType;
            SaveData.Instance.casinoPlayer.Add(itemData);
        }

        //GameEvent.Instance.OnLoadEvent += DestroyMe;
    }

    // Update is called once per frame
    void Update()
    {
        //itemData.position = transform.position;
        //itemData.rotation = transform.rotation;
    }
    void DestroyMe()
    {
        //GameEvent.Instance.OnLoadEvent -= DestroyMe;
        Destroy(gameObject);
    }
}
