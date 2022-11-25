using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundController : MonoBehaviour
{
    AudioSource myAudioSource;
    [SerializeField] AudioClip[] buttonClips;
    int clipIndex;
    Button[] sceneButtons;
    // Start is called before the first frame update
    void Awake()
    {
        myAudioSource = gameObject.AddComponent<AudioSource>();
        sceneButtons = FindObjectsOfType<Button>();
        if (sceneButtons.Length > 0)
        {
            for (int i = 0; i < sceneButtons.Length; i++)
            {
                sceneButtons[i].onClick.AddListener(PlayButtonClip);
            }
        }
    }
    void PlayButtonClip()
    {
        clipIndex = Random.Range(0, buttonClips.Length);
        myAudioSource.clip = buttonClips[clipIndex];
        myAudioSource.Play();
    }
}
