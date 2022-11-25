using CBGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkinController : MonoBehaviour
{
	[Header("Skin Scroll Config")]
	[SerializeField]
	private float skinSpace = 10f;

	[SerializeField]
	private float maxSkinScale = 2f;

	[SerializeField]
	private float movingTime = 1f;

	[SerializeField]
	private float rotatingSpeed = 50f;

	[Header("References Objects")]
	[SerializeField]
	private GameObject environmentObject;

	[SerializeField]
	private EnvironmentPack[] environmentPacks;

	private List<SkinInfor> listSkinInfor = new List<SkinInfor>();

	private int currentSkinIndex;

	private bool isMoving = true;

	private void Start()
	{
		ViewManager.Instance.OnLoadingSceneDone(SceneManager.GetActiveScene().name);
		for (int i = 0; i < environmentObject.transform.childCount; i++)
		{
			Vector3 position = environmentObject.transform.GetChild(i).transform.position;
			MeshRenderer component = UnityEngine.Object.Instantiate(environmentPacks[IngameManager.EnvironmentIndex].GroundPrefab, position, Quaternion.identity).GetComponent<MeshRenderer>();
			Vector3 position2 = component.transform.position + Vector3.left * (component.bounds.size.x / 2f);
			Object.Instantiate(environmentPacks[IngameManager.EnvironmentIndex].ListLeftWallPrefab[Random.Range(0, environmentPacks[IngameManager.EnvironmentIndex].ListLeftWallPrefab.Count)], position2, Quaternion.identity);
			Vector3 position3 = component.transform.position + Vector3.right * (component.bounds.size.x / 2f);
			Object.Instantiate(environmentPacks[IngameManager.EnvironmentIndex].ListRightWallPrefab[Random.Range(0, environmentPacks[IngameManager.EnvironmentIndex].ListRightWallPrefab.Count)], position3, Quaternion.identity);
		}
		currentSkinIndex = Mathf.Clamp(ServicesManager.Instance.SkinContainer.SelectedSkinIndex, 0, ServicesManager.Instance.SkinContainer.SkinInfors.Length - 1);
		for (int j = 0; j < ServicesManager.Instance.SkinContainer.SkinInfors.Length; j++)
		{
			int num = j - currentSkinIndex;
			GameObject gameObject = UnityEngine.Object.Instantiate(ServicesManager.Instance.SkinContainer.SkinInfors[j].gameObject, Vector3.zero, Quaternion.identity);
			SkinInfor component2 = gameObject.GetComponent<SkinInfor>();
			component2.SetSequenceNumber(j);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.position = new Vector3(0f, 0f, (float)num * skinSpace);
			listSkinInfor.Add(component2);
		}
		listSkinInfor[currentSkinIndex].ScaleThisObject(maxSkinScale * Vector3.one, movingTime);
		StartCoroutine(CRWaitAndEnableMove());
	}

	public void MoveAllSkins(Vector3 dir)
	{
		if (!isMoving && (!dir.Equals(Vector3.back) || currentSkinIndex != listSkinInfor.Count - 1) && (!dir.Equals(Vector3.forward) || currentSkinIndex != 0))
		{
			isMoving = true;
			foreach (SkinInfor item in listSkinInfor)
			{
				item.MoveThisObject(dir, skinSpace, movingTime);
			}
			listSkinInfor[currentSkinIndex].StopRotateAndResetObject(movingTime);
			currentSkinIndex = (dir.Equals(Vector3.forward) ? (currentSkinIndex - 1) : (currentSkinIndex + 1));
			listSkinInfor[currentSkinIndex].ScaleThisObject(maxSkinScale * Vector3.one, movingTime);
			StartCoroutine(CRWaitAndEnableMove());
		}
	}

	private IEnumerator CRWaitAndEnableMove()
	{
		yield return new WaitForSeconds(movingTime + 0.1f);
		isMoving = false;
		listSkinInfor[currentSkinIndex].RotateThisObject(rotatingSpeed);
		ViewManager.Instance.SkinViewController.UpdateUI(listSkinInfor[currentSkinIndex]);
	}
}
