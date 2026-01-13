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
        private Police.CrimeManager _crimeManager;
        private Police.DispatchManager _dispatchManager;
        private SurvivalManager _survivalManager;
        private PrisonManager _prisonManager;
        private GangManager _gangManager;

        public RoleplayMod()
        {
            // Initialize Core Systems
            _playerInventory = new Inventory(30, 100.0f); // Increased size
            _licenseManager = new LicenseManager(_playerInventory);
            _jobManager = new JobManager();
            _uiManager = new UIManager(_playerInventory);
            _crimeManager = new Police.CrimeManager();
            _dispatchManager = new Police.DispatchManager(_crimeManager);
            _survivalManager = new SurvivalManager();
            _prisonManager = new PrisonManager();
            _gangManager = new GangManager();

            // Register Events
            Tick += OnTick;
            KeyDown += OnKeyDown;

            // Load Jobs with Dependencies
            var allJobs = JobLibrary.CreateAllJobs(_crimeManager);
            foreach(var job in allJobs)
            {
                _jobManager.RegisterJob(job);
            }

            // Setup Default Data
            SetupInitialState();
        }

        private void SetupInitialState()
        {
            // Starter Items (Now with hunger/thirst values)
            _playerInventory.AddItem(new FoodItem("burger", "Burger", "Delicious", "food_burger", 0.5f, 20, 30.0f, 5.0f), 2);
            _playerInventory.AddItem(new FoodItem("water", "Water", "Hydrating", "drink_water", 0.5f, 5, 5.0f, 40.0f), 2);

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

            // New Managers
            _crimeManager.Update();
            _dispatchManager.OnTick();
            _survivalManager.OnTick();
            _prisonManager.OnTick();

            // Check for Arrest
            if (_crimeManager.WantedStars > 0 && GTA.Game.Player.WantedLevel == 0 && _prisonManager.SentenceTimeRemaining == 0)
            {
                 // Vanilla system cleared wanted level (Busted), so we imprison
                 // Note: Needs robust detection, simplified here
                 // _dispatchManager.AttemptArrest(); // Fines logic
                 // _prisonManager.Imprison(60); // 1 minute jail
            }

            // Draw UI with Heat Stats
            _uiManager.Draw(_crimeManager.HeatLevel);
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
                // Logic: if in inventory, click. If not, maybe consume item?
                // For now, simple mock click
                _uiManager.ProcessMouseClick();
            }

            // Surrender
             if (e.KeyCode == System.Windows.Forms.Keys.L && _crimeManager.WantedStars > 0)
            {
                _dispatchManager.AttemptArrest();
                _prisonManager.Imprison(120); // 2 mins if surrendered
            }
        }
    }
}
