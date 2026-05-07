using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI height_indicator_text;
    [SerializeField] private TextMeshProUGUI timer_text;
    [SerializeField] private TextMeshProUGUI speed_boost_text;

    public static event Action<float> OnTimeIncrement;
    private float time = 0;

    [SerializeField] private GameObject ScoreCountGO;
    public event Action OnLevelComplete;

    [SerializeField] private string CurrentLevel;

    [SerializeField] private GameObject InventoryPanel;
    private List<Button> InventoryElements = new List<Button>();

    private List<Checkpoint> checkpoints;
    private bool HasCheckpoints = true;
    [SerializeField] private UnityEvent<Vector2> OnCheckpointReached;

    // I could also code it so only one player can unpause the game after they've paused it.
    [SerializeField] private UnityEvent <bool> OnPauseFlipped;
    private bool IsPaused = false;

    public event Action OnSaveAndQuit;
    public event Action OnResetLevel;

    

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
    private void OnDisable()
    {
        OnTimeIncrement -= AdjustTimer;
    }

    private void Start()
    {
        AdjustSpeedUI(0);
        AdjustHeightIndicator(0);
        AdjustTimer(0);


        // Get the inventory slots.
        // Get all the UI slots in the inventory and add them to the list.
        foreach (Transform child in InventoryPanel.transform)
        {
            Button currentButton;
            if (!child.gameObject.TryGetComponent(out currentButton)) Debug.Log("The GameManager had a problem getting a Button from the inventory UI panel.");
            else
            {
                InventoryElements.Add(currentButton);
            }
        }

        // If there is more than one player in the game, the GameManager listens to all of their heights.
        foreach (PlayerStats ps in PlayerStats.GetPlayerStats())
        {
            ps.OnSaveAndQuitPlayer += HandlePlayerSaveData;
            ps.OnPause += Pause;
            ps.OnLevelComplete += LevelComplete;
            ps.OnInventoryChange += AdjustInventoryUI;
            ps.OnCheckpointReached += AdjustHeightIndicator;
        }
        // We listen to the addition of removal of players in run time.
        PlayerStats.OnPlayerStatsAdded += AddPlayerStats;
        PlayerStats.OnPlayerStatsRemoved += RemovePlayerStats;

        OnTimeIncrement += AdjustTimer;
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
        toRemove.OnInventoryChange += AdjustInventoryUI;
        toRemove.OnCheckpointReached += AdjustHeightIndicator;
    }
    private void AddPlayerStats(PlayerStats toAdd)
    {
        toAdd.OnSaveAndQuitPlayer += HandlePlayerSaveData;
        toAdd.OnPause += Pause;
        toAdd.OnLevelComplete += LevelComplete;
        toAdd.OnInventoryChange -= AdjustInventoryUI;
        toAdd.OnCheckpointReached += AdjustHeightIndicator;
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
        if (!ScoreCountGO_Clone.TryGetComponent(out sc)) Debug.Log("The ScoreCountGO prefab did not have its ScoreCount component. The score may not be tallied.");
        else
        {
            sc.SetVals(time, inventoryScore, GetLevelName());
        }

        SceneManager.Instance.BufferSceneChange("Score Count");
    }

    public void AdjustSpeedUI(int speedLevel)
    {
        speed_boost_text.text = speedLevel + "/" + 3 + " Speed Level Reached.";
    }
    public void AdjustTimer(float time)
    {
        timer_text.text = time.ToString();
    }
    public void AdjustHeightIndicator(int currentCheckpoint)
    {
        if (HasCheckpoints)
        {
            height_indicator_text.text = currentCheckpoint + "/" + checkpoints.Count + " Checkpoints Reached.";
        }
    }

    private void AdjustInventoryUI(PlayerInventory pi)
    {
        // Get the count of inventory items
        // Turn on that many buttons, along the way add inventory items' sprites to them

        int itemsTotal = pi.Count;
        int buttonIndex = 0;

        for (; buttonIndex < itemsTotal; buttonIndex++)
        {
            // set button to true and put in the sprite as the image
            InventoryElements[buttonIndex].gameObject.SetActive(true);

            Image buttonImage;
            if (!InventoryElements[buttonIndex].gameObject.TryGetComponent(out buttonImage)) Debug.Log("GameManager tried to change the sprite of an item slot in inventory but failed. In AdjustInventoryUI().");
            else
            {
                buttonImage.sprite = pi.GetIconAt(buttonIndex);
            }
        }
        for (; buttonIndex < InventoryElements.Count; buttonIndex++)
        {
            // set the button is active to false
            InventoryElements[buttonIndex].gameObject.SetActive(false);
        }
    }
}

