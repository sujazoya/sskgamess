/*	Globals should be kept to a minimum
*	use them in to indicate things such as enums of global gamestates
*	and lists of broadcast messages */

// Global broadcast signatures
public enum SIG
{
    GAMESTATETRIGGERED,
    SCENESTATETRIGGERED,
    SETPLAYING,
    BROADCASTSTATE
}

public enum GAMESTATES
{
    IDLE,
    MENU,
    INPUT,
    PLAY_WHEELOFFORTUNE,
    PLAY_PRICEISRIGHT,
    ENDGAME
}

public enum SCENESTATES
{
    INTRO,
    PLAY,
    LOSE,
    WIN,
    LEAVE
}

public enum GAMEMODES
{
    NONE,
    SPINTOWIN,
    WHEELOFTRIUMPH,
    EXAMPLESCENE
}