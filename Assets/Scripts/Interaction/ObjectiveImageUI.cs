using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Controls the display and transition of objective UI elements at the start of gameplay
public class ObjectiveImageUI : MonoBehaviour
{
    public RawImage startObjective;
    public RawImage cornerObjective;

    public float startDelay = 2f;
    public float displayTime = 4f;
    public float betweenDelay = 1f;

    public float fadeSpeed = 2f;

    void Start()
    {
        // Ensure both UI elements are hidden before sequence begins
        startObjective.gameObject.SetActive(false);
        cornerObjective.gameObject.SetActive(false);

        StartCoroutine(ObjectiveSequence());
    }

    // Handles the full sequence of showing the initial objective and transitioning to the corner UI
    IEnumerator ObjectiveSequence()
    {
        // Initial delay before showing first objective
        yield return new WaitForSeconds(startDelay);

        // Fade in main objective display
        startObjective.gameObject.SetActive(true);
        yield return StartCoroutine(FadeIn(startObjective));

        // Keep objective visible for a set duration
        yield return new WaitForSeconds(displayTime);

        // Fade out main objective
        yield return StartCoroutine(FadeOut(startObjective));
        startObjective.gameObject.SetActive(false);

        // Delay before transitioning to persistent corner objective
        yield return new WaitForSeconds(betweenDelay);

        // Fade in corner objective UI
        cornerObjective.gameObject.SetActive(true);
        yield return StartCoroutine(FadeIn(cornerObjective));
    }

    // Gradually increases alpha to make UI element visible
    IEnumerator FadeIn(RawImage img)
    {
        Color c = img.color;
        c.a = 0;
        img.color = c;

        while (img.color.a < 1)
        {
            c.a += Time.deltaTime * fadeSpeed;
            img.color = c;
            yield return null;
        }
    }

    // Gradually decreases alpha to hide UI element
    IEnumerator FadeOut(RawImage img)
    {
        Color c = img.color;

        while (img.color.a > 0)
        {
            c.a -= Time.deltaTime * fadeSpeed;
            img.color = c;
            yield return null;
        }
    }
}
