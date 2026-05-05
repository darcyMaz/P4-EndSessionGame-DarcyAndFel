using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeight : MonoBehaviour
{
    private static List<PlayerHeight> playerHeights = new List<PlayerHeight>();
    
    public static event Action<PlayerHeight> OnPlayerHeightAdded;
    public static event Action<PlayerHeight> OnPlayerHeightRemoved;

    public event Action<float> OnHeightChange;

    private void OnEnable()
    {
        playerHeights.Add(this);
    }
    private void OnDisable()
    {
        playerHeights.Remove(this);
    }

    private void FixedUpdate()
    {
        OnHeightChange?.Invoke(transform.position.y);
    }

    
    public static IEnumerable<PlayerHeight> GetPlayerHeights()
    {
        foreach (PlayerHeight item in playerHeights)
        {
            yield return item;
        }
    }
    
}
