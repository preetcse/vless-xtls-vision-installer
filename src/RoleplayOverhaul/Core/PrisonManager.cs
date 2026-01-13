using System;
using GTA;
using GTA.Math;

namespace RoleplayOverhaul.Core
{
    public class PrisonManager
    {
        public bool IsImprisoned { get; private set; }
        public int SentenceTimeRemaining { get; private set; } // Seconds

        // Bolingbroke Penitentiary Coords (Approx)
        private Vector3 _jailLocation = new Vector3(1850.5f, 2586.0f, 45.7f);

        public void Imprison(int seconds)
        {
            IsImprisoned = true;
            SentenceTimeRemaining = seconds;

            // Teleport
            if (GTA.Game.Player.Character != null)
            {
                GTA.Game.Player.Character.Position = _jailLocation;
                GTA.Game.Player.WantedLevel = 0; // Clear wanted
            }

            GTA.UI.Screen.ShowSubtitle($"IMPRISONED! Time remaining: {SentenceTimeRemaining}s");
        }

        public void Release()
        {
            IsImprisoned = false;
            SentenceTimeRemaining = 0;
            // Teleport to gate
            if (GTA.Game.Player.Character != null)
            {
                GTA.Game.Player.Character.Position = new Vector3(1846.0f, 2605.0f, 45.6f);
            }
            GTA.UI.Screen.ShowSubtitle("You have been released.");
        }

        public void OnTick()
        {
            if (!IsImprisoned) return;

            // Timer Logic
            if (GTA.Game.GameTime % 1000 == 0)
            {
                SentenceTimeRemaining--;
                if (SentenceTimeRemaining <= 0)
                {
                    Release();
                    return;
                }
            }

            // Keep in bounds logic
            if (GTA.Game.Player.Character.Position.DistanceTo(_jailLocation) > 200.0f)
            {
                 GTA.Game.Player.Character.Position = _jailLocation;
                 GTA.UI.Screen.ShowSubtitle("Trying to escape? Sentence extended!");
                 SentenceTimeRemaining += 30;
            }

            CheckPrisonJobs();
        }

        private void CheckPrisonJobs()
        {
            // Simple job locations in the yard
            Vector3 workoutPos = _jailLocation + new Vector3(10, 0, 0);
            Vector3 cleanPos = _jailLocation + new Vector3(-10, 0, 0);

            if (GTA.Game.Player.Character.Position.DistanceTo(workoutPos) < 2.0f)
            {
                 GTA.UI.Screen.ShowHelpText("Press E to Workout (-10s sentence)");
                 if (GTA.Game.IsControlJustPressed(GTA.Control.Context))
                 {
                     GTA.UI.Screen.FadeOut(500);
                     GTA.Wait(1000);
                     GTA.UI.Screen.FadeIn(500);
                     SentenceTimeRemaining = Math.Max(0, SentenceTimeRemaining - 10);
                     GTA.UI.Screen.ShowSubtitle("Worked out. Time reduced.");
                 }
            }
        }
    }
}
