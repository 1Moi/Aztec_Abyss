using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Passive
{
    public int Id { get; private set; }
    public string PassiveName { get; private set; }
    public List<int> Prerequisites { get; private set; }
    public int ExclusiveGroup { get; private set; }

    public Passive(int id, string passiveName, List<int> prerequisites, int exclusiveGroup)
    {
        Id = id;
        PassiveName = passiveName;
        Prerequisites = prerequisites;
        ExclusiveGroup = exclusiveGroup;
    }
}