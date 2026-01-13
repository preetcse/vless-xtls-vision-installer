using System;
using System.Collections.Generic;
using System.Linq;

namespace RoleplayOverhaul.Items
{
    public class ItemStack
    {
        public Item Item { get; set; }
        public int Count { get; set; }

        public ItemStack(Item item, int count)
        {
            Item = item;
            Count = count;
        }
    }

    public class Inventory
    {
        public List<ItemStack> Slots { get; private set; }
        public int MaxSlots { get; private set; }
        public float MaxWeight { get; private set; }

        public Inventory(int maxSlots, float maxWeight)
        {
            Slots = new List<ItemStack>();
            MaxSlots = maxSlots;
            MaxWeight = maxWeight;
        }

        public float CurrentWeight
        {
            get { return Slots.Sum(s => s.Item.Weight * s.Count); }
        }

        public bool AddItem(Item item, int count)
        {
            if (CurrentWeight + (item.Weight * count) > MaxWeight)
            {
                GTA.UI.Screen.ShowSubtitle("Inventory too heavy!");
                return false;
            }

            // Try to stack existing
            var existing = Slots.FirstOrDefault(s => s.Item.Id == item.Id && s.Count < s.Item.MaxStack);
            if (existing != null)
            {
                int space = existing.Item.MaxStack - existing.Count;
                if (space >= count)
                {
                    existing.Count += count;
                    return true;
                }
                else
                {
                    existing.Count = existing.Item.MaxStack;
                    count -= space;
                    // Continue to add remainder
                }
            }

            if (Slots.Count >= MaxSlots)
            {
                GTA.UI.Screen.ShowSubtitle("Inventory full!");
                return false;
            }

            Slots.Add(new ItemStack(item, count));
            return true;
        }

        public bool RemoveItem(string itemId, int count)
        {
            var stack = Slots.FirstOrDefault(s => s.Item.Id == itemId);
            if (stack == null || stack.Count < count) return false;

            stack.Count -= count;
            if (stack.Count <= 0)
            {
                Slots.Remove(stack);
            }
            return true;
        }

        public bool HasItem(string itemId, int count = 1)
        {
            var stack = Slots.FirstOrDefault(s => s.Item.Id == itemId);
            return stack != null && stack.Count >= count;
        }
    }
}
