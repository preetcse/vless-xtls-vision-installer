using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;

namespace RoleplayOverhaul.Police
{
    public class DispatchManager
    {
        private CrimeManager _crimeManager;
        private List<Ped> _activeCops;
        private int _lastSpawnTime;

        public DispatchManager(CrimeManager crimeManager)
        {
            _crimeManager = crimeManager;
            _activeCops = new List<Ped>();
        }

        public void OnTick()
        {
            if (_crimeManager.WantedStars > 0)
            {
                // Disable vanilla spawning to take control?
                // Function.Call(Hash.SET_CREATE_RANDOM_COPS, false);
                // Note: SHVDN allows us to manage this, but for now we SUPPLEMENT vanilla.

                ManageReinforcements();
            }
        }

        private void ManageReinforcements()
        {
            // Simple Logic: If player is wanted and not enough cops, spawn more
            if (GTA.Game.GameTime - _lastSpawnTime > 10000) // Every 10 seconds
            {
                if (_activeCops.Count < _crimeManager.WantedStars * 2) // 2 cops per star
                {
                    SpawnPoliceUnit();
                    _lastSpawnTime = GTA.Game.GameTime;
                }
            }

            // Cleanup dead cops
            _activeCops.RemoveAll(c => c.IsDead || !c.Exists());
        }

        private void SpawnPoliceUnit()
        {
            // Find spawn point near player but not seen
            Vector3 spawnPos = GTA.Game.Player.Character.Position + new Vector3(50, 0, 0); // Simplified

            // In a real mod, we'd use World.GetNextPositionOnStreet

            string vehicleName = "police";
            if (_crimeManager.WantedStars > 3) vehicleName = "fbi";
            if (_crimeManager.WantedStars > 4) vehicleName = "riot";

            // Mock Spawning (Cannot execute in sandbox)
            // var vehicle = World.CreateVehicle(vehicleName, spawnPos);
            // var cop = vehicle.CreatePedOnSeat(VehicleSeat.Driver, "s_m_y_cop_01");
            // cop.Task.FightAgainst(GTA.Game.Player.Character);
            // _activeCops.Add(cop);

            GTA.UI.Screen.ShowSubtitle("Dispatching reinforcements...");
        }

        public void AttemptArrest()
        {
            // Logic for "Busted" mechanic from LSR
            if (_crimeManager.WantedStars > 0)
            {
                 GTA.Game.Player.WantedLevel = 0;
                 _crimeManager.ReportCrime("Cleared", -100);
                 GTA.UI.Screen.ShowSubtitle("You surrendered. Fined $500.");
                 GTA.Game.Player.Money -= 500;
                 // Teleport to jail would go here
            }
        }
    }
}
