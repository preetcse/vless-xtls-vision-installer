using System;
using GTA;
using RoleplayOverhaul.Core;
using RoleplayOverhaul.Items;
using RoleplayOverhaul.Jobs;
using RoleplayOverhaul.UI;
using RoleplayOverhaul.Activities;
using RoleplayOverhaul.Jobs.Leveling;
using RoleplayOverhaul.Crafting;
using RoleplayOverhaul.Activities.Illegal;

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
        private PropertyManager _propertyManager;
        private CharacterManager _characterManager;
        private Missions.HeistManager _heistManager;
        private Banking.BankingManager _bankingManager;
        private Banking.BillManager _billManager;
        private Banking.ATMManager _atmManager;
        private Banking.BankInterior _bankInterior;
        private ConfigManager _configManager;
        private Activities.ActivityManager _activityManager;
        private Core.VehicleManager _vehicleManager;
        private Core.CareerManager _careerManager;
        private Core.BusinessManager _businessManager;
        private World.TrafficManager _trafficManager;
        private World.PopulationManager _populationManager;
        private Weapons.WeaponSystem _weaponSystem;
        private Persistence.PersistenceManager _persistenceManager;
        private Persistence.VehiclePersistence _vehiclePersistence;
        private Core.WardrobeManager _wardrobeManager;

        // Minigames
        private CookingMinigame _cookingMinigame;
        private MiningMinigame _miningMinigame;
        private OilDrillingMinigame _oilDrillingMinigame;
        private TreasureHuntingMinigame _treasureHuntingMinigame;
        private LumberjackMinigame _lumberjackMinigame;

        // New Systems
        private CraftingManager _craftingManager;
        private CraftingMenu _craftingMenu;
        private InteractionSystem _interactionSystem;
        private DrugRun _drugRun;
        private GangRaidManager _gangRaid; // Updated Type
        private KidnappingManager _kidnappingManager; // Added

        public RoleplayMod()
        {
            // Initialize Config
            _configManager = new ConfigManager();

            // Initialize Core Systems
            _playerInventory = new Inventory(30, 100.0f);
            _licenseManager = new LicenseManager(_playerInventory);
            _jobManager = new JobManager();
            _crimeManager = new Police.CrimeManager();
            _dispatchManager = new Police.DispatchManager(_crimeManager);
            _survivalManager = new SurvivalManager();
            _prisonManager = new PrisonManager();
            _gangManager = new GangManager();
            _propertyManager = new PropertyManager();
            _characterManager = new CharacterManager();
            _heistManager = new Missions.HeistManager();
            _activityManager = new Activities.ActivityManager();
            _careerManager = new Core.CareerManager();
            _wardrobeManager = new Core.WardrobeManager();

            // World Systems (Using Massive Data)
            _trafficManager = new World.TrafficManager();
            _populationManager = new World.PopulationManager();
            _weaponSystem = new Weapons.WeaponSystem();

            // Persistence
            _persistenceManager = new Persistence.PersistenceManager();
            _vehiclePersistence = new Persistence.VehiclePersistence();
            _persistenceManager.LoadGame();

            // Banking & Vehicle Systems
            _bankingManager = new Banking.BankingManager();
            _bankingManager.AutoBankIncome = _configManager.AutoBank;

            _billManager = new Banking.BillManager(_bankingManager);
            _atmManager = new Banking.ATMManager(_bankingManager);
            _bankInterior = new Banking.BankInterior(_bankingManager);
            _vehicleManager = new Core.VehicleManager(_bankingManager);
            _businessManager = new Core.BusinessManager(_bankingManager);

            // UI needs bank ref now
            _uiManager = new UIManager(_playerInventory, _bankingManager);
            _uiManager.SetSurvivalManager(_survivalManager); // Inject for vHUD

            // Initialize Minigames
            _cookingMinigame = new CookingMinigame();
            _miningMinigame = new MiningMinigame();
            _oilDrillingMinigame = new OilDrillingMinigame();
            _treasureHuntingMinigame = new TreasureHuntingMinigame();
            _lumberjackMinigame = new LumberjackMinigame();

            // Initialize New Systems
            _craftingManager = new CraftingManager(_playerInventory);
            _craftingMenu = new CraftingMenu(_playerInventory, _craftingManager);

            _drugRun = new DrugRun();
            _gangRaid = new GangRaidManager(); // Updated
            _kidnappingManager = new KidnappingManager(); // Added

            // Init Interaction System with dependency
            _interactionSystem = new InteractionSystem(_kidnappingManager);

            // Register Events
            Tick += OnTick;
            KeyDown += OnKeyDown;

            // Load Jobs with Dependencies
            var allJobs = JobLibrary.CreateAllJobs(_crimeManager);
            foreach(var job in allJobs)
            {
                _jobManager.RegisterJob(job);
            }

            // Load Heists
            _heistManager.RegisterHeist(new Missions.FleecaBankHeist(_crimeManager, _playerInventory));

            // Setup Default Data
            SetupInitialState();
        }

        private void SetupInitialState()
        {
            // Starter Items (Now with hunger/thirst values)
            _playerInventory.AddItem(new FoodItem("burger", "Burger", "Delicious", "food_burger", 0.5f, 20, 30.0f, 5.0f), 2);
            _playerInventory.AddItem(new FoodItem("water", "Water", "Hydrating", "drink_water", 0.5f, 5, 5.0f, 40.0f), 2);

            // Give Drill for testing heists
            _playerInventory.AddItem(new Items.ToolItem("tool_drill", "Thermal Drill", "For bank vaults", "drill_icon", 5.0f), 1);

            // Give Raw Materials for crafting
            _playerInventory.AddItem(new Items.ResourceItem("iron_ore", "Iron Ore", "Needs smelting", "rock_icon", 5), 5);

            // Check Licenses on start
            if (!_licenseManager.HasValidLicense(LicenseType.HealthInsurance))
            {
                GTA.UI.Screen.ShowSubtitle("Warning: You do not have active Health Insurance!");
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            try
            {
                // Diagnostics.Logger.Trace("Tick Start"); // Very spammy, uncomment if deep debugging needed

                _jobManager.OnTick();
                _licenseManager.Update();

                _crimeManager.Update();
                _dispatchManager.OnTick();
                _survivalManager.OnTick();
                _prisonManager.OnTick();
                _propertyManager.CheckInteraction();
                _propertyManager.DailyUpdate();
                _heistManager.OnTick();

                _bankingManager.OnTick();
                _billManager.OnTick();
                _atmManager.OnTick();
                _bankInterior.OnTick();
                _vehicleManager.OnTick();
                _activityManager.OnTick();
                _businessManager.OnTick();
                _trafficManager.OnTick();
                _populationManager.OnTick();
                _weaponSystem.OnTick();
                _persistenceManager.OnTick();

                // Minigame Ticks
                _cookingMinigame.OnTick();
                _miningMinigame.OnTick();
                _oilDrillingMinigame.OnTick();
                _treasureHuntingMinigame.OnTick();
                _lumberjackMinigame.OnTick();

                // New Systems Tick
                _craftingManager.OnTick();
                _craftingMenu.Draw();
                _craftingMenu.HandleInput();
                _interactionSystem.OnTick();
                _drugRun.OnTick();
                _gangRaid.OnTick();
                _kidnappingManager.OnTick(); // Added

                // Check for Arrest
                if (_crimeManager.WantedStars > 0 && GTA.Game.Player.WantedLevel == 0 && _prisonManager.SentenceTimeRemaining == 0)
                {
                     // Vanilla system cleared wanted level (Busted), so we imprison
                }

                // Draw UI with Heat Stats
                _uiManager.Draw(_crimeManager.HeatLevel);
            }
            catch (Exception ex)
            {
                Diagnostics.Logger.Error("CRASH IN TICK LOOP", ex);
                GTA.UI.Screen.ShowSubtitle("~r~Roleplay Overhaul Error! Check Log.");
                // Option: Disable mod to prevent infinite error loop
                // Tick -= OnTick;
            }
        }

        private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            // F5 to toggle Inventory
            if (e.KeyCode == System.Windows.Forms.Keys.F5)
            {
                _uiManager.ToggleInventory();
            }

            // F4 for Crafting Menu
            if (e.KeyCode == System.Windows.Forms.Keys.F4)
            {
                _craftingMenu.Toggle();
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

            // Start Heist Debug
            if (e.KeyCode == System.Windows.Forms.Keys.H)
            {
                _heistManager.StartHeist("The Fleeca Job");
            }

            // Open Banking App (Phone shortcut)
            if (e.KeyCode == System.Windows.Forms.Keys.B)
            {
                _uiManager.ToggleBankingApp();
            }

            // Start Activity Debug
            if (e.KeyCode == System.Windows.Forms.Keys.Z)
            {
                _activityManager.StartActivity("Zombie Survival");
            }

            // Minigame Hotkeys (For testing)
            if (e.KeyCode == System.Windows.Forms.Keys.NumPad1) _cookingMinigame.StartCooking();
            if (e.KeyCode == System.Windows.Forms.Keys.NumPad2) _miningMinigame.StartMining();
            if (e.KeyCode == System.Windows.Forms.Keys.NumPad3) _oilDrillingMinigame.StartDrilling();
            if (e.KeyCode == System.Windows.Forms.Keys.NumPad4) _treasureHuntingMinigame.StartHunt(Game.Player.Character.Position + new GTA.Math.Vector3(50, 0, 0));
            if (e.KeyCode == System.Windows.Forms.Keys.NumPad5) _lumberjackMinigame.StartChopping();

            // Illegal Activity Debug
            if (e.KeyCode == System.Windows.Forms.Keys.NumPad6) _drugRun.StartMission();
            if (e.KeyCode == System.Windows.Forms.Keys.NumPad7) _gangRaid.StartRaid(Game.Player.Character.Position + new GTA.Math.Vector3(50,50,0), "Ballas");
            if (e.KeyCode == System.Windows.Forms.Keys.G && !_kidnappingManager.IsKidnapping) // G to grapple closest ped if not dragging
            {
                GTA.Ped closest = GTA.World.GetClosestPed(GTA.Game.Player.Character.Position, 2.0f);
                if (closest != null) _kidnappingManager.AttemptKidnap(closest);
            }
        }
    }
}
