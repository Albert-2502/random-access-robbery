using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class LoadSceneOnGrab : MonoBehaviour
{
    [Header("Scene Transition")]
    [SerializeField] private string sceneToLoad;
    [SerializeField] private float delaySeconds = 0f;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("RAM Grabbed!");
        if (delaySeconds > 0f)
            Invoke(nameof(LoadScene), delaySeconds);
        else
            LoadScene();
    }

    private void LoadScene()
    {
        XRSceneTransition transition = FindObjectOfType<XRSceneTransition>();
        if (transition != null)
        {
            Debug.Log("Found Transition Script - Starting Fade");
            transition.LoadSceneWithFade(sceneToLoad);
        }
        else
        {
            Debug.LogError("COULD NOT FIND XRSceneTransition script in the scene!");
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
