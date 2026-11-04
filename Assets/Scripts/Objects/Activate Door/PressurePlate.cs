using UnityEngine;
using System.Collections.Generic;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private Door door;
    [SerializeField] private SizeMask allowedSizes = SizeMask.Small | SizeMask.Medium | SizeMask.Large;
    [SerializeField] private bool acceptsBoxes = true;

    [Header("Sonidos")]
    [SerializeField] private AudioClip activateClip;
    [SerializeField] private AudioClip deactivateClip;

    [Header("Color")]
    [SerializeField] private Material activeMaterial;
    private int objectsOnPlate = 0;
    private bool playerCounted = false;
    private readonly HashSet<PushableBox> boxesOnPlate = new HashSet<PushableBox>();
    private AudioSource audioSource;
    private Renderer plateRenderer;
    private Material normalMaterial;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        plateRenderer = GetComponent<Renderer>();

        if (plateRenderer != null)
        {
            normalMaterial = plateRenderer.sharedMaterial;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PushableBox box = other.GetComponentInParent<PushableBox>();
        if (box != null)
        {
            if (acceptsBoxes && boxesOnPlate.Add(box))
            {
                AddObjectToPlate();
            }
            return;
        }
        if (!other.CompareTag("Clone")) return;

        CloneBody cloneBody = other.GetComponent<CloneBody>();
        if (cloneBody == null || !IsAllowed(cloneBody.Size)) return;

        cloneBody.OnCloneDestroyed += HandleCloneDestroyed;
        AddObjectToPlate();
    }

    // Stay (y no Enter) para el jugador: puede cambiar de tamaño estando parado encima
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerSize playerSize = other.GetComponent<PlayerSize>();
        if (playerSize == null) return;

        bool allowed = IsAllowed(playerSize.CurrentSizeMask);

        if (allowed && !playerCounted)
        {
            playerCounted = true;
            AddObjectToPlate();
        }
        else if (!allowed && playerCounted)
        {
            playerCounted = false;
            RemoveObjectFromPlate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PushableBox box = other.GetComponentInParent<PushableBox>();
        if (box != null)
        {
            if (boxesOnPlate.Remove(box))
            {
                RemoveObjectFromPlate();
            }
            return;
        }
        if (!other.CompareTag("Player")) return;
        RemovePlayer();
    }

    public void RemovePlayer()
    {
        if (!playerCounted) return;

        playerCounted = false;
        RemoveObjectFromPlate();
    }

    private void HandleCloneDestroyed()
    {
        RemoveObjectFromPlate();
    }

    private bool IsAllowed(SizeMask size)
    {
        return (allowedSizes & size) != 0;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Guardo el material original en el Awake, asi solo hay que cargar el de activada
    private void SetMaterial(Material material)
    {
        if (plateRenderer != null && material != null)
        {
            plateRenderer.material = material;
        }
    }

    private void AddObjectToPlate()
    {
        objectsOnPlate++;

        if (objectsOnPlate == 1 && door != null)
        {
            SetMaterial(activeMaterial);
            PlaySound(activateClip);
            door.ActivatePlate();
        }
    }

    private void RemoveObjectFromPlate()
    {
        objectsOnPlate--;

        if (objectsOnPlate <= 0)
        {
            objectsOnPlate = 0;

            if (door != null)
            {
                SetMaterial(normalMaterial);
                PlaySound(deactivateClip);
                door.DeactivatePlate();
            }
        }
    }
}