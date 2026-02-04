# PabloJMartinez.AStar

A personal implementation of a polygonal navigation system (Pathfinding) for Unity, originally developed between 2013 and 2017 for the *Rotten Past* project.

This repository serves as both a historical archive of the original implementation and a foundation for a modern refactor following .NET 10 and C# 14 standards, while maintaining Unity compatibility (netstandard2.1).

## Technical Overview

The system provides the infrastructure required for path generation, spatial optimization, and natural movement:

### 1. A* Algorithm
Standard search algorithm implementation designed to work over polygonal navigation meshes (Navmesh). It manages open/closed node logic to find the lowest-cost path between points.

### 2. Optimized Binary Heap
To improve A* performance, I implemented a Binary Heap from scratch.
*   **Optimization:** Uses **Base-1 indexing**, significantly reducing CPU cycle overhead during the main search loop.
*   **Efficiency:** Ensures **O(log n)** complexity for the lowest-cost node extraction.

### 3. Funnel Algorithm (String Pulling)
Pathfinding over polygons often results in jagged paths. I included a Funnel algorithm implementation to smooth these routes, allowing agents to follow direct, natural linear trajectories between navigation portals.

### 4. Navmesh Generator
A custom Unity editor tool for manual and geometric navigation mesh creation.
*   Graph topology management.
*   Vertex order and convexity calculations (Clockwise/Counter-Clockwise).
*   Dynamic mesh generation using `MeshFilter` and `MeshRenderer`.

### 5. Data Persistence & SQL (Residual Code)
Includes legacy scripts for **SQLite** integration. These were originally used to serialize navigation mesh data for fast runtime loading. This code is functional but kept as part of the legacy reference.

## Repository Structure

The project is currently being refactored into a modular architecture:

*   **`src/`**: Modern, decoupled C# libraries.
*   **`tests/`**: Unit testing suites to validate algorithmic logic.
*   **`legacy/`**: The original 2013-2017 Unity source code, maintained for historical and technical reference.

## Authorship & License

All code within this repository was developed entirely by me.
Distributed under the **MIT License**.
