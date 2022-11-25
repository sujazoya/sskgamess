using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ManagerItems
{
    public GameObject[] coins;
   
    public GameObject[] deathEffects;    
    [Header("Gameover")]    
    public Text gameStatus;
    public Text currentLevelTarget;
    public Text achivedLevelTarget;
    /// <summary>
    /// /
    /// </summary>
    public GameObject[] uiItems;
    public GameObject life;
    public ParticleSystem life_AddEffect;
    public ParticleSystem shield_AddEffect;
    public GameObject levelUpgradeeffect;
    public Text enemyKilled;
    public ParticleSystem killed_Glow;
    public Text[] coinText;
    public ParticleSystem coin_Glow;
    public Text lifeText;
    public ParticleSystem life_Glow;
    public Text shieldText;
    public ParticleSystem shield_Glow;
    public GameObject transition;
    public GameObject loadingEffect;
    public GameObject[] environments;
    
}

public class GameManager_Defence : MonoBehaviour
{
    #region DELEGATES
    public delegate void OnGameStart();
    public static OnGameStart onGameStart;
    public delegate void OnGamePaused();
    public static OnGamePaused onGamepaused;
    public delegate void OnGameOver();
    public static OnGameOver onGameOver;
    public delegate void OnGameResume();
    public static OnGameResume onGameResume;
    #endregion

    public ManagerItems items;
    public static GameManager_Defence Instance;
    public Transform player;
    public Player_Tank player_script;
    public static bool playerDied = false;
    public Slider[] enemyHealthSlides;   
    //private MobilePostProcessing mobilePost;
    public GameObject game;
    public bool testBuild;
    AudioListener listener;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        if (Game.PassedLevel==0)
        {
            Game.PassedLevel = 1;
        }
        //GetPlayer();
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < enemyHealthSlides.Length; i++)
        {
            enemyHealthSlides[i].gameObject.SetActive(false);
        }       
       
        for (int i = 0; i <items.coins.Length; i++)
        {
            items.coins[i].SetActive(false);
        }
        for (int i = 0; i < items.coinText.Length; i++)
        {
            items.coinText[i].text = Game.TotalCoins.ToString();
        }
                     
        ShowUIMenu(0, true);        
        onGameStart += OnGameStarts;
        //mobilePost = Camera.main.GetComponent<MobilePostProcessing>();
        //mobilePost.enabled = false;
        game.SetActive(false);
        if (items.transition)
        {
            items.transition.SetActive(false);
        }
        if (Game.EnvTypes == 0)
        {
            Game.EnvTypes = 1;
        }
        listener = Camera.main.gameObject.GetComponent<AudioListener>();
        player.gameObject.SetActive(false);
        StartCoroutine(WaitAndConfirm());
    }
    IEnumerator WaitAndConfirm()
    {
        yield return new WaitUntil(() => GoogleSheetHandler.googlesheetInitilized);
        testBuild = GoogleSheetHandler.test_build;
    }
    public IEnumerator PlayTransition()
    {
        if (items.transition)
        {

            items.transition.SetActive(false);
            items.transition.SetActive(true);
            SoundManager.PlaySfx_Other("transition");
            yield return new WaitForSeconds(2);
            items.transition.SetActive(false);
        }

    }
    #region DELEGATES CALLS
    public void CallOnGameStarts()
    {
        if (onGameStart != null)
        {
            onGameStart();
        }
    }
    public void CallOnGameOver()
    {
        if (onGameOver != null)
        {
            onGameOver();
        }
    }
    public void CallOnGamePaused()
    {
        if (onGamepaused != null)
        {
            onGamepaused();
        }
    }
    public void CallOnGameResume()
    {
        if (onGameResume != null)
        {
            onGameResume();
        }
    }
    #endregion

    public void Play()
    {
        SoundManager.PlaySfx("button");
        StartCoroutine(OpenLevelPanel());      
    }
    IEnumerator OpenLevelPanel()
    {
        StartCoroutine(PlayTransition());
        yield return new WaitForSeconds(1);
        ShowUIMenu(2, true);       
        //mobilePost.enabled = true;
        game.SetActive(true);
        StartCoroutine(CallInterstitalAd(0.2f));
    }
    public void ShowUIMenu(int index, bool value)
    {
        for (int i = 0; i < items.uiItems.Length; i++)
        {
            items.uiItems[i].SetActive(false);
        }
        items.uiItems[index].SetActive(value);
        
    }
    int coinIndex;
    public void GiveCoin()
    {
        items.coins[coinIndex].SetActive(true);
        StartCoroutine(UpddateCoin(coinIndex, 0.65f));
        coinIndex++;
        if(coinIndex>= items.coins.Length)
        {
            coinIndex = 0;
        }
    }
    IEnumerator UpddateCoin(int index, float wait)
    {
        yield return new WaitForSeconds(wait);
        Game.TotalCoins += 10;
        for (int i = 0; i < items.coinText.Length; i++)
        {
            items.coinText[i].text = Game.TotalCoins.ToString();
        }
        SoundManager.PlaySfx("coin_add");
        items.coins[index].SetActive(false);
        items.coin_Glow.Play();
    }
    void GetPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        player_script = player.GetComponent<Player_Tank>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void Reset()
    {
        player.gameObject.SetActive(true);
        player_script.Reset();
        items.levelUpgradeeffect.SetActive(false);        
        //mobilePost.enabled = false;
    }
    int sliderIndex;
    public Slider CurrentSlider()
    {
        Slider activatedSlider;
        enemyHealthSlides[sliderIndex].gameObject.SetActive(true);
        activatedSlider = enemyHealthSlides[sliderIndex];
        sliderIndex++;
        if(sliderIndex>= enemyHealthSlides.Length-1)
        {
            sliderIndex = 0;
        }
        return activatedSlider;
    }
    int targetRemain;
    public void OnEnemyKilled()
    {
        Game.achivedLevelTarget++;
        targetRemain--;
        items.enemyKilled.text = targetRemain.ToString();
        items.killed_Glow.Play();
        if (Game.achivedLevelTarget== Game.currentLevelTarget)
        {
           Invoke(nameof(OnGameover),3);
        }
        
    }
    bool gameOvereCalled;
    private void LateUpdate()
    {
        int enemyHas = int.Parse(items.enemyKilled.text);
        if (Game.gameStatus == Game.GameStatus.isPlaying)
        {
            if (enemyHas==0&&!gameOvereCalled)
            {
                gameOvereCalled = true;
                Invoke(nameof(OnGameover), 3);
            }
        }
    }
    public void LevelPanel(bool value)
    {
        ShowUIMenu(2,value);
    }
    public void OnGameover()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length > 0)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                Destroy(enemies[i]);
            }
        }
        SoundManager.PauseMusic(0.02f);
        ShowUIMenu(3, true);
        game.SetActive(false);
        Animator goAnimator;
        goAnimator = items.uiItems[3].GetComponent<Animator>();
        if (Game.achivedLevelTarget < Game.currentLevelTarget)
        {
            goAnimator.SetTrigger("gameover");
            items.gameStatus.text = "Gameover";
            items.gameStatus.color = Color.red;
            SoundManager.PlaySfx("game_over");
        }
        else
        {
            goAnimator.SetTrigger("gamewon");
            items.gameStatus.text = "Succes";
            items.gameStatus.color = Color.green;
            SoundManager.PlaySfx("Level Won");
            int nextLevel = Game.currentLevel;
            nextLevel+=1;
            if (nextLevel > Game.PassedLevel)
            {
                Game.PassedLevel = nextLevel;
            }
        }
        items.currentLevelTarget.text = Game.currentLevelTarget.ToString();
        items.achivedLevelTarget.text=Game.achivedLevelTarget.ToString();
        StartCoroutine(CallInterstitalAd(2f));
        CallOnGameOver();
        //mobilePost.enabled = false;

    }
    IEnumerator CallInterstitalAd(float wait)
    {
        yield return new WaitForSeconds(wait);
        AdmobAdmanager.Instance.ShowInterstitial();
    }
    public void Pause(bool value) {
        if (value)
        {
            SoundManager.PlaySfx("panel_open");
        }
        else
        {
            //items.pausePanel.GetComponent<Animator>().SetTrigger("close");
            SoundManager.PlaySfx("panel_close");
        }
        ShowUIMenu(4, value);
    }
    public void ActivatePlayer()
    {
        player.gameObject.SetActive(false);
        player.gameObject.SetActive(true);
    }
    public void OnGameStarts()
    {        
        Game.gameStatus = Game.GameStatus.isPlaying;
        SoundManager.PlayMusic("Main_Music");
        targetRemain = Game.currentLevelTarget;
        if (items.loadingEffect) { items.loadingEffect.SetActive(false); }
        items.enemyKilled.text = Game.currentLevelTarget.ToString();
        UpdateUI();

    }
    public void AddLife()
    {
        items.life_AddEffect.gameObject.SetActive(true);
        items.life_AddEffect.Play();
        StartCoroutine(CompleteAddLife());
    }
   IEnumerator CompleteAddLife()
    {
        yield return new WaitForSeconds(1.2f);
        items.life_AddEffect.gameObject.SetActive(false);
        items.life_Glow.Play();
        Game.Life++;
        items.lifeText.text = Game.Life.ToString();
    }
    public void AddShield()
    {
        items.shield_AddEffect.gameObject.SetActive(true);
        items.shield_AddEffect.Play();
        StartCoroutine(CompleteAddShield());
    }
    IEnumerator CompleteAddShield()
    {
        yield return new WaitForSeconds(1.2f);
        items.shield_AddEffect.gameObject.SetActive(false);
        items.shield_Glow.Play();
        Game.Shield++;
        items.shieldText.text = Game.Shield.ToString();
    }
    public void UpdateUI()
    {
        items.lifeText.text = Game.Life.ToString();
        items.shieldText.text = Game.Shield.ToString();
        for (int i = 0; i < items.coinText.Length; i++)
        {
            items.coinText[i].text = Game.TotalCoins.ToString();
        }
        //items.enemyKilled.text = Game.achivedLevelTarget.ToString();       
    }
    public void ShowSettingPanel()
    {
        PlayButtonClip();
        items.uiItems[5].SetActive(true);
        StartCoroutine(CallInterstitalAd(1f));
    }
    public void CloseSettingPanel()
    {
        StartCoroutine(CloseSp());
        PlayButtonClip();       
    }
    IEnumerator CloseSp()
    {
        items.uiItems[5].GetComponent<Animator>().SetTrigger("close");
        yield return new WaitForSeconds(.5f);
        items.uiItems[5].SetActive(false);
    }
    public void ShowEnvPanel()
    {
        PlayButtonClip();
        items.uiItems[6].SetActive(true);
        StartCoroutine(CallInterstitalAd(1f));
    }
    public void CloseEnvPanel()
    {
        StartCoroutine(CloseEnv());
        PlayButtonClip();
    }
    IEnumerator CloseEnv()
    {
        items.uiItems[6].GetComponent<Animator>().SetTrigger("close");
        yield return new WaitForSeconds(.5f);
        items.uiItems[6].SetActive(false);
    }

    public void PlayButtonClip()
    {
        SoundManager.PlaySfx("button");
    }
    public void ApplyEnvironment()
    {
        for (int i = 0; i < items.environments.Length; i++)
        {
            items.environments[i].SetActive(false);
        }
        switch (Game.EnvTypes)
        {
            case 1:
                items.environments[0].SetActive(true);
                break;
            case 2:
                items.environments[1].SetActive(true);
                break;
            case 3:
                items.environments[2].SetActive(true);
                break;
        }
    }
    public void ActivateEnvironment(int index)
    {
        switch (index)
        {
            case 1:
                Game.EnvTypes = 1;               
                break;
            case 2:
                Game.EnvTypes = 2;               
                break;
            case 3:
                Game.EnvTypes = 3;               
                break;
        }
        CloseEnvPanel();
    }
    public void Restart()
    {
     Level_Loader.instance.LoadLevel(SceneManager.GetActiveScene().name);
    }
    private void OnDestroy()
    {
        onGameStart -= OnGameStarts;
    }   
    public void OnPause()
    {
        PlayButtonClip();
        items.uiItems[4].SetActive(true);
        Game.gameStatus = Game.GameStatus.isPaused;
        listener.enabled = false;
        StartCoroutine(CallInterstitalAd(2f));
    }
    public void OnResume()
    {
        listener.enabled = true;
        StartCoroutine(ClosePause());
        PlayButtonClip();
    }
    IEnumerator ClosePause()
    {
        items.uiItems[4].GetComponent<Animator>().SetTrigger("close");
        yield return new WaitForSeconds(.5f);
        items.uiItems[4].SetActive(false);
        yield return new WaitForSeconds(1f);
        Game.gameStatus = Game.GameStatus.isPlaying;       
    }
    void OnGamePause()
    {
        SoundManager.PauseMusic(0.2f);
    }
    public void OnWarn()
    {
        PlayButtonClip();
        items.uiItems[7].SetActive(true);
        Game.gameStatus = Game.GameStatus.isPaused;       
    }
    public void OnWarnClose()
    {        
        StartCoroutine(CloseWarn());
        PlayButtonClip();
    }
    IEnumerator CloseWarn()
    {
        items.uiItems[7].GetComponent<Animator>().SetTrigger("close");
        yield return new WaitForSeconds(.5f);
        items.uiItems[7].SetActive(false);       
    }
}
