using System;
using System.Collections.Generic;

namespace RoleplayOverhaul.Jobs
{
    public static class JobLibrary
    {
        public static List<IJob> CreateAllJobs(Police.CrimeManager crimeManager)
        {
             var jobs = new List<IJob>();

             // 1. Delivery & Transport (Now using DeliveryJob)
            jobs.Add(new DeliveryJob("Pizza Delivery", "pizzaboy"));
            jobs.Add(new DeliveryJob("Courier", "boxville"));
            jobs.Add(new DeliveryJob("Trucker", "phantom"));
            jobs.Add(new TaxiJob());
            jobs.Add(new DeliveryJob("Bus Driver", "bus"));
            jobs.Add(new GarbageJob());
            jobs.Add(new DeliveryJob("PostOp Driver", "postop"));
            jobs.Add(new DeliveryJob("Armored Truck", "stockade"));
            jobs.Add(new TowTruckJob());
            jobs.Add(new DeliveryJob("Forklift Operator", "forklift"));

            // 2. Emergency Services
            jobs.Add(new ParamedicJob());
            jobs.Add(new FirefighterJob());
            jobs.Add(new PoliceJob());
            jobs.Add(new SimpleJob("Coast Guard", "Patrol the waters.", "predator"));
            jobs.Add(new SimpleJob("Lifeguard", "Watch over the beach.", "lguard"));

            // 3. Manual Labor & Harvesting
            jobs.Add(new DeliveryJob("Miner", "rubble")); // Drive ore
            jobs.Add(new DeliveryJob("Lumberjack", "log"));
            jobs.Add(new DeliveryJob("Farmer", "tractor"));
            jobs.Add(new DeliveryJob("Fisherman", "tug"));
            jobs.Add(new DeliveryJob("Construction Worker", "mixer"));
            jobs.Add(new DeliveryJob("Oil Tycoon", "tanker"));
            jobs.Add(new DeliveryJob("Gardener", "mower"));

            // 4. Illegal / Underground
            jobs.Add(new IllegalJob("Drug Dealer", "Sell product on corners.", "none", crimeManager));
            jobs.Add(new IllegalJob("Car Thief", "Steal requested vehicles.", "none", crimeManager));
            jobs.Add(new IllegalJob("Hitman", "Eliminate high-value targets.", "none", crimeManager));
            jobs.Add(new IllegalJob("Smuggler", "Move contraband by plane.", "velum", crimeManager));
            jobs.Add(new IllegalJob("Arms Dealer", "Supply gangs with weapons.", "speedo", crimeManager));

            // 5. Service Industry
            jobs.Add(new SimpleJob("Mechanic", "Repair player vehicles.", "flatbed"));
            jobs.Add(new SimpleJob("Reporter", "Film news events.", "newsvan"));
            jobs.Add(new SimpleJob("Flight Instructor", "Teach others to fly.", "duster"));

            return jobs;
        }
    }
}
