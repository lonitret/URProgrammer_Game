using UnityEngine;
using System.Collections;

public class CoffeeMachineModule : InteractiveModule
{
    public override void Interact()
    {
        if (!isActive) return;

        Debug.Log("получаем чашку кофе");
    }
}