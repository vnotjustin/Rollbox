using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentLevelManager : MonoBehaviour
{

    public static CurrentLevelManager main;

    public bool mustReset;
    public SpriteRenderer resetDisplay;

    public bool levelComplete;
    public SpriteRenderer levelCompleteDisplay;

    private void Awake()
    {
        main = this;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }

        resetDisplay.enabled = mustReset;
        levelCompleteDisplay.enabled = levelComplete;
    }

    public void ResetGame()
    {
        if (CraneManagement.main != null)
        {
            CraneManagement.main.ClearAllCrates();
        }

        if (PlayerManager.main != null)
        {
            PlayerManager.main.ResetPlayer();
        }

        mustReset = false;
        levelComplete = false;
    }
}
