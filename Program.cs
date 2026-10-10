using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Mawanella_Intelligent_Routing_System;

internal class Program
{
    private sealed class Dataset
    {
        public string Name { get; }
        public string Nodes { get; }
        public string Edges { get; }
        public bool Synthetic { get; }

        public Dataset(string name, string nodes, string edges, bool synthetic = false)
        {
            Name = name; Nodes = nodes; Edges = edges; Synthetic = synthetic;
        }
    }

    private static readonly Dataset[] Datasets =
    {
        new Dataset("Tier 1 - 45 nodes / 85 roads", "Data/Mawanella_Routing_Nodes_FINAL.csv", "Data/Mawanella_Routing_Edges_FINAL.csv"),
        new Dataset("Tier 2 - 89 nodes / 178 roads", "Data/Mawanella_Routing_Nodes_Tier2_89_FINAL.csv", "Data/Mawanella_Routing_Edges_Tier2_178_FINAL.csv"),
        new Dataset("Tier 3 - 175 nodes / 347 roads", "Data/Mawanella_Routing_Nodes_Tier3_175_FINAL.csv", "Data/Mawanella_Routing_Edges_Tier3_347_FINAL.csv"),
        new Dataset("Tier 4 - 300 nodes / 600 roads (synthetic)", "Data/Mawanella_Routing_Nodes_Tier4_300_Synthetic.csv", "Data/Mawanella_Routing_Edges_Tier4_600_Synthetic.csv", true),
        new Dataset("Tier 5 - 600 nodes / 1200 roads (synthetic)", "Data/Mawanella_Routing_Nodes_Tier5_600_Synthetic.csv", "Data/Mawanella_Routing_Edges_Tier5_1200_Synthetic.csv", true)
    };

    private static Graph graph;
    private static Dataset activeDataset;

    private static void Main(string[] args)
    {
        if (args.Length > 0 && args[0].Equals("--performance", StringComparison.OrdinalIgnoreCase))
        {
            RunPerformanceEvaluation();
            return;
        }

        try { LoadDataset(Datasets[0]); }
        catch (Exception ex)
        {
            Console.WriteLine("Could not load the default dataset: " + ex.Message);
            Console.WriteLine("Run the application from the project folder so the Data directory can be found.");
            return;
        }

        bool exit = false;
        while (!exit)
        {
            ShowMenu();
            Console.Write("Enter your choice: ");
            switch ((Console.ReadLine() ?? "").Trim())
            {
                case "0": FindLocation(); break;
                case "1": ShortestRoute(); break;
                case "2": FastestRoute(); break;
                case "3": ConstrainedRoute(); break;
                case "4": Reachability(); break;
                case "5": Connectivity(); break;
                case "6": Bottlenecks(); break;
                case "7": NetworkInformation(); break;
                case "8": ChangeDataset(); break;
                case "9": ViewCurrentDataset(); break;
                case "10": exit = true; Console.WriteLine("Exiting the routing system."); break;
                default: Console.WriteLine("Invalid choice. Enter a listed menu option (0-10)."); break;
            }

            if (!exit)
            {
                Console.WriteLine();
                Console.Write("Press Enter to return to the main menu...");
                Console.ReadLine();
                Console.WriteLine();
            }
        }
    }

    private static void ShowMenu()
    {
        Console.WriteLine("==================================================");
        Console.WriteLine("       MAWANELLA INTELLIGENT ROUTING SYSTEM");
        Console.WriteLine("==================================================");
        Console.WriteLine("Dataset: " + activeDataset.Name);
        Console.WriteLine("Nodes: " + graph.GetNodes().Count + " | Roads: " + graph.GetEdges().Count);
        Console.WriteLine();
        Console.WriteLine("0. Find a Location by Name or Node ID");
        Console.WriteLine("1. Find the Shortest Route");
        Console.WriteLine("2. Find the Fastest Route");
        Console.WriteLine("3. Find a Route with Constraints");
        Console.WriteLine("4. Explore Reachable Locations");
        Console.WriteLine("5. Analyse Network Connectivity");
        Console.WriteLine("6. Analyse Road Network Bottlenecks");
        Console.WriteLine("7. View Network Information");
        Console.WriteLine("8. Change Dataset");
        Console.WriteLine("9. View Current Dataset Records");
        Console.WriteLine("10. Exit");
        Console.WriteLine("--------------------------------------------------");
    }

    private static void LoadDataset(Dataset dataset)
    {
        Graph loaded = new Graph();
        CsvLoader.LoadNodes(ResolveDataPath(dataset.Nodes), loaded);
        CsvLoader.LoadEdges(ResolveDataPath(dataset.Edges), loaded);
        if (loaded.GetNodes().Count == 0)
            throw new InvalidDataException("The selected dataset contains no nodes.");
        graph = loaded;
        activeDataset = dataset;
        Console.WriteLine("Loaded " + dataset.Name + ".");
    }

    private static string ResolveDataPath(string relativePath)
    {
        string folder = Directory.GetCurrentDirectory();
        for (int i = 0; i < 7 && folder != null; i++)
        {
            string candidate = Path.Combine(folder, relativePath);
            if (File.Exists(candidate)) return candidate;
            DirectoryInfo parent = Directory.GetParent(folder);
            folder = parent == null ? null : parent.FullName;
        }
        throw new FileNotFoundException("Data file not found: " + relativePath);
    }

    private static string ReadNode(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = (Console.ReadLine() ?? "").Trim();
            if (input.Length == 0)
            {
                Console.WriteLine("Enter a node ID or part of a location name.");
                continue;
            }

            string id = input.ToUpperInvariant();
            if (graph.GetNode(id) != null) return id;

            List<Node> matches = new List<Node>();
            foreach (Node node in graph.GetNodes().Values)
            {
                if (node.Location.IndexOf(input, StringComparison.OrdinalIgnoreCase) >= 0)
                    matches.Add(node);
            }

            if (matches.Count == 1)
            {
                Console.WriteLine("Selected: " + matches[0].NodeID + " - " + matches[0].Location);
                return matches[0].NodeID;
            }

            if (matches.Count > 1)
            {
                Console.WriteLine("Matching locations:");
                foreach (Node node in matches)
                    Console.WriteLine("  " + node.NodeID + " - " + node.Location + " (" + node.RoadType + ")");
                Console.WriteLine("Enter the exact node ID from the list.");
            }
            else
            {
                Console.WriteLine("No node ID or location matched '" + input + "'. Try a different name.");
            }
        }
    }

    private static void FindLocation()
    {
        Console.WriteLine("FIND A LOCATION");
        Console.Write("Enter a node ID or part of a location name: ");
        string input = (Console.ReadLine() ?? "").Trim();
        if (input.Length == 0)
        {
            Console.WriteLine("No search term entered.");
            return;
        }

        List<Node> matches = new List<Node>();
        string id = input.ToUpperInvariant();
        Node exact = graph.GetNode(id);
        if (exact != null)
        {
            matches.Add(exact);
        }
        else
        {
            foreach (Node node in graph.GetNodes().Values)
            {
                if (node.Location.IndexOf(input, StringComparison.OrdinalIgnoreCase) >= 0)
                    matches.Add(node);
            }
        }

        if (matches.Count == 0)
        {
            Console.WriteLine("No matching locations found.");
            return;
        }

        Console.WriteLine("Matching locations (" + matches.Count + "):");
        foreach (Node node in matches)
            Console.WriteLine("  " + node.NodeID + " - " + node.Location + " (" + node.RoadType + ")");
    }

    private static bool ReadNumber(string prompt, double min, double max, out double value)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine() ?? "";
            bool parsed = double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                || double.TryParse(input, out value);
            if (parsed && !double.IsNaN(value) && !double.IsInfinity(value) && value >= min && value <= max)
                return true;
            Console.WriteLine("Enter a number between " + min + " and " + max + ".");
            value = 0;
        }
    }

    private static void PrintRoute(List<string> route)
    {
        if (route.Count == 0)
        {
            Console.WriteLine("No route was found for the selected conditions.");
            return;
        }

        List<string> stops = new List<string>();
        foreach (string nodeID in route)
        {
            Node node = graph.GetNode(nodeID);
            stops.Add(node == null ? nodeID : node.NodeID + " (" + node.Location + ")");
        }

        Console.WriteLine("Route: " + string.Join(" -> ", stops));
        Console.WriteLine("Locations visited along route: " + route.Count);
        Console.WriteLine("Roads used: " + Math.Max(0, route.Count - 1));
    }

    private static double GetRouteTotal(List<string> route, bool useTravelTime)
    {
        double total = 0;
        for (int i = 0; i < route.Count - 1; i++)
        {
            string from = route[i];
            string to = route[i + 1];
            Edge matchingEdge = null;

            foreach (Edge edge in graph.GetNeighbours(from))
            {
                if (edge.To == to)
                {
                    matchingEdge = edge;
                    break;
                }
            }

            if (matchingEdge == null)
                throw new InvalidOperationException("Could not find the road connecting " + from + " and " + to + ".");

            total += useTravelTime ? matchingEdge.TravelTime : matchingEdge.Distance;
        }

        return total;
    }

    private static void ShortestRoute()
    {
        Console.WriteLine("SHORTEST-DISTANCE ROUTE");
        string start = ReadNode("Start location (name or node ID): ");
        string target = ReadNode("Destination (name or node ID): ");
        List<string> route = Dijkstra.FindShortestPath(graph, start, target, out double distance);
        PrintRoute(route);
        if (route.Count > 0)
        {
            Console.WriteLine("Total distance: " + distance.ToString("F2") + " km");
            Console.WriteLine("Estimated travel time along this route: " +
                GetRouteTotal(route, true).ToString("F2") + " minutes");
        }
    }

    private static void FastestRoute()
    {
        Console.WriteLine("FASTEST ROUTE");
        string start = ReadNode("Start location (name or node ID): ");
        string target = ReadNode("Destination (name or node ID): ");
        List<string> route = Dijkstra.FindFastestPath(graph, start, target, out double time);
        PrintRoute(route);
        if (route.Count > 0)
        {
            Console.WriteLine("Estimated travel time: " + time.ToString("F2") + " minutes");
            Console.WriteLine("Distance along this route: " + GetRouteTotal(route, false).ToString("F2") + " km");
        }
        Console.WriteLine("Travel times are project estimates, not live traffic predictions.");
    }

    private static void ConstrainedRoute()
    {
        Console.WriteLine("ROUTE WITH CONSTRAINTS");
        Console.WriteLine("1. Maximum congestion");
        Console.WriteLine("2. Avoid a node");
        Console.WriteLine("3. Avoid a road/edge");
        Console.WriteLine("4. Maximum congestion and avoid a node");
        Console.Write("Constraint choice: ");
        string option = (Console.ReadLine() ?? "").Trim();

        string start = ReadNode("Start location (name or node ID): ");
        string target = ReadNode("Destination (name or node ID): ");
        double congestion = 100;
        string excludedNode = null;
        string excludedEdge = null;

        if (option == "1" || option == "4")
            if (!ReadNumber("Maximum congestion (0-100): ", 0, 100, out congestion)) return;

        if (option == "2" || option == "4")
        {
            excludedNode = ReadNode("Node ID to avoid: ");
            if (excludedNode == start || excludedNode == target)
            {
                Console.WriteLine("The excluded node cannot be the start or destination.");
                return;
            }
        }
        else if (option == "3")
        {
            Console.Write("Edge ID to avoid (for example E012): ");
            excludedEdge = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
            if (!graph.GetEdges().Exists(e => e.EdgeID.Equals(excludedEdge, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Edge ID not found in this dataset.");
                return;
            }
        }
        else if (option != "1")
        {
            Console.WriteLine("Invalid constraint choice.");
            return;
        }

        List<string> route = Dijkstra.FindConstrainedPath(graph, start, target, congestion,
            excludedNode, excludedEdge, out double distance);
        PrintRoute(route);
        if (route.Count > 0)
        {
            Console.WriteLine("Total distance: " + distance.ToString("F2") + " km");
            Console.WriteLine("Estimated travel time along this route: " +
                GetRouteTotal(route, true).ToString("F2") + " minutes");
        }
    }

    private static void Reachability()
    {
        Console.WriteLine("REACHABLE LOCATIONS");
        string start = ReadNode("Starting location (name or node ID): ");
        if (!ReadNumber("Travel-time budget in minutes: ", 0, 1000000, out double budget)) return;

        Dictionary<string, double> times = Dijkstra.FindReachableTravelTimes(graph, start, budget);
        List<string> ids = new List<string>(times.Keys);
        ids.Sort((a, b) =>
        {
            int comparison = times[a].CompareTo(times[b]);
            return comparison != 0 ? comparison : StringComparer.Ordinal.Compare(a, b);
        });

        Console.WriteLine(ids.Count + " of " + graph.GetNodes().Count +
            " locations are reachable within " + budget.ToString("F2", CultureInfo.InvariantCulture) + " minutes.");
        Console.WriteLine("Node ID | Location | Estimated travel time (min)");
        foreach (string id in ids)
        {
            Node node = graph.GetNode(id);
            Console.WriteLine(id + " | " + node.Location + " | " +
                times[id].ToString("F2", CultureInfo.InvariantCulture));
        }
        Console.WriteLine("Travel times are project estimates, not live traffic predictions.");
    }

    private static void Connectivity()
    {
        Console.WriteLine("NETWORK CONNECTIVITY");
        string start = ReadNode("Starting node ID: ");
        List<string> visited = Traversal.DFT(graph, start);
        Console.WriteLine("Nodes reached: " + visited.Count + " of " + graph.GetNodes().Count);
        Console.WriteLine(visited.Count == graph.GetNodes().Count
            ? "The network is connected."
            : "Not every node is reachable from this starting node.");
    }

    private static void Bottlenecks()
    {
        Console.WriteLine("STRUCTURAL BOTTLENECK ANALYSIS");
        Console.WriteLine("Each candidate is temporarily ignored during analysis; the loaded graph is not changed.");
        string start = ReadNode("Starting node ID: ");
        int baseline = Traversal.DFT(graph, start).Count;
        string criticalNode = null, criticalEdge = null, edgeFrom = null, edgeTo = null;
        int lowestNodeCount = baseline, lowestEdgeCount = baseline;

        foreach (string id in graph.GetNodes().Keys)
        {
            if (id == start) continue;
            int count = Traversal.CountReachableAfterRemovingNode(graph, start, id);
            if (count < lowestNodeCount) { lowestNodeCount = count; criticalNode = id; }
        }

        foreach (Edge edge in graph.GetEdges())
        {
            int count = Traversal.CountReachableAfterRemovingEdge(graph, start, edge.From, edge.To);
            if (count < lowestEdgeCount)
            {
                lowestEdgeCount = count; criticalEdge = edge.EdgeID;
                edgeFrom = edge.From; edgeTo = edge.To;
            }
        }

        Console.WriteLine("Baseline reachable nodes: " + baseline);
        Console.WriteLine(criticalNode == null
            ? "No tested node removal reduced reachability."
            : "Largest observed node impact: " + criticalNode + " (" + lowestNodeCount + " nodes remain reachable).");
        Console.WriteLine(criticalEdge == null
            ? "No tested single-road removal reduced reachability."
            : "Largest observed road impact: " + criticalEdge + " (" + edgeFrom + " <-> " + edgeTo +
                "; " + lowestEdgeCount + " nodes remain reachable).");
        Console.WriteLine("This is removal-and-traversal analysis, not a dedicated articulation-point or bridge algorithm.");
    }

    private static void NetworkInformation()
    {
        Console.WriteLine("NETWORK INFORMATION");
        Console.WriteLine("Dataset: " + activeDataset.Name);
        Console.WriteLine("Type: " + (activeDataset.Synthetic ? "Synthetic scalability dataset" : "Project road-network dataset"));
        Console.WriteLine("Nodes: " + graph.GetNodes().Count);
        Console.WriteLine("Undirected roads: " + graph.GetEdges().Count);
        Console.WriteLine("The prototype uses project values and does not use live traffic data.");

        Node node = graph.GetNode(ReadNode("Node ID to inspect: "));
        Console.WriteLine("Location: " + node.Location);
        Console.WriteLine("Road type: " + node.RoadType);
        foreach (Edge edge in graph.GetNeighbours(node.NodeID))
            Console.WriteLine("- " + edge.EdgeID + ": " + edge.From + " -> " + edge.To +
                " | " + edge.Distance.ToString("F2") + " km | " +
                edge.TravelTime.ToString("F2") + " min | congestion " + edge.Congestion.ToString("F0") + "%");
    }


    private static void ViewCurrentDataset()
    {
        Console.WriteLine("VIEW CURRENT DATASET");
        Console.WriteLine("Active tier: " + activeDataset.Name);
        Console.WriteLine("Nodes file: " + activeDataset.Nodes);
        Console.WriteLine("Roads file: " + activeDataset.Edges);
        Console.WriteLine();
        Console.WriteLine("1. View locations (nodes)");
        Console.WriteLine("2. View roads (edges)");
        Console.WriteLine("0. Return to main menu");
        Console.Write("Choose records to view: ");
        string choice = (Console.ReadLine() ?? "").Trim();

        List<string> records = new List<string>();
        if (choice == "1")
        {
            records.Add("Node ID | Location | Road type");
            List<Node> nodes = new List<Node>(graph.GetNodes().Values);
            nodes.Sort((a, b) => StringComparer.Ordinal.Compare(a.NodeID, b.NodeID));
            foreach (Node node in nodes)
                records.Add(node.NodeID + " | " + node.Location + " | " + node.RoadType);
        }
        else if (choice == "2")
        {
            records.Add("Edge ID | From | To | Distance (km) | Travel time (min) | Speed limit (km/h) | Congestion (%) | Road type");
            List<Edge> edges = new List<Edge>(graph.GetEdges());
            edges.Sort((a, b) => StringComparer.Ordinal.Compare(a.EdgeID, b.EdgeID));
            foreach (Edge edge in edges)
                records.Add(edge.EdgeID + " | " + edge.From + " | " + edge.To + " | " +
                    edge.Distance.ToString("F2", CultureInfo.InvariantCulture) + " | " +
                    edge.TravelTime.ToString("F2", CultureInfo.InvariantCulture) + " | " +
                    edge.SpeedLimit.ToString("F2", CultureInfo.InvariantCulture) + " | " +
                    edge.Congestion.ToString("F0", CultureInfo.InvariantCulture) + " | " + edge.RoadType);
        }
        else if (choice == "0")
        {
            return;
        }
        else
        {
            Console.WriteLine("Invalid choice.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine(records[0]);
        Console.WriteLine("Showing " + (records.Count - 1) + " records from " + activeDataset.Name + ".");
        const int pageSize = 20;
        int start = 1;
        while (start < records.Count)
        {
            int end = Math.Min(start + pageSize, records.Count);
            for (int i = start; i < end; i++)
                Console.WriteLine(records[i]);

            if (end >= records.Count) break;
            Console.WriteLine("Showing records " + start + "-" + (end - 1) + " of " + (records.Count - 1) + ".");
            Console.Write("Press Enter for the next page or type Q to stop: ");
            if ((Console.ReadLine() ?? "").Trim().Equals("Q", StringComparison.OrdinalIgnoreCase))
                break;
            start = end;
        }
        Console.WriteLine("End of dataset records.");
    }

    private static void ChangeDataset()
    {
        Console.WriteLine("SELECT DATASET");
        for (int i = 0; i < Datasets.Length; i++) Console.WriteLine((i + 1) + ". " + Datasets[i].Name);
        Console.Write("Dataset number: ");
        if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > Datasets.Length)
        {
            Console.WriteLine("Invalid dataset selection.");
            return;
        }

        try { LoadDataset(Datasets[index - 1]); }
        catch (Exception ex)
        {
            Console.WriteLine("Dataset could not be loaded; the previous dataset remains active.");
            Console.WriteLine(ex.Message);
        }
    }

    private static void RunPerformanceEvaluation()
    {
        Console.WriteLine("PERFORMANCE EVALUATION (fresh measurements; results may vary)");
        foreach (Dataset dataset in Datasets)
        {
            try
            {
                Graph testGraph = new Graph();
                CsvLoader.LoadNodes(ResolveDataPath(dataset.Nodes), testGraph);
                CsvLoader.LoadEdges(ResolveDataPath(dataset.Edges), testGraph);
                List<string> ids = new List<string>(testGraph.GetNodes().Keys);
                Stopwatch timer = Stopwatch.StartNew();
                int attempts = 0, successes = 0;

                for (int startIndex = 0; startIndex < ids.Count; startIndex++)
                {
                    string start = ids[startIndex];
                    int samples = Math.Min(5, ids.Count);
                    for (int i = 0; i < samples; i++)
                    {
                        string target = ids[(startIndex + i) % ids.Count];
                        List<string> route = Dijkstra.FindShortestPath(testGraph, start, target, out double _);
                        attempts++;
                        if (route.Count > 0) successes++;
                    }
                }

                timer.Stop();
                Console.WriteLine(dataset.Name);
                Console.WriteLine("  Successful searches: " + successes + "/" + attempts);
                Console.WriteLine("  Total elapsed: " + timer.Elapsed.TotalMilliseconds.ToString("F3") + " ms");
                Console.WriteLine("  Average per search: " +
                    (attempts == 0 ? 0 : timer.Elapsed.TotalMilliseconds / attempts).ToString("F6") + " ms");
            }
            catch (Exception ex) { Console.WriteLine(dataset.Name + ": evaluation failed - " + ex.Message); }
        }
    }
}
