using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;



/// <summary>
/// A network as stored in the django backend
/// </summary>
[System.Serializable]
public class NetworkBackendStyle
{
    public int id;
    public string url;
    public List<int> nodes;
    public List<List<int>> edges;
    public List<List<int>> labels;
    public List<List<double>> node_positions;
}

