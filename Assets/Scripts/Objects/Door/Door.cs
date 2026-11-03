using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private int requiredPlates = 1;

    [Header("Sonidos")]
    [SerializeField] private AudioClip openClip;
    [SerializeField] private AudioClip closeClip;

    private int activePlates = 0;
    private bool isOpen = false;
    private MeshRenderer meshRenderer;
    private Collider doorCollider;
    private AudioSource audioSource;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        doorCollider = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
    }

    public void ActivatePlate()
    {
        activePlates++;

        if (activePlates >= requiredPlates)
        {
            OpenDoor();
        }
    }

    public void DeactivatePlate()
    {
        activePlates--;

        if (activePlates < 0)
        {
            activePlates = 0;
        }

        if (activePlates < requiredPlates)
        {
            CloseDoor();
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Apaga el mesh y el collider en vez del objeto entero, asi el AudioSource puede sonar
    private void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;
        PlaySound(openClip);
        ShowDoor(false);
    }

    private void CloseDoor()
    {
        if (!isOpen) return;

        isOpen = false;
        PlaySound(closeClip);
        ShowDoor(true);
    }

    private void ShowDoor(bool visible)
    {
        meshRenderer.enabled = visible;
        doorCollider.enabled = visible;
    }
}