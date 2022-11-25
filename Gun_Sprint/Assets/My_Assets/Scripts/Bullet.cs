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

    public void Init(Vector3 vel, bool hitsTarget)
    {
        _rb.velocity = vel;
        _hitsTarget = hitsTarget;
        if (_hitsTarget) _trail.SetActive(true);
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

        if (_hitsTarget)
        {
            TimeManager_1.Instance.ToggleSlowMo(false);
        }
        if (collision.gameObject.tag == "Enemy")
        {            
            ContactPoint contact = collision.contacts[0];
            //Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;
            TakeDamage takeDamage = collision.transform.GetComponent<BodyPart>().takeDamage;
            takeDamage.Damage(pos);
            TimeManager_1.Instance.ToggleSlowMo(false);
        }
        Destroy(gameObject);
    }
}
