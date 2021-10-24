using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldButton : MonoBehaviour
{
    Text ButtonText;
    int worldNumber;
    bool active;

    public int WorldNumber
    {
        get { return worldNumber; }
        set 
        { 
            worldNumber = value;
            ButtonText.text = worldNumber.ToString();
        }
    }

    public bool Active
    {
        get { return active; }
        set
        {
            active = value;
            if(active)
            {
                GetComponent<Button>().interactable = true;
            }
            else
            {
                GetComponent<Button>().interactable = false;
            }
        }
    }

    void Awake()
    {
        worldNumber = 0;
        ButtonText = transform.GetChild(0).gameObject.GetComponent<Text>();
        active = true;
    }


    public void OnClicked()
    {
        Debug.Log("Clicked world button:");
        Debug.Log(worldNumber);
        GameState.SetCurrentWorld(worldNumber);
        MenuManager.GoToMenu(MenuName.LevelSelect);
    }

    public void ButtonClicked()
    {
        AudioManager.Play(AudioClipName.MenuClick);
    }

}
