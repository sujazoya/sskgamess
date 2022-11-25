using System.Collections;
using UnityEngine;

namespace CBGames
{
	public class SoundManager : MonoBehaviour
	{
		private const string Sound_PPK = "SOUND_KEY";

		private const string Music_PPK = "MUSIC_KEY";

		[Header("Audio Source References")]
		[SerializeField]
		private AudioSource soundSource;

		[SerializeField]
		private AudioSource musicSource;

		[Header("Audio Clips")]
		public SoundClip background_Forest;

		public SoundClip background_Desert;

		public SoundClip background_Cemetery;

		public SoundClip background_Snowy;

		public SoundClip button;

		public SoundClip collectBody;

		public SoundClip collectCoin;

		public SoundClip enableShield;

		public SoundClip enableMagnetMode;

		public SoundClip destroyBlock;

		public SoundClip destroyBody;

		public SoundClip playerDead;

		public SoundClip rewardCoinCount;

		public SoundClip rewarded;

		public SoundClip tick;

		public SoundClip unlock;

		private void Start()
		{
			if (!PlayerPrefs.HasKey("SOUND_KEY"))
			{
				PlayerPrefs.SetInt("SOUND_KEY", 1);
			}
			if (!PlayerPrefs.HasKey("MUSIC_KEY"))
			{
				PlayerPrefs.SetInt("MUSIC_KEY", 1);
			}
		}

		public bool IsSoundOff()
		{
			return PlayerPrefs.GetInt("SOUND_KEY", 1) == 0;
		}

		public bool IsMusicOff()
		{
			return PlayerPrefs.GetInt("MUSIC_KEY", 1) == 0;
		}

		public void PlayOneSound(SoundClip clip)
		{
			soundSource.PlayOneShot(clip.AudioClip);
		}

		public void PlayMusic(SoundClip clip, float volumeUpTime)
		{
			if (!IsMusicOff())
			{
				musicSource.clip = clip.AudioClip;
				musicSource.loop = true;
				musicSource.volume = 0f;
				musicSource.Play();
				StartCoroutine(CRVolumeUp(volumeUpTime));
			}
		}

		public void PauseMusic()
		{
			musicSource.Pause();
		}

		public void ResumeMusic()
		{
			musicSource.UnPause();
		}

		public void StopMusic(float volumeDownTime)
		{
			musicSource.Stop();
			StartCoroutine(CRVolumeDown(volumeDownTime));
		}

		public void ToggleSound()
		{
			if (IsSoundOff())
			{
				PlayerPrefs.SetInt("SOUND_KEY", 1);
				soundSource.mute = false;
			}
			else
			{
				PlayerPrefs.SetInt("SOUND_KEY", 0);
				soundSource.mute = true;
			}
		}

		public void ToggleMusic()
		{
			if (IsMusicOff())
			{
				PlayerPrefs.SetInt("MUSIC_KEY", 1);
				ResumeMusic();
			}
			else
			{
				PlayerPrefs.SetInt("MUSIC_KEY", 0);
				PauseMusic();
			}
		}

		private IEnumerator CRVolumeUp(float time)
		{
			float t = 0f;
			while (t < time)
			{
				t += Time.deltaTime;
				float t2 = t / time;
				musicSource.volume = Mathf.Lerp(0f, 1f, t2);
				yield return null;
			}
		}

		private IEnumerator CRVolumeDown(float time)
		{
			float t = 0f;
			while (t < time)
			{
				t += Time.deltaTime;
				float t2 = t / time;
				musicSource.volume = Mathf.Lerp(1f, 0f, t2);
				yield return null;
			}
		}
	}
}
