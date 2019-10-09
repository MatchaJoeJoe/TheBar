using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonFade : MonoBehaviour
{
    [SerializeField] private float FadeRate;
    private Image[] images;
    private TextMeshProUGUI tmpText;
    private float targetAlpha; //serialized for debugging
    private bool isFading = false;
    // Use this for initialization
    void Start()
    {
        // getting image
        images = GetComponentsInChildren<Image>();
        if (images.Length == 0)
        {
            Debug.LogError("Error: No image on " + name);
        }
        targetAlpha = images[0].color.a;
        // getting text
        tmpText = GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText == null)
        {
            Debug.LogError("Error: No text on " + name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool thisFading = false;
        foreach (Image image in images)
        {
            Color curColor = image.color;
            float alphaDiff = Mathf.Abs(curColor.a - targetAlpha);
            if (alphaDiff > 0.01f)
            {
                thisFading = true;
                curColor.a = Mathf.Lerp(curColor.a, targetAlpha, FadeRate * Time.deltaTime);
                image.color = curColor;
            }
            else
            {
                curColor.a = targetAlpha;
                image.color = curColor;

            }
        }
        Color curTextColor = tmpText.color;
        float alphaDiffText = Mathf.Abs(curTextColor.a - targetAlpha);
        if (alphaDiffText > 0.01f)
        {
            thisFading = true;
            curTextColor.a = Mathf.Lerp(curTextColor.a, targetAlpha, FadeRate * Time.deltaTime);
            tmpText.color = curTextColor;
        } else
        {
            curTextColor.a = targetAlpha;
            tmpText.color = curTextColor;
        }
        if (thisFading)
        {
            isFading = true;
        } else
        {
            isFading = false;
        }
    }

    public bool CheckFading()
    {
        return isFading;
    }
    public void FadeOut()
    {
        targetAlpha = 0.0f;
    }

    public void FadeIn()
    {
        targetAlpha = 1.0f;
    }
}
