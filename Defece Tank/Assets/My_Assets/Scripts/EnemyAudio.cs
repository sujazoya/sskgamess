using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour {

    private AudioSource audioSource;

    [SerializeField]
    private AudioClip scream_Clip, die_Clip;

    [SerializeField]
    private AudioClip[] attack_Clips;

    public AudioSource randomSound;

    public AudioClip[] idleClips;


    // Use this for initialization
    void Awake () {
        audioSource =gameObject. AddComponent<AudioSource>();
        randomSound = gameObject.AddComponent<AudioSource>();

    }
  
    // Use this for initialization
    void Start()
    {

        CallAudio();
    }


    void CallAudio()
    {
        Invoke("RandomSoundness", 10);
    }

    //void RandomSoundness()
    //{
    //   // randomSound.clip = idleClips[Random.Range(0, idleClips.Length)];
    //   // AudioClip chosenClip = idleClips[Random.Range(0, idleClips.Length)];
    //    int index = Random.Range(0, idleClips.Length);
    //    // randomSound.Play();
    //    // randomSound.PlayClipAtPoint(idleClips[index], transform.position,0.9f,Space.World);
    //    AudioSource.PlayClipAtPoint(idleClips[index], transform.position, 0.9f);
    //    CallAudio();
        
    //}
    public void Play_ScreamSound() {
        audioSource.clip = scream_Clip;
        audioSource.Play();
    }

    public void Play_AttackSound() {
        audioSource.clip = attack_Clips[Random.Range(0, attack_Clips.Length)];
        audioSource.Play();
    }

    public void Play_DeadSound() {
        audioSource.clip = die_Clip;
        audioSource.Play();
    }

} // class


































