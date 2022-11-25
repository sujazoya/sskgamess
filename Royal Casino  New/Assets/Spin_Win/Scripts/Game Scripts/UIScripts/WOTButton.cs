using UnityEngine;
using System.Collections;

public class WOTButton : SpinButton
{
    public override void Update()
    {
        base.Update();
    }
    public override void Message()
    {
        gameManager.gameMode = GAMEMODES.WHEELOFTRIUMPH;
        gameManager.ChangeState(new GameInputState());
        base.Message();
        Debug.Log("I AM HIT"); 
    }

}