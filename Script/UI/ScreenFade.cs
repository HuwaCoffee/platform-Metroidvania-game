
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFade : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1f;

    public IEnumerator FadeOut() {
        float elapsed = 0;
        while (elapsed < fadeDuration) {
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    public IEnumerator FadeIn() {
        float elapsed = 0;
        while (elapsed < fadeDuration) {
            canvasGroup.alpha = 1 - Mathf.Clamp01(elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
    }
}