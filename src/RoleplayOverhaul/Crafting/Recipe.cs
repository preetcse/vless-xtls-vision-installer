using System.Collections.Generic;
using RoleplayOverhaul.Items;

namespace RoleplayOverhaul.Crafting
{
    public class Recipe
    {
        public string Name { get; set; }
        public Dictionary<string, int> RequiredItems { get; set; } // ItemID, Count
        public Item ResultItem { get; set; }
        public int ResultCount { get; set; }
        public float CraftingTime { get; set; } // In seconds
        public string RequiredPropModel { get; set; } // e.g. "prop_tool_bench02"

        public Recipe(string name, Item result, int count, float time)
        {
            Name = name;
            ResultItem = result;
            ResultCount = count;
            CraftingTime = time;
            RequiredItems = new Dictionary<string, int>();
            RequiredPropModel = null;
        }

        public void AddIngredient(string itemId, int count)
        {
            if (RequiredItems.ContainsKey(itemId))
                RequiredItems[itemId] += count;
            else
                RequiredItems[itemId] = count;
        }
    }
}
