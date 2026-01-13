using System;

namespace RoleplayOverhaul.Jobs
{
    public class IllegalJob : SimpleJob
    {
        private Police.CrimeManager _crimeManager;

        public IllegalJob(string name, string description, string vehicle, Police.CrimeManager crimeManager)
            : base(name, description, vehicle)
        {
            _crimeManager = crimeManager;
        }

        public override void OnTick()
        {
            base.OnTick();

            // Risk of getting caught
            if (IsActive && GTA.Game.GameTime % 20000 == 0) // Every 20 seconds
            {
                // Random chance to gain heat
                Random m = new Random();
                if (m.Next(0, 10) > 7) // 30% chance
                {
                    _crimeManager.ReportCrime("Suspicious Activity", 10);
                }
            }
        }
    }
}
