using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleAnimation : MonoBehaviour
{
    [SerializeField] private RawImage title;

    [SerializeField] private float minScale = 0.9f; 
    [SerializeField] private float maxScale = 1.1f; 
    [SerializeField] private float animationDuration = 2.0f; 

    void Start()
    {
        StartCoroutine(TextAnimation());
    }

    IEnumerator TextAnimation()
    {
        RectTransform rectTransform = title.rectTransform;
        Vector3 originalScale = rectTransform.localScale;
        Vector3 targetScale = new Vector3(maxScale, maxScale, 1f);

        while (true)
        {
            yield return StartCoroutine(ScaleOverTime(rectTransform, originalScale, targetScale, animationDuration / 2));
            yield return StartCoroutine(ScaleOverTime(rectTransform, targetScale, originalScale, animationDuration / 2));
        }
    }

    IEnumerator ScaleOverTime(RectTransform rectTransform, Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            rectTransform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.localScale = endScale;
    }
}