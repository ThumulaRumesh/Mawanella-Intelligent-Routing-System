using System;
using System.Collections.Generic;
using System.Diagnostics;

using Mawanella_Intelligent_Routing_System;


// ========================================
// PROGRAM MODE
// ========================================

// false = run normal functional tests
// true  = run performance evaluation

bool runPerformanceTest = true;


// true  = run additional edge-case tests
// false = skip edge-case tests

bool runEdgeCaseTests = false;


// ========================================
// FUNCTIONAL DATASET SELECTION
// ========================================

// Tier 1 - 45 Nodes / 85 Edges

string nodesFile =
    "Data/Mawanella_Routing_Nodes_FINAL.csv";

string edgesFile =
    "Data/Mawanella_Routing_Edges_FINAL.csv";


// Tier 2 - 89 Nodes / 178 Edges

//string nodesFile =
//    "Data/Mawanella_Routing_Nodes_Tier2_89_FINAL.csv";

//string edgesFile =
//    "Data/Mawanella_Routing_Edges_Tier2_178_FINAL.csv";


// Tier 3 - 175 Nodes / 347 Edges

//string nodesFile =
//    "Data/Mawanella_Routing_Nodes_Tier3_175_FINAL.csv";

//string edgesFile =
//    "Data/Mawanella_Routing_Edges_Tier3_347_FINAL.csv";


// Tier 4 - 300 Nodes / 600 Edges

//string nodesFile =
//    "Data/Mawanella_Routing_Nodes_Tier4_300_Synthetic.csv";

//string edgesFile =
//    "Data/Mawanella_Routing_Edges_Tier4_600_Synthetic.csv";


// Tier 5 - 600 Nodes / 1200 Edges

//string nodesFile =
//    "Data/Mawanella_Routing_Nodes_Tier5_600_Synthetic.csv";

//string edgesFile =
//    "Data/Mawanella_Routing_Edges_Tier5_1200_Synthetic.csv";


// ========================================
// FUNCTIONAL TESTS
// ========================================

if (!runPerformanceTest)
{
    // ====================================
    // CREATE GRAPH
    // ====================================

    Graph graph = new Graph();

    CsvLoader.LoadNodes(
        nodesFile,
        graph
    );

    CsvLoader.LoadEdges(
        edgesFile,
        graph
    );


    // ====================================
    // START AND TARGET
    // ====================================

    string startNode = "N01";
    string targetNode = "N15";


    // ====================================
    // SYSTEM TITLE
    // ====================================

    Console.WriteLine(
        "========================================"
    );

    Console.WriteLine(
        "Mawanella Intelligent Routing System"
    );

    Console.WriteLine(
        "========================================"
    );


    // ====================================
    // NETWORK INFORMATION
    // ====================================

    Console.WriteLine();
    Console.WriteLine("Network Information");
    Console.WriteLine("--------------------");

    Console.WriteLine(
        "Number of nodes: "
        + graph.GetNodes().Count
    );

    Console.WriteLine(
        "Number of edges: "
        + graph.GetEdges().Count
    );


    // ====================================
    // SHOW NEIGHBOURS
    // ====================================

    Console.WriteLine();
    Console.WriteLine(
        "Neighbours of "
        + startNode
        + ":"
    );

    Console.WriteLine(
        "------------------"
    );

    foreach (Edge edge
             in graph.GetNeighbours(startNode))
    {
        Console.WriteLine(
            edge.From
            + " -> "
            + edge.To
            + " | Distance: "
            + edge.Distance.ToString("F2")
            + " km | Travel Time: "
            + edge.TravelTime.ToString("F2")
            + " min | Congestion: "
            + edge.Congestion
            + "%"
        );
    }


    // ====================================
    // SHORTEST DISTANCE ROUTE
    // ====================================

    Console.WriteLine();
    Console.WriteLine("Shortest Distance Route");
    Console.WriteLine("-----------------------");

    double totalDistance;

    List<string> shortestPath =
        Dijkstra.FindShortestPath(
            graph,
            startNode,
            targetNode,
            out totalDistance
        );

    if (shortestPath.Count > 0)
    {
        Console.WriteLine(
            "Route: "
            + string.Join(
                " -> ",
                shortestPath
            )
        );

        Console.WriteLine(
            "Total Distance: "
            + totalDistance.ToString("F2")
            + " km"
        );
    }
    else
    {
        Console.WriteLine(
            "No route found."
        );
    }


    // ====================================
    // FASTEST ROUTE
    // ====================================

    Console.WriteLine();
    Console.WriteLine("Fastest Route");
    Console.WriteLine("-------------");

    double totalTravelTime;

    List<string> fastestPath =
        Dijkstra.FindFastestPath(
            graph,
            startNode,
            targetNode,
            out totalTravelTime
        );

    if (fastestPath.Count > 0)
    {
        Console.WriteLine(
            "Route: "
            + string.Join(
                " -> ",
                fastestPath
            )
        );

        Console.WriteLine(
            "Total Travel Time: "
            + totalTravelTime.ToString("F2")
            + " minutes"
        );
    }
    else
    {
        Console.WriteLine(
            "No route found."
        );
    }


    // ====================================
    // CONSTRAINED ROUTING - CONGESTION
    // ====================================

    Console.WriteLine();
    Console.WriteLine("Constrained Routing");
    Console.WriteLine("-------------------");

    double maximumCongestion = 70;

    Console.WriteLine(
        "Maximum allowed congestion: "
        + maximumCongestion
        + "%"
    );

    double constrainedDistance;

    List<string> constrainedPath =
        Dijkstra.FindConstrainedPath(
            graph,
            startNode,
            targetNode,
            maximumCongestion,
            null,
            null,
            out constrainedDistance
        );

    if (constrainedPath.Count > 0)
    {
        Console.WriteLine(
            "Route: "
            + string.Join(
                " -> ",
                constrainedPath
            )
        );

        Console.WriteLine(
            "Total Distance: "
            + constrainedDistance.ToString("F2")
            + " km"
        );
    }
    else
    {
        Console.WriteLine(
            "No route satisfies the congestion constraint."
        );
    }


    // ====================================
    // STRICT CONGESTION TEST
    // ====================================

    Console.WriteLine();
    Console.WriteLine("Strict Congestion Test");
    Console.WriteLine("----------------------");

    double strictCongestion = 50;

    Console.WriteLine(
        "Maximum allowed congestion: "
        + strictCongestion
        + "%"
    );

    double strictDistance;

    List<string> strictPath =
        Dijkstra.FindConstrainedPath(
            graph,
            startNode,
            targetNode,
            strictCongestion,
            null,
            null,
            out strictDistance
        );

    if (strictPath.Count > 0)
    {
        Console.WriteLine(
            "Route: "
            + string.Join(
                " -> ",
                strictPath
            )
        );

        Console.WriteLine(
            "Total Distance: "
            + strictDistance.ToString("F2")
            + " km"
        );
    }
    else
    {
        Console.WriteLine(
            "No route satisfies the stricter congestion constraint."
        );
    }


    // ====================================
    // EXCLUDED NODE TEST
    // ====================================

    Console.WriteLine();
    Console.WriteLine("Excluded Node Test");
    Console.WriteLine("------------------");

    string excludedNode = "N12";

    Console.WriteLine(
        "Excluded node: "
        + excludedNode
    );

    double excludedNodeDistance;

    List<string> excludedNodePath =
        Dijkstra.FindConstrainedPath(
            graph,
            startNode,
            targetNode,
            100,
            excludedNode,
            null,
            out excludedNodeDistance
        );

    if (excludedNodePath.Count > 0)
    {
        Console.WriteLine(
            "Alternative route: "
            + string.Join(
                " -> ",
                excludedNodePath
            )
        );

        Console.WriteLine(
            "Total Distance: "
            + excludedNodeDistance.ToString("F2")
            + " km"
        );
    }
    else
    {
        Console.WriteLine(
            "No route is available."
        );
    }


    // ====================================
    // EXCLUDED EDGE TEST
    // ====================================

    Console.WriteLine();
    Console.WriteLine("Excluded Edge Test");
    Console.WriteLine("------------------");

    string excludedEdgeID = "E012";

    Console.WriteLine(
        "Excluded edge: "
        + excludedEdgeID
    );

    double excludedEdgeDistance;

    List<string> excludedEdgePath =
        Dijkstra.FindConstrainedPath(
            graph,
            startNode,
            targetNode,
            100,
            null,
            excludedEdgeID,
            out excludedEdgeDistance
        );

    if (excludedEdgePath.Count > 0)
    {
        Console.WriteLine(
            "Alternative route: "
            + string.Join(
                " -> ",
                excludedEdgePath
            )
        );

        Console.WriteLine(
            "Total Distance: "
            + excludedEdgeDistance.ToString("F2")
            + " km"
        );
    }
    else
    {
        Console.WriteLine(
            "No route is available."
        );
    }


    // ====================================
    // TIME-BUDGET REACHABILITY
    // ====================================

    Console.WriteLine();
    Console.WriteLine("Time-Budget Reachability");
    Console.WriteLine("------------------------");

    double[] timeBudgets =
    {
    5,
    10,
    15,
    20,
    30
};

    foreach (double timeBudget in timeBudgets)
    {
        List<string> reachableNodes =
            Dijkstra.FindReachableNodes(
                graph,
                startNode,
                timeBudget
            );

        Console.WriteLine(
            timeBudget
            + " minutes -> "
            + reachableNodes.Count
            + " nodes reachable"
        );
    }


    // ====================================
    // CONNECTIVITY TEST
    // ====================================

    Console.WriteLine();
    Console.WriteLine("Connectivity Test");
    Console.WriteLine("------------------");

    List<string> visitedNodes =
        Traversal.DFT(
            graph,
            startNode
        );

    Console.WriteLine(
        "Nodes reached using DFT: "
        + visitedNodes.Count
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


    // ====================================
    // BOTTLENECK NODE TEST
    // ====================================

    Console.WriteLine();
    Console.WriteLine("Bottleneck Node Test");
    Console.WriteLine("--------------------");

    string mostCriticalNode = null;

    int lowestReachableCount =
        graph.GetNodes().Count;

    foreach (string nodeID
             in graph.GetNodes().Keys)
    {
        // The starting node cannot be tested
        // because the traversal starts from it.
        if (nodeID == startNode)
        {
            continue;
        }

        int reachableCount =
            Traversal.CountReachableAfterRemovingNode(
                graph,
                startNode,
                nodeID
            );

        Console.WriteLine(
            "Removing "
            + nodeID
            + ": "
            + reachableCount
            + " nodes reachable"
        );

        if (reachableCount <
            lowestReachableCount)
        {
            lowestReachableCount =
                reachableCount;

            mostCriticalNode =
                nodeID;
        }
    }

    if (mostCriticalNode != null)
    {
        Console.WriteLine();

        Console.WriteLine(
            "Most critical node: "
            + mostCriticalNode
        );

        Console.WriteLine(
            "Reachable nodes after removal: "
            + lowestReachableCount
        );
    }
    else
    {
        Console.WriteLine(
            "No critical node was detected."
        );
    }


    // ====================================
    // CRITICAL EDGE TEST
    // ====================================

    Console.WriteLine();
    Console.WriteLine("Critical Edge Test");
    Console.WriteLine("------------------");

    bool criticalEdgeFound = false;

    foreach (Edge edge
             in graph.GetEdges())
    {
        int reachableCount =
            Traversal.CountReachableAfterRemovingEdge(
                graph,
                startNode,
                edge.From,
                edge.To
            );

        if (reachableCount <
            graph.GetNodes().Count)
        {
            Console.WriteLine(
                "Removing "
                + edge.EdgeID
                + ": "
                + reachableCount
                + " nodes reachable"
            );

            criticalEdgeFound = true;
        }
    }

    if (!criticalEdgeFound)
    {
        Console.WriteLine(
            "No single critical edge was detected."
        );
    }


    // ====================================
    // EDGE CASE TESTS
    // ====================================

    if (runEdgeCaseTests)
    {
        Console.WriteLine();
        Console.WriteLine("Edge Case Tests");
        Console.WriteLine("---------------");


        // =================================
        // INVALID START NODE
        // =================================

        double invalidStartDistance;

        List<string> invalidStartPath =
            Dijkstra.FindShortestPath(
                graph,
                "INVALID",
                targetNode,
                out invalidStartDistance
            );

        Console.WriteLine(
            "Invalid start node: "
            + (invalidStartPath.Count == 0
                ? "PASS"
                : "FAIL")
        );


        // =================================
        // INVALID TARGET NODE
        // =================================

        double invalidTargetDistance;

        List<string> invalidTargetPath =
            Dijkstra.FindShortestPath(
                graph,
                startNode,
                "INVALID",
                out invalidTargetDistance
            );

        Console.WriteLine(
            "Invalid target node: "
            + (invalidTargetPath.Count == 0
                ? "PASS"
                : "FAIL")
        );


        // =================================
        // START = TARGET
        // =================================

        double sameNodeDistance;

        List<string> sameNodePath =
            Dijkstra.FindShortestPath(
                graph,
                startNode,
                startNode,
                out sameNodeDistance
            );

        bool sameNodePassed =
            sameNodePath.Count == 1 &&
            sameNodePath[0] == startNode &&
            sameNodeDistance == 0;

        Console.WriteLine(
            "Start equals target: "
            + (sameNodePassed
                ? "PASS"
                : "FAIL")
        );


        // =================================
        // EXCLUDED START NODE
        // =================================

        double excludedStartDistance;

        List<string> excludedStartPath =
            Dijkstra.FindConstrainedPath(
                graph,
                startNode,
                targetNode,
                100,
                startNode,
                null,
                out excludedStartDistance
            );

        Console.WriteLine(
            "Excluded start node: "
            + (excludedStartPath.Count == 0
                ? "PASS"
                : "FAIL")
        );


        // =================================
        // EXCLUDED TARGET NODE
        // =================================

        double excludedTargetDistance;

        List<string> excludedTargetPath =
            Dijkstra.FindConstrainedPath(
                graph,
                startNode,
                targetNode,
                100,
                targetNode,
                null,
                out excludedTargetDistance
            );

        Console.WriteLine(
            "Excluded target node: "
            + (excludedTargetPath.Count == 0
                ? "PASS"
                : "FAIL")
        );


        // =================================
        // ZERO-MINUTE TIME BUDGET
        // =================================

        List<string> zeroBudgetNodes =
            Dijkstra.FindReachableNodes(
                graph,
                startNode,
                0
            );

        bool zeroBudgetPassed =
            zeroBudgetNodes.Count == 1 &&
            zeroBudgetNodes[0] == startNode;

        Console.WriteLine(
            "Zero-minute time budget: "
            + (zeroBudgetPassed
                ? "PASS"
                : "FAIL")
        );


        // =================================
        // ZERO CONGESTION LIMIT
        // =================================

        double zeroCongestionDistance;

        List<string> zeroCongestionPath =
            Dijkstra.FindConstrainedPath(
                graph,
                startNode,
                targetNode,
                0,
                null,
                null,
                out zeroCongestionDistance
            );

        Console.WriteLine(
            "Zero congestion limit: "
            + (zeroCongestionPath.Count == 0
                ? "PASS"
                : "FAIL")
        );


        // =================================
        // EDGE CASE SUMMARY
        // =================================

        Console.WriteLine();

        Console.WriteLine(
            "Edge case testing completed."
        );
    }
}


// ========================================
// PERFORMANCE TEST
// ========================================

else
{
    // ====================================
    // PERFORMANCE TEST DATASETS
    // ====================================

    string[] nodesFiles =
    {
        "Data/Mawanella_Routing_Nodes_FINAL.csv",
        "Data/Mawanella_Routing_Nodes_Tier2_89_FINAL.csv",
        "Data/Mawanella_Routing_Nodes_Tier3_175_FINAL.csv",
        "Data/Mawanella_Routing_Nodes_Tier4_300_Synthetic.csv",
        "Data/Mawanella_Routing_Nodes_Tier5_600_Synthetic.csv"
    };

    string[] edgesFiles =
    {
        "Data/Mawanella_Routing_Edges_FINAL.csv",
        "Data/Mawanella_Routing_Edges_Tier2_178_FINAL.csv",
        "Data/Mawanella_Routing_Edges_Tier3_347_FINAL.csv",
        "Data/Mawanella_Routing_Edges_Tier4_600_Synthetic.csv",
        "Data/Mawanella_Routing_Edges_Tier5_1200_Synthetic.csv"
    };

    string[] datasetNames =
    {
        "Tier 1",
        "Tier 2",
        "Tier 3",
        "Tier 4",
        "Tier 5"
    };


    // ====================================
    // DATASET SIZES
    // ====================================

    int[] nodeCounts =
    {
        45,
        89,
        175,
        300,
        600
    };

    int[] edgeCounts =
    {
        85,
        178,
        347,
        600,
        1200
    };


    // ====================================
    // PERFORMANCE SETTINGS
    // ====================================

    int numberOfRounds = 5;


    // ====================================
    // TITLE
    // ====================================

    Console.WriteLine(
        "========================================"
    );

    Console.WriteLine(
        "Mawanella Dijkstra Performance Evaluation"
    );

    Console.WriteLine(
        "========================================"
    );

    Console.WriteLine();

    Console.WriteLine(
        "Rounds per dataset: "
        + numberOfRounds
    );

    Console.WriteLine(
        "Build configuration: Release"
    );

    Console.WriteLine();


    // ====================================
    // TIMER INFORMATION
    // ====================================

    Console.WriteLine(
        "High-resolution timer: "
        + Stopwatch.IsHighResolution
    );

    Console.WriteLine(
        "Timer frequency: "
        + Stopwatch.Frequency
        + " ticks/second"
    );

    Console.WriteLine();


    // ====================================
    // RESULTS HEADER
    // ====================================

    Console.WriteLine(
        "Results"
    );

    Console.WriteLine(
        "-------------------------------------------------------------"
    );

    Console.WriteLine(
        "Dataset | Nodes | Edges | Searches | Total Time | Avg Time"
    );

    Console.WriteLine(
        "-------------------------------------------------------------"
    );


    // ====================================
    // TEST EACH DATASET
    // ====================================

    for (int i = 0;
         i < nodesFiles.Length;
         i++)
    {
        // =================================
        // LOAD GRAPH
        // =================================

        Graph graph = new Graph();

        CsvLoader.LoadNodes(
            nodesFiles[i],
            graph
        );

        CsvLoader.LoadEdges(
            edgesFiles[i],
            graph
        );


        // =================================
        // SELECT START AND TARGET
        // =================================

        string startNode;
        string targetNode;

        if (i == 0)
        {
            startNode = "N01";
            targetNode = "N45";
        }
        else if (i == 1)
        {
            startNode = "N001";
            targetNode = "N089";
        }
        else if (i == 2)
        {
            startNode = "N001";
            targetNode = "N175";
        }
        else if (i == 3)
        {
            startNode = "N0001";
            targetNode = "N0300";
        }
        else
        {
            startNode = "N0001";
            targetNode = "N0600";
        }


        // =================================
        // GET SOURCE NODES
        // =================================

        List<string> sourceNodes =
            new List<string>(
                graph.GetNodes().Keys
            );


        // =================================
        // WARM-UP
        // =================================

        foreach (string sourceNode
                 in sourceNodes)
        {
            double warmupDistance;

            List<string> warmupPath =
                Dijkstra.FindShortestPath(
                    graph,
                    sourceNode,
                    targetNode,
                    out warmupDistance
                );

            GC.KeepAlive(warmupPath);
        }


        // =================================
        // CHECKSUM AND VALIDATION
        // =================================

        double checksum = 0;

        int successfulSearches = 0;


        // =================================
        // START TIMER
        // =================================

        Stopwatch stopwatch =
            Stopwatch.StartNew();


        // =================================
        // REPEATED DIJKSTRA TEST
        // =================================

        for (int round = 0;
             round < numberOfRounds;
             round++)
        {
            foreach (string sourceNode
                     in sourceNodes)
            {
                double testDistance;

                List<string> path =
                    Dijkstra.FindShortestPath(
                        graph,
                        sourceNode,
                        targetNode,
                        out testDistance
                    );


                // Use the returned values so that
                // the benchmark work is observable.

                checksum +=
                    testDistance;

                checksum +=
                    path.Count;

                if (path.Count > 0)
                {
                    successfulSearches++;
                }
            }
        }


        // =================================
        // STOP TIMER
        // =================================

        stopwatch.Stop();


        // =================================
        // TOTAL TIME
        // =================================

        double totalMilliseconds =
            (double)stopwatch.ElapsedTicks
            / Stopwatch.Frequency
            * 1000.0;


        // =================================
        // TOTAL SEARCHES
        // =================================

        int totalSearches =
            sourceNodes.Count
            * numberOfRounds;


        // =================================
        // AVERAGE TIME
        // =================================

        double averageTime =
            totalMilliseconds
            / totalSearches;


        // =================================
        // DISPLAY RESULT
        // =================================

        Console.WriteLine(
            datasetNames[i]
            + "     | "
            + nodeCounts[i]
            + "   | "
            + edgeCounts[i]
            + "   | "
            + totalSearches
            + "      | "
            + totalMilliseconds.ToString("F4")
            + " ms"
            + " | "
            + averageTime.ToString("F6")
            + " ms"
        );

        Console.WriteLine(
            "          Target: "
            + targetNode
            + " | Successful: "
            + successfulSearches
            + "/"
            + totalSearches
            + " | Checksum: "
            + checksum.ToString("F2")
        );

        Console.WriteLine();
    }


    // ====================================
    // END
    // ====================================

    Console.WriteLine(
        "-------------------------------------------------------------"
    );

    Console.WriteLine();

    Console.WriteLine(
        "Performance test completed."
    );

    Console.WriteLine(
        "All datasets were tested using the same Release configuration."
    );
}