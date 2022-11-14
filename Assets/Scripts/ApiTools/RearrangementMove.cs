using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

/// <summary>
/// A rearrangement move
/// </summary>
public class RearrangementMove
{
    #region Fields

    public int movingEndpointId {get; }
    public int movingEdgeTailId {get; }
    public int movingEdgeHeadId {get; }
    public int goalEdgeTailId {get; }
    public int goalEdgeHeadId {get; }
    public int originEdgeTailId {get; }
    public int originEdgeHeadId {get; }

    GraphNode movingEndpoint;
    GraphEdge movingEdge;
    GraphEdge goalEdge;
    GraphNode originEdgeTail;
    GraphNode originEdgeHead;
    #endregion


    #region Initialize
    public RearrangementMove(GraphNode movingEndpt, GraphEdge movingEd, GraphEdge goalEd, GraphNode originEdTail, GraphNode originEdHead)
    {
        movingEndpoint = movingEndpt;
        movingEndpointId = movingEndpoint.LevelTextId;
        
        movingEdge = movingEd;
        movingEdgeTailId = movingEdge.Tail.LevelTextId;
        movingEdgeHeadId = movingEdge.Head.LevelTextId;

        goalEdge = goalEd;
        goalEdgeTailId = goalEdge.Tail.LevelTextId;
        goalEdgeHeadId = goalEdge.Head.LevelTextId;

        originEdgeTail = originEdTail;
        originEdgeHead = originEdHead;
        originEdgeTailId = originEdgeTail.LevelTextId;
        originEdgeHeadId = originEdgeHead.LevelTextId;

	}
    #endregion


    #region Properties

    public GraphNode MovingEndpoint
    {
        get { return movingEndpoint; }
    }

    public GraphEdge MovingEdge
    {
        get { return movingEdge; }
    }

    public GraphEdge GoalEdge
    {
        get { return goalEdge; }
    }



    /// <summary>
    /// Converts the move to a string
    /// </summary>
    /// <returns>the string</returns>
    public override string ToString()
    {
        return $"endpoint {movingEndpointId} of ({movingEdgeTailId}, {movingEdgeHeadId}) from ({originEdgeTailId}, {originEdgeHeadId}) to ({goalEdgeTailId}, {goalEdgeHeadId})";
    }
    
    public MoveType Type()
    {
        if (movingEndpointId == movingEdgeTailId){
            return MoveType.TAIL;
        }
        return MoveType.HEAD;
    }
    
    public string ToJson()
    {
        string type = "TAIL";
        if (Type()==MoveType.HEAD){
            type = "HEAD";
        }
        return $"{{\"move_type\": \"{type}\", \"origin\": [{originEdgeTailId}, {originEdgeHeadId}],  \"moving_edge\": [{movingEdgeTailId}, {movingEdgeHeadId}],  \"target\": [{goalEdgeTailId}, {goalEdgeHeadId}]}}";
    }
    

    public string ToJsonReversed(List<List<int>> isomorphism)
    {
        Dictionary<int, int> isomorphism_as_dict = new Dictionary<int, int>();
        foreach (List<int> pair in isomorphism){
            isomorphism_as_dict[pair[1]] = pair[0];
        }
        string type = "TAIL";
        if (Type()==MoveType.HEAD){
            type = "HEAD";
        }
        return $"{{\"move_type\": \"{type}\", \"origin\": [{isomorphism_as_dict[goalEdgeTailId]}, {isomorphism_as_dict[goalEdgeHeadId]}],  \"moving_edge\": [{isomorphism_as_dict[movingEdgeTailId]}, {isomorphism_as_dict[movingEdgeHeadId]}],  \"target\": [{isomorphism_as_dict[originEdgeTailId]}, {isomorphism_as_dict[originEdgeHeadId]}]}}";
    }

    #endregion
}

