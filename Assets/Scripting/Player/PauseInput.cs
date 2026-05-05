using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseInput : MonoBehaviour
{
    private ProjectActions _actions;
    private InputAction pause;

    public event Action OnPause;

    private void Awake()
    {
        _actions = new ProjectActions();
    }
    private void OnEnable()
    {
        pause = _actions.Player.Pause;
        pause.Enable();
        pause.performed += PauseGame;
    }
    private void PauseGame(InputAction.CallbackContext context)
    {
        if (context.performed) OnPause?.Invoke();
    }
}
