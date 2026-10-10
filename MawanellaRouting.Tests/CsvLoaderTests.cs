using System;
using System.IO;
using Mawanella_Intelligent_Routing_System;
using Xunit;

namespace MawanellaRouting.Tests;

public class CsvLoaderTests
{
    [Fact]
    public void LoadNodes_AcceptsSyntheticDatasetHeader()
    {
        string filePath = Path.Combine(Path.GetTempPath(), $"synthetic-nodes-{Guid.NewGuid():N}.csv");

        try
        {
            File.WriteAllText(filePath,
                "NodeID,LocationName,RoadOrArea\nN0001,N0001,Synthetic\n");

            var graph = new Graph();
            CsvLoader.LoadNodes(filePath, graph);

            Assert.NotNull(graph.GetNode("N0001"));
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
