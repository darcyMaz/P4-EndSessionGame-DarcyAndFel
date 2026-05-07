using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    // ALSO PUT THE TIMER INTO THIS

    [SerializeField] private Vector3 LastCheckPointPos;
    [SerializeField] private int CheckpointNumber;
    [SerializeField] private string LevelName;
    [SerializeField] private List<string> inventory;
    [SerializeField] private int PlayerNum;

    public PlayerSaveData(Vector3 aLCP, int aCN, string aLN, List<ItemData> aInventory, int aPlayerNum)
    {
        LastCheckPointPos = aLCP;
        CheckpointNumber = aCN;
        LevelName = aLN;

        // Deep copy the list.
        foreach (ItemData item in aInventory)
        {
            inventory.Add(  JsonUtility.ToJson(item) );
        }

        PlayerNum = aPlayerNum;
    }

    public PlayerSaveData(Vector3 aLCP, int aCN, string aLN, IEnumerable<ItemData> aInventory, int aPlayerNum)
    {
        LastCheckPointPos = aLCP;
        CheckpointNumber = aCN;
        LevelName = aLN;

        // Deep copy the list.
        foreach (ItemData item in aInventory)
        {
            inventory.Add( JsonUtility.ToJson(item) );
        }

        PlayerNum = aPlayerNum;
    }

    // Getter functions
    public Vector3 GetCheckpointPos()
    {
        return LastCheckPointPos;
    }
    public int GetCheckpointNum()
    {
        return CheckpointNumber;
    }
    public string GetLevelName()
    {
        return LevelName;
    }
    public int GetPlayerNum()
    {
        return PlayerNum;
    }
    public IEnumerable<ItemData> GetInventory()
    {
        foreach(string item in inventory)
        {
            yield return JsonUtility.FromJson<ItemData>(item);
        }
    }

    public static PlayerSaveData LoadJson(string json)
    {
        return JsonUtility.FromJson<PlayerSaveData>(json);
    }
}
