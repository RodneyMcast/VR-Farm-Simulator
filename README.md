# VR-Farm-Simulator

A fully immersive Virtual Reality farming simulation game built with Unity. Step into the boots of a farmer and manage your own virtual farm with realistic mechanics and engaging gameplay.

---

## 🎮 What You Can Do in This Game

### Farming & Crop Management
- **Plant Seeds**: Use the seed spawner to place wheat seeds in designated farm plots
- **Watch Crops Grow**: Observe your crops grow through multiple stages on the farm
- **Harvest Wheat**: Collect mature wheat crops from your fields
- **Manage Resources**: Track your inventory and seed counts via the UI system

### Equipment & Vehicles
- **Drive a Tractor**: Mount and operate a fully functional tractor with realistic steering controls
- **Rotate Equipment**: Rotate tools and objects 90-degree increments while holding them for precise placement
- **Grab & Manipulate**: Pick up and interact with objects using VR hand gestures

### Farm Infrastructure
- **Build Fences**: Place and construct farm fences to organize your property
- **Create Farm Layout**: Design your farm layout with multiple plots and boundaries
- **Organize Crates**: Collect and manage harvest crates for crop storage

### Animal Interaction
- **Pet Cows**: Interact with farm animals through touch-based interactions
- **Experience Farm Life**: Encounter realistic farm animals in your environment

### Inventory System
- **Track Harvests**: Monitor collected crops and resources in real-time
- **Organize Items**: Manage your farm inventory with a visual UI system
- **Resource Management**: Keep track of seeds and harvested goods

---

## ⚠️ Important Notice

### VR Framework & Input Systems
The basic VR interaction framework (hand tracking, grab mechanics, and input systems) is built using **Unity's XR Interaction Toolkit** and **XR Hands package**. These are industry-standard Unity plugins that provide the foundational VR functionality.

### 3D Assets
All 3D models and visual assets (farm equipment, crops, animals, fencing, etc.) are sourced from the **Unity Asset Store**. These professionally-designed models ensure high visual quality and realistic farm aesthetics.

---

## 🛠️ Custom Game Logic & Systems

While the project leverages Unity's VR framework and pre-made assets, the following **custom gameplay systems and logic** were developed specifically for this game:

### Core Systems
- **Crop Growth System** (`PlotGrowthSocket.cs`): Custom logic for crop progression through growth stages
- **Inventory Management** (`InventoryManager.cs`, `InventoryUI.cs`, `InventoryItem.cs`): Complete inventory tracking and UI integration
- **Seed Management** (`SeedCountUI.cs`, `SeedSpawnerUI.cs`): Seed counting and spawning mechanics
- **Vehicle Controls** (`TractorSteeringDrive.cs`, `TractorMount.cs`): Custom tractor steering and mounting system
- **Grabbing Mechanics** (`FixedGrabWheel.cs`): Custom grab wheel implementation for VR interactions
- **Object Rotation** (`RotateWhileHeld90.cs`): Custom logic for rotating held objects by 90-degree increments
- **Crate Collection** (`CrateCollector.cs`): Custom harvest crate collection system
- **Animal Interaction** (`CowTouchInteract.cs`): Custom touch-based cow interaction logic

### Programming Language
- **C#** (69% of codebase): Primary game logic and systems
- **ShaderLab** (23.2%): Custom visual effects and materials
- **HLSL** (4.4%): Advanced shader programming
- **Wolfram Language** (3.4%): Data and analysis tools

---

## 🚀 Getting Started

1. **Install Unity**: Requires a recent version of Unity with VR support
2. **Install Required Packages**: XR Interaction Toolkit, XR Hands, and Input System packages
3. **Clone the Repository**: Get the full project with all custom scripts and configurations
4. **Load the Main Scene**: Start playing from the main farm scene
5. **Enjoy Your Farm**: Put on your VR headset and start farming!

---

## 📋 Project Structure

```
Assets/
├── Scripts/               # Custom C# game logic
├── Scenes/               # Main game scenes
├── Prefabs/              # Reusable game objects (fences, crops, etc.)
├── Sounds/               # Audio and sound effects
├── Settings/             # VR and project settings
└── XR/                   # XR-specific configurations
```

---

## 📝 License

This project uses Unity's XR Interaction Toolkit and assets from the Unity Asset Store. Please refer to the respective licenses for those components.

---

**Developed with ❤️ for immersive farming experiences in VR**
