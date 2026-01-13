using System;
using GTA;
using GTA.Math;
using RoleplayOverhaul.Core;

namespace RoleplayOverhaul.Jobs
{
    // Simplified to match what JobLibrary is likely calling,
    // or we update JobLibrary. Let's make this flexible or update JobLibrary.
    // Assuming JobLibrary is calling new SimpleJob(id, name, desc, type, tier, salary)

    public class SimpleJob : JobBase
    {
        public SimpleJob(string id, string name, string description, JobType type, int tier, int salary)
            : base(id, name, description, type, tier, salary)
        {
        }

        public override void OnTick() { }
        public override void StartShift() { GTA.UI.Notification.Show($"Started shift: {Name}"); }
        public override void EndShift() { GTA.UI.Notification.Show($"Ended shift: {Name}"); }
    }

    public class IllegalJob : JobBase
    {
        private Police.CrimeManager _crimeManager;

        // Constructor matching usage in JobLibrary if it passes CrimeManager
        public IllegalJob(string id, string name, string description, JobType type, int tier, int salary, Police.CrimeManager crimeManager)
            : base(id, name, description, type, tier, salary)
        {
            _crimeManager = crimeManager;
        }

        // Constructor overload if JobLibrary doesn't pass CrimeManager
         public IllegalJob(string id, string name, string description, JobType type, int tier, int salary)
            : base(id, name, description, type, tier, salary)
        {
        }

        public override void OnTick() { }
        public override void StartShift() { GTA.UI.Notification.Show($"Started illegal job: {Name}"); }
        public override void EndShift() { GTA.UI.Notification.Show($"Ended illegal job: {Name}"); }
    }
}
