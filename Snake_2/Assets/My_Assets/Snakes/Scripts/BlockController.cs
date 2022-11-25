using CBGames;
using UnityEngine;

public class BlockController : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	private MeshRenderer meshRender;

	[SerializeField]
	private TextMesh textMesh;

	[SerializeField]
	private BlockType blockType;

	private int currentBrokenNumber;

	public void InitValues(int blockNumber)
	{
		currentBrokenNumber = blockNumber;
		textMesh.text = currentBrokenNumber.ToString();
	}

	public BlockType GetBlockType()
	{
		return blockType;
	}

	public void CountDownBrokenNumber()
	{
		currentBrokenNumber--;
		ServicesManager.Instance.ScoreManager.AddCurrentScore(1);
		textMesh.text = currentBrokenNumber.ToString();
		if (currentBrokenNumber <= 0)
		{
			Explode();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Respawn"))
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void Explode()
	{
		ServicesManager.Instance.SoundManager.PlayOneSound(ServicesManager.Instance.SoundManager.destroyBlock);
		base.gameObject.SetActive(value: false);
		EffectManager.Instance.CreateBlockExplode(blockType, base.transform.position);
		Vector3 pos = base.transform.position + Vector3.up * (meshRender.bounds.size.y + 0.1f);
		IngameManager.Instance.CreateFadingText(pos, blockType);
	}
}
