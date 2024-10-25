using UnityEngine;
using UnityEngine.UI;

public class NyanCatEffect : MonoBehaviour
{
    public RawImage rawImage;
    public float heightChangeSpeed = 0.5f;
    public float colorChangeSpeed = 1.0f;
    public float heightChangeRange = 10.0f;
    private float currentHeight;
    private RectTransform rectTransform;
    private Vector2 startPosition;

    void Start()
    {
        if (rawImage == null)
        {
            rawImage = GetComponent<RawImage>();
        }

        rectTransform = rawImage.GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
        currentHeight = rectTransform.sizeDelta.y;
    }

    void Update()
    {
        // 計算新的高度
        currentHeight = Mathf.PingPong(Time.time * heightChangeSpeed, heightChangeRange) + 128.39f;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, currentHeight);

        // 獲取當前顏色並保持 alpha 值為 1
        Color currentColor = rawImage.color;
        rawImage.color = Color.HSVToRGB(Mathf.PingPong(Time.time * colorChangeSpeed, 1), 1, 1);
        rawImage.color = new Color(rawImage.color.r, rawImage.color.g, rawImage.color.b, currentColor.a);
    }
}