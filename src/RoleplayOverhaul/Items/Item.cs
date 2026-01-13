using System;

namespace RoleplayOverhaul.Items
{
    public abstract class Item
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string IconTexture { get; private set; } // Texture dictionary or file name
        public float Weight { get; private set; }
        public int MaxStack { get; private set; }
        public bool IsUsable { get; private set; }

        public Item(string id, string name, string description, string icon, float weight, int maxStack, bool usable)
        {
            Id = id;
            Name = name;
            Description = description;
            IconTexture = icon;
            Weight = weight;
            MaxStack = maxStack;
            IsUsable = usable;
        }

        public virtual void OnUse()
        {
            // Base behavior
            GTA.UI.Screen.ShowSubtitle($"Used {Name}");
        }
    }
}
