using System;
using GTA;
using GTA.Math;

namespace RoleplayOverhaul.Jobs
{
    public class FirefighterJob : JobBase
    {
        private Vector3 _fireLocation;
        private int _fireIntensity;
        private Blip _fireBlip;

        public FirefighterJob() : base("Firefighter") { }

        public override void Start()
        {
            base.Start();
            SpawnFire();
        }

        private void SpawnFire()
        {
            _fireLocation = GTA.Game.Player.Character.Position + new Vector3(50, 50, 0); // Mock offset
            _fireIntensity = 100;

            _fireBlip = World.CreateBlip(_fireLocation);
            _fireBlip.Color = 1; // Red

            // StartFire(_fireLocation); // Native
            GTA.UI.Screen.ShowSubtitle("Dispatch: Fire reported! Respond immediately.");
        }

        public override void OnTick()
        {
            if (!IsActive) return;

            if (GTA.Game.Player.Character.Position.DistanceTo(_fireLocation) < 15.0f)
            {
                GTA.UI.Screen.ShowHelpText("Use Fire Extinguisher or Hose");

                // Check if player is spraying
                if (GTA.Game.IsControlPressed(GTA.Control.Attack))
                {
                    _fireIntensity--;
                    if (_fireIntensity <= 0)
                    {
                        GTA.UI.Screen.ShowSubtitle("Fire Extinguished! +$300");
                        GTA.Game.Player.Money += 300;
                        End(); // Or spawn next fire
                        Start(); // Loop
                    }
                    else
                    {
                         if (GTA.Game.GameTime % 1000 == 0) GTA.UI.Screen.ShowSubtitle($"Fire Integrity: {_fireIntensity}%");
                    }
                }
            }
        }

        public override void End()
        {
            base.End();
            if (_fireBlip != null) _fireBlip.Delete();
        }
    }
}
