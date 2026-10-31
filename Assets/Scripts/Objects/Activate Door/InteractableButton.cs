using UnityEngine;

public class InteractableButton : MonoBehaviour, IInteractable
{
    [SerializeField] private Door door;
    private bool isActivated = false;
    [SerializeField] private string prompt = "[F] Interactuar";
    public string InteractionPrompt => prompt;
    public void Interact()
    {
        isActivated = !isActivated;
        if (isActivated)
        {
            door.ActivatePlate();
        }
        else
        {
            door.DeactivatePlate();
        }

    }
 
}