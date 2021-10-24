using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSelectScreen : MonoBehaviour
{
    #region fields
    [SerializeField]
    GameObject[] worldButtons;
    int numberOfButtons;
    #endregion


    void Start()
    {
        numberOfButtons = worldButtons.Length;
        LoadButtons();
    }

    void LoadButtons()
    {
        // Set all the world buttons.
        for (int j =0; j<GameState.CurrentMedals.Count; j++)
        {
            Debug.Log("world "+(j+1).ToString()+" medals"+GameState.CurrentMedals[j].Count.ToString());
        }
        
        
        int i = 1;
        foreach (GameObject button in worldButtons)
        {
            WorldButton buttonScript = button.GetComponent<WorldButton>();
            buttonScript.WorldNumber = i;
            button.SetActive(true);
            buttonScript.Active = true;
            if (GameState.CurrentMedals[i-1].Count<1)
            {
                button.SetActive(true);
                buttonScript.Active = false;
            }
            else
            {
                button.SetActive(true);
            }
            i = i + 1;
        }
    }
}
