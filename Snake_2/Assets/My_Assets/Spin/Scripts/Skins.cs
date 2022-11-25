using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Skins", menuName = "My Skin/New Skin", order = 1)]
public class Skins : ScriptableObject
{
    public string _name;
    public Sprite background;
}
