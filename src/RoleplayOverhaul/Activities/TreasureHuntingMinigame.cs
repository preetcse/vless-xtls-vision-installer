using System;
using GTA;
using GTA.Math;
using GTA.UI;
using System.Drawing;

namespace RoleplayOverhaul.Activities
{
    public class TreasureHuntingMinigame
    {
        private bool isHunting = false;
        private Vector3 targetLocation;
        private Blip targetBlip;

        public void StartHunt(Vector3 location)
        {
            isHunting = true;
            targetLocation = location;
            targetBlip = World.CreateBlip(targetLocation);
            targetBlip.Color = BlipColor.Yellow;
            targetBlip.Name = "Treasure Search Area";
            // Create a large radius blip instead of exact point for realism
            targetBlip.ShowRoute = true;
        }

        public void OnTick()
        {
            if (!isHunting) return;

            // Fixed World.GetDistance usage to Vector3.Distance
            float dist = Vector3.Distance(Game.Player.Character.Position, targetLocation);

            // Beeper logic (closer = faster beep)
            if (dist < 100)
            {
                // Play sound or visual cue
                new TextElement($"Signal Strength: {(100 - dist):0.0}%", new PointF(500, 500), 0.5f).Draw();
            }

            if (dist < 2.0f && Game.IsControlJustPressed(Control.Context))
            {
                FinishHunt();
            }
        }

        private void FinishHunt()
        {
            isHunting = false;
            targetBlip.Delete();
            Game.Player.Character.Task.PlayAnimation("amb@medic@standing@kneel@base", "base", 8.0f, -1, AnimationFlags.None);
            GTA.UI.Notification.Show("Treasure Found!");
            // Give random loot
        }
    }
}
