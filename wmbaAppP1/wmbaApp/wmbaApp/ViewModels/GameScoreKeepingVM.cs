namespace wmbaApp.ViewModels
{
    /// <summary>
    ///  A class to represent data used in score keeping
    ///  - Nadav Hilu
    /// </summary>
    public class GameScoreKeepingVM
    {
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
        public int GameID { get; set; }
=======
        public int GameID {  get; set; }
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
=======
        public int GameID {  get; set; }
=======
        public int GameID { get; set; }
>>>>>>> 29e156e (fixed merged solution issue)
>>>>>>> 3b13cb3 (fixed merged solution issue)
=======
        public int GameID { get; set; }
>>>>>>> b47d29c (reset main branch to Nadav)
        public string HomeTeamName { get; set; }
        public string AwayTeamName { get; set; }
        public int HomeTeamScore { get; set; } = 0;
        public int AwayTeamScore { get; set; } = 0;
        //public LineUp LineUp {  get; set; } 
        //public PlayByPlay PlayByPlay {  get; set; } 
        public int CurrentInning { get; set; } = 0;
        public InningScoreKeepingVM[] Innings { get; set; } = new InningScoreKeepingVM[9];

        public string Score
            => Innings.Sum(i => i?.TotalRunsThisInning).ToString();

    }
}
