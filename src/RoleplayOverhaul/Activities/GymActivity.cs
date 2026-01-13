using System;
using GTA;
using GTA.Math;
using GTA.UI;
using RoleplayOverhaul.Core.Progression;

namespace RoleplayOverhaul.Activities
{
    public class GymActivity
    {
        private ExperienceManager _xpManager;
        private Vector3 _gymLocation = new Vector3(-1203.4f, -1570.6f, 4.6f); // Vespucci Beach Weights
        private bool _isExercising = false;
        private float _reps = 0;
        private float _stamina = 100f;

        public GymActivity(ExperienceManager xpManager)
        {
            _xpManager = xpManager;
            // Create Blip
            var blip = World.CreateBlip(_gymLocation);
            blip.Sprite = BlipSprite.Muscle;
            blip.Color = BlipColor.Yellow;
            blip.Name = "Gym";
            blip.ShortRange = true;
        }

        public void OnTick()
        {
            float dist = Vector3.Distance(Game.Player.Character.Position, _gymLocation);

            if (!_isExercising)
            {
                if (dist < 2.0f)
                {
                    Screen.ShowHelpText("Press ~INPUT_CONTEXT~ to Workout");
                    if (Game.IsControlJustPressed(Control.Context))
                    {
                        StartWorkout();
                    }
                }
            }
            else
            {
                WorkoutLoop();
            }
        }

        private void StartWorkout()
        {
            _isExercising = true;
            _reps = 0;
            _stamina = 100f;

            // Align player
            Game.Player.Character.Position = _gymLocation;
            Game.Player.Character.Rotation = new Vector3(0, 0, 125);
            Game.Player.Character.Task.PlayAnimation("amb@world_human_muscle_free_weights@male@idle_a", "idle_a", 8.0f, -1, AnimationFlags.Loop);

            Screen.ShowSubtitle("Mash ~g~[SPACE]~w~ to lift! Don't run out of Stamina!");
        }

        private void WorkoutLoop()
        {
            // HUD
            new TextElement($"Reps: {(int)_reps}", new System.Drawing.PointF(100, 100), 0.5f).Draw();
            new TextElement($"Stamina: {(int)_stamina}%", new System.Drawing.PointF(100, 130), 0.5f, System.Drawing.Color.Cyan).Draw();

            // Input
            if (Game.IsControlJustPressed(Control.Jump)) // Space
            {
                _reps += 1;
                _stamina -= 5f;
                Game.Player.Character.Task.PlayAnimation("amb@world_human_muscle_free_weights@male@idle_a", "idle_b", 8.0f, 1000, AnimationFlags.Loop); // Curl anim
            }

            // Stamina regen
            _stamina = Math.Min(100, _stamina + 0.1f);

            // Exit conditions
            if (_stamina <= 0)
            {
                FinishWorkout(false);
            }

            if (_reps >= 10)
            {
                FinishWorkout(true);
            }

            // Cancel
            if (Game.IsControlJustPressed(Control.VehicleExit))
            {
                FinishWorkout(false);
            }
        }

        private void FinishWorkout(bool success)
        {
            _isExercising = false;
            Game.Player.Character.Task.ClearAll();

            if (success)
            {
                Screen.ShowSubtitle("Good Set! +50 Strength XP");
                _xpManager.AddXP(ExperienceManager.Skill.Strength, 50);
            }
            else
            {
                Screen.ShowSubtitle("You gave up. Weak.");
            }
        }
    }
}
