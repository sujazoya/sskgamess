using System;
using System.Collections.Generic;
using UnityEngine;

namespace CBGames
{
	[Serializable]
	public class EnvironmentPack
	{
		[SerializeField]
		private EnvironmentType environmentType;

		[SerializeField]
		private GameObject groundPrefab;

		[SerializeField]
		private List<ObstacleController> listObstaclePrefab;

		[SerializeField]
		private List<GameObject> listLeftWallPrefab;

		[SerializeField]
		private List<GameObject> listRightWallPrefab;

		public EnvironmentType EnvironmentType => environmentType;

		public GameObject GroundPrefab => groundPrefab;

		public List<ObstacleController> ListObstaclePrefab => listObstaclePrefab;

		public List<GameObject> ListLeftWallPrefab => listLeftWallPrefab;

		public List<GameObject> ListRightWallPrefab => listRightWallPrefab;
	}
}
