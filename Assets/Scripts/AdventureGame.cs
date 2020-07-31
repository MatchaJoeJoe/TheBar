using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions.Must;
using UnityEngine.Audio;

public class AdventureGame : MonoBehaviour
{
    #pragma warning disable 0649
    [Header("UI")]
    [SerializeField] private ImageFade textBGFade;
    [SerializeField] private TextFade textFade;
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private GameObject buttonHolder;
    [SerializeField] private GameObject theButtonPrefab;
    [SerializeField] private GameObject theBGImage1;
    [SerializeField] private GameObject theBGImage2;
    [Header("Audio")]
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private GameObject rainAudio;
    [SerializeField] private GameObject musicAudio;
    [SerializeField] private AudioSource waterDripSource;
    [SerializeField] private AudioSource heartBeatSource;
    [SerializeField] private Image audioController;
    [SerializeField] private Sprite audioOn;
    [SerializeField] private Sprite audioOff;
    [Header("Misc")]
    [SerializeField] private State startingState;
    [SerializeField] private float startDelay;
    private Image theBGImage1image;
    private Image theBGImage2image;
    private ImageFade theBGImage2imageFade;
    private AdventureGame theGame;
    private ImageFade buttonHolderFade;
    private AudioSource rainSource;
    private AudioSource musicSource;
    private AudioLowPassFilter rainFilter;
    private AudioLowPassFilter musicFilter;
    private float masterVolume;
    private bool isMuted = false;
    [Header("Debug")]
    [SerializeField] private GameObject[] activatableObjs;
    [SerializeField] private State currentState; //serialized for debugging

    // Start is called before the first frame update
    void Start()
    {
        // Getting all the objects marked Toggleable
        GetActivatableObjs();
        // Getting components from objects
        theGame = FindObjectOfType<AdventureGame>();
        theBGImage1image = theBGImage1.GetComponent<Image>();
        theBGImage2image = theBGImage2.GetComponent<Image>();
        theBGImage2imageFade = theBGImage2.GetComponent<ImageFade>();
        buttonHolderFade = buttonHolder.GetComponent<ImageFade>();
        rainSource = rainAudio.GetComponent<AudioSource>();
        musicSource = musicAudio.GetComponent<AudioSource>();
        rainFilter = rainAudio.GetComponent<AudioLowPassFilter>();
        musicFilter = musicAudio.GetComponent<AudioLowPassFilter>();
        masterMixer.GetFloat("masterVolume", out masterVolume);
        // Transitioning to starting state
        NextState(startingState);
    }
    private void GetActivatableObjs()
    {
        activatableObjs = GameObject.FindGameObjectsWithTag("Toggleable");
        foreach (GameObject activatableObj in activatableObjs)
        {
            activatableObj.SetActive(false);
        }
    }
    public void NextState(State nextState)
    {
        // Removing old buttons
        foreach (Transform child in buttonHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        StartCoroutine(StateTransition(nextState));
    }
    IEnumerator StateTransition(State nextState)
    {
        // Toggling activated objects from states
        if (currentState != null)
        {
            string[] theTags = currentState.GetActivatedTags();
            foreach(string thisTag in theTags)
            {
                ToggleObjByName(thisTag);
            }
        }
        string[] nextTags = nextState.GetActivatedTags();
        foreach (string nextTag in nextTags)
        {
            ToggleObjByName(nextTag);
        }

        // Updating state 
        currentState = nextState;

        // Fading old state out
        textBGFade.FadeOut();
        textFade.FadeOut();
        buttonHolderFade.FadeOut();

        // Transition Audio
        bool[] clipBools = nextState.GetClipBools();
        bool rainOn = clipBools[0];
        bool rainMuffled = clipBools[1];
        bool musicOn = clipBools[2];
        bool musicMuffled = clipBools[3];
        bool waterDripOn = clipBools[4];
        bool heartBeatOn = clipBools[5];
        rainFilter.enabled = rainMuffled;
        rainSource.mute = !rainOn;
        rainFilter.enabled = rainMuffled;
        musicSource.mute = !musicOn;
        musicFilter.enabled = musicMuffled;
        waterDripSource.mute = !waterDripOn;
        heartBeatSource.mute = !heartBeatOn;

        // Updating background image
        Sprite nextSprite = nextState.GetBackgroundImage();
        if (nextSprite != null)
        {
            BackgroundFade(nextSprite);
            yield return new WaitForSeconds(0.1f);
            while (theBGImage2imageFade.CheckFading())
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
        yield return new WaitForSeconds(0.1f);
        while (textBGFade.CheckFading() || textFade.CheckFading() || buttonHolderFade.CheckFading()) {
            yield return new WaitForSeconds(0.1f);
        }
        textComponent.text = nextState.GetStateStory();

        // Adding new buttons
        var nextStates = nextState.GetNextStates();
        var nextStatesNames = nextState.GetNextStatesNames();
        var index = 0;
        foreach (State child in nextStates)
        {
            GameObject newButton = Instantiate(theButtonPrefab);
            newButton.transform.SetParent(buttonHolder.transform, false);
            string nextStateName = nextStatesNames[index];
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = nextStateName;
            newButton.GetComponentInChildren<Button>().onClick.AddListener(() => { theGame.NextState(child); });
            index++;
        }

        // Fading in buttons
        float delayTime = 1f;
        if (GetCurrentStateName() == startingState.name)
        {
            delayTime = startDelay;
        }
        StartCoroutine(ButtonsFadeIn());

        textBGFade.FadeIn();
        textFade.FadeIn();
        buttonHolderFade.FadeIn();
    }
        IEnumerator ButtonsFadeIn()
    {
        yield return new WaitForSeconds(0.1f);
        while (textBGFade.CheckFading() || textFade.CheckFading() || buttonHolderFade.CheckFading())
        {
            yield return new WaitForSeconds(0.1f);
        }
        ButtonFade[] theButtons = FindObjectsOfType<ButtonFade>();
        ButtonFade previousButton = null;
        for (int i = theButtons.Length - 1; i >= 0; i--)
        {
            if(previousButton != null)
            {
                yield return new WaitForSeconds(0.1f);
                while (previousButton.CheckFading())
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
            theButtons[i].FadeIn();
            previousButton = theButtons[i];
        }
    }
    private void BackgroundFade(Sprite newSprite)
    {
        // checking which image to set new sprite on cross fade
        if (theBGImage2imageFade.GetTargetAlpha() == 0)
        {
            theBGImage2image.sprite = newSprite;
            theBGImage2imageFade.FadeIn();
        } else
        {
            theBGImage1image.sprite = newSprite;
            theBGImage2imageFade.FadeOut();
        }
    }

    private void ToggleObjByName(string objName)
    {
        foreach (GameObject activatedObj in activatableObjs)
        {
            if (activatedObj.name == objName)
            {
                activatedObj.gameObject.SetActive(!activatedObj.gameObject.activeSelf);
            }
        }
    }
    public string GetCurrentStateName()
    {
        return currentState.name;
    }
    public void ToggleAudio()
    {
        if (isMuted)
        {
            audioController.sprite = audioOn;
            masterMixer.SetFloat("masterVolume", masterVolume);
            isMuted = false;
        } else
        {
            audioController.sprite = audioOff;
            masterMixer.SetFloat("masterVolume", -80f);
            isMuted = true;
        }
    }
}
