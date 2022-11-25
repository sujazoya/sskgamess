using UnityEngine;
using System.Collections;

public class STWButton : SpinButton 
{
    public override void Update()
    {
        base.Update();
    }
    public override void Message()
    {
        gameManager.ChangeState(new GameInputState());
        gameManager.gameMode = GAMEMODES.SPINTOWIN;
        base.Message();

    }
	
}
