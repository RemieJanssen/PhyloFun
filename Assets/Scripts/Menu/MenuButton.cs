using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToMain()
    {
        MenuManager.GoToMenu(MenuName.Main);
    }

    public void QuitLevel()
    {
        // First remove all nodes and edges from the screen
        ObjectPool.ReturnAllNodesAndEdges();
        // Then go to the level select screen
        GoToLevelSelect();
    }

    public void GoToHelpPage()
    {
        MenuManager.GoToMenu(MenuName.Help);
    }

    public void GoToLevelSelect()
    {
        MenuManager.GoToMenu(MenuName.LevelSelect);
    }

    public void GoToWorldSelect()
    {
        MenuManager.GoToMenu(MenuName.WorldSelect);
    }

    public void GoToPauseMenu()
    {
        MenuManager.GoToMenu(MenuName.Pause);
    }

    public void ReturnToGame()
    {
        MenuManager.ResumeGame();
    }

    public void RestartLevel()
    {
        // First remove all nodes and edges from the screen
        ObjectPool.ReturnAllNodesAndEdges();
        // Then restart the level
        MenuManager.GoToMenu(MenuName.Gameplay);
    }

    public void NextLevel()
    {
        // First remove all nodes and edges from the screen
        ObjectPool.ReturnAllNodesAndEdges();
        // Then go to the next level
        GameState.NextLevel();
        MenuManager.GoToMenu(MenuName.Gameplay);
    }

    public void ButtonClicked()
    {
        AudioManager.Play(AudioClipName.MenuClick);
    }

}
