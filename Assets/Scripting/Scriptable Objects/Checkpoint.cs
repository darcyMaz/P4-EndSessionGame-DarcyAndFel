using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Checkpoint", menuName = "Checkpoint/Checkpoint")]
public class Checkpoint : ScriptableObject, IComparable<Checkpoint>
{
    [SerializeField] private int Level;
    [SerializeField] private Vector2 RespawnPos;
    [SerializeField] private Vector2 TriggerVals;
    [SerializeField] private Vector2 TriggerValsDir;

    public int CompareTo(Checkpoint other)
    {
        return Level - other.GetLevel();
    }

    public int GetLevel()
    {
        return Level;
    }
    public Vector2 GetRespawnPos()
    {
        return RespawnPos;
    }
    public Vector2 GetTriggerVals()
    {
        return TriggerVals;
    }
    public Vector2 GetTriggerValsDir()
    {
        return TriggerValsDir;
    }
}