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
            if (objectsOnPlate == 1 && door != null)
            {
                door.ActivatePlate();
            }
            if (other.CompareTag("Clone"))
            {
                CloneBody cloneBody = other.GetComponent<CloneBody>();
                if (cloneBody != null)
                {
                    cloneBody.OnCloneDestroyed += HandleCloneDestroyed; // suscripcion
                }
            }

            if (objectsOnPlate == 1)
            {
                door.ActivatePlate();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        RemoveObjectFromPlate();
    }
    private void HandleCloneDestroyed()
    {

        RemoveObjectFromPlate();
    }
    private void RemoveObjectFromPlate()
    {
        objectsOnPlate--;
        if (objectsOnPlate <= 0)
        {
            objectsOnPlate = 0;

            if (door != null)
            {
                door.DeactivatePlate();
            }
        }
    }
    public void RemovePlayer()
    {
        RemoveObjectFromPlate();
    }
}