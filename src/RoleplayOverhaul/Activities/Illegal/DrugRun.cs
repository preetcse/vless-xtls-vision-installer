using System;
using GTA;
using GTA.Math;
using GTA.UI;

namespace RoleplayOverhaul.Activities.Illegal
{
    public class DrugRun
    {
        private bool _isActive = false;
        private Vehicle _drugVan;
        private Vector3 _destination;
        private Blip _destBlip;
        private float _timeLimit;
        private Random _rnd;

        public DrugRun()
        {
            _rnd = new Random();
        }

        public void StartMission()
        {
            _isActive = true;

            // Dynamic destination on street
            Vector3 farAway = Game.Player.Character.Position + new Vector3(_rnd.Next(-2000, 2000), _rnd.Next(-2000, 2000), 0);
            _destination = World.GetNextPositionOnStreet(farAway, true);

            _destBlip = World.CreateBlip(_destination);
            _destBlip.ShowRoute = true;
            _destBlip.Color = BlipColor.Red;
            _destBlip.Name = "Drop Off";

            // Spawn Van
            Vector3 vanSpawn = World.GetNextPositionOnStreet(Game.Player.Character.Position + new Vector3(10, 10, 0), true);
            _drugVan = World.CreateVehicle(VehicleHash.Burrito3, vanSpawn);
            if (_drugVan != null)
            {
                _drugVan.AddBlip().Name = "Drug Van";
            }

            _timeLimit = 300f; // 5 mins

            GTA.UI.Notification.Show("Get in the van and drive to the drop-off! Avoid the cops.");
            Game.Player.WantedLevel = 1; // Instant heat
        }

        public void OnTick()
        {
            if (!_isActive) return;

            _timeLimit -= Game.LastFrameTime;
            if (_timeLimit <= 0)
            {
                FailMission("Time ran out.");
                return;
            }

            if (_drugVan == null || !_drugVan.Exists() || _drugVan.IsDead)
            {
                FailMission("Van destroyed.");
                return;
            }

            if (Game.Player.Character.IsInVehicle(_drugVan) && Vector3.Distance(_drugVan.Position, _destination) < 10f)
            {
                CompleteMission();
            }
        }

        private void FailMission(string reason)
        {
            _isActive = false;
            if (_destBlip != null) _destBlip.Delete();
            GTA.UI.Notification.Show($"Drug Run Failed: {reason}");
        }

        private void CompleteMission()
        {
            _isActive = false;
            if (_destBlip != null) _destBlip.Delete();
            GTA.UI.Notification.Show("Delivery Successful! Here is your cut.");
            Game.Player.Money += 5000;
        }
    }
}
