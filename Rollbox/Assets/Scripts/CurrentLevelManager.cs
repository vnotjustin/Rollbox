using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentLevelManager : MonoBehaviour
{

    public static CurrentLevelManager main;
    public LevelSelectInfo thisLevel;
    public Palette generalPalette;
    public int totalCratesDropped;
    [Space]
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

        totalCratesDropped = 0;
        TopLevelUI.main.UpdateCrateCount();

        mustReset = false;
        levelComplete = false;
    }

    public void AddCrate()
    {
        if (!levelComplete)
        {
            totalCratesDropped++;
            TopLevelUI.main.UpdateCrateCount();
        }
    }

    public void LevelCompleted()
    {
        if (!levelComplete)
        {
            levelComplete = true;
            thisLevel.levelCompletionData.completed = true;
            if (totalCratesDropped < thisLevel.levelCompletionData.cratesUsed)
            {
                thisLevel.levelCompletionData.cratesUsed = totalCratesDropped;
            }
            else if (thisLevel.levelCompletionData.cratesUsed == 0)
            {
                thisLevel.levelCompletionData.cratesUsed = totalCratesDropped;
            }
        }
    }
}
