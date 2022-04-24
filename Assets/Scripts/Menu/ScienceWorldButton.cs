using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScienceWorldButton : MonoBehaviour
{
    Text ButtonText;
    bool active;

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
        ButtonText = transform.GetChild(0).gameObject.GetComponent<Text>();
        active = true;
    }

    public void OnClicked()
    {
        Debug.Log($"Clicked world button: Science");
        Debug.Log(ButtonText.text);
        GameState.OnlineLevel = true;
        GameState.SetCurrentWorld(-1);
        MenuManager.GoToMenu(MenuName.LevelSelectScience);
    }


    public void ButtonClicked()
    {
        AudioManager.Play(AudioClipName.MenuClick);
    }

}
