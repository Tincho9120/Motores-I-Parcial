using UnityEngine;
using UnityEngine.InputSystem;

public class VictoryTrigger : MonoBehaviour
{
    [SerializeField] private GameObject victoryText;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerInput playerInput = other.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }

        victoryText.SetActive(true);
    }
}