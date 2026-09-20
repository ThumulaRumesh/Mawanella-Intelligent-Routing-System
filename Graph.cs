using System.Collections.Generic;

namespace Mawanella_Intelligent_Routing_System
{
    public class Graph
    {
        private Dictionary<string, Node> nodes;
        private Dictionary<string, List<Edge>> adjacencyList;
        private List<Edge> edges;

        public Graph()
        {
            nodes = new Dictionary<string, Node>();
            adjacencyList = new Dictionary<string, List<Edge>>();
            edges = new List<Edge>();
        }

        // Add a node to the graph
        public void AddNode(Node node)
        {
            if (!nodes.ContainsKey(node.NodeID))
            {
                nodes.Add(node.NodeID, node);
                adjacencyList.Add(
                    node.NodeID,
                    new List<Edge>()
                );
            }
        }

        // Add an undirected edge to the graph
        public void AddEdge(Edge edge)
        {
            if (adjacencyList.ContainsKey(edge.From) &&
                adjacencyList.ContainsKey(edge.To))
            {
                // Store the original edge once
                edges.Add(edge);

                // Add edge in the original direction
                adjacencyList[edge.From].Add(edge);

                // Create the reverse edge
                Edge reverseEdge = new Edge(
                    edge.EdgeID,
                    edge.To,
                    edge.From,
                    edge.Distance,
                    edge.SpeedLimit,
                    edge.Congestion,
                    edge.TravelTime,
                    edge.RoadType
                );

                // Add edge in the reverse direction
                adjacencyList[edge.To].Add(reverseEdge);
            }
        }

        // Get a node by its ID
        // Average Time: O(1)
        // Auxiliary Space: O(1)
        public Node GetNode(string nodeID)
        {
            if (nodes.ContainsKey(nodeID))
            {
                return nodes[nodeID];
            }

            return null;
        }

        // Get the neighbours of a node
        // Average Time: O(1)
        // Auxiliary Space: O(1)
        public List<Edge> GetNeighbours(string nodeID)
        {
            if (adjacencyList.ContainsKey(nodeID))
            {
                return adjacencyList[nodeID];
            }

            return new List<Edge>();
        }

        // Get all nodes
        // Time: O(1)
        // Auxiliary Space: O(1)
        public Dictionary<string, Node> GetNodes()
        {
            return nodes;
        }

        // Get all original edges
        // Used for critical-edge testing
        public List<Edge> GetEdges()
        {
            return edges;
        }
    }
}