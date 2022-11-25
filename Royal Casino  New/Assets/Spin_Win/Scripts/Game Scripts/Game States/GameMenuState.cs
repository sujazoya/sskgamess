using UnityEngine;
using System.Collections;

public class GameMenuState : FSMState<GameManager>
{
    GameManager manager;

    public GameMenuState() { }

    public override void Enter(GameManager owner)
    {
        Debug.Log("Entered Game Menu State");

        manager = owner;
        manager.SceneLoaded();
    }

    public override void Execute(){}

    public override void Exit()
    {
        Debug.Log("Exited Game Menu State");
    }
}
