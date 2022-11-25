using CBGames;
using System.Collections;
using UnityEngine;

public class SkinInfor : MonoBehaviour
{
	[Header("The name of this skin. This field must be different than others")]
	[Header("Skin Infor Config")]
	[SerializeField]
	private string skinName = string.Empty;

	[Header("Price of the skin")]
	[SerializeField]
	private int skinPrice;

	[Header("The material's color of this skin when it locked.")]
	[SerializeField]
	private Color lockedColor = Color.black;

	[Header("The material's color of this skin when it unlocked.")]
	[SerializeField]
	private Color unlockedColor = Color.white;

	[Header("Skin Infor References")]
	[SerializeField]
	private MeshFilter headMeshFilter;

	[SerializeField]
	private MeshFilter bodyMeshFilter;

	[SerializeField]
	private MeshRenderer headMeshRender;

	[SerializeField]
	private MeshRenderer bodyMeshRender;

	[SerializeField]
	private MeshRenderer tailMeshRender;

	private Coroutine CRRotatingObject;

	public Mesh BodyMesh => bodyMeshFilter.sharedMesh;

	public Mesh HeahMesh => headMeshFilter.sharedMesh;

	public Material Material => headMeshRender.sharedMaterial;

	public int SequenceNumber
	{
		get;
		private set;
	}

	public int SkinPrice => skinPrice;

	public bool IsUnlocked => PlayerPrefs.GetInt(skinName, 0) == 1;

	private void Awake()
	{
		if (skinPrice == 0)
		{
			PlayerPrefs.SetInt(skinName, 1);
			PlayerPrefs.Save();
		}
		headMeshRender.material.color = (IsUnlocked ? unlockedColor : lockedColor);
		bodyMeshRender.material.color = (IsUnlocked ? unlockedColor : lockedColor);
		tailMeshRender.material.color = (IsUnlocked ? unlockedColor : lockedColor);
	}

	public void SetSequenceNumber(int number)
	{
		SequenceNumber = number;
	}

	public void Unlock()
	{
		PlayerPrefs.SetInt(skinName, 1);
		PlayerPrefs.Save();
		ServicesManager.Instance.SoundManager.PlayOneSound(ServicesManager.Instance.SoundManager.unlock);
		ServicesManager.Instance.CoinManager.RemoveTotalCoins(skinPrice);
		headMeshRender.material.color = unlockedColor;
		bodyMeshRender.material.color = unlockedColor;
		tailMeshRender.material.color = unlockedColor;
	}

	public void RotateThisObject(float speed)
	{
		CRRotatingObject = StartCoroutine(CRRotatingSkin(speed));
	}

	private IEnumerator CRRotatingSkin(float speed)
	{
		while (true)
		{
			base.transform.eulerAngles += Vector3.up * speed * Time.deltaTime;
			yield return null;
		}
	}

	public void StopRotateAndResetObject(float scaledownTime)
	{
		StopCoroutine(CRRotatingObject);
		StartCoroutine(CRResetingAngles());
		StartCoroutine(CRResetingScale(scaledownTime));
	}

	private IEnumerator CRResetingAngles()
	{
		Vector3 startAngles = base.transform.rotation.eulerAngles;
		Vector3 endAngles = Vector3.zero;
		float rotatingTime = Mathf.Abs(endAngles.y - startAngles.y) / 180f;
		float t = 0f;
		while (t < rotatingTime)
		{
			t += Time.deltaTime;
			float t2 = EasyType.MatchedLerpType(LerpType.EaseInOutQuart, t / rotatingTime);
			Vector3 eulerAngles = Vector3.Lerp(startAngles, endAngles, t2);
			base.transform.eulerAngles = eulerAngles;
			yield return null;
		}
	}

	private IEnumerator CRResetingScale(float scaleDownTime)
	{
		Vector3 startScale = base.transform.localScale;
		Vector3 endScale = Vector3.one;
		float t = 0f;
		while (t < scaleDownTime)
		{
			t += Time.deltaTime;
			float t2 = EasyType.MatchedLerpType(LerpType.EaseInOutQuart, t / scaleDownTime);
			base.transform.localScale = Vector3.Lerp(startScale, endScale, t2);
			yield return null;
		}
	}

	public void MoveThisObject(Vector3 dir, float distance, float movingTime)
	{
		StartCoroutine(CRMoveHorizontal(dir, distance, movingTime));
	}

	private IEnumerator CRMoveHorizontal(Vector3 dir, float distance, float movingTime)
	{
		Vector3 startPos = base.transform.position;
		Vector3 endPos = startPos + dir * distance;
		float t = 0f;
		while (t < movingTime)
		{
			t += Time.deltaTime;
			float t2 = EasyType.MatchedLerpType(LerpType.EaseInOutQuart, t / movingTime);
			base.transform.position = Vector3.Lerp(startPos, endPos, t2);
			yield return null;
		}
	}

	public void ScaleThisObject(Vector3 endScale, float scaleTime)
	{
		StartCoroutine(CRScaleThisObject(endScale, scaleTime));
	}

	private IEnumerator CRScaleThisObject(Vector3 endScale, float scaleTime)
	{
		Vector3 startScale = base.transform.localScale;
		float t = 0f;
		while (t < scaleTime)
		{
			t += Time.deltaTime;
			float t2 = EasyType.MatchedLerpType(LerpType.EaseInOutQuart, t / scaleTime);
			base.transform.localScale = Vector3.Lerp(startScale, endScale, t2);
			yield return null;
		}
	}
}
