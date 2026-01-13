using System;
using System.Collections.Generic;
using RoleplayOverhaul.Items;
using RoleplayOverhaul.Core.Inventory; // New System
using GTA;

namespace RoleplayOverhaul.Core
{
    public enum LicenseType
    {
        Driver,
        Weapon,
        Hunting,
        Flying,
        Commercial,
        HealthInsurance
    }

    public class LicenseTest
    {
        public LicenseType Type { get; set; }
        public bool IsActive { get; private set; }
        public int CheckpointsPassed { get; set; }
        public int TotalCheckpoints { get; set; }

        public LicenseTest(LicenseType type)
        {
            Type = type;
            IsActive = false;
            TotalCheckpoints = 10;
        }

        public void Start()
        {
            IsActive = true;
            CheckpointsPassed = 0;
            GTA.UI.Screen.ShowSubtitle($"Starting {Type} Test. Follow the instructions!");
        }

        public void PassCheckpoint()
        {
            CheckpointsPassed++;
            GTA.UI.Screen.ShowSubtitle($"Checkpoint {CheckpointsPassed}/{TotalCheckpoints}");

            if (CheckpointsPassed >= TotalCheckpoints)
            {
                Finish(true);
            }
        }

        public void Fail()
        {
            Finish(false);
        }

        private void Finish(bool success)
        {
            IsActive = false;
            if (success)
            {
                GTA.UI.Screen.ShowSubtitle($"Passed {Type} Test!");
            }
            else
            {
                GTA.UI.Screen.ShowSubtitle($"Failed {Type} Test. Try again.");
            }
        }
    }

    public class LicenseManager
    {
        private GridInventory _playerInventory; // Updated Dependency
        private LicenseTest _activeTest;
        private const int LICENSE_VALIDITY_DAYS = 30;

        public LicenseManager(GridInventory inventory)
        {
            _playerInventory = inventory;
        }

        public bool HasValidLicense(LicenseType type)
        {
            string id = "license_" + type.ToString().ToLower();

            // Search GridInventory slots
            foreach(var slot in _playerInventory.Slots)
            {
                if (slot != null && slot.ID == id)
                {
                    if (slot is LicenseItem lic)
                    {
                        return lic.ExpiryDate > DateTime.Now;
                    }
                }
            }
            return false;
        }

        public void StartLicenseTest(LicenseType type)
        {
            if (HasValidLicense(type))
            {
                GTA.UI.Screen.ShowSubtitle($"You already have a valid {type} license!");
                return;
            }

            _activeTest = new LicenseTest(type);
            _activeTest.Start();
        }

        public void Update()
        {
            if (_activeTest != null && _activeTest.IsActive)
            {
                if (GTA.Game.GameTime % 5000 == 0)
                {
                     _activeTest.PassCheckpoint();
                     if (!_activeTest.IsActive)
                     {
                         IssueLicense(_activeTest.Type);
                         _activeTest = null;
                     }
                }
            }
        }

        public void IssueLicense(LicenseType type)
        {
            string id = "license_" + type.ToString().ToLower();

            // Remove old license if exists (Renew)
            int count = _playerInventory.GetItemCount(id);
            if (count > 0) _playerInventory.RemoveItem(id, count);

            var newLicense = new LicenseItem(
                id,
                $"{type} License",
                type.ToString(),
                DateTime.Now.AddDays(LICENSE_VALIDITY_DAYS)
            );

            _playerInventory.AddItem(newLicense);
            GTA.UI.Screen.ShowSubtitle($"Issued {type} License. Valid for 30 days.");
        }

        public void RevokeLicense(LicenseType type)
        {
             string id = "license_" + type.ToString().ToLower();
             _playerInventory.RemoveItem(id, 1);
             GTA.UI.Screen.ShowSubtitle($"Revoked {type} License.");
        }

        // DMV Menu Logic (Simple UI)
        public void DrawMenu()
        {
            // Simple text menu for DMV
             GTA.UI.Screen.ShowHelpText("Press ~INPUT_CONTEXT~ to Open DMV Menu");
             if (GTA.Game.IsControlJustPressed(GTA.Control.Context))
             {
                 // Mock buying insurance
                 if (GTA.Game.Player.Money >= 500)
                 {
                     GTA.Game.Player.Money -= 500;
                     IssueLicense(LicenseType.HealthInsurance);
                     GTA.UI.Notification.Show("Purchased Health Insurance ($500)");
                 }
                 else
                 {
                     GTA.UI.Notification.Show("Need $500 for Insurance.");
                 }
             }
        }
    }
}
