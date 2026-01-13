using System;
using GTA; // For Ped
using RoleplayOverhaul.Core.Inventory; // New System

namespace RoleplayOverhaul.Items
{
    // Now inherits from GridInventory's InventoryItem
    public class LicenseItem : InventoryItem
    {
        public DateTime ExpiryDate { get; set; }
        public string LicenseType { get; set; }

        public LicenseItem(string id, string name, string type, DateTime expiry)
            : base(id, name, $"Valid until: {expiry.ToShortDateString()}", 0.0f, "license_icon")
        {
            LicenseType = type;
            ExpiryDate = expiry;
            MaxStack = 1;
        }

        public override void OnUse(Ped player)
        {
            GTA.UI.Screen.ShowSubtitle($"License: {Name} (Expires {ExpiryDate.ToShortDateString()})");
        }
    }
}
