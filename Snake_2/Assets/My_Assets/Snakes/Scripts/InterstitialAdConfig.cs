using CBGames;
using System;
using System.Collections.Generic;

[Serializable]
internal class InterstitialAdConfig
{
	public IngameState GameStateForShowingAd = IngameState.Ingame_GameOver;

	public int GameStateCountForShowingAd = 3;

	public float ShowAdDelay = 0.2f;

	public List<InterstitialAdType> ListInterstitialAdType = new List<InterstitialAdType>();
}
