using UnityEngine;
using System.Collections;

public class ScenePlayState : FSMState<SceneManagerSpin>
{
    SceneManagerSpin scene;

    // Timer until spinning is active
    float time = 0f;
    float maxTime = 0f;

    // Timer until score is decided
    float spinTime = 0f;
    float spinMaxTime = 0.25f;

    bool canSpin = false;
    bool isInSpinningMode = false;
    bool timerExpired = false;

	private ApplyForce force;

    public ScenePlayState(){}

    public override void Enter(SceneManagerSpin owner)
    {
        Debug.Log("Entered Play State");
        scene = owner;

        if (Game.TotalSpinCount == 1)
        {
            scene.spinCounter.text = Game.TotalSpinCount + " Spin Left";
        }
        else
        {
            scene.spinCounter.text = Game.TotalSpinCount + " Spins Left";
        }

        force = scene.wheel.GetComponent<ApplyForce>();
    }

    public override void Execute()
    {
        Spin();
        SpinCounter(); 

        // Wait to allow spinning
        if (time > maxTime && !timerExpired)
        {
            time = 0f;
            timerExpired = true;
            Debug.Log("NOW YOU CAN SPIN!");
            canSpin = true;
            force.arrow.SetActive(true);
            MusicManager.UnpauseMusic();
            AdmobAdmanager.Instance.ShowInterstitial();
            // FacebookAdManager.Instance.ShowInterstitial();
        }
        
        time += Time.deltaTime;
    }

    public override void Exit()
    {
        Debug.Log("Exiting Play State");
        if (force)
        {
            isInSpinningMode = false;
            force.spinCount = 0;
            force.enabled = false;
        }
    }

    private void Spin()
    {
        if (canSpin && !isInSpinningMode)
        {
            if (force)
            {
                force.enabled = true;
                isInSpinningMode = true;
            }
        }

        //Allow player to Spin
        if (isInSpinningMode && force && force.isSpinning && SpinStop())
        { 
            force.isSpinning = false;

            if (force.hasSpun == true)
            {
                scene.Scoring();

                if (scene.spinAgain)
                {
                    //force.Reset();
                }
 
                Debug.Log(force.spinCount);

                if (force.spinCount >= Game.TotalSpinCount && !scene.spinAgain)
                {
                    scene.FinalScore();
                    force.Reset();
                }
                else
                {
                    force.arrow.SetActive(true);
                    MusicManager.UnpauseMusic();
                    AdmobAdmanager.Instance.ShowInterstitial();
                    // FacebookAdManager.Instance.ShowInterstitial();
                }


                force.hasSpun = false;
            }
            //AdmobAdmanager.Instance.ShowInterstitial();
        }
    }
    private void SpinCounter()
    {
        int spincnt = Game.TotalSpinCount - force.spinCount;

        if (spincnt == 1)
        {
            scene.spinCounter.text = spincnt.ToString() + " Spin Left";
        }
        else
        {
            scene.spinCounter.text = spincnt.ToString() + " Spins Left";
        }

        if (scene.spinAgain)
        {
            scene.spinCounter.text =   " Bonus Spin ";
            scene.spinAgain = false; 
        } 
    }
    private bool SpinStop()
    {

        if (scene.wheel.GetComponent<Rigidbody>().angularVelocity.magnitude <= scene.minVel)
        {
            if (spinTime > spinMaxTime)
            {
                spinTime = 0;
                return true;
            }
            
            spinTime += Time.deltaTime;
            
        }
           
        return false;
    }
    

}
                        