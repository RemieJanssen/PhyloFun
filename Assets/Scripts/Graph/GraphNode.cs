using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// A graph node
/// </summary>
public class GraphNode : MonoBehaviour
{
    #region Fields

    // graph support
    DiGraph graph;
    List<GraphNode> children;
    List<GraphNode> parents;
    int id;
    int levelTextId;

    // type support
    [SerializeField]
    Sprite movableNode;
    [SerializeField]
    Sprite immovableNode;
    [SerializeField]
    Sprite movableNodeLabeled;
    [SerializeField]
    Sprite immovableNodeLabeled;
    [SerializeField]
    Sprite immovableNodeLabeledHighlighted;

    bool movable;
    bool rearrangementEndpoint;
    bool labeled;

    // body support
    Rigidbody2D rb2D;
    CircleCollider2D collider;
    float nodeRadius;

    // drag support
    Vector3 previousPosition;
    Vector3 currentPosition;
    float tapTime;
    bool dragging = false;
    bool dragCorresponding = false;
    GraphEdge current_edge_socket;
    float dragUpBoundary; 
    float dragDownBoundary;
    Vector3 dragOffset;
    float zValue;




    // socket support
    ContactFilter2D nodeFilter;
    ContactFilter2D edgeFilter;
    Collider2D[] results;
    bool isMapped;
    GraphNode mappedNode;
    bool mappedCorrectly;
    List<GraphNode> sameLabelNodes;
    int labelId;
    MappingEvent mappingEvent;



    // rearrangement support
    StartRearrangement rearrangementEvent;
    GraphNode otherEndpointRearrEdge;
    bool rearranging;


    #endregion


    #region Initialize
    public void AddRearrangementListener(UnityAction<GraphNode> listener)
    {
        rearrangementEvent.AddListener(listener);
        mappingEvent = new MappingEvent();
        EventManager.AddMappingInvoker(this);

    }

    public void Initialize(int id)
    {
	
        // body
        rb2D = GetComponent<Rigidbody2D>();
        previousPosition = transform.position;
        collider = GetComponent<CircleCollider2D>();
        nodeRadius = collider.radius * GameConstants.NodeBaseSize;
        zValue = 1;

        // graph node
        this.id = id;
        children = new List<GraphNode>();
        parents = new List<GraphNode>();
        dragUpBoundary = ScreenUtils.GameplayTop;
        dragDownBoundary = ScreenUtils.GameplayBottom;

        // isomorphism
        nodeFilter = new ContactFilter2D();
        nodeFilter.layerMask = LayerMask.GetMask("ImmovableNodes");
        nodeFilter.useLayerMask = true;
        results = new Collider2D[1];
        isMapped = false;
        labeled = false;

        // rearrangement
        rearrangementEvent = new StartRearrangement();
        EventManager.AddRearrangementInvoker(this);
        edgeFilter = new ContactFilter2D();
        edgeFilter.layerMask = LayerMask.GetMask("MovableEdges");
        edgeFilter.useLayerMask = true;

    }

    #endregion


    #region Properties




    public int LabelId
    {
        get { return labelId; }
    }

    public int LevelTextId
    {
        get { return levelTextId; }
        set { levelTextId = value; }
    }


    public bool Labeled
    {
        get { return labeled; }
    }


    public List<GraphNode> SameLabelNodes
    {
        get { return sameLabelNodes; }
    }

    public void ResetLabel()
    {
        labeled = false;
        SetSprite();
        SetSpriteColour();
    }

    public void SetLabel(int label_Id, List<GraphNode> same_Labeled_Nodes)
    {
        labeled = true;
        labelId = label_Id;
        sameLabelNodes = same_Labeled_Nodes;
        SetSprite();
        SetSpriteColour();
    }


    public bool RearrangementEndpoint
    {
        get { return rearrangementEndpoint; }
        set 
        { 
            rearrangementEndpoint = value; 
            otherEndpointRearrEdge = null;
            foreach (GraphNode parent in Parents)
            {
                otherEndpointRearrEdge = parent;
            }
            foreach (GraphNode child in Children)
            {
                otherEndpointRearrEdge = child;
            }
            SetSpriteColour();
            
        }
    }


    public bool MappedCorrectly
    {
        get { return mappedCorrectly; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the node is mapped to another node
    /// </summary>
    /// <value><c>true</c> if is mapped; otherwise, <c>false</c>.</value>
    public bool IsMapped
    {
        get { return isMapped; }
        set { isMapped = value; }
    }

    /// <summary>
    /// Gets or sets node this node is mapped to.
    /// </summary>
    /// <value>The mapped node.</value>
    public GraphNode MappedNode
    {
        get { return mappedNode; }
        set { mappedNode = value; }
    }


    /// <summary>
    /// Gets or sets whether the node is movable
    /// Setting the variable also changes the colour of the node accordingly.
    /// </summary>
    /// <value><c>true</c> if movable; otherwise, <c>false</c>.</value>
    public bool Movable
    {
        get { return movable; }
        set
        {
            movable = value;
            SetSize();
            SetSprite();
        }
    }




    /// <summary>
    /// Gets the game object.
    /// </summary>
    /// <value>The game object.</value>
    public GameObject GameObject
    {
        get { return gameObject; }
    }

    public int Id
    {
        get { return id; }
    }

    /// <summary>
    /// Gets the graph the node belongs to.
    /// </summary>
    /// <value>The graph.</value>
    public DiGraph Graph
    {
        get { return graph; }
        set { graph = value; }
    }

    /// <summary>
    /// Gets a read-only list of the children of the node
    /// </summary>
    public IList<GraphNode> Children
    {
        get { return children.AsReadOnly(); }
    }

    /// <summary>
    /// Gets a read-only list of the parents of the node
    /// </summary>
    public IList<GraphNode> Parents
    {
        get { return parents.AsReadOnly(); }
    }

    /// <summary>
    /// Gets or sets the position.
    /// Setting the position here should only be done when initializing
    /// </summary>
    /// <value>The position.</value>
    public Vector3 Position
    {
        get { return gameObject.transform.position; }
        set 
        { 
            Vector3 newPos = value;
            newPos.z = zValue;
            GameObject.transform.position = newPos;
            PreviousPosition = Position;
        }
    }

    public Vector3 PreviousPosition
    {
        get { return previousPosition; }
        set { previousPosition = value; }
    }


    #endregion

    #region Methods Object

    Color IdToColor(int id){
        int reduced_Id = id;
        float current_factor = 1.0f;
        float current_c = 0.0f;
        while (reduced_Id>0){
            current_c += current_factor*(reduced_Id%3);
            current_factor *= 0.3f;
            reduced_Id = reduced_Id/3;
        }
        float[] colourValues = new float[3]{1.0f,1.0f,1.0f};
        int main_colour = (int) current_c;
        int second_colour = (main_colour+1)%3;
        colourValues[main_colour]=current_c-main_colour;
        colourValues[second_colour]=1.0f - colourValues[main_colour];
        Color newColor = new Color(colourValues[0], colourValues[1], colourValues[2], 1.0f);
        Debug.Log("colourValues");
        Debug.Log(colourValues[0]);
        Debug.Log(colourValues[1]);
        Debug.Log(colourValues[2]);
        return newColor;

    }

    void SetSpriteColour()
    {
        SpriteRenderer sprRend = GetComponent<SpriteRenderer>();
        sprRend.material.color = Color.white;
        if (rearrangementEndpoint){
            sprRend.material.color = new Color(0.8f,0.8f,1.0f,1.0f);
        }
        
        if (labeled){
            sprRend.material.color = IdToColor(labelId);
        }
        
    }

    void SetSize()
    {
	    float size = GameConstants.NodeBaseSize;
            if(movable)
            {
                transform.localScale = new Vector3( size, size, size );
                GameObject.layer = 8;
                Vector3 pos = Position;
                pos.z = -1;
                Position = pos;
                previousPosition = pos;
                zValue = -1;
            }
            else
            {
		        size = size *1.2f;
                transform.localScale = new Vector3( size, size, size );
                GameObject.layer = 9;
                Vector3 pos = Position;
                pos.z = 1;
                Position = pos;
                previousPosition = pos;
                zValue = 1;
            }    
    }


    void SetSprite()
    {
        if (movable && labeled)
        {
            GetComponent<SpriteRenderer>().sprite = movableNodeLabeled;
        }
        else if (movable && !labeled)
        {
            GetComponent<SpriteRenderer>().sprite = movableNode;
        }
        else if (!movable && labeled)
        {
            if(dragCorresponding)
            {
                GetComponent<SpriteRenderer>().sprite = immovableNodeLabeledHighlighted;
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = immovableNodeLabeled;
            }
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = immovableNode;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        if (dragging)
        {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - dragOffset;
            newPosition.x = ClampedX(newPosition.x, ScreenUtils.GameplayLeft, ScreenUtils.GameplayRight);
            newPosition.y = ClampedY(newPosition.y, dragDownBoundary, dragUpBoundary);
            newPosition.z = zValue;
            rb2D.MovePosition(newPosition);
            UpdateAdjacentEdgePositions();
            
            GraphEdge socket = CheckForSocketEdge();
            if(socket != current_edge_socket && rearrangementEndpoint){
                if(current_edge_socket!=null){
                    current_edge_socket.Highlighted=false;
                }
                current_edge_socket=null;
                if(socket!=null && !socket.Tail.RearrangementEndpoint        && !socket.Head.RearrangementEndpoint 
                                &&  socket.Tail!=GameState.RearrangementNode &&  socket.Head!=GameState.RearrangementNode
                                &&  socket.Tail!=otherEndpointRearrEdge      &&  socket.Head!=otherEndpointRearrEdge)
                {
                    current_edge_socket=socket;
                    current_edge_socket.Highlighted=true;
                }
            }

        }
    }

    public void StopMoving()
    {
        rb2D.velocity = Vector2.zero;
    }

    public void OnMouseDownHandling()
    {
        if (graph.Movable && (!GameState.InRearrangementMode || rearrangementEndpoint))
        {
            tapTime = Time.time;
            dragging = true;
            if(labeled)
            {
                foreach (GraphNode node in sameLabelNodes)
                {
                    node.DraggingCorrespondingNode(true);
                }
            }
            SetMovingAdjacentEdges(true);
            SetUpDownBoundaries();
            dragOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        }
    }

    public void DraggingCorrespondingNode(bool draggingOtherNode)
    {
        dragCorresponding = draggingOtherNode;
        SetSprite();
    }


    public void ConnectToSocket()
    {
        // Snap to a socket node if possible
        Vector3 newPosition = CheckForSocketNode();
        // Set the new position
        newPosition.z = zValue;
        transform.position = newPosition;
        // Update all edges around the updated node
        UpdateAdjacentEdgePositions();
        CheckMapping();
        // Set the previous position for next tap etc.
        previousPosition = transform.position;
    }

    public void OnMouseUpHandling()
    {
        if (labeled)
        {
            foreach (GraphNode node in sameLabelNodes)
            {
                node.DraggingCorrespondingNode(false);
            }
        }
        if (dragging)
        {
            dragging = false;
            SetMovingAdjacentEdges(false);
            //On tap, do the following:
            if (Time.time - tapTime < GameConstants.TapDelay)
            {
                transform.position = previousPosition;
                UpdateAdjacentEdgePositions();
                bool split = SplitEndpoints();
                if(split)
                {
                    rearrangementEvent.Invoke(this);
                }
            }
            //After dragging, do the following:
            else
            {
                //if the dragged thing was not the endpoint of a half-edge
                if (!rearrangementEndpoint)
                {
                    Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - dragOffset;
                    // Clamp the node in the region between its child and parent nodes
                    newPosition.x = ClampedX(newPosition.x, ScreenUtils.GameplayLeft, ScreenUtils.GameplayRight);
                    newPosition.y = ClampedY(newPosition.y, dragDownBoundary, dragUpBoundary);
                    transform.position = newPosition;
                    ConnectToSocket();
                }
                //if the dragged thing was the endpoint of a half-edge
                else
                {
                    // Check if the endpoint was dragged on an edge it could end up on
                    GraphEdge socket = current_edge_socket;
                    // this is the case:
                    if(socket!=null)
                    {
                        current_edge_socket.Highlighted = false;
                        current_edge_socket = null;
                        GraphEdge movingEdge = null;
                        foreach(GraphNode child in children)
                        {
                            movingEdge = graph.Find(this, child);
                        }
                        foreach (GraphNode parent in parents)
                        {
                            movingEdge = graph.Find(parent,this);
                        }
                        GameState.Rearrange(this, movingEdge, socket);
                        GameState.EndRearrangement();
                    }
                    // otherwise, snap back to original position.
                    else
                    {
                        transform.position = previousPosition;
                        UpdateAdjacentEdgePositions();
                    }
                }
            }
        }
    }

    bool SplitEndpoints()
    {
        int noOfParents = parents.Count;
        int noOfChildren = children.Count;
        bool split = false;
        GraphNode newEndpoint;
        GraphEdge newEdge;
        if (noOfParents>1 && GameState.HeadMovesAllowed)
        {
            foreach(GraphNode parent in parents)
            {
                // Hide actual parent edge
                graph.Find(parent, this).Hidden = true;
                // Make new endpoint to move
                newEndpoint = graph.AddNode(transform.position + 1.5f*nodeRadius * Vector3.Normalize(parent.Position - transform.position));
			newEndpoint.LevelTextId = this.LevelTextId;
                newEdge = graph.AddEdge(parent, newEndpoint);
                newEdge.NonSocket(true);
                newEndpoint.RearrangementEndpoint = true;
                GameState.AddEndpoint(newEndpoint);
                newEndpoint.UpdateAdjacentEdgePositions();
            }
            split = true;
        }
        if(noOfChildren>1 && GameState.TailMovesAllowed)
        {
            foreach (GraphNode child in children)
            {
                // Hide actual child edge
                graph.Find(this,child).Hidden = true;
                // Make new endpoint to move
                newEndpoint = graph.AddNode(transform.position + 1.5f*nodeRadius * Vector3.Normalize(child.Position - transform.position));
                newEdge = graph.AddEdge(newEndpoint,child);
                newEdge.NonSocket(true);
                newEndpoint.RearrangementEndpoint = true;
                GameState.AddEndpoint(newEndpoint);
                newEndpoint.UpdateAdjacentEdgePositions();
            }
            split = true;
        }
        return split;
    }


    void SetUpDownBoundaries()
    {
        dragUpBoundary = ScreenUtils.GameplayTop;
        foreach (GraphNode node in Parents)
        {
            dragUpBoundary = Mathf.Min(dragUpBoundary, node.Position.y);
        }
        dragDownBoundary = ScreenUtils.GameplayBottom;
        foreach (GraphNode node in Children)
        {
            dragDownBoundary = Mathf.Max(dragDownBoundary, node.Position.y);
        }
    }

    float ClampedX(float xCoordinate, float minX, float maxX)
    {
//        Debug.Log($"{xCoordinate}, {nodeRadius}, {minX}, {maxX}");
        if (xCoordinate + 2*nodeRadius > maxX)
        {
            return maxX - 2*nodeRadius;
        }
        else if (xCoordinate - 2*nodeRadius < minX)
        {
            return minX + 2*nodeRadius;
        }
        return xCoordinate;
    }

    float ClampedY(float yCoordinate, float minY, float maxY)
    {
        if (yCoordinate + 2*nodeRadius > maxY)
        {
            return maxY - 2*nodeRadius;
        }
        else if (yCoordinate - 2*nodeRadius < minY)
        {
            return minY + 2*nodeRadius;
        }
        return yCoordinate;
    }

    public void UpdateAdjacentEdgePositions()
    {
        GraphEdge edge;
        foreach (GraphNode child in children)
        {
            edge = graph.Find(this, child);
            edge.UpdatePosition();
        }
        foreach (GraphNode parent in parents)
        {
            edge = graph.Find(parent, this);
            edge.UpdatePosition();
        }
    }

    void SetMovingAdjacentEdges(bool moving)
    {
        foreach (GraphNode parent in parents)
        {
            graph.Find(parent, this).Moving = moving;
        }
        foreach (GraphNode child in children)
        {
            graph.Find(this, child).Moving = moving;
        }

    }


    Vector3 CheckForSocketNode()
    {
        int colliderCount = collider.OverlapCollider(nodeFilter,results);
        if (colliderCount > 0)
        {
            GraphNode socket = results[0].gameObject.GetComponent<GraphNode>();
            bool correctNode = true;
            if(labeled)
            {
                if (!socket.Labeled || labelId!=socket.LabelId)
                {
                    SetMapping(null);
                    correctNode = false;
                }
            }
            else if (socket.Labeled)
            {
                SetMapping(null);
                correctNode = false;
            }
            if (correctNode && (!socket.isMapped || socket.MappedNode == this))
            {
                SetMapping(socket);
                return socket.gameObject.transform.position;
            }
        }
        else
        {
            SetMapping(null);
        }
        return transform.position;
    }

    GraphEdge CheckForSocketEdge()
    {
        int colliderCount = collider.OverlapCollider(edgeFilter, results);
        if (colliderCount > 0)
        {
            GraphEdge socket = results[0].gameObject.GetComponent<GraphEdge>();
            return socket;
        }
        return null;
    }




    #endregion

    #region Isomorphism 
    public void SetMapping(GraphNode otherNode)
    {
        if(mappedNode!=null)
        {
            mappedNode.IsMapped = false;
            mappedNode.MappedNode = null;
            mappedNode.CheckMapping();
        }
        if (otherNode==null)
        {
            isMapped = false;
            mappedNode = null;
        }
        else
        {
            otherNode.isMapped = true;
            otherNode.mappedNode = this;
            isMapped = true;
            mappedNode = otherNode;
            mappedNode.CheckMapping();
            mappingEvent.Invoke(this);
        }
        CheckMapping();
    }

    public bool CheckMapping()
    {
        bool mappedCorrectlyMaybe = true;
        foreach(GraphNode parent in parents)
        {
            mappedCorrectlyMaybe =  graph.Find(parent, this).CheckIsomorphism() && mappedCorrectlyMaybe;
        }
        foreach(GraphNode child in children)
        {
            mappedCorrectlyMaybe =  graph.Find( this, child).CheckIsomorphism() && mappedCorrectlyMaybe;
        }
        mappedCorrectly = mappedCorrectlyMaybe;
        if(mappedNode!=null)
        {
            mappedNode.mappedCorrectly = mappedCorrectly;
        }
        return mappedCorrectly;
    }

    public void AddMappingListener(UnityAction<GraphNode> listener)
    {
        mappingEvent.AddListener(listener);
    }

    #endregion

    #region Methods Graph


    /// <summary>
    /// Adds the given node as a neighbor for this node
    /// </summary>
    /// <param name="child">neighbor to add</param>
    /// <returns>true if the neighbor was added, false otherwise</returns>
    public bool AddChild(GraphNode child)
    {
        // don't add duplicate nodes
        if (children.Contains(child))
        {
            return false;
        }
        else
        {
            children.Add(child);
            return true;
        }
    }

    /// <summary>
    /// Removes the given node as a neighbor for this node
    /// </summary>
    /// <param name="child">neighbor to remove</param>
    /// <returns>true if the neighbor was removed, false otherwise</returns>
    public bool RemoveChild(GraphNode child)
    {
        // only remove neighbors in list
        return children.Remove(child);
    }

    /// <summary>
    /// Adds the given node as a neighbor for this node
    /// </summary>
    /// <param name="parent">neighbor to add</param>
    /// <returns>true if the neighbor was added, false otherwise</returns>
    public bool AddParent(GraphNode parent)
    {
        // don't add duplicate nodes
        if (parents.Contains(parent))
        {
            return false;
        }
        else
        {
            parents.Add(parent);
            return true;
        }
    }

    /// <summary>
    /// Removes the given node as a neighbor for this node
    /// </summary>
    /// <param name="parent">neighbor to remove</param>
    /// <returns>true if the neighbor was removed, false otherwise</returns>
    public bool RemoveParent(GraphNode parent)
    {
        // only remove neighbors in list
        return parents.Remove(parent);
    }



    /// <summary>
    /// Removes all the neighbors for the node
    /// </summary>
    /// <returns>true if the neighbors were removed, false otherwise</returns>
    public bool RemoveAllNeighbors()
    {
        RemoveAllChildren();
        RemoveAllParents();
        return true;
    }


    /// <summary>
    /// Removes all the neighbors for the node
    /// </summary>
    /// <returns>true if the neighbors were removed, false otherwise</returns>
    public bool RemoveAllChildren()
    {
        for (int i = children.Count - 1; i >= 0; i--)
        {
            children.RemoveAt(i);
        }
        return true;
    }

    /// <summary>
    /// Removes all the neighbors for the node
    /// </summary>
    /// <returns>true if the neighbors were removed, false otherwise</returns>
    public bool RemoveAllParents()
    {
        for (int i = parents.Count - 1; i >= 0; i--)
        {
            parents.RemoveAt(i);
        }
        return true;
    }




    /// <summary>
    /// Converts the node to a string
    /// </summary>
    /// <returns>the string</returns>
    public override string ToString()
    {
        Vector2 gpPos = ScreenUtils.WorldToGameplayPosition(transform.position);
        string vertexInfo = $"V,0,{LevelTextId},{gpPos.x},{gpPos.y}";
        return vertexInfo;
    }

    #endregion
}

