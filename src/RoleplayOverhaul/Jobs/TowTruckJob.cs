using System;
using GTA;
using GTA.Math;

namespace RoleplayOverhaul.Jobs
{
    public class TowTruckJob : JobBase
    {
        private Vehicle _targetVehicle;
        private Blip _targetBlip;

        public TowTruckJob() : base("Tow Truck Driver") { }

        public override void Start()
        {
            base.Start();
            SpawnTowTarget();
        }

        private void SpawnTowTarget()
        {
            Vector3 pos = GTA.Game.Player.Character.Position + new Vector3(100, 0, 0);
            _targetVehicle = World.CreateVehicle("adder", pos); // Illegal parking
            _targetBlip = World.CreateBlip(pos);
            GTA.UI.Screen.ShowSubtitle("Dispatch: Illegally parked vehicle reported.");
        }

        public override void OnTick()
        {
            if (!IsActive) return;

            if (_targetVehicle != null && _targetVehicle.Exists())
            {
                if (GTA.Game.Player.Character.Position.DistanceTo(_targetVehicle.Position) < 5.0f)
                {
                    GTA.UI.Screen.ShowHelpText("Hook vehicle and drive to Pound.");
                    // Check if hooked (Native check usually)
                    // If hooked, set destination to Pound
                }
            }
        }

        public override void End()
        {
            base.End();
            if (_targetBlip != null) _targetBlip.Delete();
        }
    }
}
