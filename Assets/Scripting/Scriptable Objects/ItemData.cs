using UnityEngine;

[CreateAssetMenu(fileName = "NewItem",
    menuName = "Collectible/Item")]
public class ItemData : ScriptableObject

{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;
    [SerializeField] private int value;
    [SerializeField] private bool isConsumable;

}
