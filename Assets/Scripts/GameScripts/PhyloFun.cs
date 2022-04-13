using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Globalization;



public class PhyloFun : MonoBehaviour
{
    DiGraph changeGraph;
    DiGraph goalGraph;
    DiGraph[] graphs;

    //LevelFile support
    string levelFileText;

    //support for building levels
    //node dicts
    Dictionary<int, GraphNode> goalNodesByID;
    Dictionary<int, GraphNode> changeNodesByID;
    Dictionary<int, GraphNode>[] graphNodesById;
    //labelIds
    int currentLabelId;


    // Separation parameter for overlaying the networks.
    //Float from 0 to 1.
    // 0 meaning complete overlay
    // 1 complete separation. [DEFAULT]
    float separation;
    float separationDefault = 1;


    // Start is called before the first frame update
    void Start()
    {
        //make a new graph
        changeGraph = new DiGraph();
        changeGraph.Movable = true;
        goalGraph = new DiGraph();
        goalGraph.Movable = false;
        graphs = new DiGraph[2] { changeGraph, goalGraph };
        EventManager.AddMappingListener(CheckWon);
        StartCoroutine(LoadLevel(GameState.CurrentLevel)); 
    }
    
   

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            print(changeGraph.ToString());
        }
    }
    
    void ClampAllNodesToSocket()
    {
        foreach (GraphNode node in changeGraph.Nodes)
        {
            node.ConnectToSocket();
        }

    }

    void CheckWon(GraphNode unused)
    {
        if(goalGraph.Edges.Count != changeGraph.Edges.Count)
        {
            Debug.Log("Wrong number of edges!");
            return;
        }
        foreach (GraphEdge edge in goalGraph.Edges)
        {
            if(!edge.MappedCorrectly)
            {
                Debug.Log("Not all edges mapped correctly");
                return;
            }
        }
        List<List<int>> isomorphism = new List<List<int>>();
        foreach (GraphNode node in goalGraph.Nodes)
        {
            if (!node.IsMapped)
            {
                Debug.Log("Not all nodes mapped");
                return;
            }
            isomorphism.Add(new List<int>(){node.LevelTextId, node.MappedNode.LevelTextId});
        }
        //The level has been won
        if (GameState.OnlineLevel) {
            GameState.SendLevelSolution(isomorphism);
        }

        //Check whether a second medal is earned
        int medals = 1;
        int currentMedals = GameState.CurrentMedals[GameState.CurrentWorld-1][GameState.CurrentLevel-1];
        if(GameState.MovesGoal==-1 || GameState.MovesUsed<=GameState.MovesGoal)
        {
            medals++;
        }
        if(currentMedals>medals)
        {
            medals = currentMedals;
        } 
        
        // Set the new unlocked levels
        GameState.SetMedals(GameState.CurrentWorld,GameState.CurrentLevel,medals);
        // play some sound
        AudioManager.Play(AudioClipName.LevelWin);
        // open the level-won pop-up menu
        MenuManager.GoToMenu(MenuName.Won);
        print("Yay, you won!");
    }





    IEnumerator LoadLevel( int id )
    {
        // set the game state to allow for no rearrangement
        GameState.HeadMovesAllowed = false;
        GameState.TailMovesAllowed = false;
        GameState.LevelMessage     = "";
        GameState.MovesGoal        = -1;
        GameState.MovesGoalHead    = -1;
        GameState.MovesGoalTail    = -1;
        currentLabelId = 0;

        // set the graphs to new empty graphs
        goalGraph = new DiGraph();
        goalGraph.Movable = false;
        goalNodesByID = new Dictionary<int, GraphNode>();
        changeGraph = new DiGraph();
        changeGraph.Movable = true;
        changeNodesByID = new Dictionary<int, GraphNode>();
        graphs = new DiGraph[2] { goalGraph, changeGraph };
        graphNodesById = new Dictionary<int, GraphNode>[2] { goalNodesByID, changeNodesByID };
        
        //now read the file for the level
        //string levelPath = Path.Combine(Application.streamingAssetsPath, levelFileName);
        StreamReader input = null;

        //wait for the loading to finish
        print("starting load:");
        while(LevelUtils.Loading)
        {
            print("waiting for read level file.");
            yield return new WaitForSeconds(0.1f);
        }
        print("done loading!");

        try
        {
            //Set the level name to standard: "Level: [levelnumber]"
            GameState.LevelName = "";
            //Default separation 
            separation = separationDefault;
            using (Stream stream = GenerateStreamFromString(LevelUtils.LevelText))
            {
                input = new StreamReader(stream);
                string line = input.ReadLine();
                while (line.Length > 0)
                {
                    ParseDataValues(line);
                    line = input.ReadLine();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            if (input != null)
            {
                input.Close();
            }
        }
        ClampAllNodesToSocket();
        GameState.NumberOfLabels=currentLabelId;
        GameState.ResetLevel();
    }
   
    

    public static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }





    void ParseDataValues(string line)
    {
        string[] values = line.Split(',');
        string type = values[0];
        int graphNo;
        switch (type)
        {
            case "S":
                //If used, must occur before the nodes!
               separation = float.Parse(values[1]);
                break;
            case "V":
//                Debug.Log("Adding node");
                graphNo = int.Parse(values[1]);
                int nodeId = int.Parse(values[2]);
                float posx = (float.Parse(values[3]) + separation*graphNo) * ScreenUtils.GameplayWidth / (1+separation);
                float posy = -float.Parse(values[4]) * ScreenUtils.GameplayHeight;
                Vector3 pos = new Vector3(posx, posy, 0)+ ScreenUtils.GameplayTopLeft;
                GraphNode newNode = graphs[graphNo].AddNode(pos);
                graphNodesById[graphNo][nodeId] = newNode;
                newNode.LevelTextId = nodeId;
                break;
            case "E":
//                Debug.Log("Adding an edge");
//              Must occur after the endpoints of the edge have been added!
                graphNo = int.Parse(values[1]);
                int tailId = int.Parse(values[2]);
                int headId = int.Parse(values[3]);
                GraphNode tail = graphNodesById[graphNo][tailId];
                GraphNode head = graphNodesById[graphNo][headId];
                GraphEdge edge = graphs[graphNo].AddEdge(tail, head);
                edge.UpdatePosition();
                break;
            case "L":
//                Debug.Log("Adding label");
                List<GraphNode> allNodesSameLabel = new List<GraphNode>();
                for (int i = 1; i< values.Length; i=i+2)
                {
                    int graph_No = int.Parse(values[i]);
                    int node_Id = int.Parse(values[i+1]);
                    allNodesSameLabel.Add(graphNodesById[graph_No][node_Id]);
                }
                foreach (GraphNode node in allNodesSameLabel)
                {
                    node.SetLabel(currentLabelId, allNodesSameLabel);
                }
                currentLabelId++;
                break;
            case "M":
//                Debug.Log("Adding move type");
                string moveType = values[1];
                switch(moveType)
                {
                    case "T":
                        GameState.TailMovesAllowed = true;
                        break;
                    case "H":
                        GameState.HeadMovesAllowed = true;
                        break;
                }
                break;
            case "MESSAGE":
                GameState.LevelMessage = values[1];
                break;
            case "GOAL":
                if (values.Length==3)
                {
                    GameState.MovesGoalTail = int.Parse(values[1]);
                    GameState.MovesGoalHead = int.Parse(values[2]);
                }
                else
                {
                    GameState.MovesGoal = int.Parse(values[1]);
                }
                break;
        }
    }




}
