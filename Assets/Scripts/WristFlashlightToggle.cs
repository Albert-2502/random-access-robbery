using UnityEngine;
using UnityEngine.InputSystem;

public class WristFlashlightToggle : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Light targetLight;

    [Header("Input Settings")]
    [SerializeField] private InputActionReference toggleAction;

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
        if (targetLight == null)
            return;

        targetLight.enabled = !targetLight.enabled;
    }
}
