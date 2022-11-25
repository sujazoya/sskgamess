using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MusicIndivisual : MonoBehaviour
{
    [Header("Every Musib Need Unique Key")]
    public string musicKey;
    public static string currentMusicKey;
    public AudioClip[] music;					// list of available music tracks
    private AudioSource musicAudio;				// AudioSource component for playing music
    [SerializeField] Button[] musicButton;
	[SerializeField] GameObject[] offGameobject;
	public bool playOnStart;
	// Start is called before the first frame update
	void Awake()
	{
		currentMusicKey = musicKey;		
	

		if (!GetComponent<AudioSource>())
		{
			musicAudio = gameObject.AddComponent<AudioSource>();
		}
		else
		{
			musicAudio = GetComponent<AudioSource>();
		}		
		musicAudio.loop = true;
		if (musicButton.Length > 0)
		{
			for (int i = 0; i < musicButton.Length; i++)
			{
				musicButton[i].onClick.AddListener(ToggleMusic);
			}
		}
		if (offGameobject.Length > 0)
		{
			if (!PlayerPrefs.HasKey(musicKey))
			{
				for (int i = 0; i < offGameobject.Length; i++)
				{
					offGameobject[i].SetActive(true);
				}

			}
			else
			{
				for (int i = 0; i < offGameobject.Length; i++)
				{
					offGameobject[i].SetActive(false);
				}
			}
		}
        if (playOnStart)
        {
			PlayMusic();

		}

	}
	void PlayMusic()
    {
		if (!PlayerPrefs.HasKey(musicKey))
		{
			int index = Random.Range(0, music.Length);
			musicAudio.clip = music[index];
			musicAudio.Play();
        }
        else
        {
			musicAudio.Stop();
		}
	}
	public void ToggleMusic()
	{
		if (!PlayerPrefs.HasKey(musicKey))
		{
			PlayerPrefs.SetString(musicKey, musicKey);
			currentMusicKey = musicKey;
			musicAudio.Stop();
			if (offGameobject.Length > 0)
			{
				for (int i = 0; i < offGameobject.Length; i++)
				{
					offGameobject[i].SetActive(true);

				}
			}

		}
		else
		{
			PlayerPrefs.DeleteKey(musicKey);
			currentMusicKey = string.Empty;
			int index = Random.Range(0, music.Length);
			musicAudio.clip = music[index];
			musicAudio.Play();
			if (offGameobject.Length > 0)
			{
				for (int i = 0; i < offGameobject.Length; i++)
				{
					offGameobject[i].SetActive(false);
				}
			}
		}
	}
}
