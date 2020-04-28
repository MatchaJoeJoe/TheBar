using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFade : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] AudioSource theAudioSource;
    [SerializeField] float targetVolume;
    [SerializeField] float lerpAmount = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        theAudioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        float currentVolume = theAudioSource.volume;
        if (currentVolume != targetVolume)
        {
            theAudioSource.volume = Mathf.Lerp(currentVolume, targetVolume, lerpAmount);
            currentVolume = theAudioSource.volume;
            if (Mathf.Abs(currentVolume - targetVolume) < .01)
            {
                currentVolume = targetVolume;
            }
        }
    }
    public void FadeToVolume(float newVolume, float seconds)
    {
        targetVolume = newVolume;
        lerpAmount = seconds * Time.deltaTime;
    }
}
