using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
	[SerializeField]
	private Transform[] blockPoints;

	[SerializeField]
	private Transform[] itemPoints;

	[SerializeField]
	private Transform[] obstaclePoints;

	private MeshRenderer meshRender;

	public Vector3 GetLeftPos()
	{
		if (meshRender == null)
		{
			meshRender = GetComponent<MeshRenderer>();
		}
		return base.transform.position + Vector3.left * (meshRender.bounds.size.x / 2f);
	}

	public Vector3 GetRightPos()
	{
		if (meshRender == null)
		{
			meshRender = GetComponent<MeshRenderer>();
		}
		return base.transform.position + Vector3.right * (meshRender.bounds.size.x / 2f);
	}

	public void CreateObjects(int bodyPartAdderNumber, int blockNumber, int obstacleNumber, int coinNumber, bool isCreateShield, bool isCreateMagnet)
	{
		StartCoroutine(CRCreateObjects(bodyPartAdderNumber, blockNumber, obstacleNumber, coinNumber, isCreateShield, isCreateMagnet));
	}

	private IEnumerator CRCreateObjects(int bodyPartAdderNumber, int blockNumber, int obstacleNumber, int coinNumber, bool isCreateShield, bool isCreateMagnet)
	{
		List<Vector3> listItemPos = new List<Vector3>();
		Transform[] array = itemPoints;
		foreach (Transform transform in array)
		{
			listItemPos.Add(transform.position);
		}
		for (int bodyPartAdderNumberTemp = bodyPartAdderNumber; bodyPartAdderNumberTemp > 0; bodyPartAdderNumberTemp--)
		{
			int index = UnityEngine.Random.Range(0, listItemPos.Count);
			IngameManager.Instance.CreateBodyPartAdder(listItemPos[index]);
			listItemPos.RemoveAt(index);
			yield return null;
		}
		for (int coinNumberTemp = coinNumber; coinNumberTemp > 0; coinNumberTemp--)
		{
			int index2 = UnityEngine.Random.Range(0, listItemPos.Count);
			IngameManager.Instance.CreateCoinItem(listItemPos[index2]);
			listItemPos.RemoveAt(index2);
			yield return null;
		}
		if (isCreateShield && listItemPos.Count > 0)
		{
			int index3 = UnityEngine.Random.Range(0, listItemPos.Count);
			IngameManager.Instance.CreateShieldItem(listItemPos[index3]);
			listItemPos.RemoveAt(index3);
			yield return null;
		}
		if (isCreateMagnet && listItemPos.Count > 0)
		{
			int index4 = UnityEngine.Random.Range(0, listItemPos.Count);
			IngameManager.Instance.CreateMagnetItem(listItemPos[index4]);
			listItemPos.RemoveAt(index4);
			yield return null;
		}
		List<Vector3> listBlockPos = new List<Vector3>();
		array = blockPoints;
		foreach (Transform transform2 in array)
		{
			listBlockPos.Add(transform2.position);
		}
		for (int blockNumberTemp = blockNumber; blockNumberTemp > 0; blockNumberTemp--)
		{
			int index5 = UnityEngine.Random.Range(0, listBlockPos.Count);
			IngameManager.Instance.CreateBlock(listBlockPos[index5]);
			listBlockPos.RemoveAt(index5);
			yield return null;
		}
		List<Vector3> listObstaclePos = new List<Vector3>();
		array = obstaclePoints;
		foreach (Transform transform3 in array)
		{
			listObstaclePos.Add(transform3.position);
		}
		for (int obstacleNumberTemp = obstacleNumber; obstacleNumberTemp > 0; obstacleNumberTemp--)
		{
			int index6 = UnityEngine.Random.Range(0, listObstaclePos.Count);
			IngameManager.Instance.CreateObstacle(listObstaclePos[index6]);
			listObstaclePos.RemoveAt(index6);
			yield return null;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Respawn"))
		{
			base.gameObject.SetActive(value: false);
			IngameManager.Instance.CreateGround(isCreateObjects: true);
		}
	}
}
