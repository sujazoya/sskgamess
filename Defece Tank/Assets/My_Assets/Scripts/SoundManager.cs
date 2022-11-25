using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// A simple sound manager class for handling the playing of sound effects and music
/// </summary>
/// 

public class SoundManager : MonoBehaviour
{
	public AudioClip[] sounds;                  // list of available sound clips
	public AudioClip[] music;                   // list of available music tracks

	private static SoundManager soundMan;       // global SoundManager instance
	private AudioSource sfxAudio;               // AudioSource component for playing sound fx.
	private AudioSource sfxAudio_Other;               // AudioSource component for playing sound fx.
	private AudioSource musicAudio;             // AudioSource component for playing music
	private static string musicKey = "mainmusic";
	public Button[] musicButtons;
	public Sprite musicSprite_On;
	public Sprite musicSprite_Off;
	private static string soundKey = "mainsound";
	public Button[] soundButtons;
	public Sprite soundSprite_On;
	public Sprite soundSprite_Off;

	void Awake()
	{
		if (soundMan != null)
		{
			Debug.LogError("More than one SoundManager found in the scene");
			return;
		}

		soundMan = this;
		sfxAudio = gameObject.AddComponent<AudioSource>();
		sfxAudio_Other = gameObject.AddComponent<AudioSource>();
		musicAudio = gameObject.AddComponent<AudioSource>();
		sfxAudio_Other.playOnAwake = false;
		sfxAudio.playOnAwake = false;
		musicAudio.playOnAwake = false;
		musicAudio.loop = true;
	}
    private void Start()
    {
        if (musicButtons.Length > 0)
        {
			for (int i = 0; i < musicButtons.Length; i++)
			{
				musicButtons[i].onClick.AddListener(ToggleMusic);			

			}
		}
		if (soundButtons.Length > 0)
		{
			for (int i = 0; i < soundButtons.Length; i++)
			{
				soundButtons[i].onClick.AddListener(ToggleSound);

			}
		}
	}
    void ToggleMusic()
    {
        if (PlayerPrefs.HasKey(musicKey))
        {
			PlayerPrefs.DeleteKey(musicKey);
            for (int i = 0; i < musicButtons.Length; i++)
            {
				musicButtons[i].GetComponent<Image>().sprite = musicSprite_On;
				string musicClip = soundMan.musicAudio.clip.name;
				PlayMusic(musicClip);

			}
        }
        else
        {
			for (int i = 0; i < musicButtons.Length; i++)
			{
				PlayerPrefs.SetString(musicKey, musicKey);
				musicButtons[i].GetComponent<Image>().sprite = musicSprite_Off;				
				PauseMusic(0.02f);
			}		
			
		}
    }
	void ToggleSound()
	{
		if (PlayerPrefs.HasKey(soundKey))
		{
			PlayerPrefs.DeleteKey(soundKey);
			for (int i = 0; i < soundButtons.Length; i++)
			{
				soundButtons[i].GetComponent<Image>().sprite = soundSprite_On;				
			}
		}
		else
		{
			for (int i = 0; i < soundButtons.Length; i++)
			{
				PlayerPrefs.SetString(soundKey, soundKey);
				soundButtons[i].GetComponent<Image>().sprite = soundSprite_Off;				
			}

		}
	}
	/// <summary>
	/// Play a sound clip by name
	/// </summary>
	/// <param name="sfxName">The name of the sound to play</param>
	public static void PlaySfx(string sfxName)
	{
		if (!PlayerPrefs.HasKey(soundKey))
		{
			if (soundMan == null)
			{
				Debug.LogWarning("Attempt to play a sound with no SoundManager in the scene");
				return;
			}

			soundMan.PlaySound(sfxName, soundMan.sounds, soundMan.sfxAudio);
		}
	}
	public static void PlaySfx_Other(string sfxName)
	{
		if (!PlayerPrefs.HasKey(soundKey))
		{
			if (soundMan == null)
			{
				Debug.LogWarning("Attempt to play a sound with no SoundManager in the scene");
				return;
			}

			soundMan.PlaySound(sfxName, soundMan.sounds, soundMan.sfxAudio_Other);
		}
	}
	/// <summary>
	/// Plays a given sound clip	
	/// </summary>
	/// <param name="clip">The sound clip to play.</param>
	public static void PlaySfx(AudioClip clip)
	{
		soundMan.PlaySound(clip, soundMan.sfxAudio);
	}

	/// <summary>
	/// Start playing a music track from the beginning
	/// </summary>
	/// <param name="trackName">Track name.</param>
	public static void PlayMusic(string trackName)
	{
		if (!PlayerPrefs.HasKey(musicKey))
		{
			if (soundMan == null)
			{
				Debug.LogWarning("Attempt to play a sound with no SoundManager in the scene");
				return;
			}

			// reset track to beginning
			soundMan.musicAudio.time = 0.0f;
			soundMan.musicAudio.volume = 1.0f;

			soundMan.PlaySound(trackName, soundMan.music, soundMan.musicAudio);
		}
	}

	/// <summary>
	/// Pauses the music.
	/// </summary>
	/// <param name="fadeTime">Fade out time.</param>
	public static void PauseMusic(float fadeTime)
	{
		if (fadeTime > 0.0f)
			soundMan.StartCoroutine(soundMan.FadeMusicOut(fadeTime));
		else
			soundMan.musicAudio.Pause();
	}

	/// <summary>
	/// Unpauses the music.
	/// </summary>
	public static void UnpauseMusic()
	{
		if (!PlayerPrefs.HasKey(musicKey))
		{
			soundMan.musicAudio.volume = 1.0f;
			soundMan.musicAudio.Play();
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
}

