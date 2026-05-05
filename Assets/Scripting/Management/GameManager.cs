using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{

    public static event Action<float> OnTimeIncrement;
    private float time = 0;

    private float HeighestPointReached = float.MinValue;

    [SerializeField] private string CurrentLevel;

    private Queue<Checkpoint> checkpoints;
    private bool HasCheckpoints = true;

    // I could also code it so only one player can unpause the game after they've paused it.
    [SerializeField] private UnityEvent <bool> OnPauseFlipped;
    private bool IsPaused = false;



    private void Awake()
    {
        // Load in the SOs of checkpoints that I'm gonna make
        Checkpoint[] checkpoints_arr = Resources.LoadAll<Checkpoint>("Checkpoints/" + CurrentLevel);

        if (checkpoints_arr.Length == 0)
        {
            Debug.Log("No checkpoints were loaded from their resource folder. Checkpoints have been turned off.");
            HasCheckpoints = false;
        }
        else
        {
            checkpoints = new Queue<Checkpoint>();

            //List<string> templist = new List<string>();
            //templist.Sort();
        }

    }

    private void Start()
    {
        // If there is more than one player in the game, the GameManager listens to all of their heights.
        foreach (PlayerStats ph in PlayerStats.GetPlayerStats())
        {
            ph.OnHeightChange += HeightUpdate;
            ph.OnPause += Pause;
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
    
    public void SaveAndQuit()
    {
        // save the gamestate into a json utility
        // save the last checkpoint basically

        // and then go to the main menu

        Debug.Log("SaveAndQuit() called: GameManager");
    }

    public void ResetLevel()
    {
        Debug.Log("Reset level: GM");
        // call scene manager, buffer the current level
    }
}

