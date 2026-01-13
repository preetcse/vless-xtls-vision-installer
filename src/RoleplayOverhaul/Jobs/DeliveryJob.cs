using System;
using GTA;
using GTA.Math;
using GTA.UI;

namespace RoleplayOverhaul.Jobs
{
    public class DeliveryJob : JobBase
    {
        public string VehicleModel { get; private set; }
        public Vector3 CurrentDestination { get; private set; }
        private Vehicle _jobVehicle;
        private Blip _destBlip;

        // Simple list of delivery points (Mocking map locations)
        private static List<Vector3> _destinations = new List<Vector3>
        {
            new Vector3(100, 200, 20),
            new Vector3(-500, 300, 20),
            new Vector3(1200, -1500, 20),
            new Vector3(-200, -500, 20),
            new Vector3(800, 800, 20)
        };
        private Random _random;

        public DeliveryJob(string name, string vehicle) : base(name)
        {
            VehicleModel = vehicle;
            _random = new Random();
        }

        public override void Start()
        {
            base.Start();
            SpawnVehicle();
            SetNextDestination();
        }

        private void SpawnVehicle()
        {
            if (GTA.Game.Player.Character == null) return;

            // Spawn near player
            Vector3 spawnPos = GTA.Game.Player.Character.Position + new Vector3(5, 0, 0); // Simplified offset

             try
            {
                _jobVehicle = World.CreateVehicle(VehicleModel, spawnPos);
                // if (_jobVehicle != null) { _jobVehicle.AddBlip(); }
                GTA.UI.Screen.ShowSubtitle($"Job Started. Get in the {VehicleModel}.");
            }
            catch (Exception ex)
            {
                Diagnostics.Logger.Error("Failed to spawn job vehicle", ex);
                GTA.UI.Screen.ShowSubtitle("Error: Could not spawn vehicle!");
                End();
            }
        }

        private void SetNextDestination()
        {
            CurrentDestination = _destinations[_random.Next(_destinations.Count)];

            if (_destBlip != null) _destBlip.Delete();

            _destBlip = World.CreateBlip(CurrentDestination);
            _destBlip.Color = 66; // Yellow
            _destBlip.ShowRoute = true;

            GTA.UI.Screen.ShowSubtitle($"New Delivery Assigned. Drive to destination.");
        }

        public override void End()
        {
            base.End();
            if (_destBlip != null) _destBlip.Delete();
            if (_jobVehicle != null) _jobVehicle.Delete();
        }

        public override void OnTick()
        {
            if (!IsActive) return;

            float dist = GTA.Game.Player.Character.Position.DistanceTo(CurrentDestination);

            // 50.0f is a generous radius since we can't verify Z coords accurately in blind dev
            if (dist < 50.0f)
            {
                // Draw marker logic would go here
                GTA.UI.Screen.ShowHelpText("Press E to Deliver");

                if (GTA.Game.IsControlJustPressed(GTA.Control.Context))
                {
                    GTA.Game.Player.Money += 200;
                    GTA.UI.Screen.ShowSubtitle("Delivery Complete! +$200");
                    SetNextDestination();
                }
            }
        }
    }
}
