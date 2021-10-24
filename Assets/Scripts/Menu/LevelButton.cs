using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    Text ButtonText;
    int levelNumber;
    bool active;
    [SerializeField]
    Sprite normalSprite;
    [SerializeField]
    Sprite normalSpriteHighlighted;
    [SerializeField]
    Sprite medalSprite;
    [SerializeField]
    Sprite medalSpriteHighlighted;
    [SerializeField]
    Sprite deactivatedSprite;
    

    public int LevelNumber
    {
        get { return levelNumber; }
        set 
        { 
            levelNumber = value;
            ButtonText.text = levelNumber.ToString();
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
        levelNumber = 0;
        ButtonText = transform.GetChild(0).gameObject.GetComponent<Text>();
        SetMedal(false);
        active = true;
    }


    public void SetMedal(bool hasMedal)
    {
        SpriteState ss = new SpriteState();
        ss.disabledSprite = deactivatedSprite;
        if (hasMedal)
        {
            gameObject.GetComponent<Image>().sprite=medalSprite;
            ss.highlightedSprite = medalSpriteHighlighted;
            GetComponent<Button>().spriteState = ss;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite=normalSprite;
            ss.highlightedSprite = normalSpriteHighlighted;
            GetComponent<Button>().spriteState = ss;
        }
    }


    public void OnClicked()
    {
        Debug.Log("Clicked level button with level:");
        Debug.Log(levelNumber);
        GameState.SetCurrentLevel(levelNumber);
        GoToLevel();
    }

    void GoToLevel()
    {
        MenuManager.GoToMenu(MenuName.Gameplay);
    }

    public void ButtonClicked()
    {
        AudioManager.Play(AudioClipName.MenuClick);
    }

}
