using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerClone : MonoBehaviour
{
    [Header("Clone")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private int maxClones = 2;

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;

    private Queue<GameObject> clones = new Queue<GameObject>();

    private PlayerMovement playerMovement;
    private CharacterController characterController;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        characterController = GetComponent<CharacterController>();
    }

    public void OnClone(InputValue value)
    {
        if (value.isPressed)
        {
            CreateClone();
        }
    }
    private void CreateClone()
    {
        if (maxClones <= 0)
        {
            return;
        }
        // Guardamos la posicion, rotacion y escala actual del jugador
        Vector3 clonePosition = transform.position;
        Quaternion cloneRotation = transform.rotation;
        Vector3 cloneScale = transform.localScale;
        // Si llegamos al limite, eliminamos el clon mas viejo
        if (clones.Count >= maxClones)
        {
            GameObject oldestClone = clones.Dequeue();
            Destroy(oldestClone);
        }
        // Teletransportamos al jugador al respawn
        characterController.enabled = false;
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;
        characterController.enabled = true;
        playerMovement.ResetVerticalVelocity();

        // Creamos el clon donde estaba el jugador
        GameObject newClone = Instantiate(
            clonePrefab,
            clonePosition,
            cloneRotation
        );
        // El clon mantiene el tamaño que tenia el jugador
        newClone.transform.localScale = cloneScale;
        // Lo agregamos a la cola
        clones.Enqueue(newClone);
    }
}