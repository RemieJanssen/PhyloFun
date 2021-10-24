using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplaySideBar : MonoBehaviour
{
    #region Fields
    [SerializeField]
    GameObject sideBarBackground;
    [SerializeField]
    GameObject reticulation;
    [SerializeField]
    GameObject split;
    [SerializeField]
    Text levelName;
    [SerializeField]
    Text movesUsed;
    [SerializeField]
    Text movesUsedTail;
    [SerializeField]
    Text movesUsedHead;


    #endregion

    void Start()
    {
        levelName.text = GameState.LevelName;    
        SetMovesText();
        SetRearrFigures();
    }


    public void SetRearrFigures()
    {
        split.SetActive(GameState.TailMovesAllowed);
        reticulation.SetActive(GameState.HeadMovesAllowed);
    
    }    
    
//    public void SetSplitFigure()
//    {
//        split.SetActive(GameState.TailMovesAllowed);
//    }


//    public void SetReticFigure()
//    {
 //       split.SetActive(GameState.HeadMovesAllowed);
  //  }

    public void SetLevelName(string name)
    {
        levelName.text = name;
    }


    public void SetMovesText()
    {
        
        if((!GameState.HeadMovesAllowed && !GameState.TailMovesAllowed) || (GameState.MovesGoal==-1 && GameState.MovesGoalTail==-1 && GameState.MovesGoalHead==-1))
        {
            movesUsed.text      = "";
            movesUsedTail.text  = "";
            movesUsedHead.text  = "";
        }
        else if(GameState.MovesGoal!=-1)
        {
            movesUsedTail.text  = "";
            movesUsedHead.text  = "";
            movesUsed.text      = GameState.MovesUsed.ToString()+"/"+GameState.MovesGoal.ToString();
        }
        else 
        {
            movesUsed.text = "";
            movesUsedTail.text = GameState.MovesUsedTail.ToString()+"/"+GameState.MovesGoalTail.ToString();
            movesUsedHead.text = GameState.MovesUsedHead.ToString()+"/"+GameState.MovesGoalHead.ToString();
        }
        
    }


    void Awake()
    {
        Image sideBarBackgroundImage = sideBarBackground.GetComponent<Image>();
    }



}
