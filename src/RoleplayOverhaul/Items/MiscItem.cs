namespace RoleplayOverhaul.Items
{
    public class MiscItem : Item
    {
        public MiscItem(string id, string name, string description, string iconModel, float weight)
            : base(id, name, description, iconModel, weight)
        {
        }

        public override void Use(RoleplayOverhaul.Core.Inventory inventory)
        {
            GTA.UI.Notification.Show($"Used {Name}. Nothing happened.");
        }
    }
}
