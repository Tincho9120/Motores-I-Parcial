using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSize : MonoBehaviour
{
    [Header("Escalas")]
    [SerializeField] private float smallScale = 0.5f;
    [SerializeField] private float mediumScale = 1f;
    [SerializeField] private float largeScale = 1.5f;

    [Header("Chico")]
    [SerializeField] private float smallMoveSpeed = 6f;
    [SerializeField] private float smallSprintSpeed = 9f;
    [SerializeField] private float smallJumpHeight = 1.8f;

    [Header("Mediano")]
    [SerializeField] private float mediumMoveSpeed = 5f;
    [SerializeField] private float mediumSprintSpeed = 8f;
    [SerializeField] private float mediumJumpHeight = 1.5f;

    [Header("Grande")]
    [SerializeField] private float largeMoveSpeed = 3.5f;
    [SerializeField] private float largeSprintSpeed = 5.5f;
    [SerializeField] private float largeJumpHeight = 1f;

    [Header("Colisión al cambiar de tamaño")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float groundClearance = 0.1f;
    private PlayerMovement playerMovement;
    private CharacterController characterController;
    private int currentSize = 2;
    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        characterController = GetComponent<CharacterController>();
        ChangeSize(2);
    }
    public void OnSmall(InputValue value)
    {
        if (value.isPressed)
        {
            ChangeSize(1);
        }
    }
    public void OnMedium(InputValue value)
    {
        if (value.isPressed)
        {
            ChangeSize(2);
        }
    }
    public void OnLarge(InputValue value)
    {
        if (value.isPressed)
        {
            ChangeSize(3);
        }
    }
    private void ChangeSize(int newSize)
    {
        float targetScale = GetScaleForSize(newSize);

        if (!CanChangeSize(targetScale))
        {
            return;
        }
        currentSize = newSize;

        switch (currentSize)
        {
            case 1:
                transform.localScale =
                    new Vector3(smallScale, smallScale, smallScale);
                playerMovement.SetMovementValues(
                    smallMoveSpeed,
                    smallSprintSpeed,
                    smallJumpHeight
                );
                break;

            case 2:
                transform.localScale =
                    new Vector3(mediumScale, mediumScale, mediumScale);
                playerMovement.SetMovementValues(
                    mediumMoveSpeed,
                    mediumSprintSpeed,
                    mediumJumpHeight
                );
                break;

            case 3:
                transform.localScale =
                    new Vector3(largeScale, largeScale, largeScale);
                playerMovement.SetMovementValues(
                    largeMoveSpeed,
                    largeSprintSpeed,
                    largeJumpHeight
                );
                break;
        }
    }
    private float GetScaleForSize(int size)
    {
        switch (size)
        {
            case 1: return smallScale;
            case 3: return largeScale;
            default: return mediumScale;
        }
    }
    private bool CanChangeSize(float targetScale)
    {
        float radius = characterController.radius * targetScale;
        float height = characterController.height * targetScale;
        float halfHeight = Mathf.Max(height * 0.5f - radius, 0.01f);

        Vector3 center = transform.position + transform.rotation * (characterController.center * targetScale);
        Vector3 point1 = center + Vector3.up * halfHeight;
        Vector3 point2 = center - Vector3.up * halfHeight + Vector3.up * groundClearance;

        return !Physics.CheckCapsule(point1, point2, radius, obstacleLayer, QueryTriggerInteraction.Ignore);
    }
}