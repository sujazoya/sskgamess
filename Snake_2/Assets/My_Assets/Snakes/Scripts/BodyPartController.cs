using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class BodyPartController : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer meshRender;

	[SerializeField]
	private GameObject shield;

	private void OnBecameVisible()
	{
		meshRender.shadowCastingMode = ShadowCastingMode.On;
		meshRender.receiveShadows = true;
		if (PlayerController.Instance.IsImmortal)
		{
			shield.SetActive(value: true);
		}
		else
		{
			shield.SetActive(value: false);
		}
	}

	private void OnBecameInvisible()
	{
		meshRender.shadowCastingMode = ShadowCastingMode.Off;
		meshRender.receiveShadows = false;
	}

	public void Scale(float upFactor, float scalingTime)
	{
		StartCoroutine(CRScale(upFactor, scalingTime));
	}

	private IEnumerator CRScale(float upFactor, float scalingTime)
	{
		float t2 = 0f;
		Vector3 startScale = base.transform.localScale;
		Vector3 endScale = startScale * upFactor;
		while (t2 < scalingTime)
		{
			t2 += Time.deltaTime;
			float t3 = t2 / scalingTime;
			base.transform.localScale = Vector3.Lerp(startScale, endScale, t3);
			yield return null;
		}
		t2 = 0f;
		while (t2 < scalingTime)
		{
			t2 += Time.deltaTime;
			float t4 = t2 / scalingTime;
			base.transform.localScale = Vector3.Lerp(endScale, startScale, t4);
			yield return null;
		}
	}

	public void SetActiveShield(bool active)
	{
		shield.SetActive(active);
	}
}
