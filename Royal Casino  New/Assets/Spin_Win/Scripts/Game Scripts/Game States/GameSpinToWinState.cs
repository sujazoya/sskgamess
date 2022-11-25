using UnityEngine;
using System.Collections;
/// <summary>
/// This game state is where I assign the specified gameobjects to the scenemanager after its created. 
/// </summary>
public class GameSpinToWinState : FSMState<GameManager>
{
    private SceneSpinToWin scene;

    public GameSpinToWinState(){}

    public override void Enter(GameManager owner)
    {
        Debug.Log("Entered Game STW State");

        scene = (SceneSpinToWin)GameObject.Instantiate(owner.scene);
        scene.wheel = owner.wheelSTW;
        scene.spinner = owner.pointerSTW;
        scene.scoreObj = owner.scoreObj;
        scene.cameraPos[0] = owner.cameraLocation[0];
        scene.cameraPos[1] = owner.cameraLocation[1];
        scene.winLoseobj = owner.winLoseObj;
        scene.spinCounter = owner.spinCounter;
    }

    public override void Execute(){}

    public override void Exit()
    {
        Debug.Log("Exited Game STW State");
    }

}