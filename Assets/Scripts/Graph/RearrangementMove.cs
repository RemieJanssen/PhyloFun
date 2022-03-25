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

    GraphNode movingEndpoint;
    GraphEdge movingEdge;
    GraphEdge goalEdge;
    #endregion


    #region Initialize
    public RearrangementMove(GraphNode movingEndpt, GraphEdge movingEd, GraphEdge goalEd)
    {
        movingEndpoint = movingEndpt;
        movingEdge = movingEd;
        goalEdge = goalEd;
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
        return $"endpoint {movingEndpoint.Id} of ({movingEdge.Tail.Id}, {movingEdge.Head.Id}) to ({goalEdge.Tail.Id}, {goalEdge.Head.Id})";
    }

    #endregion
}

