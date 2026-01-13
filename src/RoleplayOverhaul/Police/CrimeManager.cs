using System;
using GTA;
using GTA.Math;
using RoleplayOverhaul.UI;

namespace RoleplayOverhaul.Police
{
    public class CrimeManager
    {
        public int HeatLevel { get; private set; } // 0-100 scale
        public int WantedStars { get; private set; } // 0-5 vanilla stars
        private bool _justCommittedCrime;

        public CrimeManager()
        {
            HeatLevel = 0;
            WantedStars = 0;
        }

        public void ReportCrime(string crimeName, int severity)
        {
            HeatLevel += severity;
            _justCommittedCrime = true;

            // Map Heat to Stars for UI/Logic
            if (HeatLevel > 20) WantedStars = 1;
            if (HeatLevel > 40) WantedStars = 2;
            if (HeatLevel > 60) WantedStars = 3;
            if (HeatLevel > 80) WantedStars = 4;
            if (HeatLevel >= 100) WantedStars = 5;

            // Cap heat
            if (HeatLevel > 100) HeatLevel = 100;

            GTA.UI.Screen.ShowSubtitle($"Crime Reported: {crimeName}. Heat: {HeatLevel}%");

            // Apply vanilla wanted level to ensure cops are hostile (hybrid approach)
            GTA.Game.Player.WantedLevel = WantedStars;
        }

        public void Update()
        {
            // Decay heat if hidden
            if (!_justCommittedCrime && WantedStars > 0 && GTA.Game.Player.WantedLevel == 0)
            {
                // Vanilla cooldown ended, decay our custom heat
                if (GTA.Game.GameTime % 1000 == 0) // Every second
                {
                    HeatLevel = Math.Max(0, HeatLevel - 1);
                    if (HeatLevel == 0) WantedStars = 0;
                }
            }

            // Check for simple crimes
            if (GTA.Game.Player.Character.IsShooting)
            {
               // ReportCrime("Discharge Firearm", 5);
            }

            CheckTrafficViolations();

            _justCommittedCrime = false;
        }

        private void CheckTrafficViolations()
        {
            if (GTA.Game.Player.Character.IsInVehicle())
            {
                var vehicle = GTA.Game.Player.Character.CurrentVehicle;
                float speed = vehicle.Speed; // m/s

                // Speed Limit Logic (Simplified: 30 m/s ~ 100 km/h as generic limit)
                // In a real mod, we use vehicle.GetSpeedLimit() or get road data
                if (speed > 40.0f) // ~140 km/h
                {
                    if (GTA.Game.GameTime % 5000 == 0) // Don't spam
                    {
                        ReportCrime("Felony Speeding", 5);
                    }
                }
                else if (speed > 25.0f && WantedStars == 0)
                {
                    // Minor speeding
                     if (GTA.Game.GameTime % 10000 == 0)
                    {
                        // Chance to be spotted
                        ReportCrime("Speeding Violation", 1);
                        GTA.UI.Screen.ShowSubtitle("Police: Pull over immediately!", 2000);
                    }
                }
            }
        }
    }
}
