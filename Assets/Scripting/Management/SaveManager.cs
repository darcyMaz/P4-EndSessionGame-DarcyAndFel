using System;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    public bool SavePlayerData(string json, int playerNum, string levelName)
    {
        try
        {
            System.IO.File.WriteAllText(Application.persistentDataPath + "/" + levelName + "/" + playerNum + "_PlayerData.json", json);
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("The SaveManager tried to save data to the persistant data folder. It failed.");
            Debug.LogError(e);
            return false;
        }
    }
    public PlayerSaveData LoadPlayerdata(string pathName)
    {
        try
        {
            string fileContents = System.IO.File.ReadAllText(pathName);

            if (fileContents == "{}") return null;
            return PlayerSaveData.LoadJson(fileContents);
        }
        catch
        {
            Debug.Log("LoadPlayerData in SaveManager could not find a file for a player. This may be normal.");
            return null;
        }
    }
}
