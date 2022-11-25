using System;
using UnityEngine;

namespace CBGames
{
	[Serializable]
	public class FadingTextConfig
	{
		[SerializeField]
		private BlockType blockType;

		[SerializeField]
		private Color textColor = Color.white;

		[SerializeField]
		private string[] texts;

		public BlockType BlockType => blockType;

		public Color TextColor => textColor;

		public string[] Texts => texts;
	}
}
