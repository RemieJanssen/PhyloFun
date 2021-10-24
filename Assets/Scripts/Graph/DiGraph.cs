using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


/// <summary>
/// A graph
/// </summary>
/// <typeparam name="T">type of values stored in graph</typeparam>
public class DiGraph
{
    #region Fields
    
    List<GraphNode> nodes = new List<GraphNode>();
    List<GraphEdge> edges = new List<GraphEdge>();
    bool movable;

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    public DiGraph()
    {
    }

    #endregion

    #region Properties

    public bool Movable
    {
        get { return movable; }
        set 
        { 
            if(movable != value)
            {
                movable = value;
                foreach(GraphNode node in nodes)
                {
                    node.Movable = value;
                }
                foreach(GraphEdge edge in edges)
                {
                    edge.Movable = value;
                }
            }
        }
    }


    /*
    /// <summary>
    /// Gets the number of nodes in the graph
    /// </summary>
    public int Count
    {
        get { return nodes.Count; }
    }
    */

    /// <summary>
    /// Gets a read-only list of the nodes in the graph
    /// </summary>
    public IList<GraphNode> Nodes
    {
        get { return nodes.AsReadOnly(); }
    }

    public IList<GraphEdge> Edges
    {
        get { return edges.AsReadOnly(); }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Clears all the nodes from the graph
    /// </summary>
    public void Clear()
    {
        // remove all the neighbors from each node
        // so nodes can be garbage collected
        foreach (GraphNode node in nodes)
        {
            node.RemoveAllNeighbors();
        }

        //similarly for edges
        foreach (GraphEdge edge in edges)
        {
            edge.RemoveEdge();
        }

        // now remove all the nodes from the graph
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            GraphNode node = nodes[i];
            nodes.RemoveAt(i);
            ObjectPool.ReturnNode(node.GameObject);
        }

        // and remove the edges as well
        for (int i = edges.Count -1; i>=0; i--)
        {
            GraphEdge edge = edges[i];
            edges.RemoveAt(i);
            ObjectPool.ReturnEdge(edge.GameObject);
        }
    }

    /// <summary>
    /// Adds a node with the given value to the graph. If a node
    /// with the same value is in the graph, the value 
    /// isn't added and the method returns false
    /// </summary>
    /// <param name="node">value to add</param>
    /// <returns>true if the value is added, false otherwise</returns>
    public GraphNode AddNode(Vector3 pos)
    {
        GraphNode newGraphNode = ObjectPool.GetNode().GetComponent<GraphNode>();
        newGraphNode.Graph = this;
        nodes.Add(newGraphNode);
        newGraphNode.Movable = movable;
        newGraphNode.Position = pos;
        newGraphNode.PreviousPosition = pos;
        newGraphNode.RearrangementEndpoint = false;
        newGraphNode.ResetLabel();
        return newGraphNode;
    }

    /// <summary>
    /// Adds an edge between the nodes with the given values 
    /// in the graph. If one or both of the values don't exist 
    /// in the graph the method returns false. If an edge
    /// already exists between the nodes the edge isn't added
    /// and the method retruns false
    /// </summary>
    /// <param name="tail">first value to connect</param>
    /// <param name="head">second value to connect</param>
    /// <returns>true if the edge is added, false otherwise</returns>
    public GraphEdge AddEdge(GraphNode tail, GraphNode head)
    {
        if (tail == null ||
            head == null)
        {
            return null;
        }
        else if (tail.Children.Contains(head))
        {
            // edge already exists
            return null;
        }
        else
        {
            // directed graph, so add edge from node1 to node2
            tail.AddChild(head);
            head.AddParent(tail);
            GraphEdge newGraphEdge = ObjectPool.GetEdge().GetComponent<GraphEdge>();
            newGraphEdge.Tail = tail;
            newGraphEdge.Head = head;
            newGraphEdge.Hidden = false;
            edges.Add(newGraphEdge);
            newGraphEdge.Movable = movable;
            return newGraphEdge;
        }
    }

    /// <summary>
    /// Removes the node with the given value from the graph.
    /// If the node isn't found in the graph, the method 
    /// returns false
    /// </summary>
    /// <param name="node">value to remove</param>
    /// <returns>true if the value is removed, false otherwise</returns>
    public bool RemoveNode(GraphNode removeGraphNode)
    {
        if (removeGraphNode == null)
        {
            return false;
        }
        else
        {
            // need to remove as neighbour for all nodes
            // in graph
            while(removeGraphNode.Parents.Count>0)
            {
                RemoveEdge(removeGraphNode.Parents[removeGraphNode.Parents.Count - 1], removeGraphNode);                
                //removeGraphNode.Parents[i].RemoveChild(removeGraphNode);
            }
            while (removeGraphNode.Children.Count > 0)
            {
                RemoveEdge(removeGraphNode, removeGraphNode.Children[removeGraphNode.Children.Count - 1]);
                //removeGraphNode.Children[i].RemoveParent(removeGraphNode);
            }
            nodes.Remove(removeGraphNode);
            ObjectPool.ReturnNode(removeGraphNode.GameObject);
            return true;
        }
    }

    /// <summary>
    /// Removes an edge between the nodes with the given values 
    /// from the graph. If one or both of the values don't exist 
    /// in the graph the method returns false
    /// </summary>
    /// <param name="value1">first value to disconnect</param>
    /// <param name="value2">second value to disconnect</param>
    /// <returns>true if the edge is removed, false otherwise</returns>
    public bool RemoveEdge(GraphNode node1, GraphNode node2)
    {
        if (node1 == null ||
            node2 == null)
        {
            return false;
        }
        else if (!node1.Children.Contains(node2))
        {
            Debug.Log("edge doesn't exist");
            return false;
        }
        else
        {
            // directed graph, so remove node2 as neighbor of node 1
            GraphEdge edge = Find(node1, node2);
            node1.RemoveChild(node2);
            node2.RemoveParent(node1);
            edges.Remove(edge);
            ObjectPool.ReturnEdge(edge.GameObject);
            return true;
        }
    }

    /*
    /// <summary>
    /// Finds the graph node with the given value
    /// </summary>
    /// <param name="value">value to find</param>
    /// <returns>graph node or null if not found</returns>
    public GraphNode Find(GraphNode value)
    {
        foreach (GraphNode node in nodes)
        {
            if (node.Equals(value))
            {
                return node;
            }
        }
        return null;
    }
    */

    /// <summary>
    /// Finds the graph edge with the given values
    /// </summary>
    /// <param name="valueX">values to find</param>
    /// <returns>graph edge or null if not found</returns>
    public GraphEdge Find(GraphNode node1, GraphNode node2)
    {
        foreach (GraphEdge edge in edges)
        {
            if (edge.Tail.Id == node1.Id  && edge.Head.Id == node2.Id)
            {
                return edge;
            }
        }
        return null;
    }




    /// <summary>
    /// Converts the Graph to a comma-separated string of nodes
    /// </summary>
    /// <returns>comma-separated string of nodes</returns>
    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < nodes.Count; i++)
        {
            builder.Append(nodes[i].ToString());
            builder.Append("\r\n");
        }
        for (int i = 0; i < edges.Count; i++)
        {
            builder.Append(edges[i].ToString());
            builder.Append("\r\n");
        }
        return builder.ToString();
    }

    #endregion
}
