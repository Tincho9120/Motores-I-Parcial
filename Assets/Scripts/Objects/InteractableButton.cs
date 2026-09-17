using UnityEngine;

public class InteractableButton : MonoBehaviour, IInteractable
{
    [SerializeField] private Door door;

    public void Interact()
    {
        door.ActivatePlate();
    }
}