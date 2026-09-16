using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private Door door;

    private int objectsOnPlate = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Clone"))
        {
            objectsOnPlate++;

            if (objectsOnPlate == 1)
            {
                door.ActivatePlate();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Clone"))
        {
            objectsOnPlate--;

            if (objectsOnPlate <= 0)
            {
                objectsOnPlate = 0;
                door.DeactivatePlate();
            }
        }
    }
}