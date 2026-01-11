using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private InputActionProperty jumpButton;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private CharacterController cc;
    [SerializeField] private LayerMask groundLayers;

    private float gravity = Physics.gravity.y;
    private Vector3 movement;

    void Update()
    {
        bool _isGrounded = IsGrounded();

        if (jumpButton.action.WasPressedThisFrame() && _isGrounded)
        {
            Jump();
        }

        movement.y += gravity * Time.deltaTime;
        cc.Move(movement * Time.deltaTime);
    }

    public void Jump()
    {
        movement.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
    }

    public bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position, 0.2f, groundLayers);
    }
}
