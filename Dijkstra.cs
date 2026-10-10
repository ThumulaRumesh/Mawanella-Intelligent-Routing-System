using System;
using System.Collections.Generic;

namespace Mawanella_Intelligent_Routing_System
{
    public class Dijkstra
    {
        // ============================================================
        // SHORTEST DISTANCE PATH
        // ============================================================

        // Find the shortest-distance path between two nodes
        // using Dijkstra's algorithm.
        public static List<string> FindShortestPath(
            Graph graph,
            string startNode,
            string targetNode,
            out double totalDistance)
        {
            totalDistance = 0;

            // Check whether both nodes exist
            if (!graph.GetNodes().ContainsKey(startNode) ||
                !graph.GetNodes().ContainsKey(targetNode))
            {
                return new List<string>();
            }

            Dictionary<string, double> distance =
                new Dictionary<string, double>();

            Dictionary<string, string> previous =
                new Dictionary<string, string>();

            HashSet<string> unvisited =
                new HashSet<string>();

            // Initialize all nodes
            foreach (string nodeID in graph.GetNodes().Keys)
            {
                distance[nodeID] = double.PositiveInfinity;
                previous[nodeID] = null;
                unvisited.Add(nodeID);
            }

            // Distance from start node to itself is 0
            distance[startNode] = 0;

            while (unvisited.Count > 0)
            {
                // Find the unvisited node with the smallest distance
                string currentNode = GetClosestNode(
                    unvisited,
                    distance
                );

                // No reachable node remains
                if (currentNode == null)
                {
                    break;
                }

                // Mark current node as visited
                unvisited.Remove(currentNode);

                // Stop when target is reached
                if (currentNode == targetNode)
                {
                    break;
                }

                // Check all neighbouring nodes
                foreach (Edge edge in graph.GetNeighbours(currentNode))
                {
                    if (!unvisited.Contains(edge.To))
                    {
                        continue;
                    }

                    // Calculate new possible distance
                    double newDistance =
                        distance[currentNode] + edge.Distance;

                    // Update if a shorter path is found
                    if (newDistance < distance[edge.To])
                    {
                        distance[edge.To] = newDistance;
                        previous[edge.To] = currentNode;
                    }
                }
            }

            // No path exists
            if (double.IsPositiveInfinity(distance[targetNode]))
            {
                return new List<string>();
            }

            totalDistance = distance[targetNode];

            // Reconstruct the path
            List<string> path =
                new List<string>();

            string current = targetNode;

            while (current != null)
            {
                path.Insert(0, current);
                current = previous[current];
            }

            return path;
        }


        // ============================================================
        // HELPER METHOD
        // ============================================================

        // Find the unvisited node with the smallest value.
        private static string GetClosestNode(
            HashSet<string> unvisited,
            Dictionary<string, double> values)
        {
            string closestNode = null;

            double smallestValue =
                double.PositiveInfinity;

            foreach (string nodeID in unvisited)
            {
                if (values[nodeID] < smallestValue)
                {
                    smallestValue = values[nodeID];
                    closestNode = nodeID;
                }
            }

            return closestNode;
        }


        // ============================================================
        // FASTEST PATH
        // ============================================================

        // Find the fastest path between two nodes
        // using TravelTime as the edge weight.
        public static List<string> FindFastestPath(
            Graph graph,
            string startNode,
            string targetNode,
            out double totalTime)
        {
            totalTime = 0;

            // Check whether both nodes exist
            if (!graph.GetNodes().ContainsKey(startNode) ||
                !graph.GetNodes().ContainsKey(targetNode))
            {
                return new List<string>();
            }

            Dictionary<string, double> travelTimes =
                new Dictionary<string, double>();

            Dictionary<string, string> previous =
                new Dictionary<string, string>();

            HashSet<string> unvisited =
                new HashSet<string>();

            // Initialize all nodes
            foreach (string nodeID in graph.GetNodes().Keys)
            {
                travelTimes[nodeID] =
                    double.PositiveInfinity;

                previous[nodeID] = null;

                unvisited.Add(nodeID);
            }

            // Starting node
            travelTimes[startNode] = 0;

            while (unvisited.Count > 0)
            {
                // Find the unvisited node with
                // the smallest travel time
                string currentNode = GetClosestNode(
                    unvisited,
                    travelTimes
                );

                // No reachable node remains
                if (currentNode == null)
                {
                    break;
                }

                // Mark current node as visited
                unvisited.Remove(currentNode);

                // Stop when target is reached
                if (currentNode == targetNode)
                {
                    break;
                }

                // Check neighbouring nodes
                foreach (Edge edge in graph.GetNeighbours(currentNode))
                {
                    if (!unvisited.Contains(edge.To))
                    {
                        continue;
                    }

                    // Calculate new travel time
                    double newTravelTime =
                        travelTimes[currentNode]
                        + edge.TravelTime;

                    // Update if a faster route is found
                    if (newTravelTime <
                        travelTimes[edge.To])
                    {
                        travelTimes[edge.To] =
                            newTravelTime;

                        previous[edge.To] =
                            currentNode;
                    }
                }
            }

            // No route found
            if (double.IsPositiveInfinity(
                travelTimes[targetNode]))
            {
                return new List<string>();
            }

            totalTime =
                travelTimes[targetNode];

            // Reconstruct the path
            List<string> path =
                new List<string>();

            string current = targetNode;

            while (current != null)
            {
                path.Insert(0, current);
                current = previous[current];
            }

            return path;
        }


        // ============================================================
        // CONSTRAINED ROUTING
        // ============================================================

        // Find the shortest-distance path while
        // ignoring roads above the congestion limit.
        public static List<string> FindConstrainedPath(
            Graph graph,
            string startNode,
            string targetNode,
            double maxCongestion,
            string excludedNode,
            string excludedEdgeID,
            out double totalDistance)
        {
            totalDistance = 0;

            // Check whether both nodes exist
            if (!graph.GetNodes().ContainsKey(startNode) ||
                !graph.GetNodes().ContainsKey(targetNode))
            {
                return new List<string>();
            }

            if (startNode == excludedNode ||
                targetNode == excludedNode)
            {
                return new List<string>();
            }

            Dictionary<string, double> distance =
                new Dictionary<string, double>();

            Dictionary<string, string> previous =
                new Dictionary<string, string>();

            HashSet<string> unvisited =
                new HashSet<string>();

            // Initialize all nodes
            foreach (string nodeID in graph.GetNodes().Keys)
            {
                distance[nodeID] =
                    double.PositiveInfinity;

                previous[nodeID] = null;

                unvisited.Add(nodeID);
            }

            distance[startNode] = 0;

            while (unvisited.Count > 0)
            {
                string currentNode = GetClosestNode(
                    unvisited,
                    distance
                );

                if (currentNode == null)
                {
                    break;
                }

                unvisited.Remove(currentNode);

                // Stop when target is reached
                if (currentNode == targetNode)
                {
                    break;
                }

                // Check neighbouring nodes
                foreach (Edge edge in graph.GetNeighbours(currentNode))
                {
                    if (!unvisited.Contains(edge.To))
                    {
                        continue;
                    }

                    // Ignore the excluded node
                    if (edge.To == excludedNode ||
                        edge.From == excludedNode)
                    {
                        continue;
                    }

                    // Ignore the excluded edge
                    if (edge.EdgeID == excludedEdgeID)
                    {
                        continue;
                    }

                    // Ignore roads above the congestion limit
                    if (edge.Congestion > maxCongestion)
                    {
                        continue;
                    }

                    // Calculate new possible distance
                    double newDistance =
                        distance[currentNode]
                        + edge.Distance;

                    // Update if a shorter path is found
                    if (newDistance < distance[edge.To])
                    {
                        distance[edge.To] =
                            newDistance;

                        previous[edge.To] =
                            currentNode;
                    }
                }
            }

            // No route exists
            if (double.IsPositiveInfinity(
                distance[targetNode]))
            {
                return new List<string>();
            }

            totalDistance =
                distance[targetNode];

            // Reconstruct the path
            List<string> path =
                new List<string>();

            string current = targetNode;

            while (current != null)
            {
                path.Insert(0, current);
                current = previous[current];
            }

            return path;
        }


        // ============================================================
        // TIME-BUDGET REACHABILITY
        // ============================================================

        // Find all nodes reachable from a starting node
        // within a given travel-time budget.
        public static Dictionary<string, double> FindReachableTravelTimes(Graph graph, string startNode, double timeBudget)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();
            if (!graph.GetNodes().ContainsKey(startNode) || timeBudget < 0 ||
                double.IsNaN(timeBudget) || double.IsInfinity(timeBudget))
                return result;

            Dictionary<string, double> times = new Dictionary<string, double>();
            HashSet<string> unvisited = new HashSet<string>();
            foreach (string id in graph.GetNodes().Keys)
            {
                times[id] = double.PositiveInfinity;
                unvisited.Add(id);
            }
            times[startNode] = 0;

            while (unvisited.Count > 0)
            {
                string current = GetClosestNode(unvisited, times);
                if (current == null || times[current] > timeBudget)
                    break;
                unvisited.Remove(current);

                foreach (Edge edge in graph.GetNeighbours(current))
                {
                    if (!unvisited.Contains(edge.To))
                        continue;
                    double candidate = times[current] + edge.TravelTime;
                    if (candidate < times[edge.To] && candidate <= timeBudget)
                        times[edge.To] = candidate;
                }
            }

            foreach (KeyValuePair<string, double> item in times)
                if (item.Value <= timeBudget)
                    result[item.Key] = item.Value;
            return result;
        }

        public static List<string> FindReachableNodes(
            Graph graph,
            string startNode,
            double timeBudget)
        {
            // Keep the original list-based API while reusing the travel-time calculation.
            return new List<string>(
                FindReachableTravelTimes(graph, startNode, timeBudget).Keys);
        }
    }
}