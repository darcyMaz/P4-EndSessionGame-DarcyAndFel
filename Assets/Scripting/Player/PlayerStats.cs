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

    // Complete Level
    public event Action <int> OnLevelComplete;

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
        if (!TryGetComponent(out inventory)) Debug.Log("The PlayerStats component could not find its PlayerInventory ");
        else HasInventory = true;

        GameManager.Instance.OnSaveAndQuit += SaveAndQuit;

        // Check to see if this level has a folder for persistant file saving.
        SaveManager.Instance.TryMakeLevelDir(GameManager.Instance.GetLevelName());

        string potentialFileName = Application.persistentDataPath + "/" + GameManager.Instance.GetLevelName() + "/" + PlayerNum + "_PlayerData.json";
        PlayerSaveData psd = SaveManager.Instance.LoadPlayerdata(potentialFileName);

        // 
        //Debug.Log("psd.GetCheckPos(): " + psd.GetCheckpointPos());

        // Try to load save data.
        // If there is save data, then ...
        if (psd != null)
        {
            // If the game manager has checkpoints for this level.
            if (HasCheckpoints = GameManager.Instance.DoesLevelHaveCheckpoints())
            {
                CheckpointIndex = psd.GetCheckpointNum();

                // If the loaded data indicates we've pass the last checkpoint
                if (CheckpointIndex == GameManager.Instance.HowManyCheckpoints())
                {
                    Debug.Log("PlayerStats loaded a save file where the player passed the last checkpoint but did not finish the game.");
                    PassedLastCheckpoint = true;
                    LastCheckpointPos = GameManager.Instance.GetCheckpoint(GameManager.Instance.HowManyCheckpoints() - 1).GetRespawnPos();
                    ResetHeight = GameManager.Instance.GetCheckpoint(GameManager.Instance.HowManyCheckpoints() - 1).GetTriggerVals().y;
                }
                else
                {
                    NextCheckpoint = GameManager.Instance.GetCheckpoint(CheckpointIndex);

                    // If, for some unknown reason, the checkpoint could not be loaded.
                    if (NextCheckpoint == null)
                    {
                        Debug.Log("The PlayerStats script tried to get a Checkpoint, but the CheckpointIndex was out of bounds when it shouldn't have been.");
                        HasCheckpoints = false;

                        LastCheckpointPos = transform.position;
                        ResetHeight = transform.position.y - 5f; // A little lower than the start pos.
                    }
                    else
                    {
                        ResetHeight = GameManager.Instance.GetCheckpoint(psd.GetCheckpointNum() - 1).GetTriggerVals().y;
                        LastCheckpointPos = psd.GetCheckpointPos();
                    }
                }
                
            }
            else
            {
                // Set the checkpoint variables to default values if there's no checkpoints.
                LastCheckpointPos = transform.position;
                ResetHeight = transform.position.y - 5f; // A little lower than the start pos.
            }

            // If the player has the inventory component attached.
            if (HasInventory)
            {
                foreach (ItemData item in psd.GetInventory())
                {
                    // Add the item from the save data.
                    inventory.PickUp(item);
                }
            }
        }
        else
        {
            // This code is run if there's no save data.
            // Debug.Log("No save data");
            if (HasCheckpoints = GameManager.Instance.DoesLevelHaveCheckpoints())
            {
                Debug.Log("GM has checkpoints");
                NextCheckpoint = GameManager.Instance.GetCheckpoint(CheckpointIndex);

                if (NextCheckpoint == null)
                {
                    Debug.Log("The PlayerStats script tried to get a Checkpoint, but the CheckpointIndex was out of bounds when it shouldn't have been.");
                    HasCheckpoints = false;
                }
            }

            Debug.Log("Setting to default values");
            LastCheckpointPos = transform.position;
            ResetHeight = transform.position.y - 5f; // A little lower than the start pos.

            // Debug.Log("LastCheckPoint: " + LastCheckpointPos + " ResetHeight: " + ResetHeight + " CIndex: " + CheckpointIndex);
        }
    }

    

    private void Update()
    {
        // Debug.Log(CheckpointIndex);

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

        //Debug.Log(HasCheckpoints + " " + PassedLastCheckpoint);

        // If we have no checkpoints in front of us.
        if (!HasCheckpoints || PassedLastCheckpoint)
        {
            // Debug.Log("No checkpoints in front: PlayerStats");
            ReachedEndCheck();

            //Debug.Log("Check reset height: " + transform.position.y + " < " + ResetHeight);

            // If we've fallen below the reset height.
            if (transform.position.y < ResetHeight)
            {
                PlayerDeath();
            }

            return;
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
            Debug.Log("Is this being called twice?");

            LastCheckpointPos = NextCheckpoint.GetRespawnPos();
            ResetHeight = triggerVals.y;
            OnCheckpointReached?.Invoke(++CheckpointIndex);
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
        // Debug.Log("SetNextCheck(): "+index);

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

    private void SaveAndQuit()
    {
        string json = BuildSaveData();
        Debug.Log(json);

        OnSaveAndQuitPlayer?.Invoke(json, PlayerNum);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "EndZone")
        {
            int inventoryScore = 0;
            if (HasInventory) inventoryScore = inventory.GetTotalScore();


            OnLevelComplete?.Invoke(inventoryScore);
        }
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
