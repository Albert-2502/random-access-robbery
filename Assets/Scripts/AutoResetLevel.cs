using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class AutoResetLevel : MonoBehaviour
{
    [Tooltip("The Y-height at which the scene will restart")]
    public float threshold = -5f;
    private bool isResetting = false;

    void Update()
    {
        if (isResetting) return;

        if (transform.position.y < threshold)
        {
            isResetting = true;
            TriggerFadeReset();
        }
    }

    private void TriggerFadeReset()
    {
        XRSceneTransition transition = FindObjectOfType<XRSceneTransition>();

        if (transition != null)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            transition.LoadSceneWithFade(currentSceneName);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
