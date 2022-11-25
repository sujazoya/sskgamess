using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]

public enum ItemType
{
    Diemonds,
    Coins
}

[System.Serializable]
public class CasinoItemData
{
    public string id;
    public ItemType itemType;

    //public Vector3 position;
    //public Quaternion rotation;
}
