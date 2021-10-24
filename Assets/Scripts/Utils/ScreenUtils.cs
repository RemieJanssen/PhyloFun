using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides screen utilities
/// </summary>
public static class ScreenUtils
{
    #region Fields

    //Margin in screenSize system
    static float hudMarginLeft = 0f;
    static float hudMarginRight = 300f;
    static float hudMarginTop = 0f;
    static float hudMarginBottom = 0f;


    // cached for efficient boundary checking
    static float screenLeft;
    static float screenRight;
    static float screenTop;
    static float screenBottom;
    static float gameplayLeft;
    static float gameplayRight;
    static float gameplayTop;
    static float gameplayBottom;
    static float gameplayWidth;
    static float gameplayHeight;
    static Vector3 gameplayTopLeft;

    #endregion

    #region Properties

    public static float HudMarginLeft
    {
        get { return hudMarginLeft; }
    }

    public static float HudMarginRight
    {
        get { return hudMarginRight; }
    }

    public static float HudMarginTop
    {
        get { return hudMarginTop; }
    }

    public static float HudMarginBottom
    {
        get { return hudMarginBottom; }
    }



    /// <summary>
    /// Gets the left edge of the screen in world coordinates
    /// </summary>
    /// <value>left edge of the screen</value>
    public static float ScreenLeft
    {
        get { return screenLeft; }
    }

    /// <summary>
    /// Gets the right edge of the screen in world coordinates
    /// </summary>
    /// <value>right edge of the screen</value>
    public static float ScreenRight
    {
        get { return screenRight; }
    }

    /// <summary>
    /// Gets the top edge of the screen in world coordinates
    /// </summary>
    /// <value>top edge of the screen</value>
    public static float ScreenTop
    {
        get { return screenTop; }
    }

    /// <summary>
    /// Gets the bottom edge of the screen in world coordinates
    /// </summary>
    /// <value>bottom edge of the screen</value>
    public static float ScreenBottom
    {
        get { return screenBottom; }
    }

    /// <summary>
    /// Gets the left edge of the gameplay part in world coordinates
    /// </summary>
    /// <value>left edge of the gameplay part</value>
    public static float GameplayLeft
    {
        get { return gameplayLeft; }
    }

    /// <summary>
    /// Gets the right edge of the gameplay part in world coordinates
    /// </summary>
    /// <value>right edge of the gameplay part</value>
    public static float GameplayRight
    {
        get { return gameplayRight; }
    }

    /// <summary>
    /// Gets the top edge of the gameplay part in world coordinates
    /// </summary>
    /// <value>top edge of the gameplay part</value>
    public static float GameplayTop
    {
        get { return gameplayTop; }
    }

    /// <summary>
    /// Gets the bottom edge of the gameplay part in world coordinates
    /// </summary>
    /// <value>bottom edge of the gameplay part</value>
    public static float GameplayBottom
    {
        get { return gameplayBottom; }
    }

    /// <summary>
    /// Gets the width of the gameplay part of the screen
    /// </summary>
    /// <value>The width of the gameplay part of the screen.</value>
    public static float GameplayWidth
    {
        get { return gameplayWidth; }
    }

    /// <summary>
    /// Gets the height of the gameplay part of the screen
    /// </summary>
    /// <value>The height of the gameplay part of the screen.</value>
    public static float GameplayHeight
    {
        get { return gameplayHeight; }
    }

    public static Vector3 GameplayTopLeft
    {
        get { return gameplayTopLeft; }
    }




    #endregion

    #region Methods

    public static Vector2 WorldToGameplayPosition(Vector2 worldPos)
    {
        Vector2 gpPos = worldPos;
        gpPos.y = -(gpPos.y - gameplayTop) / GameplayHeight;
        gpPos.x = (gpPos.x - gameplayLeft) / GameplayWidth;
        return gpPos;
    }


    /// <summary>
    /// Initializes the screen utilities
    /// </summary>
    public static void Initialize()
    {
        // save screen edges in world coordinates
        float screenZ = -Camera.main.transform.position.z;
        Vector3 lowerLeftCornerScreen = new Vector3(0, 0, screenZ);
        Vector3 upperRightCornerScreen = new Vector3(
            Screen.width, Screen.height, screenZ);
        Vector3 lowerLeftCornerWorld =
            Camera.main.ScreenToWorldPoint(lowerLeftCornerScreen);
        Vector3 upperRightCornerWorld =
            Camera.main.ScreenToWorldPoint(upperRightCornerScreen);
        screenLeft = lowerLeftCornerWorld.x;
        screenRight = upperRightCornerWorld.x;
        screenTop = upperRightCornerWorld.y;
        screenBottom = lowerLeftCornerWorld.y;
        Debug.Log("Screen bounds: left right top down");
        Debug.Log(screenLeft);
        Debug.Log(screenRight);
        Debug.Log(screenTop);
        Debug.Log(screenBottom);


        // Set the part of the screen used for the actual gameplay.
        Debug.Log(Screen.width);
        Debug.Log(Screen.width - hudMarginRight);
        Vector3 lowerLeftCornerGame = new Vector3(hudMarginLeft, hudMarginBottom, screenZ);
        Vector3 upperRightCornerGame = new Vector3(
            Screen.width- hudMarginRight, Screen.height- hudMarginTop, screenZ);
        Vector3 lowerLeftCornerGameWorld =
            Camera.main.ScreenToWorldPoint(lowerLeftCornerGame);
        Vector3 upperRightCornerGameWorld =
            Camera.main.ScreenToWorldPoint(upperRightCornerGame);
        gameplayLeft = lowerLeftCornerGameWorld.x;
        gameplayRight = upperRightCornerGameWorld.x;
        gameplayTop = upperRightCornerGameWorld.y;
        gameplayBottom = lowerLeftCornerGameWorld.y;
        gameplayWidth = gameplayRight - gameplayLeft;
        gameplayHeight = gameplayTop - gameplayBottom;
        gameplayTopLeft = new Vector3(gameplayLeft, gameplayTop, 0);
        Debug.Log("Gameplay bounds: left right top down");
        Debug.Log(gameplayLeft);
        Debug.Log(gameplayRight);
        Debug.Log(gameplayTop);
        Debug.Log(gameplayBottom);

    }

    #endregion
}
