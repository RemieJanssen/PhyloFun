using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

/// <summary>
/// A rearrangement problem list as stored in the django backend
/// </summary>
[System.Serializable]
public class ProblemListBackendStyle
{
    public int count;
    public string next;
    public string previous;
    public List<ProblemBackendStyle> results;
}

