using UnityEngine;
using System.Collections;

public class GameIdleState : FSMState<GameManager>
{
    GameManager manager;
    float time = 0f;
    float maxTime = 0f;

    public GameIdleState(){}

    public override void Enter(GameManager owner)
    {
        Debug.Log("Entered Game Idle State");
        manager = owner;
    }

    public override void Execute()
    {
        if (time > maxTime)
        {
            manager.ChangeState(new GameMenuState());
            time = 0f;
        }
        time += Time.deltaTime; 
    }

    public override void Exit()
    {
        Debug.Log("Exited Game Idle State");
    }
}
