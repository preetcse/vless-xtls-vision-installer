using System;
using System.Drawing;
using System.Collections.Generic;
using GTA.UI;
using GTA;
using RoleplayOverhaul.Items;
using RoleplayOverhaul.Core;
using RoleplayOverhaul.Banking;

namespace RoleplayOverhaul.UI
{
    public class UIManager
    {
        private Inventory _inventory;
        private bool _isVisible;

        // Grid Settings
        private const int SLOT_SIZE = 80;
        private const int PADDING = 10;
        private const int COLS = 5;
        private Point _startPos = new Point(200, 200);

        // Banking App
        public bool IsBankingAppOpen { get; private set; }
        private Banking.BankingManager _bank;

        // HUD State (vHUD features)
        private SurvivalManager _survival; // Need to inject survival manager

        public UIManager(Inventory inventory, Banking.BankingManager bank)
        {
            _inventory = inventory;
            _bank = bank;
            _isVisible = false;
        }

        public void SetSurvivalManager(SurvivalManager survival)
        {
            _survival = survival;
        }

        public void ToggleBankingApp()
        {
            IsBankingAppOpen = !IsBankingAppOpen;
            if (IsBankingAppOpen) _isVisible = false; // Close inv if bank open
        }

        public void ToggleInventory()
        {
            _isVisible = !_isVisible;
            if (_isVisible)
            {
                // In a real script we would enable a cursor
                // Function.Call(Hash.SET_MOUSE_CURSOR_VISIBLE_IN_MENUS, true);
                GTA.UI.Screen.ShowSubtitle("Inventory Opened. Click items to use.");
            }
            else
            {
                // Function.Call(Hash.SET_MOUSE_CURSOR_VISIBLE_IN_MENUS, false);
            }
        }

        public void ProcessMouseClick()
        {
            if (!_isVisible) return;

            // Get Real Mouse Position (Supported by both Game and Stub now)
            Point mousePos = GTA.UI.Screen.MousePosition;
            float mouseX = mousePos.X;
            float mouseY = mousePos.Y;

            // Check collision with slots
            for (int i = 0; i < _inventory.MaxSlots; i++)
            {
                int col = i % COLS;
                int row = i / COLS;

                float x = _startPos.X + PADDING + (col * (SLOT_SIZE + PADDING));
                float y = _startPos.Y + PADDING + (row * (SLOT_SIZE + PADDING));

                if (mouseX >= x && mouseX <= x + SLOT_SIZE && mouseY >= y && mouseY <= y + SLOT_SIZE)
                {
                    // Clicked slot i
                    if (i < _inventory.Slots.Count)
                    {
                        var stack = _inventory.Slots[i];
                        stack.Item.OnUse();

                        // Handle consumption (Stack reduction)
                        if (stack.Item.IsUsable)
                        {
                            // Hook into Survival System if it's food
                            if (stack.Item is FoodItem food && _survival != null)
                            {
                                _survival.Consume(food);
                            }

                            stack.Count--;
                            if (stack.Count <= 0)
                            {
                                _inventory.Slots.Remove(stack);
                            }
                        }
                        return; // Click handled
                    }
                }
            }
        }

        public void Draw(int heatLevel)
        {
            // Always draw HUD elements like Heat Level
            new TextElement($"Heat: {heatLevel}%", new PointF(10, 10), 0.5f, heatLevel > 0 ? Color.Red : Color.White).Draw();

            // vHUD: Draw Bars
            DrawHUD();

            // Draw Heist Status if Active
            // Note: In real app, we'd inject HeistManager, but for loose coupling we can rely on external calls or a singleton.
            // For this UI update, I'll stick to the inventory, but logic would go here.

            if (!_isVisible) return;

            if (IsBankingAppOpen)
            {
                DrawBankingApp();
                return; // Exclusive view
            }

            // Draw Background
            int rows = (int)Math.Ceiling((double)_inventory.MaxSlots / COLS);
            int width = (COLS * (SLOT_SIZE + PADDING)) + PADDING;
            int height = (rows * (SLOT_SIZE + PADDING)) + PADDING;

            new Rectangle(new PointF(_startPos.X, _startPos.Y), new SizeF(width, height), Color.FromArgb(200, 0, 0, 0)).Draw();

            // Draw Slots
            for (int i = 0; i < _inventory.MaxSlots; i++)
            {
                int col = i % COLS;
                int row = i / COLS;

                float x = _startPos.X + PADDING + (col * (SLOT_SIZE + PADDING));
                float y = _startPos.Y + PADDING + (row * (SLOT_SIZE + PADDING));

                // Slot Background
                new Rectangle(new PointF(x, y), new SizeF(SLOT_SIZE, SLOT_SIZE), Color.FromArgb(150, 50, 50, 50)).Draw();

                // Draw Item if exists
                if (i < _inventory.Slots.Count)
                {
                    var stack = _inventory.Slots[i];

                    // Icon
                    // In real engine: new Sprite("textures", stack.Item.IconTexture, ...).Draw();
                    new TextElement(stack.Item.Name.Substring(0, Math.Min(4, stack.Item.Name.Length)), new PointF(x + 5, y + 5), 0.4f, Color.White).Draw();

                    // Count
                    if (stack.Count > 1)
                    {
                        new TextElement(stack.Count.ToString(), new PointF(x + SLOT_SIZE - 20, y + SLOT_SIZE - 20), 0.35f, Color.Yellow).Draw();
                    }
                }
            }
        }

        private void DrawBankingApp()
        {
             // Phone-like UI background
             PointF pos = new PointF(1500, 500);
             new Rectangle(pos, new SizeF(300, 500), Color.FromArgb(255, 30, 30, 30)).Draw();
             new TextElement("Maze Bank", new PointF(pos.X + 100, pos.Y + 20), 0.6f, Color.Red).Draw();

             new TextElement($"Balance: ${_bank.Balance:N0}", new PointF(pos.X + 20, pos.Y + 80), 0.5f, Color.White).Draw();
             new TextElement($"Debt: ${_bank.Debt:N0}", new PointF(pos.X + 20, pos.Y + 120), 0.5f, Color.Red).Draw();

             new TextElement("Recent Transactions:", new PointF(pos.X + 20, pos.Y + 180), 0.4f, Color.Gray).Draw();

             int i = 0;
             foreach(var t in _bank.History) // In real app, take last 5 reverse
             {
                 if (i > 5) break;
                 new TextElement($"{t.Description}: ${t.Amount}", new PointF(pos.X + 20, pos.Y + 220 + (i*30)), 0.35f, t.Amount > 0 ? Color.Green : Color.Red).Draw();
                 i++;
             }
        }

        private void DrawHUD()
        {
            if (_survival == null) return;

            // Draw Bottom Left Bars (GTA V style area)
            // Hunger (Orange)
            float hungerW = _survival.Hunger * 2.0f;
            new Rectangle(new PointF(20, 1000), new SizeF(200, 10), Color.FromArgb(100, 0, 0, 0)).Draw(); // BG
            new Rectangle(new PointF(20, 1000), new SizeF(hungerW, 10), Color.Orange).Draw();

            // Thirst (Blue)
            float thirstW = _survival.Thirst * 2.0f;
            new Rectangle(new PointF(20, 1015), new SizeF(200, 10), Color.FromArgb(100, 0, 0, 0)).Draw(); // BG
            new Rectangle(new PointF(20, 1015), new SizeF(thirstW, 10), Color.Blue).Draw();

            // Fatigue (Gray)
            float fatigueW = _survival.Sleep * 2.0f;
            new Rectangle(new PointF(20, 1030), new SizeF(200, 10), Color.FromArgb(100, 0, 0, 0)).Draw(); // BG
            new Rectangle(new PointF(20, 1030), new SizeF(fatigueW, 10), Color.Gray).Draw();

            // Speedometer (Text)
            if (GTA.Game.Player.Character.IsInVehicle())
            {
                var veh = GTA.Game.Player.Character.CurrentVehicle;
                float speedKmh = veh.Speed * 3.6f;
                float fuel = 100.0f; // Mock Fuel

                new TextElement($"{speedKmh:F0} KM/H", new PointF(1700, 900), 1.0f, Color.White).Draw();
                new TextElement($"Fuel: {fuel}%", new PointF(1700, 950), 0.5f, Color.Yellow).Draw();
            }
        }
    }
}
