using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MenuManager
{
    static bool popupMenuActive = false;

    public static void GoToMenu(MenuName menuName)
    {
        switch (menuName)
        {
            case MenuName.Main:
                popupMenuActive = false;
                Time.timeScale = 1;
                SceneManager.LoadScene("MainMenu");
                break;
            case MenuName.Help:
                popupMenuActive = false;
                Time.timeScale = 1;
                SceneManager.LoadScene("HelpScreen");
                break;
            case MenuName.Gameplay:
                popupMenuActive = false;
                Time.timeScale = 1;
                SceneManager.LoadScene("GameScreen");
                break;
            case MenuName.LevelSelect:
                popupMenuActive = false;
                Time.timeScale = 1;
                SceneManager.LoadScene("LevelSelect");
                break;
            case MenuName.LevelSelectScience:
                popupMenuActive = false;
                Time.timeScale = 1;
                SceneManager.LoadScene("LevelSelectScience");
                break;
            case MenuName.WorldSelect:
                popupMenuActive = false;
                Time.timeScale = 1;
                SceneManager.LoadScene("WorldSelect");
                break;
            case MenuName.Pause:
                if (!popupMenuActive)
                {
                    popupMenuActive = true;
                    Time.timeScale = 0;
                    Object.Instantiate(Resources.Load("PauseMenu"));
                    GameObject sideBar = GameObject.FindWithTag("SideBar");
                    GameObject.Destroy(sideBar);
                }
                break;
            case MenuName.Message:
                Debug.Log("trying to make message");
                if (!popupMenuActive)
                {
                    popupMenuActive = true;
                    Time.timeScale = 0;
                    Object.Instantiate(Resources.Load("MessageBox"));
                    GameObject sideBar = GameObject.FindWithTag("SideBar");
                    GameObject.Destroy(sideBar);
                }
                break;
            case MenuName.Won:
                if (!popupMenuActive)
                {
                    popupMenuActive = true;
                    Time.timeScale = 0;
                    Object.Instantiate(Resources.Load("WonMenu"));
                    GameObject sideBar = GameObject.FindWithTag("SideBar");
                    GameObject.Destroy(sideBar);
                }
                break;
        }
    }

    public static void ResumeGame()
    {
        popupMenuActive = false;
        Time.timeScale = 1;
        GameObject popupMenu = GameObject.FindWithTag("PopUpMenu");
        GameObject.Destroy(popupMenu);
        GameObject messageBox = GameObject.FindWithTag("MessageBox");
        GameObject.Destroy(messageBox);
        Object.Instantiate(Resources.Load("SideBar"));
    }

}
