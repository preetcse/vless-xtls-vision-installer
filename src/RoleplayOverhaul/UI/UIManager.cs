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

            // In a real environment we would get the mouse cursor position
            // Since we are mocking dependencies, we'll simulate a click on the first slot for demonstration
            // if no cursor logic is available.

            // However, to make this logic robust for the actual mod, we implement the loop:

            // Mock Mouse Position (Center of Slot 0 for testing)
            float mouseX = _startPos.X + PADDING + 10;
            float mouseY = _startPos.Y + PADDING + 10;

            // Note: In real Game, use: Point mousePos = GTA.UI.Screen.MousePosition;
            // mouseX = mousePos.X; mouseY = mousePos.Y;

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
