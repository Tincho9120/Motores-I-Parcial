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
    private PlayerMovement playerMovement;
    private int currentSize = 2;
    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
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
}