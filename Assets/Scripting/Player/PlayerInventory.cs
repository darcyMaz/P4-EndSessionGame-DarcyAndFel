using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInventory : MonoBehaviour
{
    // inspector setup
    [Header("Settings")]
   
    [SerializeField] private int maxCapacity = 5;

    // [Header("Events")]
    public event Action <ItemType> onInventoryChanged;
    
    // private state
    private List<ItemData> heldItems;

    private void Awake()
    {
        heldItems = new List<ItemData>(); 
    }

    private void Start()
    {
        onInventoryChanged?.Invoke(ItemType.None);
    }

    // public API

    public int Count => heldItems.Count;
    public bool IsFull => heldItems.Count >= maxCapacity;

    // inventory logic on pickup
    public bool PickUp(ItemData type)
    {
        if (IsFull)
        {
            Debug.Log($"[Inventory] is full");
            return false;
        }
        heldItems.Add(type);
        onInventoryChanged?.Invoke(ItemType.None);
        return true;
    }

    public void InventoryButtonClicked(int slotIndex)
    {
        RemoveItemAt(slotIndex);
    }

    public void RemoveItemAt(int index)
    {
        //Debug.Log(index + " " + heldItems.Count);
        //heldItems.ForEach((ItemData a) => { Debug.Log(a.ItemName()); });

        if (index >= 0 && index < heldItems.Count)
        {
            ItemType it = heldItems[index].Type();
            heldItems.RemoveAt(index);
            onInventoryChanged.Invoke(it);
        }
    }

    public bool Has(ItemData type)
        => heldItems.Contains(type);

    public int GetTotalScore()
    {
        int score = 0;
        foreach (ItemData item in heldItems)
        {
            score += item.Value();
        }
        return score;
    }

    public Sprite GetIconAt(int index)
    {
        return heldItems[index].Icon();
    }

    public IEnumerable<ItemData> GetItems()
    {
        foreach (ItemData item in heldItems)
        {
            yield return item;
        }
    }
}
