using System;
using GTA;
using GTA.Math;
using GTA.UI;
using RoleplayOverhaul.Police;

namespace RoleplayOverhaul.Jobs
{
    public class IllegalJob : JobBase
    {
        private string _description;
        private string _vehicleModel;
        private CrimeManager _crimeManager;
        private Vehicle _jobVehicle;
        private Blip _targetBlip;
        private Vector3 _targetPos;
        private Random _rnd;
        private bool _hasPackage;

        public IllegalJob(string name, string description, string vehicleModel, CrimeManager crimeManager) : base(name)
        {
            _description = description;
            _vehicleModel = vehicleModel;
            _crimeManager = crimeManager;
            _rnd = new Random();
        }

        public override void Start()
        {
            base.Start();
            SpawnVehicle();
            SetNextTask();
            GTA.UI.Screen.ShowSubtitle($"Crime Started: {Name}. {_description}");
        }

        private void SpawnVehicle()
        {
            if (_vehicleModel == "none") return;

            Vector3 spawnPos = World.GetNextPositionOnStreet(GTA.Game.Player.Character.Position + new Vector3(0, 10, 0), true);
            _jobVehicle = World.CreateVehicle(_vehicleModel, spawnPos);
            if (_jobVehicle != null) _jobVehicle.AddBlip();
        }

        private void SetNextTask()
        {
            // Pick a random spot (mocked safe spots)
            // In a real scenario, use a curated list of "Hideouts"
            _targetPos = GTA.Game.Player.Character.Position + new Vector3(_rnd.Next(-500, 500), _rnd.Next(-500, 500), 0);
            _targetPos = World.GetNextPositionOnStreet(_targetPos, true);

            if (_targetBlip != null) _targetBlip.Delete();
            _targetBlip = World.CreateBlip(_targetPos);
            _targetBlip.Color = BlipColor.Red;
            _targetBlip.Name = "Illegal Task";
            _targetBlip.ShowRoute = true;

            _hasPackage = false;
            GTA.UI.Screen.ShowSubtitle("Go to the marked location.");
        }

        public override void OnTick()
        {
            if (!IsActive) return;

            if (GTA.Game.Player.Character.Position.DistanceTo(_targetPos) < 5.0f)
            {
                GTA.UI.Screen.ShowHelpText("Press ~INPUT_CONTEXT~ to commit crime");

                if (GTA.Game.IsControlJustPressed(Control.Context))
                {
                    // Action
                    GTA.Game.Player.Money += 500;
                    _crimeManager.ReportCrime(new Crime(CrimeType.GrandTheftAuto, GTA.Game.Player.Character.Position)); // Generic crime
                    AwardXP(RoleplayOverhaul.Core.Progression.ExperienceManager.Skill.Strength, 50);

                    GTA.UI.Screen.ShowSubtitle("Success! Police are alerted. +50XP");
                    SetNextTask();
                }
            }
        }

        public override void End()
        {
            base.End();
            if (_targetBlip != null) _targetBlip.Delete();
            if (_jobVehicle != null) _jobVehicle.Delete();
        }
    }
}
