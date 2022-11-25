using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeholderShooter : MonoBehaviour
{
    [SerializeField] Bullet bullet;
    [SerializeField] Transform _spawnPoint;
    [SerializeField] private float _bulletSpeed = 12;
    [SerializeField] LayerMask _targetLayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Shoot()
    {
        var hitsTarget = Physics.Raycast(_spawnPoint.position, _spawnPoint.forward, float.PositiveInfinity, _targetLayer);
        var bullets = Instantiate(bullet, _spawnPoint.position, _spawnPoint.rotation);
        bullets.Init(_spawnPoint.forward * _bulletSpeed, hitsTarget);
    }
}
