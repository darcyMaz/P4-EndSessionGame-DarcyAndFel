using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSaveData : MonoBehaviour
{
    private Vector3 LastCheckPointPos;
    private int CheckpointNumber;
    private string LevelName;
    private List<ItemData> inventory;

    public PlayerSaveData(Vector3 aLCP, int aCN, string aLN, List<ItemData> aInventory)
    {
        LastCheckPointPos = aLCP;
        CheckpointNumber = aCN;
        LevelName = aLN;

        // Deep copy the list.
        foreach (ItemData item in aInventory)
        {
            inventory.Add(item);
        }
    }

    public PlayerSaveData(Vector3 aLCP, int aCN, string aLN, IEnumerable<ItemData> aInventory)
    {
        LastCheckPointPos = aLCP;
        CheckpointNumber = aCN;
        LevelName = aLN;

        // Deep copy the list.
        foreach (ItemData item in aInventory)
        {
            inventory.Add(item);
        }
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
}
