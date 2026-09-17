using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactableLayer;

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward,
            out RaycastHit hit, interactRange, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
}