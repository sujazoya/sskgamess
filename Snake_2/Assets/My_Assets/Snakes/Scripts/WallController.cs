using UnityEngine;

public class WallController : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Respawn"))
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
