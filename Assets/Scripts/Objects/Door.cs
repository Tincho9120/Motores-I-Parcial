using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private int requiredPlates = 1;

    private int activePlates = 0;

    public void ActivatePlate()
    {
        activePlates++;
        Debug.Log($"{name}: ActivatePlate, activePlates={activePlates}, requiredPlates={requiredPlates}");

        if (activePlates >= requiredPlates)
        {
            Debug.Log($"{name}: llamando OpenDoor");
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

    private void OpenDoor()
    {
        gameObject.SetActive(false);
    }

    private void CloseDoor()
    {
        gameObject.SetActive(true);
    }
}