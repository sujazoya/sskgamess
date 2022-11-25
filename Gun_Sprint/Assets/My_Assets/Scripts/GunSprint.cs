using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GunSprint : MonoBehaviour
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private ParticleSystem _smokeSystem;
    [SerializeField] private LayerMask _targetLayer;

    [SerializeField] private float _bulletSpeed = 12;
    [SerializeField] private float _torque = 120;
    [SerializeField] private float _maxTorqueBonus = 150;

    [SerializeField] private float _maxAngularVelocity = 10;

    [SerializeField] private float _forceAmount = 600;
    [SerializeField] private float _maxUpAssist = 30;

    [SerializeField] private float _smokeLength = 0.5f;

    [SerializeField] private float _maxY = 10;

    private Rigidbody _rb;
    private float _lastFired;
    private bool _fire;
    [SerializeField] AudioSource fireSound;
    [HideInInspector] public Vector3 gunStartPos;
    [HideInInspector] public Quaternion gunStartRot;
    private void Awake() => _rb = GetComponent<Rigidbody>();
    [SerializeField] GameObject[] gunModels;
    public GameObject smoke;
    [HideInInspector] public Gun_Model gun_Model;
    AudioClip clipToPlay;
    private void Start()
    {
        StoreGunPos();
        smoke.SetActive(false);
       GameManager_Sprint.onGameStarts += OnGameStart;

        //CreateLevel(0);
    }
    void OnGameStart()
    {            
        StartCoroutine(PlayGun());
    }
    IEnumerator PlayGun()
    {
        smoke.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < gunModels.Length; i++)
        {
            gunModels[i].SetActive(false);
        }
        gunModels[Game.CurrentGun].SetActive(true);
        gun_Model = gunModels[Game.CurrentGun].GetComponent<Gun_Model>();
        clipToPlay = gun_Model.shootClip;
        fireSound.clip = clipToPlay;
    }
    private void OnDestroy()
    {
        GameManager_Sprint.onGameStarts -= OnGameStart;
    }
    void StoreGunPos()
    {
        gunStartPos =new Vector3( transform.position.x,-0.12f, transform.position.z);
        gunStartRot = transform.rotation;
    }
    void Update()
    {
        // Clamp max velocity
        _rb.angularVelocity = new Vector3(0, 0, Mathf.Clamp(_rb.angularVelocity.z, -_maxAngularVelocity, _maxAngularVelocity));

        if (Input.GetMouseButtonDown(0)&&Game.gameStatus==Game.GameStatus.isPlaying)
        {
            switch (gun_Model.gunType)
            {
                case Gun_Model.GunType.Normal:
                    StartCoroutine(TripleShoot(1));
                    break;
                case Gun_Model.GunType.SemiAuto:
                    StartCoroutine(TripleShoot(2));
                    break;
                case Gun_Model.GunType.Auto:
                    StartCoroutine(TripleShoot(3));
                    break;
            }
            //Shoot();
           
        }

        if (_smokeSystem.isPlaying && _lastFired + _smokeLength < Time.time) _smokeSystem.Stop();
        if (transform.position.y < -10&&!reStarded&& Game.gameStatus == Game.GameStatus.isPlaying)
        {
            reStarded = true;
            GameManager_Sprint.Instance.ShowWarn();
            //Restart();
        }
    }
    IEnumerator TripleShoot(int shootCount)
    {
        for (int i = 0; i < shootCount; i++)
        {
            Shoot();
            yield return new WaitForSeconds(0.2f);
        }

    }
    void Shoot()
    {
        // Check if on target
        var hitsTarget = Physics.Raycast(_spawnPoint.position, _spawnPoint.forward, float.PositiveInfinity, _targetLayer);
        if (hitsTarget) TimeManager_1.Instance.ToggleSlowMo(true);

        // Spawn
        var bullet = Instantiate(_bulletPrefab, _spawnPoint.position, _spawnPoint.rotation);
        bullet.Init(_spawnPoint.forward * _bulletSpeed, hitsTarget);
        _smokeSystem.Play();
        _lastFired = Time.time;

        // Apply force - More up assist depending on y position
        var assistPoint = Mathf.InverseLerp(0, _maxY, _rb.position.y);
        var assistAmount = Mathf.Lerp(_maxUpAssist, 0, assistPoint);
        var forceDir = -transform.forward * _forceAmount + Vector3.up * assistAmount;
        if (_rb.position.y > _maxY) forceDir.y = Mathf.Min(0, forceDir.y);
        _rb.AddForce(forceDir);

        // Determine the additional torque to apply when swapping direction
        var angularPoint = Mathf.InverseLerp(0, _maxAngularVelocity, Mathf.Abs(_rb.angularVelocity.z));
        var amount = Mathf.Lerp(0, _maxTorqueBonus, angularPoint);
        var torque = _torque + amount;

        // Apply torque
        var dir = Vector3.Dot(_spawnPoint.forward, Vector3.right) < 0 ? Vector3.back : Vector3.forward;
        _rb.AddTorque(dir * torque);
        if (fireSound) {            
            fireSound.Play(); 
        }
    }
    bool reStarded;
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Replay()
    {
        transform.position = gunStartPos;
        transform.rotation = gunStartRot;
        reStarded = false;
    }
}


