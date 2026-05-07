using System;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            //Debug.Log(Instance + ": " + SaveManager.Instance.name);
            //Debug.Log((Instance == this) + " SaveManager");
            Destroy(gameObject);
            return;
        }
        //Debug.Log("Save Manager after if statement awake");

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

    public bool TryMakeLevelDir(string LevelName)
    {
        string levelpath = Application.persistentDataPath + "/" + LevelName;

        try
        {
            Directory.CreateDirectory(levelpath);
            return true;
        }
        catch
        {
            Debug.Log("There was an attempt to make a directort but it failed. This may be normal");
            return false;
        }
    }

    public bool DeleteSaveData(string LevelName, int PlayerNum)
    {
        try
        {
            System.IO.File.Delete(Application.persistentDataPath + "/" + LevelName + "/" + PlayerNum + "_PlayerData.json");
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("There was an attempt to delete a save file but it did not work. This may be normal.");
            Debug.LogError(e);
            return false;
        }
    }
    public bool DeleteLevelSaveData(string LevelName)
    {
        try
        {
            string directory_path = Application.persistentDataPath + "/" + LevelName;
            DirectoryInfo dir = new DirectoryInfo(directory_path);

            foreach (var file in dir.GetFiles())
            {
                System.IO.File.Delete(file.FullName);
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.Log("There was an attempt to delete save files but it did not work. This may be normal.");
            Debug.LogError(e);
            return false;
        }
    }
    
    public bool DeleteAllSaveData()
    {
        try
        {
            string directory_path = Application.persistentDataPath;
            DirectoryInfo persist_dir = new DirectoryInfo(directory_path);

            // Deletes all the folders and their data recursively.
            foreach (DirectoryInfo dir in persist_dir.EnumerateDirectories())
            {
                dir.Delete(true);
            }

            // Delete files stored directly in this folder too.
            foreach (var file in persist_dir.EnumerateFiles())
            {
                file.Delete();
            }
        }
        catch (Exception e)
        {
            Debug.Log("There was an attempt to delete all save data, but it failed.");
            Debug.LogError(e);
        }


        return false;
    }
}
