using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    private Vector3 LastCheckPointPos;
    private int CheckpointNumber;
    private string LevelName;
    private List<ItemData> inventory;
    private int PlayerNum;

    public PlayerSaveData(Vector3 aLCP, int aCN, string aLN, List<ItemData> aInventory, int aPlayerNum)
    {
        LastCheckPointPos = aLCP;
        CheckpointNumber = aCN;
        LevelName = aLN;

        // Deep copy the list.
        foreach (ItemData item in aInventory)
        {
            inventory.Add(item);
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
            inventory.Add(item);
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
    public IEnumerable<ItemData> GetInventory()
    {
        foreach(var item in inventory)
        {
            yield return item;
        }
    }

    public static PlayerSaveData LoadJson(string json)
    {
        return JsonUtility.FromJson<PlayerSaveData>(json);
    }
}
