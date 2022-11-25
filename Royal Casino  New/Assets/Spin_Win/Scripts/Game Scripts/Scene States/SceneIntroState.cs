using UnityEngine;
using System.Collections;

public class SceneIntroState : FSMState<SceneManagerSpin>
{
    private float time = 0f;
    private float maxTime = 0f;
    SceneManagerSpin scene;


    public SceneIntroState(){}

    public override void Enter(SceneManagerSpin owner)
    {
        Debug.Log("Entered Intro State");
        scene = owner;
        scene.spinCounter.text = Game.TotalSpinCount + " Spins Left";
        scene.winLose = -1;
    }

    public override void Execute()
    {

        if (time > maxTime)
        {
            time = 0f;
            scene.ChangeState(new ScenePlayState()); 
        }
        time += Time.deltaTime;
    }

    public override void Exit()
    {
        Debug.Log("Exiting Intro State");
    }

}
