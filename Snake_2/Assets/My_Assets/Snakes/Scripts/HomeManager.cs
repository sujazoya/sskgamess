using CBGames;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeManager : MonoBehaviour
{
	[SerializeField]
	private GameObject environmentObject;

	[SerializeField]
	private EnvironmentPack[] environmentPacks;

	[SerializeField]
	private MeshFilter headMeshFilter;

	[SerializeField]
	private MeshFilter bodyMeshFilter;

	[SerializeField]
	private MeshFilter tailMeshFilter;

	private void Start()
	{
		Application.targetFrameRate = 60;
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
		SkinInfor skinInfor = ServicesManager.Instance.SkinContainer.SkinInfors[ServicesManager.Instance.SkinContainer.SelectedSkinIndex];
		headMeshFilter.mesh = skinInfor.HeahMesh;
		headMeshFilter.GetComponent<MeshRenderer>().material = skinInfor.Material;
		bodyMeshFilter.mesh = skinInfor.BodyMesh;
		bodyMeshFilter.GetComponent<MeshRenderer>().material = skinInfor.Material;
		tailMeshFilter.mesh = skinInfor.BodyMesh;
		tailMeshFilter.GetComponent<MeshRenderer>().material = skinInfor.Material;
	}
}
