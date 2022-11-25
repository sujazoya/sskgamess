using UnityEngine;
using System.Collections;

public class SceneSpinToWin : SceneManagerSpin 
{
    public override void Start()
    {
        gameObject.name = "SceneSTW";
        Game.TotalSpinCount = 3;
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Scoring()
    {
        int preScore = spinner.ptsVal;

        Debug.Log(preScore + " Points");

        if (preScore == -1)
        {
            score = 0;
        }
        else
        {
            score += preScore;
        }

        scoreObj.text = "SCORE: " + score.ToString();
        MusicManager.UnpauseMusic();
        MusicManager.PlaySfx("start");
        manager.ShowResult(score);
    }

    public override void FinalScore()
    {
        if (score >= 1800)
        {
            ChangeState(new SceneWinState());
            Debug.Log("YOU WIN");
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
