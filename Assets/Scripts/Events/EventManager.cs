using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    // support edge mapping event
    static List<GraphNode> mappingInvokers = new List<GraphNode>();
    static List<UnityAction<GraphNode>> mappingListeners = new List<UnityAction<GraphNode>>();
    // support rearrangement starting event
    static List<GraphNode> rearrangementInvokers = new List<GraphNode>();
    static List<UnityAction<GraphNode>> rearrangementListeners = new List<UnityAction<GraphNode>>();

    // new edge mapping event
    public static void AddMappingInvoker(GraphNode invoker)
    {
        mappingInvokers.Add(invoker);
        foreach (UnityAction<GraphNode> listener in mappingListeners)
        {
            invoker.AddMappingListener(listener);
        }
    }

    public static void AddMappingListener(UnityAction<GraphNode> listener)
    {
        mappingListeners.Add(listener);
        foreach (GraphNode invoker in mappingInvokers)
        {
            invoker.AddMappingListener(listener);
        }
    }

    // rearrangement event
    public static void AddRearrangementInvoker(GraphNode invoker)
    {
        rearrangementInvokers.Add(invoker);
        foreach (UnityAction<GraphNode> listener in rearrangementListeners)
        {
            invoker.AddRearrangementListener(listener);
        }
    }

    public static void AddRearrangementListener(UnityAction<GraphNode> listener)
    {
        rearrangementListeners.Add(listener);
        foreach (GraphNode invoker in rearrangementInvokers)
        {
            invoker.AddRearrangementListener(listener);
        }
    }

}
