using UnityEngine;

public class CameraRotator : MonoBehaviour
{
	private void Update()
	{
		base.transform.eulerAngles += Vector3.down * 10f * Time.deltaTime;
	}
}
