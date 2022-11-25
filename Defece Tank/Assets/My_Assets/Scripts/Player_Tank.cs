using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class Player_Tank : MonoBehaviour
{
    // Slow Motion Bar
    [Header("Slow Motion Slider Bar")]   
    [SerializeField] private float slowSliderSmoothSpeed = 0.125f;
    [SerializeField] Slider healthSlider;
    public float health;
    [SerializeField] Image fillImage;
    [SerializeField] Animation hitAnim;
    [HideInInspector] public WeaponController weapon;
    [SerializeField] AudioClip HitSound;
    [SerializeField] GameObject tankFractured;
    [SerializeField] GameObject[] fires;   
    [HideInInspector]public GameObject current_tankFractured;
    [SerializeField] GameObject lifeUpgradeEffect;
    [SerializeField] ParticleSystem hpEffect;
    [SerializeField] Image nob;
    public static Player_Tank Insance;
    public ParticleSystem shieldEffect;
    public ParticleSystem shieldActiveEffect;
    public bool shieldActivated;    
    private void Awake()
    {
        Insance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Reset();
        weapon = this.transform.GetComponentInChildren(typeof(WeaponController)).GetComponent<WeaponController>();
        lifeUpgradeEffect.SetActive(false);
        ShootButton();
        
    }
    public void Reset()
    {
        health = 100;
        healthSlider.minValue = 0;
        healthSlider.maxValue = health;
        healthSlider.value = health;
        if (tankFractured)
        {
            if (current_tankFractured) { Destroy(current_tankFractured); }
            current_tankFractured = Instantiate(tankFractured, transform.position, Quaternion.identity);
            current_tankFractured.SetActive(false);
        }
        for (int i = 0; i < fires.Length; i++)
        {
            fires[i].SetActive(false);
        }        
        GameManager_Defence.playerDied = false;
        Color newColor = Color.green;
        fillImage.color = newColor;        
        ParticleSystem.MainModule settings = hpEffect.main;
        settings.startColor = new ParticleSystem.MinMaxGradient(newColor);
        nob.color = newColor;
        shieldActiveEffect.gameObject.SetActive(false);
        Game.currentLevelTarget = 0;
        Game.achivedLevelTarget = 0;
      
    }

    public void Damage(float damageAmount)
    {
        if (health > 0)
        {
            if(health>= damageAmount) { health -= damageAmount; }
            else
            { damageAmount = health;  health -= damageAmount; }
        }
    }
    bool ShootButton()
    {
        if (Application.isMobilePlatform)
        {
            return CrossPlatformInputManager.GetButton("Shoot");
        }
        else
            return Input.GetButton("Fire1");
    }
    // Update is called once per frame
    void Update()
    {
        if (Game.gameStatus==Game.GameStatus.isPlaying)
        {
            if (ShootButton())
            {
                if (weapon)
                    weapon.LaunchWeapon();
            }                   
        }        
    }    
    bool onSlowMotion;
    float slowMotionTimeLeft;
    IEnumerator DecreaseSlowMotionTime()
    {
        while (onSlowMotion)
        {
            // decrease slow motion without considering Time.timeScale
            slowMotionTimeLeft -= Time.unscaledDeltaTime*5;
            health -= Time.unscaledDeltaTime * 5;
            healthSlider.value = health;
            // if slow motion time is less than or equal to 0 break the loop
            if (slowMotionTimeLeft <= 0f)
            {
                onSlowMotion = false;                
            }
            // will execute again in the next frame
            yield return null;
        }
    }
    bool increase;
    IEnumerator IncreaseSlowMotionTime()
    {
        while (increase)
        {
            // decrease slow motion without considering Time.timeScale
            lifeIncreaser += Time.unscaledDeltaTime*50;
            health += Time.unscaledDeltaTime * 50;
            healthSlider.value = health;
            // if slow motion time is less than or equal to 0 break the loop
            if (lifeIncreaser >= 100f)
            {
                increase = false;
            }
            // will execute again in the next frame
            yield return null;
        }
    }
    public void Dead(bool die)
    {
        if (die)
        {
            GameManager_Defence.playerDied = true;
            if (current_tankFractured)
            {
                current_tankFractured.SetActive(true);
            }
            GameManager_Defence.Instance.OnWarnClose();
            GameManager_Defence.Instance.OnGameover();
            gameObject.SetActive(false);
        }
        else
        {
            ShowWarn();
        }
    }
    void ShowWarn()
    {
     GameManager_Defence.Instance.OnWarn();
     Game.gameStatus = Game.GameStatus.isPaused;        
    }
    public void WatchAdAndGetLife()
    {
        if (GoogleSheetHandler.adx_applied)
        {
            RewardedAdManager.Instance.ShowRewardedAd();
        }
        else
        {
            AdmobAdmanager.Instance.ShowInterstitial();
        }
        GameManager_Defence.Instance.OnWarnClose();
        ActiveNewLife();
    }
    public void ApplyDamage(int damage)
    {
        if (!shieldActivated) {
            if (health > 0) {
                if (hitAnim)
                {
                    if (!hitAnim.isPlaying)
                    {
                        hitAnim.Play("hit");
                    }
                }
                if (HitSound)
                {
                    AudioSource.PlayClipAtPoint(HitSound, transform.position);
                }
                //health -= damage;
                if (health <= 0)
                {
                    Dead(false);
                }
                if (health > 60)
                {
                    for (int i = 0; i < fires.Length; i++)
                    {
                        fires[i].SetActive(false);
                    }

                }
                if (health < 70 && health > 50)
                {
                    fires[0].SetActive(true);
                }
                if (health < 50 && health > 30)
                {
                    Color newColor = Color.yellow;
                    fillImage.color = newColor;
                    fires[1].SetActive(true);
                    ParticleSystem.MainModule settings = hpEffect.main;
                    settings.startColor = new ParticleSystem.MinMaxGradient(newColor);
                    nob.color = newColor;
                }
                if (health < 30)
                {
                    fires[2].SetActive(true);
                    Color newColor = Color.red;
                    fillImage.color = newColor;
                    ParticleSystem.MainModule settings = hpEffect.main;
                    settings.startColor = new ParticleSystem.MinMaxGradient(newColor);
                    nob.color = newColor;
                }
                onSlowMotion = true;
                slowMotionTimeLeft = damage;
                StartCoroutine(DecreaseSlowMotionTime());
            }
            else
            {
                Dead(false);
            }
            
        //healthSlider.value = health;
        }
    }
    float lifeIncreaser;
    public void UpgradeLife()
    {
        lifeUpgradeEffect.SetActive(true);
        lifeUpgradeEffect.GetComponent<ParticleSystem>().Play();
        SoundManager.PlaySfx("life_up");
        increase = true;
        lifeIncreaser = health;
        StartCoroutine(IncreaseSlowMotionTime());
    }
    public void ActiveNewLife()
    {
        Game.gameStatus = Game.GameStatus.isPlaying;
        Reset();
    }
    public void ActiveShield()
    {
        shieldEffect.gameObject.SetActive(true);
        shieldEffect.Play();
        SoundManager.PlaySfx("life_up");       
    }
    public void ActivateShield()
    {
        StartCoroutine(TryActivateShield());
    }
    IEnumerator TryActivateShield()
    {
        shieldActivated = true;
        shieldActiveEffect.gameObject.SetActive(true);
        shieldActiveEffect.Play();
        yield return new WaitForSeconds(30);
        shieldActivated = false;
        shieldActiveEffect.gameObject.SetActive(false);
    }
}
