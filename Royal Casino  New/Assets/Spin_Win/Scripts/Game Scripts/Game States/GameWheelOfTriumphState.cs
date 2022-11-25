using UnityEngine;
using System.Collections;
/// <summary>
/// This game state is where i assign the specified gameobjects to the scenemanager after its created. 
/// </summary>
public class GameWheelOfTriumphState : FSMState<GameManager>
{
    SceneWheelOfTriumph scene;

    public GameWheelOfTriumphState(){}

    public override void Enter(GameManager owner)
    {
        scene = (SceneWheelOfTriumph)GameObject.Instantiate(owner.scene);
        scene.wheel = owner.wheelWOT;
        scene.pointer = owner.pointerWOT;
        scene.scoreObj = owner.scoreObj;
        scene.cameraPos[0] = owner.cameraLocation[0];
        scene.cameraPos[1] = owner.cameraLocation[1];
        scene.winLoseobj = owner.winLoseObj;
        scene.spinCounter = owner.spinCounter;
    }

    public override void Execute(){}

    public override void Exit(){}

}
