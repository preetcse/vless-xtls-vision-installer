using System;
using GTA;
using RoleplayOverhaul.Items;
using RoleplayOverhaul.UI;

namespace RoleplayOverhaul.Core
{
    public class SurvivalManager
    {
        public float Hunger { get; private set; } // 0 = Starving, 100 = Full
        public float Thirst { get; private set; }
        public float Sleep { get; private set; }

        private int _lastTick;

        public SurvivalManager()
        {
            Hunger = 100.0f;
            Thirst = 100.0f;
            Sleep = 100.0f;
            _lastTick = GTA.Game.GameTime;
        }

        public void OnTick()
        {
            int currentTime = GTA.Game.GameTime;
            if (currentTime - _lastTick > 1000) // Every second
            {
                // Decay rates
                Hunger = Math.Max(0, Hunger - 0.05f);
                Thirst = Math.Max(0, Thirst - 0.1f);
                Sleep = Math.Max(0, Sleep - 0.02f);

                _lastTick = currentTime;

                ApplyEffects();
            }
        }

        private void ApplyEffects()
        {
            if (GTA.Game.Player.Character == null) return;

            // Health damage if critical
            if (Hunger <= 0 || Thirst <= 0)
            {
                GTA.Game.Player.Character.Health -= 1;
            }

            // Stumble if tired
            if (Sleep < 20)
            {
                GTA.UI.Screen.ShowSubtitle("You are exhausted...", 1000);
                if (Sleep <= 0)
                {
                    // Blackout logic mock
                    GTA.UI.Screen.FadeOut(1000);
                    // GTA.Game.Player.Character.IsRagdoll = true;
                }
            }
        }

        public void Consume(FoodItem food)
        {
            Hunger = Math.Min(100, Hunger + food.HungerRestore);
            Thirst = Math.Min(100, Thirst + food.ThirstRestore);
            GTA.UI.Screen.ShowSubtitle($"Consumed {food.Name}. Hunger: {(int)Hunger}% Thirst: {(int)Thirst}%");
        }

        public void Rest(float hours)
        {
             Sleep = Math.Min(100, Sleep + (hours * 10));
             GTA.UI.Screen.ShowSubtitle("You feel rested.");
        }
    }
}
