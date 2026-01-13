using System;
using System.Collections.Generic;
using GTA;

namespace RoleplayOverhaul.Jobs
{
    public class JobManager
    {
        private List<IJob> _jobs;
        public IJob CurrentJob { get; private set; }

        public JobManager()
        {
            _jobs = new List<IJob>();
        }

        public void RegisterJob(IJob job)
        {
            _jobs.Add(job);
        }

        public void StartJob(string jobName)
        {
            if (CurrentJob != null)
            {
                GTA.UI.Screen.ShowSubtitle("You already have an active job!");
                return;
            }

            var job = _jobs.Find(j => j.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));
            if (job != null)
            {
                CurrentJob = job;
                CurrentJob.Start();
            }
        }

        public void EndCurrentJob()
        {
            if (CurrentJob != null)
            {
                CurrentJob.End();
                CurrentJob = null;
            }
        }

        public void OnTick()
        {
            if (CurrentJob != null)
            {
                CurrentJob.OnTick();
            }
        }
    }
}
