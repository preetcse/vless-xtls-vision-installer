using System;
using GTA;
using RoleplayOverhaul.Core.Progression;

namespace RoleplayOverhaul.Jobs
{
    public interface IJob
    {
        string Name { get; }
        bool IsActive { get; }
        void Start();
        void End();
        void OnTick();
    }

    public abstract class JobBase : IJob
    {
        public string Name { get; protected set; }
        public bool IsActive { get; private set; }
        public ExperienceManager XPManager { get; set; } // Property Injection

        protected JobBase(string name)
        {
            Name = name;
        }

        public virtual void Start()
        {
            IsActive = true;
            GTA.UI.Screen.ShowSubtitle($"Started job: {Name}");
        }

        public virtual void End()
        {
            IsActive = false;
            GTA.UI.Screen.ShowSubtitle($"Ended job: {Name}");
            // Cleanup blips/entities here
        }

        public abstract void OnTick();

        protected void AwardXP(ExperienceManager.Skill skill, int amount)
        {
            if (XPManager != null)
            {
                XPManager.AddXP(skill, amount);
            }
        }
    }
}
