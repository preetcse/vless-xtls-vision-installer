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

        public void StartMission()
        {
            _isActive = true;
            _destination = new Vector3(100, 100, 10); // Placeholder coord
            _destBlip = World.CreateBlip(_destination);
            _destBlip.ShowRoute = true;

            _drugVan = World.CreateVehicle(VehicleHash.Burrito3, Game.Player.Character.Position + Game.Player.Character.ForwardVector * 5);
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

            if (_drugVan.IsDead)
            {
                FailMission("Van destroyed.");
                return;
            }

            // Fixed World.GetDistance usage to Vector3.Distance
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
