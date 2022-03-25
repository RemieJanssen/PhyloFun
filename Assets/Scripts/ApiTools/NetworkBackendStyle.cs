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
    public string nodes;
    public string edges;
    public string labels;
}

