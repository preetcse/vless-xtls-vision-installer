using System;
using GTA;
using GTA.Math;
using System.Collections.Generic;

namespace RoleplayOverhaul.Jobs
{
    public class GarbageJob : JobBase
    {
        private List<Vector3> _route;
        private int _currentStop;
        private Blip _routeBlip;

        public GarbageJob() : base("Trash Collector")
        {
            _route = new List<Vector3>
            {
                new Vector3(100, 100, 0),
                new Vector3(200, 200, 0),
                new Vector3(300, 100, 0)
            };
        }

        public override void Start()
        {
            base.Start();
            _currentStop = 0;
            SetNextStop();
        }

        private void SetNextStop()
        {
            if (_currentStop >= _route.Count)
            {
                GTA.UI.Screen.ShowSubtitle("Route Complete! Return to Depot. +$500");
                GTA.Game.Player.Money += 500;
                End();
                return;
            }

            if (_routeBlip != null) _routeBlip.Delete();
            _routeBlip = World.CreateBlip(_route[_currentStop]);
            _routeBlip.ShowRoute = true;

            GTA.UI.Screen.ShowSubtitle($"Go to Stop {_currentStop + 1}");
        }

        public override void OnTick()
        {
            if (!IsActive) return;

            if (GTA.Game.Player.Character.Position.DistanceTo(_route[_currentStop]) < 5.0f)
            {
                GTA.UI.Screen.ShowHelpText("Press E to Collect Trash");
                if (GTA.Game.IsControlJustPressed(GTA.Control.Context))
                {
                    GTA.Game.Player.Character.Task.PlayAnimation("missfbi4prepp1", "_bag_pickup_garbage_man");
                    // Wait(2000);
                    _currentStop++;
                    SetNextStop();
                }
            }
        }

        public override void End()
        {
            base.End();
            if (_routeBlip != null) _routeBlip.Delete();
        }
    }
}
