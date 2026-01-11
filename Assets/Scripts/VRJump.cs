using UnityEngine;
using UnityEngine.InputSystem;

public class VRJump : MonoBehaviour
{
    public InputActionProperty jumpButton;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    private CharacterController _characterController;
    private Vector3 _velocity;
    private bool _isGrounded;

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        _isGrounded = _characterController.isGrounded;

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = 0f;
        }

        if (jumpButton.action.WasPerformedThisFrame() && _isGrounded)
        {
            _velocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }

        _velocity.y += gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);
    }
}