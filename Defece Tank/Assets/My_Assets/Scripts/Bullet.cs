using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Put this in its own file
public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private GameObject _trail;
    private Rigidbody _rb;
    private bool _hitsTarget;

    private void Awake() => _rb = GetComponent<Rigidbody>();
    [HideInInspector]public List<GameObject> enimies;
    private void OnEnable() 
    {
        GameObject[] cn = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var item in cn)
        {
            enimies.Add(item);
        }

        if (enimies.Count>0)
        {
            for (int i = 0; i < enimies.Count; i++)
            {
                Physics.IgnoreCollision(enimies[i].gameObject.GetComponent<Collider>(), GetComponent<Collider>());
            }
           
        }
    }

    public void Init(Vector3 vel, bool hitsTarget)
    {
        _rb.velocity = vel;
        _hitsTarget = hitsTarget;
        if (_trail) { _trail.SetActive(true); }
    }
    private void Update()
    {
        var dir = _rb.velocity;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);       
        if (collision.gameObject.tag == "Player")
        {
            GameManager_Defence.Instance.player.GetComponent<Player_Tank>().ApplyDamage(5);
        }

        Destroy(gameObject);
    }
}
