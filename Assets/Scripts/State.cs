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
    [SerializeField] bool rainOn;
    [SerializeField] bool rainMuffled;
    [SerializeField] bool musicOn;
    [SerializeField] bool musicMuffled;
    [SerializeField] bool waterDripOn;
    [SerializeField] bool heartBeatOn;

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
    public bool[] GetClipBools()
    {
        bool[] clipBools = new bool[6];
        clipBools[0] = rainOn;
        clipBools[1] = rainMuffled;
        clipBools[2] = musicOn;
        clipBools[3] = musicMuffled;
        clipBools[4] = waterDripOn;
        clipBools[5] = heartBeatOn;
        return clipBools;
    }
}
