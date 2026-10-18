using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerClone : MonoBehaviour
{
    [Header("Clone")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private int maxClones = 2;
    [SerializeField] private float cloneCooldown = 0.3f;

    private Queue<GameObject> clones = new Queue<GameObject>();
    private List<Collider> ignoredClones = new List<Collider>();
    private float lastCloneTime = -999f;
    private bool cloningAllowed = true;
    private CharacterController characterController;
    private PlayerSize playerSize;
    [SerializeField] private float stepDistance = 1.5f;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerSize = GetComponent<PlayerSize>();
    }
    private void Update()
    {
        for (int i = ignoredClones.Count - 1; i >= 0; i--)
        {
            Collider cloneCollider = ignoredClones[i];
            if (cloneCollider == null) 
            {
                ignoredClones.RemoveAt(i);
            }
            else if (!characterController.bounds.Intersects(cloneCollider.bounds))
            {            
                Physics.IgnoreCollision(characterController, cloneCollider, false);
                ignoredClones.RemoveAt(i);
            }
        }
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
        if (maxClones <= 0) return;
        if (!cloningAllowed) return;
        if (Time.time - lastCloneTime < cloneCooldown) return;
        lastCloneTime = Time.time;
        if (clones.Count >= maxClones)
        {
            Destroy(clones.Dequeue());
        }
        GameObject newClone = Instantiate(clonePrefab, transform.position, transform.rotation);
        newClone.transform.localScale = transform.localScale;
        CloneBody cloneBody = newClone.GetComponent<CloneBody>();
        if (cloneBody != null)
        {
            cloneBody.Size = playerSize.CurrentSizeMask;
        }
        Collider cloneCollider = newClone.GetComponent<Collider>();
        if (cloneCollider != null)
        {
            Physics.IgnoreCollision(characterController, cloneCollider, true);
            ignoredClones.Add(cloneCollider);
        }
        characterController.Move(transform.forward * stepDistance * transform.localScale.x);
        clones.Enqueue(newClone);
    }
    private void DeleteOldestClone()
    {
        if (clones.Count > 0)
        {
            Destroy(clones.Dequeue());
        }
    }
}