using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Spin_MusicManager : MonoBehaviour
{
    public string key;
    public Button musicButton;
    public GameObject offObject;
    public AudioClip[] musics;
    AudioSource audioSource;
    [Range(0, 1)][SerializeField] float volume = 1;
    int index;
    // Start is called before the first frame update
    void Start()
    {
        index = Random.Range(0, musics.Length);
        if (!audioSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
        PlayMusic();
        if (musicButton)
        {
            musicButton.onClick.AddListener(ToggleMusic);
        }
    }
    public void PlayMusic()
    {
        if (!PlayerPrefs.HasKey(key))
        {            
            audioSource.clip = musics[index];
            audioSource.Play();
            if (offObject) { offObject.SetActive(false); }
        }
    }
    public void StopMusic()
    { 
            audioSource.Stop();        
    }
    public void ToggleMusic()
    {
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetString(key, key);
            audioSource.Stop();
            if (offObject) { offObject.SetActive(true); }
        }
        else
        {
            PlayerPrefs.DeleteKey(key);
            audioSource.clip = musics[index];
            audioSource.Play();
            if (offObject) { offObject.SetActive(false); }
        }
    }
}
