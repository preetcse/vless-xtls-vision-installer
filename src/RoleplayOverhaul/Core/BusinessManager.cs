using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using RoleplayOverhaul.Banking;

namespace RoleplayOverhaul.Core
{
    public class Business
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public int Stock { get; set; } // 0-100
        public int IncomePerTick { get; set; }
        public bool IsOwned { get; set; }

        public Business(string name, int price, int income)
        {
            Name = name;
            Price = price;
            IncomePerTick = income;
            Stock = 0;
            IsOwned = false;
        }
    }

    public class BusinessManager
    {
        private BankingManager _bank;
        private List<Business> _businesses;
        private int _lastIncomeTime;

        public BusinessManager(BankingManager bank)
        {
            _bank = bank;
            _businesses = new List<Business>();
            LoadBusinesses();
        }

        private void LoadBusinesses()
        {
            _businesses.Add(new Business("Vanilla Unicorn Nightclub", 500000, 2000)); // Passive income high
            _businesses.Add(new Business("Smoke on the Water (Weed)", 250000, 1000));
            _businesses.Add(new Business("Los Santos Customs", 1000000, 5000));
            _businesses.Add(new Business("McKenzie Field Hangar", 150000, 500));
        }

        public void BuyBusiness(string name)
        {
            var biz = _businesses.Find(b => b.Name == name);
            if (biz != null && !biz.IsOwned)
            {
                if (_bank.Withdraw(biz.Price, $"Business Purchase: {name}"))
                {
                    biz.IsOwned = true;
                    biz.Stock = 50; // Starter stock
                    GTA.UI.Screen.ShowSubtitle($"Congratulations! You now own {name}.");
                }
                else
                {
                    GTA.UI.Screen.ShowSubtitle("Insufficient funds to buy business.");
                }
            }
        }

        public void StartResupply(string name)
        {
            var biz = _businesses.Find(b => b.Name == name);
            if (biz != null && biz.IsOwned)
            {
                // Logic to start a DeliveryJob mission targeting the business
                // For now, mock supply
                biz.Stock = Math.Min(100, biz.Stock + 25);
                GTA.UI.Screen.ShowSubtitle($"Resupplied {name}. Stock: {biz.Stock}%");
            }
        }

        public void OnTick()
        {
            // Passive Income (Daily or Hourly)
            // Mock: Every minute for demo
            if (GTA.Game.GameTime - _lastIncomeTime > 60000)
            {
                int totalIncome = 0;
                foreach(var biz in _businesses)
                {
                    if (biz.IsOwned && biz.Stock > 0)
                    {
                        totalIncome += biz.IncomePerTick;
                        biz.Stock = Math.Max(0, biz.Stock - 5); // Consumes stock
                    }
                }

                if (totalIncome > 0)
                {
                    _bank.Deposit(totalIncome, "Business Income");
                    GTA.UI.Screen.ShowSubtitle($"Business Profits: ${totalIncome}");
                }

                _lastIncomeTime = GTA.Game.GameTime;
            }
        }

        public List<Business> GetAvailableBusinesses()
        {
            return _businesses;
        }
    }
}
