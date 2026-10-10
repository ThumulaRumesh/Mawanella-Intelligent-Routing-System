using Mawanella_Intelligent_Routing_System;
using Xunit;

namespace MawanellaRouting.Tests;

public class DijkstraTests
{
    private static Graph CreateGraph()
    {
        var graph = new Graph();
        foreach (string id in new[] { "A", "B", "C", "D", "E" })
            graph.AddNode(new Node(id, id, "Test road"));

        graph.AddEdge(new Edge("AB", "A", "B", 2, 40, 10, 5, "Test road"));
        graph.AddEdge(new Edge("AC", "A", "C", 1, 40, 80, 2, "Test road"));
        graph.AddEdge(new Edge("CB", "C", "B", 2, 40, 20, 2, "Test road"));
        graph.AddEdge(new Edge("BD", "B", "D", 2, 40, 20, 3, "Test road"));
        graph.AddEdge(new Edge("CD", "C", "D", 8, 40, 5, 1, "Test road"));
        return graph;
    }

    [Fact]
    public void ShortestPath_ReturnsMinimumDistanceRoute()
    {
        var route = Dijkstra.FindShortestPath(CreateGraph(), "A", "D", out double distance);
        Assert.Equal(new[] { "A", "B", "D" }, route);
        Assert.Equal(4, distance);
    }

    [Fact]
    public void FastestPath_ReturnsMinimumTravelTimeRoute()
    {
        var route = Dijkstra.FindFastestPath(CreateGraph(), "A", "D", out double time);
        Assert.Equal(new[] { "A", "C", "D" }, route);
        Assert.Equal(3, time);
    }

    [Fact]
    public void StartEqualsTarget_ReturnsSingleNodeAndZeroCost()
    {
        var route = Dijkstra.FindShortestPath(CreateGraph(), "A", "A", out double distance);
        Assert.Equal(new[] { "A" }, route);
        Assert.Equal(0, distance);
    }

    [Fact]
    public void InvalidStartingNode_ReturnsEmptyRouteAndZeroCost()
    {
        var route = Dijkstra.FindShortestPath(CreateGraph(), "UNKNOWN", "D", out double distance);
        Assert.Empty(route);
        Assert.Equal(0, distance);
    }

    [Fact]
    public void UnreachableDestination_ReturnsEmptyRoute()
    {
        var route = Dijkstra.FindShortestPath(CreateGraph(), "A", "E", out double distance);
        Assert.Empty(route);
        Assert.Equal(0, distance);
    }

    [Fact]
    public void ConstrainedRoute_RespectsCongestionLimit()
    {
        var route = Dijkstra.FindConstrainedPath(
            CreateGraph(), "A", "D", 30, null, null, out double distance);
        Assert.Equal(new[] { "A", "B", "D" }, route);
        Assert.Equal(4, distance);
    }

    [Fact]
    public void ConstrainedRoute_AvoidsExcludedNode()
    {
        var route = Dijkstra.FindConstrainedPath(
            CreateGraph(), "A", "D", 100, "B", null, out double distance);
        Assert.Equal(new[] { "A", "C", "D" }, route);
        Assert.Equal(9, distance);
    }

    [Fact]
    public void ConstrainedRoute_AvoidsExcludedUndirectedEdge()
    {
        var route = Dijkstra.FindConstrainedPath(
            CreateGraph(), "A", "D", 100, null, "AB", out double distance);
        Assert.Equal(new[] { "A", "C", "B", "D" }, route);
        Assert.Equal(5, distance);
    }

    [Fact]
    public void Reachability_ReturnsOnlyNodesWithinTravelTimeBudget()
    {
        var reachable = Dijkstra.FindReachableTravelTimes(CreateGraph(), "A", 3);
        Assert.Equal(3, reachable.Count);
        Assert.Equal(0, reachable["A"]);
        Assert.Equal(2, reachable["C"]);
        Assert.Equal(3, reachable["D"]);
        Assert.False(reachable.ContainsKey("B"));
        Assert.False(reachable.ContainsKey("E"));
    }

    [Fact]
    public void DepthFirstTraversal_DoesNotReachDisconnectedNode()
    {
        var visited = Traversal.DFT(CreateGraph(), "A");
        Assert.Contains("A", visited);
        Assert.Contains("B", visited);
        Assert.Contains("C", visited);
        Assert.Contains("D", visited);
        Assert.DoesNotContain("E", visited);
    }

    [Fact]
    public void NodeRemoval_CountsReachableNodesWithoutRemovedNode()
    {
        int count = Traversal.CountReachableAfterRemovingNode(CreateGraph(), "A", "B");
        Assert.Equal(3, count); // A, C and D remain reachable.
    }

    [Fact]
    public void EdgeRemoval_StillUsesAlternativeConnections()
    {
        int count = Traversal.CountReachableAfterRemovingEdge(CreateGraph(), "A", "A", "B");
        Assert.Equal(4, count); // A, B, C and D remain connected; E is isolated.
    }
}
