using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

[System.Serializable]
public class SlotItems
{
    public Transform[] slotPieces;
    public Slider speedSlider;
    //public Sprite[] statusSprite;   
    public GameObject winEffect;
    public Text cashText;
    public GameObject push_to_start;
    public AudioClip pullClip, leftClip, slotClip;
    public GameObject resultEffect;    
}

public class SlotManager : MonoBehaviour
{
   
    [SerializeField] SlotItems Items;
    float speed = 15f;
   public  bool playCasino;
    Animator slotAnimator;
    float pushTime;
    Slot_Piece[] slots;
    [HideInInspector] public int Bonus, Wild, Bar, Seven;
    private AudioSource audioSource;
    private AudioSource otheraudioSource;
    [SerializeField] AudioSource slot_AudioSource;
    int slotCount = 20;
    public Text slotCount_text;
    private void Awake()
    {
        Items.winEffect.SetActive(false);
        slotCount = 20;
        if (slotCount_text) { slotCount_text.text = slotCount.ToString(); }
    }
    // Start is called before the first frame update
    void Start()
    {
        slotAnimator = GetComponent<Animator>();
        slots = FindObjectsOfType<Slot_Piece>();
        Items.cashText.text = Game.TotalCoins.ToString();
        Items.push_to_start.SetActive(true);
        audioSource = gameObject.AddComponent<AudioSource>();
        otheraudioSource = gameObject.AddComponent<AudioSource>();
        otheraudioSource.playOnAwake = false;
        audioSource.playOnAwake = false;
        MusicManager.PlayMusic("slot_music");
        if (Items.resultEffect) { Items.resultEffect.SetActive(false); }
        //slotAnimator.enabled = false;
    }
    public void RoundCasino()
    {
        
        //speed = Random.Range(15, 40);
        playCasino = true;       
        //casinoStatus = CasinoStatus.Rounding;
        //resultext.gameObject.SetActive(false);
    }
   
    // Update is called once per frame
    void Update()
    {
        if (playCasino)
        {
            Items.slotPieces[0].Rotate(0, 0, speed);
            Items.slotPieces[1].Rotate(0, 0, speed);
            Items.slotPieces[2].Rotate(0, 0, -speed);
            Items.speedSlider.value = speed;
            if (!PlayerPrefs.HasKey(Game.soundKey))
            {

                if (!slot_AudioSource.isPlaying)
                {
                    slot_AudioSource.loop = true;
                    slot_AudioSource.Play();
                }
            }
            if (speed > 0)
            {
                speed -= Time.deltaTime;
               
                if (speed >= 7)
                {
                    //if (!spin_Sound.isPlaying) { spin_Sound.Play(); }
                }
                else
                {
                    //if (spin_Sound.isPlaying) { spin_Sound.Stop(); }
                    //if (!wheel_stopping.isPlaying) { wheel_stopping.Play(); }
                }
            }
            else
            {
                playCasino = false;
                speed = 15f;
                for (int i = 0; i < slots.Length; i++)
                {
                    slots[i].FixRotation();
                }
                StartCoroutine(ShowWin());
                
                
                //casinoStatus = CasinoStatus.Stopped;
                //OnGameComplete();               
                //resultext.gameObject.SetActive(true);
                //if (wheel_stopping.isPlaying) { wheel_stopping.Stop(); }               
                //MusicManager.PauseMusic(0.2f);
            }

        }
        if (CrossPlatformInputManager.GetButtonDown("push") && !playCasino&&slotCount>0)
        {
            slotAnimator.SetTrigger("push");
            Items.push_to_start.SetActive(false);
            PlaySoundClip(Items.pullClip);
            MusicManager.PauseMusic(0.2f);
            //gotItem = GotItem.None;

        }
        if (CrossPlatformInputManager.GetButton("push") && !playCasino)
        {
            pushTime += Time.deltaTime;
            //slotAnimator.enabled = true;
            speed += Time.deltaTime*6;
            Items.speedSlider.value = speed;
        }
        if (CrossPlatformInputManager.GetButtonUp("push")&&!playCasino)
        {
            StartSlot();
            

        }
        if (pushTime > 3f && !playCasino)
        {
            StartSlot();
            
        }
    }
    bool win;
    int givingAmmount;
    IEnumerator ShowWin()
    {
        slot_AudioSource.Stop();
        yield return new WaitForSeconds(1);
        if (Items.resultEffect)
        {
            Items.resultEffect.SetActive(true);
        }
        Items.winEffect.SetActive(true);
        MusicManager.PlaySfx_Other("result");
        GameObject DollarBlast = Items.winEffect.transform.Find("DollarBlast").gameObject;
        //GameObject trys = Items.winEffect.transform.Find("try").gameObject;
        //trys.SetActive(false);
        Text ammount = Items.winEffect.transform.Find("ammount").GetComponent<Text>();
        ammount.gameObject.SetActive(false);
        
        if (Bar == 3)
        {
            
            SetTexture(m_MainTexture[0]);
            win = true;
            givingAmmount = 50;
        }
        if (Bonus == 3)
        {
           
            SetTexture(m_MainTexture[1]);
            win = true;
            givingAmmount = 150;
        }
        if (Seven == 3)
        {
           
            SetTexture(m_MainTexture[2]);
            win = true;
            givingAmmount = 100;
        }
        if (Wild == 3)
        {
            
            SetTexture(m_MainTexture[3]);
            win = true;
            givingAmmount = 200;
        }
        if (win)
        {
            DollarBlast.SetActive(true);
            MusicManager.PlaySfx("doller");
            yield return new WaitForSeconds(1);
            ammount.gameObject.SetActive(true);
            ammount.text = givingAmmount.ToString();
            MusicManager.PlaySfx("bell");
            Game.TotalCoins += givingAmmount;
            yield return new WaitForSeconds(2);
            Items.cashText.text = Game.TotalCoins.ToString();
            MusicManager.PlaySfx("coin");
        }
        else
        {
            SetTexture(m_MainTexture[4]);          
           
            MusicManager.PlaySfx("fail");
            DollarBlast.SetActive(false);
            //trys.SetActive(true);
        }       
        yield return new WaitForSeconds(3);        
        Items.winEffect.SetActive(false);
        Items.push_to_start.SetActive(true);
        AdmobAdmanager.Instance.ShowInterstitial();
        // FacebookAdManager.Instance.ShowInterstitial();
        if (Items.resultEffect)
        {
            Items.resultEffect.SetActive(false);
        }
    }
    void StartSlot()
    {
        slotCount--;
        PlaySoundClip(Items.leftClip);
        Items.slotPieces[0].GetComponent<Slot_Piece>().ResetRotation();
        Items.slotPieces[1].GetComponent<Slot_Piece>().ResetRotation();
        Items.slotPieces[2].GetComponent<Slot_Piece>().ResetRotation();
        win = false;
        pushTime = 0;
        Bonus = 0;
        Wild = 0;
        Bar = 0;
        Seven = 0;
        givingAmmount = 0;
        RoundCasino();
        slotAnimator.SetTrigger("pushEnd");
        Items.push_to_start.SetActive(false);
        if (slotCount_text) { slotCount_text.text = slotCount.ToString(); }
    } 
    void PlaySoundClip(AudioClip clip)
    {
        if (!PlayerPrefs.HasKey(Game.soundKey))
        {
            otheraudioSource.Stop();
            if (!otheraudioSource.isPlaying)
           {

                otheraudioSource.clip = null;
                otheraudioSource.clip = clip;
                otheraudioSource.Play();
          }
            otheraudioSource.volume = 1f;
        }
    }
    public Texture[] m_MainTexture;
    [SerializeField ] Material m_Renderer;

    // Use this for initialization
    void SetTexture(Texture myText)
    {  
        m_Renderer.SetTexture("_MainTex", myText);        
    }

}
