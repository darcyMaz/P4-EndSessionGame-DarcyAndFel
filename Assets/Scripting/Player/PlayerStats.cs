using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    private static List<PlayerStats> playerStats = new List<PlayerStats>();
    
    public static event Action<PlayerStats> OnPlayerStatsAdded;
    public static event Action<PlayerStats> OnPlayerStatsRemoved;

    public event Action<float> OnHeightChange;
    
    public event Action OnPause;
    private ProjectActions _actions;
    private InputAction pause;

    private void Awake()
    {
        _actions = new ProjectActions();
    }

    private void OnEnable()
    {
        playerStats.Add(this);

        pause = _actions.Player.Pause;
        pause.Enable();
        pause.performed += PauseGame;

    }
    private void OnDisable()
    {
        playerStats.Remove(this);

        pause.Disable();
        pause.performed -= PauseGame;
    }

    private void FixedUpdate()
    {
        OnHeightChange?.Invoke(transform.position.y);
    }

    private void PauseGame(InputAction.CallbackContext context)
    {
        Debug.Log("Gamne paused: Player stats");

        if (context.performed) OnPause?.Invoke();
    }

    public static IEnumerable<PlayerStats> GetPlayerStats()
    {
        foreach (PlayerStats item in playerStats)
        {
            yield return item;
        }
    }
    
}
