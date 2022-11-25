using System;
using UnityEngine;

namespace CBGames
{
	public class Utilities
	{
		public static string FloatToMeters(float distance)
		{
			if (distance < 1000f)
			{
				return Mathf.RoundToInt(distance).ToString() + " M";
			}
			float num = distance / 1000f;
			return $"{num:0.00}" + " KM";
		}

		public static string SecondsToTimeFormat(double seconds)
		{
			int num = (int)seconds / 3600;
			int num2 = (int)seconds % 3600 / 60;
			seconds = Math.Round(seconds % 60.0, 0);
			return num + ":" + num2 + ":" + seconds;
		}
	}
}
