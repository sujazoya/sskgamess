using UnityEngine;
using System.Collections;

public class SceneWinState : FSMState<SceneManagerSpin>
{
    float time = 0f;
    float maxTime = 5f;
    SceneManagerSpin scene;

    public SceneWinState()
    {
    }

    public override void Enter(SceneManagerSpin owner)
    {
        scene = owner;
        scene.GetComponent<AudioSource>().clip = scene.sounds[0];
        scene.GetComponent<AudioSource>().Play(); 
        scene.winLose = 0;
       
    }

    public override void Execute()
    {
        //Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, scene.cameraPos[0].position, Time.deltaTime * scene.timeScale);
        //Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, scene.cameraPos[0].rotation, Time.deltaTime * scene.timeScale); 
        if (time > maxTime)
        {
            time = 0f;
            scene.ChangeState(new SceneLeaveState());
        }
        time += Time.deltaTime;
    }

    public override void Exit()
    {

    }

}
