using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Checkpoint", menuName = "Checkpoint/Checkpoint")]
public class Checkpoint : ScriptableObject, IComparable<Checkpoint>
{
    [SerializeField] private int Level;
    [SerializeField] private Vector3 RespawnPos;
    [SerializeField] private float HeightTrigger;
    [SerializeField] private float DistanceTrigger;

    public int CompareTo(Checkpoint other)
    {
        return Level - other.GetLevel();
    }

    public int GetLevel()
    {
        return Level;
    }
    public Vector3 GetRespawnPos()
    {
        return RespawnPos;
    }
    public float[] GetTriggerVals()
    {
        float[] vals = { DistanceTrigger, HeightTrigger };
        return vals;
    }
}