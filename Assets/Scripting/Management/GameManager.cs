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
    public event Action OnResetLevel;

    [SerializeField] private GameObject ScoreCountGO;
    public event Action OnLevelComplete;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Check to see if this level has a folder for persistant file saving.
        SaveManager.Instance.TryMakeLevelDir(CurrentLevel);

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
        // If there is more than one player in the game, the GameManager listens to all of their heights.
        foreach (PlayerStats ps in PlayerStats.GetPlayerStats())
        {
            ps.OnSaveAndQuitPlayer += HandlePlayerSaveData;
            ps.OnPause += Pause;
            ps.OnLevelComplete += LevelComplete;
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
        toRemove.OnLevelComplete -= LevelComplete;
        toRemove.OnPause -= Pause;
    }
    private void AddPlayerStats(PlayerStats toAdd)
    {
        toAdd.OnSaveAndQuitPlayer += HandlePlayerSaveData;
        toAdd.OnPause += Pause;
        toAdd.OnLevelComplete += LevelComplete;
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

    public int HowManyCheckpoints()
    {
        return checkpoints.Count;
    }

    public string GetLevelName()
    {
        return CurrentLevel;
    }


    public void SaveAndQuit()
    {
        Pause();
        OnSaveAndQuit?.Invoke();
        SceneManager.Instance.BufferSceneChange("Main Menu");
    }

    public void ResetLevel()
    {
        Pause();
        OnResetLevel?.Invoke();
        SaveManager.Instance.DeleteLevelSaveData(CurrentLevel);
        SceneManager.Instance.BufferSceneChange(CurrentLevel);
    }

    public void LevelComplete(int inventoryScore)
    {
        // create new score count, gm will hold the gameobject prefab
        GameObject ScoreCountGO_Clone = Instantiate(ScoreCountGO);

        ScoreCount sc;
        if (!TryGetComponent(out sc)) Debug.Log("The ScoreCountGO prefab did nto have its ScoreCount component. The score may not be tallied.");
        else
        {
            sc.SetVals(time, inventoryScore);
        }
    }
}

