using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private int requiredPlates = 1;

    [Header("Hojas")]
    [SerializeField] private Transform leftPanel;
    [SerializeField] private Transform rightPanel;
    [SerializeField] private float slideDistance = 10f;
    [SerializeField] private float slideSpeed = 4f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip openClip;
    [SerializeField] private AudioClip closeClip;

    private int activePlates = 0;
    private bool isOpen = false;
    private AudioSource audioSource;
    private Vector3 leftClosedPosition;
    private Vector3 rightClosedPosition;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (leftPanel != null)
        {
            leftClosedPosition = leftPanel.localPosition;
        }

        if (rightPanel != null)
        {
            rightClosedPosition = rightPanel.localPosition;
        }
    }

    private void Update()
    {
        SlidePanel(leftPanel, leftClosedPosition, -slideDistance);
        SlidePanel(rightPanel, rightClosedPosition, slideDistance);
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

    private void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;
        PlaySound(openClip);
    }

    private void CloseDoor()
    {
        if (!isOpen) return;

        isOpen = false;
        PlaySound(closeClip);
    }

    // Cada hoja se corre para su costado sobre el eje X, y vuelve al lugar cuando se cierra
    private void SlidePanel(Transform panel, Vector3 closedPosition, float offset)
    {
        if (panel == null) return;

        Vector3 target = closedPosition;

        if (isOpen)
        {
            target.x += offset;
        }

        panel.localPosition = Vector3.MoveTowards(panel.localPosition, target, slideSpeed * Time.deltaTime);
    }
}