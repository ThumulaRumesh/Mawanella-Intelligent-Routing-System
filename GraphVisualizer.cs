using System;
using System.IO;
using System.Text;

namespace Mawanella_Intelligent_Routing_System
{
    public class GraphVisualizer
    {
        public static void CreateDotFile(
            Graph graph,
            string outputFile)
        {
            StringBuilder dot = new StringBuilder();

            dot.AppendLine("graph MawanellaRoadNetwork");
            dot.AppendLine("{");

            dot.AppendLine("    node [shape=circle, style=filled, fillcolor=lightblue];");
            dot.AppendLine("    edge [color=gray];");

            foreach (var node in graph.GetNodes().Values)
            {
                dot.AppendLine(
                    $"    {node.NodeID} [label=\"{node.NodeID}\"];"
                );
            }

            foreach (var node in graph.GetNodes().Values)
            {
                foreach (Edge edge in graph.GetNeighbours(node.NodeID))
                {
                    // Prevent drawing the same undirected edge twice
                    if (string.Compare(edge.From, edge.To) < 0)
                    {
                        dot.AppendLine(
                            $"    {edge.From} -- {edge.To};"
                        );
                    }
                }
            }

            dot.AppendLine("}");

            File.WriteAllText(outputFile, dot.ToString());

            Console.WriteLine(
                "\nGraph visualization file created: " + outputFile
            );
        }
    }
}