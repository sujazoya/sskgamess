using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField]
	private float smoothTime = 0.1f;

	private Vector3 velocity = Vector3.zero;

	private float originalZDistance;

	public static CameraController Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			return;
		}
		UnityEngine.Object.DestroyImmediate(Instance.gameObject);
		Instance = this;
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	private void Start()
	{
		originalZDistance = Mathf.Abs(Mathf.Abs(base.transform.position.z) - Mathf.Abs(PlayerController.Instance.transform.position.z));
	}

	public void ResetOriginalPosition(float time)
	{
		StartCoroutine(CRResetPosition(time));
	}

	private IEnumerator CRResetPosition(float time)
	{
		Vector3 startPos = base.transform.position;
		Vector3 endPos = new Vector3(0f, startPos.y, startPos.z);
		float t = 0f;
		while (t < time)
		{
			t += Time.deltaTime;
			float t2 = t / time;
			base.transform.position = Vector3.Lerp(startPos, endPos, t2);
			yield return null;
		}
	}

	public void FollowPlayer()
	{
		float x = (PlayerController.Instance.transform.position - base.transform.position).x;
		float num = (PlayerController.Instance.transform.position - base.transform.position).z - originalZDistance;
		if (num > 0f)
		{
			Vector3 target = base.transform.position + new Vector3(x, 0f, num);
			base.transform.position = Vector3.SmoothDamp(base.transform.position, target, ref velocity, smoothTime);
		}
	}
}
