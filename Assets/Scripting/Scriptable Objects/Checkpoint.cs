using UnityEngine;

[CreateAssetMenu(fileName = "Checkpoint", menuName = "Checkpoint/Checkpoint")]
public class Checkpoint : ScriptableObject
{
    [SerializeField] private int Level;
    [SerializeField] private Vector3 RespawnPos;
    [SerializeField] private float Height;

    public int GetLevel()
    {
        return Level;
    }
    public Vector3 GetRespawnPos()
    {
        return RespawnPos;
    }
    public float GetHeight()
    {
        return Height;
    }
}