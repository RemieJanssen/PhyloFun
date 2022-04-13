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
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo( "en-US" );
        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo( "en-US" );
        // initialize utilities
        ScreenUtils.Initialize();
        ObjectPool.Initialize();
        LevelUtils.Initialize();
        GameState.Initialize();
        GameState.LoadGame();
    }
}
