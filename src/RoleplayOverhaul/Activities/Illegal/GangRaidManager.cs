using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;

namespace RoleplayOverhaul.Activities.Illegal
{
    public class GangRaidManager
    {
        private bool _isActive;
        private int _stage;
        private Vector3 _location;
        private List<Ped> _enemies = new List<Ped>();
        private List<Prop> _lootCrates = new List<Prop>();
        private Blip _missionBlip;
        private Ped _boss;

        public void StartRaid(Vector3 location, string factionName)
        {
            if (_isActive) return;

            _isActive = true;
            _location = location;
            _stage = 1;

            _missionBlip = World.CreateBlip(_location, 60f);
            _missionBlip.Color = BlipColor.Red;
            _missionBlip.Name = $"Raid: {factionName}";

            SpawnStage1_Guards();
            GTA.UI.Notification.Show($"RAID STARTED: Eliminate the {factionName} guards!");
        }

        public void OnTick()
        {
            if (!_isActive) return;

            // Cleanup dead enemies from list
            _enemies.RemoveAll(p => !p.Exists());

            switch (_stage)
            {
                case 1: // Guards
                    if (CountAliveEnemies() <= 1)
                    {
                        GTA.UI.Notification.Show("Perimeter Clear! Breach the hideout!");
                        _stage = 2;
                        SpawnStage2_Stash();
                    }
                    break;
                case 2: // Stash & Boss
                    if (_boss != null && _boss.IsDead)
                    {
                        GTA.UI.Notification.Show("Boss Defeated! Loot the crates!");
                        _stage = 3;
                    }
                    break;
                case 3: // Looting
                    CheckLooting();
                    break;
            }
        }

        private void SpawnStage1_Guards()
        {
            for (int i = 0; i < 6; i++)
            {
                Vector3 spawnPos = _location + Vector3.RandomXY() * 15;
                Ped guard = World.CreatePed(PedHash.Ballasog, spawnPos);
                guard.Weapons.Give(WeaponHash.Pistol, 500, true, true);
                guard.Task.FightAgainst(Game.Player.Character);
                guard.Armor = 20;
                _enemies.Add(guard);
            }
        }

        private void SpawnStage2_Stash()
        {
            // Spawn Crates
            Prop crate = World.CreateProp("prop_box_wood02a_pu", _location, true, false);
            _lootCrates.Add(crate);
            World.CreateBlip(crate.Position).Color = BlipColor.Green; // Loot blip

            // Spawn Boss
            _boss = World.CreatePed(PedHash.G, _location + new Vector3(2, 0, 0));
            _boss.Weapons.Give(WeaponHash.AssaultRifle, 500, true, true);
            _boss.Health = 500; // Tanky
            _boss.Armor = 100;
            _boss.Task.FightAgainst(Game.Player.Character);
            _enemies.Add(_boss);

            GTA.UI.Notification.Show("The Boss has appeared! Take him down.");
        }

        private void CheckLooting()
        {
            foreach (var crate in _lootCrates)
            {
                // Fixed World.GetDistance usage to Vector3.Distance
                if (crate.Exists() && Vector3.Distance(Game.Player.Character.Position, crate.Position) < 2.0f)
                {
                    GTA.UI.Screen.ShowHelpText("Press ~INPUT_CONTEXT~ to Loot Stash");

                    if (Game.IsControlPressed(Control.Context))
                    {
                        Game.Player.Character.Task.PlayAnimation("amb@medic@standing@kneel@base", "base", 8.0f, -1, AnimationFlags.None);
                        GTA.UI.Notification.Show("Looting...");
                        // Should add a timer here but for now instant
                        crate.Delete();
                        Game.Player.Money += 5000;
                        Game.Player.Character.Task.ClearAll();
                        GTA.UI.Notification.Show("Loot Secured! +$5000");

                        EndRaid();
                        return;
                    }
                }
            }
        }

        private int CountAliveEnemies()
        {
            int count = 0;
            foreach (var p in _enemies)
            {
                if (p.IsAlive) count++;
            }
            return count;
        }

        private void EndRaid()
        {
            _isActive = false;
            if (_missionBlip != null) _missionBlip.Delete();
            foreach (var p in _enemies) if (p.Exists()) p.Delete(); // Cleanup remaining
            _enemies.Clear();
            _lootCrates.Clear();
        }
    }
}
