using CBGames;
using UnityEngine;

public class IngameViewController : MonoBehaviour
{
	[SerializeField]
	private PlayingViewController playingViewControl;

	[SerializeField]
	private ReviveViewController reviveViewControl;

	[SerializeField]
	private GameOverViewController gameOverViewControl;

	public PlayingViewController PlayingViewController => playingViewControl;

	public void OnShow()
	{
		IngameManager.GameStateChanged += GameManager_GameStateChanged;
	}

	private void OnDisable()
	{
		IngameManager.GameStateChanged -= GameManager_GameStateChanged;
	}

	private void GameManager_GameStateChanged(IngameState obj)
	{
		switch (obj)
		{
		case IngameState.Ingame_Revive:
			reviveViewControl.gameObject.SetActive(value: true);
			reviveViewControl.OnShow();
			playingViewControl.gameObject.SetActive(value: false);
			gameOverViewControl.gameObject.SetActive(value: false);
			break;
		case IngameState.Ingame_GameOver:
			gameOverViewControl.gameObject.SetActive(value: true);
			gameOverViewControl.OnShow();
			reviveViewControl.gameObject.SetActive(value: false);
			playingViewControl.gameObject.SetActive(value: false);
			break;
		case IngameState.Ingame_Playing:
			playingViewControl.gameObject.SetActive(value: true);
			playingViewControl.OnShow();
			reviveViewControl.gameObject.SetActive(value: false);
			gameOverViewControl.gameObject.SetActive(value: false);
			break;
		}
	}
}
