using UnityEngine;
public class Game
{
	public enum GameStatus
    {
		isInMenu,
		isGameover,
		isPlaying,
		isPaused,
		isGameWin
	}
	public static GameStatus gameStatus;
	public static bool isGameover = false;
	public static bool isMoving = true;
	public static bool playerIdDead;
    public static string itemTag = "Object";
    public static string powerupTag = "Powerup";
	public static string blastTag = "Obstacle";
	public static string diemondTag = "Diemond";
	public static string coinTag = "Coin";
	public static string shop = "Shop";

	public static string snakeTag = "Snake";
	public static string foodTag = "food";
	public static string poisonTag = "poison";
	public static string SnakeBody = "SnakeBody";

	public static string levelKey = "levelKey";
	public static string Menu = "Menu";
	public static string SettingPanel = "SettingPanel";
	public static string Level = "Level";
	public static string HUD = "HUD";
	public static string Gameover = "Gameover";
	public static string GameWin = "GameWin";
	public static string Pause = "Pause";
	public static string enemyTag = "Enemy";
	public static string playerTag = "Player";
	public static string Warn_Panel = "Warn-Panel";

	public static string musicKey = "mainMusic";
	public static string soundKey = "mainSound";
	public static string vibrationKey = "mainVibration";

	public static int coinToGive=10;
	public static int diemondToGive = 3;
	public static int lifeToGive = 1;
	public static int currentScore;

	public static int pistolBulletPrice=100;
	public static int akBulletPrice = 300;
	public static int rifleBulletPrice = 300;
	public static int lifePrice = 500;

	public static int currentLevelTarget;
	public static int achivedLevelTarget;

	public static float levelMinTime;
	public static float levelMaxTime;
	public static float levelPlayTime;
	public static int foodCountToEat;
	public static int foodEaten;
	public static string playfabID;
	public static int BonusTime
	{
		get { return PlayerPrefs.GetInt("BonusTime", 0); }
		set { PlayerPrefs.SetInt("BonusTime", value); }
	}
	public static int TotalSpinCount
	{
		get { return PlayerPrefs.GetInt("TotalSpinCount", 0); }
		set { PlayerPrefs.SetInt("TotalSpinCount", value); }
	}
	public static int TotalScratchCount
	{
		get { return PlayerPrefs.GetInt("TotalScratchCount", 0); }
		set { PlayerPrefs.SetInt("TotalScratchCount", value); }
	}
	public static int DateCount
	{
		get { return PlayerPrefs.GetInt("DateCount", 0); }
		set { PlayerPrefs.SetInt("DateCount", value); }
	}

	public static int GameMode
	{
		get { return PlayerPrefs.GetInt("GameMode", 0); }
		set { PlayerPrefs.SetInt("GameMode", value); }
	}

	public static int retryCount
	{
		get { return PlayerPrefs.GetInt("retryCount", 0); }
		set { PlayerPrefs.SetInt("retryCount", value); }
	}
	public static int TotalCoins
	{
		get { return PlayerPrefs.GetInt("TotalCoins", 0); }
		set { PlayerPrefs.SetInt("TotalCoins", value); }
	}
	public static int TotalDiemonds
	{
		get { return PlayerPrefs.GetInt("TotalDiemonds", 0); }
		set { PlayerPrefs.SetInt("TotalDiemonds", value); }
	}
	public static int CurrentLevel
	{
		get { return PlayerPrefs.GetInt("CurrentLevel", 0); }
		set { PlayerPrefs.SetInt("CurrentLevel", value); }
	}
	public static int HighScore
	{
		get { return PlayerPrefs.GetInt("HighScore", 0); }
		set { PlayerPrefs.SetInt("HighScore", value); }
	}
    public static int Life
    {
        get { return PlayerPrefs.GetInt("Life", 0); }
        set { PlayerPrefs.SetInt("Life", value); }
    }
	public static int PistolBullet
	{
		get { return PlayerPrefs.GetInt("PistolBullet", 0); }
		set { PlayerPrefs.SetInt("PistolBullet", value); }
	}
	public static int AKBullet
	{
		get { return PlayerPrefs.GetInt("AKBullet", 0); }
		set { PlayerPrefs.SetInt("AKBullet", value); }
	}
	public static int RifleBullet
	{
		get { return PlayerPrefs.GetInt("RifleBullet", 0); }
		set { PlayerPrefs.SetInt("RifleBullet", value); }
	}
	public static int EasyStars
	{
		get { return PlayerPrefs.GetInt("EasyStars", 0); }
		set { PlayerPrefs.SetInt("EasyStars", value); }
	}
	public static int MediuStars
	{
		get { return PlayerPrefs.GetInt("MediuStars", 0); }
		set { PlayerPrefs.SetInt("MediuStars", value); }
	}
	public static int HardStars
	{
		get { return PlayerPrefs.GetInt("HardStars", 0); }
		set { PlayerPrefs.SetInt("HardStars", value); }
	}
	public static int ExpertStars
	{
		get { return PlayerPrefs.GetInt("ExpertStars", 0); }
		set { PlayerPrefs.SetInt("ExpertStars", value); }
	}
}
