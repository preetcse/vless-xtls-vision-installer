using System;
using GTA;
using GTA.Math; // Added for Vector3
using RoleplayOverhaul.Core;
using RoleplayOverhaul.Items;
using RoleplayOverhaul.Jobs;
using RoleplayOverhaul.UI;
using RoleplayOverhaul.Activities;
using RoleplayOverhaul.Core.Progression;
using RoleplayOverhaul.Core.Inventory;
using RoleplayOverhaul.Crafting;
using RoleplayOverhaul.Activities.Illegal;
using System.Windows.Forms;

namespace RoleplayOverhaul
{
    public class RoleplayMod : Script
    {
        // Core Systems
        private ExperienceManager _xpManager;
        private GridInventory _inventory;
        private InventoryMenu _inventoryMenu;
        private TruckingJob _truckingJob;

        // UI
        private JobMenu _jobMenu;

        // Legacy Systems (Keeping for now but refactoring)
        private LicenseManager _licenseManager;
        private JobManager _jobManager;
        private Police.CrimeManager _crimeManager;
        private Police.DispatchManager _dispatchManager;
        private SurvivalManager _survivalManager;
        private PrisonManager _prisonManager;
        private GangManager _gangManager;
        private PropertyManager _propertyManager;
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
        private GangRaidManager _gangRaid;
        private KidnappingManager _kidnappingManager;
        private GymActivity _gymActivity;

        public RoleplayMod()
        {
            // Initialize Config
            _configManager = new ConfigManager();

            // Phase 1: Leveling
            _xpManager = new ExperienceManager();

            // Phase 2: Inventory
            _inventory = new GridInventory(30);
            _inventoryMenu = new InventoryMenu(_inventory);

            // Phase 3: Jobs
            _truckingJob = new TruckingJob();

            // Legacy Initialization with Unified Inventory
            _licenseManager = new LicenseManager(_inventory); // FIXED: Uses GridInventory now
            _jobManager = new JobManager();
            _crimeManager = new Police.CrimeManager();
            _dispatchManager = new Police.DispatchManager(_crimeManager);
            _survivalManager = new SurvivalManager(_licenseManager);
            _prisonManager = new PrisonManager();
            _gangManager = new GangManager();
            _propertyManager = new PropertyManager();
            _heistManager = new Missions.HeistManager();

            // Register Heist with Unified Inventory
            _heistManager.RegisterHeist(new Missions.FleecaBankHeist(_crimeManager, _inventory));

            _activityManager = new Activities.ActivityManager();
            _careerManager = new Core.CareerManager();
            _wardrobeManager = new Core.WardrobeManager();
            _trafficManager = new World.TrafficManager();
            _populationManager = new World.PopulationManager();
            _weaponSystem = new Weapons.WeaponSystem();
            _persistenceManager = new Persistence.PersistenceManager();
            _vehiclePersistence = new Persistence.VehiclePersistence();
            _persistenceManager.LoadGame();
            _bankingManager = new Banking.BankingManager();
            _billManager = new Banking.BillManager(_bankingManager);
            _atmManager = new Banking.ATMManager(_bankingManager);
            _bankInterior = new Banking.BankInterior(_bankingManager);
            _vehicleManager = new Core.VehicleManager(_bankingManager);
            _businessManager = new Core.BusinessManager(_bankingManager);

            _cookingMinigame = new CookingMinigame();
            _miningMinigame = new MiningMinigame();
            _oilDrillingMinigame = new OilDrillingMinigame();
            _treasureHuntingMinigame = new TreasureHuntingMinigame();
            _lumberjackMinigame = new LumberjackMinigame();

            _drugRun = new DrugRun();
            _gangRaid = new GangRaidManager();
            _kidnappingManager = new KidnappingManager();
            _interactionSystem = new InteractionSystem(_kidnappingManager);
            _gymActivity = new GymActivity(_xpManager);

            // Wiring Up Missing Systems (Jobs & Crafting)
            _craftingManager = new CraftingManager(_inventory);
            _craftingMenu = new CraftingMenu(_inventory, _craftingManager);

            // Register Events
            Tick += OnTick;
            KeyDown += OnKeyDown;

            // Load Jobs (Wiring the 30+ jobs)
            var allJobs = JobLibrary.CreateAllJobs(_crimeManager, _xpManager);
            foreach(var job in allJobs)
            {
                _jobManager.RegisterJob(job);
            }

            // Setup Job Menu
            _jobMenu = new JobMenu(_jobManager);
            _jobMenu.SetJobs(allJobs);

            SetupInitialState();
        }

        private void SetupInitialState()
        {
            // Starter Items in New Grid Inventory (Fully Qualified to avoid Ambiguity)
            _inventory.AddItem(new RoleplayOverhaul.Core.Inventory.FoodItem("burger", "Burger", 20f, 5f));
            _inventory.AddItem(new RoleplayOverhaul.Core.Inventory.FoodItem("water", "Water", 5f, 40f));
            _inventory.AddItem(new RoleplayOverhaul.Core.Inventory.WeaponItem("pistol", "Pistol", WeaponHash.Pistol));
        }

        private void OnTick(object sender, EventArgs e)
        {
            try
            {
                // New Systems
                _truckingJob.OnTick();
                _inventoryMenu.Draw();
                _jobMenu.Draw(); // Draw Job Menu

                // Legacy Systems
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
                _cookingMinigame.OnTick();
                _miningMinigame.OnTick();
                _oilDrillingMinigame.OnTick();
                _treasureHuntingMinigame.OnTick();
                _lumberjackMinigame.OnTick();
                _interactionSystem.OnTick();
                _drugRun.OnTick();
                _gangRaid.OnTick();
                _kidnappingManager.OnTick();
                _gymActivity.OnTick();

                // Crafting
                _craftingManager.OnTick();
                _craftingMenu.Draw();
                _craftingMenu.HandleInput();

                // DMV Check (Davis)
                if (Vector3.Distance(Game.Player.Character.Position, new Vector3(-54, -1111, 26)) < 3.0f)
                {
                    _licenseManager.DrawMenu();
                }
            }
            catch (Exception ex)
            {
                Diagnostics.Logger.Error("CRASH IN TICK LOOP", ex);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // New Inventory Toggle
            if (e.KeyCode == Keys.I)
            {
                _inventoryMenu.Toggle();
            }

            // Handle Menu Input
            _inventoryMenu.HandleInput(e);
            _jobMenu.HandleInput(e); // Job Menu Input

            // Job Menu Toggle
            if (e.KeyCode == Keys.J)
            {
                _jobMenu.Toggle();
            }

            // Crafting Toggle
            if (e.KeyCode == Keys.F4)
            {
                _craftingMenu.Toggle();
            }

            // Debug: Add XP
            if (e.KeyCode == Keys.F10)
            {
                _xpManager.AddXP(ExperienceManager.Skill.Global, 500);
            }

            // Interaction key
            if (e.KeyCode == Keys.E)
            {
                // Logic handled in interaction system
            }

            // Surrender
             if (e.KeyCode == Keys.L && _crimeManager.WantedStars > 0)
            {
                _dispatchManager.AttemptArrest();
                _prisonManager.Imprison(120);
            }

            if (e.KeyCode == Keys.H) _heistManager.StartHeist("The Fleeca Job");
            if (e.KeyCode == Keys.B) { /* Open Banking */ }

            if (e.KeyCode == Keys.NumPad1) _cookingMinigame.StartCooking();
            if (e.KeyCode == Keys.NumPad2) _miningMinigame.StartMining();
            if (e.KeyCode == Keys.NumPad3) _oilDrillingMinigame.StartDrilling();
            if (e.KeyCode == Keys.NumPad4) _treasureHuntingMinigame.StartHunt(Game.Player.Character.Position + new GTA.Math.Vector3(50, 0, 0));
            if (e.KeyCode == Keys.NumPad5) _lumberjackMinigame.StartChopping();

            if (e.KeyCode == Keys.NumPad6) _drugRun.StartMission();
            if (e.KeyCode == Keys.NumPad7) _gangRaid.StartRaid(Game.Player.Character.Position + new GTA.Math.Vector3(50,50,0), "Ballas");
            if (e.KeyCode == Keys.G && !_kidnappingManager.IsKidnapping)
            {
                GTA.Ped closest = GTA.World.GetClosestPed(GTA.Game.Player.Character.Position, 2.0f);
                if (closest != null) _kidnappingManager.AttemptKidnap(closest);
            }
        }
    }
}
