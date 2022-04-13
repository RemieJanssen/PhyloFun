using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelUtils
{
    #region Fields
    //Contains the filename for each level.
    static List<List<string>> levelFileNames;
    static LevelLoader loader;
    static List<int> noOfLevels;
    static int noOfWorlds;
    static List<int> unlockNewWorldAt;
    #endregion




    #region Properties
    public static int NoOfWorlds
    {
        get { return noOfWorlds; }
    }


    public static string LevelText
    {
        get { return loader.LevelText; }
    }

    public static bool Loading
    {
        get { return loader.Loading; }
    }


    #endregion




    #region Methods

    public static int UnlockNewWorldAt(int world)
    {
        return unlockNewWorldAt[world-1];
    }

    // get the level name for a given level id
    public static string LevelName(int level_id, int world_id)
    {
        return levelFileNames[world_id-1][level_id-1];
    }

    // get the number of levels in a given world
    public static int NoOfLevels(int world_id)
    {
        return noOfLevels[world_id-1];
    }



    /// <summary>
    /// Initializes the level utilities
    /// </summary>
    public static void Initialize()
    {
        loader = GameObject.FindWithTag("LevelLoader").GetComponent<LevelLoader>();
        // setting levels to the right files
        levelFileNames = new List<List<string>>();
        noOfWorlds = 4;
        noOfLevels = new List<int>();
        unlockNewWorldAt = new List<int> {7,4,4,999};
        
        for(int i=0;i<noOfWorlds;i++){
            levelFileNames.Add(new List<string>());
            noOfLevels.Add(0);
        }

// testing etc.
//        levelFileNames.Add("TailSwapReticulationsLadder.lvl");// swap a reticulation from/to the ladder

// move only
        levelFileNames[0].Add("Tutorial1.lvl");// one node
        levelFileNames[0].Add("Tutorial2.lvl");// three nodes
//        levelFileNames[0].Add("Tutorial3.lvl");// two labeled nodes
        levelFileNames[0].Add("Tutorial4.lvl");// three labeled, two non-labeled nodes
//        levelFileNames[0].Add("Tutorial5.lvl");// one edge
//        levelFileNames[0].Add("Tutorial6.lvl");// cherry
        levelFileNames[0].Add("Tutorial7.lvl");// cherry, one label
        levelFileNames[0].Add("Tutorial8.lvl");// Rotated square box
        //levelFileNames[0].Add("Level1.lvl");
        levelFileNames[0].Add("Level2.lvl");// triplet, labeled leaves
        levelFileNames[0].Add("Level3.lvl");// balanced one label
        levelFileNames[0].Add("Level5.lvl");// sideways tree
        levelFileNames[0].Add("Level4.lvl");// weird network

//tail moves
        levelFileNames[1].Add("TailMoveBasic.lvl");// most basic scheme for a tail move
        levelFileNames[1].Add("TailTreeRearr1.lvl");// triplet rearrange
        levelFileNames[1].Add("TailSubtree.lvl");// subtree move
        levelFileNames[1].Add("TailLabelPermutation.lvl");// Permute leaves of a tree
        levelFileNames[1].Add("TailSimulateHeadMoveC.lvl");// Head move C from tail move paper
        levelFileNames[1].Add("TailTriangleDirection.lvl");// change triangle direction
        levelFileNames[1].Add("TailTriangleWithExtraCherry.lvl");// change triangle direction
        levelFileNames[1].Add("TailSimulateHeadMoveB.lvl");// Head move B from tail move paper
        levelFileNames[1].Add("TailAllLabelsSmall1.lvl");// triplet rearrange internal labels
        levelFileNames[1].Add("TailAllLabelsSmall2.lvl");// triplet rearrange internal labels
        levelFileNames[1].Add("TailSwapAdjacentReticLabels.lvl");// Swap the labels of two adjacent reticulation nodes
        levelFileNames[1].Add("TailSwapAdjacentReticLabels2.lvl");// Swap the labels of two adjacent reticulation nodes
        //        levelFileNames.Add("TailOneLeafImpossibleInternal.lvl");// Try to swap adjacent retic labels, impossible

//head moves
        levelFileNames[2].Add("HeadMoveBasic.lvl");// most basic scheme for a head move
        levelFileNames[2].Add("HeadUDTreeRearr1.lvl");// triplet rearrange upside down
        levelFileNames[2].Add("HeadTriangleDirection.lvl");// change triangle direction
        levelFileNames[2].Add("HeadTriangleMove.lvl");// move triangle
        levelFileNames[2].Add("HeadTriangleMoveHard.lvl");// move triangle and change direction
        levelFileNames[2].Add("HeadNeatlyOnTop.lvl");// change retics at the top
        levelFileNames[2].Add("HeadTrianglesToTheTop.lvl");// move triangles to the top
        levelFileNames[2].Add("HeadSimulateBasicTailMove.lvl");// Tail move using extra head

        levelFileNames[3].Add("1");// A-networks tail and head moves
        levelFileNames[3].Add("2");
        levelFileNames[3].Add("3");


        for(int i=0; i<noOfWorlds; i++){
            noOfLevels[i] = levelFileNames[i].Count;
            Debug.Log($"{noOfLevels[i]} levels in world {i+1}");
        }        
        
    }
    #endregion
}
