using UnityEngine;

public class InteractableButton : MonoBehaviour, IInteractable
{
    [SerializeField] private Door door;
    private bool isActivated = false;
    [SerializeField] private string prompt = "[F] Interactuar";

    [Header("Color")]
    [SerializeField] private Renderer buttonRenderer;
    [SerializeField] private Material activeMaterial;
    private Material normalMaterial;
    public string InteractionPrompt => prompt;
    private void Awake()
    {
        if (buttonRenderer != null)
        {
            normalMaterial = buttonRenderer.sharedMaterial;
        }
    }
    // Guardo el material original en el Awake, igual que en la placa
    private void SetMaterial(Material material)
    {
        if (buttonRenderer != null && material != null)
        {
            buttonRenderer.material = material;
        }
    }
    public void Interact()
    {
        isActivated = !isActivated;
        if (isActivated)
        {
            door.ActivatePlate();
            SetMaterial(activeMaterial);
        }
        else
        {
            door.DeactivatePlate();
            SetMaterial(normalMaterial);
        }

    }
 
}