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
    [SerializeField] private float startDelay;
    private Image theBGImage1image;
    private Image theBGImage2image;
    private ImageFade theBGImage2imageFade;
    private AdventureGame theGame;
    private ImageFade buttonHolderFade;
    [Header("Debug")]
    [SerializeField] private GameObject[] activatableObjs;
    [SerializeField] private State currentState; //serialized for debugging

    // Start is called before the first frame update
    void Start()
    {
        GetActivatableObjs();
        theGame = FindObjectOfType<AdventureGame>();
        theBGImage1image = theBGImage1.GetComponent<Image>();
        theBGImage2image = theBGImage2.GetComponent<Image>();
        theBGImage2imageFade = theBGImage2.GetComponent<ImageFade>();
        buttonHolderFade = buttonHolder.GetComponent<ImageFade>();
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
        StartCoroutine(TextFade(nextState));
    }
    IEnumerator TextFade(State nextState)
    {
        // Toggling objects from state
        if (currentState != null)
        {
            string thisTag = currentState.GetActivatedTag();
            ToggleObjByName(thisTag);
        }
        string nextTag = nextState.GetActivatedTag();
        ToggleObjByName(nextTag);
        // Updating state
        currentState = nextState;
        textBGFade.FadeOut();
        textFade.FadeOut();
        buttonHolderFade.FadeOut();

        // Updating background
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
}
