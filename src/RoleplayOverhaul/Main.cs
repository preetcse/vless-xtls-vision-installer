using System;
using GTA;
using RoleplayOverhaul.Core;
using RoleplayOverhaul.Items;
using RoleplayOverhaul.Jobs;
using RoleplayOverhaul.UI;

namespace RoleplayOverhaul
{
    public class RoleplayMod : Script
    {
        private Inventory _playerInventory;
        private LicenseManager _licenseManager;
        private JobManager _jobManager;
        private UIManager _uiManager;

        public RoleplayMod()
        {
            // Initialize Core Systems
            _playerInventory = new Inventory(30, 100.0f); // Increased size
            _licenseManager = new LicenseManager(_playerInventory);
            _jobManager = new JobManager();
            _uiManager = new UIManager(_playerInventory);

            // Register Events
            Tick += OnTick;
            KeyDown += OnKeyDown;
            // MouseDown += OnMouseDown; // Hypothetical event

            // Load Jobs
            var allJobs = JobLibrary.CreateAllJobs();
            foreach(var job in allJobs)
            {
                _jobManager.RegisterJob(job);
            }

            // Setup Default Data
            SetupInitialState();
        }

        private void SetupInitialState()
        {
            // Starter Items
            _playerInventory.AddItem(new FoodItem("burger", "Burger", "Delicious", "food_burger", 0.5f, 20), 2);
            _playerInventory.AddItem(new FoodItem("water", "Water", "Hydrating", "drink_water", 0.5f, 10), 2);

            // Check Licenses on start
            if (!_licenseManager.HasValidLicense(LicenseType.HealthInsurance))
            {
                GTA.UI.Screen.ShowSubtitle("Warning: You do not have active Health Insurance!");
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            _jobManager.OnTick();
            _licenseManager.Update(); // Check tests
            _uiManager.Draw();
        }

        private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            // F5 to toggle Inventory
            if (e.KeyCode == System.Windows.Forms.Keys.F5)
            {
                _uiManager.ToggleInventory();
            }

            // Debug: Start a test
            if (e.KeyCode == System.Windows.Forms.Keys.F6)
            {
                _licenseManager.StartLicenseTest(LicenseType.Driver);
            }

            // Interaction key (e.g., E) could simulate click for now
            if (e.KeyCode == System.Windows.Forms.Keys.E)
            {
                _uiManager.ProcessMouseClick(); // Mock click at current cursor
            }
        }
    }
}
