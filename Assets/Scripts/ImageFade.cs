using UnityEngine;
using UnityEngine.UI;

public class ImageFade : MonoBehaviour
{
    [SerializeField] private float FadeRate;
    private bool isFading = false;
    private Image[] images;
    private float targetAlpha;
    void Start()
    {
        images = GetComponentsInChildren<Image>();
        if (images.Length == 0)
        {
            Debug.LogError("Error: No image on " + this.name);
        }
        targetAlpha = images[0].color.a;
    }

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
            } else
            {
                curColor.a = targetAlpha;
                image.color = curColor;

            }
        }
        if (thisFading)
        {
            isFading = true;
        }
        else
        {
            isFading = false;
        }
    }
    public bool CheckFading()
    {
        return isFading;
    }

    public float GetTargetAlpha()
    {
        return targetAlpha;
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
