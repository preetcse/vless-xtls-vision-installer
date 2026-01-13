using System;
using System.Collections.Generic;

namespace RoleplayOverhaul.Jobs
{
    public static class JobLibrary
    {
        public static List<IJob> CreateAllJobs()
        {
            var jobs = new List<IJob>();

            // 1. Delivery & Transport
            jobs.Add(new SimpleJob("Pizza Delivery", "Deliver pizzas to customers.", "pizzaboy"));
            jobs.Add(new SimpleJob("Courier", "Deliver packages around the city.", "boxville"));
            jobs.Add(new SimpleJob("Trucker", "Haul cargo across the state.", "phantom"));
            jobs.Add(new SimpleJob("Taxi Driver", "Ferry passengers to their destinations.", "taxi"));
            jobs.Add(new SimpleJob("Bus Driver", "Follow the route and pick up passengers.", "bus"));
            jobs.Add(new SimpleJob("Trash Collector", "Clean up the city streets.", "trash"));
            jobs.Add(new SimpleJob("PostOp Driver", "Deliver mail.", "postop"));
            jobs.Add(new SimpleJob("Armored Truck", "Transport cash securely.", "stockade"));
            jobs.Add(new SimpleJob("Tow Truck", "Impound illegally parked cars.", "towtruck"));
            jobs.Add(new SimpleJob("Forklift Operator", "Move crates at the docks.", "forklift"));

            // 2. Emergency Services
            jobs.Add(new SimpleJob("Paramedic", "Save injured civilians.", "ambulance"));
            jobs.Add(new SimpleJob("Firefighter", "Put out fires.", "firetruck"));
            jobs.Add(new SimpleJob("Police Officer", "Patrol and arrest criminals.", "police"));
            jobs.Add(new SimpleJob("Coast Guard", "Patrol the waters.", "predator"));
            jobs.Add(new SimpleJob("Lifeguard", "Watch over the beach.", "lguard"));

            // 3. Manual Labor & Harvesting
            jobs.Add(new SimpleJob("Miner", "Mine for ore in the quarry.", "rubble"));
            jobs.Add(new SimpleJob("Lumberjack", "Cut down trees in Paleto.", "log"));
            jobs.Add(new SimpleJob("Farmer", "Harvest crops.", "tractor"));
            jobs.Add(new SimpleJob("Fisherman", "Catch fish at sea.", "tug"));
            jobs.Add(new SimpleJob("Construction Worker", "Work on building sites.", "mixer"));
            jobs.Add(new SimpleJob("Oil Tycoon", "Maintain oil pumps.", "tanker"));
            jobs.Add(new SimpleJob("Gardener", "Tend to lawns in Vinewood.", "mower"));

            // 4. Illegal / Underground
            jobs.Add(new SimpleJob("Drug Dealer", "Sell product on corners.", "none"));
            jobs.Add(new SimpleJob("Car Thief", "Steal requested vehicles.", "none"));
            jobs.Add(new SimpleJob("Hitman", "Eliminate high-value targets.", "none"));
            jobs.Add(new SimpleJob("Smuggler", "Move contraband by plane.", "velum"));
            jobs.Add(new SimpleJob("Arms Dealer", "Supply gangs with weapons.", "speedo"));

            // 5. Service Industry
            jobs.Add(new SimpleJob("Mechanic", "Repair player vehicles.", "flatbed"));
            jobs.Add(new SimpleJob("Reporter", "Film news events.", "newsvan"));
            jobs.Add(new SimpleJob("Flight Instructor", "Teach others to fly.", "duster"));

            return jobs;
        }
    }

    public class SimpleJob : JobBase
    {
        public string Description { get; private set; }
        public string VehicleModel { get; private set; }

        public SimpleJob(string name, string description, string vehicle) : base(name)
        {
            Description = description;
            VehicleModel = vehicle;
        }

        public override void Start()
        {
            base.Start();
            GTA.UI.Screen.ShowSubtitle($"Go to the depot to start {Name}: {Description}");
            // In a real implementation, this would spawn the vehicle
        }

        public override void OnTick()
        {
            // Simple tick logic
            if (IsActive && GTA.Game.GameTime % 10000 == 0)
            {
                // Pay player periodically for "working"
                GTA.Game.Player.Money += 50;
                GTA.UI.Screen.ShowSubtitle($"Paid $50 for {Name} work.");
            }
        }
    }
}
