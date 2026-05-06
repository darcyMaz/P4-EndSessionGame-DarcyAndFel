using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{

    public static event Action<float> OnTimeIncrement;
    private float time = 0;

    [SerializeField] private string CurrentLevel;

    private List<Checkpoint> checkpoints;
    private int CheckpointIndex = 0;
    private bool HasCheckpoints = true;
    private Vector2 LastCheckpointPos;
    [SerializeField] private UnityEvent<Vector2> OnCheckpointReached;

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
            checkpoints = new List<Checkpoint>();

            foreach (Checkpoint item in checkpoints_arr)
            {
                checkpoints.Add(item);
            }

            checkpoints.Sort();
            // checkpoints.ForEach(item => { Debug.Log(item.GetLevel()); });
        }

    }

    private void Start()
    {
        // If there is more than one player in the game, the GameManager listens to all of their heights.
        foreach (PlayerStats ph in PlayerStats.GetPlayerStats())
        {
            ph.OnPosChange += PosUpdate;
            ph.OnPause += Pause;
        }
        PlayerStats.OnPlayerStatsAdded += AddPlayerStats;
        PlayerStats.OnPlayerStatsRemoved += RemovePlayerStats;

        // Set the last checkpoint position to be the startig point of the level.
        LastCheckpointPos = transform.position;
    }

    private void Update()
    {
        // Time Event
        time += Time.deltaTime;
        OnTimeIncrement?.Invoke(time);
    }

    private void PosUpdate(Vector2 CurrPos)
    {
        // HeighestPointReached = (currentHeight > HeighestPointReached) ? currentHeight : HeighestPointReached;

        // Check if we've reached the next checkpoint. X
        // If we fall below the last threshold, respawn the player.

        if (HasCheckpoints)
        {
            // Get the values of the next checkpoint.
            Vector2 triggerVals = checkpoints[CheckpointIndex].GetTriggerVals();
            Vector2 triggerValDirs = checkpoints[CheckpointIndex].GetTriggerValsDir();

            bool triggerX = false, triggerY = false;

            // This isn't necessary is it
            if (triggerValDirs.x == -1) triggerX = CurrPos.x < triggerVals.x;
            else if (triggerValDirs.x == 1) triggerX = CurrPos.x > triggerVals.x;
            else 
            {
                Debug.Log("A Checkpoint object had a value other than 1 or -1 in it's triggerValDirs in the x position.");
                triggerX = false;
            }

            if (triggerValDirs.y == -1) triggerY = CurrPos.y < triggerVals.y;
            else if (triggerValDirs.y == 1) triggerY = CurrPos.y > triggerVals.y;
            else
            {
                Debug.Log("A Checkpoint object had a value other than 1 or -1 in it's triggerValDirs in the y position.");
                triggerY = false;
            }

            // Check if we've reached the next checkpoint.
            if (triggerY && triggerX)
            {
                Debug.Log("checkpoint reached!");

                LastCheckpointPos = triggerVals;
                OnCheckpointReached?.Invoke(checkpoints[CheckpointIndex++].GetRespawnPos());

                // If we've passed the final checkpoint, ignore checkpoints going forward.
                if (checkpoints.Count == CheckpointIndex) HasCheckpoints = false;
            }
            
        }



        // Check if we've fallen below the previous checkpoint.
        // if (CurrPos.x < )

    }
    private void RemovePlayerStats(PlayerStats toRemove)
    {
        toRemove.OnPause -= Pause;
        toRemove.OnPosChange -= PosUpdate;
    }
    private void AddPlayerStats(PlayerStats toAdd)
    {
        toAdd.OnPause += Pause;
        toAdd.OnPosChange += PosUpdate;
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

