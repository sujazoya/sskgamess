using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// This is a general manager for the enitre game its where you assign variables and game objects associated with the individual scenes. In future updatesi want to implement this 
///  objects DoNotdestroy on load behavior. dfor now its in both scenes. 
/// </summary>
public class GameManager : MonoBehaviour 
{

    private FiniteStateMachine<GameManager> FSM = new FiniteStateMachine<GameManager>();

    [HideInInspector]
    public GAMEMODES gameMode = GAMEMODES.NONE;
    [HideInInspector]
    public GAMESTATES activeState;
    //[HideInInspector]
    //public float idleTimer;

    public Transform[] cameraLocation; 
    
    public SceneManagerSpin scene;

    public string[] sceneNames;

    //public float maxIdleTime; 

    public GameObject wheelSTW;
    public GameObject wheelWOT;
    public GameObject wheelsSM; 
    //public GameObject menuObj; 
    public GameObject arrowIndicator; 

    public SpinnerCollision pointerSTW;
    public PointRay pointerWOT;
    public PointRay[] pointerSM;
    public WinLoseObj winLoseObj; 

    //public GUIText scoreObj;
    //public GUIText spinCounter; 
    public Text scoreObj;
    public Text spinCounter;
    public GameObject resultObj;
    public TextMeshPro resultText;
    public GameObject winObj;
    public Text winText;
    public Text ammountText;
    #region UnityMethods
    void Start () 
	{
        Messenger.AddListener<FSMState<GameManager>>(SIG.BROADCASTSTATE.ToString(),ChangeState);
        FSM.Initialize(this, new GameIdleState());
        activeState = GAMESTATES.IDLE;
        if (resultObj) { resultObj.SetActive(false); }
        if (winObj) { winObj.SetActive(false); }
        UpdateBalance();
    }
    public void ShowResult(int score)
    {
       StartCoroutine( ShowResults(score));
    }
    IEnumerator ShowResults(int score)
    {
        if (resultObj) { resultObj.SetActive(transform); }
        resultText.text = score.ToString();
        yield return new WaitForSeconds(5);
        resultText.text = string.Empty;
        if (resultObj) { resultObj.SetActive(false); }

    }
    public void ShowWin(int score)
    {
        StartCoroutine(ShowWins(score));
    }
    IEnumerator ShowWins(int score)
    {
        if (winObj) { winObj.SetActive(transform); }
        winText.text = score.ToString();
        yield return new WaitForSeconds(5);
        winText.text = string.Empty;
        if (winObj) { winObj.SetActive(false); }
        UpdateBalance();

    }
    void Update()
    {
        FSM.Update();
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Application.LoadLevel(sceneNames[0]);
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Application.LoadLevel(sceneNames[1]);
        }
        
    }
    #endregion

    #region Actions
    public void ChangeState(FSMState<GameManager> state)
    {
        FSM.ChangeState(state);
    }
    void UpdateBalance()
    {
        ammountText.text = Game.TotalCoins.ToString();
    }
    public void SceneLoaded()
    {
        string loadedScene = Application.loadedLevelName;

        if (loadedScene == sceneNames[0])
        {
            ChangeState(new GameSpinToWinState());
            gameMode = GAMEMODES.SPINTOWIN;
            MusicManager.PlayMusic("spinToWin");
        }
        else if (loadedScene == sceneNames[1])
        {
            ChangeState(new GameWheelOfTriumphState());
            gameMode = GAMEMODES.WHEELOFTRIUMPH;
            MusicManager.PlayMusic("WOT");
        }
        else if (loadedScene == sceneNames[2])
        {
            ChangeState(new GameExampleSceneState());
            gameMode = GAMEMODES.EXAMPLESCENE;
        }
    }
    #endregion
}
