using System;

namespace RoleplayOverhaul.Items
{
    public class FoodItem : Item
    {
        public int HealthRestore { get; private set; }
        public float HungerRestore { get; private set; }
        public float ThirstRestore { get; private set; }

        public FoodItem(string id, string name, string description, string icon, float weight, int healthRestore, float hunger, float thirst)
            : base(id, name, description, icon, weight, 10, true)
        {
            HealthRestore = healthRestore;
            HungerRestore = hunger;
            ThirstRestore = thirst;
        }

        // Note: OnUse logic for SurvivalManager needs to be handled via event or main loop since Item doesn't know about Manager.
        // For now, we keep the health logic here and let Main handle the rest via casting.
        public override void OnUse()
        {
             if (GTA.Game.Player.Character != null)
            {
                int current = GTA.Game.Player.Character.Health;
                int max = GTA.Game.Player.Character.MaxHealth;
                GTA.Game.Player.Character.Health = Math.Min(max, current + HealthRestore);
            }
        }
    }

    public class LicenseItem : Item
    {
        public DateTime ExpiryDate { get; set; }
        public string LicenseType { get; set; }

        public LicenseItem(string id, string name, string type, DateTime expiry)
            : base(id, name, $"Valid until {expiry.ToShortDateString()}", "license_icon", 0.0f, 1, false)
        {
            LicenseType = type;
            ExpiryDate = expiry;
        }
    }
}
