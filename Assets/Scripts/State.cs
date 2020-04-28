using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
[CreateAssetMenu(menuName = "State")]
public class State : ScriptableObject
{
    [TextArea(10, 14)] [SerializeField] string storyText;
    [SerializeField] State[] nextStates;
    [SerializeField] string[] nextStatesNames;
    [SerializeField] Sprite backgroundImage;
    [SerializeField] string activatedTag;

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
}
