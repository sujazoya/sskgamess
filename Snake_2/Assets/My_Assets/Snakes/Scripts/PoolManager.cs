using CBGames;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
	[SerializeField]
	private GameObject bodyPartAdderPrefab;

	[SerializeField]
	private GameObject bodyPartPrefab;

	[SerializeField]
	private GameObject coinPrefab;

	[SerializeField]
	private GameObject shieldPrefab;

	[SerializeField]
	private GameObject magnetPrefab;

	[SerializeField]
	private GameObject fadingTextPrefab;

	[SerializeField]
	private BlockController[] blockPrefabs;

	[SerializeField]
	private EnvironmentPack[] environmentPacks;

	private List<GroundController> listGroundControl = new List<GroundController>();

	private List<BlockController> listBlockControl = new List<BlockController>();

	private List<BodyPartAdder> listBodyPartAdder = new List<BodyPartAdder>();

	private List<BodyPartController> listBodyPartControl = new List<BodyPartController>();

	private List<ObstacleController> listObstacleControl = new List<ObstacleController>();

	private List<ItemController> listCoinControl = new List<ItemController>();

	private List<ItemController> listShieldControl = new List<ItemController>();

	private List<ItemController> listMagnetControl = new List<ItemController>();

	private List<FadingTextController> listFadingTextControl = new List<FadingTextController>();

	private List<GameObject> listLeftWall = new List<GameObject>();

	private List<GameObject> listRightWall = new List<GameObject>();

	public static PoolManager Instance
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
		SkinInfor skinInfor = ServicesManager.Instance.SkinContainer.SkinInfors[ServicesManager.Instance.SkinContainer.SelectedSkinIndex];
		bodyPartAdderPrefab.GetComponent<MeshFilter>().mesh = skinInfor.BodyMesh;
		bodyPartAdderPrefab.GetComponent<MeshRenderer>().material = skinInfor.Material;
		bodyPartAdderPrefab.GetComponent<MeshCollider>().sharedMesh = skinInfor.BodyMesh;
		bodyPartPrefab.GetComponent<MeshFilter>().mesh = skinInfor.BodyMesh;
		bodyPartPrefab.GetComponent<MeshRenderer>().material = skinInfor.Material;
		bodyPartPrefab.transform.GetChild(0).GetComponent<MeshFilter>().mesh = skinInfor.BodyMesh;
		shieldPrefab.GetComponentInChildren<MeshFilter>().mesh = skinInfor.BodyMesh;
	}

	public Vector3 GetGroundSize()
	{
		return environmentPacks[IngameManager.EnvironmentIndex].GroundPrefab.GetComponent<MeshRenderer>().bounds.size;
	}

	public GroundController GetGroundControl()
	{
		GroundController groundController = (from a in listGroundControl
			where !a.gameObject.activeInHierarchy
			select a).FirstOrDefault();
		if (groundController == null)
		{
			groundController = UnityEngine.Object.Instantiate(environmentPacks[IngameManager.EnvironmentIndex].GroundPrefab, Vector3.zero, Quaternion.identity).GetComponent<GroundController>();
			groundController.gameObject.SetActive(value: false);
			listGroundControl.Add(groundController);
		}
		return groundController;
	}

	public BlockController GetBlockControl(List<BlockConfig> listBlockConfig)
	{
		BlockType blockType = RandomAvailableBlockType(listBlockConfig);
		BlockController blockController = (from a in listBlockControl
			where !a.gameObject.activeInHierarchy && a.GetBlockType().Equals(blockType)
			select a).FirstOrDefault();
		if (blockController == null)
		{
			blockController = UnityEngine.Object.Instantiate(GetBlockPrefab(blockType), Vector3.zero, Quaternion.identity).GetComponent<BlockController>();
			blockController.gameObject.SetActive(value: false);
			listBlockControl.Add(blockController);
		}
		return blockController;
	}

	private BlockType RandomAvailableBlockType(List<BlockConfig> listBlockConfig)
	{
		List<BlockType> list = new List<BlockType>();
		int currentScore = ServicesManager.Instance.ScoreManager.CurrentScore;
		for (int i = 0; i < listBlockConfig.Count; i++)
		{
			if (currentScore >= listBlockConfig[i].MinScoreToCreate)
			{
				list.Add(listBlockConfig[i].BlockType);
			}
		}
		return list[Random.Range(0, list.Count)];
	}

	private GameObject GetBlockPrefab(BlockType type)
	{
		return (from a in blockPrefabs
			where a.GetBlockType().Equals(type)
			select a).FirstOrDefault().gameObject;
	}

	public ObstacleController GetObstacleControl()
	{
		int index = UnityEngine.Random.Range(0, environmentPacks[IngameManager.EnvironmentIndex].ListObstaclePrefab.Count);
		string obstacleName = environmentPacks[IngameManager.EnvironmentIndex].ListObstaclePrefab[index].ObstacleName;
		ObstacleController obstacleController = (from a in listObstacleControl
			where !a.gameObject.activeInHierarchy && a.ObstacleName.Equals(obstacleName)
			select a).FirstOrDefault();
		if (obstacleController == null)
		{
			obstacleController = UnityEngine.Object.Instantiate(environmentPacks[IngameManager.EnvironmentIndex].ListObstaclePrefab[index], Vector3.zero, Quaternion.identity).GetComponent<ObstacleController>();
			obstacleController.gameObject.SetActive(value: false);
			listObstacleControl.Add(obstacleController);
		}
		return obstacleController;
	}

	public ItemController GetCoinControl()
	{
		ItemController itemController = (from a in listCoinControl
			where !a.gameObject.activeInHierarchy
			select a).FirstOrDefault();
		if (itemController == null)
		{
			itemController = UnityEngine.Object.Instantiate(coinPrefab, Vector3.zero, Quaternion.identity).GetComponent<ItemController>();
			itemController.gameObject.SetActive(value: false);
			listCoinControl.Add(itemController);
		}
		return itemController;
	}

	public ItemController GetShieldControl()
	{
		ItemController itemController = (from a in listShieldControl
			where !a.gameObject.activeInHierarchy
			select a).FirstOrDefault();
		if (itemController == null)
		{
			itemController = UnityEngine.Object.Instantiate(shieldPrefab, Vector3.zero, Quaternion.identity).GetComponent<ItemController>();
			itemController.gameObject.SetActive(value: false);
			listShieldControl.Add(itemController);
		}
		return itemController;
	}

	public ItemController GetMagnetControl()
	{
		ItemController itemController = (from a in listMagnetControl
			where !a.gameObject.activeInHierarchy
			select a).FirstOrDefault();
		if (itemController == null)
		{
			itemController = UnityEngine.Object.Instantiate(magnetPrefab, Vector3.zero, Quaternion.identity).GetComponent<ItemController>();
			itemController.gameObject.SetActive(value: false);
			listMagnetControl.Add(itemController);
		}
		return itemController;
	}

	public BodyPartAdder GetBodyPartAdder()
	{
		BodyPartAdder bodyPartAdder = (from a in listBodyPartAdder
			where !a.gameObject.activeInHierarchy
			select a).FirstOrDefault();
		if (bodyPartAdder == null)
		{
			bodyPartAdder = UnityEngine.Object.Instantiate(bodyPartAdderPrefab, Vector3.zero, Quaternion.identity).GetComponent<BodyPartAdder>();
			bodyPartAdder.gameObject.SetActive(value: false);
			listBodyPartAdder.Add(bodyPartAdder);
		}
		return bodyPartAdder;
	}

	public BodyPartController GetBodyPart()
	{
		BodyPartController bodyPartController = (from a in listBodyPartControl
			where !a.gameObject.activeInHierarchy
			select a).FirstOrDefault();
		if (bodyPartController == null)
		{
			bodyPartController = UnityEngine.Object.Instantiate(bodyPartPrefab, Vector3.zero, Quaternion.identity).GetComponent<BodyPartController>();
			listBodyPartControl.Add(bodyPartController);
			bodyPartController.gameObject.SetActive(value: false);
		}
		return bodyPartController;
	}

	public FadingTextController GetFadingTextControl()
	{
		FadingTextController fadingTextController = (from a in listFadingTextControl
			where !a.gameObject.activeInHierarchy
			select a).FirstOrDefault();
		if (fadingTextController == null)
		{
			fadingTextController = UnityEngine.Object.Instantiate(fadingTextPrefab, Vector3.zero, Quaternion.identity).GetComponent<FadingTextController>();
			fadingTextController.gameObject.SetActive(value: false);
			listFadingTextControl.Add(fadingTextController);
		}
		return fadingTextController;
	}

	public GameObject GetLeftWall()
	{
		GameObject gameObject = (from a in listLeftWall
			where !a.activeInHierarchy
			select a).FirstOrDefault();
		if (gameObject == null)
		{
			int index = UnityEngine.Random.Range(0, environmentPacks[IngameManager.EnvironmentIndex].ListLeftWallPrefab.Count);
			gameObject = UnityEngine.Object.Instantiate(environmentPacks[IngameManager.EnvironmentIndex].ListLeftWallPrefab[index], Vector3.zero, Quaternion.identity);
			listLeftWall.Add(gameObject);
			gameObject.SetActive(value: false);
		}
		return gameObject;
	}

	public GameObject GetRightWall()
	{
		GameObject gameObject = (from a in listRightWall
			where !a.activeInHierarchy
			select a).FirstOrDefault();
		if (gameObject == null)
		{
			int index = UnityEngine.Random.Range(0, environmentPacks[IngameManager.EnvironmentIndex].ListRightWallPrefab.Count);
			gameObject = UnityEngine.Object.Instantiate(environmentPacks[IngameManager.EnvironmentIndex].ListRightWallPrefab[index], Vector3.zero, Quaternion.identity);
			listRightWall.Add(gameObject);
			gameObject.SetActive(value: false);
		}
		return gameObject;
	}
}
