using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using RoleplayOverhaul.Items;

namespace RoleplayOverhaul.Core
{
    public class Property
    {
        public string Name { get; private set; }
        public Vector3 Location { get; private set; }
        public int Price { get; private set; }
        public int RentPrice { get; private set; } // Daily
        public bool IsOwned { get; set; }
        public bool IsRented { get; set; }

        public Property(string name, Vector3 loc, int price, int rent)
        {
            Name = name;
            Location = loc;
            Price = price;
            RentPrice = rent;
        }
    }

    public class PropertyManager
    {
        private List<Property> _properties;
        private int _lastDayChecked;

        public PropertyManager()
        {
            _properties = new List<Property>();
            LoadProperties();
            _lastDayChecked = DateTime.Now.Day;
        }

        private void LoadProperties()
        {
            // Apartments
            _properties.Add(new Property("Eclipse Towers Apt 9", new Vector3(-774.0f, 342.0f, 196.0f), 500000, 1000));
            _properties.Add(new Property("Alta St Apt 10", new Vector3(-270.0f, -955.0f, 31.0f), 200000, 500));

            // Motels (Rentals)
            _properties.Add(new Property("Pink Cage Motel", new Vector3(310.0f, -200.0f, 50.0f), 0, 50)); // Purchase 0 means rental only

            // Safehouses
            _properties.Add(new Property("Sandy Shores Trailer", new Vector3(1900.0f, 3700.0f, 32.0f), 50000, 100));
        }

        public void CheckInteraction()
        {
            Vector3 playerPos = GTA.Game.Player.Character.Position;
            foreach (var prop in _properties)
            {
                if (playerPos.DistanceTo(prop.Location) < 3.0f)
                {
                    if (prop.IsOwned || prop.IsRented)
                    {
                        GTA.UI.Screen.ShowHelpText($"Press E to Enter {prop.Name}");
                        if (GTA.Game.IsControlJustPressed(GTA.Control.Context))
                        {
                            EnterProperty(prop);
                        }
                    }
                    else
                    {
                        GTA.UI.Screen.ShowHelpText($"Press E to View {prop.Name} (Buy: ${prop.Price} / Rent: ${prop.RentPrice})");
                        if (GTA.Game.IsControlJustPressed(GTA.Control.Context))
                        {
                            TryPurchaseOrRent(prop);
                        }
                    }
                }
            }
        }

        private void EnterProperty(Property prop)
        {
            // Save Game logic
            GTA.Game.DoAutoSave();
            GTA.UI.Screen.ShowSubtitle($"Entered {prop.Name}. Game Saved.");

            // Restore stats (Safehouse benefit)
            GTA.Game.Player.Character.Health = GTA.Game.Player.Character.MaxHealth;
        }

        private void TryPurchaseOrRent(Property prop)
        {
            int money = GTA.Game.Player.Money;

            if (prop.Price > 0) // Purchasable
            {
                if (money >= prop.Price)
                {
                    GTA.Game.Player.Money -= prop.Price;
                    prop.IsOwned = true;
                    GTA.UI.Screen.ShowSubtitle($"Purchased {prop.Name}!");
                }
                else
                {
                    GTA.UI.Screen.ShowSubtitle("Not enough money to buy!");
                }
            }
            else // Rental only
            {
                 if (money >= prop.RentPrice)
                {
                    GTA.Game.Player.Money -= prop.RentPrice;
                    prop.IsRented = true; // Valid for 1 day
                    GTA.UI.Screen.ShowSubtitle($"Rented {prop.Name} for the day!");
                }
            }
        }

        public void DailyUpdate()
        {
            // Collect Rent
            if (DateTime.Now.Day != _lastDayChecked)
            {
                int totalRent = 0;
                foreach (var prop in _properties)
                {
                    if (prop.IsRented)
                    {
                        totalRent += prop.RentPrice;
                        // Expire rental if cant pay?
                        // prop.IsRented = false;
                    }
                }

                if (totalRent > 0)
                {
                    GTA.Game.Player.Money -= totalRent;
                    GTA.UI.Screen.ShowSubtitle($"Paid daily rent: ${totalRent}");
                }
                _lastDayChecked = DateTime.Now.Day;
            }
        }
    }
}
