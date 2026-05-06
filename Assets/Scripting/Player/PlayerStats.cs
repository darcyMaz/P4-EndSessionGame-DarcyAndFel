using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    private static List<PlayerStats> playerStats = new List<PlayerStats>();
    
    public static event Action<PlayerStats> OnPlayerStatsAdded;
    public static event Action<PlayerStats> OnPlayerStatsRemoved;

    public event Action<Vector2> OnPosChange;
    
    public event Action OnPause;
    private ProjectActions _actions;
    private InputAction pause;

    public event Action <int>OnSpeedBoostIncrement;
    [SerializeField] private int SpeedBoostStack = 0;
    [SerializeField] private int MaxSpeedBoost;

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
        OnPosChange?.Invoke(transform.position);
    }

    private void PauseGame(InputAction.CallbackContext context)
    {
        if (context.performed) OnPause?.Invoke();
    }

    public int IncrementSpeedBoost()
    {
        SpeedBoostStack = (SpeedBoostStack + 1 < MaxSpeedBoost) ? SpeedBoostStack + 1 : SpeedBoostStack;
        return SpeedBoostStack;

    }

    public int GetCurrentSpeedBoost()
    {
        return SpeedBoostStack;
    }

    public static IEnumerable<PlayerStats> GetPlayerStats()
    {
        foreach (PlayerStats item in playerStats)
        {
            yield return item;
        }
    }
    
}
