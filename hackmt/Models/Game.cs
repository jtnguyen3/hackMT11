namespace HackMT2026.Models
{
    public class Game

    {
        public int GameId { get; set; }
        public string GameDate { get; set; } = string.Empty;

        // Home team
        public int HomeTeamId { get; set; }
        public int HomeTeamSeason { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public string HomeAgeGroup { get; set; } = string.Empty;

        // Away team
        public int AwayTeamId { get; set; }
        public int AwayTeamSeason { get; set; }
        public string AwayTeamName { get; set; } = string.Empty;
        public string AwayAgeGroup { get; set; } = string.Empty;
    }

}

