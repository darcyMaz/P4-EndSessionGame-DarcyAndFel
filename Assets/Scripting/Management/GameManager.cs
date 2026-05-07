using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static event Action<float> OnTimeIncrement;
    private float time = 0;

    [SerializeField] private string CurrentLevel;

    private List<Checkpoint> checkpoints;
    private bool HasCheckpoints = true;
    [SerializeField] private UnityEvent<Vector2> OnCheckpointReached;

    // I could also code it so only one player can unpause the game after they've paused it.
    [SerializeField] private UnityEvent <bool> OnPauseFlipped;
    private bool IsPaused = false;

    public event Action OnSaveAndQuit;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Checkpoints as SOs
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

            // Sorted by their checkpoint number, low to high.
            checkpoints.Sort();
        }
    }

    private void OnEnable()
    {
        // DeathZone.OnDeath += ;
    }
    private void OnDisable()
    {
        
    }

    private void Start()
    {
        // Load player data


        // If there is more than one player in the game, the GameManager listens to all of their heights.
        foreach (PlayerStats ps in PlayerStats.GetPlayerStats())
        {
            ps.OnSaveAndQuitPlayer += HandlePlayerSaveData;
            ps.OnPause += Pause;
        }
        // We listen to the addition of removal of players in run time.
        PlayerStats.OnPlayerStatsAdded += AddPlayerStats;
        PlayerStats.OnPlayerStatsRemoved += RemovePlayerStats;
    }

    private void Update()
    {
        // Time Event
        time += Time.deltaTime;
        OnTimeIncrement?.Invoke(time);
    }

    private void RemovePlayerStats(PlayerStats toRemove)
    {
        toRemove.OnSaveAndQuitPlayer += HandlePlayerSaveData;
        toRemove.OnPause -= Pause;
    }
    private void AddPlayerStats(PlayerStats toAdd)
    {
        toAdd.OnSaveAndQuitPlayer += HandlePlayerSaveData;
        toAdd.OnPause += Pause;
    }

    private void Pause()
    {
        IsPaused = (IsPaused) ? false : true;
        OnPauseFlipped.Invoke(IsPaused);
    }

    private void HandlePlayerSaveData(string savedata, int playerNum)
    {
        SaveManager.Instance.SavePlayerData(savedata, playerNum, CurrentLevel);
    }

    public Checkpoint GetCheckpoint(int index)
    {
        return (index < checkpoints.Count) ? checkpoints[index] : null;
    }
    
    public bool DoesLevelHaveCheckpoints()
    {
        return HasCheckpoints;
    }

    public string GetLevelName()
    {
        return CurrentLevel;
    }


    public void SaveAndQuit()
    {
        // save the gamestate into a json utility
        // save the last checkpoint basically

        // and then go to the main menu


        Pause();
        OnSaveAndQuit?.Invoke();
        SceneManager.Instance.BufferSceneChange("Main Menu");

        // Debug.Log("SaveAndQuit() called: GameManager");
    }

    public void ResetLevel()
    {
        Debug.Log("Reset level: GM");
        // call scene manager, buffer the current level
    }
}

