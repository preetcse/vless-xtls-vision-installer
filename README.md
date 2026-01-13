# Roleplay Overhaul Mod
+
+A comprehensive Single Player Roleplay mod for GTA V, built using ScriptHookVDotNet.
+
+## Features
+
+### 🎒 Grid-Based Inventory
+- Press **F5** to open your backpack.
+- Visual grid interface for managing items.
+- Supports Food, Licenses, and specialized items.
+- **Interactive:** Click items (press **E**) to use them (Eat food, check licenses).
+
+### 💼 30+ Unique Jobs
+Experience a variety of careers including:
+- **Delivery:** Pizza Boy, Courier, Trucker, PostOp...
+- **Emergency:** Police, Paramedic, Firefighter, Coast Guard...
+- **Manual Labor:** Miner, Lumberjack, Farmer, Fisherman...
+- **Underground:** Drug Dealer, Car Thief, Hitman...
+- **Services:** Taxi, Bus Driver, Mechanic, Reporter...
+
+### 👮 Police & Crime (Los Santos RED Integration)
+- **Heat System:** Committing crimes raises "Heat" separate from Vanilla stars.
+- **Dispatch:** Custom police reinforcements based on Heat level.
+- **Arrest:** Press **L** to surrender when wanted.
+- **Jail:** Serve time at Bolingbroke Penitentiary if arrested. Work off your sentence or try to escape!
+
+### 🍔 Survival Mechanics
+- **Hunger & Thirst:** You must eat and drink to survive.
+- **Fatigue:** Rest regularly to avoid passing out.
+- **Status Bars:** Monitor your vital stats on the HUD.
+
+### 🤘 Gang Reputation
+- Build or destroy relationships with Families, Ballas, Vagos, and The Lost.
+- **Territory:** Entering hostile gang territory can lead to attacks.
+
+### 🪪 Advanced Licensing System
+- **Licenses:** Driver, Weapon, Hunting, Flying, Commercial.
+- **Health Insurance:** Mandatory for cheaper hospital bills.
+- **Testing:** Pass practical exams to earn your licenses.
+- **Expiry:** Licenses and insurance expire every 30 in-game days.
+
+## Installation
+
+1. Ensure you have **ScriptHookV** and **ScriptHookVDotNet** installed.
+2. Compile this project or copy the source files into your `GTA V/scripts/` directory.
+   > **Note:** If compiling manually in Visual Studio, delete `src/Dependencies/SHVDN_Stubs.cs` as it is only for development without the game installed.
+3. Launch the game.
+
+## Controls
+- **F5**: Open/Close Inventory
+- **E**: Interact / Click Item in Inventory
+- **L**: Surrender to Police (When Wanted)
+- **F6**: Start Driving Test (Debug)
+
+## Developer Notes
+This mod is structured to be modular.
+- `Core/`: Manages global systems like Licenses, Survival, Gangs, and Prison.
+- `Police/`: Handles Crime detection and Dispatch logic.
+- `Items/`: Defines item properties and inventory logic.
+- `Jobs/`: Contains the logic for all 30+ jobs.
+- `UI/`: Handles the custom drawing of the grid menu.
+