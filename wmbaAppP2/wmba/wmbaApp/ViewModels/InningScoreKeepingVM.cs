using wmbaApp.Models;

namespace wmbaApp.ViewModels
{
    public class InningScoreKeepingVM
    {
        public List<PlayerScoreKeepingVM> Players { get; set; }
        public List<PlayByPlay> PlayByPlays {  get; set; } = new List<PlayByPlay>();
        public int HomeTeamScore { get; set; } = 0;
        public int AwayTeamScore { get; set; } = 0;
        public int CurrentBatter { get; set; } = 0;
        public bool InningTop { get; set; } = true;
        public bool HandleFirstBase {  get; set; } = false;
        public bool HandleSecondBase {  get; set; } = false;
        public bool HandleThirdBase {  get; set; } = false;


        public int TotalRunsThisInning
            => Players.Sum(p => p.Runs);
        public int TotalOutsThisInning
        {
            get
            {
                return Players.Sum(p => p.Outs);
            }
        }

        //gets the players on each base including home base
        public PlayerScoreKeepingVM Batter
        {
            get
            {
                if (CurrentBatter >= Players.Count)
                {
                    CurrentBatter = 0;
                }
                return Players[CurrentBatter];
            }
        }
        public PlayerScoreKeepingVM PlayerOnFirst
            => Players.FirstOrDefault(p => p.FirstBase && p != this.Batter);
        public PlayerScoreKeepingVM PlayerOnSecond
            => Players.FirstOrDefault(p => p.SecondBase && p != this.Batter);
        public PlayerScoreKeepingVM PlayerOnThird
            => Players.FirstOrDefault(p => p.ThirdBase && p != this.Batter);

        //constructor for JSON serialization
        public InningScoreKeepingVM()
        {

        }

        //constructor for debugging
        public InningScoreKeepingVM(string debug)
        {
            this.Players = new List<PlayerScoreKeepingVM>()
            {
                new PlayerScoreKeepingVM("Player One", 1),
                new PlayerScoreKeepingVM("Player Two", 2),
                new PlayerScoreKeepingVM("Player Three", 3),
                new PlayerScoreKeepingVM("Player Four", 4),
                new PlayerScoreKeepingVM("Player Five", 5),
                new PlayerScoreKeepingVM("Player Six", 6),
                new PlayerScoreKeepingVM("Player Seven", 7),
                new PlayerScoreKeepingVM("Player Eight", 8),
                new PlayerScoreKeepingVM("Player Nine", 9)
            };
        }

        //loaded inning constructor with players param
        public InningScoreKeepingVM(List<PlayerScoreKeepingVM> players)
        {
            this.Players = players;
        }
    }
}
