using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HarvestStack
{
    public bool IsStackFull { get { return harvestTurns == harvestTurnsMax; } }

    public int primaryResourceCount;
    public int secondaryResourceCount;
    private int harvestTurns, harvestTurnsMax;


    public HarvestStack(int maxTurns)
    {
        primaryResourceCount = secondaryResourceCount = harvestTurns = 0;
        harvestTurnsMax = maxTurns;
    }

    public void AddResources(int primary, int secondary)
    {
        primaryResourceCount += primary;
        secondaryResourceCount += secondary;
        harvestTurns++;
    }
}
