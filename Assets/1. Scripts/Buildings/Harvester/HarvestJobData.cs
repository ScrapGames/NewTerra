using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HarvestJobData
{    
    public enum JobStatus { Harvesting, AtCapacity, Unloading }
    public bool IsAtMaxCapacity { get { return stackCount == stackCountMax; } }
    public bool StackCreatedThisJob { get; private set; }
    public int PrimaryResourceCount 
    { 
        get 
        { 
            return stackCount * primaryHarvestTick * turnsPerStack; 
        } 
    }
    public int SecondaryResourceCount
    {
        get {
            return stackCount * secondaryHarvestTick * turnsPerStack;
        }
    }

    public JobStatus jobStatus;
    public int harvesterId;
    public int stackCount;
    public int stackCountMax;
    public HarvestStack currentHarvestStack;
    private int primaryHarvestTick, secondaryHarvestTick;  // Amount of resources harvested per tick
    private int turnsPerStack;
    

    public HarvestJobData(int id, int primaryHarvestTick, int secondaryHarvestTick, 
        int stackStorageCountMax, int turnsPerStack)
    {
        harvesterId = id;
        stackCount = 0;
        this.stackCountMax = stackStorageCountMax;
        this.primaryHarvestTick = primaryHarvestTick; 
        this.secondaryHarvestTick = secondaryHarvestTick;
        this.turnsPerStack = turnsPerStack;
        currentHarvestStack = new HarvestStack(turnsPerStack);
        jobStatus = JobStatus.Harvesting;
        StackCreatedThisJob = false;
    }
    

    public void Harvest()
    {        
        HarvestStack stack = currentHarvestStack;
        StackCreatedThisJob = false;

        // Increase stack resources
        stack.AddResources(primaryHarvestTick, secondaryHarvestTick);        

        // Check if stack is full
        if(stack.IsStackFull)
        {
            // Increase stack storage count
            stackCount++;

            // Set new stack
            stack = new HarvestStack(turnsPerStack);
            StackCreatedThisJob = true;
        }
        

        // Set stack into data
        currentHarvestStack = stack;
    }

}
