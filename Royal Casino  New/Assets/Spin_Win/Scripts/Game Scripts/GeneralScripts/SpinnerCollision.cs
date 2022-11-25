using UnityEngine;
using System.Collections;
/// <summary>
/// Simple script for scoring.. 
/// </summary>
/// 
[RequireComponent(typeof(AudioSource))]
public class SpinnerCollision : MonoBehaviour
{
    public bool playSoundOnTriggerEnter = false;
    public bool showDebug = false;

    [HideInInspector]
    public int ptsVal;
    [SerializeField] private AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        ptsVal = other.gameObject.GetComponent<PointValue>().pointVal;

        if (showDebug)
        {
            Debug.Log("Collided with " + ptsVal + " points.");
        }

        if (playSoundOnTriggerEnter)
        {
            audioSource.Play();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        audioSource.Play();
        Debug.Log("click..........");
    }
}
