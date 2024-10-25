using System.Collections;
using UnityEngine;
using TMPro;

public class DynamicTextDisplay : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public TextAsset textFile;
    public Vector3 shakeAmount = new Vector3(2f, 2f, 0f);
    public float shakeDuration = 0.5f;

    private string[] lines;
    private int currentLineIndex = 0;
    private RectTransform textTransform;
    private Vector3 originalPos;

    void Start()
    {
        lines = textFile.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        textTransform = textDisplay.GetComponent<RectTransform>();
        originalPos = textTransform.anchoredPosition;
        StartCoroutine(DisplayTextWithShakeCoroutine());
    }

    IEnumerator DisplayTextWithShakeCoroutine()
    {
        yield return null;

        while (true)
        {
            string line = lines[currentLineIndex].TrimEnd();
            StartCoroutine(ShakeText());

            (string displayLine, float displayDuration) = ExtractDuration(line);
            textDisplay.text = displayLine;

            yield return new WaitForSeconds(displayDuration);

            currentLineIndex++;
            if (currentLineIndex >= lines.Length)
            {
                currentLineIndex = 0;
            }
        }
    }

    IEnumerator ShakeText()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-shakeAmount.x, shakeAmount.x),
                Random.Range(-shakeAmount.y, shakeAmount.y),
                0f
            );

            textTransform.anchoredPosition = originalPos + randomOffset;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        textTransform.anchoredPosition = originalPos; 
    }

    (string, float) ExtractDuration(string line)
    {
        string[] parts = line.Split('/');
        if (parts.Length > 1)
        {
            string durationPart = parts[0].Trim();
            float displayDuration = ParseDurationToSeconds(durationPart);
            string displayLine = parts[1].Trim();
            return (displayLine, displayDuration);
        }
        return (line, 1f);
    }

    float ParseDurationToSeconds(string duration)
    {
        string[] timeParts = duration.Split(':');
        float totalSeconds = 0f;

        if (timeParts.Length == 2 &&
            int.TryParse(timeParts[0], out int minutes) &&
            int.TryParse(timeParts[1], out int frames))
        {
            totalSeconds = (minutes * 60) + (frames / 30f);
        }
        return totalSeconds;
    }
}
