using System;
using System.Collections.Generic;

namespace RoleplayOverhaul.Core
{
    public enum CareerPath
    {
        Civilian,
        Police,
        EMS,
        Criminal
    }

    public class CareerManager
    {
        private Dictionary<CareerPath, int> _xp;
        private Dictionary<CareerPath, int> _rank;

        public CareerManager()
        {
            _xp = new Dictionary<CareerPath, int>();
            _rank = new Dictionary<CareerPath, int>();

            foreach(CareerPath p in Enum.GetValues(typeof(CareerPath)))
            {
                _xp[p] = 0;
                _rank[p] = 1;
            }
        }

        public void AddXP(CareerPath path, int amount)
        {
            _xp[path] += amount;
            CheckRankUp(path);
        }

        private void CheckRankUp(CareerPath path)
        {
            int required = _rank[path] * 1000;
            if (_xp[path] >= required)
            {
                _rank[path]++;
                _xp[path] -= required;
                GTA.UI.Screen.ShowSubtitle($"PROMOTED! {path} Rank {_rank[path]} reached!");
                UnlockPerks(path, _rank[path]);
            }
        }

        private void UnlockPerks(CareerPath path, int rank)
        {
            // Logic to unlock vehicles/uniforms
            if (path == CareerPath.Police && rank == 2)
            {
                GTA.UI.Screen.ShowSubtitle("Unlocked: Police Cruiser (Scout)");
            }
        }

        public string GetStatus()
        {
            return $"Pol: {_rank[CareerPath.Police]} | EMS: {_rank[CareerPath.EMS]} | Crim: {_rank[CareerPath.Criminal]}";
        }
    }
}
