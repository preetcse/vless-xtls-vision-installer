using System;
using GTA;
using GTA.Math;
using System.Collections.Generic;

namespace RoleplayOverhaul.Activities
{
    public class ZombieSurvival : ActivityBase
    {
        private int _wave;
        private int _enemiesRemaining;
        private List<Ped> _zombies;
        private int _lastSpawn;

        public ZombieSurvival()
        {
            Name = "Zombie Survival";
            _zombies = new List<Ped>();
        }

        public override void Start()
        {
            base.Start();
            _wave = 1;
            StartWave();
        }

        private void StartWave()
        {
            _enemiesRemaining = _wave * 5; // Scaling difficulty
            GTA.UI.Screen.ShowSubtitle($"Wave {_wave} Started! Survive.");
        }

        public override void OnTick()
        {
            if (!IsActive) return;

            // Spawn Logic
            if (_enemiesRemaining > 0 && GTA.Game.GameTime - _lastSpawn > 2000)
            {
                SpawnZombie();
                _lastSpawn = GTA.Game.GameTime;
            }

            // Check Kills
            for (int i = _zombies.Count - 1; i >= 0; i--)
            {
                if (_zombies[i].IsDead)
                {
                    _zombies.RemoveAt(i);
                }
            }

            if (_enemiesRemaining <= 0 && _zombies.Count == 0)
            {
                _wave++;
                StartWave();
            }
        }

        private void SpawnZombie()
        {
            Vector3 pos = GTA.Game.Player.Character.Position + new Vector3(10, 0, 0); // Simplified random offset
            Ped z = World.CreatePed("u_m_y_zombie_01", pos); // Using a zombie model if available, or random
            if (z != null)
            {
                // Make them behave like zombies
                // Function.Call(Hash.SET_PED_MOVEMENT_CLIPSET, z, "move_m@drunk@verydrunk", 1.0f);
                z.Weapons.RemoveAll();
                z.Task.FightAgainst(GTA.Game.Player.Character);
                z.MaxHealth = 200;
                z.Health = 200;
                _zombies.Add(z);
                _enemiesRemaining--;
            }
        }

        protected override void OnEnd()
        {
            foreach(var z in _zombies) if (z.Exists()) z.Delete();
            _zombies.Clear();
        }
    }

    public class RiotMode : ActivityBase
    {
        public RiotMode() { Name = "Angry Peds (Riot)"; }

        public override void OnTick()
        {
            if (!IsActive) return;

            if (GTA.Game.GameTime % 1000 == 0)
            {
                // Simplified Riot Logic: Get nearby peds and make them mad
                // Note: SHVDN World.GetNearbyPeds needed
                /*
                var peds = World.GetNearbyPeds(Game.Player.Character.Position, 100.0f);
                foreach(var p in peds)
                {
                    if (!p.IsPlayer)
                    {
                        p.Weapons.Give(WeaponHash.Bat, 1, true, true);
                        p.Task.FightAgainst(Game.Player.Character);
                    }
                }
                */
                GTA.UI.Screen.ShowHelpText("Riot Mode Active: Everyone hates you!");
            }
        }
    }

    public class TimeStop : ActivityBase
    {
        public TimeStop() { Name = "Time Stop"; }

        public override void Start()
        {
            base.Start();
            GTA.Game.TimeScale = 0.0f; // Freeze
            // Note: Player might freeze too depending on engine, usually needs setEntityInvincible to time
        }

        public override void End()
        {
            base.End();
            GTA.Game.TimeScale = 1.0f;
        }

        public override void OnTick()
        {
            // Allow player movement if possible (requires advanced natives)
             GTA.UI.Screen.ShowHelpText("Time Stopped. Press Stop Activity to Resume.");
        }
    }
}
