using System;
using System.Collections.Generic;
using GTA;

namespace RoleplayOverhaul.Banking
{
    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        Salary,
        Bill,
        Transfer,
        Purchase,
        Fine
    }

    public class Transaction
    {
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public int BalanceAfter { get; set; }

        public Transaction(TransactionType type, int amount, string desc, int balanceAfter)
        {
            Date = DateTime.Now; // In real mod, use World.CurrentDate
            Type = type;
            Amount = amount;
            Description = desc;
            BalanceAfter = balanceAfter;
        }
    }

    public class BankingManager
    {
        public int Balance { get; private set; }
        public int Debt { get; private set; }
        public List<Transaction> History { get; private set; }

        // Settings
        public bool AutoBankIncome { get; set; } = true;
        private int _lastCash;

        public BankingManager()
        {
            Balance = 5000; // Starter money
            Debt = 0;
            History = new List<Transaction>();
            _lastCash = 0; // Will sync on first tick
        }

        public void OnTick()
        {
            // AutoBank Logic
            if (AutoBankIncome)
            {
                int currentCash = GTA.Game.Player.Money;
                int diff = currentCash - _lastCash;

                if (diff > 0)
                {
                    // Income detected
                    // Determine if this is "Job" money or random pickup
                    // For now, we route 50% to bank automatically as a feature example
                    int toBank = diff / 2;
                    if (toBank > 0)
                    {
                        GTA.Game.Player.Money -= toBank;
                        Deposit(toBank, "Auto-Deposit");
                        currentCash -= toBank; // Update local tracker
                        GTA.UI.Screen.ShowSubtitle($"Auto-Banked ${toBank}");
                    }
                }
                _lastCash = currentCash;
            }
        }

        public void Deposit(int amount, string reason = "Deposit")
        {
            Balance += amount;
            RecordTransaction(TransactionType.Deposit, amount, reason);
            Diagnostics.Logger.Info($"Deposit: {amount} ({reason})");
        }

        public bool Withdraw(int amount, string reason = "Withdrawal")
        {
            if (Balance >= amount)
            {
                Balance -= amount;
                RecordTransaction(TransactionType.Withdrawal, -amount, reason);
                Diagnostics.Logger.Info($"Withdraw: {amount} ({reason})");
                return true;
            }
            Diagnostics.Logger.Info($"Withdraw Failed: {amount} ({reason}) - Insufficient Funds");
            return false;
        }

        public void AddDebt(int amount, string reason)
        {
            Debt += amount;
            RecordTransaction(TransactionType.Bill, 0, $"Missed Bill: {reason} (Added to Debt)", Balance);
            GTA.UI.Screen.ShowSubtitle($"Warning: Debt increased by ${amount}!", 3000);
        }

        public bool PayDebt(int amount)
        {
            if (Balance >= amount)
            {
                Balance -= amount;
                Debt = Math.Max(0, Debt - amount);
                RecordTransaction(TransactionType.Bill, -amount, "Debt Payment");
                return true;
            }
            return false;
        }

        private void RecordTransaction(TransactionType type, int amount, string desc, int? balanceOverride = null)
        {
            History.Add(new Transaction(type, amount, desc, balanceOverride ?? Balance));
            if (History.Count > 50) History.RemoveAt(0); // Keep log manageable
        }

        public void ProcessSalary(int amount, string jobName)
        {
            // Salary goes directly to bank (RealBank feature)
            Balance += amount;
            RecordTransaction(TransactionType.Salary, amount, $"Salary: {jobName}");
            GTA.UI.Screen.ShowSubtitle($"Salary Received: ${amount}");
        }
    }
}
