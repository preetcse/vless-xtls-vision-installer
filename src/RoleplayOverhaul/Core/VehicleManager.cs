using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using RoleplayOverhaul.Banking; // Payment logic

namespace RoleplayOverhaul.Core
{
    public class OwnedVehicle
    {
        public string ModelName { get; set; }
        public string Plate { get; set; }
        public float FuelLevel { get; set; }
        public bool IsImpounded { get; set; }

        public OwnedVehicle(string model, string plate)
        {
            ModelName = model;
            Plate = plate;
            FuelLevel = 100.0f;
            IsImpounded = false;
        }
    }

    public class VehicleManager
    {
        private List<OwnedVehicle> _myVehicles;
        private BankingManager _bank;

        // Logic for Fuel
        private Vehicle _lastVehicle;

        public VehicleManager(BankingManager bank)
        {
            _bank = bank;
            _myVehicles = new List<OwnedVehicle>();
            // Starter car
            _myVehicles.Add(new OwnedVehicle("asea", "STARTER"));
        }

        public void BuyVehicle(string model, int price)
        {
            if (_bank.Withdraw(price, $"Vehicle Purchase: {model}"))
            {
                _myVehicles.Add(new OwnedVehicle(model, "NEWCAR"));
                GTA.UI.Screen.ShowSubtitle($"Purchased {model}!");
            }
            else
            {
                GTA.UI.Screen.ShowSubtitle("Insufficient funds for vehicle!");
            }
        }

        public void SpawnPersonalVehicle(int index)
        {
            if (index >= 0 && index < _myVehicles.Count)
            {
                var vData = _myVehicles[index];
                Vector3 pos = GTA.Game.Player.Character.Position + new Vector3(5, 0, 0);
                var v = World.CreateVehicle(vData.ModelName, pos);
                if (v != null)
                {
                    // v.Mods.LicensePlate = vData.Plate;
                    GTA.UI.Screen.ShowSubtitle($"Personal Vehicle {vData.ModelName} delivered.");
                }
            }
        }

        public void OnTick()
        {
            // Fuel Logic
            if (GTA.Game.Player.Character.IsInVehicle())
            {
                var veh = GTA.Game.Player.Character.CurrentVehicle;
                if (veh != null && veh.IsEngineRunning)
                {
                    // Consume fuel
                    // Simplified: We don't link this to the specific OwnedVehicle instance in this prototype loop
                    // In a real mod, we'd map Vehicle.Handle -> OwnedVehicle

                    // Logic mostly visual for UI unless we stop engine
                }
            }
        }
    }
}
