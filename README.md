# Mawanella Intelligent Routing System

COMP50065 Data Structures & Algorithms  
Individual Practical Algorithmic Problem-Solving Assessment

## 1. Project Overview

The Mawanella Intelligent Routing System is a C# console-based prototype that models a selected road network around Mawanella as a weighted graph.

The system demonstrates:

- Network representation using an adjacency list
- Shortest-distance route finding
- Fastest route finding using travel time
- Congestion-constrained routing
- Excluded-node routing
- Excluded-edge routing
- Time-budget reachability
- Network connectivity using Depth-First Traversal (DFT)
- Bottleneck-node testing
- Critical-edge testing
- Performance evaluation as the graph size increases

The project is intended as an algorithmic prototype for the DSA assessment. It is not a live navigation system.

## 2. Technology

- Programming language: C#
- Framework: .NET 8
- Application type: Console Application
- Development environment: Visual Studio
- Programming approach: Object-Oriented Programming (OOP)

### Dependencies

- .NET 8 SDK/runtime
- No external NuGet packages; standard .NET libraries are used.

## 3. Project Structure

```text
Mawanella Intelligent Routing System
│
├── Data
│   ├── Mawanella_Routing_Nodes_FINAL.csv
│   ├── Mawanella_Routing_Edges_FINAL.csv
│   ├── Mawanella_Routing_Nodes_Tier2_89_FINAL.csv
│   ├── Mawanella_Routing_Edges_Tier2_178_FINAL.csv
│   ├── Mawanella_Routing_Nodes_Tier3_175_FINAL.csv
│   ├── Mawanella_Routing_Edges_Tier3_347_FINAL.csv
│   ├── Mawanella_Routing_Nodes_Tier4_300_Synthetic.csv
│   ├── Mawanella_Routing_Edges_Tier4_600_Synthetic.csv
│   ├── Mawanella_Routing_Nodes_Tier5_600_Synthetic.csv
│   └── Mawanella_Routing_Edges_Tier5_1200_Synthetic.csv
│
├── Node.cs
├── Edge.cs
├── Graph.cs
├── CsvLoader.cs
├── Dijkstra.cs
├── Traversal.cs
├── GraphVisualizer.cs
├── Program.cs
└── README.md
```

## 4. Main Data Structures

### Weighted Graph

The road network is represented as a weighted undirected graph.

- Vertex/node = selected Mawanella location
- Edge = road connection
- Edge weights/attributes = distance, travel time, speed limit and congestion

The graph is treated as undirected because the prototype does not use one-way-road information.

### Adjacency List

The graph uses an adjacency list.

Each node stores its connected edges, allowing the routing and traversal algorithms to process the actual neighbouring roads.

## 5. Algorithms

### Dijkstra's Algorithm

The current implementation uses Dijkstra's algorithm with a linear search for the next unvisited node with the smallest known cost.

It is used for:

- Shortest distance routing using `Distance`
- Fastest routing using `TravelTime`
- Congestion-constrained routing
- Excluded-node routing
- Excluded-edge routing
- Time-budget reachability using travel time

The current implementation has:

- Time complexity: O(V² + E)
- Auxiliary space: O(V)

### Depth-First Traversal (DFT)

DFT is implemented iteratively using a stack.

It is used for:

- Network connectivity
- Repeated connectivity checks after removing a node
- Repeated connectivity checks after removing an edge

Complexity:

- Time: O(V + E)
- Auxiliary space: O(V)

### Bottleneck Testing

The prototype uses a remove-and-test approach:

1. Temporarily ignore one candidate node or edge.
2. Run DFT from the starting node.
3. Count the reachable nodes.
4. Compare the reachable count with the original network.

This is a simple prototype method based on the graph traversal techniques taught in the module. It is not a dedicated articulation-point or bridge-finding algorithm.

## 6. Functional Dataset

The baseline dataset contains:

- 45 nodes
- 85 edges

The baseline network is connected.

Each edge contains:

```text
EdgeID
From
To
Distance_km
SpeedLimit_kmh
TravelTime_min
Congestion_percent
RoadType
```

## 7. Functional Test Results

The current baseline test uses:

- Start node: `N01`
- Target node: `N15`

### Shortest-distance route

```text
N01 -> N10 -> N11 -> N12 -> N13 -> N14 -> N15
Total Distance: 21.00 km
```

### Fastest route

```text
N01 -> N10 -> N11 -> N12 -> N13 -> N14 -> N15
Total Travel Time: 17.90 minutes
```

### Congestion-constrained route

Maximum allowed congestion:

```text
70%
```

The same 21.00 km route is available.

### Strict congestion test

Maximum allowed congestion:

```text
50%
```

No route satisfies the constraint.

### Excluded-node test

Excluded node:

```text
N12
```

Alternative route:

```text
N01 -> N10 -> N11 -> N36 -> N24 -> N13 -> N14 -> N15
Total Distance: 34.00 km
```

### Excluded-edge test

Excluded edge:

```text
E012
```

Alternative route:

```text
N01 -> N25 -> N11 -> N12 -> N13 -> N14 -> N15
Total Distance: 21.50 km
```

### Time-budget reachability

The system tests reachability using different travel-time budgets.

| Time budget | Reachable nodes |
|---:|---:|
| 5 minutes | 9 |
| 10 minutes | 17 |
| 15 minutes | 34 |
| 20 minutes | 41 |
| 30 minutes | 45 |

The results show how the number of reachable locations changes as the available travel-time budget increases.

### Connectivity

```text
45 of 45 nodes reached using DFT
Graph is connected
```

### Bottleneck-node test

The starting node is excluded from the candidate list because the connectivity test starts from that node.

The result for the baseline graph is:

```text
Most critical node: N07
Reachable nodes after removal: 41
```

### Critical-edge test

```text
No single critical edge was detected.
```

## 8. Performance Evaluation

Five graph sizes were used to evaluate how the current Dijkstra implementation behaves as the input grows.

| Dataset | Nodes | Edges | Dijkstra searches | Average time |
|---|---:|---:|---:|---:|
| Tier 1 | 45 | 85 | 225 | 0.026416 ms |
| Tier 2 | 89 | 178 | 445 | 0.070910 ms |
| Tier 3 | 175 | 347 | 875 | 0.389667 ms |
| Tier 4 | 300 | 600 | 1500 | 0.329722 ms |
| Tier 5 | 600 | 1200 | 3000 | 1.341842 ms |

The benchmark was run using the Release configuration.

The average execution time generally increased as the network size increased, although the measured value for Tier 4 was slightly lower than Tier 3. This variation is retained as measured rather than being manually adjusted.

Each benchmark search completed successfully:

- Tier 1: 225/225
- Tier 2: 445/445
- Tier 3: 875/875
- Tier 4: 1500/1500
- Tier 5: 3000/3000

## 9. Complexity Summary

| Component | Time | Auxiliary Space |
|---|---:|---:|
| Adjacency-list graph storage | — | O(V + E) |
| DFT | O(V + E) | O(V) |
| Current Dijkstra | O(V² + E) | O(V) |
| Fastest-path Dijkstra | O(V² + E) | O(V) |
| Constrained Dijkstra | O(V² + E) | O(V) |
| Time-budget reachability | O(V² + E) | O(V) |
| One removal + DFT | O(V + E) | O(V) |
| Repeated node-removal test | O(V(V + E)) | O(V) |
| Repeated edge-removal test | O(E(V + E)) | O(V) |

Where:

- `V` = number of vertices/nodes
- `E` = number of edges/roads

## 10. Dataset Assumptions

- The study area is limited to Mawanella town and surrounding areas.
- The baseline network contains 45 nodes and 85 edges.
- The baseline network is treated as undirected.
- Distances are approximate project data rather than surveyed road measurements.
- Travel-time values are modelled/project values.
- Congestion values are synthetic project data rather than live traffic measurements.
- Speed limits are simplified project assumptions.
- Synthetic larger datasets are used for scalability testing.
- The prototype does not model live traffic, turn restrictions, traffic signals or real-time incidents.

## 11. How to Run

### Functional demonstration

1. Open the project in Visual Studio.
2. Select the `Release` configuration when testing final performance.
3. Set:

```csharp
bool runPerformanceTest = false;
```

4. Build/Rebuild the solution.
5. Run the application.

The functional tests use the Tier 1 dataset by default.

### Performance evaluation

Set:

```csharp
bool runPerformanceTest = true;
```

Then rebuild and run the application.

The performance section automatically loads the five datasets and reports the benchmark results.

The .NET CLI also supports explicitly selecting the Release configuration with `dotnet build --configuration Release`. Microsoft documents `dotnet build` as the standard command for building a project and its dependencies. 

## 12. Limitations

This is a Level 5 DSA prototype rather than a production navigation application.

Current limitations include:

- No live traffic data
- Synthetic congestion values
- Approximate/project road distances
- Simplified speed-limit assumptions
- No one-way-road model
- No turn restrictions
- No real-time incidents
- Simple CSV parsing using comma splitting
- Bottleneck detection uses repeated removal and traversal rather than a specialized graph algorithm
- Current Dijkstra implementation uses a linear search rather than a priority queue

These limitations are documented intentionally so that the prototype's scope is clear.

## 13. AI Use Declaration

AI assistance was used during development for:
- explaining DSA concepts
- checking code structure and syntax
- helping troubleshoot implementation issues
- helping organize documentation and presentation material

## 14. Assessment Alignment

The project demonstrates:

- Problem decomposition
- Data-structure comparison and selection
- Algorithm comparison and selection
- Object-oriented implementation
- Time and space complexity analysis
- Functional testing
- Performance evaluation as input size increases
- Reflection on limitations and possible improvements

## 15. Possible Future Improvements

For a production-quality system, possible improvements would include:

- Priority-queue-based Dijkstra
- Real road-network data
- Live traffic information
- One-way and turn restrictions
- More specialized bottleneck algorithms
- More robust CSV parsing
- Larger and more varied performance experiments