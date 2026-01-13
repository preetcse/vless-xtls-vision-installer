using System;
using System.Collections.Generic;

namespace RoleplayOverhaul.Items
{
    public static class ItemLibrary
    {
        public static List<Item> CreateAllItems()
        {
            var items = new List<Item>();

            // Food & Drink (Survival)
            items.Add(new FoodItem("burger", "Burger", "Bleeder Burger", "food_burger", 0.5f, 20, 40, 5));
            items.Add(new FoodItem("taco", "Taco", "Spicy", "food_taco", 0.3f, 15, 25, 5));
            items.Add(new FoodItem("sandwich", "Sandwich", "Ham & Cheese", "food_sandwich", 0.3f, 15, 30, 0));
            items.Add(new FoodItem("hotdog", "Hot Dog", "Street meat", "food_hotdog", 0.4f, 10, 20, 0));
            items.Add(new FoodItem("donut", "Donut", "Glazed", "food_donut", 0.1f, 5, 10, -5));
            items.Add(new FoodItem("water", "Water Bottle", "Spring Water", "drink_water", 0.5f, 5, 0, 40));
            items.Add(new FoodItem("cola", "eCola", "Sugary", "drink_cola", 0.3f, 5, 5, 20));
            items.Add(new FoodItem("sprunk", "Sprunk", "Refresh yourself", "drink_sprunk", 0.3f, 5, 5, 20));
            items.Add(new FoodItem("beer", "Pisswasser", "Cheap beer", "drink_beer", 0.5f, -5, 5, 15)); // Alcohol
            items.Add(new FoodItem("wine", "Red Wine", "Fancy", "drink_wine", 0.7f, 5, 5, 15));

            // Tools
            items.Add(new ToolItem("wrench", "Wrench", "Fix vehicles", "tool_wrench", 1.0f));
            items.Add(new ToolItem("hammer", "Hammer", "Construction", "tool_hammer", 1.5f));
            items.Add(new ToolItem("drill", "Thermal Drill", "Open vaults", "tool_drill", 5.0f));
            items.Add(new ToolItem("lockpick", "Lockpick", "Open doors", "tool_lockpick", 0.1f));
            items.Add(new ToolItem("jerrycan", "Jerry Can", "Fuel", "tool_jerrycan", 2.0f));
            items.Add(new ToolItem("repairkit", "Repair Kit", "Fix engine", "tool_repairkit", 3.0f));
            items.Add(new ToolItem("bandage", "Bandage", "Stop bleeding", "med_bandage", 0.2f));
            items.Add(new ToolItem("medkit", "Medkit", "Restore health", "med_medkit", 2.0f));

            // Illegal
            items.Add(new Item("weed", "Weed", "1g Baggy", "drug_weed", 0.01f, 100, true));
            items.Add(new Item("coke", "Cocaine", "1g Baggy", "drug_coke", 0.01f, 100, true));
            items.Add(new Item("meth", "Meth", "Blue sky", "drug_meth", 0.01f, 100, true));
            items.Add(new Item("c4", "C4 Explosive", "Go boom", "wep_c4", 1.0f, 5, true));

            // Random
            items.Add(new Item("phone", "Smartphone", "Communication", "misc_phone", 0.2f, 1, false));
            items.Add(new Item("keys", "Car Keys", "Vehicle access", "misc_keys", 0.1f, 1, false));
            items.Add(new Item("wallet", "Wallet", "Money holder", "misc_wallet", 0.2f, 1, false));
            items.Add(new Item("watch", "Gold Watch", "Expensive", "misc_watch", 0.3f, 1, false));

            return items;
        }
    }
}
