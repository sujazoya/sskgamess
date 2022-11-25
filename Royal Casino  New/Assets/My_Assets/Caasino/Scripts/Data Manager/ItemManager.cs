using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSave()
    {
        SerializationManger.Save("itemsave", SaveData.Instance);
    }
    public void OnLoade()
    {
        SaveData.Instance = (SaveData)SerializationManger.Load(Application.persistentDataPath + "/saves/Save.save");
        for (int i = 0; i < SaveData.Instance.casinoPlayer.Count; i++)
        {
            //CasinoItemData myItem = SaveData.Instance.casinoItems[i];
            //GameObject obj;//=Instantiate(items([int)currentItem.itemType]);
            //ItemHandler itemHandler = obj.GetComponent<ItemHandler>();
            //itemHandler.itemData = currentItem;
            //itemHandler.transform.position = currentItem.position;
            //itemHandler.transform.rotation = currentItem.rotation;
        }
    }
}
