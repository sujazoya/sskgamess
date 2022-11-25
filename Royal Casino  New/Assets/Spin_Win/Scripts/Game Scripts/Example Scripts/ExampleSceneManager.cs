using UnityEngine;
using System.Collections;

public class ExampleSceneManager : SceneManagerSpin {

    public override void Start()
    {
        gameObject.name = "SceneExample";
        Game.TotalSpinCount = 2;
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
    }

    public override void FinalScore()
    {
        if (score >= 500)
        {
            ChangeState(new SceneWinState());
            Debug.Log("YOU WIN");
            Game.TotalCoins += score;
        }
        else
        {
            ChangeState(new SceneLoseState());
            Debug.Log("YOU LOSE");
        }

        base.FinalScore();
    }
}
