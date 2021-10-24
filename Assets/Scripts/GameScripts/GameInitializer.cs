using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Initializes the game
/// </summary>
public class GameInitializer : MonoBehaviour 
{
    /// <summary>
    /// Awake is called before Start
    /// </summary>
	void Awake()
    {
        // initialize utilities
        ScreenUtils.Initialize();
        ObjectPool.Initialize();
        LevelUtils.Initialize();
        GameState.Initialize();
        GameState.LoadGame();
    }
}
