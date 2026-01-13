using System;
using GTA;
using GTA.Math;
using GTA.UI;
using System.Drawing;
using RoleplayOverhaul.Activities.Illegal;

namespace RoleplayOverhaul.Core
{
    public class InteractionSystem
    {
        private Entity _hoveredEntity;
        private KidnappingManager _kidnappingManager; // Dependency Injection

        public bool IsTargetingMode { get; set; } = false;

        public InteractionSystem(KidnappingManager kidnappingManager)
        {
            _kidnappingManager = kidnappingManager;
        }

        public void OnTick()
        {
            // Toggle Targeting Mode with ALT
            if (Game.IsControlPressed(Control.CharacterWheel)) // Simulating ALT key behavior
            {
                IsTargetingMode = true;
                PerformRaycast();
            }
            else
            {
                IsTargetingMode = false;
                _hoveredEntity = null;
            }
        }

        private void PerformRaycast()
        {
            Vector3 camPos = GameplayCamera.Position;
            Vector3 camDir = GameplayCamera.Direction;
            RaycastResult result = World.Raycast(camPos, camDir, 10.0f, IntersectFlags.Everything);

            if (result.DidHit && result.HitEntity != null && result.HitEntity != Game.Player.Character)
            {
                _hoveredEntity = result.HitEntity;
                DrawTargetUI(_hoveredEntity);

                if (Game.IsControlJustPressed(Control.Context))
                {
                    InteractWith(_hoveredEntity);
                }
            }
            else
            {
                _hoveredEntity = null;
                new TextElement("o", new PointF(640, 360), 0.5f, Color.White, Font.ChaletComprimeCologne, Alignment.Center).Draw();
            }
        }

        private void DrawTargetUI(Entity entity)
        {
            Vector2 screenPos = World.WorldToScreen(entity.Position);
            if (screenPos != Vector2.Zero)
            {
                string label = "Unknown";
                if (entity is Ped p) label = p.IsAlive ? "Person (E to Interact)" : "Body (E to Search)";
                if (entity is Vehicle v) label = $"Vehicle (E to Interact)";

                new TextElement(label, new PointF(screenPos.X, screenPos.Y), 0.4f, Color.White, Font.ChaletLondon, Alignment.Center).Draw();
            }
        }

        private void InteractWith(Entity entity)
        {
            if (entity is Ped ped)
            {
                if (ped.IsDead)
                {
                    Notification.Show("Searching Body...");
                }
                else
                {
                    // Interaction Context Logic
                    if (Game.IsControlPressed(Control.Aim))
                    {
                         // Call instance method
                        _kidnappingManager.AttemptKidnap(ped);
                    }
                    else
                    {
                        Extortion.AttemptExtortion(ped);
                    }
                }
            }
            else if (entity is Vehicle veh)
            {
                Notification.Show("Interacting with Vehicle...");
            }
        }
    }
}
