namespace Mawanella_Intelligent_Routing_System
{
    public class Node
    {
        public string NodeID { get; set; }
        public string Location { get; set; }
        public string RoadType { get; set; }

        public Node(string nodeID, string location, string roadType)
        {
            NodeID = nodeID;
            Location = location;
            RoadType = roadType;
        }
    }
}
