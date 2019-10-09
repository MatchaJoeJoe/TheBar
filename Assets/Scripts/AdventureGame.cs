using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;

public class AdventureGame : MonoBehaviour
{
    [SerializeField] private State startingState;
    [SerializeField] private ImageFade textBGFade;
    [SerializeField] private TextFade textFade;
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private GameObject buttonHolder;
    [SerializeField] private GameObject theButtonPrefab;
    [SerializeField] private GameObject theBGImage1;
    [SerializeField] private GameObject theBGImage2;
    [SerializeField] private GameObject rainImage;
    [SerializeField] private float startDelay;
    private Image theBGImage1image;
    private ImageFade theBGImage1imageFade;
    private Image theBGImage2image;
    private ImageFade theBGImage2imageFade;
    private AdventureGame theGame;
    private ImageFade buttonHolderFade;
    [SerializeField] private State currentState; //serialized for debugging

    // Start is called before the first frame update
    void Start()
    {
        theGame = FindObjectOfType<AdventureGame>();
        theBGImage1image = theBGImage1.GetComponent<Image>();
        theBGImage1imageFade = theBGImage1.GetComponent<ImageFade>();
        theBGImage2image = theBGImage2.GetComponent<Image>();
        theBGImage2imageFade = theBGImage2.GetComponent<ImageFade>();
        buttonHolderFade = buttonHolder.GetComponent<ImageFade>();
        NextState(startingState);
    }

    IEnumerator ButtonsFadeIn(float waitTime)
    {
        if (GetCurrentStateName() == startingState.name)
        {
            textBGFade.FadeIn();
            textFade.FadeIn();
            buttonHolderFade.FadeIn();
        }
        yield return new WaitForSeconds(waitTime);
        ButtonFade[] theButtons = FindObjectsOfType<ButtonFade>();
        int buttonWaitTime = 0;
        for (int i = theButtons.Length - 1; i >= 0; i--)
        {
            yield return new WaitForSeconds(buttonWaitTime * waitTime);
            theButtons[i].FadeIn();
            if (buttonWaitTime == 0)
            {
                buttonWaitTime = 1;
            }

        }
    }
    private void BackgroundFade(Sprite newSprite)
    {
        // moving rain to background if not the starting state
        if (GetCurrentStateName() == startingState.name)
        {
            rainImage.transform.SetAsLastSibling();
        }
        else
        {
            rainImage.transform.SetAsFirstSibling();
        }
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

    public void NextState(State nextState)
    {
        // Removing old buttons
        foreach (Transform child in buttonHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // Updating text
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

        // Updating state
        currentState = nextState;

        // Fading in buttons
        float delayTime = 1f;
        if (GetCurrentStateName() == startingState.name)
        {
            delayTime = startDelay;
        }
        StartCoroutine(ButtonsFadeIn(delayTime));

        // Updating background
        Sprite nextSprite = nextState.GetBackgroundImage();
        if (nextSprite != null)
        {
            BackgroundFade(nextSprite);
        }
    }

    public string GetCurrentStateName()
    {
        return currentState.name;
    }
}
