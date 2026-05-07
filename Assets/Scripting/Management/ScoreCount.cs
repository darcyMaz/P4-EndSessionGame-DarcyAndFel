using UnityEngine;

public class ScoreCount : MonoBehaviour
{
    private bool OneTimeSet = true;

    private int TimerScore;
    private int ItemScore;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetVals(float aTimer, int aItemScore)
    {
        if (OneTimeSet)
        {
            TimerScore = GetScoreFromTimer(aTimer);
            ItemScore = aItemScore;
        }
        OneTimeSet = false;
    }

    public int GetTimerScore()
    {
        return TimerScore;
    }
    public int GetItemScore()
    {
        return ItemScore;
    }
    public int GetScore()
    {
        return ItemScore + TimerScore;
    }

    // I'll def need to implement level specific scores for the time.
    private int GetScoreFromTimer(float time)
    {
        if (time < 30)
        {
            return 30;
        }
        else if (time < 60)
        {
            return 15;
        }
        else if (time < 180)
        {
            return 5;
        }
        else return 0;
    }

}
