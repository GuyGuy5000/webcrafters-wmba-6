using wmbaApp.Models;

namespace wmbaApp.ViewModels
{
    public class InningScoreKeepingVM
    {
        public List<PlayerScoreKeepingVM> Players { get; set; }
        public int CurrentBatter { get; set; } = 0;
<<<<<<< HEAD
<<<<<<< HEAD
=======
        public bool CurrentlyBatting { get; set; } = true;
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
=======
        public bool CurrentlyBatting { get; set; } = true;
=======
>>>>>>> 29e156e (fixed merged solution issue)
>>>>>>> 3b13cb3 (fixed merged solution issue)

        public int TotalRunsThisInning
            => Players.Sum(p => p.Runs);
        public int TotalOutsThisInning
<<<<<<< HEAD
<<<<<<< HEAD
        {
            get
            {
                return Players.Sum(p => p.Outs);
            }
            set
            {
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
=======
=======
>>>>>>> 3b13cb3 (fixed merged solution issue)
            => Players.Sum(p => p.Outs);

        //gets the players on each base including home base
        public PlayerScoreKeepingVM Batter
            => Players[CurrentBatter];
<<<<<<< HEAD
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
=======
=======
        {
            get
            {
                return Players.Sum(p => p.Outs);
            }
            set
            {
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
>>>>>>> 29e156e (fixed merged solution issue)
>>>>>>> 3b13cb3 (fixed merged solution issue)
        public PlayerScoreKeepingVM PlayerOnFirst
            => Players.FirstOrDefault(p => p.FirstBase && p != this.Batter);
        public PlayerScoreKeepingVM PlayerOnSecond
            => Players.FirstOrDefault(p => p.SecondBase && p != this.Batter);
        public PlayerScoreKeepingVM PlayerOnThird
            => Players.FirstOrDefault(p => p.ThirdBase && p != this.Batter);

<<<<<<< HEAD
        //constructor for JSON serialization
=======
        //unloaded inning constructor for debugging
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
        public InningScoreKeepingVM()
        {

        }

<<<<<<< HEAD
<<<<<<< HEAD
        //constructor for debugging
=======
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
=======
        //constructor for debugging
>>>>>>> c4ae6ca (Added pitch button and batter actions select list for scorekeeping.)
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
        public InningScoreKeepingVM(Player[] players)
        {
            this.Players = new List<PlayerScoreKeepingVM>(PlayerScoreKeepingVM.GetPlayers(players));
        }
    }
}
