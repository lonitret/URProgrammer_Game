using UnityEngine;

public abstract class InteractiveModule : MonoBehaviour
{
    [SerializeField] protected bool isActive = true;
    public bool IsActive => isActive;

    public virtual void Activate() => isActive = true;
    public virtual void Deactivate() => isActive = false;

    public abstract void Interact();
}