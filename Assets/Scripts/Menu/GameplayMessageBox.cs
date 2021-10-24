using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayMessageBox : MonoBehaviour
{
    #region Fields
    [SerializeField]
    GameObject messageBoxBackground;
    [SerializeField]
    Text levelMessage;
    #endregion


    void Start()
    {
        levelMessage.text = GameState.LevelMessage;
    }


    void Awake()
    {
        Image sideBarBackgroundImage = messageBoxBackground.GetComponent<Image>();
    }



}
