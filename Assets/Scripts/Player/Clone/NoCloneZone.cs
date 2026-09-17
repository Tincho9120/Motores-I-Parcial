using UnityEngine;

public class NoCloneZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerClone playerClone = other.GetComponent<PlayerClone>();
        if (playerClone != null)
        {
            playerClone.SetCloningAllowed(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerClone playerClone = other.GetComponent<PlayerClone>();
        if (playerClone != null)
        {
            playerClone.SetCloningAllowed(true);
        }
    }
}