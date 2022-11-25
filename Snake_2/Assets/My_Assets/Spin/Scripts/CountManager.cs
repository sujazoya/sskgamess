using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CountManager : MonoBehaviour
{
	DateTime dateCurrent;
	string dateString;
	// Start is called before the first frame update
	void Start()
    {
		SaveDate();
		CheckForSpinRenew();
	}

	void CheckForSpinRenew()
	{
		if (NextDate())
		{
			Game.TotalSpinCount = 0;
			Game.TotalScratchCount = 0;
			Game.TotalSpinCount += 10;
			Game.TotalScratchCount += 10;
			Game.DateCount = 0;
		}
	}
	void SaveDate()
	{
		if (Game.DateCount == 0)
		{
			Game.DateCount += 1;
			dateCurrent = DateTime.Now;
			dateString = dateCurrent.ToString(); // and use somewhere, somehow..
			if (!PlayerPrefs.HasKey(dateString))
			{
				PlayerPrefs.SetString(dateString, dateString);
			}
		}
	}
	bool NextDate()
	{
		DateTime dateC = DateTime.Now;
		string dateS = dateC.ToString();
		if (!PlayerPrefs.HasKey(dateS))
		{
			return true;
		}
		else
			return false;
	}
}
