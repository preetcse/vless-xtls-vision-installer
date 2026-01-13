using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;

namespace RoleplayOverhaul.Activities
{
    public class ActivityManager
    {
        private List<ActivityBase> _activities;
        public ActivityBase CurrentActivity { get; private set; }

        public ActivityManager()
        {
            _activities = new List<ActivityBase>();
            _activities.Add(new ZombieSurvival());
            _activities.Add(new RiotMode());
            _activities.Add(new TimeStop());
        }

        public void StartActivity(string name)
        {
            if (CurrentActivity != null)
            {
                GTA.UI.Screen.ShowSubtitle("End current activity first!");
                return;
            }

            var act = _activities.Find(a => a.Name.Contains(name));
            if (act != null)
            {
                CurrentActivity = act;
                CurrentActivity.Start();
            }
        }

        public void StopCurrentActivity()
        {
            if (CurrentActivity != null)
            {
                CurrentActivity.End();
                CurrentActivity = null;
            }
        }

        public void OnTick()
        {
            if (CurrentActivity != null)
            {
                CurrentActivity.OnTick();
            }
        }
    }
}
