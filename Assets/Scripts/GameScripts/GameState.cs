using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public static class GameState
{

    #region Fields
    static int currentLevel;
    static int currentWorld;
    static List<List<int>> currentMedals;
    static LevelLoader loader;
    static bool inRearrangementMode;
    static bool checkedButtonOnce;
    static GraphNode rearrangementNode;
    static List<GraphNode> endpoints;
    static MouseHandling mouseHandler;
    static bool headMovesAllowed;
    static bool tailMovesAllowed;
    static string levelName;
    static string levelMessage;
    static List<RearrangementMove> movesUsedSequence;
    static int movesUsed;
    static int movesUsedTail;
    static int movesUsedHead;
    static int movesGoal;
    static int movesGoalTail;
    static int movesGoalHead;
    static int numberOfLabels;
    #endregion

    #region Initialization
    public static void Initialize()
    {
        loader = GameObject.FindWithTag("LevelLoader").GetComponent<LevelLoader>();
        SetCurrentWorld(1);
        SetCurrentLevel(1);
        EventManager.AddRearrangementListener(SetRearrangementNode);
        inRearrangementMode = false;
        endpoints = new List<GraphNode>();
        movesUsedSequence = new List<RearrangementMove>();
    }
    #endregion


    #region Properties
    
    
    public static GraphNode RearrangementNode
    {
        get { return rearrangementNode; }
    }

    
    public static int MovesUsed
    {
        get { return movesUsed; }
    }

    public static int MovesUsedHead
    {
        get { return movesUsedHead; }
    }
    
    public static int MovesUsedTail
    {
        get { return movesUsedTail; }
    }    

    public static int MovesGoal
    {
        get { return movesGoal; }
        set { movesGoal = value;}
    }    

    public static int MovesGoalTail
    {
        get { return movesGoalTail; }
        set { movesGoalTail = value;}
    }    

    public static int MovesGoalHead
    {
        get { return movesGoalHead; }
        set { movesGoalHead = value;}
    }    


    public static int NumberOfLabels
    {
        get { return numberOfLabels; }
        set { numberOfLabels = value;}
    }    

    
    public static string LevelName
    {
        get { return levelName; }
        set
        {
            levelName = value;
            if(levelName=="")
            {
                levelName = "Level: " + currentWorld.ToString()+"."+currentLevel.ToString();
            }
            GameObject.FindWithTag("SideBar").GetComponent<GameplaySideBar>().SetLevelName(levelName);
        }
    }


    public static string LevelMessage
    {
        get { return levelMessage; }
        set 
        { 
            levelMessage = value;
            if(levelMessage!="")
            {
                MenuManager.GoToMenu(MenuName.Message);
            }
        }
    }



    public static bool HeadMovesAllowed
    {
        get { return headMovesAllowed; }
        set 
        { 
            headMovesAllowed = value;
        }
    }

    public static bool TailMovesAllowed
    {
        get { return tailMovesAllowed; }
        set 
        { 
            tailMovesAllowed = value;
        }
    }

    public static int CurrentLevel
    {
        get { return currentLevel; }
    }

    public static int CurrentWorld
    {
        get { return currentWorld; }
    }


    public static bool InRearrangementMode
    {
        get { return inRearrangementMode; }
    }


    public static List<List<int>> CurrentMedals
    {
        get { return currentMedals;}
    }


    #endregion


    #region Methods
    private static Save CreateSaveGameObject()
    {
      Save save = new Save();
      save.medals = currentMedals;
      return save;
    }
    

    public static void SetMovesInSideBar()
    {
        GameObject.FindWithTag("SideBar").GetComponent<GameplaySideBar>().SetMovesText();
    }

    public static void ResetLevel()
    {
        movesUsedSequence.Clear();
        movesUsed     = 0;
        movesUsedHead = 0;
        movesUsedTail = 0;
        SetMovesInSideBar();
        GameObject.FindWithTag("SideBar").GetComponent<GameplaySideBar>().SetRearrFigures();
    }

    
    
    public static void SaveGame()
    {
      Save save = CreateSaveGameObject();

      BinaryFormatter bf = new BinaryFormatter();
      FileStream file = File.Create(Application.persistentDataPath + "/PhyloFun.save");
      bf.Serialize(file, save);
      file.Close();
      Debug.Log("Game Saved");
    }
    
    public static void LoadGame()
    { 
      if (File.Exists(Application.persistentDataPath + "/PhyloFun.save"))
      {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/PhyloFun.save", FileMode.Open);
        Save save = (Save)bf.Deserialize(file);
        file.Close();
        currentMedals = save.medals;
        Debug.Log("Game Loaded");
      }
      else
      {
        currentMedals = new List<List<int>>();
        for(int i=1; i<=LevelUtils.NoOfWorlds; i++)
        {
            currentMedals.Add(new List<int>());
        }
        currentMedals[0].Add(0);
        Debug.Log("No game saved!");
      }
    }
    
    
    
    
    
    public static void SetMedals(int worldNumber, int levelNumber, int medalNumber)
    {
        //Make sure the list is long enough to modify the medal count of the level
        for (int i = currentMedals[worldNumber-1].Count;  i<=levelNumber+1; i++)
        {
            currentMedals[worldNumber-1].Add(0);
        }
        //actually set the number of medals
        currentMedals[worldNumber-1][levelNumber-1] = medalNumber;
        //also unlock the next one if enough levels in this world are solved
        if (levelNumber>=LevelUtils.UnlockNewWorldAt(worldNumber) && medalNumber>0 && currentMedals[worldNumber].Count==0)
        {
            currentMedals[worldNumber].Add(0);
        }     
        SaveGame();
    }
    
    

    public static bool SetCurrentLevel(int levelNumber)
    {
        if (levelNumber <= LevelUtils.NoOfLevels(currentWorld))
        {
            currentLevel = levelNumber;
            loader.LoadLevel(currentLevel,currentWorld);
            return true;
        }
        return false;
    }


    public static bool SetCurrentWorld(int worldNumber)
    {
        if (worldNumber <= LevelUtils.NoOfWorlds)
        {
            currentWorld = worldNumber;
            currentLevel = 1;
            loader.LoadLevel(currentLevel,currentWorld);
            return true;
        }
        return false;
    }

    public static bool NextLevel()
    {
        bool existsInWorld = SetCurrentLevel(currentLevel + 1);
        if(!existsInWorld)
        {
            return SetCurrentWorld(currentWorld + 1);
        }
        return false;
    }

    public static void SetRearrangementNode(GraphNode node)
    {
        if(node!=null)
        {
            inRearrangementMode = true;
            rearrangementNode = node;
            Debug.Log("Starting rearrangement");
        }
        else
        {
            inRearrangementMode = false;
            rearrangementNode = null;
        }
    }


    public static void AddEndpoint(GraphNode endpoint)
    {
        endpoints.Add(endpoint);
    }

    public static void RemoveAllEndpoints()
    {
        GraphNode node = null;
        DiGraph graph;
        for(int i = endpoints.Count-1; i>=0; i--)
        {
            node = endpoints[i];
            graph = node.Graph;
            graph.RemoveNode(node);
            endpoints.RemoveAt(i);
        }
        if(node!=null)
        {
            foreach (GraphEdge edge in node.Graph.Edges)
            {
                edge.Hidden = false;
            }
        }
    }

    /// <summary>
    /// Quits the rearrangement mode.
    /// </summary>
    public static void EndRearrangement()
    {
        RemoveAllEndpoints();
        GraphNode rearrNodeCopy = rearrangementNode;
        SetRearrangementNode(null);
        mouseHandler = GameObject.FindWithTag("MouseHandler").GetComponent<MouseHandling>();
        mouseHandler.TapListen = false;
        rearrNodeCopy.ConnectToSocket();
    }


    // A function that rearranges the graph that contains the edges movingEdge goalEdge and the node endpoint.
    // The node endpoint was attached to is moved from movingEdge to goalEdge.
    // Returns true if there is a proper rearrangement, false if not (i.e., if we could not rearrange, or if the move was to an adjacent edge.
    public static bool Rearrange(RearrangementMove move)
    {
        
        DiGraph graph = move.MovingEdge.Tail.Graph;
        GraphNode actualEndpoint = rearrangementNode;
        GraphNode newParent = move.GoalEdge.Tail;
        GraphNode newChild = move.GoalEdge.Head;
        //Find the other endpoints of the `edge' we are removing an endpoint from
        //And find the other endpoint of the moving edge
        GraphNode currentParent = null;
        GraphNode currentChild = null;
        foreach (GraphNode parent in actualEndpoint.Parents)
        {
            if (parent.Id != move.MovingEdge.Tail.Id)
            {
                currentParent = parent;
            }
        }
        foreach (GraphNode child in actualEndpoint.Children)
        {
            if (child.Id != move.MovingEdge.Head.Id)
            {
                currentChild = child;
            }
        }
        //If somehow, we do not find one of them, return false
        if (currentParent == null || currentChild == null)
        {
            return false;
        }

        // If the move is to an adjacent edge, the graph does not change, we just move the endpoint.
        if (move.GoalEdge.Tail == actualEndpoint || move.GoalEdge.Head == actualEndpoint)
        {
            actualEndpoint.Position = move.MovingEndpoint.Position;//(newParent.Position + newChild.Position) / 2;
            actualEndpoint.UpdateAdjacentEdgePositions();
            return false;
        }



        //check whether we create parallel edges
        if (graph.Find(currentParent,currentChild)!=null)
        {
            Debug.Log("Creates parallel");
            return false;
        }

        // otherwise, we are set to do the rearrangement.
        // change the part on the current side
        Debug.Log("Changing original side");
        graph.AddEdge(currentParent, currentChild);
        graph.RemoveEdge(actualEndpoint,currentChild);
        graph.RemoveEdge(currentParent, actualEndpoint);
        Debug.Log("Canging receiving side");
        // change the part on the receiving side
        graph.RemoveEdge(move.GoalEdge.Tail,move.GoalEdge.Head);
        graph.AddEdge(newParent, actualEndpoint);
        graph.AddEdge(actualEndpoint, newChild);

        // check and set mappings
        actualEndpoint.SetMapping(null);
        newParent.CheckMapping();
        newChild.CheckMapping();
        currentParent.CheckMapping();
        currentChild.SetMapping(currentChild.MappedNode);

        //update positions
        actualEndpoint.Position = move.MovingEndpoint.Position;//(newParent.Position + newChild.Position) / 2;
        currentChild.UpdateAdjacentEdgePositions();
        actualEndpoint.UpdateAdjacentEdgePositions();

        movesUsed++;
        movesUsedSequence.Add(move);
        Debug.Log("Current sequence:");
        foreach (RearrangementMove m in movesUsedSequence){
            Debug.Log(m.ToString());        
        }
        if (rearrangementNode==move.MovingEdge.Tail)
        {
            movesUsedTail++;
        }
        else
        {
            movesUsedHead++;
        }
        SetMovesInSideBar();
//        actualEndpoint.ConnectToSocket();
        return true;
    }


    #endregion
}
