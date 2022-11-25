using System.Collections;
using UnityEngine;

public class MeshLooper : MonoBehaviour
{
	[SerializeField]
	private float delay = 0.2f;

	[SerializeField]
	private Mesh[] meshs;

	[SerializeField]
	private MeshFilter meshFilter;

	private void Start()
	{
		StartCoroutine(CRLoppMesh());
	}

	private IEnumerator CRLoppMesh()
	{
		int i = 0;
		while (true)
		{
			meshFilter.mesh = meshs[i];
			yield return new WaitForSeconds(delay);
			i++;
			if (i == meshs.Length)
			{
				i = 0;
			}
		}
	}
}
