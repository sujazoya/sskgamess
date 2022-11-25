using UnityEngine;
using System.Collections;
/// <summary>
/// Ending state of the Game also resets the game manger. 
/// </summary>
public class GameEndGameState : FSMState<GameManager>
{
    private GameManager manager;

    private float time = 0f;
    private float maxTime = 1.5f;

    public GameEndGameState()
    {
    }

    public override void Enter(GameManager owner)
    {
        manager = owner;
    }

    public override void Execute()
    {
       
        if (time > maxTime)
        {
            time = 0f;
            manager.ChangeState(new GameIdleState()); 
            
        }
        time += Time.deltaTime;
       
    }

    public override void Exit()
    {
       
    }

}
