using System;
using UnityEngine;

namespace CBGames
{
	[Serializable]
	public class BlockConfig
	{
		[SerializeField]
		private int minScoreToCreate;

		[SerializeField]
		private BlockType blockType;

		[SerializeField]
		private int minBlockNumber = 1;

		[SerializeField]
		private int maxBlockNumber = 3;

		public int MinScoreToCreate => minScoreToCreate;

		public BlockType BlockType => blockType;

		public int MinBlockNumber => minBlockNumber;

		public int MaxBlockNumber => maxBlockNumber;
	}
}
