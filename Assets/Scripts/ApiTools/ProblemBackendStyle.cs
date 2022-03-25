using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

/// <summary>
/// A rearrangement problem as stored in the django backend
/// </summary>
[System.Serializable]
public class ProblemBackendStyle
{
    public int id;
    public int move_type;
    public bool vertical_allowed;
    public NetworkBackendStyle network1;
    public NetworkBackendStyle network2;
}

