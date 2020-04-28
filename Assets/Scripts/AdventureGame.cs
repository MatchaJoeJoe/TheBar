using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;

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
    [SerializeField] private GameObject audioObj1;
    [SerializeField] private GameObject audioObj2;
    [Header("Misc")]
    [SerializeField] private State startingState;
    [SerializeField] private float startDelay;
    private Image theBGImage1image;
    private Image theBGImage2image;
    private ImageFade theBGImage2imageFade;
    private AdventureGame theGame;
    private ImageFade buttonHolderFade;
    private AudioSource audioSource1;
    private AudioSource audioSource2;
    private AudioFade audioFade1;
    private AudioFade audioFade2;
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
        audioSource1 = audioObj1.GetComponent<AudioSource>();
        audioSource2 = audioObj2.GetComponent<AudioSource>();
        audioFade1 = audioObj1.GetComponent<AudioFade>();
        audioFade2 = audioObj2.GetComponent<AudioFade>();
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
            string thisTag = currentState.GetActivatedTag();
            ToggleObjByName(thisTag);
        }
        string nextTag = nextState.GetActivatedTag();
        ToggleObjByName(nextTag);

        // Updating state 
        currentState = nextState;

        // Fading old state out
        textBGFade.FadeOut();
        textFade.FadeOut();
        buttonHolderFade.FadeOut();

        // Transition Audio
        AudioClip nextClip = nextState.GetAudioClip();
        if (nextClip != null)
        {
            AudioFade(nextClip, nextState.GetClipFloats(), nextState.GetClipLoop());
            yield return new WaitForSeconds(0.1f);
            while (theBGImage2imageFade.CheckFading())
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

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

    private void AudioFade(AudioClip nextClip, float[] clipData, bool clipLoop)
    {
        StartCoroutine(AudioFadeRoutine(nextClip, clipData, clipLoop));
    }

    IEnumerator AudioFadeRoutine(AudioClip nextClip, float[] clipFloats, bool clipLoop)
    {
        float clipVolume = clipFloats[0];
        float clipFadeTime = clipFloats[1];
        float clipDelay = clipFloats[2];
        if (audioSource1.isPlaying)
        {
            audioFade1.FadeToVolume(0, clipFadeTime);
        } else
        {
            audioFade2.FadeToVolume(0, clipFadeTime);
        }
        yield return new WaitForSeconds(clipDelay);
        if (audioSource1.isPlaying)
        {
            audioSource1.Stop();
            audioSource2.volume = 0;
            audioSource2.clip = nextClip;
            audioSource2.loop = clipLoop;
            audioSource2.Play();
            audioFade2.FadeToVolume(clipVolume, clipFadeTime);
        } else
        {
            audioSource2.Stop();
            audioSource1.volume = 0;
            audioSource1.clip = nextClip;
            audioSource1.loop = clipLoop;
            audioSource1.Play();
            audioFade1.FadeToVolume(clipVolume, clipFadeTime);
        }
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
}
