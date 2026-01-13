using System;
using System.Collections.Generic;
using GTA;
using GTA.UI;
using GTA.Math;
using RoleplayOverhaul.Items;
using System.Linq;

namespace RoleplayOverhaul.Crafting
{
    public class CraftingManager
    {
        private Inventory _inventory;
        public List<Recipe> Recipes { get; private set; }
        private bool _isCrafting = false;
        private float _craftProgress = 0f;
        private Recipe _currentRecipe;

        public CraftingManager(Inventory inventory)
        {
            _inventory = inventory;
            Recipes = new List<Recipe>();
            LoadDefaultRecipes();
        }

        private void LoadDefaultRecipes()
        {
            var lockpickRecipe = new Recipe("Lockpick", new Items.MiscItem("tool_lockpick", "Lockpick", "For breaking locks", "prop_tool_pliers", 10), 1, 3.0f);
            lockpickRecipe.AddIngredient("metal_scrap", 2);
            lockpickRecipe.RequiredPropModel = "prop_tool_bench02";
            Recipes.Add(lockpickRecipe);

            var bandageRecipe = new Recipe("Bandage", new Items.FoodItem("med_bandage", "Bandage", "Heals small wounds", "prop_ld_health_pack", 0.1f, 0, 15f, 0f), 1, 2.0f);
            bandageRecipe.AddIngredient("cloth_scrap", 2);
            Recipes.Add(bandageRecipe);

            var metalRecipe = new Recipe("Refined Metal", new Items.ResourceItem("refined_metal", "Refined Metal", "Strong building material", "prop_ingot_01", 50), 1, 5.0f);
            metalRecipe.AddIngredient("iron_ore", 2);
            metalRecipe.RequiredPropModel = "prop_idol_case_02";
            Recipes.Add(metalRecipe);
        }

        public bool CanCraft(Recipe recipe)
        {
            foreach (var kvp in recipe.RequiredItems)
            {
                if (_inventory.GetItemCount(kvp.Key) < kvp.Value)
                    return false;
            }

            if (!string.IsNullOrEmpty(recipe.RequiredPropModel))
            {
                if (!IsNearProp(recipe.RequiredPropModel))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsNearProp(string modelName)
        {
            Model model = new Model(modelName);
            // SHVDN doesn't have World.GetClosestProp usually, so we use GetNearbyProps
            Prop[] props = World.GetNearbyProps(Game.Player.Character.Position, 3.0f);
            foreach(var p in props)
            {
                if (p.Model == model) return true;
            }
            return false;
        }

        public string GetMissingRequirement(Recipe recipe)
        {
             if (!string.IsNullOrEmpty(recipe.RequiredPropModel))
            {
                if (!IsNearProp(recipe.RequiredPropModel)) return $"Requires nearby {recipe.RequiredPropModel}";
            }
            return "Missing Ingredients";
        }

        public void StartCrafting(Recipe recipe)
        {
            if (!CanCraft(recipe))
            {
                Notification.Show(GetMissingRequirement(recipe));
                return;
            }

            _currentRecipe = recipe;
            _isCrafting = true;
            _craftProgress = 0f;

            string animDict = "amb@prop_human_parking_meter@male@base";
            if (recipe.Name.Contains("Metal")) animDict = "amb@world_human_welding@male@base";

            Game.Player.Character.Task.PlayAnimation(animDict, "base", 8.0f, -1, AnimationFlags.Loop);
        }

        public void OnTick()
        {
            if (!_isCrafting) return;

            _craftProgress += Game.LastFrameTime;

            new TextElement($"Crafting {_currentRecipe.Name}... {(_craftProgress / _currentRecipe.CraftingTime) * 100:0}%", new System.Drawing.PointF(600, 600), 0.5f).Draw();

            if (_craftProgress >= _currentRecipe.CraftingTime)
            {
                CompleteCrafting();
            }
        }

        private void CompleteCrafting()
        {
            _isCrafting = false;
            Game.Player.Character.Task.ClearAll();

            foreach (var kvp in _currentRecipe.RequiredItems)
            {
                _inventory.RemoveItem(kvp.Key, kvp.Value);
            }

            _inventory.AddItem(_currentRecipe.ResultItem, _currentRecipe.ResultCount);
            Notification.Show($"Crafted {_currentRecipe.ResultCount}x {_currentRecipe.Name}!");
        }
    }
}
