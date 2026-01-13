using System;
using GTA;
using GTA.Math;
using GTA.UI;

namespace RoleplayOverhaul.Jobs
{
    public class SimpleJob : JobBase
    {
        private string _description;
        private string _vehicleModel;
        private Vector3? _fixedSpawn;
        private Vehicle _jobVehicle;
        private bool _hasStarted;

        public SimpleJob(string name, string description, string vehicleModel, Vector3? fixedSpawn = null) : base(name)
        {
            _description = description;
            _vehicleModel = vehicleModel;
            _fixedSpawn = fixedSpawn;
        }

        public override void Start()
        {
            base.Start();
            SpawnJobVehicle();
            GTA.UI.Screen.ShowSubtitle($"Job Started: {Name}. {_description}");
        }

        private void SpawnJobVehicle()
        {
            if (_vehicleModel == "none") return;

            Vector3 spawnPos = _fixedSpawn ?? GTA.Game.Player.Character.Position + GTA.Game.Player.Character.ForwardVector * 5.0f;

            // Try to snap to street if generic spawn
            if (!_fixedSpawn.HasValue && !IsWaterVehicle(_vehicleModel))
            {
                spawnPos = World.GetNextPositionOnStreet(spawnPos, true);
            }

            try
            {
                _jobVehicle = World.CreateVehicle(_vehicleModel, spawnPos);
                if (_jobVehicle != null)
                {
                    _jobVehicle.PlaceOnGround();
                    if (_fixedSpawn.HasValue) _jobVehicle.Rotation = new Vector3(0, 0, 0); // Reset rotation if fixed

                    // Add blip
                    var blip = _jobVehicle.AddBlip();
                    blip.Sprite = BlipSprite.PersonalVehicleCar;
                    blip.Color = BlipColor.Yellow;
                    blip.Name = "Job Vehicle";
                }
            }
            catch (Exception ex)
            {
                GTA.UI.Screen.ShowSubtitle("Failed to spawn job vehicle.");
                Diagnostics.Logger.Error($"SimpleJob Spawn Error: {ex.Message}");
            }
        }

        private bool IsWaterVehicle(string model)
        {
            return model == "predator" || model == "tug" || model == "seashark" || model == "dinghy";
        }

        public override void OnTick()
        {
            if (!IsActive) return;

            // Basic loop: If inside vehicle, get paid periodically?
            // Or just RP presence.
            // For now, simple "Pay every minute" logic to make it functional.

            if (GTA.Game.GameTime % 60000 == 0) // Every minute roughly
            {
                if (_jobVehicle != null && GTA.Game.Player.Character.IsInVehicle(_jobVehicle))
                {
                    int pay = 50;
                    GTA.Game.Player.Money += pay;
                    AwardXP(RoleplayOverhaul.Core.Progression.ExperienceManager.Skill.Global, 20);
                    GTA.UI.Notification.Show($"Paid ${pay} + 20XP for {Name} work.");
                }
            }
        }

        public override void End()
        {
            base.End();
            if (_jobVehicle != null && _jobVehicle.Exists()) _jobVehicle.Delete();
        }
    }
}
