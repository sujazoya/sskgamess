using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf_Bezier : MonoBehaviour
{
    public Transform[] routes;

    int routeToGo;
    float tParam;
    Vector3 enemyPosition;
   [SerializeField]  float speedModifier;
    bool coroutineAllowed;
    [SerializeField] ParticleSystem attackParticle;
    DamageManager damageManager;
    [SerializeField] float attackInterval = 3;
    Player_Tank player_;
    [SerializeField] float speed;
    Animator animator;
    string attack =    "attack";
    string die =       "die";
    string walk=       "walk";
    string run =       "run";
    int totallRoute;
    // Start is called before the first frame update
    void Start()
    {
        Reset();
        animator = GetComponent<Animator>();
    }
    private void Reset()
    {
        routeToGo = 0;
        tParam = 0;
        //speedModifier = 0.5f;
        coroutineAllowed = true;
        if (attackParticle)
        {
            attackParticle.gameObject.SetActive(false);
        }
        damageManager = GetComponent<DamageManager>();
        player_ = GameManager_Defence.Instance.player.GetComponent<Player_Tank>();
        totallRoute = routes.Length - 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (coroutineAllowed && Game.gameStatus == Game.GameStatus.isPlaying)
        {
            StartCoroutine(GoByTheRoute(routeToGo));
        }
        
    }
    private void LateUpdate()
    {
        if (GameManager_Defence.playerDied == false && !attacking && canAtack && Game.gameStatus == Game.GameStatus.isPlaying)
        {
            StartCoroutine(Attack());
        }
    }
    bool attacking;
    bool canAtack;
    void PlayAnim(string animName)
    {
        animator.SetTrigger(animName);
    }
    IEnumerator GoByTheRoute(int routeNo)
    {
        if (damageManager.dead)
           yield return null;
        coroutineAllowed = false;
        transform.LookAt(GameManager_Defence.Instance.player);
        Vector3 p0 = routes[routeNo].GetChild(0).position;
        Vector3 p1 = routes[routeNo].GetChild(1).position;
        Vector3 p2 = routes[routeNo].GetChild(2).position;
        Vector3 p3 = routes[routeNo].GetChild(3).position;
        if (routeToGo < totallRoute - 1)
        {
            PlayAnim(walk);
        }
        else
        {
            PlayAnim(run);
        }
        while (tParam < 1)
        {
            tParam += Time.deltaTime/speedModifier;
            enemyPosition = Mathf.Pow(1 - tParam, 3) * p0 +
                3 * Mathf.Pow(1 - tParam, 2) *tParam* p1 +
              3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 +
              Mathf.Pow(tParam, 3) * p3;
            if (!damageManager.dead)
            {
                transform.position = enemyPosition;
            }           
            //transform.rotation = Quaternion.Euler(enemyPosition);
            yield return new WaitForEndOfFrame();
        }
        tParam = 0;
        routeToGo += 1;
        //ResetRotation();      
        if (routeToGo > routes.Length - 1)
        {
            routeToGo = 0;
            if (player_.health > 0 && Game.gameStatus == Game.GameStatus.isPlaying)
            {
                StartCoroutine(Attack());
                canAtack = true;
            }          
        }
        else
        {
            coroutineAllowed = true;
        }   
         
    }
    void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
    }
    IEnumerator Attack()
    {
        if (!damageManager.dead)
        {
            attacking = true;
            PlayAnim(attack);
            transform.LookAt(GameManager_Defence.Instance.player);
            yield return new WaitForSeconds(attackInterval);
            if (player_.health > 0)
            {
                StartCoroutine(Attack());
            }
            else
            {
                attacking = false;
            }
        }
    }
    public void TryToAttack()
    {
        if (attackParticle)
        {
            attackParticle.gameObject.SetActive(false);
            attackParticle.gameObject.SetActive(true);
            attackParticle.Play();
            Invoke(nameof(MakeDamage), 0.5f);
        }
        this.gameObject.SendMessage("OnAttack", SendMessageOptions.DontRequireReceiver);
    }
    void MakeDamage()
    {
        GameManager_Defence.Instance.player.GetComponent<Player_Tank>().ApplyDamage(5);
    }
}
