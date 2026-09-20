namespace Mawanella_Intelligent_Routing_System
{
    public class Edge
    {
        public string EdgeID { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public double Distance { get; set; }
        public double SpeedLimit { get; set; }
        public double Congestion { get; set; }
        public double TravelTime { get; set; }
        public string RoadType { get; set; }

        public Edge(string edgeID, string from, string to,
                    double distance, double speedLimit,
                    double congestion, double travelTime,
                    string roadType)
        {
            EdgeID = edgeID;
            From = from;
            To = to;
            Distance = distance;
            SpeedLimit = speedLimit;
            Congestion = congestion;
            TravelTime = travelTime;
            RoadType = roadType;
        }
    }
}
