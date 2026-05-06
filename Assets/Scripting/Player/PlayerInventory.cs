using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    // inspector setup
    [Header("Settings")]
   
    [SerializeField] private int maxCapacity = 5;

    [Header("Events")]
    [SerializeField] private UnityEvent onInventoryChanged;
    

    // private state
    private List<ItemData> heldItems;

    

    private void Awake()
    {
        heldItems = new List<ItemData>(); 
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
        onInventoryChanged.Invoke();
        return true;
    }

    public bool Has(ItemData type)
        => heldItems.Contains(type);

}
