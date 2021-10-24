using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectPool
{
    static GameObject prefabEdge;
    static List<GameObject> edgePool;

    static GameObject prefabNode;
    static List<GameObject> nodePool;

    

    static bool initialized = false;

    public static void Initialize()
    {   
        int initialPoolSize = 16;
    
        if(!initialized)
        {
            initialized = true;
            prefabEdge = Resources.Load<GameObject>("Edge");
            edgePool = new List<GameObject>();
            for (int i = 0; i < initialPoolSize; i++)
            {
                edgePool.Capacity++;
                edgePool.Add(GetNewEdge());
            }

            prefabNode = Resources.Load<GameObject>("Node");
            nodePool = new List<GameObject>();
            for (int i = 0; i < initialPoolSize; i++)
            {
                nodePool.Capacity++;
                nodePool.Add(GetNewNode());
            }
        }
    }


    #region return all

    public static void ReturnAllNodesAndEdges()
    {
        bool done = false;
        while (!done)
        {
            GameObject[] nodeList = GameObject.FindGameObjectsWithTag("Node");
            if(nodeList.Length==0)
            {
                done = true;
            }
            else
            {
                GraphNode node = nodeList[0].GetComponent<GraphNode>();
                DiGraph graph = node.Graph;
                graph.Clear();
            }
        }
    }

    #endregion

    #region edge


    public static void ReturnEdge(GameObject edge)
    {
        edge.SetActive(false);
        edgePool.Add(edge);
        edge.GetComponent<GraphEdge>().MappedCorrectly = false;
    }

    public static GameObject GetEdge()
    {
        GameObject edge;
        if (edgePool.Count == 0)
        {
            Debug.Log("Expanding edge pool...");
            edgePool.Capacity++;
            edgePool.Add(GetNewEdge());
        }
        edge = edgePool[edgePool.Count - 1];
        edgePool.RemoveAt(edgePool.Count - 1);
        edge.SetActive(true);
        return edge;
    }

    static GameObject GetNewEdge()
    {
        GameObject edge = GameObject.Instantiate(prefabEdge);
        edge.GetComponent<GraphEdge>().Initialize();
        edge.SetActive(false);
        GameObject.DontDestroyOnLoad(edge);
        return edge;
    }

    #endregion


    #region node
    /*
    public static void ReturnAllNodes()
    {
        foreach (GameObject node in GameObject.FindGameObjectsWithTag("Node"))
        {
            GraphNode graphNode = node.GetComponent<GraphNode>();
            DiGraph graph = graphNode.Graph;
            graph.RemoveNode(graphNode);
        }
    }
    */

    public static void ReturnNode(GameObject node)
    {
        node.GetComponent<GraphNode>().StopMoving();
        node.GetComponent<GraphNode>().RemoveAllChildren();
        node.GetComponent<GraphNode>().RemoveAllParents();
        Debug.Log("Now resetting mapping");
        node.GetComponent<GraphNode>().SetMapping(null);
        node.SetActive(false);
        nodePool.Add(node);
    }

/*
    public static GameObject GetNode()
    {
        GameObject node;
        if (nodePool.Count > 0)
        {
            node = nodePool[nodePool.Count - 1];
            nodePool.RemoveAt(nodePool.Count - 1);
        }
        else
        {
            Debug.Log("Expanding node pool...");
            nodePool.Capacity++;
            node = GetNewNode();
        }
        node.SetActive(true);
        return node;
    }
*/


    public static GameObject GetNode()
    {
        GameObject node;
        if (nodePool.Count == 0)
        {
            Debug.Log("Expanding node pool...");
            nodePool.Capacity++;
            nodePool.Add(GetNewNode());
        }
        node = nodePool[nodePool.Count - 1];
        nodePool.RemoveAt(nodePool.Count - 1);
        node.SetActive(true);
        return node;
    }

    static GameObject GetNewNode()
    {
        GameObject node = GameObject.Instantiate(prefabNode);
        node.GetComponent<GraphNode>().Initialize(nodePool.Capacity);
        node.SetActive(false);
        GameObject.DontDestroyOnLoad(node);
        return node;
    }

    #endregion


}
