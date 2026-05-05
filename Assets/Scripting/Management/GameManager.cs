using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // so this listens to the player's height
    // connect thru an event

    // i want a timer event here as well

    public static event Action<float> OnTimeIncrement;
    private float time = 0;

    private float HeighestPointReached = float.MinValue;

    private void Start()
    {
        foreach (PlayerHeight ph in PlayerHeight.GetPlayerHeights())
        {
            ph.OnHeightChange += HeightUpdate;
        }
        PlayerHeight.OnPlayerHeightAdded += AddPlayerHeight;
        PlayerHeight.OnPlayerHeightRemoved += RemovePlayerHeight;
    }

    private void Update()
    {
        time += Time.deltaTime;
        OnTimeIncrement?.Invoke(time);
    }

    private void HeightUpdate(float currentHeight)
    {
        HeighestPointReached = (currentHeight > HeighestPointReached) ? currentHeight : HeighestPointReached;
    }
    private void RemovePlayerHeight(PlayerHeight toRemove)
    {
        toRemove.OnHeightChange -= HeightUpdate;
    }
    private void AddPlayerHeight(PlayerHeight toAdd)
    {
        toAdd.OnHeightChange += HeightUpdate;
    }
}
