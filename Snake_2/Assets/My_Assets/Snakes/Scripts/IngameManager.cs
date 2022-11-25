using CBGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameManager : MonoBehaviour
{
	[Header("In Game Config")]
	[SerializeField]
	private float reviveWaitTime = 5f;

	[SerializeField]
	private int firstEmptyGround = 5;

	[SerializeField]
	private int nextNormalGround = 5;

	[SerializeField]
	private float firstGroundZPos = -5f;

	[SerializeField]
	[Range(0f, 3f)]
	private int minBodyPartAdder = 2;

	[SerializeField]
	[Range(0f, 8f)]
	private int maxBodyPartAdder = 8;

	[SerializeField]
	private int scoreToIncreaseBodyPartAdder = 30;

	[SerializeField]
	private int bodyPartAdderIncreaseAmount = 1;

	[SerializeField]
	[Range(0f, 5f)]
	private int minBlock = 5;

	[SerializeField]
	[Range(0f, 13f)]
	private int maxBlock = 13;

	[SerializeField]
	private int scoreToIncreaseBlock = 30;

	[SerializeField]
	private int blockIncreaseAmount = 1;

	[SerializeField]
	[Range(0f, 3f)]
	private int minObstacle = 3;

	[SerializeField]
	[Range(0f, 13f)]
	private int maxObstacle = 13;

	[SerializeField]
	private int scoreToIncreaseObstacle = 30;

	[SerializeField]
	private int obstacleIncreaseAmount = 1;

	[SerializeField]
	[Range(0f, 3f)]
	private int minCoin;

	[SerializeField]
	[Range(0f, 5f)]
	private int maxCoin = 5;

	[SerializeField]
	[Range(0f, 1f)]
	private float shieldFrequency = 0.15f;

	[SerializeField]
	[Range(0f, 1f)]
	private float magnetFrequency = 0.01f;

	[SerializeField]
	private float textMovingUpSpeed = 10f;

	[SerializeField]
	private List<BlockConfig> listBlockConfig = new List<BlockConfig>();

	[SerializeField]
	private List<BodyPartAdderConfig> listBodyPartAdderConfig = new List<BodyPartAdderConfig>();

	[SerializeField]
	private List<FadingTextConfig> listFadingTextConfig = new List<FadingTextConfig>();

	private IngameState gameState = IngameState.Ingame_GameOver;

	private Vector3 groundSize = Vector3.zero;

	private Vector3 nextGroundPos = Vector3.zero;

	private int currentBodyPartAdderNumber;

	private int currentBlockNumber;

	private int currentObstacleNumber;

	public static IngameManager Instance
	{
		get;
		private set;
	}

	public static int EnvironmentIndex
	{
		get;
		private set;
	}

	public IngameState IngameState
	{
		get
		{
			return gameState;
		}
		private set
		{
			if (value != gameState)
			{
				gameState = value;
				IngameManager.GameStateChanged(gameState);
			}
		}
	}

	public float ReviveWaitTime
	{
		get;
		private set;
	}

	public bool IsRevived
	{
		get;
		private set;
	}

	public static event Action<IngameState> GameStateChanged;

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
		Application.targetFrameRate = 60;
		ViewManager.Instance.OnLoadingSceneDone(SceneManager.GetActiveScene().name);
		ServicesManager.Instance.ScoreManager.ResetCurrentScore();
		ServicesManager.Instance.CoinManager.ResetCollectedCoins();
		ReviveWaitTime = reviveWaitTime;
		IsRevived = false;
		currentBodyPartAdderNumber = minBodyPartAdder;
		currentBlockNumber = minBlock;
		currentObstacleNumber = minObstacle;
		groundSize = PoolManager.Instance.GetGroundSize();
		nextGroundPos = new Vector3(0f, 0f, firstGroundZPos);
		for (int i = 0; i < firstEmptyGround; i++)
		{
			CreateGround(isCreateObjects: false);
		}
		for (int j = 0; j < nextNormalGround; j++)
		{
			CreateGround(isCreateObjects: true);
		}
		PlayingGame();
	}

	public void PlayingGame()
	{
		IngameState = IngameState.Ingame_Playing;
		gameState = IngameState.Ingame_Playing;
		if (IsRevived)
		{
			ResumeBackgroundMusic(0.5f);
		}
		else
		{
			PlayBackgroundMusic(0.5f);
		}
		StartCoroutine(CRIncreaseBodyPartAdder());
		StartCoroutine(CRIncreaseBlock());
		StartCoroutine(CRIncreaseObstacle());
	}

	public void PauseGame()
	{
		IngameState = IngameState.Ingame_Pause;
		gameState = IngameState.Ingame_Pause;
		PauseBackgroundMusic(0f);
	}

	public void UnPauseGame()
	{
		IngameState = IngameState.Ingame_Playing;
		gameState = IngameState.Ingame_Playing;
		ResumeBackgroundMusic(0f);
	}

	public void Revive()
	{
		IngameState = IngameState.Ingame_Revive;
		gameState = IngameState.Ingame_Revive;
		PauseBackgroundMusic(0.5f);
	}

	public void GameOver()
	{
		IngameState = IngameState.Ingame_GameOver;
		gameState = IngameState.Ingame_GameOver;
		StopBackgroundMusic(0f);
	}

	public void LoadScene(string sceneName, float delay)
	{
		StartCoroutine(CRLoadingScene(sceneName, delay));
	}

	private IEnumerator CRLoadingScene(string sceneName, float delay)
	{
		yield return new WaitForSeconds(delay);
		SceneManager.LoadScene(sceneName);
	}

	private void PlayBackgroundMusic(float delay)
	{
		StartCoroutine(CRPlayBGMusic(delay));
	}

	private IEnumerator CRPlayBGMusic(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (EnvironmentIndex == 0 && ServicesManager.Instance.SoundManager.background_Forest != null)
		{
			ServicesManager.Instance.SoundManager.PlayMusic(ServicesManager.Instance.SoundManager.background_Forest, 0.5f);
		}
		else if (EnvironmentIndex == 1 && ServicesManager.Instance.SoundManager.background_Desert != null)
		{
			ServicesManager.Instance.SoundManager.PlayMusic(ServicesManager.Instance.SoundManager.background_Desert, 0.5f);
		}
		else if (EnvironmentIndex == 2 && ServicesManager.Instance.SoundManager.background_Cemetery != null)
		{
			ServicesManager.Instance.SoundManager.PlayMusic(ServicesManager.Instance.SoundManager.background_Cemetery, 0.5f);
		}
		else if (EnvironmentIndex == 3 && ServicesManager.Instance.SoundManager.background_Snowy != null)
		{
			ServicesManager.Instance.SoundManager.PlayMusic(ServicesManager.Instance.SoundManager.background_Snowy, 0.5f);
		}
	}

	private void StopBackgroundMusic(float delay)
	{
		StartCoroutine(CRStopBGMusic(delay));
	}

	private IEnumerator CRStopBGMusic(float delay)
	{
		yield return new WaitForSeconds(delay);
		if ((EnvironmentIndex == 0 && ServicesManager.Instance.SoundManager.background_Forest != null) || (EnvironmentIndex == 1 && ServicesManager.Instance.SoundManager.background_Desert != null) || (EnvironmentIndex == 2 && ServicesManager.Instance.SoundManager.background_Cemetery != null) || (EnvironmentIndex == 3 && ServicesManager.Instance.SoundManager.background_Snowy != null))
		{
			ServicesManager.Instance.SoundManager.StopMusic(0.5f);
		}
	}

	private void PauseBackgroundMusic(float delay)
	{
		StartCoroutine(CRPauseBGMusic(delay));
	}

	private IEnumerator CRPauseBGMusic(float delay)
	{
		yield return new WaitForSeconds(delay);
		if ((EnvironmentIndex == 0 && ServicesManager.Instance.SoundManager.background_Forest != null) || (EnvironmentIndex == 1 && ServicesManager.Instance.SoundManager.background_Desert != null) || (EnvironmentIndex == 2 && ServicesManager.Instance.SoundManager.background_Cemetery != null) || (EnvironmentIndex == 3 && ServicesManager.Instance.SoundManager.background_Snowy != null))
		{
			ServicesManager.Instance.SoundManager.PauseMusic();
		}
	}

	private void ResumeBackgroundMusic(float delay)
	{
		StartCoroutine(CRResumeBGMusic(delay));
	}

	private IEnumerator CRResumeBGMusic(float delay)
	{
		yield return new WaitForSeconds(delay);
		if ((EnvironmentIndex == 0 && ServicesManager.Instance.SoundManager.background_Forest != null) || (EnvironmentIndex == 1 && ServicesManager.Instance.SoundManager.background_Desert != null) || (EnvironmentIndex == 2 && ServicesManager.Instance.SoundManager.background_Cemetery != null) || (EnvironmentIndex == 3 && ServicesManager.Instance.SoundManager.background_Snowy != null))
		{
			ServicesManager.Instance.SoundManager.ResumeMusic();
		}
	}

	private int GetRandomBodyPartNumber()
	{
		int currentScore = ServicesManager.Instance.ScoreManager.CurrentScore;
		int minBodyPartNumber = listBodyPartAdderConfig[0].MinBodyPartNumber;
		int num = 0;
		for (int i = 0; i < listBodyPartAdderConfig.Count; i++)
		{
			if (currentScore >= listBodyPartAdderConfig[i].MinScoreToCreate && num < listBodyPartAdderConfig[i].MaxBodyPartNumber)
			{
				num = listBodyPartAdderConfig[i].MaxBodyPartNumber;
			}
		}
		return UnityEngine.Random.Range(minBodyPartNumber, num);
	}

	private int GetBlockNumber(BlockType type)
	{
		foreach (BlockConfig item in listBlockConfig)
		{
			if (item.BlockType.Equals(type))
			{
				return UnityEngine.Random.Range(item.MinBlockNumber, item.MaxBlockNumber);
			}
		}
		return 1;
	}

	private IEnumerator CRIncreaseBodyPartAdder()
	{
		while (true)
		{
			if (gameState == IngameState.Ingame_Playing)
			{
				int num = minBodyPartAdder;
				int num2 = ServicesManager.Instance.ScoreManager.CurrentScore / scoreToIncreaseBodyPartAdder;
				for (int i = 0; i < num2; i++)
				{
					num += bodyPartAdderIncreaseAmount;
				}
				currentBodyPartAdderNumber = num;
				if (currentBodyPartAdderNumber >= maxBodyPartAdder)
				{
					break;
				}
				yield return null;
				continue;
			}
			yield break;
		}
		currentBodyPartAdderNumber = maxBodyPartAdder;
	}

	private IEnumerator CRIncreaseBlock()
	{
		while (true)
		{
			if (gameState == IngameState.Ingame_Playing)
			{
				int num = minBlock;
				int num2 = ServicesManager.Instance.ScoreManager.CurrentScore / scoreToIncreaseBlock;
				for (int i = 0; i < num2; i++)
				{
					num += blockIncreaseAmount;
				}
				currentBlockNumber = num;
				if (currentBlockNumber >= maxBlock)
				{
					break;
				}
				yield return null;
				continue;
			}
			yield break;
		}
		currentBlockNumber = maxBlock;
	}

	private IEnumerator CRIncreaseObstacle()
	{
		while (true)
		{
			if (gameState == IngameState.Ingame_Playing)
			{
				int num = minObstacle;
				int num2 = ServicesManager.Instance.ScoreManager.CurrentScore / scoreToIncreaseObstacle;
				for (int i = 0; i < num2; i++)
				{
					num += obstacleIncreaseAmount;
				}
				currentObstacleNumber = num;
				if (currentObstacleNumber >= maxObstacle)
				{
					break;
				}
				yield return null;
				continue;
			}
			yield break;
		}
		currentObstacleNumber = maxObstacle;
	}

	private IEnumerator PlayParticle(ParticleSystem par)
	{
		par.Play();
		yield return new WaitForSeconds(par.main.startLifetimeMultiplier);
		par.gameObject.SetActive(value: false);
	}

	public void SetContinueGame()
	{
		IsRevived = true;
		PlayingGame();
	}

	public static void SetEnvironmentIndex(int index)
	{
		EnvironmentIndex = index;
	}

	public void CreateGround(bool isCreateObjects)
	{
		GroundController groundControl = PoolManager.Instance.GetGroundControl();
		groundControl.transform.position = nextGroundPos;
		groundControl.gameObject.SetActive(value: true);
		if (isCreateObjects)
		{
			int bodyPartAdderNumber = UnityEngine.Random.Range(0, currentBodyPartAdderNumber);
			int blockNumber = UnityEngine.Random.Range(0, currentBlockNumber);
			int coinNumber = UnityEngine.Random.Range(minCoin, maxCoin);
			int obstacleNumber = UnityEngine.Random.Range(0, currentObstacleNumber);
			groundControl.CreateObjects(bodyPartAdderNumber, blockNumber, obstacleNumber, coinNumber, UnityEngine.Random.value <= shieldFrequency, UnityEngine.Random.value <= magnetFrequency);
		}
		GameObject leftWall = PoolManager.Instance.GetLeftWall();
		leftWall.transform.position = groundControl.GetLeftPos();
		leftWall.SetActive(value: true);
		GameObject rightWall = PoolManager.Instance.GetRightWall();
		rightWall.transform.position = groundControl.GetRightPos();
		rightWall.SetActive(value: true);
		nextGroundPos = groundControl.transform.position + Vector3.forward * groundSize.z;
	}

	public void CreateBodyPartAdder(Vector3 pos)
	{
		BodyPartAdder bodyPartAdder = PoolManager.Instance.GetBodyPartAdder();
		bodyPartAdder.transform.position = pos;
		bodyPartAdder.gameObject.SetActive(value: true);
		bodyPartAdder.InitValues(GetRandomBodyPartNumber());
	}

	public void CreateBlock(Vector3 pos)
	{
		BlockController blockControl = PoolManager.Instance.GetBlockControl(listBlockConfig);
		blockControl.transform.position = pos;
		blockControl.gameObject.SetActive(value: true);
		blockControl.InitValues(GetBlockNumber(blockControl.GetBlockType()));
	}

	public void CreateObstacle(Vector3 pos)
	{
		ObstacleController obstacleControl = PoolManager.Instance.GetObstacleControl();
		obstacleControl.transform.position = pos;
		obstacleControl.transform.eulerAngles = new Vector3(0f, UnityEngine.Random.Range(0, 360), 0f);
		obstacleControl.gameObject.SetActive(value: true);
	}

	public void CreateCoinItem(Vector3 pos)
	{
		ItemController coinControl = PoolManager.Instance.GetCoinControl();
		coinControl.transform.position = pos;
		coinControl.gameObject.SetActive(value: true);
		coinControl.InitValues();
	}

	public void CreateShieldItem(Vector3 pos)
	{
		ItemController shieldControl = PoolManager.Instance.GetShieldControl();
		shieldControl.transform.position = pos;
		shieldControl.gameObject.SetActive(value: true);
		shieldControl.InitValues();
	}

	public void CreateMagnetItem(Vector3 pos)
	{
		ItemController magnetControl = PoolManager.Instance.GetMagnetControl();
		magnetControl.transform.position = pos;
		magnetControl.gameObject.SetActive(value: true);
		magnetControl.InitValues();
	}

	public void CreateFadingText(Vector3 pos, BlockType blockType)
	{
		FadingTextConfig fadingTextConfig = null;
		foreach (FadingTextConfig item in listFadingTextConfig)
		{
			if (item.BlockType == blockType)
			{
				fadingTextConfig = item;
				break;
			}
		}
		FadingTextController fadingTextControl = PoolManager.Instance.GetFadingTextControl();
		fadingTextControl.transform.position = pos;
		fadingTextControl.gameObject.SetActive(value: true);
		fadingTextControl.SetTextAndMoveUp(fadingTextConfig.Texts[UnityEngine.Random.Range(0, fadingTextConfig.Texts.Length)], textMovingUpSpeed, fadingTextConfig.TextColor);
	}

	static IngameManager()
	{
		IngameManager.GameStateChanged = delegate
		{
		};
	}
}
