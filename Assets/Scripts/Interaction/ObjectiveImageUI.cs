using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
        startObjective.gameObject.SetActive(false);
        cornerObjective.gameObject.SetActive(false);

        StartCoroutine(ObjectiveSequence());
    }

    IEnumerator ObjectiveSequence()
    {
        yield return new WaitForSeconds(startDelay);

        startObjective.gameObject.SetActive(true);
        yield return StartCoroutine(FadeIn(startObjective));

        yield return new WaitForSeconds(displayTime);

        yield return StartCoroutine(FadeOut(startObjective));
        startObjective.gameObject.SetActive(false);

        yield return new WaitForSeconds(betweenDelay);

        cornerObjective.gameObject.SetActive(true);
        yield return StartCoroutine(FadeIn(cornerObjective));
    }

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