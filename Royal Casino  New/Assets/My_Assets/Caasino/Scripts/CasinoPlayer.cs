using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CasinoPlayer
{
    public string playerName;
    public int balance;
    public int diemond;
    public int coin;
    public int avatarIndex;

    public CasinoPlayer(string id, int balances, int dieonds, int coins,int avatarInd)
    {
        // Set all the class variables
        this.playerName = id;

        balance = balances;
        diemond = dieonds;
        coin = coins;
        avatarIndex = avatarInd;
    }
}
