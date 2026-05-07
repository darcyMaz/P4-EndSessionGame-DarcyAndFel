using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public void StartLevel(string LevelName)
    {
        SceneManager.Instance.BufferSceneChange(LevelName);
    }
    public void QuitGame()
    {
        Debug.Log("Quit Game pressed.");
        Application.Quit();
    }
    public void DeleteSaveData()
    {
        // Remove all the files in the persistant path folder.
        SaveManager.Instance.DeleteAllSaveData();
        
        //Debug.Log("Delete save data pressed. Not implemented.");
    }
}
