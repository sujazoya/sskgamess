using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemTypes_Defence
{
    public string itemName;
    public GameObject itemPrefab;
    public List<GameObject> itemStorage;
    public int itemCount = 100;
    public float itemDieTime = 2;
}

public class PoolManager_Defence : MonoBehaviour
{
    public ItemTypes_Defence[] itemTypes_;
   
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CreateBlood());
        StartCoroutine(CreateDebris());
        StartCoroutine(CreateBullet());
        StartCoroutine(CreateRocket());
        StartCoroutine(CreateMiniRocket());
        StartCoroutine(CreateExplosion());
        StartCoroutine(CreateDeathEffects());
        StartCoroutine(CreateDeathEffectsSmall());
    }
    IEnumerator CreateBlood()
    {
        GameObject bloodParent = new GameObject("bloodParent");
        for (int i = 0; i < itemTypes_[0].itemCount; i++)
        {
            GameObject blood = Instantiate(itemTypes_[0].itemPrefab, Vector3.zero, Quaternion.identity);
            blood.transform.parent = bloodParent.transform;
            itemTypes_[0].itemStorage.Add(blood);
            blood.SetActive(false);
            yield return new WaitForSeconds(0.02f);
        }
        
    }
    IEnumerator CreateDeathEffects()
    {
        GameObject DeathEffect = new GameObject("DeathEffect");
        for (int i = 0; i < itemTypes_[6].itemCount; i++)
        {
            GameObject df = Instantiate(itemTypes_[6].itemPrefab, Vector3.zero, Quaternion.identity);
            df.transform.parent = DeathEffect.transform;
            itemTypes_[6].itemStorage.Add(df);
            df.SetActive(false);
            yield return new WaitForSeconds(0.02f);
        }

    }
    IEnumerator CreateDeathEffectsSmall()
    {
        GameObject DeathEffectSall = new GameObject("DeathEffectSall");
        for (int i = 0; i < itemTypes_[7].itemCount; i++)
        {
            GameObject dfs = Instantiate(itemTypes_[7].itemPrefab, Vector3.zero, Quaternion.identity);
            dfs.transform.parent = DeathEffectSall.transform;
            itemTypes_[7].itemStorage.Add(dfs);
            dfs.SetActive(false);
            yield return new WaitForSeconds(0.02f);
        }

    }
    IEnumerator CreateBullet()
    {
        GameObject BulletParent = new GameObject("BulletParent");
        for (int i = 0; i < itemTypes_[2].itemCount; i++)
        {
            GameObject bullet = Instantiate(itemTypes_[2].itemPrefab, Vector3.zero, Quaternion.identity);
            bullet.transform.parent = BulletParent.transform;
            itemTypes_[2].itemStorage.Add(bullet);
            bullet.SetActive(false);
            yield return new WaitForSeconds(0.02f);
        }
      
    }
    IEnumerator CreateDebris()
    {
        GameObject DebrisParent = new GameObject("DebrisParent");
        for (int i = 0; i < itemTypes_[1].itemCount; i++)
        {
            GameObject debris = Instantiate(itemTypes_[1].itemPrefab, Vector3.zero, Quaternion.identity);
            debris.transform.parent = DebrisParent.transform;
            itemTypes_[1].itemStorage.Add(debris);
            debris.SetActive(false);
            yield return new WaitForSeconds(0.02f);
        }
       
    }
    IEnumerator CreateRocket()
    {
        GameObject RocketParent = new GameObject("RocketParent");
        for (int i = 0; i < itemTypes_[3].itemCount; i++)
        {
            GameObject debris = Instantiate(itemTypes_[3].itemPrefab, Vector3.zero, Quaternion.identity);
            debris.transform.parent = RocketParent.transform;
            itemTypes_[3].itemStorage.Add(debris);
            debris.SetActive(false);
            yield return new WaitForSeconds(0.02f);
        }
       
    }
    IEnumerator CreateMiniRocket()
    {
        GameObject RocketParent = new GameObject("MiniRocketParent");
        for (int i = 0; i < itemTypes_[4].itemCount; i++)
        {
            GameObject debris = Instantiate(itemTypes_[4].itemPrefab, Vector3.zero, Quaternion.identity);
            debris.transform.parent = RocketParent.transform;
            itemTypes_[4].itemStorage.Add(debris);
            debris.SetActive(false);
            yield return new WaitForSeconds(0.02f);
        }
        Debug.Log("Creaing Done");
    }
    IEnumerator CreateExplosion()
    {
        GameObject ExplosionParent = new GameObject("ExplosionParent");
        for (int i = 0; i < itemTypes_[5].itemCount; i++)
        {
            GameObject explosion = Instantiate(itemTypes_[5].itemPrefab, Vector3.zero, Quaternion.identity);
            explosion.transform.parent = ExplosionParent.transform;
            itemTypes_[5].itemStorage.Add(explosion);
            explosion.SetActive(false);
            yield return new WaitForSeconds(0.02f);
        }
        Debug.Log("Creaing Done");
    }
    int bloodIndex = 0;
    public void PlayBlood(Vector3 position)
    {
        itemTypes_[0].itemStorage[bloodIndex].transform.position = position;
        itemTypes_[0].itemStorage[bloodIndex].SetActive(true);
        StartCoroutine(GetOffItem(itemTypes_[0].itemStorage[bloodIndex], itemTypes_[0].itemDieTime));
        bloodIndex++;
        if (bloodIndex >= itemTypes_[0].itemCount)
        {
            bloodIndex = 0;
        }
    }
    int debrisIndex = 0;
    public void PlayDebris(Vector3 position)
    {
        itemTypes_[1].itemStorage[debrisIndex].transform.position = position;
        itemTypes_[1].itemStorage[debrisIndex].SetActive(true);
        StartCoroutine(GetOffItem(itemTypes_[1].itemStorage[debrisIndex], itemTypes_[1].itemDieTime));
        debrisIndex++;
        if (debrisIndex >= itemTypes_[1].itemCount)
        {
            debrisIndex = 0;
        }
    }
    int explosionIndex = 0;
    public void PlayExplosion(Vector3 position)
    {
        itemTypes_[5].itemStorage[explosionIndex].transform.position = position;
        itemTypes_[5].itemStorage[explosionIndex].SetActive(true);
        StartCoroutine(GetOffItem(itemTypes_[5].itemStorage[explosionIndex], itemTypes_[5].itemDieTime));
        explosionIndex++;
        if (explosionIndex >= itemTypes_[5].itemCount)
        {
            explosionIndex = 0;
        }
    }
    IEnumerator GetOffItem(GameObject item, float offTime)
    {
        yield return new WaitForSeconds(offTime);
        item.SetActive(false);
    }
    int bulletIndex = 0;
    public GameObject CurrentBullet()
    {
        bulletIndex++;
        if (bulletIndex >= itemTypes_[2].itemCount)
        {
            bulletIndex = 0;
        }
        return itemTypes_[2].itemStorage[bulletIndex];

    }
    int rocketIndex;
    public GameObject CurrentRocket()
    {
        rocketIndex++;
        if (rocketIndex >= itemTypes_[3].itemCount)
        {
            rocketIndex = 0;
        }
        return itemTypes_[3].itemStorage[rocketIndex];

    }
    int miniRocketIndex;
    public GameObject CurrentMiniRocket()
    {
        miniRocketIndex++;
        if (miniRocketIndex >= itemTypes_[4].itemCount)
        {
            miniRocketIndex = 0;
        }
        return itemTypes_[4].itemStorage[miniRocketIndex];

    }
    int DeathEffectIndex = 0;
    public void PlayDeathEffect(Vector3 position)
    {
        itemTypes_[6].itemStorage[DeathEffectIndex].transform.position = position;
        itemTypes_[6].itemStorage[DeathEffectIndex].SetActive(true);
        StartCoroutine(GetOffItem(itemTypes_[6].itemStorage[DeathEffectIndex], itemTypes_[6].itemDieTime));
        DeathEffectIndex++;
        if (DeathEffectIndex >= itemTypes_[6].itemCount)
        {
            DeathEffectIndex = 0;
        }
    }
    int DeathEffectIndexs = 0;
    public void PlayDeathEffectSmall(Vector3 position)
    {
        itemTypes_[7].itemStorage[DeathEffectIndexs].transform.position = position;
        itemTypes_[7].itemStorage[DeathEffectIndexs].SetActive(true);
        StartCoroutine(GetOffItem(itemTypes_[7].itemStorage[DeathEffectIndexs], itemTypes_[7].itemDieTime));
        DeathEffectIndexs++;
        if (DeathEffectIndexs >= itemTypes_[7].itemCount)
        {
            DeathEffectIndexs = 0;
        }
    }
}
