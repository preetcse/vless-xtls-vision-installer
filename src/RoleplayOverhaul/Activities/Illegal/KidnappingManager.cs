using System;
using GTA;
using GTA.Native;
using GTA.Math;
using RoleplayOverhaul.Core;

namespace RoleplayOverhaul.Activities.Illegal
{
    public class KidnappingManager
    {
        private Ped _victim;
        private bool _isDragging;
        private Entity _attachedVehicle;

        public bool IsKidnapping => _victim != null;

        public void OnTick()
        {
            if (_victim == null) return;

            if (_victim.IsDead)
            {
                ReleaseVictim();
                return;
            }

            // Dragging Logic
            if (_isDragging)
            {
                // Keep victim disabled
                Function.Call(Hash.SET_PED_TO_RAGDOLL, _victim, 1000, 1000, 0, false, false, false);

                // Controls
                if (Game.IsControlJustPressed(Control.Context)) // E to Release
                {
                    ReleaseVictim();
                }

                // Check for Vehicle interaction
                if (Game.IsControlJustPressed(Control.Attack)) // Left Click to put in car
                {
                    TryPutInVehicle();
                }
            }
        }

        public void AttemptKidnap(Ped target)
        {
            if (target.IsDead || target.IsPlayer) return;

            // Intimidation Check
            if (Game.Player.Character.Weapons.Current.Hash == WeaponHash.Unarmed)
            {
                GTA.UI.Notification.Show("You need a weapon to grapple!");
                return;
            }

            // Success
            _victim = target;
            _isDragging = true;

            _victim.Task.ClearAll();
            _victim.BlockPermanentEvents = true;

            // Attach to player (Grapple hold)
            Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, _victim, Game.Player.Character,
                11816, // Bone ID (SKEL_L_Hand)
                0.45f, 0.45f, 0.0f, // Pos
                0.0f, 0.0f, 0.0f,   // Rot
                false, false, false, false, 2, true);

            // Play Anim
            Game.Player.Character.Task.PlayAnimation("missminuteman_1ig_2", "handsup_enter", 8.0f, -1, AnimationFlags.UpperBodyOnly | AnimationFlags.AllowRotation);
            _victim.Task.PlayAnimation("random@arrests@busted", "idle_a", 8.0f, -1, AnimationFlags.Loop);

            GTA.UI.Notification.Show("Victim Grappled! [LMB] Put in Car, [E] Release.");
        }

        public void ReleaseVictim()
        {
            if (_victim != null)
            {
                _victim.Detach();
                _victim.Task.ClearAll();
                _victim.Task.ReactAndFlee(Game.Player.Character);
                _victim.BlockPermanentEvents = false;
                _victim = null;
                _isDragging = false;
                GTA.UI.Notification.Show("Victim released.");
            }
        }

        private void TryPutInVehicle()
        {
            Vehicle nearbyVehicle = World.GetClosestVehicle(Game.Player.Character.Position, 5.0f);
            if (nearbyVehicle != null)
            {
                // Detach from player
                _victim.Detach();

                // Put in Trunk if available, else back seat
                if (nearbyVehicle.HasBone("boot")) // Check for trunk
                {
                    // Open trunk
                    nearbyVehicle.Doors[VehicleDoorIndex.Trunk].Open();

                    // Attach to trunk
                    // Note: This requires specific offsets per vehicle usually, using generic for now
                    Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, _victim, nearbyVehicle,
                        nearbyVehicle.Bones["boot"].Index,
                        0.0f, -0.5f, 0.0f,
                        0.0f, 0.0f, 0.0f,
                        false, false, false, false, 2, true);

                    _victim.Task.PlayAnimation("fin_ext_p1-7", "cs_lisa_trunk_0", 8.0f, -1, AnimationFlags.Loop);
                    GTA.UI.Notification.Show("Victim stashed in trunk.");
                }
                else if (nearbyVehicle.IsSeatFree(VehicleSeat.Driver)) // Just force into seat
                {
                    _victim.Task.WarpIntoVehicle(nearbyVehicle, VehicleSeat.Passenger);
                     GTA.UI.Notification.Show("Victim forced into seat.");
                }

                _isDragging = false;
                // _victim remains set so we know we have a prisoner
            }
            else
            {
                GTA.UI.Notification.Show("No vehicle nearby!");
            }
        }
    }
}
