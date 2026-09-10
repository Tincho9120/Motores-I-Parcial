using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Salto")]
    [SerializeField] private float jumpHeight = 1.5f;

    [Header("Gravedad")]
    [SerializeField] private float gravity = -9.81f;

    private CharacterController characterController;

    private Vector2 moveInput;
    private float verticalVelocity;

    private bool isSprinting;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && characterController.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    public void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }

    private void Move()
    {
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);

        if (direction.magnitude > 0.1f)
        {
            direction.Normalize();

            transform.forward = Vector3.Lerp(
                transform.forward,
                direction,
                rotationSpeed * Time.deltaTime
            );
        }

        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        Vector3 movement = direction * currentSpeed;
        movement.y = verticalVelocity;

        characterController.Move(movement * Time.deltaTime);
    }
}