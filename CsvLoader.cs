using System;
using System.IO;

namespace Mawanella_Intelligent_Routing_System
{
    public class CsvLoader
    {
        // Load nodes from a CSV file
        public static void LoadNodes(
            string filePath,
            Graph graph)
        {
            string[] lines =
                File.ReadAllLines(filePath);

            // Skip the header line
            for (int i = 1; i < lines.Length; i++)
            {
                string[] values =
                    lines[i].Split(',');

                string nodeID = values[0];
                string location = values[1];
                string roadType = values[2];

                Node node = new Node(
                    nodeID,
                    location,
                    roadType
                );

                graph.AddNode(node);
            }
        }


        // Load edges from a CSV file
        public static void LoadEdges(
            string filePath,
            Graph graph)
        {
            string[] lines =
                File.ReadAllLines(filePath);

            // Skip the header line
            for (int i = 1; i < lines.Length; i++)
            {
                string[] values =
                    lines[i].Split(',');

                string edgeID = values[0];
                string from = values[1];
                string to = values[2];

                double distance =
                    Convert.ToDouble(values[3]);

                double speedLimit =
                    Convert.ToDouble(values[4]);

                double travelTime =
                    Convert.ToDouble(values[5]);

                double congestion =
                    Convert.ToDouble(values[6]);

                string roadType = values[7];

                Edge edge = new Edge(
                    edgeID,
                    from,
                    to,
                    distance,
                    speedLimit,
                    congestion,
                    travelTime,
                    roadType
                );

                graph.AddEdge(edge);
            }
        }
    }
}