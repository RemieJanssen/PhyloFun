using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A graph node
/// </summary>
public class GraphEdge : MonoBehaviour
{

    // Initialize when first used
    public void Initialize()
    {
        movable = true;
        mappedCorrectly = false;
        line = GetComponent<LineRenderer>();
        SetColour();
        collider = GetComponent<EdgeCollider2D>();
    }


    #region Fields

    LineRenderer line;
    GraphNode tail;
    GraphNode head;
    bool movable;
    bool mappedCorrectly;
    bool moving;
    bool highlighted;
    bool hidden;
    EdgeCollider2D collider;


    #endregion


    #region Properties

    public bool Hidden
    {
        get { return hidden; }
        set
        {
            hidden = value;
            NonSocket(hidden);
            SetColour();
        }
    }

    public bool Movable
    {
        get { return movable; }
        set
        {
            movable = value;
            if (movable)
            {
                SetColour();
                gameObject.layer = 10;
                GetComponent<Renderer>().sortingLayerName = "MovableEdges";
                /*
                Vector3 pos = transform.position;
                pos.z = -2;
                transform.position = pos;
                */               
            }
            else
            {
                SetColour();
                gameObject.layer = 11;
                GetComponent<Renderer>().sortingLayerName = "ImmovableEdges";
                /*
                Vector3 pos = transform.position;
                pos.z = 2;
                transform.position = pos;
                */
            }
        }
    }

    public bool Moving
    {
        get { return moving; }
        set 
        {
            moving = value;
            SetColour();
        }
    }

    public bool Highlighted
    {
        get { return highlighted; }
        set 
        {
            highlighted = value;
            SetColour();
        }
    }


    public bool MappedCorrectly
    {
        get { return mappedCorrectly; }
        set { mappedCorrectly = false; }
    }

    /// <summary>
    /// Gets the game object.
    /// </summary>
    /// <value>The game object.</value>
    public GameObject GameObject
    {
        get { return gameObject; }
    }

    /// <summary>
    /// Gets the tail of the edge
    /// </summary>
    public GraphNode Tail
    {
        get { return tail; }
        set { tail = value; }
    }

    /// <summary>
    /// Gets the head of the edge.
    /// </summary>
    public GraphNode Head
    {
        get { return head; }
        set { head = value; }
    }

    #endregion

    void SetMappedCorrectly(bool correct)
    {
        if (! correct==mappedCorrectly )
        {
            //set new correctness and colour
            mappedCorrectly = correct;
            SetColour();
        }
    }

    public void NonSocket(bool nonSocket)
    {
        if(nonSocket)
        {
            gameObject.layer = 12;
        }
        else
        {
            Movable = movable;
        }
    }




    public bool CheckIsomorphism()
    {
        if( head.IsMapped && tail.IsMapped)
        {
            GraphNode otherHead = head.MappedNode;
            GraphNode otherTail = tail.MappedNode;
            if(otherTail.Children.Contains(otherHead))
            {
                SetMappedCorrectly(true);
                return true;
            }
        }
        SetMappedCorrectly(false);
        return false;
    }

    public void SetEndpoints(Vector3 tailPosition, Vector3 headPosition)
    {
        line.SetPosition(0, tailPosition);
        line.SetPosition(1, headPosition);
        collider.points = new Vector2[2] { tailPosition, headPosition };
    }

    void SetColour()
    {
        Color currentColor = line.startColor;
        //immovable edges are blue
        if (!movable)
        {
            currentColor = Color.grey;
        }
        //moving edges are always blue
        else if (moving)
        {
            currentColor = Color.blue;
        }
        else if (highlighted)
        {
            currentColor = Color.red;
        }
        else
        {
            //if not moving, edges that are mapped correctly are green
            if (mappedCorrectly)
            {
                currentColor = Color.green;
            }
            else
            {
                //non-correct non-moving movable edges are black
                currentColor = Color.black;
            }
        }
        switch (hidden)
        {
            case true:
                currentColor.a = 0;
                break;
            case false:
                currentColor.a = 1;
                break;
        }
        line.startColor = currentColor;
        line.endColor = currentColor;

    }


    public void RemoveEdge()
    {
        tail = default(GraphNode);
        head = default(GraphNode);
        mappedCorrectly = false;
    }

    public void UpdatePosition()
    {
//        Console.WriteLine(tail);
        SetEndpoints(tail.Position, head.Position);
    }




    /// <summary>
    /// Converts the edge to a string
    /// </summary>
    /// <returns>the string</returns>
    public override string ToString()
    {
        return "E,,"+ tail.Id.ToString() + "," + head.Id.ToString();
    }

}

