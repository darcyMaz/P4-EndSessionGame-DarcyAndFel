using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    [SerializeField] private ItemData PlusFive;
    [SerializeField] private ItemData PlusTen;
    [SerializeField] private ItemData Speed;

    private Dictionary<ItemType, ItemData> items;

    public static ItemDatabase Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Debug.LogError("multiple instances of ItemDatabase found, destroying the gameObject");
            Destroy(Instance);
            return;
        }
        Instance = this;
        BuildDatabase();
    }

    public bool TryGetItem(ItemType type, out ItemData data) => items.TryGetValue(type, out data);

    private void BuildDatabase()
    {
        items = new Dictionary<ItemType, ItemData>
        {
            {ItemType.Points, PlusFive},
            {ItemType.Points, PlusTen},
            {ItemType.Speed,  Speed },
        };
     }


}
