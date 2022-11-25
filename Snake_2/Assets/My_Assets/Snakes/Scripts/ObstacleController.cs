using UnityEngine;

public class ObstacleController : MonoBehaviour
{
	public string ObstacleName
	{
		get
		{
			return base.gameObject.name.Split('(')[0];
		}
		private set
		{
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Respawn"))
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
