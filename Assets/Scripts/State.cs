using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
[CreateAssetMenu(menuName = "State")]
public class State : ScriptableObject
{
    #pragma warning disable 0649
    [TextArea(10, 14)] [SerializeField] string storyText;
    [SerializeField] State[] nextStates;
    [SerializeField] string[] nextStatesNames;
    [SerializeField] Sprite backgroundImage;
    [SerializeField] string activatedTag;
    [SerializeField] AudioClip theClip;
    [SerializeField] float clipVolume;
    [SerializeField] float clipFadeTime;
    [SerializeField] float clipDelay;
    [SerializeField] bool clipLoop;
    private float[] clipFloats;

    public string GetStateStory()
    {
        return storyText;
    }

    public State[] GetNextStates()
    {
        return nextStates;
    }
    public string[] GetNextStatesNames()
    {
        return nextStatesNames;
    }
    public Sprite GetBackgroundImage()
    {
        return backgroundImage;
    }
    public string GetActivatedTag()
    {
        return activatedTag;
    }
    public AudioClip GetAudioClip()
    {
        return theClip;
    }
    public float[] GetClipFloats()
    {
        clipFloats = new float[3];
        clipFloats[0] = clipVolume;
        clipFloats[1] = clipFadeTime;
        clipFloats[2] = clipDelay;
        return clipFloats;
    }
    public bool GetClipLoop()
    {
        return clipLoop;
    }
}
