using System;

namespace RoleplayOverhaul.Items
{
    public class FoodItem : Item
    {
        public int HealthRestore { get; private set; }

        public FoodItem(string id, string name, string description, string icon, float weight, int healthRestore)
            : base(id, name, description, icon, weight, 10, true)
        {
            HealthRestore = healthRestore;
        }

        public override void OnUse()
        {
            if (GTA.Game.Player.Character != null)
            {
                int current = GTA.Game.Player.Character.Health;
                int max = GTA.Game.Player.Character.MaxHealth;
                GTA.Game.Player.Character.Health = Math.Min(max, current + HealthRestore);
                GTA.UI.Screen.ShowSubtitle($"Ate {Name}. Health: {GTA.Game.Player.Character.Health}");
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
