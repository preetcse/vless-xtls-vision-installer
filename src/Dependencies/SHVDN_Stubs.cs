using System;
using System.Collections.Generic;

namespace GTA
{
    public class Script
    {
        public event EventHandler Tick;
        public event System.Windows.Forms.KeyEventHandler KeyDown;
        public event System.Windows.Forms.KeyEventHandler KeyUp;
        public int Interval { get; set; }
    }

    public static class Game
    {
        public static Player Player { get; set; }
        public static void Pause(bool value) { }
        public static bool IsPaused { get; set; }
        public static float LastFrameTime { get; set; }
        public static int GameTime { get; set; }
    }

    public class Entity
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public float Heading { get; set; }
        public bool IsVisible { get; set; }
        public void Delete() { }
        public bool Exists() { return true; }
        public int Handle { get; set; }
    }

    public class Ped : Entity
    {
        public Vehicle CurrentVehicle { get; set; }
        public bool IsInVehicle() { return false; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Armor { get; set; }
        public WeaponCollection Weapons { get; set; }
        public TaskInvoker Task { get; set; }
        public bool IsDead { get; set; }
        public void ClearTasks() { }
    }

    public class Player
    {
        public Ped Character { get; set; }
        public int Money { get; set; }
        public bool CanControlCharacter { get; set; }
        public int WantedLevel { get; set; }
    }

    public class Vehicle : Entity
    {
        public float Speed { get; set; }
        public float EngineHealth { get; set; }
        public float BodyHealth { get; set; }
        public bool IsEngineRunning { get; set; }
        public void Repair() { }
    }

    public class WeaponCollection
    {
        public void Give(WeaponHash hash, int ammo, bool equip, bool isAmmoLoaded) { }
        public void Select(WeaponHash hash) { }
        public bool HasWeapon(WeaponHash hash) { return false; }
    }

    public class TaskInvoker
    {
        public void FightAgainst(Ped target) { }
        public void FleeFrom(Ped target) { }
        public void WanderAround() { }
        public void EnterVehicle(Vehicle vehicle, VehicleSeat seat) { }
    }

    public enum VehicleSeat { Driver, Passenger }
    public enum WeaponHash { Unarmed, Pistol, AssaultRifle }

    namespace Math
    {
        public struct Vector3
        {
            public float X, Y, Z;
            public Vector3(float x, float y, float z) { X = x; Y = y; Z = z; }
            public static Vector3 Zero = new Vector3(0, 0, 0);
            public float DistanceTo(Vector3 other) { return 0f; }
            public static Vector3 operator +(Vector3 a, Vector3 b) { return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }
            public static Vector3 operator *(Vector3 a, float d) { return new Vector3(a.X * d, a.Y * d, a.Z * d); }
        }
    }

    namespace UI
    {
        public enum Font { ChakraPetch, HouseScript, Monospace }
        public enum Alignment { Center, Left, Right }

        public class TextElement
        {
            public TextElement(string caption, System.Drawing.PointF position, float scale) { }
            public TextElement(string caption, System.Drawing.PointF position, float scale, System.Drawing.Color color) { }
            public System.Drawing.Color Color { get; set; }
            public bool Enabled { get; set; }
            public void Draw() { }
        }

        public class ContainerElement
        {
             public System.Drawing.PointF Position { get; set; }
             public System.Drawing.SizeF Size { get; set; }
             public bool Enabled { get; set; }
             public System.Drawing.Color Color { get; set; }
             public void Draw() { }
        }

        public static class Screen
        {
            public static float Width { get; } = 1920;
            public static float Height { get; } = 1080;
            public static void ShowSubtitle(string message, int duration = 2500) { }
            public static void ShowHelpText(string message, int duration = -1, bool beep = true, bool looped = false) { }
        }
    }

    namespace Native
    {
        public static class Function
        {
            public static void Call(Hash hash, params object[] arguments) { }
            public static T Call<T>(Hash hash, params object[] arguments) { return default(T); }
        }
        public enum Hash { GET_PLAYER_PED, SET_ENTITY_COORDS } // Add hashes as needed
    }
}
