using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RouteSet
{
    public Transform[] routes;
}
[System.Serializable]
public class ItemsEnemy
{
    public GameObject crab;
    public GameObject beholder;
    public GameObject rat;
}
public class Enemy_Manager_Defence : MonoBehaviour
{
    [SerializeField] ItemsEnemy items;
    [SerializeField] RouteSet[] routeSets;
    public GameObject[] enemyPrefabs;
    public GameObject[] wormPrefabs;
    public GameObject[] wolfPrefabs;
    public GameObject[] lizardPrefabs;
    public GameObject[] nightsPrefabs;
    public int enemyCountForLevel;
    public int maxEnemyCountInScene;
    public int minEnemyCountInScene;
    public List <GameObject> enemyInScene;
    [SerializeField] Vector3 firstPoint;
    // Start is called before the first frame update
    void Start()
    {
        firstPoint = new Vector3(-7.6f, 6.8f, 35f);  
        
        for (int i = 0; i < wormPrefabs.Length; i++)
        {  wormPrefabs[i].SetActive(false); }

        for (int i = 0; i < wolfPrefabs.Length; i++)
        { wolfPrefabs[i].SetActive(false);  }

        for (int i = 0; i < lizardPrefabs.Length; i++)
        {   lizardPrefabs[i].SetActive(false);  }

        for (int i = 0; i < nightsPrefabs.Length; i++)
        { nightsPrefabs[i].SetActive(false); }
        SoundManager.PlayMusic("menu_music");
        GameManager_Defence.onGameOver += OnGameOver;
        //StartCoroutine(CreateEnemy());
        //StartCoroutine(CreateWorm());
        //StartCoroutine(CreateWolf());
        //StartCoroutine(CreateCrab());
        //StartCoroutine(CreateBeeholder());
        //StartCoroutine(CreateRat());
        //StartCoroutine(CreateKnight());
        //Game.currentLevelTarget = 15;         
    }
   
    // Update is called once per frame
    void Update()
    {
        
    }
    int intervelTime=11;
    public IEnumerator CreateEnemy(int countToCreate)
    {
        for (int i = 0; i < countToCreate; i++)
        {
            SoundManager.PlaySfx("instantiate");
            int routIndex= Random.Range(0, routeSets.Length);
            int index = Random.Range(0, enemyPrefabs.Length);
            GameObject en = Instantiate(enemyPrefabs[index], firstPoint, Quaternion.Euler(0,180,0));
            Enemy_Bezier bezier = en.GetComponent<Enemy_Bezier>();
            bezier.routes = routeSets[routIndex].routes;
            yield return new WaitForSeconds(IntervelTime());            
        }
    }
    int IntervelTime()
    {
        intervelTime--;
        if (intervelTime < 3&&Game.currentLevelTarget<11)
        {
            intervelTime = 3;
        }else
             if (intervelTime < 3 && Game.currentLevelTarget > 10 && Game.currentLevelTarget < 16)
        {
            intervelTime = 5;
        }
        else
            if (intervelTime < 3 && Game.currentLevelTarget > 15 && Game.currentLevelTarget < 21)
        {
            intervelTime = 7;
        }
        else if (intervelTime < 3 && Game.currentLevelTarget > 20 && Game.currentLevelTarget < 41)
        {
            intervelTime = 9;
        }
        return intervelTime;
    }
    public IEnumerator CreateBeeholder(int countToCreate)
    {
        yield return new WaitForSeconds(1);

        for (int i = 0; i < countToCreate; i++)
        {
            int routIndex = Random.Range(0, routeSets.Length);           
            GameObject en = Instantiate(items.beholder, firstPoint, Quaternion.Euler(0, 180, 0));
            Enemy_Bezier bezier = en.GetComponent<Enemy_Bezier>();
            bezier.routes = routeSets[routIndex].routes;
            yield return new WaitForSeconds(IntervelTime());
        }
    }
    IEnumerator CreateWorm(int countToCreate)
    {       
        for (int i = 0; i < countToCreate; i++)
        {
            wormPrefabs[i].SetActive(true);
            yield return new WaitForSeconds(IntervelTime());
        }       
    }
    IEnumerator CreateWolf(int countToCreate)
    {
        for (int i = 0; i < countToCreate; i++)
        {
            wolfPrefabs[i].SetActive(true);
            yield return new WaitForSeconds(1);
        }
    }
    IEnumerator CreateLizard(int countToCreate)
    {
        for (int i = 0; i < countToCreate; i++)
        {
            lizardPrefabs[i].SetActive(true);
            yield return new WaitForSeconds(IntervelTime());
        }
    }
    IEnumerator CreateKnight(int countToCreate)
    {
        for (int i = 0; i < countToCreate; i++)
        {
            nightsPrefabs[i].SetActive(true);
            yield return new WaitForSeconds(IntervelTime());
        }
    }
    IEnumerator CreateCrab(int countToCreate)
    {
        for (int i = 0; i < countToCreate; i++)
        {
            Vector3 randomPose = new Vector3(Random.Range(10f, -9f), 4f, Random.Range(16f,40f));
            Instantiate(items.crab, randomPose, Quaternion.identity);
            yield return new WaitForSeconds(IntervelTime());
        }
       
    }
    IEnumerator CreateRat(int countToCreate)
    {
        for (int i = 0; i < countToCreate; i++)
        {
            Vector3 randomPose = new Vector3(Random.Range(10f, -9f), 4f, Random.Range(16f, 40f));
            Instantiate(items.rat, randomPose, Quaternion.identity);
            yield return new WaitForSeconds(IntervelTime());
        }

    }
    public void ActivateLevel(int levelCount)
    {
        SoundManager.PlaySfx("button");
        StartCoroutine(GameManager_Defence.Instance.PlayTransition());       
        StartCoroutine(StartGame(levelCount));
        GameManager_Defence.Instance.ApplyEnvironment();
    }
    IEnumerator StartGame(int levelCount)
    {
        yield return new WaitForSeconds(2);        
        GameManager_Defence.Instance.ShowUIMenu(1, true);
        GameManager_Defence.Instance.ActivatePlayer();
        yield return new WaitForSeconds(1);
        Game.currentLevelTarget = 0;
        switch (levelCount)
        {
            case 1:
                StartCoroutine(CreateEnemy(10));
                Game.currentLevelTarget = 10;                
                break;
            case 2:
                StartCoroutine(CreateWolf(10));
                Game.currentLevelTarget = 10;
                break;
            case 3:
                StartCoroutine(CreateBeeholder(20));
                Game.currentLevelTarget = 20;
                break;
            case 4:
                StartCoroutine(CreateCrab(25));
                Game.currentLevelTarget = 25;
                break;
            case 5:
                StartCoroutine(CreateWorm(15));
                Game.currentLevelTarget = 15;
                break;
            case 6:
                StartCoroutine(CreateRat(20));
                Game.currentLevelTarget = 20;
                break;
            case 7:
                StartCoroutine(CreateLizard(15));
                Game.currentLevelTarget = 15;
                break;
            case 8:
                StartCoroutine(CreateKnight(20));
                Game.currentLevelTarget = 20;
                break;
            case 9:
                StartCoroutine(CreateBeeholder(10));
                StartCoroutine(CreateRat(10));
                Game.currentLevelTarget = 20;
                break;
            case 10:
                StartCoroutine(CreateCrab(10));
                StartCoroutine(CreateWorm(10));
                Game.currentLevelTarget = 20;
                break;
            case 11:
                StartCoroutine(CreateEnemy(10));
                StartCoroutine(CreateWolf(10));
                Game.currentLevelTarget = 20;
                break;
            case 12:
                StartCoroutine(CreateBeeholder(10));
                StartCoroutine(CreateKnight(5));
                StartCoroutine(CreateCrab(5));
                Game.currentLevelTarget = 20;
                break;
            case 13:
                StartCoroutine(CreateBeeholder(10));
                StartCoroutine(CreateRat(10));
                StartCoroutine(CreateLizard(10));
                Game.currentLevelTarget = 30;
                break;
            case 14:
                StartCoroutine(CreateBeeholder(10));
                StartCoroutine(CreateKnight(10));
                StartCoroutine(CreateCrab(10));
                Game.currentLevelTarget = 30;
                break;
            case 15:
                StartCoroutine(CreateEnemy(10));
                StartCoroutine(CreateBeeholder(10));
                StartCoroutine(CreateWolf(10));
                StartCoroutine(CreateWorm(10));
                Game.currentLevelTarget = 40;
                break;
        }
        Game.currentLevel = levelCount;
        GameManager_Defence.Instance.CallOnGameStarts();
    }
    void OnGameOver()
    {
        StopAllCoroutines();
    }
    private void OnDestroy()
    {
        GameManager_Defence.onGameOver -= OnGameOver;
    }
}
