using System;
using GTA;
using GTA.Math;
using RoleplayOverhaul.Core.Inventory; // New System
using RoleplayOverhaul.Police;

namespace RoleplayOverhaul.Missions
{
    public class FleecaBankHeist : MissionBase
    {
        private CrimeManager _crimeManager;
        private GridInventory _inventory; // New Dependency

        // Stages
        private bool _hasGetawayCar;
        private bool _hasDrill;

        // Coords
        private Vector3 _bankLocation = new Vector3(-351.0f, -49.0f, 49.0f); // Burton Fleeca

        public FleecaBankHeist(CrimeManager crime, GridInventory inventory)
            : base("The Fleeca Job", "Rob the Fleeca Bank on Burton.", 150000)
        {
            _crimeManager = crime;
            _inventory = inventory;
        }

        public override void OnTick()
        {
            switch (State)
            {
                case MissionState.Setup:
                    if (GTA.Game.Player.Character.IsInVehicle())
                    {
                        var veh = GTA.Game.Player.Character.CurrentVehicle;
                        if (veh.PassengerSeats >= 3)
                        {
                            _hasGetawayCar = true;
                            State = MissionState.Prep;
                            GTA.UI.Screen.ShowSubtitle("Setup Complete: Getaway Car Acquired. Now get a Drill.");
                        }
                    }
                    else
                    {
                         if (GTA.Game.GameTime % 5000 == 0)
                            GTA.UI.Screen.ShowHelpText("Steal a 4-door vehicle for the getaway.");
                    }
                    break;

                case MissionState.Prep:
                    // Use GridInventory Count
                    if (_inventory.GetItemCount("tool_drill") > 0)
                    {
                        _hasDrill = true;
                        State = MissionState.Finale;
                        GTA.UI.Screen.ShowSubtitle("Prep Complete. Go to the Bank.");
                    }
                    else
                    {
                         if (GTA.Game.GameTime % 5000 == 0)
                            GTA.UI.Screen.ShowHelpText("Acquire a Drill from the Hardware Store (Crafting).");
                    }
                    break;

                case MissionState.Finale:
                    float dist = Vector3.Distance(GTA.Game.Player.Character.Position, _bankLocation);
                    if (dist < 10.0f)
                    {
                        if (_crimeManager.WantedStars < 3) _crimeManager.ReportCrime(new Crime(CrimeType.ArmedRobbery, _bankLocation));

                        if (dist < 2.0f)
                        {
                            GTA.UI.Screen.ShowHelpText("Press ~INPUT_CONTEXT~ to Drill Vault");
                            if (GTA.Game.IsControlJustPressed(GTA.Control.Context))
                            {
                                // Remove Drill
                                _inventory.RemoveItem("tool_drill", 1);
                                GTA.UI.Notification.Show("Used Drill!");

                                GTA.Game.Player.Money += 150000;
                                Complete();
                            }
                        }
                    }
                    else
                    {
                         if (GTA.Game.GameTime % 5000 == 0)
                            GTA.UI.Screen.ShowSubtitle($"Go to Fleeca Bank: {dist:0}m");
                    }
                    break;
            }
        }
    }
}
