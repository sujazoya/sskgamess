using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using suja;
using TMPro;


[System.Serializable]
public class MyPlayer
{
    public Text       name;
    public Image      avatar;
    public Text       number;
    public Text       ammount;
    public Text       betDiemonds;
    public Text       betCoins;
    public Image      status;
    public Text       balance;
    public Text       diemonds;
    public Text       coins;
    public GameObject locks;
}
//[System.Serializable]
//public class CasinoPlayer
//{
//    public  string    playerName;
//    public  int       playerBalance;
//    public  int       playerDiemonds;
//    public  int       playerCoins;

//}
[System.Serializable]
public class MassegeItems
{
    public GameObject massegeCanvas;
    public Text massegeText;
    public Button massegeCancelButton;
}

[System.Serializable]
public class EntryChecker
{
    public GameObject ecItem;
    public Text nameText;
    public Text numberText;
    public Text ammountText;
    public Text diemondText;
    public Text coinText;
    public Image name_checker;
    public Image number_checker;
    public Image ammount_checker;
    public Image diemond_checker;
    public Image coin_checker;
}
[System.Serializable]
public class PlayerNumber
{
    public GameObject playerObject;
    public Image avatar;
    public Text name;
    public Text number;
    public Text ammounts;
    public Text diemonds;
    public Text coins;   
}
[System.Serializable]
public class MoneyGiver
{
    public GameObject mpneyGiver;
    public Text name;
    public Text addingText;
    public Text ammount;
    public GameObject[] givverEffect;
    public GameObject awardEffect;
    public GameObject starEffect;
    public Text flashText;
}
[System.Serializable]
public class ResultItems
{
    public GameObject resultParent;
    public Text nameText;
    public Text resultText;
    public Text itemNameText;
    public GameObject dollarEffect;
    public GameObject diemondEffect;
    public GameObject coinEffect;
    public GameObject cutEffect;
    public GameObject winImage;
    public GameObject failImage;
   
}
[System.Serializable]
public class FailItems
{
    public GameObject failParent;
    public Text header;
    public Text ammounts;
}

public class Casino : MonoBehaviour
{
    public GameObject[] casinoObjects;
    public CasinoPlayer[] casinoPlayer;
    [SerializeField] ResultItems resultItems;

    [SerializeField] PlayerNumber[] playerNumberObject;

    [SerializeField] MassegeItems massegeItems;

    [SerializeField] EntryChecker[] entryCheckers;
    public GameObject entryCheckersParent;
    public Button entryCheckerCancelButton;
    Text eCCancelButtonText;    
    public enum PlayerCount
    {
        One,Two,Three,Four
    }
    public PlayerCount playerCount;
    public MyPlayer[] player = new MyPlayer[4];
    [SerializeField] GameObject[] playerUIObject = new GameObject[4];
    private bool allEntryOK;
    public enum CasinoStatus
    {
       Freemode,Rounding,Stopped
    }
    public CasinoStatus casinoStatus;
    [SerializeField] Transform casino;
    [SerializeField] Transform casinoHilder;
    float speed;
    bool playCasino;
    public Button playerCanvasPlayButton;
    [SerializeField] Button casinoButton;
    [SerializeField] GameObject resulObject;
    [SerializeField] TextMeshPro resultext;
    [SerializeField] Text batedNNumber;
    [SerializeField] List <GameObject>UIObjects;
    [SerializeField] MoneyGiver moneyGiver;
    PlayerManager playerManager;    
    //[SerializeField] GameObject menuPanel;
    //[SerializeField] GameObject playerPanel;
    //[SerializeField] GameObject PlayerCountPanel;
    //[SerializeField] GameObject transition;
    int batedNumber;
    // Start is called before the first frame update
    private AudioSource spin_Sound;
    private AudioSource wheel_stopping;
    [SerializeField] GameObject transition;
    //GameObject env;
    public static Casino Instance;
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        playerManager = GetComponent<PlayerManager>();
        casinoPlayer = new CasinoPlayer[4];
        
    }
    void Start()
    {
        resulObject.gameObject.SetActive(false);       
        //casinoButton.gameObject.SetActive(false);        
        CasinoObjects(false);       
        entryCheckersParent.SetActive(false);
        massegeItems.massegeCancelButton.onClick.AddListener(CancellMassege);
        entryCheckerCancelButton.onClick.AddListener(CancellChecker);
        eCCancelButtonText = entryCheckerCancelButton.transform.Find("Text").GetComponent<Text>();
        spin_Sound = transform.Find("wheelspin").GetComponent<AudioSource>();
        wheel_stopping = transform.Find("wheelstopping").GetComponent<AudioSource>();
        //env = transform.Find("Env").gameObject;
        GoHome();
        MusicManager.PlayMusic("casual_music3");
        //playerCanvasPlayButton.interactable = false;
        ////UpdatePlayerMoney();
    }
   void GoHome()
    {
        OffAllUIObjects();
        UIObject("Menu").SetActive(true);
        //env.SetActive(true);
    }
    public GameObject UIObject(string name)
    {
        int objectIndex = UIObjects.FindIndex(gameObject => string.Equals(name, gameObject.name));
        return UIObjects[objectIndex];        
    }
    void CasinoObjects(bool value)
    {
        for (int i = 0; i < casinoObjects.Length; i++)
        {
            casinoObjects[i].SetActive(value);
        }
    }
    void OffAllentryCheckers()
    {
        for (int i = 0; i < entryCheckers.Length; i++)
        {
            entryCheckers[i].ecItem.SetActive(false);
        }
    }
    void CancellChecker()
    {
        entryCheckersParent.SetActive(false);
    }
    void MenuPlay()
    {

    }
    public void Play()
    {
        resulObject.gameObject.SetActive(false);
        //if(Random.value>.5f)
        MusicManager.PlaySfx("button");
        entryCheckersParent.SetActive(true);
        checkersImage.Clear();
        OffAllentryCheckers();
        if (playerCount == PlayerCount.One)
        {
            entries = new bool[1];
            entryCheckers[0].ecItem.SetActive(true);
            CheckEntry(0);           

        }
        if (playerCount == PlayerCount.Two)
        {
            entries = new bool[2];
            entryCheckers[0].ecItem.SetActive(true);
            entryCheckers[1].ecItem.SetActive(true);
            CheckEntry(0);
            CheckEntry(1);
        }
        if (playerCount == PlayerCount.Three)
        {
            entries = new bool[3];
            entryCheckers[0].ecItem.SetActive(true);
            entryCheckers[1].ecItem.SetActive(true);
            entryCheckers[2].ecItem.SetActive(true);
           
            CheckEntry(0);
            CheckEntry(1);
            CheckEntry(2);
        }
        if (playerCount == PlayerCount.Four)
        {
            entries = new bool[4];
            entryCheckers[0].ecItem.SetActive(true);
            entryCheckers[1].ecItem.SetActive(true);
            entryCheckers[2].ecItem.SetActive(true);
            entryCheckers[3].ecItem.SetActive(true);
            CheckEntry(0);
            CheckEntry(1);
            CheckEntry(2);
            CheckEntry(3);
        }
        foreach (var entry in entries)
        {
            if (entry == true)
            {
                entryCheckerCancelButton.onClick.RemoveAllListeners();
                entryCheckerCancelButton.onClick.AddListener(GoForPlay);
                eCCancelButtonText.text = "OK GO";
            }
            else
            {
                eCCancelButtonText.text = "Check Entries";
                entryCheckerCancelButton.onClick.RemoveAllListeners();
                entryCheckerCancelButton.onClick.AddListener(CancellChecker);
            }
        }
        AdmobAdmanager.Instance.ShowInterstitial();
    }
   [HideInInspector] public bool[] entries;
    void CheckEntry(int playerIndex)
    {
        if (player[playerIndex].name.text.ToString() == string.Empty || player[playerIndex].name.text.ToString() == "Name")
        {
            entryCheckers[playerIndex].nameText.text = "Please Enter Player" + playerIndex + "Name";
            entryCheckers[playerIndex].name_checker.color = Color.red;
        }
        else
        {
            entryCheckers[playerIndex].nameText.text = player[playerIndex].name.text.ToString();
            entryCheckers[playerIndex].name_checker.color = Color.green;
        }
        int numbers = int.Parse(player[playerIndex].number.text.ToString());
        if (numbers < 0 || numbers > 38)
        {
            entryCheckers[playerIndex].numberText.text = "The Number Should" + " Between 0 To 38";
            entryCheckers[playerIndex].number_checker.color = Color.red;
        }
        else
        {
            entryCheckers[playerIndex].numberText.text = player[playerIndex].number.text.ToString();
            entryCheckers[playerIndex].number_checker.color = Color.green;
        }
        int ammounts = int.Parse(player[playerIndex].ammount.text.ToString());

        if (ammounts > 0) { 
        //{
                entryCheckers[playerIndex].ammount_checker.color = Color.green;
                //if (ammounts < 100 || ammounts > 500)
                //{
                //    entryCheckers[playerIndex].ammountText.text = "Ammounts Should Between" +
                //       "100 To 500";
                //    entryCheckers[playerIndex].ammount_checker.color = Color.red;
                //}
                //else
                //{
                //    entryCheckers[playerIndex].ammountText.text = player[playerIndex].ammount.text.ToString();
                //    entryCheckers[playerIndex].ammount_checker.color = Color.green;
                //}
            //}
        }
        else
        {
            entryCheckers[playerIndex].ammount_checker.color = Color.red;
        }     
       
        int diemonds = int.Parse(player[playerIndex].betDiemonds.text.ToString());
        if (diemonds <=0)
        {
           
            entryCheckers[playerIndex].diemond_checker.color = Color.red;
        }
        else
        {
            entryCheckers[playerIndex].diemondText.text = player[playerIndex].betDiemonds.text.ToString();
            entryCheckers[playerIndex].diemond_checker.color = Color.green;
        }
        int coins = int.Parse(player[playerIndex].betCoins.text.ToString());
        if (coins <= 0)
        {

            entryCheckers[playerIndex].coin_checker.color = Color.red;
        }
        else
        {
            entryCheckers[playerIndex].coinText.text = player[playerIndex].betCoins.text.ToString();
            entryCheckers[playerIndex].coin_checker.color = Color.green;
        }
            if(entryCheckers[playerIndex].name_checker.color   == Color.green
            && entryCheckers[playerIndex].number_checker.color == Color.green
            &&       EntryOK(playerIndex)
            )
        {
            entries[playerIndex] = true;
        }
        else
        {
            ShowMassege(casinoPlayer[playerIndex].playerName.ToString() + " " +"You Have To Add Credits Or Diemonds Or Coin");
            entries[playerIndex] = false;
        }
    }
    public bool EntryOK(int playerIndex)
    {
        if(
               entryCheckers[playerIndex].ammount_checker.color == Color.green
            || entryCheckers[playerIndex].diemond_checker.color == Color.green
            || entryCheckers[playerIndex].coin_checker.color    == Color.green)
        {
            return true;
        }
        else
        {
            return false;
        }

    }


    void ContineueGoForPlay()
    {
        CasinoObjects(true);
        entryCheckersParent.SetActive(false);
        casinoButton.gameObject.SetActive(true);
        #region UPDATE PLAYER NUMBER
        for (int i = 0; i < playerNumberObject.Length; i++)
        {
            playerNumberObject[i].playerObject.SetActive(false);
        }
        if (playerCount == PlayerCount.One)
        {
            UpdatePlayerNumber(0);
        }
        else
            if (playerCount == PlayerCount.Two)
        {
            UpdatePlayerNumber(0);
            UpdatePlayerNumber(1);
        }
        else
        if (playerCount == PlayerCount.Three)
        {
            UpdatePlayerNumber(0);
            UpdatePlayerNumber(1);
            UpdatePlayerNumber(2);
        }
        else
            if (playerCount == PlayerCount.Four)
        {
            UpdatePlayerNumber(0);
            UpdatePlayerNumber(1);
            UpdatePlayerNumber(2);
            UpdatePlayerNumber(3);
        }
        MusicManager.PlayMusic("casino_music");
        //env.SetActive(false);
    }
    void GoForPlay()
    {
        StartCoroutine(PlayTransition());
        MusicManager.PlaySfx("button");       
        ShowUI("HUD");
        Invoke("ContineueGoForPlay", 1);
        #endregion
        AdmobAdmanager.Instance.ShowInterstitial();
    }
    public void UpdatePlayerNumber(int playerIndex)
    {
        playerNumberObject[playerIndex].playerObject.SetActive(true);
        playerNumberObject[playerIndex].name.text = player[playerIndex].name.text.ToString();
        playerNumberObject[playerIndex].number.text = player[playerIndex].number.text.ToString();
        playerNumberObject[playerIndex].avatar.sprite = player[playerIndex].avatar.sprite;
        playerNumberObject[playerIndex].ammounts.text = player[playerIndex].ammount.text.ToString();
        playerNumberObject[playerIndex].diemonds.text = player[playerIndex].betDiemonds.text.ToString();
        playerNumberObject[playerIndex].coins.text = player[playerIndex].betCoins.text.ToString();
    }
    public List<Image> checkersImage = new List<Image>();
    public void ShowMassege(string massege)
    {

        massegeItems.massegeCanvas.SetActive(true);
        massegeItems.massegeText.text = string.Empty;
        massegeItems.massegeText.text = massege;
    }
    void CancellMassege()
    {
        MusicManager.PlaySfx("button");
        massegeItems.massegeText.text = string.Empty;
        massegeItems.massegeCanvas.SetActive(false);
        //AdmobAdmanager.Instance.ShowInterstitial();
    }
   
    void OffAllUIObjects()
    {
        for (int i = 0; i < UIObjects.Count; i++)
        {
            UIObjects[i].SetActive(false);
        }
    }
    public void ShowUI(string uiName)
    {

        StartCoroutine(ContineuShowUI(uiName));
    }

    IEnumerator PlayTransition()
    {
        //SoundManager.PlaySfx("transition");
        transition.SetActive(true);
        yield return new WaitForSeconds(2f);
        transition.SetActive(false);
    }
    IEnumerator ContineuShowUI(string uiName)
    {
        MusicManager.PlaySfx("button");       
        yield return new WaitForSeconds(1f);
        OffAllUIObjects();
        UIObject(uiName).SetActive(true);
    }

    public bool[] hasplayerCount;
    public void ActivePlayerCount(int count)
    {
        StartCoroutine(PlayTransition());
        ShowUI("Players_Canvas");
        if (count == 1)
        {
            playerCount = PlayerCount.One;
            playerUIObject[0].SetActive(true);
            playerUIObject[1].SetActive(false);
            playerUIObject[2].SetActive(false);
            playerUIObject[3].SetActive(false);
            hasplayerCount = new bool[1];
        }
        else
             if (count == 2)
        {
            playerCount = PlayerCount.Two;
            playerUIObject[0].SetActive(true);
            playerUIObject[1].SetActive(true);
            playerUIObject[2].SetActive(false);
            playerUIObject[3].SetActive(false);
            hasplayerCount = new bool[2];
        }
        else
             if (count == 3)
        {
            playerCount = PlayerCount.Three;
            playerUIObject[0].SetActive(true);
            playerUIObject[1].SetActive(true);
            playerUIObject[2].SetActive(true);
            playerUIObject[3].SetActive(false);
            hasplayerCount = new bool[3];
        }
        else
             if (count == 4)
        {
            playerCount = PlayerCount.Four;
            playerUIObject[0].SetActive(true);
            playerUIObject[1].SetActive(true);
            playerUIObject[2].SetActive(true);
            playerUIObject[3].SetActive(true);
            hasplayerCount = new bool[4];
        }

        //env.SetActive(false);
        AdmobAdmanager.Instance.ShowInterstitial();
    }
    public void RoundCasino()
    {
        speed = Random.Range(15, 40);
        playCasino = true;
        casinoButton.gameObject.SetActive(false);
        casinoStatus = CasinoStatus.Rounding;
        resultext.gameObject.SetActive(false);
        resultShown = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (playCasino)
        {
            casino.Rotate(0, 0, speed);
            casinoHilder.Rotate(0, -speed, 0);

                      
            if (speed > 0)
            {
                speed -= Time.deltaTime;
                if (speed >= 7)
                {
                    if (!spin_Sound.isPlaying) { spin_Sound.Play(); }
                }
                else
                {
                    if (spin_Sound.isPlaying) { spin_Sound.Stop(); }
                    if (!wheel_stopping.isPlaying) { wheel_stopping.Play(); }
                }
            }
            else
            {
                playCasino = false;
                //numberPanel.SetActive(true);
                casinoStatus = CasinoStatus.Stopped;
                OnGameComplete();               
                //numberPanel.SetActive(true);
                //resulObject.gameObject.SetActive(true);
                if (wheel_stopping.isPlaying) { wheel_stopping.Stop(); }
                //casinoButton.gameObject.SetActive(true);
                MusicManager.PauseMusic(0.2f);
            }

        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RoundCasino();
        }
        
    }
    private float givingAmmount;

    private void LateUpdate()
    {
        if(giveMoney && givingAmmount < 2000)
        {
            givingAmmount += Time.deltaTime*100;
            moneyGiver.ammount.text = givingAmmount.ToString();
            if (givingAmmount>=2000 && giveMoney)
            {
                giveMoney = false;
                StartCoroutine(StopMoneyGiving());
            }
        }      
    }
    IEnumerator StopMoneyGiving()
    {
        MusicManager.PauseMusic(0.5f);
        giveMoney = false;
        for (int i = 0; i < moneyGiver. givverEffect.Length; i++)
        {
            moneyGiver.givverEffect[i].SetActive(false);
        }       
        moneyGiver.flashText.gameObject.SetActive(true);
        moneyGiver.starEffect.gameObject.SetActive(true);
        moneyGiver.flashText.text = "2000";
        moneyGiver.ammount.text = "Credits";
        yield return new WaitForSeconds(2);
        moneyGiver.flashText.gameObject.SetActive(false);
        moneyGiver.ammount.gameObject.SetActive(false);
        moneyGiver.starEffect.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        moneyGiver.flashText.gameObject.SetActive(true);
        moneyGiver.ammount.gameObject.SetActive(true);
        moneyGiver.starEffect.gameObject.SetActive(true);
        moneyGiver.flashText.text = "50";
        moneyGiver.ammount.text = "Coins";
        yield return new WaitForSeconds(2);
        moneyGiver.flashText.gameObject.SetActive(false);
        moneyGiver.ammount.gameObject.SetActive(false);
        moneyGiver.starEffect.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        moneyGiver.flashText.gameObject.SetActive(true);
        moneyGiver.ammount.gameObject.SetActive(true);
        moneyGiver.starEffect.gameObject.SetActive(true);
        moneyGiver.flashText.text = "10";
        moneyGiver.ammount.text = "Diemonds";
        yield return new WaitForSeconds(2);
        moneyGiver.flashText.gameObject.SetActive(false);
        moneyGiver.ammount.gameObject.SetActive(false);
        moneyGiver.starEffect.gameObject.SetActive(false);
        yield return new WaitForSeconds(2);
        moneyGiver.mpneyGiver.SetActive(false);
        if (playerMoneyGiven == 0)  {
            GiveBonusToNewPlayer(0);           
        }
        if (playerMoneyGiven == 1) {
            GiveBonusToNewPlayer(1);
        }
        if (playerMoneyGiven == 2) {
            GiveBonusToNewPlayer(2);
        }
        if (playerMoneyGiven == 3) {
            GiveBonusToNewPlayer(3);
        }
       AdmobAdmanager.Instance.ShowInterstitial();
        //UpdatePlayerMoney();
    }
    bool giveMoney;
    int playerMoneyGiven;
    public void GiveMoney(int playerIndexs)
    {
        playerMoneyGiven = playerIndexs;
        moneyGiver.mpneyGiver.               SetActive(true);
        for (int i = 0; i < moneyGiver.givverEffect.Length; i++)
        {
            moneyGiver.givverEffect[i].SetActive(true);
        }
        moneyGiver.ammount.gameObject.       SetActive(true);
        moneyGiver.name.text =               player[playerIndexs].name.text.ToString();
        givingAmmount = 0;
        moneyGiver.ammount.text =            givingAmmount.ToString();
        StartCoroutine(ProccessGiveMoney());
    }
    [HideInInspector]public List<ParticleSystem> flashes;
    void GiveBonusToNewPlayer(int playerIndex)
    {
        casinoPlayer[playerIndex]= playerManager.curentPlayer;
        casinoPlayer[playerIndex].balance += 2000;
        casinoPlayer[playerIndex].diemond += 10;
        casinoPlayer[playerIndex].coin += 50;
        playerManager.curentPlayer = casinoPlayer[playerIndex];
        playerManager.SaveCurrentPlayer(casinoPlayer[playerIndex].playerName);
        player[playerIndex].name.text = playerManager.curentPlayer.playerName.ToString();
        player[playerIndex].balance.text = playerManager.curentPlayer.balance.ToString();
        player[playerIndex].diemonds.text = playerManager.curentPlayer.diemond.ToString();
        player[playerIndex].coins.text = playerManager.curentPlayer.coin.ToString();

        flashes.Add(player[playerIndex].balance.transform.Find("flash").GetComponent<ParticleSystem>());
        flashes.Add(player[playerIndex].diemonds.transform.Find("flash").GetComponent<ParticleSystem>());
        flashes.Add(player[playerIndex].coins.transform.Find("flash").GetComponent<ParticleSystem>());
        for (int i = 0; i < flashes.Count; i++)
        {
            flashes[i].Play();
        }
        flashes.Clear();
        player[playerIndex].locks.SetActive(false);
        AdmobAdmanager.Instance.ShowInterstitial();
    } 
    public void UpdatePlayerStatus(CasinoPlayer casinoPlayer, int playerIndex )
    {
        player[playerIndex].name.text = casinoPlayer.playerName.ToString();
        player[playerIndex].balance.text = playerManager.curentPlayer.balance.ToString();
        player[playerIndex].diemonds.text = playerManager.curentPlayer.diemond.ToString();
        player[playerIndex].coins.text = playerManager.curentPlayer.coin.ToString();
        player[playerIndex].avatar.sprite = playerManager.avatars[playerManager.curentPlayer.avatarIndex].sprite;
        AdmobAdmanager.Instance.ShowInterstitial();
    }
    bool resultShown;
    public void ShowResult(int result)
    {
        if (!resultShown)
        {
            resultShown = true;
            resulObject.gameObject.SetActive(true);
            resultext.text = result.ToString();
        }
    } 

    IEnumerator ProccessGiveMoney()
    {
        yield return new WaitForSeconds(1f);
        //SoundManager.PlayMusic("jackpot");
        yield return new WaitForSeconds(1f);
        giveMoney = true;
    }
    void OnGameComplete()
    {
        AdmobAdmanager.Instance.ShowInterstitial();
        StartCoroutine(GameComplete());
    }
    IEnumerator GameComplete()
    {
        yield return new WaitForSeconds(5f);
        resulObject.gameObject.SetActive(false);
        ShowUI("Players_Canvas");       
        yield return new WaitForSeconds(1f);
        CasinoObjects(false);
        yield return new WaitForSeconds(0.4f);
        if (playerCount == PlayerCount.One)
        {
            StartCoroutine(ChckStatus(0));
        }
        else
          if (playerCount == PlayerCount.Two)
        {
            StartCoroutine(ChckStatus(0));
            yield return new WaitUntil(() => showAvailable);           
            StartCoroutine(ChckStatus(1));
        }
        else
      if (playerCount == PlayerCount.Three)
        {
            StartCoroutine(ChckStatus(0));
            yield return new WaitUntil(() => showAvailable);
            StartCoroutine(ChckStatus(1));
            yield return new WaitUntil(() => showAvailable);
            StartCoroutine(ChckStatus(2));
        }
        else
          if (playerCount == PlayerCount.Four)
        {
            StartCoroutine(ChckStatus(0));
            yield return new WaitUntil(() => showAvailable);
            StartCoroutine(ChckStatus(1));
            yield return new WaitUntil(() => showAvailable);
            StartCoroutine(ChckStatus(2));
            yield return new WaitUntil(() => showAvailable);
            StartCoroutine( ChckStatus(3));
        }
        
    }
    bool showAvailable;
    IEnumerator ChckStatus(int playerIndex)
    {
        showAvailable = false;
        int numbers = int.Parse(player[playerIndex].number.text.ToString());
        resultItems.resultParent.SetActive(true);
        if (numbers == int.Parse(resultext.text.ToString()))
        {
            player[playerIndex].status.color = Color.green;
           
            resultItems.winImage.SetActive(true);
            resultItems.failImage.SetActive(false);
            resultItems.nameText.text = player[playerIndex].name.text.ToString();

            int awardMoney = int.Parse(player[playerIndex].ammount.text.ToString());
            if (awardMoney > 0)
            {
                resultItems.itemNameText.text = "$";
                resultItems.dollarEffect.SetActive(true);
                resultItems.resultText.text = awardMoney.ToString();
                casinoPlayer[playerIndex].balance += awardMoney;
                //player[playerIndex].balance.text = casinoPlayer[playerIndex].balance.ToString();
                yield return new WaitForSeconds(2);
                resultItems.dollarEffect.SetActive(false);
            }
            int awardableDiemonds = int.Parse(player[playerIndex].betDiemonds.text.ToString());
            if (awardableDiemonds > 0)
            {
                resultItems.diemondEffect.SetActive(true);
                resultItems.itemNameText.text = "Diemonds";
                resultItems.resultText.text = awardableDiemonds.ToString();
                casinoPlayer[playerIndex].diemond += awardableDiemonds;
                //player[playerIndex].diemonds.text = casinoPlayer[playerIndex].diemond.ToString();
                yield return new WaitForSeconds(2);
                resultItems.diemondEffect.SetActive(false);
            }
            int awardableCoins = int.Parse(player[playerIndex].betCoins.text.ToString());
            if (awardableCoins > 0)
            {
                resultItems.coinEffect.SetActive(true);
                resultItems.itemNameText.text = "Coins";
                resultItems.resultText.text = awardableCoins.ToString();
                casinoPlayer[playerIndex].coin += awardableCoins;
                //player[playerIndex].coins.text = casinoPlayer[playerIndex].coin.ToString();
                yield return new WaitForSeconds(2);
                resultItems.coinEffect.SetActive(false);
            }
           
        }
        else
        {
            resultItems.resultParent.SetActive(true);
            resultItems.failImage.SetActive(true);
            resultItems.winImage.SetActive(false);

            player[playerIndex].status.color = Color.red;
            resultItems.nameText.text = player[playerIndex].name.text.ToString();
            int awardMoney = int.Parse(player[playerIndex].ammount.text.ToString());
            if (awardMoney > 0)
            {
                resultItems.cutEffect.gameObject.SetActive(true);
                resultItems.resultText.gameObject.SetActive(true);
                resultItems.itemNameText.text = "$";
                casinoPlayer[playerIndex].balance -= awardMoney;
                resultItems.resultText.text = awardMoney.ToString();
               
               
                yield return new WaitForSeconds(2);
                resultItems.resultText.gameObject.SetActive(false);
                resultItems.cutEffect.gameObject.SetActive(false);
               
            }
            int awardableDiemonds = int.Parse(player[playerIndex].betDiemonds.text.ToString());
            if (awardableDiemonds > 0)
            {
                resultItems.cutEffect.gameObject.SetActive(true);
                resultItems.resultText.gameObject.SetActive(true);
                resultItems.itemNameText.text = "Diemonds";
                casinoPlayer[playerIndex].diemond -= awardableDiemonds;
                resultItems.resultText.text = awardableDiemonds.ToString();
               
               
                yield return new WaitForSeconds(2);
                resultItems.resultText.gameObject.SetActive(false);
                resultItems.cutEffect.gameObject.SetActive(false);
                
            }
            int awardableCoins = int.Parse(player[playerIndex].betCoins.text.ToString());
            if (awardableCoins > 0)
            {
                resultItems.cutEffect.gameObject.SetActive(true);
                resultItems.resultText.gameObject.SetActive(true);
                resultItems.itemNameText.text = "Coins";
                resultItems.resultText.text = awardableCoins.ToString();
                casinoPlayer[playerIndex].coin -= awardableCoins;
               
                yield return new WaitForSeconds(2);
                resultItems.resultText.gameObject.SetActive(false);
                resultItems.cutEffect.gameObject.SetActive(false);
               
            }
           

        }
        playerManager.curentPlayer=casinoPlayer[playerIndex];
        playerManager.SaveCurrentPlayer(casinoPlayer[playerIndex].playerName);

        yield return new WaitForSeconds(2);
        resultItems.resultParent.SetActive(false);
        player[playerIndex].diemonds.text = casinoPlayer[playerIndex].diemond.ToString();
        player[playerIndex].coins.text = casinoPlayer[playerIndex].coin.ToString();
        player[playerIndex].balance.text = casinoPlayer[playerIndex].balance.ToString();
        StartCoroutine(ClearStatus(playerIndex));
        AdmobAdmanager.Instance.ShowInterstitial();
        showAvailable = true;
    }       
    IEnumerator ClearStatus(int playerIndex)
    {
        yield return new WaitForSeconds(2);
        player[playerIndex].betCoins.text = "0";
        player[playerIndex].betDiemonds.text = "0";
        player[playerIndex].ammount.text = "0";
        player[playerIndex].number.text = "0";
    }  
    public void GiveReward()
    {
        Game.TotalCoins += 100;
        MessegeManager.Instance.ShowMassege
                ("Congratulation"
                , "You Won 100 Credits", true);
    }
    public void ShowVideo()
    {
        Application.OpenURL("https://youtu.be/5SBRZ68k4SQ");
    }
}
