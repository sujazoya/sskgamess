using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
[System.Serializable]
public class ItemSprints
{
    public Text levelText;
    public Text targetCounterText;
    public Text levelTargetText;
    public Text[] cionText;   
    public Text startCounter;
    public GameObject tap;
    [Header("Level Status")]   
    public Image bg;
    public Text levelStatus;
    public Text levelnum;
    public Text levelTarget;
    public Text killed;
    public string[] musics;
    public GameObject[] UIElements;
    public GameObject transition;
    public Text messageText;
    public GameObject[] floors;    
}

public class GameManager_Sprint : MonoBehaviour
{
    public delegate void OnGameStarts();
    public static OnGameStarts onGameStarts;
    [SerializeField] ItemSprints itemSprints;
    public Level_Sprint[] levels;
    public int killedCount;
    public static GameManager_Sprint Instance;
    GunSprint gunSprint;
    [SerializeField] GameObject gun;
    public bool test_build;
    private void Awake()
    {
        Instance = this;
        if (Game.PassedLevel == 0)
        {
            Game.PassedLevel = 1;
        }       
    }
    public void CallOnGameStarts()
    {
      if(onGameStarts != null)
        {
            onGameStarts();
        }
    }
    public void Restart()
    {
        MusicManager.PlaySfx("button");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < itemSprints.UIElements.Length; i++)
        {
            itemSprints.UIElements[i].SetActive(false);
        }
        itemSprints.UIElements[0].SetActive(true);
        itemSprints.startCounter.gameObject.SetActive(false);
        itemSprints.tap.gameObject.SetActive(false);
        gun.GetComponent<Rigidbody>().isKinematic = true;
        itemSprints.targetCounterText.gameObject.SetActive(false);       
        gunSprint = gun.GetComponent<GunSprint>();
        itemSprints.transition.SetActive(false);
        for (int i = 0; i < itemSprints.floors.Length; i++)
        {
            itemSprints.floors[i].SetActive(false);
        }
        onGameStarts += OnGameStart;
        for (int i = 0; i < itemSprints.cionText.Length; i++)
        {
            itemSprints.cionText[i].text = Game.TotalCoins.ToString();
        }
        //CreateLevel(0);
    }
    void OnGameStart()
    {
       
    }
    
    private void OnDestroy()
    {
        onGameStarts -= OnGameStart;
    }
    public void Play()
    {
        MusicManager.PlaySfx("button");
        StartCoroutine(ShowPanel(1, true));
       
    }
    public void CreateLevel(int currentLevel)
    {
        StartCoroutine(ShowPanel(2,false));        
        itemSprints.floors[currentLevel].SetActive(true);
        //Instantiate(levels[currentLevel]);        
        itemSprints.levelTargetText.text = levels[currentLevel].levelTarget.ToString();
        for (int i = 0; i < itemSprints.cionText.Length; i++)
        {
            itemSprints.cionText[i].text = Game.TotalCoins.ToString();
        }        
        targetLeft = levels[currentLevel].levelTarget;
        StartCoroutine(LateStart());
        Game.currentLevelTarget = levels[currentLevel].levelTarget;
        Game.currentLevel = currentLevel+1;
        itemSprints.levelText.text = Game.currentLevel.ToString();
        Game.achivedLevelTarget = 0;
    }
    IEnumerator LateStart()
    {
       
        yield return new WaitForSeconds(2f);
        CallOnGameStarts();
        itemSprints.startCounter.gameObject.SetActive(true);
        MusicManager.PlaySfx("count_down");
        itemSprints.startCounter.text = "1";
        yield return new WaitForSeconds(1f);
        itemSprints.startCounter.text = "2";
        yield return new WaitForSeconds(1f);
        itemSprints.startCounter.text = "3";
        yield return new WaitForSeconds(1f);
        itemSprints.startCounter.gameObject.SetActive(false);
        itemSprints.tap.SetActive(true);
        Game.gameStatus = Game.GameStatus.isPlaying;
        int musicIndex = Random.Range(0, itemSprints.musics.Length);
        MusicManager.PlayMusic(itemSprints.musics[musicIndex]);
       
        gun.GetComponent<Rigidbody>().isKinematic = false;
    }
    public void UpdateTaget()
    {
        StartCoroutine(TryUpdateTaget());
    }
    int targetLeft;
    IEnumerator TryUpdateTaget()
    {
        MusicManager.PlaySfx("blast");
        killedCount++;
        itemSprints.targetCounterText.gameObject.SetActive(true);
        itemSprints.targetCounterText.text = killedCount.ToString();
        yield return new WaitForSeconds(1.2f);
        MusicManager.PlaySfx("powerup");
        targetLeft--;
        itemSprints.levelTargetText.text = targetLeft.ToString();
        itemSprints.targetCounterText.gameObject.SetActive(false);
        Game.TotalCoins += 10;
        for (int i = 0; i < itemSprints.cionText.Length; i++)
        {
            itemSprints.cionText[i].text = Game.TotalCoins.ToString();
        }
        Game.achivedLevelTarget++;
        if(Game.achivedLevelTarget>= Game.currentLevelTarget)
        {
            OnGameover();
        }
    }
    public IEnumerator PlayPowerupSound(float wait)
    {
        yield return new WaitForSeconds(wait);
        MusicManager.PlaySfx("level_complete");

    }
    public void PlayBlastSound()
    {        
        MusicManager.PlaySfx("blast");

    }
    public void OnGameover()
    {
        if (Game.achivedLevelTarget < Game.currentLevelTarget)
        {
            StartCoroutine(Gameover(false));
        }
        else
        {
            StartCoroutine(Gameover(true));
        }        
    }
    IEnumerator Gameover(bool value)
    {
        MusicManager.PauseMusic(.01f);
        Game.gameStatus = Game.GameStatus.isGameover;
        StartCoroutine(ShowPanel(5, true));
        itemSprints.levelnum.text = Game.currentLevel.ToString();
        if (value == true)
        {
            MusicManager.PlaySfx("level_complete");
            itemSprints.levelStatus.text = "SUCCESS";
            itemSprints.levelStatus.color = Color.green;
            if(Game.PassedLevel<= Game.currentLevel)
            {
                int newLevel = Game.currentLevel;
                newLevel++;
                Game.PassedLevel = newLevel;
            }
           
        }
        else
        {
            MusicManager.PlaySfx("failed");
            itemSprints.bg.color = Color.red;
            itemSprints.levelStatus.text = "Fail";
            itemSprints.levelStatus.color = Color.red;                        
        }
        itemSprints.levelTarget.text = Game.currentLevelTarget.ToString();
        itemSprints.killed.text = Game.achivedLevelTarget.ToString();
        yield return new WaitForSeconds(1f);
       
    }
    public void ShowWarn()
    {
        Game.gameStatus = Game.GameStatus.isPaused;
        itemSprints.UIElements[6].gameObject.SetActive(true);
        AdmobAdmanager.Instance.ShowInterstitial();
    }
    public void GiveReward()
    {
        gunSprint.Replay();
        itemSprints.UIElements[6].gameObject.SetActive(false);
        StartCoroutine(ActivatePlayer());
    }
    public void ContituiWithCoins()
    {
        if (Game.TotalCoins >= 500&&!GoogleSheetHandler.test_build)
        {
            Game.TotalCoins -= 500;
            gunSprint.Replay();
            itemSprints.UIElements[6].gameObject.SetActive(false);
            StartCoroutine(ActivatePlayer());
        }
        else
        {
            ShowMessage("You Don't Hane Enough Coins");
        }
    }
    IEnumerator ActivatePlayer()
    {
        yield return new WaitForSeconds(2f);
        Game.gameStatus = Game.GameStatus.isPlaying;
    }
    public void ShowMessage(string message)
    {
        itemSprints.UIElements[4].gameObject.SetActive(true);
        itemSprints.messageText.text = message;
    }
    public void CloseMessage()
    {
        itemSprints.UIElements[4].gameObject.SetActive(false);        
    }
    public void WarnCancel()
    {
        itemSprints.UIElements[6].gameObject.SetActive(false);
        OnGameover();
    }
    public void ShowGunPanel()
    {
        StartCoroutine(ShowPanel(3, true));

    }
    public void CloseGunPanel()
    {
        StartCoroutine(ShowPanel(0,true));
    }
    public IEnumerator ShowPanel(int panelIndex,bool showAd)
    {
        itemSprints.transition.SetActive(true);
        MusicManager.PlaySfx_Other("transition");
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < itemSprints.UIElements.Length; i++)
        {
            itemSprints.UIElements[i].SetActive(false);
        }
        itemSprints.UIElements[panelIndex].SetActive(true);       
        yield return new WaitForSeconds(1f);
        itemSprints.transition.SetActive(false);
        if (showAd) { AdmobAdmanager.Instance.ShowInterstitial(); }
        

    }
}
