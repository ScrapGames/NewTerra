using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;

public class HarvestJobManager
{
    private List<HarvesterBase> harvesters;
    private List<HarvestJobData> jobs;
    private const float TICK_RATE = 1f;
    private float nextTick;

    public static int GetCountPerSec(int countPerTick) => (int)Mathf.Ceil(1/TICK_RATE * countPerTick);

    public HarvestJobManager()
    {
        harvesters = new List<HarvesterBase>();
        jobs = new List<HarvestJobData>();
        SetNextTick();
    }

    public void RegisterHarvester(HarvesterBase harvester)
    {
        int id;
        harvesters.Add(harvester);
        id = harvesters.Count - 1;
        HarvesterData data = harvester.data as HarvesterData;

        jobs.Add(new HarvestJobData(
            id,
            data.primaryExtraction.outputPerTick,
            data.secondaryExtraction.outputPerTick,
            data.baseStackStorage,
            data.turnsPerStack
            ));
    }

    private void SetNextTick() => nextTick = Time.fixedTime + TICK_RATE;
    
    
    public void UpdateJobs()
    {
        if (Time.fixedTime < nextTick) return;

        for (int i = 0; i < jobs.Count; i++)
        {
            HarvestJobData job = jobs[i];

            switch(job.jobStatus)
            {
                default:
                case HarvestJobData.JobStatus.AtCapacity:
                    // Idle
                    break;
                case HarvestJobData.JobStatus.Harvesting:
                    // Increase stack
                    job.Harvest();
                    break;
                case HarvestJobData.JobStatus.Unloading:
                    // Unloading resources 
                    break;
            }   
            
            // Update harvester
            harvesters[job.harvesterId].OnHarvestJobUpdate(job);

            jobs[i] = job;
        }

        SetNextTick();
    }

    
}
