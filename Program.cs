using System;
using System.Collections.Generic;
using System.Diagnostics;

using Mawanella_Intelligent_Routing_System;

// false = run the functional demonstration using Tier 1
// true  = run the Tier 5 performance test

bool performanceTest = false;


//Tier 1 dataset 45,85 
string tier1NodesFile = "Data/Mawanella_Routing_Nodes_FINAL.csv";
string tier1EdgesFile = "Data/Mawanella_Routing_Edges_FINAL.csv";

//Tier 2 dataset 89, 178
string tier2NodesFile = "Data/Mawanella_Routing_Nodes_Tier2_89_FINAL.csv";
string tier2EdgesFile = "Data/Mawanella_Routing_Edges_Tier2_178_FINAL.csv";

//Tier 3 dataset 175, 347
string tier3NodesFile = "Data/Mawanella_Routing_Nodes_Tier3_175_FINAL.csv";
string tier3EdgesFile = "Data/Mawanella_Routing_Edges_Tier3_347_FINAL.csv";

//Tier 4 dataset 300, 600
string tier4NodesFile = "Data/Mawanella_Routing_Nodes_Tier4_300_Synthetic.csv";
string tier4EdgesFile = "Data/Mawanella_Routing_Edges_Tier4_600_Synthetic.csv";

//Tier 5 dataset 600, 1200
string tier5NodesFile = "Data/Mawanella_Routing_Nodes_Tier5_600_Synthetic.csv";
string tier5EdgesFile = "Data/Mawanella_Routing_Edges_Tier5_1200_Synthetic.csv";

// ============================================================
// LOAD GRAPH
// ============================================================

string nodesFile;
string edgesFile;

if (performanceTest)
{
    nodesFile = tier5NodesFile;
    edgesFile = tier5EdgesFile;
}
else
{
    nodesFile = tier1NodesFile;
    edgesFile = tier1EdgesFile;
}


Graph graph = new Graph();

// Load nodes
CsvLoader.LoadNodes(nodesFile, graph);

// Load edges
CsvLoader.LoadEdges(edgesFile, graph);


// ============================================================
// FUNCTIONAL TESTS
// ============================================================

if (!performanceTest)
{
    Console.WriteLine();
    Console.WriteLine("========================================");
    Console.WriteLine("Mawanella Intelligent Routing System");
    Console.WriteLine("========================================");

    Console.WriteLine();
    Console.WriteLine("Network Information");
    Console.WriteLine("--------------------");

    Console.WriteLine(
        "Number of nodes: " +
        graph.GetNodes().Count
    );

    Console.WriteLine(
        "Number of edges: " +
        graph.GetEdges().Count
    );


    // ========================================================
    // DISPLAY NEIGHBOURS
    // ========================================================

    Console.WriteLine();
    Console.WriteLine("Neighbours of N01:");
    Console.WriteLine("------------------");

    List<Edge> neighbours =
        graph.GetNeighbours("N01");

    foreach (Edge edge in neighbours)
    {
        Console.WriteLine(
            edge.From + " -> " +
            edge.To +
            " | Distance: " +
            edge.Distance.ToString("F2") +
            " km" +
            " | Travel Time: " +
            edge.TravelTime.ToString("F2") +
            " min" +
            " | Congestion: " +
            edge.Congestion +
            "%"
        );
    }


    // ========================================================
    // SHORTEST DISTANCE ROUTE
    // ========================================================

    Console.WriteLine();
    Console.WriteLine("Shortest Distance Route");
    Console.WriteLine("-----------------------");

    double totalDistance;

    List<string> path =
        Dijkstra.FindShortestPath(
            graph,
            "N01",
            "N15",
            out totalDistance
        );

    if (path.Count > 0)
    {
        Console.WriteLine(
            "Route: " +
            string.Join(" -> ", path)
        );

        Console.WriteLine(
            "Total Distance: " +
            totalDistance.ToString("F2") +
            " km"
        );
    }
    else
    {
        Console.WriteLine("No route found.");
    }


    // ========================================================
    // FASTEST ROUTE
    // ========================================================

    Console.WriteLine();
    Console.WriteLine("Fastest Route");
    Console.WriteLine("-------------");

    double totalTime;

    List<string> fastestPath =
        Dijkstra.FindFastestPath(
            graph,
            "N01",
            "N15",
            out totalTime
        );

    if (fastestPath.Count > 0)
    {
        Console.WriteLine(
            "Route: " +
            string.Join(" -> ", fastestPath)
        );

        Console.WriteLine(
            "Total Travel Time: " +
            totalTime.ToString("F2") +
            " minutes"
        );
    }
    else
    {
        Console.WriteLine("No route found.");
    }


    // ========================================================
    // CONSTRAINED ROUTING
    // ========================================================

    Console.WriteLine();
    Console.WriteLine("Constrained Routing");
    Console.WriteLine("-------------------");

    double maxCongestion = 70;

    Console.WriteLine(
        "Maximum allowed congestion: " +
        maxCongestion +
        "%"
    );

    double constrainedDistance;

    List<string> constrainedPath =
        Dijkstra.FindConstrainedPath(
            graph,
            "N01",
            "N15",
            maxCongestion,
            out constrainedDistance
        );

    if (constrainedPath.Count > 0)
    {
        Console.WriteLine(
            "Route: " +
            string.Join(" -> ", constrainedPath)
        );

        Console.WriteLine(
            "Total Distance: " +
            constrainedDistance.ToString("F2") +
            " km"
        );
    }
    else
    {
        Console.WriteLine(
            "No route satisfies the congestion constraint."
        );
    }


    // ========================================================
    // STRICT CONGESTION EDGE CASE
    // ========================================================

    Console.WriteLine();
    Console.WriteLine("Strict Congestion Test");
    Console.WriteLine("----------------------");

    double strictCongestion = 50;
    double strictDistance;

    List<string> strictPath =
        Dijkstra.FindConstrainedPath(
            graph,
            "N01",
            "N15",
            strictCongestion,
            out strictDistance
        );

    Console.WriteLine(
        "Maximum allowed congestion: " +
        strictCongestion +
        "%"
    );

    if (strictPath.Count > 0)
    {
        Console.WriteLine(
            "Route: " +
            string.Join(" -> ", strictPath)
        );

        Console.WriteLine(
            "Total Distance: " +
            strictDistance.ToString("F2") +
            " km"
        );
    }
    else
    {
        Console.WriteLine(
            "No route satisfies the stricter congestion constraint."
        );
    }


    // ========================================================
    // TIME-BUDGET REACHABILITY
    // ========================================================

    Console.WriteLine();
    Console.WriteLine("Time-Budget Reachability");
    Console.WriteLine("------------------------");

    double timeBudget = 15;

    List<string> reachableNodes =
        Dijkstra.FindReachableNodes(
            graph,
            "N01",
            timeBudget
        );

    Console.WriteLine(
        "Time budget: " +
        timeBudget +
        " minutes"
    );

    Console.WriteLine(
        "Reachable nodes: " +
        reachableNodes.Count
    );

    Console.WriteLine(
        "Nodes: " +
        string.Join(", ", reachableNodes)
    );


    // ========================================================
    // CONNECTIVITY TEST
    // ========================================================

    Console.WriteLine();
    Console.WriteLine("Connectivity Test");
    Console.WriteLine("------------------");

    List<string> visitedNodes =
        Traversal.DFT(
            graph,
            "N01"
        );

    Console.WriteLine(
        "Nodes reached using DFT: " +
        visitedNodes.Count
    );

    if (visitedNodes.Count ==
        graph.GetNodes().Count)
    {
        Console.WriteLine(
            "The graph is connected."
        );
    }
    else
    {
        Console.WriteLine(
            "The graph is not connected."
        );
    }


    // ========================================================
    // BOTTLENECK NODE TEST
    // ========================================================

    Console.WriteLine();
    Console.WriteLine("Bottleneck Node Test");
    Console.WriteLine("--------------------");

    string criticalNode = "";

    int lowestReachableAfterNode =
        graph.GetNodes().Count - 1;

    // The graph is connected before removal.
    // Removing one normal node should leave
    // all other nodes reachable.
    int normalReachableAfterRemoval =
        visitedNodes.Count - 1;

    foreach (string nodeID in graph.GetNodes().Keys)
    {
        // Do not remove the starting node
        if (nodeID == "N01")
        {
            continue;
        }

        int reachable =
            Traversal.CountReachableAfterRemovingNode(
                graph,
                "N01",
                nodeID
            );

        Console.WriteLine(
            "Removing " +
            nodeID +
            ": " +
            reachable +
            " nodes reachable"
        );

        // Only treat the node as a bottleneck if
        // removing it disconnects additional nodes.
        if (reachable < normalReachableAfterRemoval &&
            reachable < lowestReachableAfterNode)
        {
            lowestReachableAfterNode = reachable;
            criticalNode = nodeID;
        }
    }

    Console.WriteLine();

    if (criticalNode == "")
    {
        Console.WriteLine(
            "No single bottleneck node was detected."
        );
    }
    else
    {
        Console.WriteLine(
            "Most critical node: " +
            criticalNode
        );

        Console.WriteLine(
            "Reachable nodes after removal: " +
            lowestReachableAfterNode
        );
    }


    // ========================================================
    // CRITICAL EDGE TEST
    // ========================================================

    Console.WriteLine();
    Console.WriteLine("Critical Edge Test");
    Console.WriteLine("------------------");

    string criticalEdge = "";

    int lowestReachableAfterEdge =
        visitedNodes.Count;

    foreach (Edge edge in graph.GetEdges())
    {
        int reachable =
            Traversal.CountReachableAfterRemovingEdge(
                graph,
                "N01",
                edge.From,
                edge.To
            );

        Console.WriteLine(
            "Removing " +
            edge.EdgeID +
            " (" +
            edge.From +
            " - " +
            edge.To +
            "): " +
            reachable +
            " nodes reachable"
        );

        // Only select an edge if its removal
        // disconnects part of the graph.
        if (reachable < lowestReachableAfterEdge)
        {
            lowestReachableAfterEdge = reachable;
            criticalEdge = edge.EdgeID;
        }
    }

    Console.WriteLine();

    if (criticalEdge == "")
    {
        Console.WriteLine(
            "No single critical edge was detected."
        );
    }
    else if (lowestReachableAfterEdge ==
             visitedNodes.Count)
    {
        Console.WriteLine(
            "No single critical edge was detected."
        );
    }
    else
    {
        Console.WriteLine(
            "Most critical edge: " +
            criticalEdge
        );

        Console.WriteLine(
            "Reachable nodes after removal: " +
            lowestReachableAfterEdge
        );
    }
}


// ============================================================
// PERFORMANCE TEST
// ============================================================

if (performanceTest)
{
    Console.WriteLine();
    Console.WriteLine("========================================");
    Console.WriteLine("Performance Test");
    Console.WriteLine("========================================");

    Console.WriteLine(
        "Nodes: " +
        graph.GetNodes().Count
    );

    Console.WriteLine(
        "Edges: " +
        graph.GetEdges().Count
    );

    Stopwatch stopwatch =
        new Stopwatch();

    int numberOfRuns = 10;

    double totalTime = 0;

    for (int i = 0; i < numberOfRuns; i++)
    {
        stopwatch.Restart();

        double testDistance;

        Dijkstra.FindShortestPath(
            graph,
            "N0001",
            "N0015",
            out testDistance
        );

        stopwatch.Stop();

        totalTime +=
            stopwatch.Elapsed.TotalMilliseconds;
    }

    double averageTime =
        totalTime / numberOfRuns;

    Console.WriteLine(
        "Average Dijkstra execution time: " +
        averageTime.ToString("F4") +
        " ms"
    );
}