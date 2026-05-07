using UnityEngine;

public class ScoreCount : MonoBehaviour
{
    public static ScoreCount Instance { get; private set; }

    private bool OneTimeSet = true;

    private float Timer;
    private string LevelName;
    private int TimerScore;
    private int ItemScore;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetVals(float aTimer, int aItemScore, string aLevelName)
    {
        if (Instance != null) return;

        if (OneTimeSet)
        {
            Timer = aTimer;
            LevelName = aLevelName;
            TimerScore = GetScoreFromTimer(aTimer);
            ItemScore = aItemScore;
            Instance = this;
        }
        OneTimeSet = false;
    }

    public void DestroyScoreCount()
    {
        Instance = null;
        Destroy(gameObject);
    }

    public float GetTime()
    {
        return Timer;
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
    public string GetLevelName()
    {
        return LevelName;
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
