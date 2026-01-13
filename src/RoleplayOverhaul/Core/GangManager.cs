using System;
using System.Collections.Generic;
using GTA;

namespace RoleplayOverhaul.Core
{
    public enum GangFaction
    {
        Families,
        Ballas,
        Vagos,
        LostMC
    }

    public class GangManager
    {
        private Dictionary<GangFaction, int> _reputation;

        public GangManager()
        {
            _reputation = new Dictionary<GangFaction, int>
            {
                { GangFaction.Families, 0 },
                { GangFaction.Ballas, -50 }, // Hostile start
                { GangFaction.Vagos, -10 },
                { GangFaction.LostMC, -20 }
            };
        }

        public int GetReputation(GangFaction gang)
        {
            return _reputation.ContainsKey(gang) ? _reputation[gang] : 0;
        }

        public void ChangeReputation(GangFaction gang, int amount)
        {
            if (_reputation.ContainsKey(gang))
            {
                _reputation[gang] += amount;
                // Clamp -100 to 100
                if (_reputation[gang] > 100) _reputation[gang] = 100;
                if (_reputation[gang] < -100) _reputation[gang] = -100;

                GTA.UI.Screen.ShowSubtitle($"Reputation with {gang}: {_reputation[gang]}");
            }
        }

        public void CheckTerritory()
        {
            // In a real mod, we check World.GetZoneName(PlayerPos)
            // Mock:
            /*
            string zone = "DAVIS"; // Example
            if (zone == "DAVIS" && GetReputation(GangFaction.Ballas) < -50)
            {
                // Spawn enemy gang members
            }
            */
        }
    }
}
