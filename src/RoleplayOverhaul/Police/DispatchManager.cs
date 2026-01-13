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
            Vector3 playerPos = GTA.Game.Player.Character.Position;
            Vector3 spawnPos = playerPos + new Vector3(100, 0, 0); // Further out

            // Determine Region (Simplified Y check)
            bool isCountry = playerPos.Y > 1000.0f; // North of city
            bool isHighways = false; // logic would check road node type

            string vehicleName = isCountry ? "sheriff" : "police";
            string pedName = isCountry ? "s_m_y_sheriff_01" : "s_m_y_cop_01";

            // Escalation Logic
            if (_crimeManager.WantedStars == 3)
            {
                 vehicleName = isCountry ? "sheriff2" : "police3"; // Interceptors
            }
            if (_crimeManager.WantedStars == 4)
            {
                 vehicleName = "fbi";
                 pedName = "s_m_y_swat_01";
            }
            if (_crimeManager.WantedStars >= 5)
            {
                vehicleName = "riot";
                pedName = "s_m_y_swat_01";
            }

            // Mock Spawning
            /*
            var vehicle = World.CreateVehicle(vehicleName, spawnPos);
            if (vehicle != null)
            {
                var cop = vehicle.CreatePedOnSeat(VehicleSeat.Driver, pedName);
                cop.Task.FightAgainst(GTA.Game.Player.Character);
                cop.Weapons.Give(GTA.WeaponHash.CarbineRifle, 999, true, true);

                // Add passenger for higher levels
                if (_crimeManager.WantedStars >= 3)
                {
                     vehicle.CreatePedOnSeat(VehicleSeat.Passenger, pedName).Task.FightAgainst(GTA.Game.Player.Character);
                }

                _activeCops.Add(cop);
            }
            */

            GTA.UI.Screen.ShowSubtitle($"Dispatching {vehicleName} unit...");
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
