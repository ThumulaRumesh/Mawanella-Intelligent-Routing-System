using System.Collections.Generic;

namespace Mawanella_Intelligent_Routing_System
{
    public class Traversal
    {
        // Depth-First Traversal of the graph
        public static List<string> DFT(
            Graph graph,
            string startNode)
        {
            List<string> visited =
                new List<string>();

            HashSet<string> visitedSet =
                new HashSet<string>();

            Stack<string> stack =
                new Stack<string>();

            // Check whether the starting node exists
            if (!graph.GetNodes().ContainsKey(startNode))
            {
                return visited;
            }

            stack.Push(startNode);

            while (stack.Count > 0)
            {
                string currentNode =
                    stack.Pop();

                if (visitedSet.Contains(currentNode))
                {
                    continue;
                }

                visitedSet.Add(currentNode);
                visited.Add(currentNode);

                foreach (
                    Edge edge in
                    graph.GetNeighbours(currentNode))
                {
                    if (!visitedSet.Contains(edge.To))
                    {
                        stack.Push(edge.To);
                    }
                }
            }

            return visited;
        }


        // Count reachable nodes after removing a specific node
        public static int CountReachableAfterRemovingNode(
            Graph graph,
            string startNode,
            string removedNode)
        {
            // Check whether the starting node exists
            if (!graph.GetNodes().ContainsKey(startNode))
            {
                return 0;
            }

            // If the starting node itself is removed
            if (startNode == removedNode)
            {
                return 0;
            }

            HashSet<string> visited =
                new HashSet<string>();

            Stack<string> stack =
                new Stack<string>();

            stack.Push(startNode);

            while (stack.Count > 0)
            {
                string currentNode =
                    stack.Pop();

                if (currentNode == removedNode)
                {
                    continue;
                }

                if (visited.Contains(currentNode))
                {
                    continue;
                }

                visited.Add(currentNode);

                foreach (
                    Edge edge in
                    graph.GetNeighbours(currentNode))
                {
                    if (edge.To != removedNode &&
                        !visited.Contains(edge.To))
                    {
                        stack.Push(edge.To);
                    }
                }
            }

            return visited.Count;
        }


        // Count reachable nodes after removing a specific edge
        public static int CountReachableAfterRemovingEdge(
            Graph graph,
            string startNode,
            string fromNode,
            string toNode)
        {
            // Check whether the starting node exists
            if (!graph.GetNodes().ContainsKey(startNode))
            {
                return 0;
            }

            HashSet<string> visited =
                new HashSet<string>();

            Stack<string> stack =
                new Stack<string>();

            stack.Push(startNode);

            while (stack.Count > 0)
            {
                string currentNode =
                    stack.Pop();

                if (visited.Contains(currentNode))
                {
                    continue;
                }

                visited.Add(currentNode);

                foreach (
                    Edge edge in
                    graph.GetNeighbours(currentNode))
                {
                    // Ignore the selected edge in both directions
                    if ((edge.From == fromNode &&
                         edge.To == toNode) ||
                        (edge.From == toNode &&
                         edge.To == fromNode))
                    {
                        continue;
                    }

                    if (!visited.Contains(edge.To))
                    {
                        stack.Push(edge.To);
                    }
                }
            }

            return visited.Count;
        }
    }
}