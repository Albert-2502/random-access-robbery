using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class XRSceneTransition : MonoBehaviour
{
    [SerializeField] private Renderer fadeRenderer;
    [SerializeField] private float fadeDuration = 1f;

    private Material fadeMaterial;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        fadeMaterial = fadeRenderer.material;
        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        Color color = fadeMaterial.color;
        color.a = alpha;
        fadeMaterial.color = color;
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            SetAlpha(Mathf.Lerp(0f, 1f, timer / fadeDuration));
            yield return null;
        }

        SceneManager.LoadScene(sceneName);

        yield return null;

        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            SetAlpha(Mathf.Lerp(1f, 0f, timer / fadeDuration));
            yield return null;
        }
    }
}
