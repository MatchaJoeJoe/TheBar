using UnityEngine;

public class SizeScaler : MonoBehaviour
{
    public RectTransform sizeToMatch;
    public int offset;

    // Update is called once per frame
    void Update()
    {
        Vector2 currentSize = GetComponent<RectTransform>().sizeDelta;
        float desiredHeight = sizeToMatch.sizeDelta.y + offset;
        GetComponent<RectTransform>().sizeDelta = new Vector2(currentSize.x, desiredHeight);
    }
}
