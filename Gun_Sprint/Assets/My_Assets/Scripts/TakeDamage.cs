using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamage : MonoBehaviour
{
    
    [SerializeField] bool canDie;
    [SerializeField] Animator animator;
    [SerializeField] GameObject damageEffect;
    [SerializeField] GameObject dieEffect;
    bool targetUpdated;
    // Start is called before the first frame update
    void Start()
    {
       
    }
    public void Damage(Vector3 hitPos)
    {
        CameraEffects.ShakeOnce();
        if (canDie && animator != null)
        {
            animator.SetTrigger("die");
            if (damageEffect) {
                GameObject de = Instantiate(damageEffect, hitPos, Quaternion.identity);
                Destroy(gameObject,2);
                Destroy(de, 4);
            }
        }
        if (dieEffect)
        {
            GameObject de = Instantiate(dieEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
            Destroy(de, 4);
        }
        if (targetUpdated)
            return;
        targetUpdated = true;
        GameManager_Sprint.Instance.UpdateTaget();
    }

}
