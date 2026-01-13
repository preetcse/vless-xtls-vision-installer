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

            // Find a safe spawn on the street near the player
            Vector3 spawnOrigin = GTA.Game.Player.Character.Position + (GTA.Game.Player.Character.ForwardVector * 10f);
            Vector3 spawnPos = World.GetNextPositionOnStreet(spawnOrigin, true);

            try
            {
                _jobVehicle = World.CreateVehicle(VehicleModel, spawnPos);
                if (_jobVehicle != null)
                {
                    _jobVehicle.PlaceOnGround();
                    _jobVehicle.Rotation = GTA.Game.Player.Character.Rotation; // Align somewhat
                    var b = _jobVehicle.AddBlip();
                    b.Sprite = BlipSprite.PersonalVehicleCar;
                    b.Name = Name + " Vehicle";
                }
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
            // Dynamic Location Generation: Pick a point in a radius, snap to street.
            // This prevents "Ocean Spawns" by ensuring the API finds a road node.

            Vector3 playerPos = GTA.Game.Player.Character.Position;
            Vector3 randomOffset = new Vector3(_random.Next(-1500, 1500), _random.Next(-1500, 1500), 0);
            Vector3 roughPos = playerPos + randomOffset;

            // Critical Fix: Use SHVDN API to snap to nearest road.
            CurrentDestination = World.GetNextPositionOnStreet(roughPos, true);

            if (_destBlip != null) _destBlip.Delete();

            _destBlip = World.CreateBlip(CurrentDestination);
            _destBlip.Color = BlipColor.Yellow;
            _destBlip.ShowRoute = true;
            _destBlip.Name = "Delivery Point";

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

            if (dist < 10.0f) // Tighter radius for realism
            {
                GTA.UI.Screen.ShowHelpText("Press ~INPUT_CONTEXT~ to Deliver");

                if (GTA.Game.IsControlJustPressed(GTA.Control.Context))
                {
                    // Verify vehicle
                    if (_jobVehicle != null && !GTA.Game.Player.Character.IsInVehicle(_jobVehicle))
                    {
                        GTA.UI.Screen.ShowSubtitle("You need the work vehicle!");
                        return;
                    }

                    GTA.Game.Player.Money += 300;
                    AwardXP(RoleplayOverhaul.Core.Progression.ExperienceManager.Skill.Trucking, 100);
                    GTA.UI.Screen.ShowSubtitle("Delivery Complete! +$300 +100XP");
                    SetNextDestination();
                }
            }
        }
    }
}
