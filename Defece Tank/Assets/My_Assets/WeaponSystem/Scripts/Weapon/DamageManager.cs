using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class DamageManager : MonoBehaviour
{
    public GameObject floatingTex;
    public AudioClip[] HitSound;
    public GameObject Effect;
    public int HP = 100;
    [SerializeField] Animator animator;   
    PoolManager_Defence poolManager;
    [SerializeField] bool haveRagdoll;
    [HideInInspector] public List<Collider> rdColliders;
    [HideInInspector] public List<Rigidbody> rdRigidbody;
    [SerializeField] MonoBehaviour enemyScript;
    [HideInInspector]public bool dead = false;
    Slider mySlider;
    [SerializeField] Rigidbody rigidbody1;
    [SerializeField] bool activeRBOodead;
    [SerializeField] AudioClip attackSound, dieSound, screamSound;
    AudioSource audioSource;
    public bool life;
    public bool shield;
    public enum AnimalType
    {
        None,Fly,Walk,UnderGround
    }
    public enum AnimalSize
    {
        Big,Small
    }
    [SerializeField] AnimalSize animalSize;
    [SerializeField] AnimalType animalType;
    private void Start()
    {
       
        poolManager = FindObjectOfType<PoolManager_Defence>();
        if (haveRagdoll)
        {
            Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rigidbody in rigidbodies)
            {
                rdRigidbody.Add(rigidbody);
            }
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                rdColliders.Add(collider);
            }
            rdColliders.RemoveAt(0);
            rdRigidbody.RemoveAt(0);
            SetColliderState(false);
            SetRigidbodyState(true);
        }
        animator = GetComponent<Animator>();
        switch (animalType)
        {
            case AnimalType.Fly:

                break;
            case AnimalType.Walk:

                break;
            case AnimalType.UnderGround:                
                break;
        }
        mySlider = GameManager_Defence.Instance.CurrentSlider();
        mySlider.value = HP;
        audioSource = gameObject.AddComponent<AudioSource>();
        int randomIndex = Random.Range(0, 10);
        if (randomIndex == 1)
        {
            life = true;
        }
        else if (randomIndex == 2)
        {
            shield = true;
        }
        else
        {
           
        }
        GameManager_Defence.onGameOver += OnGameOver;
    }
    public void SetRigidbodyState(bool state)
    {
       
        for (int i = 0; i < rdRigidbody.Count; i++)
        {
            rdRigidbody[i].isKinematic = state;
        }
    }
    public void SetColliderState(bool state)
    {
        for (int i = 0; i < rdColliders.Count; i++)
        {
            rdColliders[i].enabled = state;
        }  
    }
    public void ApplyDamage(int damage)
    {
		if(HP <= 0 || dead)
		return;
        switch (animalType)
        {
            case AnimalType.Fly:

                break;
            case AnimalType.Walk:

                break;
            case AnimalType.UnderGround:
              
                Invoke(nameof(OffSlider), 1);
                break;
        }

        if (HitSound.Length > 0)
        {
            AudioSource.PlayClipAtPoint(HitSound[Random.Range(0, HitSound.Length)], transform.position);
        }
        if(damage<= HP)
        {
            // do nothing
        }
        else
        {
            damage = HP;
        }
        HP -= damage;
        if (HP <= 0)
        {
            Dead();
        }
        if (HP < 50 && HP > 30)
        {
            //fillImage.color = Color.yellow;
        }
        if (HP < 30)
        {
            //fillImage.color = Color.red;
        }       
        mySlider.value = HP;
       
        poolManager.PlayBlood(transform.position);
        if (screamSound)
        {
            audioSource.clip = screamSound;
            audioSource.Play();
        }
    }
    void OffSlider()
    {
       
    }
    public void OnAttack()
    {
        if (attackSound)
        {
            audioSource.clip = attackSound;
            audioSource.Play();
        }
    }
   
    private void Dead()
    {
        if (dead)
            return;
        mySlider.gameObject.SetActive(false);  
        dead = true;
        poolManager.PlayBlood(transform.position);
        if (animator) {
           
            animator.SetTrigger("die");            
            switch (animalType)
            {
                case AnimalType.Fly:
                    Invoke(nameof(FalseAnimator), 1.5f);
                    break;
                case AnimalType.Walk:
                    Invoke(nameof(FalseAnimator), 1f);
                    break;
                case AnimalType.UnderGround:                   
                    Invoke(nameof(OffSlider), 1);
                    Invoke(nameof(FalseAnimator), 2.5f);
                    break;
            }
            Destroy(this.gameObject, 5); 
        }       
		this.gameObject.SendMessage("OnDead",SendMessageOptions.DontRequireReceiver);
        if (haveRagdoll)
        {
            enemyScript.enabled = false;
            GetComponent<Collider>().enabled = false;
            SetColliderState(true);
            SetRigidbodyState(false);
        }
        if (rigidbody1 && activeRBOodead)
        {
            rigidbody1.isKinematic = false;
        }        
        if (dieSound)
        {
            audioSource.clip = dieSound;
            audioSource.Play();
        }
        switch (animalSize)
        {
            case AnimalSize.Big:
                poolManager.PlayDeathEffect(transform.position);
                break;
            case AnimalSize.Small:
                poolManager.PlayDeathEffectSmall(transform.position);
                break;
        }
        if (life)
        {
            GameManager_Defence.Instance.AddLife();           
        }
        else if(shield)
        {
            GameManager_Defence.Instance.AddShield();
        }
        else
        {
            GameManager_Defence.Instance.GiveCoin();
        }
        GameManager_Defence.Instance.OnEnemyKilled();
    }
    void FalseAnimator()
    {
        animator.enabled = false;
    }
    void ShowFloatingText()
    {
      GameObject GO=  Instantiate(floatingTex, transform.position, Quaternion.identity, transform);
        GO.GetComponent<TextMesh>().text = HP.ToString();
    }

    void OnGameOver()
    {
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        GameManager_Defence.onGameOver -= OnGameOver;
    }
}
