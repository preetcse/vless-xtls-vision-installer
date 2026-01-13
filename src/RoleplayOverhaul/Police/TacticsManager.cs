using System;
using GTA;
using GTA.Math;
using GTA.Native;
using RoleplayOverhaul.Dependencies; // For stubs/enums

namespace RoleplayOverhaul.Police
{
    public class TacticsManager
    {
        private int _lastTacticTime;

        public void Update(int heatLevel, int wantedStars)
        {
            if (wantedStars < 3) return; // Tactics start at 3 stars

            if (GTA.Game.GameTime - _lastTacticTime > 15000) // Every 15 seconds try a tactic
            {
                if (wantedStars >= 3) AttemptAirSupport();
                if (wantedStars >= 4) AttemptRoadblock();
                if (wantedStars >= 5) AttemptSpikeStrip();

                _lastTacticTime = GTA.Game.GameTime;
            }
        }

        private void AttemptAirSupport()
        {
            // Only if outdoors
            Vector3 pos = GTA.Game.Player.Character.Position;
            Vector3 heliPos = pos + new Vector3(0, 0, 50);

            // Mock Heli Spawn
            // Vehicle heli = World.CreateVehicle("polmav", heliPos);
            // Ped pilot = heli.CreatePedOnSeat(VehicleSeat.Driver, "s_m_y_pilot_01");
            // pilot.Task.ChaseWithHelicopter(Game.Player.Character, Vector3.Zero);

            GTA.UI.Screen.ShowSubtitle("Dispatch: Air Unit requested.");
        }

        private void AttemptRoadblock()
        {
            // Logic to find road node in front of player
            Vector3 playerPos = GTA.Game.Player.Character.Position;
            Vector3 forward = GTA.Game.Player.Character.ForwardVector;
            Vector3 blockPos = playerPos + (forward * 100); // 100m ahead

            // Spawn 2 cars perpendicular
            // Vehicle v1 = World.CreateVehicle("police", blockPos);
            // v1.Rotation = new Vector3(0, 0, 90);

            GTA.UI.Screen.ShowSubtitle("Dispatch: Setting up roadblock ahead.");
        }

        private void AttemptSpikeStrip()
        {
            // Deploy spikes logic
            // Prop spike = World.CreateProp("p_ld_stinger_s", playerPos + forward * 50, ...);
            GTA.UI.Screen.ShowSubtitle("Dispatch: Spike strips authorized!");
        }
    }
}
