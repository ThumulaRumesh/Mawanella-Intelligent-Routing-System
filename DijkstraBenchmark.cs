using System;
using System.Collections.Generic;

namespace Mawanella_Intelligent_Routing_System
{
    // Retains the original linear-search Dijkstra implementation only for
    // comparing its performance with the priority-queue implementation.
    internal static class DijkstraBenchmark
    {
        public static List<string> FindShortestPathLinear(
            Graph graph,
            string startNode,
            string targetNode,
            out double totalDistance)
        {
            totalDistance = 0;

            if (!graph.GetNodes().ContainsKey(startNode) ||
                !graph.GetNodes().ContainsKey(targetNode))
            {
                return new List<string>();
            }

            Dictionary<string, double> distance = new Dictionary<string, double>();
            Dictionary<string, string?> previous = new Dictionary<string, string?>();
            HashSet<string> unvisited = new HashSet<string>();

            foreach (string nodeID in graph.GetNodes().Keys)
            {
                distance[nodeID] = double.PositiveInfinity;
                previous[nodeID] = null;
                unvisited.Add(nodeID);
            }

            distance[startNode] = 0;

            while (unvisited.Count > 0)
            {
                string? currentNode = GetClosestNode(unvisited, distance);

                if (currentNode == null)
                {
                    break;
                }

                unvisited.Remove(currentNode);

                if (currentNode == targetNode)
                {
                    break;
                }

                foreach (Edge edge in graph.GetNeighbours(currentNode))
                {
                    if (!unvisited.Contains(edge.To))
                    {
                        continue;
                    }

                    double candidate = distance[currentNode] + edge.Distance;

                    if (candidate < distance[edge.To])
                    {
                        distance[edge.To] = candidate;
                        previous[edge.To] = currentNode;
                    }
                }
            }

            if (double.IsPositiveInfinity(distance[targetNode]))
            {
                return new List<string>();
            }

            totalDistance = distance[targetNode];
            List<string> path = new List<string>();
            string? current = targetNode;

            while (current != null)
            {
                path.Insert(0, current);
                current = previous[current];
            }

            return path;
        }

        private static string? GetClosestNode(
            HashSet<string> unvisited,
            Dictionary<string, double> values)
        {
            string? closestNode = null;
            double smallestValue = double.PositiveInfinity;

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
    }
}