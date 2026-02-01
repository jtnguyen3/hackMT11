namespace HackMT2026.Models
{
    public class PitcherStats
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int TotalPitches { get; set; }
        public int TotalAccurate { get; set; }

        // Key = Pitch Type (e.g. "Fastball", "Curveball")
        public Dictionary<string, PitchTypeStats> PitchTypes { get; set; }
            = new Dictionary<string, PitchTypeStats>();
    }

}