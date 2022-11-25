using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SaveData 
{

    public static SaveData Instance;

    //public static SaveData Instance
    //{
    //    get
    //    {
    //        if (_instance == null)
    //        {
    //            _instance = new SaveData();
    //        }
    //        return _instance;
    //    }
    //}

    public CasinoPlayer playerProfile;
    
    //public List<CasinoItemData> casinoItems;
    public List<CasinoPlayer> casinoPlayer;
}
