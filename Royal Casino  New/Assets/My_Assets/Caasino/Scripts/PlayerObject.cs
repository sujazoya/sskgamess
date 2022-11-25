using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Player",menuName ="Player")]
public class PlayerObject : ScriptableObject
{
    public new string   name;
    public Sprite       avatar;
    public int          number;
    public int          ammount;
}
