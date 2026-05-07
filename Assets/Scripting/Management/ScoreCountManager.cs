using UnityEngine;
using UnityEngine.Events;

public class ScoreCountManager : MonoBehaviour
{
    // let's build the score coutn scene
    // so we get the timer, the 

    private string PrevLevelName = "";

    [SerializeField] private UnityEvent OnCountScore;

    public void MainMenu()
    {
        SceneManager.Instance.BufferSceneChange("Main Menu");
    }
    public void RestartLevel()
    {
        SceneManager.Instance.BufferSceneChange(PrevLevelName);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
