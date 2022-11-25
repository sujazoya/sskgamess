using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Heat : MonoBehaviour
{
    [SerializeField] Transform car;
    [SerializeField] float speedX=3f;
    [SerializeField] float speedY = 3f;
    Camera _camera;

    [SerializeField] float minClampV;
    [SerializeField] float maxClampV;
    [SerializeField] float carOffset;
    [SerializeField] Transform colliderPrefab;
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speedX * Time.deltaTime, Input.GetAxis("Vertical")*speedY* Time.deltaTime, 0);
        _camera.transform.position = new Vector3(transform.position.x + 7.2f, 1, _camera.transform.position.z);
        transform.position = new Vector3(transform.position.x ,Mathf.Clamp(transform.position.y, minClampV, maxClampV), transform.position.z);
        car.transform.position = new Vector3(transform.position.x - carOffset, car.transform.position.y, car.transform.position.z);
        Instantiate(colliderPrefab, transform.position, Quaternion.identity);
    }
}
