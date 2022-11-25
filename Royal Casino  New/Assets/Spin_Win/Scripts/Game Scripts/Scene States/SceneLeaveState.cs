using UnityEngine;
using System.Collections;

public class SceneLeaveState : FSMState<SceneManagerSpin>
{
    SceneManagerSpin scene;

    public SceneLeaveState(){}

    public override void Enter(SceneManagerSpin owner)
    {
        scene = owner;
        scene.Reset(); 
		Messenger.Broadcast<FSMState<GameManager>>(SIG.BROADCASTSTATE.ToString(), new GameEndGameState());
    }

    public override void Execute(){}

    public override void Exit(){}
   
}
