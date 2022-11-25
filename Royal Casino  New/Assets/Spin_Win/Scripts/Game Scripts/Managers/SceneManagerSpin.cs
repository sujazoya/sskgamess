using UnityEngine;
using System.Collections;
/// <summary>
/// Base class for a scene manager.. The scene manager runs the individual game you are playing. Reference the exisiting scenemangers if you wish to add more scenes. 
/// </summary>
public abstract class SceneManagerSpin : MonoBehaviour
{
    #region Variables
    private FiniteStateMachine<SceneManagerSpin> FSM = new FiniteStateMachine<SceneManagerSpin>();

    [HideInInspector]
    public SCENESTATES activeState;
    [HideInInspector]
    public int winLose = -1;
    [HideInInspector]
    public GameObject wheel;
    [HideInInspector]
    public SpinnerCollision spinner;
    [HideInInspector]
    public PointRay pointer;
    [HideInInspector]
    public WinLoseObj winLoseobj;

    //[HideInInspector]
    //public GUIText spinCounter;
    public UnityEngine.UI.Text spinCounter;

    public AudioClip[] sounds; 
    
    //public GUIText scoreObj;
    public UnityEngine.UI.Text scoreObj;

    public Transform[] cameraPos;

    public int score;
    //public int maxSpins;
    
    public float minVel;
    public float timeScale;

    public bool spinAgain = false;
    public GameManager manager;
    #endregion

    #region UnityMethods

    public virtual void Start()
    {
        winLose = -1; 
        FSM.Initialize(this, new SceneIntroState());
        manager = FindObjectOfType<GameManager>();
    }

    public virtual void Update()
    {
        FSM.Update();
    }

    #endregion

    #region Actions
    public virtual void ChangeState(FSMState<SceneManagerSpin> state)
    {
        FSM.ChangeState(state);
        
    }

    public virtual void Scoring(){}
    public virtual void FinalScore() { winLoseobj.WinLose(winLose); }
    public virtual void Reset()
    {
        winLoseobj.Reset();
        score = 0;
        scoreObj.text = "SCORE:";
        spinCounter.text = Game.TotalSpinCount + " Spins Left";
        Destroy(this.gameObject, .2f); 
    } 
    #endregion
}
