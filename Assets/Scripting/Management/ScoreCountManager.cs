using TMPro;
using UnityEngine;

public class ScoreCountManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI LevelName;
    [SerializeField] private TextMeshProUGUI Time;
    [SerializeField] private TextMeshProUGUI TimeScore;
    [SerializeField] private TextMeshProUGUI InventoryScore;
    [SerializeField] private TextMeshProUGUI TotalScore;

    private void Start()
    {
        //OnCountScore?.Invoke();

        if (ScoreCount.Instance == null)
        {
            Debug.Log("The score could not be read from the ScoreCount Instance.");
            return;
        }

        LevelName.text = ScoreCount.Instance.GetLevelName();
        Time.text = ScoreCount.Instance.GetTime().ToString();
        TimeScore.text = ScoreCount.Instance.GetTimerScore().ToString();
        InventoryScore.text = ScoreCount.Instance.GetItemScore().ToString();
        TotalScore.text = ScoreCount.Instance.GetScore().ToString();
    }

    

    public void MainMenu()
    {
        if (ScoreCount.Instance != null)
        {
            string levelname = ScoreCount.Instance.GetLevelName();
            SaveManager.Instance.DeleteLevelSaveData(levelname);
            ScoreCount.Instance.DestroyScoreCount();
        }
        
        SceneManager.Instance.BufferSceneChange("Main Menu");
    }
    public void RestartLevel()
    {
        string levelname;
        if (ScoreCount.Instance != null)
        {
            levelname = ScoreCount.Instance.GetLevelName();
            SaveManager.Instance.DeleteLevelSaveData(levelname);
            ScoreCount.Instance.DestroyScoreCount();
        }
        else levelname = "Main Menu";

        SceneManager.Instance.BufferSceneChange(levelname);
    }
    public void Quit()
    {
        if (ScoreCount.Instance != null)
        {
            string levelname = ScoreCount.Instance.GetLevelName();
            SaveManager.Instance.DeleteLevelSaveData(levelname);
            ScoreCount.Instance.DestroyScoreCount();
        }

        Application.Quit();
    }
}
