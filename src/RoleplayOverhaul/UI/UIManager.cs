using System;
using System.Drawing;
using System.Collections.Generic;
using GTA.UI;
using RoleplayOverhaul.Items;
using GTA;

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

        public UIManager(Inventory inventory)
        {
            _inventory = inventory;
            _isVisible = false;
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

            // Mock getting mouse position (In real SHVDN: Game.MousePosition or via Natives)
            // For this logic, we assume we can get X/Y
            // Point mousePos = Game.MousePosition;
            // Since we can't compile Game.MousePosition without correct references, we will simulate the logic structure

            /*
            float mouseX = ...;
            float mouseY = ...;

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
                        // If item consumed/removed, refresh?
                        if (stack.Count <= 0) _inventory.Slots.Remove(stack);
                    }
                }
            }
            */
        }

        public void Draw()
        {
            if (!_isVisible) return;

            // Draw Background
            int rows = (int)Math.Ceiling((double)_inventory.MaxSlots / COLS);
            int width = (COLS * (SLOT_SIZE + PADDING)) + PADDING;
            int height = (rows * (SLOT_SIZE + PADDING)) + PADDING;

            new ContainerElement(new PointF(_startPos.X, _startPos.Y), new SizeF(width, height), Color.FromArgb(200, 0, 0, 0)).Draw();

            // Draw Slots
            for (int i = 0; i < _inventory.MaxSlots; i++)
            {
                int col = i % COLS;
                int row = i / COLS;

                float x = _startPos.X + PADDING + (col * (SLOT_SIZE + PADDING));
                float y = _startPos.Y + PADDING + (row * (SLOT_SIZE + PADDING));

                // Slot Background
                new ContainerElement(new PointF(x, y), new SizeF(SLOT_SIZE, SLOT_SIZE), Color.FromArgb(150, 50, 50, 50)).Draw();

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
    }
}
