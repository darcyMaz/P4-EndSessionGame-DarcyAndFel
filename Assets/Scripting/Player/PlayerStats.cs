using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    // Player stats
    private static List<PlayerStats> playerStats = new List<PlayerStats>();
    private int PlayerNum;
    public static event Action<PlayerStats> OnPlayerStatsAdded;
    public static event Action<PlayerStats> OnPlayerStatsRemoved;
    
    // Pause
    public event Action OnPause;
    private ProjectActions _actions;
    private InputAction pause;

    // Speed boost
    public event Action <int> OnSpeedBoostChange;
    [SerializeField] private int SpeedBoostLevel = 0;
    [SerializeField] private int MaxSpeedBoost;

    // Death
    public event Action <Vector3> OnDeath;

    // Save
    public event Action<string, int> OnSaveAndQuitPlayer;
    private bool LoadedData = false;

    // Checkpoints
    public event Action <int> OnCheckpointReached;
    private Checkpoint NextCheckpoint;
    private bool HasCheckpoints;
    private bool PassedLastCheckpoint = false;
    private int CheckpointIndex = 0;
    private Vector2 LastCheckpointPos;
    private float ResetHeight;

    // Inventory
    private PlayerInventory inventory;
    private bool HasInventory = false;

    private void Awake()
    {
        _actions = new ProjectActions();
    }
    private void OnEnable()
    {
        playerStats.Add(this);
        PlayerNum = playerStats.Count;
        OnPlayerStatsAdded?.Invoke(this);

        pause = _actions.Player.Pause;
        pause.Enable();
        pause.performed += PauseGame;

        OnCheckpointReached += SetNextCheckpoint;
    }
    private void OnDisable()
    {
        playerStats.Remove(this);
        OnPlayerStatsRemoved?.Invoke(this);

        pause.Disable();
        pause.performed -= PauseGame;

        OnCheckpointReached -= SetNextCheckpoint;
    }

    private void Start()
    {
        // try to load save data, if there is no save data for this player,
        // put in the default stuff

        try
        {
            string potentialFileName = Application.persistentDataPath + "/" + GameManager.Instance.GetLevelName() + "/" + PlayerNum + "_PlayerData.json";
            string fileContents = System.IO.File.ReadAllText(potentialFileName);
            PlayerSaveData psd = PlayerSaveData.LoadJson(fileContents);

            // If the game manager has checkpoints for this level.
            if (HasCheckpoints = GameManager.Instance.DoesLevelHaveCheckpoints())
            {
                ResetHeight = GameManager.Instance.GetCheckpoint(psd.GetCheckpointNum()).GetTriggerVals().y;
                LastCheckpointPos = psd.GetCheckpointPos();
                NextCheckpoint = GameManager.Instance.GetCheckpoint(psd.GetCheckpointNum());
            }
            else
            {
                // Set the checkpoint variables to default values if there's no checkpoints.
                LastCheckpointPos = transform.position;
                ResetHeight = transform.position.y - 5f; // A little lower than the start pos.
            }

        }
        catch
        {
            // This code is run if there's no save data.

            if (HasCheckpoints = GameManager.Instance.DoesLevelHaveCheckpoints()) NextCheckpoint = GameManager.Instance.GetCheckpoint(CheckpointIndex);
            
            LastCheckpointPos = transform.position;
            ResetHeight = transform.position.y - 5f; // A little lower than the start pos.

            Debug.Log("A PlayerStats component looked for a save file and found none. This is not unusual.");
        }


        if (HasCheckpoints = GameManager.Instance.DoesLevelHaveCheckpoints())
        {
            // Rather than getting the first checkpoint, I want to have gotten the save data and worked from that

            // get the first checkpoint and save it
            NextCheckpoint = GameManager.Instance.GetCheckpoint(CheckpointIndex);
        }

        // set the reset values to be based on the starting pos
        // These will update as we pass checkpoints
        LastCheckpointPos = transform.position;
        ResetHeight = transform.position.y - 5f; // A little lower than the start pos.

        if (!TryGetComponent(out inventory)) Debug.Log("The PlayerStats component could not find its PlayerInventory ");
        else HasInventory = true;

        GameManager.Instance.OnSaveAndQuit += SaveAndQuit;
    }

    

    private void Update()
    {
        CheckpointCheck();
    }

    private void PauseGame(InputAction.CallbackContext context)
    {
        if (context.performed) OnPause?.Invoke();
    }

    private void PlayerDeath()
    {
        // transform.position = LastCheckpointPos;
        OnDeath?.Invoke(LastCheckpointPos);
    }

    private void CheckpointCheck()
    {
        // If we have no checkpoints in front of us.
        if (!HasCheckpoints || PassedLastCheckpoint)
        {
            // If we've fallen below the reset height.
            if (transform.position.y < ResetHeight)
            {
                ReachedEndCheck();
                return;
            }
        }

        // Otherwise, check if we have passed the next checkpoint.

        Vector2 triggerVals = NextCheckpoint.GetTriggerVals();
        Vector2 triggerValDirs = NextCheckpoint.GetTriggerValsDir();

        bool triggerX = false, triggerY = false;
        
        // This code checks whether the checkpoint has been passed, but we need to clarify in which direction the player needs to pass.
        if (triggerValDirs.x == -1) triggerX = transform.position.x < triggerVals.x;
        else if (triggerValDirs.x == 1) triggerX = transform.position.x > triggerVals.x;
        else
        {
            Debug.Log("A Checkpoint object had a value other than 1 or -1 in it's triggerValDirs in the x position.");
            triggerX = false;
        }
        if (triggerValDirs.y == -1) triggerY = transform.position.y < triggerVals.y;
        else if (triggerValDirs.y == 1) triggerY = transform.position.y > triggerVals.y;
        else
        {
            Debug.Log("A Checkpoint object had a value other than 1 or -1 in it's triggerValDirs in the y position.");
            triggerY = false;
        }

        // Check if we've reached the next checkpoint.
        if (triggerY && triggerX)
        {
            LastCheckpointPos = NextCheckpoint.GetRespawnPos();
            ResetHeight = triggerVals.y;
            OnCheckpointReached?.Invoke(CheckpointIndex++);
        }
        // In the case where we haven't reached the next checkpoint (which is almost all the time)...
        else
        {
            // Check if we have gone below the threshold of the previous checkpoint.
            // If so, call the player death event.
            if (transform.position.y < ResetHeight)
            {
                // Kill and reset this player
                PlayerDeath();
            }
        }

    }

    private void SetNextCheckpoint(int index)
    {
        Checkpoint nextCheckpoint = GameManager.Instance.GetCheckpoint(index);

        if (nextCheckpoint != null) NextCheckpoint = nextCheckpoint;
        else { PassedLastCheckpoint = true; }
    }

    private void ReachedEndCheck()
    {
        Debug.Log("Checking if we've reached the end - player stats");
    }

    private string BuildSaveData()
    {
        // Check whether or not this player has the PlayerInventory script attached.
        IEnumerable<ItemData> inventoryToSend;
        if (HasInventory) inventoryToSend = inventory.GetItems();
        else inventoryToSend = new List<ItemData>();

        // Make this a string instead with json whatever
        PlayerSaveData toReturn = new PlayerSaveData(LastCheckpointPos, CheckpointIndex, GameManager.Instance.GetLevelName(), inventoryToSend, PlayerNum);
        
        return JsonUtility.ToJson(toReturn);
    }

    private void LoadSaveData(string loaddata)
    {
        if (LoadedData) return;

        // interpret the data and then apply it
    }

    private void SaveAndQuit()
    {
        OnSaveAndQuitPlayer?.Invoke(BuildSaveData(), PlayerNum);
    }

    public int ChangeSpeedBoost(int delta)
    {
        SpeedBoostLevel = (SpeedBoostLevel + delta > MaxSpeedBoost) ? MaxSpeedBoost : (SpeedBoostLevel + delta < 0) ? 0: SpeedBoostLevel + delta;
        OnSpeedBoostChange?.Invoke(SpeedBoostLevel);
        return SpeedBoostLevel;
    }

    public int GetCurrentSpeedBoost()
    {
        return SpeedBoostLevel;
    }

    public static IEnumerable<PlayerStats> GetPlayerStats()
    {
        foreach (PlayerStats item in playerStats)
        {
            yield return item;
        }
    }
    
}
