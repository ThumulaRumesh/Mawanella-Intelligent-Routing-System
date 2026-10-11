using System;
using System.Collections.Generic;

namespace Mawanella_Intelligent_Routing_System
{
    public class Dijkstra
    {
        // ============================================================
        // SHORTEST DISTANCE PATH
        // ============================================================

        // Find the shortest-distance path between two nodes.
        public static List<string> FindShortestPath(
            Graph graph,
            string startNode,
            string targetNode,
            out double totalDistance)
        {
            return FindPath(
                graph,
                startNode,
                targetNode,
                edge => edge.Distance,
                edge => true,
                out totalDistance);
        }

        // ============================================================
        // FASTEST PATH
        // ============================================================

        // Find the fastest path using TravelTime as the edge weight.
        public static List<string> FindFastestPath(
            Graph graph,
            string startNode,
            string targetNode,
            out double totalTime)
        {
            return FindPath(
                graph,
                startNode,
                targetNode,
                edge => edge.TravelTime,
                edge => true,
                out totalTime);
        }

        // ============================================================
        // CONSTRAINED ROUTING
        // ============================================================

        // Find the shortest-distance path while applying road constraints.
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

            return FindPath(
                graph,
                startNode,
                targetNode,
                edge => edge.Distance,
                edge =>
                    edge.From != excludedNode &&
                    edge.To != excludedNode &&
                    edge.EdgeID != excludedEdgeID &&
                    edge.Congestion <= maxCongestion,
                out totalDistance);
        }

        // Shared priority-queue implementation used by the route methods.
        // .NET's PriorityQueue removes the entry with the lowest priority first.
        private static List<string> FindPath(
            Graph graph,
            string startNode,
            string targetNode,
            Func<Edge, double> getWeight,
            Func<Edge, bool> edgeAllowed,
            out double totalCost)
        {
            totalCost = 0;

            if (!graph.GetNodes().ContainsKey(startNode) ||
                !graph.GetNodes().ContainsKey(targetNode))
            {
                return new List<string>();
            }

            Dictionary<string, double> distance = new Dictionary<string, double>();
            Dictionary<string, string?> previous = new Dictionary<string, string?>();

            foreach (string nodeID in graph.GetNodes().Keys)
            {
                distance[nodeID] = double.PositiveInfinity;
                previous[nodeID] = null;
            }

            PriorityQueue<string, double> priorityQueue =
                new PriorityQueue<string, double>();

            distance[startNode] = 0;
            priorityQueue.Enqueue(startNode, 0);

            while (priorityQueue.Count > 0)
            {
                if (!priorityQueue.TryDequeue(out string? currentNode, out double currentDistance) ||
                    currentNode == null)
                {
                    continue;
                }

                // Ignore an old queue entry if a better route was found later.
                if (currentDistance > distance[currentNode])
                {
                    continue;
                }

                if (currentNode == targetNode)
                {
                    break;
                }

                foreach (Edge edge in graph.GetNeighbours(currentNode))
                {
                    if (!edgeAllowed(edge))
                    {
                        continue;
                    }

                    double candidateDistance =
                        currentDistance + getWeight(edge);

                    if (candidateDistance < distance[edge.To])
                    {
                        distance[edge.To] = candidateDistance;
                        previous[edge.To] = currentNode;
                        priorityQueue.Enqueue(edge.To, candidateDistance);
                    }
                }
            }

            if (double.IsPositiveInfinity(distance[targetNode]))
            {
                return new List<string>();
            }

            totalCost = distance[targetNode];

            List<string> path = new List<string>();
            string? current = targetNode;

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

        // Find the minimum travel time to every node within the time budget.
        public static Dictionary<string, double> FindReachableTravelTimes(
            Graph graph,
            string startNode,
            double timeBudget)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();

            if (!graph.GetNodes().ContainsKey(startNode) ||
                timeBudget < 0 ||
                double.IsNaN(timeBudget) ||
                double.IsInfinity(timeBudget))
            {
                return result;
            }

            Dictionary<string, double> times = new Dictionary<string, double>();

            foreach (string nodeID in graph.GetNodes().Keys)
            {
                times[nodeID] = double.PositiveInfinity;
            }

            PriorityQueue<string, double> priorityQueue =
                new PriorityQueue<string, double>();

            times[startNode] = 0;
            priorityQueue.Enqueue(startNode, 0);

            while (priorityQueue.Count > 0)
            {
                if (!priorityQueue.TryDequeue(out string? currentNode, out double currentTime) ||
                    currentNode == null)
                {
                    continue;
                }

                // Skip stale entries and stop when the budget is exceeded.
                if (currentTime > times[currentNode])
                {
                    continue;
                }

                if (currentTime > timeBudget)
                {
                    break;
                }

                foreach (Edge edge in graph.GetNeighbours(currentNode))
                {
                    double candidateTime = currentTime + edge.TravelTime;

                    if (candidateTime <= timeBudget &&
                        candidateTime < times[edge.To])
                    {
                        times[edge.To] = candidateTime;
                        priorityQueue.Enqueue(edge.To, candidateTime);
                    }
                }
            }

            foreach (KeyValuePair<string, double> item in times)
            {
                if (item.Value <= timeBudget)
                {
                    result[item.Key] = item.Value;
                }
            }

            return result;
        }

        public static List<string> FindReachableNodes(
            Graph graph,
            string startNode,
            double timeBudget)
        {
            // Keep the original list-based API for existing callers.
            return new List<string>(
                FindReachableTravelTimes(graph, startNode, timeBudget).Keys);
        }
    }
}