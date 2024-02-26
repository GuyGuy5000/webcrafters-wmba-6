//Done By: Emmanuel James, Nadav Hilu

using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
<<<<<<< HEAD
<<<<<<< HEAD
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
=======
using System.Linq;
using System.Text;
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
=======
using System.Linq;
using System.Text;
=======
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
>>>>>>> 29e156e (fixed merged solution issue)
>>>>>>> 3b13cb3 (fixed merged solution issue)
using System.Threading.Tasks;
using wmbaApp.Models;

namespace wmbaApp.ViewModels
{
    public enum AtBatOutcome
    {
<<<<<<< HEAD
<<<<<<< HEAD
        Single,
        Double,
        Triple,
        HomeRun,
        Ball,
        Strike,
        FoulBall,
        FoulTipOut,
        HitByPitch,
        IntentionalWalk,
        CatcherInterference,

=======
=======
>>>>>>> 3b13cb3 (fixed merged solution issue)
        Ball,
        CalledStrike,
        SwingAndMiss,
        FoulBall,
        FoulTipOut,
        HitByPitch,
        IntentionalBall,
        IntentionalWalk,
        CatcherInterference,
        IllegalPitch,
        Walk,
        Strikeout,
        Hit,
        Single,
        Double,
        Triple,
        HomeRun
<<<<<<< HEAD
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
=======
=======
        Single,
        Double,
        Triple,
        HomeRun,
        Ball,
        Strike,
        FoulBall,
        FoulTipOut,
        HitByPitch,
        IntentionalWalk,
        CatcherInterference,

>>>>>>> 29e156e (fixed merged solution issue)
>>>>>>> 3b13cb3 (fixed merged solution issue)
    }

    public class PlayerScoreKeepingVM
    {
<<<<<<< HEAD
<<<<<<< HEAD
        //Fields and Properties
        public int PlateAppearances { get; set; } = 0;
        public int AtBats { get; set; } = 0;
        public int Hits { get; set; } = 0;
        public int Singles { get; set; } = 0;
        public int Doubles { get; set; } = 0;
        public int Triples { get; set; } = 0;
        public int HR { get; set; } = 0;
        public int RBI { get; set; } = 0;
        public int BB { get; set; } = 0;
        public int HitByPitches { get; set; } = 0;
        public int Sacrifices { get; set; } = 0;
        public int CatcherInterference { get; set; } = 0;
        public int Balls { get; set; } = 0;
        public int Strikes { get; set; } = 0;
        public int Fouls { get; set; } = 0;
        public int FoulStrikes { get; set; } = 0; //used to subtract from pitch count when adding strikes, balls, and fouls
        public int Runs { get; set; } = 0;
        public int Outs { get; set; } = 0;
=======
=======
>>>>>>> 3b13cb3 (fixed merged solution issue)
        // Fields and Properties
        public int AtBats = 0;
        public int Hits = 0;
        public int Singles = 0;
        public int Doubles = 0;
        public int Triples = 0;
        public int HomeRuns = 0;
        public int RBI = 0;
        public int PlateAppearances = 0;
        public int Walks = 0;
        public int HitByPitches = 0;
        public int Sacrifices = 0;
        public int CatcherInterference = 0;
        public int Balls = 0;
        public int Strikes = 0;
        public int Fouls = 0;
        public int Runs = 0;
        public int Outs = 0;
<<<<<<< HEAD
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
=======
=======
        //Fields and Properties
        public int PlateAppearances { get; set; } = 0;
        public int AtBats { get; set; } = 0;
        public int Hits { get; set; } = 0;
        public int Singles { get; set; } = 0;
        public int Doubles { get; set; } = 0;
        public int Triples { get; set; } = 0;
        public int HR { get; set; } = 0;
        public int RBI { get; set; } = 0;
        public int BB { get; set; } = 0;
        public int HitByPitches { get; set; } = 0;
        public int Sacrifices { get; set; } = 0;
        public int CatcherInterference { get; set; } = 0;
        public int Balls { get; set; } = 0;
        public int Strikes { get; set; } = 0;
        public int Fouls { get; set; } = 0;
        public int FoulStrikes { get; set; } = 0; //used to subtract from pitch count when adding strikes, balls, and fouls
        public int Runs { get; set; } = 0;
        public int Outs { get; set; } = 0;
>>>>>>> 29e156e (fixed merged solution issue)
>>>>>>> 3b13cb3 (fixed merged solution issue)
        public bool FirstBase { get; set; }
        public bool SecondBase { get; set; }
        public bool ThirdBase { get; set; }
        public bool BasesLoaded => FirstBase && SecondBase && ThirdBase;
        public string Name { get; set; }
<<<<<<< HEAD
<<<<<<< HEAD
        public int ID { get; set; }
=======
        public int ID {  get; set; }
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
=======
        public int ID {  get; set; }
=======
        public int ID { get; set; }
>>>>>>> 29e156e (fixed merged solution issue)
>>>>>>> 3b13cb3 (fixed merged solution issue)
        public List<AtBatOutcome> AtBatActions { get; set; } = new List<AtBatOutcome>();


        public PlayerScoreKeepingVM()
        {

        }
        public PlayerScoreKeepingVM(string name, int ID)
        {
            this.Name = name;
            this.ID = ID;
            this.AtBatActions = new List<AtBatOutcome>();
        }

        // Constructor that instantiates the fields.
        public PlayerScoreKeepingVM(int atBats, int hits, int singles, int doubles, int triples, int homeRuns, int plateAppearances,
            int walks, int hitsByPitches, int sacrifices, int catcherInterference, int ballsCount, int strikesCount,
            int foulsCount, int runsCount, int outsCount)
        {
            this.AtBats = atBats;
            this.Hits = hits;
            this.Singles = singles;
            this.Doubles = doubles;
            this.Triples = triples;
<<<<<<< HEAD
<<<<<<< HEAD
            this.HR = homeRuns;
            this.PlateAppearances = plateAppearances;
            this.BB = walks;
=======
            this.HomeRuns = homeRuns;
            this.PlateAppearances = plateAppearances;
            this.Walks = walks;
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
=======
            this.HomeRuns = homeRuns;
            this.PlateAppearances = plateAppearances;
            this.Walks = walks;
=======
            this.HR = homeRuns;
            this.PlateAppearances = plateAppearances;
            this.BB = walks;
>>>>>>> 29e156e (fixed merged solution issue)
>>>>>>> 3b13cb3 (fixed merged solution issue)
            this.HitByPitches = hitsByPitches;
            this.Sacrifices = sacrifices;
            this.CatcherInterference = catcherInterference;
            this.Runs = runsCount;
            this.Fouls = foulsCount;
            this.Balls = ballsCount;
            this.Outs = outsCount;
            this.Strikes = strikesCount;
        }

        // Does the total AtBats
        public int PAtBats
<<<<<<< HEAD
<<<<<<< HEAD
            => PlateAppearances - BB - HitByPitches - Sacrifices - CatcherInterference;

        //Calculates pitch count
        public int PitchCount
            => Strikes + BB + Fouls - FoulStrikes;
=======
            => PlateAppearances - Walks - HitByPitches - Sacrifices - CatcherInterference;
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
=======
            => PlateAppearances - Walks - HitByPitches - Sacrifices - CatcherInterference;
=======
            => PlateAppearances - BB - HitByPitches - Sacrifices - CatcherInterference;

        //Calculates pitch count
        public int PitchCount
            => Strikes + BB + Fouls - FoulStrikes;
>>>>>>> 29e156e (fixed merged solution issue)
>>>>>>> 3b13cb3 (fixed merged solution issue)

        // Calculates OPS
        public double CalculateOBP()
        {
<<<<<<< HEAD
<<<<<<< HEAD
            double OPS = (double)(Singles + Doubles * 2 + Triples * 3 + HR * 4) / AtBats;
=======
            double OPS = (double)(Singles + Doubles * 2 + Triples * 3 + HomeRuns * 4) / AtBats;
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
=======
            double OPS = (double)(Singles + Doubles * 2 + Triples * 3 + HomeRuns * 4) / AtBats;
=======
            double OPS = (double)(Singles + Doubles * 2 + Triples * 3 + HR * 4) / AtBats;
>>>>>>> 29e156e (fixed merged solution issue)
>>>>>>> 3b13cb3 (fixed merged solution issue)
            return Math.Round(OPS, 3);
        }

        // Calculates OBP
        public double CalculateSLG()
        {
<<<<<<< HEAD
<<<<<<< HEAD
            double OBP = (double)(Hits + BB + HitByPitches) / (AtBats + BB + HitByPitches + Sacrifices);
=======
            double OBP = (double)(Hits + Walks + HitByPitches) / (AtBats + Walks + HitByPitches + Sacrifices);
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
=======
            double OBP = (double)(Hits + Walks + HitByPitches) / (AtBats + Walks + HitByPitches + Sacrifices);
=======
            double OBP = (double)(Hits + BB + HitByPitches) / (AtBats + BB + HitByPitches + Sacrifices);
>>>>>>> 29e156e (fixed merged solution issue)
>>>>>>> 3b13cb3 (fixed merged solution issue)
            return Math.Round(OBP, 3);
        }

        // Calculates OPS
        public double CalculateOPS()
        {
            double OPS = (double)(CalculateOBP() + CalculateSLG());
            return Math.Round(OPS, 3);
        }

        // Calculates the B.Avg
        public double CalculateBAvg()
        {
            int totalAtBats = PAtBats;

            if (totalAtBats == 0)
            {
                return 0.0;
            }

            double result = (double)Hits / totalAtBats;
            return Math.Round(result, 3);
        }

        // PLAYER ACTIONS

        private void RecordHit(AtBatOutcome outcome)
        {
            if (outcome == AtBatOutcome.Single)
            {
                FirstBase = true;
                SecondBase = false;
                ThirdBase = false;

                Singles++;
            }
            else if (outcome == AtBatOutcome.Double)
            {
                FirstBase = false;
                SecondBase = true;
                ThirdBase = false;
                Doubles++;
            }
            else if (outcome == AtBatOutcome.Triple)
            {
                FirstBase = false;
                SecondBase = false;
                ThirdBase = true;
                Triples++;
            }
            else
            {
                FirstBase = false;
                SecondBase = false;
                ThirdBase = false;
                Runs++;
<<<<<<< HEAD
<<<<<<< HEAD
                HR++;
=======
                HomeRuns++;
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
=======
                HomeRuns++;
=======
                HR++;
>>>>>>> 29e156e (fixed merged solution issue)
>>>>>>> 3b13cb3 (fixed merged solution issue)
            }
        }

        public void RecordBall() => Balls++;
        public void RecordFoul() => Fouls++;
        public void AwardRun() => Runs++;
        private void RecordOut()
        {
            Outs++;
            ResetCount();
        }
        private void ResetCount()
        {
            Balls = 0;
            Strikes = 0;
        }
        public void RecordStrike()
        {
            Strikes++;
            if (Strikes >= 3)
            {
                RecordOut();
            }
        }
        private void RecordFoulTipOut()
        {
            RecordOut();
        }
        private void RecordHitByPitch()
        {
            HitByPitches++;

            if (IsBasesLoaded())
            {
                AwardRun();
            }
        }
        private bool IsBasesLoaded()
            => BasesLoaded;
        private void RecordIntentionalBall()
            => RecordBall();
        private void RecordWalk()
<<<<<<< HEAD
<<<<<<< HEAD
            => BB++;
=======
            => Walks++;
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
=======
            => Walks++;
=======
            => BB++;
>>>>>>> 29e156e (fixed merged solution issue)
>>>>>>> 3b13cb3 (fixed merged solution issue)
        private void RecordCatcherInterference()
            => CatcherInterference++;
        private void RecordSwingAndMiss()
        {
            Strikes++;
            if (Strikes >= 3)
                RecordOut();
        }
        private void RecordIntentionalWalk()
            => ResetCount();
        private void RecordIllegalPitch()
            => RecordBall();

        public AtBatOutcome AnalyzeAtBat()
        {
            if (Balls >= 4)
            {
                Balls = 0;
<<<<<<< HEAD
<<<<<<< HEAD
                return AtBatOutcome.Ball;
            }
            else if (Strikes >= 3)
                return AtBatOutcome.FoulTipOut;
            else
            {
                ///////Still need to add Logic for fouls
                return AtBatOutcome.Single;
=======
=======
>>>>>>> 3b13cb3 (fixed merged solution issue)
                return AtBatOutcome.Walk;
            }
            else if (Strikes >= 3)
                return AtBatOutcome.Strikeout;
            else
            {
                ///////Still need to add Logic for fouls
                return AtBatOutcome.Hit;
<<<<<<< HEAD
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
=======
=======
                return AtBatOutcome.Ball;
            }
            else if (Strikes >= 3)
                return AtBatOutcome.FoulTipOut;
            else
            {
                ///////Still need to add Logic for fouls
                return AtBatOutcome.Single;
>>>>>>> 29e156e (fixed merged solution issue)
>>>>>>> 3b13cb3 (fixed merged solution issue)
            }
        }

        public void AnalyzePlayerActions(List<AtBatOutcome> playerActions)
        {
            foreach (var action in playerActions)
            {
                switch (action)
                {
                    case AtBatOutcome.Single:
                        RecordHit(action);
                        break;
                    case AtBatOutcome.Double:
                        RecordHit(action);
                        break;
                    case AtBatOutcome.Triple:
                        RecordHit(action);
                        break;
                    case AtBatOutcome.HomeRun:
                        RecordHit(action);
                        break;
                    case AtBatOutcome.Ball:
                        RecordBall();
                        break;
<<<<<<< HEAD
<<<<<<< HEAD
                    case AtBatOutcome.Strike:
=======
                    case AtBatOutcome.CalledStrike:
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
=======
                    case AtBatOutcome.CalledStrike:
=======
                    case AtBatOutcome.Strike:
>>>>>>> 29e156e (fixed merged solution issue)
>>>>>>> 3b13cb3 (fixed merged solution issue)
                        RecordStrike();
                        break;
                    case AtBatOutcome.FoulBall:
                        RecordFoul();
                        break;
<<<<<<< HEAD
<<<<<<< HEAD
=======
                    case AtBatOutcome.SwingAndMiss:
                        RecordSwingAndMiss();
                        break;
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
=======
                    case AtBatOutcome.SwingAndMiss:
                        RecordSwingAndMiss();
                        break;
=======
>>>>>>> 29e156e (fixed merged solution issue)
>>>>>>> 3b13cb3 (fixed merged solution issue)
                    case AtBatOutcome.FoulTipOut:
                        RecordFoulTipOut();
                        break;
                    case AtBatOutcome.HitByPitch:
                        RecordHitByPitch();
                        break;
<<<<<<< HEAD
<<<<<<< HEAD
=======
                    case AtBatOutcome.IntentionalBall:
                        RecordIntentionalBall();
                        break;
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
=======
                    case AtBatOutcome.IntentionalBall:
                        RecordIntentionalBall();
                        break;
=======
>>>>>>> 29e156e (fixed merged solution issue)
>>>>>>> 3b13cb3 (fixed merged solution issue)
                    case AtBatOutcome.IntentionalWalk:
                        RecordIntentionalWalk();
                        break;
                    case AtBatOutcome.CatcherInterference:
                        RecordCatcherInterference();
                        break;
<<<<<<< HEAD
<<<<<<< HEAD
=======
                    case AtBatOutcome.IllegalPitch:
                        RecordIllegalPitch();
                        break;
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
=======
                    case AtBatOutcome.IllegalPitch:
                        RecordIllegalPitch();
                        break;
=======
>>>>>>> 29e156e (fixed merged solution issue)
>>>>>>> 3b13cb3 (fixed merged solution issue)
                }
            }
        }

        public void AddAction(AtBatOutcome action)
            => AtBatActions.Add(action);

        public static List<PlayerScoreKeepingVM> GetPlayers(Player[] players)
        {
            List<PlayerScoreKeepingVM> displayPlayers = new();

            foreach (Player player in players)
            {
                string playerName = $"{player.PlyrFirstName} {player.PlyrLastName}";

                PlayerScoreKeepingVM playerActions = new(playerName, player.ID);
                displayPlayers.Add(playerActions);
            }

            return displayPlayers;
        }
    }
}