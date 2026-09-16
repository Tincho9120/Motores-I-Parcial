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
            Debug.Log($"[{name}] Enter: {other.name}, objectsOnPlate={objectsOnPlate}");
            if (other.CompareTag("Clone"))
            {
                CloneBody cloneBody = other.GetComponent<CloneBody>();
                if (cloneBody != null)
                {
                    cloneBody.OnCloneDestroyed += HandleCloneDestroyed; // suscripcion
                    Debug.Log($"[{name}] Suscripto a {other.name}");
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
        Debug.Log($"[{name}] Recibí evento de destrucción, objectsOnPlate antes={objectsOnPlate}");

        RemoveObjectFromPlate();
    }
    private void RemoveObjectFromPlate()
    {
        objectsOnPlate--;
        Debug.Log($"[{name}] objectsOnPlate ahora={objectsOnPlate}");


        if (objectsOnPlate <= 0)
        {
            objectsOnPlate = 0;
            door.DeactivatePlate();
        }
    }
    public void RemovePlayer()
    {
        RemoveObjectFromPlate();
    }
}