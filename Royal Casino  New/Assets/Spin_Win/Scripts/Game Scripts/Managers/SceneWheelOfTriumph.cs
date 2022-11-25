using UnityEngine;
using System.Collections;

public class SceneWheelOfTriumph : SceneManagerSpin 
{
    public override void Start()
    {
        gameObject.name = "SceneWOT";
        Game.TotalSpinCount = 3;
        minVel = 0.01f;
        base.Start();
    }
   
    public override void Scoring()
    {
        int preScore = pointer.GetScore();
        Debug.Log(preScore + " Points");
        score += preScore;
        if (score == 15 || score == 5)
        {
            spinAgain = true;
            Game.TotalSpinCount = 3; 
            Debug.Log("SpinAgain");
        }
        scoreObj.text = "SCORE: " + score.ToString(); 
    }

    public override void FinalScore()
    {
        spinAgain = false; 
        if (score >= 100)
        {
            ChangeState(new SceneWinState());
            Debug.Log("You Win");
            Game.TotalCoins += score;
            manager.ShowWin(score);
        }
        else
        {
            ChangeState(new SceneLoseState());
            Debug.Log("YOU LOSE");
        }
        base.FinalScore();
    }
}
