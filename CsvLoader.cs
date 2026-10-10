using System;
using System.Globalization;
using System.IO;

namespace Mawanella_Intelligent_Routing_System
{
    public class CsvLoader
    {
        // Load nodes from a CSV file and reject malformed records.
        public static void LoadNodes(string filePath, Graph graph)
        {
            if (graph == null) throw new ArgumentNullException(nameof(graph));

            string[] lines = File.ReadAllLines(filePath);
            if (lines.Length == 0 || lines[0].Trim() != "NodeID,Location,RoadType")
                throw new InvalidDataException("Invalid node CSV header in '" + filePath + "'.");

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                string[] values = lines[i].Split(',');
                if (values.Length != 3)
                    throw RowError(filePath, i + 1, "expected 3 comma-separated fields");

                string nodeID = values[0].Trim();
                string location = values[1].Trim();
                string roadType = values[2].Trim();

                if (nodeID.Length == 0 || location.Length == 0 || roadType.Length == 0)
                    throw RowError(filePath, i + 1, "node ID, location and road type are required");

                if (graph.GetNode(nodeID) != null)
                    throw RowError(filePath, i + 1, "duplicate node ID '" + nodeID + "'");

                graph.AddNode(new Node(nodeID, location, roadType));
            }
        }

        // Load edges only when their endpoints and numeric values are valid.
        public static void LoadEdges(string filePath, Graph graph)
        {
            if (graph == null) throw new ArgumentNullException(nameof(graph));

            string[] lines = File.ReadAllLines(filePath);
            const string expectedHeader =
                "EdgeID,From,To,Distance_km,SpeedLimit_kmh,TravelTime_min,Congestion_percent,RoadType";

            if (lines.Length == 0 || lines[0].Trim() != expectedHeader)
                throw new InvalidDataException("Invalid edge CSV header in '" + filePath + "'.");

            var seenEdgeIds = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                string[] values = lines[i].Split(',');
                if (values.Length != 8)
                    throw RowError(filePath, i + 1, "expected 8 comma-separated fields");

                string edgeID = values[0].Trim();
                string from = values[1].Trim();
                string to = values[2].Trim();
                string roadType = values[7].Trim();

                if (edgeID.Length == 0 || from.Length == 0 || to.Length == 0 || roadType.Length == 0)
                    throw RowError(filePath, i + 1, "edge ID, endpoints and road type are required");

                if (from == to)
                    throw RowError(filePath, i + 1, "self-loop roads are not supported");

                if (graph.GetNode(from) == null || graph.GetNode(to) == null)
                    throw RowError(filePath, i + 1, "both endpoint IDs must exist in the node dataset");

                if (!seenEdgeIds.Add(edgeID))
                    throw RowError(filePath, i + 1, "duplicate edge ID '" + edgeID + "'");

                foreach (Edge existing in graph.GetEdges())
                    if (existing.EdgeID.Equals(edgeID, StringComparison.OrdinalIgnoreCase))
                        throw RowError(filePath, i + 1, "duplicate edge ID '" + edgeID + "'");

                double distance = ParseNumber(values[3], "distance", filePath, i + 1);
                double speedLimit = ParseNumber(values[4], "speed limit", filePath, i + 1);
                double travelTime = ParseNumber(values[5], "travel time", filePath, i + 1);
                double congestion = ParseNumber(values[6], "congestion", filePath, i + 1);

                if (distance <= 0)
                    throw RowError(filePath, i + 1, "distance must be greater than zero");
                if (speedLimit <= 0)
                    throw RowError(filePath, i + 1, "speed limit must be greater than zero");
                if (travelTime <= 0)
                    throw RowError(filePath, i + 1, "travel time must be greater than zero");
                if (congestion < 0 || congestion > 100)
                    throw RowError(filePath, i + 1, "congestion must be between 0 and 100");

                graph.AddEdge(new Edge(edgeID, from, to, distance, speedLimit,
                    congestion, travelTime, roadType));
            }
        }

        private static double ParseNumber(string text, string field, string filePath, int rowNumber)
        {
            if (!double.TryParse(text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                || double.IsNaN(value) || double.IsInfinity(value))
            {
                throw RowError(filePath, rowNumber, "invalid numeric " + field + " value '" + text + "'");
            }

            return value;
        }

        private static InvalidDataException RowError(string filePath, int rowNumber, string message)
        {
            return new InvalidDataException(
                "Invalid data in '" + filePath + "' at row " + rowNumber + ": " + message + ".");
        }
    }
}