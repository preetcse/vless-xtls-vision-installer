using System;
using System.Collections.Generic;
using RoleplayOverhaul.Items;
using GTA; // For UI and Input

namespace RoleplayOverhaul.Core
{
    public enum LicenseType
    {
        Driver,
        Weapon,
        Hunting,
        Flying,
        Commercial,
        HealthInsurance // Treated as a license for logic simplicity
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
            TotalCheckpoints = 10; // Default
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
                // Callback to manager to issue license would happen here
            }
            else
            {
                GTA.UI.Screen.ShowSubtitle($"Failed {Type} Test. Try again.");
            }
        }
    }

    public class LicenseManager
    {
        private Inventory _playerInventory;
        private LicenseTest _activeTest;
        private const int LICENSE_VALIDITY_DAYS = 30;

        public LicenseManager(Inventory inventory)
        {
            _playerInventory = inventory;
        }

        public bool HasValidLicense(LicenseType type)
        {
            string id = "license_" + type.ToString().ToLower();
            var stack = _playerInventory.Slots.Find(s => s.Item.Id == id);

            if (stack == null) return false;

            if (stack.Item is LicenseItem lic)
            {
                 // Check expiry
                 return lic.ExpiryDate > DateTime.Now;
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
                // Logic to simulate passing checkpoints for demonstration
                // In a real game, this would check distance to markers
                if (GTA.Game.GameTime % 5000 == 0) // Every 5 seconds mock checkpoint
                {
                     _activeTest.PassCheckpoint();
                     if (!_activeTest.IsActive) // Test finished
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
            _playerInventory.RemoveItem(id, 1); // clear old

            var newLicense = new LicenseItem(
                id,
                $"{type} License",
                type.ToString(),
                DateTime.Now.AddDays(LICENSE_VALIDITY_DAYS)
            );

            _playerInventory.AddItem(newLicense, 1);
            GTA.UI.Screen.ShowSubtitle($"Issued {type} License. Valid for 30 days.");
        }

        public void RevokeLicense(LicenseType type)
        {
             string id = "license_" + type.ToString().ToLower();
             _playerInventory.RemoveItem(id, 1);
             GTA.UI.Screen.ShowSubtitle($"Revoked {type} License.");
        }
    }
}
