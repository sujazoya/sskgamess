using CBGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
	private const string BestDistance_PPK = "BEST_DISTANCE";

	private PlayerState playerState;

	[Header("Player Config")]
	[SerializeField]
	private int originalBodySize = 10;

	[SerializeField]
	private float bodyDistance = 1.8f;

	[SerializeField]
	private float minMovingSpeed = 12f;

	[SerializeField]
	private float maxMovingSpeed = 30f;

	[SerializeField]
	private int scoreToIncreaseMovingSpeed = 30;

	[SerializeField]
	private float movingSpeedIncreaseAmount = 1f;

	[SerializeField]
	private float rotatingSpeed = 300f;

	[SerializeField]
	private float limitRotatingAngle = 30f;

	[SerializeField]
	private float bodyPartScaleFactor = 1.3f;

	[SerializeField]
	private float activeShieldTime = 5f;

	[SerializeField]
	private float magnetModeTime = 10f;

	[Header("Player References")]
	[SerializeField]
	private BodyPartController bodyPartControl;

	[SerializeField]
	private TextMesh textMesh;

	[SerializeField]
	private MeshFilter meshFilter;

	[SerializeField]
	private MeshRenderer meshRender;

	[SerializeField]
	private MeshCollider meshCollider;

	[SerializeField]
	private MeshFilter shieldMeshFilter;

	private List<BodyPartController> listActiveBodyPartControl = new List<BodyPartController>();

	private RaycastHit hit;

	private float firstInputX;

	private float currentMovingSpeed;

	private bool allowMove;

	private bool firstCallPlayingState;

	public static PlayerController Instance
	{
		get;
		private set;
	}

	public PlayerState PlayerState
	{
		get
		{
			return playerState;
		}
		private set
		{
			if (value != playerState)
			{
				value = playerState;
				PlayerController.PlayerStateChanged(playerState);
			}
		}
	}

	public bool IsImmortal
	{
		get;
		private set;
	}

	public bool IsOnMagnetMode
	{
		get;
		private set;
	}

	public static event Action<PlayerState> PlayerStateChanged;

	private void OnEnable()
	{
		IngameManager.GameStateChanged += GameManager_GameStateChanged;
	}

	private void OnDisable()
	{
		IngameManager.GameStateChanged -= GameManager_GameStateChanged;
	}

	private void GameManager_GameStateChanged(IngameState obj)
	{
		if (obj == IngameState.Ingame_Playing)
		{
			PlayerLiving();
		}
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
		PlayerState = PlayerState.Player_Prepare;
		playerState = PlayerState.Player_Prepare;
		SkinInfor skinInfor = ServicesManager.Instance.SkinContainer.SkinInfors[ServicesManager.Instance.SkinContainer.SelectedSkinIndex];
		meshFilter.mesh = skinInfor.HeahMesh;
		meshCollider.sharedMesh = skinInfor.HeahMesh;
		shieldMeshFilter.mesh = skinInfor.HeahMesh;
		meshRender.material = skinInfor.Material;
		IsImmortal = false;
		listActiveBodyPartControl.Add(bodyPartControl);
		for (int i = 0; i < originalBodySize - 1; i++)
		{
			AddBodyPart();
		}
		StartCoroutine(CRSpeepUp());
	}

	private void Update()
	{
		if (playerState == PlayerState.Player_Living)
		{
			textMesh.text = listActiveBodyPartControl.Count.ToString();
		}
	}

	private void PlayerLiving()
	{
		PlayerState = PlayerState.Player_Living;
		playerState = PlayerState.Player_Living;
		if (IngameManager.Instance.IsRevived)
		{
			firstCallPlayingState = false;
			textMesh.gameObject.SetActive(value: true);
			meshRender.enabled = true;
			for (int i = 0; i < originalBodySize - 1; i++)
			{
				AddBodyPart();
			}
			EnableAllShields();
		}
		if (!firstCallPlayingState)
		{
			firstCallPlayingState = true;
			//StartCoroutine(CRMoveBodyParts());
			//StartCoroutine(CRIncreaseMovingSpeed());
		}
	}

	private void PlayerDie()
	{
		PlayerState = PlayerState.Player_Dead;
		playerState = PlayerState.Player_Dead;
		textMesh.gameObject.SetActive(value: false);
		ServicesManager.Instance.ShareManager.CreateScreenshot();
		bodyPartControl.SetActiveShield(active: false);
		StartCoroutine(CRHandleGameState());
		ServicesManager.Instance.SoundManager.PlayOneSound(ServicesManager.Instance.SoundManager.playerDead);
		if (base.transform.position.z >= PlayerPrefs.GetFloat("BEST_DISTANCE", 0f))
		{
			PlayerPrefs.SetFloat("BEST_DISTANCE", base.transform.position.z);
		}
	}
    private void FixedUpdate()
    {
		if (playerState != PlayerState.Player_Living|| IngameManager.Instance.IngameState == IngameState.Ingame_Pause)		
			return;
		listActiveBodyPartControl[0].transform.Translate(listActiveBodyPartControl[0].transform.forward * currentMovingSpeed * Time.smoothDeltaTime, Space.World);
		if (Input.GetMouseButtonDown(0))
		{
			//allowMove = false;
			if (EventSystem.current.currentSelectedGameObject == null)
			{
				allowMove = true;
				firstInputX = Camera.main.ScreenToWorldPoint(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 10f)).x;
			}
		}
	 if (Input.GetMouseButton(0) && allowMove)
		{
			float x = Camera.main.ScreenToWorldPoint(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 10f)).x;
			if (Mathf.Abs(Mathf.Abs(x) - Mathf.Abs(firstInputX)) > 0.1f)
			{
				if (x > firstInputX)
				{
					Transform transform = listActiveBodyPartControl[0].transform;
					transform.Rotate(Vector3.up * rotatingSpeed * Time.deltaTime);
					float y = transform.eulerAngles.y;
					y = ((y > 180f) ? (y - 360f) : y);
					if (y >= limitRotatingAngle)
					{
						transform.eulerAngles = new Vector3(transform.eulerAngles.x, limitRotatingAngle, transform.eulerAngles.z);
					}
					listActiveBodyPartControl[0].transform.rotation = transform.rotation;
				}
				else if (x < firstInputX)
				{
					Transform transform2 = listActiveBodyPartControl[0].transform;
					transform2.Rotate(Vector3.down * rotatingSpeed * Time.deltaTime);
					float y2 = transform2.eulerAngles.y;
					y2 = ((y2 > 180f) ? (y2 - 360f) : y2);
					if (y2 <= 0f - limitRotatingAngle)
					{
						transform2.eulerAngles = new Vector3(transform2.eulerAngles.x, 0f - limitRotatingAngle, transform2.eulerAngles.z);
					}
					listActiveBodyPartControl[0].transform.rotation = transform2.rotation;
				}
			}
			firstInputX = x;
		}
		for (int i = 1; i < listActiveBodyPartControl.Count; i++)
		{
			float num = Vector3.Distance(listActiveBodyPartControl[i].transform.position, listActiveBodyPartControl[i - 1].transform.position);
			Vector3 position = listActiveBodyPartControl[i - 1].transform.position;
			position.y = listActiveBodyPartControl[0].transform.position.y;
			float num2 = Time.deltaTime * num / bodyDistance * currentMovingSpeed;
			if (num2 > 0.5f)
			{
				num2 = 0.5f;
			}
			listActiveBodyPartControl[i].transform.position = Vector3.Slerp(listActiveBodyPartControl[i].transform.position, position, num2);
			listActiveBodyPartControl[i].transform.rotation = Quaternion.Slerp(listActiveBodyPartControl[i].transform.rotation, listActiveBodyPartControl[i - 1].transform.rotation, num2);
		}
		CameraController.Instance.FollowPlayer();

		if (currentMovingSpeed < minMovingSpeed)		
			return ;	
		
			if (currentMovingSpeed < maxMovingSpeed)
			{
				float num = minMovingSpeed;
				int num2 = ServicesManager.Instance.ScoreManager.CurrentScore / scoreToIncreaseMovingSpeed;
				for (int i = 0; i < num2; i++)
				{
					num += movingSpeedIncreaseAmount;
				}
				currentMovingSpeed = num;				
			}
		//currentMovingSpeed = maxMovingSpeed;
	}
    private IEnumerator CRMoveBodyParts()
	{
		while (true)
		{
			if (playerState != PlayerState.Player_Living)
			{
				yield break;
			}
			if (IngameManager.Instance.IngameState == IngameState.Ingame_Pause)
			{
				yield return null;
			}
			else
			{
				if (IngameManager.Instance.IngameState != 0)
				{
					continue;
				}
				Ray ray = new Ray(meshRender.bounds.center, base.transform.forward);
				Ray ray2 = new Ray(meshRender.bounds.center + Vector3.left * (meshRender.bounds.size.x / 3f), base.transform.forward);
				Ray ray3 = new Ray(meshRender.bounds.center + Vector3.right * (meshRender.bounds.size.x / 3f), base.transform.forward);
				bool flag = Physics.Raycast(ray, out hit, meshRender.bounds.size.z / 2f + 0.1f);
				if (!flag)
				{
					flag = Physics.Raycast(ray2, out hit, meshRender.bounds.size.z / 2f + 0.1f);
				}
				if (!flag)
				{
					flag = Physics.Raycast(ray3, out hit, meshRender.bounds.size.z / 2f + 0.1f);
				}
				if (flag)
				{
					//if (hit.collider.CompareTag("Respawn"))
					//{
					//	BlockController component = hit.collider.GetComponent<BlockController>();
					//	if (!IsImmortal)
					//	{
					//		component.CountDownBrokenNumber();
					//		EffectManager.Instance.CreateBodyPartExplode(base.transform.position);
					//		meshRender.enabled = false;
					//		if (listActiveBodyPartControl.Count == 1)
					//		{
					//			PlayerDie();
					//			yield break;
					//		}
					//		yield return null;
					//		base.transform.position = listActiveBodyPartControl[1].transform.position;
					//		listActiveBodyPartControl[1].gameObject.SetActive(value: false);
					//		meshRender.enabled = true;
					//		listActiveBodyPartControl.RemoveAt(1);
					//		ServicesManager.Instance.SoundManager.PlayOneSound(ServicesManager.Instance.SoundManager.destroyBody);
					//	}
					//	else
					//	{
					//		component.CountDownBrokenNumber();
					//	}
					//}
					//else if (hit.collider.CompareTag("Finish"))
					//{
					//	EffectManager.Instance.CreateBodyPartExplode(base.transform.position);
					//	meshRender.enabled = false;
					//	if (listActiveBodyPartControl.Count == 1)
					//	{
					//		break;
					//	}
					//	yield return null;
					//	base.transform.position = listActiveBodyPartControl[1].transform.position;
					//	listActiveBodyPartControl[1].gameObject.SetActive(value: false);
					//	meshRender.enabled = true;
					//	listActiveBodyPartControl.RemoveAt(1);
					//	ServicesManager.Instance.SoundManager.PlayOneSound(ServicesManager.Instance.SoundManager.destroyBody);
					//}
					continue;
				}
				listActiveBodyPartControl[0].transform.Translate(listActiveBodyPartControl[0].transform.forward * currentMovingSpeed * Time.smoothDeltaTime, Space.World);
				if (Input.GetMouseButtonDown(0))
				{
					allowMove = false;
					if (EventSystem.current.currentSelectedGameObject == null)
					{
						allowMove = true;
						firstInputX = Camera.main.ScreenToWorldPoint(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 10f)).x;
					}
				}
				else if (Input.GetMouseButton(0) && allowMove)
				{
					float x = Camera.main.ScreenToWorldPoint(new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, 10f)).x;
					if (Mathf.Abs(Mathf.Abs(x) - Mathf.Abs(firstInputX)) > 0.1f)
					{
						if (x > firstInputX)
						{
							Transform transform = listActiveBodyPartControl[0].transform;
							transform.Rotate(Vector3.up * rotatingSpeed * Time.deltaTime);
							float y = transform.eulerAngles.y;
							y = ((y > 180f) ? (y - 360f) : y);
							if (y >= limitRotatingAngle)
							{
								transform.eulerAngles = new Vector3(transform.eulerAngles.x, limitRotatingAngle, transform.eulerAngles.z);
							}
							listActiveBodyPartControl[0].transform.rotation = transform.rotation;
						}
						else if (x < firstInputX)
						{
							Transform transform2 = listActiveBodyPartControl[0].transform;
							transform2.Rotate(Vector3.down * rotatingSpeed * Time.deltaTime);
							float y2 = transform2.eulerAngles.y;
							y2 = ((y2 > 180f) ? (y2 - 360f) : y2);
							if (y2 <= 0f - limitRotatingAngle)
							{
								transform2.eulerAngles = new Vector3(transform2.eulerAngles.x, 0f - limitRotatingAngle, transform2.eulerAngles.z);
							}
							listActiveBodyPartControl[0].transform.rotation = transform2.rotation;
						}
					}
					firstInputX = x;
				}
				for (int i = 1; i < listActiveBodyPartControl.Count; i++)
				{
					float num = Vector3.Distance(listActiveBodyPartControl[i].transform.position, listActiveBodyPartControl[i - 1].transform.position);
					Vector3 position = listActiveBodyPartControl[i - 1].transform.position;
					position.y = listActiveBodyPartControl[0].transform.position.y;
					float num2 = Time.deltaTime * num / bodyDistance * currentMovingSpeed;
					if (num2 > 0.5f)
					{
						num2 = 0.5f;
					}
					listActiveBodyPartControl[i].transform.position = Vector3.Slerp(listActiveBodyPartControl[i].transform.position, position, num2);
					listActiveBodyPartControl[i].transform.rotation = Quaternion.Slerp(listActiveBodyPartControl[i].transform.rotation, listActiveBodyPartControl[i - 1].transform.rotation, num2);
				}
				yield return null;
				Ray ray4 = new Ray(meshRender.bounds.center, base.transform.forward);
				ray2 = new Ray(meshRender.bounds.center + Vector3.left * (meshRender.bounds.size.x / 3f), base.transform.forward);
				ray3 = new Ray(meshRender.bounds.center + Vector3.right * (meshRender.bounds.size.x / 3f), base.transform.forward);
				flag = Physics.Raycast(ray4, out hit, meshRender.bounds.size.z * 2f + 0.1f);
				if (!flag)
				{
					flag = Physics.Raycast(ray2, out hit, meshRender.bounds.size.z * 2f + 0.1f);
				}
				if (!flag)
				{
					flag = Physics.Raycast(ray3, out hit, meshRender.bounds.size.z * 2f + 0.1f);
				}
				if (!flag)
				{
					CameraController.Instance.FollowPlayer();
				}
			}
		}
		PlayerDie();
	}

	private IEnumerator CRScaleAllBodies()
	{
		for (int i = 0; i < listActiveBodyPartControl.Count; i++)
		{
			listActiveBodyPartControl[i].Scale(bodyPartScaleFactor, 0.05f);
			yield return new WaitForSeconds(0.05f);
		}
	}

	private IEnumerator CRHandleGameState()
	{
		yield return new WaitForSeconds(0.5f);
		if (!IngameManager.Instance.IsRevived )
		{
			IngameManager.Instance.Revive();
		}
		else
		{
			IngameManager.Instance.GameOver();
		}
	}

	//private IEnumerator CRIncreaseMovingSpeed()
	//{
	//	while (currentMovingSpeed < minMovingSpeed)
	//	{
	//		yield return null;
	//	}
	//	while (true)
	//	{
	//		if (currentMovingSpeed < maxMovingSpeed)
	//		{
	//			float num = minMovingSpeed;
	//			int num2 = ServicesManager.Instance.ScoreManager.CurrentScore / scoreToIncreaseMovingSpeed;
	//			for (int i = 0; i < num2; i++)
	//			{
	//				num += movingSpeedIncreaseAmount;
	//			}
	//			currentMovingSpeed = num;
	//			if (currentMovingSpeed >= maxMovingSpeed)
	//			{
	//				break;
	//			}
	//			yield return null;
	//			continue;
	//		}
	//		yield break;
	//	}
	//	currentMovingSpeed = maxMovingSpeed;
	//}

	private IEnumerator CRSpeepUp()
	{
		float t = 0f;
		float startSpeed = 0f;
		float endSpeed = minMovingSpeed;
		float increasingTime = 2f;
		while (t < increasingTime)
		{
			while (IngameManager.Instance.IngameState == IngameState.Ingame_Pause)
			{
				yield return null;
			}
			t += Time.deltaTime;
			float t2 = t / increasingTime;
			currentMovingSpeed = Mathf.Lerp(startSpeed, endSpeed, t2);
			yield return null;
		}
	}

	private IEnumerator CRCountDownShieldTime()
	{
		float temp = activeShieldTime;
		while (temp > 0f)
		{
			temp -= Time.deltaTime;
			yield return null;
		}
		IsImmortal = false;
		for (int i = 0; i < listActiveBodyPartControl.Count; i++)
		{
			listActiveBodyPartControl[i].SetActiveShield(active: false);
			yield return null;
		}
	}

	private IEnumerator CREnableAllShields()
	{
		IsImmortal = true;
		for (int i = 0; i < listActiveBodyPartControl.Count; i++)
		{
			listActiveBodyPartControl[i].SetActiveShield(active: true);
			yield return null;
		}
		StartCoroutine(CRCountDownShieldTime());
	}

	public void AddBodyPart()
	{
		Vector3 position = listActiveBodyPartControl[listActiveBodyPartControl.Count - 1].transform.position;
		Quaternion rotation = listActiveBodyPartControl[listActiveBodyPartControl.Count - 1].transform.rotation;
		BodyPartController bodyPart = PoolManager.Instance.GetBodyPart();
		bodyPart.transform.position = position;
		bodyPart.transform.rotation = rotation;
		bodyPart.gameObject.SetActive(value: true);
		listActiveBodyPartControl.Add(bodyPart);
	}

	public void ScaleAllBodies()
	{
		StopCoroutine("CRScaleAllBodies");
		StartCoroutine(CRScaleAllBodies());
	}

	public float GetBestDistance()
	{
		return PlayerPrefs.GetFloat("BEST_DISTANCE");
	}

	public void EnableAllShields()
	{
		StartCoroutine(CREnableAllShields());
	}

	public void SetMagetMode(bool isActive)
	{
		IsOnMagnetMode = isActive;
		if (IsOnMagnetMode)
		{
			ViewManager.Instance.IngameViewController.PlayingViewController.CountDownMagnetMode(magnetModeTime);
		}
	}

	static PlayerController()
	{
		PlayerController.PlayerStateChanged = delegate
		{
		};
	}   
    private void OnTriggerEnter(Collider collision)
    {
		if (collision.gameObject.CompareTag("Respawn"))
		{
			BlockController component = collision.gameObject.GetComponent<BlockController>();
			if (!IsImmortal)
			{
				component.CountDownBrokenNumber();
				EffectManager.Instance.CreateBodyPartExplode(base.transform.position);
				meshRender.enabled = false;
				if (listActiveBodyPartControl.Count == 1)
				{
					PlayerDie();					
				}				 
				base.transform.position = listActiveBodyPartControl[1].transform.position;
				listActiveBodyPartControl[1].gameObject.SetActive(value: false);
				meshRender.enabled = true;
				listActiveBodyPartControl.RemoveAt(1);
				ServicesManager.Instance.SoundManager.PlayOneSound(ServicesManager.Instance.SoundManager.destroyBody);
			}
			else
			{
				component.CountDownBrokenNumber();
			}
		}
		else if (collision.gameObject.CompareTag("Finish"))
		{
			EffectManager.Instance.CreateBodyPartExplode(base.transform.position);
			meshRender.enabled = false;
			if (listActiveBodyPartControl.Count == 1)
			{
				PlayerDie();
			}			
			transform.position = listActiveBodyPartControl[1].transform.position;
			listActiveBodyPartControl[1].gameObject.SetActive(value: false);
			meshRender.enabled = true;
			listActiveBodyPartControl.RemoveAt(1);
			ServicesManager.Instance.SoundManager.PlayOneSound(ServicesManager.Instance.SoundManager.destroyBody);
		}
	}
}
