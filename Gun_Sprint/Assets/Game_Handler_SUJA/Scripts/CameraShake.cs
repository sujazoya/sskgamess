using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

   
    public float shakeLength = 5;
    public float shakeTimer;
    public float shakeAmount = 3;
    public float shakeSpeed = 20;
    public bool isShaking = false;
    public bool shakeOnce = false;
    Vector3 originalPos;
    Vector3 newPos;

    void Awake()
    {
        shakeTimer = shakeLength;
    }

    void OnEnable()
    {
        originalPos = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !isShaking)
        {
            shakeOnce = true;
            shakeTimer = shakeLength;
            newPos = transform.position;
        }

        if (shakeOnce)
        {
            Shake();
        }
    }

    public void Shake()
    {
        if (shakeTimer > 0)
        {
            isShaking = true;

            if (Vector3.Distance(newPos, transform.position) <= shakeAmount / 30) { newPos = originalPos + Random.insideUnitSphere * shakeAmount; }

            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * shakeSpeed);

            shakeTimer -= Time.deltaTime;
        }
        else
        {
            shakeTimer = 0f;
            transform.position = originalPos;
            isShaking = false;
            shakeOnce = false;
        }
    }
}
