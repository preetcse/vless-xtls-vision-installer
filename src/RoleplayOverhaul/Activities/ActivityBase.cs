using System;
using GTA;
using GTA.Math;
using GTA.Native;
using System.Collections.Generic;

namespace RoleplayOverhaul.Activities
{
    public abstract class ActivityBase
    {
        public string Name { get; protected set; }
        public bool IsActive { get; private set; }

        public virtual void Start()
        {
            IsActive = true;
            GTA.UI.Screen.ShowSubtitle($"Activity Started: {Name}");
        }

        public virtual void End()
        {
            IsActive = false;
            GTA.UI.Screen.ShowSubtitle($"Activity Ended: {Name}");
            OnEnd();
        }

        public abstract void OnTick();
        protected virtual void OnEnd() { }
    }
}
