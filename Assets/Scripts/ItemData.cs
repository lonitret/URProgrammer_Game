using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    [TextArea] public string description;

    public bool isCoffee;
    public float stressRelief;
    public float speedBoostMultiplier = 1f;
    public float speedBoostDuration = 0f;
    public bool isTool;
}