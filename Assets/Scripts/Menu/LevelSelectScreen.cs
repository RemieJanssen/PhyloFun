using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectScreen : MonoBehaviour
{
    #region fields
    [SerializeField]
    GameObject[] levelButtons;
    [SerializeField]
    GameObject NextButton;
    [SerializeField]
    GameObject PreviousButton;
    int firstLevelOfScreen;
    int numberOfButtons;
    #endregion


    void Start()
    {
        firstLevelOfScreen = 1;
        numberOfButtons = levelButtons.Length;
        LoadButtons();
    }

    void LoadButtons()
    {
        // Set all the level buttons.
        int i = firstLevelOfScreen;
        int world_number = GameState.CurrentWorld;
        foreach (GameObject button in levelButtons)
        {
            LevelButton buttonScript = button.GetComponent<LevelButton>();
            buttonScript.LevelNumber = i;
            button.SetActive(true);
            buttonScript.Active = true;
            if (i > LevelUtils.NoOfLevels(world_number))
            {
                //button not present because the level does not exist
                button.SetActive(false);
            }
            else if (i >= GameState.CurrentMedals[world_number-1].Count && i!=1)
            {
                //button present but inactive: level not unlocked yet
                button.SetActive(true);
                buttonScript.Active = false;
            }
            else
            {
                //button present and active
                button.SetActive(true);
                buttonScript.SetMedal(GameState.CurrentMedals[world_number-1][i-1]>1);
            }
            i = i + 1;
        }
        // Now set the previous button
        if(firstLevelOfScreen==1)
        {
            PreviousButton.SetActive(false);
        }
        else
        {
            PreviousButton.SetActive(true);
        }
        // Now set the next button
        if(firstLevelOfScreen+numberOfButtons > LevelUtils.NoOfLevels(world_number))
        {
            NextButton.SetActive(false);
        }
        else
        {
            NextButton.SetActive(true);
        }
    }

    public void NextLevels()
    {
        firstLevelOfScreen = firstLevelOfScreen + numberOfButtons;
        LoadButtons();
    }

    public void PreviousLevels()
    {
        firstLevelOfScreen = firstLevelOfScreen - numberOfButtons;
        LoadButtons();
    }


}
