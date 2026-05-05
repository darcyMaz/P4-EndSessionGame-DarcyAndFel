using System;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{

    public static event Action<float> OnTimeIncrement;
    private float time = 0;

    private float HeighestPointReached = float.MinValue;

    private string CurrentLevel;

    // I could also code it so only one player can unpause the game after they've paused it.
    [SerializeField] private UnityEvent <bool> OnPauseFlipped;
    private bool IsPaused = false;

    private void Awake()
    {
        // Load in the SOs of checkpoints that I'm gonna make
        // So no struct haha, SOs
        // I'll copy paste it tho


    }

    private void Start()
    {
        // If there is more than one player in the game, the GameManager listens to all of their heights.
        foreach (PlayerStats ph in PlayerStats.GetPlayerStats())
        {
            ph.OnHeightChange += HeightUpdate;
        }
        PlayerStats.OnPlayerStatsAdded += AddPlayerStats;
        PlayerStats.OnPlayerStatsRemoved += RemovePlayerStats;
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
    private void RemovePlayerStats(PlayerStats toRemove)
    {
        toRemove.OnPause -= Pause;
        toRemove.OnHeightChange -= HeightUpdate;
    }
    private void AddPlayerStats(PlayerStats toAdd)
    {
        toAdd.OnPause += Pause;
        toAdd.OnHeightChange += HeightUpdate;
    }

    private void Pause()
    {
        IsPaused = (IsPaused) ? false : true;
        OnPauseFlipped.Invoke(IsPaused);
    }
    
}

