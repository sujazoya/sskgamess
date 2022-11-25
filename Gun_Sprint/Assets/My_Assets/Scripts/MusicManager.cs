using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    [Header("Every Musib Need Unique Key")]
	public  string musicKey;	
	public static string currentMusicKey;	
	public AudioClip[] music;					// list of available music tracks
    private AudioSource musicAudio;				// AudioSource component for playing music
    [SerializeField] Button[] musicButton;	
	public static MusicManager musicManager;
	[SerializeField] GameObject[] offGameobject;
	[Header("Sounds")]
	public string soundKey;
	public static string currentSoundKey;
	public AudioClip[] sounds;                  // list of available sound clips
	[SerializeField] Button[] soundButton;
	[SerializeField] GameObject[] soundOffGameobject;
	public static AudioSource sfxAudio;               // AudioSource component for playing sound fx.
	public static AudioSource sfxAudio_other;               // AudioSource component for playing sound fx.

	void Awake()
    {
		currentMusicKey = musicKey;
		currentSoundKey = soundKey;
		if (musicManager != null)
		{
			Debug.LogError("More than one SoundManager found in the scene");
			return;
		}
		musicManager = this;

		sfxAudio = gameObject.AddComponent<AudioSource>();
		musicAudio = gameObject.AddComponent<AudioSource>();
		sfxAudio_other = gameObject.AddComponent<AudioSource>();
		sfxAudio.playOnAwake = false;
		musicAudio.playOnAwake = false;
		musicAudio.loop = true;

		musicAudio.playOnAwake = false;
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
		if (soundButton.Length > 0)
		{
			for (int i = 0; i < soundButton.Length; i++)
			{
				soundButton[i].onClick.AddListener(ToggleSound);
			}
		}
		if (soundOffGameobject.Length > 0)
		{
			if (!PlayerPrefs.HasKey(soundKey))
			{
				for (int i = 0; i < soundOffGameobject.Length; i++)
				{
					soundOffGameobject[i].SetActive(true);
				}

			}
			else
			{
				for (int i = 0; i < soundOffGameobject.Length; i++)
				{
					soundOffGameobject[i].SetActive(false);
				}
			}
		}
	}
	public void ToggleMusic(AudioSource musicName)
	{
		if (!PlayerPrefs.HasKey(musicKey))
		{
			PlayerPrefs.SetString(musicKey, musicKey);
			currentMusicKey = musicKey;
			musicName.Stop();
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
			musicName.Play();
			if (offGameobject.Length > 0)
			{
				for (int i = 0; i < offGameobject.Length; i++)
				{
					offGameobject[i].SetActive(false);
				}
			}
		}
	}
	void ToggleMusic()
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
	void ToggleSound()
	{
		if (!PlayerPrefs.HasKey(soundKey))
		{
			PlayerPrefs.SetString(soundKey, soundKey);
			currentSoundKey = soundKey;
			//sfxAudio.Stop();
			if (soundOffGameobject.Length > 0)
			{
				for (int i = 0; i < soundOffGameobject.Length; i++)
				{
					soundOffGameobject[i].SetActive(true);

				}
			}

		}
		else
		{
			PlayerPrefs.DeleteKey(soundKey);
			currentSoundKey = string.Empty;
			//sfxAudio.Play();
			if (soundOffGameobject.Length > 0)
			{
				for (int i = 0; i < soundOffGameobject.Length; i++)
				{
					soundOffGameobject[i].SetActive(false);
				}
			}
		}
	}
	public static IEnumerator PlayMusic(string trackName,float wait)
    {
		yield return new WaitForSeconds(wait);
		PlayMusic(trackName);
	}
	public static void PlayMusic(string trackName)
    {
        if (musicManager == null)
        {
            Debug.LogWarning("Attempt to play a sound with no SoundManager in the scene");
            return;
        }

        if (!PlayerPrefs.HasKey(currentMusicKey))
        {
			musicManager.musicAudio.time = 0.0f;
			musicManager.musicAudio.volume = 1.0f;

			musicManager.PlaySound(trackName, musicManager.music, musicManager.musicAudio);
		}
        // reset track to beginning
      
    }
	/// <summary>
	/// Pauses the music.
	/// </summary>
	/// <param name="fadeTime">Fade out time.</param>
	public static void PauseMusic(float fadeTime)
	{
		if (!PlayerPrefs.HasKey(currentMusicKey))
		{
			if (fadeTime > 0.0f)
				musicManager.StartCoroutine(musicManager.FadeMusicOut(fadeTime));
			else
				musicManager.musicAudio.Pause();
		}
		
	}

	/// <summary>
	/// Unpauses the music.
	/// </summary>
	public static void UnpauseMusic()
	{
		if (!PlayerPrefs.HasKey(currentMusicKey))
		{
			musicManager.musicAudio.volume = 1.0f;
			musicManager.musicAudio.Play();
		}
		
	}

	/// <summary>
	/// Plays a sound using a given AudioSource
	/// </summary>
	private void PlaySound(string soundName, AudioClip[] pool, AudioSource audioOut)
	{
		// loop through our list of clips until we find the right one.
		foreach (AudioClip clip in pool)
		{
			if (clip.name == soundName)
			{
				PlaySound(clip, audioOut);
				return;
			}
		}

		Debug.LogWarning("No sound clip found with name " + soundName);
	}

	/// <summary>
	/// Plays a sound using a given AudioSource
	/// </summary>
	private void PlaySound(AudioClip clip, AudioSource audioOut)
	{
		audioOut.clip = clip;
		audioOut.Play();
	}

	/// <summary>
	/// Co-Routine for fading out the music
	/// </summary>
	/// <param name="time">Fade time</param>
	IEnumerator FadeMusicOut(float time)
	{
		float startVol = musicAudio.volume;
		float startTime = Time.realtimeSinceStartup;

		while (true)
		{
			// use realtimeSinceStartup because Time.time doesn't increase when the game is paused.
			float t = (Time.realtimeSinceStartup - startTime) / time;
			if (t < 1.0f)
			{
				musicAudio.volume = (1.0f - t) * startVol;
				yield return 0;
			}
			else
			{
				break;
			}
		}

		// once we've fully faded out, pause the track
		musicAudio.Pause();
	}
	public static void PlaySfx(string sfxName)
	{
        if (!PlayerPrefs.HasKey(currentSoundKey))
        {
			if (musicManager == null)
			{
				Debug.LogWarning("Attempt to play a sound with no SoundManager in the scene");
				return;
			}

			musicManager.PlaySound(sfxName, musicManager.sounds, sfxAudio);
		}
		
	}
	public static void PlaySfx_Other(string sfxName)
	{
		if (!PlayerPrefs.HasKey(currentSoundKey))
		{
			if (musicManager == null)
			{
				Debug.LogWarning("Attempt to play a sound with no SoundManager in the scene");
				return;
			}

			musicManager.PlaySound(sfxName, musicManager.sounds, sfxAudio_other);
		}

	}
	public static IEnumerator WaitAndPlaySfx_Other(string sfxName,float wait)
	{
		yield return new WaitForSeconds(wait);
		if (!PlayerPrefs.HasKey(currentSoundKey))
		{
			if (musicManager == null)
			{
				Debug.LogWarning("Attempt to play a sound with no SoundManager in the scene");
			 yield	return  null;
			}

			musicManager.PlaySound(sfxName, musicManager.sounds, sfxAudio_other);
		}

	}
}
