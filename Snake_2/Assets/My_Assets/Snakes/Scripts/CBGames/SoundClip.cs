using System;
using UnityEngine;

namespace CBGames
{
	[Serializable]
	public class SoundClip
	{
		[SerializeField]
		private AudioClip audioClip;

		public AudioClip AudioClip => audioClip;
	}
}
