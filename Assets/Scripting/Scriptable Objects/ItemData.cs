using UnityEngine;

[CreateAssetMenu(fileName = "NewItem",
    menuName = "Collectible/Item")]
public class ItemData : ScriptableObject

{
    [SerializeField] private ItemData type;
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;
    [SerializeField] private int value;
    [SerializeField] private bool isConsumable;

    public ItemData Type() { return type; }
    public string ItemName() { return itemName; }
    public Sprite Icon() { return icon; }
    public int Value() { return value; }
    

}
