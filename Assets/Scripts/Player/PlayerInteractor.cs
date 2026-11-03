using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactableLayer;
    public event Action<string> OnPromptChanged;
    private IInteractable current;
    private void Update()
    {
        IInteractable found = FindInteractable();
        if (found != current)
        {
            current = found;
            OnPromptChanged?.Invoke(current != null ? current.InteractionPrompt : null);
        }
    }
    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;

        current?.Interact();
    }
    private IInteractable FindInteractable()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward,
            out RaycastHit hit, interactRange, interactableLayer))
        {
            return hit.collider.GetComponent<IInteractable>();
        }
        return null;
    }
}