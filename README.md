# Roleplay Overhaul Mod

A comprehensive Single Player Roleplay mod for GTA V, built using ScriptHookVDotNet.

## Features

### 🎒 Grid-Based Inventory
- Press **F5** to open your backpack.
- Visual grid interface for managing items.
- Supports Food, Licenses, and specialized items.

### 💼 30+ Unique Jobs
Experience a variety of careers including:
- **Delivery:** Pizza Boy, Courier, Trucker, PostOp...
- **Emergency:** Police, Paramedic, Firefighter, Coast Guard...
- **Manual Labor:** Miner, Lumberjack, Farmer, Fisherman...
- **Underground:** Drug Dealer, Car Thief, Hitman...
- **Services:** Taxi, Bus Driver, Mechanic, Reporter...

### 🪪 Advanced Licensing System
- **Licenses:** Driver, Weapon, Hunting, Flying, Commercial.
- **Health Insurance:** Mandatory for cheaper hospital bills.
- **Testing:** Pass practical exams to earn your licenses.
- **Expiry:** Licenses and insurance expire every 30 in-game days.

## Installation

1. Ensure you have **ScriptHookV** and **ScriptHookVDotNet** installed.
2. Compile this project or copy the source files into your `GTA V/scripts/` directory.
   > **Note:** If compiling manually in Visual Studio, delete `src/Dependencies/SHVDN_Stubs.cs` as it is only for development without the game installed.
3. Launch the game.

## Controls
- **F5**: Open/Close Inventory
- **F6**: Start Driving Test (Debug)
- **E**: Interact / Click Item in Inventory

## Developer Notes
This mod is structured to be modular.
- `Core/`: Manages global systems like Licenses.
- `Items/`: Defines item properties and inventory logic.
- `Jobs/`: Contains the logic for all 30+ jobs.
- `UI/`: Handles the custom drawing of the grid menu.
