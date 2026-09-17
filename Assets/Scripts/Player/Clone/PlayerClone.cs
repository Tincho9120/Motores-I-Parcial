using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerClone : MonoBehaviour
{
    [Header("Clone")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private int maxClones = 2;
    [SerializeField] private float cloneCooldown = 0.3f;

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;

    private Queue<GameObject> clones = new Queue<GameObject>();
    private float lastCloneTime = -999f;
    private bool cloningAllowed = true;
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
    public void OnDeleteClone(InputValue value)
    {
        if (value.isPressed)
        {
            DeleteOldestClone();
        }
    }
    public void SetCloningAllowed(bool allowed)
    {
        cloningAllowed = allowed;
    }
    private void CreateClone()
    {
        if (maxClones <= 0)
        {
            return;
        }
        // Guardamos la posicion, rotacion y escala actual del jugador
        if (!cloningAllowed) return;
        if (Time.time - lastCloneTime < cloneCooldown) return;
        lastCloneTime = Time.time;
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
        NotifyPlatesBeforeLeaving();
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
    private void DeleteOldestClone()
    {
        if (clones.Count > 0)
        {
            GameObject oldestClone = clones.Dequeue();
            Destroy(oldestClone);
        }
    }
    private void NotifyPlatesBeforeLeaving()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, characterController.radius);
        foreach (var hit in hits)
        {
            PressurePlate plate = hit.GetComponent<PressurePlate>();
            if (plate != null)
            {
                plate.RemovePlayer();
            }
        }
    }
}