using UnityEngine;

public class ScoreCountManager : MonoBehaviour
{
    // let's build the score coutn scene
    // so we get the timer, the 

    private string PrevLevelName = "";

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
