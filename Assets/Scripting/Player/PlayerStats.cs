using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    private static List<PlayerStats> playerStats = new List<PlayerStats>();
    
    public static event Action<PlayerStats> OnPlayerStatsAdded;
    public static event Action<PlayerStats> OnPlayerStatsRemoved;
    
    public event Action OnPause;
    private ProjectActions _actions;
    private InputAction pause;

    public event Action <int> OnSpeedBoostChange;
    [SerializeField] private int SpeedBoostLevel = 0;
    [SerializeField] private int MaxSpeedBoost;

    public event Action <Vector3> OnDeath;

    public event Action <int> OnCheckpointReached;
    private Checkpoint NextCheckpoint;
    private bool HasCheckpoints;
    private bool PassedLastCheckpoint = false;
    private int CheckpointIndex = 0;
    private Vector2 LastCheckpointPos;
    private float ResetHeight;

    private void Awake()
    {
        _actions = new ProjectActions();
    }

    private void Start()
    {
        // check the gm to see if there are checkpoints
        // if so get the first checkpoint
        // upon checkpoint reached, ask for next checkpoint

        if (HasCheckpoints = GameManager.Instance.DoesLevelHaveCheckpoints())
        {
            // get the first checkpoint and save it
            NextCheckpoint = GameManager.Instance.GetCheckpoint(CheckpointIndex);
        }

        // set the reset values to be based on the starting pos
        // These will update as we pass checkpoints
        LastCheckpointPos = transform.position;
        ResetHeight = transform.position.y - 5f; // A little lower than the start pos.
    }

    private void OnEnable()
    {
        playerStats.Add(this);

        pause = _actions.Player.Pause;
        pause.Enable();
        pause.performed += PauseGame;

        OnCheckpointReached += SetNextCheckpoint;
    }
    private void OnDisable()
    {
        playerStats.Remove(this);

        pause.Disable();
        pause.performed -= PauseGame;

        OnCheckpointReached -= SetNextCheckpoint;
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
        if (!HasCheckpoints) return;

        Vector2 triggerVals = NextCheckpoint.GetTriggerVals();
        Vector2 triggerValDirs = NextCheckpoint.GetTriggerValsDir();

        bool triggerX = false, triggerY = false;

        // If we passed the last checkpoint then we don't want to check whether we passed
        //      the nonexistant next checkpoint.
        if (!PassedLastCheckpoint)
        {
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
        }
        // This can be the spot to check if we have reached the end.
        else
        {
            ReachedEndCheck();
            return;
        }

        // Check if we've reached the next checkpoint.
        if (triggerY && triggerX)
        {
            Debug.Log("checkpoint reached!");

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
        Debug.Log(index);
        Checkpoint nextCheckpoint = GameManager.Instance.GetCheckpoint(index);

        if (nextCheckpoint != null) NextCheckpoint = nextCheckpoint;
        else { PassedLastCheckpoint = true; Debug.Log("Last checkpoint passed - player stats"); }
    }

    private void ReachedEndCheck()
    {
        Debug.Log("Checking if we've reached the end - player stats");
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
