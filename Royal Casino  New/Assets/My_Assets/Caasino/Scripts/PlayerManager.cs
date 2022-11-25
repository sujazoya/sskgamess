using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using suja;
public class PlayerManager : MonoBehaviour
{
   
    Casino casino;
    public Button[] iamOld;
    public Button[] buy;
    public GameObject PlayerProfile;

    Button name1, name2, name3, name4;
    public GameObject NameUpdater;
    public Text Note_Text;
    public Text balance_Text;
    public InputField inputField;
    public Button inputFieldButton;
    Text myName;
    Button number1, number2, number3, number4;
    Button ammount1, ammount2, ammount3, ammount4;
    Button[] dimondsButton;
    Button[] coinsButton;
    public Image[] avatars;   
    public GameObject avatarPanel;
    Button p1Avatar, p2Avatar, p3Avatar, p4Avatar;
    [HideInInspector] public Image myAvatar;  

    public static PlayerManager instance;
    public GameObject loadArea;
    public GameObject loadButtonPrefab;
    public GameObject avaialbePlayer_Panel;

    public enum PlayerIndex
    {
        first,second,third,forth
    }
    [SerializeField] PlayerIndex playerIndex;
    public int CurrentPlayerIndex()
    {
        if(playerIndex==PlayerIndex.first)
        {
            return 0;
        }else
            if (playerIndex == PlayerIndex.second)
        {
            return 1;
        }
        else
            if (playerIndex == PlayerIndex.third)
        {
            return 2;
        }
        else
            if (playerIndex == PlayerIndex.forth)
        {
            return 3;
        }
        else
        {
            return 0;
        }


    }

    public SaveData saveData;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        casino = GetComponent<Casino>();
    }

    public enum IAm
    {
        First,Scond,Third,Fourth
    }
    public IAm iAm;
    // Start is called before the first frame update
    void Start()
    {
        //availablePlayer.onClick.AddListener(ShowLoadScreen);
       
        UpdateAvatars();
        UpdateNames();
        UpdateNumbers();
        UpdateAmmounts();
        UpdateDiemonds();
        UpdateCoinItem();
        avaialbePlayer_Panel.SetActive(false);
        PlayerProfile.SetActive(false);
        if (balance_Text) { balance_Text.text = Game.TotalCoins.ToString(); }
    }
    private void OnEnable()
    {
        GetLoadFiles();
        UpdatePlayer();
       
    }
    public void GiveBalanceToCurrentPlayer()
    {
        if (Game.TotalCoins > 0 &&  ! string.IsNullOrWhiteSpace(casino.casinoPlayer[0].playerName))
        {           
            casino.casinoPlayer[0].balance += Game.TotalCoins;
            MessegeManager.Instance.ShowPopup("Player 1 Got" + Game.TotalCoins);
            SaveCurrentPlayer(casino.casinoPlayer[0].playerName);
            casino.player[0].balance.text = casino.casinoPlayer[0].balance.ToString();
            Game.TotalCoins = 0;
            if (balance_Text) { balance_Text.text = Game.TotalCoins.ToString(); }
        }
        else
        {
            MessegeManager.Instance.ShowPopup("No Player Activated");
        }
    }
    private void LateUpdate()
    {
        if (balance_Text) { balance_Text.text = Game.TotalCoins.ToString(); }
    }
    public void CloseNameUpdater()
    {
        NameUpdater.SetActive(false);
    }
    #region AVATARS ACTIVATOR
    void UpdateAvatars()
    {
        casino = FindObjectOfType<Casino>();
        p1Avatar = casino.player[0].avatar.transform.GetComponent<Button>();
        p1Avatar.onClick.AddListener(ActFirstAvatar);

        p2Avatar = casino.player[1].avatar.transform.GetComponent<Button>();
        p2Avatar.onClick.AddListener(ActSecondAvatar);

        p3Avatar = casino.player[2].avatar.transform.GetComponent<Button>();
        p3Avatar.onClick.AddListener(ActThirdAvatar);

        p4Avatar = casino.player[3].avatar.transform.GetComponent<Button>();
        p4Avatar.onClick.AddListener(ActFourthAvatar);

        avatarPanel.SetActive(false);
    }
    public void CloseAvailablePlayer()
    {
        avaialbePlayer_Panel.SetActive(false);
    }
    void ActFirstAvatar()
    {
        avatarPanel.SetActive(true);        
        iAm = IAm.First;
    }
    void ActSecondAvatar()
    {
        avatarPanel.SetActive(true);
        iAm = IAm.Scond;
    }
    void ActThirdAvatar()
    {
        avatarPanel.SetActive(true);
        iAm = IAm.Third;
    }
    void ActFourthAvatar()
    {
        avatarPanel.SetActive(true);
        iAm = IAm.Fourth;
    }
    public void ActivateAvatar(Sprite avatar,int avaIndex)
    {
        if (iAm == IAm.First)
        {
            myAvatar = casino.player[0].avatar;
            myAvatar.sprite = avatar;
            avatarPanel.SetActive(false);
            TryToSaveAvatar(0, avaIndex);
        }
        else
        if (iAm == IAm.Scond)
        {
            myAvatar = casino.player[1].avatar;
            myAvatar.sprite = avatar;
            avatarPanel.SetActive(false);
            TryToSaveAvatar(1, avaIndex);
        }
        else if(iAm == IAm.Third)
        {
            myAvatar = casino.player[2].avatar;
            myAvatar.sprite = avatar;
            avatarPanel.SetActive(false);
            TryToSaveAvatar(2, avaIndex);
        }
        else if(iAm == IAm.Fourth)
        {
            myAvatar = casino.player[3].avatar;
            myAvatar.sprite = avatar;
            avatarPanel.SetActive(false);
            TryToSaveAvatar(3, avaIndex);
        }        
    }
    void TryToSaveAvatar(int playerIndex,int avaIndex)
    {
       casino.casinoPlayer[playerIndex].avatarIndex = avaIndex;
        SaveCurrentPlayer(casino.casinoPlayer[playerIndex].playerName);
    }
    #endregion

    void NameUpdaterSet(bool value,string note)
    {
        NameUpdater.SetActive(value);
        if (Note_Text) { Note_Text.text = note; }
        if (value == true)
        {
            inputField.text = string.Empty;
        }
    }
    #region NAME UPDATER
    void UpdateNames()
    {
        name1 = casino.player[0].name.transform.GetComponent<Button>();
        name1.onClick.AddListener(ActFirstName);

        name2 = casino.player[1].name.transform.GetComponent<Button>();
        name2.onClick.AddListener(ActSecondName);

        name3 = casino.player[2].name.transform.GetComponent<Button>();
        name3.onClick.AddListener(ActThirdName);

        name4 = casino.player[3].name.transform.GetComponent<Button>();
        name4.onClick.AddListener(ActFourthName);
        NameUpdater.SetActive(false);
    }


    //private int playerIndex;
    void ActFirstName()
    {
        //playerIndex = 0;
        playerIndex = PlayerIndex.first;
        NameUpdaterSet(true,"Enter Your Name Below");
        inputFieldButton.onClick.RemoveAllListeners();
        inputFieldButton.onClick.AddListener(ActivateNames);
        iAm = IAm.First;
        SoundManager.PlaySfx("button");
    }
    void ActSecondName()
    {
        //playerIndex = 1;
        playerIndex = PlayerIndex.second;
        NameUpdaterSet(true, "Enter Your Name Below");
        inputFieldButton.onClick.RemoveAllListeners();
        inputFieldButton.onClick.AddListener(ActivateNames);
        iAm = IAm.Scond;
        SoundManager.PlaySfx("button");
    }
    void ActThirdName()
    {
        //playerIndex = 2;
        playerIndex = PlayerIndex.third;
        NameUpdaterSet(true, "Enter Your Name Below");
        inputFieldButton.onClick.RemoveAllListeners();
        inputFieldButton.onClick.AddListener(ActivateNames);
        iAm = IAm.Third;
        SoundManager.PlaySfx("button");
    }
    void ActFourthName()
    {
        //playerIndex = 3;
        playerIndex = PlayerIndex.forth;
        NameUpdaterSet(true, "Enter Your Name Below");
        inputFieldButton.onClick.RemoveAllListeners();
        inputFieldButton.onClick.AddListener(ActivateNames);
        iAm = IAm.Fourth;
        SoundManager.PlaySfx("button");
    }

    public void UpdatePlayer()
    {
        iamOld[0].onClick.AddListener(IamOld1);
        iamOld[1].onClick.AddListener(IamOld2);
        iamOld[2].onClick.AddListener(IamOld3);
        iamOld[3].onClick.AddListener(IamOld4);
        buy[0].onClick.AddListener(() =>  { int pIndex = 0;  ActBuy(pIndex); });
        buy[1].onClick.AddListener(() => { int pIndex = 1; ActBuy(pIndex); });
        buy[2].onClick.AddListener(() => { int pIndex = 2; ActBuy(pIndex); });
        buy[3].onClick.AddListener(() => { int pIndex = 3; ActBuy(pIndex); });
    }
    public void ClosePlayerProfile()
    {
        PlayerProfile.SetActive(false);
    }
    void ActBuy(int playerIndex)
    {
        curentPlayer = casino.casinoPlayer[playerIndex];
        
        PlayerProfile.SetActive(true);
        PlayerProfile.GetComponent<PlayerProfile>().playerIndex = playerIndex;
    }
    void IamOld1()
    {
        playerIndex = PlayerIndex.first;
        ShowLoadScreen();
        SoundManager.PlaySfx("button");
    }
    void IamOld2()
    {
        playerIndex = PlayerIndex.second;
        ShowLoadScreen();
        SoundManager.PlaySfx("button");
    }
    void IamOld3()
    {
        playerIndex = PlayerIndex.third;
        ShowLoadScreen();
        SoundManager.PlaySfx("button");
    }
    void IamOld4()
    {
        playerIndex = PlayerIndex.forth;
        ShowLoadScreen();
        SoundManager.PlaySfx("button");
    }
    public string[] saveFiles;
     
    public void GetLoadFiles()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/saves/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves/");
        }        
        saveFiles = Directory.GetFiles(Application.persistentDataPath + "/saves/");      

    }
    private CasinoPlayer newPlayer;
    public CasinoPlayer curentPlayer;
    void ActivateNames()
    {
        ActivateNames(inputField.text.ToString());
        newPlayer = new CasinoPlayer(inputField.text.ToString(),0,0,0,0);
        newPlayer.playerName = inputField.text.ToString();
        if (!PlayerAvailable(newPlayer.playerName))
        {
            //availablePlayers.Add(newPlayer);
            curentPlayer = newPlayer;
            SaveCurrentPlayer(newPlayer.playerName);
            casino.casinoPlayer[CurrentPlayerIndex()] = newPlayer;           
            StartCoroutine(GiveBonusAmmount());
            SaveName(newPlayer.playerName);
            casino.hasplayerCount[CurrentPlayerIndex()] = true;
        }       
        else
        {
            iamOld[CurrentPlayerIndex()].gameObject.SetActive(false);
            buy[CurrentPlayerIndex()].gameObject.SetActive(true);
            casino.player[CurrentPlayerIndex()].locks.SetActive(false);
            casino.hasplayerCount[CurrentPlayerIndex()] = true;
        }
        CheckIfAllPlayersAssigned();

    }
    public void SaveName(string name)
    {
        SerializationManger.Save(name, SaveData.Instance);
    }

    //public List<CasinoPlayer> availablePlayers;

    public void SaveCurrentPlayer(string playerName)
    {

        // Getting the objects from the randomplacer
        //savableObjects = randomPlacer.savableObjects;
        //availablePlayers.Add(player);
        // Turns the data from the objects class into binary data
        FileStream fs = File.Create(Application.persistentDataPath + "/" + playerName + ".dat");
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, curentPlayer);
        fs.Close(); // You MUST close the filestream otherwise it will cause errors!!
    }
    public bool PlayerAvailable(string playerName)
    {
        //randomPlacer = GetComponent<RandomPlacer>();
        string path = Application.persistentDataPath + "/" + playerName + ".dat";
        // Checking if the file exists
        if (File.Exists(path))
        {
            FileStream fs = File.Open(path, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            // Making sure the file is not empty
            if (fs.Length > 0)
            {
                // Turns the data back from the binary into strings and floats
                //availablePlayers = (List<CasinoPlayer>)bf.Deserialize(fs);
                curentPlayer = (CasinoPlayer)bf.Deserialize(fs);
                casino.casinoPlayer[CurrentPlayerIndex()] = curentPlayer;
                casino.casinoPlayer[CurrentPlayerIndex()].playerName = curentPlayer.playerName;
                casino.casinoPlayer[CurrentPlayerIndex()].balance = curentPlayer.balance;
                casino.casinoPlayer[CurrentPlayerIndex()].diemond = curentPlayer.diemond;
                casino.casinoPlayer[CurrentPlayerIndex()].coin = curentPlayer.coin;
                casino.casinoPlayer[CurrentPlayerIndex()].avatarIndex = curentPlayer.avatarIndex;
                casino.UpdatePlayerStatus(curentPlayer, CurrentPlayerIndex());
                //randomPlacer.savableObjects = savableObjects;
                //Reinstantiate();


                fs.Close(); // You MUST close the filestream otherwise it will cause errors!!
                return true;
            }
        }
        // This will happen if the file is non existant or empty
        return false;
    }

    public void ShowLoadScreen()
    {
        GetLoadFiles();
        avaialbePlayer_Panel.SetActive(true);
        if (loadArea.transform.childCount > 0)
        {
            foreach (Transform child in loadArea.transform)
            {
                Destroy(child.gameObject);
            }

        }
        for (int i = 0; i < saveFiles.Length; i++)
        {
            GameObject buttonObject = Instantiate(loadButtonPrefab);
            buttonObject.transform.SetParent(loadArea.transform, false);

            var index = i;
            string name = saveFiles[index].Replace(Application.persistentDataPath + "/saves/", "");
            buttonObject.GetComponentInChildren<Text>().text = saveFiles[index].Replace(Application.persistentDataPath + "/saves/", "");
            string buttonText = buttonObject.GetComponentInChildren<Text>().text.ToString();
            buttonObject.GetComponent<Button>().onClick.AddListener(() =>
            {              
                GetOldPlayer(buttonText);//OnLoade(saveFiles[index]);
            });        
           
        }
            SoundManager.PlaySfx("button");

    }
    bool getOldPlayer;
    void GetOldPlayer( string name)
    {
        getOldPlayer = PlayerAvailable(name);
        avaialbePlayer_Panel.SetActive(false);
        iamOld[CurrentPlayerIndex()].gameObject.SetActive(false);
        buy[CurrentPlayerIndex()].gameObject.SetActive(true);
        casino.player[CurrentPlayerIndex()].locks.SetActive(false);
        casino.hasplayerCount[CurrentPlayerIndex()] = true;
        SoundManager.PlaySfx("button");
        CheckIfAllPlayersAssigned();        
    }
    void CheckIfAllPlayersAssigned()
    {       
        foreach (var item in casino.hasplayerCount)
        {
           if (item == true)
            {
                casino.playerCanvasPlayButton.interactable = true;
            }
            else
            {
                casino.playerCanvasPlayButton.interactable = false;
            }
        }
        //AdmobAdmanager.Instance.ShowInterstitial();
    }
    IEnumerator GiveBonusAmmount()
    {
        yield return new WaitForSeconds(1);       
        casino.GiveMoney(CurrentPlayerIndex());
    }
    public void ActivateNames(string name)
    {
        if (iAm == IAm.First)
        {
            myName = casino.player[0].name;
            myName.text = name.ToString();
            NameUpdater.SetActive(false);
            iamOld[0].gameObject.SetActive(false);
            buy[0].gameObject.SetActive(true);
        }
        else
        if (iAm == IAm.Scond)
        {
            myName = casino.player[1].name;
            myName.text = name.ToString();
            NameUpdater.SetActive(false);
            iamOld[1].gameObject.SetActive(false);
            buy[1].gameObject.SetActive(true);
        }
        else if (iAm == IAm.Third)
        {
            myName = casino.player[2].name;
            myName.text = name.ToString();
            NameUpdater.SetActive(false);
            iamOld[2].gameObject.SetActive(false);
            buy[2].gameObject.SetActive(true);
        }
        else if (iAm == IAm.Fourth)
        {
            myName = casino.player[3].name;
            myName.text = name.ToString();
            NameUpdater.SetActive(false);
            iamOld[3].gameObject.SetActive(false);
            buy[3].gameObject.SetActive(true);
        }

    }
    #endregion
    #region Number UPDATER
    void UpdateNumbers()
    {
        number1 = casino.player[0].number.transform.GetComponent<Button>();
        number1.onClick.AddListener(ActFirstNumber);

        number2 = casino.player[1].number.transform.GetComponent<Button>();
        number2.onClick.AddListener(ActSecondNumber);

        number3 = casino.player[2].number.transform.GetComponent<Button>();
        number3.onClick.AddListener(ActThirdNumber);

        number4 = casino.player[3].number.transform.GetComponent<Button>();
        number4.onClick.AddListener(ActFourthNumber);
        NameUpdater.SetActive(false);
    }
    void ActFirstNumber()
    {
        NameUpdaterSet(true, "Number Should between 0 TO 36");
        inputFieldButton.onClick.RemoveAllListeners();
        inputFieldButton.onClick.AddListener(UpdateNumber);
        iAm = IAm.First;
        SoundManager.PlaySfx("button");
    }
    void ActSecondNumber()
    {
        NameUpdaterSet(true, "Number Should between 0 TO 36");
        inputFieldButton.onClick.RemoveAllListeners();
        inputFieldButton.onClick.AddListener(UpdateNumber);
        iAm = IAm.Scond;
        SoundManager.PlaySfx("button");
    }
    void ActThirdNumber()
    {
        NameUpdaterSet(true, "Number Should between 0 TO 36");
        inputFieldButton.onClick.RemoveAllListeners();
        inputFieldButton.onClick.AddListener(UpdateNumber);
        iAm = IAm.Third;
        SoundManager.PlaySfx("button");
    }
    void ActFourthNumber()
    {
        NameUpdaterSet(true, "Number Should between 0 TO 36");
        inputFieldButton.onClick.RemoveAllListeners();
        inputFieldButton.onClick.AddListener(UpdateNumber);
        iAm = IAm.Fourth;
        SoundManager.PlaySfx("button");
    }
    void UpdateNumber()
    {
        UpdateNumber(inputField.text.ToString());
        SoundManager.PlaySfx("button");
    }
    public void UpdateNumber(string name)
    {
        int num = int.Parse(name);
        if (num < 0 || num > 38)
        {
            casino.ShowMassege("Number Should Bitween" +
                " 0 To 38");
        }
        else
        {
            if (iAm == IAm.First)
            {
                myName = casino.player[0].number;
                myName.text = name.ToString();
                NameUpdater.SetActive(false);
            }
            else
            if (iAm == IAm.Scond)
            {
                myName = casino.player[1].number;
                myName.text = name.ToString();
                NameUpdater.SetActive(false);
            }
            else if (iAm == IAm.Third)
            {
                myName = casino.player[2].number;
                myName.text = name.ToString();
                NameUpdater.SetActive(false);
            }
            else if (iAm == IAm.Fourth)
            {
                myName = casino.player[3].number;
                myName.text = name.ToString();
                NameUpdater.SetActive(false);
            }
        }
    }
    #endregion

    #region AmmountUpdater

    void UpdateAmmounts()
    {
        ammount1 = casino.player[0].ammount.transform.GetComponent<Button>();
        ammount1.onClick.AddListener(ActFirstAmmount);

        ammount2 = casino.player[1].ammount.transform.GetComponent<Button>();
        ammount2.onClick.AddListener(ActSecondAmmount);

        ammount3 = casino.player[2].ammount.transform.GetComponent<Button>();
        ammount3.onClick.AddListener(ActThirdAmmount);

        ammount4 = casino.player[3].ammount.transform.GetComponent<Button>();
        ammount4.onClick.AddListener(ActFourthAmmount);
        NameUpdater.SetActive(false);
    }
    void ActFirstAmmount()
    {
        NameUpdaterSet(true, "Amounnt Should between 10 TO 500");
        inputFieldButton.onClick.RemoveAllListeners();
        inputFieldButton.onClick.AddListener(UpdateAmmount);
        iAm = IAm.First;
        SoundManager.PlaySfx("button");
    }

    void ActSecondAmmount()
    {
        NameUpdaterSet(true, "Amounnt Should between 10 TO 500");
        inputFieldButton.onClick.RemoveAllListeners();
        inputFieldButton.onClick.AddListener(UpdateAmmount);
        iAm = IAm.Scond;
        SoundManager.PlaySfx("button");
    }
    void ActThirdAmmount()
    {
        NameUpdaterSet(true, "Amounnt Should between 10 TO 500");
        inputFieldButton.onClick.RemoveAllListeners();
        inputFieldButton.onClick.AddListener(UpdateAmmount);
        iAm = IAm.Third;
        SoundManager.PlaySfx("button");
    }
    void ActFourthAmmount()
    {
        NameUpdaterSet(true, "Amounnt Should between 10 TO 500");
        inputFieldButton.onClick.RemoveAllListeners();
        inputFieldButton.onClick.AddListener(UpdateAmmount);
        iAm = IAm.Fourth;
        SoundManager.PlaySfx("button");
    }
    void UpdateAmmount()
    {
        UpdateAmmount(inputField.text.ToString());
        SoundManager.PlaySfx("button");
    }
    public void UpdateAmmount(string name)
    {
        int count = int.Parse(name);
        if (CashAvailable(count))
        {
            if (iAm == IAm.First)
            {
                myName = casino.player[0].ammount;
                myName.text = name.ToString();
                NameUpdater.SetActive(false);
            }
            else
            if (iAm == IAm.Scond)
            {
                myName = casino.player[1].ammount;
                myName.text = name.ToString();
                NameUpdater.SetActive(false);
            }
            else if (iAm == IAm.Third)
            {
                myName = casino.player[2].ammount;
                myName.text = name.ToString();
                NameUpdater.SetActive(false);
            }
            else if (iAm == IAm.Fourth)
            {
                myName = casino.player[3].ammount;
                myName.text = name.ToString();
                NameUpdater.SetActive(false);
            }
        }
        else
        {
            casino.ShowMassege("Not Enougf Credits");
        }      

    }
    #endregion
    #region DiemondsUpdater

    void UpdateDiemonds()
    {
        dimondsButton = new Button[4];

        dimondsButton[0] = casino.player[0].betDiemonds.transform.GetComponent<Button>();
        dimondsButton[1] = casino.player[1].betDiemonds.transform.GetComponent<Button>();
        dimondsButton[2] = casino.player[2].betDiemonds.transform.GetComponent<Button>();
        dimondsButton[3] = casino.player[3].betDiemonds.transform.GetComponent<Button>();
        dimondsButton[0].onClick.AddListener(ActFirstDiemond);       
        dimondsButton[1].onClick.AddListener(ActSecondDiemond);       
        dimondsButton[2].onClick.AddListener(ActThirdDiemond);
        dimondsButton[3].onClick.AddListener(ActFourthDiemond);
        NameUpdater.SetActive(false);
    }
    void ActFirstDiemond()
    {
        NameUpdaterSet(true, "Diemonds Should between 1 TO 100");
        inputFieldButton.onClick.RemoveAllListeners();
        inputFieldButton.onClick.AddListener(UpdateDiemond);
        iAm = IAm.First;
        SoundManager.PlaySfx("button");
    }

    void ActSecondDiemond()
    {
        NameUpdaterSet(true, "Diemonds Should between 1 TO 100");
        inputFieldButton.onClick.RemoveAllListeners();
        inputFieldButton.onClick.AddListener(UpdateDiemond);
        iAm = IAm.Scond;
        SoundManager.PlaySfx("button");
    }
    void ActThirdDiemond()
    {
        NameUpdaterSet(true , "Diemonds Should between 1 TO 100");
        inputFieldButton.onClick.RemoveAllListeners();
        inputFieldButton.onClick.AddListener(UpdateDiemond);
        iAm = IAm.Third;
        SoundManager.PlaySfx("button");
    }
    void ActFourthDiemond()
    {
        NameUpdaterSet(true, "Diemonds Should between 1 TO 100");
        inputFieldButton.onClick.RemoveAllListeners();
        inputFieldButton.onClick.AddListener(UpdateDiemond);
        iAm = IAm.Fourth;
        SoundManager.PlaySfx("button");
    }
    void UpdateDiemond()
    {
        UpdateDiemond(inputField.text.ToString());
        SoundManager.PlaySfx("button");
    }
    public void UpdateDiemond(string name)
    {
        int count = int.Parse(name);
        if (DiemondsAvailable(count))
        {
            if (iAm == IAm.First)
            {
                myName = casino.player[0].betDiemonds;
                myName.text = name.ToString();
                NameUpdater.SetActive(false);
            }
            else
        if (iAm == IAm.Scond)
            {
                myName = casino.player[1].betDiemonds;
                myName.text = name.ToString();
                NameUpdater.SetActive(false);
            }
            else if (iAm == IAm.Third)
            {
                myName = casino.player[2].betDiemonds;
                myName.text = name.ToString();
                NameUpdater.SetActive(false);
            }
            else if (iAm == IAm.Fourth)
            {
                myName = casino.player[3].betDiemonds;
                myName.text = name.ToString();
                NameUpdater.SetActive(false);
            }
        }
        else
        {
            casino.ShowMassege("Not Enougf Diemonds");
        }
        
    }
    #endregion

    #region CoinUpdater

    void UpdateCoinItem()
    {
        coinsButton = new Button[4];

        coinsButton[0] = casino.player[0].betCoins.transform.GetComponent<Button>();
        coinsButton[1] = casino.player[1].betCoins.transform.GetComponent<Button>();
        coinsButton[2] = casino.player[2].betCoins.transform.GetComponent<Button>();
        coinsButton[3] = casino.player[3].betCoins.transform.GetComponent<Button>();
        coinsButton[0].onClick.AddListener(ActFirstCoin);
        coinsButton[1].onClick.AddListener(ActSecondCoin);
        coinsButton[2].onClick.AddListener(ActThirdCoin);
        coinsButton[3].onClick.AddListener(ActFourthCoin);
        NameUpdater.SetActive(false);
    }
    void ActFirstCoin()
    {
        NameUpdaterSet(true, "Coins Should between 1 TO 100");
        inputFieldButton.onClick.RemoveAllListeners();
        inputFieldButton.onClick.AddListener(UpdateCoin);
        iAm = IAm.First;
        SoundManager.PlaySfx("button");
    }

    void ActSecondCoin()
    {
        NameUpdaterSet(true, "Coins Should between 1 TO 100");
        inputFieldButton.onClick.RemoveAllListeners();
        inputFieldButton.onClick.AddListener(UpdateCoin);
        iAm = IAm.Scond;
        SoundManager.PlaySfx("button");
    }
    void ActThirdCoin()
    {
        NameUpdaterSet(true, "Coins Should between 1 TO 100");
        inputFieldButton.onClick.RemoveAllListeners();
        inputFieldButton.onClick.AddListener(UpdateCoin);
        iAm = IAm.Third;
        SoundManager.PlaySfx("button");
    }
    void ActFourthCoin()
    {
        NameUpdaterSet(true, "Coins Should between 1 TO 100");
        inputFieldButton.onClick.RemoveAllListeners();
        inputFieldButton.onClick.AddListener(UpdateCoin);
        iAm = IAm.Fourth;
        SoundManager.PlaySfx("button");
    }
    void UpdateCoin()
    {
        UpdateCoins(inputField.text.ToString());
        SoundManager.PlaySfx("button");
    }
    public void UpdateCoins(string name)
    {
        int count = int.Parse(name);
        if (CoinAvailable(count))
        {
            if (iAm == IAm.First)
            {
                myName = casino.player[0].betCoins;
                myName.text = name.ToString();
                NameUpdater.SetActive(false);
            }
            else
            if (iAm == IAm.Scond)
            {
                myName = casino.player[1].betCoins;
                myName.text = name.ToString();
                NameUpdater.SetActive(false);
            }
            else if (iAm == IAm.Third)
            {
                myName = casino.player[2].betCoins;
                myName.text = name.ToString();
                NameUpdater.SetActive(false);
            }
            else if (iAm == IAm.Fourth)
            {
                myName = casino.player[3].betCoins;
                myName.text = name.ToString();
                NameUpdater.SetActive(false);
            }
        }
        else
        {
            casino.ShowMassege("Not Enougf Coins");
        }

    }
    CasinoPlayer myPlayer()
    {
        if (iAm == IAm.First)
        {
            return  casino.casinoPlayer[0];           
        }
        else
            if (iAm == IAm.Scond)
        {
            return casino.casinoPlayer[1];
        }
        else if (iAm == IAm.Third)
        {
            return casino.casinoPlayer[2];
        }
        else if (iAm == IAm.Fourth)
        {
            return casino.casinoPlayer[3];
        }
        return null;
    }
    #endregion
    bool CoinAvailable(int count)
    {
        if(myPlayer().coin>= count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool CashAvailable(int count)
    {
        if (myPlayer().balance >= count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool DiemondsAvailable(int count)
    {
        if (myPlayer().diemond >= count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


}
