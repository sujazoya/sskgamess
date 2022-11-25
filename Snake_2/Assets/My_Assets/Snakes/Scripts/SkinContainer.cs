using UnityEngine;

public class SkinContainer : MonoBehaviour
{
	private static readonly string SELECTED_SKIN_PPK = "CBGAMES_CURRENT_SKIN";

	[SerializeField]
	private SkinInfor[] skinInfors;

	public int SelectedSkinIndex => PlayerPrefs.GetInt(SELECTED_SKIN_PPK, 0);

	public SkinInfor[] SkinInfors => skinInfors;

	public void SetSelectedSkinIndex(int index)
	{
		PlayerPrefs.SetInt(SELECTED_SKIN_PPK, index);
		PlayerPrefs.Save();
	}
}
