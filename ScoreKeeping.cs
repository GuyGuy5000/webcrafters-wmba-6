//Done By: Emmanuel James

using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
<<<<<<< HEAD

namespace wmbaApp.Models
=======
using wmbaApp.Models;

namespace wmbaApp.ViewModels
>>>>>>> dc85b26 (Add files via upload)
{
    public class ScoreKeeping
    {
        // Fields or Properties
        private int AtBats;
<<<<<<< HEAD
        private int[] Hits ;
=======
        private int[] Hits;
>>>>>>> dc85b26 (Add files via upload)
        private int Doubles;
        private int Triples;
        private int HomeRuns;
        private int PlateAppearances;
        private int Walks;
        private int HitByPitches;
        private int Sacrifices;
        private int CatcherInterference;
<<<<<<< HEAD

        // Construvtor that instantiates the fields.
        public ScoreKeeping(int atBats, int [] hits, int doubles, int triples, int homeRuns, int plateAppearances, 
            int walks, int hitsByPitches, int sacrifices, int catcherInterference)
        {
            this.AtBats = atBats;
            this.Hits = hits;
            this.Doubles = doubles;
            this.Triples = triples;
            this.HomeRuns = homeRuns;
            this.PlateAppearances = plateAppearances;
            this.Walks = walks;
            this.HitByPitches = hitsByPitches;
            this.Sacrifices = sacrifices;
            this.CatcherInterference = catcherInterference;
=======
        private int Balls;
        private int Strikes;
        private int Fouls;
        private int Runs;
        private int Outs;


        private bool FirstBase { get; set; }
        private bool SecondBase { get; set; }
        private bool ThirdBase { get; set; }
        private bool BasesLoaded => FirstBase && SecondBase && ThirdBase;

        // Constructor that instantiates the fields.
        public ScoreKeeping(int atBats, int[] hits, int doubles, int triples, int homeRuns, int plateAppearances,
            int walks, int hitsByPitches, int sacrifices, int catcherInterference, int ballsCount, int strikesCount,
            int foulsCount, int runsCount, int outsCount)
        {
            AtBats = atBats;
            Hits = hits;
            Doubles = doubles;
            Triples = triples;
            HomeRuns = homeRuns;
            PlateAppearances = plateAppearances;
            Walks = walks;
            HitByPitches = hitsByPitches;
            Sacrifices = sacrifices;
            CatcherInterference = catcherInterference;
            Runs = runsCount;
            Fouls = foulsCount;
            Balls = ballsCount;
            Outs = outsCount;
            Strikes = strikesCount;
>>>>>>> dc85b26 (Add files via upload)
        }

        // Properties to determine the type of hit. PS: Not so sure about this implementation, It can be modified.
        public int TotalSingles()
        {
            return Hits.Count(h => h == 1);
        }

        public int TotalDoubles()
        {
            return Hits.Count(h => h == 2);
        }

        public int TotalTriples()
        {
            return Hits.Count(h => h == 3);
        }

        // I don't know how we are gonna evaluate a Home-Run. This method evaluates a Hom-Run as four hits by the player.
        public int TotalHomeRuns()
        {
<<<<<<< HEAD
            return Hits.Count(h => h == 4); 
=======
            return Hits.Count(h => h == 4);
>>>>>>> dc85b26 (Add files via upload)
        }




        // This gets the total hits of a player
        public int TotalHits()
        {
            return Hits.Sum();
        }


        // Does the total AtBats
        public int PAtBats
        {
            get
            {
                AtBats = PlateAppearances - Walks - HitByPitches - Sacrifices - CatcherInterference;
                return AtBats;
            }
        }

        // Calculates OPS
<<<<<<< HEAD
        public double CalculateOBP() 
=======
        public double CalculateOBP()
>>>>>>> dc85b26 (Add files via upload)
        {
            double OPS = (double)(TotalSingles() + TotalDoubles() * 2 + TotalTriples() * 3 + TotalHomeRuns() * 4) / AtBats;
            return Math.Round(OPS, 3);
        }


        // Calculates OBP
        public double CalculateSLG()
        {
            double OBP = (double)(TotalHits() + Walks + HitByPitches) / (AtBats + Walks + HitByPitches + Sacrifices);
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

            double result = (double)TotalHits() / totalAtBats;
            return Math.Round(result, 3);
        }

<<<<<<< HEAD
=======
        // PLAYER ACTIONS






        public enum AtBatOutcome
        {
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
        }



        private void RecordHit()
        {
            if (!FirstBase)
            {
                FirstBase = true;
            }
            else if (!SecondBase)
            {
                SecondBase = true;
            }
            else if (!ThirdBase)
            {
                ThirdBase = true;
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
        {
            return BasesLoaded;
        }

        private void RecordIntentionalBall()
        {

            RecordBall();
        }

        private void RecordWalk()
        {
           
            Walks++;


        }

        private void RecordCatcherInterference()
        {
            CatcherInterference++;
        }

        private void RecordSwingAndMiss()
        {
            Strikes++;

            if (Strikes >= 3)
            {
                RecordOut();
            }
        }


        private void RecordIntentionalWalk()
        {
            ResetCount();
        }



        private void RecordIllegalPitch()
        {
            RecordBall();
        }

        public AtBatOutcome AnalyzeAtBat()
        {
            if (Balls >= 4)
            {
                return AtBatOutcome.Walk;
            }
            else if (Strikes >= 3)
            {
                return AtBatOutcome.Strikeout;
            }
            else
            {

///////Still need to add Logic for fouls
                return AtBatOutcome.Hit;
            }
        }

        public void AnalyzePlayerActions(List<AtBatOutcome> playerActions)
        {
            foreach (var action in playerActions)
            {
                switch (action)
                {
                    case AtBatOutcome.Hit:
                        RecordHit();
                        break;
                    case AtBatOutcome.Ball:
                        RecordBall();
                        break;
                    case AtBatOutcome.CalledStrike:
                        RecordStrike();
                        break;
                    case AtBatOutcome.FoulBall:
                        RecordFoul();
                        break;
                    case AtBatOutcome.SwingAndMiss:
                        RecordSwingAndMiss();
                        break;

                    case AtBatOutcome.FoulTipOut:
                        RecordFoulTipOut();
                        break;
                    case AtBatOutcome.HitByPitch:
                        RecordHitByPitch();
                        break;
                    case AtBatOutcome.IntentionalBall:
                        RecordIntentionalBall();
                        break;
                    case AtBatOutcome.IntentionalWalk:
                        RecordIntentionalWalk();
                        break;
                    case AtBatOutcome.CatcherInterference:
                        RecordCatcherInterference();
                        break;
                    case AtBatOutcome.IllegalPitch:
                        RecordIllegalPitch();
                        break;
                }
            }
        }
        public string Name { get; set; }
        public List<AtBatOutcome> AtBatActions { get; set; } = new List<AtBatOutcome>();


        public ScoreKeeping(string name)
        {
            Name = name;
            AtBatActions = new List<AtBatOutcome>();

        }

        public void AddAction(AtBatOutcome action)
        {
            AtBatActions.Add(action);

        }


        public static List<ScoreKeeping> GetPlayers(Player[] players)
        {
            List<ScoreKeeping> displayPlayers = new List<ScoreKeeping>();

            foreach (Player player in players)
            {

                string playerName = $"{player.PlyrFirstName} {player.PlyrLastName}";


                ScoreKeeping playerActions = new ScoreKeeping(playerName);


                displayPlayers.Add(playerActions);
            }

            return displayPlayers;
        }
>>>>>>> dc85b26 (Add files via upload)

    }
}
