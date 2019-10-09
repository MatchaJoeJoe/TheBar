using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChecker : MonoBehaviour
{
    public AdventureGame theGame;

    private void Start()
    {
        theGame = GameObject.FindObjectOfType<AdventureGame>();
    }
    // Update is called once per frame
    void Update()
    {
        if( theGame.GetCurrentStateName() != "A0 Starting instructions")
        {
            gameObject.SetActive(false);
        }
    }
}
