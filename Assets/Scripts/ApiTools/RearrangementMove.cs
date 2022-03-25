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
        movingEndpointId = movingEndpoint.Id;
        
        movingEdge = movingEd;
        movingEdgeTailId = movingEdge.Tail.Id;
        movingEdgeHeadId = movingEdge.Head.Id;

        goalEdge = goalEd;
        goalEdgeTailId = goalEdge.Tail.Id;
        goalEdgeHeadId = goalEdge.Head.Id;

        originEdgeTail = originEdTail;
        originEdgeHead = originEdHead;
        originEdgeTailId = originEdgeTail.Id;
        originEdgeHeadId = originEdgeHead.Id;

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
        return $"endpoint {movingEndpoint.Id} of ({movingEdgeTailId}, {movingEdgeHeadId}) from ({originEdgeTailId}, {originEdgeHeadId}) to ({goalEdgeTailId}, {goalEdgeHeadId})";
    }
    
    public MoveType Type()
    {
        if (movingEndpoint == movingEdge.Tail){
            return MoveType.TAIL;
        }
        return MoveType.HEAD;
    }
    
    public string ToJson()
    {
        string type = "Tail";
        if (Type()==MoveType.HEAD){
            type = "Head";
        }
        return $"{{\"move_type\": \"{type}\", \"origin\": ({originEdgeTailId}, {originEdgeHeadId}),  \"moving_edge\": ({movingEdgeTailId}, {movingEdgeHeadId}),  \"target\": ({goalEdgeTailId}, {goalEdgeHeadId})}}";
    }
    

    #endregion
}

