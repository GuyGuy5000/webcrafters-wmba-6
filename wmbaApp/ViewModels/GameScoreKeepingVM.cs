using wmbaApp.Models;

namespace wmbaApp.ViewModels
{
    /// <summary>
    ///  A class to represent data used in score keeping
    ///  - Nadav Hilu
    /// </summary>
    public class GameScoreKeepingVM
    {
        public int GameID { get; set; }
        public string HomeTeamName { get; set; }
        public string AwayTeamName { get; set; }
        public int HomeTeamScore { get; set; } = 0;
        public int AwayTeamScore { get; set; } = 0;
        public List<PlayerScoreKeepingVM> LineUp { get; set; }
        public int CurrentInning { get; set; } = 0;
        public List<InningScoreKeepingVM> Innings { get; set; } = new List<InningScoreKeepingVM>();

        public string Score
            => Innings.Sum(i => i?.TotalRunsThisInning).ToString();

        //empty constructor for JSON serialization
        public GameScoreKeepingVM()
        {

        }

        public GameScoreKeepingVM(int gameID, string homeTeamName, string awayTeamName, List<PlayerScoreKeepingVM> lineUp)
        {
            this.GameID = gameID;
            this.HomeTeamName = homeTeamName;
            this.AwayTeamName = awayTeamName;
            this.HomeTeamScore = 0;
            this.AwayTeamScore = 0;
            this.LineUp = lineUp;
            this.CurrentInning = 0;
        }

    }
}
