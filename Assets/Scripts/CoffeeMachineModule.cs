using UnityEngine;
using System.Collections;

public class CoffeeMachineModule : InteractiveModule
{
    [SerializeField] private ItemData coffeeItem;


    public override void Interact()
    {
        if (!isActive) return;

        if (coffeeItem != null)
        {
            bool success = InventoryManager.Instance.AddItem(coffeeItem, 1);

            if (success)
            {
                Debug.Log("Получаем чашку кофе");
                AudioManager.Instance.PlaySFX(SoundType.CoffeeMachine);
            }
        }
        else
        {
            Debug.LogError("нет кофе в инспекторе кофемашины");
        }
    }
}