using UnityEngine;
using UnityEngine.InputSystem;

public class VRPauseMenuToggle : MonoBehaviour
{
    [Header("Pause Menu Settings")]
    [SerializeField] private GameObject pauseMenu;

    [Tooltip("Typically the XR Camera (Main Camera)")]
    [SerializeField] private Transform playerCamera;

    [Tooltip("Distance in meters in front of the player")]
    [SerializeField] private float menuDistance = 1.5f;

    [Header("Input Settings")]
    [SerializeField] private InputActionReference toggleAction;

    private bool isPaused;

    private void OnEnable()
    {
        if (toggleAction != null)
        {
            toggleAction.action.performed += OnTogglePerformed;
            toggleAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (toggleAction != null)
        {
            toggleAction.action.performed -= OnTogglePerformed;
            toggleAction.action.Disable();
        }
    }

    private void OnTogglePerformed(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    private void TogglePause()
    {
        if (pauseMenu == null || playerCamera == null)
            return;

        isPaused = !isPaused;

        if (isPaused)
        {
            PositionMenuInFrontOfPlayer();
        }

        pauseMenu.SetActive(isPaused);
        //Time.timeScale = isPaused ? 0f : 1f;
    }

    private void PositionMenuInFrontOfPlayer()
    {
        Vector3 forward = playerCamera.forward;
        forward.y = 0f; // Keep menu level
        forward.Normalize();

        Vector3 targetPosition = playerCamera.position + forward * menuDistance;

        pauseMenu.transform.position = targetPosition;

        // Rotate menu to face the player
        pauseMenu.transform.rotation = Quaternion.LookRotation(forward);
    }
}
