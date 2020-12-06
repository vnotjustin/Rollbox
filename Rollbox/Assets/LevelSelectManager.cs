using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    [Header("Level Select")]
    public LevelSelectButton[] allGrassyLevels;
    [Header("Level Select Palettes")]
    public Palette grassyPalette;

    // Start is called before the first frame update
    void Start()
    {
        UpdateLevelDisplays();
    }

    // Update is called once per frame
    void Update()
    {
        DetectClicks();
    }

    void UpdateLevelDisplays()
    {
        UpdateUnlocks();

        int levelNo = 1;
        for (int i = 0; i < allGrassyLevels.Length; i++)
        {
            allGrassyLevels[i].UpdateDisplay(grassyPalette,levelNo);
            levelNo++;
        }
    }


    /// <summary>
    /// Updates which levels are able to be unlocked and which are not. Currently, a level unlocks if the previous one is beaten.
    /// </summary>
    void UpdateUnlocks()
    {
        bool previousCompleted = true;
        for (int i = 0; i < allGrassyLevels.Length; i++)
        {
            allGrassyLevels[i].unlocked = previousCompleted;

            previousCompleted = allGrassyLevels[i].levelToTravelTo.levelCompletionData.completed;
        }
    }

    void DetectClicks()
    {
        bool pressed = false;
        Vector3 touchPos = Vector3.zero;
        ExtensionMethods.DetectTouches(out pressed, out touchPos);

        if (pressed)
        {
            for (int i = 0; i < allGrassyLevels.Length; i++)
            {
                if (ExtensionMethods.TouchedHitbox(allGrassyLevels[i].touchHitbox, touchPos))
                {
                    if (allGrassyLevels[i].unlocked)
                    {
                        SceneManager.LoadScene(allGrassyLevels[i].levelToTravelTo.buildSceneNumber);
                        break;
                    }
                }
            }
        }
    }
}
